using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseConfig<T> where T : BaseConfigInfo
{
    public List<T> data;

    public void AddInfo(T configInfo)
    {
        data.Add(configInfo);
    }

    public T GetInfo(int id)
    {
        for (int i = 0;i<data.Count;i++)
        {
            T info = data[i];
            if (info.id == id)
            {
                return info;
            }
        }

        return default(T);
    }
}

public class BaseConfigInfo
{
    public int id;
}
