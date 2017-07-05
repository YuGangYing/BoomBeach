using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using BoomBeach;

public class UIButtonEvent : MonoBehaviour {

	private static UIButtonEvent instance;

	public static UIButtonEvent Instance
	{
		get{ return instance; }
	}
	void Awake()
	{
		if (instance == null)
						instance = this;
	}

	public void OnAchivementClick()
	{
		//PopManage.Instance.ShowTestWin ();
		PopManage.Instance.ShowAchievementsWin();
	}
	
	//int i = 0;
	public void OnConfigClick()
	{
		//TestReplay.Export ();
		//PopManage.Instance.ShowSetWin();
	}

	public void OnFriendClick()
	{
		//TestReplay.Import ();
		//Debug.Log ("OnFriendClick");
		//SMGameEnvironment.Instance.SceneManager.TransitionPrefab = "Transitions/SMTilesTransition";
		//SMGameEnvironment.Instance.SceneManager.LoadLevel ("MainScene");
		PopManage.Instance.ShowTopPlayersWin();
	}



	public void OnLevelClick()
	{
		PopUI popBox = UIButton.current.transform.Find ("PopBox").GetComponent<PopUI>();
		PopUI.closePops(popBox);
		if(popBox.gameObject.activeSelf)
			popBox.pop (false);
		else
			popBox.pop (true);

	}

	public void OnMedalClick()
	{
		PopUI popBox = UIButton.current.transform.Find ("PopBox").GetComponent<PopUI>();
		PopUI.closePops(popBox);
		if(popBox.gameObject.activeSelf)
			popBox.pop (false);
		else
			popBox.pop (true);
	}


	private void PopResouceBox()
	{
		if (ResourceBarPop.Instance.button == null||ResourceBarPop.Instance.button != UIButton.current) 
		{
			ResourceBarPop.Instance.setPop (UIButton.current);
			PopUI popBox = ResourceBarPop.Instance.popBox;
			PopUI.closePops (popBox);

			if(!popBox.gameObject.activeSelf)
				popBox.pop (true);
		} 
		else
		{
			PopUI popBox = ResourceBarPop.Instance.popBox;
			PopUI.closePops (popBox);
			if(popBox.gameObject.activeSelf)
				popBox.pop (false);
			else
				popBox.pop (true);
		}
	}

	public void OnGoldIcoClick()
	{
		ScreenUIManage.Instance.UpdateGoldResourceTip ();
		PopResouceBox ();
	}

	public void OnWoodIcoClick()
	{
		ScreenUIManage.Instance.UpdateWoodResourceTip ();
		PopResouceBox ();
	}

	public void OnStoneIcoClick()
	{
		ScreenUIManage.Instance.UpdateStoneResourceTip ();
		PopResouceBox ();
	}

	public void OnIronIcoClick()
	{
		ScreenUIManage.Instance.UpdateIronResourceTip ();
		PopResouceBox ();
	}

	public void OnDiamondIcoClick()
	{
		PopManage.Instance.ShowDiamondPanel ();
	}

    /// <summary>
    /// 商城按钮被按下事件
    /// </summary>
	public void OnShopIcoClick()
	{
		PopManage.Instance.ShowShopPanel ();

		UIManager.GetInstance().GetController<ShopCtrl>().ShowPanel();
	}


	public int currentExp = 100;
	public int UpgradeExp = 340;
	public int nextGrade = 3;
	public bool playAni;

	public int currentGold = 1000;
	public int maxGold = 10000;

	public int currentDiamond = 1;

	public GameObject MainCamera;
	public GameObject WorldMap;

	//切换到主场景;
	public void OnHomeIcoClick()
	{
		//Debug.Log("OnHomeIcoClick");
		GameLoader.Instance.SwitchScene(SceneStatus.HOME,0,0);
	}


	//切换到世界地图;
	public void OnWorldIcoClick()
	{
		MoveOpEvent.Instance.ResetBuild();
		GameLoader.Instance.SwitchScene(SceneStatus.WORLDMAP);
		//GameLoader.Instance.SwitchScene(SceneStatus.BATTLEREPLAY,0,0,0,0,328,2);

	}



	public void OnOpenEnemyActivityWin()
	{
		PopManage.Instance.ShowEnemyActivityWin(true);
	}

