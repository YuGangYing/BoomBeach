using UnityEngine;
using System.Collections;

public class ResourceBarPop : MonoBehaviour 
{

	private static ResourceBarPop instance;
	public static ResourceBarPop Instance{
		get{ return instance;}
	}
	void Awake()
	{
		instance = this;
	}

	public PopUI popBox;

	public UIButton button;

	public void setPop(UIButton btn)
	{
		popBox.transform.parent = btn.transform;
		popBox.transform.localPosition = new Vector3 (-40f, -50f, 0f);	
		button = btn;
	}

}
