using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

static public class GemaEditor
{
    const string kClearPlayerPrefs = GemaEditorConst.Root + "/Clear/PlayerPrefs";
    [MenuItem(kClearPlayerPrefs)]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    const string kClearEditorPrefs = GemaEditorConst.Root + "/Clear/EditorPrefs";
    [MenuItem(kClearEditorPrefs)]
    static void ClearEditorPrefs()
    {
        EditorPrefs.DeleteAll();
    }

    public static void ShowProgress(int cur, int total, string title = "...",string content = "...")
    {
        EditorUtility.DisplayProgressBar(title, content + string.Format("{0}/{1}", cur, total), cur / (float)total);
    }
}
