using UnityEngine;
using System.Collections;
using BoomBeach;

public class ScreenUIManage : MonoBehaviour {

	public float increaseStep = 0.05f;
	public Camera uiCamera;

	//单例定义;
	private static ScreenUIManage instance;
	public static ScreenUIManage Instance{
		get{ return instance; }
	}

	//攻击查看的左上角资源UI;
	public EnemyNameAndResource enemyNameAndResource;

	//查看敌方时的士兵列表;
	public EnemyViewTrooperList enemyViewTrooperList;

	public BattleTrooperList battleTrooperList;

	public BattleWeaponList battleWeaponList;

	void Awake()
	{
		instance = this;
		if(data==null)
		data = new ScreenUIData ();
		enemyNameAndResource.Init ();
		enemyViewTrooperList.Init ();
		battleTrooperList.Init ();
		battleWeaponList.Init ();
	}

	void Update()
	{
		if (IsUpdateUserLevel)OnUpdateUserLevel ();
		if(IsUpdateGoldResource)OnUpdateGoldResource();
		if(IsUpdateWoodResource)OnUpdateWoodResource();
		if(IsUpdateStoneResource)OnUpdateStoneResource();
		if(IsUpdateIronResource)OnUpdateIronResource();
		if(IsUpdateDiamondResource)OnUpdateDiamondResource();
	}

	public ScreenUIData data; 


	//关于用户名的更新;
	public UILabel UserNameLabel; //用户名;
	public void UpdateUserName () {
		UserNameLabel.text = data.UserName;
	}
	//关于用户名的更新;

	//关于用户等级的更新;
	public UILabel UserLevelLabel; //当前等级;
	public UISprite UserLevelBar;  //等级百分比进度条;
	public UILabel UserLevelPopLabel; //显示于界面内的等级;
	public UILabel UserExpLabel; //经验条;
	private bool IsUpdateUserLevel;

	private float currentPercent; //当前经验百分比;
	private float increasePrecent; //增加的百分比;
	public void UpdateUserLevel(bool playAnim)
	{
		if(playAnim)
		{
			UserLevelLabel.GetComponent<TweenScale> ().onFinished = new System.Collections.Generic.List<EventDelegate> ();
			UserLevelLabel.GetComponent<TweenScale> ().onFinished.Add (new EventDelegate(this,"OnLevelTweenEnd"));
			currentPercent = data.OldExp * 1f / data.OldUpgradeExp;
			increasePrecent = (data.UserLevel - data.OldUserLevel) + data.CurrentExp*1f/data.UpgradeExp - currentPercent ;
			IsUpdateUserLevel = true;
			UserLevelLabel.GetComponent<TweenScale> ().Reset ();
			UserLevelLabel.GetComponent<TweenScale> ().PlayForward ();
		}
		else
		{
			IsUpdateUserLevel = false;
			UserLevelLabel.GetComponent<TweenScale> ().onFinished = new System.Collections.Generic.List<EventDelegate> ();
			data.OldUserLevel = data.UserLevel;
			data.OldExp = data.CurrentExp;
			data.OldUpgradeExp = data.UpgradeExp;
			UserLevelLabel.text = data.UserLevel.ToString();
			UserLevelBar.fillAmount =  data.CurrentExp*1f/data.UpgradeExp;
			UserLevelPopLabel.text =  StringFormat.Format(LocalizationCustom.instance.Get ("TID_LEVEL_NUM"),new string[]{data.UserLevel.ToString()});
			UserExpLabel.text = data.CurrentExp.ToString () + "/" + data.UpgradeExp.ToString ();
		}
	}
	private void OnLevelTweenEnd()
	{
		UserLevelLabel.GetComponent<TweenScale> ().Reset ();
		UserLevelLabel.GetComponent<TweenScale> ().PlayForward ();
	}
	private void OnUpdateUserLevel()
	{
		int currentUserLevel = 0;
		if(increasePrecent>0)
		{
			increasePrecent-=increaseStep;
			currentPercent+=increaseStep;
			if(currentPercent>1)
			{
				data.OldUserLevel+=1;
				currentPercent-=1;
			}
			currentUserLevel = data.OldUserLevel;
		}
		else
		{
			currentPercent = data.CurrentExp*1f/data.UpgradeExp;
			currentUserLevel = data.UserLevel;
			IsUpdateUserLevel = false;

			data.OldUserLevel = data.UserLevel;
			data.OldExp = data.CurrentExp;
			data.OldUpgradeExp = data.UpgradeExp;

			UserLevelLabel.GetComponent<TweenScale> ().onFinished = new System.Collections.Generic.List<EventDelegate> ();
		}

		UserLevelLabel.text = currentUserLevel.ToString();
		UserLevelBar.fillAmount = currentPercent;
		UserLevelPopLabel.text =  StringFormat.Format(LocalizationCustom.instance.Get ("TID_LEVEL_NUM"),new string[]{data.UserLevel.ToString()});
		UserExpLabel.text = data.CurrentExp.ToString () + "/" + data.UpgradeExp.ToString ();


	}
	//关于用户等级的更新;

