using UnityEngine;
using System.Collections;

public class TrooperUpdater: MonoBehaviour {

	public CharInfo charInfo;



	void Awake(){
		charInfo = GetComponent<CharInfo> ();
	}

	private float shakeTimeCount=0f;
	private float shakeSpan = 2f;
	private float lightTimeCount = 0f;
	private float lightSpan = 0.5f;

	// Update is called once per frame
	void Update () {
		if (charInfo.isDead) {		
			//死亡的相关操作;
			EffectController.PlayEffect("Model/Effect/"+charInfo.trooperData.csvInfo.DieEffect,charInfo.Position);
			if(charInfo.trooperData.tid=="TID_RIFLEMAN")
			{
				AudioPlayer.Instance.PlaySfx("assault_troop_die_04");
			}
			if(charInfo.trooperData.tid=="TID_HEAVY")
			{
				AudioPlayer.Instance.PlaySfx("heavy_die_05");
			}
			if(charInfo.trooperData.tid=="TID_ZOOKA")
			{
				AudioPlayer.Instance.PlaySfx("bazooka_die_04");
			}
			if(charInfo.trooperData.tid=="TID_WARRIOR")
			{
				AudioPlayer.Instance.PlaySfx("native_die_04");
			}
			if(charInfo.trooperData.tid=="TID_TANK")
			{
				AudioPlayer.Instance.PlaySfx("tank_die_01");
			}
			//EffectController.PlayEffect("Model/HitEffect/Heavy Gunner Die",charInfo.Position);
			gameObject.SetActive(false);
			return;
		}
		if(charInfo.isShake)
		{
			float currentSpeed = charInfo.shakeSpeed * Globals.TimeRatio;
			if(charInfo.moveStepCount+currentSpeed>=charInfo.moveStep)
			{
				currentSpeed = charInfo.moveStep - charInfo.moveStepCount;
			}
			Transform main = charInfo.transform.Find("characterPos");
			Vector3 pos = main.localPosition;
			main.localPosition = new Vector3(pos.x+charInfo.shakeDirect*currentSpeed,pos.y,pos.z+charInfo.shakeDirect*currentSpeed);
			charInfo.moveStepCount+=currentSpeed;
			if(charInfo.moveStepCount==charInfo.moveStep)
			{
				charInfo.shakeDirect*=-1;
				charInfo.moveStepCount = 0;
			}
			charInfo.shakeTimeCount+=Time.deltaTime;
			if(charInfo.shakeTimeCount>=charInfo.shakeTime)
			{
				main.localPosition = charInfo.shakeOriginPos;
				charInfo.shakeTimeCount = 0f;
				charInfo.isShake = false;
			}			
		}
		if (charInfo.isInStun)
		{
			if(shakeTimeCount==0)
			{
				charInfo.trooperCtl.CMDShake();
			}
			shakeTimeCount+=Time.deltaTime;
			if(shakeTimeCount>=shakeSpan)
			{
				shakeTimeCount = 0f;
			}
			if(lightTimeCount==0)
			{
				EffectController.PlayEffect("Model/Effect/StunLight",charInfo.Position);
				shakeSpan = Random.Range(0.5f,1.5f);
			}
			lightTimeCount+=Time.deltaTime;
			if(lightTimeCount>=lightSpan)lightTimeCount = 0f;
			charInfo.anim.Stop();
			return;
		}
		//以下是指令控制;
		if(charInfo.State==AISTATE.STANDING)
		{
			charInfo.trooperCtl.PlayStand();
		}
		if(!BattleData.Instance.BattleIsEnd&&charInfo.State==AISTATE.FINDINGDEST)
		{
			charInfo.trooperCtl.PlayStand();
			if(charInfo.IsFindDest)//如果找到了目的地，则开始搜寻到目的地的路径
			{
				charInfo.trooperCtl.CMDFindPath();  //已寻到攻击目标建筑，开始寻路;
			}
		}
		if(charInfo.State==AISTATE.FINDINGPATH)
		{
			charInfo.trooperCtl.PlayStand();			
			if((charInfo.path!=null&&charInfo.path.Count>0)||charInfo.IsOnlyMove)
			{
				if(charInfo.IsOnlyMove)
					Debug.Log("yes");
				charInfo.trooperCtl.CMDMove();  //找到路径，开始行走;
			}
		}
		if(charInfo.State==AISTATE.MOVING)
		{
			charInfo.trooperCtl.PlayMove();
			charInfo.trooperCtl.DoMove();
			if(!BattleData.Instance.BattleIsEnd&&charInfo.AttackState!=AISTATE.ATTACKING&&charInfo.trooperCtl.CheckBeginAttack ())
			{
				charInfo.trooperCtl.CMDAttack();
			}
		}
		if(!BattleData.Instance.BattleIsEnd&&charInfo.AttackState == AISTATE.ATTACKING)
		{
			if(charInfo.isInSmoke)
			{
				charInfo.trooperCtl.PlayStand();
				return;
			}
			//攻击;
			if(charInfo.trooperCtl.CheckEndAttack())
			{
				charInfo.trooperCtl.CMDFindDest();
			}
			else
			{
				charInfo.trooperCtl.DoAttack();
			}
		}
	}
}
