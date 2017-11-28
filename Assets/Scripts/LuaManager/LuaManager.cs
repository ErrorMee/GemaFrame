using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaManager : SingletonBehaviour<LuaManager>
{
    public LuaEnv luaEnv;

    private static float lastGCTime = 0;
    private const float GCInterval = 1;//1 second 

    private void Awake()
    {
        luaEnv = new LuaEnv();
    }

    private void Start()
    {
        GameEvent.SendEvent(GameEventType.LuaManagerReady);
    }

    private void Update()
    {
        if (Time.time - LuaManager.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaManager.lastGCTime = Time.time;
        }
    }
}
