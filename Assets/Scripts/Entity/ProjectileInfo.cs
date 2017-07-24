using UnityEngine;
using System.Collections;

public class ProjectileInfo:MonoBehaviour {

	public void BattleInit()
	{
		if(FireBuildInfo!=null||FireSpellInfo!=null)
		{
			if(ProjectileName!="Machine Gun Ammo"&&ProjectileName!="Machine Nest Gun Ammo"&&ProjectileName!="Tower Ammo")
			{
				tk2dSprite projectileSprite = GetComponentInChildren<tk2dSprite>();
				if(projectileSprite!=null)
					projectileSprite.scale = Vector3.one*2f;
			}
		}
		projectileCtl = ProjectileController.Instantiate (this);

		if(FireSpellInfo!=null&&FireSpellInfo.focusBuildInfo!=null)
		{
			//信号弹;
			FocusFireAmmoController ffac = (FocusFireAmmoController)projectileCtl;
			ffac.focusBuildInfo = FireSpellInfo.focusBuildInfo;
		}

		projectileCtl.projectileInfo = this;
		if(gameObject.GetComponent<ProjectileUpdater>()==null)
		{
			gameObject.AddComponent<ProjectileUpdater> (); 
		}
	}

	public string ProjectileName;

	public Projectiles projectileData;

	/// <summary>
	/// 每个子弹的控制器;
	/// </summary>
	public ProjectileController projectileCtl;

	public bool IsBoom{
		set{
			gameObject.SetActive(!value);
			if(value)
			{
				/* 炮弹使用不用检测战斗结束，有bug，12发的导弹只有最后一发才作检测;
				if(Globals.sceneStatus == SceneStatus.ENEMYBATTLE)
				{
					if(projectileData.Name=="Barrage Ammo"
					   ||projectileData.Name=="ArtilleryAmmo")
					{
						if(BattleController.checkBattleEnd())
						{
							BattleData.Instance.BattleIsEnd = true;
							//炮弹打完，且兵也都消死了，直接弹窗;
							ScreenUIManage.Instance.battleResultWin.ShowResultWin();
						}



					}
				}
				*/
				BattleData.Instance.projectileCacheList[ProjectileName].Enqueue(this);
			
			}
		}
		get{
			return !gameObject.activeSelf;
		}
	}  //是否已爆炸（表示可再次启用）;

	public AISTATE State;

	/// <summary>
	/// 子弹的攻击人物;
	/// </summary>
	public CharInfo AttackCharInfo;

	/// <summary>
	/// 子弹的攻击建筑;
	/// </summary>
	public BuildInfo AttackBuildInfo;

	/// <summary>
	/// 子弹的发射人物;
	/// </summary>
	public CharInfo FireCharInfo;
	
	/// <summary>
	/// 子弹的发射建筑;
	/// </summary>
	public BuildInfo FireBuildInfo;

	/// <summary>
	/// 子弹的发射药水发射器;
	/// </summary>
	public SpellInfo FireSpellInfo;


	/// <summary>
	/// 子弹的打击点;
	/// </summary>
	public Vector3 AttackPoint;

	/// <summary>
	/// 子弹的打击的目标,0:全部,1:士兵 2:建筑;
	/// </summary>
	public byte AttackType;


	public float Speed{
		get{
			return projectileData.Speed/100f * Time.deltaTime;
		}
	}

	/// <summary>
	/// 伤害值;
	/// </summary>
	public float Damage;


	/// <summary>
	/// 伤害范围;
	/// </summary>
	public float DamageRadius;

	/// <summary>
	/// 伤害范围针对兵;
	/// </summary>
	public float DamageRadiusTrooper;

	public string HitEffect;

}
