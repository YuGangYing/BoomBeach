using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//医疗弹;
public class HealAmmoController : ProjectileController {


	//打击次数;
	private int NumberOfHits;

	private float TimeBetweenHits;

	private bool isArraival;

	public override void DoAttack()
	{

		if(!isArraival)
		{ 
			EffectController e = EffectController.PlayEffect("Model/Effect/HealRing",projectileInfo.AttackPoint,projectileInfo.FireSpellInfo.NumberOfHits*projectileInfo.FireSpellInfo.TimeBetweenHits,true);
			e.stopDelay = 3f;
			projectileInfo.transform.Find("sprite").gameObject.SetActive(false);
			isArraival = true;
		}

		if(NumberOfHits<=projectileInfo.FireSpellInfo.NumberOfHits)
		{
			if(TimeBetweenHits==0)
			{
				//开始打击（加血）;
				//计算当前打击点;

				Vector3 randPoint = Globals.GetRandPointInCircle(projectileInfo.AttackPoint,projectileInfo.FireSpellInfo.RandomRadius);

				//范围伤害;
				foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
				{
					if(!charInfo.isDead&&(randPoint - charInfo.Position).magnitude<=projectileInfo.DamageRadiusTrooper)
					{
						charInfo.trooperCtl.CMDUnderAttack(projectileInfo.Damage);
					}
				}
				NumberOfHits++;
				TimeBetweenHits+=Time.deltaTime;
			}
			else
			{
				if(TimeBetweenHits>=projectileInfo.FireSpellInfo.TimeBetweenHits)
				{
					TimeBetweenHits = 0;
				}
				else
				{
					TimeBetweenHits+=Time.deltaTime;
				}
			}

			AudioPlayer.Instance.PlaySfx("healing_01");
		}
		else
		{
			projectileInfo.IsBoom = true;
			isArraival = false;
			projectileInfo.transform.Find("sprite").gameObject.SetActive(true);
		}

	}
}
