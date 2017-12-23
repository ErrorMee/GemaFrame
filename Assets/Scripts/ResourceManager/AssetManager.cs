using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetManager : SingletonBehaviour<AssetManager>
{
    private void Awake()
    {
        ResourceManager.Instance.assetManager = this;
    }

    private AssetBundleManifest manifest;
    private Dictionary<string, AssetBundleInfo> assetBundleInfoDic = new Dictionary<string, AssetBundleInfo>();

    private ResourcesConfig resourceConfig = new ResourcesConfig();

    public void Init()
    {
#if UNITY_EDITOR
        if (AssetManager.bundleLoadMode)//编辑器下的Bundle加载模式
        {
            string manifestPath = PathUtil.GetAssetPath("patchs");
            AssetBundle manifestbundle = AssetBundle.LoadFromFile(manifestPath);
            manifest = manifestbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestbundle.Unload(false);
        }
#else
        {
            string manifestPath = PathUtil.GetAssetPath("patchs");
            AssetBundle manifestbundle = AssetBundle.LoadFromFile(manifestPath);
            manifest = manifestbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestbundle.Unload(false);
        }
#endif

        StartCoroutine(LoadAsyncImpl<TextAsset>(PathUtil.GetAssetPath("config_.ab"), "ab_resources", (res) =>
        {
            JsonUtility.FromJsonOverwrite(res.text, resourceConfig);
            GLog.Log("resourceConfig.data.Count:" + resourceConfig.data.Count);
            GameEvent.SendEvent(GameEventType.AssetManagerReady);
        }));
    }

    /// <summary>
    /// bundle加载模式 编辑器模式下可以切换
    /// </summary>
    public static bool bundleLoadMode 
    {
        get
        {
#if UNITY_EDITOR
            return Game.Instance.gameSetting.loadModeIsBundle;
#else
            return true;
#endif
        }
    }

    /// <summary>
    /// 通用异步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="assetName"></param>
    /// <param name="finishLoad"></param>
    public void LoadAsync<T>(string assetName, System.Action<T> finishLoad) where T : UnityEngine.Object
    {
        string abName = ".ab";
        ResourcesConfigInfo resourceConfigInfo = resourceConfig.GetInfoByName(assetName);
        abName = resourceConfigInfo.ab + abName;

        StartCoroutine(LoadAsyncImpl<T>(abName, assetName, finishLoad));
    }
    
    IEnumerator LoadAsyncImpl<T>(string abName, string assetName, System.Action<T> finishLoad) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (!bundleLoadMode)//编辑器下的非Bundle加载模式
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(abName, assetName);

            T target = null;
            if (assetPaths.Length > 0)
            {
                target = AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
            }

            yield return null;
            if (finishLoad != null)
            {
                finishLoad(target);
            }

            yield break;
        }
#endif
        yield return StartCoroutine(LoadAssetBundle(abName));

        AssetBundleInfo abInfo = null;
        if (assetBundleInfoDic.TryGetValue(abName, out abInfo))
        {
            AssetBundleRequest abRequest = abInfo.ab.LoadAssetAsync(assetName);
            while (!abRequest.isDone)
            {
                yield return null;
            }
            //内存标记2 LoadAsset
            T res = abRequest.asset as T;
            
            finishLoad(res);
            abInfo.UseAsset(assetName, abRequest.asset);
        }
        else
        {
            GLog.Error(abName + " is not find~");
        }
    }

    IEnumerator LoadAssetBundle(string abName)
    {
        string[] dependencies = manifest.GetAllDependencies(abName);
        for (int i = 0; i < dependencies.Length; i++)
        {
            string depABName = dependencies[i];
            yield return StartCoroutine(OnLoadAssetBundle(depABName));
        }
        yield return StartCoroutine(OnLoadAssetBundle(abName));
    }

    IEnumerator OnLoadAssetBundle(string abName)
    {
        AssetBundleInfo abInfo = null;

        if (assetBundleInfoDic.TryGetValue(abName, out abInfo))
        {
            //已加载过
        }
        else
        {
            AssetBundleCreateRequest abRequest;
            if (!abName.Contains("/patchs"))
            {
                abRequest = AssetBundle.LoadFromFileAsync(PathUtil.GetAssetPath(abName));
            }
            else
            {
                abRequest = AssetBundle.LoadFromFileAsync(abName);
            }
            
            while (!abRequest.isDone)
            {
                yield return null;
            }
            abInfo = new AssetBundleInfo();
            abInfo.Load(abRequest);
            assetBundleInfoDic.Add(abName, abInfo);
        }
    }

    private void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }
}
