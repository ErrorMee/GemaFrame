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

        Application.targetFrameRate = 60;

        gameObject.AddComponent<HttpManager>();
        gameObject.AddComponent<PatchManager>();
        gameObject.AddComponent<AssetManager>();
        gameObject.AddComponent<UIManager>();
        gameObject.AddComponent<LuaManager>();

        GameEvent.RegisterEvent(GameEventType.PatchComplete, OnPatchComplete);
        GameEvent.RegisterEvent(GameEventType.AssetManagerReady, OnAssetManagerReady);
        GameEvent.RegisterEvent(GameEventType.LuaManagerReady, OnLuaManagerReady);
    }

    void Start()
    {
        PatchManager.Instance.Init();
    }

    void OnPatchComplete(object[] param)
    {
        GameEvent.UnregisterEvent(GameEventType.PatchComplete, OnPatchComplete);

        AssetManager.Instance.Init();
    }

    void OnAssetManagerReady(object[] param)
    {
        GameEvent.UnregisterEvent(GameEventType.AssetManagerReady, OnAssetManagerReady);
        LuaManager.Instance.Init();
    }

    void OnLuaManagerReady(object[] param)
    {
        GameEvent.UnregisterEvent(GameEventType.LuaManagerReady, OnLuaManagerReady);
        UIManager.Instance.OpenUI("LoginUI");
    }

}
