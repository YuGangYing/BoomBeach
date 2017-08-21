using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoomBeach
{
	public class BuildGridManager : SingleMonoBehaviour<BuildGridManager>
	{
		//Gird
		public GridInfo[,] GridArray;
		//常量，建筑区域大小;
		public const int GRID_TOTAL = 50;
		//不可通行权重;
		public const int GRID_BUILD_COST = 0;
		//可通行权重;
		public const int GRID_EMPTY_COST = 1;

		protected override void Awake ()
		{
			base.Awake ();
			InitGrid ();
		}

		void InitGrid ()
		{
			GridArray = new GridInfo[GRID_TOTAL, GRID_TOTAL];
			for (int a = 0; a < GRID_TOTAL; a++) {
				for (int b = 0; b < GRID_TOTAL; b++) {
					GridInfo gridInfo = new GridInfo ();
					gridInfo.A = a;
					gridInfo.B = b;
					gridInfo.GridPosition = new Vector3 (a, 0f, b);
					gridInfo.standPoint = new Vector3 ((float)a + 0.5f, 0f, (float)b + 0.5f);
					gridInfo.isInArea = true;
					gridInfo.cost = GRID_EMPTY_COST;
					if (CSVManager.GetInstance.islandGridsDic [Globals.islandType.ToString ()] [a, b] == 1) {
						gridInfo.isInArea = false;
						gridInfo.cost = GRID_BUILD_COST;
					}
					GridArray [a, b] = gridInfo; 
				}
			}	
		}

		/* 获取任意一个坐标点所对应的GridInfo */
		public GridInfo LocateGridInfo (Vector3 position)
		{
			int A = (int)position.x;
			int B = (int)position.z;
			GridInfo gridInfo = null;
			if (A >= 0 && A <= GridArray.GetUpperBound (0) && B >= 0 && B <= GridArray.GetUpperBound (1)) {
				gridInfo = GridArray [A, B];
			} else {
				gridInfo = new GridInfo ();
				gridInfo.A = A;
				gridInfo.B = B;
				gridInfo.standPoint = new Vector3 ((float)A + 0.5f, 0f, (float)A + 0.5f);
				gridInfo.isInArea = false;
				gridInfo.GridPosition = new Vector3 (A, 0, B);
			}
			return gridInfo;
		}

	}
}
