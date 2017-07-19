using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingInfo : BaseEntity{
	public int id;
	public int user_id;
	public string build_type;
	public int level;
	public int status;
	public int start_time;
	public int end_time;
	public int pos_x;
	public int pos_y;
}
