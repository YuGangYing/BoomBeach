//==========================================
// Created By yingyugang At 2/14/2016 8:33:08 PM
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
    public class AchivementPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Button m_btnClose;
        public Transform m_gridCarditems;
        public GameObject m_scrollCarditems;
        public Button m_btnPlayers;
        public Transform m_gridPlayers;
        public GameObject m_containerPlayers;
        public Button m_btnFriends;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.Find("#txt_title").GetComponent<Text>();
            m_btnClose = m_Trans.Find("#btn_close").GetComponent<Button>();
            m_gridCarditems = m_Trans.Find("#scroll_carditems/#grid_carditems");
            m_scrollCarditems = m_Trans.Find("#scroll_carditems").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_txtTitle = null;
            m_btnClose = null;
            m_gridCarditems = null;
            m_scrollCarditems = null;
            m_btnPlayers = null;
            m_gridPlayers = null;
            m_containerPlayers = null;
            m_btnFriends = null;

		}
    }
}
