/********************************************************************
	created:	2014/11/27
	created:	27:11:2014   10:16
	filename: 	\CommonPlatform\Server\Framework\Utility\Protocol\Proto\Protocol.cs
	file path:	\CommonPlatform\Server\Framework\Utility\Protocol\Proto
	file base:	Protocol
	file ext:	cs
	author:		史耀力
	
	purpose:	消息结构定义.
    usage:
            所有的消息都由ProtoHead与ProtoBody组成.
            对于Server间通讯的消息,在ProtoHead外还添加了一个ServerHead头.
            [除ServerHead头外,消息可以继续包裹不同的扩展协议头(IProtoHeadExtend)]
 
            ProtoBody使用ProtoBuf-Net进行编码,用户需要自己继承ProtoBody,并按照ProtoBuf-Net要求设置特性
 
            范例如下:
            [ProtoContract]
            public class UserLoginReq : ProtoBody
            {
                [ProtoMember(1)]
                public string userName { get; set; }

                [ProtoMember(2)]
                public string passWord { get; set; }

                [ProtoMember(3)]
                public int platformType { get; set; }
            }
 
*********************************************************************/
using System;
using System.IO;
using ProtoBuf;

namespace Protocols
{
    //完整协议体
    public class Protocol
    {
        public ProtoID GetID()
        {
            return protoHead.CmdID;
        }

        public UInt32 GetSource()
        {
            return serverHead.Source;
        }

        public void SetSource(UInt32 source)
        {
            serverHead.Source = source;
        }

        public UInt32 GetDest()
        {
            return serverHead.Target;
        }

        public void SetDest(UInt32 dest)
        {
            serverHead.Target = dest;
        }

        public UInt64 GetRoleID()
        {
            return serverHead.roleID;
        }

        public void SetRoleID(UInt64 roleID)
        {
            serverHead.roleID = roleID;
        }

        public UInt32 GetSessionID()
        {
            return serverHead.sessionID;
        }

        public void SetSessionID(UInt32 sessionID)
        {
            serverHead.sessionID = sessionID;
        }

        public ProtoBody GetBody()
        {
            return protoBody;
        }

        public void SetBody(ProtoBody body)
        {
            protoBody = body;
            protoHead.CmdID = ProtoID.GetProtoIDByBody(body);
        }

        public UInt32 GetBodySize()
        {
            return protoHead.BodyLen;
        }

        //服务器间通讯用头部,以后可能会有复数个
        private ServerHead serverHead;

        private ProtoHead protoHead;

        private ProtoBody protoBody;

        /// <summary>
        /// 仅生成ProtoHead
        /// </summary>
        public Protocol()
        {
            protoHead = new ProtoHead();
            serverHead = new ServerHead();

            protoBody = null;
        }

        public Protocol(ProtoID id, ProtoBody body)
        {
            protoHead = new ProtoHead();
            protoHead.CmdID = id;
            serverHead = new ServerHead();
            protoBody = body;
        }

        /// <summary>
        /// 生成ProtoHead,ServerHead,ProtoID,并使用传入的ProtoBody赋值
        /// </summary>
        /// <param name="bodyIn"></param>
        public Protocol(ProtoBody bodyIn)
        {
            protoHead = new ProtoHead();
            serverHead = new ServerHead();
            protoBody = bodyIn;

            protoHead.CmdID = ProtoID.GetProtoIDByBody(bodyIn);
        }

        public int Serialize(System.IO.Stream stream)
        {
            long basePosition = stream.Position;
            long protoHeadPosition = stream.Position;
            protoHead.Serialize(stream);
            long headTotalLen = stream.Position - basePosition;

            //serialize body
            protoBody.Serialize(stream);

            //计算包体大小并调整
            protoHead.BodyLen = (UInt32)(stream.Length - headTotalLen);
            stream.Position = protoHeadPosition;
            protoHead.Serialize(stream);
            stream.Position = stream.Length;

            return (int)stream.Length;
        }

        //[12/3/2014 史耀力] TODO:此处性能需要优化! 
        public int Deserialize(System.IO.Stream stream)
        {
            //LogSys.Warn("[PROTO] Begin Deser len {0}, position {1}", stream.Length, stream.Position);

            long initPos = stream.Position;
            if (protoHead.Deserialize(stream) < 0)
            {
                stream.Position = initPos;
                return -1;
            }

            Stream bodyStream = stream;
            //长度过长,需要分包
            if ((stream.Length - stream.Position) > protoHead.BodyLen)
            {
                bodyStream = new MemoryStream();
                if (0 != protoHead.BodyLen)
                {
                    long curPos = stream.Position;

                    //stream.CopyTo(bodyStream);
					int len = (int)stream.Length;
					byte[] buffer = new byte[len];
					stream.Read(buffer, 0, len);
					bodyStream.Write(buffer, 0, len);

                    //修正位置
                    stream.Position = curPos + protoHead.BodyLen;
                    bodyStream.SetLength(protoHead.BodyLen);
                    bodyStream.Position = 0;
                }
            }
            //长度不够
            else if ((stream.Length - stream.Position) < protoHead.BodyLen)
            {
                stream.Position = initPos;
                return -1;
            }

            if (protoHead.CmdID.BodyType == null) //typeof(LuaProtoBody))
            {
                protoBody = new LuaProtoBody();

                byte[] byteArray = new byte[bodyStream.Length - bodyStream.Position];
                bodyStream.Read(byteArray, 0, byteArray.Length);
                //转成bytearray
                //((LuaProtoBody)protoBody).byteArray = ((MemoryStream)bodyStream).ToArray();
                ((LuaProtoBody)protoBody).byteArray = byteArray;
            }
            else 
                protoBody = ProtoBody.Deserialize(bodyStream, protoHead.CmdID.BodyType);

            //if (!isClient)
            //{
            //    protoBody.BodyRoleID = serverHead.roleID;
            //}

            //LogSys.Warn("[PROTO] End Deser len {0}, position {1}", stream.Length, stream.Position);

            return 0;
        }

        //重载ToString方法,便于输出
        public override string ToString()
        {
            return GetID().ToString();
        }
    }

    //基础头部
    //internal class IProtoHead
    //{
    //    int GetSize();

    //    //序列化
    //    int Serialize(System.IO.Stream stream);

    //    //反序列化
    //    int Deserialize(System.IO.Stream stream);
    //}

    //Server间可扩展的头部
    //internal class IProtoHeadExtend : IProtoHead
    //{
    //    //魔数,用于区分不同的扩展包头
    //    byte GetHeadMagicNum();

    //    //协议头名称
    //    string GetHeadName();
    //}

    //调试用的协议追踪头
    public struct TraceHead // : IProtoHeadExtend
    {

    }

    //Pipeline头
    public class PipelineHead // : IProtoHeadExtend
    {

    }
}
