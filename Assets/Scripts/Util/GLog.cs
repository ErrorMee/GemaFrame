using System;
using System.IO;
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




    static public void CloseGame()
    {
        if (swLog != null)
        {
            swLog.Close();
            swLog.Dispose();
            swLog = null;
            Application.logMessageReceivedThreaded -= HandleLog;
        }
    }

    static public void SetWriteLog()
    {
        // win C:/Users/user/AppData/Local/Temp/company/project
        //android simulator mnt/media-rw/sdcard/Android/data/com.company.project/cache/
        string fullLogFolder = string.Concat(Application.temporaryCachePath, "/" + Application.productName + "/");
        if (!Directory.Exists(fullLogFolder))
        {
            Directory.CreateDirectory(fullLogFolder);
        }

        string fileName = string.Concat(DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss"),".txt");

        string fullLogPath = string.Concat(fullLogFolder, fileName);
        FileInfo fileInfo = new FileInfo(fullLogPath);
        if (fileInfo.Exists)
        {
            swLog = fileInfo.AppendText();
        } else
        {
            swLog = fileInfo.CreateText();
        }

        Application.logMessageReceivedThreaded += HandleLog;

        DelectExpiredFile(fullLogFolder);
    }

    private static void DelectExpiredFile(string fullLogFolder)
    {
        string[] logFiles = Directory.GetFiles(fullLogFolder);
        DateTime nowTime = DateTime.Now;
        DateTime tempDateTime;
        for (int i = 0; i < logFiles.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(logFiles[i]);
            if (!string.IsNullOrEmpty(fileName))
            {
                string[] dateStrings = fileName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (dateStrings.Length >= 2)
                {
                    dateStrings[0] = dateStrings[0].Replace("_", "-");
                    dateStrings[1] = dateStrings[1].Replace("_", ":");
                    fileName = string.Format("{0} {1}", dateStrings);
                    if (DateTime.TryParse(fileName, out tempDateTime))
                    {
                        if ((nowTime - tempDateTime).TotalSeconds >= ExpiredTime)
                        {
                            File.Delete(logFiles[i]);
                        }
                    }
                }
            }
        }
    }
    private static int ExpiredTime = 1800; //30分钟
    private static StreamWriter swLog;
    static private void HandleLog(string logString, string stackTrace, LogType type)
    {
        swLog.WriteLine(DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss"));
        swLog.WriteLine(type.ToString() + ":" + logString + "\n");
        if (NeedLogStackTrace(type))
        {
            swLog.WriteLine(stackTrace);
        }
        swLog.Flush();
    }

    static private bool NeedLogStackTrace(LogType type)
    {
        return LogType.Exception == type || LogType.Error == type || LogType.Assert == type || LogType.Warning == type;
    }
}
