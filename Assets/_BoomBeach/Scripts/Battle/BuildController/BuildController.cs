using UnityEngine;
using System.Collections;
using BoomBeach;

public class BuildController {

	/// <summary>
	/// 相应的建筑物实体;
	/// </summary>
	public BuildInfo buildInfo;

	/// <summary>
	/// 攻击计时器;
	/// </summary>
	protected float attackTimeCounter = 0f;


	public virtual void Init()
	{

	}

	/// <summary>
	/// 检测是否要停止攻击;
	/// </summary>
	public virtual bool CheckEndAttack()
	{
		if(buildInfo.AttackCharInfo!=null)
		{
			float distance = (BattleController.GetBuildCenterPosition(buildInfo) - buildInfo.AttackCharInfo.Position).magnitude;
			bool isEnd = false;
			if(buildInfo.AttackCharInfo.isDead||buildInfo.AttackCharInfo.isInSmoke||distance>buildInfo.AttackRange||distance<buildInfo.MinAttackRange)
			{
				buildInfo.IsFindDest = false;
				buildInfo.AttackCharInfo = null;
				buildInfo.State = AISTATE.STANDING;
				buildInfo.AttackState = AISTATE.STANDING;
				//attackTimeCounter = 0; //攻击计时器归零;
				isEnd = true;
			}
			return isEnd;
		}
		else
		{
			return true;
		}
		
	}

	//==================指令函数===================

	public void CMDShake()
	{
		buildInfo.isShake = true;
	}

	/// <summary>
	/// 攻击指令;
	/// </summary>
	public void CMDAttack()
	{
		buildInfo.AttackState = AISTATE.ATTACKING;
		buildInfo.State = AISTATE.STANDING;
	}
	
	/// <summary>
	/// 站立指令,即停止所有指令;
	/// </summary>
	public void CMDStand()
	{
		buildInfo.State = AISTATE.STANDING;
	}

