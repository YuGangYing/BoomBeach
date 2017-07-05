using UnityEngine;
using System.Collections;
using BoomBeach;
using Sfs2X;
using System;
using Sfs2X.Core;
using Sfs2X.Requests;

public class GameFacade : MonoBehaviour {
    
	public GameObject gameLogicGo;
	private SmartFox smartFox;

    void Awake()
    {
#if UNITY_STANDALONE
        Screen.SetResolution(1334,740,false);
#endif
        Application.targetFrameRate = 60;
		Application.runInBackground = true;
		DontDestroyOnLoad(gameLogicGo);//Init 场景只会在开启游戏的时候加载，所以多次切换场景这个go不会有多份
		//UIManager.Instance().loginCtrl.ShowPanel();

		//读取系统配置
		GlobalsHelper.InitGlobals();

		//读取国际化
		LocalizationCustom.instance.Init();
		Debug.Log (Globals.defaultLanguage);
		string lng = Globals.defaultLanguage;// DataManager.sysConfig.GetStringProperties (SysConfigKeys.defaultLanguage);
		if (PlayerPrefs.HasKey("currentLanguage"))
		{
			lng = PlayerPrefs.GetString("currentLanguage");
			bool has_lng = false;
			for (int i = 0; i < LocalizationCustom.instance.languages.Length; ++i)
			{
				TextAsset asset = LocalizationCustom.instance.languages[i];
				if (asset != null && asset.name == lng)
				{
					has_lng = true;
				}
			}
			if (has_lng == false)
			{
				lng = Globals.defaultLanguage;
			}
		}
		LocalizationCustom.instance.currentLanguage = lng;

		//初始化音效
		AudioPlayer.Init();
		PlayerPrefs.SetInt("SoundEffectSwitch",1);
		PlayerPrefs.SetInt("MusicSwitch",1);
		//UIManager.Instance ();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");

		//读取csv数据
		//CSVMananger.instance.Init ();
    }
}
