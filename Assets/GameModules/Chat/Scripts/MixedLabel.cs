using LuaFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MixedLabel : MonoBehaviour, IDisposable
{
    List<MotionEmoji> emojiList;
    List<HyperLink> hyperlinkList;

    Text mText;
    RectTransform mRectTransform;
    Vector2 cursorPos; // the cursor always located at the left-top corner
    int fontSize;
    Font font;
    float lineHeight;
    float lineSpace;
    float lineCount;
    int emojiSpaceCount;
    int spaceCharWidth;
    StringBuilder mStringBuilder;
    StringBuilder mEscapeBuilder;
    Stack<char> escapeStack;

    int totalWidth;
    int totalHeight;
    int hyperCount;

    TextSettings mTextSettings;
    void Awake()
    {
        mText = GetComponent<Text>();
        mTextSettings = new TextSettings();
        mRectTransform = GetComponent<RectTransform>();
        if (mText == null)
        {
            mText = gameObject.AddComponent<Text>();
        }
    }

    /// <summary>
    /// Reset all parameter before reuse and initialization
    /// </summary>
    void Reset()
    {
        mEscapeBuilder = new StringBuilder();
        mStringBuilder = new StringBuilder();
        cursorPos = Vector2.zero;
        fontSize = mText.fontSize;
        font = mText.font;
        lineSpace = mText.lineSpacing;
        lineCount = 1;
        GetLineHeight();
        MixedLabelUtil.GetCharacterSize(' ', font, ref fontSize, out spaceCharWidth);
        emojiSpaceCount = (int)Mathf.Round(MixedLabelUtil.s_emojiSize / spaceCharWidth);
        totalWidth = 0;
        totalHeight = 0;
        hyperCount = 0;
        mTextSettings.font = font;
        mTextSettings.fontSize = fontSize;
        mTextSettings.fontStyle = mText.fontStyle;
        mTextSettings.hoverColor = Color.red;// mText.color;
        mTextSettings.defaultColor = Color.blue;// *0.5f;
        mTextSettings.pressColor = Color.green;// *1.3f;

    }

    public void Init(string str, int maxWidth, params Action<int>[] hyperLinkActions)
    {
        Reset();
        int tmpCharWidth = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == '[')
            {
                mEscapeBuilder = new StringBuilder();
                i++;
                while (i < str.Length)
                {
                    if (str[i] != ']')
                        mEscapeBuilder.Append(str[i]);
                    else
                        break;
                    i++;
                }

                string escapeWord = mEscapeBuilder.ToString();
                if (StringUtil.StartsWith(escapeWord, MixedLabelUtil.eChunk))
                {
                    //exceed lineMaxWidth
                    if (cursorPos.x + emojiSpaceCount * spaceCharWidth > maxWidth)
                        ChangeLine(mStringBuilder, ref cursorPos);

                    //空格填充
                    mStringBuilder.Append(' ', emojiSpaceCount);
                    int emojiId = int.Parse(escapeWord.Substring(2, escapeWord.Length - 2));
                    CreateEmoji(ref emojiId, ref cursorPos);
                    cursorPos.x += emojiSpaceCount * spaceCharWidth;
                }
                else if (StringUtil.StartsWith(escapeWord, MixedLabelUtil.iChunk))
                {
                    //TO-DO
                }
                else if (StringUtil.StartsWith(escapeWord, MixedLabelUtil.hChunk))
                {
                    string linkText;
                    int linkNumber;
                    MixedLabelUtil.GetHtmlInfo(escapeWord, out linkText, out linkNumber);


                    HyperLink hyperLink;
                    if (hyperLinkActions != null && hyperLinkActions.Length > hyperCount)
                        hyperLink = CreateLink(hyperLinkActions[hyperCount], mTextSettings, linkNumber);
                    else
                        hyperLink = CreateLink(null, mTextSettings, linkNumber);
                    hyperCount++;

                    float linkTextWidth = 0;
                    string currentLinkText = "";
                    for (int j = 0; j < linkText.Length; j++)
                    {
                        MixedLabelUtil.GetCharacterSize(linkText[j], mText.font, ref fontSize, out tmpCharWidth);

                        if (cursorPos.x + linkTextWidth + tmpCharWidth > maxWidth)
                        {
                            mStringBuilder.Append(' ', (int)Mathf.Round(linkTextWidth / spaceCharWidth));

                            hyperLink.CreateSubLink(ref cursorPos, currentLinkText, ref linkTextWidth, ref lineHeight);

                            //换行
                            ChangeLine(mStringBuilder, ref cursorPos);
                            currentLinkText = linkText[j].ToString();
                            linkTextWidth = tmpCharWidth;
                        }
                        else
                        {
                            linkTextWidth += tmpCharWidth;
                            currentLinkText += linkText[j];
                        }
                    }

                    mStringBuilder.Append(' ', (int)Mathf.Round(linkTextWidth / spaceCharWidth));
                    hyperLink.CreateSubLink(ref cursorPos, currentLinkText, ref linkTextWidth, ref lineHeight);
                    cursorPos.x += linkTextWidth;

                }
            }
            else
            {
                MixedLabelUtil.GetCharacterSize(str[i], font, ref fontSize, out tmpCharWidth);
                if(cursorPos.x + tmpCharWidth > maxWidth)
                    ChangeLine(mStringBuilder, ref cursorPos);
                cursorPos.x += tmpCharWidth;
                mStringBuilder.Append(str[i]);
            }
        }
        mText.text = mStringBuilder.ToString();
        totalWidth = maxWidth;
        totalHeight = (int)Math.Round(lineCount * lineHeight);

        mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    public void Dispose(){
        
    }

    void GetLineHeight()
    {
        TextGenerator textGenenerator = new TextGenerator();
        TextGenerationSettings generationSettings =
        mText.GetGenerationSettings(mText.rectTransform.rect.size);
        lineHeight = textGenenerator.GetPreferredHeight("A", generationSettings);

        Debug.Log("Calculated Line Height is " + lineHeight + " " + font.ascent);
    }
    void ChangeLine(StringBuilder stringBuilder ,ref Vector2 widgetPos)
    {
        stringBuilder.Append('\n');
        widgetPos.x = 0;
        if (lineCount == 1)
            widgetPos.y -= lineHeight;
        else
            widgetPos.y -= lineSpace * lineHeight;
        lineCount++;
    }

    MotionEmoji CreateEmoji(ref int id ,ref Vector2 pos)
    {
        GameObject obj = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Emoji);
        MotionEmoji newEmoji = obj.GetComponent<MotionEmoji>();
        newEmoji.transform.SetParent(mText.transform);
        newEmoji.transform.localScale = Vector3.one;
        newEmoji.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + MixedLabelUtil.s_emojiSize / 2, pos.y - lineHeight/2);
        newEmoji.gameObject.SetActive(true);
        newEmoji.Init(id);
        if (emojiList == null)
            emojiList = new List<MotionEmoji>();
        emojiList.Add(newEmoji);
        return newEmoji;
    }

    HyperLink CreateLink(Action<int> clickCallback, TextSettings settings, int code)
    {
        HyperLink newLink = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Hyperlink).GetComponent<HyperLink>();
        newLink.transform.SetParent(mText.transform);
        newLink.transform.localScale = Vector3.one;
        newLink.gameObject.SetActive(true);
        //newLink.InitText(linkContent, linkAction);
        if (hyperlinkList == null)
            hyperlinkList = new List<HyperLink>();
        hyperlinkList.Add(newLink);
        newLink.Init(settings, clickCallback, code);
        return newLink;
    }
}