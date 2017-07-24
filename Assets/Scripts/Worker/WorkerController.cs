using UnityEngine;
using System.Collections;
using PathFinding.Core;

public class WorkerController : MonoBehaviour {

	public WorkerInfo characterInfo;
	
	void Awake(){
		characterInfo = GetComponent<WorkerInfo> ();
	}
	// Update is called once per frame
	void Update () {
		if (!WorkerManager.GetInstance.isInit)
						return;
		if(characterInfo.wcmd==WorkerCMD.NORMAL)
		{
			if(characterInfo.workingBuild!=null)
			{
				secondCounter = 0f;
				characterInfo.wcmd = WorkerCMD.WORK;
				return;
			}
		}
		else if(characterInfo.wcmd==WorkerCMD.WORK)
		{
			if(characterInfo.workingBuild==null)
			{
				secondCounter = 0f;
				characterInfo.wcmd = WorkerCMD.NORMAL;
				return;
			}

			if(characterInfo.workingBuild.status!=BuildStatus.New&&characterInfo.workingBuild.status!=BuildStatus.Upgrade)
			{
				secondCounter = 0f;
				characterInfo.workingBuild = null;
				characterInfo.wcmd = WorkerCMD.NORMAL;
				return;
			}
		}

		
		if (characterInfo.wstate == WorkerSTATE.STANDING) 
		{
			stand();
			return;
		}

		
		if (characterInfo.wstate == WorkerSTATE.WALKING) 
		{
			move();	
			return;
		}
				
	}


	private float secondSpan = 2f; //常态时站立秒数与工作状态秒数;
	private float secondCounter = 0f;
	
    /// <summary>
    /// 在2到5秒之间的思考时间
    /// </summary>
    /// <returns></returns>
	bool countTime()
	{
		if(secondCounter==0)
		{
			secondSpan = Random.Range(2f,5f);
		}
		secondCounter+=Time.fixedDeltaTime;

		if(secondCounter>=secondSpan)
		{
			secondCounter = 0f;
			return false;
		}
		return true;
	}

	//站立时执行(包含工作中与普通站立);
	string clipStand = "Stand";
	string clipWork = "Work";
	string clipName = "";
	void stand()
	{
		clipName = "";
		if(characterInfo.wcmd == WorkerCMD.NORMAL)//如果角色为行进状态，则采用Stand动画
		{
			clipName = clipStand;
		}
		else if(characterInfo.wcmd == WorkerCMD.WORK)//如果角色为工作状态，则采用Work动画
		{
			clipName = clipWork;
            //计算建筑的中心点相对于角色的方位
			caclDirect(characterInfo.workingBuild.Position + new Vector3(characterInfo.workingBuild.GridCount*1f/2f,0,characterInfo.workingBuild.GridCount*1f/2f) );

		}

		string direct = characterInfo.direction.ToString();
		if(characterInfo.direction==Direct.RIGHTUP)
		{
			if(characterInfo.anim.GetClipByName(clipName+direct.ToString())==null)//帧动作师如果没有做这种动画，则用现有的一些动画替代
				direct=Direct.RIGHT.ToString();				
		}
		if(characterInfo.direction==Direct.LEFTDOWN)
		{
			if(characterInfo.anim.GetClipByName(clipName+direct.ToString())==null)
				direct=Direct.LEFT.ToString();
		}
		clipName+=direct.ToString();//选择动画名

        this.currentClipName = clipName;

        if (!characterInfo.anim.IsPlaying(clipName))
		{
			characterInfo.anim.Play(clipName);
		}

		if(!countTime())
		{
			characterInfo.Think();
		}
	}


