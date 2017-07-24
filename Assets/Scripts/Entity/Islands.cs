using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Islands : SingleMonoBehaviour<Islands>
{

	public IslandType type;
	public bool useType;
	public IslandData[] islands;
	public Dictionary<IslandType,IslandData> islandDic;

	protected override void Awake ()
	{
		base.Awake ();
		islandDic = new Dictionary<IslandType, IslandData> ();
		for (int i = 0; i < islands.Length; i++) {
			islandDic.Add (islands [i].type, islands [i]);
		}
	}

}