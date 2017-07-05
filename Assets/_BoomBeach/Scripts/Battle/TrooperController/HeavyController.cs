using UnityEngine;
using System.Collections;
using PathFinding.Core;
using BoomBeach;


public class HeavyController :TrooperController {

	/// <summary>
	/// 检测是否开始攻击;
	/// </summary>
	public override bool CheckBeginAttack()	{
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{
			if(charInfo.AttackBuildInfo!=null&&(charInfo.Position - charInfo.AttackDest).magnitude<= charInfo.MaxAttackRange+charInfo.AttackBuildInfo.GridCount/2f )
			{							
				//重机枪兵开始攻击时不改变行走状态;

				//重新计算兵种方向;
				//CaclCharInfoDirect(BattleController.GetBuildCenterPosition(charInfo.AttackBuildInfo));
				//CMDStand ();
				//if(charInfo.path!=null)
				//	charInfo.path.Clear();
				//行走结束，上传路径结点(重机枪兵需重写，不用上传，等攻击过程中检测成立后上传);
				//if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
				//	BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData.ToStringArray());
				
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
			//重机枪兵攻击时不播放动画;
			//PlayAttack();
			//发射子弹;
			float damage = charInfo.Damage * charInfo.AttackSpeed;
			ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(charInfo.trooperData.csvInfo.Projectile);

			projectileInfo.projectileData = Globals.projectileData[charInfo.trooperData.csvInfo.Projectile];

			//初始化子弹起点;
			Vector3 startPoint = charInfo.transform.Find("FirePoint/"+charInfo.direction.ToString()).position;
			EffectController.PlayEffect("Model/Effect/"+charInfo.trooperData.csvInfo.AttackEffect,startPoint);
			EffectController.PlayEffect("Model/Effect/"+charInfo.trooperData.csvInfo.AttackEffect2,charInfo.Position);

			float offset = projectileInfo.projectileData.StartOffset/100f/2f; 
			startPoint = Globals.GetRandPointInCircle(startPoint,offset);
			projectileInfo.transform.position = startPoint;
			EffectController.PlayEffect("Model/Effect/bulletshield",startPoint);


			projectileInfo.AttackBuildInfo = charInfo.AttackBuildInfo;
			projectileInfo.FireCharInfo = charInfo;
			projectileInfo.HitEffect = charInfo.trooperData.csvInfo.HitEffect;

			//初始化攻击点;
			float AttackOffset = charInfo.trooperData.csvInfo.DamageSpread / 100f /2f;
			float randAttackX = Random.Range(charInfo.AttackDest.x-AttackOffset,charInfo.AttackDest.x+AttackOffset);
			float randAttackZ = Random.Range(charInfo.AttackDest.z-AttackOffset,charInfo.AttackDest.z+AttackOffset);
			projectileInfo.AttackPoint = new Vector3(randAttackX,0f,randAttackZ);

			projectileInfo.HitEffect = charInfo.trooperData.csvInfo.HitEffect;
			projectileInfo.AttackType = 2;
			projectileInfo.Damage = damage;
			projectileInfo.DamageRadius = charInfo.DamageRadius;
			projectileInfo.DamageRadiusTrooper = charInfo.DamageRadius;
			projectileInfo.BattleInit();

			projectileInfo.projectileCtl.CMDFire();

			AudioPlayer.Instance.PlaySfx("heavy_fire_01");
			//AudioPlayer.Instance.PlaySfx("machinegun_attack_01");

		}
		if(attackTimeCounter>=charInfo.AttackSpeed)
		{
			attackTimeCounter = 0f;
		}
		else
		{
			attackTimeCounter+=Time.deltaTime;
		}

		//检测进入攻击范围站立攻击;
		if(charInfo.State!=AISTATE.STANDING)
		{
			if((charInfo.Position - charInfo.AttackDest).magnitude<= charInfo.AttackRange+charInfo.AttackBuildInfo.GridCount/2f )
			{
				//重新计算兵种方向;
				CaclCharInfoDirect(BattleController.GetBuildCenterPosition(charInfo.AttackBuildInfo));
				
				CMDStand ();
				if(charInfo.path!=null)
					charInfo.path.Clear();

				//行走结束，上传路径结点;
				if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
					BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);
				
			}
		}
	}


