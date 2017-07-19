using UnityEngine;
using System;
using Sfs2X.Entities.Data;
using System.Collections.Generic;


public struct BuildParam
{
	public ISFSObject item;
	public string tid;
	public int level;
	public string tid_level;
	public string cardName;
	public string tabPageInfo;
	public Models.BuildingData bbd;
}

namespace BoomBeach
{
	public class BuildManager : SingleMonoBehaviour<BuildManager>
	{
		public Transform buildContainer;

		protected override void Awake ()
		{
			base.Awake ();
			if (buildContainer == null)
				buildContainer = GameObject.Find ("PBuilds").transform;
		}

		public void InitBuildings (List<Network.BuildingModel> buildingList)
		{
			for (int i = 0; i < buildingList.Count; i++) {
				BuildParam param = new BuildParam ();
				param.tid = buildingList [i].type;
				param.level = buildingList [i].level;
				param.tid_level = param.tid + "_" + param.level;
				Debug.Log (param.tid_level);
				CreateBuild (param);
			}
		}

		//创建建筑物
		//创建完后，会缓存一份buildInfo到BuildList
		//item != null 时,是从服务器中同步下来的数据; item == null时，tid,level不能为空,是本地新建;
		public static GameObject CreateBuild (BuildParam param)
		{
			if (param.item != null) {//如果是服务器回传回来的数据，用服务器的tid和level
				param.tid = param.item.GetUtfString ("tid");
				param.level = param.item.GetInt ("level");
			}
			if (param.bbd != null) {
				param.tid = param.bbd.tid;
				param.level = (int)param.bbd.level;
			}
			string tid_level = param.tid + "_" + param.level;
			BuildInfo buildInfo = BuildInfo.loadFromBuildInfoCache (tid_level);
			BuildManager bbm = BuildManager.GetInstance;

			if (param.tid == "TID_BUILDING_LANDING_SHIP" || param.tid == "TID_BUILDING_GUNSHIP") {
				//return Create3DBuild(item, tid, level, cardName, tabPageInfo);
				if (buildInfo == null) {
					bbm.CreateBuildGo3D (out buildInfo, new BuildParam () {
						tid_level = tid_level,
						tid = param.tid
					});
				}
				bbm.InitBuilding3D (ref buildInfo, param);
			} else {
				//创建模型（新）
				if (buildInfo == null) {
					bbm.CreateBuildGo (out buildInfo, new BuildParam () {
						tid_level = tid_level,
						tid = param.tid
					});
				}
				//初始化建筑（新）
				bbm.InitBuilding (ref buildInfo, param);
			}
			buildInfo.transform.name = buildInfo.tid + "_" + buildInfo.level;
			return buildInfo.gameObject;
		}

		public GameObject CreateBuildGo3D (out BuildInfo buildInfo, BuildParam param)
		{
			CsvInfo csvData = CSVManager.GetInstance.csvTable [param.tid_level] as CsvInfo;
			string buildLayoutPath = string.Empty;
			string buildSpritePath = string.Empty;
			if (csvData.TID.Equals ("TID_BUILDING_LANDING_SHIP")) {
				buildLayoutPath = "Model/LayoutNew/LandCraftLayout";
			}
			if (csvData.TID.Equals ("TID_BUILDING_GUNSHIP")) {
				buildLayoutPath = "Model/LayoutNew/GunShipLayout";
			}
			buildSpritePath = "Model/Build3d/" + csvData.ExportName;
			/**
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("Build Name : " + cardName + " Tid Level : " + tid_level);
            sb.AppendLine("buildLayoutPath : " + buildLayoutPath);
            sb.AppendLine("buildSpritePath : " + buildSpritePath);
            **/
			GameObject buildLayoutInstance = Instantiate (ResourceCache.load (buildLayoutPath)) as GameObject;
			GameObject buildSpriteInstance = Instantiate (ResourceCache.load (buildSpritePath)) as GameObject;
			buildSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
			buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
			buildSpriteInstance.transform.localPosition = Vector3.zero;
			buildSpriteInstance.transform.name = "BuildMain";
			buildLayoutInstance.transform.parent = this.buildContainer;
			buildInfo = buildLayoutInstance.GetComponent<BuildInfo> ();
			buildInfo.buildSpritePath = buildSpritePath;
			buildInfo.is3D = true;
			buildInfo.tid = param.tid;
			buildInfo.level = csvData.Level;
			buildInfo.tid_level = param.tid_level;
			buildInfo.tid_type = csvData.TID_Type;
			buildInfo.csvInfo = csvData;
			return buildLayoutInstance;
		}

