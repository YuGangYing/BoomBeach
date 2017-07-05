using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITask : MonoBehaviour {

	private static AITask instance;
	public static AITask Instance{
		get{
			return instance;
		}
	}

	void Awake()
	{
		instance = this;
		FindDestList = new Queue<CharInfo> ();
		FindPathList = new Queue<CharInfo> ();

	}

	public void ResetAll()
	{
		FindDestList = new Queue<CharInfo> ();
		FindPathList = new Queue<CharInfo> ();
	}


	private Queue<CharInfo> FindDestList;

	private Queue<CharInfo> FindPathList;



	// Update is called once per frame

	void Update () {
		//float startTime= Time.realtimeSinceStartup;
		//float useTime1 = 0f;
		//float useTime2 = 0f;

		while(FindDestList.Count>0)
		{
			CharInfo findDestCharInfo = FindDestList.Dequeue();
			if(!findDestCharInfo.IsFindDest)
				findDestCharInfo.trooperCtl.FindDest();
			/*
			float nowTime1 = Time.realtimeSinceStartup;
			useTime1 = nowTime1 - startTime;
			if(useTime1>=Time.deltaTime)
			{
				return;
			}
			*/
		}
		while(FindPathList.Count>0)
		{
			CharInfo findPathCharInfo = FindPathList.Dequeue();
			if(findPathCharInfo.path==null||findPathCharInfo.path.Count==0)
				findPathCharInfo.trooperCtl.FindPath();
			/*
			float nowTime2 = Time.realtimeSinceStartup;
			useTime2 = nowTime2 - startTime;
			if(useTime2>=Time.deltaTime)//这样判断deltaTime有可能会递增，这样做如果是帧运动下的话就不行。
			{
				return;
			}
			*/
		}

	}


	/// <summary>
	/// 士兵寻找目标;
	/// </summary>
	public void FindDest(CharInfo charInfo)
	{
		FindDestList.Enqueue(charInfo);
	}


	/// <summary>
	/// 士兵寻找路径;
	/// </summary>
	public void FindPath(CharInfo charInfo)
	{
		FindPathList.Enqueue(charInfo);//
	}
}
