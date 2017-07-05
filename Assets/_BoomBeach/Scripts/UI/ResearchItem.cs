using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class ResearchItem : MonoBehaviour {

	private ResearchTid TidData;
	public void BindData(ResearchTid tidData)
	{
		this.TidData = tidData;
		if(TidData.hasUpgrade!=null)
		{

			if(!Helper.isUnLock(TidData.tid_level))
			{
				//灰;
				transform.Find("bg").GetComponent<UISprite>().color = Color.black;
				transform.Find("avatar").GetComponent<UISprite>().color = Color.black;

			}
			else
			{
				//淡色;
				transform.Find("bg").GetComponent<UISprite>().color = new Color(1f,1f,1f,0.3f);
				transform.Find("avatar").GetComponent<UISprite>().color = new Color(1f,1f,1f,0.7f);
			}

			GetComponent<UIButton>().enabled = false;
			transform.Find("price").gameObject.SetActive(false);
			transform.Find("tip").gameObject.SetActive(true);
			transform.Find("tip").GetComponent<UILabel>().text = TidData.hasUpgrade;
			transform.Find("Sprite").gameObject.SetActive(false);
			
			UIButton btn = transform.GetComponent<UIButton>();
			btn.onClick = new List<EventDelegate>();

		}
		else
		{
			//可以升级;
			UIButton btn = transform.GetComponent<UIButton>();
			btn.enabled = true;
			btn.onClick = new List<EventDelegate>();
			btn.onClick.Add(new EventDelegate(this,"OnClickResearch"));

			transform.Find("price").gameObject.SetActive(true);
			transform.Find("tip").gameObject.SetActive(false);
			transform.Find("price").GetComponent<UILabel>().text = TidData.upgradeCost.ToString();
			transform.Find("bg").GetComponent<UISprite>().color = Color.white;
			transform.Find("avatar").GetComponent<UISprite>().color = Color.white;
			if(TidData.upgradeCost>DataManager.GetInstance().userInfo.gold_count)
				transform.Find("price").GetComponent<UILabel>().color = Color.red;
			else
				transform.Find("price").GetComponent<UILabel>().color = Color.white;
			transform.Find("Sprite").gameObject.SetActive(true);
		}

		transform.Find ("avatar").GetComponent<UISprite> ().spriteName = tidData.tid;

		UIButton infoBtn = transform.Find("info").GetComponent<UIButton>();
		infoBtn.onClick = new List<EventDelegate>();
		infoBtn.onClick.Add(new EventDelegate(this,"OnClickInfo"));

	}

	void OnClickResearch()
	{
		PopManage.Instance.ShowTroopUpgradeWin (TidData);
	}

	void OnClickInfo()
	{
		PopManage.Instance.ShowTroopInfoWin (TidData.tid_level,"Research");
	}

}
