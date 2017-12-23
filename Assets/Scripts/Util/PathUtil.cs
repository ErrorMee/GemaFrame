using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    private static string _PatchPath = Application.persistentDataPath + "/patchs/";
    private static string StreamingPatchPath = Application.streamingAssetsPath + "/patchs/";
    /// <summary>
    /// 热更目录 
    /// http://blog.csdn.net/ynnmnm/article/details/52253674
    /// IOS Application.persistentDataPath:/var/mobile/Containers/Data/Application/app sandbox/Documents
    /// AND Application.persistentDataPath:/storage/emulated/0/Android/data/package name/files
    /// WIN Application.persistentDataPath:C:\Users\username\AppData\LocalLow\company name\product name
    /// </summary>
    public static string PatchPath
    {
        get
        {
            if (!Directory.Exists(_PatchPath))
            {
                Directory.CreateDirectory(_PatchPath);
            }
            return _PatchPath;
        }
    }

    /// <summary>
    /// appPath
    /// </summary>
    public static string AppDataPath
    {
        get
        {
            return Application.dataPath;
        }
    }

    /// <summary>
    /// Streamingassets目录
    /// IOS Application.dataPath + "/Raw";
    /// AND Application.dataPath + "!/assets";
    /// WIN Application.dataPath + "/Streamingassets";
    /// </summary>
    public static string StreamingassetsPath
    {
        get
        {
            return Application.streamingAssetsPath;
        }
    }

    public static string RemotePatchFilesPath()
    {
        return Game.Instance.gameSetting.GetPatchRootPath() + "patchfiles.txt";
    }

    /// <summary>
    /// 本地热更位置的 patchfiles
    /// </summary>
    /// <returns></returns>
    public static string LocalPatchFilesPath()
    {
        return PatchPath + "patchfiles.txt";
    }

    public static string StreamingPatchFilesPath()
    {
        return StreamingPatchPath + "patchfiles.txt";
    }

    /// <summary>
    /// 根据绝对路径定位是热更位置还是streamingasset位置
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetAssetPath(string relativePath)
    {
#if UNITY_EDITOR
        if (!AssetManager.bundleLoadMode)//编辑器下的非Bundle加载模式
        {
            return relativePath;
        }
#endif
        
        if (Game.Instance.gameSetting.patchOpen)
        {
            string patchPath = PathUtil.PatchPath + relativePath;
            if (File.Exists(patchPath))
            {
                return patchPath;
            }
            return StreamingPatchPath + relativePath;
        }
        else
        {
            return StreamingPatchPath + relativePath;
        }

    }
}
