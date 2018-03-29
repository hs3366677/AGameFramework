/********************************************************************
	created:	2014/12/11
	created:	11:12:2014   19:24
	filename: 	CommonPlatform\Server\ServerInstance\CommonLib\Protocols\ProtoScene.cs
	file path:	CommonPlatform\Server\ServerInstance\CommonLib\Protocols
	file base:	ProtoScene
	file ext:	cs
	author:		史耀力
	
	purpose:    Protocol for Login Module 
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using playerid = System.UInt64;
using System.Net;

namespace Protocols
{
    public enum DisconnReason
    {
        Active,             //客户端主动断开
        Passive,            //服务器断开
        Exception,          //异常断开
    }

    public class ServerConnected : ProtoBody
    {
        public IPEndPoint endpoint;
        public bool success;
    }
    
    public class ServerDisconnected : ProtoBody
    {
        public DisconnReason reason;
    }
}