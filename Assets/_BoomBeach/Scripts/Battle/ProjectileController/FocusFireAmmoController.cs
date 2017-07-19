using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//信号弹;
using BoomBeach;


public class FocusFireAmmoController : ProjectileController {

	public static ProjectileInfo FocusFireProjectile;

	private bool IsFocus; //是否通知士兵聚合过;
	private float LifeTime; //过了有效期，重新通知士兵查找目标;

	public BuildInfo focusBuildInfo;

	public void DoFocus(CharInfo charInfo)
	{
		if(focusBuildInfo==null)
		{
			Vector3 randLandPoint = Globals.GetRandPointInCircle(projectileInfo.AttackPoint,1.5f);
			charInfo.Dest.x = randLandPoint.x;
			charInfo.Dest.z = randLandPoint.z;
			charInfo.IsOnlyMove = true;
			charInfo.path = null;
			charInfo.AttackBuildInfo = null;
			charInfo.AttackState = AISTATE.STANDING;
			charInfo.trooperCtl.CMDFindPath();	
		}
		else
		{

			charInfo.path = null;
			charInfo.AttackBuildInfo = null;
			charInfo.AttackState = AISTATE.STANDING;
			charInfo.State = AISTATE.FINDINGDEST;
			charInfo.trooperCtl.NotifyCharInfoFindDest(charInfo,focusBuildInfo);
		}
	}
	public override void DoAttack()
	{
		if(!IsFocus)
		{
			if(FocusFireProjectile!=null)
			{
				FocusFireProjectile.transform.Find("sprite").gameObject.SetActive(true);
				FocusFireProjectile.IsBoom = true;
			}

			FocusFireProjectile = this.projectileInfo;
			if(DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE&&!BattleData.Instance.BattleIsEnd)
			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				DoFocus(charInfo);		
			}

			FocusFireProjectile.transform.Find("sprite").gameObject.SetActive(false);
			EffectController.PlayEffect("Model/Effect/FlashFire",FocusFireProjectile.AttackPoint,FocusFireProjectile.FireSpellInfo.BoostTime,true);

			if(focusBuildInfo!=null)
			{
				EffectController.PlayEffect("Model/Effect/FlareFocus"+focusBuildInfo.csvInfo.Width,BattleController.GetBuildCenterPosition(focusBuildInfo));
			}
		}
		IsFocus = true;
		if(LifeTime<=projectileInfo.FireSpellInfo.BoostTime)
		{
			LifeTime+=Time.deltaTime;
		}
		else
		{
			if(DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE&&!BattleData.Instance.BattleIsEnd)
			foreach(CharInfo charInfo in BattleData.Instance.AllocateTrooperList)
			{
				if(charInfo.AttackState!=AISTATE.ATTACKING&&charInfo.State!=AISTATE.FINDINGDEST)
				{
					charInfo.path = null;
					charInfo.IsFindDest = false;
					charInfo.IsOnlyMove = false;
					charInfo.AttackState = AISTATE.STANDING;
					charInfo.State = AISTATE.STANDING;
					charInfo.trooperCtl.CMDFindDest();
				}			
			}
			FocusFireProjectile = null;
			projectileInfo.transform.Find("sprite").gameObject.SetActive(true);
			focusBuildInfo = null;
			projectileInfo.IsBoom = true;
			IsFocus = false;
		}
		
		AudioPlayer.Instance.PlaySfx("flare_02");
	}
}
