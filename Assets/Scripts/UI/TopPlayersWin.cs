using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace BoomBeach{
	public class TopPlayersWin : MonoBehaviour {
		private bool isInit;
		private Transform GridList;
		private GameObject cPrefab;
		private UIScrollView scrollView;
		private Transform btn_panel;
		private UIAnchor btn_panel_anchor;
		private int select_user_id = 0;
		public void Init()
		{
			if(!isInit)
			{
				GridList = transform.Find ("players_panel/GridList");
				scrollView = transform.Find ("players_panel").GetComponent<UIScrollView>();
				cPrefab = Resources.Load ("UI/top_player") as GameObject;

				while(GridList.childCount > 0){
					DestroyImmediate(GridList.GetChild(0).gameObject);
				}

				btn_panel = transform.Find ("players_panel/btn_panel");
				btn_panel_anchor = transform.Find ("players_panel/btn_panel").GetComponent<UIAnchor>();
				isInit = true;
			}

			if (GridList.childCount == 0){
				ISFSObject data = new SFSObject();
				SFSNetworkManager.Instance.SendRequest(data, "top_players", false, HandleResponse);
			}
			btn_panel.gameObject.SetActive(false);
		}

		void HandleResponse(ISFSObject dt,BuildInfo buildInfo)
		{
			ISFSArray user_list2 = Helper.SFSObjToArr(dt.GetSFSObject("user_list2"));
			while(GridList.childCount > 0){
				DestroyImmediate(GridList.GetChild(0).gameObject);
			}

			//Debug.Log(user_list2.GetDump());

			for(int i = 0; i < user_list2.Count; i ++){
				int top = i + 1;
				ISFSObject item = user_list2.GetSFSObject(i);
				GameObject player = Instantiate(cPrefab) as GameObject;

				UIEventListener.Get(player).onDrag = HideBtnPanel;


				player.name = item.GetInt("id").ToString();// top.ToString();
				player.transform.parent = GridList;
				player.transform.localScale = new Vector3(1,1,1);
				float y = 363-125*i;
				//Debug.Log(y);
				player.transform.localPosition = new Vector3(0,y,0);

				UILabel top_label = player.transform.Find("top_label").GetComponent<UILabel>();
				top_label.text = top.ToString() + ".";


				UILabel lv_label = player.transform.Find("lv_label").GetComponent<UILabel>();
				lv_label.text = item.GetInt("exp_level").ToString();

				UILabel user_name = player.transform.Find("user_label").GetComponent<UILabel>();
				user_name.text = item.GetUtfString("user_name");

	//			GameObject hint_sprite = player.transform.Find("hint_sprite").gameObject;
				//hint_sprite.transform.localPosition = new Vector3(100,0,0);


				UILabel reward_count = player.transform.Find("reward_label").GetComponent<UILabel>();
				reward_count.text = item.GetInt("reward_count").ToString();

				UIButtonMessage btn = player.transform.GetComponent<UIButtonMessage>();
				btn.target = this.gameObject;

				UIDragScrollView drag = player.transform.GetComponent<UIDragScrollView>();
				drag.scrollView = scrollView;

				if (DataManager.GetInstance.model.user_info.id == item.GetInt("id")){
					player.transform.Find("bg_sprite").GetComponent<UISprite>().spriteName = "blue_item";
				}else{
					if (i % 2 == 0)
						player.transform.Find("bg_sprite").gameObject.SetActive(true);
					else
						player.transform.Find("bg_sprite").gameObject.SetActive(false);
				}
			}

			//GridList.GetComponent<UIGrid>().Reposition();
		}

		public void HideBtnPanel(GameObject obj, Vector2 vec2){
			btn_panel.gameObject.SetActive(false);
		}

		public void OnClickItem(GameObject sender)
		{
			//Debug.Log("OnClickItem:" + sender.name);

			if (DataManager.GetInstance.model.user_info.id == int.Parse(sender.name)){
				btn_panel.gameObject.SetActive(false);
				select_user_id = 0;
			}else{
				btn_panel.gameObject.SetActive(true);
				//btn_panel.name = sender.name;
				select_user_id = int.Parse(sender.name);
				GameObject hint_sprite = sender.transform.Find("hint_sprite").gameObject;

				btn_panel_anchor.enabled = true;
				btn_panel_anchor.container = hint_sprite;
			}

		}

		public void OnVisit()
		{
			Debug.Log("select_user_id:" + select_user_id);
			PopManage.Instance.popWin.CloseTween();
			GameLoader.Instance.SwitchScene(SceneStatus.FRIENDVIEW,select_user_id);
		}
	}
}
