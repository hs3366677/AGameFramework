using UnityEngine;
using UnityEngine.U2D;


public class AtlasLoader : MonoBehaviour
{
    //void OnEnable()
    //{
    //    SpriteAtlasManager.atlasRequested += RequestAtlas;
    //}

    //void OnDisable()
    //{
    //    SpriteAtlasManager.atlasRequested -= RequestAtlas;
    //}

    void Start()
    {
        AssetBundle currentSelectedBundle = AssetBundle.LoadFromFile(Application.dataPath + "/../AssetBundles_Standalone/abtag20");

        Object[] objs = currentSelectedBundle.LoadAllAssets();

        foreach (var x in objs)
        {
            Object.Instantiate(x);
        }

    }

    //void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
    //{
    //    var sa = Resources.Load<SpriteAtlas>(tag);
    //    callback(sa);
    //}
}