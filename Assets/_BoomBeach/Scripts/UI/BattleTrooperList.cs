using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleTrooperList : MonoBehaviour {

	public BattleTrooperItem[] LineUpBattleTrooperItems;
	public BattleTrooperItem[] LineDownBattleTrooperItems;
	public List<BattleItem> BattleItems;
	private static BattleTrooperList instance;
	public static BattleTrooperList Instance{
		get{return instance; }
	}

	//private List<BattleTrooperData> _TrooperData;
	public void InitTrooper(List<BattleTrooperData> _TrooperData){

			BattleItems = new List<BattleItem> ();
			for(int i=0;i<LineDownBattleTrooperItems.Length;i++)
			{
				if(i < _TrooperData.Count)
				{
					LineDownBattleTrooperItems[i].gameObject.SetActive(true);
					LineDownBattleTrooperItems[i].data = _TrooperData[i];
					BattleItems.Add(LineDownBattleTrooperItems[i]);
					LineDownBattleTrooperItems[i].isDisabled = false;
					BattleItem item = LineDownBattleTrooperItems[i].GetComponent<BattleItem>();
					if(i==0)
					{						
						item.current = true;						
						BattleData.Instance.currentSelectBtd = item.btd;
					}
					else
					{
						item.current = false;	
					}
				}
				else
				{
					LineDownBattleTrooperItems[i].gameObject.SetActive(false);
				}
			}

			for(int i=0;i<LineUpBattleTrooperItems.Length;i++)
			{
				//_TrooperData[i+3]!=null)
				if(i+4 < _TrooperData.Count)
				{
					LineUpBattleTrooperItems[i].gameObject.SetActive(true);
					LineUpBattleTrooperItems[i].data = _TrooperData[i+4];
					BattleItems.Add(LineUpBattleTrooperItems[i]);
					LineUpBattleTrooperItems[i].isDisabled = false;
					LineUpBattleTrooperItems[i].GetComponent<BattleItem>().current = false;
				}
				else
				{
					LineUpBattleTrooperItems[i].gameObject.SetActive(false);
				}
			}
		
	}


	public void Init()
	{
		LineUpBattleTrooperItems = transform.Find ("LineUp").GetComponentsInChildren<BattleTrooperItem> (true);
		LineDownBattleTrooperItems = transform.Find ("LineDown").GetComponentsInChildren<BattleTrooperItem> (true);
		BattleItems = new List<BattleItem> ();
		for(int i=0;i<LineUpBattleTrooperItems.Length;i++)
		{
			LineUpBattleTrooperItems[i].Init();
		}
		for(int i=0;i<LineDownBattleTrooperItems.Length;i++)
		{
			LineDownBattleTrooperItems[i].Init();
		}
		instance = this;
	}
}
