using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleWeaponList : MonoBehaviour {

	public BattleWeaponItem[] LineUpBattleWeaponItems;
	public BattleWeaponItem[] LineDownBattleWeaponItems;
	public List<BattleItem> BattleItems;
	private static BattleWeaponList instance;
	public static BattleWeaponList Instance{
		get{ return instance; }
	}

	//private List<BattleTrooperData> _TrooperData;
	private UILabel WeaponTotalLabel;
	private GameObject WeaponTotalObj;
	public void InitTrooper(List<BattleTrooperData> _TrooperData){

		BattleItems = new List<BattleItem> ();
		Vector3 weaponPos = WeaponTotalObj.transform.localPosition;
		if(_TrooperData.Count>4)
		{
			WeaponTotalObj.transform.localPosition = new Vector3(weaponPos.x,430f,weaponPos.z);
		}
		else
		{
			WeaponTotalObj.transform.localPosition = new Vector3(weaponPos.x,240f,weaponPos.z);
		}
		
		for(int i=0;i<LineDownBattleWeaponItems.Length;i++)
		{
			if(i < _TrooperData.Count)
			{
				_TrooperData[i].id = _TrooperData[i].id;// BattleData.Instance.WeaponIdx2;
				//BattleData.Instance.WeaponIdx2++;
				LineDownBattleWeaponItems[i].gameObject.SetActive(true);
				LineDownBattleWeaponItems[i].data = _TrooperData[i];
				BattleItems.Add(LineDownBattleWeaponItems[i]);
				LineDownBattleWeaponItems[i].isDisabled =false;
				LineDownBattleWeaponItems[i].GetComponent<BattleItem>().current = false;
			}
			else
			{
				LineDownBattleWeaponItems[i].gameObject.SetActive(false);
			}
		}
		
		for(int i=0;i<LineUpBattleWeaponItems.Length;i++)
		{
			if(i + 4 < _TrooperData.Count)
			{
				_TrooperData[i].id = _TrooperData[i].id;// BattleData.Instance.WeaponIdx2;
				//BattleData.Instance.WeaponIdx2++;
				LineUpBattleWeaponItems[i].gameObject.SetActive(true);
				LineUpBattleWeaponItems[i].data = _TrooperData[i+4];
				BattleItems.Add(LineUpBattleWeaponItems[i]);
				LineUpBattleWeaponItems[i].isDisabled =false;
				LineUpBattleWeaponItems[i].GetComponent<BattleItem>().current = false;
			}
			else
			{
				LineUpBattleWeaponItems[i].gameObject.SetActive(false);
			}
		}

	}

	public int totalWeapon{
		set{
			WeaponTotalLabel.text = value.ToString();
		}
	}
	
	
	public void Init()
	{
		LineUpBattleWeaponItems = transform.Find ("LineUp").GetComponentsInChildren<BattleWeaponItem> (true);
		LineDownBattleWeaponItems = transform.Find ("LineDown").GetComponentsInChildren<BattleWeaponItem> (true);
		BattleItems = new List<BattleItem> ();
		for(int i=0;i<LineUpBattleWeaponItems.Length;i++)
		{
			LineUpBattleWeaponItems[i].Init();
		}
		for(int i=0;i<LineDownBattleWeaponItems.Length;i++)
		{
			LineDownBattleWeaponItems[i].Init();
		}
		WeaponTotalObj = transform.Find ("WeaponTotal").gameObject;
		WeaponTotalLabel = WeaponTotalObj.transform.Find ("count").GetComponent<UILabel> ();
		instance = this;
	}
}
