using System;
using System.Collections.Generic;
using ProtoBuf;
using Protocols;

namespace Protocols.Pipelines
{
    [ProtoContract]
    public class Pipeline : ProtoBody
    {

        #region Pipeline Operation

        /// <summary>
        /// Get pipeline current status 
        /// </summary>
        /// <returns></returns>
        public PipelineStatus GetStatus()
        {
            if (assembled)
            {
                return PipelineStatus.Assembled;
            }

            foreach (Process proc in listProcess)
            {
                if (proc.Status == ProcessStatus.NotDone)
                {
                    return PipelineStatus.Processing;
                }

                if (proc.Status == ProcessStatus.Fail )
                {
                    return PipelineStatus.Fail;
                }
            }

            return PipelineStatus.Success;
        }

        /// <summary>
        /// pipeline now ready to assemble
        /// </summary>
        /// <returns></returns>
        public bool IsReadyToAssemble()
        {
            return GetStatus() == PipelineStatus.Success;
        }



        /// <summary>
        /// all process is handled?no matter process result is success or failed.
        /// </summary>
        /// <returns></returns>
        public bool IsAllProcessHandled()
        {
            foreach (Process proc in listProcess)
            {
                if (proc.Status == ProcessStatus.NotDone)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Is pipeline handle failed?
        /// process with Required flag has handled failed would cause pipeline fail.
        /// </summary>
        /// <returns></returns>
        public bool IsPipelineFail()
        {
            return GetStatus() == PipelineStatus.Fail;
        }

        public UInt32 GetAssembleAddr()
        {
            return assembleServer;
        }

        public void SetAssembleAddr(UInt32 addrSet)
        {
            assembleServer = addrSet;
        }

        public UInt16 GetPipelineID()
        {
            return pipelineID;
        }

        public void SetPipelineID(UInt16 idSet)
        {
            pipelineID = idSet;
        }

        public void SetIndex(UInt64 indexSet)
        {
            index = indexSet;
        }

        public UInt64 GetIndex()
        {
            return index;
        }

        public void SetRoleID(UInt64 idIn)
        {
            roleID = idIn;
        }

        public UInt64 GetRoleID()
        {
            return roleID;
        }

        public void SetAssembled()
        {
            assembled = true;
        }

        public string Dump()
        {
            return string.Format("ID:{0}, Index:{1}, ProcCount:{2}", GetPipelineID(), GetIndex(), GetProcessCount());
        }

        #endregion

        #region Process Operation

        public int AddProcess(Process proc)
        {
            if (null != proc)
            {
                listProcess.Add(proc);

                return 0;
            }

            return -1;
        }

        public int AddProcess(ProtoBody body)
        {
            return AddProcess(new Process(body));
        }

        public Process GetNextProcess()
        {
            foreach (Process proc in listProcess)
            {
                if (proc.Status == ProcessStatus.NotDone)
                {
                    return proc;
                }
            }

            return null;
        }

        public int GetProcessCount()
        {
            return listProcess.Count;
        }

        public Process GetProcess(int index)
        {
            if (index < 0 || index >= GetProcessCount())
            {
                return null;
            }

            return listProcess[index];
        }



        #endregion

        #region Data Member

        /// <summary>
        /// pipeline类型ID
        /// </summary>
        [ProtoMember(1)]
        private UInt16 pipelineID;

        /// <summary>
        /// server address where assemble the pipeline 
        /// </summary>
        [ProtoMember(2)]
        private UInt32 assembleServer;

        /// <summary>
        /// 工序集合
        /// </summary>
        [ProtoMember(3)]
        private List<Process> listProcess = new List<Process>();

        [ProtoMember(4)]
        private UInt64 index;

        [ProtoMember(5)]
        private UInt64 roleID;

        [ProtoMember(6)]
        private bool assembled = false;

        #endregion
    }
}