	/// <summary>
	/// 扣血指令，该指令比较特殊，没有AI状态，即实时更新UI;
	/// </summary>
	public void CMDUnderAttack(float damage)
	{		
		if(BattleData.Instance.BattleIsOver)return;
		if(!buildInfo.IsDead)
		{
			buildInfo.CurrentHitPoint -= damage;
			if(buildInfo.CurrentHitPoint>=buildInfo.BattleHitpoint)buildInfo.CurrentHitPoint = buildInfo.BattleHitpoint;
			if(buildInfo.CurrentHitPoint<=0)
			{
				buildInfo.CurrentHitPoint = 0f;
				//记录回放数据;
				if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
				{
					ReplayNodeData rnd = new ReplayNodeData();
					rnd.SelfID = buildInfo.BattleID;
					rnd.SelfType = EntityType.Build;
					rnd.IsUnderAttack = 1;
					BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
					buildInfo.IsDead = true;

					buildInfo.Invoke("AddAmmo",0.5f);

					float townHallDamage = buildInfo.BattleHitpoint*0.2f;
					if(BattleData.Instance.TownHall.CurrentHitPoint/BattleData.Instance.TownHall.BattleHitpoint>0.3f&&
					   buildInfo.tid!="TID_BUILDING_PALACE"&&
					   buildInfo.tid!="TID_BUILDING_COMMAND_CENTER")
					BattleData.Instance.TownHall.buildCtl.CMDUnderAttack(townHallDamage);

					if(buildInfo.tid=="TID_BUILDING_PALACE"||buildInfo.tid=="TID_BUILDING_COMMAND_CENTER")
					{
						BattleData.Instance.BattleIsSuccess = true;
						BattleData.Instance.BattleIsEnd = true;
                        //主堡消灭，直接弹窗;
                        if (ScreenUIManage.Instance != null) 
							ScreenUIManage.Instance.battleResultWin.ShowResultWin();
						UIManager.GetInstance ().GetController<BattleResultCtrl>().ShowPanel ();
					}
					//将攻击日志发送到服务器;
					//Helper.SendAttackLog(false);
				}

				if(BattleData.Instance.BattleIsEnd)
				foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
				{
					charInfo.path = null;
					charInfo.AttackState = AISTATE.STANDING;
					charInfo.State = AISTATE.STANDING;
				}
			}
			if(buildInfo.CurrentHitPoint<buildInfo.BattleHitpoint)
			if(buildInfo.buildUIManage!=null)
                    buildInfo.buildUIManage.SetHealthPercent (buildInfo.CurrentHitPoint/buildInfo.BattleHitpoint); //更新UI;
            if (buildInfo.buildUI != null)
                buildInfo.buildUI.SetHealthPercent(buildInfo.CurrentHitPoint / buildInfo.BattleHitpoint); //更新UI;
        }
	}
	//==================指令函数===================;




	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public virtual void DoAttack()
	{
		if(attackTimeCounter>=buildInfo.AttackSpeed)
		{
			//实现发射子弹;
			if(weapon==null) weapon = buildInfo.GetComponentInChildren<Weapon>();

			if(weapon!=null)
			{
				Vector2 beginPoint = new Vector2(weapon.transform.position.x,weapon.transform.position.z);
				Vector2 endPoint = new Vector2(buildInfo.AttackCharInfo.Position.x,buildInfo.AttackCharInfo.Position.z);
				weaponAngel = Globals.CaclDegree(beginPoint,endPoint);
				
				weapon.onAim = DoFire;
				weapon.aim(weaponAngel);
			}
			else
			{
				DoFire();
			}

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

	protected Weapon weapon;
	
	protected float weaponAngel;

	//用于瞄准后执行，或直接调用;
	protected virtual void DoFire()
	{
		if (buildInfo.AttackCharInfo == null)
			return;
		float damage = buildInfo.BattleDamage * buildInfo.AttackSpeed;
		ProjectileInfo projectileInfo = ProjectileController.InstantiateGameObject(buildInfo.csvInfo.Projectile);
		
		projectileInfo.projectileData = Globals.projectileData[buildInfo.csvInfo.Projectile];
		

		
		//初始化子弹起点,需要重设为炮口发射点;
		Vector3 startPoint = BattleController.GetBuildCenterPosition (buildInfo);
		int idx = 0;
		if(buildInfo.tid!="TID_GUARD_TOWER")
		{		
			idx = weapon.angleToIdx (weaponAngel);
		}
		else
		{
	
			Vector2 beginPoint = new Vector2(startPoint.x,startPoint.z);
			Vector2 endPoint = new Vector2(buildInfo.AttackCharInfo.Position.x,buildInfo.AttackCharInfo.Position.z);
			weaponAngel = Globals.CaclDegree(beginPoint,endPoint);			
			if(weaponAngel>=45&&weaponAngel<135)
			{
				idx = 1;
			}
			else if(weaponAngel>=135&&weaponAngel<225)
			{
				idx = 2;
			}
			else if(weaponAngel>=225&&weaponAngel<315)
			{
				idx = 3;
			}
		}
		Transform fp = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx);
		if(fp!=null)
			startPoint = buildInfo.transform.Find ("buildPos/BuildMain/FirePoint/P" + idx).position;
		//Vector3 startPoint = charInfo.transform.Find("FirePoint/P"+idx).position;
		float offset = projectileInfo.projectileData.StartOffset/100f/2f; 
		//float randX = Random.Range(startPoint.x-offset,startPoint.x+offset);
		//float randZ = Random.Range(startPoint.z-offset,startPoint.z+offset);
		//startPoint = new Vector3(randX,0f,randZ);

		startPoint = Globals.GetRandPointInCircle (startPoint,offset);

		EffectController.PlayEffect("Model/Effect/"+buildInfo.csvInfo.AttackEffect,startPoint);

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

		if(buildInfo.tid=="TID_GUARD_TOWER")
		{
			AudioPlayer.Instance.PlaySfx("assault_troop_shoot_01");
		}
		else if(buildInfo.tid=="TID_BUILDING_MORTAR"||buildInfo.tid=="TID_BUILDING_BOSS_MORTAR")
		{
			AudioPlayer.Instance.PlaySfx("mortar_fire_02_with_fall");
		}
		else if(buildInfo.tid=="TID_MACHINE_GUN_NEST"||buildInfo.tid=="TID_BUILDING_BOSS_MACHINE_GUN")
		{
			AudioPlayer.Instance.PlaySfx("machinegun_attack_01");
		}
		else if(buildInfo.tid=="TID_MISSILE_LAUNCHER")
		{
			AudioPlayer.Instance.PlaySfx("rocket_launcher_fire_01");
		}
		else if(buildInfo.tid=="TID_FLAME_THROWER")
		{
			AudioPlayer.Instance.PlaySfx("flame_thrower_01");
		}
		else if(buildInfo.tid=="TID_BUILDING_BIG_BERTHA")
		{
			AudioPlayer.Instance.PlaySfx("boom_cannon_01");
		}


	}


	protected float findDestTimer;
	/// <summary>
	/// 查找目标，必需返回buildInfo.AttackCharInfo;
	/// </summary>
	public virtual void CMDFindDest()
	{
		//仅真实攻击时才寻找攻击目标，回放时由回放数据列中获取;
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{

			findDestTimer+=Time.deltaTime;
			if(findDestTimer>=0.5f)findDestTimer = 0;
			if(findDestTimer>0)return;

			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				if(charInfo!=null&&!charInfo.isDead&&!charInfo.isInSmoke)
				{
					float distance =(BattleController.GetBuildCenterPosition(buildInfo) - charInfo.Position).magnitude;
					if(distance<=buildInfo.AttackRange&&distance>=buildInfo.MinAttackRange)
					{

						if(buildInfo.AttackCharInfo==null||
						   buildInfo.AttackCharInfo.Id!=charInfo.Id)
						{
							//记录回放数据;
							if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
							{
								ReplayNodeData rnd = new ReplayNodeData();
								rnd.SelfID = buildInfo.BattleID;
								rnd.SelfType = EntityType.Build;
								rnd.AttackState = AISTATE.ATTACKING;
								rnd.AttackType = EntityType.Trooper;
								rnd.AttackID = charInfo.Id;
								BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
							}
						}

						buildInfo.AttackCharInfo = charInfo;
						buildInfo.IsFindDest = true;
						buildInfo.State = AISTATE.FINDINGDEST;



						return;
					}
				}
			}
		}				
	}

	/// <summary>
	/// 获取实例对象;
	/// </summary>
	public static BuildController Instantiate(BuildInfo _buildInfo)
	{
		BuildController buildCtl = null;
		if (_buildInfo.csvInfo.TID_Type == "TRAPS")
				buildCtl = new TrapController ();
		else if (_buildInfo.tid == "TID_MACHINE_GUN_NEST"||_buildInfo.tid=="TID_BUILDING_BOSS_MACHINE_GUN")
			buildCtl = new TID_MACHINE_GUN_NESTController ();
		else if (_buildInfo.tid == "TID_BUILDING_CANNON"||_buildInfo.tid == "TID_BUILDING_BIG_BERTHA")
			buildCtl = new TID_BUILDING_CANNONController ();
		else if (_buildInfo.tid == "TID_MISSILE_LAUNCHER")
			buildCtl = new TID_MISSILE_LAUNCHERController ();
		else if (_buildInfo.tid == "TID_FLAME_THROWER")
			buildCtl = new TID_FLAME_THROWERController ();
		else
			buildCtl = new BuildController ();
		buildCtl.buildInfo = _buildInfo;
		return buildCtl;		
	}
}
