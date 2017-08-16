using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class TestAStarAI : MonoBehaviour {

	//FiniteStateMent mFSM;
	//Seeker mSeeker;
	public int hp = 100;

	// Use this for initialization
	void Awake(){
		//mSeeker = GetComponent<Seeker>();
		//mFSM = GetComponent<FiniteStateMent> ();
		hp = 1000;
	}

	/*
	List<GraphNode> nodes;
	Vector3 zeroIntersect;
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			zeroIntersect = ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);
			//moveAction.Reset();
			TestAStarController.Instance ().researchQueue.Enqueue (this);
			if (blockNode != null) {
				blockNode.Penalty -= penalty;
				if (blockNode.Penalty >= 1000000)
					blockNode.Penalty -= 1000000;
				blockNode = null;
			}
				
		}
		//moveAction.OnUpdate ();
	}

	Path mPath = null;
	public void Repath(){
		mPath = mSeeker.StartPath (transform.position, zeroIntersect,OnPathComplete);
		moveAction.targetPos = zeroIntersect;
		AstarPath.active.ReturnPaths (true);
	}

	public void OnPathComplete (Path p)
	{
		TestAStarController.Instance ().researchAble = true;
		//Debug.Log (p.vectorPath.Count);
		moveAction.path = p;
		moveAction.canMove = true;
		GameObject ob = new GameObject("LineRenderer", typeof(LineRenderer));
		LineRenderer lr = ob.GetComponent<LineRenderer>();
		//lr.sharedMaterial = lineMat;
		lr.SetWidth(0.2f, 0.2f);
		lr.SetVertexCount(p.vectorPath.Count);
		for (int i = 0; i < p.vectorPath.Count; i++) {
			lr.SetPosition(i, p.vectorPath[i]);
		}
		GridGraph gg = (GridGraph)AstarPath.active.graphs [0];
		//求路径与目标圆的交点。
		//后面在寻路的单位经过这点的消耗就变多了。
		for (int i = 0; i < p.vectorPath.Count; i++) {
			if (i + 1 >= p.vectorPath.Count) {
				break;
			}
			Vector3 startPos = p.vectorPath [i];
			Vector3 endPos = p.vectorPath [i + 1];
			Vector2 intersection;
			if (ComplexMathf.CircleLineInstersect (new Vector2 (startPos.x, startPos.z),new Vector2 (endPos.x, endPos.z), 
				new Vector2 (zeroIntersect.x, zeroIntersect.z), moveAction.fireDist, out intersection)) {
				Vector3 pos = new Vector3 (intersection.x, 0, intersection.y);
				NNInfo info = gg.GetNearest (pos);
				info.node.Penalty += penalty;
				if (info.node.Penalty >= penalty * 2) {
					info.node.Penalty += 1000000;
					//info.node.Walkable = false;
				}
				blockNode = info.node;
				//if (info.node.Penalty > 200)
				//	info.node.Walkable = false;
				break;
			}
		}
	}

	void SetPenalty(){
		GridGraph gg=  (GridGraph)AstarPath.active.graphs[0];
		Debug.Log (gg.nodes.Length);
		foreach(GridNode node in gg.nodes){
			if(Mathf.Abs(node.position.x / 1000) <= 20  && Mathf.Abs(node.position.z / 1000) < 5)
			{
				node.Penalty = 3;
			}
		}
	}
	*/

}
