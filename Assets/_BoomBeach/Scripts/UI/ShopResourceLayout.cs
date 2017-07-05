using UnityEngine;
using System.Collections;

public class ShopResourceLayout : MonoBehaviour {


	private Vector3[] ItemCountPos;

	void Awake(){
		ItemCountPos = new Vector3[4];
		ItemCountPos [0] = new Vector3 (-80, 65, 0);
		ItemCountPos [1] = new Vector3 (-200, 65, 0);
		ItemCountPos [2] = new Vector3 (-200, 95, 0);
		ItemCountPos [3] = new Vector3 (-200, 95, 0);
	}
	private int itemCount;
	public int ItemCount
	{
		set{ 
			itemCount = value; 
			if(ItemCount>0&&ItemCount<4&&ItemCountPos!=null)
			transform.localPosition = ItemCountPos [ItemCount-1];  	
		}
		get{return itemCount; }
	}



}
