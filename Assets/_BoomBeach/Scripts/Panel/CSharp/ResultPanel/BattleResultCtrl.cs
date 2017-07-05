using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sfs2X.Entities.Data;

namespace BoomBeach
{
    public class BattleResultCtrl : BaseCtrl
    {

        ResultPanelView mResultPanelView;
		LayoutElement[] deadTroopItems;

		public override void ShowPanel()
        {
            bool isCreate;
			mResultPanelView = UIMgr.ShowPanel<ResultPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
			UIMgr.GetController<BattleInterfaceCtrl>().Close ();
			InitReward (DataManager.GetInstance ().battleData);
			UIMgr.GetController<MaskCtrl>().ShowPanel(null);
        }

        void OnCreatePanel()
        {
			deadTroopItems = mResultPanelView.m_containerTroop.GetComponentsInChildren<LayoutElement> ();
			mResultPanelView.m_btnReturn.onClick.AddListener (OnClickReturn);
        }

		void InitReward(ISFSObject dt){

			//reset 
			mResultPanelView.m_containerRewardgold.SetActive (false);
			mResultPanelView.m_containerRewardwood.SetActive (false);
			mResultPanelView.m_containerRewardstone.SetActive (false);
			mResultPanelView.m_containerRewardiron.SetActive (false);
			mResultPanelView.m_containerRewardmedal.SetActive (false);
			mResultPanelView.m_containerRewardpiece.SetActive (false);
			foreach(LayoutElement ele in deadTroopItems)
			{
				ele.gameObject.SetActive (false);
			}
			//set reward
			//ISFSObject user = dt.GetSFSObject ("user");
			int resourceCount = 0;
			if(BattleData.Instance.BattleIsSuccess){
				int loot_boot = dt.GetInt("loot_boot");
				
				if (dt.GetInt ("loot_gold") > 0) {
					mResultPanelView.m_containerRewardgold.SetActive (true);
					int count = dt.GetInt ("loot_gold") + (int)(dt.GetInt ("loot_gold") * loot_boot / 100f);
					mResultPanelView.m_containerRewardgold.transform.FindChild ("Text").GetComponent<Text>().text = count.ToString();
					resourceCount++;
				}
				if (dt.GetInt("loot_wood") > 0) {
					mResultPanelView.m_containerRewardwood.SetActive (true);
							int count = dt.GetInt ("loot_wood") + (int)(dt.GetInt ("loot_wood") * loot_boot / 100f);
					mResultPanelView.m_containerRewardwood.transform.FindChild ("Text").GetComponent<Text>().text = count.ToString();
					resourceCount++;
				}
				if (dt.GetInt("loot_stone") > 0) {
					mResultPanelView.m_containerRewardstone.SetActive (true);
									int count = dt.GetInt ("loot_stone") + (int)(dt.GetInt ("loot_stone") * loot_boot / 100f);
					mResultPanelView.m_containerRewardstone.transform.FindChild ("Text").GetComponent<Text>().text = count.ToString();
					resourceCount++;
				}
				if (dt.GetInt("loot_iron") > 0) {
					mResultPanelView.m_containerRewardiron.SetActive (true);
					int count = dt.GetInt ("loot_iron") + (int)(dt.GetInt ("loot_iron") * loot_boot / 100f);
					mResultPanelView.m_containerRewardiron.transform.FindChild ("Text").GetComponent<Text>().text = count.ToString();
					resourceCount++;
				}
				int medal = dt.GetInt ("pal_reward") + dt.GetInt ("add_reward");
				if (medal > 0) {
					mResultPanelView.m_containerRewardmedal.SetActive (true);
					mResultPanelView.m_containerRewardmedal.transform.FindChild ("Text").GetComponent<Text>().text = medal.ToString();
					resourceCount++;
				}
				//Artifacts TODO 需要定义一下
				//int artifactType = dt.GetInt ("artifact");//0 Piece 1 Ice 2 Fire 3 Dark
				mResultPanelView.m_containerReward.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100 * resourceCount + 20 * (resourceCount-1),100);
			}		
			//set dead troops
			Dictionary<string,int> deadList = new Dictionary<string, int> ();
			foreach(CharInfo c in BattleData.Instance.DeadTrooperList)
			{
				if(!deadList.ContainsKey(c.trooperData.tid))
				{
					deadList.Add(c.trooperData.tid,0);
				}	
				deadList[c.trooperData.tid]++;
			}
			int i = 0;
			foreach(string k in deadList.Keys)
			{
				if (deadTroopItems.Length <= i) {
					break;
				} else {
					deadTroopItems [i].gameObject.SetActive (true);
					deadTroopItems [i].transform.FindChild ("Image").GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.avaterSpriteDic[k];
					deadTroopItems [i].transform.Find ("Text").GetComponent<Text> ().text = deadList [k].ToString ();
				}
				i++;
			}
			//胜利失败标题
			if (BattleData.Instance.BattleIsSuccess) {
				mResultPanelView.m_txtBattlewin.gameObject.SetActive (true);
				mResultPanelView.m_txtBattlefail.gameObject.SetActive (false);
			} else {
				mResultPanelView.m_txtBattlewin.gameObject.SetActive (false);
				mResultPanelView.m_txtBattlefail.gameObject.SetActive (true);
			}
			//是否显示描述文本
			if (resourceCount > 0)
				mResultPanelView.m_txtReward.gameObject.SetActive (false);
			else
				mResultPanelView.m_txtReward.gameObject.SetActive (true);
			if (deadList.Count > 0)
				mResultPanelView.m_txtTroop.gameObject.SetActive (false);
			else
				mResultPanelView.m_txtTroop.gameObject.SetActive (true);

		}

        public override void Close()
        {
            UIMgr.ClosePanel("ResultPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }

		bool is_server_return = false;
		void OnClickReturn()
		{
			is_server_return = true;
			if (is_server_return){
				if (BattleData.Instance.BattleIsSuccess){
					UserRegions ur = DataManager.GetInstance().userRegionsList[Globals.LastSceneRegionsId] as UserRegions;
					if (ur != null && (ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE")){
						//攻击胜利，如果是资源岛屿的话,则重新返回到该资源岛屿;
						GameLoader.Instance.SwitchScene(SceneStatus.HOMERESOURCE,DataManager.GetInstance().userInfo.id,Globals.LastSceneRegionsId,0,0);
					}else{
						Globals.LastSceneUserId = -1;
						Globals.LastSceneRegionsId = -1;
						GameLoader.Instance.SwitchScene(SceneStatus.WORLDMAP);
						AudioPlayer.Instance.PlayMusic("home_music");
					}
				}else{
					Globals.LastSceneUserId = -1;
					Globals.LastSceneRegionsId = -1;
					GameLoader.Instance.SwitchScene(SceneStatus.WORLDMAP);
					AudioPlayer.Instance.PlayMusic("home_music");
				}
				UIMgr.GetController<MainInterfaceCtrl>().ShowWorld ();
				Close ();
				CloseMask ();
			}
		}
    }
}
