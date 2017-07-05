using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScanAction : BaseAction {

	public const string actionName = "scanAction";
	public List<Transform> scanTargets;
	public int scanInterval = 4;//frame;
	private int mScanInterval = 0;
	private Transform mTrans;

	public override void OnAwake(){
		mTrans = mSelf.transform;
	}

	public override void OnEnter(){
		mScanInterval = scanInterval + Time.frameCount;
	}

	public override void OnUpdate(){
		if(mScanInterval <= Time.frameCount){
			mScanInterval = scanInterval + Time.frameCount;
			float minDist = Mathf.Infinity;
			float dist = 0;
			Transform target = null;
			Vector3 pos = Vector3.zero;
			for(int i=0;i < scanTargets.Count;i++){
				if (scanTargets[i].GetComponent<TestAStarAI> ().hp <= 0)
					continue;
				pos	= mTrans.position - scanTargets [i].position;
				dist = pos.x * pos.x + pos.z * pos.z;
				if(minDist > dist){
					minDist = dist;
					target = scanTargets [i];
				}
			}
			if(target != null){
				Fsm.currentStateName = TestAStarController.moveStateName;
				BaseState state = Fsm.stateDic [TestAStarController.moveStateName];
				MoveAction action = (MoveAction)(state.actionDic[MoveAction.actionName]);
				action.targetPos = target.position;
				action.target = target;
				//Change State
			}
		}
	}

	public override void OnExit(){
		
	}

}
