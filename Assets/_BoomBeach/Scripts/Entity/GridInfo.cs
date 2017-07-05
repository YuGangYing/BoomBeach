using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridInfo {

	/* 单元格底点坐标 */
	public Vector3 GridPosition;
	/* 单元格A值 */
	public int A;
	/* 单元格B值 */
	public int B;
	//是否被建筑;
	public bool isBuild;

	//是否在建筑区内;
	public bool isInArea; 

	public BuildInfo buildInfo; //该格子上的建筑物;
	public int trooperCount;//格子上的士兵数量

	private int p_cost;
	public int cost{
		get{ return p_cost;}
		set{ 
			p_cost = value;
			Globals.mMatrix[A,B] = (byte)p_cost;
		}
	}  //A*权重，空地为0,非空地为1,待定;

	public Vector3 standPoint;//站立地点

	public List<BuildMark> buildMarks = new List<BuildMark>();
	
	/*
	public bool isBuild;
	public string buildingKey = "";  //唯一的建筑标识ID，用于选取游戏对象,即名称;
	public BuildingInfo buildInfo = null;
	public string buildInfoTid = "";
	public string buildInfoTidLevel = "";
	
	
	public string leftdownKey;
	public string rightdownKey;
	public string leftupKey;
	public string rightupKey;
	
	public MiniGridInfo leftdown;
	public MiniGridInfo leftup;
	public MiniGridInfo rightdown;
	public MiniGridInfo rightup;
	
	public int cost;  //权重;
	public bool IsObstacle; //是否可通行;
	public bool IsAllowTroop;  //是否可在该格出兵;
	*/
	
}
