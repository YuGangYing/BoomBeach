using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;

public class CardItem : MonoBehaviour {

	private Transform CardBg; //底色;
	private Transform CardBgBg; //底色阴影;
	private UIButton UIbtn;
	public ShopItem shopItem;

    /// <summary>
    /// 该Card所属的Tab的类型信息
    /// </summary>
    public string tabInfo;

	public CardItem NextCard;	   //下一张;

	public Shader NormalShader;
	public Shader GrayShader;

	private UILabel NameLable;
	private UILabel DescriptionLabel;
	private Transform ImageBox;
	private GameObject ImageItem;
	private tk2dSprite ShopSprite;
	private Transform BuildTime;
	private UILabel BuildTimeLabel;
	private Transform BuildCount;
	private UILabel BuildCountLabel;
	private UILabel DisableDescriptionLabel;
	private Transform ResourceBox;
	private Transform DiamondResource;
	//private UILabel DiamondResourceLabel;
	private Transform GoldResource;
	//private UILabel GoldResourceLabel;
	private Transform WoodResource;
	//private UILabel WoodResourceLabel;
	private Transform StoneResource;
	//private UILabel StoneResourceLabel;
	private Transform IronResource;
	//private UILabel IronResourceLabel;
	private Transform MoneyResource;
	//private UILabel MoneyResourceLabel;

	private ShopResourceLayout ResourceLayout;

	private Transform DiamondShowBox;
	private UILabel DiamondShowLabel;

	public void Init()
	{
		UIbtn = GetComponent<UIButton> ();
		CardBg = transform.Find ("CardPostionTween/CardBg");
		CardBgBg = CardBg.Find("bg");
		isEnabled = true;

		NameLable = transform.Find ("CardPostionTween/Name").GetComponent<UILabel>();
		DescriptionLabel = transform.Find ("CardPostionTween/Description").GetComponent<UILabel>();
		ImageBox = transform.Find ("CardPostionTween/ImageBox");
		ImageItem = ImageBox.Find ("ShopItemImage").gameObject;
		ShopSprite = ImageItem.GetComponent<tk2dSprite> ();
		BuildTime = transform.Find ("CardPostionTween/BuildTime");
		BuildTimeLabel = transform.Find ("CardPostionTween/BuildTimeC").GetComponent<UILabel>();
		BuildCount = transform.Find ("CardPostionTween/Build");
		BuildCountLabel = transform.Find ("CardPostionTween/BuildC").GetComponent<UILabel>(); 
		DisableDescriptionLabel = transform.Find ("CardPostionTween/WhiteDescBox/Label").GetComponent<UILabel>(); 

		ResourceBox = transform.Find ("CardPostionTween/WhiteDescBox/Grid");

		DiamondResource = transform.Find ("CardPostionTween/WhiteDescBox/Grid/DiamondResourceItem");
		//DiamondResourceLabel = DiamondResource.Find ("Label").GetComponent<UILabel> ();
		GoldResource = transform.Find ("CardPostionTween/WhiteDescBox/Grid/GoldResourceItem");
		//GoldResourceLabel = GoldResource.Find ("Label").GetComponent<UILabel> ();
		WoodResource = transform.Find ("CardPostionTween/WhiteDescBox/Grid/WoodResourceItem");
		//WoodResourceLabel = WoodResource.Find ("Label").GetComponent<UILabel> ();
		StoneResource = transform.Find ("CardPostionTween/WhiteDescBox/Grid/StoneResourceItem");
		//StoneResourceLabel = StoneResource.Find ("Label").GetComponent<UILabel> ();
		IronResource = transform.Find ("CardPostionTween/WhiteDescBox/Grid/IronResourceItem");
		//IronResourceLabel = IronResource.Find ("Label").GetComponent<UILabel> ();
		MoneyResource = transform.Find ("CardPostionTween/WhiteDescBox/Grid/MoneyResourceItem");
		//MoneyResourceLabel = MoneyResource.Find ("Label").GetComponent<UILabel> ();

		DiamondShowBox = transform.Find ("CardPostionTween/DiamondShow");
		DiamondShowLabel = DiamondShowBox.Find ("Label").GetComponent<UILabel> ();

		ResourceLayout = transform.Find ("CardPostionTween/WhiteDescBox/Grid").GetComponent<ShopResourceLayout> ();

	}

	private bool isEnabled;
	public bool IsEnabled{
		set{
			isEnabled = value;
			if(isEnabled)
			{
				UIbtn.enabled = true;
				UIbtn.tweenTarget = CardBg.gameObject;

				//ShopSprite.color = Color.white;

				MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
				for(int i=0;i<renderers.Length;i++)
				{
					renderers[i].material.shader = NormalShader;
				}
			}
			else
			{
				UIbtn.enabled = false;
				CardBg.GetComponent<UISprite>().color = UIbtn.disabledColor;
				CardBgBg.GetComponent<UISprite>().color = Color.black;

				//ShopSprite.color = Color.black;

				MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
				for(int i=0;i<renderers.Length;i++)
				{
					renderers[i].material.shader = GrayShader;
				}
			}
		}
		get{
			return isEnabled;
		}
	}

