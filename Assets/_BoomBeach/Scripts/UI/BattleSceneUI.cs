using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;



public class BattleSceneUI : MonoBehaviour {

	public static List<BattleSceneUI> BattleSceneUIS;
	public static void SetBattleUI()
	{
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE||DataManager.GetInstance().sceneStatus==SceneStatus.BATTLEREPLAY)
		{		
			if(BattleSceneUIS!=null)
			foreach(BattleSceneUI battleSceneUI in BattleSceneUIS)
			{
				if(BattleData.Instance.BattleIsStart)
				{
					if(battleSceneUI.isHideWhileStart)
					{
						battleSceneUI.gameObject.SetActive (false);
					}
				}

				if(BattleData.Instance.BattleIsEnd)
				{
					if(battleSceneUI.isHideWhileEnd)
					{
						battleSceneUI.gameObject.SetActive (false);
					}
				}

				if(!BattleData.Instance.BattleIsStart&&!BattleData.Instance.BattleIsEnd)
				{
					battleSceneUI.gameObject.SetActive (true);
				}
			}
		}
	}

	public bool isHideWhileStart;

	public bool isHideWhileEnd;

	// Use this for initialization
	void Start () 
	{
		if (BattleSceneUI.BattleSceneUIS == null)
			BattleSceneUI.BattleSceneUIS = new List<BattleSceneUI> ();

		if(!BattleSceneUI.BattleSceneUIS.Contains(this))
		BattleSceneUI.BattleSceneUIS.Add (this);
	}

}
