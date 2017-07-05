using UnityEngine;
using System.Collections;

public class ProjectileUpdater : MonoBehaviour {


	public ProjectileInfo projectileInfo;
	
	void Awake(){
		projectileInfo = GetComponent<ProjectileInfo> ();
	}


	// Update is called once per frame
	void Update () {
		if (projectileInfo.IsBoom) {
			return;
		}

		if(projectileInfo.State==AISTATE.MOVING)
		{		
			projectileInfo.projectileCtl.DoMove();
			if(projectileInfo.projectileCtl.CheckBeginAttack ())
			{
				projectileInfo.projectileCtl.CMDAttack();
			}
		}
		
		
		if(projectileInfo.State == AISTATE.ATTACKING)
		{
			//攻击;
			projectileInfo.projectileCtl.DoAttack();
		}
	}

}
