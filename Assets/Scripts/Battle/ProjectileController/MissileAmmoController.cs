using UnityEngine;
using System.Collections;

public class MissileAmmoController : ProjectileController {

	public override void DoAttack ()
	{
		if(trail!=null)
		{
			trail.transform.parent = SpawnManager.GetInstance.bulletContainer;
			trail.GetComponent<ParticleEmitter>().emit = false;
		}

		isInit = false;
		string Hiteffect = projectileInfo.HitEffect;


		if(Hiteffect!="")
			EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);
		base.DoAttack ();
		CameraOpEvent.Instance.Shake ();

		AudioPlayer.Instance.PlaySfx("missile_hit_01");
	}


	private float maxHeight;
	
	private float totalDistance;
	
	//private Vector3 StartPointBack;
	
	private bool isInit;
	
	//顶点式抛物线 y = ax^2 弹道抛物线开口向下，a < 0;
	
	private float a;
	
	private tk2dSprite projectileSprite;

	private GameObject trail;
	void Init()
	{
		totalDistance = (projectileInfo.transform.position - projectileInfo.AttackPoint).magnitude;
		maxHeight = totalDistance/10f;
		float halfdistance = totalDistance / 2f;
		a = maxHeight / (halfdistance*halfdistance) * -1f;
		//StartPointBack = projectileInfo.transform.position;
		projectileSprite = projectileInfo.GetComponentInChildren<tk2dSprite> ();
		isInit = true;

		if(projectileInfo.projectileData.ParticleEmitter!="")
		{
			trail = GameObject.Instantiate(ResourceCache.Load("Model/Trail/"+projectileInfo.projectileData.ParticleEmitter)) as GameObject;
			trail.transform.parent = projectileInfo.transform;
			trail.transform.localPosition = Vector3.zero;
		}
	}
	
	
	
	public override void DoMove()
	{
		if(Camera.main==null)return;
		if (!isInit)Init ();
		Vector3 point = new Vector3 (projectileInfo.AttackPoint.x, 0f, projectileInfo.AttackPoint.z);
		Vector3 currentPoint = new Vector3(projectileInfo.transform.position.x,0f, projectileInfo.transform.position.z);	
		
		
		
		Vector3 movePosition = Vector3.MoveTowards (currentPoint, point, projectileInfo.Speed);
		//开始计算抛物线高度，以最高点和总距离中点为原点;
		
		float distanceToEnd = (movePosition - point).magnitude;
		float x = totalDistance /2f - distanceToEnd;
		float y = maxHeight + a * x * x;
		
		Vector3 dest = Vector3.zero;
		if (distanceToEnd <= projectileInfo.Speed)
			dest = projectileInfo.AttackPoint;
		else
			dest = new Vector3 (movePosition.x,y,movePosition.z);
		
		
		//计算角度;
		Vector3 begin = Camera.main.WorldToScreenPoint (projectileInfo.transform.position);
		Vector3 end = Camera.main.WorldToScreenPoint (dest);
		float degree = Globals.CaclDegree (begin,end);
		Vector3 angle = projectileSprite.transform.localEulerAngles;
		projectileSprite.transform.localEulerAngles = new Vector3 (angle.x,angle.y,degree);
		
		projectileInfo.transform.position = dest;

		
	}
	
	public override void CMDFire()
	{
		//计算方向;
		/*
		Vector2 begin = new Vector2 (projectileInfo.transform.position.x,projectileInfo.transform.position.z);
		Vector2 end = new Vector2 (projectileInfo.AttackPoint.x,projectileInfo.AttackPoint.z);
		float degree = 0 - Globals.CaclDegree (begin,end);
		Vector3 angle = projectileInfo.transform.eulerAngles;
		projectileInfo.transform.eulerAngles = new Vector3 (angle.x,degree,angle.z);
		*/

		projectileInfo.State = AISTATE.MOVING;
	}
}
