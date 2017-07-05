using UnityEngine;
using System.Collections;
using BoomBeach;

public class SceneUI : MonoBehaviour {

	public SceneStatus[] ShowsScene;

	public void setStatus()
	{
		for(int i=0;i<ShowsScene.Length;i++)
		{
			if(DataManager.GetInstance().sceneStatus==ShowsScene[i])
			{
				gameObject.SetActive(true);
				return;
			}
		}
		gameObject.SetActive (false);
	}

}
