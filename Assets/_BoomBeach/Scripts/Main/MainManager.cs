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

	private void LoadHome (int regions_id = 0)
	{
		Debug.Log ("LoadHome");
		AudioPlayer.Instance.PlayMusic ("");
		CameraOpEvent.Instance.ResetIm (new Vector3 (-10, 30, 0));
		InitBuildings (DataManager.GetInstance.model.building_list);
	}

	private void LoadEnemyBattle(){
		List<Network.BuildingModel> builingModels = new List<Network.BuildingModel> ();
		for(int i=0;i<DataManager.GetInstance.model.building_list.Count;i++){
			Network.BuildingModel model = DataManager.GetInstance.model.building_list [i];
			if(model.type == GameConstant.TID_BUILDING_LANDING_SHIP || model.type == GameConstant.TID_BUILDING_GUNSHIP)
				builingModels.Add (DataManager.GetInstance.model.building_list[i]);
		}
		InitBuildings (builingModels);
	}

	private void LoadEnemyView(){
		InitBuildings (DataManager.GetInstance.model.building_list);
	}

	private void LoadResourceIsland(){
	
	}


	private void InitBuildings (List<Network.BuildingModel> builingModels)
	{

		Transform characters = SpawnManager.GetInstance.characterContainer;
		Transform bullets = SpawnManager.GetInstance.bulletContainer;
			WorkerManage.Instance.ClearWorkers ();
			//清空旧数据;
			while (characters.childCount > 0) {
				DestroyImmediate (characters.GetChild (0).gameObject);
			}	
			while (bullets.childCount > 0) {
				DestroyImmediate (bullets.GetChild (0).gameObject);
			}
//			if (buildingList != null)
//				buildSize = buildingList.Size ();
//			if (buildingList.Count == 0) {
//				//设置工人工作;
//				WorkerManage.Instance.WorkBuildInfo = Helper.getWorkBuilding ();
//				initStep++;
//			} else {				
//				long startTime = DateTime.Now.Ticks;
//				long useTime = 0;
		for(int i=0;i<builingModels.Count;i++){
			Network.BuildingModel model = DataManager.GetInstance.model.building_list [i];
			if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE) {
				if(model.type == GameConstant.TID_BUILDING_LANDING_SHIP || model.type == GameConstant.TID_BUILDING_GUNSHIP){
					break;
				}
			}
			BuildManager.CreateBuild (new BuildParam () {
				tid = model.type,
				level = model.level,
				tid_level = model.type + "_" +model.level
			});
		}
		if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW || DataManager.GetInstance.sceneStatus == SceneStatus.FRIENDVIEW || DataManager.GetInstance.sceneStatus == SceneStatus.HOMERESOURCE) {
			if (!WorkerManage.Instance.isInit && !WorkerManage.Instance.isIniting) {//初始化工人相关
				if (DataManager.GetInstance.sceneStatus == SceneStatus.HOMERESOURCE)//如果是己方资源场景，则只分配5个工人
					WorkerManage.Instance.workerCount = 5;
				else
					WorkerManage.Instance.workerCount = 15;//否则分配15个工人
				WorkerManage.Instance.init ();//出事话工人相关
			}
			if (WorkerManage.Instance.isInit) {
				WorkerManage.Instance.setWorkBuilding (Helper.getWorkBuilding ());//设置工人干活的Building
			}
		} 



