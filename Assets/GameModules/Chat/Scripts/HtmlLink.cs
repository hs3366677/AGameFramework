using LuaFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HtmlLink : MonoBehaviour
{
    public Color m_defaultColor = Color.blue;
    public Color m_pressColor = Color.yellow;

    Text m_selfText;                                        //文字
    Text m_selfLine;                                        //下划线
    List<HtmlLink> m_otherLink = new List<HtmlLink>();      //其他超链接（用于处理换行情况）
    //Action m_clickAction;                                 //用于处理点击事件的代理
    string m_linkAction;

    void Awake()
    {
        LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        //luaMgr.DoFile("LinkDelegate/LinkDelegate.lua");

        m_selfText = GetComponent<Text>();
        m_selfText.color = m_defaultColor;
    }

    public void InitText(string str , string action)
    {
        m_selfText.text = str;
        m_selfLine = CreateLink(str, m_selfText, m_defaultColor);
        m_linkAction = action;
    }

    public void InitOtherLink(List<HtmlLink> link)
    {
        m_otherLink = new List<HtmlLink>(link);
        m_otherLink.Remove(this);
    }

    public void OnPress()
    {
        ChangeColor(m_pressColor);

        for (int i = 0; i < m_otherLink.Count; i++)
            m_otherLink[i].ChangeColor(m_pressColor);
    }

    public void OnRelease()
    {
        ChangeColor(m_defaultColor);

        for (int i = 0; i < m_otherLink.Count; i++)
            m_otherLink[i].ChangeColor(m_defaultColor);

        Util.CallMethod("LinkDelegate", "OnAction", m_linkAction);
    }

    public void ChangeColor(Color col)
    {
        m_selfText.color = col;
        if (m_selfLine != null)
            m_selfLine.color = col;
    }

    static Text CreateLink(string str,Text text , Color defaultColor)
    {
        GameObject go = new GameObject("line");
	    go.transform.SetParent(text.transform);
	    Text line = go.AddComponent<Text>();
	    line.font = text.font;
	    line.fontSize = text.fontSize;
	    line.text = "_";
	    float perWidth = line.preferredWidth;
	    float textPerWidth = text.preferredWidth;
	    int count = (int)Mathf.Round(textPerWidth / perWidth);

	    line.text = new string('_',count);
	    line.color = defaultColor;
	    line.horizontalOverflow = UnityEngine.HorizontalWrapMode.Overflow;
	    line.verticalOverflow = UnityEngine.VerticalWrapMode.Overflow;
	
	    RectTransform rt = line.rectTransform;
	    rt.anchoredPosition3D = Vector3.zero;  
	    rt.offsetMax = Vector2.zero;
	    rt.offsetMin = Vector2.zero;
	    rt.anchorMax = Vector2.one; 
	    rt.anchorMin = Vector2.zero;

        return line;
    }
}