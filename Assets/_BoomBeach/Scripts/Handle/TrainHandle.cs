using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using BoomBeach;

public static class TrainHandle {

	//训练
	public static void OnTrain(BuildInfo s,string troops_tid){
		Dictionary<string,TrainTid> TrainTidList = Helper.getTrainTidList(s,troops_tid);
		TrainTid trainTid = TrainTidList[troops_tid];
		if (trainTid != null){
			ISFSObject dt = null;
			dt = Helper.getCostDiffToGems(trainTid.tid_level,2,true, trainTid.trainCost * trainTid.trainNum);
			dt.PutUtfString("train_tid",trainTid.tid_level);
			dt.PutInt ("train_time",trainTid.trainTime);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog (
					dt.GetUtfString ("msg"),
					dt.GetUtfString ("title"),
					gems.ToString (),
					PopDialogBtnType.ImageBtn,
					dt,
					OnDialogTrainYes,true,s);
			}else{
				Train(s,dt.GetUtfString("train_tid"),dt.GetInt("train_time"),dt.GetInt("Gold"),dt.GetInt("Gem"));
			}
		}
	}

	static void OnDialogTrainYes(ISFSObject dt,BuildInfo s = null){
		if (DataManager.GetInstance.userInfo.diamond_count >= dt.GetInt("Gems")){
			Train (s,dt.GetUtfString("train_tid"),dt.GetInt("train_time"),dt.GetInt("Gold"),dt.GetInt("Gem"));
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	static void Train(BuildInfo s,string train_tid,int train_time,int gold,int gems){
		if (s.status == BuildStatus.Normal) {
			//扣除用户本地资源;
			if (gold >= 0) {
				Helper.SetResource (-gold, 0, 0, 0, -gems, true);
			} else {
				//负数时，退还差价;
				int collect_num = -gold;
				if (collect_num > 0) {
					int max_num = DataManager.GetInstance.userInfo.max_gold_count;
					int c = 0;
					if (max_num > 0) {
						c = Mathf.CeilToInt (collect_num * 100f / max_num);
					}
					if (c > 15)
						c = 15;
					int addC = Mathf.RoundToInt (collect_num / 500f);
					if (addC > 15)
						addC = 15;
					c += addC;
					s.emitter.enabled = true;
					s.emitter.Emit (c, s.transform.position, PartType.Gold, collect_num);
					//重新设置，所有建筑物的升级提醒标识;
					Helper.SetBuildUpgradeIcon();
				}
			}
			CsvInfo csvInfo = CSVManager.GetInstance.csvTable[train_tid] as CsvInfo;
			s.start_time = Helper.current_time();
			s.troops_start_time = s.start_time;
			s.end_time = s.start_time + train_time;		
			s.last_collect_time = s.end_time;//要到新建结束后，才能采集;

			s.status = BuildStatus.Train;
			s.status_tid_level = csvInfo.TID_Level;
			if (s.troops_tid != csvInfo.TID){
				//清空军队里面的，旧的数量;
				s.troops_num = 0;
				s.GetComponent<LandCraft>().InitTrooper(s.troops_tid,s.troops_num);
			}
			s.troops_tid = csvInfo.TID;
			s.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(s);
			SendTrainToServer(s.building_id,csvInfo.TID);
		}
	}

	//加速完成训练
	public static void OnSpeedUpTrain(BuildInfo s){
		if (s.status == BuildStatus.Train) {
			int current_time = Helper.current_time ();
			int gems = CalcHelper.calcTimeToGems (s.end_time - current_time);
			if (gems > 0) {
				string title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP_ALL_TROOP_TRAIN");
				string msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_ALL_TROOP_TRAINING",new object[]{gems});
				ISFSObject dt = new SFSObject ();
				UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog (
					msg,
					title,
					gems.ToString (),
					PopDialogBtnType.ImageBtn,
					dt,
					OnSpeedUpTrainDialogYes,true,s);
			}
		}
	}

	static void OnSpeedUpTrainDialogYes(ISFSObject dt,BuildInfo s){
		if (s.status == BuildStatus.Train) {
			int current_time = Helper.current_time ();
			int gems = CalcHelper.calcTimeToGems (s.end_time - current_time);
			if (DataManager.GetInstance.userInfo.diamond_count >= gems){
				Helper.SetResource (0, 0, 0, 0, -gems, true);
				SendTrainSpeedUpToServer (s);
				TrainTroops(s,s.end_time);//时间决定数量
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog();
			}
			s.buildUI.RefreshBuildBtn ();
		}
	}

	//取消训练
	public static void OnCancelTrain(BuildInfo s){
		int train_time = Helper.GetBuildTime(s.status_tid_level);
		int current_time = Helper.current_time ();
		int troops_num = Mathf.FloorToInt((current_time - s.troops_start_time) / train_time);//计算已经训练好的士兵数
		int remain_troops_num = Mathf.FloorToInt ((s.end_time - s.troops_start_time) / train_time)-troops_num ;//计算还需要训练的士兵数
		if(remain_troops_num>0){
			int gold = Helper.GetBuildCost (s.status_tid_level).gold * remain_troops_num;
			int c = Helper.GetEmitCount (Helper.RES_TYPE_GOLD,gold);
			s.emitter.enabled = true;
			s.emitter.Emit(c,s.transform.position,PartType.Gold,gold);
		}
		s.status = BuildStatus.Normal;
		s.buildUI.RefreshBuildBtn ();
		SendCancelTrainToServer (s);
	}

	public static void SendTrainToServer(long building_id,string tid){
		Debug.Log("SendTrainToServer" + ":" + "building_id:" + building_id + ",tid:" +tid);
		ISFSObject data = new SFSObject();
		data.PutLong("building_id", building_id);
		data.PutUtfString ("createSoldierTid",tid);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_Train, false, BuildHandle.HandleResponse);
	}

	public static void SendTrainAllToServer(){
		Debug.Log("SendTrainAllToServer");
		ISFSObject data = new SFSObject();
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_TrainAll, false, BuildHandle.HandleResponse);
	}

	public static void SendTrainSpeedUpToServer(BuildInfo s){
		Debug.Log("SendTrainSpeedUpToServer");
		ISFSObject data = new SFSObject();
		data.PutLong("id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_SpeedUPTrain, false, BuildHandle.HandleResponse);
		//SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_SpeedUPTrain, false, HandleResponse);                                                                        nse);
	}

	public static void SendTrainSpeedUpAllToServer(){
		Debug.Log ("SendTrainSpeedUpAllToServer");
		ISFSObject data = new SFSObject();
		SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_SpeedUPTrainAll, false, BuildHandle.HandleResponse);
	}

	public static void SendCancelTrainToServer(BuildInfo s){
		Debug.Log("SendCancelTrainToServer");
		ISFSObject data = new SFSObject();
		data.PutLong("building_id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_CancelTraining, false, BuildHandle.HandleResponse);
	}

	//返回军队生产数量;
	//每生产一个兵，troops_start_time = troops_start_time + train_time 直至 troops_start_time = end_time;
	public static int TrainTroops(BuildInfo s, int current_time){
		Debug.Log ("TrainTroops");
		int troops_num = 0;
		if (s.status == BuildStatus.Train){
			int train_time = Helper.GetBuildTime(s.status_tid_level);			
			if (current_time > s.end_time)
				current_time = s.end_time;
			//通过时间计算出生产成功的数量;
			troops_num = Mathf.FloorToInt((current_time - s.troops_start_time) / train_time);
			if (troops_num > 0){
				int max_space = s.csvInfo.HousingSpace;
				CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[s.status_tid_level];
				int max_troops_num = Mathf.FloorToInt(max_space / csvData.HousingSpace);
				//不能大于最大容量;
				if (s.troops_num + troops_num > max_troops_num){
					troops_num = max_troops_num - s.troops_num;
				}
				s.troops_num += troops_num;
				s.troops_start_time = s.troops_start_time + train_time * troops_num;
				if (s.troops_start_time >= s.end_time){
					//所有兵都训练完成;
					s.troops_start_time = s.end_time;
					s.status = BuildStatus.Normal;
					//刷新按钮状态;
					s.buildUI.RefreshBuildBtn ();
				}
				LandCraft landCraft = s.transform.GetComponentInChildren<LandCraft>();
				if(landCraft!=null)
					landCraft.InitTrooper(s.troops_tid,s.troops_num);
			}
		}
		return troops_num;
	}

	public static void TrainUpdate(BuildInfo s){
		TrainTroops (s,Helper.current_time());
		if(Helper.current_time() >= s.end_time){
			s.status = BuildStatus.Normal;
		}
	}
}
