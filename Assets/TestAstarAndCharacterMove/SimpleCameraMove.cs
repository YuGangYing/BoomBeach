using UnityEngine;
using System.Collections;

public class SimpleCameraMove : MonoBehaviour {

	Vector3 dir;

	//bool btnDown;
	Vector3 btnDownPos;
	Vector3 currentBtnPos;
	//float speed = 3;
	Transform camTrans;
	Vector3 camStartPos;
	//Quaternion qua;
	// Use this for initialization
	void Start () {
		camTrans = Camera.main.transform;
		//qua = new Quaternion (0, Mathf.Sin (-camTrans.eulerAngles.y / 360), 0, Mathf.Cos (-camTrans.eulerAngles.y / 360));
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			//btnDown = true;
			camStartPos = camTrans.position;
			btnDownPos = Input.mousePosition;
		}
	}

	void LateUpdate(){
		if(Input.GetMouseButton(0)){
			Vector3 deltaPos = Input.mousePosition - btnDownPos;
			//Vector3 realDeltaPos = new Vector3 (deltaPos.x, 0, deltaPos.y);
			//realDeltaPos = qua * realDeltaPos;
			//camTrans.position =  Vector3.Lerp(camStartPos,camStartPos + realDeltaPos,0.1f);
			Vector3 forward = camTrans.forward;
			forward.y = 0;
			camTrans.position = Vector3.Lerp(camStartPos,camStartPos - camTrans.right * deltaPos.x - forward.normalized * deltaPos.y,0.1f);
		}
	}
}
