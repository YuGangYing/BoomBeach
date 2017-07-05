using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using BoomBeach;


public class BattleResult : MonoBehaviour {

	public UILabel ResultTitleLabel;
	public string ResultTitle{
		set{
			ResultTitleLabel.text = value;
		}
		get{
			return ResultTitleLabel.text;
		}
	}

	public GameObject ReceiveEmpty;

	public GameObject CostEmpty;

	public Transform CostBox;

	public Transform ReceiveBox;

	public List<BattleResultItem> CostResultList;
	public List<BattleResultItem> ReceiveResultList;
	private BattleResultItem parentItem;
	//private int regions_id = 0;
	private bool is_server_return = false;//服务器返回;

	public void OnClickReturn()
	{
		is_server_return = true;
		if (is_server_return){
			BattleResultItem[] items1 = CostBox.GetComponentsInChildren<BattleResultItem> (true);
			BattleResultItem[] items2 = ReceiveBox.GetComponentsInChildren<BattleResultItem> (true);
			for (int i=0; i<items1.Length; i++) {
				Destroy(items1[i].gameObject);		
			}
			for (int i=0; i<items2.Length; i++) {
				Destroy(items2[i].gameObject);		
			}
			isShow = false;
			if (BattleData.Instance.BattleIsSuccess){
				UserRegions ur = DataManager.GetInstance().userRegionsList[Globals.LastSceneRegionsId] as UserRegions;
				if (ur != null && (ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE")){
					//攻击胜利，如果是资源岛屿的话,则重新返回到该资源岛屿;
					GameLoader.Instance.SwitchScene(SceneStatus.HOMERESOURCE,DataManager.GetInstance().userInfo.id,Globals.LastSceneRegionsId,0,0);
				}else{
					Globals.LastSceneUserId = -1;
					Globals.LastSceneRegionsId = -1;
					UIButtonEvent.Instance.OnWorldIcoClick();
					AudioPlayer.Instance.PlayMusic("home_music");
				}
			}else{
				Globals.LastSceneUserId = -1;
				Globals.LastSceneRegionsId = -1;
				UIButtonEvent.Instance.OnWorldIcoClick();
				AudioPlayer.Instance.PlayMusic("home_music");
			}
		}
	}


	private bool isShow;
	public void ShowResultWin()
	{
		if(isShow)return;
		isShow = true;
		is_server_return = false;
		Invoke("PlayWin",1f);

	}

	//结束攻击;
	void SendAttackEnd(){
		//Debug.Log("SendAttackDeployTroops");
		ISFSObject data = new SFSObject();
		
		data.PutInt("end_time", BattleData.Instance.TimeFromBegin);
		
		if (BattleData.Instance.BattleIsSuccess)
			data.PutInt("is_victory",1);
		else
			data.PutInt("is_victory",0);
		
		SFSNetworkManager.Instance.SendRequest(data, "attack_end", false, HandleAttackEndResponse);
	}
	
	void HandleAttackEndResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		//Debug.Log(dt.GetDump());

		if (dt.ContainsKey("user_regions")){
			ISFSObject obj = dt.GetSFSObject("user_regions");

			UserRegions ur = DataManager.GetInstance().userRegionsList[obj.GetInt("regions_id")] as UserRegions;								
			ur.ISFSObjectToBean(obj);			

			//CSVMananger.GetInstance().userRegionsList[ur.regions_id] = ur;

			ur.worldHouse.initData(ur,ur.worldHouse.worldCamera);
		}

		if (dt.ContainsKey("user")){
			//更新用户资源;
			ISFSObject obj = dt.GetSFSObject("user");
			DataManager.GetInstance().userInfo.gold_count = obj.GetInt("gold_count");
			DataManager.GetInstance().userInfo.wood_count = obj.GetInt("wood_count");
			DataManager.GetInstance().userInfo.stone_count = obj.GetInt("stone_count");
			DataManager.GetInstance().userInfo.iron_count = obj.GetInt("iron_count");
			DataManager.GetInstance().userInfo.reward_count = obj.GetInt("reward_count");
		
			Helper.UpdateResUI("All",false);
		}

		is_server_return = true;
	}

