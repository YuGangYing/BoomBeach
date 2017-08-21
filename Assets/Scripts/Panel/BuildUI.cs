using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BoomBeach {
    [ExecuteInEditMode]
    public class BuildUI : MonoBehaviour {

        public bool load;
        public Transform NameAndLevel;
        public Text NameLabel;
        public Text LevelLabel;

        public Transform PopDownPanel1;
        public Button Panel1Btn1;

        public Transform PopDownPanel2;
        public Button Panel2Btn1;
        public Button Panel2Btn2;

        public Transform PopDownPanel3;
        public Button Panel3Btn1;
        public Button Panel3Btn2;
        public Button Panel3Btn3;

        public Transform PopInstant;
        public Button PopInstantCancel;
        public Button PopInstantOk;
        public Image PopInstantIco;
        public Slider PopInstantBar;
        public Text PopInstantLabel;
        public Text PopInstantDiamondLabel;

        public Transform PopResource;
        public Image PopResourceArrow;
        public Image PopResourceBox;
        public Image PopResourceIco;
        public Button PopResourceBtn;

        public Transform TimeBar;
        public Slider timeSlider;
        public Image TimeBarIco;
        public Image TimeBarBar;
        public Text TimeBarLabel;

        public Transform PopUpPanel;
        public Button PopUpCancel;
        public Button PopUpOk;

        public Transform XuXianBox;
        public Text XuXianLabel;

        public Transform UpdBrand;
        public Transform HealthBar;
        public Slider healthBarSlider;

        public Transform BuildArrows;
        public Shader GrayShader;
        //public bool DisableCollect;
        public double time_interval = 0f;
        
        public BuildInfo buildInfo;
        public List<BuildButton> DownButtons;
        public List<Button> DownButtonsUI;
        public TweenPosition tweener;
        public BuildBattleUI buildBattleInfo;

        void Awake()
        {
            if (Application.isPlaying)
            {
                //TODO
                //buildBattleInfo.Init();
                buildBattleInfo.gameObject.SetActive(false);
                Panel1Btn1.onClick.AddListener(OnDownBtnClick0);
                Panel2Btn1.onClick.AddListener(OnDownBtnClick0);
                Panel2Btn2.onClick.AddListener(OnDownBtnClick1);
                Panel3Btn1.onClick.AddListener(OnDownBtnClick0);
                Panel3Btn2.onClick.AddListener(OnDownBtnClick1);
                Panel3Btn3.onClick.AddListener(OnDownBtnClick2);
                PopInstantCancel.onClick.AddListener(OnCancelBuild);
                PopInstantOk.onClick.AddListener(OnInstantBuild);
                PopUpCancel.onClick.AddListener(buildInfo.OnCancelCreate);
                PopUpOk.onClick.AddListener(OnOkCreate);
                PopResourceBtn.onClick.AddListener(OnClickCollect);

				LoadSubUIs ();
            }
        }

		void OnOkCreate(){
			BuildHandle.OnCreateBuild (buildInfo);
		}

        //加载所有子UI对象
        void LoadSubUIs()
        {
            NameAndLevel = transform.Find("UI/UIS/NameAndLevel");

            NameLabel = NameAndLevel.Find("Name").GetComponent<Text>();
            LevelLabel = NameAndLevel.Find("Level").GetComponent<Text>();

            PopDownPanel1 = transform.Find("UI/UIS/PopDownPanel1");
            Panel1Btn1 = PopDownPanel1.Find("Btn1").GetComponent<Button>();

            PopDownPanel2 = transform.Find("UI/UIS/PopDownPanel2");
            Panel2Btn1 = PopDownPanel2.Find("Btn1").GetComponent<Button>();
            Panel2Btn2 = PopDownPanel2.Find("Btn2").GetComponent<Button>();

            PopDownPanel3 = transform.Find("UI/UIS/PopDownPanel3");
            Panel3Btn1 = PopDownPanel3.Find("Btn1").GetComponent<Button>();
            Panel3Btn2 = PopDownPanel3.Find("Btn2").GetComponent<Button>();
            Panel3Btn3 = PopDownPanel3.Find("Btn3").GetComponent<Button>();

            PopInstant = transform.Find("UI/UIS/PopInstant");
            PopInstantCancel = PopInstant.Find("Btn1").GetComponent<Button>();
            PopInstantOk = PopInstant.Find("Btn2").GetComponent<Button>();
            PopInstantIco = PopInstant.Find("TimeBar/IcoBg/Sprite").GetComponent<Image>();
            PopInstantBar = PopInstant.Find("TimeBar/Barbg").GetComponent<Slider>();
            PopInstantLabel = PopInstant.Find("TimeBar/Label").GetComponent<Text>();
            PopInstantDiamondLabel = PopInstant.Find("Btn2/Label").GetComponent<Text>();

            PopResource = transform.Find("UI/UIS/PopResource");

            PopResourceArrow = PopResource.Find("arrow").GetComponent<Image>();
            PopResourceBox = PopResource.GetComponent<Image>();
            PopResourceIco = PopResource.Find("Sprite").GetComponent<Image>();
            PopResourceBtn = PopResource.GetComponent<Button>();
            PopResourceBtn.onClick.AddListener(OnClickCollect);

            PopUpPanel = transform.Find("UI/PopUpPanel");

            PopUpCancel = PopUpPanel.Find("CancelBtn").GetComponent<Button>();
            PopUpOk = PopUpPanel.Find("OkBtn").GetComponent<Button>();

            TimeBar = transform.Find("UI/UIS/TimeBar");
            timeSlider = TimeBar.Find("Barbg").GetComponent<Slider>();
            TimeBarIco = TimeBar.Find("IcoBg/Sprite").GetComponent<Image>();
            TimeBarBar = TimeBar.Find("Barbg/bar").GetComponent<Image>();
            TimeBarLabel = TimeBar.Find("Label").GetComponent<Text>();

            HealthBar = transform.Find("UI/UIS/HealthBar");
            //if (HealthBar != null)
                healthBarSlider = HealthBar.Find("Barbg").GetComponent<Slider>();
            XuXianBox = transform.Find("UI/UIS/XuXianBox");
            XuXianLabel = XuXianBox.Find("Label").GetComponent<Text>();
            BuildArrows = transform.Find("UI/Arrows");
            UpdBrand = transform.Find("UpdBrand");

            buildInfo = GetComponent<BuildInfo>();
            GrayShader = Shader.Find("MyShader/GrayShader");
			buildBattleInfo = GetComponentInChildren<BuildBattleUI>(true);
            buildBattleInfo.Init();
        }
	
	    void Update () {
            if (load) {
                LoadSubUIs();
                load = false;
            }
            if (!Application.isPlaying)
                return;
            if (DataManager.GetInstance.sceneStatus != SceneStatus.HOME && DataManager.GetInstance.sceneStatus != SceneStatus.HOMERESOURCE)
                return;
            if (!Globals.IsSceneLoaded)
                return;
			if (mHideHealbarTime <= Time.time && mIsShowHealbar) {
				healthBarSlider.gameObject.SetActive (false);
				mIsShowHealbar = false;
			}
			if (time_interval < Time.time)
            {
				time_interval = Time.time + 1;

                if (IsPopDownButtons)
                {
                    BindDownButtons();
                }

                /*
                if(IsPopResourceBox)
                {
                    ShowResourceBox(true,"",null);
                }
                */

                if (IsShowTimeBar)
                {
                    BindTimeBar();
                }

                if (IsPopInstantPanel)
                {
                    BindPopInstantTimeBar();
                }

				//这段应该buildInfo去计算 TODO
                //计算升级，新建进度是否完成;
                if (buildInfo.status == BuildStatus.New
                    || buildInfo.status == BuildStatus.Upgrade
                    || buildInfo.status == BuildStatus.Research
                    || buildInfo.status == BuildStatus.CreateStatue
                    || buildInfo.status == BuildStatus.Removal)
                {
                    if (buildInfo.end_time <= Helper.current_time())
                    {
                        if (buildInfo.tid_type == "OBSTACLES")
                        {
                            //buildInfo.RemovalBuildToServer(buildInfo);
                        }
                        else
                        {
							//TODO
                            //BuildInfo.FinishBuildToServer(buildInfo);
                        }
                        //buildInfo.status = BuildStatus.Normal;
                        //PopManage.Instance.RefreshBuildBtn(buildInfo);
                        //Debug.Log("新建升级完成");
                    }
                }
                else if (buildInfo.status == BuildStatus.Train)
                {
                    //buildInfo.TrainFinishOneToServer(buildInfo);
					//TrainHandle.OnSpeedUpTrain (buildInfo);
                }
                else if (buildInfo.status == BuildStatus.Normal)
                {
                    //BuildUIManage uiManage = buildInfo.GetComponent<BuildUIManage>();
                    if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME && !buildInfo.isSelected)
                    {
                        string res_ico = "";

                        if (buildInfo.tid == "TID_BUILDING_HOUSING")
                        {
                            res_ico = "goldIco";
                        }
                        else if (buildInfo.tid == "TID_BUILDING_WOODCUTTER")
                        {
                            res_ico = "woodIco";
                        }
                        else if (buildInfo.tid == "TID_BUILDING_STONE_QUARRY")
                        {
                            res_ico = "stoneIco";
                        }
                        else if (buildInfo.tid == "TID_BUILDING_METAL_MINE")
                        {
                            res_ico = "ironIco";
                        }
                        else if (buildInfo.tid == "TID_BUILDING_LABORATORY")
                        {
                            //研究;
                            ShowXuXianBox(true, StringFormat.FormatByTid("TID_BUILDING_TEXT_RESEARCH"));
                        }
                        else if (buildInfo.tid == "TID_BUILDING_ARTIFACT_WORKSHOP")
                        {
                            //创建,部署;
                            if (buildInfo.hasStatue())
                            {
                                ShowXuXianBox(true, StringFormat.FormatByTid("TID_BUILDING_TEXT_ALERT"));
                            }
                            else
                            {
                                ShowXuXianBox(true, StringFormat.FormatByTid("TID_BUILDING_TEXT_INVENT"));
                            }
                        }
                        else if (buildInfo.tid == "TID_BUILDING_LANDING_SHIP")
                        {
                            //"训练";
                            //ShowXuXianBox(true,StringFormat.FormatByTid("TID_BUILDING_TEXT_TRAIN"));
                        }

                        if (res_ico != "")
                        {
                            //0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
                            int cStatus = Helper.CollectStatus(buildInfo);
                            if (cStatus == 0)
                            {
                                ShowResourceBox(false, res_ico, false);
                            }
                            else
                            {
                                ShowResourceBox(true, res_ico, cStatus > 1);
                            }
                        }
                    }
                    else
                    {
                        ShowXuXianBox(false, "");
                        ShowResourceBox(false, "");
                    }
                }

                //BindUpDBrand ();
            }
        }

        public void AfterInit()
        {
            BuildTopUI btu = transform.GetComponentInChildren<BuildTopUI>();
            if (btu != null)
                NameAndLevel.transform.position = btu.position;
            if (btu != null)
                PopResource.transform.position = btu.position;
            if (btu != null)
            {
                PopUpPanel.transform.position = btu.position;
                PopUpPanel.transform.localPosition = new Vector3(PopUpPanel.transform.localPosition.x, PopUpPanel.transform.localPosition.y + 100f, PopUpPanel.transform.localPosition.z);
            }
            if (btu != null)
            {
                TimeBar.transform.position = btu.position;
                TimeBar.transform.localPosition = new Vector3(TimeBar.transform.localPosition.x, TimeBar.transform.localPosition.y + 50f, TimeBar.transform.localPosition.z);
            }

            if (btu != null)
            {
                HealthBar.transform.position = btu.position;
                HealthBar.transform.localPosition = new Vector3(HealthBar.transform.localPosition.x, HealthBar.transform.localPosition.y + 50f, HealthBar.transform.localPosition.z);
            }

            if (btu != null)
                XuXianBox.transform.position = btu.position;
        }

        public void ShowBattleInfo(bool is_show)
        {
            if (DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYVIEW || DataManager.GetInstance.sceneStatus == SceneStatus.FRIENDVIEW)
                buildBattleInfo.gameObject.SetActive(is_show);
            else
                buildBattleInfo.gameObject.SetActive(false);
        }

        //计算是否显示可升级的牌子;
        public void BindUpDBrand(bool is_show)
        {
            if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME)
                UpdBrand.gameObject.SetActive(is_show);
            else
                UpdBrand.gameObject.SetActive(false);
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
            NameAndLevel.gameObject.SetActive(isShow);
        }

        //显示新建的功能按钮;
        public void ShowNewBox(bool isShow)
        {
            PopUpPanel.gameObject.SetActive(isShow);
        }

        //设置新建确认按钮是否可以按下;
        public void SetNewOk(bool status)
        {

            if (status)
            {
                PopUpOk.GetComponent<Image>().color = new Color(0.235f, 0.667f, 0.274f);
                PopUpOk.GetComponent<Button>().enabled = true;
            }
            else
            {
                PopUpOk.GetComponent<Image>().color = Color.white;
                PopUpOk.GetComponent<Button>().enabled = false;
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
            PopDownPanel1.gameObject.SetActive(false);
            PopDownPanel2.gameObject.SetActive(false);
            PopDownPanel3.gameObject.SetActive(false);
        }


        //弹出底部菜单;
        public bool IsPopDownButtons;
        bool isPlayBack;
        public void PopDownButtons(List<BuildButton> buttons)
        {
            ResetPanel();
            DownButtons = buttons;
            //Debug.Log(buttons.Count);
            if (buttons.Count == 1)
            {
                DownButtonsUI = new List<Button>();
                DownButtonsUI.Add(Panel1Btn1);
                PopDownPanel1.gameObject.SetActive(true);
                tweener = PopDownPanel1.GetComponent<TweenPosition>();
                if (!isPlayBack)
                    tweener.Reset();
                tweener.onFinished = new List<EventDelegate>();
                //Debug.Log(buildInfo.Id+":PopDownButtons1");
                tweener.PlayForward();
            }

            if (buttons.Count == 2)
            {
                DownButtonsUI = new List<Button>();
                DownButtonsUI.Add(Panel2Btn1);
                DownButtonsUI.Add(Panel2Btn2);
                PopDownPanel2.gameObject.SetActive(true);
                tweener = PopDownPanel2.GetComponent<TweenPosition>();
                if (!isPlayBack)
                    tweener.Reset();
                tweener.onFinished = new List<EventDelegate>();
                //Debug.Log(buildInfo.Id+":PopDownButtons2");
                tweener.PlayForward();
            }

            if (buttons.Count == 3)
            {
                DownButtonsUI = new List<Button>();
                DownButtonsUI.Add(Panel3Btn1);
                DownButtonsUI.Add(Panel3Btn2);
                DownButtonsUI.Add(Panel3Btn3);
                PopDownPanel3.gameObject.SetActive(true);
                tweener = PopDownPanel3.GetComponent<TweenPosition>();
                if (!isPlayBack)
                    tweener.Reset();
                tweener.onFinished = new List<EventDelegate>();
                //Debug.Log(buildInfo.Id+":PopDownButtons3");
                tweener.PlayForward();
            }
            SetCollectShader();
            tweener.onFinished.Add(new EventDelegate(this, "BindDownButtons"));
        }

        //关闭底部功能按钮;
        public void CloseDownButtons(bool isAnim = true)
        {
            //		Debug.Log(buildInfo.Id+":CloseDownButtons");
            IsPopDownButtons = false;
            //ResourceBtnDc = null;
            if (isAnim)
            {
                //Debug.Log(buildInfo.Id+":CloseDownButtons isAnim");
                if (tweener != null)
                {
                    //Debug.Log(buildInfo.Id+":CloseDownButtons isAnim tweener!=null");

                    isPlayBack = true;
                    tweener.onFinished = new List<EventDelegate>();
                    tweener.onFinished.Add(new EventDelegate(this, "ResetPanel"));
                    tweener.PlayReverse();
                }
            }
            else
            {
                //Debug.Log(buildInfo.Id+":CloseDownButtons !isAnim");
                isPlayBack = false;
                ResetPanel();
            }
        }


        void OnDownBtnClick0()
        {
            OnDownBtnClick(DownButtons[0]);
        }

        void OnDownBtnClick1()
        {
            OnDownBtnClick(DownButtons[1]);
        }

        void OnDownBtnClick2()
        {
            OnDownBtnClick(DownButtons[2]);
        }


        void OnDownBtnClick(BuildButton btn)
        {
            Debug.Log("OnDownBtnClick:" + btn.buildInfo.name + ";type:" + btn.Type.ToString());
            //DownButtons[0].OnClick
            if (btn.Type == BuildButtonType.INFO)
            {
				UIManager.GetInstance.GetController<BuildDetailCtrl>().ShowInfo(btn.buildInfo);
                //PopManage.Instance.ShowBuildInfo(btn.buildInfo);
            }
            else if (btn.Type == BuildButtonType.REMOVE)
            {
                btn.buildInfo.OnRemoval(btn.buildInfo);
            }
            else if (btn.Type == BuildButtonType.UPGRADE)
            {
                //PopManage.Instance.ShowBuildUpgrade(btn.buildInfo);
				UIManager.GetInstance.GetController<BuildDetailCtrl>().ShowUpgrade(btn.buildInfo);
            }
            else if (btn.Type == BuildButtonType.RESEARCH)
            {
                //PopManage.Instance.ShowResearchWin();
				UIManager.GetInstance.GetController<ResearchCtrl>().ShowPanel();
            }
            else if (btn.Type == BuildButtonType.TRAIN)
            {
                Globals.currentTrainBuildInfo = btn.buildInfo;
                //PopManage.Instance.ShowTroopTrainWin();
				UIManager.GetInstance.GetController<TroopTrainCtrl>().ShowTroop();
            }
            else if (btn.Type == BuildButtonType.BUILD)
            {
                //PopManage.Instance.ShowCreateStatueWin(btn.buildInfo);
				UIManager.GetInstance.GetController<ArtifactCtrl>().ShowArtifactPanel(btn.buildInfo);
            }
            else if (btn.Type == BuildButtonType.COLLECT)
            {
				CollectHandle.Collect(btn.buildInfo);
                //0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
            }
        }

        void BindDownButtons()
        {
            //Debug.Log("BindDownButtons");
            for (int i = 0; i < DownButtons.Count; i++)
            {
                Image ico_sprite = DownButtonsUI[i].transform.Find("Image").GetComponent<Image>();

                ico_sprite.sprite= ResourceManager.GetInstance.atlas.commonSpriteDic[DownButtons[i].IcoName];
                //ico_sprite.MakePixelPerfect();
                //ico_sprite.transform.localScale = Vector3.one * 1.2f;
                //DownButtonsUI[i].onClick = DownButtons[i].OnClick;
                //DownButtons[i].Type
                DownButtonsUI[i].onClick.RemoveAllListeners();
                if (i == 0)
                {
                    DownButtonsUI[i].onClick.AddListener(new UnityEngine.Events.UnityAction(OnDownBtnClick0));
                } else if (i == 1)
                {
                    DownButtonsUI[i].onClick.AddListener(new UnityEngine.Events.UnityAction(OnDownBtnClick1));
                }
                else {
                    DownButtonsUI[i].onClick.AddListener(new UnityEngine.Events.UnityAction(OnDownBtnClick2));
                }

               // ResourceBtnDc = null;
            }

            IsPopDownButtons = true;

            SetCollectShader();

        }


        public void SetCollectShader()
        {
            if (DownButtons == null)
                return;
            for (int i = 0; i < DownButtons.Count; i++)
            {
                //&&!buildInfo.isMoving
                if (DownButtons[i].Type == BuildButtonType.COLLECT)
                {
					//Debug.Log (Helper.getCollectNum(DownButtons[i].buildInfo));
                    //没有可采集的数量时，显示：灰色;
                    if (Helper.getCollectNum(DownButtons[i].buildInfo) == 0)
                    {
                        //DownButtonsUI[i].GetComponent<UISprite>().color = Color.black;
                        //DownButtonsUI[i].transform.Find("Image").GetComponent<Image>().color = new Color(0.5f,0.5f, 0.5f,1);

                        Image img = DownButtonsUI[i].transform.Find("Image").GetComponent<Image>();
                        img.material = ResourceCache.Load("Materials/GrayUGUI") as Material;
                        DownButtonsUI[i].enabled = false;
                        //ResourceBtnDc.dynamicMaterial.shader = GrayShader;
                    }
                    else
                    {
                        //DownButtonsUI[i].GetComponent<UISprite>().color = Color.white;
                        //DownButtonsUI[i].transform.Find("Image").GetComponent<Image>().color = Color.white;
                        Image img = DownButtonsUI[i].transform.Find("Image").GetComponent<Image>();
                        img.color = Color.white;
                        img.material = null;
                        DownButtonsUI[i].enabled = true;
                    }
                }
            }
        }
        //以上关于底部功能菜单的相关操作;

        //显示建筑移动的箭号;
        public void ShowArrows(bool IsShow)
        {
            BuildArrows.gameObject.SetActive(IsShow);
        }

        //显示虚线框;
        public void ShowXuXianBox(bool IsShow, string text = "")
        {
            XuXianLabel.text = text;
            XuXianBox.gameObject.SetActive(IsShow);
        }

        //显示可采集资源的弹出框;
        //private bool IsPopResourceBox;
        public void ShowResourceBox(bool IsShow, string spriteName, bool isFull = false)
        {
            PopResource.gameObject.SetActive(IsShow);
            //bool isFull = false;   //该值另行传入;
            if (IsShow)
            {
                PopResourceIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic[spriteName];
                //PopResourceIco.MakePixelPerfect();
                //PopResourceIco.transform.localScale = Vector3.one * 0.5f;
                PopResourceBtn.onClick.RemoveAllListeners();
                PopResourceBtn.onClick.AddListener(OnClickCollect);
            }
            if (isFull)
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
            if (IsShow)
                BindTimeBar();
            else
            {
                IsShowTimeBar = false;
            }
            TimeBar.gameObject.SetActive(IsShow);
        }
        private void BindTimeBar()
        {
            if (!IsShowTimeBar)
            {
                IsShowTimeBar = true;

                //设置图标;
                if (buildInfo.status == BuildStatus.New || buildInfo.status == BuildStatus.CreateStatue)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
                    TimeBarIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic["BuidSmallIco"];
                }
                else if (buildInfo.status == BuildStatus.Upgrade)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
                    TimeBarIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic["UpgradeSmallIco"];
                }
                else if (buildInfo.status == BuildStatus.Removal)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
                    TimeBarIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic["RemoveSmallIco"];
                }
                else if (buildInfo.status == BuildStatus.Research)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.avatarAtlas;
                    CsvInfo csv = CSVManager.GetInstance.csvTable[buildInfo.status_tid_level] as CsvInfo;
					TimeBarIco.sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic[csv.TID];
                }
                else if (buildInfo.status == BuildStatus.Train)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.avatarAtlas;
					TimeBarIco.sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic[buildInfo.troops_tid];
                }
            }
            if (buildInfo.end_time > buildInfo.start_time)
            {
                //设置百分比;
                float percent = (Helper.current_time() - buildInfo.start_time) * 1f / (buildInfo.end_time - buildInfo.start_time);
                if (percent > 1) percent = 1f;
                timeSlider.value = percent;
                //TimeBarBar.transform.localScale = new Vector3(percent, 1f, 1f);
            }
            else
            {
                timeSlider.value = 1;
                //TimeBarBar.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            //设置文字;
            TimeBarLabel.text = Helper.GetFormatTime(buildInfo.end_time - Helper.current_time(), 0);
        }



        //以下绑定进度条按钮框;
        public void OnCancelBuild()
        {
			Debug.Log("OnCancelBuild");
			if (buildInfo.status == BuildStatus.New) {
				BuildHandle.OnCancelCreateBuild (buildInfo);
				return;
			}
			if(buildInfo.status == BuildStatus.Upgrade){
				BuildHandle.OnCancelUpgradeBuild(buildInfo);
				return;
			}
			if(buildInfo.status == BuildStatus.Train){
				TrainHandle.OnCancelTrain (buildInfo);
				return;
			}
			if(buildInfo.status == BuildStatus.Research){
				ResearchHandle.OnCancelResearch (buildInfo);
			}

            if (buildInfo.status == BuildStatus.New
                || buildInfo.status == BuildStatus.Upgrade
                || buildInfo.status == BuildStatus.Research
                || buildInfo.status == BuildStatus.Train
                || buildInfo.status == BuildStatus.CreateStatue
                || buildInfo.status == BuildStatus.Removal)
            {
                //Debug.Log("OnCancelBuild");
				//BuildHandle.OnCancelCreateBuild (buildInfo);
                //buildInfo.CanceBuildStatus(buildInfo);
            }
        }

        public void OnInstantBuild()
        {
			if(buildInfo.status == BuildStatus.New || buildInfo.status == BuildStatus.Upgrade){
				BuildHandle.OnSpeedUpUpgradeBuild (buildInfo);
				return;
			}
			if(buildInfo.status == BuildStatus.Train){
				TrainHandle.OnSpeedUpTrain (buildInfo);
				return;
			}
			if(buildInfo.status == BuildStatus.Research){
				ResearchHandle.OnSpeedUpResearch (buildInfo);
				return;
			}


            if (
				//buildInfo.status == BuildStatus.New
                //|| buildInfo.status == BuildStatus.Upgrade
                //|| 
				buildInfo.status == BuildStatus.Research
                //|| buildInfo.status == BuildStatus.Train
                || buildInfo.status == BuildStatus.CreateStatue
                || buildInfo.status == BuildStatus.Removal)
            {
                Debug.Log("OnInstantBuild");
                buildInfo.FinishBuildStatus(buildInfo);
            }
        }

        private bool IsPopInstantPanel;
        private TweenPosition instantTweener;
        public void PopInstantPanel()
        {
            //Debug.Log (buildInfo.Id+":"+"PopInstantPanel "+isPlayBack);
            PopInstant.gameObject.SetActive(true);
            BindPopInstantTimeBar();
            instantTweener = PopInstant.GetComponent<TweenPosition>();
            if (!isPlayBack)
                instantTweener.Reset();
            instantTweener.onFinished = new List<EventDelegate>();

            instantTweener.PlayForward();
        }
        private void BindPopInstantTimeBar()
        {
            if (!IsPopInstantPanel)
            {
                IsPopInstantPanel = true;
                if (buildInfo.status == BuildStatus.New || buildInfo.status == BuildStatus.CreateStatue)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
					PopInstantIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic["BuidSmallIco"];
                }
                else if (buildInfo.status == BuildStatus.Upgrade)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
					PopInstantIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic["UpgradeSmallIco"];
                }
                else if (buildInfo.status == BuildStatus.Removal)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.uiAtlas;
					PopInstantIco.sprite = ResourceManager.GetInstance.atlas.commonSpriteDic["RemoveSmallIco"];
                }
                else if (buildInfo.status == BuildStatus.Research)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.avatarAtlas;
                    CsvInfo csv = CSVManager.GetInstance.csvTable[buildInfo.status_tid_level] as CsvInfo;
					PopInstantIco.sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic[csv.TID];
                }
                else if (buildInfo.status == BuildStatus.Train)
                {
                    //TimeBarIco.atlas = PartEmitObj.Instance.avatarAtlas;
					PopInstantIco.sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic[buildInfo.troops_tid];
                }

            }

            if (buildInfo.end_time > buildInfo.start_time)
            {
                //设置百分比;
                float percent = (Helper.current_time() - buildInfo.start_time) * 1f / (buildInfo.end_time - buildInfo.start_time);
                if (percent > 1) percent = 1f;
                //PopInstantBar.transform.localScale = new Vector3(percent, 1f, 1f);
                PopInstantBar.value= percent;
            }
            else
            {
                //PopInstantBar.transform.localScale = new Vector3(1f, 1f, 1f);
                PopInstantBar.value = 1;
            }

            //设置文字;
            PopInstantLabel.text = Helper.GetFormatTime(buildInfo.end_time - Helper.current_time(), 0);

            PopInstantDiamondLabel.text = CalcHelper.calcTimeToGems(buildInfo.end_time - Helper.current_time()).ToString();


        }
        public void CloseInstant(bool isAnim = true)
        {
            //Debug.Log (buildInfo.Id+":"+"CloseInstant");
            IsPopInstantPanel = false;
            if (isAnim)
            {
                //Debug.Log (buildInfo.Id+":"+"CloseInstantisAnim");
                if (instantTweener != null)
                {
                    //Debug.Log (buildInfo.Id+":"+"CloseInstantisAnim instantTweener!=null");
                    instantTweener.PlayReverse();
                    isPlayBack = true;
                    instantTweener.onFinished = new List<EventDelegate>();
                    instantTweener.onFinished.Add(new EventDelegate(this, "CloseInstantEnd"));
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
            PopInstant.gameObject.SetActive(false);
        }

		float mHideHealbarTime = 0;
		bool mIsShowHealbar = false;
        public void SetHealthPercent(float percent)
        {
            //StopAllCoroutines();
            if (!HealthBar.gameObject.activeSelf && !buildInfo.IsDead)
                HealthBar.gameObject.SetActive(true);
            if (percent < 0)
                percent = 0;
            if (percent > 1)
                percent = 1;
			healthBarSlider.value = percent;
           // HealthBarSprite.transform.localScale = new Vector3(percent, 1f, 1f);
			mHideHealbarTime = Time.time + 5f;
			mIsShowHealbar = true;
            //StartCoroutine(DoHideHealthBar(5f));
        }
		/*
        IEnumerator DoHideHealthBar(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            HealthBar.gameObject.SetActive(false);
        }
		*/
        public void SetHealthBarVisible(bool tag)
        {
            HealthBar.gameObject.SetActive(tag);
        }

        public void RefreshBuildBtn()
        {
            if (buildInfo.isSelected)
            {
                //CsvInfo csvInfo = CSVManager.GetInstance.csvTable[buildInfo.tid_level] as CsvInfo;
                //设置名称;
                SetName(buildInfo.ShowName);// StringFormat.FormatByTid(csvInfo.TID));
                SetLevel(buildInfo.ShowLevelName);// StringFormat.FormatByTid("TID_LEVEL_NUM",new string[]{buildInfo.level.ToString()}));
                if (buildInfo.status == BuildStatus.Create)
                {
                    if (buildInfo.CheckBuildPlaceAble())
                        SetNewOk(true);
                    else
                        SetNewOk(false);
                    ShowNameAndTitle(false);
                    ShowNewBox(true);
                    ShowBattleInfo(false);
                }
                else
                {
                    ShowNameAndTitle(true);
                    ShowNewBox(false);
                    ShowBattleInfo(true);
                }

                if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME || DataManager.GetInstance.sceneStatus == SceneStatus.HOMERESOURCE)
                {
                    if (buildInfo.status == BuildStatus.New
                        || buildInfo.status == BuildStatus.Upgrade
                        || buildInfo.status == BuildStatus.Research
                        || buildInfo.status == BuildStatus.Train
                        || buildInfo.status == BuildStatus.CreateStatue
                        || buildInfo.status == BuildStatus.Removal)
                    {
                        if (buildInfo.end_time - buildInfo.start_time > 0)
                        {
                            ShowTimeBar(false);
                            PopInstantPanel();
                            CloseDownButtons(true);
                        }
                    }
                }
                if (buildInfo.status == BuildStatus.Normal)
                {
                    CloseInstant();
                    List<BuildButton> buttons = new List<BuildButton>();
                    if (buildInfo.tid_type == "OBSTACLES")
                    {
                        //1 移除;
                        buttons.Add(addBuildButton(BuildButtonType.REMOVE, buildInfo));
                    }
                    else if (buildInfo.tid_type == "TRAPS")
                    {
                        //1 信息;
                        buttons.Add(addBuildButton(BuildButtonType.INFO, buildInfo));
                    }
                    else if (buildInfo.tid_type == "ARTIFACTS")
                    {
                        //2 信息, 移除;
                        buttons.Add(addBuildButton(BuildButtonType.INFO, buildInfo));
                        if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME)
                            buttons.Add(addBuildButton(BuildButtonType.REMOVE, buildInfo));

                    }
                    else if (buildInfo.tid_type == "BUILDING")
                    {
                        if (buildInfo.csvInfo.BuildingClass != "Artifact")
                        {
                            buttons.Add(addBuildButton(BuildButtonType.INFO, buildInfo));

                            if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME)
                            {
                                if (Helper.checkHasNextLevel(buildInfo))
                                {
                                    buttons.Add(addBuildButton(BuildButtonType.UPGRADE, buildInfo));
                                }
                                //3 信息, 升级， 采集;
                                if (buildInfo.tid == "TID_BUILDING_HOUSING" || buildInfo.tid == "TID_BUILDING_STONE_QUARRY"
                                    || buildInfo.tid == "TID_BUILDING_METAL_MINE" || buildInfo.tid == "TID_BUILDING_WOODCUTTER")
                                {
                                    buttons.Add(addBuildButton(BuildButtonType.COLLECT, buildInfo));

                                }
                                else if (buildInfo.tid == "TID_BUILDING_LABORATORY")
                                {
                                    //3 信息, 升级， 研究(兵，武器,地雷 升级);
                                    buttons.Add(addBuildButton(BuildButtonType.RESEARCH, buildInfo));
                                }
                                else if (buildInfo.tid == "TID_BUILDING_ARTIFACT_WORKSHOP")
                                {
                                    //3 信息, 升级， 雕像（create,deploy,reclaim）;
                                    buttons.Add(addBuildButton(BuildButtonType.BUILD, buildInfo));
                                }
                                else if (buildInfo.tid == "TID_BUILDING_LANDING_SHIP")
                                {
                                    //3 信息, 升级， 训练(生产兵);
                                    buttons.Add(addBuildButton(BuildButtonType.TRAIN, buildInfo));
                                }
                            }
                        }
                        else
                        {
                            //2 信息, 移除;
                            buttons.Add(addBuildButton(BuildButtonType.INFO, buildInfo));
                            if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME)
                                buttons.Add(addBuildButton(BuildButtonType.REMOVE, buildInfo));
                        }
                    }

                    if (DataManager.GetInstance.sceneStatus != SceneStatus.ENEMYVIEW && DataManager.GetInstance.sceneStatus != SceneStatus.ENEMYBATTLE && DataManager.GetInstance.sceneStatus != SceneStatus.BATTLEREPLAY && DataManager.GetInstance.sceneStatus != SceneStatus.FRIENDVIEW)
                    {
                        if (buildInfo.buildUIManage != null)
                            buildInfo.buildUIManage.PopDownButtons(buttons);
                        if (buildInfo.buildUI != null)
                            buildInfo.buildUI.PopDownButtons(buttons);
                    }

                }
            }
            else
            {
                if (buildInfo.status != BuildStatus.Create)
                {

                    ShowNameAndTitle(false);
                    ShowBattleInfo(false);

                    if (buildInfo.status == BuildStatus.Normal)
                    {

                        CloseDownButtons(!buildInfo.isAfterResetPos);
                        ShowTimeBar(false);
                    }

                    if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME || DataManager.GetInstance.sceneStatus == SceneStatus.HOMERESOURCE)
                    {
                        if (buildInfo.status == BuildStatus.New
                           || buildInfo.status == BuildStatus.Upgrade
                           || buildInfo.status == BuildStatus.Research
                           || buildInfo.status == BuildStatus.Train
                           || buildInfo.status == BuildStatus.CreateStatue
                           || buildInfo.status == BuildStatus.Removal)
                        {
                            CloseInstant(!buildInfo.isAfterResetPos);

                            if (buildInfo.end_time > Helper.current_time())
                                ShowTimeBar(true);
                        }
                    }

                }
            }
        }

        public BuildButton addBuildButton(BuildButtonType btnType, BuildInfo buildInfo)
        {
            BuildButton btn = new BuildButton();
            btn.buildInfo = buildInfo;
            //btn.OnClick = new List<EventDelegate>();
            btn.Type = btnType;
            btn.Status = true;

            //信息, 移除，采集，研究(兵，武器,地雷 升级)，雕像（create,deploy,reclaim），训练(生产兵);
            if (btn.Type == BuildButtonType.INFO)
            {
                btn.IcoName = "BuildInfoIco";
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }
            else if (btn.Type == BuildButtonType.REMOVE)
            {
                btn.IcoName = "BuildRemoveIco";
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }
            else if (btn.Type == BuildButtonType.UPGRADE)
            {
                btn.IcoName = "BuildUpgradeIco";
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }
            else if (btn.Type == BuildButtonType.RESEARCH)
            {
                btn.IcoName = "BuildResearchIco";
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }
            else if (btn.Type == BuildButtonType.TRAIN)
            {
                btn.IcoName = "BuildTrainIco";
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }
            else if (btn.Type == BuildButtonType.BUILD)
            {
                btn.IcoName = "BuildBuildico";
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }
            else if (btn.Type == BuildButtonType.COLLECT)
            {

                //0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
                btn.Status = Helper.CollectStatus(buildInfo) > 0;
                if (buildInfo.tid == "TID_BUILDING_HOUSING")
                {
                    btn.IcoName = "goldIco";
                }
                else if (buildInfo.tid == "TID_BUILDING_STONE_QUARRY")
                {
                    btn.IcoName = "stoneIco";
                }
                else if (buildInfo.tid == "TID_BUILDING_METAL_MINE")
                {
                    btn.IcoName = "ironIco";
                }
                else if (buildInfo.tid == "TID_BUILDING_WOODCUTTER")
                {
                    btn.IcoName = "woodIco";
                }
                //btn.OnClick.Add(new EventDelegate(this,"infotest"));
            }

            //Debug.Log("dxxxx");
            return btn;
        }


    }
}
