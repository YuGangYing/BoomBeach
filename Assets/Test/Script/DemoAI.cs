using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding.Core;
using BoomBeach;


public class DemoAI  {

	//static bool tag = false;
	public static void think(CharInfo characterInfo)
	{

		/**
		 * 测试AI
		 * 1. 查找随机的可立点,即寻找Dest
		 * 2. 寻路
		 * 3. 找到路径->将指令改为Walk
		 */


		//Vector3 dest = new Vector3 (Random.Range(3f,30f),0f,Random.Range(3f,30f));

		int idx = Random.Range (0, DataManager.GetInstance().buildArray.Count);

		BuildInfo b = DataManager.GetInstance().buildArray [idx];

		Vector3 dest = Globals.GetRandStandPointAroundBuild (b,characterInfo.transform);
		/*
		if (tag)
		{
			dest = b.buildStandPoints [1].position;
			tag = false;
		}
		else
		{
			dest = b.buildStandPoints [3].position;
			tag= true;
		}
		*/
		//Debug.Log (b.Id);
		characterInfo.Dest = dest;

		PathFinderFast mPathFinder = new PathFinderFast (Globals.mMatrix);		
		mPathFinder.Formula = HeuristicFormula.Manhattan;           
		mPathFinder.Diagonals = true;
		mPathFinder.HeavyDiagonals = true;
		mPathFinder.HeuristicEstimate = 2;
		mPathFinder.PunishChangeDirection = false;
		mPathFinder.TieBreaker = false;
		mPathFinder.SearchLimit = 50000;

		//Debug.Log ("a:"+characterInfo.Position+" "+dest);
		Point beginP = new Point(Mathf.FloorToInt(characterInfo.Position.x),Mathf.FloorToInt(characterInfo.Position.z)); 
		Point endP = new Point(Mathf.FloorToInt(dest.x),Mathf.FloorToInt(dest.z)); 
		//Debug.Log ("b:"+beginP.X+","+beginP.Y+" "+endP.X+","+endP.Y);
		List<PathFinderNode> path = mPathFinder.FindPath (beginP, endP); 
		if (path != null) {
						path.Reverse ();
				}
		//characterInfo.cmd = CharacterCMD.WALK;
		characterInfo.path = path;


	}
}
