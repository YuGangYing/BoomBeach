using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BoomBeach;


public enum WorkerCMD
{
	NORMAL,//普通寻路状态
    WORK//工作状态
}

public enum WorkerSTATE
{
	STANDING,//站立状态
    WALKING//行进状态
}

public class WorkerManager : SingleMonoBehaviour<WorkerManager> {


	public bool isIniting = false;
	public bool isInit = false;

	private List<WorkerInfo> workers;

	public int workerCount = 15;

	GameObject workerPrefab;

	protected override void Awake(){
		base.Awake ();
	}

	public void Init(int workerCount){
		this.workerCount = workerCount;
		init ();
		SetWorkBuilding (Helper.getWorkBuilding ());
	}

	public void init()
	{
		if (workerPrefab == null)
			workerPrefab = Resources.Load ("Model/Character/worker") as GameObject;//加载工人资源预设
		if(workers==null)
		workers = new List<WorkerInfo> ();
        isIniting = true;//正在初始化为真
		isInit = false;//初始化完成为假
	}

	void createWorker()
	{
		if(workers!=null&&workers.Count<workerCount&&DataManager.GetInstance.buildArray.Count>0)
		{
			GameObject worker = Instantiate(workerPrefab) as GameObject;
			worker.transform.parent = transform;
			workers.Add(worker.GetComponent<WorkerInfo>());
			worker.transform.name = "worker"+workers.Count;

			if(workBuildInfo==null)
			{
				int idx = Random.Range (0, DataManager.GetInstance.buildArray.Count);		//针对每个士兵随机选择一个建筑	
				BuildInfo b = DataManager.GetInstance.buildArray [idx];			
				worker.transform.position = Globals.GetRandStandPointAroundBuild (b,worker.transform);
			}
			else
			worker.transform.position = Globals.GetRandBuildStandPointAroundBuild (workBuildInfo);
		}
		else
		{
			isIniting = false;
			isInit = true;
		}
	}

    /// <summary>
    /// 销毁Workers
    /// </summary>
	public void ClearWorkers()
	{
		if(workers!=null)
		foreach(WorkerInfo worker in workers)
		{
			DestroyImmediate(worker.gameObject);
		}
		workers = null;
		isInit = false;
		isIniting = false;
	}

	void Update()
	{
		if(isIniting)
		createWorker ();//分步创建Worker，可以用开辟协同任务代替
	}

	private BuildInfo workBuildInfo;
	public BuildInfo WorkBuildInfo{
		get{ return workBuildInfo; }
		set{ workBuildInfo = value;}
	}

	public bool IsWorking{
		get{ return workBuildInfo!=null; }
	}

	public void SetWorkBuilding(BuildInfo buildInfo)
	{
		workBuildInfo = buildInfo;
		if(workers!=null)
		foreach(WorkerInfo worker in workers)
		{
			worker.workingBuild = buildInfo;
			if(workBuildInfo!=null)
				worker.wcmd = WorkerCMD.WORK;
			else
				worker.wcmd = WorkerCMD.NORMAL;
		}
		ReAction ();
	}

	//所有工作立即执行新动作;
	public void ReAction()
	{
		if(workers!=null)
		foreach(WorkerInfo worker in workers)
		{
			worker.Think();
		}
	}
}