	/// <summary>
	/// 播放站立动画;
	/// </summary>
	public override void PlayStand()
	{

		//重机枪兵攻击时无动画，直接站立;
		string clipName = "Attack";
		string direct = charInfo.direction.ToString();
		if(charInfo.direction==Direct.RIGHTUP)
		{
			
			if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Direct.RIGHT.ToString();				
		}
		if(charInfo.direction==Direct.LEFTDOWN)
		{
			if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Direct.LEFT.ToString();
		}
		clipName+=direct.ToString();
		
		if(!charInfo.anim.IsPlaying(clipName))
		{
			if(charInfo.anim.GetClipByName (clipName)!=null)
			charInfo.anim.Play(clipName);
		}

	}


	/// <summary>
	/// 位移脚本;
	/// </summary>
	public override void DoMove()
	{
		AudioPlayer.Instance.PlaySfx("heavy_move_loop_01");
		Vector3 goal = Vector3.zero;
		
		if(charInfo.path!=null&&charInfo.path.Count>0)
		{
			PathFinderNode node = charInfo.path[0]	;
			GridInfo grid = Globals.GridArray[node.X,node.Y];
			
			if(grid.standPoint==Vector3.zero)
				goal = new Vector3((float)node.X+0.5f,0f,(float)node.Y+0.5f) + new Vector3(Random.Range(-0.5f,0.5f),0f,Random.Range(-0.5f,0.5f));
			else
				goal = grid.standPoint;
		}
		else
		{
			goal = charInfo.Dest;
		}
		
		if(charInfo.AttackState==AISTATE.ATTACKING)
		CaclCharInfoDirect(BattleController.GetBuildCenterPosition(charInfo.AttackBuildInfo));
		else
		CaclCharInfoDirect(goal);
		charInfo.Position = Vector3.MoveTowards(charInfo.Position,goal,charInfo.Speed*Globals.TimeRatio);
		
		GridInfo standGrid = Globals.LocateGridInfo(charInfo.Position);
		if(lastGrid==null||lastGrid!=standGrid)
		{
			//还原原经过的遮挡建筑透明度;
			if(lastGrid!=null)
			{
				for(int i=0;i<lastGrid.buildMarks.Count;i++)
				{
					BuildTweener tw = lastGrid.buildMarks[i].buildInfo.GetComponentInChildren<BuildTweener>();
					if(lastGrid.buildMarks[i].buildInfo.behindCharacters.Contains(charInfo))
						lastGrid.buildMarks[i].buildInfo.behindCharacters.Remove(charInfo);
					
					
					if(lastGrid.buildMarks[i].buildInfo.behindCharacters.Count==0
					   &&lastGrid.buildMarks[i].buildInfo.behindBuilds.Count==0&&tw!=null)
					{
						tw.enabled = true;
						tw.Alpha = 1f; 
					}
				}
			}
			
			lastGrid = standGrid;
			//设置当前站立格子的遮挡建筑透明度;
			for(int i=0;i<standGrid.buildMarks.Count;i++)
			{
				BuildTweener tw = standGrid.buildMarks[i].buildInfo.GetComponentInChildren<BuildTweener>();
				
				if(!lastGrid.buildMarks[i].buildInfo.behindCharacters.Contains(charInfo))
					lastGrid.buildMarks[i].buildInfo.behindCharacters.Add(charInfo);
				
				if(tw!=null)
				{
					tw.enabled = true;
					tw.Alpha =  Globals.obstacleAlpha;
				}
			}
			
		}
		
		if((charInfo.Position-goal).magnitude <= charInfo.Speed )
		{
			if(charInfo.path!=null&&charInfo.path.Count>0)
			{
				PathFinderNode node = charInfo.path[0];
				//开始往录像节点中放入行进过的格子;
				if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
				{
					charInfo.walkListReplayData.walkList.add(node);
				}
				charInfo.path.Remove(node);
			}
			else
			{
				//行走到目的地，但却未能触发战斗，重新寻找目标;
				//行走结束，上传路径结点;
				if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
					BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);
				
				if(BattleData.Instance.BattleIsEnd)
				{
					BattleData.Instance.RetreatTrooperList.Add(charInfo);
					charInfo.trooperCtl.CMDStand();
					charInfo.IsRetreat = true;
					charInfo.IsOnlyMove = false;
				}
				else
				{
					charInfo.IsOnlyMove = false;
					charInfo.trooperCtl.CMDFindDest();
				}
			}
		}
	}

}
