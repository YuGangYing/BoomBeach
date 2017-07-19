using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using BoomBeach;

public enum PopDialogBtnType{
	YesAndNoBtn,
	ConfirmBtn,
	ImageBtn,
	InputYesNoBtn
}

public class PopManage : MonoBehaviour {

	public PopDialog popDialog;  //对话弹窗组件;

	private Transform currentDialogContent; //当前的对话框内容对象;

	public PopWin popWin;  //弹窗组件;
	private Transform currentWinContent; //当前的弹窗内容对象;

	public PopPanel popPanel;  //弹底部面板组件;

	public List<Transform> popList = new List<Transform>();

	public GameObject WorldMap;

	//=================对话框==========================
	public Transform DoxDialog; //对话框按钮类型;
	public delegate void OnDialogDelegate(ISFSObject dt);
	// 提示框确定，取消回调;
	private OnDialogDelegate onDialogYes, onDialogNo, onOpenShopYes;
	private ISFSObject dlgDialogParms;//代理回调参数;
	//=================对话框==========================

	//单例定义;
	private static PopManage instance;
	public static PopManage Instance{
		get{ return instance; }
	}
	void Awake()
	{
		instance = this;	
		popDialog.Init ();
		popWin.Init();
		popPanel.Init ();
	}

	public void ShowMsg(string msg){
        UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
	}

	public BuildButton addBuildButton(BuildButtonType btnType,BuildInfo buildInfo){
		BuildButton btn = new BuildButton();
		btn.buildInfo = buildInfo;
		//btn.OnClick = new List<EventDelegate>();
		btn.Type = btnType;
		btn.Status = true;
		
		//信息, 移除，采集，研究(兵，武器,地雷 升级)，雕像（create,deploy,reclaim），训练(生产兵);
		if (btn.Type == BuildButtonType.INFO){
			btn.IcoName = "BuildInfoIco";
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}else if (btn.Type == BuildButtonType.REMOVE){
			btn.IcoName = "BuildRemoveIco";
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}else if (btn.Type == BuildButtonType.UPGRADE){
			btn.IcoName = "BuildUpgradeIco";
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}else if (btn.Type == BuildButtonType.RESEARCH){
			btn.IcoName = "BuildResearchIco";
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}else if (btn.Type == BuildButtonType.TRAIN){
			btn.IcoName = "BuildTrainIco";
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}else if (btn.Type == BuildButtonType.BUILD){
			btn.IcoName = "BuildBuildico";
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}else if (btn.Type == BuildButtonType.COLLECT){
			
			//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
			btn.Status = Helper.CollectStatus(buildInfo) > 0;
			if (buildInfo.tid == "TID_BUILDING_HOUSING"){
				btn.IcoName = "goldIco";
			}else if (buildInfo.tid == "TID_BUILDING_STONE_QUARRY"){
				btn.IcoName = "stoneIco";
			}else if (buildInfo.tid == "TID_BUILDING_METAL_MINE"){
				btn.IcoName = "ironIco";
			}else if (buildInfo.tid == "TID_BUILDING_WOODCUTTER"){
				btn.IcoName = "woodIco";
			}
			//btn.OnClick.Add(new EventDelegate(this,"infotest"));
		}
		
		//Debug.Log("dxxxx");
		return btn;
	}

	//通过语言key,显示内容;
	public void ShowMsgByTid(string tid,System.Object[] args = null){
		ShowMsg(StringFormat.FormatByTid(tid,args));
	}

