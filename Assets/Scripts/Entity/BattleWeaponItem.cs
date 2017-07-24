using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleWeaponItem : BattleItem {

	private UISprite avatarSprite;
	private GameObject weaponObj;
	private UILabel weaponNumLabel;

	
	public BattleTrooperData data{
		set{
			btd = value;
			avatarSprite.spriteName = value.tid;
			weaponNumLabel.text = value.weaponCost.ToString();
			WeaponCost = value.weaponCost;
		}
	}

	public int weaponCost{
		set{
			WeaponCost = value;
			weaponNumLabel.text = value.ToString();
		}
	}
	
	public new void Init()
	{
		base.Init ();
		avatarSprite = transform.Find ("Icon").GetComponent<UISprite>();
		weaponObj = transform.Find ("Weapon").gameObject;
		weaponNumLabel = weaponObj.transform.Find ("WeaponCount").GetComponent<UILabel> ();

	}
	
	void OnClick()
	{
		if(GetComponent<UIButton>().enabled)
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.OnClickBattleTrooper (gameObject);
	}
}
