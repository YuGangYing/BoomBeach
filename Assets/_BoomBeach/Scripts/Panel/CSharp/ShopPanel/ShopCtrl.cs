using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace BoomBeach { 
    public class ShopCtrl : BaseCtrl
    {

        ShopPanelView mShopPanelView;

        public int currentLevel;

        public List<GameObject> cardItems = new List<GameObject>();
        public List<ShopItem> cardItemDatas;

		public override void ShowPanel()
        {
            bool isCreate;
            mShopPanelView = UIMgr.ShowPanel<ShopPanelView>(UIManager.UILayerType.Common, out isCreate);
            if (isCreate)
            {
                OnCreatePanel();
            }
			UIMgr.GetController<MaskCtrl>().ShowPanel(new UnityEngine.Events.UnityAction(Close));
            ShowResource();
        }

        void OnCreatePanel()
        {
            mShopPanelView.m_btnResource.onClick.AddListener(ShowResource);
            mShopPanelView.m_btnDefence.onClick.AddListener(ShowDefence);
            mShopPanelView.m_btnSurport.onClick.AddListener(ShowSurport);
            LayoutElement[] eles = mShopPanelView.m_gridResourcecarditems.GetComponentsInChildren<LayoutElement>(true);
            for (int i=0;i<eles.Length;i++) {
                Button btn = eles[i].transform.FindChild("Button").GetComponent<Button>();
                btn.onClick.AddListener(OnCardItemClick);
                cardItems.Add(btn.gameObject);
            }
        }

        void DeselectTabBtn(Button btn)
        {
            btn.transform.FindChild("Back").GetComponent<Image>().color = new Color(81f / 255, 186f / 255, 245f / 255, 1);
        }

        void SelectTabBtn(Button btn)
        {
            btn.transform.FindChild("Back").GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        void ShowResource()
        {
            Helper.CalcShopCates(false);
            List<ShopCate> shopCates = Globals.ShopCates;
            SetShopData(shopCates[0].ShopItems);
            SelectTabBtn(mShopPanelView.m_btnResource);
            DeselectTabBtn(mShopPanelView.m_btnDefence);
            DeselectTabBtn(mShopPanelView.m_btnSurport);

        }

        void ShowDefence()
        {
            Helper.CalcShopCates(false);
            List<ShopCate> shopCates = Globals.ShopCates;
            SetShopData(shopCates[1].ShopItems);
            DeselectTabBtn(mShopPanelView.m_btnResource);
            SelectTabBtn(mShopPanelView.m_btnDefence);
            DeselectTabBtn(mShopPanelView.m_btnSurport);
        }

        void ShowSurport()
        {
            Helper.CalcShopCates(false);
            List<ShopCate> shopCates = Globals.ShopCates;
            SetShopData(shopCates[2].ShopItems);
            DeselectTabBtn(mShopPanelView.m_btnResource);
            DeselectTabBtn(mShopPanelView.m_btnDefence);
            SelectTabBtn(mShopPanelView.m_btnSurport);
        }


        void SetShopData(List<ShopItem> shopItems)
        {
            cardItemDatas = shopItems;
            LayoutElement[] eles = mShopPanelView.m_gridResourcecarditems.GetComponentsInChildren<LayoutElement>(true);
            
            for (int i=0;i<eles.Length;i++)
            {
                eles[i].gameObject.SetActive(false);
            }
            for (int i=0;i< shopItems.Count;i++)
            {
                eles[i].gameObject.SetActive(true);
                BindData(eles[i], shopItems[i]);
            }
            HorizontalLayoutGroup horizontal = mShopPanelView.m_gridResourcecarditems.GetComponent<HorizontalLayoutGroup>();
            RectTransform rt = horizontal.GetComponent<RectTransform>();
            float width = shopItems.Count * 300 + (shopItems.Count - 1) * 10;
            //width = Mathf.Max(width, 1334);
            rt.sizeDelta = new Vector2(width, 0);
            rt.localPosition = new Vector3(width / 2, rt.localPosition.y, rt.localPosition.z);
            //horizontal.CalculateLayoutInputHorizontal();
            //mShopPanelView.m_gridResourcecarditems.localPosition = new Vector3(width/2,200,0);
            /*
            BuildingModel bm = CityController.SingleTon().buildingModel;
            LayoutElement[] eles = mShopPanelView.m_gridResourcecarditems.GetComponentsInChildren<LayoutElement>();
            string tid = bm.basicBuildingDatas[BuildingModel.HOUSING].TID;
            SetItem(eles[0], tid);
            tid = bm.basicBuildingDatas[BuildingModel.WOOD_STORAGE].TID;
            SetItem(eles[1], tid);
            */
        }

        void OnShopItemClick()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            Close();
            CloseMask();
            Debug.Log(go.transform.FindChild("img_item"));
        }


        void SetItem(LayoutElement ele,string tid)
        {
            Button button = ele.transform.FindChild("Button").GetComponent<Button>();
            button.onClick.AddListener(OnShopItemClick);
            Image image = ele.transform.FindChild("Button/img_item").GetComponent<Image>();
			Sprite sprite = ResourceManager.GetInstance().atlas.avaterSpriteDic[tid];
            image.sprite = sprite;
        }

        //绑定数据;
        public void BindData(LayoutElement ele,ShopItem shopItem)
        {
            Button button = ele.transform.FindChild("Button").GetComponent<Button>();
            Text disablemsg = ele.transform.FindChild("Button/item_disablemsg").GetComponent<Text>();
            GameObject resourceBox = ele.transform.FindChild("Button/container_resourcebox").gameObject;
            Text buildName = ele.transform.FindChild("Button/item_name").GetComponent<Text>();
            Text buildDisc = ele.transform.FindChild("Button/item_disc").GetComponent<Text>();
            Text buildTime = ele.transform.FindChild("Button/container_resourcebox/txt_builttime").GetComponent<Text>();
            Text buildCount = ele.transform.FindChild("Button/container_resourcebox/txt_build").GetComponent<Text>();
            Image buildImg = ele.transform.FindChild("Button/img_item").GetComponent<Image>(); 

            if (shopItem.isEnabled)
            {
                disablemsg.gameObject.SetActive(false);
                resourceBox.SetActive(true);

                Transform resourceLayout = ele.transform.FindChild("Button/container_resourcebox/container_resource");
               
                //LayoutElement[] resourceEles = resourceLayout.GetComponentsInChildren<LayoutElement>();
                for (int i = 0; i < resourceLayout.childCount ; i++)
                {
                    resourceLayout.GetChild(i).gameObject.SetActive(false);
                }
                for (int i=0;i< shopItem.ShopCosts.Count;i++)
                {
                    ShopCost sc = shopItem.ShopCosts[i];
                    if (sc.CostType == ShopCostType.Wood)
                    {
                        resourceLayout.GetChild(0).gameObject.SetActive(true);
                        resourceLayout.GetChild(0).transform.FindChild("txt_resource").GetComponent<Text>().text = sc.CostAmount;
                    }
                    else if (sc.CostType == ShopCostType.Stone)
                    {
                        resourceLayout.GetChild(1).gameObject.SetActive(true);
                        resourceLayout.GetChild(1).transform.FindChild("txt_resource").GetComponent<Text>().text = sc.CostAmount;
                    }
                    else if (sc.CostType == ShopCostType.Iron)
                    {
                        resourceLayout.GetChild(2).gameObject.SetActive(true);
                        resourceLayout.GetChild(2).transform.FindChild("txt_resource").GetComponent<Text>().text = sc.CostAmount;
                    }
                }


                Image[] images = ele.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].material = null;
                    button.enabled = true;
                }
            }
            else
            {
                disablemsg.gameObject.SetActive(true);
                disablemsg.text = shopItem.DisableDescription;
                resourceBox.SetActive(false);
                Image[] images = ele.GetComponentsInChildren<Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].material = ResourceManager.GetInstance().grayMat;
                    button.enabled = false;
                    //images[i].transform.localPosition = new Vector3(images[i].transform.localPosition.x, images[i].transform.localPosition.y, 0.1f);
                }
            }
            buildName.text = shopItem.Name;
            if (shopItem.Description != "")
            {
                buildDisc.gameObject.SetActive(true);
                buildDisc.text = shopItem.Description;
               
            }
            else
            {
                
                buildDisc.gameObject.SetActive(false);
            }

            if (shopItem.Buildtime != null)
            {
                buildTime.text = shopItem.Buildtime;
            }
            else
            {
                buildTime.text = "";
            }
            if (shopItem.BuildMax != 0)
            {
                buildCount.text = shopItem.Buildcount + " / " + shopItem.BuildMax;
            }
            else
            {
                buildCount.text = "0 / 0";
            }
            buildImg.sprite = ResourceManager.GetInstance().atlas.avaterSpriteDic[shopItem.tid];



            /*
            if (shopItem.diamondAmount > 0)
            {
                DiamondShowBox.gameObject.SetActive(true);
                DiamondShowLabel.text = shopItem.diamondAmount.ToString();
                ShopSprite.SetSprite(shopItem.tid_level);
                //ShopSprite.spriteName = shopItem.tid_level;
            }
            else
            {
                DiamondShowBox.gameObject.SetActive(false);
                ShopSprite.SetSprite(shopItem.tid);
                //ShopSprite.spriteName = shopItem.tid;
            }
            */


            /*
             ResourceLayout.ItemCount = shopItem.ShopCosts.Count;


             DiamondResource.gameObject.SetActive(false);
             GoldResource.gameObject.SetActive(false);
             WoodResource.gameObject.SetActive(false);
             StoneResource.gameObject.SetActive(false);
             IronResource.gameObject.SetActive(false);
             MoneyResource.gameObject.SetActive(false);

             for (int i = 0; i < shopItem.ShopCosts.Count; i++)
             {
                 ResourceBox.Find(shopItem.ShopCosts[i].CostType.ToString() + "ResourceItem").gameObject.SetActive(true);
                 ResourceBox.Find(shopItem.ShopCosts[i].CostType.ToString() + "ResourceItem/Label").GetComponent<UILabel>().text = shopItem.ShopCosts[i].CostAmount.ToString();
                 //Debug.Log(shopItem.ShopCosts[i].CostType+" "+shopItem.ShopCosts[i].CostAmount) ;
             }
             ResourceBox.GetComponent<UIGrid>().Reposition();
             IsEnabled = shopItem.isEnabled;
             */
        }

        void OnCardItemClick()
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
            int index = cardItems.IndexOf(EventSystem.current.currentSelectedGameObject);
            ShopItem shopItem = cardItemDatas[index];
            string msg = Helper.checkNewBuild(shopItem.tid);
            //#region 客户端测试，为了可以创建任意建筑
            //msg = null;
            //#endregion
            if (msg == null)
            {
            //关闭当前面板;
            //if (PopPanel.current != null)
            //   PopPanel.current.CloseTween();
            //先移除正在：创建中的建筑物(只会有一个);
	            foreach (BuildInfo s in DataManager.GetInstance().buildList.Values)
	            {
	                if (s.status == BuildStatus.Create)
	                {
	                    if (DataManager.GetInstance().buildList.ContainsKey(s.building_id))
	                    {
	                        DataManager.GetInstance().buildList.Remove(s.building_id);
	                    }
	                    if (DataManager.GetInstance().buildArray.Contains(s))
	                    {
	                        DataManager.GetInstance().buildArray.Remove(s);
	                    }
	                    //building_id = s.building_id;
	                    //清空占用格子;
	                    s.ClearGrid();
	                    Destroy(s.gameObject);
	                    break;
	                }
	            }
	            //创建一个新的建筑物;
                BuildManager.CreateBuild(new BuildParam()
                {
                    tid = shopItem.tid,
                    level = 1
                });
                Close();
	            CloseMask();
            }
            //else
            //{
                //提示一下，不能创建的原因;
              //  PopManage.Instance.ShowMsg(msg);
            //}
        }

        public override void Close()
        {
            UIMgr.ClosePanel("ShopPanel");
        }

        public void CloseMask()
        {
			UIMgr.GetController<MaskCtrl>().Close();
        }
    }
}