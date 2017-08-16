using UnityEngine;
using System.Collections;

public class DebugGrid : MonoBehaviour {


	public string[] gridDebugs;

	void Start()
	{
		gridDebugs = new string[1600];
	}
	// Update is called once per frame
	void Update () {
		int k = 0;
		for (int i=0; i<=Globals.GridArray.GetUpperBound(0); i++) {
			for (int j=0; j<=Globals.GridArray.GetUpperBound(1); j++) {
				GridInfo grid = Globals.GridArray[i,j];

				if(grid.isBuild)
				{
					gridDebugs[k] = grid.A+"_"+grid.B+"_"+grid.isBuild+"_"+grid.buildInfo.Id;
					k++;
				}
			}	
		}
	}
}
