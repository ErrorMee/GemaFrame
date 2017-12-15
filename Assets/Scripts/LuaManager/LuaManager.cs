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

        luaEnv.AddLoader((ref string filename) => {
            GLog.Log("require > " + filename);
            return null;
        });
    }

    public void Init()
    {
        luaEnv.DoString("require 'LuaSetting'");
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

    public void GetLuaString(string fileName, System.Action<string> callBack)
    {
        ResourceManager.Instance.LoadAsync<TextAsset>(fileName, (res) =>
        {
            if (res == null)
            {
                GLog.Error("error file name : " + fileName);
                return;
            }

            if (callBack != null)
            {
                callBack(res.text);
            }
        });
    }
}
