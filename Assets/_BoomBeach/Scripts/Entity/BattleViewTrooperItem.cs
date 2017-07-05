using UnityEngine;
using System.Collections;

public class BattleViewTrooperItem : MonoBehaviour {

	private UISprite avatarSprite;
	private UILabel trooperNumLabel;
	public BattleTrooperData data{
		set{
			avatarSprite.spriteName = value.tid;
			trooperNumLabel.text = value.num.ToString();
		}
	}
	public void Init()
	{
		avatarSprite = transform.Find ("avatar").GetComponent<UISprite>();
		trooperNumLabel = transform.Find ("num").GetComponent<UILabel> ();
	}
}
