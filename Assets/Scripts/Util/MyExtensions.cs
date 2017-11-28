
using UnityEngine;

public static class MyExtensions
{
    public static T GetOrCreate<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t)
        {
            return t;
        }

        return go.AddComponent<T>();
    }
}
