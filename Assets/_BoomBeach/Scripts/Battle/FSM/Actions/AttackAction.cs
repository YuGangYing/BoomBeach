using UnityEngine;
using System.Collections;

public class AttackAction :BaseAction{

	public const string actionName = "attackAction";

	public int attackInterval = 15;//frame
	private int mNextAttackFrame = 0;
	public Transform target;
	public tk2dSpriteAnimator anim;
	public override void OnAwake(){
		anim = mSelf.GetComponentInChildren<tk2dSpriteAnimator> ();
	}

	public override void OnEnter(){
		mNextAttackFrame = attackInterval + Time.frameCount;
		PlayAnim ("Stand",target.position - mSelf.transform.position);
	}

	public override void OnUpdate(){
		if(mNextAttackFrame <= Time.frameCount){
			PlayAnim ("Attack",target.position - mSelf.transform.position);
			mNextAttackFrame = attackInterval + Time.frameCount;
			TestAStarAI unit = target.GetComponent<TestAStarAI> ();
			if (unit.hp <= 0) {
				Fsm.currentStateName = TestAStarController.scaneStateName;
			} else {
				mSelf.GetComponent<TestAStarAI> ().StartCoroutine (_Attack());
			}
		}
	}

	/// <summary>
	/// 播放行走动画;
	/// </summary>
	public virtual void PlayAnim(string type,Vector3 dir)
	{
		tk2dSprite _sprite = anim.GetComponent<tk2dSprite>();
		//以下是动画播放;
		Direct direct = ComplexMathf.CaclCharInfoDirect(new Vector2(dir.x,dir.z));
		if (direct == Direct.LEFT || direct == Direct.LEFTDOWN || direct == Direct.LEFTUP || direct == Direct.UP) 
			_sprite.FlipX = true;
		else
			_sprite.FlipX = false;
		Globals.PlayTk2dAnim (type,direct,anim);
	}


	IEnumerator _Attack(){
		float t = 0;
		Vector3 startPos = mSelf.transform.position;
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		go.transform.localScale = Vector3.one * 0.2f;
		Vector3 endPos = target.position;
		while(t<1){
			t += Time.deltaTime * 2;
			go.transform.position = Vector3.Lerp (startPos,endPos,t);
			yield return null;
		}
		target.GetComponent<TestAStarAI> ().hp -= 5;
	}

	public override void OnExit(){
		
	}


}
