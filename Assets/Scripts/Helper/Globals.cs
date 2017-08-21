using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using BoomBeach;

//存放静态系统变量，切换场景不会丢失。
public static class Globals{

	public static SysConfig sysConfig;
	//public static ISFSObject battleData;//缓存服务器返回的战斗数据
	//public static ISFSObject playerData;//服务器缓存的玩家数据
	public static bool isLocalBattleData = false;//使用本地战斗数据
	public static bool isLocalHomeData = false;//是否用本地玩家数据
	public static string defaultLanguage = "Chinese";//默认语言
	public static bool isEnabelAutoLogin = false;//是否可以自动登录
	public static float sendingPeriod = 30f;//心跳包时间间隔
	public static string domain = "121.199.3.185";//linux服务器的内网IP : 192.168.180.177
	public static int port = 9933;
	public static string zone = "moba";
	public static int version = 1;
	public static Dictionary<string,Queue<BuildInfo>> buildObjectPrefabList = new Dictionary<string, Queue<BuildInfo>>();
	public static float obstacleAlpha = 0.3f;
	public static float baseTimeSpan = 0.02f;
	public static float TimeRatio{
		get{
			return Time.deltaTime/baseTimeSpan;
		}
	}

	public static long userId;
	public static int landCraftWidth = 3; //登陆舰宽;

	public static int regions_id = 0;//当前用户所在地图;0:为主地图；n:副本地图;




	public static int maxTownHallLevel = 20;//最大主堡等级;


	//岛类型;
	public static IslandType islandType = IslandType.playerbase;
	//当前的岛屿;
	public static GameObject SceneIsland;
    //游戏逻辑
    public static GameObject gameManager;

	public static Vector3[] landcraftPos = new Vector3[8]{
		new Vector3(0,0,0),
		new Vector3(4,0,0),
		new Vector3(8,0,0),
		new Vector3(12,0,0),
		new Vector3(0,0,-12),
		new Vector3(4,0,-12),
		new Vector3(8,0,-12),
		new Vector3(12,0,-12)
	};
	public static Vector3 dockPos = new Vector3(16f,-1f,-7f);
	public static Vector3 gunboatPos = new Vector3(8f,-1f,-10f);

	//public static int version = 6;//软件版本号;
	//public static int sys_type = 10;//10:app,ios;11:91 ios,12:pp ios,13:test ios; 14:com.fanwe.coc; 15:com.fanwe.clans 20:android; 打包时,里面要设置好,与fanwe_user.msg_type是一至的，使用标识软件类型;
	//public static string domain = "112.124.32.200";
    //public static string domain = "127.0.0.1";
    //public static bool is_wap_pay = false;
	//public static int scene_status = 0; //0:自己的场景; 1:攻击  2:世界地图;

	public static SceneStatus sceneStatus;  //当前场景状态;

	/*最后加载时间,从服务器返回;*/
	public static long lastLoadTime;
	
	/*服务器与客服端时差 =;*/
	public static long time_difference = 0;

	public static GridInfo[,] GridArray;
	public static int GridTotal = 50; //常量，建筑区域大小;
	public static int GridBuildCost = 0; //不可通行权重;
	public static int GridEmptyCost = 1;  //可通行权重;
	public static byte[,] mMatrix; //AStar寻路格子;

	//public static Hashtable BuildList;//当前场景建筑；
    //public static List<Models.BuildingData> buildings;
    //public static List<BuildInfo> BuildArray;  //所有可移动的建筑列表，不含树，石，船;  


	//public static Transform buildContainer;
	//public static Transform characterContainer;
	//public static Transform bulletContainer;

	//public static UserInfo userInfo = new UserInfo();
	//public static long userId;

	//public static Hashtable csvTable = new Hashtable(); //以tid_level 为索引,CsvInfo为类;
	//public static Hashtable researchLevel = new Hashtable();//用户实研室中可升级tid等级;

	public static int LastSceneUserId = -1;//最后加载的场景用户id;
	public static int LastSceneRegionsId = -1;//最后加载的d岛屿id;

	public static List<ShopCate> ShopCates = new List<ShopCate> ();//底部板块数据;

	public static IList<PopUI> popUIList; 

