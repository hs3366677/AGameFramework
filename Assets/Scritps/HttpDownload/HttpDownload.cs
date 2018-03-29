using System.Threading;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileDownload;

namespace FileDownload
{
   
    public interface IHttpDownload
    {
        long StartDownLoad(string fileUrl, string savePath, int totallength, bool highPriority = false, bool erase = false);
        double GetDownloadProgress(long taskId);

        void Destroy();
    }

    public class HttpDownload{
        public enum DownloadMechanic{
            Native,
            Coroutine
        }
        public static IHttpDownload GetHttpDownload(DownloadMechanic mechanic, int thread, MonoBehaviour coroutineRoot)
        {
            if (mechanic == DownloadMechanic.Native)
            {
                return new HttpDownloadNative(thread);
            }else
            {
                return new HttpDownloadCoroutine(coroutineRoot, thread);
            }
        }
    }

    public class wwwWrapperClass
    {
        public WWW mWWW;
        public int downloadState;

        enum DownloadState
        {
            FAIL = -1,
            RETRYING = 0,
            DOWNLOADING = 1,
            FINISHED = 2
        }
    }

    public class HttpDownloadCoroutine : IHttpDownload
    {
        long currentTaskId = 0;
        int MAX_THREAD;
        Dictionary<long, wwwWrapperClass> wwwDic;

        Queue<ParamObj> tobeDownloadedParamsHighP;
        Queue<ParamObj> tobeDownloadedParams;

        int retryTimes = 15;
        /// <summary>
        /// 创建一个HttpDownLoad
        /// </summary>
        /// <param name="maxThreadNum">制定下载任务最多的执行线程数</param>
        public HttpDownloadCoroutine(MonoBehaviour coroutineParent, int maxThreadNum)
        {
            wwwDic = new Dictionary<long, wwwWrapperClass>();
            tobeDownloadedParamsHighP = new Queue<ParamObj>();
            tobeDownloadedParams = new Queue<ParamObj>();
            for (int i = 0; i < maxThreadNum; i++)
                coroutineParent.StartCoroutine(DownloadCoroutine());
        }

        ~HttpDownloadCoroutine()
        {
            Destroy();
        }

        public void Destroy()
        {
        }

        class ParamObj
        {
            public long taskId;
            public string m_fileUrl;
            public string m_savePath;
            public int m_totalLength;
            public bool m_highPriority;
        }
        public long StartDownLoad(string fileUrl, string savePath, int totallength, bool highPriority = false, bool erase = false)
        {
            ParamObj param = new ParamObj() { taskId = currentTaskId, m_fileUrl = fileUrl, m_savePath = savePath, m_totalLength = totallength, m_highPriority = highPriority };

            if (highPriority)
                tobeDownloadedParamsHighP.Enqueue(param);
            else
                tobeDownloadedParams.Enqueue(param);

            currentTaskId++;
            return currentTaskId - 1;
        }

