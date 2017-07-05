using UnityEngine;
using System.Collections;

public class Regions:BaseEntity {
	public string Name;//区域名称: r0至r149

	public int RequiredMapRoomLevel;//需要雷达等级;
	public int ExplorationOrder;//可勘探数量;
	public int ExplorationCost;//开启费用;当status=2时，有效;
	public int ExplorationTimeSeconds;//开启时长;

	public int status = 0;//0:不可开启;1:已开启;2:可开启(但未开启);
	public string desc;//

	public bool sending=false;//true正在网络通信,不能重复发送;
	public Transform cloud;//缓存对就的云块;
	public GameObject send_sprite;//缓存正常在发送的图标;
	public GameObject gold_sprite;
	public GameObject exploration_cost;
	public GameObject Explore;//缓存开启提示;
}
