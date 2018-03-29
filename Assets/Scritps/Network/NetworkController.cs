using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using LuaFramework;

namespace Protocols
{

    public static class Debug
    {
        public static IProtoLogger CustomLogger
        {
            get;
            set;
        }
        public static void Log(string log)
        {
            CustomLogger.Log(ProtoLoggerLevel.Debug, "[" + typeof(Debug).Namespace + "]" + log);
        }

        public static void Log(string format, params object[] parameters)
        {
            CustomLogger.Log(ProtoLoggerLevel.Debug, "[" + typeof(Debug).Namespace + "]" + format, parameters);
        }

        public static void LogWarning(string format, params object[] parameters)
        {
            CustomLogger.Log(ProtoLoggerLevel.Warn, "[" + typeof(Debug).Namespace + "]" + format, parameters);
        }

        public static void LogError(string log)
        {
            CustomLogger.Log(ProtoLoggerLevel.Error, "[" + typeof(Debug).Namespace + "]" + log);
        }

        public static void LogError(string format, params object[] parameters)
        {
            CustomLogger.Log(ProtoLoggerLevel.Error, "[" + typeof(Debug).Namespace + "]" + format, parameters);
        }

        public static void LogException(Exception e)
        {
            CustomLogger.LogException(e);
        }
    }

    public class NetworkController
    {
        static readonly NetworkController _instance = new NetworkController();

        Dictionary<int, Session> allSessions;
        public static NetworkController Instance
        {
            get
            {
                return _instance;
            }
        }

        NetworkController()
        {
            allSessions = new Dictionary<int, Session>();
        }

        public void AssignLogger(IProtoLogger logger)
        {
            Debug.CustomLogger = logger;
        }


        public Protocol GetMessage(int sessionId)
        {
            Protocol proto = allSessions[sessionId].GetMessage();

            if (proto != null)
            {
                if (proto.GetID() == ProtoIDReserve.CMD_SERVER_CONNECTED)
                {
                    ServerConnected body = (ServerConnected)proto.GetBody();
                    if (body.success)
                    {
                        Debug.Log("[{0}]CONNECTED", sessionId);
                    }
                    else
                    {
                        Debug.Log("[{0}]CONNECT FAILED", sessionId);
                    }
                }
                else if (proto.GetID() == ProtoIDReserve.CMD_SERVER_DISCONNECTED)
                {
                    Debug.Log("[{0}]DISCONNECTED", sessionId);
                }
                else
                {
                    Debug.Log("[{0}][RECV]{1}({2} bytes) {3}", sessionId, proto.GetID().ToString(), proto.GetBodySize().ToString(), DateTime.Now.ToString("HH:mm:ss.fff"));
                }
            }else 
                Debug.Log("[{0}]No Message", sessionId);

            return proto;
        }

        int sessionCount = 0;

        public int InitSession(string ip, int port, ProtocolType socketType = ProtocolType.Tcp, bool isClient = true)
        {
            ProtoBody.RegisterSerializeFunc(SerializeProto, DeserializeProto);

            string[] ips = ip.Split('.');
            byte[] ipAddr = { byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]), byte.Parse(ips[3]) };
            IPAddress addr = new IPAddress(ipAddr);
            IPEndPoint endpoint = new IPEndPoint(addr, port);

            Session session;

            if (isClient)
            {
                session = new ClientSession();
                sessionCount++;
                session.SessionId = sessionCount;
                session.Init(socketType);
                ((ClientSession)session).Connect(endpoint);
            }
            else
            {
                session = new ServerSession();
                sessionCount++;
                session.SessionId = sessionCount;
                session.Init(socketType);
                ((ServerSession)session).Listen(endpoint);
            }

            allSessions.Add(sessionCount, session);