	//更新用户奖杯;
	public UILabel UserMedalLabel;
	public void UpdateUserMedal()
	{
		UserMedalLabel.text = data.UserMedal.ToString();
	}
	//更新用户奖杯;


	//关于资源的弹框文字;
	public UILabel ResourceName;
	public UILabel ResourceProduceLabel;
	public UILabel ResourceProduceFromHomeLabel;
	public UILabel ResourceProduceFromVillageLabel;
	public UILabel ResourceStorageCapacityLabel;
	public UILabel ResourceProtectedLabel;

	//关于更新金币;
	public UISprite GoldBar;
	public UILabel GoldCurrent;

	private bool IsUpdateGoldResource;
	private int currentGold;
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

			if(data.GoldStorageCapacity>0)
			GoldBar.transform.localScale = new Vector3(data.GoldCurrent*1f/data.GoldStorageCapacity,1f,1f);
			GoldCurrent.text = data.GoldCurrent.ToString();
		}
	}
	void OnUpdateGoldResource()
	{
		if(currentGold<data.GoldCurrent)
		{
			currentGold+= Mathf.CeilToInt(increaseStep*(data.GoldCurrent-data.OldGoldCurrent));
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

		GoldBar.transform.localScale = new Vector3(percent,GoldBar.transform.localScale.y,1f);
		GoldCurrent.text = currentGold.ToString();
	}


	public void UpdateGoldResourceTip()
	{
		ScreenUIManage.Instance.data.GoldProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_HOUSING");
		ScreenUIManage.Instance.data.GoldProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_HOUSING");
		ScreenUIManage.Instance.data.GoldProduce = ScreenUIManage.Instance.data.GoldProduceFromHome + ScreenUIManage.Instance.data.GoldProduceFromVillage;
		ScreenUIManage.Instance.data.GoldStorageCapacity = DataManager.GetInstance().userInfo.max_gold_count;
		
		BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
		int prot_num = 0;
		if (vault != null){
			//金币;
			if (DataManager.GetInstance().userInfo.gold_count < vault.csvInfo.MaxStoredResourceGold){
				prot_num = DataManager.GetInstance().userInfo.gold_count;
			}else{
				//被保护的资源;
				prot_num = (int)((DataManager.GetInstance().userInfo.gold_count - vault.csvInfo.MaxStoredResourceGold) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceGold;
			}
		}		
		ScreenUIManage.Instance.data.GoldProtected = prot_num;//

		ResourceName.text = LocalizationCustom.instance.Get ("TID_GOLD");
		ResourceProduceLabel.text = data.GoldProduce.ToString ();
		ResourceProduceFromHomeLabel.text = data.GoldProduceFromHome.ToString ();
		ResourceProduceFromVillageLabel.text = data.GoldProduceFromVillage.ToString();
		ResourceStorageCapacityLabel.text = data.GoldStorageCapacity.ToString();
		ResourceProtectedLabel.text = data.GoldProtected.ToString();
	}
	//关于更新金币;



	//关于更新木材;
	public UISprite WoodBar;
	public UILabel WoodCurrent;
	
	private bool IsUpdateWoodResource;
	private int currentWood;
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

			if(data.WoodStorageCapacity>0)
			WoodBar.transform.localScale = new Vector3(data.WoodCurrent*1f/data.WoodStorageCapacity,1f,1f);
			WoodCurrent.text = data.WoodCurrent.ToString();
		}
	}
	void OnUpdateWoodResource()
	{
		if(currentWood<data.WoodCurrent)
		{
			currentWood+= Mathf.CeilToInt(increaseStep*(data.WoodCurrent-data.OldWoodCurrent));
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
		
		WoodBar.transform.localScale = new Vector3(percent,WoodBar.transform.localScale.y,1f);
		WoodCurrent.text = currentWood.ToString();
	}
	public void UpdateWoodResourceTip()
	{
		ScreenUIManage.Instance.data.WoodProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_WOODCUTTER");
		ScreenUIManage.Instance.data.WoodProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_WOODCUTTER");
		ScreenUIManage.Instance.data.WoodProduce = ScreenUIManage.Instance.data.WoodProduceFromHome + ScreenUIManage.Instance.data.WoodProduceFromVillage;
		ScreenUIManage.Instance.data.WoodStorageCapacity = DataManager.GetInstance().userInfo.max_stone_count;
		
		BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
		int prot_num = 0;
		if (vault != null){
			//金币;
			if (DataManager.GetInstance().userInfo.wood_count < vault.csvInfo.MaxStoredResourceWood){
				prot_num = DataManager.GetInstance().userInfo.wood_count;
			}else{
				//被保护的资源;
				prot_num = (int)((DataManager.GetInstance().userInfo.wood_count - vault.csvInfo.MaxStoredResourceWood) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceWood;
			}
		}		
		ScreenUIManage.Instance.data.WoodProtected = prot_num;//

		ResourceName.text = LocalizationCustom.instance.Get ("TID_WOOD");
		ResourceProduceLabel.text = data.WoodProduce.ToString ();
		ResourceProduceFromHomeLabel.text = data.WoodProduceFromHome.ToString ();
		ResourceProduceFromVillageLabel.text = data.WoodProduceFromVillage.ToString();
		ResourceStorageCapacityLabel.text = data.WoodStorageCapacity.ToString();
		ResourceProtectedLabel.text = data.WoodProtected.ToString();
	}
	//关于更新木材;
	


	//关于更新石头;
	public UISprite StoneBar;
	public UILabel StoneCurrent;
	
	private bool IsUpdateStoneResource;
	private int currentStone;
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
				if (StoneBar.transform.parent.gameObject.activeSelf == false)
					StoneBar.transform.parent.gameObject.SetActive(true);

				StoneBar.transform.localScale = new Vector3(data.StoneCurrent*1f/data.StoneStorageCapacity,1f,1f);
				StoneCurrent.text = data.StoneCurrent.ToString();
			}else{
				StoneBar.transform.parent.gameObject.SetActive(false);
			}


		}
	}
	void OnUpdateStoneResource()
	{
		if(currentStone<data.StoneCurrent)
		{
			currentStone+= Mathf.CeilToInt(increaseStep*(data.StoneCurrent-data.OldStoneCurrent));
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
		
		StoneBar.transform.localScale = new Vector3(percent,StoneBar.transform.localScale.y,1f);
		StoneCurrent.text = currentStone.ToString();
	}
	public void UpdateStoneResourceTip()
	{
		ScreenUIManage.Instance.data.StoneProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_STONE_QUARRY");
		ScreenUIManage.Instance.data.StoneProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_STONE_QUARRY");
		ScreenUIManage.Instance.data.StoneProduce = ScreenUIManage.Instance.data.StoneProduceFromHome + ScreenUIManage.Instance.data.StoneProduceFromVillage;
		ScreenUIManage.Instance.data.StoneStorageCapacity = DataManager.GetInstance().userInfo.max_stone_count;
		
		BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
		int prot_num = 0;
		if (vault != null){
			//金币;
			if (DataManager.GetInstance().userInfo.stone_count < vault.csvInfo.MaxStoredResourceStone){
				prot_num = DataManager.GetInstance().userInfo.stone_count;
			}else{
				//被保护的资源;
				prot_num = (int)((DataManager.GetInstance().userInfo.stone_count - vault.csvInfo.MaxStoredResourceStone) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceStone;
			}
		}		
		ScreenUIManage.Instance.data.StoneProtected = prot_num;//

		ResourceName.text = LocalizationCustom.instance.Get ("TID_STONE");
		ResourceProduceLabel.text = data.StoneProduce.ToString ();
		ResourceProduceFromHomeLabel.text = data.StoneProduceFromHome.ToString ();
		ResourceProduceFromVillageLabel.text = data.StoneProduceFromVillage.ToString();
		ResourceStorageCapacityLabel.text = data.StoneStorageCapacity.ToString();
		ResourceProtectedLabel.text = data.StoneProtected.ToString();
	}
	//关于更新石头;
	


	//关于更新铁;
	public UISprite IronBar;
	public UILabel IronCurrent;
	
	private bool IsUpdateIronResource;
	private int currentIron;
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
				if (IronBar.transform.parent.gameObject.activeSelf == false)
					IronBar.transform.parent.gameObject.SetActive(true);
				IronBar.transform.localScale = new Vector3(data.IronCurrent*1f/data.IronStorageCapacity,1f,1f);
				IronCurrent.text = data.IronCurrent.ToString();
			}else{
				IronBar.transform.parent.gameObject.SetActive(false);
			}


		}
	}
	void OnUpdateIronResource()
	{
		if(currentIron<data.IronCurrent)
		{
			currentIron+= Mathf.CeilToInt(increaseStep*(data.IronCurrent-data.OldIronCurrent));
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
		
		IronBar.transform.localScale = new Vector3(percent,IronBar.transform.localScale.y,1f);
		IronCurrent.text = currentIron.ToString();
	}
	public void UpdateIronResourceTip()
	{
		ScreenUIManage.Instance.data.IronProduceFromHome = Helper.ResPerHourByBase("TID_BUILDING_METAL_MINE");
		ScreenUIManage.Instance.data.IronProduceFromVillage = Helper.ResPerHourByIsland("TID_BUILDING_METAL_MINE");
		ScreenUIManage.Instance.data.IronProduce = ScreenUIManage.Instance.data.IronProduceFromHome + ScreenUIManage.Instance.data.IronProduceFromVillage;
		ScreenUIManage.Instance.data.IronStorageCapacity = DataManager.GetInstance().userInfo.max_iron_count;
		
		BuildInfo vault = Helper.getBuildInfoByTid("TID_BUILDING_VAULT");
		int prot_num = 0;
		if (vault != null){
			//金币;
			if (DataManager.GetInstance().userInfo.iron_count < vault.csvInfo.MaxStoredResourceIron){
				prot_num = DataManager.GetInstance().userInfo.iron_count;
			}else{
				//被保护的资源;
				prot_num = (int)((DataManager.GetInstance().userInfo.iron_count - vault.csvInfo.MaxStoredResourceIron) * vault.csvInfo.ResourceProtectionPercent * 0.01) + vault.csvInfo.MaxStoredResourceIron;
			}
		}		
		ScreenUIManage.Instance.data.IronProtected = prot_num;//

		ResourceName.text = LocalizationCustom.instance.Get ("TID_METAL");
		ResourceProduceLabel.text = data.IronProduce.ToString ();
		ResourceProduceFromHomeLabel.text = data.IronProduceFromHome.ToString ();
		ResourceProduceFromVillageLabel.text = data.IronProduceFromVillage.ToString();
		ResourceStorageCapacityLabel.text = data.IronStorageCapacity.ToString();
		ResourceProtectedLabel.text = data.IronProtected.ToString();
	}
	//关于更新铁;
	
	//关于更新钻石;
	public UILabel DiamondCurrent;
	
	private bool IsUpdateDiamondResource;
	private int currentDiamond;
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
			DiamondCurrent.text = data.DiamondCurrent.ToString();
		}
	}
	void OnUpdateDiamondResource()
	{
		if(currentDiamond<data.DiamondCurrent)
		{
			currentDiamond+= Mathf.CeilToInt(increaseStep*(data.DiamondCurrent-data.OldDiamondCurrent));
		}
		else
		{
			currentDiamond = data.DiamondCurrent;
			IsUpdateDiamondResource = false;
			data.OldDiamondCurrent = data.DiamondCurrent;
		
		}		
		DiamondCurrent.text = currentDiamond.ToString();
	}

	//关于更新钻石;


	//关于更新商城数量;
	public Transform ShopCounter;
	public UILabel ShopCounterLabel;
	public void SetShopCount(int count)
	{
		ShopCounterLabel.text = count.ToString();
		if(count==0)
		{
			ShopCounter.gameObject.SetActive(false);
		}
		else
		{
			ShopCounter.gameObject.SetActive(true);
		}
	}

	//设置成熟按钮数量;
	//主堡，金币库升级完，树，石头移除完 也需要重新刷新一下;
	public Transform AchievementCounter;
	public UILabel AchievementCounterLabel;
	public void SetAchievementCount(int count)
	{
		AchievementCounterLabel.text = count.ToString();
		if(count==0)
		{
			AchievementCounter.gameObject.SetActive(false);
		}
		else
		{
			AchievementCounter.gameObject.SetActive(true);
		}
	}


	public GameObject PopWorld;
	//eai = null时，隐藏，反之显示;
	public void showEnemyBox(EnemyActivityItem eai, Vector3 screenPoint)
	{
		//Debug.Log("showEnemyBox");
		WorldEvent we = PopWorld.GetComponent<WorldEvent>();
		we.showEnemyBox(eai);
	}
	//show_type;1:go home; 2:Scout,Attack; 3:Scout,Attack,Find; 4:Explore; 5:Production per hour;
	//isup：true向上显示;
	public void ShowPopWorld(UserRegions ur, Vector3 screenPoint, bool is_show, int show_type = 0,bool isup = true, Regions rs = null)
	{
		//Debug.Log("ShowPopWorld");
		if (is_show){

			WorldEvent we = PopWorld.GetComponent<WorldEvent>();

			we.ur = ur;
			we.showEnemyBox(null);


			GameObject popBox1 = PopWorld.transform.Find("popBox1").gameObject;
			GameObject popBox2 = PopWorld.transform.Find("popBox2").gameObject;
			GameObject popBox3 = PopWorld.transform.Find("popBox3").gameObject;
			GameObject popBox4 = PopWorld.transform.Find("popBox4").gameObject;
			GameObject popBox5 = PopWorld.transform.Find("popBox5").gameObject;

			if (we.rs != null){
				WorldBtnEvent.Instance.ResumeCloudColor(we.rs.cloud);
			}

			GameObject popBox = null;

			popBox1.transform.localPosition = Vector3.zero;
			popBox2.transform.localPosition = Vector3.zero;
			popBox3.transform.localPosition = Vector3.zero;
			popBox4.transform.localPosition = Vector3.zero;
			popBox5.transform.localPosition = Vector3.zero;
			if (show_type == 1){
				popBox1.SetActive(true);
				popBox2.SetActive(false);
				popBox3.SetActive(false);
				popBox4.SetActive(false);
				popBox5.SetActive(false);
				popBox = popBox1;

				if (ur.res_tid == null || ur.res_tid == ""){
					popBox.transform.Find("HomeBtn/Label").GetComponent<UILabel>().text = StringFormat.FormatByTid("TID_GO_TO_BASE_BUTTON");
				}else{
					//TID_BUTTON_GO_TO_OUTPOST = 登上该岛;
					popBox.transform.Find("HomeBtn/Label").GetComponent<UILabel>().text = StringFormat.FormatByTid("TID_BUTTON_GO_TO_OUTPOST");
				}
			}else if (show_type == 2){
				popBox1.SetActive(false);
				popBox2.SetActive(true);
				popBox3.SetActive(false);
				popBox4.SetActive(false);
				popBox5.SetActive(false);
				popBox = popBox2;

				ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;

				//花费;
				//Debug.Log(el.AttackCost);
				UILabel AttackGoldNum = popBox2.transform.Find("AttackBtn/gold_num").GetComponent<UILabel>();
				AttackGoldNum.text = el.AttackCost.ToString();
			}else if (show_type == 3){
				popBox1.SetActive(false);
				popBox2.SetActive(false);
				popBox3.SetActive(true);
				popBox4.SetActive(false);
				popBox5.SetActive(false);


				popBox = popBox3;

				ExperienceLevels el = CSVManager.GetInstance().experienceLevelsList[DataManager.GetInstance().userInfo.exp_level.ToString()] as ExperienceLevels;
				//花费;
				//Debug.Log(el.AttackCost);
				UILabel AttackGoldNum = popBox3.transform.Find("AttackBtn/gold_num").GetComponent<UILabel>();
				AttackGoldNum.text = el.AttackCost.ToString();
			}else if (show_type == 4){
				popBox1.SetActive(false);
				popBox2.SetActive(false);
				popBox3.SetActive(false);
				popBox4.SetActive(false);
				popBox5.SetActive(false);


				if (rs.sending == false){
					popBox4.SetActive(true);
					popBox = popBox4;
					we.rs = rs;
					UILabel ExploreGoldNum = popBox4.transform.Find("ExploreBtn/gold_num").GetComponent<UILabel>();
					ExploreGoldNum.text = rs.ExplorationCost.ToString();//el.AttackCost.ToString();
				}
			}else if (show_type == 5){
				popBox1.SetActive(false);
				popBox2.SetActive(false);
				popBox3.SetActive(false);
				popBox4.SetActive(false);
				popBox5.SetActive(true);

				popBox = popBox5;
				
				UILabel ProductionPerHour = popBox5.transform.Find("Production per hour").GetComponent<UILabel>();
				ProductionPerHour.text = StringFormat.FormatByTid("TID_NUM_LIBERATED",new object[]{40});
			}

			if (popBox != null){
				//int heigth = popBox.transform.GetComponent<UISprite>().height;
				GameObject popArrowDown = popBox.transform.Find("popArrowDown").gameObject;
				GameObject popArrowUp = popBox.transform.Find("popArrowUp").gameObject;

				//UIAnchor anchor = popBox.transform.GetComponent<UIAnchor>();

	
				//Debug.Log("isup:" + isup + ";y:" + screenPoint.y + ";Screen.height:" + Screen.height + ";heigth:" + heigth);					



				if (isup){	
					screenPoint = uiCamera.ScreenToWorldPoint(screenPoint);
					popArrowDown.SetActive(false);
					popArrowUp.SetActive(true);
					PopWorld.transform.position = new Vector3(screenPoint.x,screenPoint.y,0);
				}else{
					float yh = popArrowUp.transform.position.y - popArrowDown.transform.position.y;
					//Debug.Log(yh);

					screenPoint = uiCamera.ScreenToWorldPoint(screenPoint);
					popArrowDown.SetActive(true);
					popArrowUp.SetActive(false);
					PopWorld.transform.position = new Vector3(screenPoint.x,screenPoint.y + yh,0);

				}
				//anchor.enabled = true;
				


				PopWorld.SetActive(true);
			}else{
				PopWorld.SetActive(false);
			}
		}else{
			if (PopWorld.activeSelf){
				WorldEvent we = PopWorld.GetComponent<WorldEvent>();

//				GameObject popBox4 = PopWorld.transform.Find("popBox4").gameObject;
				if (we.rs != null){
					WorldBtnEvent.Instance.ResumeCloudColor(we.rs.cloud);
				}
				PopWorld.SetActive(false);
			}
		}
	}


	public void OnClickBattleTrooper(GameObject sender)
	{
		BattleItem item = sender.GetComponent<BattleItem> ();

		for(int i=0;i<battleTrooperList.LineUpBattleTrooperItems.Length;i++)
		{
			if(battleTrooperList.LineUpBattleTrooperItems[i]!=item)
			battleTrooperList.LineUpBattleTrooperItems[i].current = false;
		}
		for(int i=0;i<battleTrooperList.LineDownBattleTrooperItems.Length;i++)
		{
			if(battleTrooperList.LineDownBattleTrooperItems[i]!=item)
			battleTrooperList.LineDownBattleTrooperItems[i].current = false;
		}

		for(int i=0;i<battleWeaponList.LineUpBattleWeaponItems.Length;i++)
		{
			if(battleWeaponList.LineUpBattleWeaponItems[i]!=item)
			battleWeaponList.LineUpBattleWeaponItems[i].current = false;
		}
		for(int i=0;i<battleWeaponList.LineDownBattleWeaponItems.Length;i++)
		{
			if(battleWeaponList.LineDownBattleWeaponItems[i]!=item)
			battleWeaponList.LineDownBattleWeaponItems[i].current = false;
		}

		if(!item.current)
		item.current = true;

		BattleData.Instance.currentSelectBtd = item.btd;

	}


	public UILabel CountDownLabel; //倒计时的标题;
	public UILabel CountDownCouterLabel;  //倒计时的时间;
	public UILabel EndBattleLabel;   //结束战斗的文字;

	public BattleResult battleResultWin;


	public UILabel AttackGoldLabel;//攻击金额;
	public UILabel FindGoldLabel;//搜索对手金额;

}
