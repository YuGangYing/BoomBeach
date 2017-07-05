using UnityEngine;
using System.Collections;

public class SpellUpdater : MonoBehaviour {

	public SpellInfo spellInfo;
	
	void Awake(){
		spellInfo = GetComponent<SpellInfo> ();
		DeployTime = 0f;
		TimeBetweenHits = 0f;
		NumberOfHits = 0;
	}


	//发射初延时;
	private float DeployTime;

	//打击间隔计时器;
	private float TimeBetweenHits;

	//打击次数;
	private int NumberOfHits;

	// 每次火炮发射的发射方式;
	void Update () {
		if(DeployTime>=spellInfo.DeployTime)
		{
			//开始发射子弹;
			if((spellInfo.spellData.TID=="TID_BARRAGE"&&NumberOfHits<spellInfo.NumberOfHits)
			   ||NumberOfHits==0)
			{
				if(TimeBetweenHits==0)
				{
					//发射单发子弹;
					float damage = spellInfo.Damage;
					ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(spellInfo.spellData.Projectile);
					
					projectileInfo.projectileData = Globals.projectileData[spellInfo.spellData.Projectile];
					
					//初始化子弹起点;
					Vector3 startPoint = spellInfo.transform.position;
					projectileInfo.transform.position = startPoint;

					EffectController.PlayEffect("Model/Effect/GunBoatFireSmoke",startPoint);
					
					projectileInfo.FireSpellInfo = spellInfo;
					projectileInfo.HitEffect = spellInfo.spellData.HitEffect;

					projectileInfo.AttackPoint = spellInfo.AttackPoint;

					if(spellInfo.spellData.TID=="TID_BARRAGE")
					{					
						projectileInfo.AttackPoint = Globals.GetRandPointInCircle(projectileInfo.AttackPoint,spellInfo.RandomRadius);
					}
					
					projectileInfo.AttackType = 0;
					projectileInfo.Damage = damage;
					projectileInfo.DamageRadius = spellInfo.DamageRadius;
					projectileInfo.DamageRadiusTrooper = spellInfo.DamageRadiusTrooper;
					projectileInfo.BattleInit();
					
					projectileInfo.projectileCtl.CMDFire();

					//end发射;
					NumberOfHits++;
					TimeBetweenHits+=Time.deltaTime;
				}
				else
				{
					if(TimeBetweenHits>=spellInfo.TimeBetweenHits)
					{
						TimeBetweenHits = 0f;
					}
					else
					{
						TimeBetweenHits+=Time.deltaTime;
					}
				}

			}
			else
			{
				//打击完成，销毁发射器;
				Destroy(gameObject);
			}
		}
		else
		DeployTime += Time.deltaTime;
	}
}
