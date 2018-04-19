using LuaFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HyperLink : MonoBehaviour
{
    TextSettings mTextSetting;

    List<Text> textLinks = new List<Text>();

    Action<int> clickAction;

    int code;

    void AssignPointerActions(Text text)
    {
        EventTriggerListener listener = text.GetComponent<EventTriggerListener>();
        if (listener != null)
        {
            listener.onPointerEnter += OnHoverIn;
            listener.onPointerExit += OnHoverOut;
            listener.onPointerDown += OnPress;
            listener.onPointerUp += OnRelease;
        }
    }

    public void Init(TextSettings _textSettings, Action<int> _clickAction, int _code)
    {
        GetComponent<RectTransform>().localPosition = Vector2.zero;
        mTextSetting = _textSettings;
        clickAction = _clickAction;
        code = _code;
    }

    public void CreateSubLink(ref Vector2 cursorPos, string content, ref float contentLength, ref float contentHeight)
    {
        GameObject go = new GameObject("line");
        go.transform.SetParent(transform);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.pivot = new Vector2(0, 1);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentLength);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        rt.localPosition = cursorPos;
        go.AddComponent<EventTriggerListener>();
        Text line = go.AddComponent<Text>();
        line.font = mTextSetting.font;
        line.fontSize = mTextSetting.fontSize;
        line.text = content;
        line.color = mTextSetting.defaultColor;
        line.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
        line.verticalOverflow = UnityEngine.VerticalWrapMode.Overflow;
        AssignPointerActions(line);
        textLinks.Add(line);


        GameObject goUnderline = new GameObject("underline");
        goUnderline.transform.SetParent(transform);
        RectTransform rtUnderline = goUnderline.AddComponent<RectTransform>();
        rtUnderline.pivot = new Vector2(0, 1);
        rtUnderline.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentLength);
        rtUnderline.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);
        rtUnderline.localPosition = cursorPos;
        Text Underline = goUnderline.AddComponent<Text>();
        Underline.font = mTextSetting.font;
        Underline.fontSize = mTextSetting.fontSize;
        int fontSize = Underline.fontSize;
        int charWidth;
        MixedLabelUtil.GetCharacterSize('_', Underline.font, ref fontSize, out charWidth);
        Underline.text = new string('_', (int)Mathf.Round(contentLength/charWidth));
        Underline.color = mTextSetting.defaultColor;
        Underline.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
        Underline.verticalOverflow = UnityEngine.VerticalWrapMode.Overflow;
        textLinks.Add(Underline);
    }

    public void OnHoverIn(GameObject go, PointerEventData eventData)
    {
        foreach (var x in textLinks)
        {
            x.color = mTextSetting.hoverColor;
        }
    }

    public void OnHoverOut(GameObject go, PointerEventData eventData)
    {
        foreach (var x in textLinks)
        {
            x.color = mTextSetting.defaultColor;
        }
    }

    public void OnPress(GameObject go, PointerEventData eventData)
    {
        foreach (var x in textLinks)
        {
            x.color = mTextSetting.pressColor;
        }
    }


    public void OnRelease(GameObject go, PointerEventData eventData)
    {
        foreach (var x in textLinks)
        {
            x.color = mTextSetting.hoverColor;
        }
        clickAction(code);
        
    }

}

public class TextSettings
{
    public Font font;
    public int fontSize;
    public FontStyle fontStyle;
    public Color defaultColor;
    public Color hoverColor;
    public Color pressColor;
}

