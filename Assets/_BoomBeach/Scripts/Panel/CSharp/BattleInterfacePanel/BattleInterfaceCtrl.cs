using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using System.Collections;

namespace BoomBeach {

    public class BattleInterfaceCtrl : BaseCtrl
    {
        BattleInterfacePanelView mBattleInterfacePanelView;
		public List<Button> troopBtnList;
		public List<BattleButtonItem> troopList;
		public List<Button> weaponBtnList;
		public List<BattleButtonItem> weaponList;

		public List<Button> viewTroopBtnList;//查看敌方时的士兵列表;
		//int mSelectWenponBtnIndex;
		//int mSelectTrooperBtnIndex;
        List<BattleTrooperData> mTroops;
        List<BattleTrooperData> mWeapons;

        void Awake(){
			viewTroopBtnList = new List<Button>();
		}

		public void CannotFire(){
			this.UIMgr.GetController<NormalMsgCtrl>().ShowPop("能量不足");//TODO
		}

        public void UpdateWeaponCost(BattleTrooperData btd)
        {
            int index = mWeapons.IndexOf(btd);
			BattleButtonItem item = weaponList [index]; 
			item.energyCost.text = btd.weaponCost.ToString();
        }

		public void SetEnergy(int energy){
			if (mBattleInterfacePanelView == null || mBattleInterfacePanelView.m_txtEnergy == null)
				return;
				
			mBattleInterfacePanelView.m_txtEnergy.text = energy.ToString ();

			if (mWeapons != null) {
				for(int i = 0;i < mWeapons.Count;i ++)
				{
					if (mWeapons [i].weaponCost > Globals.EnergyTotal) {
						weaponList [i].energyCost.color = Color.red;
					} else {
						weaponList [i].energyCost.color = Color.white;
					}
				}
			}
			if(mTroops!=null){
				for(int i=0;i<mTroops.Count;i++){
					if (mTroops [i].weaponCost > Globals.EnergyTotal) {
						troopList [i].energyCost.color = Color.red;
					} else {
						troopList [i].energyCost.color = Color.white;
					}
				}
			}


		}

		public void ShowBattle(ISFSObject dt){
			ShowPanel ();
			mBattleInterfacePanelView.m_imgSelecttroop.gameObject.SetActive (false);
			mBattleInterfacePanelView.m_imgSelectweapon.gameObject.SetActive (false);
			for(int i =0;i<troopList.Count;i++){
				troopList [i].btn.enabled = true;
				troopList [i].sprite.material = null;
			}
			mBattleInterfacePanelView.m_containerBattleview.SetActive (false);
			mBattleInterfacePanelView.m_containerDiamondbar.SetActive (false);
			mBattleInterfacePanelView.m_containerResourcebar.SetActive (false);
			mBattleInterfacePanelView.m_btnEnd.gameObject.SetActive (true);
			mBattleInterfacePanelView.m_btnBattle.gameObject.SetActive (false);
			mBattleInterfacePanelView.m_btnMap.gameObject.SetActive (false);
			InitReward (dt);
		}

		public void ShowBattleView(ISFSObject dt){
			ShowPanel ();
			mBattleInterfacePanelView.m_containerTroops.SetActive(false);
			mBattleInterfacePanelView.m_containerWeapons.SetActive(false);
			mBattleInterfacePanelView.m_imgSelecttroop.gameObject.SetActive (false);
			mBattleInterfacePanelView.m_imgSelectweapon.gameObject.SetActive (false);
			mBattleInterfacePanelView.m_containerBattleview.SetActive (true);
			mBattleInterfacePanelView.m_containerDiamondbar.SetActive (true);
			mBattleInterfacePanelView.m_containerResourcebar.SetActive (true);
			mBattleInterfacePanelView.m_containerEnergy.SetActive (false);
			mBattleInterfacePanelView.m_btnEnd.gameObject.SetActive (false);
			mBattleInterfacePanelView.m_btnBattle.gameObject.SetActive (true);
			mBattleInterfacePanelView.m_btnMap.gameObject.SetActive (true);
			InitResources ();
			InitReward (dt);
		}
			
