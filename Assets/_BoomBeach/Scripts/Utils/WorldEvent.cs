using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using BoomBeach;

public class WorldEvent : MonoBehaviour {
	public UserRegions ur;
	public Regions rs;
	
	public Transform EnemyBox;
	
	public GameObject popBox1,popBox2,popBox3,popBox4,popBox5;
	
	private UILabel lost_gold,lost_wood,lost_stone,lost_iron;
	
	private UILabel title_label,user_name,lv,reward;
	private GameObject reward_sprite,lv_sprite;
	private EnemyActivityItem eai;
	private Transform bottom_panel,lost_panel,deployed_panel,destroyed_panel,ReplayBtn,ClaimRewardBtn;
	private Vector3 bottom_panel_pos = Vector3.zero,lost_panel_pos = Vector3.zero,deployed_panel_pos = Vector3.zero,destroyed_panel_pos = Vector3.zero;
	private UILabel ClaimRewardLabel,ReplayLabel;
	
	void Start()
	{
		//Debug.Log("WorldEvent Awake:");
		init();
		
	}
	
	
	public void init(){
		EnemyBox = transform.Find("EnemyBox");
		if (EnemyBox != null){
			//Debug.Log("EnemyBox != null");
			if (transform.Find("popBox1") == null){
				Debug.Log("popBox1 == null");
			}else{
				
			}
			popBox1 = transform.Find("popBox1").gameObject;
			popBox2 = transform.Find("popBox2").gameObject;
			popBox3 = transform.Find("popBox3").gameObject;
			popBox4 = transform.Find("popBox4").gameObject;
			popBox5 = transform.Find("popBox5").gameObject;
			
			lost_gold = EnemyBox.Find("lost_panel/glod").GetComponent<UILabel>();
			lost_wood = EnemyBox.Find("lost_panel/wood").GetComponent<UILabel>();
			lost_stone = EnemyBox.Find("lost_panel/stone").GetComponent<UILabel>();
			lost_iron = EnemyBox.Find("lost_panel/iron").GetComponent<UILabel>();
			
			//lost_gold.text = "lost_gold222";
			//Debug.Log(lost_gold.text);
			
			title_label = EnemyBox.Find("title_panel/info").GetComponent<UILabel>();
			user_name = EnemyBox.Find("title_panel/user_name").GetComponent<UILabel>();
			lv = EnemyBox.Find("title_panel/lv").GetComponent<UILabel>();
			lv_sprite = EnemyBox.Find("title_panel/lv_sprite").gameObject;
			reward = EnemyBox.Find("title_panel/reward").GetComponent<UILabel>();
			
			reward_sprite = EnemyBox.Find("title_panel/reward_sprite").gameObject;
			ClaimRewardLabel = EnemyBox.Find("bottom_panel/ClaimRewardLabel").GetComponent<UILabel>();
			ReplayLabel = EnemyBox.Find("bottom_panel/ReplayLabel").GetComponent<UILabel>();
			bottom_panel = EnemyBox.Find("bottom_panel");
			deployed_panel = EnemyBox.Find("deployed_panel");
			destroyed_panel = EnemyBox.Find("destroyed_panel");
			lost_panel = EnemyBox.Find("lost_panel");
			
			ReplayBtn = EnemyBox.Find("btn_panel/ReplayBtn");
			ClaimRewardBtn = EnemyBox.Find("btn_panel/ClaimRewardBtn");
			
			if (bottom_panel_pos == Vector3.zero){
				bottom_panel_pos = bottom_panel.localPosition;
			}
			
			if (lost_panel_pos == Vector3.zero){
				lost_panel_pos = lost_panel.localPosition;
			}
			
			if (deployed_panel_pos == Vector3.zero){
				deployed_panel_pos = deployed_panel.localPosition;
			}
			
			if (destroyed_panel_pos == Vector3.zero){
				destroyed_panel_pos = destroyed_panel.localPosition;
			}
		}else{
			//Debug.Log("EnemyBox == null");
		}
	}
	
