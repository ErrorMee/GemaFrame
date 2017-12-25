using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetAssetBundle
{
    private static string buildAssetsPath = (Application.dataPath + "/BuildAssets").ToLower();

    private static string configPath = Application.dataPath + "/BuildAssets/Config/abConfig.json";

    private static int resourceCount = 0;
    private static Dictionary<string, List<string>> typeAssets = new Dictionary<string, List<string>>();
    private static List<string> abGroupFolders = new List<string>() { "GroupFolder" };
    private static List<string> abAloneFolders = new List<string>() { "AloneFolder" };

    private static ABConfig abConfig;

    const string kSetAssetLabels = GemaEditorConst.AssetBundle + "/SetAssetLabels";
    /// <summary>
    ///  设置所有资源的AssetLabels
    ///  默认
    ///  使用资源的相对文件夹路径作为Assetlabels
    ///  例外
    ///  abGroupFolders 组合打包 该路径下的资源使用相对文件夹路径到GroupFolder名称作为Assetlabels
    ///  abAloneFolders 独立打包 该路径下的资源使用相对文件夹路径+自身名称作为Assetlabels
    /// </summary>
    [@MenuItem(kSetAssetLabels, false, 2)]
    public static void SetAssetLabels()
    {
        resourceCount = 0;
        typeAssets = new Dictionary<string, List<string>>();

        if (File.Exists(configPath))
        {
            File.Delete(configPath);
        }

        List<string> files = FindAllFiles.ListFiles(buildAssetsPath, true);

        abConfig = new ABConfig();
        abConfig.data = new List<ABConfigInfo>();

        FileStream fs = new FileStream(configPath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        SetABName(files, true);

        string js = EditorJsonUtility.ToJson(abConfig, true);
        sw.Write(js);
        
        sw.Close(); fs.Close();

        AssetDatabase.Refresh();

        SetABName(new List<string> { configPath }, true);
    }

    const string kQuickSetAsset = GemaEditorConst.AssetBundle + "/☀ QuickSetAsset";
    [@MenuItem(kQuickSetAsset, false,1)]
    public static void QuickSetAsset()
    {
        SetAllTexture._SetAllTexture();
        SetAssetLabels();
    }

    const string kClearAssetLabels = GemaEditorConst.AssetBundle + "/ClearAssetLabels";
    [@MenuItem(kClearAssetLabels, false, 3)]
    public static void ClearAssetLabels()
    {
        resourceCount = 0;
        typeAssets = new Dictionary<string, List<string>>();
        List<string> files = FindAllFiles.ListFiles(buildAssetsPath, true);
        if (File.Exists(configPath))
        {
            File.Delete(configPath);
        }
        
        abConfig = new ABConfig();
        abConfig.data = new List<ABConfigInfo>();
        SetABName(files, false);
        AssetDatabase.Refresh();
    }

    private static void SetABName(List<string> files, bool setName = true)
    {
        GLog.Log("BuildAssets Count " + files.Count,true);
        for (int i = 0; i < files.Count; i++)
        {
            string filePath = files[i];
            string assetPath = filePath.Substring(Application.dataPath.Length - "assets".Length);
            AssetImporter ai = AssetImporter.GetAtPath(assetPath);
            GemaEditor.ShowProgress(i, files.Count, "SetAssetLabels", ai.assetPath);

            string abName = AanalyzeAssetLabels(assetPath);

            GLog.Log(string.Format("No.{0} abName:{1} assetPath:{2}", i + 1, abName, assetPath),true);
            if (setName)
            {
                ai.assetBundleName = abName;
                ai.assetBundleVariant = "ab";
            }
            else
            {
                ai.assetBundleName = null;
                //ai.assetBundleVariant = null;
            }

            string fileName = Path.GetFileName(ai.assetPath);
            fileName = fileName.Split('.')[0];
            string importerName = ai.GetType().ToString().Split('.')[1];
            string typename = importerName.Substring(0, importerName.Length - 8);

            List<string> listAsset;
            if (!typeAssets.TryGetValue(typename, out listAsset))
            {
                listAsset = new List<string>();
                typeAssets.Add(typename, listAsset);
            }

            if (listAsset.IndexOf(fileName) != -1)
            {
                GLog.Error("Repeat name " + fileName + " type " + typename, true);
                return;
            }
            listAsset.Add(fileName);
            

            ABConfigInfo abConfigInfo = abConfig.GetInfoByAB(abName);
            if (abConfigInfo == null)
            {
                abConfigInfo = new ABConfigInfo();
                abConfig.AddInfo(abConfigInfo);
                resourceCount++;
                abConfigInfo.id = resourceCount;
                abConfigInfo.ab = abName;
                abConfigInfo.type = typename;
            }
            abConfigInfo.names.Add(fileName);

        }
        EditorUtility.ClearProgressBar();
    }

    private static string AanalyzeAssetLabels(string assetPath)
    {
        int subLen = assetPath.Length - "assets/BuildAssets/".Length - Path.GetFileName(assetPath).Length - 1;

        //检查组合文件夹
        for (int i = 0; i < abGroupFolders.Count; i++)
        {
            string abExceptionFolder = abGroupFolders[i];
            if (assetPath.IndexOf(abExceptionFolder) != -1)
            {
                subLen = assetPath.IndexOf(abExceptionFolder) + abExceptionFolder.Length - "assets/BuildAssets/".Length;
                return assetPath.Substring("assets/BuildAssets/".Length, subLen).ToLower() + "_";
            }
        }

        //检查独立文件夹
        for (int i = 0; i < abAloneFolders.Count; i++)
        {
            string abExceptionFolder = abAloneFolders[i];
            if (assetPath.IndexOf(abExceptionFolder) != -1)
            {
                subLen = assetPath.Length - "assets/BuildAssets/".Length - Path.GetExtension(assetPath).Length;
                return assetPath.Substring("assets/BuildAssets/".Length, subLen).ToLower() + "_";
            }
        }
        
        return assetPath.Substring("assets/BuildAssets/".Length, subLen).ToLower() + "_";
    }
}