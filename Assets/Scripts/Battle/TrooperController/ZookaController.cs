using UnityEngine;
using System.Collections;

public class ZookaController :TrooperController {
	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public override void DoAttack()
	{
		if(attackTimeCounter==0)
		{
			//播放攻击动画;
			PlayAttack();
			
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

			AudioPlayer.Instance.PlaySfx("bazooka_troop_fire_01");
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
		AudioPlayer.Instance.PlaySfx("bazooka_unit_move_01");
		base.DoMove();
	}
}
