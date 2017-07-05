using UnityEngine;
using System.Collections;
using BoomBeach;

public class WarriorController :TrooperController {	

	/// <summary>
	/// 检测是否开始攻击;
	/// </summary>
	public override bool CheckBeginAttack()	{
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{

			//战士检测点为寻路目标点;
			if(charInfo.AttackBuildInfo!=null&&(charInfo.Position - charInfo.Dest).magnitude<= charInfo.AttackRange)
			{
				//重新计算兵种方向;
				CaclCharInfoDirect(BattleController.GetBuildCenterPosition(charInfo.AttackBuildInfo));

				//默认兵种开始攻击后，改变状态为站立;
				CMDStand ();

				//行走结束，上传路径结点(重机枪兵需重写，不用上传，等攻击过程中检测成立后上传);
				if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
					BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);
				
				return true;
			}
			else
			{
				if(charInfo.AttackBuildInfo==null||charInfo.AttackBuildInfo.IsDead)
				{
					if(!charInfo.IsOnlyMove)
					{
						charInfo.AttackBuildInfo = null;
						charInfo.IsFindDest = false;
						attackTimeCounter = 0; //攻击计时器归零;

						//行走结束，上传路径结点(重机枪兵需重写，不用上传，等攻击过程中检测成立后上传);
						if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
							BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);					
					
						CMDFindDest(); //重新查找目标;
					}
				}
				return false;
			}
		}
		else
		{
			return false;  //回放场景不作检测，等待回放控制器通知;
		}
	}

	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public override void DoAttack()
	{
		if(attackTimeCounter==0)
		{
			//播放攻击动画;
			PlayAttack();
			//计算每次攻击的伤害;
			float damage = charInfo.Damage * charInfo.AttackSpeed;

			EffectController.PlayEffect("Model/Effect/"+charInfo.trooperData.csvInfo.AttackEffect,charInfo.AttackDest);

			charInfo.AttackBuildInfo.buildCtl.CMDUnderAttack(damage);
			charInfo.AttackBuildInfo.buildCtl.CMDShake();

			float lifeLeach = charInfo.trooperData.csvInfo.LifeLeach * charInfo.AttackSpeed;
			lifeLeach = 0 - lifeLeach;
			charInfo.trooperCtl.CMDUnderAttack(lifeLeach);
			AudioPlayer.Instance.PlaySfx("native_attack_04");
		}
		if(attackTimeCounter>=charInfo.AttackSpeed)
		{
			attackTimeCounter = 0f;
		}
		else
		{
			attackTimeCounter+=Time.deltaTime;
		}
	}

	public override void DoMove()
	{
		AudioPlayer.Instance.PlaySfx("assault_unit_move_loop_01");
		base.DoMove();
	}

}
