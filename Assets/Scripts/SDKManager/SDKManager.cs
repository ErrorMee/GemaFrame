using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager : SingletonBehaviour<SDKManager>
{
    private AndroidJavaObject jo;

    public void Init()
    {
        GLog.Log("Application.platform " + Application.platform);
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }

    }

    public void TestUnityCallAndorid()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string time = jo.Call<string>("getNowTime");
            jo.Call("showToast", new object[] { time });
        }
    }

    public void TestAndoridCallUnity(string uObjName, string methodName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            jo.Call("CallUnityFunc", uObjName, methodName);
        }
    }
}
