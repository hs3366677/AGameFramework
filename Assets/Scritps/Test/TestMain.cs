using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaFramework;

public class TestMain : MonoBehaviour {



    //= new GeneralGameSetting
    //{
    //    DownloadEnabled = true,
    //    ExternFileRootPath = Application.dataPath,
    //    FileServerAddrUsage1 = "http://172.16.12.200/test/",
    //    FileServerAddrUsage2 = "http://172.16.12.200/test/"

    //};

    void Awake()
    {
        
    }

    void Start()
    {
        AppFacade.Instance.StartUp();   //启动游戏
    }
}
