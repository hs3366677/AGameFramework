using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringUtil{

    /// <summary>
    /// Use this performance-improved method instead of the default String.StartsWith() 
    /// </summary>
    /// <param name="content"></param>
    /// <param name="value"></param>
    /// <returns></returns>
	public static bool EndsWith(string content, string value)
    {
        int ap = content.Length - 1;
        int bp = value.Length - 1;
        while (ap >= 0 && bp >= 0 && content[ap] == value[bp])
        {
            ap--;
            bp--;
        }
        return (bp < 0 && content.Length >= value.Length);
    }

    public static bool StartsWith(string content, string value)
    {
        int aLen = content.Length;
        int bLen = value.Length;
        int ap = 0; int bp = 0;
        while (ap < aLen && bp < bLen && content[ap] == value[bp])
        {
            ap++;
            bp++;
        }
        return (bp == bLen && aLen >= bLen);
    }
}
