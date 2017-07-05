using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public delegate void WWWCallBack(WWW www);
public class WWWNetworkManager : MonoBehaviour {

    //public static string urlBase =  "http://localhost:8084/MobaWebServer";
    public static string urlBase = "http://121.199.3.185:8080/server";
    public  Dictionary<string,string> cookies;

	static WWWNetworkManager instance;
	public static WWWNetworkManager SingleTon(){
		if(instance == null)
		{
			GameObject go = new GameObject("_NetworkManager");
			instance = go.AddComponent<WWWNetworkManager>();
			DontDestroyOnLoad (go);
		}
		return instance;
	}


	public WWWCallBack failCallBack;

	public void GetUserBuilding(WWWCallBack callBack){
		string url = urlBase + "/UserBuildingServlet" ;
#if UNITY_EDITOR
		Debug.Log("GetUserBuilding:" + url);
#endif
		var www = new WWW( url, null, UnityCookies.GetCookieRequestHeader(cookies) );
		StartCoroutine (WaitWWW(www,callBack));
	}

	public void GetBuildingInfos(WWWCallBack callBack){
		string url = urlBase + "/BuildInfoServlet" ;
#if UNITY_EDITOR
		Debug.Log("GetBuildingInfos:" + url);
#endif
		var www = new WWW( url, null, UnityCookies.GetCookieRequestHeader(cookies) );
		StartCoroutine (WaitWWW(www,callBack));
	}

	public void CommonCallBack(WWW www)
	{
		Debug.Log ("Error: " + www.error);
		Debug.Log ("Text: " + www.text);
	}

	public void Login(string userName,string password,WWWCallBack callBack)
	{
		//string json = JsonConvert.SerializeObject(userInfo);
		//Debug.Log (json);
		WWWForm form = new WWWForm ();
		form.AddField ("userName", userName);
        form.AddField("password", password);
		string url = urlBase + "/login/submit.moba";
		WWW www = new WWW (url,form);
		StartCoroutine (WaitWWW(www,callBack));
	}

    public void Register(string userName, string password, WWWCallBack callBack)
    {
        //string json = JsonConvert.SerializeObject(userInfo);
        //Debug.Log(json);
        WWWForm form = new WWWForm();
        form.AddField("userName", userName);
        form.AddField("password", password);
        string url = urlBase + "/register/submit.moba";
        WWW www = new WWW(url, form);
        StartCoroutine(WaitWWW(www, callBack));
    }

	IEnumerator WaitWWW(WWW www,WWWCallBack callBack)
	{
		yield return www;
		Debug.Log(www.text);
		if (www.error != null) {
			Debug.LogError(www.error);
			if (failCallBack!=null)
				failCallBack (www);
		} else {
			WWWNetworkManager.SingleTon().cookies = www.ParseCookies ();//获取服务端Session
			if(callBack!=null)
				callBack(www);
		}
	}
}

public class WWWResponseData
{
    public string type;
    public string content;
    public string data;
    public bool success;
}
