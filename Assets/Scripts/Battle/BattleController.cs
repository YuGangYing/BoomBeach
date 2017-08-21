using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using BoomBeach;

public class BattleController:MonoBehaviour {

	/// <summary>
	/// 检测是否击中目标;
	/// </summary>
	public static bool CheckAttackedBuild(float DamageRadius,Vector3 AttackPoint,BuildInfo buildInfo)
	{
		float centerPosX = buildInfo.transform.position.x + (buildInfo.GridCount * 0.5f);
		float centerPosZ = buildInfo.transform.position.z + (buildInfo.GridCount * 0.5f);
		Vector3 point = new Vector3 (centerPosX,0,centerPosZ); //圆心;
		float radius = (buildInfo.GridCount - 1) * 0.5f;
		if (radius < 0) radius = 0;

		if((AttackPoint - point).magnitude-radius<=DamageRadius)
		{
			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// 获取建筑被建区中随机点，远程兵使用;
	/// </summary>
	public static Vector3 GetBuildAreaRandPosition(BuildInfo buildInfo)
	{
		float centerPosX = buildInfo.transform.position.x + (buildInfo.GridCount * 0.5f);
		float centerPosZ = buildInfo.transform.position.z + (buildInfo.GridCount * 0.5f);

        //每个建筑的坐标原点在其左下角，所以加上一个右上方向的长度为对角线一半的向量可得出该建筑的实际中心点

		Vector3 point = new Vector3 (centerPosX,0,centerPosZ); //圆心;
		float radius = (buildInfo.GridCount - 1) * 0.5f;
		if(radius>0)
		{
			return Globals.GetRandPointInCircle(point,radius);
		}
		else
		{
			return point;
		}
	}

	/// <summary>
	/// 获取建筑中心点;
	/// </summary>
	public static Vector3 GetBuildCenterPosition(BuildInfo buildInfo)
	{
		float centerPosX = buildInfo.transform.position.x + (buildInfo.GridCount * 0.5f);
		float centerPosZ = buildInfo.transform.position.z + (buildInfo.GridCount * 0.5f);
		Vector3 point = new Vector3 (centerPosX,0,centerPosZ); //圆心;
		return point;
	}

	/// <summary>
	/// 获取建筑周围随机站立点,用于寻找寻路的目标点;
	/// </summary>
	public static Vector3 GetBuildAroundRandPosition(BuildInfo buildInfo,Transform trooper = null)
	{
		return Globals.GetRandStandPointAroundBuild(buildInfo,trooper);
	}

	//是否点击在游戏区域内;
	public static bool IsClickInGameArea(Vector3 worldPosition)
	{
		GridInfo grid = Globals.LocateGridInfo(worldPosition);
		if(grid.isInArea)
			return true;
		else
		{
			UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(LocalizationCustom.instance.Get("TID_CAN_NOT_USE_ABILITY_ON_TILE"));
            //MessageManage.Instance.ShowMessage(LocalizationCustom.instance.Get("TID_CAN_NOT_USE_ABILITY_ON_TILE"));
			return false;
		}
	}

	//是否点击在登陆点;
	public static BeachData IsClickInLandArea(Vector3 touchPosition)
	{
		BeachData beachData = null;
		Ray ray = MoveOpEvent.Instance.islandCamera.ViewportPointToRay(MoveOpEvent.Instance.islandCamera.ScreenToViewportPoint(touchPosition));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit,1000f,1<<11))
		{
			beachData = hit.transform.GetComponent<BeachData>();
		}
		return beachData;

	}

