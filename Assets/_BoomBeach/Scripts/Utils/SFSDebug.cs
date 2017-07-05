using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System;
using System.Text;

public static class SFSDebug {

	public static void Log(ISFSObject user_info){
		StringBuilder sb = new StringBuilder ();
		foreach(string key in user_info.GetKeys()){
			System.Object obj = user_info.GetClass (key);
			Type t = obj.GetType ();
			if (t == typeof(int))
				sb.AppendLine (key + ":" + user_info.GetInt (key));
			else if(t==typeof(long))
				sb.AppendLine (key + ":" + user_info.GetLong(key));
			else if(t==typeof(string))
				sb.AppendLine (key + ":" + user_info.GetUtfString(key));
			else if(t==typeof(bool))
				sb.AppendLine (key + ":" + user_info.GetBool(key));
			else if(t==typeof(float))
				sb.AppendLine (key + ":" + user_info.GetFloat(key));
			else
				sb.AppendLine (key + ":" + t.ToString());
		}
		Debug.Log (sb.ToString());
	}

}
