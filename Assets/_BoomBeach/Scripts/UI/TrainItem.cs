using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BoomBeach{
	public class TrainItem : MonoBehaviour {
		private TrainTid TidData;
		public void BindData(TrainTid tidData)
		{
			TidData = tidData;
			if(TidData.hasTrain == null&&TidData.trainNum>0)
			{
				//可以训练;
				transform.Find("Number").GetComponent<UILabel>().text = TidData.trainNum.ToString()+"X";
				transform.Find("TrainIcos").gameObject.SetActive(true);
				transform.Find("Reason").gameObject.SetActive(false);
				UILabel goldLabel = transform.Find("TrainIcos/goldLabel").GetComponent<UILabel>();
				goldLabel.text = TidData.trainCost.ToString();
				if(TidData.trainCost>DataManager.GetInstance().userInfo.gold_count)
				{
					goldLabel.color = Color.red;
				}
				else
				{
					goldLabel.color = Color.white;
				}
				transform.Find("TrainIcos/clockLabel").GetComponent<UILabel>().text = TidData.trainTimeFormat;
				transform.Find("bg").GetComponent<UISprite>().color = Color.white;
				transform.Find("avatar").GetComponent<UISprite>().color = Color.white;

				UIButton btn = transform.GetComponent<UIButton>();
				btn.enabled = true;
				btn.onClick = new List<EventDelegate>();
				btn.onClick.Add(new EventDelegate(this,"OnClickTrain"));

			}
			else
			{
				if(!Helper.isUnLock(TidData.tid_level))
				{
					//灰;
					transform.Find("bg").GetComponent<UISprite>().color = Color.black;
					transform.Find("avatar").GetComponent<UISprite>().color = Color.black;
				}
				else
				{
					//淡;
					transform.Find("bg").GetComponent<UISprite>().color = new Color(1f,1f,1f,0.3f);
					transform.Find("avatar").GetComponent<UISprite>().color = new Color(1f,1f,1f,0.7f);
				}

				transform.Find("Number").GetComponent<UILabel>().text = "";
				transform.Find("TrainIcos").gameObject.SetActive(false);
				transform.Find("Reason").gameObject.SetActive(true);
				transform.Find("Reason").GetComponent<UILabel>().text = TidData.hasTrain;
				UIButton btn = transform.GetComponent<UIButton>();
				btn.enabled = false;
				btn.onClick = new List<EventDelegate>();
			}

			transform.Find ("avatar").GetComponent<UISprite> ().spriteName = tidData.tid;
			UIButton infoBtn = transform.Find("info").GetComponent<UIButton>();
			infoBtn.onClick = new List<EventDelegate>();
			infoBtn.onClick.Add(new EventDelegate(this,"OnClickInfo"));
		}

		void OnClickInfo()
		{
			PopManage.Instance.ShowTroopInfoWin (TidData.tid_level,"Train");
		}

		void OnClickTrain()
		{
			Debug.Log ("OnClickTrain");
			//BuildInfo.OnStartTrain(TidData.buildInfo,TidData.tid);
			TrainHandle.OnTrain (TidData.buildInfo,TidData.tid);
		}
	}
}
