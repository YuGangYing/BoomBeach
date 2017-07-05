#if false
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities.Data;
using System.Net;
using BoomBeach;
//using System.Security.Cryptography;

public class LoginRegisterManager : MonoBehaviour {
	public bool debug = false;
	
	//public HplBaseLoading baseLoading;
	public GameObject login_btn_pp,login_btn_91;
	
	private SmartFox smartFox;
	
	// Network info
	//private string ip = "g.fanwesoft.com";
	//private string ip = "127.0.0.1";
	//private string ip = "112.124.32.200";
	private int port = 9933;
	private string zone = "Beach";
	//private string zone = "GameTest";
	
	private Transform thisTransform;
	
	// Error display label
	//private UILabel errorMessage;
	
	// Login box and register box
	private GameObject loginBox,registerBox;
	
	// Login box account input and password input.
	private UIInput loginBoxAccountInput,loginBoxPasswordInput;
	
	// Register box account,password and confirm password.
	private Transform registerBoxAccount,registerBoxPassword,RegisterButtonTransform,LoginButtonTransform,TrialButtonTransform,RegisterButtonTransform_New;
	
	private UIInput registerBoxAccountInput,registerBoxPasswordInput;
	

	private UISprite RegisterButtonBg,LoginButtonBg,TrialButtonBg,RegisterButtonBg_NEW;
	//private UIButton RegisterButton,LoginButton,TrialButton;
	//private BoxCollider RegisterButton,LoginButton,TrialButton,RegisterButton_NEW;
	private GameObject runSprite;
	private RotateAnimation new_rotate;
	private UILabel MsgTimeLabel;
	private int lock_attack_time = 0;
	// Only number and letter can input
	private Regex numberLetterRegex = new Regex(@"[A-Za-z0-9]");
	private float timeLastSending = 0.0f;
	private float sendingPeriod = 30f; 
	private bool is_first = true;
	
	private bool isLogining = false;
	
	private UIAtlas hplUINormal,hplUIGray;
	private string UpgradeUrl;
	
	private AsyncOperation async;
	private bool isLoad = false;
	private bool isShowLoginBox=true;
	private bool not_used=true;
	
	private UITexture LoginBackground;
	private UISlider loadingSlider;
	private UILabel loadingStep;
	private GameObject load_box;
	private UILabel hint_lable;


	public TextAsset  achievements;
	public TextAsset  artifact_bonuses;
	public TextAsset  artifacts;
	public TextAsset  buildings;
	
	public TextAsset  characters;
	public TextAsset  decos;
	public TextAsset  liberated_income;
	public TextAsset  npcs;
	
	public TextAsset  obstacles;
	public TextAsset  projectiles;
	public TextAsset  regions;
	public TextAsset  spells;
	
	public TextAsset  townhall_levels;
	public TextAsset  traps;
	public TextAsset  experience_levels;

	public TextAsset island_grid;

	public TextAsset globals;

	private float time_interval = 5;


	private GameObject ErrorBox;
	private UILabel error_title, error_desc, error_btn_name;
	private UIButton error_btn;
	private EventDelegate.Callback error_callback;

