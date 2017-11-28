
using UnityEngine;
using UnityEngine.UI;

public class MainUI : UIBase
{
    public Transform bagBtn;
    public Transform fightBtn;

    private void Start()
    {
        EventTriggerListener.Get(bagBtn.gameObject).onClick = OnClickBag;
        EventTriggerListener.Get(fightBtn.gameObject).onClick = OnClickFight;
    }

    private void OnClickBag(GameObject go)
    {
        UIManager.Instance.OpenUI("BagUI");
    }

    private void OnClickFight(GameObject go)
    {
        UIManager.Instance.OpenUI("FightUI");
    }
}
