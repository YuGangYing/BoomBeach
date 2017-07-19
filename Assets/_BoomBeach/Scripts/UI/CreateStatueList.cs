using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class CreateStatueList : TabPanel {

	private Transform StatueItem1;
	private Transform StatueItem2;
	private Transform StatueItem3;

	private UIButton StatueItem1Btn;
	private UIButton StatueItem2Btn;
	private UIButton StatueItem3Btn;

	private UILabel StatueItem1Name;
	private UILabel StatueItem2Name;
	private UILabel StatueItem3Name;

	//private UITexture StatueItem1Avatar;
	//private UITexture StatueItem2Avatar;
	//private UITexture StatueItem3Avatar;

	private UILabel StatueItem1BuildTime;
	private UILabel StatueItem2BuildTime;
	private UILabel StatueItem3BuildTime;

	private UILabel StatueItem1Crystle;
	private UILabel StatueItem2Crystle;
	private UILabel StatueItem3Crystle;

	private UILabel StatueNumbers;
	private string a_type = "";
	private UISprite SpritePiece1,SpritePiece2,SpritePiece3;
	private int currentIdx;

	private BuildInfo buildInfo;

	private CrystleResource crystleResource;

	private bool isInit;
	public void Init(BuildInfo s)
	{
		buildInfo = s;
		if(!isInit)
		{
			StatueItem1 = transform.Find ("GridList/StatueItem1");
			StatueItem2 = transform.Find ("GridList/StatueItem2");
			StatueItem3 = transform.Find ("GridList/StatueItem3");

			StatueItem1Btn = StatueItem1.GetComponent<UIButton>();
			StatueItem1Btn.onClick = new List<EventDelegate>();
			StatueItem1Btn.onClick.Add(new EventDelegate(this,"OnClickStatueItem1Btn"));
			StatueItem2Btn = StatueItem2.GetComponent<UIButton>();
			StatueItem2Btn.onClick = new List<EventDelegate>();
			StatueItem2Btn.onClick.Add(new EventDelegate(this,"OnClickStatueItem2Btn"));
			StatueItem3Btn = StatueItem3.GetComponent<UIButton>();
			StatueItem3Btn.onClick = new List<EventDelegate>();
			StatueItem3Btn.onClick.Add(new EventDelegate(this,"OnClickStatueItem3Btn"));
			
			StatueItem1Name = StatueItem1.Find ("name").GetComponent<UILabel> ();
			StatueItem2Name = StatueItem2.Find ("name").GetComponent<UILabel> ();
			StatueItem3Name = StatueItem3.Find ("name").GetComponent<UILabel> ();
			
			//StatueItem1Avatar = StatueItem1.Find ("avatar").GetComponent<UITexture> ();
			//StatueItem2Avatar = StatueItem2.Find ("avatar").GetComponent<UITexture> ();
			//StatueItem3Avatar = StatueItem3.Find ("avatar").GetComponent<UITexture> ();
			
			StatueItem1Crystle = StatueItem1.Find ("crystlec").GetComponent<UILabel> ();
			StatueItem2Crystle = StatueItem2.Find ("crystlec").GetComponent<UILabel> ();
			StatueItem3Crystle = StatueItem3.Find ("crystlec").GetComponent<UILabel> ();

			StatueItem1BuildTime = StatueItem1.Find ("buildtimec").GetComponent<UILabel> ();
			StatueItem2BuildTime = StatueItem2.Find ("buildtimec").GetComponent<UILabel> ();
			StatueItem3BuildTime = StatueItem3.Find ("buildtimec").GetComponent<UILabel> ();

			StatueNumbers = transform.Find("number").GetComponent<UILabel>();

			SpritePiece1 = StatueItem1.Find ("crystle/Sprite").GetComponent<UISprite> ();
			SpritePiece2 = StatueItem2.Find ("crystle/Sprite").GetComponent<UISprite> ();
			SpritePiece3 = StatueItem3.Find ("crystle/Sprite").GetComponent<UISprite> ();


			crystleResource = transform.parent.Find ("CrystleResource").GetComponent<CrystleResource> ();
			crystleResource.Init ();

			base.Init();
			isInit = true;
		}

		currentIdx = -1;

		current = this;
		for(int i=0;i<Tabs.Length;i++)
		{
			Tabs[i].onselect = OnSelectBind;
		}
		SelectTab (0,OnSelectBind);
	}


	//切换后绑定数据;
	private void OnSelectBind(int idx)
	{
		//base.Init();
		if(idx!=currentIdx)
		{
			currentIdx = idx;
			BindData(idx + 1);
		}
	}


	public void BindData(int artifact_type)
	{

		crystleResource.BindData (artifact_type);

		int common_piece = 0;
		int rare_piece = 0;
		int epic_piece = 0;


		if (artifact_type == 1){
			a_type = "";
			common_piece = DataManager.GetInstance.userInfo.common_piece;
			rare_piece = DataManager.GetInstance.userInfo.rare_piece;
			epic_piece = DataManager.GetInstance.userInfo.epic_piece;
		}else if (artifact_type == 2){
			a_type = "_ICE";
			common_piece = DataManager.GetInstance.userInfo.common_piece_ice;
			rare_piece = DataManager.GetInstance.userInfo.rare_piece_ice;
			epic_piece = DataManager.GetInstance.userInfo.epic_piece_ice;
		}else if (artifact_type == 3){
			a_type = "_FIRE";
			common_piece = DataManager.GetInstance.userInfo.common_piece_fire;
			rare_piece = DataManager.GetInstance.userInfo.rare_piece_fire;
			epic_piece = DataManager.GetInstance.userInfo.epic_piece_fire;
		}else if (artifact_type == 4){
			a_type = "_DARK";
			common_piece = DataManager.GetInstance.userInfo.common_piece_dark;
			rare_piece = DataManager.GetInstance.userInfo.rare_piece_dark;
			epic_piece = DataManager.GetInstance.userInfo.epic_piece_dark;
		}


		int currentAmount = Helper.GetArtifactNum();

		if (buildInfo.hasStatue()){
			currentAmount += 1;
		}

		CsvInfo csvArtiact1 = (CsvInfo)CSVManager.GetInstance.csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT1" + a_type) + "_1"];
		SpritePiece1.spriteName = csvArtiact1.PieceResource;
		
		CsvInfo csvArtiact2 = (CsvInfo)CSVManager.GetInstance.csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT2" + a_type) + "_1"];
		SpritePiece2.spriteName = csvArtiact2.PieceResource;
		
		CsvInfo csvArtiact3 = (CsvInfo)CSVManager.GetInstance.csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT3" + a_type) + "_1"];
		SpritePiece3.spriteName = csvArtiact3.PieceResource;



		Helper.CreateArtifactUI(StatueItem1.Find("avatar"), "TID_BUILDING_ARTIFACT1" + a_type + "_1");
		Helper.CreateArtifactUI(StatueItem2.Find("avatar"), "TID_BUILDING_ARTIFACT2" + a_type + "_1");
		Helper.CreateArtifactUI(StatueItem3.Find("avatar"), "TID_BUILDING_ARTIFACT3" + a_type + "_1");




		StatueNumbers.text = StringFormat.FormatByTid ("TID_ARTIFACT_CAPACITY_LIMIT", new object[]{currentAmount,buildInfo.csvInfo.ArtifactCapacity});
		StatueItem1Name.text = LocalizationCustom.instance.Get ("TID_BUILDING_ARTIFACT1" + a_type);
		StatueItem2Name.text = LocalizationCustom.instance.Get ("TID_BUILDING_ARTIFACT2" + a_type);
		StatueItem3Name.text = LocalizationCustom.instance.Get ("TID_BUILDING_ARTIFACT3" + a_type);

		//CsvInfo csvInfo1 = CSVManager.GetInstance.csvTable ["TID_BUILDING_ARTIFACT1_1"] as CsvInfo;

		StatueItem1BuildTime.text = Helper.GetFormatTime(Helper.GetBuildTime ("TID_BUILDING_ARTIFACT1"  + a_type + "_1"),0);
		StatueItem2BuildTime.text = Helper.GetFormatTime(Helper.GetBuildTime ("TID_BUILDING_ARTIFACT2"  + a_type + "_1"),0);
		StatueItem3BuildTime.text = Helper.GetFormatTime(Helper.GetBuildTime ("TID_BUILDING_ARTIFACT3"  + a_type + "_1"),0);

		int statue1Cost = Helper.GetBuildCost ("TID_BUILDING_ARTIFACT1"  + a_type + "_1").piece;
		int statue2Cost = Helper.GetBuildCost ("TID_BUILDING_ARTIFACT2"  + a_type + "_1").piece;
		int statue3Cost = Helper.GetBuildCost ("TID_BUILDING_ARTIFACT3"  + a_type + "_1").piece;
		StatueItem1Crystle.text = statue1Cost.ToString();


		if(statue1Cost>common_piece)
		{
			StatueItem1Crystle.color = Color.red;
		}
		else
		{
			StatueItem1Crystle.color = Color.white;
		}

		StatueItem2Crystle.text = statue2Cost.ToString();
		if(statue2Cost>rare_piece)
		{
			StatueItem2Crystle.color = Color.red;
		}
		else
		{
			StatueItem2Crystle.color = Color.white;
		}

		StatueItem3Crystle.text = statue3Cost.ToString();
		if(statue3Cost>epic_piece)
		{
			StatueItem3Crystle.color = Color.red;
		}
		else
		{
			StatueItem3Crystle.color = Color.white;
		}
	}



	void OnClickStatueItem1Btn()
	{
		//Debug.Log("OnClickStatueItem1Btn");
		BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
		buildInfo.OnCreateStatue(buildInfo,"TID_BUILDING_ARTIFACT1"  + a_type + "_1");
	}
	void OnClickStatueItem2Btn()
	{
		//Debug.Log("OnClickStatueItem2Btn");
		BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
		buildInfo.OnCreateStatue(buildInfo,"TID_BUILDING_ARTIFACT2"  + a_type + "_1");
	}
	void OnClickStatueItem3Btn()
	{
		//Debug.Log("OnClickStatueItem3Btn");
		BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
		buildInfo.OnCreateStatue(buildInfo,"TID_BUILDING_ARTIFACT3"  + a_type + "_1");
	}
}
