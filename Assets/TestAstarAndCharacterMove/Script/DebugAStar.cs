using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding.Core;

public class DebugAStar : MonoBehaviour {

	public Vector2 begin;
	public Vector2 end;

	public bool status = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		if (status) 
		{
			PathFinderFast mPathFinder = new PathFinderFast (Globals.mMatrix);		
			mPathFinder.Formula = HeuristicFormula.Manhattan;           
			mPathFinder.Diagonals = true;
			mPathFinder.HeavyDiagonals = true;
			mPathFinder.HeuristicEstimate = 2;
			mPathFinder.PunishChangeDirection = false;
			mPathFinder.TieBreaker = false;
			mPathFinder.SearchLimit = 50000;

			Point beginP = new Point((int)begin.x,(int)begin.y);
			Point endP = new Point((int)end.x,(int)end.y);
			List<PathFinderNode> path = mPathFinder.FindPath (beginP, endP);
			//Debug.Log(mPathFinder.mCloseNodeCounter);
			for(int i=0;i<path.Count;i++)
			{
				if(i<path.Count-1)
				{
				PathFinderNode node = path[i];
				PathFinderNode nextNode = path[i+1];
				Debug.DrawLine(new Vector3(node.X,0.2f,node.Y),new Vector3(nextNode.X,0.2f,nextNode.Y));
				}
			}

		}
	}
}
