using UnityEngine;
using System.Collections;

public class TID_MACHINE_GUN_NESTController:BuildController {

	//用于瞄准后执行，或直接调用;
	protected override void DoFire()
	{
		if (buildInfo.AttackCharInfo == null)
			return;
		float damage = buildInfo.BattleDamage * buildInfo.AttackSpeed;
		ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(buildInfo.csvInfo.Projectile);
		
		projectileInfo.projectileData = Globals.projectileData[buildInfo.csvInfo.Projectile];
		
		
		
		//初始化子弹起点,需要重设为炮口发射点;
		Vector3 startPoint = BattleController.GetBuildCenterPosition (buildInfo);
		int idx = weapon.angleToIdx (weaponAngel);
		Transform fp = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx);
		if(fp!=null)
			startPoint = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx).position;

		float randR = projectileInfo.projectileData.StartOffset;
		randR = 0.6f;
		startPoint = Globals.GetRandPointInCircle (startPoint, randR / 100f /2f);
		projectileInfo.transform.position = startPoint;
		
		EffectController.PlayEffect("Model/Effect/"+buildInfo.csvInfo.AttackEffect,startPoint);
		projectileInfo.AttackCharInfo = buildInfo.AttackCharInfo;
		projectileInfo.FireBuildInfo = buildInfo;
		projectileInfo.HitEffect = buildInfo.csvInfo.HitEffect;
		//初始化攻击点;
		projectileInfo.AttackPoint = buildInfo.AttackCharInfo.Position;
		
		//初始化攻击点;
		//float AttackOffset = buildInfo.csvInfo.DamageSpread / 100f ;
		float AttackOffset = buildInfo.DamageRadius * 7f;
		AttackOffset = AttackOffset * (startPoint - projectileInfo.AttackPoint).magnitude / buildInfo.AttackRange;
		if(AttackOffset>0)  
		{
			Vector3 realAttackPoint =  Globals.GetRandPointInCircle(projectileInfo.AttackPoint,AttackOffset);
//			float distance = (realAttackPoint - projectileInfo.AttackPoint).magnitude;
			projectileInfo.AttackPoint = realAttackPoint;

		}
		
		projectileInfo.AttackType = 1;
		projectileInfo.Damage = damage;
		projectileInfo.DamageRadius = buildInfo.DamageRadius;
		projectileInfo.DamageRadiusTrooper = buildInfo.DamageRadius;
		projectileInfo.BattleInit();
		
		projectileInfo.projectileCtl.CMDFire();
		AudioPlayer.Instance.PlaySfx("machinegun_attack_01");
	}
}
