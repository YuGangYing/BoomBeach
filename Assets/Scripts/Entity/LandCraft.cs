using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;


public class LandCraft : MonoBehaviour {
	
	public BattleTrooperData btd;
	public LandCraftStand[] heavys;
	public LandCraftStand[] riflemans;
	public LandCraftStand[] tanks;
	public LandCraftStand[] warriors;
	public LandCraftStand[] zookas;
	public LandCraftStand[] troopers;

	public string currentTrooperTid;
	public int currentNum;

	private GameObject upModel;
	private GameObject rightModel;
	private GameObject rightupModel;

	private GameObject upStands;
	private GameObject rightStands;
	private GameObject rightupStands;

	private BuildInfo buildInfo;

	public BeachData beachData;  //登录的海滩数据;

	//方向;
	private Direct pdirect;
	public Direct direct{
		get{ return pdirect; }
		set{
			pdirect = value;

			if(pdirect==Direct.RIGHT)
			{
				rightModel.SetActive(true);
				upModel.SetActive(false);
				rightupModel.SetActive(false);

				heavys = rightStands.transform.Find ("heavys").GetComponentsInChildren<LandCraftStand> (true);
				riflemans = rightStands.transform.Find ("riflemans").GetComponentsInChildren<LandCraftStand> (true);
				tanks = rightStands.transform.Find ("tanks").GetComponentsInChildren<LandCraftStand> (true);
				warriors = rightStands.transform.Find ("warriors").GetComponentsInChildren<LandCraftStand> (true);
				zookas = rightStands.transform.Find ("zookas").GetComponentsInChildren<LandCraftStand> (true);

			}
			else if(pdirect==Direct.UP)
			{
				upModel.SetActive(true);
				rightModel.SetActive(false);
				rightupModel.SetActive(false);
				
				heavys = upStands.transform.Find ("heavys").GetComponentsInChildren<LandCraftStand> (true);
				riflemans = upStands.transform.Find ("riflemans").GetComponentsInChildren<LandCraftStand> (true);
				tanks = upStands.transform.Find ("tanks").GetComponentsInChildren<LandCraftStand> (true);
				warriors = upStands.transform.Find ("warriors").GetComponentsInChildren<LandCraftStand> (true);
				zookas = upStands.transform.Find ("zookas").GetComponentsInChildren<LandCraftStand> (true);

			}
			else if(pdirect==Direct.RIGHTUP)
			{
				rightupModel.SetActive(true);
				upModel.SetActive(false);
				rightModel.SetActive(false);
				
				heavys = rightupStands.transform.Find ("heavys").GetComponentsInChildren<LandCraftStand> (true);
				riflemans = rightupStands.transform.Find ("riflemans").GetComponentsInChildren<LandCraftStand> (true);
				tanks = rightupStands.transform.Find ("tanks").GetComponentsInChildren<LandCraftStand> (true);
				warriors = rightupStands.transform.Find ("warriors").GetComponentsInChildren<LandCraftStand> (true);
				zookas = rightupStands.transform.Find ("zookas").GetComponentsInChildren<LandCraftStand> (true);
				
			}
		}
	}

	public void Init()
	{
		upModel = transform.Find ("buildPos/BuildMain/model/UP").gameObject;
		rightModel = transform.Find ("buildPos/BuildMain/model/RIGHT").gameObject;
		rightupModel = transform.Find ("buildPos/BuildMain/model/RIGHTUP").gameObject;
		buildInfo = GetComponent<BuildInfo> ();
		if(upStands!=null)
			DestroyImmediate (upStands);
		if(rightStands!=null)
			DestroyImmediate (rightStands);
		if(rightupStands!=null)
			DestroyImmediate (rightupStands);
		if(ResourceCache.Load(buildInfo.buildSpritePath+"_trooper_UP")!=null)
		{
			upStands = Instantiate(ResourceCache.Load(buildInfo.buildSpritePath+"_trooper_UP")) as GameObject;
			upStands.transform.parent = transform.Find("TroopersStand");
			upStands.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
			upStands.transform.localPosition = Vector3.zero; 
			upStands.transform.name = "StandsUP";
		}
		if(ResourceCache.Load(buildInfo.buildSpritePath+"_trooper_RIGHT")!=null)
		{
			rightStands = Instantiate(ResourceCache.Load(buildInfo.buildSpritePath+"_trooper_RIGHT")) as GameObject;
			rightStands.transform.parent = transform.Find("TroopersStand");
			rightStands.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
			rightStands.transform.localPosition = Vector3.zero; 
			rightStands.transform.name = "StandsRIGHT";
		}
		if(ResourceCache.Load(buildInfo.buildSpritePath+"_trooper_RIGHTUP")!=null)
		{
			rightupStands = Instantiate(ResourceCache.Load(buildInfo.buildSpritePath+"_trooper_RIGHTUP")) as GameObject;
			rightupStands.transform.parent = transform.Find("TroopersStand");
			rightupStands.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
			rightupStands.transform.localPosition = Vector3.zero; 
			rightupStands.transform.name = "StandsRIGHTUP";
		}
		troopers = transform.GetComponentsInChildren<LandCraftStand> ();
		if(DataManager.GetInstance.sceneStatus!=SceneStatus.ENEMYBATTLE&&DataManager.GetInstance.sceneStatus!=SceneStatus.BATTLEREPLAY)
		{
			Animation[] anis = GetComponentsInChildren<Animation> ();
			for(int i=0;i<anis.Length;i++)
				anis[i].wrapMode = WrapMode.Loop;
		}
	}

