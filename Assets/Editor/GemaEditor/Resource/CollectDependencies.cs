using UnityEngine;
using UnityEditor;

public class CollectDependencies : EditorWindow
{
    static GameObject obj = null;

    const string kCollectDependencies = GemaEditorConst.Resource + "/CollectDependencies";
    [MenuItem(kCollectDependencies)]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CollectDependencies window = (CollectDependencies)EditorWindow.GetWindow(typeof(CollectDependencies));
        window.Show();
    }

    void OnGUI()
    {
        obj = EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), 
            "Find Dependency", obj, typeof(GameObject),true) as GameObject;

        if (obj)
        {
            Object[] roots = new Object[] { obj };

            if (GUI.Button(new Rect(3, 25, position.width - 6, 20), "Check Dependencies"))
            {
                Selection.objects = EditorUtility.CollectDependencies(roots);
            }
        }
        else
        {
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Missing:", "Select an object first");
        }

        Object selectedObject = Selection.activeObject;
        if (selectedObject != null)
        {
            if (GUI.Button(new Rect(3, 50, position.width - 6, 20), "Selection Dependencies"))
            {
                string path = AssetDatabase.GetAssetPath(selectedObject);
                GetDependencies(path, true);
            }

            if (GUI.Button(new Rect(3, 75, position.width - 6, 20), "Selection Dependencies false"))
            {
                string path = AssetDatabase.GetAssetPath(selectedObject);
                GetDependencies(path,false);
            }
        }
        
    }

    public void GetDependencies(string path,bool recursive = true)
    {
        string[] dss = AssetDatabase.GetDependencies(path, recursive);
        GLog.Log("-- DEP:START -- " + path,true);
        for (int i = 0; i < dss.Length; i++)
        {
            string ds = dss[i];

            GLog.Log(i + ":" + ds, true);

            AssetImporter ai = AssetImporter.GetAtPath(ds);

            if (string.IsNullOrEmpty(ai.assetBundleName))
            {
                GLog.Log(" BundleName = None !!", Color.red, true);
            }
            else
            {
                GLog.Log(" BundleName = " + ai.assetBundleName, Color.green,true);
            }
        }
        GLog.Log("-- DEP:END --", true);
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}