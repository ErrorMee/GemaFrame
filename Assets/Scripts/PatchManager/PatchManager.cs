using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class PatchManager : SingletonBehaviour<PatchManager>
{
    private PatchFiles localPatchFiles;
    private PatchFiles remotePatchFiles;
    private Queue<PatchFileInfo> needLoadPatchFiles;
    private PatchFiles refreshPatchFiles;

    private float needLoadSize;

    public delegate void FinishDownloadOneFile(bool finish);

    private Text startUpText;

    void Awake()
    {
        startUpText = Game.Instance.canvasTrans.Find("StartUp/Text").GetComponent<Text>();
    }

    public void Init()
    {
        if (Game.Instance.gameSetting.patchOpen)
        {
            HandlePatch();
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

    private void HandlePatch()
    {
        GameEvent.SendEvent(GameEventType.GameFlow, GameFlow.PatchFileLocalLoad);

        localPatchFiles = new PatchFiles();
        string localPatchFilesPath = PathUtil.LocalPatchFilesPath();
        if (File.Exists(localPatchFilesPath))
        {
            GLog.Log("localPatchFilesPath " + localPatchFilesPath);
            HttpManager.Instance.LoadText(localPatchFilesPath, (res) =>
            {
                localPatchFiles.Load(res);
                LoadRemotePatchFiles();
            });
        }
        else
        {
            string streamingPatchFilesPath = PathUtil.StreamingPatchFilesPath();
            
            if (File.Exists(streamingPatchFilesPath))
            {
                GLog.Log("localPatchFilesPath " + streamingPatchFilesPath);
                HttpManager.Instance.LoadText(streamingPatchFilesPath, (res) =>
                {
                    File.WriteAllText(localPatchFilesPath, res);
                    localPatchFiles.Load(res);
                    LoadRemotePatchFiles();
                });
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
            HttpManager.Instance.LoadText(remotePatchFilesPath, (res) =>
            {
                remotePatchFiles.Load(res);
                CompareLocalAndRemotePatchFiles();
            });
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

        float needLoadSizeM = needLoadSize / (1024 * 1024);

        GLog.Log("needLoadSizeM " + needLoadSizeM + " Count " + needLoadPatchFiles.Count, Color.red);
        if (needLoadPatchFiles.Count > 0)
        {//如果下载列表不为空
            if (NetworkUtil.GetNetworkType() == NetworkType.Wifi)
            {
                
            }
            else
            {
                if (needLoadSizeM > 5f)
                {
                    //大于5m用户提醒
                }
            }
            
            HttpPatch();
        }
        else
        {
            PatchComplete();
        }
    }


    FileStream outStream;
    private void HttpPatch()
    {
        if (needLoadPatchFiles.Count > 0)
        {
            PatchFileInfo patchFileInfo = needLoadPatchFiles.Peek();
            string fileUrl = Game.Instance.gameSetting.GetPatchRootPath() + patchFileInfo.ResPath.Trim();
            string localFilePath = PathUtil.PatchPath + patchFileInfo.ResPath.Trim();
            string localFileDirectory = Path.GetDirectoryName(localFilePath);

            if (!Directory.Exists(localFileDirectory))
            {
                Directory.CreateDirectory(localFileDirectory);
            }

            outStream = new FileStream(localFilePath, FileMode.OpenOrCreate);

            HttpManager.Instance.LoadFile(fileUrl, HttpDownloadProgress);
        }
        else
        {
            PatchComplete();
        }
    }

    private void HttpDownloadProgress(int deltaBufferLength, byte[] deltaBuffer, bool isComplete)
    {
        if (deltaBufferLength > 0)
        {
            outStream.Write(deltaBuffer, 0, deltaBufferLength);
        }
        
        if (isComplete)
        {
            outStream.Close();
            PatchFileInfo patchFile = needLoadPatchFiles.Dequeue();
            refreshPatchFiles.FileAppend(PathUtil.LocalPatchFilesPath(), patchFile.ToString());
            HttpPatch();
        }
        else
        {
            //GLog.Log("deltaBufferLength:" + deltaBufferLength + " time " + Time.time);
            if (deltaBuffer == null || deltaBuffer.Length < 1)
            {
            }
            else
            {
            }
        }
    }
}