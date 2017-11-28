using UnityEngine;
using System.Collections;

public class FightUI : UIBase
{
    public Transform closeBtn;

    private void Start()
    {
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnClickClose;
    }

    private void OnClickClose(GameObject go)
    {
        CloseSelf();
    }
}
