using System.IO;
using System.Net;
using System;
using System.Threading;
using System.Diagnostics;
namespace FileDownload
{
    class HttpDownTask
    {
        public long taskId;  // 下载任务对应的ID
        public string downLoadUrl;
        string savePath;
        string fileName;
        long totalLength = 0;
        int downFailCount = 0; // 记录连续下载失败次数

        bool eraseOldFile = false; // 是否强制删除旧文件

        bool terminated = false;

        double speedTracker = 0;  //单位是KB/s

        public long currentLength { set;  get; }

        public DateTime startTime { get; private set; } // 下载任务开始时间

        public bool finished
        {
            set;
            get;
        }

        public HttpDownTask(long id, string url, string path, long length, bool erase)
        {
            taskId = id;
            downLoadUrl = url;
            savePath = path;
            totalLength = length;
            fileName = System.IO.Path.GetFileName(savePath);
            eraseOldFile = erase;
            finished = false;
        }

        public void Terminate(){
            terminated = true;
        }
/*
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="state">represents the current state of download : 1 means refresh; 0 means continue last download</param>
        public void DownLoadFile(object state)
        {
            finished = false;
            while(downFailCount <= 10)
            {
                long startPos = 0;
                MemoryStream saveFile = new MemoryStream();
                Stream reader = null;
                WebResponse response = null;
                HttpWebRequest request = null;
                

                try
                {
                    string direName = Path.GetDirectoryName(savePath);
                    if (!Directory.Exists(direName)) //如果不存在保存文件夹路径，新建文件夹
                    {
                        Directory.CreateDirectory(direName);
                    }

                    startPos = 0;
                    
                    currentLength = startPos;

                    //UnityEngine.Debug.Log("Thread3: " + downLoadUrl + " totallength = " + totalLength);

                    request = HttpClient.getWebRequest(downLoadUrl, (int)startPos);
                    response = request.GetResponse();

                    totalLength = response.ContentLength + startPos;
                    if (startPos >= response.ContentLength && !eraseOldFile)
                    {
                        //LogSys.Info("taskid[{2}] file already down finished! file[{0}] save[{1}]", downLoadUrl, savePath, taskId);

                        // 下载完成
                        saveFile.Close();
                        saveFile.Dispose();

                        finished = true;

                        //UnityEngine.Debug.Log("Thread : " + savePath + " " + response.ContentLength);
                        return;
                    }

                    reader = response.GetResponseStream();

                    byte[] buff = new byte[1024 * 2];
                    int readSize = 0; //实际读取的字节数

                    startTime = DateTime.Now;
                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    long speedDownloadCounter = 0;
                    int loopCount = 0;
                     

                    while ((readSize = reader.Read(buff, 0, buff.Length)) > 0)
                    {
                        loopCount++;
                        speedDownloadCounter += readSize;
                        if (loopCount % 10 == 0)
                        {
                            sw.Stop();
                            if (sw.ElapsedMilliseconds == 0)
                                speedTracker = 30000;
                            else
                                speedTracker = speedDownloadCounter / sw.ElapsedMilliseconds;
                            speedDownloadCounter = 0;
                            sw.Reset();
                            sw.Start();
                        }
                        currentLength += readSize;


                        //还有最有一步解压缩
                        if (currentLength == totalLength)
                        {
                            currentLength--;
                        }

                        saveFile.Write(buff, 0, readSize);
                        saveFile.Flush();

                    }

                    //UnityEngine.Debug.Log("Thread downloadtomemory finish " + downLoadUrl);

                    if (currentLength == totalLength - 1)
                    { 

                            FileManager.Instance.SaveFile(saveFile, savePath);

                            //UnityEngine.Debug.Log("Thread savefile finish " + savePath);

                        currentLength = totalLength;
                    }
                    sw.Stop();
                    saveFile.Close();
                    saveFile.Dispose();
                    reader.Close();
                    reader.Dispose();
                    response.Close();
                    request.Abort();

                    if (currentLength == totalLength)
                    {
                        //LogSys.Info("taskid[{2}] Download success! file[{0}] save[{1}]", downLoadUrl, savePath, taskId);
                        // 下载完成
                        downFailCount = 0;

                        //UnityEngine.Debug.Log("Thread download finish: " + savePath + " " + currentLength);

                        break;
                    }
                    else
                    {
                        // 下载失败继续尝试下载
                        downFailCount++;

                        //UnityEngine.Debug.LogError(string.Format("taskid[{3}] Download failed count[{2}]! file[{0}] save[{1}] ", downLoadUrl, savePath, downFailCount, taskId));
                    }

                }
                catch (ThreadInterruptedException ex)
                {
                    //UnityEngine.Debug.Log("Thread Exception " + ex.Message + "\n" + ex.StackTrace);
                    if(null != saveFile)
                    {
                        saveFile.Close();
                        saveFile.Dispose();
                        saveFile = null;
                    }

                    if(null != reader)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                    
                    if(null != response)
                    {
                        response.Close();
                        response = null;
                    }

                    if(null != request)
                    {
                        request.Abort();
                        request = null;
                    }
                }
                catch (ThreadAbortException ex)
                {
                    //UnityEngine.Debug.Log("Thread Exception " + ex.Message + "\n" + ex.StackTrace);
                    if (null != saveFile)
                    {
                        saveFile.Close();
                        saveFile.Dispose();
                        saveFile = null;
                    }

                    if (null != reader)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }

                    if (null != response)
                    {
                        response.Close();
                        response = null;
                    }

                    if (null != request)
                    {
                        request.Abort();
                        request = null;
                    }
                }catch (SystemException ex)
                {
                    //UnityEngine.Debug.Log("Thread Exception " + ex.Message + "\n" + ex.StackTrace);
                }catch (Exception ex)
                {
                    //UnityEngine.Debug.Log("Thread Exception " + ex.Message + "\n" + ex.StackTrace);
                }

                
            }

            finished = true;
        }
        */

