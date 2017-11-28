using System;
using UnityEngine;
using UnityEditor;

public class PathEdit
{
    const string kOpenLocalPatchPath = GemaEditorConst.Path + "/OpenLocalPatchPath";
    [MenuItem(kOpenLocalPatchPath, false, 100)]
    static void OpenLocalPatchPath()
    {
        System.Diagnostics.Process.Start(PathUtil.PatchPath);
    }

    const string kOpenRemotePatchPath = GemaEditorConst.Path + "/OpenRemotePatchPath";
    [MenuItem(kOpenRemotePatchPath, false, 101)]
    static void OpenRemotePatchRootPath()
    {
        string jsonContent = System.IO.File.ReadAllText(PathUtil.PatchConfigPath(), new System.Text.UTF8Encoding(false));
        PatchConfigInfo patchConfigInfo = new PatchConfigInfo();
        JsonUtility.FromJsonOverwrite(jsonContent, patchConfigInfo);
        System.Diagnostics.Process.Start(patchConfigInfo.patchRootPath);
    }
}