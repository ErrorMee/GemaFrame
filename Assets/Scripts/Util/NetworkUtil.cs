using System;
using UnityEngine;


public enum NetworkType : int
{
    NotNetwork,//无网络
    Wifi,//wifi
    MobileNetwork,//移动网络3G/4G
}


public class NetworkUtil
{
    public static NetworkType GetNetworkType()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return NetworkType.Wifi;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return NetworkType.NotNetwork;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return NetworkType.Wifi;
        }
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return NetworkType.MobileNetwork;
        }
        return NetworkType.NotNetwork;
    }
}