        public override void ShowPanel()
        {
            bool isCreate;
			mBattleInterfacePanelView = UIManager.GetInstance().ShowPanel<BattleInterfacePanelView>(UIManager.UILayerType.Fixed, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
        }

		void OnCreatePanel(){
			Debug.Log ("OnCreatePanel");
			troopList = new List<BattleButtonItem> ();
			troopBtnList = new List<Button> ();
			Button[] btns = mBattleInterfacePanelView.m_containerTroops.GetComponentsInChildren<Button> ();
			this.troopBtnList.AddRange (btns);
			for(int i = 0;i < btns.Length;i++)
			{
				BattleButtonItem item = new BattleButtonItem ();
				Button btn = btns [i];
				item.btn = btn;
				item.energyCost = btn.transform.FindChild ("txt_energycost").GetComponent<Text>();
				item.energyCostIcon = btn.transform.FindChild ("img_energycost").GetComponent<Image>();
				item.troopNum = btn.transform.FindChild ("txt_troopnum").GetComponent<Text>();
				item.sprite = btn.transform.FindChild ("img_icon").GetComponent<Image>();
				btn.onClick.AddListener (SelectTroop);
				troopList.Add (item);
			}
			weaponList = new List<BattleButtonItem> ();
			weaponBtnList = new List<Button> ();
			btns = mBattleInterfacePanelView.m_containerWeapons.GetComponentsInChildren<Button> ();
			this.weaponBtnList.AddRange (btns);
			for(int i = 0;i < btns.Length;i++)
			{
				BattleButtonItem item = new BattleButtonItem ();
				Button btn = btns [i];
				item.btn = btn;
				item.energyCost = btn.transform.FindChild ("txt_energycost").GetComponent<Text>();
				item.sprite = btn.transform.FindChild ("img_icon").GetComponent<Image>();
				btn.onClick.AddListener (SelectWeapon);
				weaponList.Add (item);
			}
			viewTroopBtnList.AddRange (mBattleInterfacePanelView.m_containerViewtroops.GetComponentsInChildren<Button>());
			mBattleInterfacePanelView.m_btnEnd.onClick.AddListener (OnClickEndBattle);
			mBattleInterfacePanelView.m_btnBattle.onClick.AddListener (OnClickAttack);
			mBattleInterfacePanelView.m_btnMap.onClick.AddListener (OnClickWorld);
		}

		public override void Close(){
			this.UIMgr.ClosePanel ("BattleInterfacePanel");
		}

		void SelectTroop(){
			GameObject go = EventSystem.current.currentSelectedGameObject;
			int index = troopBtnList.IndexOf (go.GetComponent<Button>());
			//this.mSelectTrooperBtnIndex = index;
			BattleTrooperData data = mTroops [index];
			BattleData.Instance.currentSelectBtd = data;
			mBattleInterfacePanelView.m_imgSelecttroop.gameObject.SetActive (true);
			mBattleInterfacePanelView.m_imgSelecttroop.transform.SetParent(go.transform);
			mBattleInterfacePanelView.m_imgSelecttroop.transform.localPosition = Vector3.zero;
			mBattleInterfacePanelView.m_imgSelecttroop.transform.SetSiblingIndex (0);
		}

		void SelectWeapon(){
			GameObject go = EventSystem.current.currentSelectedGameObject;
			int index = weaponBtnList.IndexOf (go.GetComponent<Button>());
			//this.mSelectWenponBtnIndex = index;
			BattleTrooperData data = mWeapons [index];
			BattleData.Instance.currentSelectBtd = data;
			mBattleInterfacePanelView.m_imgSelectweapon.gameObject.SetActive (true);
			mBattleInterfacePanelView.m_imgSelectweapon.transform.SetParent(go.transform);
			mBattleInterfacePanelView.m_imgSelectweapon.transform.localPosition = Vector3.zero;
			mBattleInterfacePanelView.m_imgSelectweapon.transform.SetSiblingIndex (0);
		}

		//查看敌方时切换到攻击场景;
		public void OnClickAttack()
		{
			if (CSVManager.GetInstance().experienceLevelsList.ContainsKey(DataManager.GetInstance().userInfo.exp_level.ToString())){
				ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
				//花费;
				ISFSObject dt = Helper.getCostDiffToGems("",3,true,el.AttackCost);
				int gems = dt.GetInt("Gems");
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
					GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,Globals.LastSceneUserId,Globals.LastSceneRegionsId,dt.GetInt("Gold"),0);
				}
			}else{

			}
		}