	/*
	public void RefreshBuildBtn(BuildInfo buildInfo)
	{

        if (buildInfo.buildUI != null)
        {
            buildInfo.buildUI.RefreshBuildBtn();
            return;
        }
		BuildUIManage uiManage = buildInfo.GetComponent<BuildUIManage> ();
		if(buildInfo.isSelected)
		{
//			CsvInfo csvInfo = CSVManager.GetInstance.csvTable [buildInfo.tid_level] as CsvInfo;
			//设置名称;
			uiManage.SetName (buildInfo.ShowName);// StringFormat.FormatByTid(csvInfo.TID));
			uiManage.SetLevel (buildInfo.ShowLevelName);// StringFormat.FormatByTid("TID_LEVEL_NUM",new string[]{buildInfo.level.ToString()}));
			if(buildInfo.status==BuildStatus.Create)
			{
				if(buildInfo.CheckBuildPlaceAble())
					uiManage.SetNewOk(true);
				else
					uiManage.SetNewOk(false);
				uiManage.ShowNameAndTitle(false);
				uiManage.ShowNewBox(true);
				uiManage.ShowBattleInfo(false);
			}
			else
			{
				uiManage.ShowNameAndTitle(true);
				uiManage.ShowNewBox(false);
				uiManage.ShowBattleInfo(true);
			}
			
			if(Globals.sceneStatus==SceneStatus.HOME||Globals.sceneStatus==SceneStatus.HOMERESOURCE)
			{
				if (buildInfo.status == BuildStatus.New
				    || buildInfo.status == BuildStatus.Upgrade
				    || buildInfo.status == BuildStatus.Research
				    || buildInfo.status == BuildStatus.Train
				    || buildInfo.status == BuildStatus.CreateStatue
				    || buildInfo.status == BuildStatus.Removal)
				{
					if(buildInfo.end_time-buildInfo.start_time>0)
					{
						uiManage.ShowTimeBar(false);
						uiManage.PopInstantPanel();
						uiManage.CloseDownButtons(true);
					}
				}			
			}
			
			
			
			if(buildInfo.status==BuildStatus.Normal)
			{
				uiManage.CloseInstant();
				
				List<BuildButton> buttons = new List<BuildButton> ();
				
				if (buildInfo.tid_type == "OBSTACLES"){
					//1 移除;
					buttons.Add(addBuildButton(BuildButtonType.REMOVE,buildInfo));
				}else if (buildInfo.tid_type == "TRAPS"){
					//1 信息;
					buttons.Add(addBuildButton(BuildButtonType.INFO,buildInfo));
				}else if (buildInfo.tid_type == "ARTIFACTS"){
					//2 信息, 移除;
					buttons.Add(addBuildButton(BuildButtonType.INFO,buildInfo));
					if(Globals.sceneStatus==SceneStatus.HOME)
					buttons.Add(addBuildButton(BuildButtonType.REMOVE,buildInfo));
					
				}else if (buildInfo.tid_type == "BUILDING"){
					if (buildInfo.csvInfo.BuildingClass != "Artifact"){
						buttons.Add(addBuildButton(BuildButtonType.INFO,buildInfo));

						if(Globals.sceneStatus==SceneStatus.HOME)
						{
							if (Helper.checkHasNextLevel(buildInfo)){
								buttons.Add(addBuildButton(BuildButtonType.UPGRADE,buildInfo));
							}
							//3 信息, 升级， 采集;
							if (buildInfo.tid == "TID_BUILDING_HOUSING" || buildInfo.tid == "TID_BUILDING_STONE_QUARRY" 
							    || buildInfo.tid == "TID_BUILDING_METAL_MINE" ||   buildInfo.tid == "TID_BUILDING_WOODCUTTER" ){
								buttons.Add(addBuildButton(BuildButtonType.COLLECT,buildInfo));
								
							}else if (buildInfo.tid == "TID_BUILDING_LABORATORY"){
								//3 信息, 升级， 研究(兵，武器,地雷 升级);
								buttons.Add(addBuildButton(BuildButtonType.RESEARCH,buildInfo));
							}else if (buildInfo.tid == "TID_BUILDING_ARTIFACT_WORKSHOP"){
								//3 信息, 升级， 雕像（create,deploy,reclaim）;
								buttons.Add(addBuildButton(BuildButtonType.BUILD,buildInfo));
							}else if (buildInfo.tid == "TID_BUILDING_LANDING_SHIP"){
								//3 信息, 升级， 训练(生产兵);
								buttons.Add(addBuildButton(BuildButtonType.TRAIN,buildInfo));
							}
						}
					}else{
						//2 信息, 移除;
						buttons.Add(addBuildButton(BuildButtonType.INFO,buildInfo));
						if(Globals.sceneStatus==SceneStatus.HOME)
						buttons.Add(addBuildButton(BuildButtonType.REMOVE,buildInfo));
					}
				}

                if (Globals.sceneStatus != SceneStatus.ENEMYVIEW && Globals.sceneStatus != SceneStatus.ENEMYBATTLE && Globals.sceneStatus != SceneStatus.BATTLEREPLAY && Globals.sceneStatus != SceneStatus.FRIENDVIEW)
                {
                    if(buildInfo.buildUIManage!=null)
                        buildInfo.buildUIManage.PopDownButtons(buttons);
                    if (buildInfo.buildUI != null)
                        buildInfo.buildUI.PopDownButtons(buttons);
                }
                    
			}
		}
		else
		{
			if(buildInfo.status!=BuildStatus.Create)
			{

				uiManage.ShowNameAndTitle (false);
				uiManage.ShowBattleInfo(false);
				
				if(buildInfo.status==BuildStatus.Normal)
				{

					uiManage.CloseDownButtons(!buildInfo.isAfterResetPos);
					uiManage.ShowTimeBar(false);
				}

				if(Globals.sceneStatus==SceneStatus.HOME||Globals.sceneStatus==SceneStatus.HOMERESOURCE)
				{
					if(buildInfo.status==BuildStatus.New
					   ||buildInfo.status==BuildStatus.Upgrade
					   ||buildInfo.status==BuildStatus.Research
					   ||buildInfo.status==BuildStatus.Train
					   ||buildInfo.status==BuildStatus.CreateStatue
					   ||buildInfo.status==BuildStatus.Removal)
					{
						uiManage.CloseInstant(!buildInfo.isAfterResetPos);

						if(buildInfo.end_time>Helper.current_time())
						uiManage.ShowTimeBar(true);
					}
				}
				
			}
		}
	}
*/

