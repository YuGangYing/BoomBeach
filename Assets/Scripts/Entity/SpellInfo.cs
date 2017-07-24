using UnityEngine;
using System.Collections;
using BoomBeach;

public class SpellInfo : MonoBehaviour {

	public BuildInfo focusBuildInfo;

	public static void Fire(BattleTrooperData btd, Vector3 firePoint,Vector3 attackPoint)
	{
		GameObject spellObj = new GameObject (btd.tidLevel);
		SpellInfo spellInfo = spellObj.AddComponent<SpellInfo> ();
		spellObj.AddComponent<SpellUpdater> (); 
		spellInfo.transform.position = firePoint;
		spellInfo.AttackPoint = attackPoint;
		spellInfo.spellData = btd.csvInfo;

		int currentCost = btd.weaponCost;
		btd.weaponCost += btd.energyIncrease;

		/*
		BattleWeaponItem bwi = btd.uiItem as BattleWeaponItem;
		bwi.weaponCost = btd.weaponCost; //更新UI;
		*/
		Globals.EnergyTotal -= currentCost; //扣除消耗;
		UIManager.GetInstance.GetController<BattleInterfaceCtrl>().UpdateWeaponCost(btd);

        /*
		if(btd.uiItem.isDisabled)
		{
			int length = BattleWeaponList.Instance.BattleItems.Count;			
			for(int i=0;i<length;i++)
			{
				if(!BattleWeaponList.Instance.BattleItems[i].isDisabled)
				{
                    if (ScreenUIManage.Instance != null) 
						ScreenUIManage.Instance.OnClickBattleTrooper(BattleWeaponList.Instance.BattleItems[i].gameObject);
					break;
				}
			}
		}
        */


		if(btd.tid=="TID_FLARE")
		{
			GridInfo gridInfo = Globals.LocateGridInfo(attackPoint);
			BuildInfo buildInfo = gridInfo.buildInfo;
			if(buildInfo!=null
			   &&buildInfo.csvInfo.TID_Type!="OBSTACLES"
               &&buildInfo.csvInfo.TID_Type!="TRAPS"
		       &&buildInfo.csvInfo.TID_Type!="DECOS"
		       &&buildInfo.csvInfo.BuildingClass!="Artifact")
			{
				spellInfo.focusBuildInfo = buildInfo;
			}
			EffectController.PlayEffect("Model/Effect/deployring",attackPoint);
		}
		else
			EffectController.PlayEffect("Model/Effect/deployring",attackPoint);

		if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
		{
			//开始录像;
			ReplayNodeData rnd = new ReplayNodeData();
			rnd.SelfID = btd.id; //这里表示武器ID;
			rnd.SelfType = EntityType.Weapon;
			rnd.State = AISTATE.ATTACKING;
			rnd.DestX = attackPoint.x;
			rnd.DestZ = attackPoint.z;
			BattleData.Instance.BattleCommondQueue.Enqueue(rnd);


				
			if(btd.csvInfo.Projectile=="FocusFireAmmo"
			   ||btd.csvInfo.Projectile=="HealAmmo"
			   ||btd.csvInfo.Projectile=="smoke_ammo"
			   ||btd.csvInfo.Projectile=="StunAmmo")
			{
				if(BattleController.CheckAllTroopDead())
				{
					BattleData.Instance.BattleIsEnd = true;
                    //炮弹打完，且兵也都消死了，直接弹窗;
                    if (ScreenUIManage.Instance != null) 
						ScreenUIManage.Instance.battleResultWin.ShowResultWin();
					UIManager.GetInstance.GetController<BattleResultCtrl>().ShowPanel ();
				}
			}
		}
	}

	/// <summary>
	/// 药剂的数据;
	/// </summary>
	public CsvInfo spellData;

	/// <summary>
	/// 打击次数;
	/// </summary>
	public int NumberOfHits{
		get{
			return spellData.NumberOfHits;
		}
	}

	/// <summary>
	/// 每次打击间隔时间;
	/// </summary>
	public float TimeBetweenHits{
		get{
			return spellData.TimeBetweenHitsMS/1000f;
		}
	}

	/// <summary>
	/// 发射延时;
	/// </summary>
	public float DeployTime{
		get{
			return spellData.DeployTimeMS / 1000f;
		}
	}


	/// <summary>
	/// 每发子弹伤害量;
	/// </summary>
	public float Damage{
		get{
			return spellData.Damage*1f;
		}
	}


	/// <summary>
	/// 打击点的随机半径;
	/// </summary>
	public float RandomRadius
	{
		get{
			return spellData.RandomRadius/100f;
		}
	}

	/// <summary>
	/// 伤害半径;
	/// </summary>
	public float DamageRadius
	{
		get{
			return spellData.Radius/100f;
		}
	}

	/// <summary>
	/// 伤害半径;
	/// </summary>
	public float DamageRadiusTrooper
	{
		get{
			return spellData.RadiusAgainstTroops/100f;
		}
	}

	public float BoostTime{
		get{
			return spellData.BoostTimeMS / 1000f;
		}
	}

	//操作的打击点;
	public Vector3 AttackPoint;
	
}
