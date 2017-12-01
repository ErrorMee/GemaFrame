using System;
using System.Collections.Generic;
using UnityEngine;
public class GameEvent
{
    static Dictionary<string, List<System.Action<object[]>>> eventDic = new Dictionary<string, List<System.Action<object[]>>>();

    public static void RegisterEvent(string eventName, System.Action<object[]> eventHandle)
    {
        List<System.Action<object[]>> evt;
        if (eventDic.TryGetValue(eventName, out evt))
        {
            evt.Add(eventHandle);
        }
        else
        {
            evt = new List<System.Action<object[]>>();
            evt.Add(eventHandle);
            eventDic[eventName] = evt;
        }
    }

    public static void UnregisterEvent(string eventName, System.Action<object[]> eventHandle)
    {
        List<System.Action<object[]>> evtList;
        if (eventDic.TryGetValue(eventName, out evtList))
        {
            evtList.Remove(eventHandle);
        }
    }

    public static void SendEvent(string eventName, params object[] param)
    {
        if (eventName == GameEventType.GameFlow)
        {
            GameFlow gf = (GameFlow)(param[0]);
            GLog.Log("GameFlow:" + (int)gf + " : " + gf.ToString(), Color.yellow);
        }

        List<System.Action<object[]>> evtList;
        if (eventDic.TryGetValue(eventName, out evtList))
        {
            System.Action<object[]>[] evts = evtList.ToArray();
            foreach (System.Action<object[]> evt in evts)
            {
                if (evt != null)
                {
                    try
                    {
                        evt(param);
                    }
                    catch (Exception ex)
                    {
                        GLog.Error(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }
    }
}