using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BoomBeach;

public class MainManager : SingleMonoBehaviour<MainManager>
{

	protected override void Awake ()
	{
		base.Awake ();
	}

	bool mPlayCloud;

	//切换游戏场景（非unity场景）
	public void LoadSceneStatus (SceneStatus sceneStatus)
	{
		switch (sceneStatus) {
		case SceneStatus.HOME:
			LoadHome ();
			break;
		case SceneStatus.FRIENDVIEW:
			LoadFriendView ();
			break;
		case SceneStatus.ENEMYVIEW:
			LoadEnemyView ();
			break;
		case SceneStatus.ENEMYBATTLE:
			LoadEnemyBattle ();
			break;
		case SceneStatus.BATTLEREPLAY:
			LoadBattleReplay ();
			break;
		case SceneStatus.HOMERESOURCE:
			LoadResourceIsland ();
			break;
		case SceneStatus.WORLDMAP:
			LoadWorldMap ();
			break;
		}
		Resources.UnloadUnusedAssets ();
		Globals.IsSceneLoaded = true;
		SceneLoader.Instance.EndLoad ();
	}

	private void LoadWorldMap ()
	{
	
	}

	private void LoadHome (int regions_id = 0)
	{
		Debug.Log ("LoadHome");
		CameraOpEvent.Instance.ResetIm (new Vector3 (-10, 30, 0));
		InitIsland (IslandType.playerbase);
		InitBuildings (DataManager.GetInstance.model.building_list);
		InitWorkers (15);
		Helper.HandleWolrdMap ();
		InitHomeUI ();
		InitHomeOnlyBuildings ();
		UIManager.GetInstance.GetController<NormalMsgCtrl> ().ShowPop ("已创建:" + DataManager.GetInstance.buildList.Count);
		AudioPlayer.Instance.PlayMusic ("home_music");
	}

	private void LoadFriendView ()
	{
		InitIsland (IslandType.playerbase);
		InitBuildings (DataManager.GetInstance.model.building_list);
		InitWorkers (15);
		AudioPlayer.Instance.PlayMusic ("home_music");
	}

	private void LoadEnemyBattle ()
	{
		InitIsland (IslandType.playerbase);
		InitBuildings (FilterBuildingForEnemyBattle ());
		InitBattleOnlyBuildings ();
		AudioPlayer.Instance.PlayMusic ("combat_planning_music");
	}

	private void LoadEnemyView ()
	{
		InitIsland (IslandType.playerbase);
		InitBuildings (DataManager.GetInstance.model.building_list);
		InitWorkers (15);
		AudioPlayer.Instance.PlayMusic ("home_music");
	}

	private void LoadResourceIsland ()
	{
		InitIsland (IslandType.small_a);//TODO 需要从region表里面读取
		InitBuildings (DataManager.GetInstance.model.building_list);
		InitWorkers (5);
		AudioPlayer.Instance.PlayMusic ("home_music");
	}

	private void LoadBattleReplay ()
	{
		//TODO
		AudioPlayer.Instance.PlayMusic ("combat_music");
	}

	private void InitIsland (IslandType islandType)
	{
		for (int i = 0; i < Islands.GetInstance.islands.Length; i++) {
			if (Islands.GetInstance.islands [i].type == Globals.islandType) {
				GameObject mineLand = Islands.GetInstance.islands [i].gameObject;
				mineLand.SetActive (true);
				Globals.SceneIsland = mineLand;
				Transform army_area_plane = mineLand.GetComponentInChildren<BeachData> ().transform;
				army_area_plane.GetComponent<MeshRenderer> ().enabled = false;
			} else {
				Islands.GetInstance.islands [i].gameObject.SetActive (false);
			}
		}
	}

	//初始化小岛的网格
	private void InitIslandGrid ()
	{
		//初始化a星矩阵;
		byte[,] mMatrix = new byte[64, 64]; 
		//初始化格子;
		Globals.GridArray = new GridInfo[Globals.GridTotal, Globals.GridTotal];
		for (int a = 0; a < Globals.GridTotal; a++) {
			for (int b = 0; b < Globals.GridTotal; b++) {
				GridInfo gridInfo = new GridInfo ();
				gridInfo.A = a;
				gridInfo.B = b;
				gridInfo.GridPosition = new Vector3 (a, 0f, b);
				gridInfo.standPoint = new Vector3 ((float)a + 0.5f, 0f, (float)b + 0.5f);
				gridInfo.isInArea = true;
				gridInfo.cost = Globals.GridEmptyCost;
				if (CSVManager.GetInstance.islandGridsDic [Globals.islandType.ToString ()] [a, b] == 1) {
					gridInfo.isInArea = false;
					gridInfo.cost = Globals.GridBuildCost;
				}
				Globals.GridArray [a, b] = gridInfo; 
			}
		}
		Globals.mMatrix = mMatrix;
	}

	//清空旧数据;
	private void ClearCaches ()
	{
		Transform characters = SpawnManager.GetInstance.characterContainer;
		Transform bullets = SpawnManager.GetInstance.bulletContainer;
		WorkerManage.Instance.ClearWorkers ();
		while (characters.childCount > 0) {
			DestroyImmediate (characters.GetChild (0).gameObject);
		}	
		while (bullets.childCount > 0) {
			DestroyImmediate (bullets.GetChild (0).gameObject);
		}
	}

