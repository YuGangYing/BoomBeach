using UnityEngine;
using System.Collections;

public class CreateStatueWin : MonoBehaviour {

	private CreateStatueList createStatueList;
	private ManageStatueBox manageStatueBox;
	private CrystleResource crystleResource;



	private BuildInfo buildInfo;
	private bool isInit; 

	public void Init()
	{
		if(!isInit)
		{
			createStatueList = transform.Find ("CreateStatueList").GetComponent<CreateStatueList> ();
			manageStatueBox = transform.Find ("ManageStatueBox").GetComponent<ManageStatueBox> ();
			crystleResource = transform.Find ("CrystleResource").GetComponent<CrystleResource> ();


			//createStatueList.Init ();
			manageStatueBox.Init ();
			crystleResource.Init ();


			isInit = true;
		}



	}

	/*
	public void OnBtnPiece1(){
		SpritePiece1.color = Color.white;
		SpritePiece2.color = NormalColor;
		SpritePiece3.color = NormalColor;
		SpritePiece4.color = NormalColor;
		createStatueList.BindData (buildInfo,1);
		crystleResource.BindData (1);
	}

	public void OnBtnPiece2(){
		SpritePiece1.color = NormalColor;
		SpritePiece2.color = Color.white;
		SpritePiece3.color = NormalColor;
		SpritePiece4.color = NormalColor;
		createStatueList.BindData (buildInfo,2);
		crystleResource.BindData (2);
	}

	public void OnBtnPiece3(){
		SpritePiece1.color = NormalColor;
		SpritePiece2.color = NormalColor;
		SpritePiece3.color = Color.white;
		SpritePiece4.color = NormalColor;
		createStatueList.BindData (buildInfo,3);
		crystleResource.BindData (3);
	}

	public void OnBtnPiece4(){
		SpritePiece1.color = NormalColor;
		SpritePiece2.color = NormalColor;
		SpritePiece3.color = NormalColor;
		SpritePiece4.color = Color.white;
		createStatueList.BindData (buildInfo,4);
		crystleResource.BindData (4);
	}

*/

	

	public void BindCreateStatueWin(BuildInfo s)
	{
		buildInfo = s;
		if(buildInfo.hasStatue())
		{
			//显示布署;
			manageStatueBox.transform.gameObject.SetActive(true);
			createStatueList.transform.gameObject.SetActive(false);
			manageStatueBox.BindData (buildInfo);

			CsvInfo csvArtiact = CSVManager.GetInstance().csvTable [buildInfo.status_tid_level] as CsvInfo;
			crystleResource.BindData (csvArtiact.ArtifactType);
		}
		else
		{
			manageStatueBox.transform.gameObject.SetActive(false);
			createStatueList.transform.gameObject.SetActive(true);
			createStatueList.Init(buildInfo);
			//OnBtnPiece1();


		}


	}
}
