using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;
// The Network manager sends the messages to server and handles the response.
using BoomBeach;

public class SFSNetworkManager : MonoBehaviour
{



	public delegate void OnConnected();
	public delegate void OnLoadDataFinish(ISFSObject dt,BuildInfo buildInfo=null);
    private bool running = false;
    private static SFSNetworkManager instance;
    private SmartFox smartFox;  
    //事件委托哈希表
    public Hashtable delegateTable = new Hashtable();
	public Hashtable buildTable = new Hashtable ();

	private float mSendingPeriod = 30f;
	private string mDomain = "127.0.0.1";
	private int mPort = 9933;
	private string mZone = "moba";

    private int mEventIndex = 0;
    private float mTimeLastSending = 0.0f;
	public static SFSNetworkManager Instance
    {
        get
        {
            if (instance != null && instance.smartFox == null)
            {
                instance.smartFox = SmartFoxConnection.Connection;
            }
            return instance;
        }
    }

    void Awake()
    {
        if(!instance)
            instance = this;
		
		//if (SmartFoxConnection.IsInitialized) {	
		smartFox = SmartFoxConnection.Connection;
		//} else {
		//	smartFox = new SmartFox(false);			
		//}
		RegisterSFSEvent();
		//ConnectToServer(string.Empty);
        ClearDelegateTable();
		mSendingPeriod = Globals.sendingPeriod;
		//DataManager.sysConfig.GetFloatProperties (SysConfigKeys.sendingPeriod);
		mDomain = Globals.domain; 
		//DataManager.sysConfig.GetStringProperties (SysConfigKeys.domain);
		mPort = Globals.port;
		//DataManager.sysConfig.GetIntProperties (SysConfigKeys.port);
		mZone = Globals.zone;
		//DataManager.sysConfig.GetStringProperties (SysConfigKeys.zone);
    }

    void OnDestroy()
    {
        if (instance && instance == this)
            instance = null;
    }

    void Start()
    {
        running = true;
    }

    // This is needed to handle server events in queued mode
    void Update()
    {
		if (!running || smartFox==null) 
			return;
		smartFox.ProcessEvents();
		if (mTimeLastSending >= mSendingPeriod)
        {
            mTimeLastSending = 0;
            smartFox.Send(new PingPongRequest());
        }
        mTimeLastSending += Time.deltaTime;
    }

	public void ConnectToServer (string message) {
		Debug.Log ("ConnectToServer");

		if (!smartFox.IsConnected){
			try
			{
				smartFox.Connect(mDomain, mPort);
			}
			catch (Exception ex)
			{
				Debug.Log("Connect Error msg : "+ex.Message);
			}
		}	
	}

	private void RegisterSFSEvent () {
		//UnsubscribeDelegates();
		smartFox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
		smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
	}

	void OnConnection(BaseEvent evt) {
		bool success = (bool)evt.Params["success"];
		//Debug.Log1("OnConnection");
		//errorMessage.text = string.Empty;
		Debug.Log("OnConnection:" + success);
		if (success) {
			SmartFoxConnection.Connection = smartFox;
            //UIManager.Instance().loginCtrl.ShowPanel();
			LoginCtrl loginCtrl = UIManager.GetInstance.GetController<LoginCtrl> ();
			loginCtrl.ShowPanel ();
			SmartFoxConnection.Connection.AddEventListener(SFSEvent.LOGIN_ERROR,loginCtrl.OnLoginError);
			SmartFoxConnection.Connection.AddEventListener(SFSEvent.LOGIN, loginCtrl.OnLogin);
			//UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
		} else {
			if (Globals.isLocalHomeData) {
				//UIManager.Instance ().loginCtrl.ShowPanel ();
				UIManager.GetInstance.GetController<LoginCtrl> ().ShowPanel ();
				//UnityEngine.SceneManagement.SceneManager.LoadScene ("LoginScene");
			} else {
				//TODO 显示连结失败文本提示
				UIManager.GetInstance.GetComponent<PopMsgCtrl>().ShowDialog (
					LocalizationCustom.instance.Get ("TID_ERROR_MESSAGE_CONNECTION_FAILURE"),
					LocalizationCustom.instance.Get ("TID_BUTTON_OKAY"),
					null,
					PopDialogBtnType.ConfirmBtn,
					new SFSObject (),
					OnReConnect,
					false
				);
			}
				
			//Debug.Log1(evt.Params["errorMessage"]);
			//MessageManage.Instance.ShowMessage(LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_FAILURE"));
			/*Alert(LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),
				LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_FAILURE"),
				LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),OnReConnect);
			*/
			//errorMessage.text = LocalizationCustom.instance.Get("TID_ERROR_MESSAGE_CONNECTION_FAILURE");
		}
	}

	void OnReConnect (ISFSObject dt,BuildInfo buildInfo = null){
		Debug.Log ("OnReConnect");
		smartFox.RemoveAllEventListeners();
		if (smartFox != null){
			if (smartFox.IsConnected){
				smartFox.Disconnect();				
			}
			smartFox = null;
		}
		smartFox = new SmartFox(false);			
		RegisterSFSEvent();	
		//ip = "g.fanwesoft.com";
		smartFox.Connect(mDomain, mPort);	
	}

