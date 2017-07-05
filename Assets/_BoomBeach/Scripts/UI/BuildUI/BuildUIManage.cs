using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class BuildUIManage : MonoBehaviour {

	private Transform NameAndLevel;
	private UILabel NameLabel;
	private UILabel LevelLabel;

	private Transform PopDownPanel1;
	private UIButton Panel1Btn1;

	private Transform PopDownPanel2;
	private UIButton Panel2Btn1;
	private UIButton Panel2Btn2;

	private Transform PopDownPanel3;  
	private UIButton Panel3Btn1;
	private UIButton Panel3Btn2;
	private UIButton Panel3Btn3;

	private Transform PopInstant;
	//private UIButton PopInstantCancel;
	//private UIButton PopInstantOk;
	private UISprite PopInstantIco;
	private UISprite PopInstantBar;
	private UILabel PopInstantLabel;
	private UILabel PopInstantDiamondLabel;


	public Transform PopResource;
	private UISprite PopResourceArrow;
	private UISprite PopResourceBox;
	private UISprite PopResourceIco;
	private UIButton PopResourceBtn;

	private Transform PopUpPanel;
	//private UIButton PopUpCancel;
	private UIButton PopUpOk;

	private Transform TimeBar;
	private UISprite TimeBarIco;
	private UISprite TimeBarBar;
	private UILabel TimeBarLabel;

	private Transform XuXianBox;
	private UILabel XuXianLabel;

	private Transform UpdBrand;

	private BuildInfo buildInfo;

	private List<BuildButton> DownButtons;
	private List<UIButton> DownButtonsUI;
	private TweenPosition tweener;

	private Transform HealthBar;
	private UISprite HealthBarSprite;


	private Transform BuildArrows;
	//private Shader GrayShader;
	//public bool DisableCollect;
	private double time_interval = 0f;


	public BuildBattleInfo buildBattleInfo;

	public void AfterInit()
	{
		BuildTopUI btu = transform.GetComponentInChildren<BuildTopUI> ();
		if(btu!=null)
			NameAndLevel.transform.position = btu.position;
		if(btu!=null)
			PopResource.transform.position = btu.position;
		if(btu!=null)
		{
			PopUpPanel.transform.position = btu.position;
			PopUpPanel.transform.localPosition = new Vector3(PopUpPanel.transform.localPosition.x,PopUpPanel.transform.localPosition.y+100f,PopUpPanel.transform.localPosition.z);
		}
		if(btu!=null)
		{
			TimeBar.transform.position = btu.position;
			TimeBar.transform.localPosition = new Vector3(TimeBar.transform.localPosition.x,TimeBar.transform.localPosition.y+50f,TimeBar.transform.localPosition.z);
		}

		if(btu!=null)
		{
			HealthBar.transform.position = btu.position;
			HealthBar.transform.localPosition = new Vector3(HealthBar.transform.localPosition.x,HealthBar.transform.localPosition.y+50f,HealthBar.transform.localPosition.z);
		}

		if(btu!=null)
			XuXianBox.transform.position = btu.position;
	}

	public void Awake()
	{

		buildInfo = GetComponent<BuildInfo> ();
		NameAndLevel = transform.Find("UI/UIS/NameAndLevel");


		NameLabel = NameAndLevel.Find ("Name").GetComponent<UILabel> ();
		LevelLabel = NameAndLevel.Find ("Level").GetComponent<UILabel> ();

		PopDownPanel1 = transform.Find("UI/UIS/PopDownPanel1");
		Panel1Btn1 = PopDownPanel1.Find ("Btn1").GetComponent<UIButton> ();

		PopDownPanel2 = transform.Find("UI/UIS/PopDownPanel2");
		Panel2Btn1 = PopDownPanel2.Find ("Btn1").GetComponent<UIButton> ();
		Panel2Btn2 = PopDownPanel2.Find ("Btn2").GetComponent<UIButton> ();

		PopDownPanel3 = transform.Find("UI/UIS/PopDownPanel3");
		Panel3Btn1 = PopDownPanel3.Find ("Btn1").GetComponent<UIButton> ();
		Panel3Btn2 = PopDownPanel3.Find ("Btn2").GetComponent<UIButton> ();
		Panel3Btn3 = PopDownPanel3.Find ("Btn3").GetComponent<UIButton> ();

		PopInstant = transform.Find("UI/UIS/PopInstant");
		//PopInstantCancel = PopInstant.Find ("Btn1").GetComponent<UIButton>();
		//PopInstantOk = PopInstant.Find ("Btn2").GetComponent<UIButton>();
		PopInstantIco = PopInstant.Find ("TimeBar/IcoBg/Sprite").GetComponent<UISprite> ();
		PopInstantBar = PopInstant.Find ("TimeBar/Barbg/bar").GetComponent<UISprite> ();
		PopInstantLabel = PopInstant.Find ("TimeBar/Label").GetComponent<UILabel> ();
		PopInstantDiamondLabel =  PopInstant.Find ("Btn2/Label").GetComponent<UILabel> ();


		PopResource = transform.Find("UI/UIS/PopResource");

		PopResourceArrow = PopResource.Find ("arrow").GetComponent<UISprite> ();
		PopResourceBox = PopResource.Find ("box").GetComponent<UISprite> ();
		PopResourceIco = PopResource.Find ("Sprite").GetComponent<UISprite> ();
		PopResourceBtn = PopResource.GetComponent<UIButton> ();

		PopUpPanel = transform.Find("UI/PopUpPanel");

		//PopUpCancel = PopUpPanel.Find ("CancelBtn").GetComponent<UIButton> ();
		PopUpOk = PopUpPanel.Find ("OkBtn").GetComponent<UIButton> ();

		TimeBar = transform.Find("UI/UIS/TimeBar");

		TimeBarIco = TimeBar.Find ("IcoBg/Sprite").GetComponent<UISprite> ();
		TimeBarBar = TimeBar.Find ("Barbg/bar").GetComponent<UISprite> ();
		TimeBarLabel = TimeBar.Find ("Label").GetComponent<UILabel> ();

		HealthBar = transform.Find ("UI/UIS/HealthBar");
		if(HealthBar!=null)
		HealthBarSprite = HealthBar.Find ("Barbg/bar").GetComponent<UISprite> ();

		XuXianBox = transform.Find("UI/UIS/XuXianBox");

		XuXianLabel = XuXianBox.Find ("Label").GetComponent<UILabel> ();

		BuildArrows = transform.Find ("UI/Arrows");

		UpdBrand = transform.Find ("UpdBrand");

		//GrayShader = Shader.Find ("MyShader/GrayShader");

		buildBattleInfo = GetComponentInChildren<BuildBattleInfo> ();
		buildBattleInfo.Init ();
		buildBattleInfo.gameObject.SetActive (false);

	}

	public void ShowBattleInfo(bool is_show)
	{
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYVIEW||DataManager.GetInstance().sceneStatus==SceneStatus.FRIENDVIEW)
			buildBattleInfo.gameObject.SetActive (is_show);
		else
			buildBattleInfo.gameObject.SetActive (false);
	}

	//计算是否显示可升级的牌子;
	public void BindUpDBrand(bool is_show)
	{
		if(DataManager.GetInstance().sceneStatus==SceneStatus.HOME)
			UpdBrand.gameObject.SetActive (is_show);
		else
			UpdBrand.gameObject.SetActive (false);
	}

	//设置名称;
	public void SetName(string name)
	{
		NameLabel.text = name;
	}

	//设置等级;
	public void SetLevel(string level)
	{
		LevelLabel.text = level;
	}

	//显示名称与等级;
	public void ShowNameAndTitle(bool isShow)
	{
		NameAndLevel.gameObject.SetActive (isShow);
	}

	//显示新建的功能按钮;
	public void ShowNewBox(bool isShow)
	{
		PopUpPanel.gameObject.SetActive (isShow);
	}

	//设置新建确认按钮是否可以按下;
	public void SetNewOk(bool status)
	{

		if(status)
		{
			PopUpOk.GetComponent<UISprite>().color = new Color(0.235f,0.667f,0.274f);
			PopUpOk.GetComponent<UIButton>().enabled = true;
		}
		else
		{
			PopUpOk.GetComponent<UISprite>().color = Color.white;
			PopUpOk.GetComponent<UIButton>().enabled = false;
		}
		/*
			for(int j=0;j<UIDrawCall.activeList.size;j++)
			{
				UIDrawCall dc = UIDrawCall.activeList[j];
				if(dc.panel==PopUpOk.GetComponent<UIPanel>())
				{
					if (status)
						dc.dynamicMaterial.shader = dc.baseMaterial.shader;
					else
					dc.dynamicMaterial.shader = GrayShader;
				}
			}
		*/

	}
	
	
	//以下关于底部功能菜单的相关操作;
	private void ResetPanel()
	{
		PopDownPanel1.gameObject.SetActive (false);
		PopDownPanel2.gameObject.SetActive (false);
		PopDownPanel3.gameObject.SetActive (false);
	}

	//弹出底部菜单;
	public bool IsPopDownButtons;
	public void PopDownButtons(List<BuildButton> buttons)
	{
		ResetPanel ();
		DownButtons = buttons;
		//Debug.Log(buttons.Count);
		if(buttons.Count==1)
		{
			DownButtonsUI = new List<UIButton>();
			DownButtonsUI.Add(Panel1Btn1);
			PopDownPanel1.gameObject.SetActive(true);
			tweener = PopDownPanel1.GetComponent<TweenPosition>();
			if(!isPlayBack)
			tweener.Reset();
			tweener.onFinished = new List<EventDelegate> ();
			//Debug.Log(buildInfo.Id+":PopDownButtons1");
			tweener.PlayForward();
		}

		if(buttons.Count==2)
		{
			DownButtonsUI = new List<UIButton>();
			DownButtonsUI.Add(Panel2Btn1);
			DownButtonsUI.Add(Panel2Btn2);
			PopDownPanel2.gameObject.SetActive(true);
			tweener = PopDownPanel2.GetComponent<TweenPosition>();
			if(!isPlayBack)
			tweener.Reset();
			tweener.onFinished = new List<EventDelegate> ();
			//Debug.Log(buildInfo.Id+":PopDownButtons2");
			tweener.PlayForward();
		}

		if(buttons.Count==3)
		{
			DownButtonsUI = new List<UIButton>();
			DownButtonsUI.Add(Panel3Btn1);
			DownButtonsUI.Add(Panel3Btn2);
			DownButtonsUI.Add(Panel3Btn3);
			PopDownPanel3.gameObject.SetActive(true);
			tweener = PopDownPanel3.GetComponent<TweenPosition>();
			if(!isPlayBack)
			tweener.Reset();
			tweener.onFinished = new List<EventDelegate> ();
			//Debug.Log(buildInfo.Id+":PopDownButtons3");
			tweener.PlayForward();
		}

		SetCollectShader();

		tweener.onFinished.Add (new EventDelegate(this,"BindDownButtons"));

	}



	private bool isPlayBack;
	//关闭底部功能按钮;
	public void CloseDownButtons(bool isAnim=true)
	{
//		Debug.Log(buildInfo.Id+":CloseDownButtons");
		IsPopDownButtons = false;
		//ResourceBtnDc = null;
		if(isAnim)
		{
			//Debug.Log(buildInfo.Id+":CloseDownButtons isAnim");
			if(tweener!=null)
			{
				//Debug.Log(buildInfo.Id+":CloseDownButtons isAnim tweener!=null");

				isPlayBack = true;
				tweener.onFinished = new List<EventDelegate> ();
				tweener.onFinished.Add (new EventDelegate(this,"ResetPanel"));
				tweener.PlayReverse ();
			}
		}
		else
		{
			//Debug.Log(buildInfo.Id+":CloseDownButtons !isAnim");
			isPlayBack = false;
			ResetPanel();
		}
	}


	void OnDownBtnClick0(){
		OnDownBtnClick(DownButtons[0]);
	}
	
	void OnDownBtnClick1(){
		OnDownBtnClick(DownButtons[1]);
	}
	
	void OnDownBtnClick2(){
		OnDownBtnClick(DownButtons[2]);
	}


	void OnDownBtnClick(BuildButton btn){
		Debug.Log("OnDownBtnClick:" + btn.buildInfo.name + ";type:" + btn.Type.ToString());
		//DownButtons[0].OnClick
		if (btn.Type == BuildButtonType.INFO){
			UIManager.GetInstance().GetController<BuildDetailCtrl>().ShowInfo(btn.buildInfo);
			//PopManage.Instance.ShowBuildInfo(btn.buildInfo);
		}else if (btn.Type == BuildButtonType.REMOVE){
			btn.buildInfo.OnRemoval(btn.buildInfo);
		}else if (btn.Type == BuildButtonType.UPGRADE){
			//PopManage.Instance.ShowBuildUpgrade(btn.buildInfo);
			UIManager.GetInstance().GetController<BuildDetailCtrl>().ShowUpgrade (btn.buildInfo);
		}else if (btn.Type == BuildButtonType.RESEARCH){
			//PopManage.Instance.ShowResearchWin();
			UIManager.GetInstance().GetController<ResearchCtrl>().ShowPanel();
        }
        else if (btn.Type == BuildButtonType.TRAIN){
			Globals.currentTrainBuildInfo = btn.buildInfo;
			//PopManage.Instance.ShowTroopTrainWin();
			UIManager.GetInstance().GetController<TroopTrainCtrl>().ShowTroop();
		}else if (btn.Type == BuildButtonType.BUILD){
			//PopManage.Instance.ShowCreateStatueWin(btn.buildInfo);
			UIManager.GetInstance().GetController<ArtifactCtrl>().ShowArtifactPanel(btn.buildInfo);
        }
        else if (btn.Type == BuildButtonType.COLLECT){
			CollectHandle.Collect(btn.buildInfo);
			//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
		}
	}
	public UIDrawCall ResourceBtnDc;
	void BindDownButtons()
	{
		//Debug.Log("BindDownButtons");
		for(int i=0;i<DownButtons.Count;i++)
		{
			UISprite ico_sprite = DownButtonsUI[i].transform.Find("Sprite").GetComponent<UISprite>();
			
			if(DownButtons[i].Type==BuildButtonType.COLLECT)
			{
				ico_sprite.atlas = PartEmitObj.Instance.avatarAtlas;
			}
			else
			{
				ico_sprite.atlas = PartEmitObj.Instance.uiAtlas;
			}
			
			ico_sprite.spriteName = DownButtons[i].IcoName;
			ico_sprite.MakePixelPerfect();
			ico_sprite.transform.localScale = Vector3.one * 1.2f;
			//DownButtonsUI[i].onClick = DownButtons[i].OnClick;
			//DownButtons[i].Type
			DownButtonsUI[i].onClick = new List<EventDelegate>();
			DownButtonsUI[i].onClick.Add(new EventDelegate(this,"OnDownBtnClick"+i.ToString()));
			
			ResourceBtnDc = null;
		}
		
		IsPopDownButtons = true;

		SetCollectShader();

	}

	public void SetCollectShader(bool clear_ResourceBtnDc = false){
		if (clear_ResourceBtnDc){
			ResourceBtnDc = null;
		}
		if (DownButtons == null)
						return;
		for(int i=0;i<DownButtons.Count;i++)
		{
			//&&!buildInfo.isMoving
			if(DownButtons[i].Type==BuildButtonType.COLLECT)
			{

				//没有可采集的数量时，显示：灰色;
				if(Helper.getCollectNum(DownButtons[i].buildInfo) == 0)
				{
					//DownButtonsUI[i].GetComponent<UISprite>().color = Color.black;
					DownButtonsUI[i].transform.Find("Sprite").GetComponent<UISprite>().color = Color.black;
					DownButtonsUI[i].enabled = false;
					//ResourceBtnDc.dynamicMaterial.shader = GrayShader;
				}
				else
				{
					//DownButtonsUI[i].GetComponent<UISprite>().color = Color.white;
					DownButtonsUI[i].transform.Find("Sprite").GetComponent<UISprite>().color = Color.white;
					DownButtonsUI[i].enabled = true;
				}
					//ResourceBtnDc.dynamicMaterial.shader = ResourceBtnDc.baseMaterial.shader;

				/*
				if(ResourceBtnDc==null)
				{
					for(int j=0;j<UIDrawCall.activeList.size;j++)
					{
						UIDrawCall dc = UIDrawCall.activeList[j];
						if(dc.panel==DownButtonsUI[i].GetComponent<UIPanel>())
						{
							ResourceBtnDc = dc;
						}
					}
				}
				*/
				//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
				//btn.Status = Helper.CollectStatus(buildInfo) > 0;
				/*
				if(ResourceBtnDc!=null&&ResourceBtnDc.baseMaterial!=null&&ResourceBtnDc.dynamicMaterial!=null)
				{
					//没有可采集的数量时，显示：灰色;
					if(Helper.getCollectNum(DownButtons[i].buildInfo) == 0)
						ResourceBtnDc.dynamicMaterial.shader = GrayShader;
					else
						ResourceBtnDc.dynamicMaterial.shader = ResourceBtnDc.baseMaterial.shader;
				}
				*/
				/*
				else
				{
					if(ResourceBtnDc!=null&&ResourceBtnDc.baseMaterial!=null&&ResourceBtnDc.dynamicMaterial!=null)
						ResourceBtnDc.dynamicMaterial.shader = ResourceBtnDc.baseMaterial.shader;
				}*/

			}			
		}
	}
	//以上关于底部功能菜单的相关操作;

	//显示建筑移动的箭号;
	public void ShowArrows(bool IsShow)
	{
		BuildArrows.gameObject.SetActive (IsShow);
	}


	//显示虚线框;
	public void ShowXuXianBox(bool IsShow,string text = "")
	{
		XuXianLabel.text = text;
		XuXianBox.gameObject.SetActive (IsShow);
	}

	//显示可采集资源的弹出框;
	//private bool IsPopResourceBox;
	public void ShowResourceBox(bool IsShow,string spriteName,bool isFull = false)
	{
		PopResource.gameObject.SetActive (IsShow);
		//bool isFull = false;   //该值另行传入;
		if(IsShow)
		{
			PopResourceIco.spriteName = spriteName;
			PopResourceIco.MakePixelPerfect();
			PopResourceIco.transform.localScale = Vector3.one*0.5f;
			PopResourceBtn.onClick = new List<EventDelegate>();
			PopResourceBtn.onClick.Add(new EventDelegate(this,"OnClickCollect"));
		}
		if(isFull)
		{
			//PopResourceBtn.onClick = new List<EventDelegate>();
			PopResourceBox.color = Color.red;
			PopResourceArrow.color = Color.red;
		}
		else
		{
			PopResourceBox.color = Color.white;
			PopResourceArrow.color = Color.white;
		}
		//IsPopResourceBox = IsShow;
	}

	void OnClickCollect()
	{
		//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
		//int cStatus = Helper.CollectStatus(buildInfo);
        //Debug.Log(cStatus);
		//Debug.Log(cStatus+" "+gridInfo.buildInfo.name);
		//if(cStatus==1||cStatus==2)
		//{
		CollectHandle.Collect(buildInfo);
		//}
	}


	//显示倒计时;
	private bool IsShowTimeBar;
	public void ShowTimeBar(bool IsShow)
	{
		if(IsShow)
		BindTimeBar ();
		else
		{
			IsShowTimeBar = false;
		}
		TimeBar.gameObject.SetActive (IsShow);
	}
	private void BindTimeBar()
	{
		if(!IsShowTimeBar)
		{
			IsShowTimeBar = true;

			//设置图标;
			if(buildInfo.status==BuildStatus.New||buildInfo.status==BuildStatus.CreateStatue)
			{
				TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
				TimeBarIco.spriteName = "BuidSmallIco";
			}
			else if(buildInfo.status==BuildStatus.Upgrade)
			{
				TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
				TimeBarIco.spriteName = "UpgradeSmallIco";
			}
			else if(buildInfo.status==BuildStatus.Removal)
			{
				TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
				TimeBarIco.spriteName = "RemoveSmallIco";
			}
			else if(buildInfo.status==BuildStatus.Research)
			{
				TimeBarIco.atlas = PartEmitObj.Instance.avatarAtlas;
				CsvInfo csv = CSVManager.GetInstance().csvTable[buildInfo.status_tid_level] as CsvInfo;
				TimeBarIco.spriteName = csv.TID;
			}
			else if(buildInfo.status==BuildStatus.Train)
			{
				TimeBarIco.atlas = PartEmitObj.Instance.avatarAtlas;
				TimeBarIco.spriteName = buildInfo.troops_tid;
			}

		}

		if (buildInfo.end_time > buildInfo.start_time){
			//设置百分比;
			float percent = (Helper.current_time () - buildInfo.start_time)*1f / (buildInfo.end_time - buildInfo.start_time);
			if (percent > 1) percent = 1f;
			TimeBarBar.transform.localScale = new Vector3 (percent,1f,1f);
		}else{
			TimeBarBar.transform.localScale = new Vector3 (1f,1f,1f);
		}
		//设置文字;
		TimeBarLabel.text = Helper.GetFormatTime (buildInfo.end_time - Helper.current_time(),0);

	}


	//以下绑定进度条按钮框;
	public void OnCancelBuild()
	{
		if (buildInfo.status == BuildStatus.New
		    || buildInfo.status == BuildStatus.Upgrade
		    || buildInfo.status == BuildStatus.Research
		    || buildInfo.status == BuildStatus.Train
		    || buildInfo.status == BuildStatus.CreateStatue
		    || buildInfo.status == BuildStatus.Removal)
		{
		//Debug.Log("OnCancelBuild");
			buildInfo.CanceBuildStatus(buildInfo);
		}
	}

	public void OnInstantBuild()
	{
		if (buildInfo.status == BuildStatus.New
		    || buildInfo.status == BuildStatus.Upgrade
		    || buildInfo.status == BuildStatus.Research
		    || buildInfo.status == BuildStatus.Train
		    || buildInfo.status == BuildStatus.CreateStatue
		    || buildInfo.status == BuildStatus.Removal)
		{
			//Debug.Log("OnInstantBuild");
			buildInfo.FinishBuildStatus(buildInfo);
		}
	}

	private bool IsPopInstantPanel;
	private TweenPosition instantTweener;
	public void PopInstantPanel()
	{
		//Debug.Log (buildInfo.Id+":"+"PopInstantPanel "+isPlayBack);
		PopInstant.gameObject.SetActive (true);
		BindPopInstantTimeBar ();
		instantTweener = PopInstant.GetComponent<TweenPosition> ();
		if(!isPlayBack)
		instantTweener.Reset ();
		instantTweener.onFinished = new List<EventDelegate> ();

		instantTweener.PlayForward();
	}
	private void BindPopInstantTimeBar()
	{
		if (!IsPopInstantPanel)
		{
			IsPopInstantPanel = true;
			//设置图标;
			if(buildInfo.status==BuildStatus.New||buildInfo.status==BuildStatus.CreateStatue)
			{
				PopInstantIco.atlas = PartEmitObj.Instance.uiAtlas;
				PopInstantIco.spriteName = "BuidSmallIco";
			}
			else if(buildInfo.status==BuildStatus.Upgrade)
			{
				PopInstantIco.atlas = PartEmitObj.Instance.uiAtlas;
				PopInstantIco.spriteName = "UpgradeSmallIco";
			}
			else if(buildInfo.status==BuildStatus.Removal)
			{
				PopInstantIco.atlas = PartEmitObj.Instance.uiAtlas;
				PopInstantIco.spriteName = "RemoveSmallIco";
			}
			else if(buildInfo.status==BuildStatus.Research)
			{
				PopInstantIco.atlas = PartEmitObj.Instance.avatarAtlas;
				CsvInfo csv = CSVManager.GetInstance().csvTable[buildInfo.status_tid_level] as CsvInfo;
				PopInstantIco.spriteName = csv.TID;
			}
			else if(buildInfo.status==BuildStatus.Train)
			{
				PopInstantIco.atlas = PartEmitObj.Instance.avatarAtlas;
				PopInstantIco.spriteName = buildInfo.troops_tid;
			}

		}

		if (buildInfo.end_time > buildInfo.start_time){
			//设置百分比;
			float percent = (Helper.current_time () - buildInfo.start_time)*1f / (buildInfo.end_time - buildInfo.start_time);
			if (percent > 1) percent = 1f;
			PopInstantBar.transform.localScale = new Vector3 (percent,1f,1f);
		}else{
			PopInstantBar.transform.localScale = new Vector3 (1f,1f,1f);
		}

		//设置文字;
		PopInstantLabel.text = Helper.GetFormatTime (buildInfo.end_time - Helper.current_time(),0);

		PopInstantDiamondLabel.text = CalcHelper.calcTimeToGems (buildInfo.end_time - Helper.current_time()).ToString();


	}
	public void CloseInstant(bool isAnim=true)
	{
		//Debug.Log (buildInfo.Id+":"+"CloseInstant");
		IsPopInstantPanel = false;
		if(isAnim)
		{
			//Debug.Log (buildInfo.Id+":"+"CloseInstantisAnim");
			if(instantTweener!=null)
			{
				//Debug.Log (buildInfo.Id+":"+"CloseInstantisAnim instantTweener!=null");
				instantTweener.PlayReverse ();
				isPlayBack = true;
				instantTweener.onFinished = new List<EventDelegate> ();
				instantTweener.onFinished.Add (new EventDelegate(this,"CloseInstantEnd"));
			}
		}
		else
		{
			//Debug.Log (buildInfo.Id+":"+"CloseInstantisAnim = false");
			isPlayBack = false;
			CloseInstantEnd();
		}

	}
	private void CloseInstantEnd()
	{
		PopInstant.gameObject.SetActive (false);
	}

	public void SetHealthPercent(float percent)
	{
		StopAllCoroutines ();
		if (!HealthBar.gameObject.activeSelf&&!buildInfo.IsDead)
						HealthBar.gameObject.SetActive (true);
		if (percent < 0)
						percent = 0;
		if (percent > 1)
						percent = 1;

		HealthBarSprite.transform.localScale = new Vector3 (percent,1f,1f);
		StartCoroutine (DoHideHealthBar (5f));
	}

	IEnumerator DoHideHealthBar(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		HealthBar.gameObject.SetActive (false);
	}

	public void SetHealthBarVisible(bool tag)
	{
		HealthBar.gameObject.SetActive (tag);
	}
	

	// Update is called once per frame
	void Update () 
	{
		if (DataManager.GetInstance().sceneStatus != SceneStatus.HOME&&DataManager.GetInstance().sceneStatus != SceneStatus.HOMERESOURCE)
						return;
		if (!Globals.IsSceneLoaded)
						return;
		if (time_interval >= 1){											
			time_interval = 0;

			if(IsPopDownButtons)
			{
				BindDownButtons();
			}

			/*
			if(IsPopResourceBox)
			{
				ShowResourceBox(true,"",null);
			}
			*/

			if(IsShowTimeBar)
			{
				BindTimeBar();
			}

			if(IsPopInstantPanel)
			{
				BindPopInstantTimeBar();
			}

			//计算升级，工作进度是否完成;
			if (buildInfo.status == BuildStatus.New
			    || buildInfo.status == BuildStatus.Upgrade
			    || buildInfo.status == BuildStatus.Research		   
			    || buildInfo.status == BuildStatus.CreateStatue
			    || buildInfo.status == BuildStatus.Removal)
			{
				if(buildInfo.end_time<=Helper.current_time())
				{
					if (buildInfo.tid_type == "OBSTACLES"){
						buildInfo.RemovalBuildToServer(buildInfo);
					}else{
						//BuildInfo.FinishBuildToServer(buildInfo);
						//BuildHandle.BuildTimeUp(buildInfo);
					}
					//buildInfo.status = BuildStatus.Normal;
					//PopManage.Instance.RefreshBuildBtn(buildInfo);
					//Debug.Log("新建升级完成");
				}
			}else if (buildInfo.status == BuildStatus.Train){
				//buildInfo.TrainFinishOneToServer(buildInfo);
			}else if(buildInfo.status==BuildStatus.Normal)
			{
				//BuildUIManage uiManage = buildInfo.GetComponent<BuildUIManage>();
				if(DataManager.GetInstance().sceneStatus == SceneStatus.HOME&&!buildInfo.isSelected)
				{
					string res_ico = "";

					if (buildInfo.tid == "TID_BUILDING_HOUSING"){
						res_ico = "goldIco";
					}else if (buildInfo.tid == "TID_BUILDING_WOODCUTTER"){
						res_ico = "woodIco";
					}else if (buildInfo.tid == "TID_BUILDING_STONE_QUARRY"){
						res_ico = "stoneIco";
					}else if (buildInfo.tid == "TID_BUILDING_METAL_MINE"){
						res_ico = "ironIco";
					}else if (buildInfo.tid == "TID_BUILDING_LABORATORY"){
						//研究;
						ShowXuXianBox(true,StringFormat.FormatByTid("TID_BUILDING_TEXT_RESEARCH"));
					}else if (buildInfo.tid == "TID_BUILDING_ARTIFACT_WORKSHOP"){
						//创建,部署;
						if (buildInfo.hasStatue()){
							ShowXuXianBox(true,StringFormat.FormatByTid("TID_BUILDING_TEXT_ALERT"));
						}else{
							ShowXuXianBox(true,StringFormat.FormatByTid("TID_BUILDING_TEXT_INVENT"));
						}
					}else if (buildInfo.tid == "TID_BUILDING_LANDING_SHIP"){
						//"训练";
						//ShowXuXianBox(true,StringFormat.FormatByTid("TID_BUILDING_TEXT_TRAIN"));
					}

					if (res_ico != ""){
						//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
						int cStatus = Helper.CollectStatus(buildInfo);
						if (cStatus == 0){
							ShowResourceBox(false,res_ico,false);
						}else{
							ShowResourceBox(true,res_ico,cStatus > 1);
						}
					}
				}
				else
				{
					ShowXuXianBox(false,"");
					ShowResourceBox(false,"");
				}
			}

			//BindUpDBrand ();
		}else{
			time_interval = time_interval + Time.deltaTime;
		}
	}
}
