using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using BoomBeach;
using System.Collections.Generic;

public static class BuildHandle {

	static Dictionary<string,string> messages;
	static string messagePath = "message";
	static string GetMessage(int msgId){
		if (messages == null) {
			TextAsset ta = Resources.Load<TextAsset> (messagePath);
			messages = SysConfig.Load(ta);
		}
		if(messages!=null && messages.ContainsKey(msgId.ToString())){
			return messages [msgId.ToString()];
		}
		return msgId + " is not existing!";
	}

	//新建建筑
	public static void OnCreateBuild(BuildInfo s){
		if (s.CheckBuildPlaceAble ()) {
			if (!CheckWookerCount ()) {
				return;
			}
			ISFSObject dt = Helper.getCostDiffToGems(s.tid_level,0,true);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
					dt.GetUtfString("msg"), 
					dt.GetUtfString("title"), 
					gems.ToString(), 
					PopDialogBtnType.ImageBtn,
					dt, OnCreateBuildDialogYes,
					true,
					s
				);
			}
			else{
				AudioPlayer.Instance.PlaySfx("building_construct_07");
				CreateBuild (s);
			}
		}
	}

	static void OnCreateBuildDialogYes(ISFSObject dt,BuildInfo s = null){
		if (DataManager.GetInstance.userInfo.diamond_count >= dt.GetInt("Gems")){
			AudioPlayer.Instance.PlaySfx("building_construct_07");
			CreateBuild (s);
		}else{
			//宝石不够;
			//PopManage.Instance.ShowNeedGemsDialog(onDialogNo,onDialogNo);
		}
	}

	static void CreateBuild(BuildInfo build){
		ISFSObject dt = Helper.getCostDiffToGems(build.tid_level,0,true);
		//int gems = dt.GetInt("Gems");
		//当 部署 成功后，需要清空 TID_BUILDING_ARTIFACT_WORKSHOP 上的 status_tid_level
		if (build.csvInfo.BuildingClass == "Artifact"){
			build.status = BuildStatus.Normal;
			build.start_time = 0;
			build.end_time = 0;
			build.status_tid_level = null;//清空原有的神像;
			build.artifact_type = 0;//ArtifactType.None;
			build.artifact_boost = 0;
		}else{
			//扣除用户本地资源;
			Helper.SetResource(0,-dt.GetInt("Wood"),-dt.GetInt("Stone"),-dt.GetInt("Iron"),-dt.GetInt("Gems"),true);
			build.start_time = Helper.current_time();
			build.end_time = build.start_time + Helper.GetBuildTime(build.tid_level);		
			build.last_collect_time = build.end_time;//要到新建结束后，才能采集;
			build.RefreshBuildModel(build.tid,build.level);
			//需要时间;
			if (build.end_time > build.start_time)
				build.status = BuildStatus.New;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮);
			else{
				build.status = BuildStatus.Normal;//不需要时间;
				//设置最大容量及主建筑物级别;
				Helper.SetAllMaxLevel();
				Helper.SetAllMaxStored();
			}
		}
		//服务器还没有返回创建任务结束时,不能执行立即完成;
		build.isWaitForReturnBuildId = true;
		build.SetBuild (false, false);
		SendBuildNewToServer (build);
		//重新设置，所有建筑物的升级提醒标识;
		Helper.SetBuildUpgradeIcon();

		//重新计算版面数据;
		int count = Helper.CalcShopCates(false);
		//设置商城按钮数量;
		UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);
		MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid> ().clearMesh ();
		MoveOpEvent.Instance.UnDrawBuildPlan (MoveOpEvent.Instance.SelectedBuildInfo);
		build.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(build);
		//设置工人工作;
		WorkerManage.Instance.setWorkBuilding(Helper.getWorkBuilding());
	}

	static void SendBuildNewToServer(BuildInfo s){
		ISFSObject data = new SFSObject();
		data.PutInt("x",s.x);
		data.PutInt("y",s.y);
		data.PutUtfString ("buildingTid",s.tid);
		data.PutInt ("buildingLevel",1);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_CreateBuilding, false, HandleCreateResponse,s);
	}

	//取消新建建筑
	public static void OnCancelCreateBuild(BuildInfo s){
		if (s.status == BuildStatus.New){
			//新建取消;
			string title = StringFormat.FormatByTid("TID_POPUP_CANCEL_CONSTRUCTION_TITLE");
			string msg = StringFormat.FormatByTid("TID_POPUP_CANCEL_CONSTRUCTION",new object[]{StringFormat.FormatByTid(s.tid), 50});
			UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
				msg,
				title,
				"",
				PopDialogBtnType.ConfirmBtn,
				null,
				OnCancelCreateBuildDialogYes,
				true,
				s
			);
		}
	}

	static void OnCancelCreateBuildDialogYes(ISFSObject dt,BuildInfo s = null){
		if (s.status == BuildStatus.New)
			CancelCreateBuild (s);
	}

	static void CancelCreateBuild(BuildInfo s){
		Debug.Log ("CancelCreateBuild");
		if (s.status == BuildStatus.New) {
			//新建取消;返回50%;
			BuildCost bc = Helper.GetBuildCost (s.tid_level);
			bc.gold = bc.gold / 2;
			bc.wood = bc.wood / 2;
			bc.stone = bc.stone / 2;
			bc.iron = bc.iron / 2;
			s.OnCancelCreate ();
			Helper.SetResource(bc.gold,bc.wood,bc.stone,bc.iron,0,true);
			//重新计算版面数据;
			int count = Helper.CalcShopCates (false);
			//重新设置，所有建筑物的升级提醒标识;
			Helper.SetBuildUpgradeIcon();
			//设置商城按钮数量;
			//ScreenUIManage.Instance.SetShopCount (count);
			UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount (count);
			SendCancelBuildToServer (s.building_id);
			WorkerManage.Instance.setWorkBuilding(null);
		}
	}

	static void SendCancelBuildToServer(long buildId){
		Debug.Log("SendCancelBuildToServer");
		ISFSObject data = new SFSObject();
		data.PutLong("building_id", buildId);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_CancelBuilding, false, HandleResponse);
	}

	//升级建筑
	public static void OnUpgradeBuild(BuildInfo s){
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[s.tid_level] as CsvInfo;
		if (!CheckWookerCount ()) {
			return;
		}
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		if (msg == null && s.status == BuildStatus.Normal){
			ISFSObject dt = Helper.getCostDiffToGems(s.tid_level,1,true);
			dt.PutLong("building_id",s.building_id);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
					dt.GetUtfString("msg"), 
					dt.GetUtfString("title"), 
					gems.ToString(), 
					PopDialogBtnType.ImageBtn, 
					dt, 
					OnUpgradeBuildDialogYes,
					true,
					s
				);
			}
			else{
				AudioPlayer.Instance.PlaySfx("building_construct_07");
				UpgradeBuild(s);
			}
		}else{
			PopManage.Instance.ShowMsg(msg);
		}
	}

	static void OnUpgradeBuildDialogYes(ISFSObject dt,BuildInfo buildInfo = null){
		if (DataManager.GetInstance.userInfo.diamond_count >= dt.GetInt("Gems")){
			//BuildInfo s = DataManager.GetInstance.BuildList[dt.GetLong("building_id")] as BuildInfo;
			AudioPlayer.Instance.PlaySfx("building_construct_07");
			UpgradeBuild (buildInfo);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	static void UpgradeBuild(BuildInfo s)
	{
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[s.tid_level] as CsvInfo;
		ISFSObject dt = Helper.getCostDiffToGems(s.tid_level,1,true);
		if (s.status != BuildStatus.Normal) {
			Debug.LogError (s.building_id + " is not in normal status!");
			return;
		}
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		if(msg!=null){
			UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
			return;
		}
		//扣除用户本地资源;
		Helper.SetResource(-dt.GetInt("Gold"),-dt.GetInt("Wood"),-dt.GetInt("Stone"),-dt.GetInt("Iron"),-dt.GetInt("Gems"),true);
		s.start_time = Helper.current_time();
		s.end_time = s.start_time + Helper.GetUpgradeTime(s.tid_level);		
		s.last_collect_time = s.end_time;//要到新建结束后，才能采集;
		s.status = BuildStatus.Upgrade;
		if(s.transform.Find("buildin")!=null)
		{
			s.transform.Find("buildin").gameObject.SetActive(true);
		}
		BuildHandle.SendBuildUpgradeToServer (s);
		RefreshBuild (s);
	}

	static void SendBuildUpgradeToServer(BuildInfo s){
		Debug.Log("SendBuildUpgradeToServer");
		ISFSObject data = new SFSObject();
		data.PutLong("building_id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_UpgradeBuilding, false, HandleResponse);
	}

	//取消升级建筑
	public static void OnCancelUpgradeBuild(BuildInfo s){
		if (s.status == BuildStatus.Upgrade){
			//新建升级;
			string title = StringFormat.FormatByTid("TID_POPUP_CANCEL_UPGRADE_TITLE");
			string msg = StringFormat.FormatByTid("TID_POPUP_CANCEL_UPGRADE",new object[]{StringFormat.FormatByTid(s.tid), 50});
			UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
				msg,
				title,
				"",
				PopDialogBtnType.ConfirmBtn,
				null,
				OnCancelUpgradeBuildDialogYes,
				true,
				s
			);
		}
	}

	static void OnCancelUpgradeBuildDialogYes(ISFSObject dt,BuildInfo s = null){
		if (s.status == BuildStatus.Upgrade)
			CancelUpgradeBuild (s);
	}

	static void CancelUpgradeBuild(BuildInfo s){
		Debug.Log ("CancelUpgradeBuild");
		if (s.status == BuildStatus.Upgrade){
			//升级取消;返回50%;
			BuildCost bc = Helper.getUpgradeCost(s.tid_level);
			bc.gold = bc.gold/2;
			bc.wood = bc.wood/2;
			bc.stone = bc.stone/2;
			bc.iron = bc.iron/2;
			s.status = BuildStatus.Normal;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)
			Helper.SetResource(bc.gold,bc.wood,bc.stone,bc.iron,0,true);
			s.RefreshBuildModel(s.tid,s.level);
			s.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(s);
			Helper.SetBuildUpgradeIcon();//重新设置，所有建筑物的升级提醒标识;
			//int count = Helper.CalcShopCates(false);//重新计算版面数据;
			//ScreenUIManage.Instance.SetShopCount (count);
			//UIManager.Instance().GetCtroller<MainInterfaceCtrl>().SetShopCount(count);//设置商城按钮数量;
			WorkerManage.Instance.setWorkBuilding(null);
			SendCancelBuildToServer (s.building_id);
		}
	}

	//加速升级(建造)建筑
	public static void OnSpeedUpUpgradeBuild(BuildInfo s){
		int current_time = Helper.current_time();
		int gems = CalcHelper.calcTimeToGems(s.end_time - current_time);
		string title = null;
		string msg = null;
		if (s.status == BuildStatus.New){
			title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP");
			msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_CONSTRUCTION",new object[]{ gems,StringFormat.FormatByTid(s.tid) });
		}else if (s.status == BuildStatus.Upgrade){
			title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP");
			msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_UPGRADE",new object[]{ gems,StringFormat.FormatByTid(s.tid) });
		}
		ISFSObject dt = new SFSObject();
		dt.PutLong("building_id",s.building_id);
		UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(msg, title, gems.ToString(), PopDialogBtnType.ImageBtn, dt, OnSpeedUpUpgradeBuildDialogYes,true,s);
	}

	static void OnSpeedUpUpgradeBuildDialogYes(ISFSObject dt,BuildInfo s = null){
		int current_time = Helper.current_time();
		int gems = CalcHelper.calcTimeToGems(s.end_time - current_time);
		if (DataManager.GetInstance.userInfo.diamond_count >= gems){
			SpeedUpUpgradeBuild (s);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	static void SpeedUpUpgradeBuild(BuildInfo s){
		//必须再做一次判断，因为用户点击确认会有一段时间间隔，也许这个时候升级已经完成了。
		int gems = CalcHelper.calcTimeToGems(s.end_time - Helper.current_time());
		if (gems > 0) {
			if (Helper.SetResourceCount ("Gems", -gems, false, true) == -1) {
				PopManage.Instance.ShowNeedGemsDialog ();
			} else {
				SendBuildUpgradeSpeedUpToServer (s);
				OnBuildDone (s);
			}
		}
	}

	static void SendBuildUpgradeSpeedUpToServer(BuildInfo s){
		Debug.Log("SendBuildUpgradeSpeedUpToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (s.building_id);
		data.PutLong("id", s.building_id);
		data.PutInt ("type",0);
		SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_SpeedUP, false, HandleResponse);
	}

	//对正常建筑用宝石立即升级
	public static void ImmediateUpgradeBuild(BuildInfo s){
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[s.tid_level] as CsvInfo;
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);		
		if (msg == null && s.status == BuildStatus.Normal){
			int gems = Helper.GetUpgradeInstant(csvInfo.TID_Level);
			if (DataManager.GetInstance.userInfo.diamond_count >= gems){
				Helper.SetResource (0,0,0,0,-gems,true);
				SendBuildUpgradeImmediatelyToServer (s);
				OnBuildDone (s);
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog();
			}
		}else{
			if (msg != null)
				UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
		}	
	}

	static void SendBuildUpgradeImmediatelyToServer(BuildInfo s) {
		Debug.Log("SendBuildUpgradeImmediately");
		ISFSObject data = new SFSObject();
		Debug.Log (s.building_id);
		data.PutLong("id", s.building_id);
		data.PutInt ("type",0);
		SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_ImmediatelyUp, false, HandleResponse);
	}

	//升级时间结束
	public static void BuildTimeUp(BuildInfo s){
		SendBuildDoneToServer (s);
		OnBuildDone (s);
	}

	static void OnBuildDone(BuildInfo s){
		if (s.status == BuildStatus.New) {
			//升级完成,更新模型及重新初始化数据;
			//比如：level + 1, 更新模型;
			//s.level = s.level;
			s.tid_level = s.tid + "_" + s.level;
			s.csvInfo = CSVManager.GetInstance.csvTable [s.tid_level] as CsvInfo;
		} else {
			s.level = s.level + 1;
			s.tid_level = s.tid + "_" + s.level;
			s.csvInfo = CSVManager.GetInstance.csvTable [s.tid_level] as CsvInfo;
		}
		Helper.EXPCollect (s,s.csvInfo.XpGain);
		AudioPlayer.Instance.PlaySfx("building_finished_01");
		s.status = BuildStatus.Normal;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)
		WorkerManage.Instance.setWorkBuilding(null);
		Helper.SetAllMaxLevel();
		Helper.SetAllMaxStored();
		s.RefreshBuildModel(s.tid,s.level);
		if(s.transform.Find("buildin")!=null)
		{
			s.transform.Find("buildin").gameObject.SetActive(false);
		}
		if (s.tid == "TID_BUILDING_MAP_ROOM"){
			//雷达,新建或升级，需要更新世界地图数据;
			Helper.HandleWolrdMap(true);
		}
		//重新设置，所有建筑物的升级提醒标识;
		Helper.SetBuildUpgradeIcon();
		//重新计算版面数据;
		int count = Helper.CalcShopCates(false);
		//设置商城按钮数量;
		//ScreenUIManage.Instance.SetShopCount (count);
		UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);
		//关闭所有界面窗口;
		if (PopWin.current!=null)PopWin.current.CloseTween();
		//刷新按钮状态;
		//Debug.Log(s.building_id + ";status:" + s.status.ToString());
		s.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(s);
	}

	public static void RefreshBuild(BuildInfo s){
		//重新设置，所有建筑物的升级提醒标识;
		Helper.SetBuildUpgradeIcon();
		//重新计算版面数据;
		int count = Helper.CalcShopCates(false);
		//设置商城按钮数量;
		//ScreenUIManage.Instance.SetShopCount (count);
		UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);
		//设置工人工作;
		WorkerManage.Instance.setWorkBuilding(Helper.getWorkBuilding());
		//关闭所有界面窗口;
		if(PopWin.current!=null)PopWin.current.CloseTween();
		//刷新按钮状态;
		s.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(s);
	}

	static bool CheckWookerCount(){
		long min_b_id = 0;
		if (DataManager.GetInstance.userInfo.worker_count <= Helper.GetWorkeringCount (ref min_b_id)) {
			BuildInfo min_s = (BuildInfo)DataManager.GetInstance.buildList [min_b_id];
			int seconds = min_s.end_time - Helper.current_time ();					
			int gems_count = CalcHelper.calcTimeToGems (seconds);
			ISFSObject dt = new SFSObject ();
			dt.PutLong ("building_id", min_s.building_id);
			UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog (
				StringFormat.FormatByTid ("TID_NOT_ENOUGH_WORKERS_HEADER"), 
				StringFormat.FormatByTid ("TID_NOT_ENOUGH_WORKERS_TEXT"),
				gems_count.ToString (), 
				PopDialogBtnType.ImageBtn, dt,
				OnSpeedUpUpgradeBuildDialogYes,
				true,
				min_s
			);
			return false;
		} else {
			return true;
		}
	}

	//更新建筑坐标
	public static void SendBuildLocationToServer(BuildInfo s){
		ISFSObject data = new SFSObject();
		data.PutInt("x",s.x);
		data.PutInt("y",s.y);
		data.PutInt("regions_id",Globals.regions_id);
		data.PutLong("building_id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_MoveBuilding, false, HandleResponse);
	}

	//正常建造完成
	public static void SendBuildDoneToServer(BuildInfo s) {
		Debug.Log("SendBuildDoneToServer:" + s.building_id);
		ISFSObject data = new SFSObject();
		data.PutLong("building_id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_BuildDone, false, HandleResponse);
	}

	//打印客户返回的code
	public static void HandleResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		if (dt.GetInt ("code") == 1) {
			Debug.Log ("HandleResponse:" + "{message " + GetMessage (dt.GetInt ("code")) + "},{code " + dt.GetInt ("code") + "}");
		} else {
			Debug.LogError ("HandleResponse:" + "{message " + GetMessage (dt.GetInt ("code")) + "},{code " + dt.GetInt ("code") + "}");
		}
	}

	public static void HandleCreateResponse(ISFSObject dt,BuildInfo buildInfo){
		if (dt.GetInt ("code") == 1){
			Debug.Log ("HandleResponse:" + "{message " + GetMessage (dt.GetInt ("code")) + "},{code " + dt.GetInt ("code") + "}");
			if (buildInfo != null) {
				DataManager.GetInstance.buildList.Remove (buildInfo.building_id);
				buildInfo.building_id = dt.GetLong ("building_id");
				DataManager.GetInstance.buildList [buildInfo.building_id] = buildInfo;
				buildInfo.isWaitForReturnBuildId = false;
			}
		}else {
			Debug.LogError ("HandleResponse:" + "{message " + GetMessage (dt.GetInt ("code")) + "},{code " + dt.GetInt ("code") + "}");
		}
	}

	public static void BuildUpdate(BuildInfo s){
		if(Helper.current_time() >= s.end_time){
			BuildTimeUp (s);
			s.status = BuildStatus.Normal;
		}
	}

}
