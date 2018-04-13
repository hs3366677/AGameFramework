using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MixedLabelGlobal{


    public delegate GameObject CreateObjAction(ChatCreationType i);
    public delegate void DisposeObjAction(ChatCreationType i);
    public static void Init(
        string emojiBundlePath,
        CreateObjAction createDelegate,
        DisposeObjAction disposeDelegate
        )
    {
        MixedLabelUtil.assetBundlePath = emojiBundlePath;
        MixedLabel.myCreateAction = createDelegate;
        MixedLabel.myDisposeAction = disposeDelegate;
    }
}

public enum ChatCreationType
{
    Emoji, 
    Hyperlink, 
    Image
}

public interface IMixedLabel
{
    void Init(string str, int maxWidth);
}