	//建筑物资源包缓存;
	//public static Dictionary<string,GameObject> resoucesCacheDict = new Dictionary<string, GameObject>();

	public static BuildInfo currentTrainBuildInfo;

	//存放了r0~r149 共150朵云; Regions //name 为key
	//public static Hashtable RegionsList = new Hashtable();

	//用户已经已开启的岛屿;UserRegions；regions_id 为key
	//public static Hashtable UserRegionsList = new Hashtable();

	//经验列表;;ExperienceLevels；name 为key即：经验等级;
	//public static Hashtable ExperienceLevelsList = new Hashtable();

	//成孰数据;
	//public static Hashtable AchievementsList = new Hashtable();

	//岛的不可通行区配置;
	//public static Dictionary<string,int[,]> island_grid_csv = new Dictionary<string,int[,]>(); 

	public static bool IsSceneLoaded = false;

	public static ArrayList LandCrafts;

	//世界地图中的被攻击列表数据;EnemyActivityItem
	public static Hashtable EnemyActivityList = new Hashtable();

	//搜索为攻击成本的倍率;
	public static float SearchCostFactor = 0.3f;

	//当前可以使用出战的军队;
	public static List<BattleTrooperData> battleTrooperList = new List<BattleTrooperData>();

	public static Dictionary<string,Projectiles> projectileData = new Dictionary<string, Projectiles> ();

	//对应globals的csv数据;
	public static Dictionary<string,GlobalsItem> GlobalsCsv = new Dictionary<string, GlobalsItem> ();

	//当前可以使用能量弹;
	public static List<BattleTrooperData> battleEnergyList = new List<BattleTrooperData>();
	//当前可以使用的能量弹总数;
	private static int energyTotal = 0;
	public static int EnergyTotal{
		get{
			return energyTotal;
		}
		set{
			energyTotal = value;
			//if(UIManager.Instance ().battleInterfaceCtrl!=null)
			UIManager.GetInstance.GetController<BattleInterfaceCtrl>().SetEnergy (energyTotal);
            /*
			//UIManager.Instance().battleInterfaceCtrl
			if(UIManager.Instance ().battleInterfaceCtrl!=null)
				UIManager.Instance ().battleInterfaceCtrl.SetEnergy (energyTotal);
			return;
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.battleWeaponList.totalWeapon = energyTotal;

			//更新所有的按钮可用度;
			int lengthb = BattleWeaponList.Instance.BattleItems.Count;		
			for(int i=0;i<lengthb;i++)
			{
				if(BattleWeaponList.Instance.BattleItems[i].WeaponCost > energyTotal
				   &&!BattleWeaponList.Instance.BattleItems[i].isDisabled)
				{
					BattleWeaponList.Instance.BattleItems[i].isDisabled = true;
				}

				if(BattleWeaponList.Instance.BattleItems[i].WeaponCost <= energyTotal
				   &&BattleWeaponList.Instance.BattleItems[i].isDisabled)
				{
					BattleWeaponList.Instance.BattleItems[i].isDisabled = false;
				}
			}

			int lengtha = BattleTrooperList.Instance.BattleItems.Count;		
			for(int i=0;i<lengtha;i++)
			{
				if(BattleTrooperList.Instance.BattleItems[i].WeaponCost > energyTotal
				   &&!BattleTrooperList.Instance.BattleItems[i].isDisabled)
				{
					BattleTrooperList.Instance.BattleItems[i].isDisabled = true;
				}
				if(!BattleTrooperList.Instance.BattleItems[i].isClicked
				   &&BattleTrooperList.Instance.BattleItems[i].WeaponCost>0
				   &&BattleTrooperList.Instance.BattleItems[i].WeaponCost <= energyTotal
				   &&BattleTrooperList.Instance.BattleItems[i].isDisabled)
				{
					BattleTrooperList.Instance.BattleItems[i].isDisabled = false;
				}
			}
			*/
		}
	}

	//可更新岛屿的时间间隔;
	public static int FindNewOpponentTime = 79200;//默认22小时，可更新一次(只能更新比自己级别高的或资源岛屿;
	public static int overall_damage_level;//当前建筑物被摧毁级别(1,2,3);

