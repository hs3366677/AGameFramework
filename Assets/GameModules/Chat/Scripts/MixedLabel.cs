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
    List<GameObject> emojiList;
    List<GameObject> hyperlinkList;

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
    void Awake()
    {
        mText = GetComponent<Text>();
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
    }

    public void Init(string str, int maxWidth)
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
                if (StringUtil.StartsWith(escapeWord, MixedLabelUtil.eChunk))//表情转译，格式为[e-编号]
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
                else if (StringUtil.StartsWith(escapeWord, MixedLabelUtil.iChunk))//图片转译，格式为[i-路径]
                {
                    //暂未实现
                }
                else if (StringUtil.StartsWith(escapeWord, MixedLabelUtil.hChunk))//超链接转译，格式为[h-显示的连接文字_链接回调关键字] 这里将来可以拓展，填入道具ID之类的，具体字符处理可以交给lua层
                {
                    //可能存在一个链接信息过场，生成多个超链接问题
                    List<HtmlLink> links = new List<HtmlLink>();

                    //先分割字符串，获取超链接的文字和回调信息
                    string[] linkInfo = MixedLabelUtil.GetHtmlInfo(escapeWord);
                    string linkText = linkInfo[0];
                    string linkAction = linkInfo[1];

                    //遍历链接的文字信息，处理换行
                    float linkTextWidth = 0;
                    string currentLinkText = "";
                    for (int j = 0; j < linkText.Length; j++)
                    {
                        MixedLabelUtil.GetCharacterSize(linkText[j], mText.font, ref fontSize, out tmpCharWidth);
                        linkTextWidth += tmpCharWidth;
                        currentLinkText += linkText[j];
                        if (cursorPos.x + linkTextWidth > maxWidth)
                        {
                            //注意，这里如果有换行，最好是以最大行距和当前控件位置的差来补空格，不然有可能出现空格传到下一行的问题
                            linkTextWidth = maxWidth - cursorPos.x;
                            //空格填充
                            int spaceCount = (int)Mathf.Round(linkTextWidth / tmpCharWidth);
                            mStringBuilder.Append(' ', spaceCount);
                            //创建链接并加入列表
                            int linkCount = links.Count;
                            links.Add(CreateLink(currentLinkText, linkAction, ref cursorPos, ref linkCount));

                            //换行
                            ChangeLine(mStringBuilder, ref cursorPos);
                            currentLinkText = "";
                            linkTextWidth = 0;
                        }
                    }
                    //全部完成后把最后的内容都加入
                    //空格填充
                    int lastSpaceCount = (int)Mathf.Round(linkTextWidth / tmpCharWidth);
                    mStringBuilder.Append(' ', lastSpaceCount);
                    //创建链接并加入列表
                    int tmp = links.Count;
                    links.Add(CreateLink(currentLinkText, linkAction, ref cursorPos, ref tmp));
                    cursorPos.x += linkTextWidth;

                    //链接之间的互相作用
                    for (int j = 0; j < links.Count; j++)
                        links[j].InitOtherLink(links);
                }
            }
            else
            {
                MixedLabelUtil.GetCharacterSize(str[i], font, ref fontSize, out tmpCharWidth);
                cursorPos.x += tmpCharWidth;
                mStringBuilder.Append(str[i]);
                if(cursorPos.x >= maxWidth)
                    ChangeLine(mStringBuilder, ref cursorPos);
            }
        }
        mText.text = mStringBuilder.ToString();
        totalWidth = maxWidth;
        totalHeight = (int)Math.Round(lineCount * lineHeight);
        //mRectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
        
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
            emojiList = new List<GameObject>();
        emojiList.Add(newEmoji.gameObject);
        return newEmoji;
    }

    HtmlLink CreateLink(string linkContent,string linkAction ,ref Vector2 pos ,ref int linkCount)
    {
        HtmlLink newLink = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Hyperlink).GetComponent<HtmlLink>();
        newLink.transform.SetParent(mText.transform);
        newLink.transform.localScale = Vector3.one;
        newLink.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y, 0); 
        newLink.gameObject.SetActive(true);
        newLink.InitText(linkContent, linkAction);
        if (hyperlinkList == null)
            hyperlinkList = new List<GameObject>();
        hyperlinkList.Add(newLink.gameObject);
        return newLink;
    }
}