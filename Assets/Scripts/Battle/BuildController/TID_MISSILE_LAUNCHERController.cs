using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TID_MISSILE_LAUNCHERController:BuildController {



	private int NumberOfHits; //每轮打击的次数;

	private float ReloadTimeCounter;  //装载计时器;

	private List<CharInfo> findTrooperList;

	public override void Init()
	{
		base.Init ();
		findTrooperList = null;
		NumberOfHits = 0;
		ReloadTimeCounter = 0;
	}

	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public override void DoAttack()
	{


		if(NumberOfHits<buildInfo.csvInfo.ShotsBeforeReload&&ReloadTimeCounter>=buildInfo.csvInfo.ReloadTime/1000f)
		{
			if(attackTimeCounter>=buildInfo.AttackSpeed)
			{
				//实现发射子弹;
				weapon = buildInfo.GetComponentInChildren<Weapon>();
				Vector2 beginPoint = new Vector2(weapon.transform.position.x,weapon.transform.position.z);
				Vector2 endPoint = new Vector2(buildInfo.AttackCharInfo.Position.x,buildInfo.AttackCharInfo.Position.z);
				weaponAngel = Globals.CaclDegree(beginPoint,endPoint);

				int idx = weapon.angleToIdx (weaponAngel);
				if(NumberOfHits==0)
				{
					findTrooperList = new List<CharInfo>();
					BinaryHeap<BattleFindItem>  findList = new BinaryHeap<BattleFindItem> ();			
					foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
					{
						float distance = (BattleController.GetBuildCenterPosition(buildInfo) - charInfo.Position).magnitude;

						Vector2 beginPoint1 = new Vector2(weapon.transform.position.x,weapon.transform.position.z);
						Vector2 endPoint1 = new Vector2(charInfo.Position.x,charInfo.Position.z);
						float weaponAngel1 = Globals.CaclDegree(beginPoint1,endPoint1);
						
						int idx1 = weapon.angleToIdx (weaponAngel1);

						if(charInfo!=null&&!charInfo.isDead&&!charInfo.isInSmoke&&distance<=buildInfo.AttackRange&&distance>=buildInfo.MinAttackRange&&idx1==idx)
						{
							BattleFindItem eachitem = new BattleFindItem();
							eachitem.distance = Mathf.RoundToInt(distance*1000);
							eachitem.item = charInfo.gameObject;
							findList.Add(eachitem);  
						}
					}
					while(findList.Count>0&&findTrooperList.Count<3)
					{
						GameObject trooper = findList.Remove().item;
						findTrooperList.Add(trooper.GetComponent<CharInfo>()); 
					}
				}


				weapon.onAim = DoFire;
				weapon.aim(weaponAngel);

				
			}
			if(attackTimeCounter>=buildInfo.AttackSpeed)
			{
				attackTimeCounter = 0f;
			}
			else
			{
				attackTimeCounter+=Time.deltaTime;
			}
		}
		else
		{
			NumberOfHits = 0;
			if(ReloadTimeCounter>=buildInfo.csvInfo.ReloadTime/1000f)
			{
				ReloadTimeCounter = 0f;
			}
			ReloadTimeCounter+=Time.deltaTime;
		}



	}

	//用于瞄准后执行，或直接调用;
	protected override void DoFire()
	{
		if (buildInfo.AttackCharInfo == null)
			return;

		int idxt = NumberOfHits % 3;
		if(idxt<findTrooperList.Count)
		{
			buildInfo.AttackCharInfo = findTrooperList[idxt];
		}



		float damage = buildInfo.BattleDamage * buildInfo.AttackSpeed;
		ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(buildInfo.csvInfo.Projectile);
		
		projectileInfo.projectileData = Globals.projectileData[buildInfo.csvInfo.Projectile];
		
		
		
		//初始化子弹起点,需要重设为炮口发射点;
		Vector3 startPoint = BattleController.GetBuildCenterPosition (buildInfo);
		int idx = weapon.angleToIdx (weaponAngel);
		Transform fp = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx);
		if(fp!=null)
			startPoint = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx).position;

		startPoint = Globals.GetRandPointInCircle (startPoint, projectileInfo.projectileData.StartOffset / 100f /2f);
		projectileInfo.transform.position = startPoint;
		EffectController.PlayEffect("Model/Effect/"+buildInfo.csvInfo.AttackEffect,startPoint);
		
		projectileInfo.AttackCharInfo = buildInfo.AttackCharInfo;
		projectileInfo.FireBuildInfo = buildInfo;
		projectileInfo.HitEffect = buildInfo.csvInfo.HitEffect;
		//初始化攻击点;
		projectileInfo.AttackPoint = buildInfo.AttackCharInfo.Position;
		

		
		projectileInfo.AttackType = 1;
		projectileInfo.Damage = damage;
		projectileInfo.DamageRadius = buildInfo.DamageRadius;
		projectileInfo.DamageRadiusTrooper = buildInfo.DamageRadius;
		projectileInfo.BattleInit();
		
		projectileInfo.projectileCtl.CMDFire();
		NumberOfHits++;
	}
}
