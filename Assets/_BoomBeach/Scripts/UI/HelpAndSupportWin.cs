using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;

public class HelpAndSupportWin : MonoBehaviour {
	

	private bool isInit;

	private UITable table;

	public void Init()
	{
		if(!isInit)
		{
			table = transform.Find("help_panel/Table").GetComponent<UITable>();

			isInit = true;
		}
		table.Reposition();
	}

	

	public void OnClickReprot () {
		//previous.gameObject.SetActive(false);
		//helpAndSupport.SetActive(true);
		Debug.Log("OnClickReprot");
		//table.Reposition();

		//关闭窗口;
		if(PopWin.current!=null)PopWin.current.CloseTween();
	}

}
