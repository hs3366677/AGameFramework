using System;
using System.Collections.Generic;
using ProtoBuf;
using Protocols.Error;

namespace Protocols.Pipelines
{
    /// <summary>
    /// Pipeline的一个工序
    /// </summary>
    [ProtoContract]
    public class Process
    {
        //for protobuf
        public Process()
        {
        }

        public Process(ProtoBody body)
        {
            Specification = body;
            processID = ProtoID.GetProtoIDByBody(body);
        }

        public ProtoID GetProcessID()
        {
            return processID; 
        }

        public bool GetProduct<T>(out T product) where T: ProtoBody
        {
            product = Product as T;
            return null != product; 
        }

        /// <summary>
        /// 工序说明书
        /// </summary>
        [ProtoMember(1, DynamicType = true)]
        public ProtoBody Specification;

        /// <summary>
        /// 工序半成品
        /// </summary>
        [ProtoMember(2, DynamicType = true)]
        public ProtoBody Product;

        /// <summary>
        /// 工序标志位
        /// </summary>
        [ProtoMember(3)]
        public ProcessFlags Flags = ProcessFlags.Required;

        /// <summary>
        /// 工序状态
        /// </summary>
        [ProtoMember(4)]
        public ProcessStatus Status = ProcessStatus.NotDone;

        /// <summary>
        /// Error Code for Process Result
        /// </summary>
        [ProtoMember(5)]
        public ErrorID errCode = ErrorID.Success;

        [ProtoMember(6)]
        private UInt16 processID;

    }
}