	//对话框关闭事件;
	public void onCancelDialog()
	{
		//Debug.Log("onCancel");
		if(PopDialog.current!=null){
			//Debug.Log("onCancel2");
			PopDialog.current.AfterClose = null;//取消事件不能再执行，直接关闭界面即可;
			PopDialog.current.CloseTween();
		}
		if (onDialogNo != null){
			onDialogNo(dlgDialogParms);
		}
	}

	//对话框确定事件;
	public void onSureDialog()
	{
		//Debug.Log ("onSureDialog");
		//如果是输入框对话框，则获取输入框内容，并返回;
		if (dlgDialogParms != null && dlgDialogParms.GetBool("is_input_box")){			
			UILabel input_txt = DoxDialog.Find("Input/Label").GetComponent<UILabel>();

			dlgDialogParms.PutUtfString("input_txt",input_txt.text);
			dlgDialogParms.RemoveElement("is_input_box");
			input_txt.text = "";
			DoxDialog.Find("Input").GetComponent<UIInput>().value = "";
			//Debug.Log(dlgDialogParms.GetDump());
		}

		//Debug.Log("onSure");
		if(PopDialog.current!=null){
			//Debug.Log("onSure2");
			PopDialog.current.AfterClose = null;//取消事件不能再执行，直接关闭界面即可;
			PopDialog.current.CloseTween();
		}

		if (onDialogYes != null){
			//
			onDialogYes(dlgDialogParms);
		}
		else
		{
			Debug.Log("onDialogYes == null");
		}
	}

	private void onOpenShopWin(ISFSObject dt){
		PopManage.Instance.ShowDiamondPanel ();
		if (onOpenShopYes != null){
			onOpenShopYes(dt);
		}
	}

	//显示需要更多宝石对话框;
	public void ShowNeedGemsDialog(OnDialogDelegate onSure = null, 
	                               OnDialogDelegate onCancel = null
	                               ){
		//宝石不够;
		//TID_POPUP_NOT_ENOUGH_BUY_GEMS 
		//TID_POPUP_NOT_ENOUGH_DIAMONDS_TITLE	Not enough Diamonds
		//TID_POPUP_NOT_ENOUGH_DIAMONDS	Do you want to get more?
		//TID_POPUP_NOT_ENOUGH_DIAMONDS_BUTTON	Enter Shop
		onOpenShopYes = onSure;
		PopManage.Instance.ShowDialog(
			LocalizationCustom.instance.Get("TID_POPUP_NOT_ENOUGH_DIAMONDS"),
			LocalizationCustom.instance.Get("TID_POPUP_NOT_ENOUGH_DIAMONDS_TITLE"),
			true,
			PopDialogBtnType.ConfirmBtn,
			true,
			null,
			onOpenShopWin,
			onCancel,
			LocalizationCustom.instance.Get("TID_POPUP_NOT_ENOUGH_DIAMONDS_BUTTON")
			);
	}
	
