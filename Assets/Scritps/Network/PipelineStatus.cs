using System;

namespace Protocols.Pipelines
{
    /// <summary>
    /// pipeline status
    /// </summary>
    public enum PipelineStatus
    {
        /// <summary>
        /// still processing, need to trans to logic server for next process
        /// </summary>
        Processing,

        /// <summary>
        /// already failed
        /// </summary>
        Fail,

        /// <summary>
        /// already success
        /// </summary>
        Success,

        /// <summary>
        /// 已经由Assembler处理过了,完成Pipeline生命周期
        /// </summary>
        Assembled,
    }
}
