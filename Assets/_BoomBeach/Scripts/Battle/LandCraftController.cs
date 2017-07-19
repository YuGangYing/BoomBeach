using UnityEngine;
using System.Collections;
using BoomBeach;

public class LandCraftController : MonoBehaviour {

	// Use this for initialization

	public AISTATE state;

	public LandCraft landCraft;

	public Vector3 landPoint;

	private Vector3 beginPos; //出现点;
	  
	private Vector3 endPos;  //登陆点

	//定位登录舰的实际轴点;
	public void Locate()
	{
		Vector2 landPoint2D = new Vector2 (landPoint.x,landPoint.z);

		if(landCraft.beachData.beachDirect==Direct.RIGHT)
		{
			landPoint2D = new Vector2(landPoint2D.y,landPoint2D.x);
			//standAlias = Mathf.RoundToInt(landPoint.z);
		}
		else if(landCraft.beachData.beachDirect==Direct.RIGHTUP)
		{
			landPoint2D = Globals.TransformCoordinate(landPoint2D,45);
			//standAlias = Mathf.RoundToInt(landPoint.x);
		}

		int standAlias = Mathf.RoundToInt(landPoint2D.x);
		int standAliasResult = 0;  //最终计算出的停靠点;
		int length = 50;  //最小的两点距离;
		int standKey = 0;
		foreach(int i in landCraft.beachData.beachLineTag.Keys)//寻找x分量差最小的那个点
		{
			if(landCraft.beachData.beachLineTag[i]==0)
			{
				//没有停船;
				int currentLength = Mathf.RoundToInt((new Vector2(i,0) - new Vector2(standAlias,0)).magnitude);
				if(currentLength<length)
				{
					standAliasResult = i;
					length = currentLength;
					standKey = i;
				}
			}
		}

		Vector2 beginPos2D = new Vector2 (standAliasResult,landCraft.beachData.LandCraftBeginLocalY);
		Vector2 endPos2D = new Vector2 (standAliasResult,landCraft.beachData.LandCraftStopLocalY);

		if(landCraft.beachData.beachDirect==Direct.RIGHT)
		{
			beginPos2D = new Vector2(beginPos2D.y,beginPos2D.x);
			endPos2D = new Vector2(endPos2D.y,endPos2D.x);
		}
		else if(landCraft.beachData.beachDirect==Direct.RIGHTUP)
		{
			beginPos2D = Globals.TransformCoordinate(beginPos2D,-45);
			endPos2D = Globals.TransformCoordinate(endPos2D,-45);
		}


		beginPos = new Vector3 (beginPos2D.x,0,beginPos2D.y);
		endPos = new Vector3 (endPos2D.x,0,endPos2D.y);

		landCraft.beachData.beachLineTag[standKey] = 1;
		transform.position = beginPos;
 
	}

	private float speed = 0.05f; 
	// Update is called once per frame
	void Update () {
		if(state==AISTATE.MOVING)
		{		
			//进攻;
			if(Mathf.Approximately((transform.position-endPos).magnitude,0f))
			{
				state = AISTATE.STANDING;
				landCraft.setDeck("down");
				if(BattleData.Instance.AllTrooperRetreat)
				BattleData.Instance.AllTrooperRetreat = false;
				if(DataManager.GetInstance.sceneStatus==SceneStatus.ENEMYBATTLE)
				{
					if(!BattleData.Instance.BattleIsEnd)
					{
						if(!BattleData.Instance.TrooperDeployed)BattleData.Instance.TrooperDeployed = true; //已出兵;
						foreach(CharInfo charInfo in landCraft.TrooperList.Values)
						{
							//出兵前先分配撤退点;
							charInfo.RetreatPoint = charInfo.Position;
							charInfo.transform.parent = SpawnManager.GetInstance.characterContainer;//将小兵的父物体由原来的登陆舰的停靠点换成角色的全局父容器
							if(FocusFireAmmoController.FocusFireProjectile!=null)
							{
								FocusFireAmmoController ffac = FocusFireAmmoController.FocusFireProjectile.projectileCtl as FocusFireAmmoController;
								ffac.DoFocus(charInfo);
							}
							else
							{
								//charInfo.charCtl.FindDest(); //查找攻击目标;
								//通知所有的兵走到集合点;
								//登陆点直径三米范围内的随机点;
								Vector3 randLandPoint = Globals.GetRandPointInCircle(landPoint,1.5f);//登陆舰停靠完成时，小角色会从登陆舰上下来，然后往标记点上靠
								charInfo.Dest.x = randLandPoint.x;
								charInfo.Dest.z = randLandPoint.z;
								charInfo.IsOnlyMove = true;
								charInfo.trooperCtl.CMDMove();
							}
							BattleData.Instance.AllocateTrooperList.Add(charInfo);
						}
					}
				}

			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position,endPos,speed*Globals.TimeRatio); 
			}

		}
	}
}
