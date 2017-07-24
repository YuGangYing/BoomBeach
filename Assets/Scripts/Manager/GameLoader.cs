using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;
using BoomBeach;

public class GameLoader : SingleMonoBehaviour<GameLoader> {

	public ISFSObject dt;
	private UserInfo mUserInfo;//仅为了查看方便
    public ISFSArray buildingList = null;
	public Dictionary<long,int> BattleIDList = new Dictionary<long, int>();
	public Dictionary<long,int> GunboatBattleIDList = new Dictionary<long, int>();
	public int initStep;
	private bool isStart;

	public GameObject main;
	public GameObject world;

	private SceneStatus switchSceneStatus;
	public bool play_cloud;
	private ISFSArray upgradeList;

	private static GameLoader instance;
	public static GameLoader Instance{
		get{ return instance; }
	}
	protected override void Awake()
	{
		instance = this;
	}
	//切换场景相机;
	private void SwitchSceneCamera(SceneStatus sceneStatus, bool has_net){
		DataManager.GetInstance.sceneStatus = sceneStatus;
		if (has_net == false){
			SwitchSceneCameraInc();
		}else{
			Globals.HandleLoadBegin(SwitchSceneCameraInc,play_cloud);
			play_cloud = true;
		}
		//SwitchSceneCameraInc();
	}

	//切换场景相机;
	private void SwitchSceneCameraInc(){
		if (DataManager.GetInstance.sceneStatus == SceneStatus.WORLDMAP){
			main.SetActive (false);
			world.SetActive (true);
			CameraOpEvent.Instance.Status = false;
			WorldCameraOpEvent.Instance.Status = true;
			MoveOpEvent.Instance.isDown = false;
			MoveOpEvent.Instance.isDrag = false;
			MoveOpEvent.Instance.isSenceOp = false;
			MoveOpEvent.Instance.isTouchGrid = false;
			MoveOpEvent.Instance.mouseDownPos = Vector3.zero;
			//关闭弹出窗口;
			WorldCameraOpEvent.Instance.ClosePop();
			//Debug.Log("SwitchSceneCamera1:" + sceneStatus.ToString());
			Globals.SetSceneUI ();
		}else{
			main.SetActive (true);
			world.SetActive (false);
			CameraOpEvent.Instance.Status = true;
			MoveOpEvent.Instance.isDown = false;
			MoveOpEvent.Instance.isDrag = false;
			MoveOpEvent.Instance.isSenceOp = false;
			MoveOpEvent.Instance.isTouchGrid = false;
			MoveOpEvent.Instance.mouseDownPos = Vector3.zero;
			if (WorldCameraOpEvent.Instance != null){
				WorldCameraOpEvent.Instance.Status = false;
				//关闭弹出窗口;
				WorldCameraOpEvent.Instance.ClosePop();
			}
			//Debug.Log("SwitchSceneCamera2:" + sceneStatus.ToString());
			Globals.SetSceneUI ();
		}
		if (DataManager.GetInstance.sceneStatus == SceneStatus.WORLDMAP || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW){
			//
			if (CSVManager.GetInstance.experienceLevelsList.ContainsKey(DataManager.GetInstance.model.user_info.exp_level.ToString())){				
				ExperienceLevels el = CSVManager.GetInstance.experienceLevelsList[DataManager.GetInstance.model.user_info.exp_level.ToString()] as ExperienceLevels;
				int SearchCost = (int) (el.AttackCost * Globals.SearchCostFactor);
				if(ScreenUIManage.Instance!=null) ScreenUIManage.Instance.FindGoldLabel.text = SearchCost.ToString();
                if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.AttackGoldLabel.text = el.AttackCost.ToString(); 
			}			
		}
	}

	//sceneStatus:加载的场景类型;
	//user_id:需要加载那个用户,用户id;
	//regions_id:用户所在岛屿id;
	//attack_cost，gems：攻击花费;当sceneStatus == SceneStatus.ENEMYBATTLE时有效;
	//attack_id = 0,attack_type = 2(1:攻击;2:防守;); 当sceneStatus == SceneStatus.BATTLEREPLAY时有效;
	public void SwitchScene(SceneStatus sceneStatus,int user_id = 0, int regions_id = 0,int attack_cost=0, int gems=0, int attack_id = 0,int attack_type = 2){
		MoveOpEvent.Instance.ResetBuild();//释放选中建筑
		if (sceneStatus == SceneStatus.HOME || sceneStatus == SceneStatus.HOMERESOURCE){
			UIManager.GetInstance.GetController<MainInterfaceCtrl>().ShowHome ();
            UIManager.GetInstance.GetController<BattleInterfaceCtrl>().Close();
            if (user_id == 0){
				user_id = DataManager.GetInstance.model.user_info.id;
                //user_id = (int)GameFacade.Instance().ModelMgr.GetModel<Models.CharacterDataModel>().G_MainCharacterData.charid;
			}
			if (regions_id == 1){
				regions_id = 0;
			}
			if (Globals.LastSceneUserId == user_id && Globals.LastSceneRegionsId == regions_id){
				//直接切换场景，不加载数据;
				SwitchSceneCamera(sceneStatus,false);
			}else{
				//保存场景状态;
				switchSceneStatus = sceneStatus;
				//需要从服务器上加载数据;
//				LoadHome(regions_id);
				MainManager.GetInstance.LoadSceneStatus (SceneStatus.HOME);

			}
		}else if (sceneStatus == SceneStatus.WORLDMAP){
			//直接切换场景，不加载数据;
			SwitchSceneCamera(sceneStatus,false);
			UIManager.GetInstance.GetController<MainInterfaceCtrl>().ShowWorld ();
            UIManager.GetInstance.GetController<BattleInterfaceCtrl>().Close();
        }
        else if (sceneStatus == SceneStatus.ENEMYVIEW || sceneStatus == SceneStatus.FRIENDVIEW){
			//敌方查看场景;
			if (Globals.LastSceneUserId == user_id && Globals.LastSceneRegionsId == regions_id){
				//直接切换场景，不加载数据;
				SwitchSceneCamera(sceneStatus,false);
			}else{
				//保存场景状态;
				switchSceneStatus = sceneStatus;
				//需要从服务器上加载数据;
				LoadBattle(attack_cost,gems,user_id,1,regions_id);
			}
		}else if (sceneStatus == SceneStatus.ENEMYBATTLE){
			//保存场景状态;
			switchSceneStatus = sceneStatus;
			//需要从服务器上加载数据;
			LoadBattle(attack_cost,gems,user_id,0,regions_id);
			UIManager.GetInstance.GetController<MainInterfaceCtrl>().Close();
        }
        else if (sceneStatus == SceneStatus.BATTLEREPLAY){
			//保存场景状态;
			switchSceneStatus = sceneStatus;
			//需要从服务器上加载数据;
			ReplayBattle(attack_id,attack_type);
		}
	}

