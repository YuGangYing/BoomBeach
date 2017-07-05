
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Sfs2X.Entities.Data;

public static class TempleData {

	//1.获取敌人数据
	public static ISFSObject GetBattleData(){
		ISFSObject dt = new SFSObject ();
		dt.PutSFSObject ("userResource",GetUserObject());
		dt.PutSFSArray ("troops_list",GetBattleTroops());
		dt.PutSFSArray ("troops_level",GetBattleTroopLevel());
        dt.PutSFSArray("user_buildings", GetUserBuildings());
		GetBattleReward (dt);
        return dt;
	}

	//2.获取玩家数据
	public static ISFSObject GetPlayerData(){
		ISFSObject dt = new SFSObject ();
		dt.PutSFSArray ("upgrade_list",GetTempleUpgradeArray());
		ISFSObject user = GetUserObject ();
		user.PutSFSArray ("user_buildings", GetUserBuildings ());
		dt.PutSFSObject ("userResource",user);
		dt.PutSFSArray ("troops_level",GetTroopsLevel());
		dt.PutSFSArray ("user_regions",GetUserRegins());
		//dt.PutSFSArray ("user_buildings", GetUserBuildings ());
		dt.PutInt ("regions_id",0);
		return dt;
	}

	static void GetBattleReward(ISFSObject dt){
		dt.PutInt ("loot_gold",1000);
		dt.PutInt("loot_wood",2000);
		dt.PutInt("loot_stone",3000);
		dt.PutInt ("loot_iron",4000);
		dt.PutInt ("add_reward",1);
		dt.PutInt("loot_boot",200);
		dt.PutInt ("artifact",1);
		dt.PutInt("pal_reward",2);
	}

	//获取小兵数量
	static ISFSArray GetBattleTroops(){
		ISFSArray troops_list = new SFSArray ();
		ISFSObject obj = new SFSObject ();
		obj.PutUtfString("tid","TID_BUILDING_LANDING_SHIP");
		obj.PutInt("level",10);
		obj.PutUtfString("troops_tid", "TID_RIFLEMAN");
		obj.PutInt("troops_num",10);
		obj.PutLong("building_id",1010);
		troops_list.AddSFSObject (obj);
		obj = new SFSObject ();
		obj.PutUtfString("tid","TID_BUILDING_LANDING_SHIP");
		obj.PutInt("level",10);
		obj.PutUtfString("troops_tid","TID_HEAVY");
		obj.PutInt("troops_num",10);
		obj.PutLong("building_id",1011);
		troops_list.AddSFSObject (obj);
        obj = new SFSObject();
        obj.PutUtfString("tid", "TID_BUILDING_LANDING_SHIP");
        obj.PutInt("level", 4);
        obj.PutUtfString("troops_tid", "TID_HEAVY");
        obj.PutInt("troops_num", 4);
        obj.PutLong("building_id", 1012);
        troops_list.AddSFSObject(obj);
        obj = new SFSObject();
        obj.PutUtfString("tid", "TID_BUILDING_LANDING_SHIP");
        obj.PutInt("level", 10);
        obj.PutUtfString("troops_tid", "TID_ZOOKA");
        obj.PutInt("troops_num", 10);
        obj.PutLong("building_id", 1013);
        troops_list.AddSFSObject(obj);
        obj = new SFSObject();
        obj.PutUtfString("tid", "TID_BUILDING_LANDING_SHIP");
        obj.PutInt("level", 10);
        obj.PutUtfString("troops_tid", "TID_WARRIOR");
        obj.PutInt("troops_num", 10);
        obj.PutLong("building_id", 1014);
        troops_list.AddSFSObject(obj);
        obj = new SFSObject();
        obj.PutUtfString("tid", "TID_BUILDING_LANDING_SHIP");
        obj.PutInt("level", 10);
        obj.PutUtfString("troops_tid", "TID_TANK");
        obj.PutInt("troops_num", 10);
        obj.PutLong("building_id", 1015);
        troops_list.AddSFSObject(obj);
        return troops_list;
	}

	//获取小兵等级
	static ISFSArray GetBattleTroopLevel(){
		ISFSArray troops_level = new SFSArray ();
		ISFSObject obj = new SFSObject ();
		obj.PutUtfString("tid","TID_TANK");
		obj.PutInt("level",2);
		troops_level.AddSFSObject (obj);
		return troops_level;
	}

