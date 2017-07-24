using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class BuildUpgradeWin : MonoBehaviour {

	public GameObject PropertyItemPrefab;
	public GameObject UnlockItemPrefab;

	private Transform AddedTitle;
	private Transform BuildModel;
	private Transform PropertyGrid;
	private AutoSort PropertyGridAutoSort;
	private Transform UpgradeTitle;
	private Transform ResourcesAndBtns;
	private UIButton UpgradeBtn;
	private UILabel UpgradeTime;
	private UIButton InstantBtn;
	private Transform WoodResource;
	private Transform StoneResource;
	private Transform IronResource;
	private Transform DiamondResource;
	private UILabel WoodLabel;
	private UILabel StoneLabel;
	private UILabel IronLabel;
	private UILabel DiamondLabel;

	private Transform Tips;
	private UILabel TipsLabel;
	private BuildInfo buildInfo;

	private Transform HeadquaterUnlock;
	private Transform unlockList;
	private AutoSort unlockListAutoSort;


	private static BuildUpgradeWin instance;
	public static BuildUpgradeWin Instance{
		get{ return instance;}
	}

	void Awake()
	{
		instance = this;
	}
	public Transform TipBox;

	private bool isInit; 
	public void Init()
	{
		if(!isInit)
		{
			AddedTitle = transform.Find ("AddedTitle");
			BuildModel = transform.Find ("BuildModel");
			PropertyGrid = transform.Find ("BuildProperty/PropertyGrid");
			PropertyGridAutoSort = PropertyGrid.GetComponent<AutoSort>();
			PropertyGridAutoSort.autoSortList = PropertyGridAutoSort.GetComponentsInChildren<AutoSortItem>(true);
			UpgradeTitle = transform.Find ("UpgradeTitle");
			ResourcesAndBtns = transform.Find ("ResourcesAndBtns");
			UpgradeBtn = ResourcesAndBtns.Find ("UpgradeBtn").GetComponent<UIButton> ();
			UpgradeTime = ResourcesAndBtns.Find ("UpgradeBtn/NeedTime").GetComponent<UILabel> ();
			InstantBtn = ResourcesAndBtns.Find ("InstantBtn").GetComponent<UIButton> ();
			WoodResource = ResourcesAndBtns.Find ("UpgradeResource/WoodResource");
			StoneResource = ResourcesAndBtns.Find ("UpgradeResource/StoneResource");
			IronResource = ResourcesAndBtns.Find ("UpgradeResource/IronResource");
			DiamondResource = ResourcesAndBtns.Find ("InstantResource/DiamondResource");
			WoodLabel = WoodResource.Find ("bg/Label").GetComponent<UILabel> ();
			StoneLabel = StoneResource.Find ("bg/Label").GetComponent<UILabel> ();
			IronLabel = IronResource.Find ("bg/Label").GetComponent<UILabel> ();
			DiamondLabel = DiamondResource.Find ("bg/Label").GetComponent<UILabel> ();
			Tips = transform.Find("Tips");
			TipsLabel = Tips.Find ("tip").GetComponent<UILabel> ();
			
			HeadquaterUnlock = transform.Find ("HeadquaterUnlock");
			unlockList = HeadquaterUnlock.Find ("ScrollView/GridList");
			unlockListAutoSort = unlockList.GetComponent<AutoSort>();
			unlockListAutoSort.autoSortList = unlockListAutoSort.GetComponentsInChildren<AutoSortItem>(true);

			isInit = true;
		}

	}



	public void BindBuildUpgradeWin(BuildInfo buildInfo)
	{
		this.buildInfo = buildInfo;

		AddedTitle.gameObject.SetActive (false);

		Dictionary<string ,PropertyInfoNew> propertyList =	 Helper.getPropertyList (buildInfo.tid_level, true, buildInfo);

		bool isShowUpgradeTitle = false;

		int i = 0;
		foreach(PropertyInfoNew info in propertyList.Values)
		{
			GameObject propertyItem = PropertyGridAutoSort.autoSortList[i].gameObject;
			propertyItem.SetActive(true);
			propertyItem.transform.Find("Title").GetComponent<UILabel>().text = info.showName;
			propertyItem.transform.Find("Icon").GetComponent<UISprite>().spriteName = info.spriteName;
			propertyItem.transform.Find("Value").GetComponent<UILabel>().text = info.value;
			propertyItem.transform.Find("Added").gameObject.SetActive(true);
			propertyItem.transform.Find("Upgrade").gameObject.SetActive(false);
			propertyItem.transform.Find("Added").GetComponent<UILabel>().text = info.upgrade_value;
			if(!isShowUpgradeTitle&&info.upgrade_value!=""&&info.upgrade_value!=null)
			{
				isShowUpgradeTitle=true;
			}
			propertyItem.transform.name = info.name;
			if(i==0)propertyItem.transform.localPosition = Vector3.zero;
			i++;

		}

		UpgradeTitle.gameObject.SetActive (isShowUpgradeTitle);  

		string msg = Helper.CheckHasUpgrade(buildInfo.tid, buildInfo.level);
		if (msg == null) 
		{
			ResourcesAndBtns.gameObject.SetActive(true);
			Tips.gameObject.SetActive(false);

			BuildCost buildCost = Helper.getUpgradeCost(buildInfo.tid_level);
			if(buildCost.wood>0)
			{
				WoodResource.gameObject.SetActive(true);
				WoodLabel.text = buildCost.wood.ToString();
				if(buildCost.wood>DataManager.GetInstance.model.user_info.wood_count)
				{
					WoodLabel.color = Color.red;
				}
				else
				{
					WoodLabel.color = Color.black;
				}
			}
			else
			{
				WoodResource.gameObject.SetActive(false);
			}

			if(buildCost.stone>0)
			{
				StoneResource.gameObject.SetActive(true);
				StoneLabel.text = buildCost.stone.ToString();
				if(buildCost.stone>DataManager.GetInstance.model.user_info.stone_count)
				{
					StoneLabel.color = Color.red;
				}
				else
				{
					StoneLabel.color = Color.black;
				}
			}
			else
			{
				StoneResource.gameObject.SetActive(false);
			}

			if(buildCost.iron>0)
			{
				IronResource.gameObject.SetActive(true);
				IronLabel.text = buildCost.iron.ToString();
				if(buildCost.iron>DataManager.GetInstance.model.user_info.iron_count)
				{
					IronLabel.color = Color.red;
				}
				else
				{
					IronLabel.color = Color.black;
				}
			}
			else
			{
				IronResource.gameObject.SetActive(false);
			}

			DiamondLabel.text = Helper.GetUpgradeInstant(buildInfo.tid_level).ToString();
			UpgradeTime.text = Helper.GetFormatTime(Helper.GetUpgradeTime(buildInfo.tid_level),0);


		}
		else
		{
			ResourcesAndBtns.gameObject.SetActive(false);
			Tips.gameObject.SetActive(true);
			TipsLabel.text = msg;
		}

		if(buildInfo.tid=="TID_BUILDING_PALACE")
		{
		 	List<UnLockTid> unlockListTid =	Helper.getUpgradeUnLock ();
			if(unlockListTid.Count>0)
			{
				HeadquaterUnlock.gameObject.SetActive(true);			

				int k = 0;
				foreach(UnLockTid tid in unlockListTid)
				{
					GameObject unlockItemObj = unlockListAutoSort.autoSortList[k].gameObject;
					unlockItemObj.SetActive(true);

					UnlockItem item = unlockItemObj.GetComponent<UnlockItem>();
					item.Name = tid.showName;
					item.Brief = tid.Subtitle;
					item.Counter = tid.value;
					item.model = tid.tid;

					k++;
				}

			}
			else
			{
				HeadquaterUnlock.gameObject.SetActive(false);
			}
		}
		else
		{
			HeadquaterUnlock.gameObject.SetActive(false);
		}


		UpgradeBtn.onClick = new List<EventDelegate> ();
		InstantBtn.onClick = new List<EventDelegate> ();

		UpgradeBtn.onClick.Add (new EventDelegate(this,"OnClickUpgrade"));
		InstantBtn.onClick.Add (new EventDelegate(this,"OnClickInstant"));
	
	}

	void OnDisable()
	{
		int autoSortListLength = PropertyGridAutoSort.autoSortList.Length;
		for(int i=0;i<autoSortListLength;i++)
		{
			PropertyGridAutoSort.autoSortList[i].gameObject.SetActive (false);
		}

		int unlockListLength = unlockListAutoSort.autoSortList.Length;
		for(int j=0;j<unlockListLength;j++)
		{
			unlockListAutoSort.autoSortList[j].gameObject.SetActive (false);
		}
		if(TipBox!=null)
		TipBox.gameObject.SetActive (false);
	}

	public void OnClickUpgrade()
	{
		//Debug.Log ("OnClickUpgrade");
		BuildHandle.OnUpgradeBuild(buildInfo);
	}

	public void OnClickInstant()
	{
		//Debug.Log ("OnClickInstant");
		//buildInfo.InstantUpgrade(buildInfo,null);
	}

	public void InitBuildModel()
	{
		//创建模形;
		if(BuildModel.Find("model")!=null)
			Destroy (BuildModel.Find("model").gameObject);

		string tid = buildInfo.tid;
		int level = buildInfo.level+1;
		string tid_level = tid + "_" + level;
		
		CsvInfo csvData = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;
		CsvInfo Lv1CsvData = CSVManager.GetInstance.csvTable[tid+"_1"] as CsvInfo;

		string modelPath = "Build";
		if(buildInfo.is3D)
			modelPath = "Build3d";
		string buildSpritePath = "Model/"+modelPath+"/" + csvData.ExportName;


		if(ResourceCache.load(buildSpritePath)==null)
		{
			buildSpritePath = "Model/"+modelPath+"/" + Lv1CsvData.ExportName;
		}
		
		if(ResourceCache.load(buildSpritePath)==null)
		{
			buildSpritePath = "Model/Build/housing_lvl1";
		}




		GameObject buildModel = Instantiate (ResourceCache.load(buildSpritePath)) as GameObject;

		buildModel.transform.parent = BuildModel;
		buildModel.transform.localPosition = Vector3.zero;
		buildModel.transform.name = "model";
		if(buildInfo.is3D)
		{
			buildModel.transform.localScale = new Vector3 (400f,400f,1f);
			if(buildInfo.tid=="TID_BUILDING_GUNSHIP")
			{
				buildModel.transform.localScale = new Vector3 (300f,300f,1f);
				buildModel.transform.localPosition = new Vector3(100f,50f,0f);
			}
		}
		else
			buildModel.transform.localScale = new Vector3 (560f,560f,1f);



		Transform[] tts = buildModel.GetComponentsInChildren<Transform> ();

		for(int i=0;i<tts.Length;i++)
		{
			tts[i].gameObject.layer = 9;
		}

	}

}
