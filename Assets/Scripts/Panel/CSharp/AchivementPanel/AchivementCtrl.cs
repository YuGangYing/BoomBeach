using UnityEngine;
using System.Collections;

namespace BoomBeach { 

public class AchivementCtrl : BaseCtrl
{

    AchivementPanelView mAchivementPanelView;

		public override void ShowPanel()
        {
            bool isCreate;
            mAchivementPanelView = UIMgr.ShowPanel<AchivementPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
			UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
        }

        void OnCreatePanel()
        {
            mAchivementPanelView.m_btnClose.onClick.AddListener(Close);
            mAchivementPanelView.m_btnClose.onClick.AddListener(CloseMask);
            mAchivementPanelView.m_txtTitle.text = LocalizationCustom.instance.Get("TID_POPUP_ACHIEVEMENTS_TITLE");
        }

        public override void Close()
        {
            UIMgr.ClosePanel("AchivementPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }
    }
}