		public void InitBuilding3D (ref BuildInfo buildInfo, BuildParam param)
		{
			if (MoveOpEvent.Instance.SelectedBuildInfo != null) {
				MoveOpEvent.Instance.SelectedBuildInfo.ResetPosition ();
				MoveOpEvent.Instance.UnDrawBuildPlan (MoveOpEvent.Instance.SelectedBuildInfo);
				MoveOpEvent.Instance.SelectedBuildInfo.transform.Find ("UI/UIS").gameObject.SetActive (true);
			}
			MoveOpEvent.Instance.UnSelectBuild ();
			buildInfo.buildUIManage = buildInfo.GetComponent<BuildUIManage> ();
			buildInfo.buildUI = buildInfo.GetComponent<BuildUI> ();
			if (buildInfo.buildUI == null) {
				buildInfo.buildUI = buildInfo.GetComponentInChildren<BuildUI> ();
			}
			if (buildInfo.buildUIManage != null)
				buildInfo.buildUIManage.AfterInit ();
			if (buildInfo.buildUI != null)
				buildInfo.buildUI.AfterInit ();
			buildInfo.AfterInit ();
			bool isServerCreate = false;
			int status = 0;
			int artifact_type = 0;
			if (param.bbd != null) {
				isServerCreate = true;
				//status = (int)param.bbd.status;
				param.tid = param.bbd.tid;
				param.level = (int)param.bbd.level;
				artifact_type = (int)param.bbd.artifact_type;
			} else if (param.item != null) {
				//如果是服务器回传回来的数据，用服务器的tid和level
				isServerCreate = true;
				status = param.item.GetInt ("status");
				param.tid = param.item.GetUtfString ("tid");
				param.level = param.item.GetInt ("level");
				artifact_type = param.item.GetInt ("artifact_type");
			}


			//自动绑定数据;
			if (param.item != null) {
				Helper.ISFSObjectToBean (buildInfo, param.item);
				//Debug.Log (buildInfo.last_collect_time);
				buildInfo.last_collect_time = Helper.current_time (false) - buildInfo.last_collect_time;
				//buildInfo.last_collect_time1 = buildInfo.last_collect_time;
				//Debug.Log (buildInfo.last_collect_time);
			}
			if (isServerCreate) {

				Debug.Log (buildInfo.transform.name);
				buildInfo.status = (BuildStatus)status;
				buildInfo.artifact_type = (ArtifactType)artifact_type;
				//buildInfo.Position = Vector3.zero;
				InitBuildingByServerData (ref buildInfo, param.item.GetInt ("status"), param.tid + "_" + param.level, 0, param.item, null);
				//buildInfo.InitBuild();
				//buildInfo.buildUI.RefreshBuildBtn ();
				//PopManage.Instance.RefreshBuildBtn(buildInfo);
			} else {
				buildInfo.building_id = Helper.getNewBuildingId ();
				buildInfo.transform.name = "build_" + buildInfo.building_id;
				buildInfo.status = BuildStatus.Create;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;
				buildInfo.artifact_type = ArtifactType.None;
				//buildInfo.Position = Vector3.zero;
				if (buildInfo.buildUIManage != null)
					buildInfo.buildUIManage.ShowNewBox (true);
				if (buildInfo.buildUI != null)
					buildInfo.buildUI.ShowNewBox (true);
				MoveOpEvent.Instance.SelectBuild (buildInfo);
			}
			//新建的，也加入建筑物列表中，注：在取消新建时,需要移除;
			if (param.item != null)
				DataManager.GetInstance.buildList [buildInfo.building_id] = buildInfo;

			if (buildInfo.csvInfo.TID == "TID_BUILDING_LANDING_SHIP") {
				LandCraft lc = buildInfo.GetComponent<LandCraft> ();
				lc.Init ();
				lc.direct = Direct.UP;
				buildInfo.transform.position = Globals.landcraftPos [Globals.LandCrafts.Count] + Globals.dockPos;
				Globals.LandCrafts.Add (buildInfo);
			}
			if (buildInfo.csvInfo.TID == "TID_BUILDING_GUNSHIP") {
				buildInfo.transform.position = Globals.gunboatPos;
				GunBoat gb = buildInfo.GetComponent<GunBoat> ();
				gb.Init ();

			}
			buildInfo.gameObject.layer = 16;
			Transform[] t = buildInfo.transform.Find ("buildPos/BuildMain").GetComponentsInChildren<Transform> (true);
			for (int i = 0; i < t.Length; i++) {
				t [i].gameObject.layer = 16;
			}
			if (buildInfo.csvInfo.TID == "TID_BUILDING_LANDING_SHIP") {
				LandCraft landCraft = buildInfo.GetComponentInChildren<LandCraft> ();
				if (landCraft != null) {
					landCraft.InitTrooper (buildInfo.troops_tid, buildInfo.troops_num);
				}
			}
		}

