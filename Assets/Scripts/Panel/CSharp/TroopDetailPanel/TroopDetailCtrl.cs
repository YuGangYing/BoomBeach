using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BoomBeach;
using System.Collections.Generic;

public class TroopDetailCtrl : BaseCtrl
{

    TroopDetailPanelView mTroopDetailPanelView;
    LayoutElement[] mTroops;
    BaseCtrl mPreCtrl;
    string mTid_level;
    public void ShowBattle(string tid_level, BaseCtrl preCtrl = null)
    {
		ShowPanel();
        BindData(tid_level, false);
        mTid_level = tid_level;
        mPreCtrl = preCtrl;
    }

    public void ShowUpgradePanel(ResearchTid tidData, BaseCtrl preCtrl = null)
    {
		ShowPanel();
        BindData(tidData.tid_level, true);
        BindUpgradeData(tidData);
        mTid_level = tidData.tid_level;
        mPreCtrl = preCtrl;
    }

	public override void ShowPanel()
    {
        bool isCreate;
        mTroopDetailPanelView = UIMgr.ShowPanel<TroopDetailPanelView>(UIManager.UILayerType.Common, out isCreate);
        if (isCreate)
        {
            OnCreatePanel();
        }
		UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
    }

    void OnCreatePanel()
    {
        mTroops = mTroopDetailPanelView.m_gridInfo.GetComponentsInChildren<LayoutElement>(true);
        mTroopDetailPanelView.m_btnBack.onClick.AddListener(Back);
        mTroopDetailPanelView.m_btnClose.onClick.AddListener(Close);
        mTroopDetailPanelView.m_btnClose.onClick.AddListener(CloseMask);

        mTroopDetailPanelView.m_btnInstant.onClick.AddListener(OnInstant);
        mTroopDetailPanelView.m_btnUpgrade.onClick.AddListener(OnUpgrade);
    }

    void BindUpgradeData(ResearchTid tidData)
    {
        mTroopDetailPanelView.m_txtInfotitle.text = "";
        mTroopDetailPanelView.m_txtInfo.text = "";

        mTroopDetailPanelView.m_containerUpgrade.SetActive(true);
		mTroopDetailPanelView.m_txtDiamond.text = Helper.GetUpgradeInstant (tidData.tid_level).ToString();
        mTroopDetailPanelView.m_txtGold.text = tidData.upgradeCost.ToString();
        if (tidData.upgradeCost > DataManager.GetInstance.model.user_info.gold_count)
        {
            mTroopDetailPanelView.m_txtGold.color = Color.red;
        }
        else
        {
            mTroopDetailPanelView.m_txtGold.color = Color.black;
        }
        mTroopDetailPanelView.m_txtTime.text = Helper.GetFormatTime(tidData.upgradeTime, 0);
    }

    public void BindData(string tid_level,bool isUpgrade)
    {

        CsvInfo csvInfo = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;
        string ShowName = StringFormat.FormatByTid(csvInfo.TID);
        string ShowLevel = StringFormat.FormatByTid("TID_LEVEL_NUM", new string[] { csvInfo.Level.ToString() });
        string Description = StringFormat.FormatByTid(csvInfo.InfoTID);

        //UpgradeTitle.gameObject.SetActive(false);
        mTroopDetailPanelView.m_txtTitle.text = ShowName;
        mTroopDetailPanelView.m_txtLevel.text = ShowLevel;
        //LevelLabel.gameObject.SetActive(true);
        //LevelLabelLabel.text = ShowLevel;
        mTroopDetailPanelView.m_txtInfo.text = Description;
        //DescriptionLabelLabel.text = Description;

        Dictionary<string, PropertyInfoNew> propertyList = Helper.getPropertyList(tid_level, isUpgrade, null);
        
        bool isShowAddedTitle = false;
        for (int j=0; j< mTroops.Length;j++)
        {
            mTroops[j].gameObject.SetActive(false);
        }
        int i = 0;
        foreach (PropertyInfoNew info in propertyList.Values)
        {
            GameObject propertyItem = mTroops[i].gameObject;
            propertyItem.SetActive(true);
            propertyItem.transform.Find("txt_name").GetComponent<Text>().text = info.showName;
            Sprite sprite;
            if (info.spriteName!=null && ResourceManager.GetInstance.atlas.commonSpriteDic.TryGetValue(info.spriteName, out sprite))
            {
                propertyItem.transform.Find("img").gameObject.SetActive(true);
                propertyItem.transform.Find("img").GetComponent<Image>().sprite = sprite;
            }
            else
            {
                propertyItem.transform.Find("img").gameObject.SetActive(false);
            }
            propertyItem.transform.Find("txt_num").GetComponent<Text>().text = info.value;
           // propertyItem.transform.Find("Added").gameObject.SetActive(true);
           // propertyItem.transform.Find("Upgrade").gameObject.SetActive(false);
            propertyItem.transform.Find("txt_add_num").GetComponent<Text>().text = info.bonus_value;
            if (!isShowAddedTitle && info.bonus_value != "" && info.bonus_value != null)
            {
                isShowAddedTitle = true;
            }
            propertyItem.transform.name = info.name;
            if (i == 0) propertyItem.transform.localPosition = Vector3.zero;

            i++;

        }
       
        if (csvInfo.TID_Type == "CHARACTERS")
        {
            mTroopDetailPanelView.m_imgBigicon.sprite = ResourceManager.GetInstance.atlas.avaterFullSpriteDic[csvInfo.TID];
            mTroopDetailPanelView.m_imgBigicon.transform.localScale = Vector3.one;
        }
        else
        {
            mTroopDetailPanelView.m_imgBigicon.sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic [csvInfo.TID];
            mTroopDetailPanelView.m_imgBigicon.transform.localScale = Vector3.one ;
        }
        mTroopDetailPanelView.m_containerUpgrade.SetActive(false);
        /**
        if (csvInfo.TID_Type == "CHARACTERS")
        {
            TrooperAvatarSprite.atlas = PartEmitObj.Instance.avatarFullAtlas;
            TrooperAvatarSprite.spriteName = csvInfo.TID;
            TrooperAvatarSprite.MakePixelPerfect();
            TroopAvatar.localScale = Vector3.one;
        }
        else
        {
            TrooperAvatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
            TrooperAvatarSprite.spriteName = csvInfo.TID;
            TrooperAvatarSprite.MakePixelPerfect();
            TroopAvatar.localScale = Vector3.one * 2f;
        }
    **/


    }

    public void OnInstant()
    {
        Debug.Log("OnInstant");
        //实验室只有一个;
        //BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_LABORATORY");
        //BuildInfo.FinishBuildToServer(buildInfo, 0, 0, true, mTid_level);
		ResearchHandle.OnImmediateResearch (mTid_level);
        Close();
        CloseMask();
    }
    public void OnUpgrade()
    {
        Debug.Log("OnUpgrade");
        //实验室只有一个;
        BuildInfo buildInfo = Helper.getBuildInfoByTid("TID_BUILDING_LABORATORY");
        //buildInfo.OnUpgrade(buildInfo, mTid_level);
		//buildInfo.OnUpgradeTech(buildInfo,mTid_level);
		ResearchHandle.OnResearch (buildInfo,mTid_level);
        Close();
        CloseMask();
    }


    public override void Back()
    {
        Close();
        if (mPreCtrl != null)
			mPreCtrl.ShowPanel();
    }

    public override void Close()
    {
        UIMgr.ClosePanel("TroopDetailPanel");
    }

    public void CloseMask()
    {
		UIMgr.GetController<MaskCtrl>().Close();
    }

}
