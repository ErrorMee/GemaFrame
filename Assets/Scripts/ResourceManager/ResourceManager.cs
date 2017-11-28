using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResPath : MonoBehaviour
{
    public string resPath;
}

public class ResourceManager : Singleton<ResourceManager>
{

    public AssetManager assetManager;

    //对象池dic
    private Dictionary<string, ObjectPool<GameObject>> gameObjectDic = new Dictionary<string, ObjectPool<GameObject>>();

    class LoadingContext
    {
        public string fileName;
        public bool loading = true;
        public UnityEngine.Object res = null;
        public ArrayList callBack = new ArrayList();
    }
    Dictionary<string, LoadingContext> loadingDic = new Dictionary<string, LoadingContext>();

    /// <summary>
    /// 通用异步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="finishLoad"></param>
    public void LoadAsync<T>(string fileName, System.Action<T> finishLoad) where T : Object
    {
        LoadAsyncImpl(fileName, finishLoad);
    }

    void LoadAsyncImpl<T>(string fileName, System.Action<T> finishLoad) where T : Object
    {
        LoadingContext context;
        if (loadingDic.TryGetValue(fileName,out context))
        {
            if (context.loading)
            {
                context.callBack.Add(finishLoad);
            } else
            {
                finishLoad(context.res as T);
            }
        } else
        {
            context = new LoadingContext();
            context.fileName = fileName;
            context.loading = true;
            context.callBack.Add(finishLoad);
            loadingDic.Add(fileName, context);

            assetManager.LoadAsync<T>(fileName, (res) =>
            {
                context.res = res;
                context.loading = false;

                foreach (object cb in context.callBack)
                {
                    System.Action<T> action = cb as System.Action<T>;
                    action(context.res as T);
                }

                context.callBack.Clear();
            });
        }
    }

    public void GetGameObject(string fileName, System.Action<GameObject> callBack)
    {
        LoadAsync<GameObject>(fileName, (res) =>
        {
            if (res == null)
            {
                GLog.Error("error file name : " + fileName);
                return;
            }
            GameObject obj;
            ObjectPool<GameObject> pool;
            if (gameObjectDic.TryGetValue(fileName, out pool))
            {
                if (pool.template == null)
                {
                    //模板引用
                    pool.template = res;
                    pool.keepTime = ObjectPool<GameObject>.ReferenceTime;
                }
            }
            else
            {
                pool = new ObjectPool<GameObject>(res);
                pool.keepTime = ObjectPool<GameObject>.ReferenceTime;
                gameObjectDic[fileName] = pool;
            }
            obj = pool.Request();
            obj.GetOrCreate<ResPath>().resPath = fileName;
            obj.SetActive(true);

            if (callBack != null)
            {
                callBack(obj);
            }
        });
    }

    public void DestroyGameObj(GameObject obj)
    {
        ResPath rp = obj.GetComponent<ResPath>();
        if (rp)
        {
            ObjectPool<GameObject> pool;
            if (gameObjectDic.TryGetValue(rp.resPath, out pool))
            {
                pool.UpdateVisitTime();
                pool.Return(obj);
                obj.SetActive(false);
            }
            else
            {
                GameObject.Destroy(obj);
            }
        }
        else
        {
            GameObject.Destroy(obj);
        }
    }

    public void LateUpdate()
    {
        var it1 = gameObjectDic.GetEnumerator();
        while (it1.MoveNext())
        {
            it1.Current.Value.Update();
        }
    }
}
