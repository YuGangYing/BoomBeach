using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//战斗数据用到的业务类;

//AI状态，控制器依赖状态机做出表现;
public enum AISTATE{
	STANDING = 1,
	FINDINGDEST = 2,  //正在搜索攻击目标;
	FINDINGPATH = 3,  //正在查找行进路线;
	MOVING = 4, 	  //此状态下必需有目的地或路径;
	ATTACKING = 5	  //此状态下必需有目标;
}

public enum EntityType{
	LandCraft=1,Trooper=2,Build=3,Weapon=4
}


//战斗查找时的比较类，用于比较最近的对象;
public class BattleFindItem:IComparable<BattleFindItem>
{
	public int distance;   //距离;
	public GameObject item;  //对象;
	public int CompareTo(BattleFindItem other)
	{
		return this.distance - other.distance;
	}	
}

//回放的数据节点;
public class ReplayNodeData:IComparable<ReplayNodeData>
{
	public ReplayNodeData()
	{
		TimeFromBegin = BattleData.Instance.TimeFromBegin;
		walkList = new WalkList ();
	}

	public int TimeFromBegin;    //开始后的时间(Time.delta*1000);
	public EntityType SelfType;  //当前数据的对象类型;
	public int SelfID;           //对象ID，用于识别相应的TID_LEVEL;
	public float StandX;		 //当前攻击时站立的x坐标;
	public float StandZ;		 //当前攻击时站立的z坐标;
	public float HitPoints;  	 //当前被扣血量;
	public int IsUnderAttack;    //是否当下正被攻击，1:显示血条;
	public float DestX;			 //用于寻路、攻击的目标点x;
	public float DestZ;			 //用于寻路、攻击的目标点z;
	public AISTATE State;  		 //当前的状态机;
	public AISTATE AttackState;  //当前的第二状态机(攻击专用的状态机);
	//public int IsRetreat;      //是否撤退0:否 1:是;
	public int IsInStun;		 //是否被冰冻 1冰住 -1解开冰冻;
	public int IsInSmoke;		 //是否被烟雾 1罩住 -1解开烟雾;
	public EntityType AttackType;//被攻击的目标类型;
	public int AttackID;		 //被攻击的目标ID;
	public WalkList walkList;  	 //行走过的路线格子;
	public int CompareTo(ReplayNodeData other)
	{
		return this.TimeFromBegin - other.TimeFromBegin;
	}	
}
//end战斗数据用到的业务类;