    //Place troop and weapon
	public static void PlaceTroopAndWeapon(Vector3 touchPosition)
	{
		Vector3 worldPosition = MoveOpEvent.Instance.screenToWorldPosition (touchPosition);
		//Debug.Log (worldPosition); 

		if (BattleData.Instance.BattleIsEnd)
			return;

		BattleTrooperData btd = BattleData.Instance.currentSelectBtd;
		if (btd == null)return;
		//检查是否可以使用
		if (btd.weaponCost > Globals.EnergyTotal) {
			UIManager.GetInstance.GetController<BattleInterfaceCtrl>().CannotFire ();
			return;
		}
		if(btd.building_id==0)
		{
			//武器;
			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				if(IsClickInGameArea(worldPosition))
				{
					if(!BattleData.Instance.BattleIsStart)
					{
						BattleData.Instance.BattleIsStart = true;  //游戏开始;
						AudioPlayer.Instance.PlayMusic("combat_music_02");
					}
					if(!BattleData.Instance.TrooperDeployed)
						BattleData.Instance.TrooperDeployed = true; //已出兵;
					//if(btd.uiItem!=null && !btd.uiItem.isDisabled)
					//{
						FireItem fi = new FireItem();
						fi.btd = btd;
						fi.attackPoint = worldPosition;
						//立即发射该炮弹;
						BattleData.Instance.gunBoat.Fire(fi);
					//}
				}
			}
		}
		else
		{
			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				BeachData beachData = IsClickInLandArea(touchPosition);
				if(beachData!=null)
				{
					if(!BattleData.Instance.BattleIsStart)
					{
						BattleData.Instance.BattleIsStart = true;  //游戏开始;
						AudioPlayer.Instance.PlayMusic("combat_music_02");
					}

					LandCraft landCraft = BattleData.Instance.landCraftDic[btd.id.ToString()];
					//if(btd.uiItem!=null && !btd.uiItem.isDisabled)
					//{
					LandCraftController lcc = landCraft.gameObject.GetComponent<LandCraftController> ();
					if(lcc == null)
						lcc = landCraft.gameObject.AddComponent<LandCraftController>();
					if (!btd.isDeployed) {
						btd.isDeployed = true;
						landCraft.beachData = beachData;
						landCraft.direct = beachData.beachDirect;
						landCraft.initTrooperStand();

						lcc.landPoint = worldPosition;
						lcc.landCraft = landCraft;
						lcc.state = AISTATE.MOVING;
						lcc.Locate ();
						landCraft.gameObject.SetActive (true);
						UIManager.GetInstance.GetController<BattleInterfaceCtrl>().OnTroopDeploy (btd);
						Globals.EnergyTotal-=btd.weaponCost;
						/*
						int length = BattleTrooperList.Instance.BattleItems.Count;

						for(int i=0;i<length;i++)
						{
							if(!BattleTrooperList.Instance.BattleItems[i].isDisabled)
							{
								if(ScreenUIManage.Instance!=null) ScreenUIManage.Instance.OnClickBattleTrooper(BattleTrooperList.Instance.BattleItems[i].gameObject);
								break;
							}
						}
						*/
						//开始录像;
						ReplayNodeData rnd = new ReplayNodeData();
						rnd.SelfID = landCraft.btd.id;
						rnd.SelfType = EntityType.LandCraft;
						rnd.State = AISTATE.MOVING;
						rnd.DestX = worldPosition.x;
						rnd.DestZ = worldPosition.z;
						BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
						//产生旗子;
						GameObject flag = Instantiate(ResourceCache.Load("Model/flag")) as GameObject;
						flag.transform.parent = BuildManager.GetInstance.buildContainer;
						flag.transform.position = Globals.GetRandPointInCircle(worldPosition,1f);
						flag.transform.localScale = Vector3.one;
						Destroy (flag,10);
						//BattleData.Instance.flags.Enqueue(flag);
						//部署军队同时较验军队数据;
						Helper.SendAttackDeployTroops(btd);
						NextLandCraft ();
					}
				}
				else
				{
					UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(LocalizationCustom.instance.Get("TID_INVALID_PLACEMENT"));
				}
			}
		}

	}

	static void NextLandCraft(){
		BattleTrooperData currentBtd = BattleData.Instance.currentSelectBtd;
		int index = Globals.battleTrooperList.IndexOf (currentBtd);
		for(int i = index;i < Globals.battleTrooperList.Count;i++)
		{
			if(Globals.battleTrooperList[i]!=currentBtd && !Globals.battleTrooperList[i].isDeployed && Globals.battleTrooperList[i].weaponCost <= Globals.EnergyTotal){
				BattleData.Instance.currentSelectBtd = Globals.battleTrooperList [i];
				return;
			}
		}
		for(int i = 0;i < index;i++){
			if(Globals.battleTrooperList[i]!=currentBtd && !Globals.battleTrooperList[i].isDeployed && Globals.battleTrooperList[i].weaponCost <= Globals.EnergyTotal){
				BattleData.Instance.currentSelectBtd = Globals.battleTrooperList [i];
				return;
			}
		}
	}


	public static void InitShipAndTrooper(BattleTrooperData btd)
	{
		string tid_level = "";
		if (btd == null)
			tid_level = "TID_BUILDING_GUNSHIP_1";
		else
			tid_level = btd.landingShipTidLevel;
		CsvInfo csvData = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;
		string buildLayoutPath = "";
		string buildSpritePath = "";
		if(csvData.TID=="TID_BUILDING_LANDING_SHIP")
		{
			buildLayoutPath = "Model/Layout/LandCraftLayout";
		}
		if(csvData.TID=="TID_BUILDING_GUNSHIP")
		{
			buildLayoutPath = "Model/Layout/GunShipLayout";
		}
		buildSpritePath = "Model/Build3d/"+csvData.ExportName;
		GameObject buildLayoutInstance = null;
		GameObject buildSpriteInstance = null;
		buildLayoutInstance = GameObject.Instantiate(ResourceCache.Load(buildLayoutPath)) as GameObject;
		buildSpriteInstance = GameObject.Instantiate(ResourceCache.Load(buildSpritePath)) as GameObject;

		buildSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
		buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
		buildSpriteInstance.transform.localPosition = Vector3.zero; 
		buildSpriteInstance.transform.name = "BuildMain";
		buildLayoutInstance.transform.parent = SpawnManager.GetInstance.characterContainer;
		
		BuildInfo buildInfo = buildLayoutInstance.GetComponent<BuildInfo>();
		buildInfo.buildSpritePath = buildSpritePath;
		if(csvData.TID=="TID_BUILDING_LANDING_SHIP")
		{
			LandCraft lc = buildLayoutInstance.GetComponent<LandCraft>();
			lc.Init();	
			//lc.direct = BattleData.Instance.beachDirect; 不在初始化时指定，改为由海滩数据指定，在出兵时指定;
			lc.btd = btd;
			if(DataManager.GetInstance.sceneStatus==SceneStatus.BATTLEREPLAY){
				//取从服务端下载回来的值;
				lc.btd.id = GameLoader.Instance.GunboatBattleIDList[lc.btd.building_id];
			}else{
				lc.btd.id = BattleData.Instance.LandCraftIdx;
				BattleData.Instance.LandCraftIdx++;
			}
			BattleData.Instance.landCraftDic.Add(lc.btd.id.ToString(),lc);
			buildLayoutInstance.name = csvData.TID+"_"+lc.btd.id;
		}
		if (csvData.TID == "TID_BUILDING_GUNSHIP")//实例化战舰
		{
			GunBoat gb = buildLayoutInstance.GetComponent<GunBoat>();
			gb.Init();	
			gb.direct = BattleData.Instance.gunBoatDirect;
			BattleData.Instance.gunBoat = gb;
			buildLayoutInstance.name = csvData.TID;
			GunBoatController gbc =  buildLayoutInstance.AddComponent<GunBoatController>();
			gbc.gunBoat = gb;
			buildLayoutInstance.transform.position = BattleData.Instance.gunBoatPosition;
		}
		buildLayoutInstance.layer = 16;
		Transform[] t = buildSpriteInstance.GetComponentsInChildren<Transform> (true);
		for(int i =0;i<t.Length;i++)
		{
			t[i].gameObject.layer = 16;
		}
		if(csvData.TID=="TID_BUILDING_LANDING_SHIP")
		{
			LandCraft landCraft = buildLayoutInstance.GetComponentInChildren<LandCraft>();
			landCraft.TrooperList = new Dictionary<string, CharInfo>();
			if(landCraft!=null)
			{
				if(btd.num>0)
				{
					landCraft.CreateTrooper(btd);
				}
			}
		}
		if(csvData.TID=="TID_BUILDING_LANDING_SHIP")//还没开始登陆作战，先隐藏该登陆舰
			buildLayoutInstance.SetActive (false);
	}

	public static bool CheckAllTroopDead()
	{
		if(BattleData.Instance.DeadTrooperList.Count==BattleData.Instance.trooperDic.Count)
		{
			return true;
		}
		else
		{
			return false;
		}
		/*
		bool isBattleEnd = false;
		if(BattleData.Instance.DeadTrooperList.Count==BattleData.Instance.TrooperList.Count)
		{
			isBattleEnd = true;
			foreach(BattleItem it in BattleWeaponList.Instance.BattleItems)
			{
				if((it.btd.csvInfo.Name=="Artillery"||it.btd.csvInfo.Name=="Airstrike")&&!it.isDisabled)
				{
					isBattleEnd = false;
					break;
				}
			}
		}
		else
		{
			//兵未全死，兵已全出未死或未出兵两种;
			if(BattleData.Instance.AllocateTrooperList.Count==BattleData.Instance.TrooperList.Count)
			{
				//已全出未全死;
				return false;
			}
			else
			{
				//还有兵未出;
				isBattleEnd = true;
				foreach(BattleItem it in BattleWeaponList.Instance.BattleItems)
				{
					if((it.btd.csvInfo.Name=="Artillery"||it.btd.csvInfo.Name=="Airstrike")&&!it.isDisabled)
					{
						isBattleEnd = false;
						break;
					}
				}
				
				if(isBattleEnd)
				foreach(BattleItem it in BattleTrooperList.Instance.BattleItems)
				{
					if(!it.isDisabled)
					{
						isBattleEnd = false;
						break;
					}
				}

			}

		}
		return isBattleEnd;
		*/
	}

	private Queue<ReplayNodeData> replayQueue;

	void Update()
	{
		if(DataManager.GetInstance.sceneStatus!=SceneStatus.ENEMYBATTLE && DataManager.GetInstance.sceneStatus!=SceneStatus.BATTLEREPLAY)
			return;


		if(BattleData.Instance.BattleIsEnd)
		{
			//确定是否所有士兵都撤退到位，如果是显示结算面板
			if(!BattleData.Instance.AllTrooperRetreat && BattleData.Instance.DeadTrooperList.Count+BattleData.Instance.RetreatTrooperList.Count>=BattleData.Instance.AllocateTrooperList.Count)
			{
				BattleData.Instance.AllTrooperRetreat = true;
				//调试用，该行代码将放在兵的撤退控制中，全部撤完，显示;
				if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
				{
					if(ScreenUIManage.Instance!=null) 
						ScreenUIManage.Instance.battleResultWin.ShowResultWin();
					UIManager.GetInstance.GetController<BattleResultCtrl>().ShowPanel ();
				}
			}
			if(BattleData.Instance.BattleIsSuccess)
			{
				if(!BattleData.Instance.isOnSuccessConroutine)
				{
					BattleData.Instance.isOnSuccessConroutine = true;
					StartCoroutine (_SuccessConroutine());
				}
				/*
				List<string> keys = new List<string>();

				var enumerator = BattleData.Instance.BuildList.Values.GetEnumerator();
				while(enumerator.MoveNext())
				{
					var pair = enumerator.Current;
					BuildInfo b = BattleData.Instance
				}
				*/
				/**
				foreach(string k in BattleData.Instance.BuildList.Keys)
				{
					BuildInfo b = BattleData.Instance.BuildList[k];
					keys.Add(k);
					if(!b.IsDead)
					{
						b.IsDead = true;
						break;
					}
				}
				foreach(string k in keys)
				{
					BattleData.Instance.BuildList.Remove(k);
				}
				**/
			}

            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.CountDownCouterLabel.text = "0"+LocalizationCustom.instance.Get("TID_TIME_SECS");
			return;
		}
		if(BattleData.Instance.BattleIsStart)
		{
			//TODO
			BattleData.Instance.TimeFromBegin+= (int)(Time.deltaTime*1000);
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.CountDownLabel.text = LocalizationCustom.instance.Get("TID_BATTLE_TIME_TITLE");
			BattleData.Instance.BattleEndCountDown-=Time.deltaTime;
			int totalScend = Mathf.RoundToInt(BattleData.Instance.BattleEndCountDown);
			int sec = totalScend/60;
			string m="";
			if(sec>0)
				m = sec+LocalizationCustom.instance.Get("TID_TIME_MINS");
			string s = (totalScend%60)+LocalizationCustom.instance.Get("TID_TIME_SECS");
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.CountDownCouterLabel.text = m+" "+s;

			if(BattleData.Instance.BattleEndCountDown<=0&&!BattleData.Instance.BattleIsEnd)
			{
				BattleData.Instance.TrooperDeployed = true;
				BattleData.Instance.BattleIsEnd = true;
				//倒计时结束，士兵中止;
				foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
				{
					charInfo.path = null;
					charInfo.AttackState = AISTATE.STANDING;
					charInfo.State = AISTATE.STANDING;
				}
				if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE) {
					//倒计时结束，直接弹窗;
					if (ScreenUIManage.Instance != null)
						ScreenUIManage.Instance.battleResultWin.ShowResultWin();//TODO
					UIManager.GetInstance.GetController<BattleResultCtrl>().ShowPanel ();
				} 
			}

			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				if(replayQueue==null)replayQueue = new Queue<ReplayNodeData>();
				while(BattleData.Instance.BattleCommondQueue.Count>0)
				{
					if(replayQueue.Count>100)
					{
						Helper.SendAttackLog(true,replayQueue);
						replayQueue = new Queue<ReplayNodeData>();
						break;
					}
					ReplayNodeData rnd = BattleData.Instance.BattleCommondQueue.Dequeue();
					replayQueue.Enqueue(rnd);
				}
			}
			/*
			if(BattleData.Instance.BattleCommondQueue.Count>0)
			{
				ReplayNodeData rnd = BattleData.Instance.BattleCommondQueue.Dequeue();
			//ReplayNodeData rnd =  ReplayNodeData.ToData(cmd);
			Debug.Log(rnd.TimeFromBegin+" "+rnd.SelfID+" "+rnd.SelfType);
			}
*/
		}
		else
		{
			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
                if (ScreenUIManage.Instance != null)
                    ScreenUIManage.Instance.CountDownLabel.text = LocalizationCustom.instance.Get("TID_BATTLE_START_TIME");
				BattleData.Instance.BattleStartCountDown-=Time.deltaTime;
//				int second = Mathf.RoundToInt(BattleData.Instance.BattleStartCountDown);
	//			string secondStr = second+LocalizationCustom.instance.Get("TID_TIME_SECS");
				/*
				if(second<=3&&second>0&&ScreenUIManage.Instance.CountDownCouterLabel.text!=secondStr)
				{
					GameObject countDown = ResourceCache.load("UI/CountDown") as GameObject;
					GameObject countDownObj = Instantiate(countDown) as GameObject;
                    if (ScreenUIManage.Instance != null)
                        countDownObj.transform.parent = ScreenUIManage.Instance.CountDownLabel.transform.parent;
					countDownObj.transform.localScale = Vector3.one*2;
					countDownObj.GetComponent<CountDownController>().text = second.ToString();
				}
                if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.CountDownCouterLabel.text = secondStr;
				*/
				if(BattleData.Instance.BattleStartCountDown<=0)
				{
					BattleData.Instance.BattleIsStart = true;  //开始倒计时结束，战斗开始;
					AudioPlayer.Instance.PlayMusic("combat_music_02");
				}
			}

		}
	}


	public int succcessConroutineInterval = 5;//间隔5帧
	IEnumerator _SuccessConroutine(){
		Debug.Log ("_SuccessConroutine");
		//移除已经摧毁建筑
		List<string> keys = new List<string> ();
		List<string> aliveKeys = new List<string> ();
		foreach(string k in BattleData.Instance.buildDic.Keys)
		{
			BuildInfo b = BattleData.Instance.buildDic[k];
			if (b.IsDead) {
				keys.Add (k);
			} else {
				aliveKeys.Add (k);
			}
		}
		for(int i=0;i<keys.Count;i++)
		{
			BattleData.Instance.buildDic.Remove (keys[i]);
		}

		while(BattleData.Instance.buildDic.Count > 0)
		{
			for(int i =0;i < succcessConroutineInterval;i++)
			{
				yield return new WaitForEndOfFrame ();
			}
			string key = aliveKeys [0];
			aliveKeys.Remove (key);
			BuildInfo b = BattleData.Instance.buildDic [key];
			b.IsDead = true;
			BattleData.Instance.buildDic.Remove (key);
		}
	}


}