    private void UnsubscribeDelegates()
    {
		Debug.Log ("UnsubscribeDelegates");
        smartFox.RemoveAllEventListeners();
    }

    public void SendLoguotRequest(bool is_del = true)
    {
        if (is_del)
        {
            PlayerPrefs.DeleteKey("user_name");
            PlayerPrefs.DeleteKey("user_pwd");
            PlayerPrefs.Save();
        }
        smartFox.Send(new LeaveRoomRequest());
        smartFox.Send(new LogoutRequest());
		AudioPlayer.Instance.PlayMusic("");
		Debug.Log ("SendLoguotRequest");
        smartFox.Disconnect();
		Destroy (UIManager.GetInstance.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene ("LoginScene");
		//Application.LoadLevel("LoginScene");
    }

    /// <summary>
    /// 发送扩展请求到服务器
    /// </summary>
	public void SendRequest(ISFSObject data, string cmd, bool isShowActivity, OnLoadDataFinish onSuccess,BuildInfo buildInfo = null)
    {
		Debug.Log ("cmd:" + cmd);
        if (data == null)
            data = new SFSObject();
        if (smartFox == null)
        {
            Debug.LogError("Send Network Message");
            return;
			//UnityEngine.SceneManagement.SceneManager.LoadScene ("LoginScene");
			//Application.LoadLevel("LoginScene");
        }
        else
        {
			mEventIndex++;
			data.PutInt("eventindex", mEventIndex);

            if (onSuccess != null)
            {               
                delegateTable[mEventIndex] = onSuccess;
				buildTable [mEventIndex] = buildInfo;
            }
            ExtensionRequest request = new ExtensionRequest(cmd, data);
            smartFox.Send(request);
        }
    }

    // This method handles all the responses from the server
    private void OnExtensionResponse(BaseEvent evt)
    {
		Debug.Log ("OnExtensionResponse");
	    string cmd = (string)evt.Params["cmd"];
        ISFSObject dt = (SFSObject)evt.Params["params"];
		// 出错;
        int sync_error = dt.GetInt("sync_error");
		if (sync_error == 1)
        {
            // 提示出错，重新载入游戏或者其他;
            Debug.LogError("Message cmd : "+cmd+" Sync error : "+sync_error);
			PopManage.Instance.ShowDialog(
				LocalizationCustom.instance.Get("TID_ERROR_POP_UP_OUT_OF_SYNC"),
				LocalizationCustom.instance.Get("TID_ERROR_POP_UP_OUT_OF_SYNC_TITLE"),
				true,
				PopDialogBtnType.ConfirmBtn,
				false,
				null,
				ReloadGame,
			    null,
				LocalizationCustom.instance.Get("TID_ERROR_POP_UP_OUT_OF_SYNC_BUTTON"),
				null,null
			);			 
        }else{
	        if(dt.ContainsKey("eventindex"))
	        {
	            int ei = dt.GetInt("eventindex");			
				//处理成就，没有放在BuildInfo中处理，是因为清除障碍物时，BuildInfo对象会被删除,所以可能无法接收到服务端返回的数据了;
				if ((cmd == "removal" || cmd == "finish_nur" || cmd == "load") && dt.ContainsKey("achievements")){
					//Debug.Log("xxx");
					int num = Helper.setAchievementsList(dt.GetSFSObject("achievements"));
                    if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.SetAchievementCount(num);
				}else if (cmd == "change_pwd" && dt.ContainsKey("pwd")){
					//不在SetWin中处理，是为了确保SetWin被禁用后，还能收到，回调消息;
					PlayerPrefs.SetString("user_pwd",dt.GetUtfString("pwd"));
				}else if (cmd == "rename" && dt.ContainsKey("name")){
					PlayerPrefs.SetString("user_name",dt.GetUtfString("name"));
				}
	            OnLoadDataFinish callbackMethod = (OnLoadDataFinish)delegateTable[ei];
				BuildInfo buildInfo = (BuildInfo)buildTable[ei];
	            if (callbackMethod != null)
	            {
					callbackMethod(dt,buildInfo);
	            }
	            delegateTable.Remove(ei);
				buildTable.Remove (ei);
	        }
		}

		// 关闭小菊花;
       // if (SmallUI.Instance.isShow)
         //   SmallUI.Instance.IsShowActivity(false);

    }

    /// <summary>
    /// When connection is lost we load the login scene
    /// </summary>
    private void OnConnectionLost(BaseEvent evt)
    {
		Debug.Log("OnConnectionLost!reason:"+(string)evt.Params["reason"]);
        ReloadGame(null);
    }

    void OnApplicationQuit()
    {
		
       // UnsubscribeDelegates();
       // ClearDelegateTable();
    }

    void ClearDelegateTable()
    {
        delegateTable.Clear();
		buildTable.Clear ();
        mEventIndex = 0;
    }

    /// <summary>
    /// 重新载入游戏
    /// </summary>
    void ReloadGame(ISFSObject dt)
    {
        UnsubscribeDelegates();
        ClearDelegateTable();
		SendLoguotRequest (false);
    }


}