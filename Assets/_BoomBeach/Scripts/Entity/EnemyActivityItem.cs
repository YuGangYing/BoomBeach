using UnityEngine;
using System.Collections;

public class EnemyActivityItem:BaseEntity {
	public int id;//,
	public string u_name;// '攻击者用户名',
	public int u_level;// '攻击者等级',
	public int reward_count;// '得获的奖杯',
	public long attack_start_time;// '0' COMMENT '被开始攻击时间基数System.DateTime.Now.ToString("yyyyMMddHHmmssff"唯一值',
	public int has_replay;// '0:无回放;1:有',
	public int loot_gold;//
	public int loot_wood;//
	public int loot_stone;//
	public int loot_iron;//
	public int reward_diamond;// '奖励宝石',
	public int already_claimed;// '0:未领取;1:已经领取；reward_diamond > 0时有效',
	public int regions_id;// '0:基地；> 0小岛',
	public int regions_type;// '岛类型;0:无;1:主岛屿;2:资源岛(木);3:资源岛(石);4:资源岛(钢);5:自由岛',

	//世界地图中的被攻击列表明细数据;EnemyActivityDetail
	public Hashtable TroopsList = new Hashtable();
}