	//炮艇和登陆艇在攻击玩家的场景不加载
	List<Network.BuildingModel> FilterBuildingForEnemyBattle ()
	{
		List<Network.BuildingModel> builingModels = new List<Network.BuildingModel> ();
		for (int i = 0; i < DataManager.GetInstance.model.building_list.Count; i++) {
			Network.BuildingModel model = DataManager.GetInstance.model.building_list [i];
			if (model.type == GameConstant.TID_BUILDING_LANDING_SHIP || model.type == GameConstant.TID_BUILDING_GUNSHIP)
				builingModels.Add (DataManager.GetInstance.model.building_list [i]);
		}
		return builingModels;
	}

	//加载建筑
	private void InitBuildings (List<Network.BuildingModel> builingModels)
	{
		for (int i = 0; i < builingModels.Count; i++) {
			Network.BuildingModel model = DataManager.GetInstance.model.building_list [i];
			BuildManager.CreateBuild (new BuildParam () {
				tid = model.type,
				level = model.level,
				tid_level = model.type + "_" + model.level
			});
		}
	}

	//初始化战斗中的建筑
	private void InitBattleOnlyBuildings ()
	{
		foreach (BuildInfo buildInfo in DataManager.GetInstance.buildList.Values) {						
			buildInfo.BattleHitpoint = buildInfo.csvInfo.Hitpoints + Helper.getArtifactBoost (buildInfo.csvInfo.Hitpoints, ArtifactType.BoostBuildingHP);					
			buildInfo.BattleDamage = buildInfo.csvInfo.Damage + Helper.getArtifactBoost (buildInfo.csvInfo.Damage, ArtifactType.BoostBuildingDamage);
			buildInfo.BattleInit (); 
			//Debug.Log("s.csvInfo.Hitpoints:" + s.csvInfo.Hitpoints + "s.csvInfo.Hitpoints:" + s.csvInfo.Hitpoints +";s.BattleHitpoint:" + s.BattleHitpoint + ";s.BattleDamage:" + s.BattleDamage);
			if (buildInfo.csvInfo.TID_Type != "OBSTACLES"
			    && buildInfo.csvInfo.TID_Type != "TRAPS"
			    && buildInfo.csvInfo.TID_Type != "DECOS"
			    && buildInfo.csvInfo.BuildingClass != "Artifact") {
				BattleData.Instance.buildDic.Add (buildInfo.BattleID.ToString (), buildInfo);
				BattleData.Instance.buildList.Add (buildInfo);
			}
			if (buildInfo.csvInfo.BuildingClass == "Defense" || buildInfo.csvInfo.TID_Type == "TRAPS") {
				BattleData.Instance.weaponBuildDic.Add (buildInfo.BattleID.ToString (), buildInfo);
				if (buildInfo.csvInfo.TID_Type == "TRAPS") {
					BattleData.Instance.trapDic.Add (buildInfo.BattleID.ToString (), buildInfo);
				}
			}
			if (buildInfo.csvInfo.TID == "TID_BUILDING_PALACE" || buildInfo.csvInfo.TID == "TID_BUILDING_COMMAND_CENTER") {
				BattleData.Instance.TownHall = buildInfo;
			}
		}
		//实例化战斗中的登陆舰炮舰和军队
		for (int i = 0; i < Globals.battleTrooperList.Count; i++) {
			BattleTrooperData btd = Globals.battleTrooperList [i];
			BattleController.InitShipAndTrooper (btd);//实例化登陆舰
		}
		BattleController.InitShipAndTrooper (null); //实例化战舰，
	}

	private void InitHomeOnlyBuildings ()
	{
		if (Helper.getCollectNumByIsland (PartType.Gold) > 0)
			ResourceShip.CreateShip (PartType.Gold);

		if (Helper.getCollectNumByIsland (PartType.Wood) > 0)
			ResourceShip.CreateShip (PartType.Wood);

		if (Helper.getCollectNumByIsland (PartType.Iron) > 0)
			ResourceShip.CreateShip (PartType.Iron);

		if (Helper.getCollectNumByIsland (PartType.Stone) > 0)
			ResourceShip.CreateShip (PartType.Stone);
	}

	//初始化工人
	private void InitWorkers (int workerCount)
	{
		WorkerManage.Instance.workerCount = workerCount;
		WorkerManage.Instance.init ();
		WorkerManage.Instance.setWorkBuilding (Helper.getWorkBuilding ());//设置工人干活的Building
	}

	private void InitHomeUI ()
	{
		if (ScreenUIManage.Instance != null)
			ScreenUIManage.Instance.data.UserName = DataManager.GetInstance.model.user_info.user_name;
		if (ScreenUIManage.Instance != null)
			ScreenUIManage.Instance.UpdateUserName ();
		//设置每个建筑物上面的，升级提示标识;
		Helper.SetBuildUpgradeIcon ();
		//重新计算版面数据;
		int count = Helper.CalcShopCates (false);
		//设置商城按钮数量;
		UIManager.GetInstance.GetController<MainInterfaceCtrl> ().SetShopCount (count);
		Helper.SetAllMaxStored ();
		Helper.UpdateResUI ("All", false);
	}
}