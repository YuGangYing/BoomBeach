using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace WWWNetwork
{
	public class WWWNetworkManager : SingleMonoBehaviour<WWWNetworkManager>
	{
		public  Dictionary<string,string> cookies;

		public UnityAction<WWW> failCallBack;

		public void CommonCallBack (WWW www)
		{
			Debug.Log ("Error: " + www.error);
			Debug.Log ("Text: " + www.text);
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
					cookies = www.ParseCookies ();//获取服务端Session
				if (callBack != null)
					callBack (www);
			}
		}

		public void Send (string apiPath, byte[] data, UnityAction<WWW> complete)
		{
			WWW www;
			if(cookies==null)
				www = new WWW (APIConstant.SERVER_ROOT + apiPath, data,new Dictionary<string,string>{ { "DeviceID",SystemInfo.deviceUniqueIdentifier} });
			else
				 www = new WWW (APIConstant.SERVER_ROOT + apiPath, data, UnityCookies.GetCookieRequestHeader(cookies));
			StartCoroutine (WaitWWW (www, complete));
		}
	}
}