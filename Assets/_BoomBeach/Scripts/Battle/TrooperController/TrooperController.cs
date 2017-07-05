using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding.Core;
using BoomBeach;
using Pathfinding;

public class TrooperController  {

	/// <summary>
	/// 相应的角色;
	/// </summary>
	public CharInfo charInfo;

	/// <summary>
	/// 攻击计时器;
	/// </summary>
	protected float attackTimeCounter = 0f;

	//==================可重写操作===================;

	/// <summary>
	/// 检测是否开始攻击;
	/// </summary>
	public virtual bool CheckBeginAttack()	{
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{
			if(charInfo.AttackBuildInfo!=null&&(charInfo.Position - charInfo.AttackDest).sqrMagnitude<= charInfo.SquaredAttackRange)
			{
				//重新计算兵种方向;
				CaclCharInfoDirect(BattleController.GetBuildCenterPosition(charInfo.AttackBuildInfo));

				//默认兵种开始攻击后，改变状态为站立;
				CMDStand ();
				if(charInfo.path!=null)
				charInfo.path.Clear();

				//行走结束，上传路径结点(重机枪兵需重写，不用上传，等攻击过程中检测成立后上传);
				if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
					BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);
				return true;
			}
			else
			{
				if(charInfo.AttackBuildInfo==null||charInfo.AttackBuildInfo.IsDead)
				{
					if(!charInfo.IsOnlyMove)
					{
						charInfo.AttackBuildInfo = null;
						charInfo.IsFindDest = false;
						attackTimeCounter = 0; //攻击计时器归零;
						//行走结束，上传路径结点(重机枪兵需重写，不用上传，等攻击过程中检测成立后上传);
						if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
							BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);
						CMDFindDest(); //重新查找目标;
					}
				}
				return false;
			}
		}
		else
		{
			return false;  //回放场景不作检测，等待回放控制器通知;
		}
	}


	/// <summary>
	/// 检测是否要停止攻击;
	/// </summary>
	public virtual bool CheckEndAttack()
	{
		if(charInfo.AttackBuildInfo!=null)
		{
			bool isEnd = false;
			if(charInfo.AttackBuildInfo.IsDead)
			{
				charInfo.AttackBuildInfo = null;
				charInfo.IsFindDest = false;
				charInfo.AttackState = AISTATE.STANDING;
				attackTimeCounter = 0; //攻击计时器归零;
				isEnd = true;
			}
			return isEnd;
		}
		else
		{
			return true;
		}

	}

	/// <summary>
	/// 执行攻击操作;
	/// </summary>
	public virtual void DoAttack()
	{
		if(attackTimeCounter==0)
		{
			//播放攻击动画;
			PlayAttack();
			//计算每次攻击的伤害;
			float damage = charInfo.Damage * charInfo.AttackSpeed;
			charInfo.AttackBuildInfo.buildCtl.CMDUnderAttack(damage);
		}
		if(attackTimeCounter>=charInfo.AttackSpeed)
		{
			attackTimeCounter = 0f;
		}
		else
		{
			attackTimeCounter+=Time.deltaTime;
		}
	}

	public virtual void FindPathNodes(){
		Path p = ABPath.Construct (charInfo.transform.position,charInfo.Dest);
		charInfo.pathNodes = p.path;
		//AstarPath.active.ReturnPaths (false);
	}


	/// <summary>
	/// 寻路，必需找到路径;
	/// </summary>
	public virtual void FindPath()
	{
		PathFinderFast mPathFinder = new PathFinderFast (Globals.mMatrix);		
		mPathFinder.Formula = HeuristicFormula.Manhattan;           
		mPathFinder.Diagonals = true;
		mPathFinder.HeavyDiagonals = true;
		mPathFinder.HeuristicEstimate = 2;
		mPathFinder.PunishChangeDirection = false;
		mPathFinder.TieBreaker = false;
		mPathFinder.SearchLimit = 50000;

		bool bOutBound = false;
		bool eOutBound = false;

		int bx = Mathf.FloorToInt (charInfo.Position.x);
		int by = Mathf.FloorToInt (charInfo.Position.z);

		if (bx < 0) 
		{
			bx = 0;
			bOutBound = true;
		}
		if (bx >= Globals.GridTotal)
		{
			bx = Globals.GridTotal - 1;
			bOutBound = true;
		}

		if (by < 0) 
		{
			by = 0;
			bOutBound = true;
		}
		if (by >= Globals.GridTotal)
		{
			by = Globals.GridTotal - 1;
			bOutBound = true;
		}
		Point beginP = new Point (bx,by);

		int ex = Mathf.FloorToInt (charInfo.Dest.x);
		int ey = Mathf.FloorToInt (charInfo.Dest.z);
		
		if (ex < 0)
		{
			ex = 0;
			eOutBound = true;
		}
		if (ex >= Globals.GridTotal) 
		{
			ex = Globals.GridTotal - 1;
			eOutBound = true;
		}
		
		if (ey < 0) 
		{
			ey = 0;
			eOutBound = true;
		}
		if (ey >= Globals.GridTotal) 
		{
			ey = Globals.GridTotal - 1;
			eOutBound = true;
		}
		Point endP = new Point(ex,ey); 

		if(!bOutBound||!eOutBound)
		{
			List<PathFinderNode> fpath = null;
			fpath = mPathFinder.FindPath (beginP, endP); 
			if (fpath != null) {
				fpath.Reverse ();
			}
			charInfo.path = fpath;
		}
	}
	
	
	/// <summary>
	/// 查找目标，必需返回CharInfo.Dest;
	/// </summary>
	public virtual void FindDest()
	{
		BinaryHeap<BattleFindItem> findList = new BinaryHeap<BattleFindItem> ();//二插堆排序，
		BuildInfo build;
		for(int i =0;i<BattleData.Instance.buildList.Count;i++){
			build = BattleData.Instance.buildList [i];
			if(build!=null&&!build.IsDead)
			{
				BattleFindItem eachitem = new BattleFindItem();
				eachitem.distance = Mathf.RoundToInt((BattleController.GetBuildCenterPosition(build) - charInfo.Position).magnitude*1000f);
				eachitem.item = build.gameObject;
				findList.Add(eachitem);
			}
		}

		/*
		foreach(BuildInfo build in BattleData.Instance.buildDic.Values)
		{
			if(build!=null&&!build.IsDead)
			{
				BattleFindItem eachitem = new BattleFindItem();
				eachitem.distance = Mathf.RoundToInt((BattleController.GetBuildCenterPosition(build) - charInfo.Position).magnitude*1000f);
				eachitem.item = build.gameObject;
				findList.Add(eachitem);
			}
		}
		*/
		if(findList.Count>0) //找到最近的攻击目标;
		{
			BattleFindItem findItem = findList.Peek ();
			BuildInfo findBuild = findItem.item.GetComponent<BuildInfo> ();
			NotifyCharInfoFindDest(charInfo,findBuild);
			//Debug.Log("找到需通知周围兵");
			foreach(CharInfo trooper in BattleData.Instance.trooperDic.Values)
			{
				if(!trooper.IsFindDest&&(charInfo.Position - trooper.Position).magnitude<1.5f)
				{
					NotifyCharInfoFindDest(trooper,findBuild);
				}
			}
			
		}
	}

    /// <summary>
    /// 通知指定的角色并向其分配需要攻击的建筑目标
    /// </summary>
    /// <param name="charInfo"></param>
    /// <param name="findBuild"></param>
	public void NotifyCharInfoFindDest(CharInfo charInfo,BuildInfo findBuild)
	{
		charInfo.AttackBuildInfo = findBuild;
		charInfo.IsFindDest = true;//找到目标建筑
		charInfo.IsOnlyMove = false;
		//开始获取寻路点;
		charInfo.Dest = BattleController.GetBuildAroundRandPosition(findBuild);
		//charInfo.Dest = findBuild.Position;
		//获取攻击点;
		charInfo.AttackDest = BattleController.GetBuildAreaRandPosition(findBuild);
		
		//记录回放数据;
		if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			ReplayNodeData rnd = new ReplayNodeData();
			rnd.SelfID = charInfo.Id;
			rnd.SelfType = EntityType.Trooper;
			rnd.State = AISTATE.FINDINGDEST;
			rnd.DestX = charInfo.Dest.x;
			rnd.DestZ = charInfo.Dest.z;
			BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
		}
	}

	Vector3 FindDest(CharInfo charInfo,BuildInfo findBuild){
		//Vector3 pos = findBuild.Position;
		Vector3 targetPos = charInfo.transform.position - findBuild.Position;
		Quaternion Q = new Quaternion ();
		float angle = Random.Range(-15,15);
		Q.w = Mathf.Cos (angle / 2);
		Q.x = 0; 
		Q.y = Mathf.Sin (angle / 2);  
		Q.z = 0;
		targetPos = Q * targetPos * charInfo.AttackRange / targetPos.magnitude + findBuild.Position;
		return targetPos;
	}



	//==================end 可重写操作===================;

	//==================动画播放控制===================;

	/// <summary>
	/// 播放站立动画;
	/// </summary>
	public virtual void PlayStand()
	{
		//默认播放站立时，攻击状态不播放;
		if(charInfo.AttackState!=AISTATE.ATTACKING||charInfo.isInSmoke)
		{
			string clipName = "Stand";
			string direct = Globals.GetDirectionString(charInfo.direction);
			if(charInfo.direction==Direct.RIGHTUP)
			{
				if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
					direct=Globals.GetDirectionString(Direct.RIGHT);
			}
			if(charInfo.direction==Direct.LEFTDOWN)
			{
				if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
					direct=Globals.GetDirectionString(Direct.LEFT);
			}
			clipName+=direct;
			
			if(!charInfo.anim.IsPlaying(clipName))
			{
				if(charInfo.anim.GetClipByName (clipName)!=null)
				charInfo.anim.Play(clipName);
			}
		}
	}

	/// <summary>
	/// 播放行走动画;
	/// </summary>
	public virtual void PlayMove()
	{
		//以下是动画播放;
		string clipName = "Walk";
		string direct = Globals.GetDirectionString(charInfo.direction);
		if(charInfo.direction==Direct.RIGHTUP)
		{
			if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Globals.GetDirectionString(Direct.RIGHT);
		}
		if(charInfo.direction==Direct.LEFTDOWN)
		{
			if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Globals.GetDirectionString(Direct.LEFT);
		}
		clipName+=direct;
		
		if(!charInfo.anim.IsPlaying(clipName))
		{
			if(charInfo.anim.GetClipByName (clipName)!=null)
				charInfo.anim.Play(clipName);
		}
	}

	/// <summary>
	/// 播放攻击动画;
	/// </summary>
	public virtual void PlayAttack()
	{
		//以下是动画播放;
		string clipName = "Attack";
		string direct = Globals.GetDirectionString(charInfo.direction);
		if(charInfo.direction==Direct.RIGHTUP)
		{
			
			if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Globals.GetDirectionString(Direct.RIGHT);
		}
		if(charInfo.direction==Direct.LEFTDOWN)
		{
			if(charInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Globals.GetDirectionString(Direct.LEFT);
		}
		clipName+=direct;
		
		if(!charInfo.anim.IsPlaying(clipName))
		{
			if(charInfo.anim.GetClipByName(clipName)!=null)
			charInfo.anim.Play(clipName);
		}
	}

	//==================end 动画播放控制===================;


	//==================指令函数===================;


	public void CMDShake()
	{
		charInfo.isShake = true;
	}

	/// <summary>
	/// 寻找目标指令;
	/// </summary>
	public void CMDFindDest()
	{
		//仅真实攻击时才寻找攻击目标，回放时由回放数据列中获取;
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{
			AITask.Instance.FindDest (charInfo);
		}
		charInfo.State = AISTATE.FINDINGDEST;
	}
	
	/// <summary>
	/// 寻找路径指令;
	/// </summary>
	public void CMDFindPath()
	{
		//仅真实攻击时才寻路，回放时数据由回放列表中获取;
		if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE)
		{
			AITask.Instance.FindPath (charInfo);
		}
		charInfo.State = AISTATE.FINDINGPATH;
	}
	
	/// <summary>
	/// 行走指令;
	/// </summary>
	public void CMDMove()
	{
		charInfo.State = AISTATE.MOVING;
		//记录回放数据，不记录已寻到的路径，在行走中记录;
		if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			charInfo.walkListReplayData = new ReplayNodeData();
			charInfo.walkListReplayData.SelfID = charInfo.Id;
			charInfo.walkListReplayData.SelfType = EntityType.Trooper;
			charInfo.walkListReplayData.State = AISTATE.MOVING;
			charInfo.walkListReplayData.DestX = charInfo.Dest.x;
			charInfo.walkListReplayData.DestZ = charInfo.Dest.z;
			
			//不推入队列，表示不进行上传记录节点，行进结束上传;
			//BattleData.Instance.BattleCommondQueue.Enqueue(rnd.ToStringArray());
		}
	}
	
	/// <summary>
	/// 攻击指令;
	/// </summary>
	public void CMDAttack()
	{
		charInfo.AttackState = AISTATE.ATTACKING;
		//记录回放数据;
		if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
		{
			ReplayNodeData rnd = new ReplayNodeData();
			rnd.SelfID = charInfo.Id;
			rnd.SelfType = EntityType.Trooper;
			rnd.StandX = charInfo.Position.x;
			rnd.StandZ = charInfo.Position.z;
			rnd.AttackState = AISTATE.ATTACKING;
			rnd.DestX = charInfo.AttackDest.x; 
			rnd.DestZ = charInfo.AttackDest.z; 
			BattleData.Instance.BattleCommondQueue.Enqueue(rnd);
		}
	}
	
	/// <summary>
	/// 站立指令;
	/// </summary>
	public void CMDStand()
	{
		charInfo.State = AISTATE.STANDING;
	}
	
	/// <summary>
	/// 扣血指令，该指令比较特殊，没有AI状态，即实时更新UI
	/// 撤退时已退到运输船上以及Battle.Instance.BattleIsSuccess为true时不再扣血;
	/// </summary>
	public void CMDUnderAttack(float damage)
	{	
		if(BattleData.Instance.BattleIsOver)return;
		if(!charInfo.isDead)
		{
			if(damage<0)
			{
				CharTweener tweener = charInfo.GetComponentInChildren<CharTweener> ();
				if(tweener!=null)
				{
					tweener.enabled = true;
					tweener.PlayLight();
				}
			}
			charInfo.CurrentHitPoint -= damage;
			if(charInfo.CurrentHitPoint>=charInfo.HitPoint)charInfo.CurrentHitPoint = charInfo.HitPoint;

            //死亡
            if (charInfo.CurrentHitPoint<=0)
			{
				BattleData.Instance.DeadTrooperList.Add(charInfo);
				charInfo.CurrentHitPoint = 0f;
				if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
				{
                    //记录回放数据;
                    ReplayNodeData rnd = new ReplayNodeData();
					rnd.SelfID = charInfo.Id;
					rnd.SelfType = EntityType.Trooper;
					rnd.IsUnderAttack = 1;
                    BattleData.Instance.BattleCommondQueue.Enqueue(rnd);

					charInfo.isDead = true;
					charInfo.trooperData.num-=1;

                    //更新UI
					//BattleTrooperItem bti = (BattleTrooperItem)charInfo.trooperData.uiItem;
					//bti.trooperNum = charInfo.trooperData.num;
					//TODO


					//有兵死亡时,通知服务器;
					Helper.SendAttackDestroyedTroops(charInfo.Id, charInfo.trooperData);

					//战斗结束检测;
					if(BattleController.CheckAllTroopDead())
					{
						BattleData.Instance.BattleIsEnd = true;
                        //炮弹打完，且兵也都消死了，直接弹窗;
                        if (ScreenUIManage.Instance != null) 
							ScreenUIManage.Instance.battleResultWin.ShowResultWin();
						UIManager.GetInstance ().GetController<BattleResultCtrl>().ShowPanel ();
					}

				}
			}

			if(charInfo.CurrentHitPoint<charInfo.HitPoint)
			charInfo.SetHealthPercent (charInfo.CurrentHitPoint/charInfo.HitPoint); //更新UI;
		}
	}

	//==================end 指令函数===================;

	//==================工具函数===================;
	protected int tcount = 0;
	protected GridInfo lastGrid;


	public void MoveAction(){
		if (charInfo.pathNodes != null && charInfo.pathNodes.Count > 0) {
		
		
		}
	}



	/// <summary>
	/// 位移脚本;
	/// </summary>
	public virtual void DoMove()
	{
		Vector3 goal = Vector3.zero;

		if(charInfo.path!=null&&charInfo.path.Count>0)
		{
			PathFinderNode node = charInfo.path[0]	;
			GridInfo grid = Globals.GridArray[node.X,node.Y];

			if(grid.standPoint==Vector3.zero)
				goal = new Vector3((float)node.X+0.5f,0f,(float)node.Y+0.5f) + new Vector3(Random.Range(-0.5f,0.5f),0f,Random.Range(-0.5f,0.5f));
			else
				goal = grid.standPoint;
		}
		else
		{
			goal = charInfo.Dest;
		}

		
		CaclCharInfoDirect(goal);
		charInfo.Position = Vector3.MoveTowards(charInfo.Position,goal,charInfo.Speed*Globals.TimeRatio);
		
		GridInfo standGrid = Globals.LocateGridInfo(charInfo.Position);
		if(lastGrid==null||lastGrid!=standGrid)//当角色经过建筑时会对建筑做一些透明处理
		{
			//还原原经过的遮挡建筑透明度;
			if(lastGrid!=null)
			{
				for(int i=0;i<lastGrid.buildMarks.Count;i++)
				{
					BuildTweener tw = lastGrid.buildMarks[i].buildInfo.GetComponentInChildren<BuildTweener>();
					if(lastGrid.buildMarks[i].buildInfo.behindCharacters.Contains(charInfo))
						lastGrid.buildMarks[i].buildInfo.behindCharacters.Remove(charInfo);
					
					
					if(lastGrid.buildMarks[i].buildInfo.behindCharacters.Count==0
					   &&lastGrid.buildMarks[i].buildInfo.behindBuilds.Count==0&&tw!=null)
					{
						tw.enabled = true;
						tw.Alpha = 1f; 
					}
				}
			}
			
			lastGrid = standGrid;
			//设置当前站立格子的遮挡建筑透明度;
			for(int i=0;i<standGrid.buildMarks.Count;i++)
			{
				BuildTweener tw = standGrid.buildMarks[i].buildInfo.GetComponentInChildren<BuildTweener>();
				
				if(!lastGrid.buildMarks[i].buildInfo.behindCharacters.Contains(charInfo))
					lastGrid.buildMarks[i].buildInfo.behindCharacters.Add(charInfo);
				
				if(tw!=null)
				{
					tw.enabled = true;
					tw.Alpha =  Globals.obstacleAlpha;
				}
			}
			
		}
		
		if((charInfo.Position-goal).magnitude <= charInfo.Speed )
		{
			if(charInfo.path!=null&&charInfo.path.Count>0)
			{
				PathFinderNode node = charInfo.path[0];
				//开始往录像节点中放入行进过的格子;
				if(DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE)
				{
					charInfo.walkListReplayData.walkList.add(node);
				}
				charInfo.path.Remove(node);
			}
			else
			{
				//行走到目的地，但却未能触发战斗，重新寻找目标;
				//行走结束，上传路径结点;
				if(DataManager.GetInstance().sceneStatus==SceneStatus.ENEMYBATTLE&&charInfo.walkListReplayData!=null)
					BattleData.Instance.BattleCommondQueue.Enqueue(charInfo.walkListReplayData);

				if(BattleData.Instance.BattleIsEnd)
				{
					if(charInfo.IsRetreating)
					{
						BattleData.Instance.RetreatTrooperList.Add(charInfo);
						charInfo.IsRetreat = true;
						charInfo.IsRetreating  = false;
					}
					charInfo.IsOnlyMove = false;
					charInfo.trooperCtl.CMDStand();
				}
				else
				{
					charInfo.IsOnlyMove = false;
					charInfo.trooperCtl.CMDFindDest();

				}
			}
		}
	}

	/// <summary>
	/// 计算角色方向;
	/// </summary>
	public void CaclCharInfoDirect(Vector3 goal)
	{
		Vector3 charPosition = charInfo.Position;
		float degree = Globals.CaclDegree (new Vector2(charPosition.x,charPosition.z),new Vector2(goal.x,goal.z));
		charInfo.Degree = degree;
		charInfo.NextPoint = goal;
		
		Vector2 offset = new Vector2(goal.x,goal.z) - new Vector2(charPosition.x,charPosition.z);
		
		charInfo.RealDegree = Mathf.Atan (offset.y / offset.x) * Mathf.Rad2Deg;
		
		if ((degree >= 0 && degree < 22.5) || (degree <= 360f && degree >= (360 - 22.5))) 
		{
			charInfo.direction = Direct.RIGHT;		
		}
		else if( degree>=22.5&&degree<(45+22.5))
		{
			charInfo.direction = Direct.RIGHTUP;	
		}
		else if( degree>=(45+22.5)&&degree<(90+22.5))
		{
			charInfo.direction = Direct.UP;	
		}
		else if( degree>=(90+22.5)&&degree<(135+22.5))
		{
			charInfo.direction = Direct.LEFTUP;	
		}
		else if( degree>=(135+22.5)&&degree<(180+22.5))
		{
			charInfo.direction = Direct.LEFT;	
		}
		else if( degree>=(180+22.5)&&degree<(225+22.5))
		{
			charInfo.direction = Direct.LEFTDOWN;	
		}
		else if( degree>=(225+22.5)&&degree<(270+22.5))
		{
			charInfo.direction = Direct.DOWN;	
		}
		else if( degree>=(270+22.5)&&degree<(315+22.5))
		{
			charInfo.direction = Direct.RIGHTDOWN;	
		}
	}
	//==================end 工具函数===================;

	/// <summary>
	/// 获取实例对象;
	/// </summary>
	public static TrooperController Instantiate(CharInfo _charInfo)
	{
		TrooperController trooperCtl = null;
		if(_charInfo.trooperData.tid=="TID_RIFLEMAN")
		{
			trooperCtl = new RiflemanController();
		}
		if(_charInfo.trooperData.tid=="TID_HEAVY")
		{
			trooperCtl = new HeavyController();
		}
		if(_charInfo.trooperData.tid=="TID_ZOOKA")
		{
			trooperCtl = new ZookaController();
		}
		if(_charInfo.trooperData.tid=="TID_WARRIOR")
		{
			trooperCtl = new WarriorController();
		}
		if(_charInfo.trooperData.tid=="TID_TANK")
		{
			trooperCtl = new TankController();
		}
		trooperCtl.charInfo = _charInfo;
		return trooperCtl;

	}

}