	void Awake() {
		//Debug.Log("HplLoginRegisterManager:Awake");
		is_first = true;

		if (PlayerPrefs.HasKey("SoundEffectSwitch")){
			PlayerPrefs.SetInt("SoundEffectSwitch",1);
		}

		if (PlayerPrefs.HasKey("MusicSwitch")){
			PlayerPrefs.SetInt("MusicSwitch",1);
		}

		AudioPlayer.Init();
		intcsv();


		Application.runInBackground = true;
        /*
		// In a webplayer (or editor in webplayer mode) we need to setup security policy negotiation with the server first
		if (Application.isWebPlayer || Application.isEditor) {
			if (!Security.PrefetchSocketPolicy(serverName, serverPort, 500)) {
				Debug.LogError("Security Exception. Policy file load failed!");
			}
		}		
		*/



        #region 原Demo

        //String lng = "Chinese";
        //if (PlayerPrefs.HasKey("currentLanguage")){
        //	lng = PlayerPrefs.GetString("currentLanguage");
        //	bool has_lng = false;
        //	for (int i = 0; i < Localization.instance.languages.Length; ++i)
        //	{
        //		TextAsset asset = Localization.instance.languages[i];				
        //		if (asset != null && asset.name == lng)
        //		{
        //			has_lng = true;
        //		}
        //	}

        //	if (has_lng == false){
        //		lng = "Chinese";
        //	}
        //	//Localization.instance.
        //}
        //Localization.instance.currentLanguage = lng;

        #endregion

        LocalizationCustom.instance.Init();
        String lng = "Chinese";
        if (PlayerPrefs.HasKey("currentLanguage"))
        {
            lng = PlayerPrefs.GetString("currentLanguage");
            bool has_lng = false;
            for (int i = 0; i < LocalizationCustom.instance.languages.Length; ++i)
            {
                TextAsset asset = LocalizationCustom.instance.languages[i];
                if (asset != null && asset.name == lng)
                {
                    has_lng = true;
                }
            }

            if (has_lng == false)
            {
                lng = "Chinese";
            }
            //Localization.instance.
        }

        LocalizationCustom.instance.currentLanguage = lng;


		// Get some reference
		thisTransform = transform;
		//errorMessage = thisTransform.Find("ErrorMessage").GetComponent<UILabel>();
		loginBox = thisTransform.Find("LoginBox").gameObject;
		registerBox = thisTransform.Find("RegisterBox").gameObject;
		runSprite =thisTransform.Find("Sprite (run)").gameObject;
		new_rotate=runSprite.GetComponent<RotateAnimation>();
		
		RegisterButtonTransform=loginBox.transform.Find("Content/RegisterButton");
		if (RegisterButtonTransform == null){
			Debug.Log("RegisterButtonTransform IS NULL");
		}
		//RegisterButton = RegisterButtonTransform.GetComponent<BoxCollider>();
		RegisterButtonBg = RegisterButtonTransform.GetComponent<UISprite>();
		//RegisterButtonLable = RegisterButtonTransform.Find("Label").GetComponent<UILabel>();
		//RegisterButtonLable.text= LocalizationCustom.instance.Get("REG_QUICK");
		
		LoginButtonTransform = loginBox.transform.Find("Content/LoginButton");
		//LoginButton = LoginButtonTransform.GetComponent<BoxCollider>();
		LoginButtonBg = LoginButtonTransform.GetComponent<UISprite>();
		//LoginButtonLable = LoginButtonTransform.Find("Label").GetComponent<UILabel>();
		//LoginButtonLable.text =  LocalizationCustom.instance.Get("LOAD_BUTTON");
		
		TrialButtonTransform = loginBox.transform.Find("Content/TrialButton");
		//TrialButton = TrialButtonTransform.GetComponent<BoxCollider>();
		TrialButtonBg = TrialButtonTransform.GetComponent<UISprite>();
		//TrialButtonLable = TrialButtonTransform.Find("Label").GetComponent<UILabel>();
		//TrialButtonLable.text = LocalizationCustom.instance.Get("PLAY_TRY");
		
		RegisterButtonTransform_New=registerBox.transform.Find("Content/RegisterButton");
		//RegisterButton_NEW = RegisterButtonTransform_New.GetComponent<BoxCollider>();
		RegisterButtonBg_NEW = RegisterButtonTransform_New.GetComponent<UISprite>();
		//RegisterButtonLable_NEW = RegisterButtonTransform_New.Find("Label").GetComponent<UILabel>();
		//RegisterButtonLable_NEW.text = LocalizationCustom.instance.Get("REG_CONFIRM");
		
		loginBoxAccountInput = loginBox.transform.Find("Content/AccountInput").GetComponent<UIInput>();
		loginBoxPasswordInput = loginBox.transform.Find("Content/PasswordInput").GetComponent<UIInput>();

		registerBoxAccount = registerBox.transform.Find("Content/Account");
		registerBoxAccountInput = registerBoxAccount.Find("Input").GetComponent<UIInput>();

		registerBoxPassword = registerBox.transform.Find("Content/Password");
		registerBoxPasswordInput = registerBoxPassword.Find("Input").GetComponent<UIInput>();


		string user_name = PlayerPrefs.GetString("user_name");
		if (user_name != null && !"".Equals(user_name)){
			loginBoxAccountInput.text = user_name;//PlayerPrefs.GetString("user_name",LocalizationCustom.instance.Get("TID_ACCOUNT").Replace(":",""));
			string user_pwd = PlayerPrefs.GetString("user_pwd");
			if (user_pwd != null && !"".Equals(user_pwd))
				loginBoxPasswordInput.text = user_pwd;
		}

		ErrorBox = thisTransform.Find("ErrorBox").gameObject;
		ErrorBox.SetActive(false);

		error_title = ErrorBox.transform.Find("WinTop/Title").GetComponent<UILabel>(); 
		error_desc = ErrorBox.transform.Find("Content/BoxDialog/MsgDialog").GetComponent<UILabel>();
		error_btn_name  = ErrorBox.transform.Find("Content/BoxDialog/ConfirmBtn/BtnTitle").GetComponent<UILabel>();
		error_btn  = ErrorBox.transform.Find("Content/BoxDialog/ConfirmBtn/BtnBg").GetComponent<UIButton>();
		error_btn.onClick = new List<EventDelegate>();
		error_btn.onClick.Add(new EventDelegate(this,"OnAlertClick"));

		//if(Application.platform == RuntimePlatform.IPhonePlayer){
		// Check upgrade
		//HplIOSCall.CheckUpgrade();
		//}
		//else{
		ConnectToServer(string.Empty);
		//}
		
		
		
		//hplUINormal = Resources.Load("Altas/HplUI-Ipad",typeof(UIAtlas)) as UIAtlas;
		//hplUIGray = Resources.Load("Altas/HplUI-Ipad-Gray",typeof(UIAtlas)) as UIAtlas;
		
		
		
	}
	
