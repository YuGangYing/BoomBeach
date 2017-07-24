using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class TroopUpgradeWin : MonoBehaviour {

	public GameObject PropertyItemPrefab;

	private Transform AddedTitle;
	private Transform TroopAvatar;
	private UISprite TrooperAvatarSprite;
	private Transform PropertyGrid;
	private AutoSort PropertyGridAutoSort;
	private Transform UpgradeTitle;

	private UILabel DiamondResourceLabel;
	private UILabel GoldResourceLabel;
	private UIButton InstantBtn;
	private UIButton UpgradeBtn;
	private UILabel TimeLabel;

	private bool isInit;
	private ResearchTid tidData;

	public void Init()
	{
		if(!isInit)
		{
			AddedTitle = transform.Find ("AddedTitle");
			TroopAvatar = transform.Find ("TroopAvatar");
			TrooperAvatarSprite = TroopAvatar.GetComponent<UISprite> ();
			PropertyGrid = transform.Find ("TroopProperty/PropertyGrid");
			PropertyGridAutoSort = PropertyGrid.GetComponent<AutoSort>();
			PropertyGridAutoSort.autoSortList = PropertyGridAutoSort.GetComponentsInChildren<AutoSortItem>(true);
			UpgradeTitle = transform.Find ("UpgradeTitle");

			DiamondResourceLabel = transform.Find ("Btns/DiamondResource/bg/Label").GetComponent<UILabel> ();
			GoldResourceLabel = transform.Find ("Btns/GoldResource/bg/Label").GetComponent<UILabel> ();
			InstantBtn = transform.Find ("Btns/DiamondBtn").GetComponent<UIButton> ();
			UpgradeBtn = transform.Find ("Btns/UpgradeBtn").GetComponent<UIButton> ();
			TimeLabel = UpgradeBtn.transform.Find ("NeedTime").GetComponent<UILabel> ();
			isInit = true;
		}
	}



	public void BindTroopInfoWin(ResearchTid tidData, bool is_update)
	{
		this.tidData = tidData;
		string tid_level = tidData.tid_level;
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable [tid_level] as CsvInfo;

//		string ShowName = StringFormat.FormatByTid (csvInfo.TID);
//		string ShowLevel = StringFormat.FormatByTid("TID_LEVEL_NUM",new string[]{csvInfo.Level.ToString()});

		AddedTitle.gameObject.SetActive (false);

	
		Dictionary<string ,PropertyInfoNew> propertyList =	 Helper.getPropertyList (tid_level, is_update, null);

		bool isShowUpgradeTitle = false;

		int i = 0;
		foreach(PropertyInfoNew info in propertyList.Values)
		{
			GameObject propertyItem = PropertyGridAutoSort.autoSortList[i].gameObject;
			propertyItem.SetActive(true);
			propertyItem.transform.Find("Title").GetComponent<UILabel>().text = info.showName;
			propertyItem.transform.Find("Icon").GetComponent<UISprite>().spriteName = info.spriteName;
			propertyItem.transform.Find("Value").GetComponent<UILabel>().text = info.value;
			propertyItem.transform.Find("Added").gameObject.SetActive(false);
			propertyItem.transform.Find("Upgrade").gameObject.SetActive(true);
			//Debug.Log();
			propertyItem.transform.Find("Upgrade").GetComponent<UILabel>().text = info.upgrade_value;
			if(!isShowUpgradeTitle&&info.upgrade_value!=""&&info.upgrade_value!=null)
			{
				isShowUpgradeTitle=true;
			}
			propertyItem.transform.name = info.name;
			if(i==0)propertyItem.transform.localPosition = Vector3.zero;
			i++;

		}
		PropertyGridAutoSort.resort ();
		UpgradeTitle.gameObject.SetActive (isShowUpgradeTitle);  

		DiamondResourceLabel.text = CalcHelper.calcTimeToGems (tidData.upgradeTime).ToString();
		GoldResourceLabel.text = tidData.upgradeCost.ToString ();
		if(tidData.upgradeCost>DataManager.GetInstance.model.user_info.gold_count)
		{
			GoldResourceLabel.color = Color.red;
		}
		else
		{
			GoldResourceLabel.color = Color.black;
		}
		TimeLabel.text = Helper.GetFormatTime (tidData.upgradeTime, 0);

		if(csvInfo.TID_Type=="CHARACTERS")
		{
			TrooperAvatarSprite.atlas = PartEmitObj.Instance.avatarFullAtlas;
			TrooperAvatarSprite.spriteName = csvInfo.TID;
			TrooperAvatarSprite.MakePixelPerfect ();
			TroopAvatar.localScale = Vector3.one;
		}
		else
		{
			TrooperAvatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
			TrooperAvatarSprite.spriteName = csvInfo.TID;
			TrooperAvatarSprite.MakePixelPerfect ();
			TroopAvatar.localScale = Vector3.one*2f;
		}

		InstantBtn.onClick = new List<EventDelegate> ();
		UpgradeBtn.onClick = new List<EventDelegate> ();
		InstantBtn.onClick.Add (new EventDelegate(this,"OnInstant"));
		UpgradeBtn.onClick.Add (new EventDelegate(this,"OnUpgrade"));
	
	}

	public void OnInstant()
	{
		Debug.Log("OnInstant");
		//实验室只有一个;
		BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_LABORATORY");
        BuildInfo.FinishBuildToServer(buildInfo,0,0,true,tidData.tid_level);
	}
	/*
	public void OnUpgrade()
	{
		Debug.Log("OnUpgrade");
		//实验室只有一个;
		BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_LABORATORY");
		buildInfo.OnUpgradeTech(buildInfo,tidData.tid_level);
	}
*/

	void OnDisable()
	{
		int autoSortListLength = PropertyGridAutoSort.autoSortList.Length;
		for(int i=0;i<autoSortListLength;i++)
		{
			PropertyGridAutoSort.autoSortList[i].gameObject.SetActive (false);
		}
	}

}
