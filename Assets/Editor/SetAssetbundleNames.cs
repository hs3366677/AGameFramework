using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System;

public class SetAllAssetName
{
    static List<string> rootDirs = new List<string>{ 
                                   Application.dataPath + "/Resources/"

                                   };

    [MenuItem("打包/设置资源bundle名", false, 200)]
    public static void SetName()
    {
        //找到各个目录并且进行设置
        List<string> fileList = new List<string>();

        foreach (var x in rootDirs)
        {
            fileList.AddRange(Directory.GetFiles(x));
        }

        int length = Application.dataPath.Length;
        foreach (var file in fileList)
        {
            string path = file.Remove(0, length + 1);
            path = string.Format("Assets/{0}", path);
            AssetImporter improter = AssetImporter.GetAtPath(path);
            if (improter != null)
                improter.assetBundleName = Path.GetFileNameWithoutExtension(file);
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }

    [MenuItem("5.x Pack/清除资源bundle名")]
    public static void ClearName()
    {
        //找到各个目录并且进行设置
        List<string> fileList = new List<string>();
        fileList.AddRange(GetFiles(Application.dataPath + "/ResourcesCopy/"));
        fileList.AddRange(GetFiles(Application.dataPath + "/Fonts/"));

        int length = Application.dataPath.Length;
        foreach (var file in fileList)
        {
            string path = file.Remove(0, length + 1);
            path = string.Format("Assets/{0}", path);
            AssetImporter improter = AssetImporter.GetAtPath(path);
            if (improter != null)
                improter.assetBundleName = "";
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }

    //静态方法
    static List<string> GetFiles(string root)
    {
        List<string> myFileList = new List<string>();
        var dirFiles = Directory.GetFiles(root);
        foreach (var file in dirFiles)
        {
            if (file.EndsWith(".prefab") ||
                file.EndsWith(".controller") ||
                file.EndsWith(".mp3") ||
                file.EndsWith(".wav") ||
                file.EndsWith(".mask") ||
                file.EndsWith(".png") ||
                file.EndsWith(".jpg") ||
                file.EndsWith(".TTF") ||
                file.EndsWith(".ttf") ||
                file.EndsWith(".asset") ||
                file.EndsWith(".xml")
                )
                myFileList.Add(file.Replace('\\', '/'));
        }

        var directorys = Directory.GetDirectories(root);
        foreach (var dir in directorys)
            myFileList.AddRange(GetFiles(dir));

        return myFileList;
    }

    [MenuItem("5.x Pack/PackAll")]
    public static void PackAll()
    {
        //DateTime time = DateTime.Now;
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Debug.LogWarning("[PROFILE][PACKALL] Start pack ui ");
        string outPutPath = Application.dataPath + "/../AssetBundles/UI/";

        if (!Directory.Exists(outPutPath))
            Directory.CreateDirectory(outPutPath);

        BuildPipeline.BuildAssetBundles(outPutPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        //Debug.LogWarning("UI打包完成：" + DateTime.Now.ToShortTimeString());
        Debug.LogWarning(string.Format("[PROFILE][PACKALL] pack ui {0}s", sw.Elapsed.TotalSeconds));

        //    int totalNow = DateTime.Now.Hour * 360 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        //    int totalBefore = time.Hour * 360 + time.Minute * 60 + time.Second;

        //    int usedTime = totalNow - totalBefore;

        //    int hour = usedTime / 360;
        //    int min = (usedTime - hour * 360) / 60;
        //    int sec = usedTime - hour * 360 - min * 60;
        //    Debug.LogWarning(string.Format("UI打包总耗时：{0}小时{1}分钟{2}秒",hour,min,sec));
    }
}