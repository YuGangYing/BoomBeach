//==========================================
// Created By yingyugang At 2/7/2016 5:31:13 PM
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
    public class ResearchPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Button m_btnClose;
        public Button m_btnBack;
        public Transform m_gridArmy;
        public GameObject m_containerTroop;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.FindChild("#txt_title").GetComponent<Text>();
            m_btnClose = m_Trans.FindChild("#btn_close").GetComponent<Button>();
            m_btnBack = m_Trans.FindChild("#btn_back").GetComponent<Button>();
            m_gridArmy = m_Trans.FindChild("#container_troop/#grid_army");
            m_containerTroop = m_Trans.FindChild("#container_troop").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_txtTitle = null;
            m_btnClose = null;
            m_btnBack = null;
            m_gridArmy = null;
            m_containerTroop = null;

		}
    }
}