	//用户的建筑
	static ISFSArray GetUserBuildings(){
		ISFSArray buildings = new SFSArray ();
		ISFSObject item = new SFSObject ();
        int bid = 1001;
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 20);
		item.PutUtfString ("tid", "TID_BUILDING_PALACE");
		item.PutInt ("x", 20);
		item.PutInt ("y", 20);
		buildings.AddSFSObject (item);//基地
        bid++;
        item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 1);
		item.PutUtfString ("tid", "TID_BUILDING_HOUSING");
		item.PutInt ("status",1);
		item.PutInt ("x", 20);
		item.PutInt ("y", 25);
		buildings.AddSFSObject (item);//民居
        bid++;
        item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 1);
		item.PutInt ("status",2);
		item.PutUtfString ("tid", "TID_BUILDING_HOUSING");
		item.PutInt ("start_time",Helper.current_time());
		item.PutInt ("end_time",Helper.current_time() + 60);
		item.PutInt ("last_collect_time",Helper.current_time());
		item.PutInt ("x", 20);
		item.PutInt ("y", 15);
		buildings.AddSFSObject (item);//民居
        bid++;
        //Laboratory
        item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 20);
		item.PutUtfString ("tid", "TID_BUILDING_LABORATORY");
		item.PutInt ("x", 20);
		item.PutInt ("y", 10);
		buildings.AddSFSObject (item);//图书馆
        bid++;
        item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 1);
		item.PutUtfString ("tid", "TID_BUILDING_WOODCUTTER");
		item.PutInt ("x", 26);
		item.PutInt ("y", 20);
		buildings.AddSFSObject (item);//伐木场
        bid++;
        item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 1);
		item.PutUtfString ("tid", "TID_BUILDING_GUNSHIP");
		buildings.AddSFSObject (item);//炮舰
		bid++;
		item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 15);
		item.PutUtfString ("tid", "TID_MACHINE_GUN_NEST");
		item.PutInt ("x", 5);
		item.PutInt ("y", 20);
		buildings.AddSFSObject (item);

		bid++;
		item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 15);
		item.PutUtfString ("tid", "TID_MACHINE_GUN_NEST");
		item.PutInt ("x", 5);
		item.PutInt ("y", 16);
		buildings.AddSFSObject (item);
		bid++;
		item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 15);
		item.PutUtfString ("tid", "TID_MACHINE_GUN_NEST");
		item.PutInt ("x", 5);
		item.PutInt ("y", 24);
		buildings.AddSFSObject (item);
		bid++;
		item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 15);
		item.PutUtfString ("tid", "TID_MACHINE_GUN_NEST");
		item.PutInt ("x", 5);
		item.PutInt ("y", 8);
		buildings.AddSFSObject (item);
		bid++;
		item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 15);
		item.PutUtfString ("tid", "TID_MACHINE_GUN_NEST");
		item.PutInt ("x", 5);
		item.PutInt ("y", 12);
		buildings.AddSFSObject (item);
		bid++;
		item = new SFSObject ();
		item.PutInt ("building_id", bid);
		item.PutInt ("level", 15);
		item.PutUtfString ("tid", "TID_MACHINE_GUN_NEST");
		item.PutInt ("x", 5);
		item.PutInt ("y", 28);
		buildings.AddSFSObject (item);
		return buildings;
	}

	//用户已经展开的地区
	static ISFSArray GetUserRegins(){
		ISFSArray regions = new SFSArray ();
		SFSObject reginObj = new SFSObject ();
		reginObj.PutInt ("id",0);
		reginObj.PutInt ("regions_id", 1);//1是玩家自己的基地
		reginObj.PutUtfString("regions_name","r0");//r0是玩家自己的基地上面的云层
		regions.AddSFSObject(reginObj);//初始化必须有的，玩家自己的基地

		reginObj = new SFSObject ();//对应的regions.txt文件
		reginObj.PutInt ("id",1);
		reginObj.PutInt ("regions_id", 2);
		reginObj.PutUtfString("regions_name","r1");
		reginObj.PutInt ("capture_id",1001);
		reginObj.PutUtfString("capture_name","喂人民服雾");
		reginObj.PutInt ("capture_level",10);
		reginObj.PutInt ("capture_time",(int)Time.time);
		reginObj.PutInt ("capture_regions_id",0);
		regions.AddSFSObject(reginObj);//其他玩家的基地

		return regions;
	}

	//科技的等级
	static ISFSArray GetTroopsLevel(){
		ISFSArray troopsLevelList = new SFSArray();
		ISFSObject item = new SFSObject ();
		item.PutUtfString ("tid","TID_RIFLEMAN");
		item.PutInt ("level",3);
		troopsLevelList.AddSFSObject (item);
		return troopsLevelList;
	}

	//用户的基本数据
	static ISFSObject GetUserObject(){
		ISFSObject user = new SFSObject ();
		user.PutInt ("island_type",0);
		user.PutInt ("exp_level",1);
		user.PutInt ("exp_count",5);
		user.PutInt ("gold_count",10);
		user.PutInt ("wood_count",2000);
		user.PutInt ("stone_count",0);
		user.PutInt ("iron_count",0);
		user.PutInt ("max_gold_count",2000);
		user.PutInt ("max_wood_count",2000);
		user.PutInt ("max_stone_count",2000);
		user.PutInt ("max_iron_count",2000);
		user.PutInt ("town_hall_level",20);
		user.PutInt ("diamond_count",99999);
		user.PutInt ("worker_count",1);
		user.PutInt ("max_iron_count",2000);
		user.PutInt ("common_piece",99);
		user.PutInt ("common_piece_dark",99);
		user.PutInt ("common_piece_fire",99);
		user.PutInt ("common_piece_ice",99);
		user.PutInt ("rare_piece",99);
		user.PutInt ("rare_piece_dark",99);
		user.PutInt ("rare_piece_fire",99);
		user.PutInt ("rare_piece_ice",99);
		user.PutInt ("epic_piece",99);
		user.PutInt ("epic_piece_dark",99);
		user.PutInt ("epic_piece_fire",99);
		user.PutInt ("epic_piece_ice",99);
        user.PutInt("gunship_level",1);
		user.PutInt ("laboratory_level",20);
		user.PutInt ("gunship_level",20);
        user.PutUtfString("user_name","张三");
		return user;
	}

	//升级完成的消息列表，仅用于加载home场景的建造完成提示，包括建筑升级和科技升级
	static ISFSArray GetTempleUpgradeArray(){
		ISFSArray upgradeList = new SFSArray();
		ISFSObject item = new SFSObject ();
		item.PutInt ("is_new",1);
		item.PutUtfString ("tid","TID_BUILDING_HOUSING");
		item.PutInt ("level",2);
		upgradeList.AddSFSObject (item);
		return upgradeList;
	}

}
