using UnityEngine;
using System.Collections;

public class PopUI : MonoBehaviour {

	public void pop(bool tag)
	{
		if (!tag) 
		{
			gameObject.SetActive(tag);
			Globals.popUIList.Remove(this);
		}
		else
		{
			gameObject.SetActive(tag);
			Globals.popUIList.Add(this);
		}	
	}

	public static void closePops(PopUI excludePop)
	{
		if (Globals.popUIList != null){
			for (int i=0; i<Globals.popUIList.Count; i++) {
				if(excludePop==null||excludePop!=Globals.popUIList[i])
				Globals.popUIList[i].pop(false);		
			}
		}
	}
}