		#region 创建建筑

		/// <summary>
		/// 创建建筑模型
		/// 1.创建建筑基座（3x3,4x4,5x5...）
		/// 2.创建建筑模型主体
		/// 3.创建建筑的sp
		/// 4.设置buildInfo的csv数据
		/// </summary>
		/// <param name="tid_level"></param>
		/// <param name="tid"></param>
		/// <returns></returns>
		public GameObject CreateBuildGo (out BuildInfo buildInfo, BuildParam param)
		{
			CsvInfo csvData = CSVManager.GetInstance.csvTable [param.tid_level] as CsvInfo;//该建筑level数据
			CsvInfo Lv1CsvData = CSVManager.GetInstance.csvTable [param.tid + "_1"] as CsvInfo;//该建筑1级时的数据
			//string buildLayoutPath = "Model/Layout/build" + csvData.Width;
			//string buildSpPath = "Model/Build/" + csvData.ExportName + "_sp";
			//string buildSpritePath = "Model/Build/" + csvData.ExportName;
			//string buildNewSpritePath = "Model/Build/buildnew" + csvData.Width;
			//1.读取建筑需要的四个基本模型
			GameObject buildLayoutInstance = CreateBuildLayoutInstance (csvData);//基座
			GameObject buildSpInstance = null;
			GameObject buildSpriteInstance = null;
			string buildSpritePath = "";
			GameObject buildNewSpriteInstance = CreateNewSpriteInstance (csvData);
			CreateBuildSpriteInstance (csvData, Lv1CsvData, ref buildSpriteInstance, ref buildSpInstance, ref buildSpritePath);
			//如果前面找不到配置的模型资源，则读取默认的模型代替。
			if (buildSpriteInstance == null) {
				CreateDefaultBuilding (ref buildLayoutInstance, ref buildSpriteInstance, ref buildSpInstance);
			}
			//2.组合四个基本资源
			AssembleBuilding (csvData, buildLayoutInstance, buildSpriteInstance, buildSpInstance, buildNewSpriteInstance);
			buildLayoutInstance.transform.parent = this.buildContainer;
			buildInfo = buildLayoutInstance.GetComponent<BuildInfo> ();
			buildInfo.buildSpritePath = buildSpritePath;
			buildInfo.tid = param.tid;
			buildInfo.level = csvData.Level;
			buildInfo.tid_level = param.tid_level;
			buildInfo.tid_type = csvData.TID_Type;
			buildInfo.csvInfo = csvData;

			/*GameObject uiPrefab = Resources.Load<GameObject> ("Model/BuildUI");
			Transform uiPos = buildInfo.transform.FindChild ("buildUIPos");*/
			return buildLayoutInstance;
		}
		//获取建筑基座模型
		GameObject CreateBuildLayoutInstance (CsvInfo csvData)
		{
			//string buildLayoutPath = "Model/Layout/build" + csvData.Width;

			string buildLayoutPath = "Model/LayoutNew/build" + csvData.Width;
			if (ResourceCache.load (buildLayoutPath) == null) {//如果没有合适的Layout,则用默认的build3
				buildLayoutPath = "Model/LayoutNew/build3";
			}
			return Instantiate (ResourceCache.load (buildLayoutPath)) as GameObject;
		}
		//获取建筑模型，建筑sp，建筑模型路径
		void CreateBuildSpriteInstance (CsvInfo csvData, CsvInfo Lv1CsvData, ref GameObject buildSpriteInstance, ref GameObject buildSpInstance, ref string buildSpritePath)
		{
			string buildSpPath = "Model/Build/" + csvData.ExportName + "_sp";
			buildSpritePath = "Model/Build/" + csvData.ExportName;
			//如果没有默认的Sprite，则用1级的sprite
			if (ResourceCache.load (buildSpritePath) == null) {
				buildSpritePath = "Model/Build/" + Lv1CsvData.ExportName;
				buildSpPath = "Model/Build/" + Lv1CsvData.ExportName + "_sp";
			}
			if (ResourceCache.load (buildSpPath) == null) {
				buildSpPath = "Model/Build/" + Lv1CsvData.ExportName + "_sp";
			}
			if (ResourceCache.load (buildSpritePath) != null)
				buildSpriteInstance = Instantiate (ResourceCache.load (buildSpritePath)) as GameObject;
			if (ResourceCache.load (buildSpPath) != null)
				buildSpInstance = Instantiate (ResourceCache.load (buildSpPath)) as GameObject;
		}
		//获取建筑升级模型
		GameObject CreateNewSpriteInstance (CsvInfo csvData)
		{
			string buildNewSpritePath = "Model/Build/buildnew" + csvData.Width;
			GameObject prefab = ResourceCache.load (buildNewSpritePath) as GameObject;
			if (prefab == null) {
				return null;
			}
			GameObject buildNewSpriteInstance = Instantiate (prefab) as GameObject;
			return buildNewSpriteInstance;
		}
		//获取默认模型组合
		void CreateDefaultBuilding (ref GameObject buildLayoutInstance, ref GameObject buildSpriteInstance, ref GameObject buildSpInstance)
		{
			string buildSpritePath = "Model/Build/housing_lvl1";
			string buildSpPath = "Model/Build/housing_lvl1_sp";
			string buildLayoutPath = "Model/LayoutNew/build3";
			buildLayoutInstance = Instantiate (ResourceCache.load (buildLayoutPath)) as GameObject;
			buildSpriteInstance = Instantiate (ResourceCache.load (buildSpritePath)) as GameObject;
			buildSpInstance = Instantiate (ResourceCache.load (buildSpPath)) as GameObject;
		}
		//组合四个基本模型
		void AssembleBuilding (CsvInfo csvData, GameObject buildLayoutInstance, GameObject buildSpriteInstance, GameObject buildSpInstance, GameObject buildNewSpriteInstance)
		{
			if (buildNewSpriteInstance != null) {
				buildNewSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
				buildNewSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
				buildNewSpriteInstance.transform.localPosition = Vector3.zero;
				buildNewSpriteInstance.transform.name = "BuildNew";
				buildNewSpriteInstance.gameObject.SetActive (false);//默认情况下会被禁用
			}
			buildSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
			buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
			buildSpriteInstance.transform.localPosition = Vector3.zero;
			buildSpriteInstance.transform.name = "BuildMain";
			buildSpInstance.transform.parent = buildLayoutInstance.transform;
			buildSpInstance.transform.name = "StandPoints";
			buildSpInstance.transform.localPosition = Vector3.zero;
			if (csvData.TID_Type == "OBSTACLES" || csvData.BuildingClass == "Artifact") {//如果时障碍物或者遗迹，则删掉Floor
				Transform floor = buildLayoutInstance.transform.Find ("Floor");
				if (floor != null)
					Destroy (floor.gameObject);
			}
		}

