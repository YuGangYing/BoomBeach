using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BoomBeach;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TroopTrainCtrl : BaseCtrl
{

    TroopTrainPanelView mTroopTrainPanelView;

    List<LayoutElement> mTroops;
    List<TrainTid> mTroopDatas;

    public void ShowTroop()
    {
		ShowPanel();
        BuildInfo buildInfo = Globals.currentTrainBuildInfo;
        if (buildInfo.troops_tid != null && buildInfo.troops_tid != "" && buildInfo.troops_num > 0)
        {
            ShowChangeTroop();
        }
        else
        {
            ShowTroopDatas();
        }
    }

    void ShowChangeTroop()
    {
        mTroopTrainPanelView.m_containerTroop.SetActive(false);
        mTroopTrainPanelView.m_containerChangetroop.SetActive(true);
        BuildInfo buildInfo = Globals.currentTrainBuildInfo;
        mTroopTrainPanelView.m_txtTitle.text = LocalizationCustom.instance.Get("TID_POPUP_CHANGE_TRAIN_TITLE");
        mTroopTrainPanelView.m_txtNum.text = buildInfo.troops_num.ToString();
        Sprite sprite = ResourceManager.GetInstance.atlas.avaterFullSpriteDic[buildInfo.troops_tid];
        mTroopTrainPanelView.m_imgBigicon.sprite = sprite;
        sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic[buildInfo.troops_tid];
        mTroopTrainPanelView.m_imgSmallicon.sprite = sprite;
    }

    void ShowTroopDatas()
    {
        mTroopTrainPanelView.m_containerTroop.SetActive(true);
        mTroopTrainPanelView.m_containerChangetroop.SetActive(false);
        BuildInfo buildInfo = Globals.currentTrainBuildInfo;
        Dictionary<string, TrainTid> trainList = Helper.getTrainTidList(buildInfo);
       
        mTroopTrainPanelView.m_txtTitle.text = LocalizationCustom.instance.Get("TID_POPUP_TRAIN_TITLE");
        mTroopDatas = new List<TrainTid>();
        int i = 0;
        foreach (TrainTid trainItem in trainList.Values)
        {
            LayoutElement c = mTroops[i];
            c.gameObject.SetActive(true);
            BindData(c,trainItem);
            mTroopDatas.Add(trainItem);
            i++;
        }

        for (int j = i; j < mTroops.Count; j++)
        {
            mTroops[j].gameObject.SetActive(false);
        }
    }

    public void BindData(LayoutElement ele,TrainTid tidData)
    {
        Transform trans = ele.transform;
        if (tidData.hasTrain == null && tidData.trainNum > 0)
        {
            //可以训练;
            
            trans.Find("container_info").gameObject.SetActive(true);
            trans.Find("container_info/txt_num").GetComponent<Text>().text = tidData.trainNum.ToString() + "X";
            trans.Find("txt_reason").gameObject.SetActive(false);
            Text goldText = trans.Find("container_info/txt_corn").GetComponent<Text>();
            //trans.Find("Reason").gameObject.SetActive(false);
            goldText.text = tidData.trainCost.ToString();
            if (tidData.trainCost > DataManager.GetInstance.model.user_info.gold_count)
            {
                goldText.color = Color.red;
            }
            else
            {
                goldText.color = Color.white;
            }
            trans.Find("container_info/txt_time").GetComponent<Text>().text = tidData.trainTimeFormat;

            Image[] images = ele.GetComponentsInChildren<Image>();
            Button btn = ele.GetComponent<Button>();
            for (int i = 0; i < images.Length; i++)
            {
                images[i].material = null;
                
            }
            btn.enabled = true;
            btn.onClick.AddListener(OnTrain);
        }
        else
        {
          
            if (!Helper.isUnLock(tidData.tid_level))
            {
                //灰;
                Image[] images = ele.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].material = ResourceManager.GetInstance.grayMat;

                }
            }
            else
            {
                //淡;
                Image[] images = ele.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].material = null;
                }
            }
            Button btn = ele.GetComponent<Button>();
            btn.enabled = true;
            trans.Find("container_info").gameObject.SetActive(false);
            trans.Find("txt_reason").gameObject.SetActive(true);
            trans.Find("txt_reason").GetComponent<Text>().text = tidData.hasTrain;
        }
        Sprite sprite = ResourceManager.GetInstance.atlas.avaterFullSpriteDic[tidData.tid];
        trans.Find("icon_head").GetComponent<Image>().sprite = sprite;
        Button infoBtn = trans.Find("icon_info").GetComponent<Button>(); 
        infoBtn.onClick.AddListener(OnShowTroopInfo);
    }

    void OnTrain()
    {
        GameObject troopItem = EventSystem.current.currentSelectedGameObject;
        int index = mTroops.IndexOf(troopItem.GetComponent<LayoutElement>());
       // BuildInfo.OnStartTrain(this.mTroopDatas[index].buildInfo, this.mTroopDatas[index].tid);
		TrainHandle.OnTrain (this.mTroopDatas[index].buildInfo, this.mTroopDatas[index].tid);
		Close();
		CloseMask ();
    }

    void OnShowTroopInfo()
    {
        GameObject selectObj = EventSystem.current.currentSelectedGameObject;
        LayoutElement ele = selectObj.transform.parent.GetComponent<LayoutElement>();
        int index = mTroops.IndexOf(ele);
        TrainTid trainTid = mTroopDatas[index];
		UIMgr.GetController<TroopDetailCtrl>().ShowBattle(trainTid.tid_level,this);
        Close();

    }

	public override void ShowPanel()
    {
        bool isCreate;
        mTroopTrainPanelView = UIMgr.ShowPanel<TroopTrainPanelView>(UIManager.UILayerType.Common, out isCreate);
        if (isCreate)
        {
            OnCreatePanel();
        }
		UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
    }

    void OnCreatePanel()
    {
        mTroops = new List<LayoutElement>();
        mTroopTrainPanelView.m_btnClose.onClick.AddListener(Close);
        mTroopTrainPanelView.m_btnClose.onClick.AddListener(CloseMask);
        mTroops.AddRange(mTroopTrainPanelView.m_containerTroop.GetComponentsInChildren<LayoutElement>());
        for (int i=0;i< mTroops.Count;i++)
        {
            mTroops[i].GetComponent<Button>().onClick.AddListener(OnTrain);
            mTroops[i].transform.Find("icon_info").GetComponent<Button>().onClick.AddListener(OnShowTroopInfo);
        }
        mTroopTrainPanelView.m_btnChangetroop.onClick.AddListener(ShowTroopDatas);
    }

    public override void Close()
    {
        UIMgr.ClosePanel("TroopTrainPanel");
    }

    public void CloseMask()
    {
		UIMgr.GetController<MaskCtrl>().Close();
    }

}
