/********************************************************************
	created:	2015/04/07
	created:	7:4:2015   14:40
	filename: 	D:\百度云\CommonPlatform\Server\Framework\Protocols\Helper\IProtoLogger.cs
	file path:	D:\百度云\CommonPlatform\Server\Framework\Protocols\Helper
	file base:	IProtoLogger
	file ext:	cs
	author:		史耀力
	
	purpose:	ProtoLogger接口
*********************************************************************/
using System;

namespace Protocols
{
    /// <summary>
    /// Log等级
    /// </summary>
    public enum ProtoLoggerLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
    }

    /// <summary>
    /// 可实现此接口以依据环境个性化
    /// </summary>
    public interface IProtoLogger
    {
        void Log(ProtoLoggerLevel level, string msg, params object[] args);
        void LogModule(ProtoLoggerLevel level, string moduleName, string msg, params object[] args);
        void LogException(Exception e);
    }
}