//				for (int b = 0; b < 10; b++) {
//					ISFSObject item = buildingList.GetSFSObject (0);
//					buildingList.RemoveElementAt (0);
//					if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE) {
//						string tid = item.GetUtfString ("tid");
//						//战斗场景不加载敌人炮舰和登陆艇
//						if (tid != "TID_BUILDING_LANDING_SHIP" && tid != "TID_BUILDING_GUNSHIP") {
//							BuildManager.CreateBuild (new BuildParam () {
//								item = item
//							});
//						}
//					} else {
//						BuildManager.CreateBuild (new BuildParam () {
//							item = item
//						});
//					}
//					useTime = DateTime.Now.Ticks - startTime;
//				}
//			}
//		} else if (initStep == 2) {
			//Debug.Log("step2");
			//初始化角色列表;
			/*
			ArrayList trooperList = new ArrayList();
			
			if(UserData.scene_status!=1&&UserData.scene_status!=4&&UserData.scene_status!=5){
				//攻击,回放时不生成生;
				foreach(DictionaryEntry de in UserData.userInfo.user_characters)
				{
					UserTroops userTroops = de.Value as UserTroops;
					for(int i=0;i<userTroops.num;i++)
					{
						trooperList.Add(userTroops.tid+"_"+userTroops.level);
					}
				}
			}
			*/


			//string[] cprefabs = new string[]{"heavys","tank","zooka","warrior","rifleman"};
			//Debug.Log ("a");
			/*
			for (int i=0; i<100; i++) {
				int randIdx = UnityEngine.Random.Range(0,5);
				string prefabname = cprefabs[randIdx];
				GameObject cPrefab = Resources.Load ("Model/Character/"+prefabname) as GameObject;
				GameObject c = Instantiate(cPrefab) as GameObject;
				c.transform.parent = Globals.CharacterContainer;
				CharInfo cc = c.GetComponent<CharInfo>();
				cc.Position = new Vector3(UnityEngine.Random.Range(3f,30f),0f,UnityEngine.Random.Range(3f,30f));
				cc.Id = "cc_"+i;
			}*/
			//Debug.Log ("ba");

			//初始化工人

			if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE || DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY) {
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
			}
			//实例化战斗中的登陆舰炮舰和军队
			if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE || DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY) {
				for (int i = 0; i < Globals.battleTrooperList.Count; i++) {
					BattleTrooperData btd = Globals.battleTrooperList [i];
					BattleController.InitShipAndTrooper (btd);//实例化登陆舰
				}
				BattleController.InitShipAndTrooper (null); //实例化战舰，
			}

			if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME) {
				//初始化用户地图数据;
				Helper.HandleWolrdMap ();
				if (ScreenUIManage.Instance != null)
					ScreenUIManage.Instance.data.UserName = DataManager.GetInstance.model.user_info.user_name;
				if (ScreenUIManage.Instance != null)
					ScreenUIManage.Instance.UpdateUserName ();
				//设置每个建筑物上面的，升级提示标识;
				Helper.SetBuildUpgradeIcon ();
				//重新计算版面数据;
				int count = Helper.CalcShopCates (false);
				//设置商城按钮数量;
				//ScreenUIManage.Instance.SetShopCount (count);
				UIManager.GetInstance.GetController<MainInterfaceCtrl> ().SetShopCount (count);
				//将资源分配到各仓库中;
				Helper.autoAllocGoldStored (0);
				Helper.autoAllocWoodStored (0);
				Helper.autoAllocStoneStored (0);
				Helper.autoAllocMetalStored (0);
				Helper.SetAllMaxStored ();
				Helper.UpdateResUI ("All", false);
			}

			Resources.UnloadUnusedAssets ();
			Globals.IsSceneLoaded = true;

			if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME) {//创建资源回收小船
				if (Helper.getCollectNumByIsland (PartType.Gold) > 0)
					ResourceShip.CreateShip (PartType.Gold);

				if (Helper.getCollectNumByIsland (PartType.Wood) > 0)
					ResourceShip.CreateShip (PartType.Wood);

				if (Helper.getCollectNumByIsland (PartType.Iron) > 0)
					ResourceShip.CreateShip (PartType.Iron);

				if (Helper.getCollectNumByIsland (PartType.Stone) > 0)
					ResourceShip.CreateShip (PartType.Stone);
			}

			if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE) {
				//攻击数据检查;
				Helper.SendAttackCheck ();
			}
			if (DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY) {

			}
			//屏幕显示debug
			UIManager.GetInstance.GetController<NormalMsgCtrl> ().ShowPop ("需创建:" + buildSize);
			UIManager.GetInstance.GetController<NormalMsgCtrl> ().ShowPop ("已创建:" + DataManager.GetInstance.buildList.Count);

			if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW || DataManager.GetInstance.sceneStatus == SceneStatus.WORLDMAP) {
				AudioPlayer.Instance.PlayMusic ("home_music");
			} else if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE) {
				AudioPlayer.Instance.PlayMusic ("combat_planning_music");
			} else if (DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY) {
				AudioPlayer.Instance.PlayMusic ("combat_music");
			}
			//Debug.Log("先执行完加载后的操作，再运行委拖2");
//			SceneLoader.Instance.endLoad = ShowUpgradeList;
			SceneLoader.Instance.EndLoad ();
			//gameObject.SetActive(false);
//		}
	}
}