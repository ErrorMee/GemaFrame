using UnityEngine;
using UnityEditor;

public class BuildResources
{

    const string kBuildResources = GemaEditorConst.Resource + "/☀ BuildResources";
    /// <summary>
    ///  生成资源包
    /// </summary>
    [@MenuItem(kBuildResources, false, 2052)]
    public static void _BuildResources()
    {
        CopyLuaToStreamingAssets.OnCopyLuaToStreaming();

        BuildPipeline.BuildAssetBundles(PathUtil.StreamingassetsPath + "/patchs", BuildAssetBundleOptions.None,BuildTarget.Android);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        GeneratePatchFiles.bundlesPath = Application.streamingAssetsPath + "/patchs/";

        GeneratePatchFiles.GenerateFiles();

    }
	
}
