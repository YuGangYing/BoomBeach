//==========================================
// Created By yingyugang At 2/5/2016 4:52:44 PM
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
    public class TroopTrainPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtTitle;
        public Button m_btnClose;
        public Button m_btnBack;
        public Transform m_gridArmy;
        public GameObject m_containerTroop;
        public Image m_imgBigicon;
        public Image m_imgSmallicon;
        public Text m_txtNum;
        public Button m_btnChangetroop;
        public GameObject m_containerChangetroop;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtTitle = m_Trans.FindChild("#txt_title").GetComponent<Text>();
            m_btnClose = m_Trans.FindChild("#btn_close").GetComponent<Button>();
            m_btnBack = m_Trans.FindChild("#btn_back").GetComponent<Button>();
            m_gridArmy = m_Trans.FindChild("#container_troop/#grid_army");
            m_containerTroop = m_Trans.FindChild("#container_troop").gameObject;
            m_imgBigicon = m_Trans.FindChild("#container_changetroop/#img_bigicon").GetComponent<Image>();
            m_imgSmallicon = m_Trans.FindChild("#container_changetroop/Image/#img_smallicon").GetComponent<Image>();
            m_txtNum = m_Trans.FindChild("#container_changetroop/Image/#txt_num").GetComponent<Text>();
            m_btnChangetroop = m_Trans.FindChild("#container_changetroop/#btn_changetroop").GetComponent<Button>();
            m_containerChangetroop = m_Trans.FindChild("#container_changetroop").gameObject;

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
            m_imgBigicon = null;
            m_imgSmallicon = null;
            m_txtNum = null;
            m_btnChangetroop = null;
            m_containerChangetroop = null;

		}
    }
}
