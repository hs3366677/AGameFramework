using System;
using ProtoBuf;

namespace Protocols
{


    //CS间通讯用协议头
    public class ProtoHead //: IProtoHead
    {
        //协议ID,实际序列化结构使用UInt16
        private ProtoID cmdID;
        public ProtoID CmdID
        {
            get { return cmdID; }
            set { cmdID = value; }
        }

        //获取协议对应模块ID
        public System.UInt16 GetModuleID()
        {
            return (System.UInt16)((cmdID & 0xff00) >> 8);
        }

        private UInt32 bodyLen;
        public System.UInt32 BodyLen
        {
            get { return bodyLen; }
            set { bodyLen = value; }
        }
        //序列化所需要的长度
        internal static int GetSerializeLength()
        {
            return sizeof(UInt16) + sizeof(UInt32);
        }

        internal int GetSize()
        {
            return GetSerializeLength();
        }

        internal int Serialize(System.IO.Stream stream)
        {
            stream.Write(BitConverter.GetBytes(cmdID.ToUInt16()), 0, sizeof(UInt16));
            stream.Write(BitConverter.GetBytes(bodyLen), 0, sizeof(UInt32));

            return 0;
        }

        internal int Deserialize(System.IO.Stream stream)
        {
            if ((stream.Length - stream.Position) < GetSize())
            {
                return -1;
            }

            byte[] buffer1 = new byte[2];

            //cmdId
            stream.Read(buffer1, 0, sizeof(UInt16));
            cmdID = BitConverter.ToUInt16(buffer1, 0);

            byte[] buffer2 = new byte[4];

            //bodyLen
            stream.Read(buffer2, 0, sizeof(UInt32));
            bodyLen = BitConverter.ToUInt32(buffer2, 0);

            return 0;
        }
    }



}