	void OnDestroy() {
		//print("Script was destroyed");
		//LoginBackground.mainTexture = null;
		//Resources.UnloadUnusedAssets();
	}


	private void intcsv(){
		Helper.initNameToTid();
		Globals.csvTable.Clear();
		
		/*
		loadcsv(achievements,false,true);
		loadcsv(liberated_income,false,true);
		loadcsv(npcs,false,true);
		loadcsv(projectiles,false,true);
		loadcsv(regions,false,true);
*/

		Helper.loadIsLandCsv(island_grid);

		Helper.loadcsv(experience_levels,Globals.ExperienceLevelsList,"ExperienceLevels", false,false);
		/*
		Debug.Log(Globals.ExperienceLevelsList.Count);

		foreach(string key in Globals.ExperienceLevelsList.Keys){
			Debug.Log("level:" + key);
		}

		foreach(ExperienceLevels el in Globals.ExperienceLevelsList.Values){
			Debug.Log(el.Name + ":" + el.ExpPoints);
		}

	*/

		Helper.loadcsv(regions,Globals.RegionsList,"REGIONS", false,false);

        Helper.loadcsv(buildings,Globals.csvTable,"BUILDING", true,false);
		//Debug.Log(Globals.csvTable.Count);
		Helper.loadcsv(townhall_levels,Globals.csvTable,"BUILDING",true,false);
		//Debug.Log(Globals.csvTable.Count);
		
		Helper.loadcsv(characters,Globals.csvTable,"CHARACTERS",true,false);
		//Debug.Log(Globals.csvTable.Count);
		Helper.loadcsv(decos,Globals.csvTable,"DECOS",true,false);
		//Debug.Log(Globals.csvTable.Count);
		Helper.loadcsv(obstacles,Globals.csvTable,"OBSTACLES",true,false);
		//Debug.Log(Globals.csvTable.Count);
		Helper.loadcsv(spells,Globals.csvTable,"SPELLS",true,false);
		//Debug.Log(Globals.csvTable.Count);
		Helper.loadcsv(traps,Globals.csvTable,"TRAPS",true,false);
		//Debug.Log(Globals.csvTable.Count);
		
		//artifacts与buildings中的3个Artifact需要注意区分与关联;
		Helper.loadcsv(artifacts,Globals.csvTable,"ARTIFACTS",true,false);//没有TID，这个需要补TID，把name转为tid



		//Debug.Log(Globals.csvTable.Count);
		Helper.loadcsv(artifact_bonuses,Globals.csvTable,"ARTIFACT_BONUSES",true,false);
		//Debug.Log(Globals.csvTable.Count);
		
		Globals.projectileData.Clear();
		Hashtable ProjectilesList = new Hashtable();
		Helper.loadcsv(projectiles,ProjectilesList,"PROJECTILES",true,false);
		foreach(string key in ProjectilesList.Keys){
			Projectiles val = ProjectilesList[key] as Projectiles;
			//Debug.Log("key:" + key + ";val.name:" + val.Name + ";val.Speed:" + val.Speed);
			Globals.projectileData.Add(key,val);
		}		


		Globals.GlobalsCsv.Clear();
		Hashtable GlobalsCsvList = new Hashtable();
		Helper.loadcsv(globals,GlobalsCsvList,"GLOBALS",true,false);
		foreach(string key in GlobalsCsvList.Keys){
			GlobalsItem val = GlobalsCsvList[key] as GlobalsItem;
			//Debug.Log("key:" + key + ";val.TextValue:" + val.TextValue + ";val.NumberValue:" + val.NumberValue);
			Globals.GlobalsCsv.Add(key,val);
		}
		//Debug.Log("TOWN_HALL_DAMAGE_FROM_OTHER_BUILDINGS_PERCENT:" + Globals.GlobalsCsv["TOWN_HALL_DAMAGE_FROM_OTHER_BUILDINGS_PERCENT"].NumberValue);
		//Debug.Log("TOWN_HALL_MAX_DAMAGE_FROM_ONE_BUILDING_PERCENT:" + Globals.GlobalsCsv["TOWN_HALL_MAX_DAMAGE_FROM_ONE_BUILDING_PERCENT"].NumberValue);
	}

	public string md5(string str)
    {		
		
		String md_str = FanweMD5.MDString(str);
		return md_str.Replace("-","").ToLower();
		/*
        MD5 m = new MD5CryptoServiceProvider();
        byte[] s = m.ComputeHash(UnicodeEncoding.UTF8.GetBytes(str));
        String md_str = BitConverter.ToString(s);		
		return md_str.Replace("-","").ToLower();
		*/
    }
	
