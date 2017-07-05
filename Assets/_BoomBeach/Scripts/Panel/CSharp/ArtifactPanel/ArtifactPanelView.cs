//==========================================
// Created By yingyugang At 2/9/2016 9:33:31 PM
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
    public class ArtifactPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Text m_txtInfo;
        public Button m_btnClose;
        public Transform m_gridInfo;
        public Transform m_gridUsercrystal;
        public Text m_txtTotalinfo;
        public Button m_btnGreen;
        public Button m_btnBlue;
        public Button m_btnRed;
        public Button m_btnDeepp;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.FindChild("#txt_title").GetComponent<Text>();
            m_txtInfo = m_Trans.FindChild("#txt_info").GetComponent<Text>();
            m_btnClose = m_Trans.FindChild("#btn_close").GetComponent<Button>();
            m_gridInfo = m_Trans.FindChild("#grid_info");
            m_gridUsercrystal = m_Trans.FindChild("#grid_usercrystal");
            m_txtTotalinfo = m_Trans.FindChild("#txt_totalinfo").GetComponent<Text>();
            m_btnGreen = m_Trans.FindChild("#btn_green").GetComponent<Button>();
            m_btnBlue = m_Trans.FindChild("#btn_blue").GetComponent<Button>();
            m_btnRed = m_Trans.FindChild("#btn_red").GetComponent<Button>();
            m_btnDeepp = m_Trans.FindChild("#btn_deepp").GetComponent<Button>();

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_txtTitle = null;
            m_txtInfo = null;
            m_btnClose = null;
            m_gridInfo = null;
            m_gridUsercrystal = null;
            m_txtTotalinfo = null;
            m_btnGreen = null;
            m_btnBlue = null;
            m_btnRed = null;
            m_btnDeepp = null;

		}
    }
}