	//全局变量初始化;
	public static void Init()
	{
		Globals.IsSceneLoaded = false;
		//初始化士兵与建筑的容器;
		//Globals.buildContainer = GameObject.Find ("PBuilds").transform;
		//Globals.characterContainer = GameObject.Find ("PCharacters").transform;
		//Globals.bulletContainer = GameObject.Find ("PBullets").transform;
		if (Islands.GetInstance.useType)
		{
			Globals.islandType = Islands.GetInstance.type;
			Debug.Log("user island by test");
		}
		for(int i=0;i<Islands.GetInstance.islands.Length;i++)
		{
			if(Islands.GetInstance.islands[i].type==Globals.islandType)
			{
				GameObject mineLand = Islands.GetInstance.islands[i].gameObject;

                mineLand.SetActive(true);
                Globals.SceneIsland = mineLand;
			    Transform army_area_plane = mineLand.GetComponentInChildren<BeachData>().transform;
			    army_area_plane.GetComponent<MeshRenderer>().enabled = false;
			}
			else
			{
				Islands.GetInstance.islands[i].gameObject.SetActive(false);
			}
		}

		//初始化a星矩阵;
		mMatrix = new byte[64, 64]; 
		//初始化格子;
		Globals.GridArray = new GridInfo[Globals.GridTotal, Globals.GridTotal];
		for (int a=0; a<Globals.GridTotal; a++) 
		{
			for (int b=0; b<Globals.GridTotal; b++) 
			{
				GridInfo gridInfo = new GridInfo();
				gridInfo.A = a;
				gridInfo.B = b;
				gridInfo.GridPosition = new Vector3(a,0f,b);
				gridInfo.standPoint = new Vector3((float)a+0.5f,0f,(float)b+0.5f);
				gridInfo.isInArea = true;
				gridInfo.cost = Globals.GridEmptyCost;
				if(CSVManager.GetInstance.islandGridsDic[Globals.islandType.ToString()][a,b]==1)
				{
					gridInfo.isInArea = false;
					gridInfo.cost = Globals.GridBuildCost;
				}
				Globals.GridArray[a,b] = gridInfo; 
			}
		}		
		/*
		//清空成熟列表;
		AchievementsList.Clear();

		//用户已经已开启的岛屿;
		UserRegionsList.Clear();
		foreach(Regions r in Globals.RegionsList.Values){
			r.status = 0;//0:不可开启;1:已开启;2:可开启(但未开启);
			r.desc = null;
		}
*/
		//初始化建筑清单;
		if(MoveOpEvent.Instance.SelectedBuildInfo!=null)
		{
			MoveOpEvent.Instance.SelectedBuildInfo.ResetPosition (); 
			MoveOpEvent.Instance.UnDrawBuildPlan(MoveOpEvent.Instance.SelectedBuildInfo);
			MoveOpEvent.Instance.SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(true);
			MoveOpEvent.Instance.UnSelectBuild();
		}
		if(DataManager.GetInstance.buildList!=null)
		{
			UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop("需删除:"+DataManager.GetInstance.buildList.Count);
			foreach(BuildInfo b in DataManager.GetInstance.buildList.Values)
			{
				BuildInfo.destroyAndCacheBuildInfo(b);
			}
			/*
			int iii = 0;
			foreach(BuildInfo b in DataManager.GetInstance.BuildList.Values)
			{
				if(!b.gameObject.activeSelf)
					iii++;
			}
			MessageManage.Instance.ShowMessage("实际删除:"+iii);
			*/
		}
		WorkerManager.GetInstance.SetWorkBuilding (null);

		foreach(ResourceShip rs in ResourceShip.ResourceShips)
		{
			if(rs!=null)GameObject.Destroy(rs.gameObject);
		}
		ResourceShip.ResourceShips = new List<ResourceShip> ();
		DataManager.GetInstance.buildList = new Hashtable ();
		if(BattleData.Instance!=null&&BattleData.Instance.flags!=null)
		{
			while(BattleData.Instance.flags.Count>0)
			{
				GameObject.Destroy(BattleData.Instance.flags.Dequeue());
			}
		}
		//初始化建筑列表,随机获取用;
		//BuildArray = new List<BuildInfo> ();

		LandCrafts = new ArrayList ();

		//初始化提示的弹出;
		popUIList = new List<PopUI> ();
		//加载UI用的模型资料包;		

		//if (!Globals.resoucesCacheDict.ContainsKey("TestShopImage"))
		//	Globals.resoucesCacheDict.Add ("TestShopImage",Resources.Load("Test/ShopModel/TestShopImage") as GameObject);


		regions_id = 0;//当前用户所在地图;0:为主地图；n:副本地图;
		//scene_status = 0; //0:自己的场景; 1:攻击;
		//sceneStatus = SceneStatus.HOME;
		maxTownHallLevel = 20;//最大主堡等级;
		/*最后加载时间,从服务器返回;*/
		lastLoadTime = 0;
		
		/*服务器与客服端时差 =;*/
		time_difference = 0;
		//底部板块数据;
		ShopCates.Clear();
	}

