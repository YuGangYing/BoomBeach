using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileController  {

	public ProjectileInfo projectileInfo;

	/// <summary>
	/// 检测是否开始攻击;
	/// </summary>
	public virtual bool CheckBeginAttack()
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

		if((projectileInfo.transform.position-point).magnitude<=projectileInfo.Speed)
		{
			return true;
		}
		else
		return false;
	}


	public virtual void DoMove()
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

		Vector3 movePosition = Vector3.MoveTowards (projectileInfo.transform.position, point, projectileInfo.Speed);
		projectileInfo.transform.position = movePosition;
	}

	public virtual void DoAttack()
	{

		//打人;
		if(projectileInfo.DamageRadiusTrooper>0&&(projectileInfo.AttackType==1||projectileInfo.AttackType==0))
		{
			//范围伤害;
			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				if(!charInfo.isDead&&(projectileInfo.AttackPoint - charInfo.Position).magnitude<=projectileInfo.DamageRadiusTrooper)
				{
					charInfo.trooperCtl.CMDUnderAttack(projectileInfo.Damage);
				}
			}
		}
		else
		{
			//无伤害范围不可能存在群体攻击，type只为1;
			if(projectileInfo.AttackCharInfo!=null)
			projectileInfo.AttackCharInfo.trooperCtl.CMDUnderAttack(projectileInfo.Damage);
		}

		//打建筑;
		if(projectileInfo.DamageRadius>0&&(projectileInfo.AttackType==2||projectileInfo.AttackType==0))
		{
			//bool hitted = false;

			foreach(BuildInfo buildInfo in BattleData.Instance.buildDic.Values)
			{
				if(!buildInfo.IsDead&&BattleController.CheckAttackedBuild(projectileInfo.DamageRadius,projectileInfo.AttackPoint,buildInfo))
				{
					buildInfo.buildCtl.CMDUnderAttack(projectileInfo.Damage);
//					hitted = true;
				}
			}	

			if(projectileInfo.projectileData.Name=="ArtilleryAmmo"||
			   projectileInfo.projectileData.Name=="Barrage Ammo")
			{
				foreach(BuildInfo buildInfo in BattleData.Instance.trapDic.Values)
				{
					if(!buildInfo.IsDead&&BattleController.CheckAttackedBuild(projectileInfo.DamageRadius,projectileInfo.AttackPoint,buildInfo))
					{
						buildInfo.buildCtl.CMDAttack();
//						hitted = true;
					}
				}	
			}	


		}
		else
		{
			//无伤害范围不可能存在群体攻击，type只为2;
			if(projectileInfo.AttackBuildInfo!=null)
			{
				projectileInfo.AttackBuildInfo.buildCtl.CMDUnderAttack(projectileInfo.Damage);
			}
		}

		projectileInfo.IsBoom = true;
	}

	public virtual void CMDFire()
	{
		//计算方向;
		Vector2 begin = new Vector2 (projectileInfo.transform.position.x,projectileInfo.transform.position.z);
		Vector2 end = new Vector2 (projectileInfo.AttackPoint.x,projectileInfo.AttackPoint.z);
		float degree = 0 - Globals.CaclDegree (begin,end);
		Vector3 angle = projectileInfo.transform.eulerAngles;
		projectileInfo.transform.eulerAngles = new Vector3 (angle.x,degree,angle.z);


		projectileInfo.State = AISTATE.MOVING;
	}

	public void CMDAttack()
	{
		projectileInfo.State = AISTATE.ATTACKING;
	}


	/// <summary>
	/// 获取实例对象;
	/// </summary>
	public static ProjectileController Instantiate(ProjectileInfo _ProjectileInfo)
	{
		ProjectileController projectileCtl = null;
		if(_ProjectileInfo.ProjectileName=="Machine Gun Ammo"||_ProjectileInfo.ProjectileName=="Machine Nest Gun Ammo")
			projectileCtl = new MachineGunAmmoController();
		else if(_ProjectileInfo.ProjectileName=="FocusFireAmmo")
			projectileCtl = new FocusFireAmmoController();
		else if(_ProjectileInfo.ProjectileName=="HealAmmo")
			projectileCtl = new HealAmmoController(); 
		else if(_ProjectileInfo.ProjectileName=="StunAmmo")
			projectileCtl = new StunAmmoController(); 
		else if(_ProjectileInfo.ProjectileName=="smoke_ammo")
			projectileCtl = new smoke_ammoController(); 
		else if(_ProjectileInfo.ProjectileName=="Mortar Ammo"||_ProjectileInfo.ProjectileName=="Boss Mortar")
			projectileCtl = new MortarAmmoController(); 
		else if(_ProjectileInfo.ProjectileName=="ArtilleryAmmo"
		        ||_ProjectileInfo.ProjectileName=="Barrage Ammo"
		        ||_ProjectileInfo.ProjectileName=="Missile Ammo")
			projectileCtl = new MissileAmmoController(); 
		else if(_ProjectileInfo.ProjectileName=="flamethrower_projectile")
			projectileCtl = new flamethrower_projectileController();
		else if(_ProjectileInfo.ProjectileName=="Cannonball")
			projectileCtl = new CannonballController();
		else if(_ProjectileInfo.ProjectileName=="Bazooka Ammo")
			projectileCtl = new	BazookaAmmoController();
		else if(_ProjectileInfo.ProjectileName=="Tank Shell")
			projectileCtl = new	TankShellController();
		else
			projectileCtl = new ProjectileController();

		return projectileCtl;
		
	}


	/// <summary>
	/// 实例化子弹对象;
	/// </summary>
	public static ProjectileInfo InstantiateGameObject(string name)
	{
		ProjectileInfo bullet = null;
		if(BattleData.Instance.projectileCacheList.ContainsKey(name))
		{
			Queue<ProjectileInfo> projectList = BattleData.Instance.projectileCacheList[name];
			if(projectList!=null&&projectList.Count>0)
			{
				bullet = projectList.Dequeue();
			}
			else
			{
				if(projectList == null) BattleData.Instance.projectileCacheList[name] = new Queue<ProjectileInfo>();
			}
		}
		else
		{
			Queue<ProjectileInfo> projectList = new Queue<ProjectileInfo>();
			BattleData.Instance.projectileCacheList.Add(name,projectList);
		}

		if(bullet==null)
		{
			string path = "Model/Projectile/"+name;
			if(ResourceCache.load(path)==null) path="Model/Projectile/Machine Gun Ammo"; //防止报错，发布后禁用;
			GameObject bulletPrefab = ResourceCache.load(path) as GameObject;
			GameObject bulletObj = GameObject.Instantiate(bulletPrefab) as GameObject;
			bulletObj.transform.parent = SpawnManager.GetInstance().bulletContainer;
			bulletObj.transform.localScale = Vector3.one;
			bullet = bulletObj.GetComponent<ProjectileInfo>();
			bullet.ProjectileName = name;

		}
		bullet.IsBoom = false;
		return bullet;
	}


}
