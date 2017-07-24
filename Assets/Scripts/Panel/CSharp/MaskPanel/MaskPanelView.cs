//==========================================
// Created By yingyugang At 1/28/2016 12:16:24 PM
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
    public class MaskPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Button m_btnMask;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_btnMask = m_Trans.Find("#btn_mask").GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_btnMask = null;
		}
    }
}
