//==========================================
// Created By yingyugang At 2/21/2016 11:18:59 AM
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
    public class MainInterfacePanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtName;
        public Image m_imgLevelbg;
        public Image m_imgLevelbar;
        public Text m_txtLevel;
        public GameObject m_containerLevel;
        public Image m_imgMedalbg;
        public Image m_imgMedal;
        public Text m_txtMedal;
        public GameObject m_containerMedal;
        public GameObject m_containerInfo;
        public Text m_txtBuildnum;
        public GameObject m_containerBuildnum;
        public Button m_btnShop;
        public Button m_btnMap;
        public Button m_btnIronback;
        public Button m_btnIronfill;
        public Text m_txtIron;
        public Button m_btnIron;
        public Slider m_sliderIron;
        public Button m_btnStoneback;
        public Button m_btnStonefill;
        public Text m_txtStone;
        public Button m_btnStone;
        public Slider m_sliderStone;
        public Button m_btnWoodback;
        public Button m_btnWoodfill;
        public Text m_txtWood;
        public Button m_btnWood;
        public Slider m_sliderWood;
        public Button m_btnGoldback;
        public Button m_btnGoldfill;
        public Text m_txtGold;
        public Button m_btnGold;
        public Slider m_sliderGold;
        public GameObject m_containerResourcebar;
        public Image m_imgDiamondbg;
        public Button m_btnAdddiamond;
        public Image m_imgDiamond;
        public Text m_txtDiamond;
        public GameObject m_containerDiamondbar;
        public Button m_btnAchivement;
        public Button m_btnFriend;
        public Button m_btnTeam;
        public Image m_imgSetting;
        public Button m_btnSetting;
        public Image m_imgMail;
        public Button m_btnMail;
        public GameObject m_containerMainbtns;
        public Button m_btnChat;
        public Button m_btnTeambattle;
        public Text m_txtLevelpopboxtitle;
        public Text m_txtProgress_to_next_level;
        public GameObject m_containerLevelpopbox;
        public Text m_txtMedalpopboxtitle;
        public GameObject m_containerMedalpopbox;
        public Text m_txtTitle;
        public Text m_txtProduction_per_hour;
        public Text m_txtFrom_base;
        public Text m_txtFrom_freed_villages;
        public Text m_txtStorage_capacity;
        public Text m_txtProtected_by_vault;
        public GameObject m_containerResourcepopbox;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtName = m_Trans.Find("#container_info/#txt_name").GetComponent<Text>();
            m_imgLevelbg = m_Trans.Find("#container_info/#container_level/#img_levelbg").GetComponent<Image>();
            m_imgLevelbar = m_Trans.Find("#container_info/#container_level/#img_levelbar").GetComponent<Image>();
            m_txtLevel = m_Trans.Find("#container_info/#container_level/#txt_level").GetComponent<Text>();
            m_containerLevel = m_Trans.Find("#container_info/#container_level").gameObject;
            m_imgMedalbg = m_Trans.Find("#container_info/#container_medal/#img_medalbg").GetComponent<Image>();
            m_imgMedal = m_Trans.Find("#container_info/#container_medal/#img_medal").GetComponent<Image>();
            m_txtMedal = m_Trans.Find("#container_info/#container_medal/#txt_medal").GetComponent<Text>();
            m_containerMedal = m_Trans.Find("#container_info/#container_medal").gameObject;
            m_containerInfo = m_Trans.Find("#container_info").gameObject;
            m_txtBuildnum = m_Trans.Find("#btn_shop/#container_buildnum/#txt_buildnum").GetComponent<Text>();
            m_containerBuildnum = m_Trans.Find("#btn_shop/#container_buildnum").gameObject;
            m_btnShop = m_Trans.Find("#btn_shop").GetComponent<Button>();
            m_btnMap = m_Trans.Find("#btn_map").GetComponent<Button>();
            m_btnIronback = m_Trans.Find("#container_resourcebar/#slider_iron/#btn_ironback").GetComponent<Button>();
            m_btnIronfill = m_Trans.Find("#container_resourcebar/#slider_iron/Fill Area/#btn_ironfill").GetComponent<Button>();
            m_txtIron = m_Trans.Find("#container_resourcebar/#slider_iron/#txt_iron").GetComponent<Text>();
            m_btnIron = m_Trans.Find("#container_resourcebar/#slider_iron/#btn_iron").GetComponent<Button>();
            m_sliderIron = m_Trans.Find("#container_resourcebar/#slider_iron").GetComponent<Slider>();
            m_btnStoneback = m_Trans.Find("#container_resourcebar/#slider_stone/#btn_stoneback").GetComponent<Button>();
            m_btnStonefill = m_Trans.Find("#container_resourcebar/#slider_stone/Fill Area/#btn_stonefill").GetComponent<Button>();
            m_txtStone = m_Trans.Find("#container_resourcebar/#slider_stone/#txt_stone").GetComponent<Text>();
            m_btnStone = m_Trans.Find("#container_resourcebar/#slider_stone/#btn_stone").GetComponent<Button>();
            m_sliderStone = m_Trans.Find("#container_resourcebar/#slider_stone").GetComponent<Slider>();
            m_btnWoodback = m_Trans.Find("#container_resourcebar/#slider_wood/#btn_woodback").GetComponent<Button>();
            m_btnWoodfill = m_Trans.Find("#container_resourcebar/#slider_wood/Fill Area/#btn_woodfill").GetComponent<Button>();
            m_txtWood = m_Trans.Find("#container_resourcebar/#slider_wood/#txt_wood").GetComponent<Text>();
            m_btnWood = m_Trans.Find("#container_resourcebar/#slider_wood/#btn_wood").GetComponent<Button>();
            m_sliderWood = m_Trans.Find("#container_resourcebar/#slider_wood").GetComponent<Slider>();
            m_btnGoldback = m_Trans.Find("#container_resourcebar/#slider_gold/#btn_goldback").GetComponent<Button>();
            m_btnGoldfill = m_Trans.Find("#container_resourcebar/#slider_gold/Fill Area/#btn_goldfill").GetComponent<Button>();
            m_txtGold = m_Trans.Find("#container_resourcebar/#slider_gold/#txt_gold").GetComponent<Text>();
            m_btnGold = m_Trans.Find("#container_resourcebar/#slider_gold/#btn_gold").GetComponent<Button>();
            m_sliderGold = m_Trans.Find("#container_resourcebar/#slider_gold").GetComponent<Slider>();
            m_containerResourcebar = m_Trans.Find("#container_resourcebar").gameObject;
            m_imgDiamondbg = m_Trans.Find("#container_diamondbar/#img_diamondbg").GetComponent<Image>();
            m_btnAdddiamond = m_Trans.Find("#container_diamondbar/#btn_adddiamond").GetComponent<Button>();
            m_imgDiamond = m_Trans.Find("#container_diamondbar/#img_diamond").GetComponent<Image>();
            m_txtDiamond = m_Trans.Find("#container_diamondbar/#txt_diamond").GetComponent<Text>();
            m_containerDiamondbar = m_Trans.Find("#container_diamondbar").gameObject;
            m_btnAchivement = m_Trans.Find("#container_mainbtns/#btn_achivement").GetComponent<Button>();
            m_btnFriend = m_Trans.Find("#container_mainbtns/#btn_friend").GetComponent<Button>();
            m_btnTeam = m_Trans.Find("#container_mainbtns/#btn_team").GetComponent<Button>();
            m_imgSetting = m_Trans.Find("#container_mainbtns/#btn_setting/#img_setting").GetComponent<Image>();
            m_btnSetting = m_Trans.Find("#container_mainbtns/#btn_setting").GetComponent<Button>();
            m_imgMail = m_Trans.Find("#container_mainbtns/#btn_mail/#img_mail").GetComponent<Image>();
            m_btnMail = m_Trans.Find("#container_mainbtns/#btn_mail").GetComponent<Button>();
            m_containerMainbtns = m_Trans.Find("#container_mainbtns").gameObject;
            m_btnChat = m_Trans.Find("#btn_chat").GetComponent<Button>();
            m_btnTeambattle = m_Trans.Find("#btn_teambattle").GetComponent<Button>();
            m_txtLevelpopboxtitle = m_Trans.Find("#container_levelpopbox/#txt_levelpopboxtitle").GetComponent<Text>();
            m_txtProgress_to_next_level = m_Trans.Find("#container_levelpopbox/#txt_progress_to_next_level").GetComponent<Text>();
            m_containerLevelpopbox = m_Trans.Find("#container_levelpopbox").gameObject;
            m_txtMedalpopboxtitle = m_Trans.Find("#container_medalpopbox/#txt_medalpopboxtitle").GetComponent<Text>();
            m_containerMedalpopbox = m_Trans.Find("#container_medalpopbox").gameObject;
            m_txtTitle = m_Trans.Find("#container_resourcepopbox/#txt_title").GetComponent<Text>();
            m_txtProduction_per_hour = m_Trans.Find("#container_resourcepopbox/#txt_production_per_hour").GetComponent<Text>();
            m_txtFrom_base = m_Trans.Find("#container_resourcepopbox/#txt_from_base").GetComponent<Text>();
            m_txtFrom_freed_villages = m_Trans.Find("#container_resourcepopbox/#txt_from_freed_villages").GetComponent<Text>();
            m_txtStorage_capacity = m_Trans.Find("#container_resourcepopbox/#txt_storage_capacity").GetComponent<Text>();
            m_txtProtected_by_vault = m_Trans.Find("#container_resourcepopbox/#txt_protected_by_vault").GetComponent<Text>();
            m_containerResourcepopbox = m_Trans.Find("#container_resourcepopbox").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_txtName = null;
            m_imgLevelbg = null;
            m_imgLevelbar = null;
            m_txtLevel = null;
            m_containerLevel = null;
            m_imgMedalbg = null;
            m_imgMedal = null;
            m_txtMedal = null;
            m_containerMedal = null;
            m_containerInfo = null;
            m_txtBuildnum = null;
            m_containerBuildnum = null;
            m_btnShop = null;
            m_btnMap = null;
            m_btnIronback = null;
            m_btnIronfill = null;
            m_txtIron = null;
            m_btnIron = null;
            m_sliderIron = null;
            m_btnStoneback = null;
            m_btnStonefill = null;
            m_txtStone = null;
            m_btnStone = null;
            m_sliderStone = null;
            m_btnWoodback = null;
            m_btnWoodfill = null;
            m_txtWood = null;
            m_btnWood = null;
            m_sliderWood = null;
            m_btnGoldback = null;
            m_btnGoldfill = null;
            m_txtGold = null;
            m_btnGold = null;
            m_sliderGold = null;
            m_containerResourcebar = null;
            m_imgDiamondbg = null;
            m_btnAdddiamond = null;
            m_imgDiamond = null;
            m_txtDiamond = null;
            m_containerDiamondbar = null;
            m_btnAchivement = null;
            m_btnFriend = null;
            m_btnTeam = null;
            m_imgSetting = null;
            m_btnSetting = null;
            m_imgMail = null;
            m_btnMail = null;
            m_containerMainbtns = null;
            m_btnChat = null;
            m_btnTeambattle = null;
            m_txtLevelpopboxtitle = null;
            m_txtProgress_to_next_level = null;
            m_containerLevelpopbox = null;
            m_txtMedalpopboxtitle = null;
            m_containerMedalpopbox = null;
            m_txtTitle = null;
            m_txtProduction_per_hour = null;
            m_txtFrom_base = null;
            m_txtFrom_freed_villages = null;
            m_txtStorage_capacity = null;
            m_txtProtected_by_vault = null;
            m_containerResourcepopbox = null;

		}
    }
}