	/*仅显示消信的对话框
	msg:对话框内容;
	title:对话框标题;
	closeOther:显示对话框时,是否先关闭其它界面;
	btnType:对话框类型;参考 PopDialogBtnType
	showLeftTopClose: 是否显示：右上角的关闭按钮
	otherParms:对话框,回调事件参数
	onSure: 确定回调事件;
	onCancel: 取消回调事件;
	SureTitle: 确定按钮名字(默认为：确定)
	CancelTitel: 取消按钮名字(默认为：取消)
	 */
	public void ShowDialog(
		string msg,
		string title = null,
		bool closeOther = true,
		PopDialogBtnType btnType = PopDialogBtnType.ConfirmBtn, 
		bool showLeftTopClose = true,
		ISFSObject otherParms = null, 
		OnDialogDelegate onSure = null, 
		OnDialogDelegate onCancel = null,
		string SureTitle = null,
		string CancelTitel = null,
		string ImageName = "diamondIco")
	{

		dlgDialogParms = otherParms;
		onDialogYes = onSure;
		onDialogNo = onCancel;


		UILabel MsgText = DoxDialog.Find("MsgDialog").GetComponent<UILabel>();
		UILabel InputLable = DoxDialog.Find("InputLable").GetComponent<UILabel>();
		GameObject InputBox = DoxDialog.Find("Input").gameObject;


		MsgText.gameObject.SetActive(true);
		InputLable.gameObject.SetActive(false);
		InputBox.SetActive(false);

		//出现2个按钮，确定与取消;
		Transform YesBtn = DoxDialog.Find("YesBtn");
		Transform NoBtn = DoxDialog.Find("NoBtn");
		//仅出现一个：确定 按钮;
		Transform ConfirmBtn = DoxDialog.Find("ConfirmBtn");
		//仅出现一个带：图标的 确定按钮;
		Transform ImageBtn = DoxDialog.Find("ImageBtn");


		if (btnType == PopDialogBtnType.YesAndNoBtn){
			YesBtn.gameObject.SetActive(true);
			NoBtn.gameObject.SetActive(true);
			ConfirmBtn.gameObject.SetActive(false);
			ImageBtn.gameObject.SetActive(false);
		}else if (btnType == PopDialogBtnType.ConfirmBtn){
			YesBtn.gameObject.SetActive(false);
			NoBtn.gameObject.SetActive(false);
			ConfirmBtn.gameObject.SetActive(true);
			ImageBtn.gameObject.SetActive(false);
		}else if (btnType == PopDialogBtnType.ImageBtn){
			YesBtn.gameObject.SetActive(false);
			NoBtn.gameObject.SetActive(false);
			ConfirmBtn.gameObject.SetActive(false);
			ImageBtn.gameObject.SetActive(true);
		}else if (btnType == PopDialogBtnType.InputYesNoBtn){
			YesBtn.gameObject.SetActive(true);
			NoBtn.gameObject.SetActive(true);
			ConfirmBtn.gameObject.SetActive(false);
			ImageBtn.gameObject.SetActive(false);

			InputLable.gameObject.SetActive(true);
			InputLable.text = msg;
			InputBox.SetActive(true);
			MsgText.gameObject.SetActive(false);

			if (dlgDialogParms == null){
				dlgDialogParms = new SFSObject();
			}

			DoxDialog.Find("Input/Label").GetComponent<UILabel>().text = "";
			DoxDialog.Find("Input").GetComponent<UIInput>().value = "";

			dlgDialogParms.PutBool("is_input_box",true);
		}
		

		MsgText.text = msg;
		
		if (SureTitle == null){
			SureTitle = LocalizationCustom.instance.Get("TID_BUTTON_OKAY");
		}
		YesBtn.Find("BtnTitle").GetComponent<UILabel>().text = SureTitle;
		ConfirmBtn.Find("BtnTitle").GetComponent<UILabel>().text = SureTitle;
		ImageBtn.Find("BtnTitle").GetComponent<UILabel>().text = SureTitle;
		
		
		if (CancelTitel == null){
			CancelTitel = LocalizationCustom.instance.Get("TID_BUTTON_CANCEL");
		}
		NoBtn.Find("BtnTitle").GetComponent<UILabel>().text = CancelTitel;

		//绑定取消事件;
		List<EventDelegate> onCancelDelegate = new List<EventDelegate> ();
		onCancelDelegate.Add (new EventDelegate(this,"onCancelDialog"));		
		NoBtn.Find("BtnBg").GetComponent<UIButton> ().onClick = onCancelDelegate;

		//绑定确定事件;
		List<EventDelegate> onSureDelegate = new List<EventDelegate> ();
		onSureDelegate.Add (new EventDelegate(this,"onSureDialog"));		
		YesBtn.Find("BtnBg").GetComponent<UIButton> ().onClick = onSureDelegate;
		ConfirmBtn.Find("BtnBg").GetComponent<UIButton> ().onClick = onSureDelegate;
		ImageBtn.Find("BtnBg").GetComponent<UIButton> ().onClick = onSureDelegate;

		//图标;
		ImageBtn.Find("BtnIco").GetComponent<UISprite>().spriteName = ImageName;



		if(closeOther)
		{
			if(PopWin.current!=null)PopWin.current.CloseTween();
		}

		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentDialogContent != null)
		{
			currentDialogContent.gameObject.SetActive (false);	
		}
		currentDialogContent = DoxDialog;
		currentDialogContent.gameObject.SetActive (true);

