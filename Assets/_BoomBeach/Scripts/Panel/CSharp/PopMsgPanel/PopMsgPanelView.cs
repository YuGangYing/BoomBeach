//==========================================
// Created By yingyugang At 2/4/2016 12:47:25 PM
//==========================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BoomBeach
{
    ///<summary>
    ///
    ///</summary>
    public class PopMsgPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Text m_txtConfirm;
        public GameObject m_containerDiamond;
        public Text m_txtDiamond;
        public Button m_btnConfirm;
        public Text m_txtMsg;
        public Button m_btnClose;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.FindChild("#txt_title").GetComponent<Text>();
            m_txtConfirm = m_Trans.FindChild("#btn_confirm/#txt_confirm").GetComponent<Text>();
            m_containerDiamond = m_Trans.FindChild("#btn_confirm/#container_diamond").gameObject;
            m_txtDiamond = m_Trans.FindChild("#btn_confirm/#txt_diamond").GetComponent<Text>();
            m_btnConfirm = m_Trans.FindChild("#btn_confirm").GetComponent<Button>();
            m_txtMsg = m_Trans.FindChild("#txt_msg").GetComponent<Text>();
            m_btnClose = m_Trans.FindChild("#btn_close").GetComponent<Button>();

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_txtTitle = null;
            m_txtConfirm = null;
            m_containerDiamond = null;
            m_txtDiamond = null;
            m_btnConfirm = null;
            m_txtMsg = null;
            m_btnClose = null;

		}
    }
}
