using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TroopInfoWin : MonoBehaviour {

	public GameObject PropertyItemPrefab;

	private Transform AddedTitle;
	private Transform TroopAvatar;
	private UISprite TrooperAvatarSprite;
	private Transform PropertyGrid;
	private AutoSort PropertyGridAutoSort;
	private Transform DescriptionLabel;
	private Transform LevelLabel;
	private Transform UpgradeTitle;

	private UILabel DescriptionLabelLabel;
	private UILabel LevelLabelLabel;


	private bool isInit; 
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
			DescriptionLabel = transform.Find ("DescriptionLabel");
			DescriptionLabelLabel = DescriptionLabel.GetComponent<UILabel> ();
			LevelLabel = transform.Find ("LevelLabel");
			LevelLabelLabel = LevelLabel.GetComponent<UILabel> ();
			UpgradeTitle = transform.Find ("UpgradeTitle");
			isInit = true;
		}

	}



	public void BindTroopInfoWin(string tid_level)
	{

		CsvInfo csvInfo = CSVManager.GetInstance.csvTable [tid_level] as CsvInfo;

//		string ShowName = StringFormat.FormatByTid (csvInfo.TID);
		string ShowLevel = StringFormat.FormatByTid("TID_LEVEL_NUM",new string[]{csvInfo.Level.ToString()});
		string Description = StringFormat.FormatByTid (csvInfo.InfoTID);

		UpgradeTitle.gameObject.SetActive (false);

		LevelLabel.gameObject.SetActive (true);
		LevelLabelLabel.text = ShowLevel;
		DescriptionLabelLabel.text = Description;
	
		Dictionary<string ,PropertyInfoNew> propertyList =	 Helper.getPropertyList (tid_level, false, null);

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
		PropertyGridAutoSort.resort ();
		AddedTitle.gameObject.SetActive (isShowAddedTitle);  

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


	
	}

	void OnDisable()
	{
		int autoSortListLength = PropertyGridAutoSort.autoSortList.Length;
		for(int i=0;i<autoSortListLength;i++)
		{
			PropertyGridAutoSort.autoSortList[i].gameObject.SetActive (false);
		}
	}
	

}
