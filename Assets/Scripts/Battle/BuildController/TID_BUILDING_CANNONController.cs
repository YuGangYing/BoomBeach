using UnityEngine;
using System.Collections;

public class TID_BUILDING_CANNONController:BuildController {

	//用于瞄准后执行，或直接调用;
	protected override void DoFire()
	{
		if (buildInfo.AttackCharInfo == null)
			return;
		float damage = buildInfo.BattleDamage * buildInfo.AttackSpeed;
		if(buildInfo.AttackCharInfo.trooperData.tid=="TID_TANK")
		{
			damage*=2f;
		}
		ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(buildInfo.csvInfo.Projectile);
		
		projectileInfo.projectileData = Globals.projectileData[buildInfo.csvInfo.Projectile];
		
		
		
		//初始化子弹起点,需要重设为炮口发射点;
		Vector3 startPoint = BattleController.GetBuildCenterPosition (buildInfo);
		int idx = weapon.angleToIdx (weaponAngel);

		Transform fp = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx);
		if(fp!=null)
			startPoint = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx).position;
		//float offset = projectileInfo.projectileData.StartOffset/100f/2f; 

		EffectController.PlayEffect("Model/Effect/"+buildInfo.csvInfo.AttackEffect,startPoint);
		
		//startPoint = Globals.GetRandPointInCircle (startPoint,offset);
		projectileInfo.transform.position = startPoint;
		
		
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
		AudioPlayer.Instance.PlaySfx("cannon");
	}
}
