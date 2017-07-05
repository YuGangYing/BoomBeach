using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopWin : MonoBehaviour {


	public static PopWin current;

	private int width;
	private int height;

	private Transform WinBG;
	private Transform BackIco;
	private Transform CloseIco;
	private Transform WinTop;
	private Transform WinTopBg;
	private Transform WinTitle;
	private TweenScale tweener; 
	public int Width{
		set{
			width = value;
			WinBG.GetComponent<UIWidget>().width = width;
			WinTopBg.GetComponent<UIWidget> ().width = width;
			GetComponent<BoxCollider>().size = new Vector3((float)width,GetComponent<BoxCollider>().size.y,0f);
			CloseIco.GetComponent<UIAnchor>().enabled = true;
		}
		get{
			return width;
		}
	}

	public int Height{
		set{
			height = value;
			WinBG.GetComponent<UIWidget>().height = height;
			GetComponent<BoxCollider>().size = new Vector3(GetComponent<BoxCollider>().size.x,(float)height,0f);
		}
		get{
			return height;
		}
	}

	public string Title{
		set{
			WinTitle.GetComponent<UILabel> ().text = value;
		}
		get{
			return WinTitle.GetComponent<UILabel> ().text;
		}
	}


	public void Init()
	{
		WinBG = transform.Find("WinBg");
		WinTop = transform.Find("WinTop");
		WinTopBg = WinTop.Find ("TopBg");
		BackIco = WinTop.Find ("BackIco");
		CloseIco = WinTop.Find ("CloseIco");
		WinTitle = WinTop.Find ("Title");

		width = WinBG.GetComponent<UIWidget>().width;
		height = WinBG.GetComponent<UIWidget>().height;

		//同步top宽;
		WinTopBg.GetComponent<UIWidget> ().width = width;
		GetComponent<BoxCollider>().size = new Vector3((float)width,(float)height,0f);


		tweener = gameObject.GetComponent<TweenScale> ();
	}

	public delegate void AfterCloseDelegate();
	public AfterCloseDelegate AfterClose; 
	void CloseWin()
	{
		gameObject.SetActive (false);
		Height = 1000;

        Debug.LogError("PopManage.Instance is null : " + (PopManage.Instance == null));

		if(current!=null) PopManage.Instance.popList.Remove (current.transform);
		current = null;
		if(PopManage.Instance.popList.Count==0)
			UIMask.Mask.gameObject.SetActive (false);

		if(AfterClose!=null)AfterClose ();
		tweener.onFinished = new List<EventDelegate> ();
	}

	public void CloseTween()
	{
		if(tweener.onFinished.Count>0)
		{
			EventDelegate.Execute(tweener.onFinished);
		}

		//绑定关闭事件;
		List<EventDelegate> onTweenFinish = new List<EventDelegate> ();
		onTweenFinish.Add (new EventDelegate (this, "CloseWin"));
		tweener.onFinished = onTweenFinish;		
		tweener.Reset ();
		tweener.PlayForward();
	}

	public void OpenWin()
	{
		//绑定返回事件;
		if(OnBack!=null)
		{
			List<EventDelegate> OnBackDelegate = new List<EventDelegate> ();
			OnBackDelegate.Add (new EventDelegate(this,"BackWin"));
			BackIco.GetComponent<UIButton> ().onClick = OnBackDelegate;
			BackIco.gameObject.SetActive(true);
		}
		else
		{
			BackIco.gameObject.SetActive(false);
		}

		if(current==null)
		{

			if(tweener.onFinished.Count>0)
			{
				EventDelegate.Execute(tweener.onFinished);
			}
			//绑定关闭事件;
			List<EventDelegate> onCloseDelegate = new List<EventDelegate> ();
			onCloseDelegate.Add (new EventDelegate(this,"CloseTween"));
			CloseIco.GetComponent<UIButton> ().onClick = onCloseDelegate;
			gameObject.SetActive (true);
			UIMask.Mask.gameObject.SetActive (true);
			tweener.onFinished = new List<EventDelegate>();
			tweener.onFinished.Add (new EventDelegate(this,"AfterOpenWin"));
			tweener.Reset ();
			tweener.PlayForward ();
			current = this;
			PopManage.Instance.popList.Add (current.transform);
		}
	}

	public delegate void AfterOpenDelegate();
	public AfterOpenDelegate AfterOpen;
	void AfterOpenWin()
	{
		//CloseIco.GetComponent<UIAnchor>().enabled = true;
		UIAnchor[] anchors = GetComponentsInChildren<UIAnchor>();
		for(int i=0;i<anchors.Length;i++)
		{
			anchors[i].enabled = true;
		}
		if (AfterOpen != null)
		{
			AfterOpen ();
			AfterOpen = null;
		}
	}

	public delegate void OnBackDelegate();
	public OnBackDelegate OnBack;
	void BackWin()
	{ 
		if (OnBack != null)OnBack ();
	}


	void OnClick()
	{
		if (BuildUpgradeWin.Instance!=null&&BuildUpgradeWin.Instance.TipBox != null)
			BuildUpgradeWin.Instance.TipBox.gameObject.SetActive (false);
		if (CrystleResource.Instance!=null&&CrystleResource.Instance.crystletip != null)
			CrystleResource.Instance.crystletip.gameObject.SetActive (false);
	}

}
