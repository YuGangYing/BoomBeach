using UnityEngine;
using System.Collections;

public class AutoSortItem : MonoBehaviour {
	public AutoSortItem parentItem;
	public AutoSort box;

	public void sort(AutoSortDirection anchor,float padding)
	{
		if (parentItem != null) 
		{
			Vector3 offset = Vector3.zero;

			if(anchor==AutoSortDirection.TOP)
			{
				offset = new Vector3(0,padding,0);
			}
			else if(anchor==AutoSortDirection.BOTTOM)
			{
				offset = new Vector3(0,-padding,0);
			}
			else if(anchor==AutoSortDirection.LEFT)
			{
				offset = new Vector3(-padding,0,0);
			}
			else if(anchor==AutoSortDirection.RIGHT)
			{
				offset = new Vector3(padding,0,0);
			}

			transform.localPosition = parentItem.transform.localPosition+offset;
		}
	}

	void OnEnable()
	{
		if(box!=null)
		box.resort ();
	}

	void OnDisable()
	{
		if(box!=null)
		box.resort ();
	}
}
