/********************************************************************
	created:	2015/01/08
	created:	8:1:2015   17:47
	filename: 	D:\百度云\CommonPlatform\Server\Framework\Utility\Protocols\Proto\ProtoBody.cs
	file path:	D:\百度云\CommonPlatform\Server\Framework\Utility\Protocols\Proto
	file base:	ProtoBody
	file ext:	cs
	author:		史耀力
	
	purpose:	
*********************************************************************/

using System;
using ProtoBuf;
using System.IO;

namespace Protocols
{
    //协议体基类
    [ProtoContract]
    public class ProtoBody
    {
        public delegate ProtoBody DeserializeFunc(Stream stream, Type bodyType);
        static Action<Stream, ProtoBody> _serializeFunc;
        static DeserializeFunc _deserializeFunc;

        public static void RegisterSerializeFunc(Action<Stream, ProtoBody> serializeFunc, DeserializeFunc deserializeFunc)
        {
            _serializeFunc = serializeFunc;
            _deserializeFunc = deserializeFunc;
        }

        internal int Serialize(Stream stream)
        {
//#if USE_DLL
//            ProtoSerializer serializer = new ProtoSerializer();
//            serializer.Serialize(stream, this);
//#else
//            ProtoBuf.Serializer.NonGeneric.Serialize(stream, this);
//#endif
            _serializeFunc(stream, this);
            return 0;
        }

        internal static ProtoBody Deserialize(Stream stream, Type bodyType)
        {
//#if USE_DLL
//            ProtoSerializer serializer = new ProtoSerializer();
//            return serializer.Deserialize(stream, null, bodyType) as ProtoBody;
//#else
//            return Serializer.NonGeneric.Deserialize(bodyType, stream) as ProtoBody;
//#endif
            return _deserializeFunc(stream, bodyType);
        }
    }

    /// <summary>
    /// 热更新的协议体；传到Lua里面解析
    /// </summary>
    public class LuaProtoBody : ProtoBody
    {
        public byte[] byteArray;
    }
}
