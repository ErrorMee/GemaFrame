
using UnityEngine;

//------------------------------------------------------------
// 游戏统一处理log
//------------------------------------------------------------
public static class GLog
{
    public static bool isOpen = true;

    static public void Log(string str, bool force = false)
    {
        if (isOpen || force)
        {
            Debug.Log(str);
        }
    }

    static public void Log(string str, Color color, bool force = false)
    {
        if (isOpen || force)
        {
            Debug.Log(ColorUtil.ColorString(color, str));
        }
    }

    static public void Warning(string str, bool force = false)
    {
        if (isOpen || force)
        {
            Debug.LogWarning(str);
        }
    }

    static public void Error(string str,bool force = false)
    {
        if (isOpen || force)
        {
            Debug.LogError(str);
        }
    }
}
