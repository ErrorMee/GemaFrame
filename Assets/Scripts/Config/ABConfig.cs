using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ABConfig : BaseConfig<ABConfigInfo>
{
    private Dictionary<string, ABConfigInfo> adDic = new Dictionary<string, ABConfigInfo>();

    public ABConfigInfo GetInfoByName(string _name)
    {
        ABConfigInfo info;
        if (adDic.TryGetValue(_name, out info))
        {
            return info;
        }

        for (int i = 0; i < data.Count; i++)
        {
            info = data[i];

            for (int j = 0; j < info.names.Count; j++)
            {
                if (info.names[j] == _name)
                {
                    adDic.Add(_name, info);
                    return info;
                }
            }
        }

        return null;
    }

    public ABConfigInfo GetInfoByAB(string _ab)
    {
        ABConfigInfo info;
        for (int i = 0; i < data.Count; i++)
        {
            info = data[i];

            if (info.ab == _ab)
            {
                return info;
            }
        }
        return null;
    }
}

[Serializable]
public class ABConfigInfo : BaseConfigInfo
{
    public List<string> names = new List<string>();
    public string ab;
    public string type;
}