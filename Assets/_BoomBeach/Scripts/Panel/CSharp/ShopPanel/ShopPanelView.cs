//==========================================
// Created By yingyugang At 2/7/2016 1:42:20 PM
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
    public class ShopPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Button m_btnDefence;
        public Button m_btnSurport;
        public Button m_btnResource;
        public GameObject m_tabCardtypes;
        public Transform m_gridResourcecarditems;
        public GameObject m_scrollCarditems;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_btnDefence = m_Trans.Find("#tab_cardtypes/#btn_defence").GetComponent<Button>();
            m_btnSurport = m_Trans.Find("#tab_cardtypes/#btn_surport").GetComponent<Button>();
            m_btnResource = m_Trans.Find("#tab_cardtypes/#btn_resource").GetComponent<Button>();
            m_tabCardtypes = m_Trans.Find("#tab_cardtypes").gameObject;
            m_gridResourcecarditems = m_Trans.Find("#scroll_carditems/#grid_resourcecarditems");
            m_scrollCarditems = m_Trans.Find("#scroll_carditems").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_btnDefence = null;
            m_btnSurport = null;
            m_btnResource = null;
            m_tabCardtypes = null;
            m_gridResourcecarditems = null;
            m_scrollCarditems = null;

		}
    }
}
