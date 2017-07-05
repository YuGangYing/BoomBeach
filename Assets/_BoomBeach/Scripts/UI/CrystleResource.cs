using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class CrystleResource : MonoBehaviour {

	private static CrystleResource instance;
	public static CrystleResource Instance{
		get{ return instance; }
	}
	void Awake()
	{
		instance = this;
	}

	private UIButton crystle1;
	private UIButton crystle2;
	private UIButton crystle3;
	private UILabel crystle1Amout;
	private UILabel crystle2Amout;
	private UILabel crystle3Amout;
	private UILabel crystletipName;
	private UILabel crystletipDesc;
	public Transform crystletip;
	private string a_type;
	private bool isInit;

	private UISprite SpritePiece1,SpritePiece2,SpritePiece3;

	public void Init()
	{
		if(!isInit)
		{
			crystle1 = transform.Find ("crystle1").GetComponent<UIButton>();
			crystle1.onClick = new List<EventDelegate> ();
			crystle1.onClick.Add(new EventDelegate(this,"OnClickCrystle1"));
			crystle2 = transform.Find ("crystle2").GetComponent<UIButton>();
			crystle2.onClick = new List<EventDelegate> ();
			crystle2.onClick.Add(new EventDelegate(this,"OnClickCrystle2"));
			crystle3 = transform.Find ("crystle3").GetComponent<UIButton>();
			crystle3.onClick = new List<EventDelegate> ();
			crystle3.onClick.Add(new EventDelegate(this,"OnClickCrystle3"));
			crystle1Amout = crystle1.transform.Find ("amount").GetComponent<UILabel> ();
			crystle2Amout = crystle2.transform.Find ("amount").GetComponent<UILabel> ();
			crystle3Amout = crystle3.transform.Find ("amount").GetComponent<UILabel> ();
			crystletip = transform.Find("crystletip");
			crystletipName = crystletip.Find ("name").GetComponent<UILabel> ();
			crystletipDesc = crystletip.Find ("desc").GetComponent<UILabel> ();



			SpritePiece1 = transform.Find ("crystle1/Sprite").GetComponent<UISprite> ();
			SpritePiece2 = transform.Find ("crystle2/Sprite").GetComponent<UISprite> ();
			SpritePiece3 = transform.Find ("crystle3/Sprite").GetComponent<UISprite> ();

			isInit = true;
		}

	}

	public void BindData(int artifact_type)
	{
		int common_piece = 0;
		int rare_piece = 0;
		int epic_piece = 0;
		
		
		if (artifact_type == 1){
			a_type = "";
			common_piece = DataManager.GetInstance().userInfo.common_piece;
			rare_piece = DataManager.GetInstance().userInfo.rare_piece;
			epic_piece = DataManager.GetInstance().userInfo.epic_piece;


		}else if (artifact_type == 2){
			a_type = "_ICE";
			common_piece = DataManager.GetInstance().userInfo.common_piece_ice;
			rare_piece = DataManager.GetInstance().userInfo.rare_piece_ice;
			epic_piece = DataManager.GetInstance().userInfo.epic_piece_ice;
		}else if (artifact_type == 3){
			a_type = "_FIRE";
			common_piece = DataManager.GetInstance().userInfo.common_piece_fire;
			rare_piece = DataManager.GetInstance().userInfo.rare_piece_fire;
			epic_piece = DataManager.GetInstance().userInfo.epic_piece_fire;
		}else if (artifact_type == 4){
			a_type = "_DARK";
			common_piece = DataManager.GetInstance().userInfo.common_piece_dark;
			rare_piece = DataManager.GetInstance().userInfo.rare_piece_dark;
			epic_piece = DataManager.GetInstance().userInfo.epic_piece_dark;
		}

		CsvInfo csvArtiact1 = (CsvInfo)CSVManager.GetInstance().csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT1" + a_type) + "_1"];
		SpritePiece1.spriteName = csvArtiact1.PieceResource;

		CsvInfo csvArtiact2 = (CsvInfo)CSVManager.GetInstance().csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT2" + a_type) + "_1"];
		SpritePiece2.spriteName = csvArtiact2.PieceResource;

		CsvInfo csvArtiact3 = (CsvInfo)CSVManager.GetInstance().csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT3" + a_type) + "_1"];
		SpritePiece3.spriteName = csvArtiact3.PieceResource;


		crystletip.gameObject.SetActive(false);

		crystle1Amout.text = common_piece.ToString ();
		crystle2Amout.text = rare_piece.ToString ();
		crystle3Amout.text = epic_piece.ToString ();
	}

	void OnClickCrystle1()
	{
		crystletip.gameObject.SetActive (true);
		crystletip.position = new Vector3 (crystle1.transform.position.x,crystletip.position.y,crystletip.position.z);
		crystletipName.text = LocalizationCustom.instance.Get ("TID_COMMON_PIECE" + a_type);
		crystletipDesc.text = LocalizationCustom.instance.Get ("TID_COMMON_PIECE_SUBTITLE" + a_type);
	}

	void OnClickCrystle2()
	{
		crystletip.gameObject.SetActive (true);
		crystletip.position = new Vector3 (crystle2.transform.position.x,crystletip.position.y,crystletip.position.z);
		crystletipName.text = LocalizationCustom.instance.Get ("TID_RARE_PIECE" + a_type);
		crystletipDesc.text = LocalizationCustom.instance.Get ("TID_RARE_PIECE_SUBTITLE" + a_type);
	}

	void OnClickCrystle3()
	{
		crystletip.gameObject.SetActive (true);
		crystletip.position = new Vector3 (crystle3.transform.position.x,crystletip.position.y,crystletip.position.z);
		crystletipName.text = LocalizationCustom.instance.Get ("TID_EPIC_PIECE" + a_type);
		crystletipDesc.text = LocalizationCustom.instance.Get ("TID_EPIC_PIECE_SUBTITLE" + a_type);
	}
}
