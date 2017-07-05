using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum BuildButtonType{
	INFO,
	UPGRADE,
	COLLECT,
	RESEARCH,
	BUILD,
	REMOVE,
	TRAIN
}

public class BuildButton  {
	public BuildInfo buildInfo;	//绑定对象;
	public BuildButtonType Type;  //按钮类型;
	public string IcoName;		  //图标名称;
	//public List<EventDelegate> OnClick;  //点击事件;
	public bool Status;			  //按钮状态;
}
