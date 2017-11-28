using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameFlow
{
    PatchConfigLoad,            //热更配置加载
    PatchFileLocalLoad,         //本地热更文件加载   
    PatchFileRemoteLoad,        //远端热更文件加载   
    PatchStart,                 //热更开始 
    PatchComplete,              //热更结束
}

public class GameEventType
{
    //游戏流程事件
    public const string GameFlow = "GameFlow";

    //热更结束
    public const string PatchComplete = "PatchComplete";

    //AssetManager初始化結束
    public const string AssetManagerReady = "AssetManagerReady";

    //LuaManager初始化结束
    public const string LuaManagerReady = "LuaManagerReady";
}