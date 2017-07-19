using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class SetWin : MonoBehaviour {
	

	private bool isInit;

	private UISprite music_sprite;
	private UISprite sfx_sprite;
	private UILabel music_lable;
	private UILabel sfx_lable;
	private Transform btn_panel;
	private Transform lng_panel;

	public void Init()
	{
		if(!isInit)
		{

			btn_panel = transform.Find("btn_panel");
			lng_panel = transform.Find("lng_panel");


			music_sprite = btn_panel.Find("music_btn").GetComponent<UISprite>();
			music_lable = btn_panel.Find("music_btn/Label").GetComponent<UILabel>();

			sfx_sprite = btn_panel.Find("sfx_btn").GetComponent<UISprite>();
			sfx_lable = btn_panel.Find("sfx_btn/Label").GetComponent<UILabel>();


			isInit = true;
		}

		btn_panel.gameObject.SetActive(true);
		lng_panel.gameObject.SetActive(false);
	}



	public void BindInfoWin()
	{

		if(PlayerPrefs.GetInt("MusicSwitch",0) == 1){
			music_sprite.spriteName = "OrangeButtonBg";
			music_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
		}
		else{
			music_sprite.spriteName = "GreenButtonBg";
			music_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
		}
		
		if(PlayerPrefs.GetInt("SoundEffectSwitch",0) == 1){
			sfx_sprite.spriteName = "OrangeButtonBg";
			sfx_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
		}
		else{
			sfx_sprite.spriteName = "GreenButtonBg";
			sfx_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
		}
	}

	public void OnSwitchMusic () {
		if(PlayerPrefs.GetInt("MusicSwitch",0) == 1){
			PlayerPrefs.SetInt("MusicSwitch",0);
			music_sprite.spriteName = "GreenButtonBg";
			music_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
			AudioPlayer.Instance.IsPlayMusic = false;
			//Audio_music.stop();
		}
		else{
			PlayerPrefs.SetInt("MusicSwitch",1);
			music_sprite.spriteName = "OrangeButtonBg";
			music_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
			AudioPlayer.Instance.IsPlayMusic = true;
			//Audio_music.play();
		}
	}
	
	public void OnSwitchSoundEffect () {
		if(PlayerPrefs.GetInt("SoundEffectSwitch",0) == 1){
			PlayerPrefs.SetInt("SoundEffectSwitch",0);
			sfx_sprite.spriteName = "GreenButtonBg";
			sfx_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
			AudioPlayer.Instance.IsPlaySfx = false;
		}
		else{
			PlayerPrefs.SetInt("SoundEffectSwitch",1);
			sfx_sprite.spriteName = "OrangeButtonBg";
			sfx_lable.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
			AudioPlayer.Instance.IsPlaySfx = true;
		}
	}

	private void onDialogExitYes(ISFSObject dt){
		//Debug.Log("onDialogExitYes");
		SFSNetworkManager.Instance.SendLoguotRequest(true);
	}

	public void OnClickExit () {
		PopManage.Instance.ShowDialog(
			StringFormat.FormatByTid("TID_EXIT_INFO"),
			StringFormat.FormatByTid("TID_EXIT_TITLE"),
			false,
			PopDialogBtnType.YesAndNoBtn,
			false,
			null,
			onDialogExitYes
			);
	}

	public void OnClickChangePwd(){
		//Debug.Log("OnClickChangePwd");
		PopManage.Instance.ShowDialog(
			StringFormat.FormatByTid("TID_NEW_PWD"),
			StringFormat.FormatByTid("TID_CHANGE_PWD"),
			false,
			PopDialogBtnType.InputYesNoBtn,
			false,
			null,
			onDialogInputYes
			);
		
	}

	private void onDialogInputYes(ISFSObject dt){
		string input_txt = dt.GetUtfString("input_txt");
		if (input_txt == null || input_txt == ""){
			PopManage.Instance.ShowDialog(StringFormat.FormatByTid("TID_NEW_PWD_ERROR"));
		}else{

			//Debug.Log("onDialogInputYes:" + input_txt);

			//通知服务器;
			ISFSObject data = new SFSObject();
			data.PutUtfString("pwd",input_txt);
			//在NetworkManager.OnExtensionResponse 中处理回调，确保SetWin被禁用后，还能收到，回调消息;
			SFSNetworkManager.Instance.SendRequest(data, "change_pwd", false, null);
			//关闭窗口;
			if(PopWin.current!=null)PopWin.current.CloseTween();
		}
		//NetworkManager.Instance.SendLoguotRequest(true);
	}



	public void OnClickLng () {
		//Localization.instance.currentLanguage = sender.name;
		//OnBackToTrainBox(null);
		btn_panel.gameObject.SetActive(false);
		lng_panel.gameObject.SetActive(true);
	}

	public void OnClickBackLng () {
		//Localization.instance.currentLanguage = sender.name;
		//OnBackToTrainBox(null);
		btn_panel.gameObject.SetActive(true);
		lng_panel.gameObject.SetActive(false);
	}

	public void OnSelectLngEn () {
		//Localization.instance.currentLanguage = sender.name;
		//OnBackToTrainBox(null);
		ChangeLanguage("English");
	}

	public void OnSelectLngZh () {
		//Localization.instance.currentLanguage = sender.name;
		//OnBackToTrainBox(null);
		ChangeLanguage("Chinese");
	}

	public void ChangeLanguage(string lng) {
		//TID_POPUP_CHANGE_LANGUAGE_TITLE = 语言设置
		//TID_POPUP_CHANGE_LANGUAGE = 确认改变语言设置?
		SFSObject dlgDialogParms = new SFSObject();
		dlgDialogParms.PutUtfString("lng",lng);
		PopManage.Instance.ShowDialog(
			StringFormat.FormatByTid("TID_POPUP_CHANGE_LANGUAGE"),
			StringFormat.FormatByTid("TID_POPUP_CHANGE_LANGUAGE_TITLE"),
			false,
			PopDialogBtnType.YesAndNoBtn,
			false,
			dlgDialogParms,
			onDialogChangeLngYes
			);
	}

	private void onDialogChangeLngYes(ISFSObject dt){
        LocalizationCustom.instance.currentLanguage = dt.GetUtfString("lng");

		PlayerPrefs.SetString("currentLanguage", LocalizationCustom.instance.currentLanguage);


		//通知服务器;
		ISFSObject data = new SFSObject();
		data.PutUtfString("lng", LocalizationCustom.instance.currentLanguage);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_CHANGE_LNG, false, null);

		//关闭窗口;
		if(PopWin.current!=null)PopWin.current.CloseTween();
	}


	public void OnClickHelpAndSupport () {
		PopManage.Instance.ShowHelpAndSupportWin();
	}

}