	public void showEnemyBox(EnemyActivityItem eai){
		if (eai == null){
			if (EnemyBox != null){
				EnemyBox.gameObject.SetActive(false);
			}
		}else{
			init();
			
			this.eai = eai;
			Debug.Log(eai.id);
			
			
			this.gameObject.SetActive(true);
			
			popBox1.SetActive(false);
			popBox2.SetActive(false);
			popBox3.SetActive(false);
			popBox4.SetActive(false);
			popBox5.SetActive(false);
			EnemyBox.gameObject.SetActive(true);
			
			//regions_type 岛类型;0:无;1:主岛屿;2:资源岛(木);3:资源岛(石);4:资源岛(钢);5:自由岛;6:博士岛;
			if (eai.regions_type == 5 || eai.regions_type == 6){
				
				//title_label.text = StringFormat.FormatByTid("TID_YOU_LOST_OUTPOST");
				if (DataManager.GetInstance.userRegionsList.ContainsKey(eai.regions_id)){
					UserRegions ur = DataManager.GetInstance.userRegionsList[eai.regions_id] as UserRegions;
					
					bottom_panel.gameObject.SetActive(true);
					deployed_panel.gameObject.SetActive(false);
					destroyed_panel.gameObject.SetActive(false);
					lost_panel.gameObject.SetActive(false);
					
					ReplayBtn.gameObject.SetActive(false);
					ClaimRewardBtn.gameObject.SetActive(false);
					lv_sprite.SetActive(false);
					
					user_name.text = "";
					lv.text = "";
					
					if (eai.regions_type == 5){
						if (ur.is_npc == 0){
							//TID_VILLAGE_LOST_PLAYER = Mercenary invasion
							//TID_VILLAGE_LOST_HINT_PLAYER = Blackguard mercenaries invaded this village!
							title_label.text = StringFormat.FormatByTid("TID_VILLAGE_LOST_PLAYER");
							ReplayLabel.text = StringFormat.FormatByTid("TID_VILLAGE_LOST_HINT_PLAYER");
						}else{
							//TID_VILLAGE_LOST = Blackguard invasion
							//TID_VILLAGE_LOST_HINT = The Blackguard invaded this village.
							title_label.text = StringFormat.FormatByTid("TID_VILLAGE_LOST");
							ReplayLabel.text = StringFormat.FormatByTid("TID_VILLAGE_LOST_HINT");
						}
					}else{
						//TID_EVENT_POPOVER_TITLE = Terror Island!
						//TID_EVENT_POPOVER_HINT = The evil Dr. Terror has set up his experiment here. Stop him!
						title_label.text = StringFormat.FormatByTid("TID_EVENT_POPOVER_TITLE");
						ReplayLabel.text = StringFormat.FormatByTid("TID_EVENT_POPOVER_HINT");
					}
					
					if (eai.reward_count > 0){
						reward_sprite.SetActive(true);
						reward.text = "-" + eai.reward_count.ToString();
					}else{
						reward_sprite.SetActive(false);
						reward.text ="";
					}
					
					
					ClaimRewardLabel.gameObject.SetActive(false);
					ReplayLabel.gameObject.SetActive(true);
				}
			}else{
				
				lv_sprite.SetActive(true);
				
				bottom_panel.localPosition = bottom_panel_pos;
				lost_panel.localPosition = lost_panel_pos;
				deployed_panel.localPosition = deployed_panel_pos;
				destroyed_panel.localPosition = destroyed_panel_pos;
				
				
				
				
				//WorldCameraOpEvent.Instance.ClosePop();
				
				lost_gold.text = eai.loot_gold.ToString();
				lost_wood.text = eai.loot_wood.ToString();
				lost_stone.text = eai.loot_stone.ToString();
				lost_iron.text = eai.loot_iron.ToString();
				
				
				user_name.text = eai.u_name;
				lv.text = eai.u_level.ToString();
				
				
				//是否可以回看;
				
				if (eai.has_replay == 1){
					ReplayBtn.gameObject.SetActive(true);
					ReplayLabel.gameObject.SetActive(false);
				}else{
					ReplayBtn.gameObject.SetActive(false);
					ReplayLabel.gameObject.SetActive(true);
					ReplayLabel.text = StringFormat.FormatByTid("TID_REPLAY_NOT_AVAILABLE");
				}
				
				//是否可以收集宝石;
				
				if (eai.reward_diamond > 0 && eai.reward_count > 0 && eai.already_claimed == 0){
					ClaimRewardBtn.gameObject.SetActive(true);
					ClaimRewardLabel.gameObject.SetActive(false);
					UILabel ClaimRewardNum = EnemyBox.Find("btn_panel/ClaimRewardBtn/num").GetComponent<UILabel>();
					ClaimRewardNum.text = eai.reward_diamond.ToString();
				}else{
					ClaimRewardBtn.gameObject.SetActive(false);
					if (eai.reward_diamond > 0 && eai.already_claimed == 1){
						ClaimRewardLabel.gameObject.SetActive(true);
						ClaimRewardLabel.text = StringFormat.FormatByTid("TID_REWARD_ALREADY_CLAIMED");
					}else{
						ClaimRewardLabel.gameObject.SetActive(false);
					}
				}
				
				
				
				
				
				
				//处理军队;
				for(int i = 0; i < 6; i ++){
					deployed_panel.Find("troops" + i.ToString()).gameObject.SetActive(false);
					destroyed_panel.Find("troops" + i.ToString()).gameObject.SetActive(false);
				}
				
				deployed_panel.gameObject.SetActive(false);
				destroyed_panel.gameObject.SetActive(false);
				int k = 0;
				int k2 = 0;
				foreach(EnemyActivityDetail ead in eai.TroopsList.Values){
					Transform troops = deployed_panel.Find("troops" + k.ToString());
					troops.gameObject.SetActive(true);
					
					setTroopsPlane(troops,ead,ead.deployed_num);
					
					if (ead.destroyed_num > 0){
						Transform troops2 = destroyed_panel.Find("troops" + k2.ToString());
						troops2.gameObject.SetActive(true);
						
						setTroopsPlane(troops2,ead,ead.destroyed_num);
						k2 ++;
					}
					
					
					k ++;
				}
				
				if (k > 0){
					deployed_panel.gameObject.SetActive(true);
				}
				
				if (k2 > 0){
					destroyed_panel.gameObject.SetActive(true);
				}else{
					ClaimRewardLabel.text = StringFormat.FormatByTid("TID_REWARD_LIMIT_NOT_REACHED");
					ClaimRewardLabel.gameObject.SetActive(true);
				}
				//eai.TroopsList.Count;
				
				if (ClaimRewardLabel.gameObject.activeSelf || ReplayLabel.gameObject.activeSelf){
					bottom_panel.gameObject.SetActive(true);
				}else{
					bottom_panel.gameObject.SetActive(false);
				}
				
				
				//regions_type 岛类型;0:无;1:主岛屿;2:资源岛(木);3:资源岛(石);4:资源岛(钢);5:自由岛;6:博士岛;
				//TID_YOU_LOST_OUTPOST_TITLE = Our resource base was captured by
				//TID_OUTPOST_WAS_ATTACKED_TITLE = We defended a resource base against
				//TID_YOU_LOST_BASE_TITLE = Our base was raided by
				//TID_BASE_WAS_ATTACKED_TITLE = We defended our base against
				
				//Blackguard invasion
				//Mercenary invasion
				if (eai.reward_count > 0){
					reward_sprite.SetActive(true);
					reward.text = "-" + eai.reward_count.ToString();
					
					if (eai.regions_type == 1){
						title_label.text = StringFormat.FormatByTid("TID_YOU_LOST_BASE_TITLE");
					}else{
						title_label.text = StringFormat.FormatByTid("TID_YOU_LOST_OUTPOST_TITLE");
					}
					
					lost_panel.gameObject.SetActive(true);
				}else{
					reward_sprite.SetActive(false);
					reward.text = "";
					
					if (eai.regions_type == 1){
						title_label.text = StringFormat.FormatByTid("TID_BASE_WAS_ATTACKED_TITLE");
					}else{
						title_label.text = StringFormat.FormatByTid("TID_OUTPOST_WAS_ATTACKED_TITLE");
					}
					
					lost_panel.gameObject.SetActive(false);
				}
				
				
			}
			
			
			int ih = 703;
			
			if (lost_panel.gameObject.activeSelf == false){
				bottom_panel.localPosition = new Vector3(bottom_panel.localPosition.x,destroyed_panel.localPosition.y,bottom_panel.localPosition.z);
				
				destroyed_panel.localPosition = new Vector3(destroyed_panel.localPosition.x,deployed_panel.localPosition.y,destroyed_panel.localPosition.z);
				
				deployed_panel.localPosition = new Vector3(deployed_panel.localPosition.x,lost_panel.localPosition.y,deployed_panel.localPosition.z);
				ih = ih - 170;
			}
			
			
			if (deployed_panel.gameObject.activeSelf == false){
				bottom_panel.localPosition = new Vector3(bottom_panel.localPosition.x,destroyed_panel.localPosition.y,bottom_panel.localPosition.z);
				
				destroyed_panel.localPosition = new Vector3(destroyed_panel.localPosition.x,deployed_panel.localPosition.y,destroyed_panel.localPosition.z);
				ih = ih - 170;
			}
			
			if (destroyed_panel.gameObject.activeSelf == false){
				bottom_panel.localPosition = new Vector3(bottom_panel.localPosition.x,destroyed_panel.localPosition.y,bottom_panel.localPosition.z);
				ih = ih - 170;
			}
			
			if (bottom_panel.gameObject.activeSelf == false){
				ih = ih - 70;
			}
			
			UISprite uiSprite = EnemyBox.transform.Find("bg_sprite").GetComponent<UISprite>();
			uiSprite.height = ih;
			if (eai.regions_type == 5 || eai.regions_type == 6){
				uiSprite.width = 654;
				reward_sprite.transform.localPosition = new Vector3(230f,reward_sprite.transform.localPosition.y,reward_sprite.transform.localPosition.z);
				reward.transform.localPosition = new Vector3(180f,reward.transform.localPosition.y,reward.transform.localPosition.z);		
			}else{
				reward_sprite.transform.localPosition = new Vector3(410f,reward_sprite.transform.localPosition.y,reward_sprite.transform.localPosition.z);
				reward.transform.localPosition = new Vector3(350f,reward.transform.localPosition.y,reward.transform.localPosition.z);		
				uiSprite.width = 854;
			}
			
		}
	}
	
	
	public void setTroopsPlane(Transform troops,EnemyActivityDetail ead, int num){
		UISprite avatar = troops.Find("avatar").GetComponent<UISprite>();
		avatar.spriteName = ead.tid;
		UILabel lv = troops.Find("Label").GetComponent<UILabel>();
		lv.text = "lv " + ead.level.ToString();
		UILabel Number = troops.Find("Number").GetComponent<UILabel>();
		Number.text = "x " + num.ToString();
	}
	
