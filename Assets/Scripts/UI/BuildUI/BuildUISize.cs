using UnityEngine;
using System.Collections;

public class BuildUISize : MonoBehaviour {

	public static bool isZooming = false;

	private Vector3 baseScale;

	void Awake()
	{
		baseScale = transform.localScale;
		resetUISize();
	}


	// Update is called once per frame
	void Update () {
		if (!isZooming)
						return;

		resetUISize();
	}


	void resetUISize()
	{
		float max = CameraOpEvent.Instance.maxField - CameraOpEvent.Instance.minField;
		float current =  CameraOpEvent.Instance.currentField - CameraOpEvent.Instance.minField;
		float baseScaleRatio = CameraOpEvent.Instance.baseScaleRatio;
		Vector3 scale = baseScale * (baseScaleRatio + current / max);
		transform.localScale = scale;
	}
}