		#endregion

		#region 初始化建筑

		public void InitBuilding (ref BuildInfo buildInfo, BuildParam param)
		{
			bool isServerCreate = false;
			int status = 0;
			int artifact_type = 0;
			string tid_level = param.tid + "_" + param.level;
			if (param.bbd != null) {
				isServerCreate = true;
				//status = (int)bbd.status;
				param.tid = param.bbd.tid;
				param.level = (int)param.bbd.level;
				artifact_type = (int)param.bbd.artifact_type;
			} else if (param.item != null) {
				//如果是服务器回传回来的数据，用服务器的tid和level
				isServerCreate = true;
				status = param.item.GetInt ("status");
				param.tid = param.item.GetUtfString ("tid");
				param.level = param.item.GetInt ("level");
				artifact_type = param.item.GetInt ("artifact_type");
			}
			buildInfo.buildUIManage = buildInfo.GetComponent<BuildUIManage> ();
			buildInfo.buildUI = buildInfo.GetComponent<BuildUI> ();
			//初始化建筑UI
			if (buildInfo.buildUIManage != null)
				buildInfo.buildUIManage.AfterInit ();
			if (buildInfo.buildUI != null)
				buildInfo.buildUI.AfterInit ();
			//初始化当前建筑的遮挡点和播放建筑Tweener动画
			buildInfo.AfterInit ();
			//清除原来的选中
			if (MoveOpEvent.Instance.SelectedBuildInfo != null) {//UI选中相关
				MoveOpEvent.Instance.SelectedBuildInfo.ResetPosition ();
				MoveOpEvent.Instance.UnDrawBuildPlan (MoveOpEvent.Instance.SelectedBuildInfo);
				MoveOpEvent.Instance.SelectedBuildInfo.transform.Find ("UI/UIS").gameObject.SetActive (true);
			}
			MoveOpEvent.Instance.UnSelectBuild ();
			if (isServerCreate) {//与服务器创建相关
				InitBuildingByServerData (ref buildInfo, status, tid_level, artifact_type, param.item, param.bbd);
			} else {
				InitBuildingTemp (ref buildInfo, tid_level);
			}
			if (buildInfo.csvInfo.BuildingClass == "Artifact") {
				Artifact artifact = buildInfo.transform.GetComponentInChildren<Artifact> ();
				if (artifact != null) {
					artifact.setStatus (buildInfo.artifact_type);
					artifact.buildInfo = buildInfo;
				}
			}

			if (buildInfo.status == global::BuildStatus.Upgrade) {//如果处于正在升级状态,则显示buildin
				if (buildInfo.transform.Find ("buildin") != null)
					buildInfo.transform.Find ("buildin").gameObject.SetActive (true);
			}

			BuildRing buildRing = buildInfo.GetComponentInChildren<BuildRing> ();
			if (buildRing != null) {
				if (buildInfo.csvInfo.TID_Type == "TRAPS") {
					buildRing.OuterRange = buildInfo.csvInfo.TriggerRadius / 100f;
					buildRing.InnerRange = 0f;
				} else {
					buildRing.OuterRange = buildInfo.csvInfo.AttackRange / 100f;
					buildRing.InnerRange = buildInfo.csvInfo.MinAttackRange / 100f;
				}
			}

			//新建的，也加入建筑物列表中，注：在取消新建时,需要移除;
			if (param.item != null)
				DataManager.GetInstance.buildList [buildInfo.building_id] = buildInfo;
			if (buildInfo.csvInfo.TID_Type != "OBSTACLES")
				DataManager.GetInstance.buildArray.Add (buildInfo);

			if (buildInfo.csvInfo.TID_Type != "TRAPS") {
				if (buildInfo.buildUIManage != null) {
					buildInfo.buildUIManage.buildBattleInfo.Health = buildInfo.csvInfo.Hitpoints;
					buildInfo.buildUIManage.buildBattleInfo.HealthAdd = Helper.getArtifactBoost (buildInfo.csvInfo.Hitpoints, ArtifactType.BoostBuildingHP);
					buildInfo.buildUIManage.buildBattleInfo.Dps = buildInfo.csvInfo.Damage;
					buildInfo.buildUIManage.buildBattleInfo.DpsAdd = Helper.getArtifactBoost (buildInfo.csvInfo.Damage, ArtifactType.BoostBuildingDamage);
				}
				if (buildInfo.buildUI != null) {
					buildInfo.buildUI.buildBattleInfo.Health = buildInfo.csvInfo.Hitpoints;
					buildInfo.buildUI.buildBattleInfo.HealthAdd = Helper.getArtifactBoost (buildInfo.csvInfo.Hitpoints, ArtifactType.BoostBuildingHP);
					buildInfo.buildUI.buildBattleInfo.Dps = buildInfo.csvInfo.Damage;
					buildInfo.buildUI.buildBattleInfo.DpsAdd = Helper.getArtifactBoost (buildInfo.csvInfo.Damage, ArtifactType.BoostBuildingDamage);
				}
			}
		}

