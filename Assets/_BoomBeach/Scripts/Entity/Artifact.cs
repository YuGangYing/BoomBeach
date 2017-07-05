using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Artifact : MonoBehaviour {


	public ArtifactType type{
		get{ return artifactType; }
	}
	public BuildInfo buildInfo;

	private ArtifactType artifactType = ArtifactType.None;
	private Transform status_none;
	private Transform status_gem;
	private Transform status_shield;
	private Transform status_weapon;

	private Transform crystlePart;



	void Awake()
	{
		status_none = transform.Find ("model/status_none");
		status_gem = transform.Find ("model/status_gem");
		status_shield = transform.Find ("model/status_shield");
		status_weapon = transform.Find ("model/status_weapon");
		setStatus (ArtifactType.None);
	}

	public void setStatus(ArtifactType i_artifacttype)
	{
		artifactType = i_artifacttype;
		status_none.gameObject.SetActive (false);
		status_gem.gameObject.SetActive (false);
		status_shield.gameObject.SetActive (false);
		status_weapon.gameObject.SetActive (false);

		if(artifactType==ArtifactType.BoostGold||
		        artifactType==ArtifactType.BoostWood||
		        artifactType==ArtifactType.BoostStone||
		        artifactType==ArtifactType.BoostMetal||
		  		artifactType==ArtifactType.BoostLoot||
		   		artifactType==ArtifactType.BoostArtifactDrop||
		   		artifactType==ArtifactType.BoostAllResources)
		{
			//花;
			status_gem.gameObject.SetActive(true);
		}
		else if(artifactType==ArtifactType.BoostTroopHP||
		        artifactType==ArtifactType.BoostBuildingHP)
		{
			//盾;
			status_shield.gameObject.SetActive(true);
		}
		else if(artifactType==ArtifactType.BoostTroopDamage||
		        artifactType==ArtifactType.BoostBuildingDamage||
		        artifactType==ArtifactType.BoostGunshipEnergy)
		{
			//矛;
			status_weapon.gameObject.SetActive(true);
		}
		else
		{
			//无;
			status_none.gameObject.SetActive(true);
		}

	}

	public void removeArtifact(string SellResource)
	{
		Debug.Log (SellResource);
		GameObject crystlePartObj = Instantiate(ResourceCache.load ("UI/CrystlePart")) as GameObject;
		crystlePart = crystlePartObj.transform;
		crystlePart.parent = transform;
		crystlePart.name = "crystle";
		crystlePart.localPosition = Vector3.zero;
		crystlePart.localRotation = new Quaternion (0,0,0,0);

		crystlePart.GetComponent<UISprite> ().spriteName = SellResource;
		crystlePart.GetComponent<UISprite> ().MakePixelPerfect ();
		crystlePart.transform.localScale = Vector3.one * 0.002f;
		TweenPosition tp = crystlePart.GetComponent<TweenPosition> ();
		tp.onFinished = new List<EventDelegate> ();
		tp.onFinished.Add (new EventDelegate(this,"removeDone"));
		tp.PlayForward ();

		crystlePart.GetComponent<TweenAlpha> ().PlayForward ();


	}

	void removeDone()
	{
		if (buildInfo!=null&&buildInfo.status==BuildStatus.Removaling) {
			buildInfo.OnCancelCreate();		
		}
	}
}