	/* 获取任意一个坐标点所对应的GridInfo */
	public static GridInfo LocateGridInfo(Vector3 position)
	{
		int A = (int)position.x;
		int B = (int)position.z;
		GridInfo gridInfo = null;
		if(A>=0 && A<=Globals.GridArray.GetUpperBound(0)&&B>=0&&B<=Globals.GridArray.GetUpperBound(1))
		{
			gridInfo = Globals.GridArray[A,B];
		}
		else
		{
			gridInfo = new GridInfo();
			gridInfo.A = A;
			gridInfo.B = B;
			gridInfo.standPoint = new Vector3((float)A+0.5f,0f,(float)A+0.5f);
			gridInfo.isInArea = false;
			gridInfo.GridPosition = new Vector3(A,0,B);
		}

		return gridInfo;
	}

	//两点计算角度;
	public static float CaclDegree(Vector2 begin,Vector2 end)
	{
		Vector2 offset = end - begin;				
		float angle = 0;
		if (offset.magnitude > 0){
			angle = Mathf.Rad2Deg*Mathf.Asin (offset.y  /offset.magnitude);
			if(offset.x<0&&offset.y>=0) angle = 180 - angle;		
			if(offset.y<0)
			{
				if (offset.x <= 0){
					angle =  180 - angle;
				}else{
					angle =  360 + angle;
				}
			}
		}
		return angle;
	}

	//获取建筑的随机站立点;
	public static Vector3 GetRandBuildStandPointAroundBuild (BuildInfo buildInfo)
	{
		Vector3[] points = new Vector3[4]{new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0)};

		int p1Idx = UnityEngine.Random.Range(0,4);
		int p2Idx = p1Idx+1==4?0:p1Idx+1;
		Vector3 p1 =  buildInfo.Location+points[p1Idx]*buildInfo.GridCount*1f;
		Vector3 p2 =  buildInfo.Location+points[p2Idx]*buildInfo.GridCount*1f;


		//return p1;
		Vector2 bp = Vector2.zero;
		Vector2 ep = Vector2.zero;
		