	/************
     * Unity callback methods
     ************/
	void FixedUpdate() {
        
		smartFox.ProcessEvents();
		//return;
		
		if(timeLastSending >= sendingPeriod){
			timeLastSending = 0;
			//Debug.Log1("smartFox.Send(new PingPongRequest())");
			smartFox.Send(new PingPongRequest());
		}
			//Time.fixedDeltaTime
		timeLastSending += Time.deltaTime;
	}
	


	private IEnumerator DelayLoadScene () {
		yield return new WaitForSeconds(0.5f);
		
		async.allowSceneActivation = true;
	}
	
	//对方结束攻击;
	private void OnExtensionResponse(BaseEvent evt) {		

	}	
	

	
	void Start(){
		//Debug.Log("HplLoginRegisterManager.Start");

		
		if (is_first == false){
			OnReConnect();
		}
		
		is_first = false;
	}

	
	private void runStatus(string type,bool is_load){
		return;

		if(type=="load"){
			if(is_load){
				runSprite.SetActive(true);
				new_rotate.StartRotationAnimation();
			
				loginBoxAccountInput.enabled=false;
				loginBoxPasswordInput.enabled=false;
				
				//RegisterButton.enabled=false;
				//LoginButton.enabled=false;
				//TrialButton.enabled=false;
				
				
				//RegisterButtonBg.atlas = hplUIGray;
				LoginButtonBg.atlas = hplUIGray;
				TrialButtonBg.atlas = hplUIGray;
				
			}else{
				runSprite.SetActive(false);
				new_rotate.StopRotationAnimation();
				loginBoxAccountInput.enabled=true;
				loginBoxPasswordInput.enabled=true;
				//RegisterButton.enabled=true;
				//LoginButton.enabled=true;
				//TrialButton.enabled=true;
				
				//RegisterButtonBg.atlas = hplUINormal;
				LoginButtonBg.atlas = hplUINormal;
				TrialButtonBg.atlas = hplUINormal;
				
				
			}
		}else if(type=="reg"){
			
			if(is_load){
				runSprite.SetActive(true);
				new_rotate.StartRotationAnimation();
				registerBoxAccountInput.enabled=false;
				registerBoxPasswordInput.enabled=false;
				//registerBoxConfirmPasswordInput.enabled=false;
				//RegisterButton_NEW.enabled=false;
				
				//RegisterButtonBg_NEW.atlas = hplUIGray;
				
			}else{
				runSprite.SetActive(false);
				new_rotate.StopRotationAnimation();
				
				registerBoxAccountInput.enabled=true;
				registerBoxPasswordInput.enabled=true;
				//registerBoxConfirmPasswordInput.enabled=true;
				//RegisterButton_NEW.enabled=true;
				
				//RegisterButtonBg_NEW.atlas = hplUINormal;
			}
			
		}
		
	}
	
