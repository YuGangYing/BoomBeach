using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResearchWin : MonoBehaviour {

	public void BindReaearchWin()
	{
		List<ResearchTid> researchList = Helper.getResearchTidList ();
		Transform container = transform.Find ("GridList");
		
		int i = 0;
		foreach(Transform researchItembox in container)
		{
			if(i<researchList.Count)
			{
				researchItembox.gameObject.SetActive(true);
				ResearchTid tid = researchList[i];
				researchItembox.GetComponent<ResearchItem>().BindData(tid);
			}
			else
			{
				researchItembox.gameObject.SetActive(false);
			}
			i++;
		}
	}
}
