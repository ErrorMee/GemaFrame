
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UIBase
{
    public InputField accountTxt;
    public Transform loginBtn;

    private void Awake()
    {
        string defaultAccount = PlayerPrefs.GetString("account");
        if(defaultAccount == null || defaultAccount == "")
        {
            defaultAccount = "test1";
        }
        accountTxt.text = defaultAccount;

        
    }

    private void Start()
    {
        EventTriggerListener.Get(loginBtn.gameObject).onClick = OnClickLogin;
    }

    private void OnClickLogin(GameObject go)
    {
        GLog.Log("登录账号：" + accountTxt.text);
        if(accountTxt.text == null || accountTxt.text == "")
        {
            GLog.Error("账号为空");
            return;
        }

        UIManager.Instance.OpenUI("MainUI");
    }
}
