using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class PatchManager : SingletonBehaviour<PatchManager>
{
    public PatchConfigInfo patchConfigInfo;

    private PatchFiles localPatchFiles;
    private PatchFiles remotePatchFiles;
    private Queue<PatchFileInfo> needLoadPatchFiles;
    private PatchFiles refreshPatchFiles;

    private float needLoadSize;

    public delegate void FinishDownloadOneFile(bool finish);

    private Text startUpText;

    void Awake()
    {
        startUpText = GameManager.Instance.canvasTrans.Find("StartUp/Text").GetComponent<Text>();
    }

    public void OnStart()
    {
        InitPatchConfigInfo();
    }

    private void InitPatchConfigInfo()
    {
        string p = PathUtil.PatchPath;

        GameEvent.SendEvent(GameEventType.GameFlow, GameFlow.PatchConfigLoad);

        patchConfigInfo = new PatchConfigInfo();

        string configText = FileHelper.GetText(PathUtil.PatchConfigPath());

        JsonUtility.FromJsonOverwrite(configText, patchConfigInfo);
        
        if (patchConfigInfo.openPatch)
        {
            LoadLocalPatchFiles();
            LoadRemotePatchFiles();

            CompareLocalAndRemotePatchFiles();
        }
        else
        {
            PatchComplete();
        }
    }

    private void PatchComplete()
    {
        GameEvent.SendEvent(GameEventType.GameFlow, GameFlow.PatchComplete);
        DestroyImmediate(startUpText.transform.parent.gameObject);
        GameEvent.SendEvent(GameEventType.PatchComplete);
    }

    private void LoadLocalPatchFiles()
    {
        GameEvent.SendEvent(GameEventType.GameFlow, GameFlow.PatchFileLocalLoad);

        localPatchFiles = new PatchFiles();
        string localPatchFilesPath = PathUtil.LocalPatchFilesPath();
        if (File.Exists(localPatchFilesPath))
        {
            string filesText = FileHelper.GetText(localPatchFilesPath);
            localPatchFiles.Load(filesText);
            GLog.Log("localPatchFilesPath " + localPatchFilesPath);
        }
        else
        {
            string streamingPatchFilesPath = PathUtil.StreamingPatchFilesPath();
            GLog.Log("localPatchFilesPath " + streamingPatchFilesPath);
            if (File.Exists(streamingPatchFilesPath))
            {
                string filesText = FileHelper.GetText(streamingPatchFilesPath);
                if (filesText != null)
                {
                    File.WriteAllText(localPatchFilesPath, filesText);
                    localPatchFiles.Load(filesText);
                }
                else
                {
                    GLog.Error(string.Format("{0} is null", streamingPatchFilesPath), true);
                }
            }
            else
            {
                GLog.Error(string.Format("{0} is not find", streamingPatchFilesPath),true);
            }
        }
    }

    private void LoadRemotePatchFiles()
    {
        GameEvent.SendEvent(GameEventType.GameFlow, GameFlow.PatchFileRemoteLoad);
        remotePatchFiles = new PatchFiles();
        string remotePatchFilesPath = PathUtil.RemotePatchFilesPath();
        GLog.Log("remotePatchFilesPath " + remotePatchFilesPath);
        if (File.Exists(remotePatchFilesPath))
        {
            string filesText = FileHelper.GetText(remotePatchFilesPath);
            remotePatchFiles.Load(filesText);
        }
        else
        {
            GLog.Error(string.Format("{0} is null", remotePatchFilesPath), true);
        }
    }

    private void CompareLocalAndRemotePatchFiles()
    {
        needLoadPatchFiles = new Queue<PatchFileInfo>();
        refreshPatchFiles = new PatchFiles();

        needLoadSize = 0;
        foreach (PatchFileInfo remotePatchFileInfo in remotePatchFiles.dic.Values)
        {
            if (!localPatchFiles.dic.ContainsKey(remotePatchFileInfo.ResPath))
            {//新增文件
                needLoadSize += remotePatchFileInfo.Size;
                needLoadPatchFiles.Enqueue(remotePatchFileInfo);
            }
            else if (localPatchFiles.dic[remotePatchFileInfo.ResPath].Md5 != remotePatchFileInfo.Md5)
            {//修改文件
                needLoadSize += remotePatchFileInfo.Size;
                needLoadPatchFiles.Enqueue(remotePatchFileInfo);
            }
            else
            {//同样文件
                //refreshPatchFiles.Add(remotePatchFileInfo);
                needLoadSize += remotePatchFileInfo.Size;
                needLoadPatchFiles.Enqueue(remotePatchFileInfo);
            }
        }

        refreshPatchFiles.FileReplace(PathUtil.LocalPatchFilesPath());
        GLog.Log("needLoadSize " + needLoadSize + " Count " + needLoadPatchFiles.Count, Color.red);
        if (needLoadPatchFiles.Count > 0)
        {//如果下载列表不为空
            needLoadSize = needLoadSize / (float)(1024 * 1024);
            if (NetworkUtil.GetNetworkType() == NetworkType.Wifi)
            {
                
            }
            else
            {
                if (needLoadSize > 5f)
                {
                    //大于5m用户提醒
                }
            }

            StartCoroutine(PatchAsset());
        }
        else
        {
            PatchComplete();
        }
    }
    
    /// <summary>
    /// 热更资源
    /// </summary>
    IEnumerator PatchAsset()
    {
        if (needLoadPatchFiles.Count > 0)
        {
            StartCoroutine(DownloadUWR(needLoadPatchFiles.Peek()));
        }
        else
        {
            PatchComplete();
        }
        yield break;
    }

    IEnumerator DownloadUWR(PatchFileInfo patchFileInfo)
    {
        string fileUrl = patchConfigInfo.patchRootPath + patchFileInfo.ResPath.Trim();
        string localFilePath = PathUtil.PatchPath + patchFileInfo.ResPath.Trim();
        string localFileDirectory = Path.GetDirectoryName(localFilePath);

        if (!Directory.Exists(localFileDirectory))
        {
            Directory.CreateDirectory(localFileDirectory);
            yield return null;//创建目录暂停一帧
        }
        
        GLog.Log("patch <= " + localFilePath, Color.green, true);

        FileStream outStream = new FileStream(localFilePath, FileMode.OpenOrCreate);
        UnityWebRequest uwr = new UnityWebRequest(fileUrl);
        uwr.downloadHandler = new PatchDownloadHandler(patchFileInfo, outStream, OnFinishDownloadOneFile);

        yield return uwr.Send();

        yield break;
    }

    private void OnFinishDownloadOneFile(bool finish)
    {
        if (finish)
        {
            if (needLoadPatchFiles.Count > 0)
            {
                PatchFileInfo patchFile = needLoadPatchFiles.Dequeue();
                refreshPatchFiles.FileAppend(PathUtil.LocalPatchFilesPath(), patchFile.ToString());
                if (needLoadPatchFiles.Count > 0)
                {
                    StartCoroutine(DownloadUWR(needLoadPatchFiles.Peek()));
                }
                else
                {
                    StartCoroutine(PatchAsset());
                }
            }
            else
            {
                StartCoroutine(PatchAsset());
            }
        }
    }
}


public class PatchDownloadHandler : DownloadHandlerScript
{
    PatchFileInfo patchFileInfo;
    private Stream outStream;
    PatchManager.FinishDownloadOneFile finishOneCallBack;

    public PatchDownloadHandler(PatchFileInfo _patchFileInfo, Stream _outStream, PatchManager.FinishDownloadOneFile _finishOneCallBack) : base()
    {
        patchFileInfo = _patchFileInfo;
        outStream = _outStream;
        finishOneCallBack = _finishOneCallBack;
    }

    protected override void CompleteContent()
    {
        GLog.Log("CompleteContent:" + patchFileInfo.ResPath);
        if (outStream != null)
        {
            outStream.Close();
            if (finishOneCallBack != null)
            {
                finishOneCallBack(true);
            }
        }
    }
    
    protected override void ReceiveContentLength(int contentLength)
    {
        GLog.Log(patchFileInfo.ResPath + " ContentLength " + contentLength);
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        //GLog.Log("data " + data.Length + " dataLength " + dataLength);

        if (outStream != null)
        {
            outStream.Write(data, 0, dataLength);
        }

        return true;
    }
    
}