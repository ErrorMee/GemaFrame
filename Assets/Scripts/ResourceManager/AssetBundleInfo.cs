using System;
using UnityEngine;
using System.Collections.Generic;

//http://blog.csdn.net/u012529088/article/details/54984479?utm_source=itdadao&utm_medium=referral

public class AssetBundleInfo
{
    //ab引用
    public AssetBundle ab;
    //ab中所有资源数量
    public int assetCount = 0;
    //已经取用的数量
    public int usedAssetCount = 0;
    //已取用资源引用
    public Dictionary<string, object> assetDic = new Dictionary<string, object>();
    //是否自动释放
    public bool isAutoUnload = true;
    //释放时是否释放所有
    public bool isAllUnload = false;

    public void Load(AssetBundleCreateRequest abRequest)
    {
        //内存标记3 ABAsset
        //内存镜像数据块
        ab = abRequest.assetBundle;
        assetCount = ab.GetAllAssetNames().Length;
    }

    public void Unload(bool isAll)
    {
        if (isAll)
        {
            assetDic.Clear();
        }
        //true 释放AB内存镜像的同时释放LoadAsset内存
        ab.Unload(isAll);
        ab = null;
    }


    public void UseAsset(string assetName, object asset)
    {
        if (!assetDic.ContainsKey(assetName))
        {
            assetDic.Add(assetName, asset);
            usedAssetCount++;

            if (isAutoUnload && (assetCount == usedAssetCount))
            {
                Unload(isAllUnload);
            }
        }
    }
}