using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
//using AssetFileLibrary;
using FileDownload;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
public class DownloadManager
{
    static DownloadManager instance;

    GeneralGameSetting m_gameSetting;
    public static void Init(MonoBehaviour mono, GeneralGameSetting gameSetting)
    {
        instance = new DownloadManager();

        //这里还可以用多线程模式，速度其实差不多
        instance.httpDownload = HttpDownload.GetHttpDownload(HttpDownload.DownloadMechanic.Coroutine, 3, mono);
        instance.m_gameSetting = gameSetting;
    }

    public static DownloadManager Instance
    {
        get
        {
            return instance;
        }
    }

    //最大同时下载数
    const int MAX_DOWNLOAD_LINE = 5;

    //等待下载的文件列表
    Queue<AssetFileInfo> m_downLoadList = new Queue<AssetFileInfo>();
    //已经开始的下载
    Dictionary<AssetFileInfo, WWW> m_taskList = new Dictionary<AssetFileInfo, WWW>();
    //已经结束的下载
    List<AssetFileInfo> m_finishList = new List<AssetFileInfo>();

    IHttpDownload httpDownload;


    //等待下载的进程
    Dictionary<AssetFileInfo, long> downloadIds = new Dictionary<AssetFileInfo, long>();

    //正在进行的下载
    Dictionary<AssetFileInfo, long> taskIds = new Dictionary<AssetFileInfo, long>();

    public double GetDownloadProgress(AssetFileInfo info)
    {
        if (taskIds.ContainsKey(info))
        {
            return httpDownload.GetDownloadProgress(taskIds[info]);
        }
        else
            return 100;
    }

    double currentDownloadProgress = 0;
    public double GetDownloadProgress()
    {
        return currentDownloadProgress;
    }

