using System;
using ProtoBuf;

namespace Protocols
{
    public class ProtoHelper
    {
        public static int GetNormalProtoHeadLen()
        {
            return ServerHead.GetSerializeLength() + ProtoHead.GetSerializeLength();
        }

        public static ProtoID GetProtoIDFromBytes(byte[] data, int dataLen)
        {
            if (dataLen < (GetNormalProtoHeadLen()))
            {
                return null;
            }

            return BitConverter.ToUInt16(data, ServerHead.GetSerializeLength());
        }

        //断包器,先使用New实现.性能优化时可以考虑使用static或者ObjPool
        public static Protocol ProtocolSeg(System.IO.Stream stream)
        {
            Protocol protocol = new Protocol();

            if(protocol.Deserialize(stream) < 0)
            {
                return null;
            }


            return protocol;
        }

        //public static Protocol GetProtocolFromBytes(byte[] data, int dataLen)
        //{
        //    if (dataLen < (GetNormalProtoHeadLen()))
        //    {
        //        return null;
        //    }

        //    ProtoID protoID = BitConverter.ToUInt16(data, ServerHead.GetSerializeLength());

        //    Protocol protocol = new Protocol();
        //    protocol.protoID = protoID;
        //    //protocol.protoBody = ProtoBuf.Serializer.NonGeneric.Deserialize(protoID.BodyType,data);

        //    return protocol;
        //}

        //public static int WriteProtoHead(IProtoHead protoHead, Stream stream)
        //{
        //    protoHead.Serialize(stream);

        //    return protoHead.GetSize();
        //}
    }
}
