using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Protocols
{
    /// <summary>
    /// 协议工具类
    /// </summary>
    public class ProtoUtility
    {
        public static byte[] Encode(Dictionary<string, object> datas)
        {
            byte[] buffer = null;
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                binFormat.Serialize(stream, datas.Count);
                foreach (var pair in datas)
                {
                    binFormat.Serialize(stream, pair.Key);
                    binFormat.Serialize(stream, pair.Value);
                }
                stream.Position = 0;
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }
        public static Dictionary<string, object> Decode(byte[] buffer)
        {
            Dictionary<string, object> datas = new Dictionary<string, object>();
            BinaryFormatter binFormat = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                int count = (int)binFormat.Deserialize(stream);
                for(int i = 0; i < count; ++i)
                {
                    string key = (string)binFormat.Deserialize(stream);
                    object value = binFormat.Deserialize(stream);

                    datas.Add(key, value);
                }
            }
            return datas;
        }
    }
}
