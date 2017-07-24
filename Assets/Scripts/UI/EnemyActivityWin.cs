using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class EnemyActivityWin : MonoBehaviour {

	private Transform GridList;
	private GameObject cPrefab,cPrefabDay;
	private UIScrollView scrollView;
	private Transform btn_panel;
	private bool isInit;
	public Camera worldCamera;
	public Vector3 world_pos = Vector3.zero;
	public float movespeed = 0.02f;

	public Color normalColor1,normalColor2,selectedColor;
	public Color normal2Color1,normal2Color2,selected2Color;
	public void Init()
	{
		if(!isInit)
		{
			GridList = transform.Find ("ScrollView/GridList");
			scrollView = transform.Find ("ScrollView").GetComponent<UIScrollView>();
			cPrefab = Resources.Load ("UI/EnemyActivityItem") as GameObject;
			cPrefabDay = Resources.Load ("UI/EnemyActivityDay") as GameObject;

			while(GridList.childCount > 0){
				DestroyImmediate(GridList.GetChild(0).gameObject);
			}

			world_pos = Vector3.zero;

			//initData();
			InvokeRepeating("initData",0.2f,0);
			isInit = true;
		}


		/*
		if (Globals.AchievementsList.Count == 0){
			//主堡，金币库升级完，树，石头移除完 也需要重新刷新一下;
			ISFSObject data = new SFSObject();
			NetworkManager.Instance.SendRequest(data, "user_achievements", false, HandleResponse);
		}else{
			InvokeRepeating("initData",0.2f,0);
		}
*/

	}

	void initData(){
		//Debug.Log("initData");
		//bool is_zero = GridList.GetChildCount() == 0;

		//int select_index = 0;

		int i = 0;
		int top_y = 763;
		int i_day = 0;
		foreach(EnemyActivityItem item in Globals.EnemyActivityList.Values){

			//Transform achievement = GridList.FindChild(item.id.ToString());
			
			GameObject player = Instantiate(cPrefab) as GameObject;


			UISprite bg = player.transform.Find("bg").GetComponent<UISprite>();
			UISprite main_bg = player.transform.Find("main_bg").GetComponent<UISprite>();
			if (i % 2 == 0){
				bg.color = normalColor1;
				main_bg.color = normal2Color1;
			}else{
				bg.color = normalColor2;
				main_bg.color = normal2Color2;
			}
			//Debug.Log(bg.color);


			UISprite main_sprite = player.transform.Find("main_sprite").GetComponent<UISprite>();
			UILabel title_label = player.transform.Find("title_label").GetComponent<UILabel>();

			UILabel time_label = player.transform.Find("time_label").GetComponent<UILabel>();

			//TID_STREAM_ENTRY_AGE = <time> ago
			int itime = (int)(Helper.current_time() - item.attack_start_time);
			time_label.text = StringFormat.FormatByTid("TID_STREAM_ENTRY_AGE", new object[]{Helper.GetFormatTime(itime,2)});


			Transform diamond_sprite = player.transform.Find("diamond_sprite");

			if (item.already_claimed == 0 && item.reward_diamond > 0){
				diamond_sprite.gameObject.SetActive(true);
			}else{
				diamond_sprite.gameObject.SetActive(false);
			}

			Transform mark_sprite = player.transform.Find("mark_sprite");

			if (item.reward_count > 0){
				mark_sprite.gameObject.SetActive(true);
			}else{
				mark_sprite.gameObject.SetActive(false);
			}

			//time_label

			//regions_type 岛类型;0:无;1:主岛屿;2:资源岛(木);3:资源岛(石);4:资源岛(钢);5:自由岛;6:博士岛;
			if (item.regions_type == 1){				
				main_sprite.spriteName = "Headquarters";
			}else if (item.regions_type == 2){				
				main_sprite.spriteName = "Sawmill";
			}else if (item.regions_type == 3){				
				main_sprite.spriteName = "Quarry";
			}else if (item.regions_type == 4){				
				main_sprite.spriteName = "IronMine";
			}else if (item.regions_type == 5){				
				main_sprite.spriteName = "Residence";
			}else if (item.regions_type == 6){				
				main_sprite.spriteName = "Residence";
			}

			//TID_YOU_LOST_OUTPOST = Resource base lost
			//TID_YOU_LOST_BASE = Home base raided
			//TID_EMPIRE_ATTACKED_YOU = Village lost
			//TID_BASE_WAS_ATTACKED = Home base defended
			//TID_OUTPOST_WAS_ATTACKED = Resource base defended

			if (item.reward_count > 0){
				if (item.regions_type == 1){
					title_label.text = StringFormat.FormatByTid("TID_YOU_LOST_BASE");
				}else if (item.regions_type == 5){
					title_label.text = StringFormat.FormatByTid("TID_YOU_LOST_OUTPOST");
				}else{
					title_label.text = StringFormat.FormatByTid("TID_EMPIRE_ATTACKED_YOU");
				}
			}else{
				if (item.regions_type == 1){
					title_label.text = StringFormat.FormatByTid("TID_BASE_WAS_ATTACKED");
				}else if (item.regions_type == 6){
					title_label.text = StringFormat.FormatByTid("TID_EVENT_POPOVER_TITLE");
				}else{
					title_label.text = StringFormat.FormatByTid("TID_OUTPOST_WAS_ATTACKED");
				}

			}



			//title_label.text = item.u_name


			player.name = item.id.ToString();
			player.transform.parent = GridList;
			player.transform.localScale = new Vector3(1,1,1);

			top_y -= 130;
			
			
			
			player.transform.localPosition = new Vector3(327,top_y,0);
			
			UIDragScrollView drag = player.transform.GetComponent<UIDragScrollView>();
			drag.scrollView = scrollView;
			
			
			UIButtonMessage[] btn = player.transform.GetComponents<UIButtonMessage>();
			for(int k = 0; k < btn.Length; k ++){
				btn[k].target = this.gameObject;
			}
			//btn.trigger = UIButtonMessage.Trigger.OnPress;
			//btn.functionName = "OnClickItem";
			//btn.functionName = "OnPress2";

			int last_day = itime / 86400;
			if (last_day > i_day){
				i_day = last_day;
				top_y -= 50;
				player = Instantiate(cPrefabDay) as GameObject;

				title_label = player.transform.Find("title_label").GetComponent<UILabel>();
				//title_label.text = i_day.ToString();
				title_label.text = StringFormat.FormatByTid("TID_STREAM_ENTRY_AGE", new object[]{Helper.GetFormatTime(i_day*86400,2)});
				

				//player.name = item.TID;
				player.transform.parent = GridList;
				player.transform.localScale = new Vector3(1,1,1);
				
				player.transform.localPosition = new Vector3(327,top_y,0);
				drag = player.transform.GetComponent<UIDragScrollView>();
				drag.scrollView = scrollView;
			}

			i ++;
		}

		ClearSelected();

		scrollView.ResetPosition();

	}

	public void ClearSelected(){
		foreach(Transform t in GridList.transform){
			Transform st = t.Find("selected_bg");
			if (st != null){
				st.gameObject.SetActive(false);
			}
		}
	}

	void OnPress2 (GameObject sender){
		//Debug.Log("OnPress2:" + sender.name);
		WorldCameraOpEvent.Instance.Status = false;
	}

	void OnRelease2 (GameObject sender){
		//Debug.Log("OnRelease2:" + sender.name);	
		OnClickItem(sender);
		WorldCameraOpEvent.Instance.Status = true;
	}

	void OnPress (bool isPressed){
		//Debug.Log("OnPress:" + isPressed.ToString());
		if (isPressed){
			WorldCameraOpEvent.Instance.Status = false;
		}else{

			WorldCameraOpEvent.Instance.Status = true;
		}

	}


	public void OnClickItem(GameObject sender){
		ClearSelected();
		Debug.Log(sender.name);
		EnemyActivityItem item = Globals.EnemyActivityList[int.Parse(sender.name)] as EnemyActivityItem;

		if (item.regions_id > 0){
			Transform island = PopManage.Instance.WorldMap.transform.Find("Island/" + item.regions_id.ToString());
			if (island != null){
				world_pos = new Vector3(island.position.x-0.18f,island.position.y,worldCamera.transform.position.z);
			}else{
				world_pos = Vector3.zero;
			}
		}

		Transform st = sender.transform.Find("selected_bg");
		st.gameObject.SetActive(true);

        if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.showEnemyBox(item,world_pos);
		Debug.Log(sender.name + ";world_pos:" + world_pos);
	}

	void Update(){
		if (world_pos != Vector3.zero && world_pos != worldCamera.transform.position){		
			worldCamera.transform.position = Vector3.MoveTowards(worldCamera.transform.position,world_pos,movespeed*Globals.TimeRatio);
		}
		if (world_pos != Vector3.zero && world_pos == worldCamera.transform.position){
			world_pos = Vector3.zero;
		}
	}



	public void OnClaimReward(GameObject sender){

		UILabel res_num = sender.transform.parent.Find("DiamondShow/res_num").GetComponent<UILabel>();
		//Debug.Log("tid:" + sender.transform.parent.name + ";Gems:" + res_num.text);


		UIButtonMessage btn = sender.transform.GetComponent<UIButtonMessage>();
		btn.enabled = false;

		//Helper.setResourceCount("Gems",int.Parse(res_num.text),false,true);

		int num = int.Parse (res_num.text);
		//GameObject.Find ("UI Root (2D)").GetComponent<GemPartEmitter> ().Emit (num, sender.transform.position, num);
        GemPartEmitter.Instance().Emit(num, sender.transform.position, num);
        

        //ISFSObject data = new SFSObject();
        //data.PutUtfString("tid",sender.transform.parent.name);
        //		NetworkManager.Instance.SendRequest(data, "user_achievements", false, HandleResponse);

    }

	public void Close(){
		Debug.Log("close");
		gameObject.SetActive(false);
	}


}
