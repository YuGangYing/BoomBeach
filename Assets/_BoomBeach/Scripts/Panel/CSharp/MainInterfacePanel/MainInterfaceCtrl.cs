using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoomBeach {
    public class MainInterfaceCtrl : BaseCtrl
    {

        MainInterfacePanelView mMainInterfacePanelView;

		void Awake(){
			data = new ScreenUIData ();
		}

        //左下角的可建造建筑数量
        public void SetShopCount(int count)
        {
            mMainInterfacePanelView.m_txtBuildnum.text = count.ToString();
            if (count == 0)
            {
                mMainInterfacePanelView.m_containerBuildnum.SetActive(false);
            }
            else
            {
                mMainInterfacePanelView.m_containerBuildnum.SetActive(true);
            }
        }

		public override void ShowPanel()
        {
            bool isCreate;
            mMainInterfacePanelView = UIManager.GetInstance.ShowPanel<MainInterfacePanelView>(UIManager.UILayerType.Fixed, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
        }

		public void ShowHome(){
			ShowPanel ();
			mMainInterfacePanelView.m_btnSetting.gameObject.SetActive (true);
			mMainInterfacePanelView.m_btnAchivement.gameObject.SetActive (true);
			mMainInterfacePanelView.m_btnMail.gameObject.SetActive (true);
			mMainInterfacePanelView.m_btnShop.gameObject.SetActive (true);
			mMainInterfacePanelView.m_btnTeam.gameObject.SetActive (true);
			mMainInterfacePanelView.m_btnTeambattle.gameObject.SetActive (true);
			mMainInterfacePanelView.m_btnFriend.gameObject.SetActive (true);
		}

		public void ShowWorld(){
			ShowPanel ();
			mMainInterfacePanelView.m_btnSetting.gameObject.SetActive (false);
			mMainInterfacePanelView.m_btnAchivement.gameObject.SetActive (false);
			mMainInterfacePanelView.m_btnMail.gameObject.SetActive (false);
			mMainInterfacePanelView.m_btnShop.gameObject.SetActive (false);
			mMainInterfacePanelView.m_btnTeam.gameObject.SetActive (false);
			mMainInterfacePanelView.m_btnTeambattle.gameObject.SetActive (false);
			mMainInterfacePanelView.m_btnFriend.gameObject.SetActive (false);
		}

		public override void Close(){
			this.UIMgr.ClosePanel ("MainInterfacePanel");
		}

        public void CloseTips()
        {
            mMainInterfacePanelView.m_containerResourcepopbox.SetActive(false);
            mMainInterfacePanelView.m_containerMedalpopbox.SetActive(false);
            mMainInterfacePanelView.m_containerLevelpopbox.SetActive(false);
        }

        public void UpdateUserMedal()
        {
            mMainInterfacePanelView.m_txtMedal.text = data.UserMedal.ToString();
        }

        private float currentPercent; //当前经验百分比;
        private float increasePrecent; //增加的百分比;
        public void UpdateUserLevel(bool playAnim)
        {
            if (playAnim)
            {
                //UserLevelLabel.GetComponent<TweenScale>().onFinished = new System.Collections.Generic.List<EventDelegate>();
                //UserLevelLabel.GetComponent<TweenScale>().onFinished.Add(new EventDelegate(this, "OnLevelTweenEnd"));
                currentPercent = data.OldExp * 1f / data.OldUpgradeExp;
                increasePrecent = (data.UserLevel - data.OldUserLevel) + data.CurrentExp * 1f / data.UpgradeExp - currentPercent;
                IsUpdateUserLevel = true;
                //UserLevelLabel.GetComponent<TweenScale>().Reset();
                //UserLevelLabel.GetComponent<TweenScale>().Play();
            }
            else
            {
                IsUpdateUserLevel = false;
                //UserLevelLabel.GetComponent<TweenScale>().onFinished = new System.Collections.Generic.List<EventDelegate>();
                data.OldUserLevel = data.UserLevel;
                data.OldExp = data.CurrentExp;
                data.OldUpgradeExp = data.UpgradeExp;
                mMainInterfacePanelView.m_txtLevel.text = data.UserLevel.ToString();
                mMainInterfacePanelView.m_imgLevelbar.fillAmount = data.CurrentExp * 1f / data.UpgradeExp;
                mMainInterfacePanelView.m_txtLevelpopboxtitle.text = StringFormat.Format(LocalizationCustom.instance.Get("TID_LEVEL_NUM"), new string[] { data.UserLevel.ToString() });
                mMainInterfacePanelView.m_txtProgress_to_next_level.text = data.CurrentExp.ToString() + "/" + data.UpgradeExp.ToString();
                //UserLevelLabel.text = data.UserLevel.ToString();
                //UserLevelBar.fillAmount = data.CurrentExp * 1f / data.UpgradeExp;
                //UserLevelPopLabel.text = StringFormat.Format(LocalizationCustom.instance.Get("TID_LEVEL_NUM"), new string[] { data.UserLevel.ToString() });
                //UserExpLabel.text = data.CurrentExp.ToString() + "/" + data.UpgradeExp.ToString();
            }
        }



        public void UpdateGoldResource(bool playAnim)
		{
			if(playAnim)
			{
				currentGold = data.OldGoldCurrent;
				IsUpdateGoldResource = true;
			}
			else
			{
				IsUpdateGoldResource = false;
				data.OldGoldCurrent = data.GoldCurrent;
				data.OldGoldStorageCapacity = data.GoldStorageCapacity;
				float percent = data.GoldCurrent * 1f / data.GoldStorageCapacity;
				if (percent >= 1)
					percent = 1f;
				mMainInterfacePanelView.m_sliderGold.value = percent;
				mMainInterfacePanelView.m_txtGold.text = data.GoldCurrent.ToString();
			}
		}

        void ShowGoldResourceTip()
        {
            if (mMainInterfacePanelView.m_containerResourcepopbox.activeInHierarchy && mMainInterfacePanelView.m_containerResourcepopbox.transform.parent == mMainInterfacePanelView.m_btnGold.transform)
            {
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(false);
            }
            else
            {
                mMainInterfacePanelView.m_containerResourcepopbox.transform.parent = mMainInterfacePanelView.m_btnGold.transform;
                mMainInterfacePanelView.m_containerResourcepopbox.transform.localPosition = Vector3.zero;
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(true);
                UpdateGoldResourceTip();
            }
        }

        public void UpdateGoldResourceTip()
        {
            data.GoldProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_HOUSING");
            data.GoldProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_HOUSING");
            data.GoldProduce = data.GoldProduceFromHome + data.GoldProduceFromVillage;
            data.GoldStorageCapacity = DataManager.GetInstance.userInfo.max_gold_count;
            BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
            int prot_num = 0;
            if (vault != null)
            {
                //金币;
                if (DataManager.GetInstance.userInfo.gold_count < vault.csvInfo.MaxStoredResourceGold)
                {
                    prot_num = DataManager.GetInstance.userInfo.gold_count;
                }
                else
                {
                    //被保护的资源;
                    prot_num = (int)((DataManager.GetInstance.userInfo.gold_count - vault.csvInfo.MaxStoredResourceGold) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceGold;
                }
            }
           data.GoldProtected = prot_num;//
			mMainInterfacePanelView.m_txtTitle.text = LocalizationCustom.instance.Get("TID_GOLD");
			mMainInterfacePanelView.m_txtProduction_per_hour.text = data.GoldProduce.ToString();
			mMainInterfacePanelView.m_txtFrom_base.text = data.GoldProduceFromHome.ToString ();
			mMainInterfacePanelView.m_txtFrom_freed_villages.text = data.GoldProduceFromVillage.ToString();
			mMainInterfacePanelView.m_txtProtected_by_vault.text = data.GoldProtected.ToString();
			mMainInterfacePanelView.m_txtStorage_capacity.text = data.GoldStorageCapacity.ToString();
        }

		public void UpdateWoodResource(bool playAnim)
		{
			if(playAnim)
			{
				currentWood = data.OldWoodCurrent;
				IsUpdateWoodResource = true;
			}
			else
			{
				IsUpdateWoodResource = false;
				data.OldWoodCurrent = data.WoodCurrent;
				data.OldWoodStorageCapacity = data.WoodStorageCapacity;
				float percent = data.WoodCurrent * 1f / data.WoodStorageCapacity;
				if (percent >= 1)
					percent = 1f;
				mMainInterfacePanelView.m_sliderWood.value = percent;
				mMainInterfacePanelView.m_txtWood.text = data.WoodCurrent.ToString();
			}
		}

        void ShowWoodResourceTip()
        {
            if (mMainInterfacePanelView.m_containerResourcepopbox.activeInHierarchy && mMainInterfacePanelView.m_containerResourcepopbox.transform.parent == mMainInterfacePanelView.m_btnWood.transform)
            {
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(false);
            }
            else
            {
                mMainInterfacePanelView.m_containerResourcepopbox.transform.parent = mMainInterfacePanelView.m_btnWood.transform;
                mMainInterfacePanelView.m_containerResourcepopbox.transform.localPosition = Vector3.zero;
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(true);
                UpdateWoodResourceTip();
            }
        }

		public void UpdateWoodResourceTip()
		{
			data.WoodProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_WOODCUTTER");
			data.WoodProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_WOODCUTTER");
			data.WoodProduce = data.WoodProduceFromHome + data.WoodProduceFromVillage;
			data.WoodStorageCapacity = DataManager.GetInstance.userInfo.max_stone_count;

			BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
			int prot_num = 0;
			if (vault != null){
				if (DataManager.GetInstance.userInfo.wood_count < vault.csvInfo.MaxStoredResourceWood){
					prot_num = DataManager.GetInstance.userInfo.wood_count;
				}else{
					//被保护的资源;
					prot_num = (int)((DataManager.GetInstance.userInfo.wood_count - vault.csvInfo.MaxStoredResourceWood) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceWood;
				}
			}		
			data.WoodProtected = prot_num;//

			mMainInterfacePanelView.m_txtTitle.text = LocalizationCustom.instance.Get ("TID_WOOD");
			mMainInterfacePanelView.m_txtProduction_per_hour.text = data.WoodProduce.ToString ();
			mMainInterfacePanelView.m_txtFrom_base.text = data.WoodProduceFromHome.ToString ();
			mMainInterfacePanelView.m_txtFrom_freed_villages.text = data.WoodProduceFromVillage.ToString();
			mMainInterfacePanelView.m_txtStorage_capacity.text = data.WoodStorageCapacity.ToString();
			mMainInterfacePanelView.m_txtProtected_by_vault.text = data.WoodProtected.ToString();
		}

		public void UpdateStoneResource(bool playAnim)
		{
			if(playAnim)
			{
				currentStone = data.OldStoneCurrent;
				IsUpdateStoneResource = true;
			}
			else
			{
				IsUpdateStoneResource = false;
				data.OldStoneCurrent = data.StoneCurrent;
				data.OldStoneStorageCapacity = data.StoneStorageCapacity;

				if (data.StoneStorageCapacity > 0){
					if(!mMainInterfacePanelView.m_sliderStone.gameObject.activeInHierarchy)
						mMainInterfacePanelView.m_sliderStone.gameObject.SetActive (true);
					float percent = data.StoneCurrent * 1f / data.StoneStorageCapacity;
					if (percent >= 1)
						percent = 1f;
					mMainInterfacePanelView.m_sliderStone.value = percent;
					mMainInterfacePanelView.m_txtStone.text = data.StoneCurrent.ToString();
				}else{
					mMainInterfacePanelView.m_sliderStone.gameObject.SetActive (false);
				}
			}
		}

        void ShowStoneResourceTip()
        {
            if (mMainInterfacePanelView.m_containerResourcepopbox.activeInHierarchy && mMainInterfacePanelView.m_containerResourcepopbox.transform.parent == mMainInterfacePanelView.m_btnStone.transform)
            {
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(false);
            }
            else
            {
                mMainInterfacePanelView.m_containerResourcepopbox.transform.parent = mMainInterfacePanelView.m_btnStone.transform;
                mMainInterfacePanelView.m_containerResourcepopbox.transform.localPosition = Vector3.zero;
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(true);
                UpdateStoneResourceTip();
            }
        }

        public void UpdateStoneResourceTip()
		{
			data.StoneProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_STONE_QUARRY");
			data.StoneProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_STONE_QUARRY");
			data.StoneProduce = data.StoneProduceFromHome + data.StoneProduceFromVillage;
			data.StoneStorageCapacity = DataManager.GetInstance.userInfo.max_stone_count;

			BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
			int prot_num = 0;
			if (vault != null){
				if (DataManager.GetInstance.userInfo.stone_count < vault.csvInfo.MaxStoredResourceStone){
					prot_num = DataManager.GetInstance.userInfo.stone_count;
				}else{
					//被保护的资源;
					prot_num = (int)((DataManager.GetInstance.userInfo.stone_count - vault.csvInfo.MaxStoredResourceStone) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceStone;
				}
			}		
			data.StoneProtected = prot_num;//

			mMainInterfacePanelView.m_txtTitle.text = LocalizationCustom.instance.Get ("TID_STONE");
			mMainInterfacePanelView.m_txtProduction_per_hour.text = data.StoneProduce.ToString ();
			mMainInterfacePanelView.m_txtFrom_base.text = data.StoneProduceFromHome.ToString ();
			mMainInterfacePanelView.m_txtFrom_freed_villages.text = data.StoneProduceFromVillage.ToString();
			mMainInterfacePanelView.m_txtStorage_capacity.text = data.StoneStorageCapacity.ToString();
			mMainInterfacePanelView.m_txtProtected_by_vault.text = data.StoneProtected.ToString();
		}

		public void UpdateIronResource(bool playAnim)
		{
			if(playAnim)
			{
				currentIron = data.OldIronCurrent;
				IsUpdateIronResource = true;
			}
			else
			{
				IsUpdateIronResource = false;
				data.OldIronCurrent = data.IronCurrent;
				data.OldIronStorageCapacity = data.IronStorageCapacity;

				if (data.IronStorageCapacity > 0){
					if(!mMainInterfacePanelView.m_sliderIron.gameObject.activeInHierarchy)
						mMainInterfacePanelView.m_sliderIron.gameObject.SetActive (true);
					float percent = data.IronCurrent * 1f / data.IronStorageCapacity;
					if (percent >= 1)
						percent = 1f;
					mMainInterfacePanelView.m_sliderIron.value = percent;
					mMainInterfacePanelView.m_txtIron.text = data.IronCurrent.ToString();
				}else{
					mMainInterfacePanelView.m_sliderIron.gameObject.SetActive(false);
				}
			}
		}

        void ShowIronResourceTip()
        {
            if (mMainInterfacePanelView.m_containerResourcepopbox.activeInHierarchy && mMainInterfacePanelView.m_containerResourcepopbox.transform.parent == mMainInterfacePanelView.m_btnIron.transform)
            {
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(false);
            }
            else
            {
                mMainInterfacePanelView.m_containerResourcepopbox.transform.parent = mMainInterfacePanelView.m_btnIron.transform;
                mMainInterfacePanelView.m_containerResourcepopbox.transform.localPosition = Vector3.zero;
                mMainInterfacePanelView.m_containerResourcepopbox.SetActive(true);
                UpdateIronResourceTip();
            }
        }

        public void UpdateIronResourceTip()
		{
			data.IronProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_METAL_MINE");
			data.IronProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_METAL_MINE");
			data.IronProduce =data.IronProduceFromHome + data.IronProduceFromVillage;
			data.IronStorageCapacity = DataManager.GetInstance.userInfo.max_iron_count;

			BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
			int prot_num = 0;
			if (vault != null){
				if (DataManager.GetInstance.userInfo.iron_count < vault.csvInfo.MaxStoredResourceIron){
					prot_num = DataManager.GetInstance.userInfo.iron_count;
				}else{
					//被保护的资源;
					prot_num = (int)((DataManager.GetInstance.userInfo.iron_count - vault.csvInfo.MaxStoredResourceIron) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceIron;
				}
			}		
			data.IronProtected = prot_num;//
			mMainInterfacePanelView.m_txtTitle.text = LocalizationCustom.instance.Get ("TID_METAL");
			mMainInterfacePanelView.m_txtProduction_per_hour.text = data.IronProduce.ToString ();
			mMainInterfacePanelView.m_txtFrom_base.text = data.IronProduceFromHome.ToString ();
			mMainInterfacePanelView.m_txtFrom_freed_villages.text = data.IronProduceFromVillage.ToString();
			mMainInterfacePanelView.m_txtStorage_capacity.text = data.IronStorageCapacity.ToString();
			mMainInterfacePanelView.m_txtProtected_by_vault.text = data.IronProtected.ToString();
		}

		public void UpdateDiamondResource(bool playAnim)
		{
			if(playAnim)
			{
				currentDiamond = data.OldDiamondCurrent;
				IsUpdateDiamondResource = true;
			}
			else
			{
				IsUpdateDiamondResource = false;
				data.OldDiamondCurrent = data.DiamondCurrent;
				mMainInterfacePanelView.m_txtDiamond.text = data.DiamondCurrent.ToString();
			}
		}

        void OnCreatePanel()
        {
            mMainInterfacePanelView.m_btnSetting.onClick.AddListener(OnSettingBtnClick);
            mMainInterfacePanelView.m_btnFriend.onClick.AddListener(OnPlayerListBtnClick);
            mMainInterfacePanelView.m_btnShop.onClick.AddListener(OnShopBtnClick);
			mMainInterfacePanelView.m_btnMap.onClick.AddListener (OnWorldBtnClick);
            mMainInterfacePanelView.m_btnTeam.onClick.AddListener(OnTeamListBtnClick);
            mMainInterfacePanelView.m_btnAchivement.onClick.AddListener(OnAchivementBtnClick);

            mMainInterfacePanelView.m_btnGold.onClick.AddListener(ShowGoldResourceTip);
            mMainInterfacePanelView.m_btnWood.onClick.AddListener(ShowWoodResourceTip);
            mMainInterfacePanelView.m_btnStone.onClick.AddListener(ShowStoneResourceTip);
            mMainInterfacePanelView.m_btnIron.onClick.AddListener(ShowIronResourceTip);
        }

        void OnSettingBtnClick()
        {
			UIMgr.GetController<SettingCtrl>().ShowPanel();
        }

        void OnPlayerListBtnClick()
        {
			UIMgr.GetController<PlayerListCtrl>().ShowPanel();
        }

        void OnTeamListBtnClick()
        {
			UIMgr.GetController<TeamListCtrl>().ShowPanel();
        }

        void OnShopBtnClick()
        {
			UIMgr.GetController<ShopCtrl>().ShowPanel();
        }
 
		void OnWorldBtnClick()
		{

			if (DataManager.GetInstance.sceneStatus == SceneStatus.WORLDMAP)
            {
                GameLoader.Instance.SwitchScene(SceneStatus.HOME);
            }
            else
            {
                MoveOpEvent.Instance.ResetBuild();
                GameLoader.Instance.SwitchScene(SceneStatus.WORLDMAP);
            }
		}

        void OnAchivementBtnClick()
        {
			UIMgr.GetController<AchivementCtrl>().ShowPanel();
        }

        private bool IsUpdateUserLevel;
        private bool IsUpdateGoldResource;
        private bool IsUpdateWoodResource;
        private bool IsUpdateStoneResource;
        private bool IsUpdateIronResource;
        private bool IsUpdateDiamondResource;

        public ScreenUIData data;
        int currentGold;
        int currentWood;
        int currentStone;
        int currentIron;
        int currentDiamond;
        public float increaseStep = 0.05f;
        void Update()
        {
            if (IsUpdateUserLevel) OnUpdateUserLevel();
            if (IsUpdateGoldResource) OnUpdateGoldResource();
            if (IsUpdateWoodResource) OnUpdateWoodResource();
            if (IsUpdateStoneResource) OnUpdateStoneResource();
            if (IsUpdateIronResource) OnUpdateIronResource();
            if (IsUpdateDiamondResource) OnUpdateDiamondResource();
        }

        void OnUpdateUserLevel()
        {
            int currentUserLevel = 0;
            if (increasePrecent > 0)
            {
                increasePrecent -= increaseStep;
                currentPercent += increaseStep;
                if (currentPercent > 1)
                {
                    data.OldUserLevel += 1;
                    currentPercent -= 1;
                }
                currentUserLevel = data.OldUserLevel;
            }
            else
            {
                currentPercent = data.CurrentExp * 1f / data.UpgradeExp;
                currentUserLevel = data.UserLevel;
                IsUpdateUserLevel = false;
                data.OldUserLevel = data.UserLevel;
                data.OldExp = data.CurrentExp;
                data.OldUpgradeExp = data.UpgradeExp;
                //UserLevelLabel.GetComponent<TweenScale>().onFinished = new System.Collections.Generic.List<EventDelegate>();
            }
            mMainInterfacePanelView.m_txtLevel.text = currentUserLevel.ToString();
            mMainInterfacePanelView.m_imgLevelbar.fillAmount = currentPercent;
            mMainInterfacePanelView.m_txtLevelpopboxtitle.text = StringFormat.Format(LocalizationCustom.instance.Get("TID_LEVEL_NUM"), new string[] { data.UserLevel.ToString() });
            mMainInterfacePanelView.m_txtProgress_to_next_level.text = data.CurrentExp.ToString() + "/" + data.UpgradeExp.ToString();
        }

        void OnUpdateGoldResource()
        {
            if (currentGold < data.GoldCurrent)
            {
                currentGold += Mathf.CeilToInt(increaseStep * (data.GoldCurrent - data.OldGoldCurrent));
            }
            else
            {
                currentGold = data.GoldCurrent;
                IsUpdateGoldResource = false;
                data.OldGoldCurrent = data.GoldCurrent;
                data.OldGoldStorageCapacity = data.GoldStorageCapacity;
            }
            float percent = currentGold * 1f / data.GoldStorageCapacity;
            if (percent >= 1)
                percent = 1f;
            mMainInterfacePanelView.m_sliderGold.value = percent;
            mMainInterfacePanelView.m_txtGold.text = currentGold.ToString();
            //GoldBar.transform.localScale = new Vector3(percent, GoldBar.transform.localScale.y, 1f);
            //GoldCurrent.text = currentGold.ToString();
        }

        void OnUpdateWoodResource()
        {
            if (currentWood < data.WoodCurrent)
            {
                currentWood += Mathf.CeilToInt(increaseStep * (data.WoodCurrent - data.OldWoodCurrent));
            }
            else
            {
                currentWood = data.WoodCurrent;
                IsUpdateWoodResource = false;
                data.OldWoodCurrent = data.WoodCurrent;
                data.OldWoodStorageCapacity = data.WoodStorageCapacity;

            }

            float percent = currentWood * 1f / data.WoodStorageCapacity;
            if (percent >= 1)
                percent = 1f;
            mMainInterfacePanelView.m_sliderWood.value = percent;
            mMainInterfacePanelView.m_txtWood.text = currentWood.ToString();
        }

        void OnUpdateStoneResource()
        {
            if (currentStone < data.StoneCurrent)
            {
                currentStone += Mathf.CeilToInt(increaseStep * (data.StoneCurrent - data.OldStoneCurrent));
            }
            else
            {
                currentStone = data.StoneCurrent;
                IsUpdateStoneResource = false;
                data.OldStoneCurrent = data.StoneCurrent;
                data.OldStoneStorageCapacity = data.StoneStorageCapacity;
            }

            float percent = currentStone * 1f / data.StoneStorageCapacity;
            if (percent >= 1)
                percent = 1f;

            mMainInterfacePanelView.m_sliderStone.value = percent;
            mMainInterfacePanelView.m_txtStone.text = currentStone.ToString();
        }

        void OnUpdateIronResource()
        {
            if (currentIron < data.IronCurrent)
            {
                currentIron += Mathf.CeilToInt(increaseStep * (data.IronCurrent - data.OldIronCurrent));
            }
            else
            {
                currentIron = data.IronCurrent;
                IsUpdateIronResource = false;
                data.OldIronCurrent = data.IronCurrent;
                data.OldIronStorageCapacity = data.IronStorageCapacity;
            }

            float percent = currentIron * 1f / data.IronStorageCapacity;
            if (percent >= 1)
                percent = 1f;
            mMainInterfacePanelView.m_sliderIron.value = percent;
            mMainInterfacePanelView.m_txtIron.text = currentIron.ToString();
        }

        void OnUpdateDiamondResource()
        {
            if (currentDiamond < data.DiamondCurrent)
            {
                currentDiamond += Mathf.CeilToInt(increaseStep * (data.DiamondCurrent - data.OldDiamondCurrent));
            }
            else
            {
                currentDiamond = data.DiamondCurrent;
                IsUpdateDiamondResource = false;
                data.OldDiamondCurrent = data.DiamondCurrent;

            }
            mMainInterfacePanelView.m_txtDiamond.text = currentDiamond.ToString();
        }

    }
}
