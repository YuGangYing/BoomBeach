using UnityEngine;
using System.Collections;

public class TabItem : MonoBehaviour {

	public int idx;
	private Transform TabBg;
	private Transform TabShadow;
	//private UIButton UIBtn;
	public bool isChecked;
	private Color NormalColor;
	private UILabel titleLable;
	private Transform CountBox;
	private UILabel noticeLable;

	public OnSelect onselect;

	public void Init()
	{
		//UIBtn = GetComponent<UIButton> (); 
		TabBg = transform.Find("bg");
		TabShadow = transform.Find ("shadow");
		CountBox = transform.Find ("count");
		noticeLable = CountBox.Find ("Label").GetComponent<UILabel> ();
		titleLable = transform.Find ("Label").GetComponent<UILabel> ();
		NormalColor = TabBg.GetComponent<UISprite> ().color;
	}

	public bool IsChecked{
		set{
			isChecked = value;
			if(isChecked)
			{
				TabBg.GetComponent<UISprite> ().color = Color.white;
				TabBg.GetComponent<UISprite> ().depth = 10;
				TabShadow.GetComponent<UISprite>().depth = 9;
			}
			else
			{
				TabBg.GetComponent<UISprite> ().color = NormalColor;
				TabBg.GetComponent<UISprite> ().depth = 8;
				TabShadow.GetComponent<UISprite>().depth = 7;
			}
		}
		get{return isChecked; }
	}

	void OnClick()
	{
		//Debug.Log(TabPanel.current.name);
		AudioPlayer.Instance.PlaySfx("button_click");
		TabPanel.current.SelectTab (idx,onselect);
	}

	public string Title{
		set{
			titleLable.text = value;
		}
		get{
			return titleLable.text;
		}
	}

	public int NoticeCount
	{
		set{ 
			if(value==0)
			{
				CountBox.gameObject.SetActive(false);
			}
			else
			{
				CountBox.gameObject.SetActive(true);
				noticeLable.text = value.ToString();
			}
		}
		get{
			return int.Parse(noticeLable.text);
		}
	}
}
