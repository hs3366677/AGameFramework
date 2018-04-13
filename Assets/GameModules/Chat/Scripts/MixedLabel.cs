using LuaFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MixedLabel : MonoBehaviour, IMixedLabel
{
    public HtmlLink m_linkPrefab;
    public MotionEmoji m_emojiPrefab;
    public Image m_imagePrefab;

    Text m_text;

    public static MixedLabelGlobal.CreateObjAction myCreateAction;
    public static MixedLabelGlobal.DisposeObjAction myDisposeAction;

    void Awake()
    {
        //这一段以后要删除，切记！
        MixedLabelUtil.LoadEmojiBundle();

        m_text = GetComponent<Text>();
        string str = "我是[e-1][e-2]一段[h-超链接文字超链接文字超链接文字超链接文字超链接文字_DoAction]，一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]一个[e-1][e-2]，外加一张[i-aaa]和一张[i-bbb]";
        Init(str, (int)m_text.rectTransform.sizeDelta.x);
    }

    public void Init(string str,int maxWidth)
    {
        string tempStr = "";                                                                            //最终输出的字符串
        Vector2 widgetPos = Vector2.zero;                                                               //计算位置，方便控件创建
        float spaceWidth = MixedLabelUtil.GetCharacterSize(' ', m_text.font, m_text.fontSize);            //获取空格字符宽度

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
                    int spaceCount = (int)Mathf.Round(MixedLabelUtil.s_emojiSize / spaceWidth);
                    tempStr += new string(' ', spaceCount);

                    int emojiId = int.Parse(speicalWord.Substring(2, speicalWord.Length - 2));
                    CreateEmoji(ref emojiId, ref widgetPos);

                    widgetPos.x += spaceCount * spaceWidth;
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
                        float chWidth = MixedLabelUtil.GetCharacterSize(linkText[j], m_text.font, m_text.fontSize);
                        linkTextWidth += chWidth;
                        currentLinkText += linkText[j];
                        if(widgetPos.x + linkTextWidth > maxWidth)
                        {
                            //注意，这里如果有换行，最好是以最大行距和当前控件位置的差来补空格，不然有可能出现空格传到下一行的问题
                            linkTextWidth = maxWidth - widgetPos.x;
                            //空格填充
                            int spaceCount = (int)Mathf.Round(linkTextWidth / spaceWidth);
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
                    int lastSpaceCount = (int)Mathf.Round(linkTextWidth / spaceWidth);
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
                // 无转译，通常输入
                float chWidth = MixedLabelUtil.GetCharacterSize(str[i], m_text.font, m_text.fontSize);
                widgetPos.x += chWidth;
                tempStr += str[i];
                if(widgetPos.x >= maxWidth)
                    tempStr = ChangeLine(tempStr,ref widgetPos);
            }
        }
        m_text.text = tempStr;
        
    }

    string ChangeLine(string str ,ref Vector2 widgetPos)
    {
        str += '\n';
        widgetPos.x = 0;
        widgetPos.y -= m_text.lineSpacing * m_text.fontSize;
        return str;
    }

    MotionEmoji CreateEmoji(ref int id ,ref Vector2 pos)
    {
        int linkCount = (int)Mathf.Round(pos.y / (m_text.lineSpacing * m_text.fontSize));
        GameObject obj = myCreateAction(ChatCreationType.Emoji);
        MotionEmoji newEmoji = obj.GetComponent<MotionEmoji>();
        newEmoji.transform.SetParent(m_text.transform);
        newEmoji.transform.localScale = Vector3.one;
        newEmoji.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y + 4 * linkCount, 0);
        newEmoji.gameObject.SetActive(true);
        newEmoji.Init(id);
        return newEmoji;
    }

    HtmlLink CreateLink(string linkContent,string linkAction ,ref Vector2 pos ,ref int linkCount)
    {
        // + 4是补正，人肉调的，我也不知道为什么有这个偏差，从第二行开始有，第一行没有
        HtmlLink newLink = myCreateAction(ChatCreationType.Hyperlink).GetComponent<HtmlLink>();
        newLink.transform.SetParent(m_text.transform);
        newLink.transform.localScale = Vector3.one;
        newLink.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y - 4 * linkCount, 0); 
        newLink.gameObject.SetActive(true);
        newLink.InitText(linkContent, linkAction);
        return newLink;
    }
}