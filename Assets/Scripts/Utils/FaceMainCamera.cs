using UnityEngine;
using System.Collections;

public class FaceMainCamera : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		transform.eulerAngles = new Vector3(45f,45f,0f);
	}

}