	public void OnGoHome(){
		//Debug.Log("OnGoHome:ur.res_tid:" + ur.res_tid);
		if (ur.res_tid == null || ur.res_tid == "")
			GameLoader.Instance.SwitchScene(SceneStatus.HOME,0,ur.regions_id);
		else
			GameLoader.Instance.SwitchScene(SceneStatus.HOMERESOURCE,0,ur.regions_id);
	}
	
	public void OnClaimReward(){
		Debug.Log("OnClaimReward");
		
		if (eai.reward_diamond > 0 && eai.already_claimed == 0){
			eai.already_claimed = 1;
			
			Transform ClaimRewardBtn = EnemyBox.Find("btn_panel/ClaimRewardBtn");
			
			
			//Helper.setResourceCount("Gems",int.Parse(res_num.text),false,true);
			
			int num = eai.reward_diamond;
			//GameObject.Find ("UI Root (2D)").GetComponent<GemPartEmitter> ().Emit (num, ClaimRewardBtn.position, num);
            GemPartEmitter.Instance().GetComponent<GemPartEmitter>().Emit(num, ClaimRewardBtn.position, num);

            ISFSObject data = new SFSObject();
			data.PutInt("user_attack_2",eai.id);
			SFSNetworkManager.Instance.SendRequest(data, "claim_reward", false, null);
			
			ClaimRewardBtn.gameObject.SetActive(false);
		}
	}
	
