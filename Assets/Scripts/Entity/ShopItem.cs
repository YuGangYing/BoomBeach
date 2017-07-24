using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopItem {
	public string tid;
	public string tid_level;
	public bool isEnabled;

	public string Name;
	public string Description; //为空不显示;
	public int diamondAmount;  //0不显示;
	public string Buildtime;  //null 不显示;
	public int Buildcount;
	public int BuildMax;  //0不显示;
	public string DisableDescription;
	public List<ShopCost> ShopCosts;


}
