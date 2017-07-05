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
    public class BattleInterfacePanelView : PanelBase
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////
        public Transform m_Trans;
        public Text m_txtGoldnum;
        public Text m_txtGoldnumadd;
        public Text m_txtWoodnum;
        public Text m_txtWoodnumadd;
        public Text m_txtStonenum;
        public Text m_txtStonenumadd;
        public Text m_txtIronnum;
        public Text m_txtIronnumadd;
        public Text m_txtMedalnum;
        public Image m_imgSmall;
        public Image m_imgMiddle;
        public Image m_imgBig;
        public Text m_txtMedalnumadd;
        public Text m_txtEnergy;
        public GameObject m_containerEnergy;
        public Image m_imgSelectweapon;
        public Image m_imgSelecttroop;
        public GameObject m_containerTroops;
        public GameObject m_containerViewtroops;
        public GameObject m_containerBattleview;
        public GameObject m_containerWeapons;
        public Text m_txtName;
        public Image m_imgLevelbg;
        public Image m_imgLevelbar;
        public Text m_txtLevel;
        public GameObject m_containerLevel;
        public GameObject m_containerInfo;
        public Button m_btnEnd;
        public Text m_txtAttackgold;
        public Button m_btnBattle;
        public Button m_btnMap;
        public Button m_btnIronfill;
        public Text m_txtIron;
        public Slider m_sliderIron;
        public Button m_btnStonefill;
        public Text m_txtStone;
        public Slider m_sliderStone;
        public Button m_btnWoodfill;
        public Text m_txtWood;
        public Slider m_sliderWood;
        public Button m_btnGoldfill;
        public Text m_txtGold;
        public Slider m_sliderGold;
        public GameObject m_containerResourcebar;
        public Text m_txtDiamond;
        public GameObject m_containerDiamondbar;


        // Use this for initialization
        public override void Awake()
        {
            m_Trans = transform;
            m_txtGoldnum = m_Trans.FindChild("container_resources/item/#txt_goldnum").GetComponent<Text>();
            m_txtGoldnumadd = m_Trans.FindChild("container_resources/item/#txt_goldnumadd").GetComponent<Text>();
            m_txtWoodnum = m_Trans.FindChild("container_resources/item (1)/#txt_woodnum").GetComponent<Text>();
            m_txtWoodnumadd = m_Trans.FindChild("container_resources/item (1)/#txt_woodnumadd").GetComponent<Text>();
            m_txtStonenum = m_Trans.FindChild("container_resources/item (2)/#txt_stonenum").GetComponent<Text>();
            m_txtStonenumadd = m_Trans.FindChild("container_resources/item (2)/#txt_stonenumadd").GetComponent<Text>();
            m_txtIronnum = m_Trans.FindChild("container_resources/item (3)/#txt_ironnum").GetComponent<Text>();
            m_txtIronnumadd = m_Trans.FindChild("container_resources/item (3)/#txt_ironnumadd").GetComponent<Text>();
            m_txtMedalnum = m_Trans.FindChild("container_resources/item (4)/#txt_medalnum").GetComponent<Text>();
            m_imgSmall = m_Trans.FindChild("container_chance/item/#img_small").GetComponent<Image>();
            m_imgMiddle = m_Trans.FindChild("container_chance/item/#img_middle").GetComponent<Image>();
            m_imgBig = m_Trans.FindChild("container_chance/item/#img_big").GetComponent<Image>();
            m_txtMedalnumadd = m_Trans.FindChild("container_chance/item (1)/#txt_medalnumadd").GetComponent<Text>();
            m_txtEnergy = m_Trans.FindChild("#container_energy/#txt_energy").GetComponent<Text>();
            m_containerEnergy = m_Trans.FindChild("#container_energy").gameObject;
            m_imgSelectweapon = m_Trans.FindChild("#img_selectweapon").GetComponent<Image>();
            m_imgSelecttroop = m_Trans.FindChild("#img_selecttroop").GetComponent<Image>();
            m_containerTroops = m_Trans.FindChild("#container_troops").gameObject;
            m_containerViewtroops = m_Trans.FindChild("#container_battleview/#container_viewtroops").gameObject;
            m_containerBattleview = m_Trans.FindChild("#container_battleview").gameObject;
            m_containerWeapons = m_Trans.FindChild("#container_weapons").gameObject;
            m_txtName = m_Trans.FindChild("#container_info/#txt_name").GetComponent<Text>();
            m_imgLevelbg = m_Trans.FindChild("#container_info/#container_level/#img_levelbg").GetComponent<Image>();
            m_imgLevelbar = m_Trans.FindChild("#container_info/#container_level/#img_levelbar").GetComponent<Image>();
            m_txtLevel = m_Trans.FindChild("#container_info/#container_level/#txt_level").GetComponent<Text>();
            m_containerLevel = m_Trans.FindChild("#container_info/#container_level").gameObject;
            m_containerInfo = m_Trans.FindChild("#container_info").gameObject;
            m_btnEnd = m_Trans.FindChild("#btn_end").GetComponent<Button>();
            m_txtAttackgold = m_Trans.FindChild("#btn_battle/#txt_attackgold").GetComponent<Text>();
            m_btnBattle = m_Trans.FindChild("#btn_battle").GetComponent<Button>();
            m_btnMap = m_Trans.FindChild("#btn_map").GetComponent<Button>();
            m_btnIronfill = m_Trans.FindChild("#container_resourcebar/#slider_iron/Fill Area/#btn_ironfill").GetComponent<Button>();
            m_txtIron = m_Trans.FindChild("#container_resourcebar/#slider_iron/#txt_iron").GetComponent<Text>();
            m_sliderIron = m_Trans.FindChild("#container_resourcebar/#slider_iron").GetComponent<Slider>();
            m_btnStonefill = m_Trans.FindChild("#container_resourcebar/#slider_stone/Fill Area/#btn_stonefill").GetComponent<Button>();
            m_txtStone = m_Trans.FindChild("#container_resourcebar/#slider_stone/#txt_stone").GetComponent<Text>();
            m_sliderStone = m_Trans.FindChild("#container_resourcebar/#slider_stone").GetComponent<Slider>();
            m_btnWoodfill = m_Trans.FindChild("#container_resourcebar/#slider_wood/Fill Area/#btn_woodfill").GetComponent<Button>();
            m_txtWood = m_Trans.FindChild("#container_resourcebar/#slider_wood/#txt_wood").GetComponent<Text>();
            m_sliderWood = m_Trans.FindChild("#container_resourcebar/#slider_wood").GetComponent<Slider>();
            m_btnGoldfill = m_Trans.FindChild("#container_resourcebar/#slider_gold/Fill Area/#btn_goldfill").GetComponent<Button>();
            m_txtGold = m_Trans.FindChild("#container_resourcebar/#slider_gold/#txt_gold").GetComponent<Text>();
            m_sliderGold = m_Trans.FindChild("#container_resourcebar/#slider_gold").GetComponent<Slider>();
            m_containerResourcebar = m_Trans.FindChild("#container_resourcebar").gameObject;
            m_txtDiamond = m_Trans.FindChild("#container_diamondbar/#txt_diamond").GetComponent<Text>();
            m_containerDiamondbar = m_Trans.FindChild("#container_diamondbar").gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

		void OnDestroy()
		{
            m_txtGoldnum = null;
            m_txtGoldnumadd = null;
            m_txtWoodnum = null;
            m_txtWoodnumadd = null;
            m_txtStonenum = null;
            m_txtStonenumadd = null;
            m_txtIronnum = null;
            m_txtIronnumadd = null;
            m_txtMedalnum = null;
            m_imgSmall = null;
            m_imgMiddle = null;
            m_imgBig = null;
            m_txtMedalnumadd = null;
            m_txtEnergy = null;
            m_containerEnergy = null;
            m_imgSelectweapon = null;
            m_imgSelecttroop = null;
            m_containerTroops = null;
            m_containerViewtroops = null;
            m_containerBattleview = null;
            m_containerWeapons = null;
            m_txtName = null;
            m_imgLevelbg = null;
            m_imgLevelbar = null;
            m_txtLevel = null;
            m_containerLevel = null;
            m_containerInfo = null;
            m_btnEnd = null;
            m_txtAttackgold = null;
            m_btnBattle = null;
            m_btnMap = null;
            m_btnIronfill = null;
            m_txtIron = null;
            m_sliderIron = null;
            m_btnStonefill = null;
            m_txtStone = null;
            m_sliderStone = null;
            m_btnWoodfill = null;
            m_txtWood = null;
            m_sliderWood = null;
            m_btnGoldfill = null;
            m_txtGold = null;
            m_sliderGold = null;
            m_containerResourcebar = null;
            m_txtDiamond = null;
            m_containerDiamondbar = null;

		}
    }
}
