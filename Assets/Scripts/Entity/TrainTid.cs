using UnityEngine;
using System.Collections;

public class TrainTid {
	public BuildInfo buildInfo;
	public string tid;//;
	public string tid_level;
	public int trainNum; //可训练数量;
	public int trainCost;//训练成本,金币;
	public int trainTime;//训练时间，秒;
	public string trainTimeFormat;//训练时间，已经格式化好的时间;
	public string hasTrain;//可以训练值为：null，非null为，不可升级原因;
	//public bool disable;//当 rTid.tid == s.troops_tid 时，是在更换军队种类,可以是禁用状态;
}
