/********************************************************************
	created:	2014/11/27
	created:	27:11:2014   9:53
	filename: 	\CommonPlatform\Server\Framework\Utility\Protocol\Proto\ProtoID.cs
	file path:	\CommonPlatform\Server\Framework\Utility\Protocol\Proto
	file base:	ProtoID
	file ext:	cs
	author:		史耀力
	
	purpose:	实现协议号机制.允许用户自己扩展协议空间
    usage:
 
    //协议扩展范例(以登录系统为例)
    namespace Utility.Protocol
    {
        public partial class ProtoID
        {
            //协议定义范例
            //-使用ProtoBody声明协议体
            [ProtoBody(typeof(UserLoginReq))]
            //-使用静态变量声明协议ID
            public static ProtoID CMD_USER_LOGIN_REQ = new ProtoID(0x0001, "CMD_USER_LOGIN_REQ");
 
            //-使用ProtoBody声明协议体
            [ProtoBody(typeof(UserLoginRsp))]
            //-使用静态变量声明协议ID
            public static ProtoID CMD_USER_LOGIN_RSP = new ProtoID(0x0002, "CMD_USER_LOGIN_RSP");
        }
    }
 
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Reflection;

namespace Protocols
{
    //使用枚举类来实现协议号,便于使用者扩展
    public partial class ProtoID
    {
        const string unknownIDTag = "UNKNOWNID";

        /// <summary>
        /// [2018.03.13 黄帅]新增构造方法
        /// </summary>
        /// <param name="protoIDIn"></param>
        protected ProtoID(UInt16 protoIDIn)
        {
            id = protoIDIn;
            name = unknownIDTag;
            client = true;
            visible = true;
        }
        protected ProtoID(UInt16 protoIDIn, Type bodyTypeIn, string nameIn, bool clientIn, bool visibleIn)
        {
            id = protoIDIn;
            name = nameIn;
            bodyType = bodyTypeIn;
            client = clientIn;
            visible = visibleIn;

            RegisterToBase(this);
        }

        /// <summary>
        /// 16位协议号,高8位标识模块ID,低8位为模块内协议编号
        /// </summary>
        private UInt16 id;
        public System.UInt16 ID
        {
            get { return id; }
        }

        public byte GetModuleID()
        {
            return (byte)(id >> 8);
        }

        /// <summary>
        ///协议名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        ///包体类型,在BuildProtoIDMap时赋值
        /// </summary>
        private Type bodyType;

        public System.Type BodyType
        {
            get { return bodyType; }
        }

        /// <summary>
        ///重载ToString方法,便于输出
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        ///重载uint16
        /// </summary>
        /// <returns></returns>
        public UInt16 ToUInt16()
        {
            return id;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ProtoID))
            {
                return ID == ((ProtoID)obj).ID;
            }
            else 
                return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }    


        public static implicit operator UInt16(ProtoID protoID)
        {
            return protoID.ToUInt16();
        }

        /// <summary>
        /// 是否允许客户端发送此协议
        /// </summary>
        private bool client;

#pragma warning disable 0414
        /// <summary>
        /// 客户端是否可见
        /// </summary>
        private bool visible;
#pragma warning restore 0414

        //////////////////////////////////////////////////////////////////////////
        // ProtoID static methods

        /// <summary>
        /// 为获得更好的运行时效率,此处对所有协议进行预处理 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected static int RegisterToBase(ProtoID id)
        {
            _DelayMemberInit();

            if (null != id && !idMap.ContainsKey(id))
            {
                idMap.Add(id, id);
            }
            else if (null != id)
            {
            }

            if (null != id && !typeMap.ContainsKey(id.BodyType))
            {
                typeMap.Add(id.BodyType, id);
                //LogSys.Debug(string.Format("[PROTO] Build  Mapping for {0} - {1}", id.ToUInt16(), id));
            }
            else if (null != id)
            {
            }

            return 0;
        }

        /// <summary>
        /// 延迟初始化,避免静态变量初始化顺序问题.  
        /// </summary>
        /// <returns></returns>
        private static int _DelayMemberInit()
        {
            if (null == idMap)
            {
                idMap = new Dictionary<UInt16, ProtoID>();
            }

            if (null == typeMap)
            {
                typeMap = new Dictionary<Type, ProtoID>();
            }

            return 0;
        }

        /// <summary>
        /// 16位数字id转换为协议ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static implicit operator ProtoID(UInt16 id)
        {
            ProtoID pID;

            if (idMap.TryGetValue(id, out pID))
            {
                return pID;
            }

            Debug.LogWarning("C#收到无法辨认的协议号; DEC: {0} HEX: {1}", id, id.ToString("X4"));

            return new ProtoID(id);

        }

        /// <summary>
        /// 协议体类型转换为协议ID
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ProtoID GetProtoIDByBody(ProtoBody body)
        {
            Type type = body.GetType();

            ProtoID pID;

            if (typeMap.TryGetValue(type, out pID))
            {
                return pID;
            }

            Debug.LogWarning("没有ID号的ProtoBody : {0}", body.GetType().Name);
            return CMD_STATIC_NULL;
        }


        //[12/4/2014 史耀力]  
        //C#静态对象的构建机制是在运行时第一次使用静态对象的类时,这样太晚了不满足我们的需求
        //这里遍历各个Assembly,找到ProtoID的派生类,强制调用了他们的类构造函数,以生成静态数据成员
        public static void Init()
        {
            //AssemblyLoader.LoadFromManifest(1, (s) => { return true; });

            HashSet<string> filter = new HashSet<string>();
            filter.Add("System");
            filter.Add("mscorlib");
            filter.Add("Accessibility");
            filter.Add("log4net");
            filter.Add("protobuf-net");
            filter.Add("MySql.Data");

            System.Reflection.AssemblyName[] assemblies = System.Reflection.Assembly.GetEntryAssembly().GetReferencedAssemblies();
            foreach (System.Reflection.AssemblyName name in assemblies)
            {
                _ParseAssembly(name, filter);
            }
        }


        private static bool _NeedToParse(string assemblyName, HashSet<string> filter)
        {
            if (assemblyName.StartsWith("System", true, null))
            {
                return false;
            }

            if (assemblyName.StartsWith("Microsoft", true, null))
            {
                return false;
            }

            return !filter.Contains(assemblyName);
        }

        private static void _ParseAssembly(System.Reflection.AssemblyName name, HashSet<string> filter)
        {
            //跳过不需要解析的assembly
            if (!_NeedToParse(name.Name, filter))
            {
                return;
            }

            Assembly assem = System.Reflection.Assembly.Load(name);
            foreach (Type tp in assem.GetTypes())
            {
                if (tp.IsSubclassOf(typeof(ProtoID)))
                {
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(tp.TypeHandle);
                }
            }

            filter.Add(name.Name);

            System.Reflection.AssemblyName[] assemblies = assem.GetReferencedAssemblies();
            foreach (System.Reflection.AssemblyName nameSub in assemblies)
            {
                _ParseAssembly(nameSub, filter);
            }
        }


        /// <summary>
        ///构建uint16与ProtoID的转换
        /// </summary>
        private static Dictionary<UInt16, ProtoID> idMap;

        /// <summary>
        /// 构建协议体类型与协议ID的转换
        /// </summary>
        private static Dictionary<Type, ProtoID> typeMap;

        /// <summary>
        ///默认无效的协议,充当NULL使用
        /// </summary>
        public static readonly ProtoID CMD_STATIC_NULL = new ProtoID(0x0000, typeof(int), "CMD_STATIC_NULL", false, false);
        /// <summary>
        ///连接成功的协议
        /// </summary>
        public static readonly ProtoID CMD_SERVER_CONNECTED = new ProtoID(0xff01, typeof(ServerConnected), "CMD_SERVER_CONNECTED", false, false);
        /// <summary>
        /// 连接失败的协议
        /// </summary>
        public static readonly ProtoID CMD_SERVER_DISCONNECTED = new ProtoID(0xff02, typeof(ServerDisconnected), "CMD_SERVER_DISCONNECTED", false, false);



        /// <summary>
        /// 测试协议
        /// </summary>
        public static readonly ProtoID CMD_TEST = new ProtoID(0xff03, typeof(TestProtoBody), "CMD_TEST", false, false);
    }
}
