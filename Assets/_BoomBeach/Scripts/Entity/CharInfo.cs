using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding.Core;
using Pathfinding;
/* 
 * 备注关于csvInfo中的字段说明;
 * Speed: 每秒的移动速度(100=1米);
 * Hitpoints: 总血量，加成;  
 * MaxAttackRange: 最大的攻击范围(机枪兵，进入该范围，即开始攻击，边行走边攻击);
 * AttackRange: 攻击范围（进入该范围，停止行进，除机枪兵外开始攻击）;
 * AttackSpeed: 攻击速度（多久发射一发子弹，1000 = 1秒）;
 * Damage: DPS（每秒的伤害值），加成;
 * LifeLeach: 每攻击的回血量;
 * DamageSpread: 重机枪兵的攻击扫射范围，起点与终点都取随机值;
 * 

*/


public class CharInfo:MonoBehaviour {

	//是否仅行走，不启用攻击检测;
	public bool IsOnlyMove;

	//撤退点;
	public Vector3 RetreatPoint; 

	//已撤退;
	public bool IsRetreat;

	//撤退中;
	public bool IsRetreating;

	//用于记录行进中的路径;
	public ReplayNodeData walkListReplayData;

	//具体每个兵种的动作执行;
	public TrooperController trooperCtl;

	private Transform HealthBar;
	private UISprite HealthBarSprite;

	private Transform mTrans;
	float mSquaredAttackRange = 0;
	/// <summary>
	/// 初始化战斗角色;
	/// </summary>
	public void BattleInit()
	{
		isDead = false;
		CurrentHitPoint = trooperData.hitpoints;
		Id = BattleData.Instance.TrooperIdx;
		mTrans = transform;
		mTrans.name = trooperData.tidLevel+"_"+Id;//修改每个小兵的id
		BattleData.Instance.TrooperIdx++;
		trooperCtl = TrooperController.Instantiate (this);
		trooperCtl.CMDStand ();
		gameObject.AddComponent<TrooperUpdater> ();

		mSquaredAttackRange = Mathf.Pow(trooperData.csvInfo.AttackRange / 100f,2);
	}

	/// <summary>
	/// 战斗时的士兵ID;
	/// </summary>
	public int Id;

	/// <summary>
	/// 绑定的数据;
	/// </summary>
	public BattleTrooperData trooperData;

	public Direct direction;
	public float Degree;
	public float RealDegree;
	public Vector3 NextPoint;

	/// <summary>
	/// 移动路径;
	/// </summary>
	public List<PathFinderNode> path;

	public List<GraphNode> pathNodes;

	/// <summary>
	/// 总血量;
	/// </summary>
	public int HitPoint
	{
		get{ return trooperData.hitpoints; }
	}

	/// <summary>
	/// 当前血量;
	/// </summary>
	public float CurrentHitPoint;

	/// <summary>
	/// DPS;
	/// </summary>
	public int Damage{
		get{ return trooperData.damage; }
	}

	public float SquaredAttackRange{
		get{ 
			return mSquaredAttackRange;
		}
	}

	public float AttackRange{
		get{
			return trooperData.csvInfo.AttackRange / 100f;
		}
	}

	public float MaxAttackRange{
		get{
			return trooperData.csvInfo.MaxAttackRange/100f;
		}
	}

	public float AttackSpeed{
		get{
			return trooperData.csvInfo.AttackSpeed / 1000f;
		}
	}

	private float squrSpeed;

	/// <summary>
	/// 移动速度，每秒米，每100为1米, 获取的为每帧行走的距离;
	/// </summary>
	private float speed;
	public float Speed{
		get{ 
			if(speed==0)
			{
				speed = (trooperData.csvInfo.Speed / 100f) / (1f/Globals.baseTimeSpan); 
			}
			if(BattleData.Instance.BattleIsEnd)
			{
				return speed*2;
			}
			return speed;
		}
	}    

