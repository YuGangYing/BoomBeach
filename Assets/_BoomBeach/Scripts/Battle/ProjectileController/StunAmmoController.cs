using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//冰冻弹;
using BoomBeach;


public class StunAmmoController : ProjectileController {

	private float LifeTime; //过了有效期，重新通知新的操作;

	private bool IsFire;

	void DoStun(CharInfo charInfo)
	{
		//记录回放数据;
		if(DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			charInfo.StunProjectile = projectileInfo;
			charInfo.isInStun = true;
			ReplayNodeData rnd = new ReplayNodeData();
			rnd.SelfID = charInfo.Id;
			rnd.SelfType = EntityType.Trooper;
			rnd.IsInStun = 1;
			BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
		}

	}

	void DoStun(BuildInfo buildInfo)
	{
		//记录回放数据;
		if(DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			buildInfo.StunProjectile = projectileInfo;
			buildInfo.IsInStun = true;
			ReplayNodeData rnd = new ReplayNodeData();
			rnd.SelfID = buildInfo.BattleID;
			rnd.SelfType = EntityType.Build;
			rnd.IsInStun = 1;
			BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
		}
	}

	public override void DoAttack()
	{
		if(!IsFire&&!BattleData.Instance.BattleIsEnd)
		{
			AudioPlayer.Instance.PlaySfx("stun_grenade_03");
			IsFire = true;

			projectileInfo.transform.Find("sprite").gameObject.SetActive(false);
			EffectController.PlayEffect("Model/Effect/StunRing",projectileInfo.AttackPoint,1,true);
			EffectController.PlayEffect("Model/Effect/StunFire",projectileInfo.AttackPoint);

			foreach(CharInfo trooper in BattleData.Instance.AllocateTrooperList)
			{
				if((projectileInfo.transform.position - trooper.Position).magnitude<=projectileInfo.DamageRadiusTrooper)
				{
					DoStun(trooper);
				}
			}

			foreach(BuildInfo buildInfo in BattleData.Instance.buildDic.Values)
			{
				if((projectileInfo.transform.position - BattleController.GetBuildCenterPosition(buildInfo)).magnitude<=projectileInfo.DamageRadius)
				{
					DoStun(buildInfo);
				}
			}

		}
		if(LifeTime<=projectileInfo.FireSpellInfo.BoostTime)
		{
			LifeTime+=Time.deltaTime;
		}
		else
		{
			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				foreach(BuildInfo buildInfo in BattleData.Instance.buildDic.Values)
				{
					if(buildInfo.IsInStun&&buildInfo.StunProjectile==projectileInfo)
					{
						buildInfo.IsInStun = false;
						buildInfo.StunProjectile = null;						
						ReplayNodeData rnd = new ReplayNodeData();
						rnd.SelfID = buildInfo.BattleID;
						rnd.SelfType = EntityType.Build;
						rnd.IsInStun = -1;
						BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
						
					}
				}

				foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
				{
					if(charInfo.isInStun&&charInfo.StunProjectile==projectileInfo)
					{
						charInfo.isInStun = false;
						charInfo.StunProjectile = null;
						ReplayNodeData rnd = new ReplayNodeData();
						rnd.SelfID = charInfo.Id;
						rnd.SelfType = EntityType.Trooper;
						rnd.IsInStun = -1;
						BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
						
					}
				}
		
			}
			IsFire = false;
			projectileInfo.transform.Find("sprite").gameObject.SetActive(true);
			projectileInfo.IsBoom = true;
		}
		

	}
}
