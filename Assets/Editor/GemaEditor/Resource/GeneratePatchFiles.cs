using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class GeneratePatchFiles
{
    public static string bundlesPath = Application.streamingAssetsPath + "/patchs/";
    private static string bundlefilesName = "patchfiles.txt";

    const string kGenerateAssetBundleFiles = GemaEditorConst.Resource + "/Generate PatchFiles";
    /// <summary>
    ///  生成bundlefiles
    /// </summary>
    //[@MenuItem(kGenerateAssetBundleFiles, false, 2051)]
    public static void GenerateFiles()
    {
        string bundlefilesPath = Path.Combine(bundlesPath, bundlefilesName);

        List<string> files = FindAllFiles.ListFiles(bundlesPath, true);

        List<string> record = new List<string>();

        for (int i = 0;i< files.Count;i++)
        {
            string filePath = files[i];
            string ext = Path.GetExtension(filePath);
            string name = Path.GetFileName(filePath);

            if (ext.Equals(".meta") || ext.Equals(".manifest"))
            {
                continue;
            }

            if (ext.Equals(".ab") || ext.Equals("") || name.Contains("patchs"))
            {
                GLog.Log(name);

                byte[] fileByte = File.ReadAllBytes(filePath);

                string md5 = MD5Util.MD5(fileByte);

                int size = fileByte.Length;

                string relativePath = filePath.Replace(bundlesPath, string.Empty).Trim();

                StringBuilder sb = new StringBuilder();
                sb.Append(relativePath);
                sb.Append("|");
                sb.Append(md5);
                sb.Append("|");
                sb.Append(size);
                record.Add(sb.ToString());
            }
        }

        FileStream fs = new FileStream(bundlefilesPath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < record.Count; i++)
        {
            string value = record[i];
            sw.WriteLine(value);
        }
        sw.Close();
        fs.Close();

        AssetDatabase.Refresh();
    }
}