/// <summary>
/// 战斗的相关数据（包含回放数据列表与上传数据队列）;
/// </summary>
public class BattleData  
{
	private static BattleData instance;
	public static BattleData Instance
	{
		get{return instance; }
	}
	public static void Init()
	{
		if(BattleData.instance!=null&&BattleData.instance.flags!=null)
		{
			while(BattleData.Instance.flags.Count>0)
			{
				GameObject.Destroy(BattleData.Instance.flags.Dequeue());
			}
		}
		BattleData.instance = new BattleData ();
		BattleData.instance.flags = new Queue<GameObject> ();

		BattleData.instance.ReplayList = new BinaryHeap<ReplayNodeData> ();//回放相关
		BattleData.instance.BattleCommondQueue = new Queue<ReplayNodeData> ();//回放相关

		BattleData.instance.landCraftDic = new Dictionary<string,LandCraft>();//登陆舰
		BattleData.instance.trooperDic = new Dictionary<string, CharInfo> ();//士兵
		BattleData.instance.buildDic = new Dictionary<string, BuildInfo> ();//建筑
		BattleData.instance.buildList = new List<BuildInfo>();
		BattleData.instance.weaponBuildDic = new Dictionary<string, BuildInfo> ();//防御建筑
		BattleData.instance.trapDic = new Dictionary<string, BuildInfo> ();//地雷

		BattleData.instance.projectileCacheList = new Dictionary<string, Queue<ProjectileInfo>> ();
		BattleData.instance.effectCacheList = new Dictionary<string, Queue<EffectController>>();
		BattleData.instance.TrooperIdx = 0;
		BattleData.instance.LandCraftIdx = 0;
		//BattleData.instance.WeaponIdx = 0;
		BattleData.instance.BuildIdx = 0;
		BattleData.instance.TimeFromBegin = 0;
		BattleData.instance.DeadTrooperList = new List<CharInfo>();
		BattleData.instance.RetreatTrooperList = new List<CharInfo>();
		BattleData.instance.AllocateTrooperList = new List<CharInfo>();
		BattleData.instance.currentSelectBtd = null;
		BattleData.instance.BattleIsStart = false;
		BattleData.instance.BattleIsEnd = false;
		BattleData.instance.TrooperDeployed = false;
		BattleData.instance.BattleIsSuccess = false;
		BattleData.instance.AllTrooperRetreat = true;  //默认未出兵时，表时所有兵未撤退，一旦出兵改为false;
		BattleData.instance.BattleIsOver = false;
		BattleData.instance.BattleStartCountDown = 40f;
		BattleData.instance.BattleEndCountDown = 240f;
		BattleData.instance.isOnSuccessConroutine = false;
		BattleSceneUI.SetBattleUI ();

		Globals.overall_damage_level = 0;//当前建筑物被摧毁级别(1,2,3)


		IslandData islandData = Globals.SceneIsland.GetComponent<IslandData>();
		BattleData.instance.gunBoatDirect = islandData.GunBoatDirection;
		BattleData.instance.gunBoatPosition = islandData.GunBoatPostion;
		BeachData[] beachData = Globals.SceneIsland.GetComponentsInChildren<BeachData>(true);
		for(int i=0;i<beachData.Length;i++)
		{
			beachData[i].Init();
		}

		/*
		if (Globals.islandType == IslandType.playerbase) 
		{
			BattleData.instance.gunBoatDirect = Direct.RIGHT;	
			BattleData.instance.gunBoatPosition = new Vector3(-20,0,18);
			Globals.SceneIsland = GameObject.Find("Islands/"+Globals.islandType.ToString());
			BeachData[] beachData = Globals.SceneIsland.GetComponentsInChildren<BeachData>(true);
			for(int i=0;i<beachData.Length;i++)
			{
				beachData[i].Init();
			}
			//初始化海滩烽据;
		}else if (Globals.islandType == IslandType.mainland_a) 
		{
			BattleData.instance.gunBoatDirect = Direct.RIGHTUP;	
			BattleData.instance.gunBoatPosition = new Vector3(0,0,3);
			Globals.SceneIsland = GameObject.Find("Islands/"+Globals.islandType.ToString());
			BeachData[] beachData = Globals.SceneIsland.GetComponentsInChildren<BeachData>(true);
			for(int i=0;i<beachData.Length;i++)
			{
				beachData[i].Init();
			}
			//初始化海滩烽据;
		}else if (Globals.islandType == IslandType.mainland_b) 
		{
			BattleData.instance.gunBoatDirect = Direct.RIGHTUP;	
			BattleData.instance.gunBoatPosition = new Vector3(-3.6f,0,1.4f);
			Globals.SceneIsland = GameObject.Find("Islands/"+Globals.islandType.ToString());
			BeachData[] beachData = Globals.SceneIsland.GetComponentsInChildren<BeachData>(true);
			for(int i=0;i<beachData.Length;i++)
			{
				beachData[i].Init();
			}
			//初始化海滩烽据;
		}
		else if (Globals.islandType == IslandType.small_a) 
		{
			BattleData.instance.gunBoatDirect = Direct.UP;	
			BattleData.instance.gunBoatPosition = new Vector3(15.4f,0,-16.5f);
			Globals.SceneIsland = GameObject.Find("Islands/"+Globals.islandType.ToString());
			BeachData[] beachData = Globals.SceneIsland.GetComponentsInChildren<BeachData>(true);
			for(int i=0;i<beachData.Length;i++)
			{
				beachData[i].Init();
			}
			//初始化海滩烽据;
		}
		else if (Globals.islandType == IslandType.small_b) 
		{
			BattleData.instance.gunBoatDirect = Direct.RIGHT;	
			BattleData.instance.gunBoatPosition = new Vector3(-9.8f,0,12.7f);
			Globals.SceneIsland = GameObject.Find("Islands/"+Globals.islandType.ToString());
			BeachData[] beachData = Globals.SceneIsland.GetComponentsInChildren<BeachData>(true);
			for(int i=0;i<beachData.Length;i++)
			{
				beachData[i].Init();
			}
			//初始化海滩烽据;
		}
		*/
	}

	/// 回放数据列表;
	public BinaryHeap<ReplayNodeData> ReplayList;

