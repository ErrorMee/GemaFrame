using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : SingletonBehaviour<Game>
{
    public Transform canvasTrans;

    public GameSetting gameSetting;

    void Awake()
    {
        if (gameSetting == null)
        {
            gameSetting = Resources.Load<GameSetting>("GameSetting");
        }
        gameObject.AddComponent<HttpManager>();
        gameObject.AddComponent<PatchManager>();

        GameEvent.RegisterEvent(GameEventType.PatchComplete, OnPatchComplete);
        GameEvent.RegisterEvent(GameEventType.AssetManagerReady, OnAssetManagerReady);
        GameEvent.RegisterEvent(GameEventType.LuaManagerReady, OnLuaManagerReady);
    }

    void Start()
    {
        PatchManager.Instance.OnStart();
    }

    void OnPatchComplete(object[] param)
    {
        GLog.Log("OnPatchComplete", Color.green);
        GameEvent.UnregisterEvent(GameEventType.PatchComplete, OnPatchComplete);
        gameObject.AddComponent<AssetManager>();
        gameObject.AddComponent<UIManager>();
    }

    void OnAssetManagerReady(object[] param)
    {
        GLog.Log("OnAssetManagerReady", Color.green);
        GameEvent.UnregisterEvent(GameEventType.AssetManagerReady, OnAssetManagerReady);
        gameObject.AddComponent<LuaManager>();
    }

    void OnLuaManagerReady(object[] param)
    {
        GLog.Log("OnLuaManagerReady", Color.green);
        GameEvent.UnregisterEvent(GameEventType.LuaManagerReady, OnLuaManagerReady);
        UIManager.Instance.OpenUI("LoginUI");
    }

}
