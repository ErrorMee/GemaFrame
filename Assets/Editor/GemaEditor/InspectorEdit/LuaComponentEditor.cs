using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LuaComponent), true)]
public class LuaComponentEditor : Editor
{
    private static string LuaRootPath = "Assets/Lua/";

    private SerializedProperty luaScriptPath;

    private Object luaScriptPathObj;

    void OnEnable()
    {
        luaScriptPath = serializedObject.FindProperty("luaScriptPath");

        luaScriptPathObj = AssetDatabase.LoadMainAssetAtPath(LuaRootPath + luaScriptPath.stringValue);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        luaScriptPathObj = EditorGUILayout.ObjectField("luaScriptPathObj", luaScriptPathObj, typeof(Object), false);

        string pathTemp = AssetDatabase.GetAssetPath(luaScriptPathObj);
        if (pathTemp.Contains(LuaRootPath))
        {
            if (pathTemp.EndsWith(".lua"))
            {
                string pathOff = pathTemp.Replace(LuaRootPath, "");
                luaScriptPath.stringValue = pathOff;

                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
            else
            {
                GLog.Error("File must EndsWith lua... filter suggest:" + target.name);
            }
        }
        else
        {
            GLog.Error("File must in:" + LuaRootPath + "... filter suggest:" + target.name);
        }
        
    }
}
