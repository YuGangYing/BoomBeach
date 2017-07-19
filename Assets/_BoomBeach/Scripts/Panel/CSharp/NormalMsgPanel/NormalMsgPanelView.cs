//==========================================
// Created By yingyugang At 2/13/2016 3:39:53 PM
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
    public class NormalMsgPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public GameObject m_containerPoppoint;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_containerPoppoint = m_Trans.Find("#container_poppoint").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_containerPoppoint = null;

		}
    }
}
