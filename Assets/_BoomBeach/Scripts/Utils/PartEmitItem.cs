using UnityEngine;
using System.Collections;

public class PartEmitItem : MonoBehaviour {

	public Vector3 begin;
	public Transform dest;
	public Vector3 end;
	public PartEmitter emitter;
	private float speed=0.1f;
	private bool isReachEnd;

	public int resourceCount;


	// Update is called once per frame
	void Update () {



		Vector3 screenPos = PartEmitObj.Instance.UICamera.WorldToScreenPoint (dest.transform.position);
		Vector3 destPos = ScreenToWorldPos(screenPos);


		Vector3 pos = Vector3.zero;
		if(isReachEnd)
		{
			speed += Time.deltaTime;
			pos = Vector3.MoveTowards(transform.position,destPos,speed*Globals.TimeRatio);
		}
		else
		{
			pos = Vector3.MoveTowards(transform.position,end,speed*Globals.TimeRatio);
			if(Mathf.Approximately((pos-end).magnitude,0)&&!isReachEnd)
			{
				isReachEnd = true;
			}
		}


		transform.position = pos;
		if(Mathf.Approximately((pos-destPos).magnitude,0))
		{
			emitter.parts.Remove(this);
			emitter.NotifyPartReached(resourceCount);
			Destroy(gameObject);

		}
	}


	Vector3 ScreenToWorldPos(Vector3 screenPos)
	{
		Ray ray =  PartEmitObj.Instance.MainCamera.ViewportPointToRay(PartEmitObj.Instance.MainCamera.ScreenToViewportPoint(screenPos));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit,1000f,1<<12))
		{
			return hit.point;
		}
		return Vector3.zero;
	}
}