	private void PlayWin()
	{
		BattleData.Instance.BattleIsOver = true;

		if(BattleData.Instance.BattleIsSuccess)
		{
			AudioPlayer.Instance.PlayMusic("winwinwin",false);
		}
		else
		{
			AudioPlayer.Instance.PlayMusic("battle_lost_02",false);
		}	

		//通知服务器攻击结束;
		SendAttackEnd();


		if (BattleData.Instance.BattleIsSuccess){
			UserRegions ur = DataManager.GetInstance().userRegionsList[Globals.LastSceneRegionsId] as UserRegions;
			if (ur != null && (ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE")){
				//TID_BUTTON_GO_TO_OUTPOST = 登上该岛;
				this.transform.Find("ReturnBtn/Label").GetComponent<UILabel>().text = StringFormat.FormatByTid("TID_BUTTON_GO_TO_OUTPOST");
			}else{
				this.transform.Find("ReturnBtn/Label").GetComponent<UILabel>().text = StringFormat.FormatByTid("TID_BUTTON_HOME");
			}
		}else{
			this.transform.Find("ReturnBtn/Label").GetComponent<UILabel>().text = StringFormat.FormatByTid("TID_BUTTON_HOME");
		}

		CostResultList = new List<BattleResultItem> ();
		ReceiveResultList = new List<BattleResultItem> ();
		parentItem = null;
		Dictionary<string,int> deadList = new Dictionary<string, int> ();
		foreach(CharInfo c in BattleData.Instance.DeadTrooperList)
		{
			if(deadList.ContainsKey(c.trooperData.tid))
			{
				deadList[c.trooperData.tid]++;
			}
			else
			{
				deadList.Add(c.trooperData.tid,1);
			}			 
		}
		
		
		
		
		
		//以下开始组装两个列表以及相关数据;
		if(BattleData.Instance.BattleIsSuccess)
		{
			if(EnemyNameAndResource.Instance.GoldResource>0)
				CreateResultItem (BattleResultType.Gold, 
				                  EnemyNameAndResource.Instance.GoldResource+EnemyNameAndResource.Instance.GoldResourceAdd);
			
			if(EnemyNameAndResource.Instance.WoodResource>0)
				CreateResultItem (BattleResultType.Wood, 
				                  EnemyNameAndResource.Instance.WoodResource+EnemyNameAndResource.Instance.WoodResourceAdd);
			
			if(EnemyNameAndResource.Instance.StoneResource>0)
				CreateResultItem (BattleResultType.Stone, 
				                  EnemyNameAndResource.Instance.StoneResource+EnemyNameAndResource.Instance.StoneResourceAdd);
			
			if(EnemyNameAndResource.Instance.IronResource>0)
				CreateResultItem (BattleResultType.Iron, 
				                  EnemyNameAndResource.Instance.IronResource+EnemyNameAndResource.Instance.IronResourceAdd);
			
			
			if(EnemyNameAndResource.Instance.artifact_num1>0)
			{
				BattleResultType t = stringToCrystleType(EnemyNameAndResource.Instance.artifact_name1);
				CreateResultItem (t, EnemyNameAndResource.Instance.artifact_num1);
			}
			
			if(EnemyNameAndResource.Instance.artifact_num2>0)
			{
				BattleResultType t = stringToCrystleType(EnemyNameAndResource.Instance.artifact_name2);
				CreateResultItem (t, EnemyNameAndResource.Instance.artifact_num2);
			}
			
			if(EnemyNameAndResource.Instance.artifact_num3>0)
			{
				BattleResultType t = stringToCrystleType(EnemyNameAndResource.Instance.artifact_name3);
				CreateResultItem (t, EnemyNameAndResource.Instance.artifact_num3);
			}
			
			if(EnemyNameAndResource.Instance.MedalResource>0||EnemyNameAndResource.Instance.MedalResourceAdd>0)
				CreateResultItem (BattleResultType.Medal, 
				                  EnemyNameAndResource.Instance.MedalResource+EnemyNameAndResource.Instance.MedalResourceAdd);
			
		}
		//
		
		foreach(string k in deadList.Keys)
		{
			BattleResultType type = BattleResultType.Heavy;
			if(k=="TID_RIFLEMAN")type = BattleResultType.Rifleman;
			if(k=="TID_WARRIOR")type = BattleResultType.Warrior;
			if(k=="TID_ZOOKA")type = BattleResultType.Zooka;
			if(k=="TID_TANK")type = BattleResultType.Tank;
			CreateResultItem (type, deadList[k]);
		}
		
		ResetPosition ();

		
		UIMask.Mask.gameObject.SetActive (true);
		gameObject.SetActive (true);
		
		if (ReceiveResultList.Count > 0)
			ReceiveEmpty.gameObject.SetActive (false);
		else
			ReceiveEmpty.gameObject.SetActive (true);
		if (CostResultList.Count > 0)
			CostEmpty.gameObject.SetActive (false);
		else
			CostEmpty.gameObject.SetActive (true);
		
		if (ReceiveResultList.Count > 0)
			ReceiveResultList [0].gameObject.SetActive (true);
		else if (CostResultList.Count > 0)
			CostResultList [0].gameObject.SetActive (true);
	}


	BattleResultType stringToCrystleType(string s)
	{
		BattleResultType t = BattleResultType.CommonPiece;
		if(s=="CommonPiece")
			t = BattleResultType.CommonPiece;
		else if(s=="CommonPieceDark")
			t = BattleResultType.CommonPieceDark;
		else if(s=="CommonPieceIce")
			t = BattleResultType.CommonPieceIce;
		else if(s=="CommonPieceFire")
			t = BattleResultType.CommonPieceFire;
		else if(s=="RarePiece")
			t = BattleResultType.RarePiece;
		else if(s=="RarePieceDark")
			t = BattleResultType.RarePieceDark;
		else if(s=="RarePieceIce")
			t = BattleResultType.RarePieceIce;
		else if(s=="RarePieceFire")
			t = BattleResultType.RarePieceFire;
		else if(s=="EpicPiece")
			t = BattleResultType.EpicPiece;
		else if(s=="EpicPieceDark")
			t = BattleResultType.EpicPieceDark;
		else if(s=="EpicPieceIce")
			t = BattleResultType.EpicPieceIce;
		else if(s=="EpicPieceFire")
			t = BattleResultType.EpicPieceFire;


			return t;

	}


	void CreateResultItem(BattleResultType type,int count)
	{
		GameObject obj = Instantiate (ResourceCache.load("UI/BattleResultItem")) as GameObject;
		BattleResultItem item = obj.GetComponent<BattleResultItem> ();
		BattleResultData data = new BattleResultData ();
		data.type = type;
		data.count = count;
		item.data = data;
		if(type==BattleResultType.Heavy
		   ||type==BattleResultType.Rifleman
		   ||type==BattleResultType.Zooka
		   ||type==BattleResultType.Tank
		   ||type==BattleResultType.Warrior)
		{
			obj.transform.parent = CostBox;
			CostResultList.Add(item);
		}
		else
		{
			obj.transform.parent = ReceiveBox;
			ReceiveResultList.Add(item);
		}

		item.transform.localPosition = Vector3.zero;
		if (parentItem != null) {
			parentItem.nextItem = item;				
		}
		parentItem = item;

	}

	void ResetPosition()
	{
		int receiveCount = ReceiveResultList.Count;
		int costCount = CostResultList.Count;

		float receiveBeginx = 0;
		if(receiveCount%2==0)
		{
			receiveBeginx = 0 - (receiveCount/2-1)*200f - 100f;
		}
		else
		{
			receiveBeginx = 0 - Mathf.CeilToInt(receiveCount/2)*200f;
		}

		float costBeginx = 0;
		if(costCount%2==0)
		{
			costBeginx = 0 - (costCount/2-1)*220f - 110f;
		}
		else
		{
			costBeginx = 0 - Mathf.CeilToInt(costCount/2)*220f;
		}

		for(int i=0;i<receiveCount;i++)
		{
			Vector3 pos = ReceiveResultList[i].transform.localPosition;
			pos = new Vector3(receiveBeginx+i*200f,pos.y,pos.z);
			ReceiveResultList[i].transform.localPosition = pos;
		}

		for(int i=0;i<costCount;i++)
		{
			Vector3 pos = CostResultList[i].transform.localPosition;
			pos = new Vector3(costBeginx+i*220f,pos.y,pos.z);
			CostResultList[i].transform.localPosition = pos;
		}

	}

	public void HideResultWin()
	{
		UIMask.Mask.gameObject.SetActive (false);
		gameObject.SetActive (false);
	}
}
