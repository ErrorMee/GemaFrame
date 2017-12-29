using UnityEngine;
using System.Collections;

public class FightUI : UIBase
{
    public Transform closeBtn;

    private void Start()
    {
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnClickClose;


        TestCrush();
    }

    private void OnClickClose(GameObject go)
    {
        CloseSelf();
    }

    private void TestCrush()
    {
        int[] test = new int[1];
        test[2] = 2;
    }
}
