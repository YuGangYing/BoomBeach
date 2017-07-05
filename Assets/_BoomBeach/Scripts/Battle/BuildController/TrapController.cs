using UnityEngine;
using System.Collections;
using BoomBeach;

public class TrapController:BuildController {


	/// <summary>
	/// 检测是否要停止攻击;
	/// </summary>
	public override bool CheckEndAttack()
	{
		//一旦触发一定会爆炸;
		return false;
		
	}

	/// <summary>
	/// 查找目标，必需返回buildInfo.AttackCharInfo;
	/// </summary>
	public override void CMDFindDest()
	{
		//仅真实攻击时才寻找攻击目标，回放时由回放数据列中获取;
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{

			findDestTimer+=Time.deltaTime;
			if(findDestTimer>=0.5f)findDestTimer = 0;
			if(findDestTimer>0)return;

			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				if(charInfo!=null&&!charInfo.isDead)
				{
					float distance =(BattleController.GetBuildCenterPosition(buildInfo) - charInfo.Position).magnitude;
					if(distance<=buildInfo.TriggerRadius)
					{
						
						if(buildInfo.AttackCharInfo==null||
						   buildInfo.AttackCharInfo.Id!=charInfo.Id)
						{
							//记录回放数据;
							if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
							{
								ReplayNodeData rnd = new ReplayNodeData();
								rnd.SelfID = buildInfo.BattleID;
								rnd.SelfType = EntityType.Build;
								rnd.AttackState = AISTATE.ATTACKING;
								rnd.AttackType = EntityType.Trooper;
								rnd.AttackID = charInfo.Id;
								BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
							}
						}
						
						buildInfo.AttackCharInfo = charInfo;
						buildInfo.IsFindDest = true;
						buildInfo.State = AISTATE.FINDINGDEST;
						
						
						
						return;
					}
				}
			}
		}				
	}


	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public override void DoAttack()
	{
		//范围伤害;
		if(buildInfo.tid=="TID_TRAP_MINE")
		{
			AudioPlayer.Instance.PlaySfx("mine_explo_01");
		}
		else if(buildInfo.tid=="TID_TRAP_TANK_MINE")
		{
			AudioPlayer.Instance.PlaySfx("tank_mine_01");
		}
		Vector3 pos = BattleController.GetBuildCenterPosition (buildInfo);
		EffectController.PlayEffect("Model/Effect/MineBoom",pos);
		foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
		{
			if(!charInfo.isDead&&(pos - charInfo.Position).magnitude<=buildInfo.DamageRadius)
			{
				charInfo.trooperCtl.CMDUnderAttack(buildInfo.BattleDamage);
			}
		}
		CameraOpEvent.Instance.Shake ();
		buildInfo.IsDead = true;
	}
}
