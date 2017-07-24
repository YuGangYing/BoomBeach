using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

using System;
using System.Collections.Generic;
using BoomBeach;

//Status -1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像; 7:移动障碍飘粒子中;	
public enum BuildStatus{
	Create=-1,
	Normal=0,
	New=1,
	Upgrade=2,
	Removal=3,
	Research=4,
	Train=5,
	CreateStatue=6,
	Removaling = 7
}

public class BuildInfo : MonoBehaviour {

	private Transform mTrans;

	private GameObject crystlePartObj;

	public bool is3D = false;
	public string Id{
		set{
			mTrans.name = value;
		}
		get{
			return mTrans.name;
		}
	}

	public string ShowName{
		get{
			return	StringFormat.FormatByTid(tid);
		}
	}

	public string ShowLevelName{
		get{
			if (csvInfo.BuildingClass != "Artifact"){
				if (csvInfo.TID_Type == "OBSTACLES"){
					return null;
				}else{
					return StringFormat.FormatByTid("TID_LEVEL_NUM",new string[]{level.ToString()});
				}
			}else{
				string InfoTID = "";
				if (artifact_type == ArtifactType.BoostGold){
					InfoTID = "TID_BOOST_GOLD";
				}else if (artifact_type == ArtifactType.BoostWood){
					InfoTID = "TID_BOOST_WOOD";
				}else if (artifact_type == ArtifactType.BoostStone){
					InfoTID = "TID_BOOST_STONE";
				}else if (artifact_type == ArtifactType.BoostMetal){
					InfoTID = "TID_BOOST_METAL";
				}else if (artifact_type == ArtifactType.BoostTroopHP){
					InfoTID = "TID_BOOST_TROOP_HP";
				}else if (artifact_type == ArtifactType.BoostBuildingHP){
					InfoTID = "TID_BOOST_BUILDING_HP";
				}else if (artifact_type == ArtifactType.BoostTroopDamage){
					InfoTID = "TID_BOOST_TROOP_DAMAGE";
				}else if (artifact_type == ArtifactType.BoostBuildingDamage){
					InfoTID = "TID_BOOST_BUILDING_DAMAGE";
				}else if (artifact_type == ArtifactType.BoostGunshipEnergy){
					InfoTID = "TID_BOOST_GUNSHIP_ENERGY";
				}else if (artifact_type == ArtifactType.BoostLoot){
					InfoTID = "TID_BOOST_LOOT";
				}else if (artifact_type == ArtifactType.BoostArtifactDrop){
					InfoTID = "TID_BOOST_ARTIFACT_DROP";
				}else if (artifact_type == ArtifactType.BoostAllResources){
					InfoTID = "TID_BOOST_ALL";
				}
				return StringFormat.FormatByTid(InfoTID,new string[]{artifact_boost.ToString()});
			}
		}
	}

	//表示为当前建筑所建筑的位置;
	private Vector3 location; 
	public Vector3 Location{
		get{return location; }
	}  
	public Vector3 Position   //表示当前建筑的模型实际所在的位置;
	{
		get{ return mTrans.position; }
		set{ mTrans.position = value;}
	}
	public int GridCount;
	
	public bool isSelected = false;
	public bool isMoving = false;
	public bool isGrap = false;  //是否被抓住，用于移动;
	
	public Transform[] buildStandPoints;

	public BuildMark[] buildMarks;

	public List<CharInfo> behindCharacters;  //被遮挡的角色;
	public List<BuildInfo> behindBuilds;	 //被遮挡的建筑;
	public List<BuildInfo> frontBuilds;

	public CsvInfo csvInfo;

	public string buildSpritePath;
	
	//==========================以下属性是从服务器端同步下来的==========================
	/*唯一定位id,创建一个物品时,通过当前时间点生成System.DateTime.Now.ToString("yyyyMMddHHmmssff");*/
	public long building_id;
	public string tid; //物品TID
	public int level = 0; //当前等级	level	

	public int preX;
	public int preY;

	public int x;//当前坐标x;
	public int y;//当前坐标y;
	
	public int is_destroy; //是否被销毁:0否;1是(生命值为0时,被销毁) is_destroy

	public int last_collect_time;//距离上次采集时间(金矿山,凿石场,木屋,钢材厂,采集船) last_collect_time; 
	public BuildStatus status; //-1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;	
	//TODO
	public int start_time; /*status状态对应的 开始时间;*/
	public int end_time; /*status状态对应的 结束时间;*/

	public int total_time;//升级需要时间,本地读取
	public int rest_time;//升级剩下时间,服务器或者本地

	public int troops_start_time;//最后一个兵的生产时间，默认值为：start_time，以后每生产一个兵，向前加一个生产该兵的时间;

	public string status_tid_level;//status状态(4,5,6)对应的 物品tid_level;

	public string artifact_tid;

	public string troops_tid;//tid=TID_BUILDING_GUNSHIP为已生产兵种;
	public int troops_num;//tid=TID_BUILDING_GUNSHIP为已生产数量;

	public ArtifactType artifact_type;//神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage
	public int artifact_boost;//神像提升其它属性%值;

	//当前建筑物存储的资源容量;
	public int gold_stored;
	public int wood_stored;
	public int stone_stored;
	public int metal_stored;

	//public ArrayList troopsList;//当tid为TID_BUILDING_BARRACK,TID_BUILDING_SPELL_FORGE时有效(生产兵法术列表)
	//======================================================================================
	public bool isAfterResetPos;


	//正在发送到服务器端，还未返回结果回来;如：创建时,通知了服务器新建一个,但通知后还没有返回到客服端时,不能发送：立即结束 按钮; 升级时也一样；主要处理这2个问题;
	public bool send_ing = false;

	public bool isWaitForReturnBuildId = false;

	//当前建筑物被分配到的，存储量;
	/*
	public int gold_count = 0;
	public int wood_count = 0;
	public int stone_count = 0;
	public int metal_count = 0;
	*/

	//============以下属性是从csv中获得的======================
	public string tid_level;
	public string tid_type;//buildings,characters,decos,obstacles,spells,traps,artifacts,artifact_bonuses
	//public string building_class;//物品类型,从csv中同步过来;

	/*
	public int damage;//破坏力;
	public int damage_radius;//破坏半经;
	//public int air_targets;//空间目标;1:是;0:否;
	public int ground_targets;//地面目标;1:是;0:否;
	
	public int hitpoints;//生命值;
	public int cur_hitpoints;//当前生命值;
	
	public int attack_range;//攻击范围;
	public int min_attack_range;//最小攻击范围;
	public int attack_speed;//攻击速度;
	*/
	//==========================================================



	public BuildUIManage buildUIManage;

    public BuildUI buildUI;

    public PartEmitter emitter;
	void Awake()
	{
		mTrans = transform;
		emitter = mTrans.Find ("parts").GetComponent<PartEmitter> ();
		if(emitter!=null)
		emitter.buildInfo = this;
	}

	void Start()
	{
		
	}

