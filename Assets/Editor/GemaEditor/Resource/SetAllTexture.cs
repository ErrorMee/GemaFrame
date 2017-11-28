using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class SetAllTexture
{
    const string kSetAllTexture = GemaEditorConst.Texture + "/☀ SetAllTexture";
    /// <summary>
    /// 设置所有纹理
    /// </summary>
    [MenuItem(kSetAllTexture, false, 100)]
    public static void _SetAllTexture()
    {
        int count = 0;
        string[] all = AssetDatabase.FindAssets("t:texture2d",new string[1] { "Assets/BuildAssets/UISprite" });
        foreach (string t in all)
        {
            GemaEditor.ShowProgress(++count, all.Length);
            string s = AssetDatabase.GUIDToAssetPath(t);
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(s);
            SetTexture(texture);
        }
        EditorUtility.ClearProgressBar();
    }

    const string kResetAllTexture = GemaEditorConst.Texture + "/ResetAllTexture";
    /// <summary>
    /// 恢复所有纹理
    /// </summary>
    [MenuItem(kResetAllTexture, false, 101)]
    static void _ResetAllTexture()
    {
        int count = 0;
        string[] all = AssetDatabase.FindAssets("t:texture2d", new string[1] { "Assets/BuildAssets/UISprite" });
        foreach (string t in all)
        {
            GemaEditor.ShowProgress(++count, all.Length);
            string s = AssetDatabase.GUIDToAssetPath(t);
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(s);
            ResetTexture(texture);
        }
        EditorUtility.ClearProgressBar();
    }

    static void SetTexture(Texture texture)
    {
        if (texture == null)
            return;
        string assetPath = AssetDatabase.GetAssetPath(texture);

        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

        string endFolderName = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;

        string spritePackingTag = endFolderName;

        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.spritePackingTag = spritePackingTag;
        textureImporter.mipmapEnabled = false;
        textureImporter.npotScale = TextureImporterNPOTScale.None;
        textureImporter.alphaIsTransparency = true;

        TextureImporterPlatformSettings tips = new TextureImporterPlatformSettings();
        tips.name = "Android";
        tips.maxTextureSize = 2048;
        tips.format = textureImporter.alphaIsTransparency ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC2_RGB4;
        tips.compressionQuality = (int)TextureCompressionQuality.Normal;
        tips.allowsAlphaSplitting = false;
        tips.overridden = true;
        textureImporter.SetPlatformTextureSettings(tips);

        tips.name = "iPhone";
        tips.maxTextureSize = 2048;
        tips.format = textureImporter.alphaIsTransparency ? TextureImporterFormat.PVRTC_RGBA4 : TextureImporterFormat.PVRTC_RGB4;
        tips.compressionQuality = (int)TextureCompressionQuality.Normal;
        tips.allowsAlphaSplitting = false;
        tips.overridden = true;
        textureImporter.SetPlatformTextureSettings(tips);

        textureImporter.SaveAndReimport();
    }

    static void ResetTexture(Texture texture)
    {
        if (texture == null)
            return;
        string assetPath = AssetDatabase.GetAssetPath(texture);

        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        
        textureImporter.ClearPlatformTextureSettings("Android");
        textureImporter.ClearPlatformTextureSettings("iPhone");
        textureImporter.SaveAndReimport();
    }
}