	/*
	void Awake () {
		heavys = transform.Find ("Troopers/heavys").GetComponentsInChildren<LandCraftStand> ();
		riflemans = transform.Find ("Troopers/riflemans").GetComponentsInChildren<LandCraftStand> ();
		tanks = transform.Find ("Troopers/tanks").GetComponentsInChildren<LandCraftStand> ();
		warriors = transform.Find ("Troopers/warriors").GetComponentsInChildren<LandCraftStand> ();
		zookas = transform.Find ("Troopers/zookas").GetComponentsInChildren<LandCraftStand> ();
		troopers = transform.GetComponentsInChildren<LandCraftStand> ();
		animation.wrapMode = WrapMode.Loop;


	}
	*/

	//red/green;
	public void setLight(string light)
	{
		GameObject craftModel = null;
		if(pdirect==Direct.UP)
		{
			craftModel = upModel;
		}
		else if(pdirect==Direct.RIGHT)
		{
			craftModel = rightModel;
		}
		else if(pdirect==Direct.RIGHTUP)
		{
			craftModel = rightupModel;
		}
		if(craftModel!=null)
		{
			if(light=="red")
			{
				craftModel.transform.Find("boatlight").gameObject.SetActive(true);
			}
			else
			{
				craftModel.transform.Find("boatlight").gameObject.SetActive(false);
			}
		}
	}

	//up/down;
	public void setDeck(string deck)
	{
		GameObject craftModel = null;
		if(pdirect==Direct.UP)
		{
			craftModel = upModel;
		}
		else if(pdirect==Direct.RIGHT)
		{
			craftModel = rightModel;
		}
		else if(pdirect==Direct.RIGHTUP)
		{
			craftModel = rightupModel;
		}
		
		if(craftModel!=null)
		{
			if(deck=="down")
			{
				craftModel.transform.Find("deckdown").gameObject.SetActive(true);
				craftModel.transform.Find("deckup").gameObject.SetActive(false);
			}
			else
			{
				craftModel.transform.Find("deckup").gameObject.SetActive(true);
				craftModel.transform.Find("deckdown").gameObject.SetActive(false);
			}
		}
	}

	public void InitTrooper(string tid,int num)
	{
		currentTrooperTid = tid;
		currentNum = num;
		TrooperType trooperType = TrooperType.Heavy;
		if(tid=="TID_RIFLEMAN")
			trooperType = TrooperType.Rifleman;
		else if(tid=="TID_ZOOKA")
			trooperType = TrooperType.Zooka;
		if(tid=="TID_WARRIOR")
			trooperType = TrooperType.Warrior;
		if(tid=="TID_HEAVY")
			trooperType = TrooperType.Heavy;
		if(tid=="TID_TANK")
			trooperType = TrooperType.Tank;

		for(int i=0;i<troopers.Length;i++)
		{
			Transform trooper = troopers[i].transform.Find("trooper");
			if(trooper!=null)
				DestroyImmediate(trooper.gameObject);
		}
		LandCraftStand[] trooperStands = null;
		GameObject trooperPrefab = null;
		if(trooperType == TrooperType.Heavy)
		{
			trooperStands = heavys;
			trooperPrefab = ResourceCache.Load("Model/Character/heavys") as GameObject;
		}
		else if(trooperType == TrooperType.Rifleman)
		{
			trooperStands = riflemans;
			trooperPrefab = ResourceCache.Load("Model/Character/rifleman") as GameObject;
		}
		else if(trooperType == TrooperType.Tank)
		{
			trooperStands = tanks;
			trooperPrefab = ResourceCache.Load("Model/Character/tank") as GameObject;
		}
		else if(trooperType == TrooperType.Warrior)
		{
			trooperStands = warriors;
			trooperPrefab = ResourceCache.Load("Model/Character/warrior") as GameObject;
		}
		else if(trooperType == TrooperType.Zooka)
		{
			trooperStands = zookas;
			trooperPrefab = ResourceCache.Load("Model/Character/zooka") as GameObject;
		}
		Debug.Log ("num:"+num);
		for(int i=0;i<num;i++)
		{
			//TODO 这里可以用对象池
			GameObject trooperInstant = Instantiate(trooperPrefab) as GameObject;
			trooperInstant.name = "trooper";
			int index = i;
			if (trooperStands.Length <= i) {
				index = Random.Range (0,trooperStands.Length);//TODO
			}
			trooperInstant.transform.parent = trooperStands [index].transform;
			trooperInstant.transform.localScale = Vector3.one;
			trooperInstant.transform.localPosition = Vector3.zero;
		}

		TrainTid tt = Helper.GetTrainTid(buildInfo);
		if (tt == null || tt.trainNum > 0){
			setDeck("down");
		}else{
			setDeck("");
		}

		if (num == 0){
			setLight("red");
		}else{
			setLight("");
		}
	}

