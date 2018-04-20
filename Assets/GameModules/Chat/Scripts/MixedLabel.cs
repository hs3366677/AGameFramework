using LuaFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// Primary class
/// </summary>
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
    FontStyle fontStyle;
    StringBuilder mStringBuilder;
    StringBuilder mEscapeBuilder;
    Stack<char> escapeStack;

    public int totalWidth{get;set;}
    public int totalHeight{get;set;}
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
    void Reset(TextSettings textSettings)
    {
        mEscapeBuilder = new StringBuilder();
        mStringBuilder = new StringBuilder();
        cursorPos = Vector2.zero;
        font = mText.font;
        lineSpace = mText.lineSpacing;
        lineCount = 1;
        GetLineHeight();
        MixedLabelUtil.GetCharacterSize(' ', font, ref fontSize, out spaceCharWidth);
        emojiSpaceCount = (int)Mathf.Round(MixedLabelUtil.s_emojiSize / spaceCharWidth);
        totalWidth = 0;
        totalHeight = 0;
        hyperCount = 0;

        mText.color = textSettings.textColor;
        if (textSettings.font == null)
        {
            textSettings.font = mText.font;
        }
        font = textSettings.font;

        if (textSettings.fontSize == 0)
            textSettings.fontSize = mText.fontSize;
        fontSize = textSettings.fontSize;

        if (textSettings.fontStyle == 0)
            textSettings.fontStyle = mText.fontStyle;
        fontStyle = textSettings.fontStyle;


        //if (textSettings.hoverColor == null)
        //    textSettings.hoverColor = Color.red;
        //if (textSettings.defaultColor == null)
        //    mTextSettings.defaultColor = Color.blue;// *0.5f;
        //if (textSettings.pressColor == null)
        //    mTextSettings.pressColor = Color.green;// *1.3f;
        mEmojiCreateList = new List<EmojiInfo>();

    }

    /// <summary>
    /// init a mixed label;
    /// </summary>
    /// <param name="str">content should follow the rules : [e-xxx] represents emojis where xxx are the sprite name of the emoji;[h-xxx-11] represents a hyperlink where xxx are the content and 11 is reference number for callback</param>
    /// <param name="maxWidth">max width of the label, which is also the width of the label</param>
    /// <param name="hyperLinkActions">hyper link click callbacks, the number of params should be less or equal to the number of hyperlinks</param>
    public void Init(string str, int maxWidth, TextSettings _textSettings, params Action<int>[] hyperLinkActions)
    {
        mTextSettings = _textSettings;
        Reset(mTextSettings);
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
                    if (cursorPos.x + emojiSpaceCount * spaceCharWidth > maxWidth)
                        ChangeLine(mStringBuilder, ref cursorPos);

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
                            hyperLink.CreateSubLink(ref cursorPos, currentLinkText, linkTextWidth, lineHeight * lineSpace);
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
                    hyperLink.CreateSubLink(ref cursorPos, currentLinkText, linkTextWidth, lineHeight * lineSpace);
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
        if (lineCount > 1)
            totalWidth = maxWidth;
        else
            totalWidth = (int)cursorPos.x;
        totalHeight = (int)Math.Round(lineHeight + (lineCount -1) * lineSpace * lineHeight);

        mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
        mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        CreateAllEmoji();
    }

    /// <summary>
    /// dispose ui element(dont destroy, they can be recycled)
    /// </summary>
    public void Dispose(){
        foreach(var x in emojiList)
            MixedLabelGlobal.emojiFactory.Dispose(x.gameObject);
        foreach (var x in hyperlinkList)
            MixedLabelGlobal.emojiFactory.Dispose(x.gameObject);

        MixedLabelGlobal.emojiFactory.Dispose(gameObject);
    }

    void GetLineHeight()
    {
        TextGenerator textGenenerator = new TextGenerator();
        TextGenerationSettings generationSettings =
        mText.GetGenerationSettings(mText.rectTransform.rect.size);
        lineHeight = textGenenerator.GetPreferredHeight("A", generationSettings);
    }
    void ChangeLine(StringBuilder stringBuilder ,ref Vector2 widgetPos)
    {
        stringBuilder.Append('\n');
        widgetPos.x = 0;
        widgetPos.y -= lineSpace * lineHeight;
        lineCount++;
    }


    struct EmojiInfo
    {
        public int id;
        public Vector2 pos;
    }

    /// <summary>
    /// All emoji needs to be created after the text size is determined, otherwise their position would 
    /// mess up no matter what anchor is set(may be UGUI bug)
    /// </summary>
    List<EmojiInfo> mEmojiCreateList;

    void CreateEmoji(ref int _id, ref Vector2 _pos)
    {
        mEmojiCreateList.Add(new EmojiInfo { id = _id, pos = _pos });
    }
    void CreateAllEmoji()
    {
        foreach (var x in mEmojiCreateList)
        {
            GameObject obj = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Emoji);
            obj.name = "Emoji-" + x.id;
            MotionEmoji newEmoji = obj.GetComponent<MotionEmoji>();
            newEmoji.transform.SetParent(mText.transform);
            newEmoji.transform.localScale = Vector3.one;
            newEmoji.GetComponent<RectTransform>().localPosition = new Vector3(x.pos.x + MixedLabelUtil.s_emojiSize / 2, x.pos.y - lineHeight / 2);
            newEmoji.gameObject.SetActive(true);
            newEmoji.Init(x.id);
            if (emojiList == null)
                emojiList = new List<MotionEmoji>();
            emojiList.Add(newEmoji);
        }
    }

    HyperLink CreateLink(Action<int> clickCallback, TextSettings settings, int code)
    {
        HyperLink newLink = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Hyperlink).GetComponent<HyperLink>();
        newLink.name = "HyperLink-" + code;
        newLink.transform.SetParent(mText.transform);
        newLink.transform.localScale = Vector3.one;
        newLink.gameObject.SetActive(true);
        if (hyperlinkList == null)
            hyperlinkList = new List<HyperLink>();
        hyperlinkList.Add(newLink);
        newLink.Init(settings, clickCallback, code);
        return newLink;
    }
}