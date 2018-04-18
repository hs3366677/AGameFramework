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
            new ChatFactory(this));

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartLuaScript()
    {
        //AssetBundle ab = AssetBundle.LoadFromFile(AppConst.ConcatPath(Application.dataPath, "../AssetBundles_Standalone/testprotocolpanel"));
        //AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest") ;
        //LuaManager mLuaManager = gameObject.AddComponent<LuaManager>();

        //mLuaManager.InitStart();
        //mLuaManager.DoFile("TestMixLabel");

        //string weirdString = "TgTgTgTgTgTgTg[e-1][e-2]TgTgTgTg[e-1][e-2]TgTg[e-1][e-2]";
        string weirdString = "我是English Test Generic File[e-1][e-2]一段[h-超链接文字超链接文字超链接文字超链接文字超链接文字_DoAction]，一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]，外加一张[i-aaa]和一张[i-bbb]";

        GameObject mObj = LuaHelper.CreateInstance("Assets/GameModules/Chat/Prefabs/GameObject.prefab");
        mObj.transform.parent = LuaHelper.GetActiveCanvasParent();
        MixedLabel mixedLabel = mObj.GetComponent<MixedLabel>();


        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        mixedLabel.Init(weirdString, 100);

        sw.Stop();
        Debug.LogFormat("request time = {0}", sw.Elapsed.TotalMilliseconds);
        
    }

}

class ChatFactory : IChatFactory
{
    MonoBehaviour context;
    public ChatFactory(MonoBehaviour _context)
    {
        context = _context;
    }
    public GameObject CreateObj(ChatCreationType creationType)
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
            return Object.Instantiate(obj) as GameObject;
        else
            return null;
    }

    public Sprite CreateSprite(string spriteName)
    {
        Texture2D result = MixedLabelUtil.s_emojiBundle.LoadAsset<Texture2D>("assets/gamemodules/chat/textures/" + spriteName + ".png");
        Sprite sp = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.zero);
        if (result == null){
            Debug.LogFormat("{0} does not exist in assetbundle", spriteName);
            return null;
        }
        else
            return sp;
    }
    public void Dispose(GameObject obj)
    {
        Object.Destroy(obj);
        return;
    }

}
