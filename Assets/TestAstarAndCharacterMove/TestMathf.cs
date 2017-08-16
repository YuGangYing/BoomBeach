using UnityEngine;
using System.Collections;

public class TestMathf : MonoBehaviour {

	public Vector2 startPos = new Vector2(10,10);
	public Vector2 endPos = new Vector2 (7,5);
	public Vector2 centerPos = new Vector2(5,6);
	public float radius = 5;
	// Update is called once per frame
	void Update () {
		
		Debug.DrawLine (startPos,endPos);
		Debug.DrawLine (centerPos,endPos);
		Debug.DrawLine (startPos,centerPos);


		float f =  (centerPos - startPos).magnitude * Vector2.Dot ((centerPos - startPos).normalized,(endPos - startPos).normalized);
		Vector2 pos = startPos + (endPos - startPos).normalized * f;
		if (Vector2.Distance (pos, centerPos) > radius)
			return;
		Debug.DrawLine (centerPos,pos,Color.red);
		float d = Mathf.Log (radius * radius - (pos - centerPos).sqrMagnitude, 2);
		Vector2 intersection = startPos + (endPos - startPos).normalized * (f - d);
		Debug.DrawLine (centerPos,intersection,Color.green);
		//Debug.DrawLine (Vector3.Cross(startPos,endPos),Vector3.zero);
	}



}
