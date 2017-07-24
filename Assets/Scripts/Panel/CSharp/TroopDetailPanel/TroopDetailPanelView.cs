//==========================================
// Created By yingyugang At 2/9/2016 11:53:29 AM
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
    public class TroopDetailPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Button m_btnClose;
        public Button m_btnBack;
        public Transform m_gridInfo;
        public Image m_imgBigicon;
        public Text m_txtLevel;
        public Text m_txtInfotitle;
        public Text m_txtInfo;
        public Text m_txtTime;
        public Button m_btnUpgrade;
        public Text m_txtGold;
        public GameObject m_containerGold;
        public Text m_txtDiamond;
        public GameObject m_containerDiamond;
        public Button m_btnInstant;
        public GameObject m_containerUpgrade;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.Find("#txt_title").GetComponent<Text>();
            m_btnClose = m_Trans.Find("#btn_close").GetComponent<Button>();
            m_btnBack = m_Trans.Find("#btn_back").GetComponent<Button>();
            m_gridInfo = m_Trans.Find("#grid_info");
            m_imgBigicon = m_Trans.Find("#img_bigicon").GetComponent<Image>();
            m_txtLevel = m_Trans.Find("#txt_level").GetComponent<Text>();
            m_txtInfotitle = m_Trans.Find("#txt_infotitle").GetComponent<Text>();
            m_txtInfo = m_Trans.Find("#txt_info").GetComponent<Text>();
            m_txtTime = m_Trans.Find("#container_upgrade/#btn_upgrade/#txt_time").GetComponent<Text>();
            m_btnUpgrade = m_Trans.Find("#container_upgrade/#btn_upgrade").GetComponent<Button>();
            m_txtGold = m_Trans.Find("#container_upgrade/#container_gold/#txt_gold").GetComponent<Text>();
            m_containerGold = m_Trans.Find("#container_upgrade/#container_gold").gameObject;
            m_txtDiamond = m_Trans.Find("#container_upgrade/#container_diamond/#txt_diamond").GetComponent<Text>();
            m_containerDiamond = m_Trans.Find("#container_upgrade/#container_diamond").gameObject;
            m_btnInstant = m_Trans.Find("#container_upgrade/#btn_instant").GetComponent<Button>();
            m_containerUpgrade = m_Trans.Find("#container_upgrade").gameObject;

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
            m_gridInfo = null;
            m_imgBigicon = null;
            m_txtLevel = null;
            m_txtInfotitle = null;
            m_txtInfo = null;
            m_txtTime = null;
            m_btnUpgrade = null;
            m_txtGold = null;
            m_containerGold = null;
            m_txtDiamond = null;
            m_containerDiamond = null;
            m_btnInstant = null;
            m_containerUpgrade = null;

		}
    }
}