	public Dictionary<string,CharInfo> TrooperList;

	public void CreateTrooper(BattleTrooperData btd)
	{
		string tid = btd.tid;
		TrooperType trooperType = TrooperType.Heavy;
		if(tid=="TID_RIFLEMAN")
			trooperType = TrooperType.Rifleman;
		else if(tid=="TID_ZOOKA")
			trooperType = TrooperType.Zooka;
		if(tid=="TID_WARRIOR")
			trooperType = TrooperType.Warrior;
		if(tid=="TID_HEAVY")
			trooperType = TrooperType.Heavy;
		if(tid=="TID_TANK")
			trooperType = TrooperType.Tank;
		
		for(int i=0;i<troopers.Length;i++)
		{
			Transform trooper = troopers[i].transform.Find("trooper");
			if(trooper!=null)
				DestroyImmediate(trooper.gameObject);
		}
		GameObject trooperPrefab = null;
		if(trooperType == TrooperType.Heavy)
		{
			trooperPrefab = ResourceCache.Load("Model/Character/heavys") as GameObject;
		}
		else if(trooperType == TrooperType.Rifleman)
		{
			trooperPrefab = ResourceCache.Load("Model/Character/rifleman") as GameObject;
		}
		else if(trooperType == TrooperType.Tank)
		{
			trooperPrefab = ResourceCache.Load("Model/Character/tank") as GameObject;
		}
		else if(trooperType == TrooperType.Warrior)
		{
			trooperPrefab = ResourceCache.Load("Model/Character/warrior") as GameObject;
		}
		else if(trooperType == TrooperType.Zooka)
		{
			trooperPrefab = ResourceCache.Load("Model/Character/zooka") as GameObject;
		}
		
		for(int i=0;i<btd.num;i++)
		{
				GameObject trooperInstant = Instantiate(trooperPrefab) as GameObject;
				trooperInstant.transform.parent = transform;
				CharInfo charInfo = trooperInstant.GetComponent<CharInfo>();
				charInfo.trooperData = btd;
				charInfo.BattleInit();
				TrooperList.Add(charInfo.Id.ToString(),charInfo);
				BattleData.Instance.trooperDic.Add(charInfo.Id.ToString(),charInfo);
		}

	}

	public void initTrooperStand()
	{
		string tid = btd.tid;
		TrooperType trooperType = TrooperType.Heavy;
		if(tid=="TID_RIFLEMAN")
			trooperType = TrooperType.Rifleman;
		else if(tid=="TID_ZOOKA")
			trooperType = TrooperType.Zooka;
		if(tid=="TID_WARRIOR")
			trooperType = TrooperType.Warrior;
		if(tid=="TID_HEAVY")
			trooperType = TrooperType.Heavy;
		if(tid=="TID_TANK")
			trooperType = TrooperType.Tank;

		LandCraftStand[] trooperStands = null;
		if(trooperType == TrooperType.Heavy)
		{
			trooperStands = heavys;
		}
		else if(trooperType == TrooperType.Rifleman)
		{
			trooperStands = riflemans;
		}
		else if(trooperType == TrooperType.Tank)
		{
			trooperStands = tanks;
		}
		else if(trooperType == TrooperType.Warrior)
		{
			trooperStands = warriors;
		}
		else if(trooperType == TrooperType.Zooka)
		{
			trooperStands = zookas;
		}

		int i = 0;
		foreach(CharInfo charInfo in TrooperList.Values)
		{
			GameObject trooperInstant = charInfo.gameObject;
			if(pdirect==Direct.RIGHT)
			{
				if(trooperType == TrooperType.Tank)
					trooperInstant.transform.Find("characterPos/ani").GetComponent<tk2dSpriteAnimator>().Play("StandUP");		
				else
					trooperInstant.transform.Find("characterPos/ani").GetComponent<tk2dSpriteAnimator>().Play("StandRIGHT");		
			}	
			else if(pdirect==Direct.UP)
			{
				if(trooperType == TrooperType.Tank)
					trooperInstant.transform.Find("characterPos/ani").GetComponent<tk2dSpriteAnimator>().Play("StandLEFT");	
				else
					trooperInstant.transform.Find("characterPos/ani").GetComponent<tk2dSpriteAnimator>().Play("StandUP");	
			}
			else if(pdirect==Direct.RIGHTUP)
			{
				if(trooperType == TrooperType.Tank)
					trooperInstant.transform.Find("characterPos/ani").GetComponent<tk2dSpriteAnimator>().Play("StandLEFTUP");	
				else
					trooperInstant.transform.Find("characterPos/ani").GetComponent<tk2dSpriteAnimator>().Play("StandRIGHTUP");	
			}
			int index = i;
			if (i >= trooperStands.Length) {
				index = Random.Range (0,trooperStands.Length);
			}
			charInfo.transform.parent = trooperStands [index].transform;
			charInfo.transform.localScale = Vector3.one;
			charInfo.transform.localPosition = Vector3.zero;

			i++;
		}
		
	}

}

