using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaManager : SingletonBehaviour<LuaManager>
{
    private static string REQUIRE_FORMAT = "require('{0}')";

    public LuaEnv luaEnv;

    private static float lastGCTime = 0;
    private const float GCInterval = 1;//1 second 

    private void Awake()
    {
        luaEnv = new LuaEnv();
    }

    public void Init()
    {
        luaEnv.DoString(LuaManager.GetRequireString("LuaSetting"), "LuaSetting");

        //沙盒是不传递到require的文件的,不适用lua使用self
        luaEnv.AddLoader((ref string filepath) =>
        {
            GLog.Log("LuaLoader > " + filepath);
            byte[] buffer = LuaLoader(filepath);
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }
            else
            {
#if UNITY_EDITOR
                if (AssetManager.bundleLoadMode)
                {
                    EncryptUtil.Decryption(buffer);
                }
#else
            EncryptUtil.Decryption(buffer);
#endif
                return buffer;
            }
        });

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

    public string DoLoad(string luaScriptPath)
    {
        if (luaScriptPath.EndsWith(".lua"))
        {
            int index = luaScriptPath.LastIndexOf('.');
            luaScriptPath = luaScriptPath.Substring(0, index);
        }

        byte[] buffer = LuaLoader(luaScriptPath);
        if (buffer == null || buffer.Length == 0)
        {
            return null;
        }
        else
        {
#if UNITY_EDITOR
            if (AssetManager.bundleLoadMode)
            {
                EncryptUtil.Decryption(buffer);
            }
#else
            EncryptUtil.Decryption(buffer);
#endif
            string txt = System.Text.Encoding.Default.GetString(buffer);
            return txt;
        }
    }

    public static string GetRequireString(string luaScriptPath)
    {
        if (luaScriptPath.EndsWith(".lua"))
        {
            int index = luaScriptPath.LastIndexOf('.');
            luaScriptPath = luaScriptPath.Substring(0, index);
        }
        return string.Format(LuaManager.REQUIRE_FORMAT, luaScriptPath);
    }

    private byte[] LuaLoader(string relativePath)
    {
        string fullPath = GetLuaFullPath(relativePath);
        return HttpManager.Instance.GetBytes(fullPath);
    }

    private string GetLuaFullPath(string relativePath)
    {
        string fullPath;

#if UNITY_EDITOR
        if (!AssetManager.bundleLoadMode)
        {
            fullPath = Application.dataPath + "/Lua/" + relativePath + ".lua";
            return fullPath;
        }
#endif
        uint hash = EncryptUtil.FileNameHash(relativePath + ".lua");

        return PathUtil.GetAssetPath("lua/" + hash);
    }

}
