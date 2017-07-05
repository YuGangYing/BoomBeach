using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class AchievementsWin : MonoBehaviour {

	private Transform GridList;
	private GameObject cPrefab;
	private UIScrollView scrollView;
	private Transform btn_panel;
	private bool isInit;


	public void Init()
	{
		if(!isInit)
		{
			GridList = transform.Find ("ScrollView/GridList");
			scrollView = transform.Find ("ScrollView").GetComponent<UIScrollView>();
			cPrefab = Resources.Load ("UI/AchievementsItem") as GameObject;

			while(GridList.childCount > 0){
				DestroyImmediate(GridList.GetChild(0).gameObject);
			}

			isInit = true;
		}

		if (CSVManager.GetInstance().achievementsList.Count == 0){
			//主堡，金币库升级完，树，石头移除完 也需要重新刷新一下;
			ISFSObject data = new SFSObject();
			SFSNetworkManager.Instance.SendRequest(data, "user_achievements", false, HandleResponse);
		}else{
			InvokeRepeating("initData",0.2f,0);
		}

		//initData();
	}

	void initData(){
		//Debug.Log("initData");
		bool is_zero = GridList.childCount == 0;

		int select_index = 0;

		int i = 0;
		foreach(AchievementItem item in CSVManager.GetInstance().achievementsList.Values){
			
			
			Transform achievement = GridList.FindChild(item.TID);
			
			GameObject player = null;
			
			if (achievement == null)
				player = Instantiate(cPrefab) as GameObject;
			else
				player = achievement.gameObject;
			
			player.name = item.TID;
			player.transform.parent = GridList;
			player.transform.localScale = new Vector3(1,1,1);
			float x = 400-650*i;
			//Debug.Log(y);
			player.transform.localPosition = new Vector3(x,0,0);
			
			UIDragScrollView drag = player.transform.GetComponent<UIDragScrollView>();
			drag.scrollView = scrollView;
			
			GameObject ClaimBtn = player.transform.Find("ClaimBtn").gameObject;
			
			Transform ProgressBar = player.transform.Find("ProgressBar");
			int cur_count = item.cur_count;
			int ActionCount = item.ActionCount;
			
			//完成;
			GameObject Completed = player.transform.Find("WhiteDescBox/Completed").gameObject;
			
			
			UILabel res_num = player.transform.Find("DiamondShow/res_num").GetComponent<UILabel>();
			res_num.text = item.DiamondReward.ToString();// item.GetInt("DiamondReward").ToString();
			
			
			
			//描述;
			UILabel desc = player.transform.Find("WhiteDescBox/Label").GetComponent<UILabel>();
			desc.text = StringFormat.FormatByTid(item.InfoTID,new object[]{ActionCount.ToString()});
			
			
			UILabel user_name = player.transform.Find("Name").GetComponent<UILabel>();
			user_name.text = StringFormat.FormatByTid(item.TID);
			
			GameObject star1_sprite = player.transform.Find("star1_sprite").gameObject;
			GameObject star2_sprite = player.transform.Find("star2_sprite").gameObject;
			GameObject star3_sprite = player.transform.Find("star3_sprite").gameObject;
			star1_sprite.SetActive(false);
			star2_sprite.SetActive(false);
			star3_sprite.SetActive(false);
			long Level = item.Level;
			
			if (Level == 0){
				star1_sprite.SetActive(false);
				star2_sprite.SetActive(false);
				star3_sprite.SetActive(false);
			}else if (Level == 1){
				star1_sprite.SetActive(true);
				star2_sprite.SetActive(false);
				star3_sprite.SetActive(false);
			}else if (Level == 2){
				star1_sprite.SetActive(true);
				star2_sprite.SetActive(true);
				star3_sprite.SetActive(false);
			}else if (Level == 3){
				star1_sprite.SetActive(true);
				star2_sprite.SetActive(true);
				star3_sprite.SetActive(true);
			}
			
			if (cur_count < ActionCount){
				Completed.SetActive(false);
				ClaimBtn.SetActive(false);
				ProgressBar.gameObject.SetActive(true);
				//进度条;
				//UISlider prs = ProgressBar.GetComponent<UISlider>();
				//prs.value = cur_count*1f / ActionCount*1f;
				Transform bar = ProgressBar.Find("Foreground");
				Vector3 barScale = bar.localScale;
				bar.localScale = new Vector3(cur_count*1f / ActionCount*1f,barScale.y,barScale.z);
				
				UILabel prs_value = ProgressBar.Find("prs_value").GetComponent<UILabel>();
				
				prs_value.text = cur_count.ToString() + "/" + ActionCount.ToString();
			}else{

				
				
				
				if (Level < 3 && cur_count >= ActionCount){
					Completed.SetActive(false);
					ClaimBtn.SetActive(true);
					UIButtonMessage btn = ClaimBtn.transform.GetComponent<UIButtonMessage>();
					btn.enabled = true;
					btn.target = this.gameObject;
					ProgressBar.gameObject.SetActive(false);
					//Debug.Log("i:" + i);
					//if (select_index == 0){
						//定位到当前可以领取的：成就;
						select_index = i;
					//}
					
				}else{
					Completed.SetActive(true);
					ClaimBtn.SetActive(false);
					
					ProgressBar.gameObject.SetActive(true);
					UILabel prs_value = ProgressBar.Find("prs_value").GetComponent<UILabel>();					
					prs_value.text = ActionCount.ToString() + "/" + ActionCount.ToString();
				}

				
			}

			//if (item.TID == "TID_ACHIEVEMENT_GOLDSTORAGE_TITLE"){
				//Debug.Log(StringFormat.FormatByTid(item.TID));
				//select_index = i;
			//}

			i ++;			
		}

		//select_index = 5;

		if (is_zero && i > 0){
			scrollView.ResetPosition();
			select_index = i - select_index;

			float x = select_index *1.0f / i*1.0f;// -0.2f;
			//Debug.Log("select_index:" + select_index + ";achievements.Count:" + i + ";x:" + x);
			scrollView.SetDragAmount(x, 0f, false);
		}
	}


	void HandleResponse(ISFSObject dt,BuildInfo buildInfo)
	{
		int num = Helper.setAchievementsList(dt.GetSFSObject("achievements"));
        if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.SetAchievementCount(num);
		initData();
	}

	public void OnClaimReward(GameObject sender){

		UILabel res_num = sender.transform.parent.Find("DiamondShow/res_num").GetComponent<UILabel>();
		//Debug.Log("tid:" + sender.transform.parent.name + ";Gems:" + res_num.text);


		UIButtonMessage btn = sender.transform.GetComponent<UIButtonMessage>();
		btn.enabled = false;

		//Helper.setResourceCount("Gems",int.Parse(res_num.text),false,true);

		int num = int.Parse (res_num.text);

        //GemPartEmitter.Instance().
        GemPartEmitter.Instance().Emit (num, sender.transform.position, num);


		ISFSObject data = new SFSObject();
		data.PutUtfString("tid",sender.transform.parent.name);
		SFSNetworkManager.Instance.SendRequest(data, "user_achievements", false, HandleResponse);

	}



}
