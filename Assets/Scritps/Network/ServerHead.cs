/********************************************************************
	created:	2015/01/08
	created:	8:1:2015   17:47
	filename: 	D:\百度云\CommonPlatform\Server\Framework\Utility\Protocols\Proto\ServerHead.cs
	file path:	D:\百度云\CommonPlatform\Server\Framework\Utility\Protocols\Proto
	file base:	ServerHead
	file ext:	cs
	author:		史耀力
	
	purpose:	
*********************************************************************/

using System;

namespace Protocols
{
    //服务器间通讯用协议头
    public class ServerHead// : IProtoHeadExtend
    {
        //魔数
        private byte magicNum;
        internal byte MagicNum
        {
            get { return magicNum; }
            set { magicNum = value; }
        }

        //消息发送源
        private System.UInt32 source;
        public System.UInt32 Source
        {
            get { return source; }
            set { source = value; }
        }

        //消息发送目标
        private System.UInt32 target;
        public System.UInt32 Target
        {
            get { return target; }
            set { target = value; }
        }

        //角色ID
        public System.UInt64 roleID;

        /// <summary>
        /// 用户在GateSvr上的物理连接ID,用于尚未选择角色前的用户识别
        /// </summary>
        public UInt32 sessionID;

        //序列化所需要的长度
        internal static int GetSerializeLength()
        {
            return sizeof(byte) + 3 * sizeof(UInt32) + sizeof(UInt64);
        }

        internal int GetSize()
        {
            return GetSerializeLength();
        }

        internal int Serialize(System.IO.Stream stream)
        {
            stream.WriteByte(GetHeadMagicNum());
            stream.Write(BitConverter.GetBytes(source), 0, sizeof(UInt32));
            stream.Write(BitConverter.GetBytes(target), 0, sizeof(UInt32));
            stream.Write(BitConverter.GetBytes(roleID), 0, sizeof(UInt64));
            stream.Write(BitConverter.GetBytes(sessionID), 0, sizeof(UInt32));

            return 0;
        }

        internal int Deserialize(System.IO.Stream stream)
        {
            if ((stream.Length - stream.Position) < GetSize())
            {
                return -1;
            }

            byte[] buffer = new byte[8];

            //magic number
            magicNum = (byte)stream.ReadByte();
            //assert(magicNum == GetHeadMagicNum());

            //source
            stream.Read(buffer, 0, sizeof(UInt32));
            source = BitConverter.ToUInt32(buffer, 0);

            //target
            stream.Read(buffer, 0, sizeof(UInt32));
            target = BitConverter.ToUInt32(buffer, 0);

            //userid
            stream.Read(buffer, 0, sizeof(UInt64));
            roleID = BitConverter.ToUInt64(buffer, 0);

            //sessionid
            stream.Read(buffer, 0, sizeof(UInt32));
            sessionID = BitConverter.ToUInt32(buffer, 0);

            return 0;
        }

        internal byte GetHeadMagicNum()
        {
            return 0x1;
        }

        //协议头名称
        internal string GetHeadName()
        {
            return "ServerHead";
        }

        public static readonly ServerHead NULL = new ServerHead();
    }
}
