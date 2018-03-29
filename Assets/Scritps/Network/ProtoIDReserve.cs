/********************************************************************
	created:	2014/12/10
	created:	10:12:2014   9:44
	filename: 	CommonPlatform\Server\ServerInstance\CommonLib\Protocols\ProtoIDCommon.cs
	file path:	CommonPlatform\Server\ServerInstance\CommonLib\Protocols
	file base:	ProtoIDCommon
	file ext:	cs
	author:		史耀力
	
	purpose:    Server实例协议定义
                NOTE: 协议高8位必须与接收协议的模块ID一致!
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocols
{
    public class ProtoIDReserve : ProtoID
    {
        protected ProtoIDReserve(UInt16 protoIDIn, Type bodyTypeIn, string nameIn, bool clientIn, bool visibleIn)
            : base(protoIDIn, bodyTypeIn, nameIn, clientIn, visibleIn)
        {
        }


        /// <summary>
        /// 连接成功后由本地发送
        /// </summary>
        public static ProtoIDReserve CMD_SERVER_CONNECTED = new ProtoIDReserve(0x7f02, typeof(ServerConnected), "CMD_SERVER_CONNECTED", false, false);


        /// <summary>
        /// 连接失败后由本地发送
        /// </summary>
        public static ProtoIDReserve CMD_SERVER_DISCONNECTED = new ProtoIDReserve(0x7f03, typeof(ServerDisconnected), "CMD_SERVER_DISCONNECTED", false, false);
    }
}
