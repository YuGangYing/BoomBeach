using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FiniteStateMent : MonoBehaviour {

	public Dictionary<string,BaseState> stateDic;
	public List<BaseState> states;

	public string preStateName;
	public string lastFrameStateName;
	public string currentStateName;

	public BaseState preState;
	public BaseState currentState;

	void Awake(){
		stateDic = new Dictionary<string, BaseState> ();
		states = new List<BaseState> ();
	}

	void Update(){
		if(lastFrameStateName != currentStateName){
			if (currentState != null) {
				currentState.OnExit ();
				preState = currentState;
				preStateName = lastFrameStateName;
			}
			lastFrameStateName = currentStateName;
			currentState = stateDic [currentStateName];
			currentState.OnEnter ();
		}
		if (currentState != null) {
			currentState.OnUpdate ();
		}
	}

	public T AddState<T>(string stateName) where T : BaseState,new() {
		T t = new T();
		t.mSelf = gameObject;
		t.mStateName = stateName;;
		t.Fsm = this;
		stateDic.Add (stateName, t);
		states.Add (t);
		t.OnAwake ();
		return t;
	}

	/*
	void Update(){
		if (Input.GetKeyDown (KeyCode.H)) {
			BaseState bs = AddState<BaseState> ("baseState");
			Debug.Log (bs);
		}
	}
	*/
}
