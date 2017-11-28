using UnityEngine;
using System;
using XLua;

[LuaCallCSharp]
public class LuaUIBase : UIBase
{
    public TextAsset luaScript;
    private LuaTable scriptEnv;

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    void Awake()
    {
        scriptEnv = LuaManager.Instance.luaEnv.NewTable();
        LuaTable meta = LuaManager.Instance.luaEnv.NewTable();
        meta.Set("__index", LuaManager.Instance.luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();
        scriptEnv.Set("self", this);
        LuaManager.Instance.luaEnv.DoString(luaScript.text, gameObject.name, scriptEnv);

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
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