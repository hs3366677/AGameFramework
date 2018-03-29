using System.Collections.Generic;
using System.Threading;
using System.Net;

namespace FileDownload
{
    class HttpDownloadManager
    {
        int maxThreadNum = 4;
        // 当前正在下载的线程信息
        //Dictionary<Thread, HttpDownTask> downloadThreadMap = new Dictionary<Thread, HttpDownTask>();

        List<HttpDownTask> downloadThreadMap = new List<HttpDownTask>();
        // 所有任务列表
        Dictionary<long, HttpDownTask> allTaskMap = new Dictionary<long, HttpDownTask>();


        // 等待下载的队列，普通和高优先级队列
        Queue<HttpDownTask> downloadQueue = new Queue<HttpDownTask>();
        Queue<HttpDownTask> highPriorityQueue = new Queue<HttpDownTask>();

        public HttpDownloadManager(int threadNum)
        {
            maxThreadNum = threadNum;

            int minWorker, minIOC;
            // Get the current settings.
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            // Change the minimum number of worker threads to four, but
            // keep the old setting for minimum asynchronous I/O 
            // completion threads.
            if (ThreadPool.SetMinThreads(4, minIOC))
            {
                // The minimum number of threads was set successfully.
            }
            else
            {
                // The minimum number of threads was not changed.
            }
        }

        public void Destroy()
        {
            // 先把等待下载队列清除
            ClearNormalQueue();
            ClearPriorityQueue();

            //// 然后终止当前正在下载线程
            //foreach (var pair in downloadThreadMap)
            //{
            //    // 如果当前任务还在线程中下载处理
            //    if (pair.Key.ThreadState == ThreadState.Running)
            //    {
            //        pair.Key.Abort();
            //    }
            //}



            // 清空索引信息
            downloadThreadMap.Clear();
            allTaskMap.Clear();
        }

        long TaskId = 0;
        public long GenerateTaskId()
        {
            ++TaskId;
            if(TaskId < 0)
            {
                // 翻转
                TaskId = 0;
            }

            return TaskId;
        }

        public void EnqueueNormalTask(HttpDownTask task)
        {
            lock(downloadQueue)
            {
                downloadQueue.Enqueue(task);
            }
        }

        public HttpDownTask DequeueNormalTask()
        {
            lock (downloadQueue)
            {
                return downloadQueue.Dequeue();
            }
        }

        public int GetNormalQueueCount()
        {
            lock (downloadQueue)
            {
                return downloadQueue.Count;
            }
        }

        public void ClearNormalQueue()
        {
            lock (downloadQueue)
            {
                downloadQueue.Clear();
            }
        }

        public void EnqueueHighPriorityTask(HttpDownTask task)
        {
            lock(highPriorityQueue)
            {
                highPriorityQueue.Enqueue(task);
            }
        }

        public HttpDownTask DequeueHighPriorityTask()
        {
            lock(highPriorityQueue)
            {
                return highPriorityQueue.Dequeue();
            }
        }

        public int GetHighPriorityQueueCount()
        {
            lock(highPriorityQueue)
            {
                return highPriorityQueue.Count;
            }
        }

        public void ClearPriorityQueue()
        {
            lock(highPriorityQueue)
            {
                highPriorityQueue.Clear();
            }
        }

        public long DownLoadOneFile(string fileUrl, string savePath, bool highPriority = false, bool erase = false)
        {
            HttpWebRequest request = HttpClient.getWebRequest(fileUrl, 0);
            WebResponse response = request.GetResponse();
            long fileLength = response.ContentLength;

            long taskId =  GenerateTaskId();
            HttpDownTask downloadTask = new HttpDownTask(taskId, fileUrl, savePath, fileLength, erase);
            if (true  == highPriority)
            {
                EnqueueHighPriorityTask(downloadTask);
            }
            else
            {
                EnqueueNormalTask(downloadTask);
            }

            // 添加到所有任务列表
            allTaskMap.Add(taskId, downloadTask);

            request.Abort();
            response.Close();

            return taskId;
        }

        public long DownLoadOneFile(string fileUrl, string savePath, int totalLength, bool highPriority = false, bool erase = false)
        {
            long fileLength = totalLength;

            long taskId = GenerateTaskId();
            HttpDownTask downloadTask = new HttpDownTask(taskId, fileUrl, savePath, fileLength, erase);
            if (true == highPriority)
            {
                EnqueueHighPriorityTask(downloadTask);
            }
            else
            {
                EnqueueNormalTask(downloadTask);
            }

            // 添加到所有任务列表
            allTaskMap.Add(taskId, downloadTask);

            return taskId;
        }

