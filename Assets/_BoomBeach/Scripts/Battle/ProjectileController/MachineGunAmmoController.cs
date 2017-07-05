using UnityEngine;
using System.Collections;

public class MachineGunAmmoController : ProjectileController {

	public override void DoAttack()
	{	
		//打人;
		if(projectileInfo.DamageRadiusTrooper>0&&(projectileInfo.AttackType==1||projectileInfo.AttackType==0))
		{
			//范围伤害;
			bool hited = false;
			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				float distance = (projectileInfo.AttackPoint - charInfo.Position).magnitude;
				if(!charInfo.isDead&&distance<=projectileInfo.DamageRadiusTrooper)
				{
					charInfo.trooperCtl.CMDUnderAttack(projectileInfo.Damage);
					hited = true;
				}
			}

			if(hited)
			{
				string Hiteffect = projectileInfo.HitEffect;
				
				
				if(Hiteffect!="")
					EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);
			}
			else
			{
				//	Debug.Log("Miss");
				EffectController.PlayEffect("Model/Effect/MissEffect",projectileInfo.AttackPoint);
			}
			AudioPlayer.Instance.PlaySfx("heavy_bullet_hit_01");		
		}
		else
		{
			string Hiteffect = projectileInfo.HitEffect;
			
			
			if(Hiteffect!="")
				EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);
			//无伤害范围不可能存在群体攻击，type只为1;
			if(projectileInfo.AttackCharInfo!=null)
				projectileInfo.AttackCharInfo.trooperCtl.CMDUnderAttack(projectileInfo.Damage);

			AudioPlayer.Instance.PlaySfx("heavy_bullet_hit_01");
		}
		
		//打建筑;
		if(projectileInfo.AttackType==2||projectileInfo.AttackType==0)
		{

			if(projectileInfo.FireCharInfo.trooperData.tid=="TID_HEAVY")
				AudioPlayer.Instance.PlaySfx("heavy_bullet_hit_01");
			else if(projectileInfo.FireCharInfo.trooperData.tid=="TID_RIFLEMAN")
				AudioPlayer.Instance.PlaySfx("assault_troop_bullet_hit_01");
			if(projectileInfo.AttackBuildInfo!=null)
			{
				if(BattleController.CheckAttackedBuild(projectileInfo.DamageRadius,projectileInfo.AttackPoint,projectileInfo.AttackBuildInfo))
				{
					string Hiteffect = projectileInfo.HitEffect;
					
					
					if(Hiteffect!="")
						EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);
					projectileInfo.AttackBuildInfo.buildCtl.CMDUnderAttack(projectileInfo.Damage);
				}
				else
				{
					EffectController.PlayEffect("Model/Effect/MissEffect",projectileInfo.AttackPoint);
					//Debug.Log("Miss");
				}
			}

		}
		
		projectileInfo.IsBoom = true;
	}
}
