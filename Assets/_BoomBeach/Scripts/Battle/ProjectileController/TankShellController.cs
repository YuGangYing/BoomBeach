using UnityEngine;
using System.Collections;

public class TankShellController : ProjectileController {

	private bool isInit;

	private GameObject trail;
	void Init()
	{
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
		if(trail!=null)
		{
			trail.transform.parent = SpawnManager.GetInstance().bulletContainer;
			trail.GetComponent<ParticleEmitter>().emit = false;
		}
		isInit =false;

		string Hiteffect = projectileInfo.HitEffect;
		
		
		if(Hiteffect!="")
			EffectController.PlayEffect("Model/Effect/"+Hiteffect,projectileInfo.AttackPoint);

		AudioPlayer.Instance.PlaySfx("tank_hit_01");
		base.DoAttack ();
	}

	public override void DoMove()
	{
		if (!isInit)Init ();
		base.DoMove();
	}



}
