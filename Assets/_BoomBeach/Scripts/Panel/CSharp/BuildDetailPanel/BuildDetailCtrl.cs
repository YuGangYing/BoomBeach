using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoomBeach
{
    public class BuildDetailCtrl : BaseCtrl
    {

        BuildDetailPanelView mBuildDetailPanelView;
        BuildInfo mBuildInfo;
        LayoutElement[] infos;

        public void ShowInfo(BuildInfo buildInfo)
        {
			ShowPanel();
            mBuildDetailPanelView.m_containerUpgrade.SetActive(false);
            mBuildDetailPanelView.m_txtInfo.gameObject.SetActive(true);
            mBuildDetailPanelView.m_txtTitle.text = buildInfo.ShowName;
            InitInfo(buildInfo);
        }

        public void ShowUpgrade(BuildInfo buildInfo)
        {
			ShowPanel();
            mBuildDetailPanelView.m_txtTitle.text = StringFormat.FormatByTid("TID_UPGRADE_TITLE", new System.Object[] { buildInfo.ShowName, buildInfo.level + 1 });
            InitInfo(buildInfo,true);
        }

        void InitUpgrade(BuildInfo buildInfo)
        {
          
            if (buildInfo.tid == "TID_BUILDING_PALACE")
            {
                List<UnLockTid> unlockListTid = Helper.getUpgradeUnLock();
                if (unlockListTid.Count > 0)
                {
                    mBuildDetailPanelView.m_containerUnlock.SetActive(true);
                    mBuildDetailPanelView.m_gridInfo.localPosition = new Vector3(mBuildDetailPanelView.m_gridInfo.localPosition.x, 100, mBuildDetailPanelView.m_gridInfo.localPosition.z);

                        for (int i = 0; i < mBuildDetailPanelView.m_containerUnlock.transform.childCount; i++)
                    {
                        mBuildDetailPanelView.m_containerUnlock.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    int k = 0;
                    foreach (UnLockTid tid in unlockListTid)
                    {
                        GameObject unlockItemObj = mBuildDetailPanelView.m_containerUnlock.transform.GetChild(k).gameObject;
                        unlockItemObj.SetActive(true);
                        unlockItemObj.transform.FindChild("Image").GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.avaterSpriteDic[tid.tid];
                        unlockItemObj.transform.FindChild("Text").GetComponent<Text>().text = tid.value;
                        /**
                        UnlockItem item = unlockItemObj.GetComponent<UnlockItem>();
                        item.Name = tid.showName;
                        item.Brief = tid.Subtitle;
                        item.Counter = tid.value;
                        item.model = tid.tid;
                        **/
                        k++;
                    }

                }
                else
                {
                    mBuildDetailPanelView.m_containerUnlock.SetActive(false);
                }
            }
            else
            {
                mBuildDetailPanelView.m_containerUnlock.SetActive(false);
            }
           


            string msg = Helper.CheckHasUpgrade(buildInfo.tid, buildInfo.level);
            if (msg == null)
            {
                mBuildDetailPanelView.m_containerUpgrade.SetActive(true);
                mBuildDetailPanelView.m_txtInfo.gameObject.SetActive(false);
                BuildCost buildCost = Helper.getUpgradeCost(buildInfo.tid_level);
                //木材
                if (buildCost.wood > 0)
                {
                    mBuildDetailPanelView.m_containerWood.SetActive(true);
                    mBuildDetailPanelView.m_txtWood.text = buildCost.wood.ToString();
                    if (buildCost.wood > DataManager.GetInstance().userInfo.wood_count)
                    {
                        mBuildDetailPanelView.m_txtWood.color = Color.red;
                    }
                    else
                    {
                        mBuildDetailPanelView.m_txtWood.color = Color.white;
                    }
                }
                else
                {
                    mBuildDetailPanelView.m_containerWood.SetActive(false);
                }
                //石头
                if (buildCost.stone > 0)
                {
                    mBuildDetailPanelView.m_containerStone.SetActive(true);
                    mBuildDetailPanelView.m_txtStone.text = buildCost.stone.ToString();
                    if (buildCost.stone > DataManager.GetInstance().userInfo.stone_count)
                    {
                        mBuildDetailPanelView.m_txtStone.color = Color.red;
                    }
                    else
                    {
                        mBuildDetailPanelView.m_txtStone.color = Color.white;
                    }
                }
                else
                {
                    mBuildDetailPanelView.m_containerStone.SetActive(false);
                }
                //铁矿
                if (buildCost.iron > 0)
                {
                    mBuildDetailPanelView.m_containerIron.SetActive(true);
                    mBuildDetailPanelView.m_txtIron.text = buildCost.iron.ToString();
                    if (buildCost.iron > DataManager.GetInstance().userInfo.iron_count)
                    {
                        mBuildDetailPanelView.m_txtIron.color = Color.red;
                    }
                    else
                    {
                        mBuildDetailPanelView.m_txtIron.color = Color.white;
                    }
                }
                else
                {
                    mBuildDetailPanelView.m_containerIron.SetActive(false);
                }
                mBuildDetailPanelView.m_txtDiamond.text = Helper.GetUpgradeInstant(buildInfo.tid_level).ToString();
                mBuildDetailPanelView.m_txtTime.text = Helper.GetFormatTime(Helper.GetUpgradeTime(buildInfo.tid_level), 0);
            }
            else
            {
                mBuildDetailPanelView.m_containerUpgrade.SetActive(false);
                mBuildDetailPanelView.m_txtInfo.gameObject.SetActive(true);
                mBuildDetailPanelView.m_txtInfo.text = msg;
            }
        }

        void InitInfo(BuildInfo buildInfo,bool isUpgrade = false)
        {
            
            mBuildDetailPanelView.m_txtLevel.text = buildInfo.ShowLevelName;
            if (buildInfo.csvInfo.BuildingClass != "Artifact")
            {
                mBuildDetailPanelView.m_txtInfo.text = StringFormat.FormatByTid(buildInfo.csvInfo.InfoTID);
            }
            else
            {
                //public int artifact_type;//神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage
                string InfoTID = "";
                if (buildInfo.artifact_type == ArtifactType.BoostGold)
                {
                    InfoTID = "TID_BOOST_GOLD_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostWood)
                {
                    InfoTID = "TID_BOOST_WOOD_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostStone)
                {
                    InfoTID = "TID_BOOST_STONE_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostMetal)
                {
                    InfoTID = "TID_BOOST_METAL_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostTroopHP)
                {
                    InfoTID = "TID_BOOST_TROOP_HP_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostBuildingHP)
                {
                    InfoTID = "TID_BOOST_BUILDING_HP_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostTroopDamage)
                {
                    InfoTID = "TID_BOOST_TROOP_DAMAGE_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostBuildingDamage)
                {
                    InfoTID = "TID_BOOST_BUILDING_DAMAGE_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostGunshipEnergy)
                {
                    InfoTID = "TID_BOOST_GUNSHIP_ENERGY_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostLoot)
                {
                    InfoTID = "TID_BOOST_LOOT_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostArtifactDrop)
                {
                    InfoTID = "TID_BOOST_ARTIFACT_DROP_INFO";
                }
                else if (buildInfo.artifact_type == ArtifactType.BoostAllResources)
                {
                    InfoTID = "TID_BOOST_ALL_INFO";
                }
                mBuildDetailPanelView.m_txtInfo.text = StringFormat.FormatByTid(InfoTID);
            }

            Dictionary<string, PropertyInfoNew> propertyList = Helper.getPropertyList(buildInfo.tid_level, isUpgrade, buildInfo);
            mBuildDetailPanelView.m_gridInfo.GetComponentsInChildren<LayoutElement>(true);
            mBuildDetailPanelView.m_gridInfo.GetComponent<RectTransform>().sizeDelta = new Vector2(100, (propertyList.Count + 1) * 45);
            for (int i=1;i<infos.Length;i++)
            {
                infos[i].gameObject.SetActive(false);
            }
            int k = 1;


            foreach (string key in propertyList.Keys)
            {
                //infos[i].transform.FindChild();
                infos[k].transform.FindChild("txt_name").GetComponent<Text>().text = propertyList[key].showName;
                infos[k].transform.FindChild("img").GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic[propertyList[key].spriteName];
                infos[k].transform.FindChild("txt_num").GetComponent<Text>().text = propertyList[key].value;
                if (isUpgrade)
                {
                    infos[k].transform.FindChild("txt_add_num").GetComponent<Text>().text = propertyList[key].upgrade_value;
                }
                else
                {
                    infos[k].transform.FindChild("txt_add_num").GetComponent<Text>().text = propertyList[key].bonus_value;
                }
                infos[k].gameObject.SetActive(true);
                k++;
            }
            
            //重置特殊组件
            mBuildDetailPanelView.m_gridInfo.localPosition = new Vector3(mBuildDetailPanelView.m_gridInfo.localPosition.x, 22, mBuildDetailPanelView.m_gridInfo.localPosition.z);
            mBuildDetailPanelView.m_containerUnlock.SetActive(false);

            if (isUpgrade)
                InitUpgrade(buildInfo);
            this.mBuildInfo = buildInfo;
            InitBuildModel(buildInfo);
        }

        public void InitBuildModel(BuildInfo buildInfo)
        {
            Transform BuildModel = mBuildDetailPanelView.m_containerBuildpoint.transform;
            //创建模形;
            if (BuildModel.FindChild("model") != null)
                Destroy(BuildModel.FindChild("model").gameObject);
            if (buildInfo.csvInfo.BuildingClass == "Artifact")
            {
                Helper.CreateArtifactUI(BuildModel, buildInfo.tid_level, buildInfo.artifact_type);
            }
            else
            {
                GameObject buildModel = Instantiate(ResourceCache.load(buildInfo.buildSpritePath)) as GameObject;
                buildModel.transform.parent = BuildModel;
                buildModel.transform.localPosition = Vector3.zero;
                buildModel.transform.name = "model";
                if (buildInfo.is3D)
                {
                    buildModel.transform.localScale = new Vector3(400f, 400f, 1f);
                    if (buildInfo.tid == "TID_BUILDING_GUNSHIP")
                    {
                        buildModel.transform.localScale = new Vector3(300f, 300f, 1f);
                        buildModel.transform.localPosition = new Vector3(100f, 50f, 0f);
                    }
                }
                else
                    buildModel.transform.localScale = new Vector3(400f, 400f, 1f);
                Transform[] tts = buildModel.GetComponentsInChildren<Transform>();
                for (int i = 0; i < tts.Length; i++)
                {
                    //tts[i].gameObject.layer = 9;
                }
            }
        }
		//点击直接用宝石升级
        void OnUpgradeWithDiamond()
        {
            Debug.Log("OnUpgradeWithDiamond");
            //mBuildInfo.InstantUpgrade(mBuildInfo, null);
			BuildHandle.ImmediateUpgradeBuild(mBuildInfo);
            Close();
            CloseMask();
        }
		//点击用资源升级
        void OnUpgrade()
        {
            Debug.Log("OnUpgrade");
			BuildHandle.OnUpgradeBuild(mBuildInfo);
			//BuildHandle.UpgradeBuild (mBuildInfo);
            Close();
            CloseMask();
        }

		public override void ShowPanel()
        {
            bool isCreate;
            mBuildDetailPanelView = UIMgr.ShowPanel<BuildDetailPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
			UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
        }

        void OnCreatePanel()
        {
            mBuildDetailPanelView.m_btnClose.onClick.AddListener(Close);
            mBuildDetailPanelView.m_btnClose.onClick.AddListener(CloseMask);
            mBuildDetailPanelView.m_btnInstant.onClick.AddListener(OnUpgradeWithDiamond);
            mBuildDetailPanelView.m_btnUpgrade.onClick.AddListener(OnUpgrade);
            infos = mBuildDetailPanelView.m_gridInfo.GetComponentsInChildren<LayoutElement>(true);
        }

        public override void Close()
        {
            UIMgr.ClosePanel("BuildDetailPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }

    }
}
