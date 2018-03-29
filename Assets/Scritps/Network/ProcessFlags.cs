using System;

namespace Protocols.Pipelines
{
    /// <summary>
    /// 工序特性标志位
    /// </summary>
    [Flags]
    public enum ProcessFlags :byte
    {
        /// <summary>
        /// 工序必须成功执行,否则Pipeline失败
        /// </summary>
        Required = 0x1 << 0,

        /// <summary>
        /// Pipeline失败则此工序需要Rollback
        /// </summary>
        Rollback = 0x1 << 1, 
    }
}
