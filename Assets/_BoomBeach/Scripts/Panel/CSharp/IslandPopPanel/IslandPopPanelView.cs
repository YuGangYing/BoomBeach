//==========================================
// Created By yingyugang At 2/21/2016 3:14:14 PM
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
    public class IslandPopPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public GameObject m_containerIslandpopbox;
        public GameObject m_containerIslandpopbox1;
        public GameObject m_containerIslandpopbox11;
        public GameObject m_containerIslandpopbox2;
        public GameObject m_containerIslandpopbox21;
        public GameObject m_containerIslandpopbox22;
        public GameObject m_containerIslandpopbox3;
        public GameObject m_containerIslandpopbox31;
        public GameObject m_containerIslandpopbox32;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_containerIslandpopbox = m_Trans.Find("#container_islandpopbox").gameObject;
            m_containerIslandpopbox1 = m_Trans.Find("#container_islandpopbox1").gameObject;
            m_containerIslandpopbox11 = m_Trans.Find("#container_islandpopbox11").gameObject;
            m_containerIslandpopbox2 = m_Trans.Find("#container_islandpopbox2").gameObject;
            m_containerIslandpopbox21 = m_Trans.Find("#container_islandpopbox21").gameObject;
            m_containerIslandpopbox22 = m_Trans.Find("#container_islandpopbox22").gameObject;
            m_containerIslandpopbox3 = m_Trans.Find("#container_islandpopbox3").gameObject;
            m_containerIslandpopbox31 = m_Trans.Find("#container_islandpopbox31").gameObject;
            m_containerIslandpopbox32 = m_Trans.Find("#container_islandpopbox32").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_containerIslandpopbox = null;
            m_containerIslandpopbox1 = null;
            m_containerIslandpopbox11 = null;
            m_containerIslandpopbox2 = null;
            m_containerIslandpopbox21 = null;
            m_containerIslandpopbox22 = null;
            m_containerIslandpopbox3 = null;
            m_containerIslandpopbox31 = null;
            m_containerIslandpopbox32 = null;

		}
    }
}
