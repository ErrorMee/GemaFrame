using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    FULL,   //全屏
    WINDOW, //窗口
    PENDANT //挂件
}

[Serializable]
public class UIContext
{
    [HideInInspector]
    public string uiName;
    public UIType uiType;

    public UIContext Clone()
    {
        UIContext clone = new UIContext();
        clone.uiName = uiName;
        clone.uiType = uiType;
        return clone;
    }

    public bool CanRecover()
    {
        if (uiType == UIType.FULL || uiType == UIType.WINDOW)
        {
            return true;
        }
        return false;
    }
}

public class UIBase : MonoBehaviour
{
    public UIContext uiContext;

    protected void CloseSelf()
    {
        UIManager.Instance.CloseUI(uiContext.uiName);
    }
}
