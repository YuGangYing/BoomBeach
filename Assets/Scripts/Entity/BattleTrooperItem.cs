using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTrooperItem : BattleItem {

	private UISprite avatarSprite;
	private UILabel trooperNumLabel;
	private GameObject weaponObj;
	private UILabel weaponNumLabel;




	public BattleTrooperData data{
		set{
			btd = value;
			avatarSprite.spriteName = value.tid;
			trooperNumLabel.text = "X"+value.num.ToString();
			weaponNumLabel.text = value.weaponCost.ToString();
			if(value.weaponCost>0)
			{
				WeaponCost = value.weaponCost;
				weaponObj.gameObject.SetActive(true);
			}
			else
			{
				weaponObj.gameObject.SetActive(false);
			}
		}
	}

	public int trooperNum{
		set{
			WeaponCost = value;
			trooperNumLabel.text = "X"+value.ToString();
		}
	}

	public new void Init()
	{
		base.Init ();
		avatarSprite = transform.Find ("Icon").GetComponent<UISprite>();
		trooperNumLabel = transform.Find ("TrooperCount").GetComponent<UILabel> ();
		weaponObj = transform.Find ("Weapon").gameObject;
		weaponNumLabel = weaponObj.transform.Find ("WeaponCount").GetComponent<UILabel> ();

	}

	void OnClick()
	{
		if(GetComponent<UIButton>().enabled)
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.OnClickBattleTrooper (gameObject);
	}

}