    public string currentClipName = "";

	
	/**
	 * 角色行走移动的相关操作
	 * 1. 向最先的路径点移动一步，并计算方向
	 * 2. 验证是否移到目的点，并再无路径
	 * 3. 有继续移动，无重新指定指令为站立，并进行AI计算;
	 */
	//private GridInfo lastGrid;
	Direct direct;
	//Direct preDirect;
	string clipWalk = "Walk";
    PathFinderNode node;
    //int nodeIndex = 0;
    void move()
	{
		clipName = clipWalk;
		direct = characterInfo.direction;
		if(true)
		{
			if(characterInfo.direction==Direct.RIGHTUP)
			{
				//if(characterInfo.anim.GetClipByName(clipName+direct.ToString())==null)
					direct=Direct.RIGHT;				
			}
			if(characterInfo.direction==Direct.LEFTDOWN)
			{
				//if(characterInfo.anim.GetClipByName(clipName+direct.ToString())==null)
					direct = Direct.LEFT;
			}
			clipName += direct.ToString();
			this.currentClipName = clipName;
			if (!characterInfo.anim.IsPlaying(clipName))
			{
				characterInfo.anim.Play(clipName);
			}
		}
		if (characterInfo.path == null || (characterInfo.path.Count == 0 && Mathf.Approximately((characterInfo.Position-characterInfo.Dest).magnitude,0f)) )
		{
			characterInfo.wstate = WorkerSTATE.STANDING;
		} 
		else 
		{
			Vector3 goal = Vector3.zero;
			if(characterInfo.path.Count > 0)
			{
				PathFinderNode node = characterInfo.path[characterInfo.pathIndex];
				GridInfo grid = Globals.GridArray[node.X,node.Y];
				goal = grid.standPoint;//
			}
			else
			{
				goal = characterInfo.Dest;
				//Debug.Log("goal"+goal);
			}
			caclDirect(goal);
            //移动角色的途中施加了一定的差值，为了不让移动那么生硬
			characterInfo.Position = Vector3.MoveTowards(characterInfo.Position,goal,characterInfo.speed*Globals.TimeRatio);

            /*
			GridInfo standGrid = Globals.LocateGridInfo(characterInfo.Position);
			if(lastGrid==null||lastGrid!=standGrid)
			{
				//还原原经过的遮挡建筑透明度;
				if(lastGrid!=null)
				{
					for(int i=0;i<lastGrid.buildMarks.Count;i++)
					{
						BuildTweener tw = lastGrid.buildMarks[i].buildInfo.GetComponentInChildren<BuildTweener>();
						if(lastGrid.buildMarks[i].buildInfo.behindCharacters.Contains(characterInfo))
							lastGrid.buildMarks[i].buildInfo.behindCharacters.Remove(characterInfo);
						

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
					
					if(!lastGrid.buildMarks[i].buildInfo.behindCharacters.Contains(characterInfo))
						lastGrid.buildMarks[i].buildInfo.behindCharacters.Add(characterInfo);
					
					if(tw!=null)
					{
						tw.enabled = true;
						tw.Alpha =  Globals.ObstacleAlpha;
					}
				}
				
			}
			*/

            if (Mathf.Approximately((characterInfo.Position - goal).magnitude, 0f)) {

                //一个节点一个节点的使用
                if (characterInfo.path.Count > characterInfo.pathIndex + 1) {
                    characterInfo.pathIndex++;
                   // node = characterInfo.path.Dequeue();
                    //Debug.Log("RemoveAt");
                }
                else
                    characterInfo.wstate = WorkerSTATE.STANDING;
			}
		}
	}






    /// <summary>
    /// 计算worker的坐标A与目标点的坐标B的连线向量AB相对于世界坐标系的方位，将世界坐标系的x,z轴形成的平面分成八个方向，
    /// 计算AB相对于该坐标系的方位
    /// </summary>
    /// <param name="goal"></param>
    void caclDirect(Vector3 goal)
    {
        float degree = Globals.CaclDegree(new Vector2(characterInfo.Position.x, characterInfo.Position.z), new Vector2(goal.x, goal.z));
        characterInfo.Degree = degree;
        characterInfo.NextPoint = goal;

        Vector2 offset = new Vector2(goal.x, goal.z) - new Vector2(characterInfo.Position.x, characterInfo.Position.z);

        characterInfo.RealDegree = Mathf.Atan(offset.y / offset.x) * Mathf.Rad2Deg;

        if ((degree >= 0 && degree < 22.5) || (degree <= 360f && degree >= (360 - 22.5)))
        {
            characterInfo.direction = Direct.RIGHT;
        }
        else if (degree >= 22.5 && degree < (45 + 22.5))
        {
            characterInfo.direction = Direct.RIGHTUP;
        }
        else if (degree >= (45 + 22.5) && degree < (90 + 22.5))
        {
            characterInfo.direction = Direct.UP;
        }
        else if (degree >= (90 + 22.5) && degree < (135 + 22.5))
        {
            characterInfo.direction = Direct.LEFTUP;
        }
        else if (degree >= (135 + 22.5) && degree < (180 + 22.5))
        {
            characterInfo.direction = Direct.LEFT;
        }
        else if (degree >= (180 + 22.5) && degree < (225 + 22.5))
        {
            characterInfo.direction = Direct.LEFTDOWN;
        }
        else if (degree >= (225 + 22.5) && degree < (270 + 22.5))
        {
            characterInfo.direction = Direct.DOWN;
        }
        else if (degree >= (270 + 22.5) && degree < (315 + 22.5))
        {
            characterInfo.direction = Direct.RIGHTDOWN;
        }

        //Debug.Log("caclDirect : " + characterInfo.direction);
        //CalculateDirect2(goal);
    }
    

    public enum CustomDirection
    {
        UnKnow = -1,
        RightUp,
        Up,
        LeftUp,
        Left,
        LeftDown,
        Down,
        RightDown,
        Right,
        Count,
    }



    void CalculateDirect2(Vector3 goal)
    {

        Vector2 v0 = new Vector2(characterInfo.Position.x,characterInfo.Position.z);

        Vector2 v1 = new Vector2(goal.x,goal.z);


        Vector2 offset = v1 - v0;

        //计算类似于Y轴欧拉角，此角以z轴正向为极轴，顺时针方向为正，
        float angle = Mathf.Atan2(offset.x,offset.y) * Mathf.Rad2Deg;

        angle = 90 - (-angle);

        if (angle < 0)
        {
            angle = angle + 360;
        }

        if (angle > 360)
        {
            angle = angle - 360;
        }


        //if (angle) { }


        int dirInt =Mathf.CeilToInt((angle - 22.5f) / 45);

        CustomDirection dir = (CustomDirection)dirInt;


        Debug.Log("CalculateDirect2 : " + dir.ToString());
        
    }




}