		//根据服务端返回的数据真实创建
		void InitBuildingByServerData (ref BuildInfo buildInfo, int status, string tid_level, int artifact_type, Sfs2X.Entities.Data.ISFSObject item = null, Models.BuildingData bbd = null)
		{
			//如果当前建筑处于正在新建状态(New)
			if (status == 1) {
				Transform buildNewT = buildInfo.transform.Find ("buildPos/BuildNew");
				if (buildNewT != null) {
					buildNewT.gameObject.SetActive (true);
					buildInfo.transform.Find ("buildPos/BuildMain").gameObject.SetActive (false);
				}
			}

			if (item != null) {
				Helper.ISFSObjectToBean (buildInfo, item);
				//Debug.Log (buildInfo.last_collect_time);
				//Debug.Log("item.GetInt(status):" + item.GetInt("status"));
				buildInfo.status = (BuildStatus)item.GetInt ("status");
				buildInfo.last_collect_time = Helper.current_time (false) - buildInfo.last_collect_time;


				if (buildInfo.status == BuildStatus.New) {
					buildInfo.total_time = Helper.GetBuildTime (buildInfo.tid + "_" + buildInfo.level);
					buildInfo.start_time = Helper.current_time (false) - (buildInfo.total_time - buildInfo.rest_time);
					buildInfo.end_time = Helper.current_time (false) + buildInfo.rest_time;
					Debug.Log (buildInfo.total_time);
				} else if (buildInfo.status == BuildStatus.Upgrade) {
					buildInfo.total_time = Helper.GetBuildTime (buildInfo.tid + "_" + (buildInfo.level + 1));
					buildInfo.start_time = Helper.current_time (false) - (buildInfo.total_time - buildInfo.rest_time);
					buildInfo.end_time = Helper.current_time (false) + buildInfo.rest_time;
				} else if (buildInfo.status == BuildStatus.Research) {
					buildInfo.total_time = Helper.GetBuildTime (buildInfo.status_tid_level);
					buildInfo.start_time = Helper.current_time (false) - (buildInfo.total_time - buildInfo.rest_time);
					buildInfo.end_time = Helper.current_time (false) + buildInfo.rest_time;
				} else if (buildInfo.status == BuildStatus.Train) {
					buildInfo.status_tid_level = item.GetUtfString ("troops_tid") + "_" + DataManager.GetInstance.researchLevel [item.GetUtfString ("troops_tid")];
					buildInfo.total_time = Helper.GetBuildTime (buildInfo.status_tid_level);
					buildInfo.start_time = Helper.current_time (false) - (buildInfo.total_time - buildInfo.rest_time);
					buildInfo.end_time = Helper.current_time (false) + buildInfo.rest_time;
				}

				//buildInfo.last_collect_time1 = buildInfo.last_collect_time;
				//Debug.Log (buildInfo.last_collect_time);
			}
			if (bbd != null) {
				buildInfo.x = (int)bbd.x;
				buildInfo.y = (int)bbd.y;
				buildInfo.building_id = bbd.building_id;
			}
			#region 
			//新
			string name = "build_" + (CSVManager.GetInstance.csvTable [tid_level] as CsvInfo).Width + "__" + GetFormatStringByTime (buildInfo.building_id);//设置名字
			buildInfo.transform.name = name;
			//原
			//buildInfo.transform.name = "build_" + buildInfo.building_id;//当前建筑building_id
			#endregion
			buildInfo.status = (global::BuildStatus)status;//当前建筑Build状态
			buildInfo.artifact_type = (ArtifactType)artifact_type;//遗迹神像类型
			buildInfo.Position = new Vector3 (buildInfo.x, 0f, buildInfo.y);//所有建筑都在y=0这个平面上
			buildInfo.InitBuild ();
			buildInfo.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(buildInfo);
		}

