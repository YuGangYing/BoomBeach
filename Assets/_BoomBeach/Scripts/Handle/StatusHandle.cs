using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

namespace BoomBeach{
	public static class StatusHandle  {
		//创建神像
		public static void OnCreateStatue(BuildInfo s, string status_tid_level){
			if(s.status != BuildStatus.Normal){
				return;
			}
			CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
			int artifact_num = Helper.GetArtifactNum();
			BuildCost bc = Helper.GetBuildCost(status_tid_level);
			string msg = null;
			if (artifact_num > s.csvInfo.ArtifactCapacity) {
				msg = StringFormat.FormatByTid ("TID_SELL_ARTIFACTS");//"容量已满";
			} else {
				if (!Helper.CheckResourceCount(bc.piece_type, -bc.piece)){
					msg = StringFormat.FormatByTid("TID_NOT_ENOUGH_ARTIFACT_PIECES", new object[]{StringFormat.FormatByTid(Helper.GetNameToTid(bc.piece_type))});//"资源不足";
				}
			}
			if(msg != null){
				UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
				return;
			}
			Helper.SetResourceCount (bc.piece_type, -bc.piece, false, true);
			s.start_time = Helper.current_time();
			s.end_time = s.start_time + Helper.GetUpgradeTime(status_tid_level);		
			s.last_collect_time = s.end_time;
			s.status = BuildStatus.CreateStatue;
			s.status_tid_level = status_tid_level;
			s.artifact_boost = 0;
			s.artifact_type = ArtifactType.None;
			s.buildUI.RefreshBuildBtn ();
		}

		static void SendCreateStatusToServer(BuildInfo s){
			ISFSObject data = new SFSObject();
			Debug.Log (s.building_id);
			data.PutLong("id", s.building_id);
			//data.PutInt ("type",0);
			//SFSNetworkManager.Instance.SendRequest(data, SFSNetworkManager.CMD_SpeedUP, false, BuildHandle.HandleResponse);
		}

		//取消创建神像
		public static void OnCancelCreateStatus(BuildInfo s){
			string status_tid_level = s.status_tid_level;
			CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
			int artifact_num = Helper.GetArtifactNum();
			BuildCost bc = Helper.GetBuildCost(status_tid_level);
			Helper.SetResourceCount (bc.piece_type, bc.piece, false, true);
			s.status = BuildStatus.Normal;
			s.buildUI.RefreshBuildBtn ();
			SendCancelCreateStatusToServer (s);
		}

		static void SendCancelCreateStatusToServer(BuildInfo s){
		
		}

		//判断，当前建筑物上，是否有存在，还未 部署 神像;
		static bool HasStatue(BuildInfo s){
			if (s.status == BuildStatus.Normal && s.tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && s.status_tid_level != "" && s.status_tid_level != null){
				CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[s.status_tid_level] as CsvInfo;
				return csvArtiact.BuildingClass == "Artifact";
			}else{
				return false;
			}
		}

		//部署 一个新神像;
		public static void OnDeployStatue(BuildInfo s){
			if (HasStatue(s)){
				int artifact_num = Helper.GetArtifactNum();
				if (artifact_num < s.csvInfo.ArtifactCapacity){
					CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[s.status_tid_level] as CsvInfo;
					string tid_artifact = "TID_BUILDING_ARTIFACT3";
					if (csvArtiact.ArtifactType == 1){
						tid_artifact = "TID_BUILDING_ARTIFACT3";
					}else if (csvArtiact.ArtifactType == 2){
						tid_artifact = "TID_BUILDING_ARTIFACT3_ICE";
					}else if (csvArtiact.ArtifactType == 3){
						tid_artifact = "TID_BUILDING_ARTIFACT3_FIRE";
					}else if (csvArtiact.ArtifactType == 4){
						tid_artifact = "TID_BUILDING_ARTIFACT3_DARK";
					}
					BuildInfo epic_buildInfo = null;
					if ("TID_BUILDING_ARTIFACT3".Equals(csvArtiact.TID) || "TID_BUILDING_ARTIFACT3_ICE".Equals(csvArtiact.TID) || "TID_BUILDING_ARTIFACT3_FIRE".Equals(csvArtiact.TID) || "TID_BUILDING_ARTIFACT3_DARK".Equals(csvArtiact.TID)){
						epic_buildInfo = Helper.getBuildInfoByTid(tid_artifact);
					}
					if (epic_buildInfo == null){
						if (s.artifact_type == ArtifactType.None){
							//TID_ARTIFACT_WAIT_SERVER = 等待服务器返回雕像类型!;
							PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_ARTIFACT_WAIT_SERVER"));
						}else{
							BuildManager.CreateBuild(new BuildParam()
								{
									tid = csvArtiact.TID,
									level = csvArtiact.Level
								});
							//当 部署 成功后，需要清空 TID_BUILDING_ARTIFACT_WORKSHOP 上的 status_tid_level
						}
					}else{
						//TID_SAME_EPIC_ARTIFACT = 同一奖励类型的大雕像，你只能拥有一个！要么你就回收旧的提供<bonus>奖励的大雕像，要么回收新的大雕像.;
						PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_SAME_EPIC_ARTIFACT", new object[]{epic_buildInfo.ShowLevelName}));
					}
				}else{
					//TID_SELL_ARTIFACTS = Statue limit reached! Upgrade Sculptor to deploy more, or reclaim a statue.
					PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_SELL_ARTIFACTS"));
				}
			}
		}

		public static void StatusUpdate(BuildInfo s){
			if(Helper.current_time() >= s.end_time){
				s.status = BuildStatus.Normal;
			}
		}


	}
}