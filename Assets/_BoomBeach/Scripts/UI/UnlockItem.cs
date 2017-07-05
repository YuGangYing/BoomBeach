using UnityEngine;
using System.Collections;

public class UnlockItem : MonoBehaviour {


	public Transform Model;
	public Transform Tips;
	public UILabel NameLabel;
	public UILabel BriefLabel;
	public UILabel CountLabel;


	public string Brief{
		get{ return BriefLabel.text; }
		set{ BriefLabel.text = value; }
	}

	public string Name{
		get{ return NameLabel.text; }
		set{ NameLabel.text = value; }
	}

	public string Counter{
		get{ return CountLabel.text; }
		set{ CountLabel.text = value; }
	}

	public string model{
		set{
			UISprite sp = Model.GetComponent<UISprite>();
			sp.spriteName = value;
			sp.MakePixelPerfect();
			//sp.transform.localScale = Vector3.one*2;
		}
	}

	public void showTip()
	{
		if(Tips.gameObject.activeSelf)
		{
			Tips.gameObject.SetActive (false);
			BuildUpgradeWin.Instance.TipBox = null;
		}
		else
		{
			if(BuildUpgradeWin.Instance.TipBox!=null)
			{
				BuildUpgradeWin.Instance.TipBox.gameObject.SetActive(false);
				BuildUpgradeWin.Instance.TipBox = null;
			}
			Tips.gameObject.SetActive (true);
			BuildUpgradeWin.Instance.TipBox = Tips;
		}
	}



}
