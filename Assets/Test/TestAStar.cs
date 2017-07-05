using UnityEngine;
using System.Collections;
using Pathfinding;

public class TestAStar : MonoBehaviour {
	public GameObject go;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.H)){
			//AstarPath.active.UpdateGraphs (go.GetComponent<Collider>().bounds);
			GridGraph gg=  (GridGraph)AstarPath.active.graphs[0];
			gg.ErodeWalkableArea (0,0,90,90);
			//AstarPath.active.Scan();
			Debug.Log (gg.nodes.Length);
			foreach(GridNode node in gg.nodes){
				//node.Walkable = false;
				//Debug.Log (node.position);
				if(Mathf.Abs(node.position.x / 1000) == 20  && Mathf.Abs(node.position.z / 1000) < 5)
				{
					node.Penalty = 1;
					node.Walkable = false;
				}
			}
		}
		if(Input.GetMouseButtonDown(0)){
			//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Vector3 zeroIntersect = ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);
			//GridGraph gg=  (GridGraph)AstarPath.active.graphs[0];




//			Debug.Log (gg.Linecast(new Vector3(0,0,0),zeroIntersect));
		}


	}
}
