//==========================================
// Created By yingyugang At 03/10/2016 10:34:44
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
    public class LoginPanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public InputField m_inputAccountInputField;
        public InputField m_inputPasswordInputField;
        public GameObject m_containerDropdown;
        public Button m_btnLoginButton;
        public Button m_btnTrialButton;
        public Button m_btnRegisterButton;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_inputAccountInputField = m_Trans.Find("LoginWindow/#input_AccountInputField").GetComponent<InputField>();
            m_inputPasswordInputField = m_Trans.Find("LoginWindow/#input_PasswordInputField").GetComponent<InputField>();
            m_containerDropdown = m_Trans.Find("LoginWindow/#container_dropdown").gameObject;
            m_btnLoginButton = m_Trans.Find("LoginWindow/#btn_LoginButton").GetComponent<Button>();
            m_btnTrialButton = m_Trans.Find("LoginWindow/#btn_TrialButton").GetComponent<Button>();
            m_btnRegisterButton = m_Trans.Find("LoginWindow/#btn_RegisterButton").GetComponent<Button>();

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_inputAccountInputField = null;
            m_inputPasswordInputField = null;
            m_containerDropdown = null;
            m_btnLoginButton = null;
            m_btnTrialButton = null;
            m_btnRegisterButton = null;

		}
    }
}
