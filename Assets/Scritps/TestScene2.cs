using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaFramework;

public class TestScene2 : MonoBehaviour {

    LuaManager mLuaManager;

	// Use this for initialization
    void Start()
    {
        MixedLabelGlobal.Init(
            Application.dataPath + "/../AssetBundles/StandaloneWindows/emoji",
            createAction,
            disposeAction);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartLuaScript()
    {
        //AssetBundle ab = AssetBundle.LoadFromFile(AppConst.ConcatPath(Application.dataPath, "../AssetBundles_Standalone/testprotocolpanel"));
        //AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest") ;
        LuaManager mLuaManager = gameObject.AddComponent<LuaManager>();

        mLuaManager.InitStart();
        mLuaManager.DoFile("TestMixLabel");
        
    }

    GameObject createAction(ChatCreationType creationType)
    {
        GameObject obj = null;
        switch (creationType)
        {
            case ChatCreationType.Emoji:
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/GameModules/Chat/Prefabs/Emoji.prefab", typeof(GameObject)) as GameObject;
                break;
            case ChatCreationType.Hyperlink:
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/GameModules/Chat/Prefabs/HyperLink.prefab", typeof(GameObject)) as GameObject;
                break;
        }
        if (obj != null)
            return Instantiate(obj) as GameObject;
        else
            return null;
    }
    void disposeAction(ChatCreationType creationType)
    {
        return;
    }

}
