
local accountTxt;
local loginBtn;

function awake()
	print("lua awake...");

	accountTxt = self.transform:Find("Account"):GetComponent("InputField");
	loginBtn = self.transform:Find("Login");

end

function start()
	print(self.name .. " start...");
	
	local defaultAccount = CS.UnityEngine.PlayerPrefs.GetString("account");
    if(defaultAccount == nil or defaultAccount == "")then
        defaultAccount = "test1";
    end
    accountTxt.text = defaultAccount;

	CS.EventTriggerListener.Get(loginBtn.gameObject).onClick = OnClickLogin;
end

function OnClickLogin(go)
	print("OnClickLogin");
end

function update()
	
end

function ondestroy()
    print("lua destroy")
end

function OnClickLogin(go)
	CS.UnityEngine.PlayerPrefs.SetString("account",accountTxt.text);
	CS.UIManager.Instance:OpenUI("MainUI");
end

