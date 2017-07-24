using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct FireItem{
	public BattleTrooperData btd;
	public Vector3 attackPoint;
}
public class GunBoat : MonoBehaviour {

	public Transform FirePoint;
	private Direct pdirect;
	public Direct direct{
		set{
			pdirect = value;
			if(pdirect==Direct.RIGHTUP)
			{
				modelSprite2.gameObject.SetActive(true);
				modelSprite.gameObject.SetActive(false);
			}
			else
			{
				modelSprite2.gameObject.SetActive(false);
				modelSprite.gameObject.SetActive(true);
				if(pdirect==Direct.RIGHT)
				{
					modelSprite.scale = Vector3.one;
				}

				if(pdirect==Direct.UP)
				{
					modelSprite.scale = new Vector3(-1,1,1);
				}
			}

			FirePoint = transform.Find("buildPos/BuildMain/FirePoint/"+pdirect.ToString());
		}
	}

	private tk2dSprite modelSprite;
	private tk2dSprite modelSprite2;
	// Use this for initialization
	public void Init(){
		Animation[] anis = GetComponentsInChildren<Animation> ();
		for(int i=0;i<anis.Length;i++)
			anis[i].wrapMode = WrapMode.Loop;

		modelSprite = transform.Find ("buildPos/BuildMain/model/gunboat").GetComponent<tk2dSprite> ();
		modelSprite2 = transform.Find ("buildPos/BuildMain/model/gunboat2").GetComponent<tk2dSprite> ();

		weaponFireTime = new Dictionary<string, float> ();
		fireList = new Queue<FireItem> ();
	}


	//每种武器发射的时间点，key:tid_level, val:发射的时间间隔;
	public Dictionary<string,float> weaponFireTime;
	
	public Queue<FireItem> fireList;
	
	public void Fire(FireItem fireItem)
	{
		fireList.Enqueue (fireItem);
	}

}
