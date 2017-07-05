using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class TestAStarController : MonoBehaviour {

	public const string scaneStateName = "scane";
	public const string moveStateName = "move";//path
	public const string attackStateName = "attack";
	public const string idleStateName = "idle";

	public Transform unitParent;
	public Transform unitParent1;

	//public List<Transform> units;
	//public List<Transform> units1;

	public static TestAStarController Instance(){
		return instance;
	}
	static TestAStarController instance;

	public Queue<TestAStarAI> researchQueue = new Queue<TestAStarAI>();
	public bool researchAble = true;
	public const int targetFrameRate = 30;

	public GameObject zookaPrefab;
	public GameObject heavyPrefab;


	void Awake(){
		if (!Application.isPlaying)
			return;
		Application.targetFrameRate = targetFrameRate;
		instance = this;
		playerUnits = new List<Transform> ();
		//enemyUnits = new List<Transform> ();
		SpawnZooka ();
		SpawnHeavy ();
		/*
		units = new List<Transform> ();
		for(int i=0;i<unitParent.childCount;i++){
			units.Add (unitParent.GetChild(i));
		}
		units1 = new List<Transform> ();
		for(int i=0;i<unitParent1.childCount;i++){
			units1.Add (unitParent1.GetChild(i));
		}
		//InitUnits (units,units1);
		InitUnits (units1, units);
		*/
	}

	List<Transform> playerUnits;
	public List<Transform> enemyUnits;
	void SpawnZooka(){
		zookaPrefab = Resources.Load<GameObject> ("Model/Character/zooka");
		GameObject unitPrefab = Resources.Load<GameObject> ("Unit");
		for(int i=0;i<20;i++){
			GameObject unitGo = Instantiate (unitPrefab) as GameObject;
			unitGo.transform.position =  new Vector3 (-40,0,(i - 10) *0.5f);
			GameObject zookaGo = Instantiate (zookaPrefab) as GameObject;
			zookaGo.transform.parent = unitGo.transform;
			zookaGo.transform.localPosition = Vector3.zero;
			zookaGo.transform.localEulerAngles = Vector3.zero;
			InitUnit (unitGo.GetComponent<TestAStarAI>(),12,3,enemyUnits,playerUnits);
			unitGo.transform.parent = unitParent;
			playerUnits.Add (unitGo.transform);
		}
	}

	void SpawnHeavy(){
		heavyPrefab = Resources.Load<GameObject> ("Model/Character/heavys");
		GameObject unitPrefab = Resources.Load<GameObject> ("Unit");
		for(int i=0;i<10;i++){
			GameObject unitGo = Instantiate (unitPrefab) as GameObject;
			unitGo.transform.position = new Vector3 (-35,0,(i - 5) *2);
			GameObject heavyGo = Instantiate (heavyPrefab) as GameObject;
			heavyGo.transform.parent = unitGo.transform;
			heavyGo.transform.localPosition = Vector3.zero;
			heavyGo.transform.localEulerAngles = Vector3.zero;
			InitUnit (unitGo.GetComponent<TestAStarAI>(),6,4,enemyUnits,playerUnits);
			unitGo.transform.parent = unitParent;
			playerUnits.Add (unitGo.transform);
		}
	}

	/*
	void InitUnits(List<Transform> targetUnits,List<Transform> friendUnits){
		for(int i=0;i<friendUnits.Count;i++){
			InitUnit (friendUnits[i].GetComponent<TestAStarAI>(),targetUnits,friendUnits);
		}
	}
*/

	void InitUnit(TestAStarAI unit,float fireDist,float speed,List<Transform> targetUnits,List<Transform> friendUnits){
		FiniteStateMent fsm = unit.GetComponent<FiniteStateMent> ();
		if(fsm==null)
			fsm = unit.gameObject.AddComponent<FiniteStateMent> ();
		BaseState scanState = fsm.AddState<BaseState> (scaneStateName);
		ScanAction scanAction = scanState.AddAction<ScanAction> (ScanAction.actionName);
		scanAction.scanInterval = 4;
		scanAction.scanTargets = targetUnits;

		BaseState moveState =  fsm.AddState<BaseState> (moveStateName);
		MoveAction moveAction = moveState.AddAction<MoveAction> (MoveAction.actionName);
		moveAction.fireDist = fireDist;
		moveAction.speed = speed;

		//BaseState attackState = fsm.AddState<BaseState> (attackStateName);
		//AttackAction attackAction = attackState.AddAction<AttackAction> (AttackAction.actionName);

		//BaseState idleState = fsm.AddState<BaseState> (idleStateName);
		//IdleAction idleAction = idleState.AddAction<IdleAction> (IdleAction.actionName);

		fsm.currentStateName = scaneStateName;
	}

}
