using UI.PlayerListPanel;

namespace BoomBeach { 
public class PlayerListCtrl : BaseCtrl
{

    PlayerListPanelView mPlayerListPanelView;

		public override void ShowPanel()
    {
        bool isCreate;
        mPlayerListPanelView = UIMgr.ShowPanel<PlayerListPanelView>(UIManager.UILayerType.Common, out isCreate);
        if (isCreate)
        {
            OnCreatePanel();
        }
			UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
    }

    void OnCreatePanel()
    {
        mPlayerListPanelView.m_btnClose.onClick.AddListener(Close);
        mPlayerListPanelView.m_btnClose.onClick.AddListener(CloseMask);
    }

    public override void Close()
    {
        UIMgr.ClosePanel("PlayerListPanel");
    }

    public void CloseMask()
    {
			UIMgr.GetController<MaskCtrl>().Close();
    }

    }

}
