using UnityEngine;
using System.Collections;

public class CannonballController : ProjectileController {

	//private float maxHeight;

	//private float totalDistance;

	//private Vector3 StartPointBack;

	private bool isInit;

	//顶点式抛物线 y = ax^2 弹道抛物线开口向下，a < 0;

	//private float a;

	private tk2dSprite projectileSprite;
	private GameObject trail;
	void Init()
	{
		//totalDistance = (projectileInfo.transform.position - projectileInfo.AttackPoint).magnitude;
		//maxHeight = totalDistance/10f;
		//float halfdistance = totalDistance / 2f;
		//a = maxHeight / (halfdistance*halfdistance) * -1f;
		//StartPointBack = projectileInfo.transform.position;
		projectileSprite = projectileInfo.GetComponentInChildren<tk2dSprite> ();
		isInit = true;

		if(projectileInfo.projectileData.ParticleEmitter!="")
		{
			trail = GameObject.Instantiate(ResourceCache.load("Model/Trail/"+projectileInfo.projectileData.ParticleEmitter)) as GameObject;
			trail.transform.parent = projectileInfo.transform;
			trail.transform.localPosition = Vector3.zero;
		}
	}

	public override void DoAttack ()
	{
		isInit = false;
		if(trail!=null)
		{
			trail.transform.parent = SpawnManager.GetInstance.bulletContainer;
			trail.GetComponent<ParticleEmitter>().emit = false;
		}
		
		string Hiteffect = projectileInfo.HitEffect;
		
		
		if(Hiteffect!="")
			EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);
		base.DoAttack ();
		CameraOpEvent.Instance.Shake ();
	}



	public override void DoMove()
	{
		if(Camera.main==null)return;
		if (!isInit)Init ();
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

		//计算角度;
		Vector3 begin = Camera.main.WorldToScreenPoint (projectileInfo.transform.position);
		Vector3 end = Camera.main.WorldToScreenPoint (movePosition);
		float degree = Globals.CaclDegree (begin,end);
		Vector3 angle = projectileSprite.transform.localEulerAngles;
		projectileSprite.transform.localEulerAngles = new Vector3 (angle.x,angle.y,degree);
		projectileInfo.transform.position = movePosition;
	}

	public override void CMDFire()
	{
		projectileInfo.State = AISTATE.MOVING;
	}

}
