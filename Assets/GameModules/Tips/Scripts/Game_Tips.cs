using LuaFramework;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Tips : MonoBehaviour
{
    LuaTable m_table;
    public void InitLuaTable(LuaTable tb)
    {
        m_table = tb;
    }

    public void OnClickConfrim()
    {
        Util.CallMethod("GameTips", "OnClickConfrim", m_table);
    }

    public void OnClickCancel()
    {
        Util.CallMethod("GameTips", "OnClickCancel", m_table);
    }

    public void OnClickClose()
    {
        Util.CallMethod("GameTips", "OnClickClose", m_table);
    }
}