using UnityEngine;
using System.Collections;

public class ShowMsg: MonoBehaviour {
	public static ArrayList msgList = new ArrayList();
	public static ArrayList pushList = new ArrayList();
	public static GameObject pushObj;
	
	public static void show(string msg){
		//Debug.Log1(msg);
		//return;
		//Debug.Log("ddddd");

		GameObject msg_obj = Instantiate(ResourceCache.load("DialogBox/MsgLabel")) as GameObject;
		msg_obj.layer = GameObject.FindWithTag("CenterAnchor").layer;
		msg_obj.transform.parent = GameObject.FindWithTag("CenterAnchor").transform;
		msg_obj.layer = msg_obj.transform.parent.gameObject.layer;
		//Debug.Log("msg_obj.layer:" + msg_obj.layer);
		//msg_obj.transform.parent = GameObject.FindWithTag("CenterPanel").transform;	
		
		msg_obj.transform.localPosition = new Vector3(0,160,-10);
		msg_obj.transform.localScale = new Vector3(1,1,1);
		
		msg_obj.transform.Find("MsgLabel").GetComponent<UILabel>().text = msg;
		
		msgList.Insert(0, msg_obj);
			
		for(int i = 0; i < msgList.Count ; i ++){
			GameObject obj = msgList[i] as GameObject;			
			obj.transform.localPosition = new Vector3(0,180 + i * 30,-100);
		}
		//Debug.Log(msgList.Count);
				
		while(msgList.Count > 8){			 
			int i = msgList.Count - 1;
									
			GameObject obj = msgList[i] as GameObject;			
			DestroyImmediate(obj);			
			msgList.RemoveAt(i);		
		}
		
		//Debug.Log1("msgList.Count:" + msgList.Count);
	}

	
	public void hiddenMsg(){
		//Debug.Log1("hiddenMsg");
		
		msgList.Remove(gameObject);
		Destroy(gameObject);
		
		//Debug.Log1("msgList.Count:" + msgList.Count);
		
	}
	
}
