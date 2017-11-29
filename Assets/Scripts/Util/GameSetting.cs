using System;
using UnityEngine;

[Serializable]
public class GameSetting : ScriptableObject
{
    /// <summary>
    /// 是否开启热更
    /// </summary>
    public bool patchOpen = false;

    /// <summary>
    /// 是否使用模拟地址进行热更
    /// </summary>
    public bool patchPathSimulation = true;

    /// <summary>
    /// 热更地址(本地模拟)
    /// </summary>
    [TextArea(2,3)]
    public string patchRootPathSimulation;

    /// <summary>
    /// 热更地址(服务器)
    /// </summary>
    [TextArea(2, 3)]
    public string patchRootPathWeb;

    /// <summary>
    /// 加载资源是否使用生成的bundle 
    /// false时直接加载原始资源
    /// </summary>
    public bool loadModeIsBundle = false;

    private void OnEnable()
    {
        patchRootPathSimulation = Application.dataPath + "/../SimulationWebAddress/patchs/";
    }

    public string GetPatchRootPath()
    {
        if (patchPathSimulation)
        {
            return patchRootPathSimulation;
        }
        return patchRootPathWeb;
    }

}