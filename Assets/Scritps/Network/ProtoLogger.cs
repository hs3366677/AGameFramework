/********************************************************************
	created:	2015/04/07
	created:	7:4:2015   14:36
	filename: 	D:\百度云\CommonPlatform\Server\Framework\Protocols\Helper\ProtoLogger.cs
	file path:	D:\百度云\CommonPlatform\Server\Framework\Protocols\Helper
	file base:	ProtoLogger
	file ext:	cs
	author:		史耀力
	
	purpose:	协议工程自己使用Logger接口.可依据环境进行定制实现
*********************************************************************/

using System;

namespace Protocols
{
    public class ProtoLogger : IProtoLogger
    {
        public void Log(ProtoLoggerLevel level, string msg, params object[] args)
        {
        }

        public void LogModule(ProtoLoggerLevel level, string modleName, string msg, params object[] args)
        {
        }

        public void LogException(Exception e)
        {

        }
    }
}
