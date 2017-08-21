using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectController : MonoBehaviour {


	public float LifeTime;
	//private float TimeCounter;
	private string prefabpath;
	void OnEnable()
	{
		ParticleEmitter[] emitters = transform.GetComponentsInChildren<ParticleEmitter>();
		ParticleSystem[] parts = transform.GetComponentsInChildren<ParticleSystem>();
		SpriteFade[] fade = transform.GetComponentsInChildren<SpriteFade>();
		tk2dSpriteAnimator[] ani = transform.GetComponentsInChildren<tk2dSpriteAnimator>();
		if(emitters!=null)
		{
			for(int i=0;i<emitters.Length;i++)
			{
				emitters[i].emit = loop;
				emitters[i].Emit();
			}
		}

		if(parts!=null)
		{
			for(int i=0;i<parts.Length;i++)
			{
				parts[i].Play();
				parts[i].loop = loop;
			}
		}

		if(fade!=null)
		{
			for(int i=0;i<fade.Length;i++)
			{
				fade[i].FadeIn(1);
			}
		}

		if(ani!=null)
		{
			for(int i=0;i<ani.Length;i++)
			{
				ani[i].Play();
			}
		}

		Invoke("StopEffect",stopDelay+LifeTime);
	}

	void OnDisable()
	{
		ParticleEmitter[] emitters = transform.GetComponentsInChildren<ParticleEmitter>();
		ParticleSystem[] parts = transform.GetComponentsInChildren<ParticleSystem>();
		SpriteFade[] fade = transform.GetComponentsInChildren<SpriteFade>();
		tk2dSpriteAnimator[] ani = transform.GetComponentsInChildren<tk2dSpriteAnimator>();
		if(emitters!=null)
		{
			for(int i=0;i<emitters.Length;i++)
			{
				emitters[i].emit = false; 
			}
		}
		if(parts!=null)
		{
			for(int i=0;i<parts.Length;i++)
			{
				parts[i].Stop();
			}
		}

		if(fade!=null)
		{
			for(int i=0;i<fade.Length;i++)
			{
				fade[i].FadeOut(1);
			}
		}
		if(ani!=null)
		{
			for(int i=0;i<ani.Length;i++)
			{
				ani[i].Stop();
			}
		}
	}


	void StopEffect()
	{
		enabled = false;
		if(prefabpath=="Model/HitEffect/StunLightBuild")
		{
			stunBuildCount--;
		}
		
		if(prefabpath=="Model/HitEffect/StunLight")
		{
			stunTrooperCount--;
		}

		BattleData.Instance.effectCacheList[prefabpath].Enqueue(this);
		gameObject.SetActive(false);
		//Destroy(gameObject);
	}



	private bool loop;

	public float stopDelay = 1f;


	private static int stunTrooperCount;
	private static int stunBuildCount;

	public static EffectController PlayEffect(string path,Vector3 pos,float lifetime = 0, bool isloop = false)
	{
		if(path=="Model/HitEffect/StunLightBuild")
		{
			if(stunBuildCount>20)
			{
				//return null;
			}
			stunBuildCount++;
		}

		if(path=="Model/HitEffect/StunLight")
		{
			if(stunTrooperCount>20)
			{
				//return null;
			}
			stunTrooperCount++;
		}
		



		EffectController effect = null;
		if(BattleData.Instance.effectCacheList.ContainsKey(path))
		{
			Queue<EffectController> effectList = BattleData.Instance.effectCacheList[path];
			if(effectList!=null&&effectList.Count>0)
			{
				effect = effectList.Dequeue();
			}
			else
			{
				if(effectList == null) BattleData.Instance.effectCacheList[path] = new Queue<EffectController>();
			}
		}
		else
		{
			Queue<EffectController> effectList = new Queue<EffectController>();
			BattleData.Instance.effectCacheList.Add(path,effectList);
		}
		
		if(effect==null)
		{
			Object prefab = ResourceCache.Load(path);
			if(prefab==null)return null;
			GameObject effectObj = GameObject.Instantiate(prefab) as GameObject;
			effectObj.transform.parent = SpawnManager.GetInstance.bulletContainer;
			effect = effectObj.GetComponent<EffectController>();			
		}
		if(lifetime>0)
		effect.LifeTime = lifetime;
		effect.transform.position = pos;
		effect.prefabpath = path;
		//effect.TimeCounter = 0f;
		effect.loop = isloop;
		effect.gameObject.SetActive(true);
		effect.enabled = true;
		return effect;
	}


}
