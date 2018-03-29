using System;
using System.Net;
using System.IO.Compression;

namespace FileDownload
{
    class HttpClient
    {
        public static HttpWebRequest getWebRequest(string url, int startPos)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.AddRange(startPos);  //设置Range值
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            

            return request;
        }
    }
}
