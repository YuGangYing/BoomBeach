using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildInfoWin : MonoBehaviour {

	public GameObject PropertyItemPrefab;

	private Transform AddedTitle;
	private Transform BuildModel;
	private Transform PropertyGrid;
	private AutoSort PropertyGridAutoSort;
	private Transform DescriptionLabel;
	private Transform LevelLabel;
	private Transform UpgradeTitle;

	private UILabel DescriptionLabelLabel;
	private UILabel LevelLabelLabel;

	private bool isInit; 

	private BuildInfo currentPopBuildInfo;

	public void Init()
	{
		if(!isInit)
		{
			AddedTitle = transform.Find ("AddedTitle");
			BuildModel = transform.Find ("BuildModel");
			PropertyGrid = transform.Find ("BuildProperty/PropertyGrid");
			PropertyGridAutoSort = PropertyGrid.GetComponent<AutoSort>();
			PropertyGridAutoSort.autoSortList = PropertyGridAutoSort.GetComponentsInChildren<AutoSortItem>(true);
			DescriptionLabel = transform.Find ("DescriptionLabel");
			DescriptionLabelLabel = DescriptionLabel.GetComponent<UILabel> ();
			LevelLabel = transform.Find ("LevelLabel");
			LevelLabelLabel = LevelLabel.GetComponent<UILabel> ();
			UpgradeTitle = transform.Find ("UpgradeTitle");
			isInit = true;
		}

	}



	public void BindBuildInfoWin(BuildInfo buildInfo)
	{
		currentPopBuildInfo = buildInfo;
		UpgradeTitle.gameObject.SetActive (false);

		LevelLabel.gameObject.SetActive (true);
		LevelLabelLabel.text = buildInfo.ShowLevelName;
		/*
		TID_BOOST_GOLD_INFO = Increases gold production from residences and freed villages
			TID_BOOST_WOOD_INFO = Increases wood production from home base sawmill and wood resource bases
			TID_BOOST_STONE_INFO = Increases stone production from home base quarry and stone resource bases
			TID_BOOST_METAL_INFO = Increases metal production from home base iron mine and iron resource bases
			TID_BOOST_TROOP_HP_INFO = Increases all your troops' health
TID_BOOST_BUILDING_HP_INFO = Increases the health of your home base and resource base buildings
TID_BOOST_TROOP_DAMAGE_INFO = Increases all your troops' damage output
			TID_BOOST_BUILDING_DAMAGE_INFO = Increases the damage output of your base and resource base defenses

TID_BOOST_BUILDING_HP = Building health +<number>%
TID_BOOST_GOLD = Gold production +<number>%
TID_BOOST_BUILDING_DAMAGE = Defensive building damage +<number>%
TID_BOOST_WOOD = Wood production +<number>%
TID_BOOST_STONE = Stone production +<number>%
TID_BOOST_METAL = Iron production +<number>%
TID_BOOST_TROOP_HP = Troop health +<number>%
TID_BOOST_TROOP_DAMAGE = Troop damage +<number>%
*/
		if (buildInfo.csvInfo.BuildingClass != "Artifact"){
			DescriptionLabelLabel.text = StringFormat.FormatByTid (buildInfo.csvInfo.InfoTID);
		}else{
			//public int artifact_type;//神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage
			string InfoTID = "";
			if (buildInfo.artifact_type == ArtifactType.BoostGold){
				InfoTID = "TID_BOOST_GOLD_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostWood){
				InfoTID = "TID_BOOST_WOOD_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostStone){
				InfoTID = "TID_BOOST_STONE_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostMetal){
				InfoTID = "TID_BOOST_METAL_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostTroopHP){
				InfoTID = "TID_BOOST_TROOP_HP_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostBuildingHP){
				InfoTID = "TID_BOOST_BUILDING_HP_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostTroopDamage){
				InfoTID = "TID_BOOST_TROOP_DAMAGE_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostBuildingDamage){
				InfoTID = "TID_BOOST_BUILDING_DAMAGE_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostGunshipEnergy){
				InfoTID = "TID_BOOST_GUNSHIP_ENERGY_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostLoot){
				InfoTID = "TID_BOOST_LOOT_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostArtifactDrop){
				InfoTID = "TID_BOOST_ARTIFACT_DROP_INFO";
			}else if (buildInfo.artifact_type == ArtifactType.BoostAllResources){
				InfoTID = "TID_BOOST_ALL_INFO";
			}

			DescriptionLabelLabel.text = StringFormat.FormatByTid (InfoTID);
		}

		Dictionary<string ,PropertyInfoNew> propertyList =	 Helper.getPropertyList (buildInfo.tid_level, false, buildInfo);

		bool isShowAddedTitle = false;



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
			propertyItem.transform.Find("Added").GetComponent<UILabel>().text = info.bonus_value;
			if(!isShowAddedTitle&&info.bonus_value!=""&&info.bonus_value!=null)
			{
				isShowAddedTitle=true;
			}
			propertyItem.transform.name = info.name;

			if(i==0)propertyItem.transform.localPosition = Vector3.zero;
			i++;

		}

		AddedTitle.gameObject.SetActive (isShowAddedTitle);  

	
	}

	void OnDisable()
	{
		int autoSortListLength = PropertyGridAutoSort.autoSortList.Length;
		for(int i=0;i<autoSortListLength;i++)
		{
			PropertyGridAutoSort.autoSortList[i].gameObject.SetActive (false);
		}
	}

	public void InitBuildModel()
	{
		//创建模形;
		if(BuildModel.Find("model")!=null)
			Destroy (BuildModel.Find("model").gameObject);

		if (currentPopBuildInfo.csvInfo.BuildingClass == "Artifact"){
			Helper.CreateArtifactUI(BuildModel, currentPopBuildInfo.tid_level,currentPopBuildInfo.artifact_type);
		}else{

			GameObject buildModel = Instantiate (ResourceCache.Load(currentPopBuildInfo.buildSpritePath)) as GameObject;
			buildModel.transform.parent = BuildModel;
			buildModel.transform.localPosition = Vector3.zero;
			buildModel.transform.name = "model";
			if(currentPopBuildInfo.is3D)
			{
				buildModel.transform.localScale = new Vector3 (400f,400f,1f);
				if(currentPopBuildInfo.tid=="TID_BUILDING_GUNSHIP")
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

}