            return sessionCount;
        }

        public void Disconnect(int id)
        {
            if (!allSessions.ContainsKey(id))
            {
                Debug.LogError("[{0}]Cannot disconnect session with id ", id);
                return;
            }

            Session session = allSessions[id];
            if (session != null && session.IsConnected)
                session.Destroy();
        }

        public void SendMessage(int sessionId, Protocol proto)
        {

            Debug.Log(string.Format("[{0}][SEND] {1} {2}", sessionId, proto.GetID(), DateTime.Now.ToString("HH:mm:ss.fff")));

            allSessions[sessionId].SendMessage(proto);
        }
        public void SendMessage(int sessionId, ProtoBody body)
        {
            Protocol proto = new Protocol(body);
            SendMessage(sessionId, proto);
        }
        public void LuaSendMessage(int sessionId, UInt16 id, ByteBuffer buffer)
        {
            SendMessage(sessionId, id, buffer.ToBytes());

            Debug.Log("send lua byte array ---------: " + BitConverter.ToString(buffer.ToBytes()));
        }

        /// <summary>
        /// 写数据
        /// </summary>
        void WriteMessage(int sessionId, byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                
               byte[] payload = ms.ToArray();
               SendMessage(sessionId, 65281, payload);
            }
        }

        public void SendMessage(int sessionId, UInt16 id, byte[] byteArray)
        {
            MemoryStream protoStream = new MemoryStream();
            UInt32 bodyLen = (UInt32)byteArray.Length;
            protoStream.Position = 0;
            protoStream.Write(BitConverter.GetBytes(id), 0, sizeof(UInt16));
            protoStream.Write(BitConverter.GetBytes(bodyLen), 0, sizeof(UInt32));
            protoStream.Write(byteArray, 0, byteArray.Length);
            SendMessage(sessionId, protoStream);

        }
        /// <summary>
        /// 接收到消息
        /// </summary>
        void OnReceive(byte[] bytes, int length)
        {
            
            MemoryStream memStream = new MemoryStream();
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(bytes, 0, length);
            //Reset to beginning
            memStream.Seek(0, SeekOrigin.Begin);

            BinaryReader reader = new BinaryReader(memStream);
            while (memStream.Length - memStream.Position > 2)
            {
                ushort messageLen = reader.ReadUInt16();
                if (memStream.Length - memStream.Position >= messageLen)
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write(reader.ReadBytes(messageLen));
                    ms.Seek(0, SeekOrigin.Begin);
                    OnReceivedMessage(ms);
                }
                else
                {

                }
            }

        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="ms"></param>
        void OnReceivedMessage(MemoryStream ms)
        {
            BinaryReader r = new BinaryReader(ms);
            byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));

            ByteBuffer buffer = new ByteBuffer(message);
            int mainId = buffer.ReadShort();

            Util.CallMethod("Network", "OnSocket", 65281, buffer);
        }


        // 将二进制序列化为ProtoBody类的方法  
        // < param name="model">要序列化的对象< /param>  
        void Deserialize(byte[] bytes)
        {
            ByteBuffer buffer = new ByteBuffer(bytes);
            int mainid = buffer.ReadShort();               //类型id
            byte[] byteArray = buffer.ReadBytes();         //消息内容

            MemoryStream memStream = new MemoryStream();
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(byteArray, 0, byteArray.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            TestProtoBody pbody = ProtoBuf.Serializer.Deserialize<TestProtoBody>(memStream);
            if (pbody != null)
            {
                //Debug.Log("成功了--------" + pbody.id + pbody.name);
            }
        }

        public void SendMessage(int sessionId, Stream protoStream)
        {
            allSessions[sessionId].SendMessage(protoStream);
        }

        void SerializeProto(Stream stream, ProtoBody body)
        {
            ProtoBuf.Serializer.NonGeneric.Serialize(stream, body);
        }

        ProtoBody DeserializeProto(Stream stream, Type bodyType)
        {
            return ProtoBuf.Serializer.NonGeneric.Deserialize(bodyType, stream) as ProtoBody;
        }
    }
}