        public void DownLoadFile(object state)
        {
            finished = false;
            while (downFailCount <= 10)
            {
                long startPos = 0;
                HttpWebRequest request = null;


                try
                {
                    //判断保存路径是否存在
                    string direName = Path.GetDirectoryName(savePath);
                    if (!Directory.Exists(direName))
                    {
                        Directory.CreateDirectory(direName);
                    }

                    //使用流操作文件
                    FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                    //获取文件现在的长度
                    long fileLength = fs.Length;
                    long totalLength = GetLength(downLoadUrl);

                    if (eraseOldFile || fileLength > totalLength)
                    {
                        fs.SetLength(0);
                    }
                    else
                    {
                        startPos = fileLength;
                    }

                    if(fileLength < totalLength)
                    {
                        fs.Seek(fileLength, SeekOrigin.Begin);

                        request = HttpWebRequest.Create(downLoadUrl) as HttpWebRequest;

                        request.AddRange((int)fileLength);
                        Stream  stream = request.GetResponse().GetResponseStream();

                        byte[] buffer = new byte[1024];

                        int length = stream.Read(buffer, 0, buffer.Length);
                        while(length > 0)
                        {
                            if (terminated) break;
                            fs.Write(buffer, 0, length);
                            fileLength += length;
                            currentLength = fileLength;
                            length = stream.Read(buffer, 0, buffer.Length);
                        }
                        stream.Close();
                        stream.Dispose();
                    }

                    fs.Close();
                    fs.Dispose();
                    //如果下载完毕，执行回调
                    if(currentLength == totalLength)
                    {
                        downFailCount = 0;

                        //UnityEngine.Debug.Log("Thread download finish: " + savePath + " " + currentLength);

                        break;
                    }
                    else
                    {
                        // 下载失败继续尝试下载
                        downFailCount++;

                        //UnityEngine.Debug.LogError(string.Format("taskid[{3}] Download failed count[{2}]! file[{0}] save[{1}] ", downLoadUrl, savePath, downFailCount, taskId));
                    }

                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
                finally
                {
                    if (null != request)
                    {
                        request.Abort();
                        request = null;
                    }
                }
            }

            finished = true;
        }

        /// <summary>
        /// 获取下载文件的大小
        /// </summary>
        /// <returns>The length.</returns>
        /// <param name="url">URL.</param>
        long GetLength(string url)
        {
            HttpWebRequest requet = HttpWebRequest.Create(url) as HttpWebRequest;
            requet.Method = "HEAD";
            HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
            return response.ContentLength;
        }
        
        public double GetDownloadProgress()
        {
            return (double)currentLength/totalLength * 100;
        }

        public double GetDownloadSpeed()
        {
            return speedTracker;
        }
    }
}
