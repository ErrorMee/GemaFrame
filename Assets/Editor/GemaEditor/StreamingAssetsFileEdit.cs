using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(UnityEditor.DefaultAsset))]
public class StreamingAssetsFileEdit : Editor
{
    private string jsonPath = null;
    private string jsonContent = null;
    private Vector2 scrollPosition = Vector2.zero;

    public override void OnInspectorGUI()
    {
        string path = AssetDatabase.GetAssetPath(target);

        bool isStreamingAssetsFile = path.StartsWith("Assets/StreamingAssets/");

        if (isStreamingAssetsFile)
        {
            bool isJsonFile = path.EndsWith(".json");
            if (isJsonFile)
            {
                if (jsonPath != path)
                {
                    jsonPath = path;
                    jsonContent = System.IO.File.ReadAllText(path, new System.Text.UTF8Encoding(false));
                    scrollPosition = Vector2.zero;
                }
                GUI.enabled = true;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("保存", GUILayout.Width(50)))
                {
                    System.IO.File.WriteAllText(path, jsonContent, new System.Text.UTF8Encoding(false));
                    AssetDatabase.ImportAsset(path);
                }
                EditorGUILayout.LabelField(path);
                EditorGUILayout.EndHorizontal();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                jsonContent = EditorGUILayout.TextArea(jsonContent,GUILayout.ExpandWidth(true));
                EditorGUILayout.EndScrollView();
            }
            else if (path.EndsWith(".txt"))
            {
                string content = File.ReadAllText(path);
                if (content.Length > 7000)
                {
                    content = content.Substring(0, 7000) + " \n... ";
                }
                GUILayout.Label(content);
            }
        }
    }

}
