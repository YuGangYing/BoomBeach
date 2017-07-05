using UnityEngine.Events;


namespace BoomBeach {
public class MaskCtrl : BaseCtrl
{

    public MaskPanelView mMaskPanelView;

    void Awake() {
       
    }

    void Start() {

    }

    public void ShowPanel(UnityAction action)
    {
        bool isCreate;
        mMaskPanelView = UIManager.GetInstance().ShowPanel<MaskPanelView>(UIManager.UILayerType.Mask,out isCreate);
        mMaskPanelView.m_btnMask.onClick.RemoveAllListeners();
		if (action != null) {
			mMaskPanelView.m_btnMask.onClick.AddListener(action);
			mMaskPanelView.m_btnMask.onClick.AddListener(Close);
		}
    }

    public override void Close()
    {
        UIMgr.ClosePanel("MaskPanel");
    }

}
}