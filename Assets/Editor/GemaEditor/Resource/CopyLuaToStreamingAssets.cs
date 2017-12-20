

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CopyLuaToStreamingAssets
{

    const string kBuildResources = GemaEditorConst.LUA + "CopyLuaToStreaming";
    /// <summary>
    ///  生成LUA资源包
    /// </summary>
    [MenuItem(kBuildResources, false, 1)]
    public static void CopyLuaToStreaming()
    {
        OnCopyLuaToStreaming();
        GeneratePatchFiles.GenerateFiles();
    }

    public static void OnCopyLuaToStreaming()
    {
        string srcPath = Application.dataPath + "/lua";
        string destPath = PathUtil.StreamingassetsPath + "/patchs/lua";

        CopyFile(srcPath, destPath, true);

        Debug.Log("Copy lua finish");
    }

    public static void CopyFile(string scrPath, string desPath, bool encrpt = false)
    {
        if (!Directory.Exists(desPath))
        {
            Directory.CreateDirectory(desPath);
        }

        Dictionary<uint, string> hashToPath = new Dictionary<uint, string>();
        
        List<string> tempFiles = FindAllFiles.ListFiles(scrPath);
        
        foreach (string file in tempFiles)
        {
            string newFile = (desPath + file.Replace(scrPath, "")).ToLower();
            string newPath = Path.GetDirectoryName(newFile).ToLower();

            if (encrpt)
            {
                string hashFile = newFile.Substring(desPath.Length + 1);
                uint hash = EncryptUtil.FileNameHash(hashFile);
                string existPath;
                if (hashToPath.TryGetValue(hash, out existPath))
                {
                    Debug.LogError("Hash collision, hash code : " + hash + ", path :" + newFile + ", exist path : " + existPath);
                    continue;
                }
                newPath = desPath + "/" + hash;
            }
            else
            {
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
            }
            
            if (File.Exists(newFile))
            {
                File.Delete(newFile);
            }

            if (encrpt)
            {
                byte[] bytes = File.ReadAllBytes(file);
                EncryptUtil.Encryption(bytes);
                File.WriteAllBytes(newPath, bytes);
            }
            else
            {
                File.Copy(file, newFile, true);
            }

        }

        AssetDatabase.Refresh();
    }
}