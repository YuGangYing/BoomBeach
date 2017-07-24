using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyViewTrooperList : MonoBehaviour {

	private BattleViewTrooperItem[] TrooperItems;
	//private List<BattleTrooperData> _TrooperData;
	public void InitTrooper(List<BattleTrooperData> _TrooperData)
	{
		for(int i=0;i<TrooperItems.Length;i++)
		{

			if(i < _TrooperData.Count)
			{
				TrooperItems[i].gameObject.SetActive(true);
				TrooperItems[i].data = _TrooperData[i];
			}
			else
			{
				TrooperItems[i].gameObject.SetActive(false);
			}
		}

	}

	public void Init()
	{
		TrooperItems = GetComponentsInChildren<BattleViewTrooperItem> (true);
		for(int i=0;i<TrooperItems.Length;i++)
		{
			TrooperItems[i].Init();
		}
	}
}
