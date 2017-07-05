
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoomBeach
{
    public class SettingCtrl : BaseCtrl
    {

        SettingPanelView mSettingPanelView;

        List<GameObject> lngBtns;

		public override void ShowPanel()
        {
            bool isCreate;
            mSettingPanelView = UIMgr.ShowPanel<SettingPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
            Init();
			UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
        }

        void Init()
        {
            mSettingPanelView.m_containerMain.SetActive(true);
            mSettingPanelView.m_btnBack.gameObject.SetActive(false);
            mSettingPanelView.m_containerLng.SetActive(false);
            Debug.Log(PlayerPrefs.GetInt("MusicSwitch", 0));
            if (PlayerPrefs.GetInt("MusicSwitch", 0) == 0)
            {
                mSettingPanelView.m_btnMusic.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["OrangeButtonBg"];
                mSettingPanelView.m_txtMusic.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
            }
            else
            {
                mSettingPanelView.m_btnMusic.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["GreenButtonBg"];
                mSettingPanelView.m_txtMusic.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
            }

            if (PlayerPrefs.GetInt("SoundEffectSwitch", 0) == 0)
            {
                mSettingPanelView.m_btnSound.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["OrangeButtonBg"];
                mSettingPanelView.m_txtSound.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
            }
            else
            {
                mSettingPanelView.m_btnSound.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["GreenButtonBg"];
                mSettingPanelView.m_txtSound.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
            }
        }

        void OnCreatePanel()
        {
            mSettingPanelView.m_btnClose.onClick.AddListener(Close);
            mSettingPanelView.m_btnClose.onClick.AddListener(CloseMask);
            mSettingPanelView.m_btnBack.onClick.AddListener(Back);
            mSettingPanelView.m_btnMusic.onClick.AddListener(OnSwitchMusic);
            mSettingPanelView.m_btnSound.onClick.AddListener(OnSwitchSoundEffect);
            mSettingPanelView.m_btnLanguage.onClick.AddListener(OnSwitchLanguage);
            lngBtns = new List<GameObject>();
            Button[] btns = mSettingPanelView.m_containerLng.GetComponentsInChildren<Button>();
            for (int i=0;i< btns.Length; i++)
            {
                lngBtns.Add(btns[i].gameObject);
                btns[i].onClick.AddListener(ChangeLanguage);
            }
        }

        public void OnSwitchMusic()
        {
            if (PlayerPrefs.GetInt("MusicSwitch", 0) == 1)
            {
                PlayerPrefs.SetInt("MusicSwitch",0);

                mSettingPanelView.m_btnMusic.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["OrangeButtonBg"];
                mSettingPanelView.m_txtMusic.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
                AudioPlayer.Instance.IsPlayMusic = false;

                //Audio_music.stop();
            }
            else
            {
                PlayerPrefs.SetInt("MusicSwitch", 1);
                mSettingPanelView.m_btnMusic.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["GreenButtonBg"];
                mSettingPanelView.m_txtMusic.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
                AudioPlayer.Instance.IsPlayMusic = true;
                //Audio_music.play();
            }
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetInt("MusicSwitch", 0));
        }

        public void OnSwitchSoundEffect()
        {
            if (PlayerPrefs.GetInt("SoundEffectSwitch", 0) == 1)
            {
                PlayerPrefs.SetInt("SoundEffectSwitch", 0);

                mSettingPanelView.m_btnSound.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["OrangeButtonBg"];
                mSettingPanelView.m_txtSound.text = LocalizationCustom.instance.Get("TID_SETTINGS_OFF");
                AudioPlayer.Instance.IsPlaySfx = false;
            }
            else
            {
                PlayerPrefs.SetInt("SoundEffectSwitch",1);

                mSettingPanelView.m_btnSound.GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic["GreenButtonBg"];
                mSettingPanelView.m_txtSound.text = LocalizationCustom.instance.Get("TID_SETTINGS_ON");
                AudioPlayer.Instance.IsPlaySfx = true;
            }
            PlayerPrefs.Save();
        }

        public void OnSwitchLanguage()
        {
            mSettingPanelView.m_containerMain.SetActive(false);
            mSettingPanelView.m_btnBack.gameObject.SetActive(true);
            mSettingPanelView.m_containerLng.SetActive(true);
        }

        public override void Back()
        {
            mSettingPanelView.m_containerMain.SetActive(true);
            mSettingPanelView.m_btnBack.gameObject.SetActive(false);
            mSettingPanelView.m_containerLng.SetActive(false);
        }

        public void ChangeLanguage()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            int index = lngBtns.IndexOf(go);
            string lng = "";
            switch (index)
            {
                case 0: lng = "English"; break;
                case 1: lng = "Chinese"; break;
            }
            LocalizationCustom.instance.currentLanguage = lng;
            PlayerPrefs.SetString("currentLanguage", LocalizationCustom.instance.currentLanguage);
            PlayerPrefs.Save();
            Close();
            CloseMask();
            //Localization.instance.currentLanguage = sender.name;
            //OnBackToTrainBox(null);
        }

        /**
        private void ChangeLanguage(string lng)
        {
            LocalizationCustom.instance.currentLanguage = lng;
            PlayerPrefs.SetString("currentLanguage", LocalizationCustom.instance.currentLanguage);
         
            ISFSObject dt
            //通知服务器;
            ISFSObject data = new SFSObject();
            data.PutUtfString("lng", LocalizationCustom.instance.currentLanguage);
            NetworkManagerOld.Instance.SendRequest(data, "change_lng", false, null);
            //关闭窗口;
            if (PopWin.current != null) PopWin.current.CloseTween();
            
        }
        **/


        public override void Close()
        {
            UIMgr.ClosePanel("SettingPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }


    }
}
