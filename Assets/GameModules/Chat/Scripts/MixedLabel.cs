using LuaFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MixedLabel : MonoBehaviour, IDisposable
{
    List<GameObject> emojiList;
    List<GameObject> hyperlinkList;

    Text mText;

    void Awake()
    {
        mText = GetComponent<Text>();
        if (mText == null)
        {
            mText = gameObject.AddComponent<Text>();
        }
    }

    public void Init(string str, int maxWidth)
    {
        string tempStr = "";                                                                            //最终输出的字符串
        Vector2 widgetPos = Vector2.zero;                                                               //计算位置，方便控件创建
        int chWidth = 0;
        int chHeight = 0;
        int fontSize = mText.fontSize;
        Font font = mText.font;
        MixedLabelUtil.GetCharacterSize(' ', font, ref fontSize, out chWidth, out chHeight);            //获取空格字符宽度

        Debug.LogFormat("Space width = {0}; Space height = {1}", chWidth, chHeight);
        for (int i = 0; i < str.Length; i++ )
        {
            // 发现转译符号，并且有结尾
            if(str[i] == '[' && str.Contains("]"))
            {
                int endIndex = str.IndexOf(']', i + 1);
                string speicalWord = str.Substring(i + 1, endIndex - i - 1);

                if(StringUtil.StartsWith(speicalWord, MixedLabelUtil.eChunk))//表情转译，格式为[e-编号]
                {
                    //如果表情有一半大小在最大范围内就显示在这一行，不换行了，否则就换行
                    if(widgetPos.x + MixedLabelUtil.s_emojiSize / 2 > maxWidth)
                        tempStr = ChangeLine(tempStr, ref widgetPos);

                    //空格填充
                    int spaceCount = (int)Mathf.Round(MixedLabelUtil.s_emojiSize / chWidth);
                    tempStr += new string(' ', spaceCount);

                    int emojiId = int.Parse(speicalWord.Substring(2, speicalWord.Length - 2));
                    CreateEmoji(ref emojiId, ref widgetPos);

                    widgetPos.x += spaceCount * chWidth;
                }
                else if(StringUtil.StartsWith(speicalWord, MixedLabelUtil.iChunk))//图片转译，格式为[i-路径]
                {
                    //暂未实现
                }
                else if(StringUtil.StartsWith(speicalWord, MixedLabelUtil.hChunk))//超链接转译，格式为[h-显示的连接文字_链接回调关键字] 这里将来可以拓展，填入道具ID之类的，具体字符处理可以交给lua层
                {
                    //可能存在一个链接信息过场，生成多个超链接问题
                    List<HtmlLink> links = new List<HtmlLink>();

                    //先分割字符串，获取超链接的文字和回调信息
                    string[] linkInfo = MixedLabelUtil.GetHtmlInfo(speicalWord);
                    string linkText = linkInfo[0];
                    string linkAction = linkInfo[1];

                    //遍历链接的文字信息，处理换行
                    float linkTextWidth = 0;
                    string currentLinkText = "";
                    for(int j = 0 ; j < linkText.Length ;j++)
                    {
                        MixedLabelUtil.GetCharacterSize(linkText[j], mText.font, ref fontSize, out chWidth, out chHeight);
                        linkTextWidth += chWidth;
                        currentLinkText += linkText[j];
                        if(widgetPos.x + linkTextWidth > maxWidth)
                        {
                            //注意，这里如果有换行，最好是以最大行距和当前控件位置的差来补空格，不然有可能出现空格传到下一行的问题
                            linkTextWidth = maxWidth - widgetPos.x;
                            //空格填充
                            int spaceCount = (int)Mathf.Round(linkTextWidth / chWidth);
                            tempStr += new string(' ', spaceCount);
                            //创建链接并加入列表
                            int linkCount = links.Count;
                            links.Add(CreateLink(currentLinkText, linkAction, ref widgetPos, ref linkCount));

                            //换行
                            tempStr = ChangeLine(tempStr, ref widgetPos);
                            currentLinkText = "";
                            linkTextWidth = 0;
                        }
                    }
                    //全部完成后把最后的内容都加入
                    //空格填充
                    int lastSpaceCount = (int)Mathf.Round(linkTextWidth / chWidth);
                    tempStr += new string(' ', lastSpaceCount);
                    //创建链接并加入列表
                    int tmp = links.Count;
                    links.Add(CreateLink(currentLinkText, linkAction, ref widgetPos, ref tmp));
                    widgetPos.x += linkTextWidth;

                    //链接之间的互相作用
                    for (int j = 0; j < links.Count; j++)
                        links[j].InitOtherLink(links);
                }
                i = endIndex;
            }
            else
            {
                MixedLabelUtil.GetCharacterSize(str[i], font, ref fontSize, out chWidth, out chHeight);
                widgetPos.x += chWidth;
                tempStr += str[i];
                if(widgetPos.x >= maxWidth)
                    tempStr = ChangeLine(tempStr,ref widgetPos);
            }
        }
        mText.text = tempStr;

    }

    public void Dispose(){
        
    }
    string ChangeLine(string str ,ref Vector2 widgetPos)
    {
        str += '\n';
        widgetPos.x = 0;
        widgetPos.y -= mText.lineSpacing * mText.fontSize;
        return str;
    }

    MotionEmoji CreateEmoji(ref int id ,ref Vector2 pos)
    {
        int linkCount = (int)Mathf.Round(pos.y / (mText.lineSpacing * mText.fontSize));
        GameObject obj = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Emoji);
        MotionEmoji newEmoji = obj.GetComponent<MotionEmoji>();
        newEmoji.transform.SetParent(mText.transform);
        newEmoji.transform.localScale = Vector3.one;
        newEmoji.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y + 4 * linkCount, 0);
        newEmoji.gameObject.SetActive(true);
        newEmoji.Init(id);
        if (emojiList == null)
            emojiList = new List<GameObject>();
        emojiList.Add(newEmoji.gameObject);
        return newEmoji;
    }

    HtmlLink CreateLink(string linkContent,string linkAction ,ref Vector2 pos ,ref int linkCount)
    {
        // + 4是补正，人肉调的，我也不知道为什么有这个偏差，从第二行开始有，第一行没有
        HtmlLink newLink = MixedLabelGlobal.emojiFactory.CreateObj(ChatCreationType.Hyperlink).GetComponent<HtmlLink>();
        newLink.transform.SetParent(mText.transform);
        newLink.transform.localScale = Vector3.one;
        newLink.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y - 4 * linkCount, 0); 
        newLink.gameObject.SetActive(true);
        newLink.InitText(linkContent, linkAction);
        if (hyperlinkList == null)
            hyperlinkList = new List<GameObject>();
        hyperlinkList.Add(newLink.gameObject);
        return newLink;
    }
}