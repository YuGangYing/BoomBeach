using UnityEngine;
using System.Collections;

public class ScreenLayout : MonoBehaviour {

	//单例定义;
	private static ScreenLayout instance;
	public static ScreenLayout Instance{
		get{ return instance; }
	}

	public int Width{
		get{return GetComponent<UIWidget> ().width;}
	}

	// Use this for initialization
	void Awake () {
		instance = this;
		UIWidget widget = GetComponent<UIWidget> ();
		widget.width = widget.height*Screen.width/Screen.height;

	}

}