	public void OnReplay(){
		Debug.Log("OnReplay");
		if (eai.has_replay == 1){
			GameLoader.Instance.SwitchScene(SceneStatus.BATTLEREPLAY,0,0,0,0,eai.id,2);
		}
	}
	
	public void OnAttack(){
		//Debug.Log("OnAttack");
		if (CSVManager.GetInstance.experienceLevelsList.ContainsKey(DataManager.GetInstance.userInfo.exp_level.ToString())){
			WorldCameraOpEvent.Instance.ClosePop();
			
			ExperienceLevels el = CSVManager.GetInstance.experienceLevelsList[DataManager.GetInstance.userInfo.exp_level.ToString()] as ExperienceLevels;
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
				//GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,5,0,dt.GetInt("Gold"),0);
				GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,ur.capture_id,ur.regions_id,dt.GetInt("Gold"),0);

			}
		}else{
			
		}
	}
	
	public void OnScout(){
		//Debug.Log("OnScout");
		if (ur.user_id == ur.capture_id){
			
			if (ur.res_tid != null && ur.res_tid != ""){
				//查看自己的资源岛屿;
				GameLoader.Instance.SwitchScene(SceneStatus.HOMERESOURCE,ur.user_id,ur.regions_id,0,0);
			}else{
				Debug.Log("非资源岛屿:" + ur.res_tid + ";ur.id:" + ur.id);
			}
		}else{
			//查看敌方岛屿;
			GameLoader.Instance.SwitchScene(SceneStatus.ENEMYVIEW,ur.capture_id,ur.regions_id,0,0);
		}
	}
	
	public void OnFind(){
		Debug.Log("OnFind");
		
		if (ur.sending == false){
			WorldCameraOpEvent.Instance.ClosePop();
			
			ur.sending = true;
			ISFSObject data = new SFSObject();
			data.PutInt("regions_id",ur.regions_id);		
			SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_UPDATE_USER_REGIONS, false, HandleUpdateUserRegionsResponse);
		}
	}
	
	void HandleUpdateUserRegionsResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		ur.sending = false;
		
		Debug.Log(dt.GetDump());
		ISFSObject obj = dt.GetSFSObject("user_regions");
		
		ur.ISFSObjectToBean(obj);
		
		DataManager.GetInstance.userRegionsList[ur.regions_id] = ur;
		
		ur.worldHouse.initData(ur,ur.worldHouse.worldCamera);
		
	}
	
	
	private void onBattleDialogYes(ISFSObject dt){
		//Debug.Log("onDialogYes");
		//Debug.Log(dt.GetDump());
		
		if (DataManager.GetInstance.userInfo.diamond_count >= dt.GetInt("Gems")){
			GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,ur.capture_id,ur.regions_id,dt.GetInt("Gold"),dt.GetInt("Gems"));
			//GameLoader.Instance.StartBattle(dt.GetInt("Gold"),dt.GetInt("Gems"),0,0);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog(null,null);
		}
	}
	
	
	public void OnExplore(){
		Debug.Log("OnExplore");
		
		if (rs.sending == false){
			WorldCameraOpEvent.Instance.ClosePop();
			
			ISFSObject dt = Helper.getCostDiffToGems("",3,true,rs.ExplorationCost);
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
					onExploreDialogYes,
					null,
					gems.ToString()
					);
			}else{
				rs.sending = true;
				rs.send_sprite.SetActive(true);
				
				rs.gold_sprite.SetActive(false);
				rs.exploration_cost.SetActive(false);
				
				ISFSObject data = new SFSObject();
				data.PutUtfString("regions_name",rs.Name);
				data.PutInt("Gold",rs.ExplorationCost);
				data.PutInt("Gems",0);
				SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_EXPLORE, false, HandleExploreResponse);
				
				Helper.SetResource(-rs.ExplorationCost,0,0,0,0,true);
			}
		}	
	}
	
	
	private void onExploreDialogYes(ISFSObject dt){
		//Debug.Log("onDialogYes");
		//Debug.Log(dt.GetDump());
		
		if (DataManager.GetInstance.userInfo.diamond_count >= dt.GetInt("Gems")){
			rs.sending = true;
			rs.send_sprite.SetActive(true);
			ISFSObject data = new SFSObject();
			data.PutUtfString("regions_name",rs.Name);
			data.PutInt("Gold",dt.GetInt("Gold"));
			data.PutInt("Gems",dt.GetInt("Gems"));
			SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_EXPLORE, false, HandleExploreResponse);
			
			Helper.SetResource(-dt.GetInt("Gold"),0,0,0,-dt.GetInt("Gems"),true);
			
			//GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,ur.capture_id,ur.capture_regions_id,dt.GetInt("Gold"),dt.GetInt("Gems"));
			//GameLoader.Instance.StartBattle(dt.GetInt("Gold"),dt.GetInt("Gems"),0,0);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog(null,null);
		}
	}
	
	void HandleExploreResponse(ISFSObject dt,BuildInfo buildInfo = null)
	{
		Debug.Log("HandleExploreResponse");
		WorldCameraOpEvent.Instance.ClosePop();
		
		string regions_name = dt.GetUtfString("regions_name");
		Regions rs2 = CSVManager.GetInstance.regionsList[regions_name] as Regions;
		//regions_name
		
		rs2.sending = false;
		//Debug.Log(rs2.cloud.name);
		rs2.cloud.gameObject.SetActive(false);
		//Debug.Log(dt.GetDump());
		
		
		Transform House = PopManage.Instance.WorldMap.transform.Find("House").transform;
		
		ISFSArray user_regions = dt.GetSFSArray("user_regions");
		//Debug.Log(user_regions.GetDump());
		for(int i = 0; i < user_regions.Size(); i ++){
			
			ISFSObject obj = user_regions.GetSFSObject(i);
			
			UserRegions ur = new UserRegions();
			ur.ISFSObjectToBean(obj);
			DataManager.GetInstance.userRegionsList[ur.regions_id] = ur;
			
			GameObject islandHouse = Instantiate(ResourceCache.load("UI/islandHouse")) as GameObject;
			Transform house_pos = House.Find(ur.regions_id.ToString());
			
			/*
			while(house_pos.GetChildCount() > 0){
				Destroy(house_pos.GetChild(0).gameObject);
			}
*/
			islandHouse.transform.parent = house_pos;
			islandHouse.transform.localPosition = Vector3.zero;
			
			WorldHouse wh = islandHouse.GetComponent<WorldHouse>();
			ur.worldHouse = wh;
			wh.initData(ur,WorldBtnEvent.Instance.worldCamera);
		}
		
		/*
		ur.sending = false;
		

		ISFSObject obj = dt.GetSFSObject("user_regions");
		
		ur.ISFSObjectToBean(obj);
		
		ur.worldHouse.initData(ur,ur.worldHouse.worldCamera);
		*/
	}	
	
}