        public void FinishDownLoad(long taskId)
        {
            HttpDownTask downloadTask;
            bool bRet = allTaskMap.TryGetValue(taskId, out downloadTask);
            if (true != bRet)
            {
                // 已经删除了
                //LogSys.Warn("task already not in taskmap id[{0}]", taskId);
                return;
            }

            //foreach (var pair in downloadThreadMap)
            //{
            //    if(pair.Value.taskId == taskId)
            //    {
            //        if (pair.Key.ThreadState == ThreadState.WaitSleepJoin)
            //        {
            //            //UnityEngine.Debug.Log("task " + pair.Value.taskId + " interruptted");

            //            pair.Key.Interrupt();
            //        }
            //        // 如果当前任务还在线程中下载处理
            //        else if ((ThreadState.Stopped != pair.Key.ThreadState)
            //            || (ThreadState.Aborted != pair.Key.ThreadState))
            //        {
            //            //UnityEngine.Debug.Log("task " + pair.Value.taskId + " aborted");
            //            pair.Key.Abort();
            //        }

            //        break;
            //    }
            //}

            allTaskMap.Remove(taskId);
        }

        public double GetDownloadProgress(long taskId)
        {
            HttpDownTask downloadTask;
            bool bRet = allTaskMap.TryGetValue(taskId, out downloadTask);
            if(true != bRet)
            {
                // 没有找到对应的任务，或者可能已经完成删除了，所以返回100%
                return 100;
            }

            return downloadTask.GetDownloadProgress();
        }

        public double GetDownloadSpeed(long taskId)
        {
            HttpDownTask downloadTask;
            bool bRet = allTaskMap.TryGetValue(taskId, out downloadTask);
            if (true != bRet)
            {
                // 没有找到对应的任务，或者可能已经完成删除了，所以速度是0
                return 0;
            }

            return downloadTask.GetDownloadSpeed();
        }

        public void DownloadCheck()
        {
            //List<Thread> finishList = new List<Thread>();
            List<HttpDownTask> finishList = new List<HttpDownTask>();

            while(true)
            {
                //foreach(KeyValuePair<Thread, HttpDownTask> pair in downloadThreadMap)
                //{
                //    Thread downloadThread = pair.Key;
                //    if((pair.Key.ThreadState == ThreadState.Stopped)
                //        || (pair.Key.ThreadState == ThreadState.Aborted))
                //    {
                //        finishList.Add(pair.Key);
                //    }
                //}

                foreach (HttpDownTask pair in downloadThreadMap)
                {
                    //Thread downloadThread = pair.Key;
                    //if ((pair.Key.ThreadState == ThreadState.Stopped)
                    //    || (pair.Key.ThreadState == ThreadState.Aborted))
                    if (pair.finished)
                    {
                        finishList.Add(pair);
                    }
                }

                // 清理下载完成的任务
                foreach (HttpDownTask finishThread in finishList)
                {
                    downloadThreadMap.Remove(finishThread);
                }

                finishList.Clear();

                // 当前线程处理已满
                if(downloadThreadMap.Count >= maxThreadNum)
                {
                    Thread.Sleep(100);
                    continue;
                }

                HttpDownTask curTask = null;
                //Thread curThread;

                // 有空闲线程，则尝试开启下载任务
                while ((GetHighPriorityQueueCount() > 0) && (downloadThreadMap.Count < maxThreadNum))
                {
                    curTask = DequeueHighPriorityTask();
                    if (null != curTask)
                    {
                        //LogSys.Info("taskid[{0}] Begin to down normal file[{1}]", curTask.taskId, curTask.downLoadUrl);


                        ThreadPool.QueueUserWorkItem(new WaitCallback(curTask.DownLoadFile), 1);
                        //curThread = new Thread(new ThreadStart(curTask.DownLoadFile));
                        //downloadThreadMap.Add(curThread, curTask);

                        downloadThreadMap.Add(curTask);
                        //curThread.Start();
                        
                    }
                }

                while ((downloadThreadMap.Count < maxThreadNum) && (GetNormalQueueCount() > 0))
                {
                    curTask = DequeueNormalTask();
                    if (null != curTask)
                    {
                        //LogSys.Info("taskId[{0}] Begin to down high priority file[{1}]", curTask.taskId, curTask.downLoadUrl);

                        ThreadPool.QueueUserWorkItem(new WaitCallback(curTask.DownLoadFile), 1);

                        
                        //curThread = new Thread(new ThreadStart(curTask.DownLoadFile));
                        downloadThreadMap.Add(curTask);

                        //curThread.Start();
                    }
                }

                if (!IsBusy())
                {
                    // 已经空闲了
                    Thread.Sleep(100);
                }
                
            }
            
        }

        private bool IsBusy()
        {
            if (GetHighPriorityQueueCount() > 0)
            {
                return true;
            }

            if (GetNormalQueueCount() > 0)
            {
                return true;
            }

            if (downloadThreadMap.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
