using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.ProjectWindowCallback;

public class Utilities {

    // Add a new menu item that is accessed by right-clicking inside the RigidBody component

    [MenuItem("Assets/Create/LuaScript", false, 110)]
    public static void CreateLuaScript()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<CreateScriptAssetAction>(),
            GetSelectedPathOrFallback() + "/NewLuaScript.lua",
            null,
            "Assets/LuaFramework/Editor/Template/LuaTemplate.lua");
    }
    
    /// <summary>
    /// </summary>
    /// <returns>path starts with "Assets/"</returns>
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)){
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path)){
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    public static string NormalizePath(string path)
    {
        if (path.StartsWith("Assets"))
        {
            return Path.GetFullPath(Application.dataPath + "/../" + path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                   .ToUpperInvariant();
        }else
            return Path.GetFullPath(new Uri(path).LocalPath)
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                   .ToUpperInvariant();
    }
}

class CreateScriptAssetAction : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile){
        //创建资源
        UnityEngine.Object obj = CreateAssetFromTemplate(pathName, resourceFile);
        //高亮显示该资源
        ProjectWindowUtil.ShowCreatedAsset(obj);
    }
    internal static UnityEngine.Object CreateAssetFromTemplate(string pathName, string resourceFile){
        //获取要创建的资源的绝对路径
        string fullName = Path.GetFullPath(pathName);
        //读取本地模板文件
        StreamReader reader = new StreamReader(resourceFile);
        string content = reader.ReadToEnd();
        reader.Close();

        //获取资源的文件名
        // string fileName = Path.GetFileNameWithoutExtension(pahtName);
        //替换默认的文件名
        content = content.Replace("#TIME", "--created at " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        string fileName = Path.GetFileNameWithoutExtension(pathName);
        content = content.Replace("#NAME", fileName);
        //写入新文件
        StreamWriter writer = new StreamWriter(fullName, false, System.Text.Encoding.UTF8);
        writer.Write(content);
        writer.Close();

        //刷新本地资源
        AssetDatabase.ImportAsset(pathName);
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }
}