		// 战斗开始触发
		private void OnBattleDialogYes(ISFSObject dt,BuildInfo buildInfo = null){
			if (DataManager.GetInstance().userInfo.diamond_count >= dt.GetInt("Gems")){
				GameLoader.Instance.SwitchScene(SceneStatus.ENEMYBATTLE,Globals.LastSceneUserId,Globals.LastSceneRegionsId,dt.GetInt("Gold"),dt.GetInt("Gems"));
			}else{
				//宝石不够;
				PopManage.Instance.ShowNeedGemsDialog(null,null);
			}
		}

		//切换到世界地图;
		public void OnClickWorld()
		{
			MoveOpEvent.Instance.ResetBuild();
			GameLoader.Instance.SwitchScene(SceneStatus.WORLDMAP);
			this.UIMgr.GetController<MainInterfaceCtrl>().ShowWorld ();
			Close ();
		}

		//结束战斗;
		public void OnClickEndBattle()
		{
			BattleData.Instance.BattleIsEnd = true;
            //战斗没有开始直接返回世界场景
            if (!BattleData.Instance.BattleIsStart)
            {
                Globals.LastSceneUserId = -1;
                Globals.LastSceneRegionsId = -1;
                AudioPlayer.Instance.PlayMusic("home_music");
                GameLoader.Instance.SwitchScene(SceneStatus.WORLDMAP, DataManager.GetInstance().userInfo.id, Globals.LastSceneRegionsId, 0, 0);
            }

            //已派出的兵与已死亡的兵相等，表示派出的兵全死完或未派兵，直接弹窗;
            if (BattleData.Instance.BattleIsStart && BattleData.Instance.AllocateTrooperList.Count == BattleData.Instance.DeadTrooperList.Count) {
				if(ScreenUIManage.Instance!=null)
					ScreenUIManage.Instance.battleResultWin.ShowResultWin();
				UIManager.GetInstance ().GetController<BattleResultCtrl>().ShowPanel ();
			}
			else
			{
				if(BattleData.Instance.AllocateTrooperList.Count>0)
					AudioPlayer.Instance.PlayMusic("reef_retreat_01");
				AITask.Instance.ResetAll();
				//未死完，通知退兵;
				foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
				{
					if(!charInfo.isDead)
					{
						charInfo.Dest = charInfo.RetreatPoint;
						charInfo.IsRetreating = true;
						charInfo.IsOnlyMove = true;
						charInfo.path = null;
						charInfo.AttackState = AISTATE.STANDING;
						charInfo.trooperCtl.CMDFindPath();
					}
				}
			}
		}

		public void OnTroopDeploy(BattleTrooperData data){
			int index = mTroops.IndexOf (data);
			BattleButtonItem item = troopList [index];
			item.sprite.material = ResourceManager.GetInstance().grayMat;
			item.btn.enabled = false;
		}