	void Update(){
		if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME) {
			if (status == BuildStatus.Upgrade || status == BuildStatus.New) {
				BuildHandle.BuildUpdate (this);
			}
			if(status == BuildStatus.Train){
				TrainHandle.TrainUpdate (this);
			}
			if(status == BuildStatus.Research){
				ResearchHandle.TechUpdate (this);
			}
		}
	}

	//临时变量，移除费用;
	private int removal_gold = 0;
	private int removal_gems = 0;
	public void InitStandPoint()
	{
		if(!is3D)
		{
			int StandPointLength = GridCount * 4 - 4;
			buildStandPoints = new Transform[StandPointLength];
			Transform standPoints = transform.Find ("StandPoints");
			for(int i=1;i<=StandPointLength;i++)
			{
				buildStandPoints[i-1] = standPoints.Find("p"+i.ToString());
			}
			Transform standPoints2 = null;
			if(status==BuildStatus.New||status==BuildStatus.Upgrade)
			{
				standPoints2 = transform.Find ("buildSp");
			}
			if(standPoints2!=null)
			{
				standPoints = standPoints2;
			}
			if(standPoints!=null)
				foreach(Transform standPoint in standPoints)
			{	
				int a = (int)standPoint.position.x;
				int b = (int)standPoint.position.z;
				GridInfo grid = Globals.GridArray[a,b];
				grid.standPoint = standPoint.position;
			}
		}
	}

	public void AfterInit()
	{
		//初始化当前建筑的遮挡点;
		buildMarks = GetComponentsInChildren<BuildMark> ();
		if(buildMarks!=null)
		{
			for(int i=0;i<buildMarks.Length;i++)
			{
				buildMarks[i].buildInfo = this;
			}
		}
		BuildTweener bt = GetComponentInChildren<BuildTweener> ();
		if (bt != null)
						bt.buildInfo = this;

	}

	/* 绘制站立点边缘;
	void Update()
	{
		for(int i=0;i<buildStandPoints.Length;i++)
		{
			Vector3 bp = buildStandPoints[i].position;
			Vector3 ep = Vector3.zero;
			if(i==buildStandPoints.Length-1)
				ep = buildStandPoints[0].position;
			else
				ep = buildStandPoints[i+1].position;

			Debug.DrawLine(bp,ep,Color.black);
		}
	}*/




	//初始化建筑;
	public void InitBuild()
	{
		behindCharacters = new List<CharInfo>();  //被遮挡的角色;
		behindBuilds = new List<BuildInfo>();	 //被遮挡的建筑;
		frontBuilds = new List<BuildInfo>();
		SetBuild(false,false);
	}

	//在移除时，需要清空占用格子;
	public void ClearGrid(){
		//清空原坐标的格子状态;
		int beginA = (int)location.x;
		int beginB = (int)location.z;
		for (int i=0; i<GridCount; i++) 
		{
			for (int j=0; j<GridCount; j++) 
			{
				int gridInfoA = i+beginA;
				int gridInfoB = j+beginB;
				GridInfo gridInfo = Globals.GridArray[gridInfoA,gridInfoB];
				gridInfo.buildInfo = null;
				gridInfo.cost = Globals.GridEmptyCost;
				gridInfo.isBuild = false;
				gridInfo.standPoint = new Vector3((float)gridInfoA+0.5f,0f,(float)gridInfoB+0.5f);
			}
		}
		//将原遮挡建筑还原透明度;
		for(int i=0;i<frontBuilds.Count;i++)
		{
			if(frontBuilds[i].behindBuilds.Contains(this))
				frontBuilds[i].behindBuilds.Remove(this);
			BuildTweener tw = frontBuilds[i].GetComponentInChildren<BuildTweener>();
			if(frontBuilds[i].behindCharacters.Count==0
			   &&frontBuilds[i].behindBuilds.Count==0&&tw!=null)
			{
				tw.enabled = true;
				tw.Alpha = 1f; 
			}
		}
		frontBuilds = new List<BuildInfo> ();
		//清空所占格子的遮挡点;
		if(buildMarks!=null)
		{
			for(int i=0;i<buildMarks.Length;i++)
			{
				GridInfo grid = Globals.LocateGridInfo(buildMarks[i].transform.position);
				if(grid.buildMarks.Contains(buildMarks[i]))
				grid.buildMarks.Remove(buildMarks[i]);
			}
		}
	}

	//移动后，send_to_server=true通知服务器;
	public void SetBuild(bool send_to_server = true,bool clearOldGrid = true)
	{
		//修改by hc 2014.1.21 3d建筑不处理;
		if (is3D)
						return;
		//Debug.Log((int)BuildStatus.Create);
		//清空旧的占用格子;
		if (clearOldGrid)
			ClearGrid();//

		//设置当前位置;
		if (send_to_server && location != Position) {
			location = Position;
			if (WorkerManager.GetInstance.IsWorking && WorkerManager.GetInstance.WorkBuildInfo == this) {
				WorkerManager.GetInstance.ReAction ();
			}
		} else {
			location = Position;
		}
		int beginA = (int)location.x;
		int beginB = (int)location.z;
		for (int i=0; i<GridCount; i++) 
		{
			for (int j=0; j<GridCount; j++) 
			{
				int gridInfoA = i+beginA;
				int gridInfoB = j+beginB;
				GridInfo gridInfo = Globals.GridArray[gridInfoA,gridInfoB];
				gridInfo.buildInfo = this;
				gridInfo.isBuild = true;
				//设置格子权重;
				if(i>0&&i<GridCount-1&&j>0&&j<GridCount-1)
				{
					gridInfo.cost = Globals.GridBuildCost;
				}
				else
				{
					gridInfo.cost = Globals.GridEmptyCost;
				}
				for(int k=0;k<gridInfo.buildMarks.Count;k++)
				{
					if(!frontBuilds.Contains(gridInfo.buildMarks[k].buildInfo))
						frontBuilds.Add(gridInfo.buildMarks[k].buildInfo);
				}
			}
		}
		//设置遮挡建筑的透明度;
		if(csvInfo.BuildingClass=="Defense"||tid_type=="TRAPS")
		{
			for(int i=0;i<frontBuilds.Count;i++)
			{			
				if(!frontBuilds[i].behindBuilds.Contains(this))
					frontBuilds[i].behindBuilds.Add(this);

				BuildTweener tw = frontBuilds[i].GetComponentInChildren<BuildTweener>();
				if(tw!=null)
				{
					tw.enabled = true;
					tw.Alpha = Globals.obstacleAlpha;
				
				}
			}
		}
			
		InitStandPoint ();

		//初始化建筑格子的遮挡点;
		if(buildMarks!=null)
		{
			for(int i=0;i<buildMarks.Length;i++)
			{
				GridInfo grid = Globals.LocateGridInfo(buildMarks[i].transform.position);
				if(!grid.buildMarks.Contains(buildMarks[i]))
					grid.buildMarks.Add(buildMarks[i]);

				if(grid.buildInfo!=null&&grid.buildInfo!=this)
				{

					if(grid.buildInfo.csvInfo.BuildingClass=="Defense"||grid.buildInfo.tid_type=="TRAPS")
					{
						if(!behindBuilds.Contains(grid.buildInfo))
						{
							behindBuilds.Add(grid.buildInfo);
						}

						if(!grid.buildInfo.frontBuilds.Contains(this))
						{
							grid.buildInfo.frontBuilds.Add(this);
						}
					}
				}
			}
		}

		if(behindBuilds.Count>0)
		{
			BuildTweener tw = GetComponentInChildren<BuildTweener>();
			if(tw!=null)
			{
				tw.enabled = true;
				tw.Alpha =  Globals.obstacleAlpha;
			}
		}
		else
		{
			BuildTweener tw = GetComponentInChildren<BuildTweener>();
			if(tw!=null)
			{
				tw.enabled = true;
				tw.Alpha = 1f;
			}
		}

		isMoving = false;

		x = (int)location.x;
		y = (int)location.z;
		//移动后,保存到服务端
		//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)
		if (status != BuildStatus.New && send_to_server){
			if(preX != x || preY != y){
				BuildHandle.SendBuildLocationToServer(this);
				preX = x;
				preY = y;
			}
		}
	}

	/*
	public static void UpdateLocationToServer(BuildInfo s){
		ISFSObject data = new SFSObject();
		data.PutInt("x",x);
		data.PutInt("y",y);
		data.PutInt("regions_id",Globals.regions_id);
		data.PutLong("building_id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data,SFSNetworkManager.CMD_MoveBuilding, false, s.HandleResponse);
	}
*/

	//检测是否可以放置下去;
	public bool CheckBuildPlaceAble()
	{		
		int beginA = (int)Position.x;
		int beginB = (int)Position.z;
		for (int A = beginA; A<beginA+GridCount; A++)
		{
			for (int B = beginB; B<beginB+GridCount; B++)
			{
				Vector3 point = new Vector3(A,0f,B);
				GridInfo grid = Globals.LocateGridInfo(point);
				if((grid.isBuild&&!grid.buildInfo.Equals(this))||!grid.isInArea)
				{
					return false;
				}
			}	
		}
		return true;
	}

	//移动中的建筑重置位置;
	public void ResetPosition()
	{
		if (isSelected&&isMoving) 
		{
			Position = location;
			isAfterResetPos = true;
		}
	}
	
	//=====================================================================================================
	
	/*
	public void NewBuildToServer(int wood = 0,
	                             int stone = 0,
	                             int iron = 0,
	                             int gems = 0){

		//当 部署 成功后，需要清空 TID_BUILDING_ARTIFACT_WORKSHOP 上的 status_tid_level
		if (csvInfo.BuildingClass == "Artifact"){
			status = BuildStatus.Normal;
			start_time = 0;
			end_time = 0;

			BuildInfo bi = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
			bi.status_tid_level = null;//清空原有的神像;
			bi.artifact_type = 0;//ArtifactType.None;
			bi.artifact_boost = 0;
		}else{
			//扣除用户本地资源;
			Helper.SetResource(0,-wood,-stone,-iron,-gems,true);
			
			start_time = Helper.current_time();
			end_time = start_time + Helper.GetBuildTime(tid_level);		
			last_collect_time = end_time;//要到新建结束后，才能采集;

			RefreshBuildModel(tid,level);

			//需要时间;
			if (end_time > start_time)
				status = BuildStatus.New;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮);
			else{
				status = BuildStatus.Normal;//不需要时间;
				//设置最大容量及主建筑物级别;
				Helper.SetAllMaxLevel();
				Helper.SetAllMaxStored();
			}
		}
		//服务器还没有返回创建任务结束时,不能执行立即完成;
		//send_ing = true;

		SetBuild (false, false);
		ISFSObject data = new SFSObject();
		
		//花费;
		//data.PutInt("Wood",wood);
		//data.PutInt("Stone",stone);
		//data.PutInt("Iron",iron);
		//data.PutInt("Gems",gems);
		data.PutInt("x",x);
		data.PutInt("y",y);
		//data.PutLong("user_id",Globals.userId);
		data.PutUtfString ("buildingTid",tid);
		data.PutInt ("buildingLevel",1);
		//data.PutInt("start_time",start_time);
		//data.PutInt("end_time",end_time);
		//data.PutUtfString("tid_level",tid_level);
		//data.PutLong("building_id", building_id);
		SFSNetworkManager.Instance.SendRequest(data, "createBuilding", false, HandleResponse);
		//重新设置，所有建筑物的升级提醒标识;
		Helper.SetBuildUpgradeIcon();

		//重新计算版面数据;
		int count = Helper.CalcShopCates(false);
		//设置商城按钮数量;
		//ScreenUIManage.Instance.SetShopCount (count);
        UIManager.Instance().GetCtroller<MainInterfaceCtrl>().SetShopCount(count);

		MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid> ().clearMesh ();
		MoveOpEvent.Instance.UnDrawBuildPlan (MoveOpEvent.Instance.SelectedBuildInfo);
		PopManage.Instance.RefreshBuildBtn(this);

		//设置工人工作;
		WorkerManage.Instance.setWorkBuilding(Helper.getWorkBuilding());
	}
	*/

	public void HandleResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		Debug.Log ("HandleResponse");
		send_ing = false;	
		Debug.Log (dt.GetUtfString("message"));
		Debug.Log (dt.GetInt("code"));
		//发送新建完成;
		if (dt.ContainsKey("artifact_type") && dt.ContainsKey("artifact_boost")){
			//Debug.Log(dt.GetDump());
			//SendFinishRequest(building_id, false, 1);
			artifact_type = (ArtifactType) dt.GetInt("artifact_type");
			artifact_boost = dt.GetInt("artifact_boost");
		}
	}
	
	
	public void OnCancelCreate(){
		//Debug.Log("OnCancelCreate");

		if(MoveOpEvent.Instance.SelectedBuildInfo==this)
		{
			MoveOpEvent.Instance.UnSelectBuild ();
			MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid>().clearMesh();
		}
		
		//新建的，也加入建筑物列表中，注：在取消新建时,需要移除;
		if (DataManager.GetInstance.buildList.ContainsKey(building_id)){
			DataManager.GetInstance.buildList.Remove(building_id);
		}
		if(DataManager.GetInstance.buildArray.Contains(this))
			DataManager.GetInstance.buildArray.Remove(this);

		if(csvInfo.TID=="TID_BUILDING_LANDING_SHIP")
		{
			if(Globals.LandCrafts.Contains(this))
			Globals.LandCrafts.Remove(this);
		}
		//清空占用格子;
		ClearGrid();
		BuildInfo.destroyAndCacheBuildInfo (this);
		//Destroy(gameObject);
		
	}

	//检查工人是否忙碌;
	public bool CheckWookerCount(){
		//获得忙碌的工人数;building_id最快完工的building_id;
		long min_b_id = 0;
		if (DataManager.GetInstance.model.user_info.worker_count <= Helper.GetWorkeringCount(ref min_b_id)){
			BuildInfo min_s = (BuildInfo)DataManager.GetInstance.buildList[min_b_id];
			
			int seconds = min_s.end_time - Helper.current_time();					
			int gems_count = CalcHelper.calcTimeToGems(seconds);				
			
			ISFSObject dt = new SFSObject();
			dt.PutLong("building_id",min_s.building_id);
			UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
                StringFormat.FormatByTid("TID_NOT_ENOUGH_WORKERS_HEADER"), 
                StringFormat.FormatByTid("TID_NOT_ENOUGH_WORKERS_TEXT"),
                gems_count.ToString(), 
                PopDialogBtnType.ImageBtn, dt,
				onDialogFinishYes,true,null
			);
            /**
            PopManage.Instance.ShowDialog(
				StringFormat.FormatByTid("TID_NOT_ENOUGH_WORKERS_TEXT"),
				StringFormat.FormatByTid("TID_NOT_ENOUGH_WORKERS_HEADER"),
				true,
				PopDialogBtnType.ImageBtn,
				false,
				dt,
				onDialogFinishYes,
				null,
				gems_count.ToString()
				);
            **/
			return false;											
		}else{
			return true;
		}
	}

	/*
	public void OnOkCreate(){
		//Debug.Log("OnOkCreate");
		if (CheckBuildPlaceAble()){
			//当 部署 成功后，需要清空 TID_BUILDING_ARTIFACT_WORKSHOP 上的 status_tid_level
			if (csvInfo.BuildingClass == "Artifact"){
				//NewBuildToServer(0,0,0,0);
				//BuildHandle.CreateBuild (0,0,0,0,this);
			}else{
				if ("BUILDING".Equals(csvInfo.TID_Type) && CheckWookerCount() == false){			
					return;
				};
				ISFSObject dt = Helper.getCostDiffToGems(tid_level,0,true);
				int gems = dt.GetInt("Gems");
				//Debug.Log(gems);
				//资源不足，需要增加宝石才行;
				if (gems > 0){
                    //UIManager.Instance().msgCtrl.ShowPopWithDiamond2(dt.GetUtfString("title"), dt.GetUtfString("msg"), gems.ToString(), dt,this, this.building_id);
                    UIManager.Instance().GetComponent<PopMsgCtrl>().ShowDialog(
						dt.GetUtfString("msg"), 
						dt.GetUtfString("title"), 
						gems.ToString(), 
						PopDialogBtnType.ImageBtn,
						dt, onDialogYes);

                    
					PopManage.Instance.ShowDialog(
						dt.GetUtfString("msg"),
						dt.GetUtfString("title"),
						true,
						PopDialogBtnType.ImageBtn,
						true,
						dt,
						onDialogYes,
						onDialogNo,
						gems.ToString()
						);
                    
                }
                else{
					AudioPlayer.Instance.PlaySfx("building_construct_07");
					//NewBuildToServer(dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"));
					//BuildHandle.CreateBuild (dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"),this);
				}
			}
		}else{
			//PopManage.Instance.ShowMsg("no can checkBuild");
		}
	}
	*/

	private void onDialogYes(ISFSObject dt,BuildInfo s = null){
		//Debug.Log("onDialogYes");
		//Debug.Log(dt.GetDump());
		
		if (DataManager.GetInstance.model.user_info.diamond_count >= dt.GetInt("Gems")){
			AudioPlayer.Instance.PlaySfx("building_construct_07");
			//BuildHandle.CreateBuild (dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"),this);
			//NewBuildToServer(dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"));
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog(onDialogNo,onDialogNo);
		}
	}
	
	private void onDialogNo(ISFSObject dt){
		//Debug.Log("onDialogNo");
		OnCancelCreate();
	}
	//=====================================================================================================

	//==========================================以下是用户点击：移除后的 相关操作================================================
	public void OnRemoval(BuildInfo s){
		//Debug.Log("OnOkCreate");
		//bool has_removal = true;
		string title = "";
		string msg = "";
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
		if (csvData.TID_Type == "OBSTACLES"){
			//TID_POPUP_CLEAR_OBSTACLE_TITLE Remove <item>?
			//TID_POPUP_CLEAR_OBSTACLE Remove <item> and gain:
			//TID_CAN_NOT_CLEAR_OBSTACLE  Need Headquarters level <number> to remove this!
			
			title = StringFormat.FormatByTid("TID_POPUP_CLEAR_OBSTACLE_TITLE",new object[]{StringFormat.FormatByTid(s.tid)});
			
			msg = StringFormat.FormatByTid("TID_POPUP_CLEAR_OBSTACLE",new object[]{StringFormat.FormatByTid(s.tid)});
			msg = msg + " " + csvData.LootCount.ToString() + " " + StringFormat.FormatByTid(Helper.GetNameToTid(csvData.LootResource));
			//msg = "TID_POPUP_CLEAR_OBSTACLE";
			Debug.Log("DataManager.GetInstance.model.user_info.town_hall_level:" + DataManager.GetInstance.model.user_info.town_hall_level + ";csvData.RequiredTownHallLevel:" + csvData.RequiredTownHallLevel);
			if (DataManager.GetInstance.model.user_info.town_hall_level < csvData.RequiredTownHallLevel){
				
				msg = StringFormat.FormatByTid("TID_CAN_NOT_CLEAR_OBSTACLE",new object[]{csvData.RequiredTownHallLevel});
				PopManage.Instance.ShowMsg(msg);
				//has_removal = false;
			}else{
				//ISFSObject dt = new SFSObject();
				ISFSObject dt = Helper.getCostDiffToGems(s.tid_level,0,true);
				dt.PutLong("building_id",s.building_id);
                //TODO
                //移除障碍物
                UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
                   msg,
                   title,
                   "",
                   PopDialogBtnType.ConfirmBtn,
                   dt,
                   onDialogRemovalYes
                );

                /**
                PopManage.Instance.ShowDialog(
					msg,
					title,
					true,
					PopDialogBtnType.ImageBtn,
					false,
					dt,
					onDialogRemovalYes,
					onDialogRemovalNo,
					csvData.ClearCost.ToString(),  //StringFormat.FormatByTid("TID_BUTTON_REMOVE")
					null,
					"goldIco"
					);
                **/
			}
			
		}else if (csvData.BuildingClass == "Artifact" || (s.tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && s.hasStatue())){

			if (csvData.BuildingClass != "Artifact"){
				csvData = CSVManager.GetInstance.csvTable [s.status_tid_level] as CsvInfo;
			}

			string artiact_tid_level = Helper.BuildTIDToArtifactTID(csvData.TID) + "_1";
			CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];

			//TID_POPUP_HEADER_ABOUT_TO_SCRAP_ARTIFACT Reclaim statue?
			//TID_POPUP_ABOUT_TO_SCRAP_ARTIFACT Reclaiming the statue will destroy it and give you <number> <resource>.
			title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SCRAP_ARTIFACT");
			msg = StringFormat.FormatByTid("TID_POPUP_ABOUT_TO_SCRAP_ARTIFACT",new object[]{csvArtiact.SellResourceAmount, StringFormat.FormatByTid(Helper.GetNameToTid(csvArtiact.SellResource))});
			
			ISFSObject dt = new SFSObject();
			dt.PutLong("building_id",s.building_id);
			dt.PutInt("Gems",0);

            UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
                msg,
                title,
                "",
                PopDialogBtnType.ConfirmBtn,
                dt,
                onDialogRemovalYes
                );
            /**
			PopManage.Instance.ShowDialog(
				msg,
				title,
				true,
				PopDialogBtnType.YesAndNoBtn,
				false,
				dt,
				onDialogRemovalYes,
				onDialogRemovalNo,
				StringFormat.FormatByTid("TID_SCRAP"),
				StringFormat.FormatByTid("TID_BUTTON_CANCEL")
				);
            **/
		}
	}
	
	private void onDialogRemovalYes(ISFSObject dt,BuildInfo s = null){

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
				onDialogRemovalByGemsYes,
				onDialogRemovalNo,
				gems.ToString()
				);
		}else{
			RemovalBuildToLocal(dt);
		}

	}

	private void onDialogRemovalByGemsYes(ISFSObject dt){
		
		//int gems = dt.GetInt("Gems");
		
		if (DataManager.GetInstance.model.user_info.diamond_count >= dt.GetInt("Gems")){
			RemovalBuildToLocal(dt);
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	private void onDialogRemovalNo(ISFSObject dt){
		this.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(this);
	}
	
	public void RemovalBuildToLocal(ISFSObject dt){
		//Debug.Log(dt.GetDump());
		BuildInfo s = DataManager.GetInstance.buildList[dt.GetLong("building_id")] as BuildInfo;

		if (s.tid_type == "OBSTACLES"){
			//ISFSObject dt2 = Helper.getCostDiffToGems(s.tid_level,true,true);
			//Debug.Log(dt2.GetDump());
			s.removal_gold = dt.GetInt("Gold");
			s.removal_gems = dt.GetInt("Gems");
			//扣除用户本地资源;
			Helper.SetResource(-s.removal_gold,0,0,0,-s.removal_gems,true);


			
			
			start_time = Helper.current_time();
			end_time = start_time + Helper.GetBuildTime(s.tid_level);
			status = BuildStatus.Removal;

			/*由于移除时间太短,移除中的状态不再通知服务器；时间到后，直接通知服务器移除结束;
				RemovalBuildToServer(s);
			 */
			s.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(s);

		}else if (s.csvInfo.BuildingClass == "Artifact" || (s.tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && s.hasStatue())){
			//status = BuildStatus.Removal;
			/*
			string artiact_tid = "";
			if (s.csvInfo.BuildingClass == "Artifact"){
				artiact_tid = s.csvInfo.TID;
			}else{
				CsvInfo csvInfo = CSVManager.GetInstance.csvTable [s.status_tid_level] as CsvInfo;
				artiact_tid = csvInfo.TID;
			}

			string artiact_tid_level = Helper.BuildTIDToArtifactTID(artiact_tid) + "_1";
			CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];

			Helper.setResourceCount(csvArtiact.SellResource,csvArtiact.SellResourceAmount,false,false);

*/
			//没有时间,直接通知服务器移除完成;
			RemovalBuildToServer(s);
		}else{
			return;
		}
		//dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems");

		

	}

	public void RemovalBuildToServer(BuildInfo s, int current_time = 0){
		if (s.tid_type == "OBSTACLES" || s.csvInfo.BuildingClass == "Artifact" || (s.tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && s.hasStatue())){
			if (current_time == 0){
				current_time = Helper.current_time();
			}

			//send_ing = true;

			ISFSObject data = new SFSObject();	
			//把移除费用发送到服务器;
			if (s.tid_type == "OBSTACLES"){				
				data.PutInt("removal_gold",s.removal_gold);	
				data.PutInt("removal_gems",s.removal_gems);
			}
			
			
			data.PutInt("current_time",current_time);		
			//花费;
			data.PutLong("building_id", s.building_id);		
			SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_RemoveBuilding, false, null);
			//重新设置，所有建筑物的升级提醒标识;
			Helper.SetBuildUpgradeIcon();

			//重新计算版面数据;
			int count = Helper.CalcShopCates(false);
			//设置商城按钮数量;
			//ScreenUIManage.Instance.SetShopCount (count);
            UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);

            if (s.tid_type == "OBSTACLES" || s.csvInfo.BuildingClass == "Artifact"){

				if(s.tid_type=="OBSTACLES")
				{
					string LootResource = Helper.GetNameToTid(s.csvInfo.LootResource);

					PartType partType = PartType.Gold;
					int c = 0;
					int max_num = 0;
					if (LootResource == "TID_DIAMONDS"){
						//Helper.SetResource(0,0,0,0,s.csvInfo.LootCount,true);
						c = s.csvInfo.LootCount;
						partType = PartType.Gem;
					}else if (LootResource == "TID_WOOD"){
						//Helper.SetResource(0,s.csvInfo.LootCount,0,0,0,true);
						partType = PartType.Wood;
						max_num = DataManager.GetInstance.model.user_info.max_wood_count;
					}else if (LootResource == "TID_STONE"){
						//Helper.SetResource(0,0,s.csvInfo.LootCount,0,0,true);
						partType = PartType.Stone;
						max_num = DataManager.GetInstance.model.user_info.max_stone_count;
					}else if (LootResource == "TID_METAL"){
						//Helper.SetResource(0,0,0,s.csvInfo.LootCount,0,true);
						partType = PartType.Iron;
						max_num = DataManager.GetInstance.model.user_info.max_iron_count;
					}else if (LootResource == "TID_GOLD"){
						//Helper.SetResource(s.csvInfo.LootCount,0,0,0,0,true);
						partType = PartType.Gold;
						max_num = DataManager.GetInstance.model.user_info.max_gold_count;
					}
					
					
					if(LootResource != "TID_DIAMONDS")
					{
						
						if(max_num>0)
						{
							c =  Mathf.CeilToInt(s.csvInfo.LootCount*100f/max_num);
						}
						
						if(c>15)c=15;
						
						int addC = Mathf.RoundToInt(s.csvInfo.LootCount/500f);
						if(addC>15)addC = 15;
						
						c+=addC;
					}


					if(MoveOpEvent.Instance.SelectedBuildInfo==this)
					{
						MoveOpEvent.Instance.UnSelectBuild ();
						MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid>().clearMesh();
					}

					BuildTweener bt = s.transform.GetComponentInChildren<BuildTweener>();
					if(bt!=null)
					{
						Transform shadow = s.transform.Find("buildPos/BuildMain/Shadow");
						if(shadow!=null)
							shadow.parent = bt.transform;
						bt.enabled = true;
						bt.Alpha = 0f;
					}
					s.status = BuildStatus.Removaling;
					this.buildUI.RefreshBuildBtn ();
					//PopManage.Instance.RefreshBuildBtn (this);
					if(s.buildUIManage!=null)
                        s.buildUIManage.ShowTimeBar(false);
                    if (s.buildUI != null)
                        s.buildUI.ShowTimeBar(false);
                    s.emitter.enabled = true;
					s.emitter.Emit(c,s.transform.position,partType,s.csvInfo.LootCount);			



				}
				else
				{
					//s.OnCancelCreate();
					if(MoveOpEvent.Instance.SelectedBuildInfo==this)
					{
						MoveOpEvent.Instance.UnSelectBuild ();
						MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid>().clearMesh();
					}

					BuildTweener bt = s.transform.GetComponentInChildren<BuildTweener>();
					if(bt!=null)
					{
						Transform shadow = s.transform.Find("buildPos/BuildMain/Shadow");
						if(shadow!=null)
							shadow.parent = bt.transform;
						bt.enabled = true;
						bt.Alpha = 0f;
					}
					s.status = BuildStatus.Removaling;
					this.buildUI.RefreshBuildBtn ();
					//PopManage.Instance.RefreshBuildBtn (this);
                    if(s.buildUIManage!=null)
					    s.buildUIManage.ShowTimeBar(false);
                    if (s.buildUI != null)
                        s.buildUI.ShowTimeBar(false);

                    string artiact_tid = s.csvInfo.TID;
					string artiact_tid_level = Helper.BuildTIDToArtifactTID(artiact_tid) + "_1";
					CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];
					
					Helper.SetResourceCount(csvArtiact.SellResource,csvArtiact.SellResourceAmount,false,false);

					s.transform.GetComponentInChildren<Artifact>().removeArtifact(csvArtiact.SellResource);
				}
			}else{
				//清空TID_BUILDING_ARTIFACT_WORKSHOP 中的神像;

				string artiact_tid = "";
				CsvInfo csvInfo = CSVManager.GetInstance.csvTable [s.status_tid_level] as CsvInfo;
				artiact_tid = csvInfo.TID;								
				string artiact_tid_level = Helper.BuildTIDToArtifactTID(artiact_tid) + "_1";
				CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];
				
				Helper.SetResourceCount(csvArtiact.SellResource,csvArtiact.SellResourceAmount,false,false);


				//关闭所有界面窗口;
				if(PopWin.current!=null)PopWin.current.CloseTween();


				//Debug.Log("csvArtiact.SellResource:" + csvArtiact.SellResource);
				//播放动画;
				crystlePartObj = Instantiate(ResourceCache.load ("UI/CrystlePart")) as GameObject;
				Transform crystlePart = crystlePartObj.transform;
				crystlePart.parent = s.transform.Find("buildPos/BuildMain");
				crystlePart.name = "crystle";
				crystlePart.localPosition = Vector3.zero;
				crystlePart.localRotation = new Quaternion (0,0,0,0);				
				crystlePart.GetComponent<UISprite> ().spriteName = csvArtiact.SellResource;
				crystlePart.GetComponent<UISprite> ().MakePixelPerfect ();
				crystlePart.transform.localScale = Vector3.one*0.002f;

				//Debug.Break();
				TweenAlpha ta = crystlePart.GetComponent<TweenAlpha> ();
				ta.onFinished = new List<EventDelegate> ();
				ta.onFinished.Add (new EventDelegate(this,"removeCrystleDone"));
				ta.PlayForward ();

				s.status_tid_level = null;
				s.artifact_boost = 0;
				s.artifact_type = ArtifactType.None;


				//刷新按钮状态;
				s.buildUI.RefreshBuildBtn ();
				//PopManage.Instance.RefreshBuildBtn(s);
			}
		}
	}

	/*
	void RemoveCrystleDone()
	{
		if (crystlePartObj != null) {
			Destroy(crystlePartObj);
		}
	}
	*/
	//========================================以上是用户点击：移除后的 相关操作===================================

	
	
	//====================================以下是建筑状态在：新建，升级，生产，训练，移除等状态时，用户点：取消 操作=================================================================
	//取消（新建，升级，生产，训练，移除);
	//Status -1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;	

	public void CanceBuildStatus(BuildInfo s){
		string title = "";
		string msg = "";
		bool has_cancel = true;

		/*
		TID_POPUP_CANCEL_CONSTRUCTION_TITLE	Stop construction?
			TID_POPUP_CANCEL_CONSTRUCTION	Do you really want to stop constructing <item>? Only <number>% of the cost will be refunded.

		TID_POPUP_CANCEL_UPGRADE_TITLE	Stop upgrade?
				TID_POPUP_CANCEL_UPGRADE	Do you really want to stop upgrading <item>? Only <number>% of the cost will be refunded.

			TID_POPUP_CANCEL_CLEAR_TITLE	Stop removal?				
				TID_POPUP_CANCEL_CLEAR	Do you really want to stop removing <item>?
				
				TID_POPUP_CANCEL_TROOP_TRAINING_TITLE	Stop loading troops?
				TID_POPUP_CANCEL_TROOP_TRAINING	Do you really want to stop loading <item>? Full cost will be refunded.

TID_POPUP_CANCEL_ARTIFACT_TITLE	Stop statue production
				TID_POPUP_CANCEL_ARTIFACT	Do you really want to stop production of <item>? Materials will be refunded.

TID_STOP	Stop
				TID_BUTTON_CANCEL	Cancel	
*/
		if (s.status == BuildStatus.New){
			//新建取消;
			title = "TID_POPUP_CANCEL_CONSTRUCTION_TITLE";
			msg = "TID_POPUP_CANCEL_CONSTRUCTION";
		}else if (s.status == BuildStatus.Upgrade){
			//升级取消;
			title = "TID_POPUP_CANCEL_UPGRADE_TITLE";
			msg = "TID_POPUP_CANCEL_UPGRADE";
			
		}else if (s.status == BuildStatus.Removal){
			//移除取消;
			//title = "TID_POPUP_CANCEL_CLEAR_TITLE";
			//msg = "TID_POPUP_CANCEL_CLEAR";

			//直接取消，移除状态，不弹出确认框，不通知服务器;
			//归还资源;
			//移除取消;全部返还;
		  	//BuildCost bc = Helper.getBuildCost(s.tid_level);
			Helper.SetResource(s.removal_gold,0,0,0,s.removal_gems,true);

			if (s.tid_type == "OBSTACLES"){
				//
				string LootResource = Helper.GetNameToTid(s.csvInfo.LootResource);
				if (LootResource == "TID_DIAMONDS"){
					Helper.SetResource(0,0,0,0,-s.csvInfo.LootCount,true);
				}else if (LootResource == "TID_WOOD"){
					Helper.SetResource(0,-s.csvInfo.LootCount,0,0,0,true);
				}else if (LootResource == "TID_STONE"){
					Helper.SetResource(0,0,-s.csvInfo.LootCount,0,0,true);
				}else if (LootResource == "TID_METAL"){
					Helper.SetResource(0,0,0,-s.csvInfo.LootCount,0,true);
				}else if (LootResource == "TID_GOLD"){
					Helper.SetResource(-s.csvInfo.LootCount,0,0,0,0,true);
				}
			}
			s.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(s);
			has_cancel = false;
			return;
		}else if (s.status == BuildStatus.Research){
			//研究取消;(升级)
			title = "TID_POPUP_CANCEL_UPGRADE_TITLE";
			msg = "TID_POPUP_CANCEL_UPGRADE";
			
		}else if (s.status == BuildStatus.Train){
			//训练取消;
			title = "TID_POPUP_CANCEL_TROOP_TRAINING_TITLE";
			msg = "TID_POPUP_CANCEL_TROOP_TRAINING";
			
		}else if (s.status == BuildStatus.CreateStatue){
			//生产神像取消;
			title = "TID_POPUP_CANCEL_ARTIFACT_TITLE";
			msg = "TID_POPUP_CANCEL_ARTIFACT";
		}else{
			
			has_cancel = false;
		}
		
		if (has_cancel){
			title = StringFormat.FormatByTid(title);
			msg = StringFormat.FormatByTid(msg,new object[]{StringFormat.FormatByTid(s.tid), 50});
			
			ISFSObject dt = new SFSObject();
			dt.PutLong("building_id",s.building_id);

            UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(
                msg,
                title,
                "",
                PopDialogBtnType.ConfirmBtn,
                dt,
                onDialogCancelYes
                );

            /**
			PopManage.Instance.ShowDialog(
				msg,
				title,
				true,
				PopDialogBtnType.YesAndNoBtn,
				false,
				dt,
				onDialogCancelYes,
				onDialogCancelNo,
				StringFormat.FormatByTid("TID_BUTTON_OKAY"),
				StringFormat.FormatByTid("TID_BUTTON_CANCEL")
				);
            **/


		}
	}
	
	private void onDialogCancelYes(ISFSObject dt,BuildInfo buildInfo = null){
		//Debug.Log("onDialogCancelYes");
		//OnCancelCreate();
		//BuildInfo s = DataManager.GetInstance.BuildList[dt.GetLong("building_id")] as BuildInfo;
		//if (s.status == BuildStatus.New)
			//BuildHandle.CancelCreateBuild (s);
		//else if (s.status == BuildStatus.Upgrade)
		//	BuildHandle.CancelUpgradeBuild (s);	
		//s.CancelBuildToServer(s);
	}
	
	private void onDialogCancelNo(ISFSObject dt){
		//Debug.Log("onDialogCancelNo");
		//OnCancelCreate();
		BuildInfo s = DataManager.GetInstance.buildList[dt.GetLong("building_id")] as BuildInfo;
		s.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(s);
	}
	
	
	public void CancelBuildToServer(BuildInfo s){

		bool has_cancel = true;
		BuildCost bc = null;
		int current_time = Helper.current_time();
		if (s.status == BuildStatus.New){
			//新建取消;返回50%;
			bc = Helper.GetBuildCost(s.tid_level);
			bc.gold = bc.gold/2;
			bc.wood = bc.wood/2;
			bc.stone = bc.stone/2;
			bc.iron = bc.iron/2;
			
		}else if (s.status == BuildStatus.Upgrade){
			//升级取消;返回50%;
			bc = Helper.getUpgradeCost(s.tid_level);
			bc.gold = bc.gold/2;
			bc.wood = bc.wood/2;
			bc.stone = bc.stone/2;
			bc.iron = bc.iron/2;
			
		}else if (s.status == BuildStatus.Research){
			//研究取消;(升级)返回50%;			
			bc = Helper.getUpgradeCost(s.status_tid_level);
			bc.gold = bc.gold/2;
			bc.wood = bc.wood/2;
			bc.stone = bc.stone/2;
			bc.iron = bc.iron/2;
			
		}else if (s.status == BuildStatus.Train){
			//训练取消;返回还未生产完成的部分;
			bc = Helper.GetBuildCost(s.status_tid_level);
			int train_time = Helper.GetBuildTime(s.status_tid_level);
			//剩于时间;
			int LeftOnTime = s.end_time - current_time;
			Debug.Log("LeftOnTime:" + LeftOnTime);
			if (LeftOnTime > 0){
				//剩于时间/每个兵生产所需时间 = 还剩可生成的兵数;
				int troops_num = Mathf.CeilToInt(LeftOnTime*1f / (train_time*1f));

				//Debug.Log("troops_num:" + troops_num);
				bc.gold = troops_num * bc.gold;
			}else{
				has_cancel = false;
			}
		}else if (s.status == BuildStatus.CreateStatue){
			//生产神像取消;全部返还;
			bc = Helper.GetBuildCost(s.status_tid_level);
		}else{
			
			has_cancel = false;
		}

		//Debug.Log("has_cancel:" + has_cancel + ";send_ing:" + send_ing);

        //TODO
        /**
		if (has_cancel){
			if (send_ing){
				has_cancel = false;
			}
		}
		**/
		
		if (has_cancel){
			//添加用户本地资源;
			if (s.status == BuildStatus.CreateStatue){
				//生产神像取消;全部返还;
				//bc.piece
				CsvInfo csvInfo = (CsvInfo)CSVManager.GetInstance.csvTable[s.status_tid_level];
				if (csvInfo.TID == "TID_BUILDING_ARTIFACT1"){
					//Common Artifact;
					DataManager.GetInstance.model.user_info.common_piece += bc.piece;
				}else if (csvInfo.TID == "TID_BUILDING_ARTIFACT2"){
					//Rare Artifact;
					DataManager.GetInstance.model.user_info.rare_piece += bc.piece;
				}else if (csvInfo.TID == "TID_BUILDING_ARTIFACT3"){
					//Epic Artifact;
					DataManager.GetInstance.model.user_info.epic_piece += bc.piece;
				}

				s.status_tid_level = null;
				s.artifact_type = ArtifactType.None;
				s.artifact_boost = 0;
			}else{
				if (s.status == BuildStatus.Train){
					int collect_num = bc.gold;
					if (collect_num > 0){
						
						int max_num = DataManager.GetInstance.model.user_info.max_gold_count;
						
						
						//collect_num = max_num;
						int c = 0;
						
						if(max_num>0)
						{
							c =  Mathf.CeilToInt(collect_num*100f/max_num);
						}
						
						if(c>15)c=15;
						
						int addC = Mathf.RoundToInt(collect_num/500f);
						if(addC>15)addC = 15;
						
						c+=addC;
						s.emitter.enabled = true;
						s.emitter.Emit(c,s.transform.position,PartType.Gold,collect_num);
						
						//重新设置，所有建筑物的升级提醒标识;
						Helper.SetBuildUpgradeIcon();
						
					}
				}else{
					Helper.SetResource(bc.gold,bc.wood,bc.stone,bc.iron,0,true);
				}
			}

			//移除取消，不通知服务器，因为移除时间太长，移除开始时，也没有通知服务器;
			if (s.status != BuildStatus.Removal){
			
				//服务器还没有返回创建任务结束时,不能执行立即完成;
				//send_ing = true;
				
				
				
				ISFSObject data = new SFSObject();
				
				data.PutInt("current_time",current_time);
				
				//花费;
				data.PutInt("Gold",bc.gold);
				data.PutInt("Wood",bc.wood);
				data.PutInt("Stone",bc.stone);
				data.PutInt("Iron",bc.iron);
				data.PutInt("Piece",bc.piece);
				
				data.PutLong("building_id", building_id);
				
				SFSNetworkManager.Instance.SendRequest(data, "cancel_nur", false, HandleResponse);


			}
			
			if (s.status == BuildStatus.New){
				s.OnCancelCreate();
				//重新计算版面数据;
				int count = Helper.CalcShopCates(false);
				//设置商城按钮数量;
				//ScreenUIManage.Instance.SetShopCount (count);
                UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);
            }
            else{
				status = BuildStatus.Normal;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)

				s.RefreshBuildModel(s.tid,s.level);
				s.buildUI.RefreshBuildBtn ();
				//PopManage.Instance.RefreshBuildBtn(s);

				//重新设置，所有建筑物的升级提醒标识;
				Helper.SetBuildUpgradeIcon();

				//重新计算版面数据;
				int count = Helper.CalcShopCates(false);
				//设置商城按钮数量;
				//ScreenUIManage.Instance.SetShopCount (count);
                UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);
            }

			WorkerManager.GetInstance.SetWorkBuilding(null);
		}
	}
	
	//====================================以上是建筑状态在：新建，升级，生产，训练，移除等状态时，用户点：取消 操作===============================

	//===============================以下为：立即完成（新建，升级，生产，训练，移除)============================================================
	//立即完成（新建，升级，生产，训练，移除);
	//Status -1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;	
	public void FinishBuildStatus(BuildInfo s){

		if (s.status == BuildStatus.New || s.status == BuildStatus.Upgrade || s.status == BuildStatus.Removal
		                   ||s.status == BuildStatus.Research || s.status == BuildStatus.Train||s.status == BuildStatus.CreateStatue){
			
			int current_time = Helper.current_time();

			int gems = CalcHelper.calcTimeToGems(s.end_time - current_time);
			/*
			TID_POPUP_HEADER_ABOUT_TO_SPEED_UP = Finish now!
				TID_POPUP_SPEED_UP_UPGRADE = Do you want to finish the upgrade of <item> for <number> Diamonds?
					TID_POPUP_SPEED_UP_CONSTRUCTION = Do you want to finish the construction of <item> for <number> Diamonds?

TID_POPUP_HEADER_ABOUT_TO_SPEED_UP_RESEARCH = Finish research!
TID_POPUP_SPEED_UP_RESEARCH = Do you want to finish the researching of <item> for <number> Diamonds?

TID_POPUP_HEADER_SPEED_UP_ARTIFACT = Finish statue
TID_POPUP_SPEED_UP_ARTIFACT = Do you want to speed up creating the statue for <number> Diamonds?

TID_POPUP_HEADER_ABOUT_TO_SPEED_UP_ALL_TROOP_TRAIN = Finish loading!
TID_POPUP_SPEED_UP_ALL_TROOP_TRAINING = Do you want to finish the loading of all troops for <number> Diamonds?
			 */

			string title = "";
			string msg = "";
			if (s.status == BuildStatus.New){
				title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP");
				msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_CONSTRUCTION",new object[]{ gems,StringFormat.FormatByTid(s.tid) });
			}else if (s.status == BuildStatus.Upgrade){
				title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP");
				msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_UPGRADE",new object[]{ gems,StringFormat.FormatByTid(s.tid) });
			}else if (s.status == BuildStatus.Research){
				title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP_RESEARCH");
				CsvInfo csvInfo = CSVManager.GetInstance.csvTable[s.status_tid_level] as CsvInfo;
				msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_RESEARCH",new object[]{gems,StringFormat.FormatByTid(csvInfo.TID)});
				Debug.Log (msg);
			}else if (s.status == BuildStatus.CreateStatue){
				title = StringFormat.FormatByTid("TID_POPUP_HEADER_SPEED_UP_ARTIFACT");
				msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_ARTIFACT",new object[]{gems});
			}else if (s.status == BuildStatus.Removal){
				title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP");
				msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_CONSTRUCTION",new object[]{StringFormat.FormatByTid(s.tid), gems});
			}else if (s.status == BuildStatus.Train){
				title = StringFormat.FormatByTid("TID_POPUP_HEADER_ABOUT_TO_SPEED_UP_ALL_TROOP_TRAIN");
				msg = StringFormat.FormatByTid("TID_POPUP_SPEED_UP_ALL_TROOP_TRAINING",new object[]{gems});
			}
			ISFSObject dt = new SFSObject();
			dt.PutLong("building_id",s.building_id);
            //UIManager.Instance().msgCtrl.ShowPopWithDiamond(title,msg, gems.ToString(), s.building_id);

			UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(msg, title, gems.ToString(), PopDialogBtnType.ImageBtn, dt, onDialogFinishYes,true,s);

			/*
            PopManage.Instance.ShowDialog(
				msg,
				title,
				true,
				PopDialogBtnType.ImageBtn,
				false,
				dt,
				onDialogFinishYes,
				onDialogFinishNo,
				gems.ToString()
				);
				*/
		}else{
			//状态已经改变，没有可以：立即完成的 按钮;
			s.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(s);
		}
	}

	private void onDialogFinishNo(ISFSObject dt){
		BuildInfo s = DataManager.GetInstance.buildList[dt.GetLong("building_id")] as BuildInfo;
		//刷新按钮状态;
		s.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(s);
	}

	private void onDialogFinishYes(ISFSObject dt,BuildInfo s){
		//Debug.Log("onDialogFinishYes");
		//OnCancelCreate();
		//BuildInfo s = DataManager.GetInstance.BuildList[buildInfo.b] as BuildInfo;

		if(s.isWaitForReturnBuildId){
			string msg = StringFormat.FormatByTid("TID_ERROR_MESSAGE_WAITFORNETRETURN");
			UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop (msg);
			return;
		}
		if(s.status == BuildStatus.New || s.status == BuildStatus.Upgrade){
			//BuildHandle.SpeedUpUpgradeBuild (this);
			return;
		}

		//按确定后，需要重新计算，防止用户在弹出确定对话框后，一直不动，过了很久后（建筑物状态都已经改变），才点确定;
		if (s.status == BuildStatus.New || s.status == BuildStatus.Upgrade || s.status == BuildStatus.Removal
		    ||s.status == BuildStatus.Research || s.status == BuildStatus.Train||s.status == BuildStatus.CreateStatue){
			
			int current_time = Helper.current_time();
			
			int gems = CalcHelper.calcTimeToGems(s.end_time - current_time);
			//Debug.Log("current_time:" + current_time + ";s.end_time:" + s.end_time + ";seconds:" + (current_time - s.end_time) + ";gems:" + gems);
		
			if (DataManager.GetInstance.model.user_info.diamond_count >= gems){
				FinishBuildToServer(s,current_time, gems);
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog();
			}
		}else{
			//状态已经改变，没有可以：立即完成的 按钮;
			s.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(s);
		}
	}

	
	public static void FinishBuildToServer(BuildInfo s,int current_time = 0, int gems = 0, bool is_instant = false, string status_tid_level = null){

		if (current_time == 0){
			current_time = Helper.current_time();
		}

		if (s.tid_type == "OBSTACLES"){
			s.RemovalBuildToServer(s,current_time);

		}else{
			//Debug.Log(";status_tid_level:" + status_tid_level + ";s.status_tid_level:" + s.status_tid_level);
			if (status_tid_level == null || status_tid_level == ""){
				if (s.status == BuildStatus.Research || s.status == BuildStatus.Train||s.status == BuildStatus.CreateStatue)
					status_tid_level = s.status_tid_level;
				else
					status_tid_level = s.tid_level;
			}			
			CsvInfo csvInfo = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
			if (gems == 0){
				if (is_instant){
					gems = Helper.GetUpgradeInstant(csvInfo.TID_Level);
				}else{
					gems = CalcHelper.calcTimeToGems(s.end_time - current_time);
				}
			}
			if (is_instant && s.status == BuildStatus.Normal){
				if (s.tid == "TID_BUILDING_LABORATORY" && (csvInfo.TID_Type == "CHARACTERS" || csvInfo.TID_Type == "SPELLS" || csvInfo.TID_Type == "TRAPS")){
					s.status = BuildStatus.Research;
					s.status_tid_level = status_tid_level;
				}else{
					s.status = BuildStatus.Upgrade;
				}
			}
			if (s.status == BuildStatus.New || s.status == BuildStatus.Upgrade
			    ||s.status == BuildStatus.Research || s.status == BuildStatus.Train||s.status == BuildStatus.CreateStatue){
				if (gems > 0 && Helper.SetResourceCount("Gems",-gems,false,true) == -1){
					//宝石不够;
					PopManage.Instance.ShowNeedGemsDialog();
				}else{
					//服务器还没有返回创建任务结束时,不能执行立即完成;
					//s.send_ing = true;

					ISFSObject data = new SFSObject();
                    //data.PutInt("current_time",current_time);		
                    //花费;
                    //data.PutInt("Gems",gems);
                    //if (is_instant){
                    //	data.PutInt("is_instant",1);
                    //}else{
                    //	data.PutInt("is_instant",0);
                    //}
                    //data.PutUtfString("status_tid_level",status_tid_level);

                    data.PutLong("id", s.building_id);	
					Debug.Log (s.building_id);
					data.PutInt ("type",0);//0 building 1 troop 2 spell;
					SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_SpeedUP, false, s.HandleResponse);

                    int exp_count = 0;
					if (s.status == BuildStatus.New){
						//计算经验值;取当前等级的XpGain
						exp_count = csvInfo.XpGain;// CalcHelper.calcTimeToExp(Helper.getBuildTime(csvInfo.TID_Level));
					}
					
					if (s.status == BuildStatus.Research){
						//计算经验值;取当前等级的XpGain
						exp_count = csvInfo.XpGain;//CalcHelper.calcTimeToExp(Helper.getUpgradeTime(csvInfo.TID_Level));
						//军队等级加1;
						//Debug.Log(csvInfo.TID + ";" + Helper.getTidMaxLevel(csvInfo.TID,1));

						DataManager.GetInstance.researchLevel[csvInfo.TID] = (int)(Helper.getTidMaxLevel(csvInfo.TID,1) + 1);						
					}
					
					//生产兵，立即完成,需要增加兵的数据;
					/*
					if (s.status == BuildStatus.Train && gems > 0){
						Helper.trainTroops(s,s.end_time);
						int train_time = Helper.getBuildTime(s.status_tid_level);
						//剩于时间;
						int LeftOnTime = current_time - s.end_time;
						if (LeftOnTime > 0){
							//剩于时间/每个兵生产所需时间 = 还剩可生成的兵数;
							int num = Mathf.CeilToInt(LeftOnTime*1f / (train_time*1f));
							
							CsvInfo csvTroops = CSVManager.GetInstance.csvTable[s.status_tid_level] as CsvInfo;
							//最大可装兵数量=总容量 除以 单个兵容量;
							int max_num = (int)(s.csvInfo.HousingSpace / csvTroops.HousingSpace);
							
							s.troops_tid = s.status_tid_level;
							s.troops_num = s.troops_num + num;
							if (s.troops_num > max_num){
								s.troops_num = max_num;
							}
						}
					}
					*/

					if (s.status == BuildStatus.Upgrade){

						//升级完成,更新模型及重新初始化数据;
						//比如：level + 1, 更新模型;
						s.level = s.level + 1;
						s.tid_level = s.tid + "_" + s.level;
						s.csvInfo = CSVManager.GetInstance.csvTable[s.tid_level] as CsvInfo;
						//计算经验值;还是取下个级别的XpGain
						exp_count = s.csvInfo.XpGain;//CalcHelper.calcTimeToExp(Helper.getUpgradeTime(s.tid_level));
					}	
					//Debug.Log(exp_count);
					if (exp_count > 0){

						AudioPlayer.Instance.PlaySfx("xp_gain_06");

						//升级可获得的经验值;
						int c = Mathf.CeilToInt(exp_count*1f/3f);
						s.emitter.enabled = true;
						s.emitter.Emit(c,s.transform.position,PartType.Exp,exp_count);

						//Helper.setResourceCount("Exp", exp_count);
					}
					
					//设置最大容量及主建筑物级别;
					if (s.status == BuildStatus.Upgrade || s.status == BuildStatus.New){
					
						AudioPlayer.Instance.PlaySfx("building_finished_01");
						s.status = BuildStatus.Normal;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)

						WorkerManager.GetInstance.SetWorkBuilding(null);
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
					}
					else
					{
						s.status = BuildStatus.Normal;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮)
					}


				}

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

	}

	public void RefreshBuild3DModel(string tid,int level)
	{
		DestroyImmediate (transform.Find ("buildPos/BuildMain").gameObject);

		
		string tid_level = tid + "_" + level;
		
		CsvInfo csvData = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;

		string buildSpritePath = "Model/Build3d/"+csvData.ExportName;

		GameObject buildSpriteInstance = null;		


		buildSpriteInstance = Instantiate(ResourceCache.load(buildSpritePath)) as GameObject;

		
		
		buildSpriteInstance.transform.parent = transform.Find ("buildPos");
		buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
		buildSpriteInstance.transform.localPosition = Vector3.zero; 
		buildSpriteInstance.transform.name = "BuildMain";
		this.buildSpritePath = buildSpritePath;

		if(csvData.TID=="TID_BUILDING_LANDING_SHIP")
		{			
			LandCraft lc = GetComponent<LandCraft>();
			lc.Init();
			lc.direct = Direct.UP;
			lc.InitTrooper(lc.currentTrooperTid,lc.currentNum);
		}

		if(csvData.TID=="TID_BUILDING_GUNSHIP")
		{			
			GunBoat gb = GetComponent<GunBoat>();
			gb.Init();
		}

		

		InitStandPoint ();
		InitBuild ();


		Transform[] t = buildSpriteInstance.GetComponentsInChildren<Transform> (true);
		for(int i =0;i<t.Length;i++)
		{
			t[i].gameObject.layer = 16;
		}

		
		if(isSelected)
			MoveOpEvent.Instance.SelectBuild (this);
		else
		{
			BuildTweener tweener = GetComponentInChildren<BuildTweener>();
			tweener.enabled  = true;
			tweener.Pslay ();
			tweener.Stop();
		}

	}

	//重新加载建筑模型;
	public void RefreshBuildModel(string tid,int level)
	{
		if(is3D)
		{
			RefreshBuild3DModel(tid,level);
			return;
		}

		if (status==BuildStatus.Create)
		{
			Transform buildnew = transform.Find ("buildPos/BuildNew");
			if(buildnew!=null)
			{
				buildnew.gameObject.SetActive(true);
				transform.Find ("buildPos/BuildMain").gameObject.SetActive(false);

			}
		}
		else
		{

			Transform buildnew = transform.Find ("buildPos/BuildNew");
			if(buildnew!=null&&buildnew.gameObject.activeSelf)
			{
				buildnew.gameObject.SetActive(false);
				transform.Find ("buildPos/BuildMain").gameObject.SetActive(true);
				
			}
			else
			{
				DestroyImmediate (transform.Find ("buildPos/BuildMain").gameObject);
				DestroyImmediate (transform.Find ("StandPoints").gameObject);


				string tid_level = tid + "_" + level;
				
				CsvInfo csvData = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;
				CsvInfo Lv1CsvData = CSVManager.GetInstance.csvTable[tid+"_1"] as CsvInfo;

				string buildSpPath = "Model/Build/" + csvData.ExportName+"_sp";
				string buildSpritePath = "Model/Build/" + csvData.ExportName;

				GameObject buildSpInstance = null;
				GameObject buildSpriteInstance = null;


				if(ResourceCache.load(buildSpritePath)==null)
				{
					buildSpritePath = "Model/Build/" + Lv1CsvData.ExportName;
					buildSpPath = "Model/Build/" + Lv1CsvData.ExportName+"_sp";
				}
				
				if(ResourceCache.load(buildSpritePath)==null)
				{
					buildSpritePath = "Model/Build/housing_lvl1";
					buildSpPath = "Model/Build/housing_lvl1_sp";
				}
				
				if(ResourceCache.load(buildSpPath)==null)
				{
					buildSpPath = "Model/Build/" + Lv1CsvData.ExportName+"_sp";
				}

				

				buildSpriteInstance = Instantiate(ResourceCache.load(buildSpritePath)) as GameObject;
				buildSpInstance = Instantiate(ResourceCache.load(buildSpPath)) as GameObject;
				

				buildSpriteInstance.transform.parent = transform.Find ("buildPos");
				buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
				buildSpriteInstance.transform.localPosition = Vector3.zero; 
				buildSpriteInstance.transform.name = "BuildMain";
				buildSpInstance.transform.parent = transform;
				buildSpInstance.transform.name = "StandPoints";
				buildSpInstance.transform.localPosition = Vector3.zero;

				if(status!=BuildStatus.Upgrade)
				{
					if(transform.Find("buildin")!=null)
						transform.Find("buildin").gameObject.SetActive(false);
				}

				this.buildSpritePath = buildSpritePath;
				InitStandPoint ();
			}
		}
		InitBuild ();

		if(isSelected)
			MoveOpEvent.Instance.SelectBuild (this);
		else
		{
			BuildTweener tweener = GetComponentInChildren<BuildTweener> ();
			tweener.enabled  = true;
			tweener.Pslay ();
			tweener.Stop();
		}
	}
	
	//===============================以上为：立即完成（新建，升级，生产，训练，移除)============================================================


	//===============================以下为：升级部分;升级只有2种，建筑物升级与军队（兵，兵器，地雷）===========================================================
	//升级只有2种，建筑物升级与军队（兵，兵器，地雷）;
	//当status_tid_level有值时，升级军队;反之升级建筑物;
	public void OnUpgradeBuild(BuildInfo s){
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[s.tid_level] as CsvInfo;
		if ("BUILDING".Equals(csvInfo.TID_Type) && CheckWookerCount() == false){			
			return;
		};
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		if (msg == null && s.status == BuildStatus.Normal){
			ISFSObject dt = Helper.getCostDiffToGems(s.tid_level,1,true);
			dt.PutLong("building_id",s.building_id);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
                //UIManager.Instance().msgCtrl.ShowPopWithDiamond1(dt.GetUtfString("title"), dt.GetUtfString("msg"), gems.ToString(), dt, s.building_id);
                UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(dt.GetUtfString("msg"), dt.GetUtfString("title"), gems.ToString(), PopDialogBtnType.ImageBtn, dt, OnDialogUpgradeBuildYes);
                /**
				PopManage.Instance.ShowDialog(
					dt.GetUtfString("msg"),
					dt.GetUtfString("title"),
					false,
					PopDialogBtnType.ImageBtn,
					true,
					dt,
					onDialogUpgradeYes,
					null,
					gems.ToString()
					);
                **/
            }
            else{
				AudioPlayer.Instance.PlaySfx("building_construct_07");
				//UpgradeBuildToServer(s,status_tid_level,dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),0);
				//BuildHandle.UpgradeBuild(s,dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),0);
			}
		}else{
			PopManage.Instance.ShowMsg(msg);
		}
	}
	
	private void OnDialogUpgradeBuildYes(ISFSObject dt,BuildInfo buildInfo = null){
		if (DataManager.GetInstance.model.user_info.diamond_count >= dt.GetInt("Gems")){
			//BuildInfo s = DataManager.GetInstance.BuildList[dt.GetLong("building_id")] as BuildInfo;
			//string status_tid_level = dt.GetUtfString("status_tid_level");
			AudioPlayer.Instance.PlaySfx("building_construct_07");
			//UpgradeBuildToServer(s,status_tid_level, dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"));
			//BuildHandle.UpgradeBuild (s,dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"));
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	public void OnUpgradeTech(BuildInfo s,string tid_level){
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		if (msg == null && s.status == BuildStatus.Normal){
			ISFSObject dt = Helper.getCostDiffToGems(tid_level,1,true);
			dt.PutLong("building_id",s.building_id);
			dt.PutUtfString("tid_level",tid_level);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog(dt.GetUtfString("msg"), dt.GetUtfString("title"), gems.ToString(), PopDialogBtnType.ImageBtn, dt, OnDialogUpgradeTechYes,true,s);
			}
			else{
				AudioPlayer.Instance.PlaySfx("building_construct_07");
				//BuildHandle.UpgradeTech(s,tid_level,dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),0);
			}
		}else{
			PopManage.Instance.ShowMsg(msg);
		}
	}

	private void OnDialogUpgradeTechYes(ISFSObject dt,BuildInfo buildInfo = null){
		if (DataManager.GetInstance.model.user_info.diamond_count >= dt.GetInt("Gems")){
			//BuildInfo s = DataManager.GetInstance.BuildList[dt.GetLong("building_id")] as BuildInfo;
			//string tid_level = dt.GetUtfString("tid_level");
			AudioPlayer.Instance.PlaySfx("building_construct_07");
			//UpgradeBuildToServer(s,status_tid_level, dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"));
			//BuildHandle.UpgradeTech (s,tid_level,dt.GetInt("Gold"),dt.GetInt("Wood"),dt.GetInt("Stone"),dt.GetInt("Iron"),dt.GetInt("Gems"));
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}

	public static void UpgradeBuildToServer(BuildInfo s, string status_tid_level, int gold = 0,int wood = 0,
	                                 int stone = 0,
	                                 int iron = 0,
	                                 int gems = 0){

		if (status_tid_level == null || status_tid_level == ""){
			status_tid_level = s.tid_level;
		}
		
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;

		string msg = null;
		if (s.tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && csvInfo.BuildingClass == "Artifact"){
			int artifact_num = Helper.GetArtifactNum();
			if (artifact_num > s.csvInfo.ArtifactCapacity){
				//TID_SELL_ARTIFACTS = Statue limit reached! Upgrade Sculptor to deploy more, or reclaim a statue.
				msg = StringFormat.FormatByTid("TID_SELL_ARTIFACTS");//"容量已满";
			}else{
				BuildCost bc = Helper.GetBuildCost(status_tid_level);
				if (Helper.SetResourceCount(bc.piece_type, -bc.piece,false,false) < 0){
					//TID_NOT_ENOUGH_ARTIFACT_PIECES = Not enough <name>!
					msg = StringFormat.FormatByTid("TID_NOT_ENOUGH_ARTIFACT_PIECES", new object[]{StringFormat.FormatByTid(Helper.GetNameToTid(bc.piece_type))});//"资源不足";
				}
			}
		}else{
			msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);
		}

		if (msg == null && s.status == BuildStatus.Normal){
				//扣除用户本地资源;
			Helper.SetResource(-gold,-wood,-stone,-iron,-gems,true);
			s.start_time = Helper.current_time();
			s.end_time = s.start_time + Helper.GetUpgradeTime(status_tid_level);		
			s.last_collect_time = s.end_time;//要到新建结束后，才能采集;

			if (s.tid == "TID_BUILDING_LABORATORY" && (csvInfo.TID_Type == "CHARACTERS" || csvInfo.TID_Type == "SPELLS" || csvInfo.TID_Type == "TRAPS")){
				s.status = BuildStatus.Research;//-1:客户端准备创建(建筑物会出现：取消,确定,按钮)
				s.status_tid_level = status_tid_level;
			}else if (s.tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && csvInfo.BuildingClass == "Artifact"){
				s.status = BuildStatus.CreateStatue;
				s.status_tid_level = status_tid_level;
				s.artifact_boost = 0;
				s.artifact_type = ArtifactType.None;
			}else{
				s.status = BuildStatus.Upgrade;

				if(s.transform.Find("buildin")!=null)
				{
					s.transform.Find("buildin").gameObject.SetActive(true);
				}
			}

			//服务器还没有返回创建任务结束时,不能执行立即完成;
			//send_ing = true;
			
			//ISFSObject data = new SFSObject();
			//data.PutUtfString("status_tid_level",status_tid_level);
			//花费;
			//data.PutInt("Gold",gold);
			//data.PutInt("Wood",wood);
			//data.PutInt("Stone",stone);
			//data.PutInt("Iron",iron);
			//data.PutInt("Gems",gems);
			//data.PutInt("start_time",s.start_time);
			//data.PutInt("end_time",s.end_time);
			//data.PutLong("building_id", s.building_id);
			//Debug.Log (s.building_id);
			//SFSNetworkManager.Instance.SendRequest(data,SFSNetworkManager.CMD_UpgradeBuilding, false, s.HandleResponse);

			//BuildHandle.SendBuildUpgradeToServer (s);


			//重新设置，所有建筑物的升级提醒标识;
			Helper.SetBuildUpgradeIcon();

			//重新计算版面数据;
			int count = Helper.CalcShopCates(false);
			//设置商城按钮数量;
			//ScreenUIManage.Instance.SetShopCount (count);
            UIManager.GetInstance.GetController<MainInterfaceCtrl>().SetShopCount(count);

            //设置工人工作;
            WorkerManager.GetInstance.SetWorkBuilding(Helper.getWorkBuilding());
		}else{
			if (msg != null){
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
			}
		}

		//关闭所有界面窗口;
		if(PopWin.current!=null)PopWin.current.CloseTween();
		//刷新按钮状态;
		s.buildUI.RefreshBuildBtn ();
		//PopManage.Instance.RefreshBuildBtn(s);
	}
	//===============================以上为：升级部分;升级只有2种，建筑物升级与军队（兵，兵器，地雷）===========================================================

	//==============================以下为：直接升级完成 部分 只有2种，建筑物升级与军队（兵，兵器，地雷）=============================================================
	//升级只有2种，建筑物升级与军队（兵，兵器，地雷）;
	//当status_tid_level有值时，升级军队;反之升级建筑物;
	//Status -1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;	
	public void InstantUpgrade(BuildInfo s, string status_tid_level = null){

		if (status_tid_level == null || status_tid_level == ""){
			status_tid_level = s.tid_level;
		}
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
		if ("BUILDING".Equals(csvInfo.TID_Type) && CheckWookerCount() == false){			
			return;
		}
		string msg = Helper.CheckHasUpgrade(csvInfo.TID, csvInfo.Level);		
		if (msg == null && s.status == BuildStatus.Normal){
			int gems = Helper.GetUpgradeInstant(csvInfo.TID_Level);
			if (DataManager.GetInstance.model.user_info.diamond_count >= gems){
				s.status_tid_level = status_tid_level;
				BuildHandle.ImmediateUpgradeBuild (s);
				//FinishBuildToServer(s,0, gems, true, status_tid_level);					
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog();
			}
		}else{
			if (msg != null)
                UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
		}			
	}

	//===============================以上为：直接升级完成 部分 只有2种，建筑物升级与军队（兵，兵器，地雷）==========================================================================

	//采集;
	//================================以下是：采集部分 begin ==================================================

	public static bool OnCollect(BuildInfo s){

		/*
		List<UnLockTid> UnTidList = Helper.getUpgradeUnLock();
		for(int i = 0; i < UnTidList.Count; i ++){
			UnLockTid UnLock = UnTidList[i];
			Debug.Log(UnLock.showName + " " + UnLock.value);
		}
		*/
		string res_type = "";
		PartType partType = PartType.Gold;
		if (s.tid == "TID_BUILDING_HOUSING"){
			partType = PartType.Gold;
			res_type = "Gold";
		}else if (s.tid == "TID_BUILDING_WOODCUTTER"){
			partType = PartType.Wood;
			res_type = "Wood";
		}else if (s.tid == "TID_BUILDING_STONE_QUARRY"){
			partType = PartType.Stone;
			res_type = "Stone";
		}else if (s.tid == "TID_BUILDING_METAL_MINE"){
			partType = PartType.Iron;
			res_type = "Iron";
		}
		//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
		int cStatus = Helper.CollectStatus(s);
		if (res_type != ""&&(cStatus==0||cStatus==1||cStatus==2)){
			int collect_time = Helper.current_time();
			int collect_num = Helper.getCollectNum(s,collect_time,false);
			if (collect_num > 0){
				int max_num = 0;
				if(res_type=="Gold")
				{
					max_num = DataManager.GetInstance.model.user_info.max_gold_count;
				}
				else if(res_type=="Wood")
				{
					max_num = DataManager.GetInstance.model.user_info.max_wood_count;
				}
				else if(res_type=="Stone")
				{
					max_num = DataManager.GetInstance.model.user_info.max_stone_count;
				}
				else if(res_type=="Iron")
				{
					max_num = DataManager.GetInstance.model.user_info.max_iron_count;
				}
				//collect_num = max_num;
				int c = 0;
				
				if(max_num>0)
				{
					c =  Mathf.CeilToInt(collect_num*100f/max_num);
				}
				
				if(c>15)c=15;
				
				int addC = Mathf.RoundToInt(collect_num/500f);
				if(addC>15)addC = 15;
				
				c+=addC;

				/*
				int c = 0;
				if(collect_num<10)
					c=1;
				else if(collect_num<50)
					c = 5;
				else if(collect_num<200)
					c = 10;
				else if(collect_num<1000)
					c = 20;
				else
					c = 30;
				*/

				//c = 20;
				s.emitter.enabled = true;
				s.emitter.Emit(c,s.transform.position,partType,collect_num);

				//Helper.setResourceCount(res_type,collect_num,false,true);
				s.last_collect_time = collect_time;

				ISFSObject data = new SFSObject();
				data.PutUtfString("res_type",res_type);
				data.PutInt("collect_num",collect_num);			
				data.PutInt("collect_time",collect_time);
				data.PutLong("building_id", s.building_id);
				SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_Collect, false, s.HandleResponse);
                if(s.buildUIManage!=null)
				    s.buildUIManage.SetCollectShader(true);
                if (s.buildUI != null)
                    s.buildUI.SetCollectShader();
                //重新设置，所有建筑物的升级提醒标识;
                Helper.SetBuildUpgradeIcon();
				return true;
			}
		}else if (cStatus==3){
			//TID_RESOURCE_PACK_LOCKED = 没有足够的储存空间;
			PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_RESOURCE_PACK_LOCKED"));
		}

		return false;
	}

	//================================以上是：采集部分 end==================================================

	//================================以下是：训练部分 begin==================================================

	/*
	//status_tid_level 需要训练的兵种;
	//s 登陆艇
	public static void OnStartTrain(BuildInfo s, string troops_tid){
		//TrainTid trainTid = Helper.getTrainTid(s);
		Dictionary<string,TrainTid> TrainTidList = Helper.getTrainTidList(s,troops_tid);
		TrainTid trainTid = TrainTidList[troops_tid];
		if (trainTid != null){
			ISFSObject dt = null;
			if (trainTid.trainCost > 0){
				dt = Helper.getCostDiffToGems(trainTid.tid_level,2,true, trainTid.trainCost);
			}else{
				dt = new SFSObject();
				dt.PutInt("Gems",0);
				dt.PutInt("Gold",trainTid.trainCost);
				dt.PutUtfString("msg","");
				dt.PutUtfString("title","");
			}
			dt.PutLong("building_id",s.building_id);
			dt.PutInt("train_time",trainTid.trainTime);
			dt.PutUtfString("status_tid_level",trainTid.tid_level);
			int gems = dt.GetInt("Gems");
			//资源不足，需要增加宝石才行;
			if (gems > 0){
				UIManager.Instance ().GetComponent<PopMsgCtrl>().ShowDialog (
					dt.GetUtfString ("msg"),
					dt.GetUtfString ("title"),
					gems.ToString (),
					PopDialogBtnType.ImageBtn,
					dt,
					s.onDialogTrainYes);
			}else{
				s.StartTrainToServer(s,trainTid.tid_level,trainTid.trainTime,dt.GetInt("Gold"),0);
			}
		}

	}

	private void onDialogTrainYes(ISFSObject dt,BuildInfo buildInfo = null){
		//Debug.Log("onDialogYes");
		//Debug.Log(dt.GetDump());
		if (DataManager.GetInstance.model.user_info.diamond_count >= dt.GetInt("Gems")){
			BuildInfo s = DataManager.GetInstance.BuildList[dt.GetLong("building_id")] as BuildInfo;
			//Debug.Log(dt.GetDump());
			StartTrainToServer(s,dt.GetUtfString("status_tid_level"), dt.GetInt("train_time"), dt.GetInt("Gold"),dt.GetInt("Gems"));
		}else{
			//宝石不够;
			PopManage.Instance.ShowNeedGemsDialog();
		}
	}
	
	public void StartTrainToServer(BuildInfo s, string status_tid_level, int train_time, int gold = 0, int gems = 0){

		if (s.status == BuildStatus.Normal){
			//扣除用户本地资源;
			if (gold >= 0){
				Helper.SetResource(-gold,0,0,0,-gems,true);
			}else{
				//负数时，退还差价;
				int collect_num = -gold;
				if (collect_num > 0){
					int max_num = DataManager.GetInstance.model.user_info.max_gold_count;
					int c = 0;
					if(max_num>0)
					{
						c =  Mathf.CeilToInt(collect_num*100f/max_num);
					}
					if(c>15)c=15;
					int addC = Mathf.RoundToInt(collect_num/500f);
					if(addC>15)addC = 15;
					c+=addC;
					s.emitter.enabled = true;
					s.emitter.Emit(c,s.transform.position,PartType.Gold,collect_num);
					//重新设置，所有建筑物的升级提醒标识;
					//Helper.SetBuildUpgradeIcon();
				}
			}
			//Debug.Log("status_tid_level:" + status_tid_level);

			CsvInfo csvInfo = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;

			s.start_time = Helper.current_time();
			s.troops_start_time = s.start_time;
			s.end_time = start_time + train_time;		
			s.last_collect_time = end_time;//要到新建结束后，才能采集;
			
			s.status = BuildStatus.Train;
			s.status_tid_level = csvInfo.TID_Level;
			if (s.troops_tid != csvInfo.TID){
				//清空军队里面的，旧的数量;
				s.troops_num = 0;
				s.GetComponent<LandCraft>().InitTrooper(s.troops_tid,s.troops_num);
			}
			s.troops_tid = csvInfo.TID;

			//服务器还没有返回创建任务结束时,不能执行立即完成;
			//s.send_ing = true;



			ISFSObject data = new SFSObject();
			
			data.PutUtfString("status_tid_level",status_tid_level);
			
			//花费;
			data.PutInt("Gold",gold);
			data.PutInt("Wood",0);
			data.PutInt("Stone",0);
			data.PutInt("Iron",0);
			data.PutInt("Gems",gems);
			
			data.PutInt("start_time",s.start_time);
			data.PutInt("end_time",s.end_time);
			data.PutLong("building_id", s.building_id);
			
			SFSNetworkManager.Instance.SendRequest(data, "start_train", false, HandleResponse);

			//重新设置，所有建筑物的升级提醒标识;
			Helper.SetBuildUpgradeIcon();
		}else{
			
		}
		
		//关闭所有界面窗口;
		if(PopWin.current!=null)PopWin.current.CloseTween();
		//刷新按钮状态;
		PopManage.Instance.RefreshBuildBtn(s);
	}
	*/

	//训练完成一个兵,每秒定时刷新;
	public void TrainFinishOneToServer(BuildInfo s){
		/*
		if (s.status == BuildStatus.Train){
			int current_time = Helper.current_time();
			if (current_time > s.end_time)
				current_time = s.end_time;

			int troops_num = Helper.trainTroops(s,current_time);
			//Debug.Log("troops_num:" + troops_num);

			if (troops_num > 0){
				ISFSObject data = new SFSObject();
				data.PutInt("current_time",current_time);
				data.PutInt("troops_num",troops_num);				
				data.PutLong("building_id", s.building_id);				
				SFSNetworkManager.Instance.SendRequest(data, "train_finish_one", false, HandleResponse);
			}
		}
		*/
	}

	//================================以上是：训练部分 end==================================================

	//========================================神像 处理部分 begin==========================================================
	
	//判断，当前建筑物上，是否有存在，还未 部署 神像;
	public bool hasStatue(){
		//Debug.Log(status.ToString() + ";tid:" + tid + ";status_tid_level:" + status_tid_level + ";artifact_type:" + artifact_type.ToString() + ";artifact_boost:" + artifact_boost);
		if (status == BuildStatus.Normal && tid == "TID_BUILDING_ARTIFACT_WORKSHOP" && status_tid_level != "" && status_tid_level != null){
			CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
			return csvArtiact.BuildingClass == "Artifact";
		}else{
			return false;
		}
	}
	
	//部署 一个新神像;
	public void OnDeployStatue(){
		if (hasStatue()){
			int artifact_num = Helper.GetArtifactNum();
			if (artifact_num < csvInfo.ArtifactCapacity){
				CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
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
					if (artifact_type == ArtifactType.None){
						//TID_ARTIFACT_WAIT_SERVER = 等待服务器返回雕像类型!;
						PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_ARTIFACT_WAIT_SERVER"));
					}else{
						//关闭所有界面窗口;
						if(PopWin.current!=null)PopWin.current.CloseTween();
                        BuildManager.CreateBuild(new BuildParam()
                        {
                            tid = csvArtiact.TID,
                            level = csvArtiact.Level
                        });
                        //当 部署 成功后，需要清空 TID_BUILDING_ARTIFACT_WORKSHOP 上的 status_tid_level
                    }
				}else{
					//同类型只能有一个大石像;
					//TID_SAME_EPIC_ARTIFACT = 同一奖励类型的大雕像，你只能拥有一个！要么你就回收旧的提供<bonus>奖励的大雕像，要么回收新的大雕像.;
					PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_SAME_EPIC_ARTIFACT", new object[]{epic_buildInfo.ShowLevelName}));
				}
			}else{
				//TID_SELL_ARTIFACTS = Statue limit reached! Upgrade Sculptor to deploy more, or reclaim a statue.
				PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_SELL_ARTIFACTS"));
			}
		}
	}
	
	//创建一个新的神像
	public void OnCreateStatue(BuildInfo s, string status_tid_level){
		//Debug.Log("OnCreateStatue:" + status_tid_level);
		
		if (s.status == BuildStatus.Normal){
					
			CsvInfo csvArtiact = CSVManager.GetInstance.csvTable[status_tid_level] as CsvInfo;
			
			int artifact_num = Helper.GetArtifactNum();
			//if (hasStatue()){
			//	artifact_num += 1;
			//}
			//s.csvInfo.ArtifactCapacity
			
			if (artifact_num <= s.csvInfo.ArtifactCapacity && csvArtiact.BuildingClass == "Artifact"){

				BuildCost bc = Helper.GetBuildCost(status_tid_level);
				//Debug.Log("TypePiece:" + bc.piece_type + ";csvInfo.PiecesNeeded:" + bc.piece);

				if (Helper.SetResourceCount(bc.piece_type, -bc.piece,true,false) == 1){
					UpgradeBuildToServer(s,status_tid_level,0,0,0,0,0);
				}else{
					//
					/*
					TID_NOT_ENOUGH_COMMON_ARTIFACT_PIECES = Not enough Life Shards!
						TID_NOT_ENOUGH_RARE_ARTIFACT_PIECES = Not enough Life Crystals!
							TID_NOT_ENOUGH_EPIC_ARTIFACT_PIECES = Not enough Life Essence!
					*/



					if (status_tid_level == "TID_BUILDING_ARTIFACT1_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_COMMON_ARTIFACT_PIECES"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT2_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_RARE_ARTIFACT_PIECES"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT3_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_EPIC_ARTIFACT_PIECES"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT1_ICE_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_COMMON_ARTIFACT_PIECES_ICE"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT2_ICE_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_RARE_ARTIFACT_PIECES_ICE"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT3_ICE_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_EPIC_ARTIFACT_PIECES_ICE"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT1_FIRE_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_COMMON_ARTIFACT_PIECES_FIRE"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT2_FIRE_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_RARE_ARTIFACT_PIECES_FIRE"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT3_FIRE_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_EPIC_ARTIFACT_PIECES_FIRE"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT1_DARK_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_COMMON_ARTIFACT_PIECES_DARK"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT2_DARK_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_RARE_ARTIFACT_PIECES_DARK"));
					}else if (status_tid_level == "TID_BUILDING_ARTIFACT3_DARK_1"){
                        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(StringFormat.FormatByTid("TID_NOT_ENOUGH_EPIC_ARTIFACT_PIECES_DARK"));
					}


					//PopManage.Instance.ShowMsg("资源不足");
				}
			}else{
				//TID_SELL_ARTIFACTS = Statue limit reached! Upgrade Sculptor to deploy more, or reclaim a statue.
				PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_SELL_ARTIFACTS"));
			}
		}
	}



	//========================================神像 处理部分 end===========================================

	//设置容量;
	public void SetStorage(int current,int max)
	{
		BuildStorage bs = GetComponentInChildren<BuildStorage> ();
		if(bs!=null)
		{
			float step = max*1f/4f;
			int idx = Mathf.RoundToInt(current/step);
			//Debug.Log(building_id);
			bs.SetSprite(idx );
		}
	}

	//========================================战斗使用的数据===========================================;

	/// <summary>
	/// 初始化战斗建筑;
	/// </summary>
	public void BattleInit()
	{
		IsDead = false;
		CurrentHitPoint = BattleHitpoint;
		if(DataManager.GetInstance.sceneStatus==SceneStatus.BATTLEREPLAY){
			//取从服务端下载回来的值;
			BattleID = GameLoader.Instance.BattleIDList[this.building_id];
		}else{
			BattleID =  BattleData.Instance.BuildIdx;
			BattleData.Instance.BuildIdx++;
		}

		if(buildCtl==null)
		buildCtl = BuildController.Instantiate (this);

		buildCtl.Init ();
		BuildUpdater upd = gameObject.GetComponent<BuildUpdater> ();
		if(upd==null)
		{
			upd = gameObject.AddComponent<BuildUpdater> (); 
		}
		upd.enabled = true;
		State = AISTATE.STANDING;
		AttackState = AISTATE.STANDING;
		IsFindDest = false;
		status = BuildStatus.Normal;
		IsInStun = false;
		IsInSmoke = false;
		StunProjectile = null;
	}

	public int BattleID; //攻击时的建筑对照ID;

	public int BattleHitpoint; //战斗时的血量(含加成);

	public float CurrentHitPoint;  //战斗时的实时血量;

	public int BattleDamage;	//战斗时的DPS（含加成）;

	public bool IsDead;  //是否已被摧毁;

	public BuildController  buildCtl; //建筑物的控制器;

	public AISTATE State;  //AI状态;

	public AISTATE AttackState;  //攻击状态;

	public CharInfo AttackCharInfo;  //攻击的角色;

	public float AttackRange{
		get{
			return csvInfo.AttackRange / 100f;
		}
	}  //攻击范围;

	//最小攻击范围;
	public float MinAttackRange{
		get{
			return csvInfo.MinAttackRange / 100f;
		}
	}

	//地雷的触发半径;
	public float TriggerRadius{
		get{
			return csvInfo.TriggerRadius / 100f;
		}
	}

	//攻击速度;
	public float AttackSpeed{
		get{
			return csvInfo.AttackSpeed / 1000f;
		}
	}

	public bool IsFindDest; //是否已找到目标;

	private bool _isStun;
	public bool IsInStun{
		get{
			return _isStun;
		}
		set{
			_isStun = value;
			if(value)
			{
				BuildTweener tweener = GetComponentInChildren<BuildTweener> ();
				if(tweener!=null)
				{
					tweener.enabled = true;
					tweener.PlayFlash();
				}
			}
			else
			{
				BuildTweener tweener = GetComponentInChildren<BuildTweener> ();
				if(tweener!=null)
				{
					tweener.Stop();
				}
			}
		}
	}     

	public ProjectileInfo StunProjectile;  

	public bool IsInSmoke;

	public float DamageRadius{
		get{
			return csvInfo.DamageRadius/100f;
		}
	}

	private bool _isShake;
	public bool isShake{
		set{
			_isShake = value;
		}
		get{
			return _isShake;
		}
	}

	private float _moveStep = 0.05f;
	public float moveStep{
		get{
			return _moveStep;
		}
	}

	private float _moveStepCount;
	public float moveStepCount{
		set{
			_moveStepCount = value;
		}
		get{
			return _moveStepCount;
		}
	}

	private float _shakeSpeed = 0.05f;
	public float shakeSpeed{
		get{
			return _shakeSpeed;
		}
	}

	private int _shakeDirect = 1;
	public int shakeDirect{
		set{
			_shakeDirect = value;
		}
		get{
			return _shakeDirect;
		}
	}

	private float _shakeTime = 0.1f;
	public float shakeTime{
		get{
			return _shakeTime;
		}
	}

	private float _shakeTimeCount;
	public float shakeTimeCount{
		get{
			return _shakeTimeCount;
		}
		set{
			_shakeTimeCount = value;
		}
	}


	//========================================战斗使用的数据 end========================================;


	public static BuildInfo loadFromBuildInfoCache(string tid_level)
	{
		BuildInfo buildInfo = null;
		if(DataManager.GetInstance.buildObjectPrefabList.ContainsKey(tid_level))
		{
			if(DataManager.GetInstance.buildObjectPrefabList[tid_level]!=null&&DataManager.GetInstance.buildObjectPrefabList[tid_level].Count>0)
			{
				buildInfo = DataManager.GetInstance.buildObjectPrefabList[tid_level].Dequeue();
				if(buildInfo!=null)
				{
					buildInfo.gameObject.SetActive(true);

					buildInfo.transform.Find("buildPos/BuildMain").gameObject.SetActive(true);
					Transform t = buildInfo.transform.Find("DestroySprite");
					if(t!=null)t.gameObject.SetActive(false);

					Transform buildnew = buildInfo.transform.Find("buildPos/BuildNew");
					if(buildnew!=null)buildnew.gameObject.SetActive(false);

					Transform buildin = buildInfo.transform.Find("buildin");
					if(buildin!=null)buildin.gameObject.SetActive(false);

					Transform UpdBrand = buildInfo.transform.Find("UpdBrand");
					if(UpdBrand!=null)UpdBrand.gameObject.SetActive(false);

					Transform Arrows = buildInfo.transform.Find("UI/Arrows");
					if(Arrows!=null)Arrows.gameObject.SetActive(false);

					Transform PopUpPanel = buildInfo.transform.Find("UI/PopUpPanel");
					if(PopUpPanel!=null)PopUpPanel.gameObject.SetActive(false);

					Transform buildRing = buildInfo.transform.Find("buildRing");
					if(buildRing!=null)
					{
						buildRing.transform.Find("buildInnerRing").gameObject.SetActive(false);
						buildRing.transform.Find("buildOuterRing").gameObject.SetActive(false);
					}

					Transform UIS = buildInfo.transform.Find("UI/UIS");
					foreach(Transform tt in UIS)
					{
						if(tt.name!="BuildBattleInfo")
						tt.gameObject.SetActive(false);
						else
						{
							tt.Find("Dps").gameObject.SetActive(false);
							tt.Find("Health").gameObject.SetActive(false);
						}
					}
				}
			}
		}
		return buildInfo;

	}

	public static void destroyAndCacheBuildInfo(BuildInfo b)
	{
		if(b!=null)
		{
			if(!DataManager.GetInstance.buildObjectPrefabList.ContainsKey(b.tid_level))
			{
				Queue<BuildInfo> objQueue = new Queue<BuildInfo>();
				objQueue.Enqueue(b);
				DataManager.GetInstance.buildObjectPrefabList.Add(b.tid_level,objQueue);
			}
			else
			{
				if(DataManager.GetInstance.buildObjectPrefabList[b.tid_level]==null)
					DataManager.GetInstance.buildObjectPrefabList[b.tid_level] = new Queue<BuildInfo>();
				DataManager.GetInstance.buildObjectPrefabList[b.tid_level].Enqueue(b);
			}
			b.gameObject.SetActive (false);
		}

	}

	void AddAmmo()
	{
		Globals.EnergyTotal+=3;
		AudioPlayer.Instance.PlaySfx("ammo_counter_01");
	}

}