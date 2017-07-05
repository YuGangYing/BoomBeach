using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManageStatueBox : MonoBehaviour {

	//private UITexture avatar;
	private UILabel DeployVal;
	private UILabel ReclaimVal;
	private UISprite ReclaimCrystle;


	private UIButton DeployBtn;
	private UIButton ReclaimBtn;

	private UILabel DispName;

	private bool isInit;
	public void Init()
	{
		if(!isInit)
		{
			//avatar = transform.Find ("avatar").GetComponent<UITexture> ();
			DeployVal = transform.Find ("Deploy/val").GetComponent<UILabel> ();
			Transform ReclaimBox = transform.Find ("Reclaim");
			ReclaimVal = ReclaimBox.Find ("val").GetComponent<UILabel> ();
			ReclaimCrystle = ReclaimBox.Find ("Sprite").GetComponent<UISprite>();

			
			DeployBtn = transform.Find ("DeployBtn").GetComponent<UIButton> ();
			DeployBtn.onClick = new List<EventDelegate>();
			DeployBtn.onClick.Add(new EventDelegate(this,"OnClickDeployBtn"));
			ReclaimBtn = transform.Find ("ReclaimBtn").GetComponent<UIButton> ();
			ReclaimBtn.onClick = new List<EventDelegate>();
			ReclaimBtn.onClick.Add(new EventDelegate(this,"OnClickReclaimBtn"));
			DispName = transform.Find("name").GetComponent<UILabel>();
			isInit = true;
		}


	}


	public void BindData(BuildInfo buildInfo)
	{
		CsvInfo csvInfo = CSVManager.GetInstance().csvTable [buildInfo.status_tid_level] as CsvInfo;
		string name = StringFormat.FormatByTid ("TID_COMPLETED_ARTIFACT", new object[]{StringFormat.FormatByTid (csvInfo.TID)});
		DispName.text = name;

		//Debug.Log(buildInfo.status_tid_level);
		Helper.CreateArtifactUI(transform.Find ("avatar"), buildInfo.status_tid_level,buildInfo.artifact_type);

		string InfoTID = Helper.GetNameToTid(buildInfo.artifact_type.ToString());
		DeployVal.text = StringFormat.FormatByTid(InfoTID,new string[]{buildInfo.artifact_boost.ToString()});

		string artiact_tid_level = Helper.BuildTIDToArtifactTID(csvInfo.TID) + "_1";
		CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance().csvTable[artiact_tid_level];

		ReclaimVal.text = " x " + csvArtiact.SellResourceAmount.ToString();
		ReclaimCrystle.spriteName = csvArtiact.SellResource;

	}

	void OnClickDeployBtn()
	{
		//Debug.Log ("OnClickDeployBtn");
		BuildInfo s = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
		s.OnDeployStatue();
	}

	void OnClickReclaimBtn()
	{
		//Debug.Log("OnClickReclaimBtn");
		BuildInfo s = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
		s.OnRemoval(s);
	}
}
