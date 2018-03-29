using ProtoBuf;
using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CsToProtoEditor : MonoBehaviour 
{

    public enum SwitchType
    {
        TestProtoBody = 1,
    }

    [MenuItem("Lua/CsToProto")]
    static void CsToProto()
    {
        foreach (SwitchType item in Enum.GetValues(typeof(SwitchType)))
        {
            Creation(item);
        }

        Packager.BuildProtobufFile();  ///将转成成功的.proto文件转成.lua文件
    }

    static void Creation(SwitchType t)
    {

        string content = GetContent(t);

        string path = Application.dataPath.ToLower() + "/LuaFramework/lua/3rd/pblua/" + Enum.GetName(typeof(SwitchType),t);
        //*.proto不存在这个文档的话新建。  
        if (!File.Exists(path+".proto"))
        {
            StreamWriter strmsave = new StreamWriter(path + ".proto", false, System.Text.Encoding.Default);
            strmsave.Write(content);
            strmsave.Close();

        }
        //*.proto存在这个文档的话直接打开写入。  
        else
        {
            StreamWriter strmsave = new StreamWriter(path + ".proto", false, System.Text.Encoding.Default);
            strmsave.Write(content);
            strmsave.Close();
        }
    }

    public static string GetContent(SwitchType _type)
    {
        string str = "";
        switch (_type)
        {
            case SwitchType.TestProtoBody:
                str =  Serializer.GetProto<TestProtoBody>();
                break;
        }
        return str;
    }
}
