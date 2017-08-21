using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

	public Button btn_ok;

	void Start () {
		btn_ok = GetComponent<Button> ();
		btn_ok.onClick.AddListener (()=>{
			PlayerPrefs.SetInt("Date",System.DateTime.Now.Minute );
			PlayerPrefs.Save();
		});
	}
	
	void Update () {
		CheckLoginBonus ();	
	}

	void CheckLoginBonus(){
		if(PlayerPrefs.HasKey("Date")){
			int minute = PlayerPrefs.GetInt("Date");
			if (minute < System.DateTime.Now.Minute) {
				//TODO
				btn_ok.GetComponentInChildren<Text>(true).text = "LoginBonus";
				btn_ok.GetComponent<Image> ().color = Color.red;
				PlayerPrefs.DeleteKey ("Date");
				PlayerPrefs.Save();
			}
		}
	}

}
