using UnityEngine;
using System.Collections;

public class BazookaAmmoController : ProjectileController {

	private bool isInit;

	private GameObject trail;
	void Init()
	{
		isInit = true;

		if(projectileInfo.projectileData.ParticleEmitter!="")
		{
			trail = GameObject.Instantiate(ResourceCache.Load("Model/Trail/"+projectileInfo.projectileData.ParticleEmitter)) as GameObject;
			trail.transform.parent = projectileInfo.transform;
			trail.transform.localPosition = Vector3.zero;
		}
	}

	public override void DoAttack ()
	{
		if(trail!=null)
		{
			trail.transform.parent = SpawnManager.GetInstance.bulletContainer;
			trail.GetComponent<ParticleEmitter>().emit = false;
		}

		string Hiteffect = projectileInfo.HitEffect;
		
		
		if(Hiteffect!="")
			EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);

		AudioPlayer.Instance.PlaySfx("bazooka_hit_01");

		base.DoAttack ();
	}

	public override void DoMove()
	{
		if (!isInit)Init ();
		base.DoMove();
	}



}
