using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;



public class PopDialog : MonoBehaviour {


	public static PopDialog current;
		
	private int width;
	private int height;

	private Transform WinBG;
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
        Debug.LogError(" PopManager instance is null : "+(PopManage.Instance == null));
		if(current!=null && PopManage.Instance.popList!=null)
            PopManage.Instance.popList.Remove (current.transform);
		current = null;
		if(PopManage.Instance.popList.Count==0)
			UIMask.Mask.gameObject.SetActive (false);
		tweener.onFinished = new List<EventDelegate> ();
		if(AfterClose!=null){
			//Debug.Log("xxxx");
			AfterClose ();
		}
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
		tweener.PlayForward ();
	}


	public void OpenWin(string title, bool showLeftTopClose = true)
	{
        Debug.Log("OpenWin");
		if(current==null)
		{
			if(tweener.onFinished.Count>0)
			{
				EventDelegate.Execute(tweener.onFinished);
			}

			WinTitle.GetComponent<UILabel>().text = title;

			CloseIco.gameObject.SetActive(showLeftTopClose);


			//绑定取消事件;
			List<EventDelegate> onCancelDelegate = new List<EventDelegate> ();
			onCancelDelegate.Add (new EventDelegate(this,"CloseTween"));

			CloseIco.GetComponent<UIButton> ().onClick = onCancelDelegate;
		


			gameObject.SetActive (true);
			if(UIMask.Mask!=null) UIMask.Mask.gameObject.SetActive (true);

			tweener.onFinished = new List<EventDelegate>();
			
			tweener.Reset ();
			tweener.PlayForward ();
			current = this;
			PopManage.Instance.popList.Add (current.transform);
		}
	}




}
