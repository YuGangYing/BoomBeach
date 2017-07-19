using UnityEngine;
using System.Collections;
using BoomBeach;

public class BuildUpdater : MonoBehaviour {

	public BuildInfo buildInfo;
	
	
	
	void Awake(){
		buildInfo = GetComponent<BuildInfo> ();
	}

	private float shakeTimeCount=0f;
	private float shakeSpan = 2f;
	private float lightTimeCount = 0f;
	private float lightSpan = 0.5f;
	// Update is called once per frame
	private float findDestTimer;
	void Update () {
		if (buildInfo.IsDead) {		
			//死亡的相关操作;
			buildInfo.transform.Find("buildPos/BuildMain").gameObject.SetActive(false);
			Transform buildNew = buildInfo.transform.Find("buildPos/BuildNew");
			if(buildNew!=null)buildNew.gameObject.SetActive(false);
			Transform t = buildInfo.transform.Find("DestroySprite");
			if(t!=null)
			{
				t.gameObject.SetActive(true);
				/*Transform boomEffect = buildInfo.transform.Find("boomEffect");
				if(boomEffect!=null)
				{
					ParticleSystem boomstone1 = boomEffect.Find("boomstone1").particleSystem;
					ParticleSystem boomstone2 = boomEffect.Find("boomstone2").particleSystem;
					boomstone1.Emit(10);
					boomstone2.Emit(10);
					boomEffect.Find("boomfire").gameObject.SetActive(true);
					boomEffect.Find("boomfire").GetComponent<tk2dSpriteAnimator>().Play();
				}*/

				EffectController.PlayEffect("Model/Effect/boomEffect",BattleController.GetBuildCenterPosition(buildInfo));
				AudioPlayer.Instance.PlaySfx("building_destroyed_01");
			}
			if(buildInfo.buildUIManage!=null)
                buildInfo.buildUIManage.SetHealthBarVisible(false);
            if (buildInfo.buildUI != null)
                buildInfo.buildUI.SetHealthBarVisible(false);

            this.enabled = false;

			if(buildInfo.csvInfo.TID_Type!="TRAPS")
			CameraOpEvent.Instance.Shake ();
			return;
		}


		if(buildInfo.isShake)
		{
			float currentSpeed = buildInfo.shakeSpeed * Globals.TimeRatio;
			if(buildInfo.moveStepCount+currentSpeed>=buildInfo.moveStep)
			{
				currentSpeed = buildInfo.moveStep - buildInfo.moveStepCount;
			}
			
			Transform buildNew = buildInfo.transform.Find("buildPos/BuildNew");
			Transform buildMain = buildInfo.transform.Find("buildPos/BuildMain");
			Vector3 buildPos = buildMain.localPosition;
			buildMain.localPosition = new Vector3(buildPos.x+buildInfo.shakeDirect*currentSpeed,buildPos.y+buildInfo.shakeDirect*currentSpeed,buildPos.z);
			if(buildNew!=null)buildNew.localPosition = buildMain.localPosition;
			buildInfo.moveStepCount+=currentSpeed;
			if(buildInfo.moveStepCount==buildInfo.moveStep)
			{
				buildInfo.shakeDirect*=-1;
				buildInfo.moveStepCount = 0;
			}
			
			buildInfo.shakeTimeCount+=Time.deltaTime;
			if(buildInfo.shakeTimeCount>=buildInfo.shakeTime)
			{
				buildMain.localPosition = Vector3.zero;
				if(buildNew!=null)buildNew.localPosition = buildMain.localPosition;
				buildInfo.shakeTimeCount = 0f;
				buildInfo.isShake = false;
			}	

		}

		if (buildInfo.IsInStun)
		{
			if(shakeTimeCount==0)
			buildInfo.buildCtl.CMDShake();

			shakeTimeCount+=Time.deltaTime;
			if(shakeTimeCount>=shakeSpan)
			{
				shakeTimeCount = 0f;
			}

			
			if(lightTimeCount==0)
			{
				Vector3 BuildCenter = BattleController.GetBuildCenterPosition(buildInfo);
				EffectController.PlayEffect("Model/Effect/StunLightBuild",Globals.GetRandPointInCircle(BuildCenter,1f));
				shakeSpan = Random.Range(0.2f,1f);
			}
			lightTimeCount+=Time.deltaTime;
			if(lightTimeCount>=lightSpan)lightTimeCount = 0f;
			return;
		}

		if (buildInfo.csvInfo.BuildingClass != "Defense"&&buildInfo.csvInfo.BuildingClass != "Misc"&&buildInfo.csvInfo.TID_Type!="TRAPS")return;

		if (DataManager.GetInstance.sceneStatus != SceneStatus.ENEMYBATTLE && DataManager.GetInstance.sceneStatus != SceneStatus.BATTLEREPLAY)
						return;


		if(buildInfo.status==BuildStatus.Upgrade||buildInfo.status==BuildStatus.New)return;

		//以下是指令控制;
		if(buildInfo.State==AISTATE.STANDING)
		{
			if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
			{
				buildInfo.buildCtl.CMDFindDest();
			}
		}


		if(!BattleData.Instance.BattleIsEnd&&buildInfo.State==AISTATE.FINDINGDEST)
		{
			if(buildInfo.IsFindDest)
			{
				buildInfo.buildCtl.CMDAttack(); //开始攻击;
			}
		}


		if(!BattleData.Instance.BattleIsEnd&&buildInfo.AttackState == AISTATE.ATTACKING)
		{
			//攻击;
			if(buildInfo.buildCtl.CheckEndAttack())
			{
				buildInfo.buildCtl.CMDFindDest();
			}
			else
			{
				buildInfo.buildCtl.DoAttack();
			}
		}




	}
}
