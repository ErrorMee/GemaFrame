using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourcesConfig: BaseConfig<ResourcesConfigInfo>
{
    public ResourcesConfigInfo GetInfoByName(string _name)
    {
        for (int i = 0; i < data.Count; i++)
        {
            ResourcesConfigInfo info = data[i];
            if (info.name == _name)
            {
                return info;
            }
        }

        return null;
    }
}

[Serializable]
public class ResourcesConfigInfo: BaseConfigInfo
{
    public string name;
    public string ab;
    public string type;
}