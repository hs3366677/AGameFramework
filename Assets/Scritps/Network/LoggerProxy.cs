using System;

namespace Protocols
{
    public class LoggerProxy
    {
        static public void Log(ProtoLoggerLevel level, string msg, params object[] args)
        {
            logger.Log(level, msg, args);
        }
        static public void LogModle(ProtoLoggerLevel level, string modleName, string msg, params object[] args)
        {
            logger.LogModule(level, modleName, msg, args);
        }

        static public void SetProtoLogger(IProtoLogger newLogger)
        {
            if (null != newLogger)
            {
                logger = newLogger;
            }
        }

        static private IProtoLogger logger = new ProtoLogger();
    }
}