	//查看敌方时切换到攻击场景;
	public void OnClickAttackIco()
	{
		if (CSVManager.GetInstance().experienceLevelsList.ContainsKey(DataManager.GetInstance().userInfo.exp_level.ToString())){
			
			ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
			//花费;
			//Debug.Log(el.AttackCost);
			ISFSObject dt = Helper.getCostDiffToGems("",3,true,el.AttackCost);
			int gems = dt.GetInt("Gems");
			//Debug.Log(gems);
			//资源不足，需要增加宝石才行;
			if (gems > 0){
                

				PopManage.Instance.ShowDialog(
					dt.GetUtfString("msg"),
					dt.GetUtfString("title"),
					true,
					PopDialogBtnType.ImageBtn,
					true,
					dt,
					onBattleDialogYes,
					null,
					gems.ToString()
					);
			}else{
				//GameLoader.Instance.StartBattle(dt.GetInt("Gold"),0,0,0);
				GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,Globals.LastSceneUserId,Globals.LastSceneRegionsId,dt.GetInt("Gold"),0);
			}
		}else{
			
		}
	}


    /// <summary>
    /// 战斗开始触发
    /// </summary>
    /// <param name="dt"></param>
	private void onBattleDialogYes(ISFSObject dt){
		//Debug.Log("onDialogYes");
		//Debug.Log(dt.GetDump());
		
		if (DataManager.GetInstance().userInfo.diamond_count >= dt.GetInt("Gems")){
			GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,Globals.LastSceneUserId,Globals.LastSceneRegionsId,dt.GetInt("Gold"),dt.GetInt("Gems"));
			//GameLoader.Instance.StartBattle(dt.GetInt("Gold"),dt.GetInt("Gems"),0,0);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog(null,null);
		}
	}


	//搜索对手;
	public void OnClickSearchIco()
	{
		if (CSVManager.GetInstance().experienceLevelsList.ContainsKey(DataManager.GetInstance().userInfo.exp_level.ToString())){
			
			ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
			//花费;
			//Debug.Log(el.AttackCost);
			int SearchCost = (int) (el.AttackCost * Globals.SearchCostFactor);
			ISFSObject dt = Helper.getCostDiffToGems("",3,true,SearchCost);
			int gems = dt.GetInt("Gems");
			//Debug.Log(gems);
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				PopManage.Instance.ShowDialog(
					dt.GetUtfString("msg"),
					dt.GetUtfString("title"),
					true,
					PopDialogBtnType.ImageBtn,
					true,
					dt,
					onSearchDialogYes,
					null,
					gems.ToString()
					);
			}else{
				//GameLoader.Instance.StartBattle(dt.GetInt("Gold"),0,0,0);
				GameLoader.Instance.SwitchScene(SceneStatus.ENEMYVIEW,0,0,dt.GetInt("Gold"),0);
			}
		}else{
			
		}
	}
	
	private void onSearchDialogYes(ISFSObject dt){
		//Debug.Log("onDialogYes");
		//Debug.Log(dt.GetDump());
		
		if (DataManager.GetInstance().userInfo.diamond_count >= dt.GetInt("Gems")){
			GameLoader.Instance.SwitchScene(SceneStatus.ENEMYVIEW,0,0,dt.GetInt("Gold"),dt.GetInt("Gems"));
			//GameLoader.Instance.StartBattle(dt.GetInt("Gold"),dt.GetInt("Gems"),0,0);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog(null,null);
		}
	}


	//结束战斗;
	public void OnClickEndBattle()
	{
		BattleData.Instance.BattleIsEnd = true;
		//已派出的兵与已死亡的兵相等，表示派出的兵全死完或未派兵，直接弹窗;
		if(BattleData.Instance.BattleIsStart&&BattleData.Instance.AllocateTrooperList.Count==BattleData.Instance.DeadTrooperList.Count){
			if(ScreenUIManage.Instance!=null)
				ScreenUIManage.Instance.battleResultWin.ShowResultWin();
			UIManager.GetInstance ().GetController<BattleResultCtrl>().ShowPanel ();
		}
		else
		{
			if(BattleData.Instance.AllocateTrooperList.Count>0)
				AudioPlayer.Instance.PlayMusic("reef_retreat_01");
			AITask.Instance.ResetAll();
			//未死完，通知退兵;
			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				if(!charInfo.isDead)
				{
					charInfo.Dest = charInfo.RetreatPoint;
					charInfo.IsRetreating = true;
					charInfo.IsOnlyMove = true;
					charInfo.path = null;
					charInfo.AttackState = AISTATE.STANDING;
					charInfo.trooperCtl.CMDFindPath();
				}
				
			}
		}
	}
}
