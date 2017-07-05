using UnityEngine;
using System.Collections;

public class flamethrower_projectileController : ProjectileController {


	private bool IsArrival;

	private float BurnTimer;

	private Vector3 beginPos;
	
	public override void DoAttack()
	{
		if(!IsArrival)
		{
		
			//范围伤害;
			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				if(!charInfo.isDead&&(projectileInfo.AttackPoint - charInfo.Position).magnitude<=projectileInfo.DamageRadiusTrooper)
				{
					charInfo.trooperCtl.CMDUnderAttack(projectileInfo.Damage);
				}
			}

			IsArrival = true;

		}

		if (BurnTimer >= 0.15f)
			//projectileInfo.IsBoom = true;
		GameObject.DestroyImmediate (projectileInfo.gameObject);

		BurnTimer += Time.deltaTime;
	}


	public override void CMDFire()
	{
		projectileInfo.State = AISTATE.MOVING;
		IsArrival = false;
		BurnTimer = 0;
		beginPos = projectileInfo.transform.position;
	}


	/// <summary>
	/// 检测是否开始攻击;
	/// </summary>
	public override bool CheckBeginAttack()
	{
		Vector3 point = Vector3.zero;
		if(projectileInfo.AttackType==1&&projectileInfo.DamageRadius==0)
		{
			//移动至打击对象
			point = projectileInfo.AttackCharInfo.Position;
		}
		else
		{
			//移动至目标点;
			point = projectileInfo.AttackPoint;
		}

		if((projectileInfo.transform.position-beginPos).magnitude>=projectileInfo.FireBuildInfo.AttackRange)
		{
			return true;
		}
		if((projectileInfo.transform.position-point).magnitude<=projectileInfo.Speed)
		{
			return true;
		}
		else
			return false;
	}

}
