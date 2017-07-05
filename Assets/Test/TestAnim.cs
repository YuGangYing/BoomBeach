using UnityEngine;
using System.Collections;

public class TestAnim : MonoBehaviour {

	public float delay = 0.2f;
	public float speed = 0.3f;
	// Use this for initialization
	void Start () {
		Animation[] anims = GetComponentsInChildren<Animation> (true);
		StartCoroutine (_Anim(anims));
	}

	IEnumerator _Anim(Animation[] anims){
		foreach(Animation anim in anims){
			anim ["BB"].speed = speed; 
			anim.Play ();
			yield return new WaitForSeconds (delay);
		}
		yield return null;
	}

}
