using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pack : MonoBehaviour {


    /// <summary>
    /// printf  tag
    /// </summary>
    public static readonly string PACKTAG = "打包";

    public static readonly string ProductName = "CodeGirl";
    public static readonly string CompanyName = "SHYeyou";

    public static readonly string[] ResourcesPath = new string[] { 
        ConcatPath(Application.dataPath, "GameModules") 
    };

    public static readonly string[] ResourceExt = new string[] {
                "bytes",    //lua script extension
                "prefab",   
                "controller",
                "mp3",
                "wav",
                "mask",
                "png",
                "jpg",
                "ttf",
                "asset",
                "xml"
    };
    /// <summary>
    /// Android安装包后缀名
    /// </summary>
    public static readonly string AndroidExt = "apk";

    public static readonly string txtExt = "txt";
    /// <summary>
    /// android assetbunlde path(original)
    /// </summary>
    public static readonly string AndroidAbPath = Application.dataPath + "/../AssetBundles_Android/";
    
    /// <summary>
    /// ios assetbundle path(original)
    /// </summary>
    public static readonly string iOSAbPath = Application.dataPath + "/../AssetBundles_iOS";

    #region Standalone

    /// <summary>
    /// StandAlone应用后缀名
    /// </summary>
    public static readonly string StandalonePostfix = "exe";
    /// <summary>
    /// standalone package path(original)
    /// </summary>
    public static readonly string StandaloneAbPath = Application.dataPath + "/../AssetBundles_Standalone";

    /// <summary>
    /// standalone exe directory
    /// </summary>
    public static readonly string StandaloneExeDirectory = Application.dataPath + "/../Game";

    /// <summary>
    /// standalone exe fullpath
    /// </summary>
    public static readonly string StandaloneExeFullPath = ConcatPath(StandaloneExeDirectory, ConcatFileName(ProductName, StandalonePostfix));

    public static readonly string StandaloneAblistFullPath = ConcatPath(StandaloneAbPath, ConcatFileName("abfilelist", txtExt));

    /// <summary>
    /// this is the directory that all packed files should be copied to;
    /// this directory differs from computers
    /// </summary>
    public static readonly string StandaloneRemoteDirectory = "file://172.16.12.200/otomeweb";

    #endregion




    #region 文件名合体方法

    /// <summary>
    /// 合体路径
    /// </summary>
    /// <param name="path1"></param>
    /// <param name="path2"></param>
    /// <returns></returns>
    public static string ConcatPath(string path1, string path2){
        return string.Format("{0}/{1}", path1, path2);
    }

    /// <summary>
    /// 合体文件名
    /// </summary>
    /// <param name="name"></param>
    /// <param name="postfix"></param>
    /// <returns></returns>
    public static string ConcatFileName(string name, string postfix)
    {
        return string.Format("{0}.{1}", name, postfix);
    }

    #endregion
}