        IEnumerator DownloadCoroutine()
        {
            while (true)

            {   
                ParamObj param = null;
                if (tobeDownloadedParamsHighP != null && tobeDownloadedParamsHighP.Count >0)
                    param = tobeDownloadedParamsHighP.Dequeue();
                else if (tobeDownloadedParams != null && tobeDownloadedParams.Count > 0)
                    param = tobeDownloadedParams.Dequeue();
                
                if (param != null)
                {
                    int failCount = 0;

                    wwwWrapperClass www = null;

                    while (failCount < retryTimes)
                    {
                        if (wwwDic.ContainsKey(param.taskId))
                        {
                            if (wwwDic[param.taskId].downloadState == 1) //下载中
                            {
                                if (param.m_highPriority)
                                    wwwDic[param.taskId].mWWW.threadPriority = UnityEngine.ThreadPriority.High;
                                break;
                            }
                            else if (wwwDic[param.taskId].downloadState == 0) //重试
                            {
                                wwwDic[param.taskId].mWWW = new WWW(param.m_fileUrl);
                                www = wwwDic[param.taskId];
                                wwwDic[param.taskId].downloadState = 1;
                            }
                            else //下载失败或者下载完成
                            {
                                break;
                            }
                        }
                        else
                        {
                            www = new wwwWrapperClass() { mWWW = new WWW(param.m_fileUrl),  downloadState = 1};
                            wwwDic.Add(param.taskId,
                                www
                                );
                        }
                        try
                        {
                            if (param.m_highPriority)
                                www.mWWW.threadPriority = UnityEngine.ThreadPriority.High;
                            else
                                www.mWWW.threadPriority = UnityEngine.ThreadPriority.Low;
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                        }
                        while (!www.mWWW.isDone)
                            yield return null;

                        if (string.IsNullOrEmpty(www.mWWW.error))
                        {
                            FileManager.Instance.SaveFile(www.mWWW.bytes, param.m_savePath);

                            //Debugger.LogInfo(string.Format("背景下载完成 File : {0}, Size:{1}", param.m_fileUrl, param.m_totalLength));
                            www.mWWW.Dispose();
                            www.downloadState = 2;
                            break;
                        }
                        else
                        {
                            www.mWWW.Dispose();
                            www.downloadState = 0;
                            failCount++;
                            continue;
                        }

                    }

                    if (failCount == retryTimes)
                    {
                        if (www != null) www.downloadState = -1;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        /// <summary>
        /// 获取下载任务的下载进度
        /// </summary>
        /// <param name="taskId">下载任务ID，start的时候返回</param>
        /// <returns>下载进度，百分比数据，如54.3%，返回54.3</returns>
        public double GetDownloadProgress(long taskId)
        {
            if (wwwDic.ContainsKey(taskId))
            {
                if (wwwDic[taskId].downloadState == 2)
                {
                    return 100;
                }else if (wwwDic[taskId].downloadState == 1)
                {
                    return Mathf.Min(99, wwwDic[taskId].mWWW.progress * 100);
                }else {
                    return -1;
                }
            }
                //try
                //{
                //    return Mathf.Min(99, wwwDic[taskId].mWWW.progress * 100);
                //}
                //catch
                //{
                //    //disposed,说明下载完成了，返回100
                //    return 100;
                //}
            else
                return 0;
        }
    }

    public class HttpDownloadNative : IHttpDownload
    {
        Thread downloadMgrThread = null; // 下载管理线程
        HttpDownloadManager downloadMgr = null; // 下载管理

        /// <summary>
        /// 创建一个HttpDownLoad
        /// </summary>
        /// <param name="maxThreadNum">制定下载任务最多的执行线程数</param>
        public HttpDownloadNative(int maxThreadNum)
        {
            if (null == downloadMgr)
            {
                downloadMgr = new HttpDownloadManager(maxThreadNum);
            }

            //LogSys.RegisterLogImpl(new FileLog());

            //LogSys.Info("HttpDownload init thread[{0}]", maxThreadNum);

            downloadMgrThread = new Thread(new ThreadStart(downloadMgr.DownloadCheck));
            downloadMgrThread.Start();
            return;
        }

        ~HttpDownloadNative()
        {
            Destroy();
        }

        public void Destroy()
        {
            //LogSys.Info("Http download destroyed");

            if(ThreadState.Running == downloadMgrThread.ThreadState)
            {
                downloadMgrThread.Abort();
            }

            if (null != downloadMgr)
            {
                downloadMgr.Destroy();
                downloadMgr = null;
            }
        }

        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="fileUrl">下载文件http:URL</param>
        /// <param name="savePath">下载文件本地保存路径，包含文件名</param>
        /// <param name="highPriority">下载文件是否是高优先级，优先下载</param>
        /// <param name="erase">下载文件，如果本地有文件是否要覆盖重下</param>
        /// <returns>Taskid 下载文件任务Id</returns>
        public long StartDownLoad(string fileUrl, string savePath, bool highPriority = false, bool erase = false)
        {
            if(null == downloadMgr)
            {
                return -1;
            }
            try
            {
                long taskId = downloadMgr.DownLoadOneFile(fileUrl, savePath, highPriority, erase);

                //LogSys.Info("taskid[{4}] Start download  file[{0}] savepath[{1}] high[{2}] erase[{3}]",
                //    fileUrl, savePath, highPriority, erase, taskId);

                return taskId;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return -1;
        }

        public long StartDownLoad(string fileUrl, string savePath, int totallength, bool highPriority = false, bool erase = false)
        {
            if (null == downloadMgr)
            {
                return -1;
            }
            try
            {
                long taskId = downloadMgr.DownLoadOneFile(fileUrl, savePath, totallength, highPriority, erase);

                //LogSys.Info("taskid[{4}] Start download  file[{0}] savepath[{1}] high[{2}] erase[{3}]",
                //    fileUrl, savePath, highPriority, erase, taskId);

                return taskId;
            }
            catch (Exception ex)
            {
                //UnityEngine.Debug.LogError("Start download Exception " + ex.Message + " " + ex.StackTrace);
            }

            return -1;
        }

        /// <summary>
        /// 获取下载任务的下载进度
        /// </summary>
        /// <param name="taskId">下载任务ID，start的时候返回</param>
        /// <returns>下载进度，百分比数据，如54.3%，返回54.3</returns>
        public double GetDownloadProgress(long taskId)
        {
            if (null == downloadMgr)
            {
                return 0;
            }

            return downloadMgr.GetDownloadProgress(taskId);
        }

        public double GetDownloadSpeed(long taskId)
        {
            if (null == downloadMgr)
            {
                return 0;
            }

            return downloadMgr.GetDownloadSpeed(taskId);
        }

        /// <summary>
        /// 完成一个下载任务
        /// </summary>
        /// <param name="taskId">下载任务ID，start的时候返回</param>
        public void FinishDownLoad(long taskId)
        {
            if (null == downloadMgr)
            {
                return;
            }

            //LogSys.Info("taskid[{0}] finish download!", taskId);

            downloadMgr.FinishDownLoad(taskId);
        }
    }
}
