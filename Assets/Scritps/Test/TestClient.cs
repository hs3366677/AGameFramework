using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaFramework;
using Protocols;
using ProtoBuf;
using FileDownload;
//using AssetFileLibrary;

public class TestClient : Base
{

    public void OnClickConnect()
    {
        NetManager.ConnectTcp("172.16.12.15", 20002);
        Invoke("SendMessageAfterOneSecond", 3f);
    }

    public void OnClickDownloadLua()
    {


        List<AssetFileInfo> assetFiles = new List<AssetFileInfo>();
        ConstructAssetFiles(assetFiles);
        DownloadManager.Instance.StartDownload(assetFiles);
    }

    void SendMessageAfterOneSecond()
    {
        //NetworkManager.SendMessage(405, new TestProtoBody() { testInt = 405, testString = "全民烨游" });
    }

    void ConstructAssetFiles(List<AssetFileInfo> assetFiles)
    {
        if (assetFiles == null)
            assetFiles = new List<AssetFileInfo>();

        AssetFileInfo luaScript = new AssetFileInfo()
        {
            m_fileName = "luaTestScript",
            m_filePath = "Assetbundle/Lua/",
            m_size = 0,
            IsDownLoadFinish = false,
            m_md5 = "92195919502149",
            m_filePriority = DownLoadPriority.High
        };

        AssetFileInfo adhocUI = new AssetFileInfo()
        {
            m_fileName = "connectUI",
            m_filePath = "Assetbundle/UI/",
            m_size = 0,
            IsDownLoadFinish = false,
            m_md5 = "92195919502149",
            m_filePriority = DownLoadPriority.High
        };

        AssetFileInfo uiTexture = new AssetFileInfo()
        {
            m_fileName = "uiTexture",
            m_filePath = "Assetbundle/Texture/",
            m_size = 0,
            IsDownLoadFinish = false,
            m_md5 = "92195919502149",
            m_filePriority = DownLoadPriority.High
        };

        assetFiles.Add(luaScript);
        assetFiles.Add(adhocUI);
        assetFiles.Add(uiTexture);
    }
}

[ProtoContract]
public class TestProtoBody : ProtoBody
{
    [ProtoMember(1)]
    public Int32 testInt;

    [ProtoMember(2)]
    public string testString;
}