	//1:攻击;2:防守;
	private void ReplayBattle(int attack_id, int attack_type = 2){
		ISFSObject data = new SFSObject();
		data.PutInt("attack_id",attack_id);
		data.PutInt("attack_type",attack_type);
		SFSNetworkManager.Instance.SendRequest(data, "attack_replay", false, HandleBattleReplayResponse);
	}
	
	//attack_cost:攻击成本（金币）;当用户金币不够时使用：gems补;指定攻击某个用户：attack_uid;
	//当is_scout=1时，表示仅侦查;不扣用户金币,但attack_uid应大于0;
	//regions_id地图节点，0:为用户主场景;
	private void LoadBattle(int attack_cost, int gems, int attack_uid = 0,int is_scout = 0,int regions_id = 0){
		if (attack_cost > 0 || gems > 0){
			Helper.SetResource(-attack_cost,0,0,0,-gems,true);
		}
		//Debug.Log("attack_cost:" + attack_cost + ";gems:" + gems);
		ISFSObject data = new SFSObject();
		data.PutInt("attack_cost",attack_cost);	
		data.PutInt("gems",gems);
		data.PutInt("attack_uid",attack_uid);
		data.PutInt("is_scout",is_scout);
		data.PutInt("regions_id",regions_id);
        if (Globals.isLocalBattleData)
        {
            data = TempleData.GetBattleData();
			HandleBattleResponse(data,null);
        }
        else
        {
            SFSNetworkManager.Instance.SendRequest(data, "attack_search", false, HandleBattleResponse);
        }
	}

