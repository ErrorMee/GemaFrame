
using UnityEngine;
using System.IO;

public static class FileHelper
{

    /// <summary>
    /// 同步加载文本
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetText(string filePath)
    {
        if (!File.Exists(filePath))
        {
            GLog.Error(string.Format("FileName ({0}) doesn't exist!", filePath));
            return null;
        }

#if UNITY_IOS
		return File.ReadAllText(filePath);
#endif
        if (Application.isMobilePlatform)
        {
            WWW www = new WWW(filePath);
            while (!www.isDone)
            {
                System.Threading.Thread.Sleep(0);
            }

            if (www.error != null)
            {
                GLog.Error(www.error, true);
            }

            int wwwlen = www.bytes.Length;
            if (wwwlen <= 0)
            {
                return null;
            }

            www.Dispose();
            return www.text;
        }
        else
        {
            return File.ReadAllText(filePath);
        }
    }
}
