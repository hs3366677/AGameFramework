using System;

namespace Protocols.Pipelines
{
    /// <summary>
    /// 工序状态
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// 等待完成
        /// </summary>
        NotDone = 0,

        /// <summary>
        /// 已完成(成功)
        /// </summary>
        Done,

        /// <summary>
        /// 失败
        /// </summary>
        Fail,
    }
}
