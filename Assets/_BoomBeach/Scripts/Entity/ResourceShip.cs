using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class ResourceShip : MonoBehaviour {

	static Vector3 GoldShipStopPos = new Vector3(2,-1,-16);
	static Vector3 WoodShipStopPos = new Vector3(-1,-1,-16);
	static Vector3 StoneShipStopPos = new Vector3(-4,-1,-16);
	static Vector3 IronShipStopPos = new Vector3(-7,-1,-16);

	static Vector3 GoldShipBeginPos = new Vector3(2,-1,-31);
	static Vector3 WoodShipBeginPos = new Vector3(-1,-1,-31);
	static Vector3 StoneShipBeginPos = new Vector3(-4,-1,-31);
	static Vector3 IronShipBeginPos = new Vector3(-7,-1,-31);

	public static List<ResourceShip> ResourceShips = new List<ResourceShip>();

	public PartType partType;
	//public int resourceNum;
	public PartEmitter emitter;
	public UIButton resourceBtn;
	public UISprite resourceIco;

	//当前状态: 0:停靠 1:开进码头 2:开出码头;
	public int state;

	private float speed = 0.05f;


	public static void CreateShip(PartType i_partType)
	{
		string buildLayoutPath = "Model/Layout/ResourceShipLayout";
		string buildSpritePath = "Model/Build3d/resource_ship";
		GameObject buildLayoutInstance = Instantiate (ResourceCache.load (buildLayoutPath)) as GameObject;
		buildLayoutInstance.transform.parent = BuildManager.GetInstance.buildContainer;
		buildLayoutInstance.transform.name = i_partType.ToString()+"Ship";
		buildLayoutInstance.transform.localScale = Vector3.one;
		if(i_partType==PartType.Gold)
		{
			buildLayoutInstance.transform.position = GoldShipBeginPos;
		}
		else if(i_partType==PartType.Wood)
		{
			buildLayoutInstance.transform.position = WoodShipBeginPos;
		}
		else if(i_partType==PartType.Stone)
		{
			buildLayoutInstance.transform.position = StoneShipBeginPos;
		}
		else if(i_partType==PartType.Iron)
		{
			buildLayoutInstance.transform.position = IronShipBeginPos;
		}

		GameObject buildSpriteInstance = Instantiate (ResourceCache.load (buildSpritePath)) as GameObject;
		buildSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
		buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
		buildSpriteInstance.transform.localPosition = Vector3.zero; 
		buildSpriteInstance.transform.name = "BuildMain";

		buildLayoutInstance.layer = 16;
		Transform[] t = buildSpriteInstance.GetComponentsInChildren<Transform> (true);
		for(int i =0;i<t.Length;i++)
		{
			t[i].gameObject.layer = 16;
		}

		int resourceNum = Helper.getCollectNumByIsland(i_partType,0,false);
		if(resourceNum>0)
		{
			UISprite PopResourceArrow = buildLayoutInstance.transform.Find ("UI/UIS/PopResource/arrow").GetComponent<UISprite> ();
			UISprite PopResourceBox = buildLayoutInstance.transform.Find ("UI/UIS/PopResource/box").GetComponent<UISprite> ();

			int max_num = 0;
			int cur_num = 0;
			if (i_partType == PartType.Gold) {
				max_num = DataManager.GetInstance.model.user_info.max_gold_count;	
				cur_num = DataManager.GetInstance.model.user_info.gold_count;
			} else if (i_partType == PartType.Wood) {
				max_num = DataManager.GetInstance.model.user_info.max_wood_count;
				cur_num = DataManager.GetInstance.model.user_info.wood_count;
			} else if (i_partType == PartType.Stone) {
				max_num = DataManager.GetInstance.model.user_info.max_stone_count;
				cur_num = DataManager.GetInstance.model.user_info.stone_count;
			} else if (i_partType == PartType.Iron) {
				max_num = DataManager.GetInstance.model.user_info.max_iron_count;
				cur_num = DataManager.GetInstance.model.user_info.iron_count;
			}
			
			if (cur_num + resourceNum > max_num){
				//TID_RESOURCE_PACK_LOCKED = 没有足够的储存空间;
				//PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_RESOURCE_PACK_LOCKED"));
				PopResourceBox.color = Color.red;
				PopResourceArrow.color = Color.red;
			}else{
				//
				PopResourceBox.color = Color.white;
				PopResourceArrow.color = Color.white;
			}
		}

		ResourceShip resourceShip = buildLayoutInstance.GetComponent<ResourceShip> ();
		resourceShip.partType = i_partType;
		//resourceShip.resourceNum = i_resourceNum;
		resourceShip.emitter = buildLayoutInstance.transform.Find ("parts").GetComponent<PartEmitter> ();
		resourceShip.resourceBtn = buildLayoutInstance.transform.Find ("UI/UIS/PopResource").GetComponent<UIButton> ();
		resourceShip.resourceIco = buildLayoutInstance.transform.Find ("UI/UIS/PopResource/Sprite").GetComponent<UISprite> ();

		resourceShip.emitter.resourceShip = resourceShip;
		resourceShip.resourceBtn.onClick = new List<EventDelegate> ();
		resourceShip.resourceBtn.onClick.Add (new EventDelegate(resourceShip,"CollectResource"));
		resourceShip.resourceIco.spriteName = i_partType.ToString().ToLower()+"Ico";



		resourceShip.state = 1;  //初始化时就设为开进码头;




		ResourceShip.ResourceShips.Add (resourceShip);
	}

	public void CollectResource()
	{
		if(state==0)
		{
			int collect_time = Helper.current_time();
			int resourceNum = Helper.getCollectNumByIsland(partType,collect_time,false);
			if(resourceNum>0)
			{
				int max_num = 0;
				int cur_num = 0;
			
				if (partType == PartType.Gold) {
					max_num = DataManager.GetInstance.model.user_info.max_gold_count;	
					cur_num = DataManager.GetInstance.model.user_info.gold_count;
				} else if (partType == PartType.Wood) {
					max_num = DataManager.GetInstance.model.user_info.max_wood_count;
					cur_num = DataManager.GetInstance.model.user_info.wood_count;
				} else if (partType == PartType.Stone) {
					max_num = DataManager.GetInstance.model.user_info.max_stone_count;
					cur_num = DataManager.GetInstance.model.user_info.stone_count;
				} else if (partType == PartType.Iron) {
					max_num = DataManager.GetInstance.model.user_info.max_iron_count;
					cur_num = DataManager.GetInstance.model.user_info.iron_count;
				}

				if (cur_num + resourceNum > max_num){
					//TID_RESOURCE_PACK_LOCKED = 没有足够的储存空间;
					PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_RESOURCE_PACK_LOCKED"));
				}else{
					//collect_num = max_num;
					int c = 0;
					resourceNum = Helper.getCollectNumByIsland(partType,collect_time,true);

					if (max_num > 0) {
						c = Mathf.CeilToInt (resourceNum * 100f / max_num);
					}
			
					if (c > 15)
							c = 15;
			
					int addC = Mathf.RoundToInt (resourceNum / 500f);
					if (addC > 15)
							addC = 15;
			
					c += addC;
			
					emitter.enabled = true;
					emitter.Emit (c, transform.position, partType, resourceNum);
					resourceNum = 0;

					resourceBtn.gameObject.SetActive (false);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (state == 0)
						return;

		Vector3 BeginPos = Vector3.zero;
		Vector3 EndPos = Vector3.zero;
		if(partType==PartType.Gold)
		{
			BeginPos = GoldShipBeginPos;
			EndPos = GoldShipStopPos;
		}
		else if(partType==PartType.Wood)
		{
			BeginPos = WoodShipBeginPos;
			EndPos = WoodShipStopPos;
		}
		else if(partType==PartType.Stone)
		{
			BeginPos = StoneShipBeginPos;
			EndPos = StoneShipStopPos;
		}
		else if(partType==PartType.Iron)
		{
			BeginPos = IronShipBeginPos;
			EndPos = IronShipStopPos;
		}

		//开进码头;
		if(state==1)
		{
			if(Mathf.Approximately((transform.position - EndPos).magnitude,0f))
			{
				state = 0;
				resourceBtn.gameObject.SetActive(true);
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position,EndPos,speed*Globals.TimeRatio);
			}
		}
		//开出码头;
		if(state==2)
		{
			if(Mathf.Approximately((transform.position - BeginPos).magnitude,0f))
			{
				state = 0;
				if(ResourceShip.ResourceShips.Contains(this))
				{
					ResourceShip.ResourceShips.Remove(this);
				}
				Destroy(gameObject);
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position,BeginPos,speed*Globals.TimeRatio);
			}
		}
	}
}
