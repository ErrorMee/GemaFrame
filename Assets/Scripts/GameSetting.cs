using System;
using UnityEngine;

[Serializable]
public class GameSetting : ScriptableObject
{
    [Header("---------- Patch ----------")]

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

    [Space(10)]
    [Header("---------- Load ----------")]

    /// <summary>
    /// 加载资源是否使用生成的bundle 
    /// false时直接加载原始资源
    /// </summary>
    [Tooltip("加载资源是否使用bundle文件")]
    public bool loadModeIsBundle = true;

    [Space(10)]
    [Header("---------- Other ----------")]

    /// <summary>
    /// 游戏帧率
    /// </summary>
    [ContextMenuItem("Reset", "ResetFPS")]
    [Range(30, 100)]
    public int fps = 45;
    void ResetFPS()
    {
        fps = 45;
    }

    /// <summary>
    /// 热更地址(服务器)
    /// </summary>
    public bool writeLog = true;

    /// <summary>
    /// 设置游戏
    /// </summary>
    public void SetGame()
    {
        Application.targetFrameRate = fps;
        if (writeLog)
        {
            GLog.SetWriteLog();
        }
    }

    public void CloseGame()
    {
        GLog.CloseGame();
    }

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