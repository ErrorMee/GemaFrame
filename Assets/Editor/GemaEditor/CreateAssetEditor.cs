using UnityEditor;
using System.IO;
using UnityEngine;
using System;

public static class GameSettingEditor
{
    [MenuItem(GemaEditorConst.Setting)]
    public static void CreateSetting()
    {
        OnCreateSetting<GameSetting>();
    }

    private static void OnCreateSetting<T>() where T : ScriptableObject
    {
        ScriptableObject setting = ScriptableObject.CreateInstance<T>();

        if (setting == null)
        {
            GLog.Error("Setting ScriptableObject is not fund:" + typeof(T).ToString());
            return;
        }

        string settingPath = string.Format("Assets/Resources/{0}.asset", typeof(T).ToString());

        var tExist = File.Exists(settingPath);
        if (!tExist)
        {
            AssetDatabase.CreateAsset(setting, settingPath);
        }
    }
}


