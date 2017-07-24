using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BaseState  {

	public GameObject mSelf;
	public FiniteStateMent Fsm;
	public string mStateName;
	public Dictionary<string,BaseAction> actionDic;
	public List<BaseAction> actions;

	public BaseState(){}

	public T AddAction<T>(string actionName) where T : BaseAction,new() {
		T t = new T();
		t.mSelf = mSelf;
		t.Fsm = Fsm;
		actions.Add (t);
		actionDic.Add (actionName,t);
		t.OnAwake ();
		return t;
	}

	public virtual void OnAwake(){
		actions = new List<BaseAction> ();
		actionDic = new Dictionary<string, BaseAction> ();
		for(int i=0;i<actions.Count;i++){
			actions [i].OnAwake ();
		}
	}

	public virtual void OnEnter(){
		for(int i=0;i<actions.Count;i++){
			actions [i].OnEnter ();
		}
	}

	public virtual void OnUpdate(){
		for(int i=0;i<actions.Count;i++){
			actions [i].OnUpdate ();
		}
	}

	public virtual void OnExit(){
		for(int i=0;i<actions.Count;i++){
			actions [i].OnExit ();
		}
	}

}
