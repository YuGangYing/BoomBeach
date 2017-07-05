using UnityEngine;
using System.Collections;

public class RotateAnimation : MonoBehaviour {
	public float speed = 10;
	
	protected Transform thisTransform;
	
	protected bool canRotate = true;
	
	protected virtual void Awake () {
		thisTransform = transform;
		StopRotationAnimation();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(canRotate){
			thisTransform.Rotate(-Vector3.forward * Time.deltaTime * speed);
		}
	}
	
	public void StartRotationAnimation () {
		canRotate = true;
	}
	
	public virtual void StopRotationAnimation () {
		canRotate = false;
		//thisTransform.localRotation = Quaternion.identity;
	}
}
