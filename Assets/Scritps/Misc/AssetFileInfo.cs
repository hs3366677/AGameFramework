using System;
using System.Collections.Generic;
using System.Text;

//namespace AssetFileLibrary
//{

    //[Serializable]
    public class AssetFileInfo : IComparable<AssetFileInfo>
    {
        public string m_fileName { get; set; }                  //文件名
        public string m_filePath { get; set; }                  //文件路径
        public int m_fileVersion { get; set; }                   //文件版本号
        public string m_md5 { get; set; }                          //唯一标示MD5 
        public long m_size { get; set; }                          //大小
        public DownLoadPriority m_filePriority { get; set; }     //文件下载优先级

        bool m_isFinish;                                        //文件是否已经下载完成标记
        Action<AssetFileInfo> m_callback;                      //下载完成后的回调


        /// <summary>
        /// 文件是否已经下载完成
        /// </summary>
        public bool IsDownLoadFinish
        {
            get { return m_isFinish; }
            set { m_isFinish = value; }
        }

        /// <summary>
        /// 文件下载完成后的回调
        /// </summary>
        public Action<AssetFileInfo> CallBack
        {
            get { return m_callback; }
            set { m_callback = value; }
        }

        /// <summary>
        /// Sort提供的默认对比函数
        /// </summary>
        public int CompareTo(AssetFileInfo info)
        {
            if ((int)this.m_filePriority > (int)info.m_filePriority)
                return -1;
            else if ((int)this.m_filePriority == (int)info.m_filePriority)
                return 0;
            else
                return 1;
        }
        public override bool Equals(object obj)
        {
            AssetFileInfo other = obj as AssetFileInfo;
            if (other == null)
                return false;
            return this.m_fileName == other.m_fileName;
        }

        public override int GetHashCode()
        {
            return m_fileName.GetHashCode();
        }

        public static bool operator ==(AssetFileInfo lhs, AssetFileInfo rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true;
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                return false;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(AssetFileInfo lhs, AssetFileInfo rhs)
        {
            return !(lhs == rhs);
        }
    }

    //[Serializable]
    public enum DownLoadPriority
    {
        Low,
        Middle,
        High
    }
//}
