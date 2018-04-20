///All Configurations go here
///This file defines all the interface for external use

using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// always initialize this class before use 
/// </summary>
public static class MixedLabelGlobal{
    public static IChatFactory emojiFactory;


    public static void Init(string emojiBundlePath, IChatFactory factory){
        MixedLabelUtil.assetBundlePath = emojiBundlePath;
        emojiFactory = factory;
        MixedLabelUtil.LoadEmojiBundle();
    }

    public static MixedLabel CreateMixedLabel()
    {
        GameObject obj = emojiFactory.CreateObj(ChatCreationType.Label);
        MixedLabel mixedLabel = obj.GetComponent<MixedLabel>();
        if (mixedLabel == null)
            Debug.LogError("The factory cannot create a right mixedlabel");
        return mixedLabel;
    }
}

public enum ChatCreationType{
    Label,
    Emoji,
    Hyperlink
}

public class TextSettings
{
    public Font font;
    public int fontSize;
    public FontStyle fontStyle;
    public int lineSpace;
    public Color textColor;
    public Color hyperDefaultColor;
    public Color hyperHoverColor;
    public Color hyperPressColor;
}

/// <summary>
/// user need to implement this factory; don't simply create and destroy elements because they can 
/// be recycled for further use in a typical chat scenario; use a pool
/// </summary>
public interface IChatFactory{
    GameObject CreateObj(ChatCreationType type);
    Sprite CreateSprite(string spriteName);

    void Dispose(GameObject obj);

}