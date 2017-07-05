using UnityEngine;
using System.Collections;

public class Islands : MonoBehaviour {

	private static Islands instance;
	public static Islands Instance{
		get{
			return instance;
		}
	}
	public IslandType type;
	public bool useType;
	public IslandData[] islands;
	void Awake()
	{
		instance = this;
	}

}


