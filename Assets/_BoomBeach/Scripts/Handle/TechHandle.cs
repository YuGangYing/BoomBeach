using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using BoomBeach;

public static class ResearchHandle {
	//升级科技
	public static void OnResearch(BuildInfo s,string tid_level){
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable[tid_level] as CsvInfo;
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		if (msg == null && s.status == BuildStatus.Normal){
			ISFSObject dt = Helper.getCostDiffToGems(tid_level,1,true);
			dt.PutLong("building_id",s.building_id);
			dt.PutUtfString("tid_level",tid_level);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				UIManager.GetInstance().GetComponent<PopMsgCtrl>().ShowDialog(
					dt.GetUtfString("msg"), 
					dt.GetUtfString("title"), 
					gems.ToString(), 
					PopDialogBtnType.ImageBtn, 
					dt, 
					OnDialogUpgradeTechYes,
					true,
					s
				);
			}
			else{
				AudioPlayer.Instance.PlaySfx("building_construct_07");
				UpgradeTech(s,tid_level);
			}
		}else{
			PopManage.Instance.ShowMsg(msg);
		}
	}

	static void OnDialogUpgradeTechYes(ISFSObject dt,BuildInfo buildInfo = null){
		if (DataManager.GetInstance().userInfo.diamond_count >= dt.GetInt("Gems")){
			//BuildInfo s = Globals.BuildList[dt.GetLong("building_id")] as BuildInfo;
			string tid_level = dt.GetUtfString("tid_level");
			AudioPlayer.Instance.PlaySfx("building_construct_07");
			UpgradeTech (buildInfo,tid_level);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	static void UpgradeTech(BuildInfo s,string tid_level){
		Debug.Log ("UpgradeTech:" + tid_level);
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable[tid_level] as CsvInfo;
		ISFSObject dt = Helper.getCostDiffToGems(tid_level,1,true);
		if (s.status != BuildStatus.Normal) {
			Debug.LogError (s.building_id + " is not in normal status!");
			return;
		}
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		if(msg!=null){
			UIManager.GetInstance().GetController<NormalMsgCtrl>().ShowPop(msg);
			return;
		}
		s.start_time = Helper.current_time();
		s.end_time = s.start_time + Helper.GetUpgradeTime(tid_level);		
		s.status_tid_level = tid_level;
		s.status = BuildStatus.Research;
		Helper.SetResource(-dt.GetInt("Gold"),-dt.GetInt("Wood"),-dt.GetInt("Stone"),-dt.GetInt("Iron"),-dt.GetInt("Gems"),true);
		if (csvInfo.TID_Type == Helper.TID_TYPE_CHARACTERS)
			SendTrooperUpgradeToServer (csvInfo.TID);
		else
			SendTechUpgradeToServer (csvInfo.TID);
		s.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(s);
	}

	static void SendTrooperUpgradeToServer(string tid){
		Debug.Log("SendTrooperUpgradeToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (tid);
		data.PutUtfString("soldierTid", tid); 
		SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_UpgradeTrooper, false, BuildHandle.HandleResponse);
	}

	static void SendTechUpgradeToServer(string tid){
		Debug.Log("SendTechUpgradeToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (tid);
		data.PutUtfString("spellTid", tid); 
		SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_UpgradeTech, false, BuildHandle.HandleResponse);
	}

	//对科技立即升级
	public static void OnImmediateResearch(string mTid_level){
		Debug.Log ("mTid_level:" + mTid_level);
		BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_LABORATORY");
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable[mTid_level] as CsvInfo;
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);		
		if (msg == null && buildInfo.status == BuildStatus.Normal){
			int gems = Helper.GetUpgradeInstant(csvInfo.TID_Level);
			if (DataManager.GetInstance().userInfo.diamond_count >= gems){
				buildInfo.status_tid_level = mTid_level;
				if (csvInfo.TID_Type == Helper.TID_TYPE_CHARACTERS) {
					SendTrooperUpgradeImmediatelyToServer (csvInfo.TID);
				} else {
					SendTechUpgradeImmediatelyToServer (csvInfo.TID);
				}
				//buildInfo.research_tid_level = mTid_level;
				//buildInfo.status = BuildStatus.Research;
				OnTechOrTrooperUpgradeDone (buildInfo);
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog();
			}
		}else{
			if (msg != null)
				UIManager.GetInstance().GetController<NormalMsgCtrl>().ShowPop(msg);
		}			
	}

	static void SendTrooperUpgradeImmediatelyToServer(string tid){
		Debug.Log("SendTripperUpgradeImmediatelyToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (tid);
		data.PutUtfString("tid", tid);
		data.PutInt ("type",1);
		SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_ImmediatelyUp, false, BuildHandle.HandleResponse);
	}

	static void SendTechUpgradeImmediatelyToServer(string tid){
		Debug.Log("SendTechUpgradeImmediatelyToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (tid);
		data.PutUtfString("tid", tid); 
		data.PutInt ("type",2);
		SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_ImmediatelyUp, false, BuildHandle.HandleResponse);
	}

	//加速升级科技
	public static void OnSpeedUpResearch(BuildInfo s){
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable[s.status_tid_level] as CsvInfo;
		int gems = CalcHelper.calcTimeToGems(s.end_time - Helper.current_time());
		s.status = BuildStatus.Normal;
		if (gems > 0) {
			if(DataManager.GetInstance().userInfo.diamond_count < gems){
				PopManage.Instance.ShowNeedGemsDialog ();
			}else {
				string title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP_RESEARCH");
				string msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_RESEARCH",new object[]{gems,StringFormat.FormatByTid(csvInfo.TID)});
				ISFSObject dt = new SFSObject ();
				UIManager.GetInstance().GetComponent<PopMsgCtrl>().ShowDialog(
					msg, 
					title, 
					gems.ToString(), 
					PopDialogBtnType.ImageBtn, 
					dt, 
					OnDialogSpeedUpResearchYes,
					true,
					s
				);
			}
		}
	}

	static void OnDialogSpeedUpResearchYes(ISFSObject dt,BuildInfo s=null){
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable[s.status_tid_level] as CsvInfo;
		int gems = CalcHelper.calcTimeToGems(s.end_time - Helper.current_time());
		Helper.SetResource (0,0,0,0,-gems);
		if (csvInfo.TID_Type == Helper.TID_TYPE_CHARACTERS) {
			SendTrooperUpgradeSpeedUpToServer (csvInfo.TID);
		} else {
			SendTechUpgradeSpeedUpToServer (csvInfo.TID);
		}
		OnTechOrTrooperUpgradeDone (s);
	}

	static void SendTechUpgradeSpeedUpToServer(string tid){
		Debug.Log("SendTechUpgradeSpeedUpToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (tid);
		data.PutUtfString("tid", tid);
		data.PutInt ("type",2);
		SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_SpeedUP, false,  BuildHandle.HandleResponse);
	}

	static void SendTrooperUpgradeSpeedUpToServer(string tid){
		Debug.Log("SendTrooperUpgradeSpeedUpToServer");
		ISFSObject data = new SFSObject();
		Debug.Log (tid);
		data.PutUtfString("tid", tid);
		data.PutInt ("type",1);
		SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_SpeedUP, false,  BuildHandle.HandleResponse);
	}

	//取消科技升级
	public static void OnCancelResearch(BuildInfo s){
		Debug.Log ("CancelTechUpgrade");
		if (s.status == BuildStatus.Research){
			CsvInfo csv = CSVManager.GetInstance().csvTable [s.status_tid_level] as CsvInfo;
			string title = StringFormat.FormatByTid("TID_POPUP_CANCEL_UPGRADE_TITLE");
			string msg = StringFormat.FormatByTid("TID_POPUP_CANCEL_UPGRADE",new object[]{StringFormat.FormatByTid(csv.TID), 50});
			UIManager.GetInstance().GetComponent<PopMsgCtrl>().ShowDialog(
				msg,
				title,
				"",
				PopDialogBtnType.ConfirmBtn,
				null,
				OnDialogCancelTechYes,
				true,
				s
			);
		}
	}

	static void OnDialogCancelTechYes(ISFSObject dt,BuildInfo s){
		int gold = Helper.getUpgradeCost (s.status_tid_level).gold / 2;
		int count = Helper.GetEmitCount (Helper.RES_TYPE_GOLD,gold);
		s.emitter.Emit (count,s.transform.position,PartType.Gold,gold);
		s.buildUI.RefreshBuildBtn ();
		s.status = BuildStatus.Normal;
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable[s.status_tid_level] as CsvInfo;
		if (csvInfo.TID_Type == Helper.TID_TYPE_CHARACTERS) {
			SendCancelSoilderToServer (csvInfo.TID);
		} else {
			SendCancelTechToServer (csvInfo.TID);
		}
	}

	static void SendCancelTechToServer(string tid){
		Debug.Log("SendCancelTechToServer");
		ISFSObject data = new SFSObject();
		data.PutUtfString("tid", tid);
		SFSNetworkManager.Instance.SendRequest(data,SFSNetworkManager.CMD_CancelTech, false, BuildHandle.HandleResponse);
	}

	static void SendCancelSoilderToServer(string tid){
		Debug.Log("SendCancelSoilderToServer");
		ISFSObject data = new SFSObject();
		data.PutUtfString("tid", tid);
		SFSNetworkManager.Instance.SendRequest(data,SFSNetworkManager.CMD_CancelSoilder, false, BuildHandle.HandleResponse);
	}

	static void OnTechOrTrooperUpgradeDone(BuildInfo s){
		s.status = BuildStatus.Normal;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)
		string research_tid_level = s.status_tid_level;
		CsvInfo csv = CSVManager.GetInstance().csvTable[research_tid_level] as CsvInfo;
		DataManager.GetInstance().researchLevel [csv.TID] = csv.Level + 1;
		Helper.EXPCollect (s,csv.XpGain);
		s.buildUI.RefreshBuildBtn ();
		AudioPlayer.Instance.PlaySfx("xp_gain_06");
	}

	static void SendTechDoneToServer(string tid){
		Debug.Log ("SendTechDoneToServer");
		ISFSObject data = new SFSObject ();
		data.PutUtfString ("tid",tid);
		SFSNetworkManager.Instance.SendRequest (data,SFSNetworkManager.CMD_TechUpgradeDone,false,BuildHandle.HandleResponse);
	}

	public static void TechUpdate(BuildInfo s){
		if(Helper.current_time() >= s.end_time){
			s.status = BuildStatus.Normal;

		}
	}

}
