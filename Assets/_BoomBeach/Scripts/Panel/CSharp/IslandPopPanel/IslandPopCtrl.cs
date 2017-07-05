//==========================================
// Created By yingyugang At 2/21/2016 11:18:00 AM
//==========================================
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoomBeach
{
    ///<summary>
    ///
    ///</summary>
    public class IslandPopCtrl : BaseCtrl
    {

        IslandPopPanelView mIslandPopPanelView;
        Camera worldCamera;

		public override void ShowPanel()
        {
            bool isCreate;
            mIslandPopPanelView = UIMgr.ShowPanel<IslandPopPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
            //UIMgr.maskCtrl.ShowPanel(new UnityEngine.Events.UnityAction(Close));
        }

        int GetShowType(UserRegions ur)
        {
            int show_type = 0;
            //宝箱
            if (ur.is_npc == 9)
            {
                if (ur.sending == false)
                {
                    ur.sending = true;
                    ISFSObject data = new SFSObject();
                    data.PutInt("id", ur.id);
                    data.PutInt("regions_id", ur.regions_id);
                    //TODO
                    //NetworkManagerOld.Instance.SendRequest(data, "collect_treasure", false, HandleCollectTreasureResponse);
                }
            }
            else
            {
                //show_type;
                //1:go home;
                //11:Production per hour and go resource;
                //2:Scout,Attack;
                //21:Scout,Attack,Resource; 
                //22:Scout,Attack,Resource,Per hour; 
                //3:Scout,Attack,Find; 
                //31:Scout,Attack,Find,Resource; 
                //32:Scout,Attack,Find,Resource,Per hour; 
                if (ur.capture_id == DataManager.GetInstance().userInfo.id)//自己
                {
                    if (ur.res_tid == null || ur.res_tid == "" ||ur.regions_id == 0 || ur.regions_id == 1)
                    {
                        show_type = 1;//基地
                    }
                    else if (ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE")
                    {
                        show_type = 11;//自己的资源岛
                    }
                }
                else//npc或者其他玩家
                {
                    bool isSearched = false;//是否侦察过 TODO
                    bool isChangeable = false;//是否可以更换
                    bool isResource = false;//是否是资源岛
                    bool isNpc = false;//是否是NPC
                    if (Helper.current_time() - ur.capture_time > Globals.FindNewOpponentTime)
                    {
                        isChangeable = true;
                    }
                    if (ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE")
                    {
                        isResource = true;
                    }
                    if (ur.is_npc ==1)
                    {
                        isNpc = true;
                    }
                    if (!isSearched)
                    {
                        if (!isNpc && isChangeable)
                        {
                            show_type = 3;
                        }
                        else
                        {
                            show_type = 2;
                        }
                    }
                    else
                    {
                        if (!isNpc && isChangeable)
                        {
                            if (isResource)
                            {
                                show_type = 32;
                            }
                            else
                            {
                                show_type = 31;
                            }
                        }
                        else
                        {
                            if (isResource)
                            {
                                show_type = 22;
                            }
                            else
                            {
                                show_type = 21;
                            }
                        }
                    }
                }
            }
            return show_type;
        }

        Regions mRegions;
        //展开云层
        public void ShowCloudPop(Regions regions, Vector3 sreenPoint)
        {
            if (regions.sending == false)
            {
				ShowPanel();
                mIslandPopPanelView.m_containerIslandpopbox1.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox11.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox2.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox21.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox22.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox3.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox31.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox32.SetActive(false);
                mIslandPopPanelView.m_containerIslandpopbox.SetActive(true);
                Text goldText = mIslandPopPanelView.m_containerIslandpopbox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_islandpopgold").GetComponent<Text>();
                goldText.text = regions.ExplorationCost.ToString();

                GameObject currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox;
                GameObject currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                //float width = currentIslandPopBox.GetComponent<RectTransform>().sizeDelta.x;
                float height = currentIslandPopBox.GetComponent<RectTransform>().sizeDelta.y;
                //float screenWidth = Screen.width;
                //float screenHeight = Screen.height;
                Vector3 localPos = sreenPoint - new Vector3(Screen.width / 2, Screen.height / 2, 0);
                //float localWidth = localPos.x - screenWidth;
                //float localHeight = localPos.y - screenHeight;
                float y = 0;
                float offsetY = 10;
                float arrowY;
                if (localPos.y > 0)
                {
                    y = -height / 2 - offsetY / 2;
                    arrowY = height / 2 + offsetY;
                    currentArrow.transform.localEulerAngles = new Vector3(0, 0, 180);
                }
                else
                {
                    y = height / 2 + offsetY / 2;
                    arrowY = -height / 2 - offsetY;
                    currentArrow.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                currentIslandPopBox.transform.localPosition = localPos + new Vector3(0, y, 0);
                currentArrow.transform.localPosition = new Vector3(0, arrowY, 0);
                currentIslandPopBox.SetActive(true);
                this.mRegions = regions;
            }
            else
            {
                Debug.Log("regions is sending!");
            }
        }

        UserRegions mUserRegions;
        //显示大地图的弹出面板
        public void ShowIslandPop(UserRegions ur, Vector3 sreenPoint)
        {
			ShowPanel();
            int show_type = GetShowType(ur);

            //关闭所有自面板
            mIslandPopPanelView.m_containerIslandpopbox.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox1.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox11.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox2.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox21.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox22.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox3.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox31.SetActive(false);
            mIslandPopPanelView.m_containerIslandpopbox32.SetActive(false);

            GameObject currentIslandPopBox = null;
            GameObject currentArrow = null;
            #region 1.显示数据
            if (show_type == 1)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox1;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>().text = StringFormat.FormatByTid("TID_BUTTON_GO_TO_OUTPOST");

            }
            else if (show_type == 11)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox11;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>().text = StringFormat.FormatByTid("TID_BUTTON_GO_TO_OUTPOST");
                Text resource = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandperhour/txt_islandperhour").GetComponent<Text>();
                resource.text = ur.resource_perhour.ToString();
            }
            else if (show_type == 2)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox2;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
                Text search = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>();
                search.text = StringFormat.FormatByTid("TID_SCOUT_BUTTON");
                Text AttackGoldNum = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandattack/txt_islandpopgold").GetComponent<Text>();
                AttackGoldNum.text = el.AttackCost.ToString();
            }
            else if (show_type ==21)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox21;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
                Text search = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>();
                search.text = StringFormat.FormatByTid("TID_SCOUT_BUTTON");
                Text AttackGoldNum = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandattack/txt_islandpopgold").GetComponent<Text>();
                AttackGoldNum.text = el.AttackCost.ToString();
                LayoutElement[] eles = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandresource").GetComponentsInChildren<LayoutElement>();
                eles[0].GetComponentInChildren<Text>().text = ur.gold.ToString();
                eles[1].GetComponentInChildren<Text>().text = ur.wood.ToString();
                eles[2].GetComponentInChildren<Text>().text = ur.stone.ToString();
                eles[3].GetComponentInChildren<Text>().text = ur.iron.ToString();
            }
            else if (show_type == 22)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox22;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
                Text search = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>();
                search.text = StringFormat.FormatByTid("TID_SCOUT_BUTTON");
                Text AttackGoldNum = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandattack/txt_islandpopgold").GetComponent<Text>();
                AttackGoldNum.text = el.AttackCost.ToString();
                LayoutElement[] eles = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandresource").GetComponentsInChildren<LayoutElement>();
                eles[0].GetComponentInChildren<Text>().text = ur.gold.ToString();
                eles[1].GetComponentInChildren<Text>().text = ur.wood.ToString();
                eles[2].GetComponentInChildren<Text>().text = ur.stone.ToString();
                eles[3].GetComponentInChildren<Text>().text = ur.iron.ToString();
                Text resource = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandperhour/txt_islandperhour").GetComponent<Text>();
                resource.text = ur.resource_perhour.ToString();
            }

            else if (show_type == 3)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox3;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
                Text search = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>();
                search.text = StringFormat.FormatByTid("TID_SCOUT_BUTTON");
                Text AttackGoldNum = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandattack/txt_islandpopgold").GetComponent<Text>();
                AttackGoldNum.text = el.AttackCost.ToString();
            }
            else if (show_type == 31)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox3;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
                Text search = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>();
                search.text = StringFormat.FormatByTid("TID_SCOUT_BUTTON");
                Text AttackGoldNum = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandattack/txt_islandpopgold").GetComponent<Text>();
                AttackGoldNum.text = el.AttackCost.ToString();
                LayoutElement[] eles = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandresource").GetComponentsInChildren<LayoutElement>();
                eles[0].GetComponentInChildren<Text>().text = ur.gold.ToString();
                eles[1].GetComponentInChildren<Text>().text = ur.wood.ToString();
                eles[2].GetComponentInChildren<Text>().text = ur.stone.ToString();
                eles[3].GetComponentInChildren<Text>().text = ur.iron.ToString();
            }
            else if (show_type == 32)
            {
                currentIslandPopBox = mIslandPopPanelView.m_containerIslandpopbox3;
                currentArrow = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_arrow").gameObject;
                currentIslandPopBox.SetActive(true);
                ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
                Text search = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch/txt_confirm").GetComponent<Text>();
                search.text = StringFormat.FormatByTid("TID_SCOUT_BUTTON");
                Text AttackGoldNum = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/btn_islandattack/txt_islandpopgold").GetComponent<Text>();
                AttackGoldNum.text = el.AttackCost.ToString();
                LayoutElement[] eles = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandresource").GetComponentsInChildren<LayoutElement>();
                eles[0].GetComponentInChildren<Text>().text = ur.gold.ToString();
                eles[1].GetComponentInChildren<Text>().text = ur.wood.ToString();
                eles[2].GetComponentInChildren<Text>().text = ur.stone.ToString();
                eles[3].GetComponentInChildren<Text>().text = ur.iron.ToString();
                Text resource = currentIslandPopBox.transform.FindChild("container_islandpopboxpivot/container_islandperhour/txt_islandperhour").GetComponent<Text>();
                resource.text = ur.resource_perhour.ToString();
            }
            #endregion
            #region 2.计算弹出位置
            //float width = currentIslandPopBox.GetComponent<RectTransform>().sizeDelta.x;
            float height = currentIslandPopBox.GetComponent<RectTransform>().sizeDelta.y;
            //float screenWidth = Screen.width;
            //float screenHeight = Screen.height;
            Vector3 localPos = sreenPoint - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            //float localWidth = localPos.x - screenWidth;
            //float localHeight = localPos.y - screenHeight;
            float y = 0;
            float offsetY = 20;
            float arrowY;
            if (localPos.y > 0)
            {
                y = -height / 2 - offsetY / 2;///
                arrowY = height / 2 + offsetY/2;
                currentArrow.transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                y = height / 2 + offsetY / 2;
                arrowY = -height / 2 - offsetY/2;
                currentArrow.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

			//Camera ca = GameObject.Find("GuiCamera").GetComponent<Camera>();
			//sreenPoint = ca.ScreenToViewportPoint(sreenPoint);
			//currentIslandPopBox.transform.position = sreenPoint;
			currentIslandPopBox.transform.localPosition = sreenPoint * 1334.0f/Screen.width + new Vector3(0, y, 0);
            currentArrow.transform.localPosition = new Vector3(0, arrowY, 0);
            currentIslandPopBox.SetActive(true);
            #endregion

            mUserRegions = ur;
        }

        void OnCreatePanel()
        {
            Button button;
            button = mIslandPopPanelView.m_containerIslandpopbox.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnExplore);

            button = mIslandPopPanelView.m_containerIslandpopbox1.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnGoHome);
            button = mIslandPopPanelView.m_containerIslandpopbox11.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnGoHome);
            button = mIslandPopPanelView.m_containerIslandpopbox2.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnSearch);
            button = mIslandPopPanelView.m_containerIslandpopbox2.transform.FindChild("container_islandpopboxpivot/btn_islandattack").GetComponent<Button>();
            button.onClick.AddListener(OnAttack);
            button = mIslandPopPanelView.m_containerIslandpopbox21.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnSearch);
            button = mIslandPopPanelView.m_containerIslandpopbox21.transform.FindChild("container_islandpopboxpivot/btn_islandattack").GetComponent<Button>();
            button.onClick.AddListener(OnAttack);
            button = mIslandPopPanelView.m_containerIslandpopbox22.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnSearch);
            button = mIslandPopPanelView.m_containerIslandpopbox22.transform.FindChild("container_islandpopboxpivot/btn_islandattack").GetComponent<Button>();
            button.onClick.AddListener(OnAttack);
            button = mIslandPopPanelView.m_containerIslandpopbox3.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnSearch);
            button = mIslandPopPanelView.m_containerIslandpopbox3.transform.FindChild("container_islandpopboxpivot/btn_islandattack").GetComponent<Button>();
            button.onClick.AddListener(OnAttack);
            button = mIslandPopPanelView.m_containerIslandpopbox3.transform.FindChild("container_islandpopboxpivot/btn_islandchange").GetComponent<Button>();
            button.onClick.AddListener(OnChange);
            button = mIslandPopPanelView.m_containerIslandpopbox31.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnSearch);
            button = mIslandPopPanelView.m_containerIslandpopbox31.transform.FindChild("container_islandpopboxpivot/btn_islandattack").GetComponent<Button>();
            button.onClick.AddListener(OnAttack);
            button = mIslandPopPanelView.m_containerIslandpopbox31.transform.FindChild("container_islandpopboxpivot/btn_islandchange").GetComponent<Button>();
            button.onClick.AddListener(OnChange);
            button = mIslandPopPanelView.m_containerIslandpopbox32.transform.FindChild("container_islandpopboxpivot/btn_islandsearch").GetComponent<Button>();
            button.onClick.AddListener(OnSearch);
            button = mIslandPopPanelView.m_containerIslandpopbox32.transform.FindChild("container_islandpopboxpivot/btn_islandattack").GetComponent<Button>();
            button.onClick.AddListener(OnAttack);
            button = mIslandPopPanelView.m_containerIslandpopbox32.transform.FindChild("container_islandpopboxpivot/btn_islandchange").GetComponent<Button>();
            button.onClick.AddListener(OnChange);
        }


        #region 1.展开云层
        //展开云层
        void OnExplore()
        {
            Close();
            CloseMask();
            if (mRegions!=null)
            {
                Debug.Log("OnExplore");
                if (mRegions.sending == false)
                {
                    WorldCameraOpEvent.Instance.ClosePop();
                    ISFSObject dt = Helper.getCostDiffToGems("", 3, true, mRegions.ExplorationCost);
                    int gems = dt.GetInt("Gems");
                    //Debug.Log(gems);
                    //资源不足，需要增加宝石才行;
                    if (gems > 0)
                    {
                        string msg = dt.GetUtfString("msg");
                        string title = dt.GetUtfString("title");
						this.UIMgr.GetComponent<PopMsgCtrl>().ShowDialog(msg, title, gems.ToString(), PopDialogBtnType.ImageBtn, dt, onExploreDialogYes);
                    }
                    else
                    {
                        /**
                        mRegions.sending = true;
                        mRegions.send_sprite.SetActive(true);

                        mRegions.gold_sprite.SetActive(false);
                        mRegions.exploration_cost.SetActive(false);

                        ISFSObject data = new SFSObject();
                        data.PutUtfString("regions_name", mRegions.Name);
                        data.PutInt("Gold", mRegions.ExplorationCost);
                        data.PutInt("Gems", 0);
                        //TODO
                        //NetworkManagerOld.Instance.SendRequest(data, "explore", false, HandleExploreResponse);
                        Helper.SetResource(-mRegions.ExplorationCost, 0, 0, 0, 0, true);
                        **/
                        RequestExplore(null);
                    }
                }
            }
        }

		private void onExploreDialogYes(ISFSObject dt,BuildInfo BuildInfo = null)
        {
            Close();
            CloseMask();
            if (DataManager.GetInstance().userInfo.diamond_count >= dt.GetInt("Gems"))
            {
                /**
                mRegions.sending = true;
                mRegions.send_sprite.SetActive(true);
                mRegions.gold_sprite.SetActive(false);
                mRegions.exploration_cost.SetActive(false);
                ISFSObject data = new SFSObject();
                data.PutUtfString("regions_name", mRegions.Name);
                data.PutInt("Gold", dt.GetInt("Gold"));
                data.PutInt("Gems", dt.GetInt("Gems"));
                //TODO
                //NetworkManagerOld.Instance.SendRequest(data, "explore", false, HandleExploreResponse);
                Helper.SetResource(-dt.GetInt("Gold"), 0, 0, 0, -dt.GetInt("Gems"), true);
                //GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,ur.capture_id,ur.capture_regions_id,dt.GetInt("Gold"),dt.GetInt("Gems"));
                //GameLoader.Instance.StartBattle(dt.GetInt("Gold"),dt.GetInt("Gems"),0,0);
                **/
                RequestExplore(dt);
            }
            else
            {
                //宝石不够;
                PopManage.Instance.ShowNeedGemsDialog(null, null);
            }
        }

        private void RequestExplore(ISFSObject dt)
        {
            mRegions.sending = true;
            mRegions.send_sprite.SetActive(true);
            mRegions.gold_sprite.SetActive(false);
            mRegions.exploration_cost.SetActive(false);
            ISFSObject data = new SFSObject();
            data.PutUtfString("regions_name", mRegions.Name);
            if (dt == null)
            {
                data.PutInt("Gold", mRegions.ExplorationCost);
                data.PutInt("Gems", 0);
                Helper.SetResource(-mRegions.ExplorationCost, 0, 0, 0, 0, true);
            }
            else
            {
                data.PutInt("Gold", dt.GetInt("Gold"));
                data.PutInt("Gems", dt.GetInt("Gems"));
                Helper.SetResource(-dt.GetInt("Gold"), 0, 0, 0, -dt.GetInt("Gems"), true);
            }
            //TODO
            //NetworkManagerOld.Instance.SendRequest(data, "explore", false, HandleExploreResponse);
            
        }

        //返回云层展开过后的数据
        void ResponseExplore(ISFSObject dt)
        {
            Debug.Log("HandleExploreResponse");
            WorldCameraOpEvent.Instance.ClosePop();
            string regions_name = dt.GetUtfString("regions_name");
			Regions rs2 = CSVManager.GetInstance().regionsList[regions_name] as Regions;
            //regions_name
            rs2.sending = false;
            //Debug.Log(rs2.cloud.name);
            rs2.cloud.gameObject.SetActive(false);
            //Debug.Log(dt.GetDump());
            Transform House = PopManage.Instance.WorldMap.transform.Find("House").transform;
            ISFSArray user_regions = dt.GetSFSArray("user_regions");
            //Debug.Log(user_regions.GetDump());
            for (int i = 0; i < user_regions.Size(); i++)
            {
                ISFSObject obj = user_regions.GetSFSObject(i);
                UserRegions ur = new UserRegions();
                ur.ISFSObjectToBean(obj);
				DataManager.GetInstance().userRegionsList[ur.regions_id] = ur;
                GameObject islandHouse = Instantiate(ResourceCache.load("UI/islandHouse")) as GameObject;
                Transform house_pos = House.Find(ur.regions_id.ToString());
                /*
                while(house_pos.GetChildCount() > 0){
                    Destroy(house_pos.GetChild(0).gameObject);
                }
                */
                islandHouse.transform.parent = house_pos;
                islandHouse.transform.localPosition = Vector3.zero;
                WorldHouse wh = islandHouse.GetComponent<WorldHouse>();
                ur.worldHouse = wh;
                wh.initData(ur, WorldBtnEvent.Instance.worldCamera);
            }
            /*
            ur.sending = false;
            ISFSObject obj = dt.GetSFSObject("user_regions");
            ur.ISFSObjectToBean(obj);
            ur.worldHouse.initData(ur,ur.worldHouse.worldCamera);
            */
        }
        #endregion

        public void OnGoHome()
        {
            Close();
            CloseMask();
            //Debug.Log("OnGoHome:ur.res_tid:" + ur.res_tid);
            if (mUserRegions.res_tid == null || mUserRegions.res_tid == "")
                GameLoader.Instance.SwitchScene(SceneStatus.HOME, 0, mUserRegions.regions_id);
            else
                GameLoader.Instance.SwitchScene(SceneStatus.HOMERESOURCE, 0, mUserRegions.regions_id);
        }

       	void OnSearch()
        {
            Close();
            CloseMask();
            if (mUserRegions.user_id == mUserRegions.capture_id)
            {

                if (mUserRegions.res_tid != null && mUserRegions.res_tid != "")
                {
                    //查看自己的资源岛屿;
                    GameLoader.Instance.SwitchScene(SceneStatus.HOMERESOURCE, mUserRegions.user_id, mUserRegions.regions_id, 0, 0);
                }
                else
                {
                    Debug.Log("非资源岛屿:" + mUserRegions.res_tid + ";mUserRegions.id:" + mUserRegions.id);
                }
            }
            else
            {
                //查看敌方岛屿;
                GameLoader.Instance.SwitchScene(SceneStatus.ENEMYVIEW, mUserRegions.capture_id, mUserRegions.regions_id, 0, 0);
            }
        }

        //攻击
        void OnAttack()
        {
			Close();
			CloseMask();
            if (mUserRegions != null)
            {
				if (CSVManager.GetInstance().experienceLevelsList.ContainsKey(DataManager.GetInstance().userInfo.exp_level.ToString())){
					WorldCameraOpEvent.Instance.ClosePop();

					ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
					//花费;
					//Debug.Log(el.AttackCost);
					ISFSObject dt = Helper.getCostDiffToGems("",3,true,el.AttackCost);
					int gems = dt.GetInt("Gems");
					//Debug.Log(gems);
					//资源不足，需要增加宝石才行;
					if (gems > 0){
						this.UIMgr.GetComponent<PopMsgCtrl>().ShowDialog(
							dt.GetUtfString("msg"),
							dt.GetUtfString("title"),
							gems.ToString(),
							PopDialogBtnType.ImageBtn,
							dt,
							OnBattleDialogYes
						);
					}else{
						GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,mUserRegions.capture_id,mUserRegions.regions_id,dt.GetInt("Gold"),0);
					}
				}
            }
        }

		private void OnBattleDialogYes(ISFSObject dt,BuildInfo s){
			if (DataManager.GetInstance().userInfo.diamond_count >= dt.GetInt("Gems")){
				GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,mUserRegions.capture_id,mUserRegions.regions_id,dt.GetInt("Gold"),dt.GetInt("Gems"));
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog(null,null);
			}
		}


        //重新寻找敌人
        void OnChange()
        {
            if (mUserRegions != null)
            {

            }
        }

        public override void Close()
        {
            UIMgr.ClosePanel("IslandPopPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }
    }
}