		//Debug.Log("popDialog.AfterClose = onCancelDialog");

		popDialog.Width = 850;
		popDialog.Height = 550;
		popDialog.OpenWin (title);	
		popDialog.AfterClose = onCancelDialog;
		                   
	}



	public Transform TestWin; 
	public void ShowTestWin()
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = TestWin;
		currentWinContent.gameObject.SetActive (true);

		UIButton btn1 = TestWin.Find("testBtn1").GetComponent<UIButton> ();
		btn1.onClick = new List<EventDelegate> ();
		btn1.onClick.Add (new EventDelegate(this,"ClickTest1"));

		UIButton btn2 = TestWin.Find("testBtn2").GetComponent<UIButton> ();
		btn2.onClick = new List<EventDelegate> ();
		btn2.onClick.Add (new EventDelegate(this,"ClickTest2"));

		popWin.Title = "测试的标题";
		popWin.Width = 1700;
		popWin.Height = 1000;
		popWin.OpenWin ();
	}
	private void ClickTest1()
	{
		PopManage.Instance.ShowDialog ("title","test1",true);
	}
	private void ClickTest2()
	{
		PopManage.Instance.ShowDialog ("title","test2",false);
	}

	//弹底部面板商城;
	public void ShowShopPanel()
	{
		//重新计算版面数据; 由于此面板
		int count = Helper.CalcShopCates(false);

        Debug.Log("Count : "+count);

		popPanel.OpenWin (Globals.ShopCates);
	}

	public void ShowDiamondPanel(bool isEnabled = true)
	{
		//重新计算版面数据;
		Helper.CalcShopCates(true,isEnabled);

		if (isEnabled)
						popPanel.OpenWin (Globals.ShopCates);
				else
						popPanel.Refreash ();//刷新面板
		//Debug.Log(Globals.ShopCates[0].ShopItems[0].ShopCosts[0].CostAmount);
	}


	//关于建筑信息框;
	public Transform BuildInfoWin;
	public void ShowBuildInfo(BuildInfo buildInfo)
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = BuildInfoWin;
		currentWinContent.gameObject.SetActive (true);
		
		popWin.Title = buildInfo.ShowName;

		BuildInfoWin.GetComponent<BuildInfoWin> ().Init ();
		BuildInfoWin.GetComponent<BuildInfoWin> ().BindBuildInfoWin (buildInfo);

		popWin.AfterOpen = BuildInfoWin.GetComponent<BuildInfoWin> ().InitBuildModel;
		popWin.Width = 1700;
		popWin.Height = 1000;
		popWin.OpenWin ();

	}

	//关于建筑升级框;
	public Transform BuildUpgradeWin;
	public void ShowBuildUpgrade(BuildInfo buildInfo)
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = BuildUpgradeWin;
		currentWinContent.gameObject.SetActive (true);
		
		popWin.Title = StringFormat.FormatByTid ("TID_UPGRADE_TITLE",new System.Object[]{buildInfo.ShowName,buildInfo.level+1});
		
		BuildUpgradeWin.GetComponent<BuildUpgradeWin> ().Init ();
		BuildUpgradeWin.GetComponent<BuildUpgradeWin> ().BindBuildUpgradeWin (buildInfo);
		
		popWin.AfterOpen = BuildUpgradeWin.GetComponent<BuildUpgradeWin> ().InitBuildModel;
		popWin.Width = 1700;
		popWin.Height = 1100;
		popWin.OpenWin ();		
	}

	//关于研究的窗口;
	public Transform ResearchWin;
	public void ShowResearchWin()
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = ResearchWin;
		currentWinContent.gameObject.SetActive (true);
		
		popWin.Title = LocalizationCustom.instance.Get ("TID_POPUP_UPGRADE_TROOP_TITLE");
		
		ResearchWin.GetComponent<ResearchWin> ().BindReaearchWin ();

		popWin.Height = 1100;
		popWin.Width = 1700;
		popWin.OnBack = null;

		popWin.OpenWin ();	
	}

	public Transform TroopInfoWin;
	public void ShowTroopInfoWin(string tid_level,string type)
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = TroopInfoWin;
		currentWinContent.gameObject.SetActive (true);

		CsvInfo csvInfo = CSVManager.GetInstance.csvTable [tid_level] as CsvInfo;
		
		string ShowName = StringFormat.FormatByTid (csvInfo.TID);

		popWin.Title = ShowName;

		TroopInfoWin.GetComponent<TroopInfoWin> ().Init ();
		TroopInfoWin.GetComponent<TroopInfoWin> ().BindTroopInfoWin (tid_level);
		popWin.Height = 1100;

		if (type == "Research")
			popWin.OnBack = ShowResearchWin;

		if (type == "Train")
			popWin.OnBack = ShowTroopTrainWin;
		popWin.Width = 1700;
		popWin.Height = 1100;
		popWin.OpenWin ();
	}


	public Transform TroopUpgradeWin;
	public void ShowTroopUpgradeWin(ResearchTid tidData)
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = TroopUpgradeWin;
		currentWinContent.gameObject.SetActive (true);
		
		CsvInfo csvInfo = CSVManager.GetInstance.csvTable [tidData.tid_level] as CsvInfo;

		string ShowName = StringFormat.FormatByTid (csvInfo.TID);
		popWin.Title = StringFormat.FormatByTid ("TID_UPGRADE_TITLE",new System.Object[]{ShowName,csvInfo.Level+1});
		
		TroopUpgradeWin.GetComponent<TroopUpgradeWin> ().Init ();
		TroopUpgradeWin.GetComponent<TroopUpgradeWin> ().BindTroopInfoWin (tidData,true);
		popWin.Height = 1100;
		popWin.Width = 1700;
		popWin.OnBack = ShowResearchWin;
		popWin.OpenWin ();
	}

	public Transform TroopTrainWin;
	public void ShowTroopTrainWin()
	{
		BuildInfo buildInfo = Globals.currentTrainBuildInfo;
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = TroopTrainWin;
		currentWinContent.gameObject.SetActive (true);

		if (buildInfo.troops_tid != null && buildInfo.troops_tid != "" && buildInfo.troops_num > 0){
			//CsvInfo troopCsv = CSVManager.GetInstance.csvTable[buildInfo.status_tid_level] as CsvInfo;
			popWin.Title = LocalizationCustom.instance.Get ("TID_POPUP_TRAIN_TITLE");

			popWin.Title = popWin.Title + "【" + LocalizationCustom.instance.Get (buildInfo.troops_tid) + "x" + buildInfo.troops_num.ToString() + "】";
		}else{
			popWin.Title = LocalizationCustom.instance.Get ("TID_POPUP_TRAIN_TITLE");
		}

		TroopTrainWin.GetComponent<TroopTrainWin> ().BindTrainWin (buildInfo);
		popWin.Height = 1100;
		popWin.Width = 1700;
		popWin.OnBack = null;
		popWin.OpenWin ();
	}

	public Transform CreateStatueWin;
	public void ShowCreateStatueWin(BuildInfo buildInfo)
	{

		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = CreateStatueWin;
		currentWinContent.gameObject.SetActive (true);
		
		
		popWin.Title = LocalizationCustom.instance.Get ("TID_CREATE_ARTIFACTS");
		
		CreateStatueWin.GetComponent<CreateStatueWin> ().Init ();
		CreateStatueWin.GetComponent<CreateStatueWin> ().BindCreateStatueWin (buildInfo);
		popWin.Height = 1100;
		popWin.Width = 1700;			
		popWin.OnBack = null;
		popWin.OpenWin ();
	}

	//关于成就;
	public Transform AchievementsWin;
	public void ShowAchievementsWin()
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = AchievementsWin;
		currentWinContent.gameObject.SetActive (true);
		
		popWin.Title = LocalizationCustom.instance.Get ("TID_POPUP_ACHIEVEMENTS_TITLE");
		


		popWin.Width = 2000;
		popWin.Height = 1050;
		popWin.OpenWin ();

		AchievementsWin.GetComponent<AchievementsWin> ().Init ();
		//AchievementsWin.GetComponent<AchievementsWin> ().BindInfoWin();
	}

	//系统设置;
	public Transform SetWin;
	public void ShowSetWin()
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = SetWin;
		currentWinContent.gameObject.SetActive (true);
		
		popWin.Title = LocalizationCustom.instance.Get ("TID_SETTINGS_SCREEN_TITLE");
		
		SetWin.GetComponent<SetWin> ().Init ();
		SetWin.GetComponent<SetWin> ().BindInfoWin ();
		popWin.Height = 900;
		popWin.Width = 980;

		popWin.OpenWin ();
		
	}

	//用户排行;
	public Transform TopPlayersWin;
	public void ShowTopPlayersWin()
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = TopPlayersWin;
		currentWinContent.gameObject.SetActive (true);
		
		popWin.Title = LocalizationCustom.instance.Get ("TID_PLAYER_LEADERBOARDS");
		
		TopPlayersWin.GetComponent<TopPlayersWin> ().Init ();
		//AchievementsWin.GetComponent<AchievementsWin> ().BindBuildInfoWin (buildInfo);
		popWin.Width = 1700;
		popWin.Height = 1300;		
		popWin.OpenWin ();
		
	}

	//帮助与支持;
	public Transform HelpAndSupportWin;
	public void ShowHelpAndSupportWin()
	{
		//禁用当前显示的弹窗内容,并显示为指定的内容;
		if (currentWinContent != null)
		{
			Debug.Log("ShowHelpAndSupportWin");
			currentWinContent.gameObject.SetActive (false);	
		}
		currentWinContent = HelpAndSupportWin;
		currentWinContent.gameObject.SetActive (true);

		Transform WinTop = HelpAndSupportWin.parent.parent.Find("WinTop");
		UIAnchor[] anchors = WinTop.GetComponentsInChildren<UIAnchor>(true);
		//Debug.Log(anchors.Length);
		for(int i=0;i<anchors.Length;i++)
		{
			anchors[i].enabled = true;
		}

		popWin.Title = LocalizationCustom.instance.Get ("TID_FAQ_WINDOW_TITLE");
		
		HelpAndSupportWin.GetComponent<HelpAndSupportWin> ().Init ();
		//HelpAndSupportWin.GetComponent<HelpAndSupportWin> ().BindInfoWin ();
		//popWin.Height = 1000;
		//popWin.Width = 800;
		popWin.Width = 1700;
		popWin.Height = 1300;		
		popWin.OpenWin ();
		
	}


	public Transform EnemyActivity;
	public void ShowEnemyActivityWin(bool show)
	{
		//Debug.Log("ShowEnemyActivityWin:" + show.ToString());
		if (show && EnemyActivity.gameObject.activeSelf == false){
			EnemyActivity.gameObject.SetActive(show);
			EnemyActivity.GetComponent<EnemyActivityWin> ().Init ();
		}else if (!show && EnemyActivity.gameObject.activeSelf == true){
			EnemyActivity.gameObject.SetActive(false);
			EnemyActivity.GetComponent<EnemyActivityWin>().Close();
		}
	}
}