	private void RegisterSFSEvent () {
		//SFSEvent.CONNECTION_RESUME
		smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.LOGIN, OnLogin);
		//smartFox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
		smartFox.AddEventListener(SFSEvent.LOGIN_ERROR,OnLoginError);		
		//smartFox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);	
		//smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		//smartFox.AddEventListener(SFSEvent.CONFIG_LOAD_SUCCESS, OnConfigLoadSuccessHandler);
		//smartFox.AddEventListener(SFSEvent.CONFIG_LOAD_FAILURE, OnConfigLoadFailureHandler);
	}
	
	private void RegisterNGUIEvent () {
		/*
		loginBoxAccountInput.validator += OnInputValidator;
		loginBoxPasswordInput.validator += OnInputValidator;
		registerBoxAccountInput.validator += OnInputValidator;
		registerBoxPasswordInput.validator += OnInputValidator;
		//registerBoxConfirmPasswordInput.validator += OnInputValidator;
		*/
	}
	
	private void UnRegisterNGUIEvent () {
		/*
		loginBoxAccountInput.validator -= OnInputValidator;
		loginBoxPasswordInput.validator -= OnInputValidator;
		registerBoxAccountInput.validator -= OnInputValidator;
		registerBoxPasswordInput.validator -= OnInputValidator;
		//registerBoxConfirmPasswordInput.validator -= OnInputValidator;
		*/
	}
	
	private void UnregisterSFSSceneCallbacks() {
		// This should be called when switching scenes, so callbacks from the backend do not trigger code in this scene
		smartFox.RemoveAllEventListeners();
	}
	
	
	public void OnLoginAction () {
		//Debug.Log("OnLoginAction:" +isLogining);
		if(!isLogining){
			//Debug.Log1("OnLoginAction");
			string userName = loginBoxAccountInput.text;
			
			if(userName.Equals(string.Empty) || userName.Equals(string.Empty))
			{
                //errorMessage.text = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_HINT");
                UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_HINT"));
				return;
			}

			string userPassword = loginBoxPasswordInput.text;
			
			if(userName.Length < 2 || userPassword.Length < 3){
                //errorMessage.text = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_HINT2");
                UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_HINT2"));
				return;
			}
			
			if(userPassword.Length != 32)
				userPassword = md5(userPassword);
				
			if(smartFox == null || !smartFox.IsConnected){
				if(PlayerPrefs.HasKey("user_name"))
					PlayerPrefs.DeleteKey("user_name");
				
				if(PlayerPrefs.HasKey("user_pwd"))
					PlayerPrefs.DeleteKey("user_pwd");
				
				PlayerPrefs.SetString("user_name",userName);
				PlayerPrefs.SetString("user_pwd",userPassword);
				PlayerPrefs.Save();
				
				OnReConnect();
				return;
			}
			
			
			ISFSObject data = new SFSObject();
			data.PutInt("version", Globals.version);//当前软件版本号;
			runStatus("load",true);
			smartFox.Send(new LoginRequest(userName, userPassword, zone,data));
			
			isLogining = true;
		}
		else{

            UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_LOGIN_HINT_ISLOGINING"));
		}
	}
	
	public void OnShowRegister() {
		ErrorBox.SetActive(false);
		loginBox.SetActive(false);
		registerBox.SetActive(true);
	}
	
	public void OnCloseRegisterBox () {
		ErrorBox.SetActive(false);
		if(isShowLoginBox){
		loginBox.SetActive(true);
		}
		registerBox.SetActive(false);
		runStatus("reg",false);
	}
	
	public void OnRegisterAction () {
		string userName = registerBoxAccountInput.text.Trim();
		//Debug.Log("userName:" + userName + ";userName.length:" + userName.Length);
		if(userName.Length < 3){
            //registerBoxAccountHintLabel.gameObject.SetActive(false);
            //registerBoxAccountErrorHint.gameObject.SetActive(true);
            UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_INVALID_NAME2"));
			return;
		}
			
		if(registerBoxPasswordInput.text.Length < 3){
            UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_CHANGE_PWD_ERROR_HINT"));			
			return;
		}
		
			
		if(!Helper.CheckUserName(userName))
		{
            //errorMessage.text = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_HINT");
            UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_INVALID_NAME"));
				return;
		}
			
		/*
		if(registerBoxConfirmPasswordInput.text.Length < 3 || !registerBoxConfirmPasswordInput.text.Equals(registerBoxPasswordInput.text)){
			registerBoxConfirmPasswordHintLabel.gameObject.SetActive(false);
			registerBoxConfirmPasswordErrorHint.gameObject.SetActive(true);
			
			return;
		}*/
		
		if(smartFox == null || !smartFox.IsConnected){
			if(PlayerPrefs.HasKey("user_name"))
				PlayerPrefs.DeleteKey("user_name");
				
			if(PlayerPrefs.HasKey("user_pwd"))
				PlayerPrefs.DeleteKey("user_pwd");

			OnReConnect();
			return;
		}
		
		// Reset hint
		//registerBoxAccountHintLabel.gameObject.SetActive(true);
		//registerBoxAccountErrorHint.gameObject.SetActive(false);
		//registerBoxPasswordHintLabel.gameObject.SetActive(true);
		//registerBoxConfirmPasswordErrorHint.gameObject.SetActive(false);
		//registerBoxConfirmPasswordHintLabel.gameObject.SetActive(true);
		//registerBoxConfirmPasswordErrorHint.gameObject.SetActive(false);
		
		// Register
		ISFSObject data = new SFSObject();
		data.PutUtfString("user_name", registerBoxAccountInput.text);
		data.PutUtfString("user_pwd", registerBoxPasswordInput.text);
		data.PutInt("version", Globals.version);//当前软件版本号;	
		runStatus("reg",true);
		smartFox.Send(new LoginRequest("user_reg", "fanwe998", zone, data));
	}
	
	public void OnTrialAction () {
		//Debug.Log("OnLoginAction:" +isLogining);
		if(!isLogining){				
			if(smartFox == null || !smartFox.IsConnected){
				if(PlayerPrefs.HasKey("user_name"))
					PlayerPrefs.DeleteKey("user_name");
				
				if(PlayerPrefs.HasKey("user_pwd"))
					PlayerPrefs.DeleteKey("user_pwd");
				
				OnReConnect();
				return;
			}
			
			//Debug.Log("smartFox.IsConnected:" + smartFox.IsConnected);
			runStatus("load",true);
			isLogining = true;
			
			ISFSObject data = new SFSObject();
			data.PutInt("version", Globals.version);//当前软件版本号;
			smartFox.Send(new LoginRequest("players_reg", "fanwe998", zone,data));	
		}
		else{

            UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_LOGIN_HINT_ISLOGINING"));
		}
	}
	
	// NGUI callbacks
	public char OnInputValidator (string currentText, char nextChar) {
		//Debug.Log1("current text:"+currentText+" next char:"+nextChar.ToString());
		char result = default(char);
		
		if(numberLetterRegex.IsMatch(nextChar.ToString())){
			result = nextChar;
		}
		
		return result;
	}

	public void OnReConnect(){
		//Debug.Log("OnDoFinish ip:" + ip + ";port:" + port);
		if (smartFox != null){
					
			UnregisterSFSSceneCallbacks();
			if (smartFox.IsConnected){
				smartFox.Disconnect();				
			}
			smartFox = null;
		}else{
			//Debug.Log1("smartFox is null");
		}
		
		smartFox = new SmartFox(debug);			
		RegisterSFSEvent();	
		//ip = "g.fanwesoft.com";
		smartFox.Connect(Globals.domain, port);	
	}
	
	public void OnConnection(BaseEvent evt) {
		
		bool success = (bool)evt.Params["success"];
        //Debug.Log1("OnConnection");
		//errorMessage.text = string.Empty;
		Debug.Log("OnConnection:" + success);
		if (success) {
			SmartFoxConnection.Connection = smartFox;
			if (registerBox.activeSelf == false){

				
				if(isShowLoginBox){
					loginBox.gameObject.SetActive(true);
				}
				
				autoLogin();
			}
		} else {
			//Debug.Log1(evt.Params["errorMessage"]);
			//MessageManage.Instance.ShowMessage(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_FAILURE"));
			Alert(LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),
				LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_FAILURE"),
				LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),OnReConnect);
				
			//errorMessage.text = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_FAILURE");
		}
	}
	
	public void autoLogin(){
		//return;

		//自动登陆;

				String user_name = PlayerPrefs.GetString("user_name",string.Empty);
				String user_pwd = PlayerPrefs.GetString("user_pwd",string.Empty);
				
			//Debug.Log("user_name1:" + user_name + ";user_pwd1:" + user_pwd + ";zone:" + zone);
			//1:允许自动登陆;	0:不允许自动登陆;
			//int allowAutoLogin = PlayerPrefs.GetInt("allowAutoLogin",1);
			
				if (!string.Empty.Equals(user_name) && !string.Empty.Equals(user_pwd)){
					if (user_pwd.Length != 32){
					user_pwd = md5(user_pwd);
				}
				loginBox.gameObject.SetActive(false);
				
				if (smartFox != null || smartFox.IsConnected){
					ISFSObject data = new SFSObject();
				data.PutInt("version", Globals.version);//当前软件版本号;			
					smartFox.Send(new LoginRequest(user_name, user_pwd, zone,data));
					isLogining = true;
				}else{
					OnReConnect();
				}	
				}

		
	}
	
	public void OnConnectionLost(BaseEvent evt) {
		if (registerBox.activeSelf == false){
            //Debug.Log("OnConnectionLost");
            UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_LOST"));
			//Debug.Log1("OnConnectionLost");
			//
			
			UnregisterSFSSceneCallbacks();
			UnRegisterNGUIEvent();
			isLogining = false;
			
			OnReConnect();
		}
	}

	public void OnDebugMessage(BaseEvent evt) {
		//string message = (string)evt.Params["message"];
		//Debug.Log("[SFS DEBUG] " + message);
	}
	
	public void OnLogin(BaseEvent evt) {
		//Debug.Log("reg_error1111111:" );
		//User user = (User)evt.Params["user"];
		//Debug.Log1("OnLogin:" + (String)evt.Params["zone"]);

		// Startup up UDP
		Debug.Log("Login ok");	

		Globals.LastSceneUserId = -1;
		Globals.LastSceneRegionsId = -1;

			ISFSObject dt = (SFSObject)evt.Params["data"];
			int sync_error = dt.GetInt("sync_error");
			//Debug.Log1("sync_error:" + sync_error);
			if (sync_error == 0){
				//成功注册后,登陆;
				String user_name = dt.GetUtfString("user_name");// (String)evt.Params["user_name"];
				String user_pwd = dt.GetUtfString("user_pwd");//(String)evt.Params["user_pwd"];
			
			//Debug.Log("user_name:" + user_name + ";user_pwd:" + user_pwd);
			
				int user_id = dt.GetInt("user_id");
				PlayerPrefs.SetString("user_name",user_name);
				PlayerPrefs.SetString("user_pwd",user_pwd);
				PlayerPrefs.SetInt("user_id",user_id);
				
				PlayerPrefs.Save();
			}
		
			//Debug.Log1("LoadLevel_lobby");
			UnregisterSFSSceneCallbacks();
			UnRegisterNGUIEvent();
			//Application.LoadLevel("lobby");
			//Application.LoadLevel("HomeScene");	
		
		loginBox.SetActive(false);
		registerBox.SetActive(false);
		runSprite.SetActive(false);
		
		// Load home scene async
		//async = Application.LoadLevelAsync("MainScene");
		//async.allowSceneActivation = false;
		AudioPlayer.Instance.PlaySfx("supercell_jingle");
		Invoke("JumpScene",1f);


	}

	void JumpScene()
	{
		AudioPlayer.Instance.PlaySfx("loading_screen_jingle");
		SMGameEnvironment.Instance.SceneManager.TransitionPrefab = "SceneTransitions/LoginToMain";
		SMGameEnvironment.Instance.SceneManager.LoadLevel ("MainScene");
	}

	
	public void OnUpgrade(){
		//Debug.Log("OnUpgrade");
		//HplIOSCall.OpenUpgradePage();
		if (UpgradeUrl.LastIndexOf("?") > 0){
			UpgradeUrl += "&sys_type=" + Globals.sys_type;
		}else{
			UpgradeUrl += "?sys_type=" + Globals.sys_type;
		}

		//ErrorBox.SetActive(true);

		Application.OpenURL(UpgradeUrl);

	}

	public void ConnectToServer (string message) {
	
		//errorMessage.text = string.Empty;
		
		loginBox.SetActive(false);
		registerBox.SetActive(false);
		//Debug.Log("SmartFoxConnection.IsInitialized:" + SmartFoxConnection.IsInitialized);
		if (SmartFoxConnection.IsInitialized) {	
			if(isShowLoginBox){
			loginBox.SetActive(true);
			}
			smartFox = SmartFoxConnection.Connection;
		} else {
			smartFox = new SmartFox(debug);			
		}
		
		// Register callback delegate
		RegisterSFSEvent();
		
		// Register input delegate
		RegisterNGUIEvent();
		
		//默认音乐是开启的;
		if (PlayerPrefs.GetInt("MusicSwitch",-1) == -1){
			PlayerPrefs.SetInt("MusicSwitch",1);
		}
		
		if (PlayerPrefs.GetInt("SoundEffectSwitch",-1) == -1){
			PlayerPrefs.SetInt("SoundEffectSwitch",1);
		}		
		//smartFox.LoadConfig(Application.streamingAssetsPath + "/sfs-ancienttimes-config.xml",false);
		//PlayerPrefs.GetString("host_ip");
		//Debug.Log1(Dns.GetHostEntry("www.fanwe.com").AddressList[0]);
		//ip = Dns.GetHostEntry("g.fanwesoft.com").AddressList[0].ToString();//"121.199.23.76";
		//Debug.Log1("ip:" + ip);
		if (smartFox.IsConnected){
			is_first = true;
			autoLogin();
		}else
        {

            try
            {
                smartFox.Connect(Globals.domain, port);
            }
            catch (Exception ex)
            {
                Debug.Log("Connect Error msg : "+ex.Message);
            }
		}	
	}
	
	public void OnLoginError(BaseEvent evt) {
		runStatus("load",false);
		isLogining = false;
		//Debug.Log("OnLoginError:" + evt.ToString());
		/*
		foreach (DictionaryEntry de in evt.Params){
			//Debug.Log1("de.Key:" + de.Key + ";de.Value:" + de.Value);
			//Debug.Log1(de.Value);
		}
		*/
		//SFS
		/*
		PlayerPrefs.DeleteKey("user_name");
		PlayerPrefs.DeleteKey("user_pwd");
		*/
		
		
		string errorMsg = (string)evt.Params["errorMessage"];
		short errorCode = (short)evt.Params["errorCode"];
		//Debug.LogError("errorMsg:" + errorMsg+"  error code : "+errorCode);
		
		string[] arr = errorMsg.Split('^');
		if (arr.Length > 0){
			string code = arr[0];
			if ("-1".Equals(code)){
				//msg = "-1" + "^" + String.valueOf(type_reg) + "^" + res.getInt("user_id").toString() + "^" + res.getUtfString("user_name") + "^" + res.getUtfString("user_pwd");
				//注册;
				int type_reg = int.Parse(arr[1]);
				int user_id = int.Parse(arr[2]);
				if (user_id > 0){
					String user_name = arr[3];
					String user_pwd = arr[4];
					
					PlayerPrefs.SetString("user_name",user_name);
					PlayerPrefs.SetString("user_pwd",user_pwd);
					PlayerPrefs.SetInt("user_id",user_id);
					PlayerPrefs.Save();	
					
					runStatus("load",true);
					isLogining = true;
					//再次使用新帐户登陆;
					ISFSObject data = new SFSObject();
					data.PutInt("version", Globals.version);//当前软件版本号;					
					smartFox.Send(new LoginRequest(user_name, user_pwd, zone,data));
				}else{
					//创建新帐户失败!;
					//String msg = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_REG_ERROR");
					//MessageManage.Instance.ShowMessage(msg);
					if (type_reg == 3){
                        //用户已经存在;
                        UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_USER_NAME_EXIST"));
					}else{
                        //TID_ERROR_MESSAGE_REG_ERROR = 创建新帐户失败!
                        UIManager.Instance().normalMsgCtrl.ShowPop(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_REG_ERROR"));
					}
				
					runStatus("reg",false);			
					//loginBox.gameObject.SetActive(true);
				}				
			}else if ("1".Equals(code)){
				//抱歉，服务器正在维护中。请稍后重试;
				Alert(LocalizationCustom.instance.Get("TID_ERROR_POP_UP_SERVER_MAINTENANCE_TITLE"),
									LocalizationCustom.instance.Get("TID_ERROR_POP_UP_SERVER_MAINTENANCE"),
									LocalizationCustom.instance.Get("TID_ERROR_POP_UP_SERVER_MAINTENANCE_BUTTON"),autoLogin);				
				loginBox.gameObject.SetActive(false);				
			}else if ("2".Equals(code)){
				//好消息！现在可以免费下载新版本了;
				if (arr.Length == 2){
					UpgradeUrl = arr[1];
				}else{
					UpgradeUrl = "";
				}
				
				Alert(LocalizationCustom.instance.Get("TID_ERROR_POP_UP_WRONG_CLIENT_VERSION_TITLE"),
									LocalizationCustom.instance.Get("TID_ERROR_POP_UP_WRONG_CLIENT_VERSION"),
									LocalizationCustom.instance.Get("TID_ERROR_POP_UP_WRONG_CLIENT_VERSION_BUTTON"),OnUpgrade);								
			}else if ("3".Equals(code)){
				//TID_POPUP_HEADER_WARNING = 警告;
				//TID_POPUP_UNDER_ATTACK = 您的村庄正在遭受攻击！请稍等，稍后您的村庄信息将会自动更新;
				//TID_POPUP_TRY_AGAIN = 预计所需时间;

				//不在登陆时，提示：正在被攻击;

				lock_attack_time = int.Parse(arr[1]) + 240 + 10;
				Globals.lastLoadTime = long.Parse(arr[2]);
				long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
				Globals.time_difference = Globals.lastLoadTime - epoch;
				int itime = lock_attack_time - Helper.current_time();
				/*
				if (lock_attack_time > 0 && AttackBox.activeSelf){
					int itime = lock_attack_time - UserData.current_time();
					if (itime > 0){
						MsgTimeLabel.text = CalcCheck.GetFormatTime(itime,0);
					}else{
						lock_attack_time = 0;
						AttackBox.SetActive(false);
						autoLogin();
					}	
				}
				*/

				Alert(LocalizationCustom.instance.Get("TID_POPUP_HEADER_WARNING"),
				      LocalizationCustom.instance.Get("TID_POPUP_UNDER_ATTACK") + LocalizationCustom.instance.Get("TID_POPUP_TRY_AGAIN") + Helper.GetFormatTime(itime,0),
				      LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),autoLogin);

				loginBox.gameObject.SetActive(false);

			}else{
				String msg = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_ERROR");
                UIManager.Instance().normalMsgCtrl.ShowPop(msg);
				if(isShowLoginBox){
				loginBox.gameObject.SetActive(true);		
				}
			}			
		}else{
			String msg = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_ERROR");
            UIManager.Instance().normalMsgCtrl.ShowPop(msg);
			if(isShowLoginBox){
			loginBox.gameObject.SetActive(true);
			}
		}
		/*
		if (errorCode == 1){
			int user_id = int.Parse(arr[0]);
			string user_name = arr[1];
			string user_pwd = arr[2];
			
		}else{
		
			//Debug.Log1("Login error:"+errorMsg+"Error code:"+errorCode);
			String msg = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_ERROR") + "\n" + errorMsg + "\n" + errorCode;
			MessageManage.Instance.ShowMessage(msg);
			loginBox.gameObject.SetActive(true);
		}
		*/	
	}

	public void Alert(string title, string desc, string btn_name, EventDelegate.Callback errEvent){
		loginBox.gameObject.SetActive(false);
		registerBox.gameObject.SetActive(false);

		ErrorBox.SetActive(true);
		
		error_title.text = title; 
		error_btn_name.text = btn_name;
		error_desc.text = desc;
		error_callback = errEvent;
	

	}

	void OnAlertClick(){
		if (error_callback != null){
			//Debug.Log("aaa");
			error_callback();
			if (error_callback != OnUpgrade){
				//Debug.Log("xxx");
				ErrorBox.SetActive(false);
				error_callback = null;
			}
		}
	}
	
}
#endif