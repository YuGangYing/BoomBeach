using UnityEngine;
using System.Collections;
using BoomBeach;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArtifactCtrl : BaseCtrl
{

    ArtifactPanelView mArtifactPanelView;

    public List<Transform> items;

    //BuildInfo mBuildInfo;

    public void ShowArtifactPanel(BuildInfo buildInfo) {
		ShowPanel();
        //mBuildInfo = buildInfo;
        BindData(buildInfo);
        ShowGreen();
    }

	public override void ShowPanel()
    {
        bool isCreate;
        mArtifactPanelView = UIMgr.ShowPanel<ArtifactPanelView>(UIManager.UILayerType.Common, out isCreate);
        if (isCreate)
        {
            OnCreatePanel();
        }
		UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
    }

    void ShowGreen()
    {
        int common = DataManager.GetInstance().userInfo.common_piece;
        int rare = DataManager.GetInstance().userInfo.rare_piece;
        int epic = DataManager.GetInstance().userInfo.epic_piece;
        string a_type = "";
        mArtifactPanelView.m_btnGreen.transform.FindChild("Back").GetComponent<Image>().color = new Color(1,1, 1, 1);
        mArtifactPanelView.m_btnBlue.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnRed.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnDeepp.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        ShowArtiactSprite(a_type, common, rare, epic);
    }

    void ShowBlue()
    {
        int common = DataManager.GetInstance().userInfo.common_piece_ice;
        int rare = DataManager.GetInstance().userInfo.rare_piece_ice;
        int epic = DataManager.GetInstance().userInfo.epic_piece_ice;
        string a_type = "_ICE";
        mArtifactPanelView.m_btnGreen.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1); 
        mArtifactPanelView.m_btnBlue.transform.FindChild("Back").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        mArtifactPanelView.m_btnRed.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnDeepp.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        ShowArtiactSprite(a_type, common, rare, epic);
    }

    void ShowRed()
    {
        int common = DataManager.GetInstance().userInfo.common_piece_fire;
        int rare = DataManager.GetInstance().userInfo.rare_piece_fire;
        int epic = DataManager.GetInstance().userInfo.epic_piece_fire;
        string a_type = "_FIRE";
        mArtifactPanelView.m_btnGreen.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnBlue.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnRed.transform.FindChild("Back").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        mArtifactPanelView.m_btnDeepp.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        ShowArtiactSprite(a_type, common, rare, epic);
    }

    void ShowDark()
    {
        int common = DataManager.GetInstance().userInfo.common_piece_dark;
        int rare = DataManager.GetInstance().userInfo.rare_piece_dark;
        int epic = DataManager.GetInstance().userInfo.epic_piece_dark;
        string a_type = "_DARK";
        mArtifactPanelView.m_btnGreen.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnBlue.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnRed.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        mArtifactPanelView.m_btnDeepp.transform.FindChild("Back").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        ShowArtiactSprite(a_type, common, rare, epic);
    }

    void ShowArtiactSprite(string a_type,int common, int rare, int epic)
    {
        CsvInfo csvArtiact1 = (CsvInfo)CSVManager.GetInstance().csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT1" + a_type) + "_1"];
        items[0].FindChild("Button/img_icon").GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic[csvArtiact1.PieceResource];
        items[0].FindChild("Button/txt_count").GetComponent<Text>().color = common >= 7 ? Color.white : Color.red;

        CsvInfo csvArtiact2 = (CsvInfo)CSVManager.GetInstance().csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT2" + a_type) + "_1"];
        items[1].FindChild("Button/img_icon").GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic[csvArtiact2.PieceResource];
        items[1].FindChild("Button/txt_count").GetComponent<Text>().color = rare >= 7 ? Color.white : Color.red;

        CsvInfo csvArtiact3 = (CsvInfo)CSVManager.GetInstance().csvTable[Helper.BuildTIDToArtifactTID("TID_BUILDING_ARTIFACT3" + a_type) + "_1"];
        items[2].FindChild("Button/img_icon").GetComponent<Image>().sprite = ResourceManager.GetInstance().atlas.commonSpriteDic[csvArtiact3.PieceResource];
        items[2].FindChild("Button/txt_count").GetComponent<Text>().color = epic >= 7 ? Color.white : Color.red;
        Helper.CreateArtifactUI(items[0].Find("Button/itempoint"), "TID_BUILDING_ARTIFACT1" + a_type + "_1");
        Helper.CreateArtifactUI(items[1].Find("Button/itempoint"), "TID_BUILDING_ARTIFACT2" + a_type + "_1");
        Helper.CreateArtifactUI(items[2].Find("Button/itempoint"), "TID_BUILDING_ARTIFACT3" + a_type + "_1");
    }

    public void BindData(BuildInfo buildInfo)
    {
        int currentAmount = Helper.GetArtifactNum();
        if (buildInfo.hasStatue())
        {
            currentAmount += 1;
        }
        mArtifactPanelView.m_txtTotalinfo.text = StringFormat.FormatByTid("TID_ARTIFACT_CAPACITY_LIMIT", new object[] { currentAmount, buildInfo.csvInfo.ArtifactCapacity });
        mArtifactPanelView.m_gridUsercrystal.GetChild(0).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.common_piece.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(1).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.rare_piece.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(2).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.epic_piece.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(3).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.common_piece_ice.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(4).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.rare_piece_ice.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(5).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.epic_piece_ice.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(6).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.common_piece_fire.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(7).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.rare_piece_fire.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(8).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.epic_piece_fire.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(9).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.common_piece_dark.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(10).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.rare_piece_dark.ToString();
        mArtifactPanelView.m_gridUsercrystal.GetChild(11).FindChild("txt_num").GetComponent<Text>().text = DataManager.GetInstance().userInfo.epic_piece_dark.ToString();
    }


    void OnCreatePanel()
    {
        mArtifactPanelView.m_btnClose.onClick.AddListener(Close);
        mArtifactPanelView.m_btnClose.onClick.AddListener(CloseMask);

        mArtifactPanelView.m_btnGreen.onClick.AddListener(ShowGreen);
        mArtifactPanelView.m_btnBlue.onClick.AddListener(ShowBlue);
        mArtifactPanelView.m_btnRed.onClick.AddListener(ShowRed);
        mArtifactPanelView.m_btnDeepp.onClick.AddListener(ShowDark);
        items = new List<Transform>();
        for (int i=0;i< mArtifactPanelView.m_gridInfo.transform.childCount;i++) {
            items.Add(mArtifactPanelView.m_gridInfo.transform.GetChild(i));
        }
    }

    public override void Close()
    {
        UIMgr.ClosePanel("ArtifactPanel");
    }

    public void CloseMask()
    {
		UIMgr.GetController<MaskCtrl>().Close();
    }
}
