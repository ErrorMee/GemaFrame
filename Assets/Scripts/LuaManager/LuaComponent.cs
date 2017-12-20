using System;
using UnityEngine;
using XLua;

public class LuaComponent : MonoBehaviour
{

    public string luaScriptPath;
    private LuaTable scriptEnv;

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    void Awake()
    {
        if (!string.IsNullOrEmpty(luaScriptPath))
        {
            scriptEnv = LuaManager.Instance.luaEnv.NewTable();
            LuaTable meta = LuaManager.Instance.luaEnv.NewTable();
            meta.Set("__index", LuaManager.Instance.luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            scriptEnv.Set("self", this);

            LuaManager.Instance.luaEnv.DoString(LuaManager.Instance.DoLoad(luaScriptPath), luaScriptPath, scriptEnv);

            Action luaAwake = scriptEnv.Get<Action>("awake");
            scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("ondestroy", out luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake();
            }
        }
        else
        {
            GLog.Error("luaScriptPath is null :" + gameObject.name);
        }
    }

    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
    }
}
