using UnityEngine;
using System.Collections;

public class GemPartEmitItem : MonoBehaviour {

	public Vector3 begin;
	public Transform dest;
	public Vector3 end;
	public GemPartEmitter emitter;
	public float speed;
	private bool isReachEnd;
	
	public int resourceCount;

	public Camera mc;
	public Camera uc;
	
	
	// Update is called once per frame
	void Update () {
		
		
		
		Vector3 screenPos = uc.WorldToScreenPoint (dest.transform.position);
		Vector3 destPos = mc.ScreenToWorldPoint (screenPos);
		
		
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
	

}
