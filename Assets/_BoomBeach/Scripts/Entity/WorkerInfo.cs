using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding.Core;
using BoomBeach;

public class WorkerInfo:MonoBehaviour {

	public tk2dSpriteAnimator anim;
	
	void Awake(){
		anim = transform.Find ("characterPos/ani").GetComponent<tk2dSpriteAnimator> ();
		
	}

    /// <summary>
    /// 该CMD指出了该Worker Think的方向，初步看出其类似于GOAP的worldState
    /// </summary>
	public WorkerCMD wcmd;


	public WorkerSTATE wstate;

    /// <summary>
    /// 该工人工作的建筑
    /// </summary>
	public BuildInfo workingBuild;	

	public float speed; //工人的行进速度;

    /// <summary>
    /// 寻路的结束点位置
    /// </summary>
	public Vector3 Dest;

	public Vector3 Position{
		get{
			return transform.position;
		}
		set{
			transform.position = value;
		}
	}

	public List<PathFinderNode> path;
    public int pathIndex = 0;

	public Direct direction;
	public float Degree;
	public float RealDegree;
	public Vector3 NextPoint;

    /// <summary>
    /// worker行为决策相关
    /// </summary>
	public void Think()
	{

		if(wcmd==WorkerCMD.NORMAL)
		{
			//寻找任意建筑的随机周边点;
			int idx = Random.Range (0, DataManager.GetInstance().buildArray.Count);			
			BuildInfo b = DataManager.GetInstance().buildArray [idx];			
			Dest = Globals.GetRandStandPointAroundBuild (b,transform);
		}
		else if(wcmd == WorkerCMD.WORK)
		{
			//寻找指定建筑的随机周边点;
			Dest = Globals.GetRandBuildStandPointAroundBuild (workingBuild);
		}

		PathFinderFast mPathFinder = new PathFinderFast (Globals.mMatrix);		
		mPathFinder.Formula = HeuristicFormula.Manhattan;           
		mPathFinder.Diagonals = true;
		mPathFinder.HeavyDiagonals = true;
		mPathFinder.HeuristicEstimate = 2;
		mPathFinder.PunishChangeDirection = false;
		mPathFinder.TieBreaker = false;
		mPathFinder.SearchLimit = 50000;

		Point beginP = new Point(Mathf.FloorToInt(Position.x),Mathf.FloorToInt(Position.z)); 
		Point endP = new Point(Mathf.FloorToInt(Dest.x),Mathf.FloorToInt(Dest.z)); 
		List<PathFinderNode> fpath = null;
			fpath = mPathFinder.FindPath (beginP, endP); 
		if (fpath != null) {
			fpath.Reverse ();
		}
		wstate = WorkerSTATE.WALKING;
        // this.path = new BetterList<PathFinderNode>();
        this.path = fpath;
        pathIndex = 0;
        //Debug.Log("Think");
    }
}
