using UnityEngine;
using System.Collections;
//ur.res_tid == "TID_BUILDING_STONE_QUARRY" 石头
//ur.res_tid == "TID_BUILDING_WOODCUTTER" 木材
//ur.res_tid == "TID_BUILDING_METAL_MINE" 铁矿
public class UserRegions:BaseEntity {

	public int id;
	public int user_id;//'用户名',
	public int regions_id;//'已开启的地图',
	public string regions_name;//所在地图区域名称;
//	public int regions_level;//'当前岛屿级别',
	public int capture_id;//'占领用户id(=0,还未被占领;=user_id被user_id用户占领；否则被其它用户占领',
	public string capture_name;//'占领用户名',
	public int capture_level;//'占领者级别',
	public int capture_regions_id;//'占领者对应的regions_id，如果为0的话，则为：主基地',
	public int capture_time;//'岛屿最后归属时间',
    public int is_searched;//是否侦察过0否，1是
    public int resource_perhour;//每小时产量
    public int gold;
    public int wood;
    public int stone;
    public int iron;
	public string res_tid;//'资源岛的产出类型tid（只有一个产生tid）',
	public int res_level;//'资源等级',
	public int last_collect_time;//'最后采集时间;
	public int is_npc;//0:玩家;1:电脑;9:宝箱;
	public int is_collect = 0;//is_npc = 9时，有效;0:未采集;1:已采集;
	public WorldHouse worldHouse;
	public bool sending=false;//true正在网络通信,不能重复发送;
}