	/// <summary>
	/// 是否已死亡;
	/// </summary>
	public bool isDead;

	/// <summary>
	/// 移动目标点;
	/// </summary>
	public Vector3 Dest; 

	/// <summary>
	/// 攻击目标点;
	/// </summary>
	public Vector3 AttackDest; 

	/// <summary>
	/// 角色状态机;
	/// </summary>
	public AISTATE State;

	/// <summary>
	/// 角色攻击状态机;
	/// </summary>
	public AISTATE AttackState;

	/// <summary>
	/// 攻击的建筑物;
	/// </summary>
	public BuildInfo AttackBuildInfo;

	/// <summary>
	/// 是否已找到目标，已找到目标将执行寻路;
	/// </summary>
	public bool IsFindDest;

	/// <summary>
	/// 角色的实际坐标;
	/// </summary>
	public Vector3 Position{
		get{
			return mTrans.position;
		}
		set{
			mTrans.position = value;
		}
	}

	private bool _isStun;
	public bool isInStun{
		get{
			return _isStun;
		}
		set{
			_isStun = value;
			if(value)
			{
				CharTweener tweener = GetComponentInChildren<CharTweener> ();
				if(tweener!=null)
				{
					tweener.enabled = true;
					tweener.PlayFlash();
				}
			}
			else
			{
				CharTweener tweener = GetComponentInChildren<CharTweener> ();
				if(tweener!=null)
				{
					tweener.Stop();
				}
			}
		}
	}   

	public ProjectileInfo StunProjectile; 
	
	public bool isInSmoke;

	public ProjectileInfo SmokeProjectile;

	public float DamageRadius{
		get{return trooperData.csvInfo.DamageRadius/100f; }
	}

	public tk2dSpriteAnimator anim;

	void Awake(){
		mTrans = transform;
		anim = mTrans.Find ("characterPos/ani").GetComponent<tk2dSpriteAnimator> ();
		HealthBar = mTrans.Find ("UI/HealthBar");
		if(HealthBar!=null)
			HealthBarSprite = HealthBar.Find ("Barbg/bar").GetComponent<UISprite> ();
		_shakeOriginPos = mTrans.Find("characterPos").localPosition;
	}

	public void SetHealthPercent(float percent)
	{
		StopAllCoroutines ();
		if (!HealthBar.gameObject.activeSelf&&!isDead)
			HealthBar.gameObject.SetActive (true);
		if (percent < 0)
			percent = 0;
		if (percent > 1)
			percent = 1;
		HealthBarSprite.transform.localScale = new Vector3 (percent,1f,1f);
		StartCoroutine (DoHideHealthBar (5f));
	}
	
	IEnumerator DoHideHealthBar(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		HealthBar.gameObject.SetActive (false);
	}



	private bool _isShake;
	public bool isShake{
		set{
			_isShake = value;
		}
		get{
			return _isShake;
		}
	}
	
	private float _moveStep = 0.05f;
	public float moveStep{
		get{
			return _moveStep;
		}
	}
	
	private float _moveStepCount;
	public float moveStepCount{
		set{
			_moveStepCount = value;
		}
		get{
			return _moveStepCount;
		}
	}
	
	private float _shakeSpeed = 0.05f;
	public float shakeSpeed{
		get{
			return _shakeSpeed;
		}
	}
	
	private int _shakeDirect = 1;
	public int shakeDirect{
		set{
			_shakeDirect = value;
		}
		get{
			return _shakeDirect;
		}
	}
	
	private float _shakeTime = 0.1f;
	public float shakeTime{
		get{
			return _shakeTime;
		}
	}
	
	private float _shakeTimeCount;
	public float shakeTimeCount{
		get{
			return _shakeTimeCount;
		}
		set{
			_shakeTimeCount = value;
		}
	}

	private Vector3 _shakeOriginPos;
	public Vector3 shakeOriginPos{
		get{
			return _shakeOriginPos;
		}
	}

}