	/// 用于战斗时实时的命令队列;
	public Queue<ReplayNodeData> BattleCommondQueue;

	/// 战斗时的登陆舰列表;
	public Dictionary<string,LandCraft> landCraftDic;

	/// 战斗时的士兵列表;
	public Dictionary<string,CharInfo> trooperDic;

	/// 战斗时的所有可被攻击建筑列表;
	public Dictionary<string,BuildInfo> buildDic;
	public List<BuildInfo> buildList;

	/// 战斗时用于导弹检测的地雷列表;
	public Dictionary<string,BuildInfo> trapDic;

	/// 战斗时的所有炮台建筑列表;
	public Dictionary<string,BuildInfo> weaponBuildDic;

	public Dictionary<string,Queue<ProjectileInfo>> projectileCacheList;

	public Dictionary<string,Queue<EffectController>> effectCacheList;

	/// 战斗时主堡;
	public BuildInfo TownHall;


	/// 战斗时的炮舰;
	public GunBoat gunBoat;

	/// 战斗时炮舰的位置;
	public Vector3 gunBoatPosition;

	/// 士兵ID的序号，初始化时用;
	public int TrooperIdx;

	/// 登陆舰ID的序号，初始化时用;
	public int LandCraftIdx;

	/// 武器的对应ID;根据武器TID在： GameLoad.initBattleTrooper 中直接生成了（现在只有6种);
	//public int WeaponIdx;

	/// 建筑物的对应ID;
	public int BuildIdx;

	//当前选中的出兵类型;
	public BattleTrooperData currentSelectBtd;

	public Queue<GameObject> flags;

	//离开始的时间;
	public int TimeFromBegin;

	//战斗是否开始;
	private bool _BattleIsStart;
	public bool BattleIsStart{
		set{ 
			_BattleIsStart = value; 
			if(value)
			BattleSceneUI.SetBattleUI ();
		}
		get{ return _BattleIsStart; }
	}

	//弹窗完成，全部结束;
	public bool BattleIsOver;
	//战斗结束;
	private bool _BattleIsEnd;
	public bool BattleIsEnd
	{
		set{ 
			_BattleIsEnd = value;
            return;
			/*
			if(value)
			{
                BattleSceneUI.SetBattleUI ();
				if(BattleData.Instance.BattleIsStart)
				{
					//显示战斗成果;
					if(BattleData.instance.BattleIsSuccess)
					{
                        if (ScreenUIManage.Instance != null)
                            ScreenUIManage.Instance.battleResultWin.ResultTitle = LocalizationCustom.instance.Get("TID_VICTORY");
					}
					else
					{
                        if (ScreenUIManage.Instance != null)
                            ScreenUIManage.Instance.battleResultWin.ResultTitle = LocalizationCustom.instance.Get("TID_DEFEAT");
					}	

				}
				else
				{
					Globals.LastSceneUserId = -1;
					Globals.LastSceneRegionsId = -1;
					UIButtonEvent.Instance.OnWorldIcoClick();
					AudioPlayer.Instance.PlayMusic("home_music");
				}
			}
			*/
		}
		get{ return _BattleIsEnd; }
	}

	/// <summary>
	/// 已出兵;
	/// </summary>
	public bool TrooperDeployed{
		set{ 
			_TrooperDeployed = value; 
			if(_TrooperDeployed)
			{
                if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.EndBattleLabel.text = LocalizationCustom.instance.Get("TID_SURRENDER_BUTTON");
			}
			else
			{
                if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.EndBattleLabel.text = LocalizationCustom.instance.Get("TID_END_BATTLE_BUTTON");
			}
		}
		get{
			return _TrooperDeployed;
		}
	}
	private bool _TrooperDeployed;

	/// 炮船方向;
	public Direct gunBoatDirect;

	public float BattleStartCountDown;  //开始倒计时;

	public float BattleEndCountDown;	  //结束倒计时;

	public bool BattleIsSuccess;     //战斗结果;

	public bool AllTrooperRetreat;  //是否所有兵已撤退;

	//已挂掉的兵;
	public List<CharInfo> DeadTrooperList;

	//已撤退的兵;
	public List<CharInfo> RetreatTrooperList;

	//派出的兵数;
	public List<CharInfo> AllocateTrooperList;

	public bool isOnSuccessConroutine;

}

