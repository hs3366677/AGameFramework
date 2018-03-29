using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaFramework;

public class TestScene2 : MonoBehaviour {

    LuaManager mLuaManager;

	// Use this for initialization
    void Start()
    {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartLuaScript()
    {


        LuaManager mLuaManager = gameObject.AddComponent<LuaManager>();

        mLuaManager.InitStart();
        mLuaManager.DoFile("TestLua");
    }
}
