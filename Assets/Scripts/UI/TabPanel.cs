using UnityEngine;
using System.Collections;

public delegate void OnSelect (int idx);
public class TabPanel : MonoBehaviour {

	public static TabPanel current;

	public TabItem[] Tabs;
	public void Init()
	{

		Tabs = transform.Find ("Tabs").GetComponentsInChildren<TabItem> (true);
		for(int i=0;i<Tabs.Length;i++)
		{
			Tabs[i].Init();
			Tabs[i].idx = i;
		}
		//Debug.Log(Tabs.Length);
	}

	public void SelectTab(int idx,OnSelect onselect)
	{
		for(int i=0;i<Tabs.Length;i++)
		{
			if(i==idx)
				Tabs[i].IsChecked = true;
			else
				Tabs[i].IsChecked = false;
		}

		if(onselect!=null)
			onselect (idx);
	}
}
