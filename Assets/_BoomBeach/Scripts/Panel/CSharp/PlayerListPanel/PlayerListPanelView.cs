//==========================================
// Created By yingyugang At 1/29/2016 12:11:42 PM
//==========================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PlayerListPanel
{
    ///<summary>
    ///
    ///</summary>
    public class PlayerListPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Button m_btnClose;
        public Button m_btnPlayers;
        public Button m_btnFriends;
        public Transform m_gridPlayers;
        public GameObject m_containerPlayers;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_btnClose = m_Trans.Find("#btn_close").GetComponent<Button>();
            m_btnPlayers = m_Trans.Find("#btn_players").GetComponent<Button>();
            m_btnFriends = m_Trans.Find("#btn_friends").GetComponent<Button>();
            m_gridPlayers = m_Trans.Find("#container_players/#grid_players");
            m_containerPlayers = m_Trans.Find("#container_players").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_btnClose = null;
            m_btnPlayers = null;
            m_btnFriends = null;
            m_gridPlayers = null;
            m_containerPlayers = null;

		}
    }
}
