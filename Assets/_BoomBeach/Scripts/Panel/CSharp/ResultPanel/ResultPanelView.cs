//==========================================
// Created By yingyugang At 03/04/2016 21:12:51
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
    public class ResultPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public GameObject m_containerRewardmedal;
        public GameObject m_containerRewardgold;
        public GameObject m_containerRewardwood;
        public GameObject m_containerRewardstone;
        public GameObject m_containerRewardiron;
        public GameObject m_containerRewardpiece;
        public GameObject m_containerReward;
        public GameObject m_containerTroop;
        public Button m_btnReturn;
        public Text m_txtReward;
        public Text m_txtTroop;
        public Text m_txtBattlefail;
        public Text m_txtBattlewin;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_containerRewardmedal = m_Trans.FindChild("#container_reward/#container_rewardmedal").gameObject;
            m_containerRewardgold = m_Trans.FindChild("#container_reward/#container_rewardgold").gameObject;
            m_containerRewardwood = m_Trans.FindChild("#container_reward/#container_rewardwood").gameObject;
            m_containerRewardstone = m_Trans.FindChild("#container_reward/#container_rewardstone").gameObject;
            m_containerRewardiron = m_Trans.FindChild("#container_reward/#container_rewardiron").gameObject;
            m_containerRewardpiece = m_Trans.FindChild("#container_reward/#container_rewardpiece").gameObject;
            m_containerReward = m_Trans.FindChild("#container_reward").gameObject;
            m_containerTroop = m_Trans.FindChild("#container_troop").gameObject;
            m_btnReturn = m_Trans.FindChild("#btn_return").GetComponent<Button>();
            m_txtReward = m_Trans.FindChild("#txt_reward").GetComponent<Text>();
            m_txtTroop = m_Trans.FindChild("#txt_troop").GetComponent<Text>();
            m_txtBattlefail = m_Trans.FindChild("#txt_battlefail").GetComponent<Text>();
            m_txtBattlewin = m_Trans.FindChild("#txt_battlewin").GetComponent<Text>();

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_containerRewardmedal = null;
            m_containerRewardgold = null;
            m_containerRewardwood = null;
            m_containerRewardstone = null;
            m_containerRewardiron = null;
            m_containerRewardpiece = null;
            m_containerReward = null;
            m_containerTroop = null;
            m_btnReturn = null;
            m_txtReward = null;
            m_txtTroop = null;
            m_txtBattlefail = null;
            m_txtBattlewin = null;

		}
    }
}
