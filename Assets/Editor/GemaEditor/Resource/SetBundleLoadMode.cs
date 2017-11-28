using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetBundleLoadMode
{ 

    const string kBundleLoadMode = GemaEditorConst.Resource + "/BundleLoadModeToggle";
    /// <summary>
    /// 切换bundle加载模式
    /// </summary>
    [MenuItem(kBundleLoadMode, false, 1)]
    public static void ToggleBundleLoadMode()
    {
        AssetManager.bundleLoadMode = !AssetManager.bundleLoadMode;
        GLog.Log("加载模式 bundle = " + AssetManager.bundleLoadMode,Color.red);
    }
    [MenuItem(kBundleLoadMode, true, 901)]
    public static bool ToggleBundleLoadModeCheck()
    {
        Menu.SetChecked(kBundleLoadMode, AssetManager.bundleLoadMode);
        return true;
    }
}
