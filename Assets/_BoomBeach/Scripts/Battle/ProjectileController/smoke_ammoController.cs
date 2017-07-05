using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//烟雾弹;
using BoomBeach;


public class smoke_ammoController : ProjectileController {

	private float LifeTime; //过了有效期，重新通知新的操作;

	private int NumberOfHits;

	private float TimeBetweenHits;

	void DoSmoke(CharInfo charInfo)
	{
		//记录回放数据;
		if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			charInfo.SmokeProjectile = projectileInfo;

			if(!charInfo.isInSmoke)
			{
				charInfo.isInSmoke = true;
				ReplayNodeData rnd = new ReplayNodeData();
				rnd.SelfID = charInfo.Id;
				rnd.SelfType = EntityType.Trooper;
				rnd.IsInSmoke = 1;
				BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
			}
		}

	}

	void FreeSmoke(CharInfo charInfo)
	{
		//记录回放数据;
		if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			if(charInfo.isInSmoke&&charInfo.SmokeProjectile==projectileInfo)
			{
				charInfo.isInSmoke = false;
				ReplayNodeData rnd = new ReplayNodeData();
				rnd.SelfID = charInfo.Id;
				rnd.SelfType = EntityType.Trooper;
				rnd.IsInSmoke = -1;
				BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
			}
		}
		
	}

	private GameObject spriteObj;

	public override void DoAttack()
	{
		if(NumberOfHits==0)
		{
			AudioPlayer.Instance.PlaySfx("smoke_screen_01");
		}
		if(spriteObj==null)spriteObj = projectileInfo.transform.Find("sprite").gameObject;
		if(spriteObj.activeSelf)spriteObj.SetActive(false);
		if(NumberOfHits<projectileInfo.FireSpellInfo.NumberOfHits)
		{
			if(TimeBetweenHits==0)
			{
				foreach(CharInfo trooper in BattleData.Instance.AllocateTrooperList)
				{
					if((projectileInfo.transform.position - trooper.Position).magnitude<=projectileInfo.DamageRadiusTrooper)
					{
						DoSmoke(trooper);
					}
				}
				EffectController.PlayEffect("Model/Effect/UnderSmoke",projectileInfo.transform.position);
				NumberOfHits++;
			}
		}


		TimeBetweenHits += Time.deltaTime;
		if(TimeBetweenHits>projectileInfo.FireSpellInfo.BoostTime)
		{
			TimeBetweenHits = 0f;
			foreach(CharInfo trooper in BattleData.Instance.AllocateTrooperList)
			{
				if((projectileInfo.transform.position - trooper.Position).magnitude>projectileInfo.DamageRadiusTrooper
				   ||NumberOfHits>=projectileInfo.FireSpellInfo.NumberOfHits)
				FreeSmoke(trooper);
			}

			if(NumberOfHits>=projectileInfo.FireSpellInfo.NumberOfHits)
			{
				projectileInfo.IsBoom = true;
				spriteObj.SetActive(true);
			}
		}

		

	}
}
