using UnityEngine;
using System.Collections;

public class BuildBtnSize : MonoBehaviour {

	private Vector3 baseScale;

	void Awake()
	{
		baseScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		float max = CameraOpEvent.Instance.maxField - CameraOpEvent.Instance.minField;
		float current =  CameraOpEvent.Instance.currentField - CameraOpEvent.Instance.minField;
		float baseScaleRatio = CameraOpEvent.Instance.btnScaleRatio;
		Vector3 scale = baseScale * (baseScaleRatio + current / max * CameraOpEvent.Instance.btnScaleStep);
		transform.localScale = scale;
	}
}
