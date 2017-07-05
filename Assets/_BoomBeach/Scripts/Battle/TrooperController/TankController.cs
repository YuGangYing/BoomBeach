using UnityEngine;
using System.Collections;

public class TankController:TrooperController {



	private Weapon weapon;

	private float weaponAngel;

	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public override void DoAttack()
	{
		PlayStand ();
		if(attackTimeCounter==0)
		{
			if(weapon==null) weapon = charInfo.GetComponentInChildren<Weapon>();

			Vector2 beginPoint = new Vector2(weapon.transform.position.x,weapon.transform.position.z);
			Vector2 endPoint = new Vector2(charInfo.AttackDest.x,charInfo.AttackDest.z);
			weaponAngel = Globals.CaclDegree(beginPoint,endPoint);

			weapon.onAim = DoFire;
			weapon.aim(weaponAngel);
			

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

	void DoFire()
	{
		float damage = charInfo.Damage * charInfo.AttackSpeed;
		ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(charInfo.trooperData.csvInfo.Projectile);
		
		
		projectileInfo.projectileData = Globals.projectileData[charInfo.trooperData.csvInfo.Projectile];

		int idx = weapon.angleToIdx (weaponAngel);
		
		//初始化子弹起点;
		Vector3 startPoint = charInfo.transform.Find("FirePoint/P"+idx).position;
		EffectController.PlayEffect("Model/Effect/"+charInfo.trooperData.csvInfo.AttackEffect,startPoint);
		//float offset = projectileInfo.projectileData.StartOffset/100f/2f; 
		//float randX = Random.Range(startPoint.x-offset,startPoint.x+offset);
		//float randZ = Random.Range(startPoint.z-offset,startPoint.z+offset);
		//startPoint = new Vector3(randX,0f,randZ);
		projectileInfo.transform.position = startPoint;
		
		
		projectileInfo.AttackBuildInfo = charInfo.AttackBuildInfo;
		projectileInfo.FireCharInfo = charInfo;
		projectileInfo.HitEffect = charInfo.trooperData.csvInfo.HitEffect;
		//初始化攻击点;
		projectileInfo.AttackPoint = charInfo.AttackDest;
		
		projectileInfo.AttackType = 2;
		projectileInfo.Damage = damage;
		projectileInfo.DamageRadius = charInfo.DamageRadius;
		projectileInfo.DamageRadiusTrooper = charInfo.DamageRadius;
		projectileInfo.BattleInit();
		
		projectileInfo.projectileCtl.CMDFire();
		AudioPlayer.Instance.PlaySfx("tank_fire_01");
	}

	public override void DoMove()
	{
		AudioPlayer.Instance.PlaySfx("tank_drive_loop_01");
		base.DoMove();
	}

}
