using UnityEngine;
using System.Collections;

public class DialogBox: MonoBehaviour {

	public delegate void OnSure();

	public static void alertMsg(string msg){
		/*
		GameObject box = Instantiate(Resources.Load("DialogBox/ErrorBox")) as GameObject;
		box.transform.parent = GameObject.FindWithTag("CenterPanel").transform;	
		box.transform.localPosition = new Vector3(0,0,getMinZ());
		box.transform.localScale = new Vector3(1,1,1);
		ErrorBoxUI tipBox = box.GetComponent<ErrorBoxUI>();	
		if (tipBox != null){
			string title = LocalizationCustom.instance.Get("TID_BUTTON_OKAY");
			tipBox.alert(title, msg, LocalizationCustom.instance.Get("TID_BUTTON_OKAY"),2,null);
		}
		*/
	}	
	public static void alertErr(string title, string msg, string btn_name, int r_type = 0,OnSure onSure = null){
		/*
		GameObject box = Instantiate(Resources.Load("DialogBox/ErrorBox")) as GameObject;
		box.transform.parent = GameObject.FindWithTag("CenterPanel").transform;	
		box.transform.localPosition = new Vector3(0,0,getMinZ());
		box.transform.localScale = new Vector3(1,1,1);
		ErrorBoxUI tipBox = box.GetComponent<ErrorBoxUI>();	
		if (tipBox != null){
			tipBox.alert(title, msg, btn_name,r_type,onDoFinish);
		}
		*/
	}
	
	public static void InputNameBox() {
		/*
		GameObject box = Instantiate(Resources.Load("DialogBox/InputBox")) as GameObject;
		box.transform.parent = GameObject.FindWithTag("CenterPanel").transform;	
		box.transform.localPosition = new Vector3(0,0,getMinZ());
		box.transform.localScale = new Vector3(1,1,1);
		InputNameBoxUI tipBox = box.GetComponent<InputNameBoxUI>();
		UILabel msgLabel = tipBox.transform.Find("Input/Label").GetComponent<UILabel>();
		tipBox.titleLabel.text = LocalizationCustom.instance.Get("TID_INPUT_USER_NAME");
		msgLabel.text = "";
		box.SetActive(true);
		
		UIInput uiInput = tipBox.transform.Find("Input").GetComponent<UIInput>();
		uiInput.selected = true;
*/
	}	
	
	public static void InputPwdBox() {
		/*
		GameObject box = Instantiate(Resources.Load("DialogBox/InputPwdBox")) as GameObject;
		box.transform.parent = GameObject.FindWithTag("CenterPanel").transform;	
		box.transform.localPosition = new Vector3(0,0,getMinZ());
		box.transform.localScale = new Vector3(1,1,1);
		InputPwdBoxUI tipBox = box.GetComponent<InputPwdBoxUI>();
		UILabel msgLabel = tipBox.transform.Find("Input/Label").GetComponent<UILabel>();
		tipBox.titleLabel.text = LocalizationCustom.instance.Get("TID_CHANGE_PWD");
		msgLabel.text = "";
		box.SetActive(true);
		
		UIInput uiInput = tipBox.transform.Find("Input").GetComponent<UIInput>();
		uiInput.isPassword = true;
		uiInput.selected = true;
*/
	}	
	

	
	public static float getMinZ(){
		float minz = -1;
//		bool has_obj = false;
		foreach(Transform a in GameObject.FindWithTag("CenterPanel").transform){
	//		has_obj = true;
			if (a.localPosition.z < minz){
				minz = a.localPosition.z;
			}
		}
		/*
		if (has_obj){
			minz = minz - 5;
		}
		*/
		minz = minz - 20;
		return minz;
	}
}
