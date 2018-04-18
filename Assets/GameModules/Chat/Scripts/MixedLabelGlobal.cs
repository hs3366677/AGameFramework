///All Configurations go here
///This file defines all the interface for external use

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MixedLabelGlobal{
    public static IChatFactory emojiFactory;

    public static void Init(string emojiBundlePath, IChatFactory factory){
        MixedLabelUtil.assetBundlePath = emojiBundlePath;
        emojiFactory = factory;
        MixedLabelUtil.LoadEmojiBundle();
    }
}

public enum ChatCreationType{
    Emoji,
    Hyperlink,
    Image
}

public interface IChatFactory{
    GameObject CreateObj(ChatCreationType type);
    Sprite CreateSprite(string spriteName);

    void Dispose(GameObject obj);

}