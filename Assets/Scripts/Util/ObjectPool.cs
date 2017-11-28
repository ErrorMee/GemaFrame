using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectPool<T> : List<T> where T : Object, new()
{
    //对象引用时间
    public const int ReferenceTime = 10;
    //对象模板
    public T template;
    //在使用中的对象
    LinkedList<T> activeList = new LinkedList<T>();
    //最近一次申请时间
    float visitTime;
    //池子保留时间
    public int keepTime = -1;

    int requestCount = 0;

    public int ActiveCount
    {
        get { return activeList.Count; }
    }

    public ObjectPool(T res)
    {
        this.template = res;
    }

    public bool PoolEmpty
    {
        get { return Count == 0; }
    }

    //申请对象
    public T Request()
    {
        T obj = null;
        if (PoolEmpty)
        {
            if (template)
            {
                obj = Object.Instantiate(template) as T;
                obj.name = template.name + "_" + (++requestCount);
            }
            else
            {
                obj = new T();
            }

            activeList.AddLast(obj);
            visitTime = Time.time;
            return obj;
        }

        obj = this[Count - 1];
        RemoveAt(Count - 1);
        activeList.AddLast(obj);
        visitTime = Time.time;
        return obj;
    }

    /// <summary>
    /// 更新访问时间
    /// offTimes控制回收周期  正数提前 负数延后
    /// </summary>
    /// <param name="offTimes"></param>
    public void UpdateVisitTime(int offTimes = 0)
    {
        visitTime = Time.time + offTimes;
    }

    //使用后返回池子
    public void Return(T obj)
    {
        activeList.Remove(obj);

        if (!Contains(obj))
        {
            Add(obj);
        }
    }

    public void Update()
    {

        if (Count > 0)
        {
            if (keepTime > 0)
            {
                if (Time.time - visitTime > keepTime)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        T obj = this[i];
                        Object.Destroy(obj);
                    }

                    Clear();
                }
            }
            
        }
        else
        {
            if (keepTime > 0)
            {
                if (ActiveCount == 0 && template)
                {
                    if (Time.time - visitTime > keepTime)
                    {
                        template = null;
                    }
                }
            } 
        }
    }

    public void ClearObj(T obj)
    {
        activeList.Remove(obj);
        Remove(obj);
    }

    public void Destroy()
    {
        foreach (T obj in activeList)
        {
            Object.Destroy(obj);
        }

        activeList.Clear();

        foreach (T obj in this)
        {
            Object.Destroy(obj);
        }

        Clear();

        template = null;
    }
}