		public void ShowTroops(List<BattleTrooperData> troops){
			mTroops = troops;
			mBattleInterfacePanelView.m_containerTroops.SetActive (true);
			for(int i=0;i< 8;i++){
				BattleButtonItem item = troopList [i];
				if (troops.Count <= i) {
					item.btn.gameObject.SetActive (false);
				} else {
					item.btn.gameObject.SetActive (true);
					item.sprite.sprite = ResourceManager.GetInstance ().atlas.avaterSpriteDic [troops [i].tid];
					item.troopNum.text = "x" + troops [i].num;
					if (troops [i].weaponCost > 0) {
						item.energyCost.gameObject.SetActive (true);
						item.energyCostIcon.gameObject.SetActive (true);
						item.energyCost.text = troops [i].weaponCost.ToString ();
					} else {
						item.energyCost.gameObject.SetActive (false);
						item.energyCostIcon.gameObject.SetActive (false);
					}
				}
			}
		}

		public void ShowWeapons(List<BattleTrooperData> weapons)
		{
			mWeapons = weapons;
			mBattleInterfacePanelView.m_containerWeapons.SetActive (true);
			int count = 0;
			for(int i=0;i< 8;i++){
				BattleButtonItem item = weaponList [i];
                if (weapons.Count <= i) {
					item.btn.gameObject.SetActive (false);
				} else {
					if (weapons [i].weaponCost <= 0) {
						item.btn.gameObject.SetActive (false);
					} else {
						count++;
						item.btn.gameObject.SetActive (true);
						item.sprite.sprite = ResourceManager.GetInstance ().atlas.avaterSpriteDic [weapons [i].tid];
						item.energyCost.text = weapons [i].weaponCost.ToString ();
					}
				}
			}
			if (count > 4) {
				float x = mBattleInterfacePanelView.m_containerEnergy.transform.localPosition.x;
				mBattleInterfacePanelView.m_containerEnergy.transform.localPosition = new Vector3 (x, -70,0);
			} else {
				float x = mBattleInterfacePanelView.m_containerEnergy.transform.localPosition.x;
				mBattleInterfacePanelView.m_containerEnergy.transform.localPosition = new Vector3 (x, -200,0);
			}
		}

		public void ShowViewTroops(List<BattleTrooperData> troops){
			mTroops = troops;
			mBattleInterfacePanelView.m_containerViewtroops.SetActive (true);
			for(int i=0;i< 8;i++){
				Button btn = viewTroopBtnList [i];
				if (troops.Count <= i) {
					btn.gameObject.SetActive (false);
				} else {
					btn.gameObject.SetActive (true);
					btn.transform.FindChild ("Image").GetComponent<Image> ().sprite = ResourceManager.GetInstance ().atlas.avaterSpriteDic [troops [i].tid];
					btn.transform.FindChild ("Text").GetComponent<Text> ().text = troops [i].num.ToString();
				}
			}
		}

