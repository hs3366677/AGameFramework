/* ============================================================
* Author:       xieqin
* Time:         2014/12/8 12:03:32
* FileName:     Session
* Purpose:      连接会话
* =============================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using Protocols;

namespace Protocols
{
    internal class ClientSession : Session{

        public override void Init(ProtocolType socketType)
        {
            m_protoType = socketType;
            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, socketType);
            clientSock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            bufferStream = new MemoryStream();
            protoList = new LinkedList<Protocol>();
        }

        public override void Destroy()
        {
            if (clientSock != null)
            {
                clientSock.Close();
                clientSock = null;
            }
        }

        public void Connect(IPEndPoint endpoint)
        {
            m_endPoint = endpoint;

            try
            {
                if (IsConnected)
                {
                    RestartSocket();
                }

                object[] param = { clientSock, endpoint };
                Debug.Log("[{0}]Start connecting to server {1}", SessionId, endpoint.ToString());
                clientSock.BeginConnect(endpoint, new AsyncCallback(HandleConnect), param);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void HandleConnect(IAsyncResult result)
        {
            Debug.Log("[{0}]Connected to server {1}", SessionId, m_endPoint.ToString());
            object[] param = (object[])result.AsyncState;
            Socket sock = (Socket)param[0];
            //IPEndPoint endpoint = (IPEndPoint)param[1];

            ServerConnected body = new ServerConnected();
            body.success = false;

            try
            {
                sock.EndConnect(result);

                if (sock.Connected)
                {
                    body.endpoint = (IPEndPoint)sock.RemoteEndPoint;
                    body.success = true;
                    BeginReceive();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                RestartSocket();
            }
            finally
            {
                connecting = false;

                Protocol proto = new Protocol(body);
                PostProto(proto);
            }
        }
    }

    /// <summary>
    /// 只管理一个连接的ServerSession；收到第一个客户端连接后不再监听其他连接
    /// </summary>
    internal class ServerSession : Session{

        Socket serverSock;
        Thread serverThread;
        Thread clientThread;

        public override void Init(ProtocolType socketType)
        {
            m_protoType = socketType;
            serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, m_protoType);
            serverSock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            bufferStream = new MemoryStream();
            protoList = new LinkedList<Protocol>();
        }

        public override void Destroy()
        {
            if (serverSock != null)
            {
                serverSock.Close();
                serverSock = null;
            }

            if (clientSock != null)
            {
                clientSock.Close();
                clientSock = null;
            }
        }

        public void Listen(IPEndPoint endPoint)
        {
            serverThread = new Thread(new ParameterizedThreadStart(ListenStart));
            serverThread.Start(endPoint);
        }
        /// <summary>
        /// block method; run in a seperate thread
        /// </summary>
        void ListenStart(object endPoint)
        {
            IPEndPoint localEndPoint = (IPEndPoint)endPoint;
            serverSock.Bind(localEndPoint);
            serverSock.Listen(BACKLOG);

            Debug.Log("[{0}]Listening ...{1}", SessionId, localEndPoint);

            clientSock = serverSock.Accept();   //block method

            object[] param = { clientSock, localEndPoint };

            Debug.Log("[{0}]Incoming client {1}", SessionId, clientSock.RemoteEndPoint);
            
            BeginReceive();
        }
    }

    //连接会话
    internal class Session
    {
        public int SessionId
        {
            get;
            set;
        }

        protected const int BUFFER_LEN = 65536;
        protected const int STREAM_LEN_MAX = 65536;

        protected const int BACKLOG = 10; // maximum length of pending connections queue

        protected ProtocolType m_protoType;
        protected IPEndPoint m_endPoint;

        protected Socket clientSock;

        //消息缓冲
        byte[] buffer = new byte[BUFFER_LEN];
        protected MemoryStream bufferStream;
        protected LinkedList<Protocol> protoList;
        object thisLock = new object();
        protected bool connecting;

        protected int bytesRead;
        protected int bytesSend;
        
        public bool IsConnected
        {
            get
            {
                if (clientSock != null && clientSock.Connected)
                {
                    return true;
                }
                return false;
            }
        }

        protected Session()
        {
            connecting = false;
            clientSock = null;
        }

        public virtual void Init(ProtocolType socketType){}
        public virtual void Destroy(){}

        /// <summary>
        /// 主动断开连接
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                Destroy();
                PostDisconnectProto(DisconnReason.Active);
            }
        }

		/// <summary>
		/// 清空Session中缓存的Proto列表
		/// </summary>
	    public void ClearProtoList()
		{
			lock (thisLock)
			{
				protoList.Clear();
			}
		}

	    //发送协议
        public int SendMessage(ProtoBody body)
        {
            Protocol proto = new Protocol(body);
            return SendMessage(proto);
        }

        public int SendMessage(Protocol proto)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                proto.Serialize(stream);
                return SendMessage(stream);
            }
        }

        public Protocol GetMessage()
        {
            Protocol proto = null;
            lock (thisLock)
            {
                if (protoList.Count > 0)
                {
                    proto = protoList.First.Value;
                    protoList.RemoveFirst();
                }
            }
            return proto;
        }

        protected void RestartSocket()
        {
            Debug.Log("[{0}]Restart Session", SessionId);
            try
            {
                clientSock.Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, m_protoType);
                clientSock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            }
        }

        void PostDisconnectProto(DisconnReason reason)
        {
            ServerDisconnected body = new ServerDisconnected();
            body.reason = reason;
            Protocol proto = new Protocol(body);
            PostProto(proto);
        }

        protected void PostProto(Protocol proto)
        {
            lock (thisLock)
            {
                protoList.AddLast(proto);
            }
        }

        protected void BeginReceive()
        {
            Debug.Log("[{0}]BeginReceive", SessionId);
            clientSock.BeginReceive(buffer, 0, BUFFER_LEN, 0, new AsyncCallback(HandleReceive), null);
        }

        public int SendMessage(Stream stream)
        {
            try
            {
				if (!IsConnected)
				{
					Debug.Log("[{0}]SendMessage fail while disconnected", SessionId);
					return 1;
				}

                byte[] buf = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(buf, 0, (int)stream.Length);

                //object[] buffParam = new object[] { buf, buf.Length };
                clientSock.BeginSend(buf, 0, buf.Length, 0, new AsyncCallback(HandleSend), null);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                RestartSocket();
                PostDisconnectProto(DisconnReason.Exception);
            }

            return 0;
        }

        void HandleReceive(IAsyncResult result)
        {
            try
            {
                if (!IsConnected)
                {
                    Debug.Log("[{0}]HandleReceive fail while disconnected", SessionId);
                    return;
                }

                bytesRead = clientSock.EndReceive(result);

                if (bytesRead > 0)
                {
                    long pos = bufferStream.Position;
                    bufferStream.Position = bufferStream.Length;
                    bufferStream.Write(buffer, 0, bytesRead);
                    bufferStream.Position = pos;

                    while (true)
                    {
                        try
                        {
                            Protocol proto = ProtoHelper.ProtocolSeg(bufferStream);
                            
                            if (proto == null)
                                break;

                            PostProto(proto);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }

                    if (bufferStream.Length == bufferStream.Position)
                    {
                        Debug.Log("[{0}]BufferStream reset ", SessionId);

                        bufferStream.Position = 0;
                        bufferStream.SetLength(0);
                    }
                    else if (bufferStream.Position >= STREAM_LEN_MAX)
                    {
                        Debug.Log("[{0}]BufferStream overflow ", SessionId);

                        int len = (int)(bufferStream.Length - bufferStream.Position);
                        byte[] temp = new byte[len];
                        bufferStream.Read(temp, 0, len);
                        bufferStream.SetLength(0);
                        bufferStream.Write(temp, 0, len);
                        bufferStream.Position = 0;
                    }

                    BeginReceive();
                }
                else
                {
                    Debug.LogError("[{0}]Read 0 bytes", SessionId);
                    RestartSocket();
                    PostDisconnectProto(DisconnReason.Passive);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                RestartSocket();
                PostDisconnectProto(DisconnReason.Exception);
            }
        }

        void HandleSend(IAsyncResult result)
        {
            Debug.Log("[{0}]HandleSend ", SessionId);
            try
            {
                int byteSent = clientSock.EndSend(result);
                Debug.Log("[{0}]Bytes Sent {1}", SessionId, byteSent);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                RestartSocket();
                PostDisconnectProto(DisconnReason.Exception);
            }
        }

    }
}
