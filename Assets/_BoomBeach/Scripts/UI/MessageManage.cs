using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageManage : MonoBehaviour {

	private static MessageManage instance;
	public static MessageManage Instance{
		get{ return instance; }
	}
	void Awake(){
		instance = this;
	}


	public GameObject MessageItem;
	
	private List<UILabel> MessageLabels = new List<UILabel>();

	public int max = 5;

	public float lineHeight = 70f;


	public void ShowMessage(string text)
	{
		GameObject messageObj = Instantiate (MessageItem) as GameObject;
		UILabel label = messageObj.GetComponent<UILabel> ();
		label.text = text;

		label.transform.parent = transform;
		label.transform.localScale = Vector3.one * 2;
		MessageLabels.Add (label);

		while(MessageLabels.Count>max)
		{
			UILabel item = MessageLabels[0];
			MessageLabels.Remove(item);
			if(item!=null)
			Destroy(item.gameObject);

		}


		for(int i=MessageLabels.Count-1,j=0;i>=0;i--,j++)
		{
			UILabel currentItem = MessageLabels[i];
			if(currentItem!=null)
			currentItem.transform.localPosition = new Vector3(currentItem.transform.localPosition.x,j*lineHeight,currentItem.transform.localPosition.z);
		}

	}


}
