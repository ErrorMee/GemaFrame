using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FightUI : UIBase
{
    public Transform closeBtn;

    public Text title;

    private void Start()
    {
        EventTriggerListener.Get(closeBtn.gameObject).onClick = OnClickClose;

        SDKManager.Instance.TestUnityCallAndorid();
    }

    private void OnClickClose(GameObject go)
    {
        //SDKManager.Instance.TestAndoridCallUnity("FightUI_1", "TestAndoridCall");
        CloseSelf();
    }

    public void TestAndoridCall(string str)
    {
        title.text = str;
    }
}
