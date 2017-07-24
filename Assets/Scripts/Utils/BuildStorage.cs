using UnityEngine;
using System.Collections;

public class BuildStorage : MonoBehaviour {

	public string spritePrefix;
	tk2dSprite sprite;

	void Awake()
	{
		sprite = GetComponent<tk2dSprite> ();
	}

	/**
	 * idx: 0-4;
	 * */
	public void SetSprite(int idx)
	{
		//Debug.Log (spritePrefix+"_"+idx);
		sprite.SetSprite (spritePrefix+"_"+idx);
	}
}
