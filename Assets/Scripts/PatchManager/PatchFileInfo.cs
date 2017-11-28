using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

/// <summary>
/// 热更文件总表
/// </summary>
public class PatchFiles
{
    private int count;
    public int Count
    {
        get { return count; }
    }

    public Dictionary<string, PatchFileInfo> dic = new Dictionary<string, PatchFileInfo>();

    public void Load(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return;
        }
            
        StringReader textReader = new StringReader(content);
        string line;

        while (!string.IsNullOrEmpty(line = textReader.ReadLine()))
        {
            PatchFileInfo patchFileInfo = new PatchFileInfo(line);
            dic[patchFileInfo.ResPath] = patchFileInfo;
            count++;
        }
    }

    public void Add(PatchFileInfo patchFileInfo)
    {
        dic[patchFileInfo.ResPath] = patchFileInfo;
        count++;
    }

    public void FileReplace(string path)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in dic)
        {
            sb.AppendLine(item.Value.ToString());
        }
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        sb.Remove(0, sb.Length);
    }

    public void FileAppend(string path, string abFileText)
    {
        using (StreamWriter sw = new StreamWriter(path, true))
        {
            sw.WriteLine(abFileText);
            sw.Close();
        }
    }

    public float GetAllSize()
    {
        float allSize = 0;
        foreach (PatchFileInfo remotePatchFileInfo in dic.Values)
        {
            allSize += remotePatchFileInfo.Size;
        }
        return allSize;
    }
}

/// <summary>
/// 单个热更记录
/// </summary>
public class PatchFileInfo
{
    private string resPath;
    public string ResPath
    {
        get { return resPath; }
    }
    private string md5;
    public string Md5
    {
        get { return md5; }
    }
    private long size;
    public long Size
    {
        get
        {
            return size;
        }
    }
    public PatchFileInfo(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return;
        }
        string[] strs = content.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        this.resPath = strs[0].Trim();
        this.md5 = strs[1];
        this.size = System.Convert.ToInt64(strs[2]);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(resPath);
        sb.Append("|");
        sb.Append(md5);
        sb.Append("|");
        sb.Append(Size);
        return sb.ToString();
    }
}