	//绑定数据;
	public void Bind()
	{
		if(shopItem.isEnabled)
		{
			DisableDescriptionLabel.gameObject.SetActive(false);
			ResourceBox.gameObject.SetActive(true);
		}
		else
		{
			DisableDescriptionLabel.gameObject.SetActive(true);
			ResourceBox.gameObject.SetActive(false);
		}

		NameLable.text = shopItem.Name;
		if(shopItem.Description!="")
		{
			DescriptionLabel.gameObject.SetActive(true);
			DescriptionLabel.text = shopItem.Description;
		}
		else
		{
			DescriptionLabel.gameObject.SetActive(false);
		}

		if(shopItem.diamondAmount>0)
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




		if(shopItem.Buildtime==null)
		{
			BuildTimeLabel.gameObject.SetActive(false);
			BuildTime.gameObject.SetActive(false);
		}
		else
		{
			BuildTimeLabel.gameObject.SetActive(true);
			BuildTime.gameObject.SetActive(true);
			BuildTimeLabel.text = shopItem.Buildtime;
		}

		//最大值0时,不显示;
		if(shopItem.BuildMax==0)
		{
			BuildCountLabel.gameObject.SetActive(false);
			BuildCount.gameObject.SetActive(false);
		}
		else
		{
			BuildCountLabel.gameObject.SetActive(true);
			BuildCount.gameObject.SetActive(true);
			BuildCountLabel.text = shopItem.Buildcount.ToString () + "/" + shopItem.BuildMax.ToString ();
		}

		DisableDescriptionLabel.text = shopItem.DisableDescription;
		ResourceLayout.ItemCount = shopItem.ShopCosts.Count;


		DiamondResource.gameObject.SetActive (false);
		GoldResource.gameObject.SetActive (false);
		WoodResource.gameObject.SetActive (false);
		StoneResource.gameObject.SetActive (false);
		IronResource.gameObject.SetActive (false);
		MoneyResource.gameObject.SetActive (false);

		for (int i=0; i<shopItem.ShopCosts.Count; i++) 
		{
			ResourceBox.Find(shopItem.ShopCosts[i].CostType.ToString()+"ResourceItem").gameObject.SetActive(true);
			ResourceBox.Find(shopItem.ShopCosts[i].CostType.ToString()+"ResourceItem/Label").GetComponent<UILabel>().text = shopItem.ShopCosts[i].CostAmount.ToString();
			//Debug.Log(shopItem.ShopCosts[i].CostType+" "+shopItem.ShopCosts[i].CostAmount) ;
		}
		ResourceBox.GetComponent<UIGrid> ().Reposition ();
		IsEnabled = shopItem.isEnabled;
	}

	public void PlayAni()
	{
		TweenPosition tw = transform.Find("CardPostionTween").GetComponent<TweenPosition> ();
		if(NextCard!=null)
		{

			tw.onFinished = new List<EventDelegate> ();
			tw.onFinished.Add (new EventDelegate(NextCard,"PlayAni"));
		}
		tw.from = transform.position;
		tw.to = new Vector3 (tw.from.x,tw.from.y+35f,tw.from.z);
		tw.duration = 0.1f;
		tw.Reset ();
		tw.PlayForward ();
	}

	void OnClick()
	{
		//Debug.Log("Click Card:" + shopItem.tid_level);
		if (shopItem.tid == "TID_DIAMOND_PACK"){
			if (shopItem.isEnabled){
				//购买;
				//Debug.Log("Click Card:" + shopItem.tid_level);
				//关闭当前面板;


				//禁用面版;
				PopManage.Instance.ShowDiamondPanel(false);
			}
		}else{
			string msg = Helper.checkNewBuild(shopItem.tid);


            #region 客户端测试，为了可以创建任意建筑

            msg = null;

            #endregion



            if (msg == null){

				//关闭当前面板;
				if (PopPanel.current != null)
					PopPanel.current.CloseTween ();



				//先移除正在：创建中的建筑物(只会有一个);
				foreach(BuildInfo s in DataManager.GetInstance().buildList.Values){
					if (s.status == BuildStatus.Create){
						if (DataManager.GetInstance().buildList.ContainsKey(s.building_id)){
							DataManager.GetInstance().buildList.Remove(s.building_id);
						}
						if(DataManager.GetInstance().buildArray.Contains(s))
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
                    level = 1,
                    cardName = NameLable.text.Trim(),
                    tabPageInfo = tabInfo
                });
            }
            else{
				//提示一下，不能创建的原因;
				PopManage.Instance.ShowMsg(msg);
			}
			 //Helper.NewBuild(shopItem.tid);
		}
	}
}
