using UnityEngine;
using System.Collections;

public class BattleItem : MonoBehaviour {

	public bool isClicked;
	private GameObject activeObj;  //当前的白边;
	private bool _current;
	public bool current{
		set{
			_current = value;
			activeObj.gameObject.SetActive(value);
		}
		get{
			return _current;
		}
	}
	protected void Init()
	{
		activeObj = transform.Find ("ActLine").gameObject;
	}

	private BattleTrooperData _btd;
	public BattleTrooperData btd
	{
		set{ 
			_btd = value;
			_btd.uiItem = this; 
		}
		get{
			return _btd;
		}
	}

	private bool isdisable;
	public bool isDisabled{
		set{
			isdisable = value;
            /* Old 代码 ，for NGUI
			if(isdisable)
			{
				GetComponent<UIButton>().tweenTarget = null;
				UISprite[] sprites = GetComponentsInChildren<UISprite>();
				for(int i=0;i<sprites.Length;i++)
				{

					if(sprites[i].atlas.name=="AvatarAltas")
					{
						sprites[i].color = Color.black;
					}
				}
				GetComponent<UIButton>().isEnabled = false;

			}
			else
			{
				GetComponent<UIButton>().tweenTarget = transform.Find("Bg").gameObject;
				UISprite[] sprites = GetComponentsInChildren<UISprite>();
				for(int i=0;i<sprites.Length;i++)
				{
					if(sprites[i].atlas.name=="AvatarAltas")
					{
						sprites[i].color = Color.white;
					}
				}
				GetComponent<UIButton>().isEnabled = true;
			}
            */
		}
		get{
			return isdisable;
		}
	}

	public int WeaponCost;
}