	private void LoadHome (int regions_id = 0) {
		ISFSObject data = new SFSObject();
		data.PutLong("user_id", Globals.userId);
		data.PutInt("regions_id", regions_id);
		initStep = 0;
		if (Globals.isLocalHomeData){
            //用本地模拟数据
            ISFSObject td = TempleData.GetPlayerData();
			OnResponsHomeInfo(td,null);
        }
        else {
            //从服务器上读取数据
			Debug.Log ("RequestUserInfo");
			SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_USERINFO, false, OnResponsHomeInfo);
        }
    }
	
    /// <summary>
    /// 进入Home场景从服务器上获取用户数据
    /// </summary>
    /// <param name="dt"></param>
	void OnResponsHomeInfo(ISFSObject dt,BuildInfo buildInfo)
	{
		Debug.Log ("OnResponsHomeInfo");
		this.dt = dt;
        AudioPlayer.Instance.PlayMusic("");
        if (!play_cloud)
        {
            CameraOpEvent.Instance.ResetIm(new Vector3(-10, 30, 0));
            LoadHomeData();
        }
        else
        {
            SceneLoader.Instance.beginLoad = LoadHomeData;
            SceneLoader.Instance.BeginLoad();
        }
        //Globals.HandleLoadBegin(HandleLoad3,play_cloud);
		play_cloud = true;	
	}

	void LoadHomeData()
	{
        DataManager.GetInstance.sceneStatus = switchSceneStatus;
		DataManager.GetInstance.playerData = dt;//保存玩家服务器数据
        SwitchSceneCameraInc();
		ISFSObject user_info = dt.GetSFSObject ("userResource");
        if (dt.ContainsKey("upgrade_list")){
			upgradeList = dt.GetSFSArray("upgrade_list");//获取升级列表
		}else{
			upgradeList = null;
		}
		if (user_info!=null)//获取用户相关配置列表
        {
			DataManager.GetInstance.model.user_info.ISFSObjectToBean(user_info);
			//SFSDebug.Log (user_info);
			mUserInfo = DataManager.GetInstance.model.user_info;
			DataManager.GetInstance.model.user_info.worker_count = 1;
			//Globals.islandType = (IslandType)user_info.GetInt("island_type");
			Globals.islandType = IslandType.playerbase;//TODO
			#region 1.获取用户建筑数据
			//加载用户建筑
			if(user_info.GetSFSArray("user_buildings") != null) {
				SFSDebug.Log (user_info);
				buildingList = user_info.GetSFSArray ("user_buildings");
				foreach(ISFSObject obj in buildingList){
					SFSDebug.Log (obj);
				}
				Helper.GetUserInfoFromBuildings (mUserInfo,buildingList);
				//Debug.Log ("buildingList.Count:" + buildingList.Count);
			}
			#endregion

			#region 2.获取用户科技等级
			DataManager.GetInstance.researchLevel.Clear();
			ISFSArray user_characters = user_info.GetSFSArray ("user_characters");
			ISFSArray user_spells = user_info.GetSFSArray ("user_spells");
			if(user_characters!=null){
				for (int i = 0; i < user_characters.Size(); i++)
				{
					ISFSObject obj = user_characters.GetSFSObject(i);
					DataManager.GetInstance.researchLevel[obj.GetUtfString("tid")] = obj.GetInt("level");
					SFSDebug.Log (obj);
				}
			}
			if(user_spells!=null){
				for (int i = 0; i < user_spells.Size(); i++)
				{
					ISFSObject obj = user_spells.GetSFSObject(i);
					DataManager.GetInstance.researchLevel[obj.GetUtfString("tid")] = obj.GetInt("level");
					SFSDebug.Log (obj);
				}
			}
			#endregion

			#region 3.获取用户已经打开的地图
			if (user_info.ContainsKey("user_regions")){
				DataManager.GetInstance.userRegionsList.Clear();
				ISFSArray user_npcs = user_info.GetSFSArray("user_regions");
				for (int i = 0; i < user_npcs.Size(); i++)
				{
					ISFSObject obj = user_npcs.GetSFSObject(i);
					UserRegions ur = new UserRegions();
					ur.ISFSObjectToBean(obj);
					//CSVMananger.GetInstance.userRegionsList[ur.regions_id] = ur;
					//Globals.troopsLevel[obj.GetUtfString("tid")] = obj.GetInt("level");
					SFSDebug.Log (obj);
				}
				/*
				for(int i = 0; i < user_regions.Size(); i ++){
					ISFSObject obj = user_regions.GetSFSObject(i);
					UserRegions ur = new UserRegions();
					ur.ISFSObjectToBean(obj);
					CSVMananger.GetInstance.userRegionsList[ur.regions_id] = ur;
				}	*/	
			}
			#endregion 
		}
        Globals.Init ();
		Globals.LastSceneUserId = DataManager.GetInstance.model.user_info.id;//最后加载的场景用户id;
		Globals.LastSceneRegionsId = dt.GetInt("regions_id");//最后一次加载的岛屿id;

		//不重复处理被攻击列表数据;
		if (Globals.EnemyActivityList.Count == 0 && dt.ContainsKey("enemy_activity")){
			ISFSArray enemy_activity = Helper.SFSObjToArr(dt.GetSFSObject("enemy_activity"));
			ISFSArray enemy_activity_troops = Helper.SFSObjToArr(dt.GetSFSObject("enemy_activity_troops"));
			//Debug.Log(enemy_activity.GetDump());
			//世界地图中的被攻击列表数据;
			Globals.EnemyActivityList.Clear();
			for(int i = 0; i < enemy_activity.Count; i ++){
				ISFSObject item = enemy_activity.GetSFSObject(i);
				EnemyActivityItem aitem = new EnemyActivityItem();					
				aitem.ISFSObjectToBean(item);
				Globals.EnemyActivityList.Add(aitem.id,aitem);
			}
			//Debug.Log(enemy_activity_troops.GetDump());
			for(int i = 0; i < enemy_activity_troops.Count; i ++){
				ISFSObject item = enemy_activity_troops.GetSFSObject(i);
				EnemyActivityDetail aitem = new EnemyActivityDetail();					
				aitem.ISFSObjectToBean(item);
				if (Globals.EnemyActivityList.ContainsKey(aitem.user_attack_2_id)){
					EnemyActivityItem eai = Globals.EnemyActivityList[aitem.user_attack_2_id] as EnemyActivityItem;
					if (eai.TroopsList.ContainsKey(aitem.tid)){
						EnemyActivityDetail ead = eai.TroopsList[aitem.tid] as EnemyActivityDetail;
						ead.deployed_num += aitem.deployed_num;
						ead.destroyed_num += aitem.destroyed_num;
					}else{					
						eai.TroopsList.Add(aitem.tid,aitem);
					}
				}
			}
		}
        //资源岛
        if (DataManager.GetInstance.sceneStatus == SceneStatus.HOMERESOURCE){
			EnemyNameAndResource enar = ScreenUIManage.Instance.enemyNameAndResource;
			enar.UserName = DataManager.GetInstance.model.user_info.user_name;
			enar.UserLevel = DataManager.GetInstance.model.user_info.exp_level.ToString();
		}
		//判断是否开启wap支付;
		//Globals.is_wap_pay = dt.GetInt("is_wap_pay") == 1;
		isStart = true;
		initStep = 0;
	}
	
	void Update () {
		if(isStart)
		{
			InitBuildings();
		}
	}

	void HandleBattleResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		int sync_error = dt.GetInt("sync_error");
		if (sync_error == 0){
			this.dt = dt;
			Globals.HandleLoadBegin(HandleBattleResponse3,play_cloud);
			play_cloud = true;
		}else{
			//TID_NO_TROOPS = You need to train some troops first.
			if (sync_error == 2){
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NO_TROOPS"));
			}else{
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("sync_error:" + sync_error);
			}
		}
	}
	
	void HandleBattleResponse3(){
		DataManager.GetInstance.sceneStatus = switchSceneStatus;
		SwitchSceneCameraInc();
		HandleBattleResponse2(this.dt);
	}

    //读取战斗数据
	void HandleBattleResponse2(ISFSObject dt)
	{
		//数据成功下载;跳转到被攻击或被侦查的用户场景中;
		//Debug.Log(dt.GetDump());
		//Globals.battleData = dt;

		DataManager.GetInstance.battleData = dt;

		int sync_error = dt.GetInt("sync_error");
		if (sync_error == 0){

            /*
            if (dt.ContainsKey("upgrade_list")){
				upgradeList = dt.GetSFSArray("upgrade_list");
			}else{
				upgradeList = null;
			}
            */
            SwitchSceneCamera(switchSceneStatus, true);
			if (dt.GetSFSArray("user_buildings")!=null)
				buildingList = dt.GetSFSArray("user_buildings");
            //TODO 老代码
            if (dt.GetSFSObject("user_grid2") != null)
                buildingList = Helper.SFSObjToArr(dt.GetSFSObject("user_grid2"));
			ISFSObject user = dt.GetSFSObject("userResource");
			Globals.islandType = (IslandType)user.GetInt("island_type");
			Globals.Init ();
			//if (DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE){
				BattleData.Init();//用于上传和回放的数据
			//}
			Globals.LastSceneUserId = user.GetInt("id");//最后加载的场景用户id;
			Globals.LastSceneRegionsId = dt.GetInt("regions_id");//最后加载的d岛屿id;

            /*
			if (DataManager.GetInstance.sceneStatus == SceneStatus.FRIENDVIEW || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW){
                if (ScreenUIManage.Instance != null)
                {
                    EnemyNameAndResource enar = ScreenUIManage.Instance.enemyNameAndResource;
                    enar.UserName = StringFormat.FormatByTid(user.GetUtfString("user_name"));
                    enar.UserLevel = user.GetInt("exp_level").ToString();
                }
			}
            */

			if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE || DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW){
				
				/*
				if (ScreenUIManage.Instance != null)
                {
                    EnemyNameAndResource enar = ScreenUIManage.Instance.enemyNameAndResource;
                    enar.UserName = StringFormat.FormatByTid(user.GetUtfString("user_name"));
                    enar.UserLevel = user.GetInt("exp_level").ToString();
                    enar.GoldResource = dt.GetInt("loot_gold");
                    enar.StoneResource = dt.GetInt("loot_stone");
                    enar.WoodResource = dt.GetInt("loot_wood");
                    enar.IronResource = dt.GetInt("loot_iron");
                    enar.MedalResource = dt.GetInt("add_reward");

                    int loot_boot = dt.GetInt("loot_boot");

                    enar.GoldResourceAdd = dt.GetInt("loot_gold") * loot_boot / 100;
                    enar.StoneResourceAdd = dt.GetInt("loot_stone") * loot_boot / 100;
                    enar.WoodResourceAdd = dt.GetInt("loot_wood") * loot_boot / 100;
                    enar.IronResourceAdd = dt.GetInt("loot_iron") * loot_boot / 100;

                    enar.artifact_name1 = dt.GetUtfString("artifact_name1");//实际随机获得的石像1',CommonPiece,CommonPieceIce,CommonPieceFire,CommonPieceDark
                    enar.artifact_num1 = dt.GetInt("artifact_num1");//实际随机获得的石像1数量',

                    enar.artifact_name2 = dt.GetUtfString("artifact_name2");//实际随机获得的石像2',RarePiece,RarePieceIce,RarePieceFire,RarePieceDark
                    enar.artifact_num2 = dt.GetInt("artifact_num2");//实际随机获得的石像2数量',

                    enar.artifact_name3 = dt.GetUtfString("artifact_name3");//实际随机获得的石像3',EpicPiece,EpicPieceIce,EpicPieceFire,EpicPieceDark
                    enar.artifact_num3 = dt.GetInt("artifact_num3");//实际随机获得的石像3数量',

                    //有机会获得;
                    if (enar.artifact_num1 > 1)
                    {
                        enar.CrystleNum = 1;
                        enar.CrystleType = enar.artifact_name1;
                    }
                    else if (enar.artifact_num2 > 1)
                    {
                        enar.CrystleNum = 1;
                        enar.CrystleType = enar.artifact_name2;
                    }
                    else if (enar.artifact_num3 > 1)
                    {
                        enar.CrystleNum = 1;
                        enar.CrystleType = enar.artifact_name3;
                    }
                    else
                    {
                        enar.CrystleNum = 0;
                    }
                    enar.MedalResourceAdd = dt.GetInt("pal_reward");
                }
				*/

				InitBattleTrooper(dt.GetSFSArray("troops_list"),dt.GetSFSArray("troops_level"));//初始化军队

				//enar.
				//List<BattleTrooperData> trooperData = new List<BattleTrooperData>();
				//trooperData.Add();
				//初始化战斗ui
				if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW){
                    //Debug.Log(Globals.BattleTrooperList.Count);
                    if (ScreenUIManage.Instance != null) 
						ScreenUIManage.Instance.enemyViewTrooperList.InitTrooper(Globals.battleTrooperList);

					UIManager.GetInstance.GetController<BattleInterfaceCtrl>().ShowBattleView (dt);

					UIManager.GetInstance.GetController<BattleInterfaceCtrl>().ShowViewTroops (Globals.battleTrooperList);
				}
				if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE){
                    //初始化军队
                    if (ScreenUIManage.Instance != null) 
						ScreenUIManage.Instance.battleTrooperList.InitTrooper(Globals.battleTrooperList);
                    //ScreenUIManage.Instance.battleWeaponList.totalWeapon = Globals.EnergyTotal;
                    if (ScreenUIManage.Instance != null) 
						ScreenUIManage.Instance.battleWeaponList.InitTrooper(DataManager.GetInstance.battleEnergyList);


					UIManager.GetInstance.GetController<BattleInterfaceCtrl>().ShowBattle (dt);
					UIManager.GetInstance.GetController<BattleInterfaceCtrl>().SetEnergy (Globals.EnergyTotal);
					UIManager.GetInstance.GetController<BattleInterfaceCtrl>().ShowTroops (Globals.battleTrooperList);
					UIManager.GetInstance.GetController<BattleInterfaceCtrl>().ShowWeapons (DataManager.GetInstance.battleEnergyList);
				}
			}

			UIManager.GetInstance.GetController<MainInterfaceCtrl>().Close ();
			
			initStep = 0;
			isStart = true;
		}else{
			//TID_NO_TROOPS = You need to train some troops first.
			if (sync_error == 2){
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NO_TROOPS"));
			}else{
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("sync_error:" + sync_error);
			}
		}
	}


	void HandleBattleReplayResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		int sync_error = dt.GetInt("sync_error");
		if (sync_error == 0){
			this.dt = dt;
			Globals.HandleLoadBegin(HandleBattleReplayResponse3,play_cloud);
			play_cloud = true;
		}else{
			//TID_NO_TROOPS = You need to train some troops first.
			if (sync_error == 2){
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NO_TROOPS"));
			}else{
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("sync_error:" + sync_error);
			}
		}
	}
	
	void HandleBattleReplayResponse3(){
		DataManager.GetInstance.sceneStatus = switchSceneStatus;
		SwitchSceneCameraInc();
		HandleBattleReplayResponse2(this.dt);
	}

    /// <summary>
    /// 与客户端重放相关
    /// </summary>
    /// <param name="dt"></param>
	void HandleBattleReplayResponse2(ISFSObject dt)
	{
		
		//数据成功下载;跳转到被攻击或被侦查的用户场景中;
		//Debug.Log(dt.GetDump());
		int sync_error = dt.GetInt("sync_error");
		if (sync_error == 0){
			SwitchSceneCamera(switchSceneStatus,true);
			
			//Debug.Log("HandleBattleResponse");
			Globals.Init ();
			
			ByteArray user_grid_bytes = dt.GetByteArray("user_grid");
			if (user_grid_bytes != null){
				buildingList = SFSArray.NewFromBinaryData(user_grid_bytes);
			}else{	
				buildingList = SFSArray.NewInstance();
			}			

			
			//ISFSObject user = dt.GetSFSObject("user");
			
			Globals.LastSceneUserId = -1;//user.GetInt("id");//最后加载的场景用户id;
			Globals.LastSceneRegionsId = -1;//dt.GetInt("regions_id");//最后加载的d岛屿id;


			ISFSArray troops_list = null;
			ByteArray troops_list_bytes = dt.GetByteArray("troops_list");
			if (troops_list_bytes != null){
				troops_list = SFSArray.NewFromBinaryData(troops_list_bytes);
			}else{	
				troops_list = SFSArray.NewInstance();
			}
			
			ISFSArray troops_level = null;
			ByteArray troops_level_bytes = dt.GetByteArray("troops_level");
			if (troops_level_bytes != null){
				troops_level = SFSArray.NewFromBinaryData(troops_level_bytes);
			}else{	
				troops_level = SFSArray.NewInstance();
			}


			BattleIDList.Clear();
			GunboatBattleIDList.Clear();
			ByteArray battleID_bytes = dt.GetByteArray("battleID");
			if (battleID_bytes != null){
				ISFSObject BattleIDs = SFSObject.NewFromBinaryData(battleID_bytes);
				//Debug.Log(BattleIDs.GetDump());
				//int icount = BattleIDList.GetLongArray("building_id").Length;
				//int[] BattleID = new int[icount];//攻击时的建筑对照ID;
				//long[] building_id = new long[icount];
				//int[] type = new int[icount];//0:被攻击方(暂时最多只有8个登陆艇)；1：攻击方;
				
				int[] BattleID = BattleIDs.GetIntArray("BattleID");
				long[] building_id = BattleIDs.GetLongArray("building_id");
				int[] type = BattleIDs.GetIntArray("type");
				
				for(int i = 0; i < building_id.Length; i ++){
					//Debug.Log(type[i] + ":" + building_id[i] + ":" + BattleID[i]);
					if (type[i] == 1){
						BattleIDList.Add(building_id[i],BattleID[i]);
					}else{
						GunboatBattleIDList.Add(building_id[i],BattleID[i]);
					}
				}
			}

			InitBattleTrooper(troops_list,troops_level);

            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.battleTrooperList.InitTrooper(Globals.battleTrooperList);
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.battleWeaponList.InitTrooper(DataManager.GetInstance.battleEnergyList);	

			ByteArray attack_replay_bytes = dt.GetByteArray("attack_replay");
			if (attack_replay_bytes != null){
				ISFSObject attack_replay = SFSObject.NewFromBinaryData(attack_replay_bytes);
				//Debug.Log(attack_replay.GetDump());

				int[] TimeFromBeginArray = attack_replay.GetIntArray("TimeFromBegin");    //开始后的时间(Time.delta*1000);
				int[] SelfTypeArray = attack_replay.GetIntArray("SelfType");  //当前数据的对象类型;
				int[] SelfIDArray = attack_replay.GetIntArray("SelfID");           //对象ID，用于识别相应的TID_LEVEL;
				float[] StandXArray = attack_replay.GetFloatArray("StandX");		 //当前攻击时站立的x坐标;
				float[] StandZArray = attack_replay.GetFloatArray("StandZ");		 //当前攻击时站立的z坐标;
				float[] HitPointsArray = attack_replay.GetFloatArray("HitPoints");  	 //当前被扣血量;
				int[] IsUnderAttackArray = attack_replay.GetIntArray("IsUnderAttack");    //是否当下正被攻击，1:显示血条;
				float[] DestXArray = attack_replay.GetFloatArray("DestX");			 //用于寻路、攻击的目标点x;
				float[] DestZArray = attack_replay.GetFloatArray("DestZ");			 //用于寻路、攻击的目标点z;
				int[] StateArray = attack_replay.GetIntArray("State");  		 //当前的状态机;
				int[] AttackStateArray = attack_replay.GetIntArray("AttackState");  //当前的第二状态机(攻击专用的状态机);
//				int[] IsRetreatArray = attack_replay.GetIntArray("IsRetreat");        //是否撤退0:否 1:是;
				int[] AttackTypeArray = attack_replay.GetIntArray("AttackType");        //被攻击的目标类型;
				int[] AttackIDArray = attack_replay.GetIntArray("AttackID");        //被攻击的目标ID;
				string[] walkListArray = attack_replay.GetUtfStringArray("walkList");  	 //行走过的路线格子;
				int[] IsInStunArray = attack_replay.GetIntArray("IsInStun");
				int[] IsInSmokeArray = attack_replay.GetIntArray("IsInSmoke");

				for(int i = 0; i < TimeFromBeginArray.Length; i ++){

					ReplayNodeData rnd = new ReplayNodeData();

								
					rnd.TimeFromBegin = TimeFromBeginArray[i];    //开始后的时间(Time.delta*1000);
					rnd.SelfType = (EntityType)SelfTypeArray[i];  //当前数据的对象类型;
					rnd.SelfID = SelfIDArray[i];           //对象ID，用于识别相应的TID_LEVEL;
					rnd.StandX = StandXArray[i];		 //当前攻击时站立的x坐标;
					rnd.StandZ = StandZArray[i];		 //当前攻击时站立的z坐标;
					rnd.HitPoints = HitPointsArray[i];  	 //当前被扣血量;
					rnd.IsUnderAttack = IsUnderAttackArray[i];    //是否当下正被攻击，1:显示血条;
					rnd.DestX = DestXArray[i];			 //用于寻路、攻击的目标点x;
					rnd.DestZ = DestZArray[i];			 //用于寻路、攻击的目标点z;
					rnd.State = (AISTATE)StateArray[i];  		 //当前的状态机;
					rnd.AttackState = (AISTATE)AttackStateArray[i];  //当前的第二状态机(攻击专用的状态机);
//					rnd.IsRetreat = IsRetreatArray[i];        //是否撤退0:否 1:是;
					rnd.IsInStun = IsInStunArray[i];
					rnd.IsInSmoke = IsInSmokeArray[i];
					rnd.AttackType = (EntityType)AttackTypeArray[i];        //被攻击的目标类型;
					rnd.AttackID = AttackIDArray[i];        //被攻击的目标ID;

					rnd.walkList = WalkList.ToData(walkListArray[i]);  	 //行走过的路线格子;

					BattleData.Instance.ReplayList.Add(rnd);

				}
			}
			 
			initStep = 0;
			isStart = true;
		}else{
            UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("sync_error:" + sync_error);
		}
	}
	//当切换场景时，先保证一份当前军队数据到公共变量中;
	void InitBattleTrooper(ISFSArray troops_list,ISFSArray troops_level){
		//可以出战的军队;
		Globals.battleTrooperList.Clear();
		//Debug.Log("initBattleTrooper");
		for(int k=0; k < troops_list.Size(); k++){
			ISFSObject obj = troops_list.GetSFSObject(k);
			string tid = obj.GetUtfString("tid");
			int level = obj.GetInt("level");
			string troops_tid = obj.GetUtfString("troops_tid");
			int troops_num = obj.GetInt("troops_num");
			long building_id = obj.GetLong("building_id");
			 
			
			if ("TID_BUILDING_LANDING_SHIP".Equals(tid) && troops_num > 0){
				
				BattleTrooperData btd = new BattleTrooperData();
				btd.building_id = building_id;
				btd.tid = troops_tid;
				btd.tidLevel = troops_tid + "_" + getTidMaxLevel(btd.tid,1,troops_level).ToString();
				//Debug.Log("tid_level:" + tid_level);
				btd.landingShipTidLevel = tid + "_" + level.ToString();
				//Debug.Log("btd.LANDING_SHIP_TID_LEVEL:" + btd.LANDING_SHIP_TID_LEVEL);
				//Debug.Log("btd.TID_LEVEL:" + btd.TID_LEVEL);

				CsvInfo csvInfo = CSVManager.GetInstance.csvTable[btd.tidLevel] as CsvInfo;
				btd.csvInfo = csvInfo;
				btd.num = troops_num;
				if (btd.tid == "TID_TANK"){
					btd.weaponCost = 2*troops_num;
				}else{
					btd.weaponCost = 0;
				}
				
				
				if (csvInfo.Damage >0){
					btd.damage = csvInfo.Damage + getArtifactBoost(csvInfo.Damage,ArtifactType.BoostTroopDamage,troops_list);
				}
				
				if (csvInfo.Hitpoints >0){
					btd.hitpoints = csvInfo.Hitpoints + getArtifactBoost(csvInfo.Hitpoints,ArtifactType.BoostTroopHP,troops_list);
				}
				
				Globals.battleTrooperList.Add(btd);
				
				//Debug.Log("s2.building_id:" + s2.building_id + ";s2.troops_tid:" + s2.troops_tid + ";s2.troops_num:" + s2.troops_num);
			}
		}

		
		//当前可以使用的：能量弹总数;
		//BuildInfo s = Helper.getBuildInfoByTid("TID_BUILDING_GUNSHIP");
		//Globals.EnergyTotal = s.csvInfo.StartingEnergy + Helper.getArtifactBoost(s.csvInfo.StartingEnergy,ArtifactType.BoostGunshipEnergy);
		
		CsvInfo csvGunship = CSVManager.GetInstance.csvTable["TID_BUILDING_GUNSHIP_" + DataManager.GetInstance.model.user_info.gunship_level.ToString()] as CsvInfo;
		Globals.EnergyTotal = csvGunship.StartingEnergy + getArtifactBoost(csvGunship.StartingEnergy,ArtifactType.BoostGunshipEnergy,troops_list);
		
		
		//当前可以使用的能量弹类型;
		DataManager.GetInstance.battleEnergyList.Clear();
		
		List<string> tid_list = new List<string>();
		tid_list.Add("TID_ARTILLERY");
		tid_list.Add("TID_FLARE");
		tid_list.Add("TID_MEDKIT");
		tid_list.Add("TID_STUN");
		tid_list.Add("TID_BARRAGE");
		tid_list.Add("TID_SMOKE_SCREEN");
		
		for(int i = 0; i < tid_list.Count; i ++){
			
			string tid = tid_list[i];
			int level = getTidMaxLevel(tid,1,troops_level);
			string tid_level = tid + "_" + level;
			//Debug.Log("tid_level:" + tid_level);
			CsvInfo csvInfo = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;
			
			if (csvInfo.UnlockTownHallLevel <= DataManager.GetInstance.model.user_info.town_hall_level){
				BattleTrooperData btd = new BattleTrooperData();
				btd.csvInfo = csvInfo;
				btd.building_id = 0;
				btd.tid = csvInfo.TID;
				btd.tidLevel = csvInfo.TID_Level;
				//btd.Num = s2.troops_num;				
				btd.weaponCost = csvInfo.Energy;
				btd.energyIncrease = csvInfo.EnergyIncrease;
				
				btd.damage = csvInfo.Damage;

				if (tid.Equals("TID_ARTILLERY")){
					btd.id = 1;
				}else if (tid.Equals("TID_FLARE")){
					btd.id = 2;
				}else if (tid.Equals("TID_MEDKIT")){
					btd.id = 3;
				}else if (tid.Equals("TID_STUN")){
					btd.id = 4;
				}else if (tid.Equals("TID_BARRAGE")){
					btd.id = 5;
				}else if (tid.Equals("TID_SMOKE_SCREEN")){
					btd.id = 6;
				}
				
				DataManager.GetInstance.battleEnergyList.Add(btd);
			}
		}
	}

	/*获取指定tid当前最大级别;*/
	public int getTidMaxLevel(string tid, int min_value,ISFSArray troops_level){
		int max_level = min_value;
		for(int k=0; k < troops_level.Size(); k++){
			ISFSObject obj = troops_level.GetSFSObject(k);
			if (obj.GetUtfString("tid").Equals(tid)){
				max_level = obj.GetInt("level");
				break;
			}
		}
		return max_level;
	}
	//获得神像加成值;
	//artifact_type:神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage;
	//orgValue基础值;
	public int getArtifactBoost(int orgValue, ArtifactType artifact_type,ISFSArray troops_list){
		int boost = 0;
		for(int k=0; k < troops_list.Size(); k++){
			ISFSObject obj = troops_list.GetSFSObject(k);
			int artifact_boost = obj.GetInt("artifact_boost");
			ArtifactType artifact_type2 = (ArtifactType)obj.GetInt("artifact_type");
			if (artifact_boost > 0 && artifact_type2 == artifact_type)
			{				
				boost += (int)(orgValue * (artifact_boost / 100f));//向下取整;
				//Debug.Log("orgValue:" + orgValue + ";s.artifact_boost:" + s.artifact_boost + ";boost:" + boost);
			}
		}		
		//Debug.Log("boost:" + boost);
		return boost;
	}

	private int buildSize;
	private void InitBuildings(){
		
		Transform characters  = SpawnManager.GetInstance.characterContainer;
		//Transform buildings  = Globals.buildContainer;
		Transform bullets = SpawnManager.GetInstance.bulletContainer;

		if(initStep == 0)//第零阶段，清空旧数据
		{
			WorkerManager.GetInstance.ClearWorkers();
			//清空旧数据;
			//TODO 这里可以用对象池
			while(characters.childCount > 0){
				DestroyImmediate(characters.GetChild(0).gameObject);
			}	
			while(bullets.childCount > 0){
				DestroyImmediate(bullets.GetChild(0).gameObject);
			}
			initStep++;	
			if(buildingList!=null) 
				buildSize = buildingList.Size();
        }
		else if(initStep == 1)//第1阶段，创建建筑
        {
			if(buildingList.Count == 0)
			{
				//设置工人工作;
				WorkerManager.GetInstance.WorkBuildInfo = Helper.getWorkBuilding();
				initStep++;
			}
			else
			{				
				long startTime = DateTime.Now.Ticks;
				long useTime = 0;
                for (int b = 0;b<10;b++)
                {
					if (buildingList.Count <= 0 || useTime / 10000 > 50) break;
                    ISFSObject item = buildingList.GetSFSObject(0);
                    buildingList.RemoveElementAt(0);
                    if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE)
                    {
						string tid = item.GetUtfString("tid");
						//战斗场景不加载敌人炮舰和登陆艇
                        if (tid != "TID_BUILDING_LANDING_SHIP" && tid != "TID_BUILDING_GUNSHIP")
                        {
                            BuildManager.CreateBuild(new BuildParam()
                            {
                                item = item
                            });
                        }
                    }
                    else
                    {
                        BuildManager.CreateBuild(new BuildParam()
                        {
                            item = item
                        });
                    }
                    useTime = DateTime.Now.Ticks - startTime;
                }
			}
		}else if(initStep == 2)
        {
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
			if(DataManager.GetInstance.sceneStatus==SceneStatus.HOME||DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYVIEW||DataManager.GetInstance.sceneStatus==SceneStatus.FRIENDVIEW||DataManager.GetInstance.sceneStatus==SceneStatus.HOMERESOURCE)
			{
				if(!WorkerManager.GetInstance.isInit&&!WorkerManager.GetInstance.isIniting)//初始化工人相关
				{
					if(DataManager.GetInstance.sceneStatus==SceneStatus.HOMERESOURCE)//如果是己方资源场景，则只分配5个工人
						WorkerManager.GetInstance.workerCount = 5;
					else
						WorkerManager.GetInstance.workerCount = 15;//否则分配15个工人
					WorkerManager.GetInstance.init();//出事话工人相关
				}
				if(WorkerManager.GetInstance.isInit)
				{
					initStep++;
					WorkerManager.GetInstance.SetWorkBuilding(Helper.getWorkBuilding());//设置工人干活的Building
				}
			}
			else
			{
				initStep++;
			}
		}else if(initStep == 3){
			if(DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE||DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY)
			{
				foreach(BuildInfo buildInfo in DataManager.GetInstance.buildList.Values){						
					buildInfo.BattleHitpoint = buildInfo.csvInfo.Hitpoints + Helper.getArtifactBoost(buildInfo.csvInfo.Hitpoints,ArtifactType.BoostBuildingHP);					
					buildInfo.BattleDamage = buildInfo.csvInfo.Damage + Helper.getArtifactBoost(buildInfo.csvInfo.Damage,ArtifactType.BoostBuildingDamage);
					buildInfo.BattleInit(); 
					//Debug.Log("s.csvInfo.Hitpoints:" + s.csvInfo.Hitpoints + "s.csvInfo.Hitpoints:" + s.csvInfo.Hitpoints +";s.BattleHitpoint:" + s.BattleHitpoint + ";s.BattleDamage:" + s.BattleDamage);
					if(buildInfo.csvInfo.TID_Type!="OBSTACLES"
					   &&buildInfo.csvInfo.TID_Type!="TRAPS"
					   &&buildInfo.csvInfo.TID_Type!="DECOS"
					   &&buildInfo.csvInfo.BuildingClass!="Artifact")
					{
						BattleData.Instance.buildDic.Add(buildInfo.BattleID.ToString(),buildInfo);
						BattleData.Instance.buildList.Add (buildInfo);
					}
					if(buildInfo.csvInfo.BuildingClass=="Defense"||buildInfo.csvInfo.TID_Type == "TRAPS")
					{
						BattleData.Instance.weaponBuildDic.Add(buildInfo.BattleID.ToString(),buildInfo);
						if(buildInfo.csvInfo.TID_Type=="TRAPS")
						{
							BattleData.Instance.trapDic.Add(buildInfo.BattleID.ToString(),buildInfo);
						}
					}
					if(buildInfo.csvInfo.TID=="TID_BUILDING_PALACE" || buildInfo.csvInfo.TID=="TID_BUILDING_COMMAND_CENTER")
					{
						BattleData.Instance.TownHall = buildInfo;
					}
				}
			}
			//实例化战斗中的登陆舰炮舰和军队
			if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE||DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY){
				for(int i = 0; i < Globals.battleTrooperList.Count; i ++){
					BattleTrooperData btd  = Globals.battleTrooperList[i];
					BattleController.InitShipAndTrooper(btd);//实例化登陆舰
				}
				BattleController.InitShipAndTrooper(null); //实例化战舰，
			}

			if(DataManager.GetInstance.sceneStatus==SceneStatus.HOME)
			{
				//初始化用户地图数据;
				Helper.HandleWolrdMap();
                if (ScreenUIManage.Instance != null)
                    ScreenUIManage.Instance.data.UserName = DataManager.GetInstance.model.user_info.user_name;
                if (ScreenUIManage.Instance != null) 
					ScreenUIManage.Instance.UpdateUserName();
				//设置每个建筑物上面的，升级提示标识;
				Helper.SetBuildUpgradeIcon();
				//重新计算版面数据;
				int count = Helper.CalcShopCates(false);
				//设置商城按钮数量;
				//ScreenUIManage.Instance.SetShopCount (count);
				UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);
                //将资源分配到各仓库中;
                Helper.autoAllocGoldStored(0);
				Helper.autoAllocWoodStored(0);
				Helper.autoAllocStoneStored(0);
				Helper.autoAllocMetalStored(0);
                Helper.SetAllMaxStored();
                Helper.UpdateResUI("All",false);
			}
			
			Resources.UnloadUnusedAssets();
			isStart = false;
			Globals.IsSceneLoaded = true;

			if(DataManager.GetInstance.sceneStatus==SceneStatus.HOME)//创建资源回收小船
			{
				if (Helper.getCollectNumByIsland(PartType.Gold) > 0)		
					ResourceShip.CreateShip(PartType.Gold);

				if (Helper.getCollectNumByIsland(PartType.Wood) > 0)		
					ResourceShip.CreateShip(PartType.Wood);

				if (Helper.getCollectNumByIsland(PartType.Iron) > 0)		
					ResourceShip.CreateShip(PartType.Iron);

				if (Helper.getCollectNumByIsland(PartType.Stone) > 0)		
					ResourceShip.CreateShip(PartType.Stone);
			}

			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				//攻击数据检查;
				Helper.SendAttackCheck();
			}
			if(DataManager.GetInstance.sceneStatus==SceneStatus.BATTLEREPLAY)
			{

			}
			//屏幕显示debug
            UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("需创建:" + buildSize);
            UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("已创建:" + DataManager.GetInstance.buildList.Count);
            
			if(DataManager.GetInstance.sceneStatus==SceneStatus.HOME||DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYVIEW||DataManager.GetInstance.sceneStatus==SceneStatus.WORLDMAP)
			{
				AudioPlayer.Instance.PlayMusic("home_music");
			}
			else if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				AudioPlayer.Instance.PlayMusic("combat_planning_music");
			}
			else if(DataManager.GetInstance.sceneStatus==SceneStatus.BATTLEREPLAY)
			{
				AudioPlayer.Instance.PlayMusic("combat_music");
			}
			//Debug.Log("先执行完加载后的操作，再运行委拖2");
			SceneLoader.Instance.endLoad = ShowUpgradeList;
			SceneLoader.Instance.EndLoad();
			//gameObject.SetActive(false);
		}
	}

	private void ShowUpgradeList(){
		if (upgradeList != null){
			while(upgradeList.Count > 0){
				/*
				 ISFSObject build = SFSObject.newInstance();
							 build.putInt("is_new", 1);//新建;
							 build.putUtfString("tid", csvInfo.TID);
							 build.putInt("level", csvInfo.Level);
				*/			 

				ISFSObject build = upgradeList.GetSFSObject(0);
				int is_new = build.GetInt("is_new");
				string tid = build.GetUtfString("tid");
				int level = build.GetInt("level");

				string msg = "";
				if (is_new == 0){
					//TID_NEW_BUILDING_ON_OUTPOST = 新的防御: <name>!;
					//TID_BUILDING_UPGRADED_ON_OUTPOST = <name> 升至 <level>级;
					msg = StringFormat.FormatByTid("TID_BUILDING_UPGRADED_ON_OUTPOST",new object[]{StringFormat.FormatByTid(tid),level.ToString()});
				}else{
					msg = StringFormat.FormatByTid("TID_NEW_BUILDING_ON_OUTPOST",new object[]{StringFormat.FormatByTid(tid)});
				}
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
				upgradeList.RemoveElementAt(0);
			}
		}
	}
	
	
}
