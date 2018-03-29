using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralGameSetting : ScriptableObject
{

    /// <summary>
    /// 资源储存根目录
    /// </summary>
    public string ExternFileRootPath
    {
        get;
        set;
    }
    /// <summary>
    /// .
    /// </summary>
    public string StreamingRootPath
    {
        get;
        set;
    }
    /// <summary>
    /// 文件服务器地址（用途1）
    /// </summary>
    public string FileServerAddrUsage1
    {
        get;
        set;
    }
    /// <summary>
    /// 文件服务器地址（用途2）
    /// </summary>
    public string FileServerAddrUsage2
    {
        get;
        set;
    }
    /// <summary>
    /// 是否用本地资源替代外部资源
    /// </summary>
    public bool IsLocalResource
    {
        get;
        set;
    }
    /// <summary>
    /// 是否开启下载
    /// 如果下载没有开启，则跳过更新/下载流程
    /// </summary>
    public bool DownloadEnabled
    {
        get;
        set;
    }

}
