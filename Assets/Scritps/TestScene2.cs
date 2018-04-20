//Example Use

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaFramework;

public class TestScene2 : MonoBehaviour
{

    LuaManager mLuaManager;


    public string[] displayStrings = {
                                         "This is a demo of dorochi label.HyperText and Emoji are supported.emoji demo : [e-1][e-2]hyper link demo : [h-this is a hyperText;the code is 89-89]end",
                                         "Ceci est une démo de dorochi label.HyperText et Emoji sont pris en charge.emoji démo: [e-1][e-2] démo lien hypertexte: [h-this est un hyperText, le code est 89-89] fin",
                                         "Esta es una demo de la etiqueta dorochi. HyperText y Emoji son compatibles.emoji demo: [e-1][e-2] demo de hipervínculo: [h-this es un hiperTexto; el código es 89-89] end",
                                         "这是一个dorochi标签的演示.HyperText和Emoji支持.moji演示：[e-1][e-2]超链接演示：[h-this是超文本;代码是89-89] end",
                                         "emojiデモ：[e-1][e-2]ハイパーリンクのデモ：[h-thisはハイパーテキストで、コードは89です-89] end",
                                         "이것은 dorochi 라벨의 데모입니다 .HyperText와 Emoji가 지원됩니다. emoji 데모 : [e-1][e-2] 하이퍼 링크 데모 : [h-this는 하이퍼 텍스트이고 코드는 89입니다-89] end"
                                      };
    
    // Use this for initialization
    void Start()
    {
        MixedLabelGlobal.Init(
            Application.dataPath + "/../AssetBundles/StandaloneWindows/emoji",
            new ChatFactory(this));
        mMixedLabels = new List<MixedLabel>();

        scrollViewRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<MixedLabel> mMixedLabels;

    public RectTransform scrollViewRoot;
    public int contentWidth;

    int currentYPos = -20;
    int currentDisplayStringIndex = 0;
    public void CreateLabel()
    {
        //GameObject mObj = LuaHelper.CreateInstance("Assets/GameModules/Chat/Prefabs/GameObject.prefab");
        //mObj.transform.parent = scrollViewRoot;// LuaHelper.GetActiveCanvasParent();
        //MixedLabel mixedLabel = mObj.GetComponent<MixedLabel>();

        MixedLabel mixedLabel = MixedLabelGlobal.CreateMixedLabel();

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        mixedLabel.Init(
            displayStrings[currentDisplayStringIndex % displayStrings.Length],
            contentWidth,
            new TextSettings() { textColor = Color.black, hyperDefaultColor = Color.blue, hyperHoverColor = Color.red, hyperPressColor = Color.green},
            (x) => { Debug.LogFormat("Number {0} hyper link is clicked", x); },
            (x) => { Debug.LogFormat("Number {0} hyper link is clicked", x); });
        mixedLabel.transform.parent = scrollViewRoot;
        mixedLabel.GetComponent<RectTransform>().anchoredPosition = new Vector3(20, currentYPos, 0); //20 is the horizontal padding
        mMixedLabels.Add(mixedLabel);
        currentYPos -= mixedLabel.totalHeight + 20; //20 is vertical padding
        currentDisplayStringIndex++;

        scrollViewRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -currentYPos);
        sw.Stop();
        Debug.LogFormat("request time = {0}", sw.Elapsed.TotalMilliseconds);

    }

    public void DeleteLabel()
    {
        if (mMixedLabels.Count > 0)
        {
            currentYPos += mMixedLabels[mMixedLabels.Count - 1].totalHeight + 20;
            mMixedLabels[mMixedLabels.Count - 1].Dispose();
            mMixedLabels.RemoveAt(mMixedLabels.Count - 1);
        }
    }

}

/// <summary>
/// the example factory; write your own
/// </summary>
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
            case ChatCreationType.Label:
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/GameModules/Chat/Prefabs/Label.prefab", typeof(GameObject)) as GameObject;
                break;
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
        if (result == null)
        {
            Debug.LogFormat("{0} does not exist in assetbundle", spriteName);
            return null;
        }
        else
        {
            Sprite sp = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.zero);
            return sp;
        }
    }

    /// <summary>
    /// use a object pool system instead of simple deleting them
    /// </summary>
    /// <param name="obj"></param>
    public void Dispose(GameObject obj)
    {
        Object.Destroy(obj);
        return;
    }

}
