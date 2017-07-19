
using Newtonsoft.Json;
using UnityEngine;
using Sfs2X.Requests;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using System;
using UnityEngine.UI;

namespace BoomBeach {

    public class LoginCtrl : BaseCtrl
    {
		private string mZone = "moba";
		private int mVersion = 1;


		void Awake(){
			
			mZone = Globals.zone;
			mVersion = Globals.version;
		}

        LoginPanelView mLoginPanelView;
		public override void ShowPanel()
        {
            bool isCreate;
            mLoginPanelView = UIManager.GetInstance.ShowPanel<LoginPanelView>(UIManager.UILayerType.Fixed,out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
			if (PlayerPrefs.HasKey ("user_name")) {
				mLoginPanelView.m_inputAccountInputField.text = PlayerPrefs.GetString ("user_name");
				mLoginPanelView.m_inputPasswordInputField.text = PlayerPrefs.GetString ("user_pwd");
			} else {
				mLoginPanelView.m_inputAccountInputField.text = "";
				mLoginPanelView.m_inputPasswordInputField.text = "";
			}
        }

        void OnCreatePanel()
        {
            mLoginPanelView.m_btnLoginButton.onClick.AddListener(OnLoginBtnClick);
            mLoginPanelView.m_btnRegisterButton.onClick.AddListener(OnRegisterBtnClick);
			mLoginPanelView.m_containerDropdown.GetComponent<Dropdown> ().onValueChanged.AddListener (OnSelectDataFrom);
			if(PlayerPrefs.HasKey("isLocalData")){
				mLoginPanelView.m_containerDropdown.GetComponent<Dropdown> ().value = PlayerPrefs.GetInt ("isLocalData");
			}
        }

        public override void Close() {
            UIMgr.ClosePanel("LoginPanel");
        }

		void OnSelectDataFrom(int index)
		{
			if (index == 0) {
				Globals.isLocalHomeData = true;
				Globals.isLocalBattleData = true;
			} else {
				Globals.isLocalHomeData = false;
				Globals.isLocalBattleData = false;
			}
			PlayerPrefs.SetInt ("isLocalData",index);
		}

        void OnLoginBtnClick()
        {
            Debug.Log("LoginAction");



			if (Globals.isLocalHomeData) {
				UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
				AudioPlayer.Instance.PlaySfx("supercell_jingle");
				return;
			}
            string userName = mLoginPanelView.m_inputAccountInputField.text.Trim();
            string userPwd = mLoginPanelView.m_inputPasswordInputField.text.Trim();

			Debug.Log("userName:" + userName + ";userPwd:" + userPwd + ";mZone:" + mZone);
			SmartFoxConnection.Connection.Send(new LoginRequest(userName, userPwd, mZone));
        }
        
        void OnRegisterBtnClick()
        {
			Debug.Log ("OnRegisterBtnClick");
            string userName = mLoginPanelView.m_inputAccountInputField.text.Trim();
            string userPwd = mLoginPanelView.m_inputPasswordInputField.text.Trim();
            if (string.IsNullOrEmpty(userName))
            {
				Debug.LogError ("userName is null");
                return;
            }
			ISFSObject data = new SFSObject();
			data.PutUtfString("user_name", userName);
			data.PutUtfString("user_pwd", userPwd);
			data.PutInt("version", mVersion);//当前软件版本号;	
			SmartFoxConnection.Connection.Send(new LoginRequest("user_reg", "fanwe998", mZone, data));
        }

		public void OnLogin(BaseEvent evt) {
			Debug.Log("Login ok");	
			Globals.LastSceneUserId = -1;
			Globals.LastSceneRegionsId = -1;
			ISFSObject dt = (SFSObject)evt.Params["data"];
			int sync_error = dt.GetInt("sync_error");
			if (sync_error == 0){
				//成功注册后,登陆;
				string user_name = mLoginPanelView.m_inputAccountInputField.text.Trim();
				string user_pwd = mLoginPanelView.m_inputPasswordInputField.text.Trim();
				Globals.userId = dt.GetLong("user_id");
				PlayerPrefs.SetString("user_name",user_name);
				PlayerPrefs.SetString("user_pwd",user_pwd);
				PlayerPrefs.SetInt("user_id",(int)Globals.userId);
				PlayerPrefs.Save();
				AudioPlayer.Instance.PlaySfx("supercell_jingle");
				//TODO 此处应该是动态加载
				UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
			}
		}

		//登录失败和注册成功失败
		public void OnLoginError(BaseEvent evt) {
			Debug.Log ("OnLoginError");
			string errorMsg = (string)evt.Params["errorMessage"];
			//short errorCode = (short)evt.Params["errorCode"];
			string[] arr = errorMsg.Split('^');
			if (arr.Length > 0){
				string code = arr[0];
				if ("-1".Equals(code)){
					//msg = "-1" + "^" + String.valueOf(type_reg) + "^" + res.getInt("user_id").toString() + "^" + res.getUtfString("user_name") + "^" + res.getUtfString("user_pwd");
					//注册;
					int type_reg = int.Parse(arr[1]);
					int user_id = int.Parse(arr[2]);
					if (user_id > 0){
						string user_name = arr[3];
						string user_pwd = arr[4];
						PlayerPrefs.SetString("user_name",user_name);
						PlayerPrefs.SetString("user_pwd",user_pwd);
						PlayerPrefs.SetInt("user_id",user_id);
						PlayerPrefs.Save();	
						//再次使用新帐户登陆;
						ISFSObject data = new SFSObject();
						data.PutInt("version", mVersion);//当前软件版本号;					
						SmartFoxConnection.Connection.Send(new LoginRequest(user_name, user_pwd, mZone,data));
					}else{
						//创建新帐户失败!;
						if (type_reg == 3){
							//用户已经存在;
							UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(LocalizationCustom.instance.Get("TID_USER_NAME_EXIST"));
						}else{
							//TID_ERROR_MESSAGE_REG_ERROR = 创建新帐户失败!
							UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_REG_ERROR"));
						}
					}				
				}else if ("1".Equals(code)){
					//抱歉，服务器正在维护中。请稍后重试;
					/*
					Alert(LocalizationCustom.instance.Get("TID_ERROR_POP_UP_SERVER_MAINTENANCE_TITLE"),
						LocalizationCustom.instance.Get("TID_ERROR_POP_UP_SERVER_MAINTENANCE"),
						LocalizationCustom.instance.Get("TID_ERROR_POP_UP_SERVER_MAINTENANCE_BUTTON"),autoLogin);	
					*/
				}else if ("2".Equals(code)){
					//好消息！现在可以免费下载新版本了;
					/*
					if (arr.Length == 2){
						UpgradeUrl = arr[1];
					}else{
						UpgradeUrl = "";
					}
					*/
					/*
					Alert(LocalizationCustom.instance.Get("TID_ERROR_POP_UP_WRONG_CLIENT_VERSION_TITLE"),
						LocalizationCustom.instance.Get("TID_ERROR_POP_UP_WRONG_CLIENT_VERSION"),
						LocalizationCustom.instance.Get("TID_ERROR_POP_UP_WRONG_CLIENT_VERSION_BUTTON"),OnUpgrade);	
						*/
				}else if ("3".Equals(code)){
					//TID_POPUP_HEADER_WARNING = 警告;
					//TID_POPUP_UNDER_ATTACK = 您的村庄正在遭受攻击！请稍等，稍后您的村庄信息将会自动更新;
					//TID_POPUP_TRY_AGAIN = 预计所需时间;
					//不在登陆时，提示：正在被攻击;
					//int lock_attack_time = int.Parse(arr[1]) + 240 + 10;
					Globals.lastLoadTime = long.Parse(arr[2]);
					long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
					Globals.time_difference = Globals.lastLoadTime - epoch;
					//int itime = lock_attack_time - Helper.current_time();
					/*
					Alert(LocalizationCustom.instance.Get("TID_POPUP_HEADER_WARNING"),
						LocalizationCustom.instance.Get("TID_POPUP_UNDER_ATTACK") + LocalizationCustom.instance.Get("TID_POPUP_TRY_AGAIN") + Helper.GetFormatTime(itime,0),
						LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),autoLogin);
					*/
					//loginBox.gameObject.SetActive(false);
				}else{
					String msg = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_ERROR");
					UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
				}			
			}else{
				String msg = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_LOGIN_ERROR");
				UIManager.GetInstance.GetController<NormalMsgCtrl>().ShowPop(msg);
			}	
		}
    }
}