    public IEnumerator StartDownload(List<AssetFileInfo> fileList)
    {
        Dictionary<AssetFileInfo, long> localDownloadIds = new Dictionary<AssetFileInfo, long>();

        for (int i = 0; i < fileList.Count; i++)
        {
            if (
                !FileManager.Instance.ValidateAsset(fileList[i].m_fileName)
                //&& !downloadIds.ContainsKey(fileList[i])
                && !localDownloadIds.ContainsKey(fileList[i])
                && !m_finishList.Contains(fileList[i])
                )
            {
                if (downloadIds.ContainsKey(fileList[i]))
                {
                    //已经在下载列表中了
                    if (httpDownload.GetDownloadProgress(downloadIds[fileList[i]]) > 0)
                    {
                        UnityEngine.Debug.Log(fileList[i].m_fileName);
                        //已经开始下载了
                        localDownloadIds.Add(fileList[i], downloadIds[fileList[i]]);
                        continue;
                    }

                }

                {

                    //还没在下载列表里面的
                    AddToDownloadList(fileList[i]);



                    string remotePath = FileManager.Instance.GetRemotePath(fileList[i].m_filePath);
                    //Debug.Log(string.Format("准备下载 : {0}", remotePath));

                    Debug.Log(string.Format("准备下载(列表里) : {0}", remotePath));

                    string localPath = m_gameSetting.ExternFileRootPath + fileList[i].m_filePath;
                    long taskId = httpDownload.StartDownLoad(remotePath, localPath, (int)fileList[i].m_size, true, true);

                    if (taskId < 0)
                    {
                        Debug.LogError(string.Format("下载错误 : {0}", remotePath));
                        continue;
                    }
                    if (downloadIds.ContainsKey(fileList[i]))
                    {
                        downloadIds[fileList[i]] = taskId;
                    }
                    else
                        downloadIds.Add(fileList[i], taskId);

                    localDownloadIds.Add(fileList[i], taskId);
                }

            }
        }

        Dictionary<AssetFileInfo, double> progressTrace = new Dictionary<AssetFileInfo, double>();

        foreach (var id in localDownloadIds)
        {
            progressTrace.Add(id.Key, 0);
        }

        while (true)
        {
            List<AssetFileInfo> deleteList = new List<AssetFileInfo>();


            foreach (var id in localDownloadIds)
            {
                progressTrace[id.Key] = httpDownload.GetDownloadProgress(id.Value);

                if (progressTrace[id.Key] == 100) //下载完成
                {
                    FileManager.Instance.FileDownloadFinish(id.Key);

                    Debug.Log(string.Format("下载完成 File : {0}, Size:{1}", id.Key.m_fileName, id.Key.m_size));
                    deleteList.Add(id.Key);
                }
                else if (progressTrace[id.Key] == -1)
                {
                    Debug.Log(string.Format("下载失败 File : {0}, Size:{1}", id.Key.m_fileName, id.Key.m_size));

                    deleteList.Add(id.Key);
                }
            }

            foreach (var key in deleteList)
            {
                localDownloadIds.Remove(key);
                downloadIds.Remove(key);
                m_finishList.Add(key);
            }


            long totalSize = 0;
            currentDownloadProgress = 0;

            foreach (var progress in progressTrace)
            {
                totalSize += progress.Key.m_size;
                currentDownloadProgress += progress.Value * progress.Key.m_size;

                //Debug.Log("loading single progress : {0}", progress.Value);

            }

            if (totalSize != 0)
                currentDownloadProgress = currentDownloadProgress / totalSize;

            //Debug.Log("loading progress : {0}", currentDownloadProgress);

            if (localDownloadIds.Count == 0)
                yield break;
            else
                yield return new WaitForSeconds(0.5f);
        }
    }
    public IEnumerator StartBackgroundDownload()
    {

        if (!m_gameSetting.DownloadEnabled)
            yield break;

        foreach (var file in FileManager.Instance.ServerFileList)
        {
            if (!FileManager.Instance.ValidateAsset(file.m_fileName))
            {
                AddToDownloadList(file);

                string remotePath = FileManager.Instance.GetRemotePath(file.m_filePath);
                //Debug.Log(string.Format("准备下载 : {0}", remotePath));

                string localPath = m_gameSetting.ExternFileRootPath + file.m_filePath;
                long taskId = httpDownload.StartDownLoad(remotePath, localPath, (int)file.m_size, false, true);

                if (taskId < 0)
                {
                    Debug.LogError(string.Format("下载错误 : {0}", remotePath));
                    continue;
                }
                try
                {
                    downloadIds.Add(file, taskId);
                }
                catch (ArgumentException ex)
                {
                    Debug.LogError(ex.Message + " " + file.m_fileName);
                }
            }
        }

        //每20秒保存一次下载的文件列表
        int count = 0;
        while (true)
        {
            List<AssetFileInfo> deleteList = new List<AssetFileInfo>();

            foreach (var x in downloadIds)
            {
                double currentProgress = httpDownload.GetDownloadProgress(x.Value);
                if (currentProgress > 0 && currentProgress < 100) //下载中
                {
                    if (!taskIds.ContainsKey(x.Key))
                        taskIds.Add(x.Key, x.Value);
                }
                else if (currentProgress == 100) //下载完成
                {
                    if (!m_finishList.Contains(x.Key))
                    {
                        Debug.Log(string.Format("背景下载完成 File : {0}, Size:{1}", x.Key.m_fileName, x.Key.m_size));

                        FileManager.Instance.FileDownloadFinish(x.Key);

                        deleteList.Add(x.Key);
                    }
                }
                else if (currentProgress == -1)
                {
                    Debug.Log(string.Format("背景下载失败 File : {0}, Size:{1}", x.Key.m_fileName, x.Key.m_size));
                    deleteList.Add(x.Key);
                }
            }

            foreach (var key in deleteList)
            {
                taskIds.Remove(key);
                downloadIds.Remove(key);
                m_finishList.Add(key);
            }



            if (downloadIds.Count == 0)
            {
                FileManager.Instance.SaveFileList();

                Debug.Log(string.Format("背景下载结束。"));
                break;
            }

            if (count == 20)
            {
                FileManager.Instance.SaveFileList();
                count = 0;
            }

            count++;
            yield return new WaitForSeconds(1f);
        }
    }
    /// <summary>
    /// 立即下载，异步下载，然后保存至本地，存储位置为./AssetBundles/
    /// </summary>
    public IEnumerator StartDownload(AssetFileInfo info)
    {
        if (info == null || !m_gameSetting.DownloadEnabled)
            yield break;

        //正在下载中，则把优先级调高
        if (taskIds.ContainsKey(info))
        {
            int count = 100;
            //正在下载该文件，则最多等待100帧
            while (count > 0 && taskIds.ContainsKey(info))
            {
                count--;
                yield return null;
            }
            yield break;
        }


        AssetFileInfo tmp = m_finishList.Find(e => e.m_fileName == info.m_fileName);

        //如果从没下载过
        if (tmp == null)
        {
            float startTimer = Time.realtimeSinceStartup;

            string remotePath = FileManager.Instance.GetRemotePath(info.m_filePath);

            Debug.Log(string.Format("准备下载 : {0}", remotePath));


            string localPath = m_gameSetting.ExternFileRootPath + info.m_filePath;

            Stopwatch sw = new Stopwatch();

            sw.Start();

            int failCount = 0;
            long taskId = -1;
            while (failCount < 10)
            {
                taskId = httpDownload.StartDownLoad(remotePath, localPath, (int)info.m_size, true, true);

                if (taskId < 0)
                {
                    Debug.LogError(string.Format("预下载错误 : {0} 尝试 {1}", remotePath, failCount));
                    failCount++;
                    continue;
                }
                else
                    break;

            }

            if (failCount >= 10)
            {
                Debug.LogError(string.Format("下载错误 : {0}", remotePath));
                yield break;
            }

            taskIds.Add(info, taskId);

            double curDownloadProgress = httpDownload.GetDownloadProgress(taskId);
            while (curDownloadProgress < 100)
            {
                if (curDownloadProgress == -1)
                    break;
                yield return new WaitForSeconds(0.2f);
                curDownloadProgress = curDownloadProgress = httpDownload.GetDownloadProgress(taskId);
            }


            sw.Stop();


            if (curDownloadProgress == -1)
            {
                Debug.LogError(string.Format("下载失败 : {0} 耗时 ：{1}ms", remotePath, sw.ElapsedMilliseconds));

                taskIds.Remove(info);
            }
            else
            {
                Debug.Log(string.Format("下载完成 : {0} 耗时 ：{1}ms", remotePath, sw.ElapsedMilliseconds));

                taskIds.Remove(info);
                m_finishList.Add(info);

                FileManager.Instance.FileDownloadFinish(info);
            }

        }
    }

    public void StopAllDownload()
    {
        httpDownload.Destroy();
    }

    public void PrintDownloadStat()
    {
        long totalSize = 0;
        long totalFileNum = 0;
        foreach (AssetFileInfo info in m_finishList)
        {
            totalSize += info.m_size;
            totalFileNum++;
        }

        Debug.LogFormat("总下载量：{0};下载文件数：{1}", totalSize, totalFileNum);
    }
    public void ProcessQuit()
    {
        foreach (var task in m_taskList.Values)
            task.Dispose();
    }

    public void AddToDownloadList(AssetFileInfo info)
    {
        m_downLoadList.Enqueue(info);
    }

    public List<AssetFileInfo> FinishList
    {
        get { return m_finishList; }
    }
}