using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;

public class LoginController : MonoBehaviour {

	public Button confirmButton;
	public InputField userName;
	public InputField userPassword;

	public static string urlBase =  "http://localhost:8084/MobaWebServer";
	public static string cityLevel = "City";


	void Awake()
	{
		if(confirmButton)confirmButton.onClick.AddListener (Login);
		if(userName)userName.text = "Tester";
		if(userPassword)userPassword.text = "123456";
//		if (ES2.Exists ("userName")) {
//			DataCenter.Instance ().LoadUserInfo ();
//			Application.LoadLevel("City");			
//		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.H)){
			WWWNetworkManager.SingleTon ().Login ("Tester","123456",LoginCallBack);
		}
	}

	void Init()
	{
		WWWNetworkManager.SingleTon ().GetBuildingInfos (GetBuildingInfoCallBack);
//		MobaNetworkManager.SingleTon ().GetUserBuilding (GetUserBuildingCallBack);
	}

	void GetBuildingInfoCallBack(WWW www){
//		Debug.Log (www.responseHeaders["SET-COOKIE"].Substring(0,www.responseHeaders["SET-COOKIE"].IndexOf(";")));
//		DataCenter.Instance().buildingInfoList = JsonConvert.DeserializeObject<List<BuildingInfo>>(www.text);

	}

	void GetUserBuildingCallBack(WWW www){
//		DataCenter.Instance().userBuildingList = JsonConvert.DeserializeObject<List<UserBuilding>>(www.text);
//		Debug.Log (DataCenter.Instance().userBuildingList.Count);
	}

	void LoginCallBack(WWW www){
		WWWNetworkManager.SingleTon().cookies = www.ParseCookies ();
		Debug.Log (WWWNetworkManager.SingleTon().cookies.Count);
		//UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(www.text);
//		DataCenter.Instance().userInfo = userInfo;
		Init ();
	}

	void Login()
	{
		WWWNetworkManager.SingleTon ().Login (userName.text,userPassword.text,LoginCallBack);
	}

}