		if(p1.x>p2.x)
		{
			bp = new Vector2(p2.x,p2.z);
			ep = new Vector2(p1.x,p1.z);
		}
		else
		{
			bp = new Vector2(p1.x,p1.z);
			ep = new Vector2(p2.x,p2.z);
		}
		
		
		Vector2 offset = ep - bp;
		float rx = UnityEngine.Random.Range(bp.x,ep.x);
		float ry = bp.y;
		if(offset.x!=0)
			ry =  ((rx-bp.x)*offset.y/offset.x)+bp.y;
		else
		{
			if(bp.y>ep.y)
			{
				ry = UnityEngine.Random.Range(ep.y,bp.y);
			}
			else
			{
				ry = UnityEngine.Random.Range(bp.y,ep.y);
			}
		}
		Vector3 dest = new Vector3(rx,0f,ry);
		
		
		//Debug.Log(ep+" "+bp+" "+offset+" "+dest);
		//string ss = "(("+rx+"-"+bp.x+")*"+offset.y+"/"+offset.x+")+"+bp.y;
		//Debug.Log(bp+" "+ep+" "+dest+" form:"+ss);
		return dest;
	}
		
    /// <summary>
    /// 在建筑周围随机选择一个站立点
    /// </summary>
    /// <param name="buildInfo"></param>
    /// <returns></returns>
	public static Vector3 GetRandStandPointAroundBuild(BuildInfo buildInfo,Transform trooper)
	{
		if(buildInfo.buildStandPoints != null&&buildInfo.buildStandPoints.Length>=3) 
		{
			int p1Idx = UnityEngine.Random.Range(0, buildInfo.buildStandPoints.Length);
			int p2Idx = p1Idx+1==buildInfo.buildStandPoints.Length?0:p1Idx+1;
			Vector3 p1 = buildInfo.buildStandPoints[p1Idx].position;
			Vector3 p2 = buildInfo.buildStandPoints[p2Idx].position;

			//return p1;
			Vector2 bp = Vector2.zero;
			Vector2 ep = Vector2.zero;

			if(p1.x>p2.x)
			{
				bp = new Vector2(p2.x,p2.z);
				ep = new Vector2(p1.x,p1.z);
			}
			else
			{
				bp = new Vector2(p1.x,p1.z);
				ep = new Vector2(p2.x,p2.z);
			}


			Vector2 offset = ep - bp;
			float rx = UnityEngine.Random.Range(bp.x,ep.x);//rx随机为相邻两点x分量之间的随机值
			float ry = bp.y;
			if(offset.x!=0)
			ry =  ((rx-bp.x)*offset.y/offset.x)+bp.y;//相似三角形
			else//如果offset垂直与y轴，ry为两个相邻的点的y轴从小到大的一个随机值
			{
				if(bp.y>ep.y)
				{
					ry = UnityEngine.Random.Range(ep.y,bp.y);
				}
				else
				{
					ry = UnityEngine.Random.Range(bp.y,ep.y);
				}
			}
			Vector3 dest = new Vector3(rx,0f,ry);


			//Debug.Log(ep+" "+bp+" "+offset+" "+dest);
			//string ss = "(("+rx+"-"+bp.x+")*"+offset.y+"/"+offset.x+")+"+bp.y;
			//Debug.Log(bp+" "+ep+" "+dest+" form:"+ss);
			return dest;



		}
		else //如果standPoints的个数小于3，则直接用建筑的位置
		{
			return buildInfo.Position;		
		}
	}


	//设置场景UI;
    //TODO
	public static void SetSceneUI()
	{
        /*
        if(ScreenUIManage.Instance==null)
            return;
		GameObject ScreenUIRoot = GameObject.Find ("UI Root (2D)");
        if (ScreenUIRoot == null)
            return;
		SceneUI[] sceneUIs = ScreenUIRoot.GetComponentsInChildren<SceneUI> (true);
		for(int i=0;i<sceneUIs.Length;i++)
		{
			sceneUIs[i].setStatus();
		}
        GameObject scene = GameObject.Find ("Islands");
		SceneUI[] sceneUIs2 = scene.GetComponentsInChildren<SceneUI> (true);
		for(int i=0;i<sceneUIs2.Length;i++)
		{
			sceneUIs2[i].setStatus();
		}
         if (ScreenUIManage.Instance != null)
            ScreenUIManage.Instance.battleResultWin.HideResultWin ();
        */

        if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE||DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY)
		{
            gameManager.AddComponent<AITask>();
            //GameObject.Find ("GameManage").AddComponent<AITask>();
		}
		else
		{
            GameObject.Destroy(gameManager.GetComponent<AITask>());
            //GameObject.Destroy(GameObject.Find ("GameManage").GetComponent<AITask>());
		}
	}

	//获取圆内随机点;
	public static Vector3 GetRandPointInCircle(Vector3 point,float radius)
	{
		float randRadius = UnityEngine.Random.Range (0f,radius);
		float randAngle = UnityEngine.Random.Range (0f,360f);
		float rsin = Mathf.Sin (Mathf.Deg2Rad * randAngle);
		float z = randRadius * rsin;

		float rcos = Mathf.Cos (Mathf.Deg2Rad * randAngle);
		float x = randRadius * rcos;

		Vector3 offset = new Vector3 (x,0,z);
		return point + offset;

	}

	public static string GetDirectionString(Direct direct)
	{
		string dirstr = "";
		if(direct==Direct.UP)
			dirstr = "UP";
		else if(direct==Direct.DOWN)
			dirstr = "DOWN";
		else if(direct==Direct.LEFT)
			dirstr = "LEFT";
		else if(direct==Direct.RIGHT)
			dirstr = "RIGHT";
		else if(direct==Direct.LEFTUP)
			dirstr = "LEFTUP";
		else if(direct==Direct.LEFTDOWN)
			dirstr = "LEFTDOWN";
		else if(direct==Direct.RIGHTUP)
			dirstr = "RIGHTUP";
		else if(direct==Direct.RIGHTDOWN)
			dirstr = "RIGHTDOWN";
		return dirstr;
	}


	public static Dictionary<string,Dictionary<Direct,string>> charAnims;

	public static void InitCharAnimDic(){
		charAnims = new Dictionary<string, Dictionary<Direct, string>> ();
		charAnims.Add ("Walk",GetAnimNames("Walk"));
		charAnims.Add ("Attack",GetAnimNames("Attack"));
		charAnims.Add ("Stand",GetAnimNames("Stand"));
	}

	public static void PlayTk2dAnim(string type,Direct dir,tk2dSpriteAnimator anim){
		if (charAnims == null)
			InitCharAnimDic ();
		anim.Play (charAnims[type][dir]);
		anim.ClipFps = 30;
	}

	public static Dictionary<Direct,string> GetAnimNames(string type){
		Dictionary<Direct, string> animClips = new Dictionary<Direct, string> ();
		animClips.Add (Direct.UP,type + Globals.GetDirectionString(Direct.RIGHT));
		animClips.Add (Direct.LEFT,type + Globals.GetDirectionString(Direct.DOWN));
		animClips.Add (Direct.LEFTDOWN,type + Globals.GetDirectionString(Direct.RIGHTUP));
		animClips.Add (Direct.LEFTUP,type + Globals.GetDirectionString(Direct.RIGHTDOWN));
		animClips.Add (Direct.RIGHT,type + Globals.GetDirectionString(Direct.RIGHT));
		animClips.Add (Direct.RIGHTDOWN,type + Globals.GetDirectionString(Direct.RIGHTDOWN));
		animClips.Add (Direct.RIGHTUP,type + Globals.GetDirectionString(Direct.RIGHTUP));
		animClips.Add (Direct.DOWN,type + Globals.GetDirectionString(Direct.DOWN));
		return animClips;
	}


	/// <summary>
	/// 平面直角坐标系转换;
	/// OriginCoorDinate：原始坐标,Degree: 转角（逆时针为正）; 
    /// 行向量左乘矩阵
	/// </summary>
	public static Vector2 TransformCoordinate(Vector2 OriginCoorDinate,float Degree)
	{
		//x=x'cost-y'sint+x0,  y=x'sint+y'cost+y0.
		float rad = Degree * Mathf.Deg2Rad;
		float x = OriginCoorDinate.x * Mathf.Cos (rad) - OriginCoorDinate.y * Mathf.Sin(rad) ;
		float y = OriginCoorDinate.x * Mathf.Sin (rad) + OriginCoorDinate.y * Mathf.Cos(rad) ;

		return new Vector2 (x,y);
	}



	public static void HandleLoadWorldEnd()
	{
		//先执行完加载后的操作，再运行委拖;
		//CameraOpEvent.Instance.Reset (new Vector3(-10,30,0),15f);
		//Debug.Log("先执行完加载后的操作，再运行委拖");
	}


	//场景加载完成
	public delegate void OnLoadEnd();
	public static void HandleLoadEnd(OnLoadEnd onLoadEnd)
	{
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
		SceneLoader.Instance.endLoad = onLoadEnd;
		SceneLoader.Instance.EndLoad();
		//if(onLoadEnd!=null)onLoadEnd();
	}

	public delegate void OnLoadBegin();
	public static void HandleLoadBegin(OnLoadBegin onLoadBegin, bool play_cloud = true)
	{
		AudioPlayer.Instance.PlayMusic("");
		//先执行加载前的操作，再运行委拖;
		//Debug.Log("先执行加载前的操作，再运行委拖1");

		if(!play_cloud)
		{
			CameraOpEvent.Instance.ResetIm (new Vector3(-10,30,0));
			if(onLoadBegin!=null)onLoadBegin();
		}
		else
		{
			SceneLoader.Instance.beginLoad = onLoadBegin;
			SceneLoader.Instance.BeginLoad();
		}
	}

}
