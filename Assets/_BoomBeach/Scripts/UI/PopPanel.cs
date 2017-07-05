using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopPanel : TabPanel {


	public new static PopPanel current;

	private int width;


	private Transform BackWhite;
	private Transform BackBlue;
	private Transform ScrollViewBox;
	private UIPanel ScrollViewPanel;
	public CardItem[] Cards;
	private TweenPosition tweener; 

	public int Width{
		get{return width; }
		set{ 
			width = value;
			BackWhite.GetComponent<UIWidget>().width = width;
			BackBlue.GetComponent<UIWidget>().width = width;
			ScrollViewPanel.baseClipRegion = new Vector4 (width/2,ScrollViewPanel.baseClipRegion.y,width,ScrollViewPanel.baseClipRegion.w);
		}
	}
	

	public new void Init()
	{
		base.Init ();

		BackWhite = transform.Find("BackWhite");
		BackBlue = transform.Find("BackBlue");
		ScrollViewBox = transform.Find ("CardItems/ScrollView");
		ScrollViewPanel = ScrollViewBox.GetComponent<UIPanel> ();
		Cards = ScrollViewBox.Find ("GridList").GetComponentsInChildren<CardItem> ();

		if(ScreenLayout.Instance!=null) width = ScreenLayout.Instance.Width;
		BackWhite.GetComponent<UIWidget>().width = width; 
		BackBlue.GetComponent<UIWidget>().width = width;
		ScrollViewPanel.baseClipRegion = new Vector4 (width/2,ScrollViewPanel.baseClipRegion.y,width,ScrollViewPanel.baseClipRegion.w);
		tweener = gameObject.GetComponent<TweenPosition> ();

		for(int i=0;i<Cards.Length;i++)
		{
			Cards[i].Init();
		}	
		//gameObject.SetActive (false);
	}


	public delegate void AfterCloseDelegate();
	public AfterCloseDelegate AfterClose; 
	void CloseWin()
	{
		gameObject.SetActive (false);
        
        PopManage.Instance.popList.Remove (current.transform);
		current = null;
		TabPanel.current = null;

		if(PopManage.Instance.popList.Count==0)
			UIMask.Mask.gameObject.SetActive (false);
		isOpen = false;

		tweener.onFinished = new List<EventDelegate> ();
		if(AfterClose!=null)AfterClose ();
	}

	public void CloseTween()
	{
		//绑定关闭事件;
		if(tweener.onFinished.Count>0)
		{
			EventDelegate.Execute(tweener.onFinished);
		}
		List<EventDelegate> onTweenFinish = new List<EventDelegate> ();
		onTweenFinish.Add (new EventDelegate (this, "CloseWin"));
		tweener.onFinished = onTweenFinish;		
		tweener.PlayReverse ();
	}

	private List<ShopCate> shopCates;
	public void OpenWin(List<ShopCate> i_shopCates)
	{
		if(tweener.onFinished.Count>0)
		{
			EventDelegate.Execute(tweener.onFinished);
		}

		currentIdx = -1;
		gameObject.SetActive (true);
		shopCates = i_shopCates;
		
		if(shopCates.Count>1)
		{
			BackBlue.transform.localPosition = Vector3.zero;
			transform.Find("Tabs").GetComponent<UIAnchor>().enabled = true;
			transform.Find("CardItems").GetComponent<UIAnchor>().enabled = true;
		}
		else
		{
			BackBlue.transform.localPosition = new Vector3(BackBlue.transform.localPosition.x,-96f,0f);
			transform.Find("Tabs").GetComponent<UIAnchor>().enabled = true;
			transform.Find("CardItems").GetComponent<UIAnchor>().enabled = true;
		}
		
		for(int i=0;i<Tabs.Length;i++)
		{
			Tabs[i].onselect = OnSelectBind;
			if(i<shopCates.Count)
			{
				Tabs[i].Title = shopCates[i].Name;
				Tabs[i].NoticeCount = shopCates[i].NoticeCount;
			}
		}
		
		ScrollViewBox.GetComponent<UIScrollView> ().ResetPosition ();
		SelectTab (0,OnSelectBind);


		UIMask.Mask.gameObject.SetActive (true);
		tweener.onFinished = new List<EventDelegate>();
		tweener.onFinished.Add (new EventDelegate(this,"AfterOpen"));
		tweener.PlayForward ();
		current = this;
		TabPanel.current = this;
		PopManage.Instance.popList.Add (current.transform);
	}

	public void Refreash()
	{
		ShopCate cate = shopCates [currentIdx];
		for(int i=0;i<Cards.Length;i++)
		{
			if(i<cate.ShopItems.Count)
			{
				Cards[i].gameObject.SetActive(true);
				ShopItem shopItem = cate.ShopItems[i];
				Cards[i].shopItem = shopItem;

                Cards[i].tabInfo = cate.Name;

                if (i+1<Cards.Length)Cards[i].NextCard = Cards[i+1];
				Cards[i].Bind();
			}
			else
			{
				Cards[i].gameObject.SetActive(false);
			}
		}

	}

	private void AfterOpen()
	{
		isOpen = true;
		tweener.onFinished = new List<EventDelegate> ();
	}

	private bool isOpen = false;
	private int currentIdx;
	//切换后绑定数据;
	private void OnSelectBind(int idx)
	{
		if(idx!=currentIdx)
		{
			currentIdx = idx;
			ShopCate cate = shopCates [idx];
			for(int i=0;i<Cards.Length;i++)
			{
				if(i<cate.ShopItems.Count)
				{
					Cards[i].gameObject.SetActive(true);
					ShopItem shopItem = cate.ShopItems[i];
					Cards[i].shopItem = shopItem;
                    Cards[i].tabInfo = cate.Name;

                    if (i+1<Cards.Length)Cards[i].NextCard = Cards[i+1];
					Cards[i].Bind();
					if(i==0&&isOpen) Cards[i].PlayAni();
				}
				else
				{
					Cards[i].gameObject.SetActive(false);
				}
			}
			ScrollViewBox.GetComponent<UIScrollView> ().ResetPosition ();
		}
	}






}