		//显示临时的提示信息等
		//真正的创建是在点击了提示框的确认按钮过后
		void InitBuildingTemp (ref BuildInfo buildInfo, string tid_level)
		{
			buildInfo.building_id = Helper.getNewBuildingId ();
			string name = "build_" + (CSVManager.GetInstance.csvTable [tid_level] as CsvInfo).Width + "__" + GetFormatStringByTime (buildInfo.building_id);
			buildInfo.transform.name = name;
			//buildInfo.transform.name = "build_"+buildInfo.building_id;
			buildInfo.status = global::BuildStatus.Create;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;
			buildInfo.artifact_type = ArtifactType.None;
			if (buildInfo.csvInfo.BuildingClass == "Artifact") {
				BuildInfo s = Helper.getBuildInfoByTid ("TID_BUILDING_ARTIFACT_WORKSHOP");
				buildInfo.artifact_boost = s.artifact_boost;
				buildInfo.artifact_type = s.artifact_type;
			}
			buildInfo.Position = Helper.getBlankXY (buildInfo.GridCount);
			buildInfo.buildUIManage = buildInfo.GetComponent<BuildUIManage> ();
			buildInfo.buildUI = buildInfo.GetComponent<BuildUI> ();
			if (buildInfo.buildUIManage != null)
				buildInfo.buildUIManage.ShowNewBox (true);
			if (buildInfo.buildUI != null)
				buildInfo.buildUI.ShowNewBox (true);
			//设置当前选中的build
			MoveOpEvent.Instance.SelectBuild (buildInfo);
			MoveOpEvent.Instance.DrawBuildPlan (buildInfo);
			buildInfo.isMoving = true;
			MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid> ().drawLine ();
		}

		public string GetFormatStringByTime (long millseconds)
		{
			System.DateTime dateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			dateTime = dateTime.AddMilliseconds (millseconds);
			dateTime = DateTime.SpecifyKind (dateTime, DateTimeKind.Local);

			return dateTime.ToString ("yyyy-MM-dd-HH-mm-ss-ff");
		}

		#endregion
	}
}
