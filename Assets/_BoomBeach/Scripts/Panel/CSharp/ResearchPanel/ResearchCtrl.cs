using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoomBeach
{
    public class ResearchCtrl : BaseCtrl
    {

        ResearchPanelView mResearchPanelView;

        List<Transform> items;
        List<ResearchTid> researchList;

		public override void ShowPanel()
        {
            bool isCreate;
            mResearchPanelView = UIMgr.ShowPanel<ResearchPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
			UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
            BindDatas();
        }

        void BindDatas()
        {
            mResearchPanelView.m_txtTitle.text = LocalizationCustom.instance.Get("TID_POPUP_UPGRADE_TROOP_TITLE");
            researchList = Helper.getResearchTidList();
            Transform container = mResearchPanelView.m_gridArmy;
            int i = 0;
            foreach (Transform researchItembox in container)
            {
                if (i < researchList.Count)
                {
                    researchItembox.gameObject.SetActive(true);
                    ResearchTid tid = researchList[i];
                    BindData(researchItembox,tid);
                    //researchItembox.GetComponent<ResearchItem>().BindData(tid);
                }
                else
                {
                    researchItembox.gameObject.SetActive(false);
                }
                i++;
            }
        }

        void BindData(Transform ele, ResearchTid tid)
        {
            
            //this.TidData = tidData;
            if (tid.hasUpgrade != null)
            {
                //if (!Helper.isUnLock(tid.tid_level))
               // {
                    //灰;
                    //transform.Find("bg").GetComponent<UISprite>().color = Color.black;
                    //transform.Find("avatar").GetComponent<UISprite>().color = Color.black;

                    Image[] images = ele.GetComponentsInChildren<Image>();
                    for (int i = 0; i < images.Length; i++)
                    {
                        if(images[i].name != "icon_info")
                            images[i].material = ResourceManager.GetInstance.grayMat;
                        //images[i].transform.localPosition = new Vector3(images[i].transform.localPosition.x, images[i].transform.localPosition.y, 0.1f);
                    }
                //}
                //else
                //{
                    //淡色;
                    //transform.Find("bg").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.3f);
                    //transform.Find("avatar").GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.7f);
                 //   Image[] images = ele.GetComponentsInChildren<Image>();
                 //   for (int i = 0; i < images.Length; i++)
                  //  {
                  //      images[i].material = null;
                        //images[i].transform.localPosition = new Vector3(images[i].transform.localPosition.x, images[i].transform.localPosition.y, 0.1f);
                   // }
                //}
                ele.GetComponent<Button>().enabled = false;
                //GetComponent<UIButton>().enabled = false;
                ele.Find("txt_corn").gameObject.SetActive(false);
                ele.Find("icon_corn").gameObject.SetActive(false);
                ele.Find("txt_reason").gameObject.SetActive(true);
                ele.Find("txt_reason").GetComponent<Text>().text = tid.hasUpgrade;
                //transform.Find("price").gameObject.SetActive(false);
                //transform.Find("tip").gameObject.SetActive(true);
                //transform.Find("tip").GetComponent<UILabel>().text = tid.hasUpgrade;
               // transform.Find("Sprite").gameObject.SetActive(false);

                //UIButton btn = transform.GetComponent<UIButton>();
               // btn.onClick = new List<EventDelegate>();

            }
            else
            {
                //可以升级;
                Button btn = ele.GetComponent<Button>();
                btn.enabled = true;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(Upgrade);
                ele.Find("txt_corn").gameObject.SetActive(true);
                ele.Find("icon_corn").gameObject.SetActive(true);
                ele.Find("txt_reason").gameObject.SetActive(false);
                ele.Find("txt_corn").GetComponent<Text>().text = tid.upgradeCost.ToString();
                Image[] images = ele.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].material = null;
                }

                /**
                UIButton btn = transform.GetComponent<UIButton>();
                btn.enabled = true;
                btn.onClick = new List<EventDelegate>();
                btn.onClick.Add(new EventDelegate(this, "OnClickResearch"));
                transform.Find("price").gameObject.SetActive(true);
                transform.Find("tip").gameObject.SetActive(false);
                transform.Find("price").GetComponent<UILabel>().text = tid.upgradeCost.ToString();
                transform.Find("bg").GetComponent<UISprite>().color = Color.white;
                transform.Find("avatar").GetComponent<UISprite>().color = Color.white;
                **/

                if (tid.upgradeCost > DataManager.GetInstance.model.user_info.gold_count)
                    ele.Find("txt_corn").GetComponent<Text>().color = Color.red;
                else
                    ele.Find("txt_corn").GetComponent<Text>().color = Color.white;
                //transform.Find("Sprite").gameObject.SetActive(true);
            }

            ele.Find("icon_head").GetComponent<Image>().sprite = ResourceManager.GetInstance.atlas.avaterSpriteDic[tid.tid];
            Button infoBtn = ele.Find("icon_info").GetComponent<Button>();
            infoBtn.onClick.RemoveAllListeners();
            infoBtn.onClick.AddListener(ShowResearchInfo);

            //UIButton infoBtn = transform.Find("info").GetComponent<UIButton>();
            //infoBtn.onClick = new List<EventDelegate>();
            //infoBtn.onClick.Add(new EventDelegate(this, "OnClickInfo"));
 
        }

        void ShowResearchInfo()
        {
            Debug.Log("ShowResearchInfo");
            Transform trans = EventSystem.current.currentSelectedGameObject.transform.parent;
            int index = items.IndexOf(trans);
            ResearchTid tid = this.researchList[index];
			UIMgr.GetController<TroopDetailCtrl>().ShowBattle(tid.tid_level, this);
            Close();
        }

        void Upgrade()
        {
            Debug.Log("Upgrade");
            Transform trans = EventSystem.current.currentSelectedGameObject.transform;
            int index = items.IndexOf(trans);
            ResearchTid tid = this.researchList[index];
			UIMgr.GetController<TroopDetailCtrl>().ShowUpgradePanel(tid, this);
            Close();
            // PopManage.Instance.ShowTroopUpgradeWin(TidData.tid_level, "Research");
        }


        void OnCreatePanel()
        {
            mResearchPanelView.m_btnClose.onClick.AddListener(Close);
            mResearchPanelView.m_btnClose.onClick.AddListener(CloseMask);
            items = new List<Transform>();
            Transform container = mResearchPanelView.m_gridArmy;
            foreach (Transform researchItembox in container)
            {
                items.Add(researchItembox);
            }
        }

        public override void Close()
        {
            UIMgr.ClosePanel("ResearchPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }

    }
}
