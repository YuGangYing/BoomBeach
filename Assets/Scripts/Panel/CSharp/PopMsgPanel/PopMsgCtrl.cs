using UnityEngine;
using System.Collections;
using BoomBeach;
using UnityEngine.Events;
using Sfs2X.Entities.Data;
using UnityEngine.UI;

public class PopMsgCtrl : BaseCtrl
{
    PopMsgPanelView mMsgPanelView;

    // 提示框确定，取消回调;
	public delegate void OnDialogDelegate(ISFSObject dt,BuildInfo buildInfo);
    private OnDialogDelegate onDialogYes;
    
    ISFSObject dlgDialogParms;
	BuildInfo buildInfo;

    /*仅显示消息的对话框
msg:对话框内容;
title:对话框标题;
closeOther:显示对话框时,是否先关闭其它界面;
btnType:对话框类型;参考 PopDialogBtnType
showLeftTopClose: 是否显示：右上角的关闭按钮
otherParms:对话框,回调事件参数
onSure: 确定回调事件;
onCancel: 取消回调事件;
SureTitle: 确定按钮名字(默认为：确定)
CancelTitel: 取消按钮名字(默认为：取消)
 */
    public void ShowDialog(
        string msg, string title = null,string diamond = null,PopDialogBtnType btnType = PopDialogBtnType.ConfirmBtn,
		ISFSObject otherParms = null,OnDialogDelegate onSure = null,bool closeAble =true,BuildInfo buildInfo = null)
    {
        
		this.buildInfo = buildInfo;
        dlgDialogParms = otherParms;
        onDialogYes = onSure;
		Debug.Log (msg);
		ShowPanel();
		mMsgPanelView.m_btnClose.gameObject.SetActive (closeAble);

        mMsgPanelView.m_btnConfirm.onClick.RemoveAllListeners();
        mMsgPanelView.m_btnConfirm.onClick.AddListener(OnConfirmBtnClick);
        mMsgPanelView.m_btnConfirm.onClick.AddListener(Close);
        mMsgPanelView.m_btnConfirm.onClick.AddListener(CloseMask);

        mMsgPanelView.m_txtConfirm.gameObject.SetActive(true);
        mMsgPanelView.m_containerDiamond.gameObject.SetActive(false);
        mMsgPanelView.m_txtDiamond.gameObject.SetActive(false);

        mMsgPanelView.m_txtTitle.text = title;
        mMsgPanelView.m_txtMsg.text = msg;
        
        /**
        UILabel MsgText = DoxDialog.Find("MsgDialog").GetComponent<UILabel>();
        UILabel InputLable = DoxDialog.Find("InputLable").GetComponent<UILabel>();
        GameObject InputBox = DoxDialog.Find("Input").gameObject;
        MsgText.gameObject.SetActive(true);
        InputLable.gameObject.SetActive(false);
        InputBox.SetActive(false);
        //出现2个按钮，确定与取消;
        Transform YesBtn = DoxDialog.Find("YesBtn");
        Transform NoBtn = DoxDialog.Find("NoBtn");
        //仅出现一个：确定 按钮;
        Transform ConfirmBtn = DoxDialog.Find("ConfirmBtn");
        //仅出现一个带：图标的 确定按钮;
        Transform ImageBtn = DoxDialog.Find("ImageBtn");
        **/
        if (btnType == PopDialogBtnType.ConfirmBtn)
        {
            mMsgPanelView.m_txtConfirm.gameObject.SetActive(true);
            mMsgPanelView.m_containerDiamond.gameObject.SetActive(false);
            mMsgPanelView.m_txtDiamond.gameObject.SetActive(false);
            /**
            YesBtn.gameObject.SetActive(false);
            NoBtn.gameObject.SetActive(false);
            ConfirmBtn.gameObject.SetActive(true);
            ImageBtn.gameObject.SetActive(false);
            **/
        }
        else if (btnType == PopDialogBtnType.ImageBtn)
        {
            mMsgPanelView.m_txtConfirm.gameObject.SetActive(false);
            mMsgPanelView.m_containerDiamond.gameObject.SetActive(true);
            mMsgPanelView.m_txtDiamond.gameObject.SetActive(true);
            mMsgPanelView.m_txtDiamond.text = diamond;
            /**
            YesBtn.gameObject.SetActive(false);
            NoBtn.gameObject.SetActive(false);
            ConfirmBtn.gameObject.SetActive(false);
            ImageBtn.gameObject.SetActive(true);
            **/
        }

        /**
        else if (btnType == PopDialogBtnType.YesAndNoBtn)
        {
            YesBtn.gameObject.SetActive(true);
            NoBtn.gameObject.SetActive(true);
            ConfirmBtn.gameObject.SetActive(false);
            ImageBtn.gameObject.SetActive(false);
        }
        else if (btnType == PopDialogBtnType.InputYesNoBtn)
        {
            YesBtn.gameObject.SetActive(true);
            NoBtn.gameObject.SetActive(true);
            ConfirmBtn.gameObject.SetActive(false);
            ImageBtn.gameObject.SetActive(false);

            InputLable.gameObject.SetActive(true);
            InputLable.text = msg;
            InputBox.SetActive(true);
            MsgText.gameObject.SetActive(false);

            if (dlgDialogParms == null)
            {
                dlgDialogParms = new SFSObject();
            }

            DoxDialog.Find("Input/Label").GetComponent<UILabel>().text = "";
            DoxDialog.Find("Input").GetComponent<UIInput>().value = "";

            dlgDialogParms.PutBool("is_input_box", true);
        }
        **/

        //MsgText.text = msg;

        /**
        if (SureTitle == null)
        {
            SureTitle = LocalizationCustom.instance.Get("TID_BUTTON_OKAY");
        }
        //YesBtn.FindChild("BtnTitle").GetComponent<UILabel>().text = SureTitle;
        ConfirmBtn.FindChild("BtnTitle").GetComponent<UILabel>().text = SureTitle;
        ImageBtn.FindChild("BtnTitle").GetComponent<UILabel>().text = SureTitle;


        if (CancelTitel == null)
        {
            CancelTitel = LocalizationCustom.instance.Get("TID_BUTTON_CANCEL");
        }
        NoBtn.FindChild("BtnTitle").GetComponent<UILabel>().text = CancelTitel;

        //绑定取消事件;
        List<EventDelegate> onCancelDelegate = new List<EventDelegate>();
        onCancelDelegate.Add(new EventDelegate(this, "onCancelDialog"));
        NoBtn.FindChild("BtnBg").GetComponent<UIButton>().onClick = onCancelDelegate;

        //绑定确定事件;
        List<EventDelegate> onSureDelegate = new List<EventDelegate>();
        onSureDelegate.Add(new EventDelegate(this, "onSureDialog"));
        YesBtn.FindChild("BtnBg").GetComponent<UIButton>().onClick = onSureDelegate;
        ConfirmBtn.FindChild("BtnBg").GetComponent<UIButton>().onClick = onSureDelegate;
        ImageBtn.FindChild("BtnBg").GetComponent<UIButton>().onClick = onSureDelegate;

        //图标;
        ImageBtn.FindChild("BtnIco").GetComponent<UISprite>().spriteName = ImageName;

        if (closeOther)
        {
            if (PopWin.current != null) PopWin.current.CloseTween();
        }

        //禁用当前显示的弹窗内容,并显示为指定的内容;
        if (currentDialogContent != null)
        {
            currentDialogContent.gameObject.SetActive(false);
        }
        currentDialogContent = DoxDialog;
        currentDialogContent.gameObject.SetActive(true);

        //Debug.Log("popDialog.AfterClose = onCancelDialog");

        popDialog.Width = 850;
        popDialog.Height = 550;
        popDialog.OpenWin(title);
        popDialog.AfterClose = onCancelDialog;
        **/
    }

    void OnConfirmBtnClick()
    {
        if (onDialogYes != null)
        {
			onDialogYes(dlgDialogParms,buildInfo);
        }
    }

	public override void ShowPanel()
    {
        bool isCreate;
        mMsgPanelView = UIMgr.ShowPanel<PopMsgPanelView>(UIManager.UILayerType.Top, out isCreate);
        if (isCreate)
        {
            OnCreatePanel();
        }
		UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
    }

    void OnCreatePanel()
    {
        mMsgPanelView.m_btnClose.onClick.AddListener(Close);
        mMsgPanelView.m_btnClose.onClick.AddListener(CloseMask);
    }

    public override void Close()
    {
        UIMgr.ClosePanel("PopMsgPanel");
    }

    public void CloseMask()
    {
		UIMgr.GetController<MaskCtrl>().Close();
    }

}
