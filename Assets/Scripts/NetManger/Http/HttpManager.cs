using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void DownloadProgress(int dataLength, byte[] deltaBuffer, bool isComplete);

public class HttpManager : SingletonBehaviour<HttpManager>
{
    /// <summary>
    /// 同步获取二进制
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public byte[] GetBytes(string fullPath)
    {
        WWW www = new WWW(fullPath);
#if UNITY_IOS
		System.Threading.Thread.Sleep (100);
#endif
        while (!www.isDone)
        {
            System.Threading.Thread.Sleep(0);
        }

        if (www.error != null)
        {
            Debug.LogError(www.error + ":" + fullPath);
            www.Dispose();
            return null;
        }

        int wwwlen = www.bytes.Length;
        if (wwwlen <= 0)
        {
            www.Dispose();
            return null;
        }

        byte[] buffer = new byte[wwwlen];
        www.bytes.CopyTo(buffer, 0);
        www.Dispose();
        return buffer;
    }

    /// <summary>
    /// 异步获取文本
    /// </summary>
    /// <param name="url"></param>
    /// <param name="finishLoad"></param>
    public void LoadText(string url, Action<string> finishLoad)
    {
        StartCoroutine(OnLoadText(url, finishLoad));
    }
    IEnumerator OnLoadText(string url, Action<string> finishLoad)
    {
        UnityWebRequest uwr = new UnityWebRequest(url);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            GLog.Error(uwr.error);
        }
        else
        {
            if (finishLoad != null)
            {
                finishLoad(uwr.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// 预分配内存
    /// </summary>
    private byte[] preAllocatedBuffer = new byte[1024 * 64];

    /// <summary>
    /// 较大文件下载
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="finishLoad">回调方法</param>
    public void LoadFile(string url, DownloadProgress downloadProgress)
    {
        GLog.Log("LoadFile:" + url, Color.red);
        StartCoroutine(OnLoadFile(url, downloadProgress));
    }
    IEnumerator OnLoadFile(string url, DownloadProgress downloadProgress)
    {
        UnityWebRequest uwr = new UnityWebRequest(url,UnityWebRequest.kHttpVerbGET);
        
        MyDownloadHandler myDownloadHandler = new MyDownloadHandler(preAllocatedBuffer, downloadProgress);

        uwr.downloadHandler = myDownloadHandler;
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            GLog.Error(uwr.error);
            if (downloadProgress != null)
            {
                downloadProgress(0, null, false);
            }
        }
    }
}

/// <summary>
/// 自定义下载器 
/// 进度处理
/// 优化内存分配
/// </summary>
class MyDownloadHandler : DownloadHandlerScript
{
    /// <summary>
    /// 进度事件
    /// </summary>
    DownloadProgress downloadProgress;

    private int loadedLength;
    private int fullLength;

    /// <summary>
    /// 标准方式 每帧ReceiveData都会生成一个新的byte[]；
    /// 禁用
    /// </summary>
    public MyDownloadHandler() : base() {}

    /// <summary>
    /// 预分配byte[]
    /// 推荐使用
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="_downloadProgress"></param>
    public MyDownloadHandler(byte[] buffer, DownloadProgress _downloadProgress) : base(buffer)
    {
        downloadProgress = _downloadProgress;
    }

    /// <summary>
    /// 接收到内容长度时调用
    /// </summary>
    /// <param name="contentLength"></param>
    protected override void ReceiveContentLength(int contentLength)
    {
        //base.ReceiveContentLength(contentLength);
        fullLength = contentLength;
    }

    /// <summary>
    /// 从接收到数据开始每帧调用直至结束
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataLength"></param>
    /// <returns>ture 继续 false 终止</returns>
    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        //base.ReceiveData(data, dataLength);
        if (data == null || data.Length < 1)
        {
            GLog.Error("MyDownloadHandler :: ReceiveData - received a null/empty buffer");
            if (downloadProgress != null)
            {
                downloadProgress(dataLength, data, false);
            }
            return false;
        }
        loadedLength += dataLength;
        if (downloadProgress != null)
        {
            downloadProgress(dataLength, data, false);
        }
        return true;
    }

    protected override void CompleteContent()
    {
        if (downloadProgress != null)
        {
            downloadProgress(0, null, true);
        }
    }
}