		void InitReward(ISFSObject dt){
			ISFSObject user = dt.GetSFSObject ("userResource");
			mBattleInterfacePanelView.m_txtName.text = StringFormat.FormatByTid(user.GetUtfString("user_name"));
			mBattleInterfacePanelView.m_txtLevel.text = user.GetInt ("level").ToString();
			mBattleInterfacePanelView.m_txtGoldnum.text = dt.GetInt ("loot_gold").ToString();
			mBattleInterfacePanelView.m_txtWoodnum.text = dt.GetInt("loot_wood").ToString();
			mBattleInterfacePanelView.m_txtStonenum.text = dt.GetInt("loot_stone").ToString();
			mBattleInterfacePanelView.m_txtIronnum.text = dt.GetInt ("loot_iron").ToString();
			mBattleInterfacePanelView.m_txtMedalnum.text = dt.GetInt ("add_reward").ToString();

			int loot_boot = dt.GetInt("loot_boot");
			mBattleInterfacePanelView.m_txtGoldnumadd.text = "+" + (dt.GetInt("loot_gold") * loot_boot / 100).ToString();
			mBattleInterfacePanelView.m_txtWoodnumadd.text = "+" + (dt.GetInt("loot_wood") * loot_boot / 100).ToString();
			mBattleInterfacePanelView.m_txtStonenumadd.text = "+" + (dt.GetInt("loot_stone") * loot_boot / 100).ToString();
			mBattleInterfacePanelView.m_txtIronnumadd.text = "+" + (dt.GetInt("loot_iron") * loot_boot / 100).ToString();

			int artifactType = dt.GetInt ("artifact");//0 Piece 1 Ice 2 Fire 3 Dark
			switch(artifactType){
			case 0:
				mBattleInterfacePanelView.m_imgSmall.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["CommonPiece"];
				mBattleInterfacePanelView.m_imgMiddle.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["RarePiece"];
				mBattleInterfacePanelView.m_imgBig.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["EpicPiece"];
				break;
			case 1:
				mBattleInterfacePanelView.m_imgSmall.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["CommonPieceIce"];
				mBattleInterfacePanelView.m_imgMiddle.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["RarePieceIce"];
				mBattleInterfacePanelView.m_imgBig.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["EpicPieceIce"];
				break;
			case 2:
				mBattleInterfacePanelView.m_imgSmall.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["CommonPieceFire"];
				mBattleInterfacePanelView.m_imgMiddle.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["RarePieceFire"];
				mBattleInterfacePanelView.m_imgBig.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["EpicPieceFire"];
				break;
			case 3:
				mBattleInterfacePanelView.m_imgSmall.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["CommonPieceDark"];
				mBattleInterfacePanelView.m_imgMiddle.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["RarePieceDark"];
				mBattleInterfacePanelView.m_imgBig.sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["EpicPieceDark"];
				break;
			}
			mBattleInterfacePanelView.m_txtMedalnumadd.text = "+" + dt.GetInt("pal_reward").ToString();

		}
			
		void InitResources(){
			UserInfo userInfo = DataManager.GetInstance().userInfo;

			mBattleInterfacePanelView.m_txtDiamond.text = userInfo.diamond_count.ToString();

			float percent = userInfo.gold_count / userInfo.max_gold_count;
			if (percent >= 1)
				percent = 1f;
			mBattleInterfacePanelView.m_sliderGold.value = percent;
			mBattleInterfacePanelView.m_txtGold.text = userInfo.gold_count.ToString();

			percent = userInfo.wood_count / userInfo.max_wood_count;
			if (percent >= 1)
				percent = 1f;
			mBattleInterfacePanelView.m_sliderWood.value = percent;
			mBattleInterfacePanelView.m_txtWood.text = userInfo.wood_count.ToString();

			if (userInfo.max_stone_count <= 0) {
				mBattleInterfacePanelView.m_sliderStone.gameObject.SetActive (false);
			} else {
				mBattleInterfacePanelView.m_sliderStone.gameObject.SetActive (true);
				percent = userInfo.stone_count / userInfo.max_stone_count;
				if (percent >= 1)
					percent = 1f;
				mBattleInterfacePanelView.m_sliderStone.value = percent;
				mBattleInterfacePanelView.m_txtStone.text = userInfo.stone_count.ToString();
			}

			if (userInfo.max_iron_count <= 0) {
				mBattleInterfacePanelView.m_sliderIron.gameObject.SetActive (false);
			} else {
				mBattleInterfacePanelView.m_sliderIron.gameObject.SetActive (true);
				percent = userInfo.iron_count / userInfo.max_iron_count;
				if (percent >= 1)
					percent = 1f;
				mBattleInterfacePanelView.m_sliderIron.value = percent;
				mBattleInterfacePanelView.m_txtIron.text = userInfo.iron_count.ToString();
			}

		}

		public class BattleButtonItem
		{
			public Button btn;
			public Text troopNum;
			public Text energyCost;
			public Image energyCostIcon;
			public Image sprite;
		}
    }
}
