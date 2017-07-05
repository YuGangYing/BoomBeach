using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeachData : MonoBehaviour {

	public Direct beachDirect; //海滩朝向;

	public float LandCraftBeginLocalY;

	public float LandCraftStopLocalY;

	public int beachBeginLocalX;

	public int beachEndLocalX;

	public Dictionary<int,byte> beachLineTag;

	public void Init()
	{
		beachLineTag = new Dictionary<int, byte> ();

		for(int i = beachBeginLocalX;i<=beachEndLocalX;i+=Globals.landCraftWidth)
		{
			beachLineTag.Add(i,0);
		}

	}


}
