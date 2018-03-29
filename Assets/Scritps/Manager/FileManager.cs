using System.Collections;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
//using AssetFileLibrary;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FileManager
{

    // 服务器上的最新文件列表
    List<AssetFileInfo> m_serverFiles = new List<AssetFileInfo>();

    public List<AssetFileInfo> ServerFileList
    {
        get
        {
            return m_serverFiles;
        }
    }

    // 本地的文件列表
    List<AssetFileInfo> m_localFiles;

    static FileManager s_instance = null;

    GeneralGameSetting m_gameSetting;

    public static void Init(GeneralGameSetting setting)
    {
        s_instance = new FileManager(setting);
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    FileManager(GeneralGameSetting setting)
    {
        m_gameSetting = setting;
        ReadLocalFileList();
    }

    /// <summary>
    /// 保存data至路径fullpath，如果已存在则覆盖文件
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fullPath"></param>
    public void SaveFile(byte[] data, string fullPath)
    {
        //有些时候路径名会多出来//
        fullPath = fullPath.Replace("\\", "/");

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 检查保存路径是否存在，不存在就创建
            DirectoryInfo parentInfo = Directory.GetParent(fullPath);
            if (!parentInfo.Exists)
            {
                // 从根目录开始创建中间缺少的所有目录
                parentInfo.Root.CreateSubdirectory(parentInfo.FullName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        
        using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
        {
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
    }

    public void SaveFile(Stream data, string fullPath)
    {
        //有些时候路径名会多出来//
        fullPath = fullPath.Replace("\\", "/");
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 检查保存路径是否存在，不存在就创建
            DirectoryInfo parentInfo = Directory.GetParent(fullPath);
            if (!parentInfo.Exists)
            {
                // 从根目录开始创建中间缺少的所有目录
                parentInfo.Root.CreateSubdirectory(parentInfo.FullName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        byte[] buffer = new byte[1024];
        FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);

        int byteRead = 0;
        data.Seek(0, SeekOrigin.Begin);
        while ((byteRead = data.Read(buffer, 0 , 1024)) >0 )
        {
            fs.Write(buffer, 0, byteRead);
        }

        fs.Flush();

        fs.Close();
        
    }

    /// <summary>
    /// 文件下载完成后
    /// 把已经下载的文件信息存储到本地文件列表里
    /// 并且处理回调（如果有赋值的话）
    /// </summary>
    public void FileDownloadFinish(AssetFileInfo info)
    {
        //本地文件未读取时，需要读取
        if (m_localFiles == null)
        {
            ReadLocalFileList();
        }

        AssetFileInfo local = m_localFiles.Find(e => e.m_fileName == info.m_fileName);
        if (local == null)
        {
            local = info;
            m_localFiles.Add(info);
            info.IsDownLoadFinish = true;
        }else
        {
            local.m_filePath = info.m_filePath;
            local.m_fileVersion = info.m_fileVersion;
            local.m_md5 = info.m_md5;
            local.m_size = info.m_size;
            local.m_filePriority = info.m_filePriority;
            local.IsDownLoadFinish = true;
            info.IsDownLoadFinish = true;
        }
        
        if (info.CallBack != null)
            info.CallBack(info);
    }

    /// <summary>
    /// 退出游戏时，保存已下载的文件列表至本地
    /// </summary>
    public void SaveFileList()
    {
        m_localFiles.RemoveAll(e => e.m_fileName == "filelist");

        foreach (var file in m_localFiles)
            file.CallBack = null;//清空callback否则无法序列化写入

        try
        {
            using (FileStream fs = new FileStream(m_gameSetting.ExternFileRootPath + "/localFileList.ini", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, m_localFiles);
                fs.Close();

                //Debugger.LogInfo("LocalFileList save finished.");
            }
        }catch{

        }

    }



    /// <summary>
    /// 删除下载成功，但是数据不对的文件
    /// </summary>
    public void RemoveFromLocalList(string assetName)
    {
        m_localFiles.RemoveAll(e => e.m_fileName == assetName);
    }

    /// <summary>
    /// 获取服务器上最新的文件信息
    /// </summary>
    public AssetFileInfo GetServerAssetFileInfo(string assetName)
    {
        return m_serverFiles.Find(e => e.m_fileName == assetName);
    }
     
    public string GetRemotePath(string relativePath)
    {
        string prefix;
        if (relativePath.StartsWith("Material") ||
            relativePath.StartsWith("Icon")||
            relativePath.StartsWith("PetRes"))
            prefix = m_gameSetting.FileServerAddrUsage1;
        else
            prefix = m_gameSetting.FileServerAddrUsage2;

        return (prefix + relativePath).Replace("\\", "/");
    }
    /// <summary>
    /// 获取本地的文件信息
    /// </summary>
    public AssetFileInfo GetLocalAssetFileInfo(string assetName)
    {
        return m_localFiles.Find(e => e.m_fileName == assetName);
    }

    //服务器文件列表下载完后序列化成List
    public void ReadFileList(AssetFileInfo info)
    {
        FileStream fsServer = new FileStream(m_gameSetting.ExternFileRootPath + info.m_filePath, FileMode.Open, FileAccess.Read);
        {
            BinaryFormatter bf = new BinaryFormatter();
            m_serverFiles = bf.Deserialize(fsServer) as List<AssetFileInfo>;
            fsServer.Close();
        }


        //本地文件未读取时，需要读取
        if (m_localFiles == null)
        {
            try
            {
                ReadLocalFileList();
            }
            catch
            {
                m_localFiles = new List<AssetFileInfo>();
            }
        }
        
    }

    public void ReadLocalFileList()
    {
        try
        {
            //read local file list
            using (FileStream fsLocal = new FileStream(m_gameSetting.ExternFileRootPath + "/localFileList.ini", FileMode.OpenOrCreate))
            {
                if (fsLocal.Length != 0)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    m_localFiles = bf.Deserialize(fsLocal) as List<AssetFileInfo>;
                }
                fsLocal.Close();
            }
        }catch{

        }

        if (m_localFiles == null)
            m_localFiles = new List<AssetFileInfo>();
    }

    public static FileManager Instance
    {
        get
        {
            return s_instance;
        }
    }

    /// <summary>
    /// asset是否有效；
    /// </summary>
    /// <param name="assetName">文件名前缀</param>
    /// <param name="?"></param>
    /// <returns>服务器没有记录，true; 本地文件和服务器记录一致，true; 本地文件为空或与服务器记录不一致，false</returns>
    public bool ValidateAsset(string assetName)
    {
        AssetFileInfo localFileInfo;
        AssetFileInfo serverFileInfo;

        if (!CheckFileExistInList(assetName, out localFileInfo, out serverFileInfo))
            return false;

        if (serverFileInfo == null)
            return true;

        if (!CheckFileExist(localFileInfo, serverFileInfo) ||
            //!CheckFileMD5InList(localFileInfo, serverFileInfo) || 
            !CheckFileMD5Real(localFileInfo, serverFileInfo))
        {
            return false;
        }
        else
            return true;
    }
    bool CheckFileExist(AssetFileInfo localInfo, AssetFileInfo serverInfo)
    {
        string fullLocalPath = m_gameSetting.ExternFileRootPath;

        fullLocalPath += serverInfo.m_filePath;

        if (File.Exists(fullLocalPath))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 在列表中检查文件
    /// </summary>
    /// <param name="assetName">需要检查的文件名前缀</param>
    /// <param name="localInfo">输出本地文件信息</param>
    /// <param name="serverInfo">输出远端文件信息</param>
    /// <returns></returns>
    bool CheckFileExistInList(string assetName, out AssetFileInfo localInfo, out AssetFileInfo serverInfo)
    {
        if ((serverInfo = m_serverFiles.Find(e => e.m_fileName == assetName)) == null)
        {
            //Debugger.LogError("{0} does not exit in Server", assetName);
            localInfo = null;
            return true;
        }
        else
        {
            if ((localInfo = m_localFiles.Find(e => e.m_fileName  == assetName)) == null)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// 比对唯一标识，比对本地列表文件中记载的MD5值
    /// </summary>
    bool CheckFileMD5InList(
        AssetFileInfo localInfo, AssetFileInfo serverInfo)
    {
        if (localInfo != null && localInfo.IsDownLoadFinish)
        {
            if (localInfo.m_md5 == serverInfo.m_md5)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 比对唯一标识，比对本地列表文件中实时计算的md5值
    /// </summary>
    bool CheckFileMD5Real(AssetFileInfo localInfo, AssetFileInfo serverInfo)
    {
        if (localInfo != null && localInfo.IsDownLoadFinish)
        {
            
            //这里获取MD5码，不去除'-'
            string filePath = m_gameSetting + localInfo.m_filePath;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] output = md5Provider.ComputeHash(fs);
            md5Provider.Clear();
            fs.Close();

            string localFileMd5 = BitConverter.ToString(output);

            if (localFileMd5 == serverInfo.m_md5)
                return true;
        }
        return false;
    }

    public bool ifUpdatePackage()
    {
        int count = 0;
        int totalCount = m_serverFiles.Count;

        //if more than half files are out-dated, update the whole package
        if (m_localFiles.Count < totalCount / 2)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}