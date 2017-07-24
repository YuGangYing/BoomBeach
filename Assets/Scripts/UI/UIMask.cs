using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMask : MonoBehaviour {
	//单例定义;
	private static Transform mask;
	public static Transform Mask{
		get{ return mask; }
	}

	void Awake()
	{
		mask = transform.Find("Center/Mask");

		UIWidget widget = mask.GetComponent<UIWidget> ();
		widget.width = widget.height*Screen.width/Screen.height;

		BoxCollider box = mask.GetComponent<BoxCollider> ();
		box.size = new Vector3 (box.size.y*Screen.width/Screen.height,box.size.y,box.size.z);

		mask.GetComponent<UIButton> ().onClick = new List<EventDelegate> ();
		mask.GetComponent<UIButton> ().onClick.Add (new EventDelegate(this,"TouchMask"));
	}

	void TouchMask()
	{
		if(PopWin.current!=null)
		PopWin.current.CloseTween ();
		if(PopDialog.current!=null)
		PopDialog.current.CloseTween ();
		if (PopPanel.current != null)
		PopPanel.current.CloseTween ();

        //TODO
        UIMask.Mask.gameObject.SetActive(false);
    }
}
