using UnityEngine;
using System.Collections;

public class SpawnManager : SingleMonoBehaviour<SpawnManager> {

	public Transform characterContainer;
	public Transform bulletContainer;

	protected override void Awake(){
		base.Awake ();
		//初始化士兵与建筑的容器;
		if(characterContainer==null)
			characterContainer = GameObject.Find ("PCharacters").transform;
		if(bulletContainer==null)
			bulletContainer = GameObject.Find ("PBullets").transform;
	}

}
