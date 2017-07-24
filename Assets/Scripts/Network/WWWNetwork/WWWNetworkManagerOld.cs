using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Events;

namespace WWWNetwork
{
	public class WWWNetworkManagerOld : SingleMonoBehaviour<WWWNetworkManagerOld>
	{

		//public static string urlBase =  "http://localhost:8084/MobaWebServer";
		public static string urlBase = "http://192.168.10.100/everquest/index.php";
		public  Dictionary<string,string> cookies;

		public UnityAction<WWW> failCallBack;


		protected override void Awake ()
		{
			base.Awake ();
		}

		public void GetUserBuilding (UnityAction<WWW> callBack)
		{
			string url = urlBase + "/UserBuildingServlet";
#if UNITY_EDITOR
			Debug.Log ("GetUserBuilding:" + url);
#endif
			var www = new WWW (url, null, UnityCookies.GetCookieRequestHeader (cookies));
			StartCoroutine (WaitWWW (www, callBack));
		}

		public void GetBuildingInfos (UnityAction<WWW> callBack)
		{
			string url = urlBase + "/BuildInfoServlet";
#if UNITY_EDITOR
			Debug.Log ("GetBuildingInfos:" + url);
#endif
			var www = new WWW (url, null, UnityCookies.GetCookieRequestHeader (cookies));
			StartCoroutine (WaitWWW (www, callBack));
		}

		public void CommonCallBack (WWW www)
		{
			Debug.Log ("Error: " + www.error);
			Debug.Log ("Text: " + www.text);
		}

		public void Login (string userName, string password, UnityAction<WWW> callBack)
		{
			//string json = JsonConvert.SerializeObject(userInfo);
			//Debug.Log (json);
			WWWForm form = new WWWForm ();
			form.AddField ("userName", userName);
			form.AddField ("password", password);
			string url = urlBase;// + "/login/submit.moba";

			WWW www = new WWW (url, form);
			StartCoroutine (WaitWWW (www, callBack));
		}

		public void Register (string userName, string password, UnityAction<WWW> callBack)
		{
			//string json = JsonConvert.SerializeObject(userInfo);
			//Debug.Log(json);
			WWWForm form = new WWWForm ();
			form.AddField ("userName", userName);
			form.AddField ("password", password);
			string url = urlBase;// + "/register/submit.moba";
			WWW www = new WWW (url, form);
			StartCoroutine (WaitWWW (www, callBack));
		}

		IEnumerator WaitWWW (WWW www, UnityAction<WWW> callBack)
		{
			yield return www;
			Debug.Log (www.text);
			if (www.error != null) {
				Debug.LogError (www.error);
				if (failCallBack != null)
					failCallBack (www);
			} else {
				if (cookies == null)
					WWWNetworkManagerOld.GetInstance.cookies = www.ParseCookies ();//获取服务端Session
				if (callBack != null)
					callBack (www);
			}
		}

		void Update ()
		{
			Debug.Log (Social.Active.localUser.userName);
			if (Input.GetKeyDown (KeyCode.H)) {
//			string dataStr= "{test:adasdasda}";
//			byte[] dataByte = System.Text.ASCIIEncoding.ASCII.GetBytes (dataStr);
//			WWW www = new WWW(urlBase,dataByte);
				WWWForm form = new WWWForm ();
				form.AddField ("username", "dddd");
				form.AddField ("password", "dddddddd");
				string url = urlBase;// + "/register/submit.moba";
				WWW www = new WWW (APIConstant.SERVER_ROOT + APIConstant.SIGNUP, form);
				Debug.Log (SystemInfo.deviceUniqueIdentifier);

				StartCoroutine (WaitWWW (www, (WWW w) => {
				
				}));
			}

			if (Input.GetKeyDown (KeyCode.G)) {
				WWWForm form = new WWWForm ();
				form.AddField ("data", "dddd");
				string url = urlBase;// + "/register/submit.moba";
				var www = new WWW (url, null, UnityCookies.GetCookieRequestHeader (cookies));
				StartCoroutine (WaitWWW (www, (WWW w) => {

				}));
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
}