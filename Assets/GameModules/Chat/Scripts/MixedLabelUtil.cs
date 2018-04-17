using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MixedLabelUtil
{
    public const string eChunk = "e-";
    public const string iChunk = "i-";
    public const string hChunk = "h-";
    /// <summary>
    /// 表情占用width,height固定大小
    /// </summary>
    public static int s_emojiSize = 24;
    public static int widthChinese = 15;

    public static string assetBundlePath = Application.dataPath + "/../AssetBundles/StandaloneWindows/emoji";
    public static AssetBundle s_emojiBundle;

    /// <summary>
    /// 正则表达，获取聊天特殊转移符号
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static List<string> RegexMatch(string str)
    {
        List<string> regexStr = new List<string>();
        Regex reg = new Regex(@"\[[ehi]\-\w+\]");
        var match = reg.Matches(str);

        for (int i = 0; i < match.Count; i++)
            regexStr.Add(match[i].Value);

        return regexStr;
    }



    //public static Dictionary<FontInfo, int> lookupTable = new Dictionary<FontInfo, int>();
    /// <summary>
    /// 获取字符宽度
    /// </summary>
    /// <param name="ch"></param>
    /// <param name="font"></param>
    /// <param name="fontSize"></param>
    /// <returns></returns>
    public static void GetCharacterSize(char ch, Font font, ref int fontSize, out int width, out int height, FontStyle style = FontStyle.Normal)
    {
        height = 0;
        CharacterInfo info;

        //if (IsChinese(ch))
        //{
        //    FontInfo mFontInfo = new FontInfo() { fontName = font.name, fontSize = fontSize };
        //    lookupTable.TryGetValue(mFontInfo, out width);
        //    if (width != 0)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        //Stopwatch sw = new Stopwatch();
        //        //sw.Start();
        //        font.RequestCharactersInTexture(ch.ToString(), fontSize, style);
        //        font.GetCharacterInfo(ch, out info, fontSize, style);
        //        width = info.glyphWidth;
        //        height = info.glyphHeight;
        //        lookupTable.Add(mFontInfo, width);
        //        //sw.Stop();
        //        //Debug.LogFormat("'{0}' width = {1}; Space height = {2}; request time = {3}", ch, width, height, sw.Elapsed.TotalMilliseconds);
                
        //    }
        //}
        //else
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            font.RequestCharactersInTexture(ch.ToString(), fontSize, style);
            font.GetCharacterInfo(ch, out info, fontSize, style);
            width = info.glyphWidth;
            height = info.glyphHeight;
            //sw.Stop();
            //Debug.LogFormat("'{0}' width = {1}; Space height = {2}; request time = {3}", ch, width, height, sw.Elapsed.TotalMilliseconds);
        }
        return;
    }

    public static bool IsChinese(char ch)
    {
        return ch >= 0x4E00 && ch <= 0x9FA5;
    }



    public static string[] GetHtmlInfo(string str)
    {
        str = str.Substring(2, str.Length - 2);
        return str.Split('_');
    }

    /// <summary>
    /// 这段代码只是为了测试方便用的，将来等资源管理器写完以后要去除
    /// </summary>
    public static void LoadEmojiBundle()
    {
        if (s_emojiBundle == null)
            s_emojiBundle = AssetBundle.LoadFromFile(assetBundlePath);
    }

    public static bool SpriteContains(string sprName)
    {
        if (s_emojiBundle != null)
            return s_emojiBundle.Contains(sprName);

        return false;
    }

    public static Sprite GetSpriteByName(string sprName)
    {
        return MixedLabelGlobal.emojiFactory.CreateSprite(sprName);
    }
}

public class FontInfo
{
    public string fontName;
    public int fontSize;
    public int width;
    public int height;
    public override bool Equals(object obj)
    {
        if (obj.GetType() == this.GetType()){
            return fontName == ((FontInfo)obj).fontName && fontSize == ((FontInfo)obj).fontSize;
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return fontName.GetHashCode() ^ fontSize.GetHashCode();
    }

}