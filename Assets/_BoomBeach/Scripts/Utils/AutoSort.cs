using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AutoSortDirection{
	TOP,BOTTOM,LEFT,RIGHT
}

public class AutoSort : MonoBehaviour {

	public AutoSortItem[] autoSortList;
	public AutoSortDirection anchor;
	public float padding;

	void Awake()
	{
		for (int i=0; i<autoSortList.Length; i++) 
		{
			autoSortList[i].box = this;
		}
		resort ();
	}

	public void resort()
	{
		AutoSortItem parentItem = null;
		AutoSortItem firstItem = null;
		if(autoSortList.Length>0)
		firstItem = autoSortList[0];
		for (int i=0; i<autoSortList.Length; i++) 
		{
			if(autoSortList[i].gameObject.activeSelf)
			{
				AutoSortItem item = autoSortList[i];
				if(parentItem==null)
				{
					parentItem = item;
					item.transform.localPosition = firstItem.transform.localPosition;
				}
				else
				{
					item.parentItem = parentItem;
					item.sort(anchor,padding);
					parentItem = item;
				}
			}
		}
	}
	
}
