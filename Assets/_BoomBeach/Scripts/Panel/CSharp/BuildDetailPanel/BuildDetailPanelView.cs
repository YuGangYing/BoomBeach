//==========================================
// Created By yingyugang At 03/06/2016 21:01:27
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
    public class BuildDetailPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Button m_btnClose;
        public Button m_btnBack;
        public Text m_txtHealth_num;
        public Transform m_gridInfo;
        public GameObject m_containerBuildpoint;
        public Text m_txtLevel;
        public Text m_txtInfo;
        public Text m_txtTime;
        public Button m_btnUpgrade;
        public Text m_txtDiamond;
        public GameObject m_containerDiamond;
        public Button m_btnInstant;
        public Text m_txtIron;
        public GameObject m_containerIron;
        public Text m_txtStone;
        public GameObject m_containerStone;
        public Text m_txtWood;
        public GameObject m_containerWood;
        public GameObject m_containerUpgrade;
        public GameObject m_containerUnlock;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.FindChild("#txt_title").GetComponent<Text>();
            m_btnClose = m_Trans.FindChild("#btn_close").GetComponent<Button>();
            m_btnBack = m_Trans.FindChild("#btn_back").GetComponent<Button>();
            m_txtHealth_num = m_Trans.FindChild("#grid_info/item_title/#txt_health_num").GetComponent<Text>();
            m_gridInfo = m_Trans.FindChild("#grid_info");
            m_containerBuildpoint = m_Trans.FindChild("#container_buildpoint").gameObject;
            m_txtLevel = m_Trans.FindChild("#txt_level").GetComponent<Text>();
            m_txtInfo = m_Trans.FindChild("#txt_info").GetComponent<Text>();
            m_txtTime = m_Trans.FindChild("#container_upgrade/#btn_upgrade/#txt_time").GetComponent<Text>();
            m_btnUpgrade = m_Trans.FindChild("#container_upgrade/#btn_upgrade").GetComponent<Button>();
            m_txtDiamond = m_Trans.FindChild("#container_upgrade/#container_diamond/#txt_diamond").GetComponent<Text>();
            m_containerDiamond = m_Trans.FindChild("#container_upgrade/#container_diamond").gameObject;
            m_btnInstant = m_Trans.FindChild("#container_upgrade/#btn_instant").GetComponent<Button>();
            m_txtIron = m_Trans.FindChild("#container_upgrade/grid_upgraderesources/#container_iron/#txt_iron").GetComponent<Text>();
            m_containerIron = m_Trans.FindChild("#container_upgrade/grid_upgraderesources/#container_iron").gameObject;
            m_txtStone = m_Trans.FindChild("#container_upgrade/grid_upgraderesources/#container_stone/#txt_stone").GetComponent<Text>();
            m_containerStone = m_Trans.FindChild("#container_upgrade/grid_upgraderesources/#container_stone").gameObject;
            m_txtWood = m_Trans.FindChild("#container_upgrade/grid_upgraderesources/#container_wood/#txt_wood").GetComponent<Text>();
            m_containerWood = m_Trans.FindChild("#container_upgrade/grid_upgraderesources/#container_wood").gameObject;
            m_containerUpgrade = m_Trans.FindChild("#container_upgrade").gameObject;
            m_containerUnlock = m_Trans.FindChild("#container_unlock").gameObject;

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
            m_txtHealth_num = null;
            m_gridInfo = null;
            m_containerBuildpoint = null;
            m_txtLevel = null;
            m_txtInfo = null;
            m_txtTime = null;
            m_btnUpgrade = null;
            m_txtDiamond = null;
            m_containerDiamond = null;
            m_btnInstant = null;
            m_txtIron = null;
            m_containerIron = null;
            m_txtStone = null;
            m_containerStone = null;
            m_txtWood = null;
            m_containerWood = null;
            m_containerUpgrade = null;
            m_containerUnlock = null;

		}
    }
}
