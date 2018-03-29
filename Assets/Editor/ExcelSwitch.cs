using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelSwitch : Editor
{
    private static bool beAutoGen = false;

    [MenuItem("Tools/ExcelCreationCs")]
    public static void ExcelCreationCs()
    {
        if (!beAutoGen && EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
            return;
        }
        ReadExcelCreationCs();
    }

    /// <summary>
    /// 如果没有生成cs类的话，需要先执行Tools/ExcelCreationCs
    /// </summary>
    [MenuItem("Tools/ExcelCreationPrefab")]
    public static void ExcelCreationPrefab()
    {
        if (!beAutoGen && EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
            return;
        }
        ReadExcelCreationPrefab();

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/DelectCsFile")]
    public static void DelectCsFile()
    {
        if (!beAutoGen && EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("警告", "请等待编辑器完成编译再执行此功能", "确定");
            return;
        }
        string csPath = Application.dataPath + "/Scritps/Database/DBScritps";

        if (Directory.Exists(csPath))
        {
            string[] files = Directory.GetFiles(csPath);

            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log("Delect File" + files[i]);
                File.Delete(files[i]);
            }
        }

        CreationDBTypeCs(null);

        AssetDatabase.Refresh();
    }

    public static void ReadExcelCreationCs()
    {
        string path = Application.dataPath.Replace("Assets", "DataExcel");
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] FileInfoArr = root.GetFiles();

        List<ExcelData> eDataList = new List<ExcelData>();

        foreach (FileInfo f in FileInfoArr)
        {
            if (f.Extension == ".xlsx")
            {
                ExcelData eData = Helper.GetExcelData(f.FullName, f.Name.Replace(f.Extension, ""));
                if (eData != null)
                    eDataList.Add(eData);
            }
        }

        CreationDBTypeCs(eDataList);

        AssetDatabase.Refresh();
    }

    public static void CreationCs(ExcelData _data)
    {
        string csStyle = "";
        string attribute = "";
        string csContent = "";

        StreamReader sr = new StreamReader(Application.dataPath + "/Editor/CsStyle.txt", Encoding.Default);

        string content;
        while ((content = sr.ReadLine()) != null)
        {
            csStyle += (content + "\n");
        }

        for (int i = 0; i < _data.firstNames.Count; i++)
        {
            attribute += (string.Format("\n    public {0} {1};\n", _data.firstNames[i], _data.secondNames[i]));
        }

        csContent = string.Format(csStyle, _data.className, _data.className, _data.className, _data.className, _data.className, attribute);

        csContent = csContent.Replace("#1","{");
        csContent = csContent.Replace("#2", "}");

        string savePath = Application.dataPath + "/Scritps/Database/DBScritps/" + _data.className+"DB.cs";

        Debug.Log(csContent);
        if (!File.Exists(savePath))
        {
            StreamWriter strmsave = new StreamWriter(savePath, false, System.Text.Encoding.UTF8);
            strmsave.Write(csContent);
            strmsave.Close();

        }
        else
        {
            StreamWriter strmsave = new StreamWriter(savePath, false, System.Text.Encoding.UTF8);
            strmsave.Write(csContent);
            strmsave.Close();
        }
        Debug.Log("成功创建CS文件：" + savePath);
    }


    /// <summary>
    /// 创建prefab  db
    /// </summary>
    public static void ReadExcelCreationPrefab()
    {
        foreach (var item in DBTypeList.dbTypeList)
        {
            string csName = item.ToString().Replace("DB","");
            string path = Application.dataPath.Replace("Assets", "DataExcel") + "/" + (csName+".xlsx");

            ExcelData eData = Helper.GetExcelData(path, csName);

            GameObject go = new GameObject();

            if (eData != null)
            {
                CreationPrefab(go,eData,item);
            }
        }
    }

    public static void CreationPrefab(GameObject _ob, ExcelData _eData,Type _Type)
    {
        GameObject newPrefab = PrefabUtility.CreatePrefab("Assets/Resources/Database/" + _eData.className + "DB.prefab", _ob);

        Component cpm = newPrefab.AddComponent(_Type);
        if (cpm != null)
        {
            Debug.Log(_Type.ToString());
            AutoImportDatabase tData = cpm as AutoImportDatabase;
            tData.ImportData(_eData);
        }
    }

    /// <summary>
    /// 创建dbcs类列表，方便后面制作db
    /// </summary>
    /// <param name="_eDataList"></param>
    public static void CreationDBTypeCs(List<ExcelData> _eDataList)
    {
        string dbTypeList = "using System; \n using System.Collections.Generic;\n public static class DBTypeList \n{\n    public static List<Type> dbTypeList = new List<Type>\n    {\n ";

        if (_eDataList != null)
        {
            for (int i = 0; i < _eDataList.Count; i++)
            {
                CreationCs(_eDataList[i]);

                dbTypeList += (string.Format("\n        typeof({0}DB),", _eDataList[i].className));
            }
        }

        dbTypeList += "\n        };\n}";

        string savePath = Application.dataPath + "/Scritps/Database/DBTypeList.cs";

        if (!File.Exists(savePath))
        {
            StreamWriter strmsave = new StreamWriter(savePath, false, System.Text.Encoding.UTF8);
            strmsave.Write(dbTypeList);
            strmsave.Close();
        }
        else
        {
            StreamWriter strmsave = new StreamWriter(savePath, false, System.Text.Encoding.UTF8);
            strmsave.Write(dbTypeList);
            strmsave.Close();
        }
    }
}
