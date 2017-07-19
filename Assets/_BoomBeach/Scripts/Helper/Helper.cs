using UnityEngine;
using System.Collections;
using System.Reflection;

using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using BoomBeach;


//artifact_type:神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage;
public enum ArtifactType{
	None,
	BoostGold,
	BoostWood,
	BoostStone,
	BoostMetal,
	BoostTroopHP,
	BoostBuildingHP,
	BoostTroopDamage,
	BoostBuildingDamage,
	BoostGunshipEnergy,
	BoostLoot,
	BoostArtifactDrop,
	BoostAllResources
}

public class Helper: MonoBehaviour {
	public static Hashtable nameToTid = new Hashtable();
	public const string TID_BUILDING_PALACE = "TID_BUILDING_PALACE";//基地
	public const string TID_BUILDING_LABORATORY = "TID_BUILDING_LABORATORY";//图书馆
	public const string TID_BUILDING_ARTIFACT_WORKSHOP = "TID_BUILDING_ARTIFACT_WORKSHOP";//神庙
	public const string TID_BUILDING_GUNSHIP = "TID_BUILDING_GUNSHIP";//炮舰

	public const string TID_BUILDING_LANDING_SHIP= "TID_BUILDING_LANDING_SHIP";//登陆艇

	public const string TID_BUILDING_WOODCUTTER = "TID_BUILDING_WOODCUTTER";//伐木场
	public const string TID_BUILDING_WOOD_STORAGE = "TID_BUILDING_WOOD_STORAGE";//木材库
	public const string TID_BUILDING_STONE_QUARRY= "TID_BUILDING_STONE_QUARRY";//采石场
	public const string TID_BUILDING_STONE_STORAGE= "TID_BUILDING_STONE_STORAGE";//石头库
	public const string TID_BUILDING_HOUSING= "TID_BUILDING_HOUSING";//民房
	public const string TID_BUILDING_GOLD_STORAGE= "TID_BUILDING_GOLD_STORAGE";//金库
	public const string TID_BUILDING_METAL_MINE= "TID_BUILDING_METAL_MINE";//铁矿
	public const string TID_BUILDING_METAL_STORAGE = "TID_BUILDING_METAL_STORAGE";//钢材库


	public const string TID_BUILDING_BIG_BERTHA= "TID_BUILDING_BIG_BERTHA";
	public const string TID_GUARD_TOWER= "TID_GUARD_TOWER";
	public const string TID_BUILDING_MORTAR= "TID_BUILDING_MORTAR";
	public const string TID_MACHINE_GUN_NEST= "TID_MACHINE_GUN_NEST";
	public const string TID_BUILDING_CANNON= "TID_BUILDING_CANNON";
	public const string TID_FLAME_THROWER= "TID_FLAME_THROWER";
	public const string TID_MISSILE_LAUNCHER= "TID_MISSILE_LAUNCHER";
	public const string TID_TRAP_TANK_MINE= "TID_TRAP_TANK_MINE";
	public const string TID_TRAP_MINE= "TID_TRAP_MINE";
	public const string TID_BUILDING_MAP_ROOM= "TID_BUILDING_MAP_ROOM";
	public const string TID_BUILDING_VAULT= "TID_BUILDING_VAULT";

	public const string TID_TYPE_CHARACTERS = "CHARACTERS";
	public const string TID_TYPE_SPELLS = "SPELLS";
	public const string TID_TYPE_TRAPS = "TRAPS";


	public const string RES_TYPE_GOLD = "Gold";
	public const string RES_TYPE_WOOD = "Wood";
	public const string RES_TYPE_STONE = "Stone";
	public const string RES_TYPE_IRON = "Iron";
	public const string RES_TYPE_EXP = "";




	//从建筑列表读区全局数据，比如基地等级，科技建筑等级，神庙等级等。
	public static void GetUserInfoFromBuildings(UserInfo userInfo,ISFSArray buildingList){
		for(int i=0;i<buildingList.Count;i++){
			ISFSObject sfsObj = buildingList.GetSFSObject (i);
			string tid = sfsObj.GetUtfString ("tid");
			int level = sfsObj.GetInt ("level");
			string tid_level = tid+ "_" + level; 
			CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
			userInfo.max_gold_count = 0;
			userInfo.max_wood_count = 0;
			userInfo.max_stone_count = 0;
			userInfo.max_iron_count = 0;
			switch(tid){
			case TID_BUILDING_PALACE:
				//Debug.Log (userInfo.town_hall_level);
				userInfo.town_hall_level = level;
				break;
			case TID_BUILDING_LABORATORY:
				userInfo.laboratory_level = level;
				break;
			case TID_BUILDING_GUNSHIP:
				userInfo.gunship_level = level;
				break;
			case TID_BUILDING_VAULT:
				userInfo.vault_level = level;
				break;
			}
			userInfo.max_gold_count += csvData.MaxStoredResourceGold;
			userInfo.max_wood_count += csvData.MaxStoredResourceWood;
			userInfo.max_stone_count += csvData.MaxStoredResourceStone;
			userInfo.max_iron_count += csvData.MaxStoredResourceIron;
		}
	}

	//当s.troops_num大于0时,获得里面要补充的数量;
	//点击：训练时，用到的判断
	//返回null,则说明，登陆艇 中没有：兵; 反之 登陆艇 装有： 兵; 
	//返回非空时，trainNum == 0时，说明已经装满了, trainNum > 0时，还可以补充的数量;
	public static TrainTid GetTrainTid(BuildInfo s){
		if (s.troops_tid != "" && s.troops_tid != null && s.troops_num > 0){
			int max_space = s.csvInfo.HousingSpace;
			int troops_level = Helper.getTidMaxLevel(s.troops_tid,1);
			string troops_tid_level = s.troops_tid + "_" + troops_level;

			TrainTid rTid = new TrainTid();
			
			rTid.buildInfo = s;
			rTid.tid = s.troops_tid;
			rTid.tid_level = troops_tid_level;
			
			CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[rTid.tid_level];
			
			//补充数量 = 当前 登陆艇 最大可以装下的，兵 数量 - 已生产的数量;
			rTid.trainNum = Mathf.FloorToInt(max_space / csvData.HousingSpace) - s.troops_num;

			//已经生产满了,可再生产的数量为：0;
			if (rTid.trainNum < 0)
				rTid.trainNum = 0;

			rTid.trainCost = GetBuildCost(rTid.tid_level).gold * rTid.trainNum;
						
			if (rTid.trainCost < 0)
				rTid.trainCost = 0;
			
			rTid.trainTime = GetBuildTime(rTid.tid_level) * rTid.trainNum;
			rTid.trainTimeFormat = GetFormatTime(rTid.trainTime,0);
			
			rTid.hasTrain = null;//checkNewBuild(rTid.tid);

			return rTid;
		}else{
			return null;
		}
	}

	//兵训练列表;
	public static Dictionary<string,TrainTid> getTrainTidList(BuildInfo s, string troops_tid = null){
		Dictionary<string,TrainTid> TrainTidList = new Dictionary<string,TrainTid>();
		List<string> tid_list = new List<string>();
		if (troops_tid == null){
			tid_list.Add("TID_RIFLEMAN");
			tid_list.Add("TID_HEAVY");
			tid_list.Add("TID_ZOOKA");
			tid_list.Add("TID_WARRIOR");
			tid_list.Add("TID_TANK");
		}else{
			tid_list.Add(troops_tid);
		}
		//s.troops_tid
		//登陆艇 最大容量 s.csvInfo.HousingSpace
		int max_space = s.csvInfo.HousingSpace;
		int troops_cost = 0;
		if (s.troops_tid != "" && s.troops_tid != null && s.troops_num > 0){
			int troops_level = Helper.getTidMaxLevel(s.troops_tid,1);
			string troops_tid_level = s.troops_tid + "_" + troops_level;
			troops_cost = GetBuildCost(troops_tid_level).gold * s.troops_num;
		}
		//CsvInfo csvTroops = (CsvInfo)CSVManager.GetInstance.csvTable[troops_tid_level];
		//已经使用的成本;

		

		for(int i = 0; i < tid_list.Count; i ++){
			TrainTid rTid = new TrainTid();
			
			string tid = tid_list[i];
			int level = Helper.getTidMaxLevel(tid,1);
			string tid_level = tid + "_" + level;
			rTid.buildInfo = s;
			rTid.tid = tid;
			rTid.tid_level = tid_level;

			CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];

			if (rTid.tid == s.troops_tid){
				//rTid.disable = true;
				//补充数量 = 当前 登陆艇 最大可以装下的，兵 数量 - 已生产的数量;
				rTid.trainNum = Mathf.FloorToInt(max_space / csvData.HousingSpace) - s.troops_num;

				rTid.trainCost = GetBuildCost(tid_level).gold * rTid.trainNum;
			}else{
				//rTid.disable = false;
				//当前兵所需要容量 csvData.HousingSpace
				//当前 登陆艇 最大可以装下的，兵 数量;
				rTid.trainNum = Mathf.FloorToInt(max_space / csvData.HousingSpace);
				rTid.trainCost = GetBuildCost(tid_level).gold * rTid.trainNum - troops_cost;
			}

			//负数时，退还差价;
			//if (rTid.trainCost < 0)
			//	rTid.trainCost = 0;

			rTid.trainTime = GetBuildTime(tid_level) * rTid.trainNum;
			rTid.trainTimeFormat = GetFormatTime(rTid.trainTime,0);

			//1、先判断是否有解锁;
			if (!Helper.isUnLock(csvData.TID_Level)){
				//
				rTid.hasTrain = StringFormat.FormatByTid("TID_UPGRADE_TH_TO_UNLOCK",new object[]{csvData.UnlockTownHallLevel});
			}else{
				//	DataManager.GetInstance.model.user_info.laboratory_level
				rTid.hasTrain = null;
			}
			
			TrainTidList.Add(rTid.tid,rTid);
		}
		
		return TrainTidList;
	}

	//兵，兵器，地雷 升级列表;
	public static List<ResearchTid> getResearchTidList(){
		List<ResearchTid> ResearchTidList = new List<ResearchTid>();
		List<string> tid_list = new List<string>();
		tid_list.Add("TID_RIFLEMAN");
		tid_list.Add("TID_HEAVY");
		tid_list.Add("TID_ZOOKA");
		tid_list.Add("TID_WARRIOR");
		tid_list.Add("TID_TANK");

		tid_list.Add("TID_ARTILLERY");
		tid_list.Add("TID_FLARE");
		tid_list.Add("TID_MEDKIT");
		tid_list.Add("TID_STUN");
		tid_list.Add("TID_BARRAGE");
		tid_list.Add("TID_SMOKE_SCREEN");


		tid_list.Add("TID_TRAP_MINE");
		tid_list.Add("TID_TRAP_TANK_MINE");


		for(int i = 0; i < tid_list.Count; i ++){
			ResearchTid rTid = new ResearchTid();

			string tid = tid_list[i];
			int level = Helper.getTidMaxLevel(tid,1);
			string tid_level = tid + "_" + level;
			rTid.tid = tid;
			rTid.tid_level = tid_level;
			rTid.upgradeCost = getUpgradeCost(tid_level).gold;
			rTid.upgradeTime = GetUpgradeTime(tid_level);
			rTid.upgradeTimeFormat = GetFormatTime(rTid.upgradeTime,0);

			rTid.hasUpgrade = CheckHasUpgrade(tid,level);

			ResearchTidList.Add(rTid);
		}

		return ResearchTidList;
	}

	//司令部升级解锁建筑物;
	public static List<UnLockTid> getUpgradeUnLock(){
		List<UnLockTid> UnTidList = new List<UnLockTid>();

		List<string> tid_list = new List<string>();
		tid_list.Add("TID_BUILDING_WOOD_STORAGE");
		tid_list.Add("TID_BUILDING_STONE_STORAGE");
		tid_list.Add("TID_BUILDING_METAL_STORAGE");
		tid_list.Add("TID_BUILDING_HOUSING");
		tid_list.Add("TID_BUILDING_STONE_QUARRY");
		tid_list.Add("TID_BUILDING_METAL_MINE");
		tid_list.Add("TID_BUILDING_BIG_BERTHA");
		tid_list.Add("TID_GUARD_TOWER");
		tid_list.Add("TID_BUILDING_MORTAR");
		tid_list.Add("TID_MACHINE_GUN_NEST");
		tid_list.Add("TID_BUILDING_CANNON");
		tid_list.Add("TID_FLAME_THROWER");
		tid_list.Add("TID_MISSILE_LAUNCHER");
		tid_list.Add("TID_TRAP_TANK_MINE");
		tid_list.Add("TID_TRAP_MINE");
		tid_list.Add("TID_BUILDING_ARTIFACT_WORKSHOP");
		tid_list.Add("TID_BUILDING_MAP_ROOM");
		tid_list.Add("TID_BUILDING_VAULT");
		tid_list.Add("TID_BUILDING_GOLD_STORAGE");
		tid_list.Add("TID_BUILDING_LABORATORY");
		tid_list.Add("TID_BUILDING_LANDING_SHIP");

		for(int i = 0; i < tid_list.Count; i ++){
			string tid = tid_list[i];
			int cur_max = getBuildingMaxCount(tid,DataManager.GetInstance.model.user_info.town_hall_level);
			int new_max = getBuildingMaxCount(tid,DataManager.GetInstance.model.user_info.town_hall_level + 1);
			int dif_num = new_max - cur_max;
			if (dif_num > 0){
				CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid + "_1"];

				UnLockTid untid = new UnLockTid();
				untid.tid = tid;
				untid.showName = StringFormat.FormatByTid(tid);
				untid.Subtitle = StringFormat.FormatByTid(csvData.SubtitleTID);

				 
				if (cur_max == 0){
					untid.value = "new";
					//Debug.Log(tid + " new");
				}else{
					untid.value = "+" + dif_num.ToString();
					//Debug.Log(tid + " +" + dif_num);
				}

				UnTidList.Add(untid);
			}
		}

		return UnTidList;
	}

	//获得指定tid_level的可展示属性列表;
	public static Dictionary<string,PropertyInfoNew> getPropertyList(string tid_level, bool is_upgrade, BuildInfo s){		
		Dictionary<string,PropertyInfoNew> ht = new Dictionary<string,PropertyInfoNew>();
		//获得神像加成值;
		/*
			artifact_type:神像类型;	
			1BoostGold;
			2BoostWood;
			3BoostStone;
			4BoostMetal;
			
			5BoostTroopHP;
			7BoostTroopDamage;

			6BoostBuildingHP;
			8BoostBuildingDamage;
			*/


		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		CsvInfo csvDataNext = csvData;
		if (is_upgrade){
			string tid_level_next = csvData.TID + "_" + (csvData.Level + 1);
			if (CSVManager.GetInstance.csvTable.ContainsKey(tid_level_next))
			 csvDataNext = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level_next];
		}

		if ("BUILDING".Equals(csvData.TID_Type)){
			//血量;
			if (csvData.Hitpoints > 0){
				ht.Add("Health",getPropertyInfo("TID_HITPOINTS","panelico_heart",csvData.Hitpoints.ToString(),(csvDataNext.Hitpoints - csvData.Hitpoints).ToString(),ArtifactType.BoostBuildingHP,is_upgrade));
			}
			//Debug.Log(csvData.TID);
			//Debug.Log(is_upgrade);
			//司令部升级时，只返回一个血量;
			if (is_upgrade && csvData.TID == "TID_BUILDING_PALACE"){
				return ht;
			}

			//生产资源单元;
			if (csvData.TID == "TID_BUILDING_HOUSING"){			
				ht.Add("GoldCapacity",getPropertyInfo("TID_PRODUCTION_CAPACITY","goldIco",csvData.ResourceMax.ToString(),(csvDataNext.ResourceMax - csvData.ResourceMax).ToString(),ArtifactType.None,is_upgrade));
				string showName = StringFormat.FormatByTid("TID_PRODUCTION_RATE",new object[]{StringFormat.FormatByTid("TID_GOLD")});
				ht.Add("GoldPerHour",getPropertyInfo(showName, "goldIco",csvData.ResourcePerHour.ToString(),(csvDataNext.ResourcePerHour - csvData.ResourcePerHour).ToString(),ArtifactType.BoostGold,is_upgrade));
			}else if (csvData.TID == "TID_BUILDING_WOODCUTTER"){			
				ht.Add("WoodCapacity",getPropertyInfo("TID_PRODUCTION_CAPACITY","woodIco",csvData.ResourceMax.ToString(),(csvDataNext.ResourceMax - csvData.ResourceMax).ToString(),ArtifactType.None,is_upgrade));
				string showName = StringFormat.FormatByTid("TID_PRODUCTION_RATE",new object[]{StringFormat.FormatByTid("TID_WOOD")});
				ht.Add("WoodPerHour",getPropertyInfo(showName,"woodIco",csvData.ResourcePerHour.ToString(),(csvDataNext.ResourcePerHour - csvData.ResourcePerHour).ToString(),ArtifactType.BoostWood,is_upgrade));
			}else if (csvData.TID == "TID_BUILDING_STONE_QUARRY"){			
				ht.Add("StoneCapacity",getPropertyInfo("TID_PRODUCTION_CAPACITY","stoneIco",csvData.ResourceMax.ToString(),(csvDataNext.ResourceMax - csvData.ResourceMax).ToString(),ArtifactType.None,is_upgrade));
				string showName = StringFormat.FormatByTid("TID_PRODUCTION_RATE",new object[]{StringFormat.FormatByTid("TID_STONE")});
				ht.Add("StonePerHour",getPropertyInfo(showName,"stoneIco",csvData.ResourcePerHour.ToString(),(csvDataNext.ResourcePerHour - csvData.ResourcePerHour).ToString(),ArtifactType.BoostStone,is_upgrade));
			}else if (csvData.TID == "TID_BUILDING_METAL_MINE"){			
				ht.Add("MetalCapacity",getPropertyInfo("TID_PRODUCTION_CAPACITY","ironIco",csvData.ResourceMax.ToString(),(csvDataNext.ResourceMax - csvData.ResourceMax).ToString(),ArtifactType.None,is_upgrade));
				string showName = StringFormat.FormatByTid("TID_PRODUCTION_RATE",new object[]{StringFormat.FormatByTid("TID_METAL")});
				ht.Add("MetalPerHour",getPropertyInfo(showName,"ironIco",csvData.ResourcePerHour.ToString(),(csvDataNext.ResourcePerHour - csvData.ResourcePerHour).ToString(),ArtifactType.BoostMetal,is_upgrade));
			}

			//sculptor;
			if (csvData.TID == "TID_BUILDING_ARTIFACT_WORKSHOP"){			
				ht.Add("MaxStatues",getPropertyInfo("TID_NUM_ARTIFACTS","panelico_statue",csvData.ArtifactCapacity.ToString(),(csvDataNext.ArtifactCapacity - csvData.ArtifactCapacity).ToString(),ArtifactType.None,is_upgrade));
			}

			//Map Room;
			if (csvData.TID == "TID_BUILDING_MAP_ROOM"){
				int ExplorableRegions = csvData.ExplorableRegions;
				int ExplorableRegionsNext = csvDataNext.ExplorableRegions;
				ht.Add("ExplorableRegions",getPropertyInfo("TID_MAP_RADIUS","panelico_explore",ExplorableRegions.ToString(),(ExplorableRegionsNext - ExplorableRegions).ToString(),ArtifactType.None,is_upgrade));
			}

			//登陆舰;
			if (csvData.TID == "TID_BUILDING_LANDING_SHIP"){
				ht.Add("TroopCapacity",getPropertyInfo("TID_UNIT_STORAGE_CAPACITY","panelico_unit",csvData.HousingSpace.ToString(),(csvDataNext.HousingSpace - csvData.HousingSpace).ToString(),ArtifactType.None,is_upgrade));
			}

			//
			if (csvData.TID == "TID_BUILDING_GUNSHIP"){
				//energy 名称及图标不对;
				ht.Add("Ammo",getPropertyInfo("TID_STARTING_ENERGY","energy",csvData.StartingEnergy.ToString(),(csvDataNext.StartingEnergy - csvData.StartingEnergy).ToString(),ArtifactType.BoostGunshipEnergy,is_upgrade));
			}

			//资源保护%;
			if (csvData.TID == "TID_BUILDING_VAULT"){
				ht.Add("ResourceProtectionPercent",getPropertyInfo("TID_PROTECTION_PERCENTAGE",null,csvData.ResourceProtectionPercent.ToString(),(csvDataNext.ResourceProtectionPercent - csvData.ResourceProtectionPercent).ToString(),ArtifactType.None,is_upgrade));
			}

			//存储容量;
			if (csvData.TID == "TID_BUILDING_PALACE" || csvData.TID == "TID_BUILDING_GOLD_STORAGE" || csvData.TID == "TID_BUILDING_WOOD_STORAGE"
			    || csvData.TID == "TID_BUILDING_METAL_STORAGE" || csvData.TID == "TID_BUILDING_STONE_STORAGE" || csvData.TID == "TID_BUILDING_VAULT"){

				if (csvData.MaxStoredResourceGold > 0){
					if (is_upgrade == false && s != null){
						ht.Add("GoldStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","goldIco", s.gold_stored.ToString() + "/" + csvData.MaxStoredResourceGold.ToString(),"",ArtifactType.None,is_upgrade));
					}else{
						ht.Add("GoldStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","goldIco",csvData.MaxStoredResourceGold.ToString(),(csvDataNext.MaxStoredResourceGold - csvData.MaxStoredResourceGold).ToString(),ArtifactType.None,is_upgrade));
					}
				}

				if (csvData.MaxStoredResourceWood > 0){
					if (is_upgrade == false && s != null){
						ht.Add("WoodStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","woodIco", s.wood_stored.ToString() + "/" + csvData.MaxStoredResourceWood.ToString(),"",ArtifactType.None,is_upgrade));
					}else{
						ht.Add("WoodStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","woodIco",csvData.MaxStoredResourceWood.ToString(),(csvDataNext.MaxStoredResourceWood - csvData.MaxStoredResourceWood).ToString(),ArtifactType.None,is_upgrade));
					}
				}

				if (csvData.MaxStoredResourceStone > 0){
					if (is_upgrade == false && s != null){
						ht.Add("StoneStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","stoneIco", s.stone_stored.ToString() + "/" + csvData.MaxStoredResourceStone.ToString(),"",ArtifactType.None,is_upgrade));
					}else{
						ht.Add("StoneStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","stoneIco",csvData.MaxStoredResourceStone.ToString(),(csvDataNext.MaxStoredResourceStone - csvData.MaxStoredResourceStone).ToString(),ArtifactType.None,is_upgrade));
					}
				}

				if (csvData.MaxStoredResourceIron > 0){
					if (is_upgrade == false && s != null){
						ht.Add("MetalStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","ironIco", s.metal_stored.ToString() + "/" + csvData.MaxStoredResourceIron.ToString(),"",ArtifactType.None,is_upgrade));
					}else{
						ht.Add("MetalStored",getPropertyInfo("TID_RESOURCE_STORAGE_CAPACITY","ironIco",csvData.MaxStoredResourceIron.ToString(),(csvDataNext.MaxStoredResourceIron - csvData.MaxStoredResourceIron).ToString(),ArtifactType.None,is_upgrade));
					}
				}
			}

			//破坏力;
			if (csvData.Damage > 0){
				ht.Add("Damage",getPropertyInfo("TID_DAMAGE_PER_SECOND","panelico_damage",csvData.Damage.ToString(),(csvDataNext.Damage - csvData.Damage).ToString(),ArtifactType.BoostBuildingDamage,is_upgrade));
			}



		}else if ("TRAPS".Equals(csvData.TID_Type)){
			//破坏力;
			ht.Add("Damage",getPropertyInfo("TID_DAMAGE","panelico_damage",csvData.Damage.ToString(),(csvDataNext.Damage - csvData.Damage).ToString(),ArtifactType.BoostBuildingDamage,is_upgrade));
		}else if ("SPELLS".Equals(csvData.TID_Type)){
			//破坏力;
			if (csvData.Damage > 0){
				ht.Add("Damage",getPropertyInfo("TID_SPELL_DAMAGE","panelico_damage",csvData.Damage.ToString(),(csvDataNext.Damage - csvData.Damage).ToString(),ArtifactType.BoostTroopDamage,is_upgrade));
			}

			if (csvData.BoostTimeMS > 0){
				float duration = csvData.BoostTimeMS / 1000f;
				string durationTime = duration + " " + LocalizationCustom.instance.Get("TID_TIME_SECS");
				float duration2 = (csvDataNext.BoostTimeMS - csvData.BoostTimeMS) / 1000f - duration;
				string duration2Time ="";
				if (duration2 > 0)
					duration2Time = duration2 + " " + LocalizationCustom.instance.Get("TID_TIME_SECS");

				ht.Add("BoostTimeMS",getPropertyInfo("TID_SPELL_DURATION","panelico_timer",durationTime,duration2Time,ArtifactType.None,is_upgrade));
			}
			if ("TID_MEDKIT".Equals(csvData.TID)){


				float duration = (csvData.NumberOfHits * csvData.TimeBetweenHitsMS*1f) / 1000f;
				string durationTime = duration + " " + LocalizationCustom.instance.Get("TID_TIME_SECS");
				float duration2 = (csvDataNext.NumberOfHits * csvDataNext.TimeBetweenHitsMS*1f) / 1000f - duration;
				string duration2Time = "";
				if (duration2 > 0){
					duration2Time = duration2 + " " + LocalizationCustom.instance.Get("TID_TIME_SECS");
				}

				
				ht.Add("BoostTimeMS",getPropertyInfo("TID_SPELL_DURATION","panelico_timer",durationTime,duration2Time,ArtifactType.None,is_upgrade));


				float heal = -1*csvData.Damage * csvData.NumberOfHits;
				float heal2 = -1*csvDataNext.Damage * csvDataNext.NumberOfHits - heal;



				ht.Add("Heal",getPropertyInfo("TID_TOTAL_HEAL","panelico_timer",heal.ToString(),heal2.ToString(),ArtifactType.None,is_upgrade));

			}

		}else if ("CHARACTERS".Equals(csvData.TID_Type)){

			//血量;
			ht.Add("Health",getPropertyInfo("TID_HITPOINTS","panelico_heart",csvData.Hitpoints.ToString(),(csvDataNext.Hitpoints - csvData.Hitpoints).ToString(),ArtifactType.BoostTroopHP,is_upgrade));

			ht.Add("UnitSize",getPropertyInfo("TID_INFO_HOUSING","panelico_unit",csvData.HousingSpace.ToString(),(csvDataNext.HousingSpace - csvData.HousingSpace).ToString(),ArtifactType.None,is_upgrade));
			ht.Add("TrainingCost",getPropertyInfo("TID_TRAIN_COST","goldIco",csvData.TrainingCost,(int.Parse(csvDataNext.TrainingCost) - int.Parse(csvData.TrainingCost)).ToString(),ArtifactType.None,is_upgrade));

			//使用格式化时间显示;
			string TrainingTime = GetFormatTime(csvData.TrainingTime,1);
			string TrainingTime2 = "";
			if (csvDataNext.TrainingTime - csvData.TrainingTime > 0){
				TrainingTime2 = GetFormatTime(csvDataNext.TrainingTime - csvData.TrainingTime,1);
			}
			ht.Add("TrainingTime",getPropertyInfo("TID_INFO_TRAINING_TIME","panelico_timer",TrainingTime,TrainingTime2,ArtifactType.None,is_upgrade));

			//添加一空行;
			ht.Add("BlankLines",new PropertyInfoNew());

			ht.Add("MoveSpeed",getPropertyInfo("TID_MOVEMENT_SPEED","panelico_speed",csvData.Speed.ToString(),(csvDataNext.Speed - csvData.Speed).ToString(),ArtifactType.None,is_upgrade));
			ht.Add("AttackRange",getPropertyInfo("TID_ATTACK_RANGE","panelico_range",csvData.AttackRange.ToString(),(csvDataNext.AttackRange - csvData.AttackRange).ToString(),ArtifactType.None,is_upgrade));
			//破坏力;
			ht.Add("Damage",getPropertyInfo("TID_DAMAGE_PER_SECOND","panelico_damage",csvData.Damage.ToString(),(csvDataNext.Damage - csvData.Damage).ToString(),ArtifactType.BoostTroopDamage,is_upgrade));

		}

		return ht;
	}

	public static PropertyInfoNew getPropertyInfo(string tid, string spriteName, string value, string upgrade_value, ArtifactType artifact_type, bool is_upgrade){
		PropertyInfoNew pi = new PropertyInfoNew();
		pi.name = tid;
		pi.showName = StringFormat.FormatByTid(tid);
		pi.spriteName = spriteName;
		pi.value = value;
		if (is_upgrade){
			if (upgrade_value != "" && upgrade_value != "0"){
				pi.upgrade_value = "+" + upgrade_value;
				//Debug.Log(pi.upgrade_value);
			}
		}else{
			if (artifact_type != ArtifactType.None){
				int val = getArtifactBoost(int.Parse(pi.value),artifact_type);
				if (val > 0)
					pi.bonus_value =  "+" + val;
				else
					pi.bonus_value = "";
			}else
				pi.bonus_value = "";
		}	
		return pi;
	}
	
	//检查是否可以新建，可以的返回null,不可以的话返回，不可新建原因;
	public static string checkNewBuild(string tid){
		string tid_level = tid + "_1";		
		CsvInfo csvInfo = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];

		//1、先判断是否有解锁;
		if (!Helper.isUnLock(csvInfo.TID_Level)){
			//
			return StringFormat.FormatByTid("TID_UPGRADE_TH_TO_UNLOCK",new object[]{csvInfo.TownHallLevel});
		}else{
			int BuildMax = Helper.getBuildingMaxCount(csvInfo.TID,0);
			int Buildcount = Helper.getBuildingCurCount(csvInfo.TID,null);
			//Debug.Log("csvInfo.TID:" + csvInfo.TID + ";BuildMax:" + BuildMax + ";Buildcount:" + Buildcount);
			if (Buildcount >= BuildMax){
				//TID_UPGRADE_TH_TO_UNLOCK_MORE	Upgrade Headquarters to level <number> to build more!
				//TID_UPGRADE_TH_TO_UNLOCK	Upgrade Headquarters to level <number> to unlock!
				//TID_BUILDING_CAP_REACHED	You've already built the maximum amount of these buildings.
				//不能再新建,已经是最大容量（或需要升级主城来获得更大的容量);				
				int max_count = BuildMax;
				for(int thl = DataManager.GetInstance.model.user_info.town_hall_level + 1; thl < Globals.maxTownHallLevel + 1; thl ++){
					//获取指定建筑物的请允许我最大建筑数量;
					max_count = Helper.getBuildingMaxCount(csvInfo.TID,thl);
					
					if (BuildMax < max_count){
						return StringFormat.FormatByTid("TID_UPGRADE_TH_TO_UNLOCK_MORE",new object[]{thl});
						//break;
					}
				}	
				
				//if (item.BuildMax == max_count){
				return StringFormat.FormatByTid("TID_BUILDING_CAP_REACHED");
				//}
				
			}else{
				//还可以新建;
				return null;
			}
		}
	}

	/*获取指定tid当前最大级别;*/
	public static int getTidMaxLevel(string tid, int min_value){
		int max_level = min_value;
		if (DataManager.GetInstance.researchLevel.ContainsKey(tid)){
			//CHARACTERS,SPELLS,TRAPS;
			max_level = (int)DataManager.GetInstance.researchLevel[tid];
		}else{
			foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
				if (s.tid.Equals(tid) && s.status != BuildStatus.New && max_level < s.level){
					max_level = s.level;				
				}
			}
		}
		return max_level;
	}


	//BUILDING,TRAPS类指定tid_level需要的建筑时间;
	//如果是CHARACTERS生产时间;
	//OBSTACLES为移除时间;
	/*获得建筑类需要新建,生产，移除时间(秒);*/
	public static int GetBuildTime(string tid_level){		
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		if (csvData.BuildingClass == "Artifact"){
			string artiact_tid_level = Helper.BuildTIDToArtifactTID(csvData.TID) + "_1";
			CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];
			return csvArtiact.BuildTimeS;
		}else if ("BUILDING".Equals(csvData.TID_Type)){
			return csvData.BuildTimeD * 86400 + csvData.BuildTimeH * 3600 + csvData.BuildTimeM * 60 + csvData.BuildTimeS;
		}else if ("CHARACTERS".Equals(csvData.TID_Type)){
			return csvData.TrainingTime;
		}else if ("OBSTACLES".Equals(csvData.TID_Type)){
			return csvData.ClearTimeSeconds;
		}else if ("TRAPS".Equals(csvData.TID_Type)){
			return 0;
		}else{
			return 0;
		}
	}	
	//BUILDING,TRAPS类指定tid_level需要的建筑成本;
	//如果是CHARACTERS生产成本;
	//OBSTACLES为移除成本;
	public static BuildCost GetBuildCost(string tid_level){		
		BuildCost bc = new BuildCost();
		//Debug.Log(tid_level);
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		if (csvData.BuildingClass == "Artifact"){
			string artiact_tid_level = Helper.BuildTIDToArtifactTID(csvData.TID) + "_1";
			CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];
			bc.piece_type = csvArtiact.PieceResource;
			bc.gold = 0;
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.piece = csvArtiact.PiecesNeeded;
		}else if ("BUILDING".Equals(csvData.TID_Type)||"TRAPS".Equals(csvData.TID_Type)){
			bc.stone = csvData.BuildCostStone;
			bc.wood = csvData.BuildCostWood;
			bc.iron = csvData.BuildCostIron;
			bc.gold = 0;
			bc.piece = 0;
		}else if ("CHARACTERS".Equals(csvData.TID_Type)){
			bc.gold = int.Parse(csvData.TrainingCost);
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.piece = 0;
		}else if ("OBSTACLES".Equals(csvData.TID_Type)){
			bc.gold = int.Parse(csvData.ClearCost);
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.piece = 0;
		}else{
			bc.gold = 0;
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.piece = 0;
		}
		return bc;
	}

	//建筑类指定tid_level的最大容量;
	public static BuildCost getBuildMaxStored(string tid_level){		
		BuildCost bc = new BuildCost();
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];

		if ("BUILDING".Equals(csvData.TID_Type)){
			bc.gold = csvData.MaxStoredResourceGold;
			bc.stone = csvData.MaxStoredResourceStone;
			bc.wood = csvData.MaxStoredResourceWood;
			bc.iron = csvData.MaxStoredResourceIron;
		}else{
			bc.gold = 0;
			bc.stone = 0;
			bc.wood = 0;
			bc.iron = 0;
		}
		return bc;
	}

	
	/*检查当前建筑物是否可以升级,可升级回返null,不可升级，返回：不可升级原因;*/
	public static string CheckHasUpgrade(string tid, int level){
		string tid_level = tid + "_" + (level + 1);		
		if (CSVManager.GetInstance.csvTable.ContainsKey(tid_level)){
			/*检查配套设备是否也已经升级;*/			
			//BUILDING,CHARACTERS,OBSTACLES,SPELLS,TRAPS
			CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
			
			if ("TID_BUILDING_PALACE".Equals(csvData.TID)){
				//需要经验值判断;TID_CANT_UPGRADE_TOWNHALL_XP_NEEDED = 你的经验等级需要达到<number>级!

				//csvData.RequiredBuilding
				//csvData.RequiredExpLevel;
				//csvData.RequiredTownHallLevel;
				if (csvData.RequiredBuilding != null && csvData.RequiredBuilding != ""){
					//Debug.Log(csvData.RequiredBuilding + ";max_level:" + getTidMaxLevel(csvData.RequiredBuilding,0) + ";RequiredBuildingLevel:" + csvData.RequiredBuildingLevel);
					if (getTidMaxLevel(csvData.RequiredBuilding,0) >= csvData.RequiredBuildingLevel){
						return null;
					}else{
						//TID_MAP_ROOM_LEVEL_REQUIRED Radar level <number> needed!
						//TID_REQUIRED_ACADEMY_LEVEL Level <number> Combat Academy Required!
						if (csvData.RequiredBuilding == "TID_BUILDING_MAP_ROOM"){
							return StringFormat.FormatByTid("TID_MAP_ROOM_LEVEL_REQUIRED", new object[]{csvData.RequiredBuildingLevel});
						}else if (csvData.RequiredBuilding == "TID_BUILDING_LABORATORY"){
							return StringFormat.FormatByTid("TID_BUILDING_LABORATORY", new object[]{csvData.RequiredBuildingLevel});
						}else{
							return null;
						}
					}
				}else{
					return null;
				}								
			}else if ("BUILDING".Equals(csvData.TID_Type)){
				//TID_TOWN_HALL_LEVEL_TOO_LOW To upgrade this building, you first need Headquarters level <number>!
				if (DataManager.GetInstance.model.user_info.town_hall_level >= csvData.TownHallLevel){
					return null;
				}else{
					return StringFormat.FormatByTid("TID_TOWN_HALL_LEVEL_TOO_LOW", new object[]{csvData.TownHallLevel});
				}								
			}else if ("CHARACTERS".Equals(csvData.TID_Type) || "SPELLS".Equals(csvData.TID_Type) || "TRAPS".Equals(csvData.TID_Type)){
				//TID_REQUIRED_ACADEMY_LEVEL Level <number> Combat Academy Required
				if (DataManager.GetInstance.model.user_info.laboratory_level >= csvData.UpgradeHouseLevel){
					return null;
				}else{
					return StringFormat.FormatByTid("TID_REQUIRED_ACADEMY_LEVEL", new object[]{csvData.UpgradeHouseLevel});
				}
			}else{
				return StringFormat.FormatByTid("TID_TROOP_ALREADY_AT_MAX_LEVEL");
			}
			
		}else{
			return StringFormat.FormatByTid("TID_TROOP_ALREADY_AT_MAX_LEVEL");
		}
	}

	//使用宝石直接升级完成;
	public static int GetUpgradeInstant(string tid_level){	
		int gems = 0;				
		//将升级时间换成宝石;
		int upgradetime = GetUpgradeTime(tid_level);
		gems = CalcHelper.calcTimeToGems(upgradetime);

		//将升级资源成本换成宝石;
		BuildCost bc = getUpgradeCost(tid_level);
		if (bc.wood > 0){
			gems += CalcHelper.doCalcResourceToGems(bc.wood);
		}

		if (bc.stone > 0){
			gems += CalcHelper.doCalcResourceToGems(bc.stone);
		}

		if (bc.iron > 0){
			gems += CalcHelper.doCalcResourceToGems(bc.iron);
		}

		if (bc.gold > 0){
			gems += CalcHelper.doCalcResourceToGems(bc.gold);
		}
		
		return gems;
	}

	//升级时间;
	public static int GetUpgradeTime(string tid_level){	
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		if (csvData.BuildingClass == "Artifact"){
			string artiact_tid_level = Helper.BuildTIDToArtifactTID(csvData.TID) + "_1";
			CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];
			return csvArtiact.BuildTimeS;
		}else if ("BUILDING".Equals(csvData.TID_Type)){	
			//需要取下一级里面的,时间级金额;
			string tid_next_level = csvData.TID + "_" + (csvData.Level + 1);
			if (CSVManager.GetInstance.csvTable.ContainsKey(tid_next_level)){
				CsvInfo csvNext = (CsvInfo)CSVManager.GetInstance.csvTable[tid_next_level];
				return csvNext.BuildTimeD * 86400 + csvNext.BuildTimeH * 3600 + csvNext.BuildTimeM * 60 + csvNext.BuildTimeS;
			}else{
				return 0;
			}
		}else if ("CHARACTERS".Equals(csvData.TID_Type) || "SPELLS".Equals(csvData.TID_Type) || "TRAPS".Equals(csvData.TID_Type)){	
			return csvData.UpgradeTimeH * 3600;//当前等级中记录了,升级下一级里面的金额跟时间;
		}else if ("OBSTACLES".Equals(csvData.TID_Type)){
			return csvData.ClearTimeSeconds;
		}else{
			return 0;
		}
	}	


	//升级成本;
	public static BuildCost getUpgradeCost(string tid_level){	
		BuildCost bc = new BuildCost();
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		if (csvData.BuildingClass == "Artifact"){
			string artiact_tid_level = Helper.BuildTIDToArtifactTID(csvData.TID) + "_1";
			CsvInfo csvArtiact = (CsvInfo)CSVManager.GetInstance.csvTable[artiact_tid_level];

			bc.piece_type = csvArtiact.PieceResource;
			bc.gold = 0;
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.piece = csvArtiact.PiecesNeeded;
			return bc;
		}else if ("BUILDING".Equals(csvData.TID_Type)){	
			//需要取下一级里面的,时间级金额;
			string tid_next_level = csvData.TID + "_" + (csvData.Level + 1);
			if (CSVManager.GetInstance.csvTable.ContainsKey(tid_next_level)){
				CsvInfo csvNext = (CsvInfo)CSVManager.GetInstance.csvTable[tid_next_level];
				bc.stone = csvNext.BuildCostStone;
				bc.wood = csvNext.BuildCostWood;
				bc.iron = csvNext.BuildCostIron;
				bc.gold = 0;
				bc.piece = 0;
			}
		}else if ("CHARACTERS".Equals(csvData.TID_Type) || "SPELLS".Equals(csvData.TID_Type) || "TRAPS".Equals(csvData.TID_Type)){	
			//当前等级中记录了,升级下一级里面的金额跟时间;
			bc.stone = 0;
			bc.wood = 0;
			bc.iron = 0;
			bc.gold = int.Parse(csvData.UpgradeCost);
			bc.piece = 0;
		}else if ("OBSTACLES".Equals(csvData.TID_Type)){
			bc.gold = int.Parse(csvData.ClearCost);
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.piece = 0;
		}
		return bc;
	}

	/*返回Unix时间戳秒;*/
	public static int current_time(bool diff = true){	
		//return 55;
		
		long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
		
		if (diff){
			return (int)((epoch + Globals.time_difference)/1000);
		}else{
			return (int)(epoch/1000);
		}
		//return epoch + time_difference;
	}
	
	public static long current_millisecond(){		
		long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
		
		return epoch + Globals.time_difference;
		//return epoch + time_difference;
	}
	
	public static double current_time_d(){		
		long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
		
		return (double)((double)(epoch + Globals.time_difference)/1000.0f);
		//return epoch + time_difference;
	}	
	
	public static long getNewBuildingId(){		
		return current_millisecond();//(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;		
	}	

	public static bool CheckUserName(string name)
	{
		if (!"".Equals(name) && name.Length >= 2 && name.Length <= 12
		    && name.ToLower().LastIndexOf("players") == -1 && !"user_reg".Equals(name.ToLower()) && !"players_reg".Equals(name.ToLower()) && !"receive_msg".Equals(name.ToLower()) && !"91".Equals(name.ToLower())
		    && !"360".Equals(name.ToLower()) && !"pp".Equals(name.ToLower()) && !"admin".Equals(name.ToLower())
		    && name.LastIndexOf("^") == -1 && name.LastIndexOf("|") == -1 && name.LastIndexOf("*") == -1
		    && name.LastIndexOf("&") == -1 && name.LastIndexOf("%") == -1 && name.LastIndexOf("'") == -1
		    && name.LastIndexOf(",") == -1 && name.LastIndexOf(";") == -1 && name.LastIndexOf("\"") == -1
		    && name.LastIndexOf("?") == -1 && name.LastIndexOf("//") == -1
		    )
		{
			
			return true;
		}
		else
		{
			return false;
		}
	}

	//返回：建筑物名称对应的tid;
	public static void initNameToTid(){
		//artifacts与buildings中的3个Artifact需要注意区分与关联;
		//nameToTid["Common Artifact"]="TID_COMMON_ARTIFACT";
		//nameToTid["Epic Artifact"]="TID_EPIC_ARTIFACT";
		//nameToTid["Rare Artifact"]="TID_RARE_ARTIFACT";
		
		//artifact_bonuses
		nameToTid["BoostGunshipEnergy"]="TID_BOOST_GUNSHIP_ENERGY";
		nameToTid["BoostLoot"]="TID_BOOST_LOOT";
		nameToTid["BoostArtifactDrop"]="TID_BOOST_ARTIFACT_DROP";
		nameToTid["BoostAllResources"]="TID_BOOST_ALL";
		nameToTid["BoostBuildingDamage"]="TID_BOOST_BUILDING_DAMAGE";
		nameToTid["BoostBuildingHP"]="TID_BOOST_BUILDING_HP";
		nameToTid["BoostGold"]="TID_BOOST_GOLD";
		nameToTid["BoostMetal"]="TID_BOOST_METAL";
		nameToTid["BoostStone"]="TID_BOOST_STONE";
		nameToTid["BoostTroopDamage"]="TID_BOOST_TROOP_DAMAGE";
		nameToTid["BoostTroopHP"]="TID_BOOST_TROOP_HP";
		nameToTid["BoostWood"]="TID_BOOST_WOOD";
		//buildings
		nameToTid["AI Bunker"]="TID_AI_BUNKER";
		nameToTid["Artifact Workshop"]="TID_BUILDING_ARTIFACT_WORKSHOP";
		nameToTid["Barrels"]="TID_AI_HUT";
		nameToTid["Big Bertha"]="TID_BUILDING_BIG_BERTHA";
		nameToTid["Boat"]="TID_BUILDING_LANDING_SHIP";
		nameToTid["Boss Machine Gun"]="TID_BUILDING_BOSS_MACHINE_GUN";
		nameToTid["Boss Mortar"]="TID_BUILDING_BOSS_MORTAR";
		nameToTid["Cannon"]="TID_BUILDING_CANNON";
		nameToTid["Command Center"]="TID_BUILDING_COMMAND_CENTER";
		nameToTid["Common Artifact"]="TID_BUILDING_ARTIFACT1";
		nameToTid["Crates1"]="TID_CRATES1";
		nameToTid["Crates2"]="TID_CRATES2";
		nameToTid["Crates3"]="TID_CRATES3";
		nameToTid["Crates4"]="TID_CRATES4";
		nameToTid["Crates5"]="TID_CRATES5";
		nameToTid["Epic Artifact"]="TID_BUILDING_ARTIFACT3";
		nameToTid["Flame Thrower"]="TID_FLAME_THROWER";
		nameToTid["Gold Storage"]="TID_BUILDING_GOLD_STORAGE";
		nameToTid["Guard Tower"]="TID_GUARD_TOWER";
		nameToTid["Gunboat"]="TID_BUILDING_GUNSHIP";
		nameToTid["Housing"]="TID_BUILDING_HOUSING";
		nameToTid["HQ"]="TID_BUILDING_PALACE";
		nameToTid["Laboratory"]="TID_BUILDING_LABORATORY";
		nameToTid["Machine Gun Nest"]="TID_MACHINE_GUN_NEST";
		nameToTid["Map Room"]="TID_BUILDING_MAP_ROOM";
		nameToTid["Metal Mine"]="TID_BUILDING_METAL_MINE";
		nameToTid["Metal Storage"]="TID_BUILDING_METAL_STORAGE";
		nameToTid["Missile Launcher"]="TID_MISSILE_LAUNCHER";
		nameToTid["Mortar"]="TID_BUILDING_MORTAR";
		nameToTid["Rare Artifact"]="TID_BUILDING_ARTIFACT2";
		nameToTid["Stone Quarry"]="TID_BUILDING_STONE_QUARRY";
		nameToTid["Stone Storage"]="TID_BUILDING_STONE_STORAGE";
		nameToTid["Vault"]="TID_BUILDING_VAULT";
		nameToTid["Wall"]="TID_BUILDING_WALL";
		nameToTid["Wood Storage"]="TID_BUILDING_WOOD_STORAGE";
		nameToTid["Woodcutters Hut"]="TID_BUILDING_WOODCUTTER";
		
		//characters
		nameToTid["AI Units"]="TID_UNUSED";
		nameToTid["Heavy"]="TID_HEAVY";
		nameToTid["Rifleman"]="TID_RIFLEMAN";
		nameToTid["Tank"]="TID_TANK";
		nameToTid["Warrior"]="TID_WARRIOR";
		nameToTid["Zooka"]="TID_ZOOKA";
		
		//decos
		nameToTid["Burned Hut"]="TID_DAMAGED_HUT2";
		nameToTid["Cage"]="TID_ISLANDER_CAGE";
		nameToTid["Campfire"]="TID_CAMPFIRE";
		nameToTid["Damaged Hut"]="TID_DAMAGED_HUT1";
		nameToTid["Debris2"]="TID_DEBRIS2";
		nameToTid["Debris3"]="TID_DEBRIS3";
		nameToTid["Debris4"]="TID_DEBRIS4";
		nameToTid["Debris5"]="TID_DEBRIS5";
		
		//obstacles
		nameToTid["Native Hut"]="TID_ISLANDER_HUT";
		nameToTid["Ravaged Hut"]="TID_DAMAGED_HUT3";
		nameToTid["Airstrike"]="TID_BARRAGE";
		nameToTid["Artillery"]="TID_ARTILLERY";
		nameToTid["Flare"]="TID_FLARE";
		nameToTid["Medkit"]="TID_MEDKIT";
		nameToTid["Stun"]="TID_STUN";
		
		//traps
		nameToTid["Mine"]="TID_TRAP_MINE";
		nameToTid["Tank Mine"]="TID_TRAP_TANK_MINE";
		
		//resources
		nameToTid["Diamonds"]="TID_DIAMONDS";
		nameToTid["Resource1"]="TID_GOLD";
		nameToTid["Resource2"]="TID_WOOD";
		nameToTid["Resource3"]="TID_STONE";
		nameToTid["Resource4"]="TID_METAL";

		nameToTid["CommonPiece"]="TID_COMMON_PIECE";
		nameToTid["RarePiece"]="TID_RARE_PIECE";
		nameToTid["EpicPiece"]="TID_EPIC_PIECE";

		nameToTid["CommonPieceIce"]="TID_COMMON_PIECE_ICE";
		nameToTid["RarePieceIce"]="TID_RARE_PIECE_ICE";
		nameToTid["EpicPieceIce"]="TID_EPIC_PIECE_ICE";

		nameToTid["CommonPieceFire"]="TID_COMMON_PIECE";
		nameToTid["RarePieceFire"]="TID_RARE_PIECE";
		nameToTid["EpicPieceFire"]="TID_EPIC_PIECE";

		nameToTid["CommonPieceDark"]="TID_COMMON_PIECE_DARK";
		nameToTid["RarePieceDark"]="TID_RARE_PIECE_DARK";
		nameToTid["EpicPieceDark"]="TID_EPIC_PIECE_DARK";
		
	}
	
	public static string GetNameToTid(string name){
		if (nameToTid.ContainsKey(name)){
			return nameToTid[name] as string;
		}else{
			return name;
		}
	}


	// Get time:d h m s,type 0:1d 23m type 1:1d 23h 45m 30s type 2:1d 23h 45m
	public static string GetFormatTime (int remainTime, int type) {
		string result = string.Empty;
		
		if(type < 0 && type > 2) return result;
		
		// Day,hour,minute and second
		int day = 0;
		int minute = 0;
		int hour = 0;
		int second = 0;
		
		// Greater than or equal to one day
		if(remainTime >= 86400){
			// How many days
			day = Mathf.FloorToInt(remainTime * (1.0f/86400));
			
			// How many hours
			hour = Mathf.FloorToInt((remainTime - day*86400) * (1.0f/3600));
			
			if(type == 1 || type == 2){
				// How many minutes
				minute = Mathf.FloorToInt((remainTime - day*86400 - hour*3600) * (1.0f/60));
				
				if(type == 1){
					// How many seconds
					second = Mathf.FloorToInt(remainTime - day*86400 - hour*3600 - minute*60);
				}
			}
			
			if(LocalizationCustom.instance != null){
				if(type == 0){
					if(hour != 0)
						result = day + LocalizationCustom.instance.Get("TID_TIME_DAYS")+ " " + hour + LocalizationCustom.instance.Get("TID_TIME_HOURS");
					else
						result = day + LocalizationCustom.instance.Get("TID_TIME_DAYS");
				}
				else if(type == 1 || type == 2){
					result = day + LocalizationCustom.instance.Get("TID_TIME_DAYS");
					
					if(hour != 0)
						result += (" " + hour + LocalizationCustom.instance.Get("TID_TIME_HOURS"));
					
					if(minute != 0)
						result += (" " + minute + LocalizationCustom.instance.Get("TID_TIME_MINS"));
					
					if(type == 1 && second != 0)
						result += (" " + second + LocalizationCustom.instance.Get("TID_TIME_SECS"));
				}
			}
		}
		// Greater than or equal to one hour
		else if(remainTime >= 3600){
			// How many hours
			hour = Mathf.FloorToInt(remainTime * (1.0f/3600));
			
			// How many minutes
			minute = Mathf.FloorToInt((remainTime - hour*3600) * (1.0f/60));
			
			if(type == 1){
				// How many seconds
				second = Mathf.FloorToInt(remainTime - hour*3600 - minute*60);
			}
			
			if(LocalizationCustom.instance != null){
				if(type == 0 || type == 2){
					if(minute != 0)
						result = hour+ LocalizationCustom.instance.Get("TID_TIME_HOURS")+" "+minute+ LocalizationCustom.instance.Get("TID_TIME_MINS");
					else
						result = hour+ LocalizationCustom.instance.Get("TID_TIME_HOURS");
				}
				else if(type == 1){
					result = hour+ LocalizationCustom.instance.Get("TID_TIME_HOURS");
					
					if(minute != 0)
						result += (" " + minute + LocalizationCustom.instance.Get("TID_TIME_MINS"));
					
					if(second != 0)
						result += (" " + second + LocalizationCustom.instance.Get("TID_TIME_SECS"));
				}
			}
		}
		// Greater than or equal to one minute
		else if(remainTime >= 60){
			// How many minutes
			minute = Mathf.FloorToInt(remainTime * (1.0f/60));
			
			if(type != 2){
				// How many seconds
				second = remainTime - minute*60;
			}
			
			if(LocalizationCustom.instance != null){
				if(second != 0)
					result = minute+ LocalizationCustom.instance.Get("TID_TIME_MINS")+" "+second+ LocalizationCustom.instance.Get("TID_TIME_SECS");
				else
					result = minute+ LocalizationCustom.instance.Get("TID_TIME_MINS");
			}
		}
		// Greater than or equal to one second
		else if(type != 2 && remainTime >= 0){
			// How many seconds
			second = remainTime;
			
			if(LocalizationCustom.instance != null)
				result = second+ LocalizationCustom.instance.Get("TID_TIME_SECS");
		}
		
		return result; 
	}	

	//加载岛的不可通行区配置;
	public static void LoadIsLandCsv(TextAsset textFile){
		if (textFile == null) return;
		if (CSVManager.GetInstance.island_grid_csv != null)
			CSVManager.GetInstance.island_grid_csv.Clear ();
		else
			CSVManager.GetInstance.island_grid_csv = new Dictionary<string, int[,]> ();

		string[] lineArray  = textFile.text.Split("\n"[0]);	
		//string[] FieldNames = lineArray[0].Trim().Replace("\"","").Split(","[0]);
		/*
layout/small_a.level
layout/small_b.level
layout/mainland_a.level
layout/mainland_b.level
layout/med_a.level
layout/playerbase.level
layout/enemybase.level
*/
		String last_island_name = null;
		int[,] grid = null;//new int[40,40];
		for (int i = 1; i < lineArray.Length; i ++){			
			string[] valueStrings = lineArray[i].Trim().Split(","[0]);
			String island_name = valueStrings[0];
			if (last_island_name != island_name){
				last_island_name = island_name;
				grid = new int[Globals.GridTotal,Globals.GridTotal];
				CSVManager.GetInstance.island_grid_csv.Add(island_name,grid);
			}

			int x =  int.Parse(valueStrings[1].Trim());
			int y =  int.Parse(valueStrings[2].Trim());

			//Debug.Log("island_name:" + island_name + ";x:" + x + ";y:" + y);
			grid[x,y] = 1;
		}
		/*
		Debug.Log(CSVMananger.GetInstance.island_grid_csv.Count);

		grid = CSVMananger.GetInstance.island_grid_csv[last_island_name];
		for(int i = 0;i < 40; i ++){
			for(int j = 0;j < 40; j ++){
				Debug.Log(i + "_" + j + ":" + grid[i,j]);
			}
		}
		*/
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textFile"></param>
    /// <param name="myHashtable"></param>
    /// <param name="tid_type"></param>
    /// <param name="add_level"></param>
    /// <param name="show_sql"></param>
	public static void loadcsv(TextAsset textFile, Hashtable myHashtable, string tid_type,bool add_level = false, bool show_sql = false){
		if (textFile == null) return;

		string table_name = "fanwe_" + textFile.name;
		//Debug.Log(table_name);

		string[] lineArray  = textFile.text.Split("\n"[0]);	
		string[] FieldNames = lineArray[0].Trim().Replace("\"","").Split(","[0]);
		string[] FieldTypes2 = lineArray[1].Trim().Replace("\"","").ToLower().Split(","[0]);
		Hashtable GroupFieldValues = new Hashtable();
		Hashtable HFieldTypes = new Hashtable();
		
		ArrayList SqlList = new ArrayList();
		string sql = "";
		
		//处理字段名;
		for (int k = 0; k < FieldNames.Length; k ++){
			if (table_name == "fanwe_townhall_levels"){
				FieldNames[k] = Helper.GetNameToTid(FieldNames[k]);
			}
			HFieldTypes[FieldNames[k]] = FieldTypes2[k];
		}
		
		if (show_sql){
			
			
			for (int k = 0; k < FieldNames.Length; k ++){
				string field_name = FieldNames[k];
				string field_type = HFieldTypes[field_name] as string;// FieldTypes[k];
				
				if (field_type == "string"){
					sql += "public string " + field_name + ";\n";
				}else if (field_type == "int"){
					sql += "public int " + field_name + ";\n";
				}else if (field_type == "boolean"){
					sql += "public int " + field_name + ";\n";
				}else{
					Debug.Log(field_type + ":" + field_type);
				}
			}
			
			if (table_name == "fanwe_artifacts"){
				sql += "public string TID;";
			}
			
			if (add_level){
				sql += "public int Level;";
			}

			if (table_name == "fanwe_regions"){
				sql += "public int id;";
			}

			Debug.Log(sql);
			
			//Debug.Log(lineArray[0]);
			//Debug.Log(lineArray[0].Trim().Replace("\"",""));
			//Debug.Log(FieldNames);
			//Debug.Log(FieldTypes);
			
			SqlList.Add("DROP TABLE IF EXISTS " + table_name + ";");
			sql = "CREATE TABLE " + table_name + "(";
			for (int k = 0; k < FieldNames.Length; k ++){
				
				string field_name = FieldNames[k];		
				string field_type = HFieldTypes[field_name] as string; //FieldTypes[k];
				
				if (field_type == "string"){
					sql += "`" + field_name + "`" +  " varchar(100)";
				}else if (field_type == "int"){
					sql += "`" + field_name + "`" + " int(10)";
				}else if (field_type == "boolean"){
					sql += "`" + field_name + "`" + " int(1)";
				}else{
					Debug.Log(field_name + ":" + field_type);
				}
				
				if (k + 1 < FieldNames.Length){
					sql += ",";
				}
			}
			
			if (add_level){
				sql += ",`Level` int(5)";
			}
			
			if (table_name == "fanwe_artifacts"){
				sql += ",`TID` varchar(100)";
			}

			if (table_name == "fanwe_regions"){
				sql += ",`id` int(10) NOT NULL";
			}

			sql += ") ENGINE=InnoDB DEFAULT CHARSET=utf8;";
			
			SqlList.Add(sql);
			//Debug.Log(sql);
			
		}

		Hashtable p_FieldValues = null;//上一条记录的FieldValues;
		int level = 0;
		for (int i = 2; i < lineArray.Length; i ++){
			
			
			string[] valueStrings = lineArray[i].Trim().Split(","[0]);
			//string[] FieldValues = new string[FieldNames.Length + 1];
			Hashtable FieldValues = new Hashtable();
			if (table_name == "fanwe_regions"){
				FieldValues["id"] = (i - 1);
			}
			bool is_join = false;
			int index = 0;
			string val = "";
			if (valueStrings.Length > 1){
				
				
				for (int j = 0; j < valueStrings.Length; j ++){									
					string field_name = FieldNames[index];
					string field_type = HFieldTypes[field_name] as string; //FieldTypes[index];
					
					string str = valueStrings[j].Trim();
					
					if (str != null && str != ""){
						int first_pos = str.IndexOf("\"");
						int last_pos = str.LastIndexOf("\"");
						
						if ((first_pos == -1 && is_join == false) || (first_pos >-1 && first_pos != last_pos)){
							str = str.Trim().Replace("\"","");
							if (field_type == "boolean"){
								if (str.ToLower() == "true")
									FieldValues[field_name] = "1";
								else
									FieldValues[field_name] = "0";
							}else{
								if (table_name == "fanwe_townhall_levels" && field_name == "RequiredBuilding" )
									FieldValues[field_name] = Helper.GetNameToTid(str);
								else
									FieldValues[field_name] = str;
							}
							index ++;							
						}else if (first_pos >-1 && first_pos == last_pos){
							//有找到一个（只有一个),在开始跟结束时进入;
							if (is_join){
								is_join = false;
								FieldValues[field_name] = (val + "," + str).Replace("\"","");;
								index ++;
							}else{
								is_join = true;
								val = str;
							}
						}else if (first_pos == -1 && is_join == true){
							val = val + "," + str;
						}
					}else{
						
						
						if (FieldValues["Name"] != null && (string)FieldValues["Name"] != ""){

							if (p_FieldValues == null || table_name != "fanwe_townhall_levels"){
								if (field_type == "string"){
									FieldValues[field_name] = "";
									//Debug.Log(i + ":" + FieldNames[index] + ";1:" + FieldTypes[index] + ":" + FieldValues[index]);
								}else if (field_type == "int"){
									FieldValues[field_name] = "0";
									//Debug.Log(i + ":" + FieldNames[index] + ";2:" + FieldTypes[index] + ":" + FieldValues[index]);
								}else if (field_type == "boolean"){
									FieldValues[field_name] = "0";
									//Debug.Log(i + ":" + FieldNames[index] + ";3:" + FieldTypes[index] + ":" + FieldValues[index]);
								}else{
									FieldValues[field_name] = null;
									//Debug.Log(i + ":" + FieldNames[index] + ";4:" + FieldTypes[index] + ":" + FieldValues[index]);
								}
							}else{
								if (field_name != "Name"){
									FieldValues[field_name] = p_FieldValues[field_name];
									//Debug.Log("GroupFieldValues[" + index + "]:" + GroupFieldValues[index]);
								}
							}
							/*
							if ("int".Equals(FieldTypes[index])){
								Debug.Log("1:" + FieldTypes[index]);
							}else{
								Debug.Log("1:" + FieldTypes[index]);
							}*/
						}else{
							if (field_name != "Name"){
								FieldValues[field_name] = GroupFieldValues[field_name];
								//Debug.Log("GroupFieldValues[" + index + "]:" + GroupFieldValues[index]);
							}
						}
						
						
						index ++;
					}
					//Debug.Log(valueStrings[j] + ";IndexOf:" + val.IndexOf("\"") + ";LastIndexOf:" + val.LastIndexOf("\""));
					//tmp[titleStrings[j]] = valueStrings[j];
				}

                //以下是特殊业务逻辑
                if (!string.IsNullOrEmpty((string)FieldValues["Name"]))//CSV表里的Name字段如果不填，则表示其级别为上一行所指向的级别加1
                {

                    if (table_name == "fanwe_townhall_levels")//对于townhall_levels表，其名字就是其Level
                    {
						FieldValues["Level"] = FieldValues["Name"];
					}else{
						level = 1;//默认第一级有名字的条目
						FieldValues["Level"] = level.ToString();
					}
					for (int k = 0; k < FieldNames.Length; k ++){
						GroupFieldValues[FieldNames[k]] = FieldValues[FieldNames[k]];
					}
					
					
					//Debug.Log("GroupFieldValues[0]:" + GroupFieldValues[0]);
				}else{
					FieldValues["Name"] = GroupFieldValues["Name"];
					level ++;
					FieldValues["Level"] = level.ToString();
				}
				

              
				string field_name_value = FieldValues["Name"] as string;
				if (field_name_value == "Crates1" || field_name_value == "Crates2" || field_name_value == "Crates3" ||field_name_value == "Crates4" ||field_name_value == "Crates5"
				    ||field_name_value == "Debris2"||field_name_value == "Debris3"||field_name_value == "Debris4"||field_name_value == "Debris5"
				    ){
					FieldValues["TID"] = "TID_" + field_name_value.ToUpper();
				}
				
				if (table_name == "fanwe_artifacts")//如果是遗迹，则
                {
					FieldValues["TID"] = "TID_" + field_name_value.Replace(" ","_").ToUpper();
				}else if (table_name == "fanwe_townhall_levels"){
					FieldValues["TID"] = "TID_BUILDING_PALACE";
				}
				
				if (table_name == "fanwe_townhall_levels"){
					//Debug.Log(field_name_value + ":" + FieldValues["Name"]);

					int thl = int.Parse(field_name_value);
					if (thl > Globals.maxTownHallLevel)
						Globals.maxTownHallLevel = thl;

					//Debug.Log(Globals.maxTownHallLevel);
				}


				
				FieldValues["TID_Type"] = tid_type;
				string tid_level = FieldValues["TID"] + "_" + FieldValues["Level"];
				FieldValues["TID_Level"] = tid_level;
				p_FieldValues = FieldValues;
				/*
				foreach(string key in FieldValues.Keys){
					Debug.Log(key + ":" + FieldValues[key] + ":" + HFieldTypes[key]);
				}*/


				//Common Artifact,Rare Artifact,Epic Artifact;

				//不导入buildings中的 Tutorial HQ;
				if (field_name_value != "Tutorial HQ"){

					if (table_name == "fanwe_regions"){
						if (!myHashtable.ContainsKey(field_name_value)){						
							Regions regions = new Regions();
							myHashtable[field_name_value] = regions;
							regions.HashtableToBean(FieldValues);
						}


					}else if (table_name == "fanwe_experience_levels"){
												
						ExperienceLevels exp_lv = new ExperienceLevels();
						myHashtable[field_name_value] = exp_lv;
						exp_lv.HashtableToBean(FieldValues);
						
					}else if (table_name == "fanwe_projectiles"){
						
						Projectiles proj = new Projectiles();
						myHashtable[field_name_value] = proj;
						proj.HashtableToBean(FieldValues);
						
					}else if (table_name == "fanwe_globals"){
						
						GlobalsItem glob = new GlobalsItem();
						myHashtable[field_name_value] = glob;
						glob.HashtableToBean(FieldValues);
						
					}else{
						CsvInfo csvData = null;
						if (myHashtable.ContainsKey(tid_level)){
							csvData = myHashtable[tid_level] as CsvInfo;
						}else{
							csvData = new CsvInfo();
							myHashtable[tid_level] = csvData;
						}
						csvData.HashtableToBean(FieldValues);

					}

				}
				//break;
				
				if (show_sql){
					//Debug.Log("================================");
					
					sql = "insert into " + table_name + "(";
					for (int k = 0; k < FieldNames.Length; k ++){
						string field_name = FieldNames[k];
						
						sql += "`" + field_name + "`";
						
						if (k + 1 < FieldNames.Length){
							sql += ",";
						}
						
					}
					
					if (add_level){
						sql += ", `Level`";
					}
					
					if (table_name == "fanwe_artifacts"){
						sql += ", `tid`";
					}

					if (table_name == "fanwe_regions"){
						sql += ",`id`";
					}

					sql += " ) values (";
					for (int k = 0; k < FieldNames.Length; k ++){
						string field_name = FieldNames[k];
						string field_type = HFieldTypes[field_name] as string; //FieldTypes[k];
						string field_value =(string)FieldValues[field_name];
						
						if (field_type == "string"){
							if (field_value != null && field_value != ""){
								if (table_name == "fanwe_townhall_levels" && field_name == "RequiredBuilding" ){
									//FieldValues[FieldNames.Length] = FieldValues[0];
									sql += "\"" + Helper.GetNameToTid(field_value) +  "\"";
								}else{
									sql += "\"" + field_value +  "\"";
								}
								
							}else{
								sql += "null";
							}
						}else if (field_type == "int"){
							if (field_value != null && field_value != ""){
								sql += field_value;
							}else{
								sql += "0";
							}
						}else if (field_type == "boolean"){
							if (field_value.ToLower() == "true")
								sql += "1";
							else
								sql += "0";
						}else{
							Debug.Log(field_name + ":" + field_type + ":" + field_value);
						}
						
						if (k + 1 < FieldNames.Length){
							sql += ",";
						}
					}
					
					if (add_level){
						sql += "," + FieldValues["Level"];
					}
					
					if (table_name == "fanwe_artifacts"){
						sql += ",\"" + FieldValues["TID"] + "\"";
					}

					if (table_name == "fanwe_regions"){
						sql += "," + FieldValues["id"];
					}

					sql += ");";
					
					//不导入buildings中的 Tutorial HQ;
					if (field_name_value != "Tutorial HQ"){
						SqlList.Add(sql);
					}
				}
			}
			
			
		}
		
		if (show_sql){
			sql = "";
			for(int i = 0; i < SqlList.Count; i++){
				sql += SqlList[i] + "\n";
			}
			
			Debug.Log(sql);
		}
	}

	//与服务器的ResultSetToSFSObject对应;
	public static ISFSArray SFSObjToArr(ISFSObject msg_array){
		
		ISFSArray msg_list = SFSArray.NewInstance();
		int len = 0;
		for (int i = 0; i < msg_array.GetKeys().Length; i++){
			String key = msg_array.GetKeys()[i];
			SFSDataType type = (SFSDataType)msg_array.GetData(key).Type;				
			if (type == SFSDataType.INT_ARRAY){
				len = msg_array.GetIntArray(key).Length;
			}else if (type == SFSDataType.UTF_STRING_ARRAY){
				len = msg_array.GetUtfStringArray(key).Length;
			}else if (type == SFSDataType.FLOAT_ARRAY){
				len = msg_array.GetFloatArray(key).Length;
			}else if (type == SFSDataType.DOUBLE_ARRAY){
				len = msg_array.GetDoubleArray(key).Length;
			}else if (type == SFSDataType.SHORT_ARRAY){
				len = msg_array.GetShortArray(key).Length;
			}else if (type == SFSDataType.LONG_ARRAY){
				len = msg_array.GetLongArray(key).Length;
			}else if (type == SFSDataType.BOOL_ARRAY){
				len = msg_array.GetBoolArray(key).Length;
			}
			if (len > 0) break;
		}	
		
		
		for(int iLen = 0; iLen < len; iLen++){
			
			ISFSObject item = SFSObject.NewInstance();
			for (int i = 0; i < msg_array.GetKeys().Length; i++){
				String key = msg_array.GetKeys()[i];
				
				SFSDataType type = (SFSDataType)msg_array.GetData(key).Type;
				
				//Debug.Log(key + ":" + type.ToString());
				
				if (type == SFSDataType.INT_ARRAY){
					item.PutInt(key,msg_array.GetIntArray(key)[iLen]);
				}else if (type == SFSDataType.UTF_STRING_ARRAY){
					item.PutUtfString(key,msg_array.GetUtfStringArray(key)[iLen]);
				}else if (type == SFSDataType.FLOAT_ARRAY){
					item.PutFloat(key,msg_array.GetFloatArray(key)[iLen]);
				}else if (type == SFSDataType.DOUBLE_ARRAY){
					item.PutDouble(key,msg_array.GetDoubleArray(key)[iLen]);
				}else if (type == SFSDataType.SHORT_ARRAY){
					item.PutShort(key,msg_array.GetShortArray(key)[iLen]);
				}else if (type == SFSDataType.LONG_ARRAY){
					item.PutLong(key,msg_array.GetLongArray(key)[iLen]);
				}else if (type == SFSDataType.BOOL_ARRAY){
					item.PutBool(key,msg_array.GetBoolArray(key)[iLen]);
				}
			}
			
			msg_list.AddSFSObject(item);
		}
		
		return msg_list;
	}	

	public static void ISFSObjectToBean(UnityEngine.Object entity, ISFSObject item){
		const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;	    
		FieldInfo[] fields = entity.GetType().GetFields(flags);

        //System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //for (int i = 0; i < fields.Length; i++)
        //{

        //    sb.Append(fields[i].Name);
        //    sb.Append(":").AppendLine(fields[i].FieldType.ToString()); ;

        //}

        //Debug.LogError(sb.ToString());

        //UnityEditor.EditorApplication.isPaused = true;

		//Debug.Log(item.GetDump());			
		foreach (FieldInfo fieldInfo in fields)
		{				
			string fieldType  = fieldInfo.FieldType.ToString();			
			
			if (item.ContainsKey(fieldInfo.Name)){
				/*if (fieldInfo.Name == "ap_potential")
                    {
                        Debug.Log(item.GetDump());
				        Debug.Log("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType);	
					}*/
				//string tmp = (string)hashtable[fieldInfo.Name];					
				//Debug.Log("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType);					
				//fieldInfo.SetValue(this,tmp);
				//TODO
				if (fieldInfo.Name == "start_time" || fieldInfo.Name == "end_time")
					continue;

				if (fieldType.Equals("System.Int16")					
				    || fieldType.Equals("System.Int32") 						
				    || fieldType.Equals("System.UInt16")
				    || fieldType.Equals("System.UInt32")						
				    || fieldType.Equals("System.IntPtr")){
					
					
					fieldInfo.SetValue(entity, item.GetInt(fieldInfo.Name));
					
					
				}else if (fieldType.Equals("System.String")					
				          || fieldType.Equals("System.StringComparer")
				          ){

                    

                    try
                    {
                        //Debug.LogError("FieldInfo : " + fieldInfo.Name + " fieldType : " + fieldType + " getUFTString : " + item.GetUtfString(fieldInfo.Name));
                        fieldInfo.SetValue(entity, item.GetUtfString(fieldInfo.Name));
                    }
                    catch (Exception ex)
                    {

                        Debug.LogError("Field name : "+fieldInfo.Name+" error message : "+ex.Message);
                    }

					
				}else if (fieldType.Equals("System.UInt64") || fieldType.Equals("System.Int64")){	
					if (fieldInfo.Name == "building_id") {
						//TODO
						fieldInfo.SetValue (entity, (long)(item.GetInt (fieldInfo.Name)));	
					} else {
						fieldInfo.SetValue(entity,item.GetLong(fieldInfo.Name));
					}
				}	
				else if(fieldType.Equals("System.Single"))
				{
					SFSDataType sdtype = (SFSDataType)item.GetData(fieldInfo.Name).Type;
					if (sdtype == SFSDataType.FLOAT){
						fieldInfo.SetValue(entity, item.GetFloat(fieldInfo.Name));
					}else if (sdtype == SFSDataType.DOUBLE){
						//Debug.Log(item.GetDump());
						Double d = item.GetDouble(fieldInfo.Name);						
						//Debug.Log("d:" + d);
						float f = float.Parse(d.ToString());
						//Debug.Log("f:" + f);
						fieldInfo.SetValue(entity, f);
						//Debug.Log("----------");
					}				
				}
			}	    	
		}	
	}





	/*检查当前建筑物是否还有下一级别;*/
	public static bool checkHasNextLevel(BuildInfo s){
		//Debug.Log1("checkHasNextLevel:" + building_id);		
		string tid_level = s.tid + "_" + (s.level + 1);	
		//Debug.Log1("checkHasNextLevel:" + tid_level);
		return CSVManager.GetInstance.csvTable.ContainsKey(tid_level);
	}


	/*判断是否允许创建/是否已经解锁;*/
	public static bool isUnLock(string tid_level){

		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		
		//Debug.Log1("csvData.tid_type:" + csvData.tid_type);
		
		if ("BUILDING".Equals(csvData.TID_Type)){
			return DataManager.GetInstance.model.user_info.town_hall_level >= csvData.TownHallLevel;								
		}else if ("CHARACTERS".Equals(csvData.TID_Type)){
			//return  DataManager.GetInstance.model.user_info.laboratory_level >= csvData.UpgradeHouseLevel && DataManager.GetInstance.model.user_info.town_hall_level >= csvData.UnlockTownHallLevel;
			return  DataManager.GetInstance.model.user_info.town_hall_level >= csvData.UnlockTownHallLevel;				
		}else if ("SPELLS".Equals(csvData.TID_Type)){
			//return  DataManager.GetInstance.model.user_info.laboratory_level >= csvData.UpgradeHouseLevel && DataManager.GetInstance.model.user_info.town_hall_level >= csvData.UnlockTownHallLevel;
			return  DataManager.GetInstance.model.user_info.town_hall_level >= csvData.UnlockTownHallLevel;	
		}else if ("TRAPS".Equals(csvData.TID_Type)){
			return  true;//DataManager.GetInstance.model.user_info.laboratory_level >= csvData.UpgradeHouseLevel;
		}else if ("DECOS".Equals(csvData.TID_Type)){
			return false;			
		}else{
			return false;
		}
		
	}

	/*获取指定建筑物的当前(已建)数量;不含building_id;*/
	public static int getBuildingCurCount(string tid, BuildInfo s = null){
		int num = 0;
		foreach(BuildInfo s2 in DataManager.GetInstance.buildList.Values){
			if (tid.Equals(s2.tid) && s2 != s){
				num += 1;
			}
		}
		return num;
	}
	
	
	/*获取指定建筑物的请允许我最大建筑数量;*/
	public static int getBuildingMaxCount(string tid, int town_hall_level = 0){
		int num = 0;
		tid = tid.ToUpper();
		if (town_hall_level == 0){
			town_hall_level = DataManager.GetInstance.model.user_info.town_hall_level;
		}

		if (town_hall_level == 0){
			town_hall_level = 1;
		}

		string tid_l = tid + "_1";
		//Debug.Log1(tid_l);
		CsvInfo csvData1 = (CsvInfo)CSVManager.GetInstance.csvTable[tid_l];
		if ("DECOS".Equals(csvData1.TID_Type)){
			num = 0; //
		}else{		
			string tid_level = "TID_BUILDING_PALACE_" + town_hall_level;
			//Debug.Log(tid_level);
			CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
			/*
			if (csvData == null){
				Debug.Log("csvData is null");
			}else{
				Debug.Log("csvData is not null");
			}*/

			//const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;	    
						
			FieldInfo fieldInfo = csvData.GetType().GetField(tid);
			if (fieldInfo != null){
				//Debug.Log("fieldInfo is not null:" + tid);
				num = (int)fieldInfo.GetValue(csvData);
			}else{
				Debug.Log("fieldInfo is null:" + tid);
			}
		}
		//Debug.Log1("tid:" + tid + ";getBuildingMaxCount:" + num);
		return num;
		//BuildingData s = (BuildingData)UserData.userGrid[building_id];	
	}	


	//获得忙碌的工人数;min_seconds最快完工的还剩时间
	public static int GetWorkeringCount(ref long building_id){
		int num = 0;
		//int seconds = 0;	
		int end_time = 0;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if (s.status  == BuildStatus.New || s.status == BuildStatus.Removal || s.status == BuildStatus.Upgrade){
				if (end_time == 0 || s.end_time < end_time){
					end_time = s.end_time;
					building_id = s.building_id;
				}
				if (s.end_time - current_time() > 0){
					num += 1;
				}
			}
		}
		return num;
	}

	//判断当前采集器是否已经满了;
	//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
	public static int CollectStatus(BuildInfo s){
		int collect_num = getCollectNum(s);
		
		if (s.tid == "TID_BUILDING_HOUSING"){
			//TID_BUILDING_HOUSING
			if (DataManager.GetInstance.model.user_info.gold_count + collect_num > DataManager.GetInstance.model.user_info.max_gold_count){
				return 3;
			}
		}else if (s.tid == "TID_BUILDING_STONE_QUARRY"){
			if (DataManager.GetInstance.model.user_info.stone_count + collect_num > DataManager.GetInstance.model.user_info.max_stone_count){
				return 3;
			}
		}else if (s.tid == "TID_BUILDING_METAL_MINE"){
			if (DataManager.GetInstance.model.user_info.iron_count + collect_num > DataManager.GetInstance.model.user_info.max_iron_count){
				return 3;
			}
		}else if (s.tid == "TID_BUILDING_WOODCUTTER"){
			if (DataManager.GetInstance.model.user_info.wood_count + collect_num > DataManager.GetInstance.model.user_info.max_wood_count){
				return 3;
			}
		}else{
			return - 1;
		}
		
		if (collect_num >= s.csvInfo.ResourceMax){
			return 2;
		}else if(collect_num > 5){
			return 1;
		}else{
			return 0;
		}
	}

	//获得可采集的数量;
	public static int getCollectNum(BuildInfo s, int collect_time=0,bool show_log = false){
		//当点：加速时,会先执行一次采集,所以 s.last_collect_time >= s.boost_start_time
		
		//BuildInfo s = (BuildInfo)DataManager.GetInstance.BuildList[building_id];
		if (s == null){
			//Debug.Log1("s is null getCollectNum:building_id:" + building_id);
			return 0;
		}
		CsvInfo csvData = s.csvInfo;// (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
		
		if (collect_time == 0){
			collect_time = current_time();
		}




		int collect_num = 0;
		ArtifactType artifact_type = ArtifactType.None;
		int rph = csvData.ResourcePerHour;
		//获得神像加成值;
		//artifact_type:神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage;
		//orgValue基础值;
		if (s.tid == "TID_BUILDING_HOUSING"){
			artifact_type = ArtifactType.BoostGold;
		}else if (s.tid == "TID_BUILDING_WOODCUTTER"){
			artifact_type = ArtifactType.BoostWood;
		}else if (s.tid == "TID_BUILDING_STONE_QUARRY"){
			artifact_type = ArtifactType.BoostStone;
		}else if (s.tid == "TID_BUILDING_METAL_MINE"){
			artifact_type = ArtifactType.BoostMetal;
		}
		int artifact_num = getArtifactBoost(csvData.ResourcePerHour, artifact_type);
		rph += artifact_num;

		collect_num = (int)(((float)(collect_time - s.last_collect_time)/3600.0f) * rph);
		/*
		//Debug.Log1("collect_num:" + collect_num + ";loot_count:" + s.loot_count);
		collect_num = collect_num - s.loot_count;//被掠夺的数量（可采集的数量=未采集的数量-被掠夺的数量)，采集后要清空;
		if (collect_num < 0)
			collect_num = 0;
		*/
		if (show_log)
			Debug.Log("csvData.ResourcePerHour:" + csvData.ResourcePerHour + ";artifact_num:" + artifact_num + ";rph:" + rph + ";collect_num:" + collect_num);

		if (collect_num > csvData.ResourceMax){
			return csvData.ResourceMax;
		}else{
			return collect_num;
		}
		
	}

	public static void addItem(Dictionary<string,CsvInfo> items,string tid_level){
		//Debug.Log(tid_level);
		CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		items.Add(csvData.TID,csvData);
	}
	//shop_type: Economy, Defense,Support
	public static Dictionary<string,CsvInfo> getShopItem(string shop_type){
		//Debug.Log1("shop_type:" + shop_type);
		Dictionary<string,CsvInfo> items = new Dictionary<string,CsvInfo>();
		if ("Economy".Equals(shop_type)){
			addItem(items,"TID_BUILDING_HOUSING_1");//金矿;
			addItem(items,"TID_BUILDING_GOLD_STORAGE_1");//金库;
			addItem(items,"TID_BUILDING_WOOD_STORAGE_1");//木材库;
			addItem(items,"TID_BUILDING_STONE_STORAGE_1");//石头库;
			addItem(items,"TID_BUILDING_METAL_STORAGE_1");//钢材库;
			addItem(items,"TID_BUILDING_VAULT_1");//地下仓库;
			addItem(items,"TID_BUILDING_STONE_QUARRY_1");//凿石场;
			addItem(items,"TID_BUILDING_METAL_MINE_1");//钢材厂;
		}else if ("Support".Equals(shop_type)){

			addItem(items,"TID_BUILDING_LANDING_SHIP_1");//登陆艇;
			addItem(items,"TID_BUILDING_MAP_ROOM_1");//雷达;
			addItem(items,"TID_BUILDING_LABORATORY_1");//作战学院/实验室;
			addItem(items,"TID_BUILDING_ARTIFACT_WORKSHOP_1");//雕塑家;				
		}else if ("Defense".Equals(shop_type)){

			addItem(items,"TID_GUARD_TOWER_1");//箭塔;
			addItem(items,"TID_TRAP_MINE_1");//地雷;
			addItem(items,"TID_BUILDING_MORTAR_1");	//远程火炮;	
			addItem(items,"TID_MACHINE_GUN_NEST_1");//中程火炮;
			addItem(items,"TID_BUILDING_CANNON_1");//大炮;
			addItem(items,"TID_FLAME_THROWER_1");//火焰喷射器;
			addItem(items,"TID_TRAP_TANK_MINE_1");//地雷箱;
			addItem(items,"TID_BUILDING_BIG_BERTHA_1");//巨炮;
			addItem(items,"TID_MISSILE_LAUNCHER_1");//火箭/导弹发射器;

		}
		return items;
	}

	//获得神像加成值;
	//artifact_type:神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage;
	//orgValue基础值;
	public static int getArtifactBoost(int orgValue, ArtifactType artifact_type){
		int boost = 0;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if (s.csvInfo.BuildingClass == "Artifact" && s.artifact_boost > 0 && 
			    ((s.artifact_type == artifact_type) || 

			 ((artifact_type == ArtifactType.BoostGold || artifact_type == ArtifactType.BoostWood || artifact_type == ArtifactType.BoostMetal || artifact_type == ArtifactType.BoostStone )
			  && s.artifact_type == ArtifactType.BoostAllResources
			 )))
			{

				boost += (int)(orgValue * (s.artifact_boost / 100f));//向下取整;
				//Debug.Log("orgValue:" + orgValue + ";s.artifact_boost:" + s.artifact_boost + ";boost:" + boost);
			}
		}

		//Debug.Log("boost:" + boost);
		return boost;
	}



	//=========================================自动分配库存代码  begin =====================================================

	//将资源分配到各个资源仓库中;
	public static void autoAllocGoldStored(int stored_count = 0){
		//Debug.Log("autoAllocGoldStored");

		if (stored_count == 0)
			stored_count = DataManager.GetInstance.model.user_info.gold_count;

		//清空当前分配的;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_GOLD_STORAGE".Equals(s.tid) || "TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				s.gold_stored = 0;

				if ("TID_BUILDING_GOLD_STORAGE".Equals(s.tid)){
					s.SetStorage(s.gold_stored,tmpcsv.MaxStoredResourceGold);
				}
			}
		}

		//如果传入的参数大于用户最大可存储量，则使用最大可存储量;
		if (stored_count > DataManager.GetInstance.model.user_info.max_gold_count){
			stored_count = DataManager.GetInstance.model.user_info.max_gold_count;	
		}


		//将资源最先分配给地下仓库
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceGold){
					s.gold_stored = tmpcsv.MaxStoredResourceGold;
				}else{
					s.gold_stored = stored_count;								
				}
				stored_count -= s.gold_stored;	
				break;
			}
		}

		//将地下仓库分配剩余的资源分配给城镇中心;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_PALACE".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceGold){
					s.gold_stored = tmpcsv.MaxStoredResourceGold;
				}else{
					s.gold_stored = stored_count;								
				}
				stored_count -= s.gold_stored;	
				break;
			}
		}

		//将城镇中心分配剩余的资源平均分配到金库中;
		if (stored_count > 0)
			allocGoldStored(stored_count);
	}

	//除了，HQ，Vault外的 资源下平均分配到：Gold Storage;
	private static void allocGoldStored(int stored_gold){
		int num = 0;		
		//int over_stored = 0;
		
		//Debug.Log1("stored_gold1:" + stored_gold);
		
		//获取未装满数据;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_GOLD_STORAGE".Equals(s.tid) && s.status != BuildStatus.New){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				int max_stored = tmpcsv.MaxStoredResourceGold;
				if (max_stored > s.gold_stored){
					num += 1;
				}				
			}
		}
		//Debug.Log1("num:" + num);
		
		if (num > 0){
			//平均分配到各个仓库中;
			int avg = stored_gold / num;
			if (avg == 0){
				//不够下平均分时;
				avg = 1;
			}
			
			//Debug.Log1("avg:" + avg);
			
			foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
				//不是新建中的;
				if ("TID_BUILDING_GOLD_STORAGE".Equals(s.tid) && stored_gold > 0 && s.status != BuildStatus.New){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					int max_stored = tmpcsv.MaxStoredResourceGold;
					
					//可装的数量;					
					if (s.gold_stored + avg <= max_stored){
						stored_gold -= avg;
						s.gold_stored += avg;
					}else{
						if (max_stored > s.gold_stored){
							stored_gold -= max_stored - s.gold_stored;
							s.gold_stored += max_stored - s.gold_stored;
						}
					}

					s.SetStorage(s.gold_stored,max_stored);
				}
			}	
			
			//Debug.Log1("stored_gold2:" + stored_gold);
			//未分配的;
			if (stored_gold > 0 && num > 1){
				allocGoldStored(stored_gold);
			}
		}
	}

	//=======================================================================================================

	//将资源分配到各个资源仓库中;
	public static void autoAllocWoodStored(int stored_count = 0){
		//Debug.Log("autoAllocGoldStored");
		
		if (stored_count == 0)
			stored_count = DataManager.GetInstance.model.user_info.wood_count;
		
		//清空当前分配的;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_WOOD_STORAGE".Equals(s.tid) || "TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				s.wood_stored = 0;

				if ("TID_BUILDING_WOOD_STORAGE".Equals(s.tid)){
					s.SetStorage(s.wood_stored,tmpcsv.MaxStoredResourceWood);
				}
			}
		}
		
		//如果传入的参数大于用户最大可存储量，则使用最大可存储量;
		if (stored_count > DataManager.GetInstance.model.user_info.max_wood_count){
			stored_count = DataManager.GetInstance.model.user_info.max_wood_count;	
		}
		
		
		//将资源最先分配给地下仓库;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceWood){
					s.wood_stored = tmpcsv.MaxStoredResourceWood;
				}else{
					s.wood_stored = stored_count;								
				}
				stored_count -= s.wood_stored;	
				break;
			}
		}
		
		//将地下仓库分配剩余的资源分配给城镇中心;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			//Debug.Log("xxxx");
			if ("TID_BUILDING_PALACE".Equals(s.tid)){
				//Debug.Log("TID_BUILDING_PALACE");
				//Debug.Log(s.tid_level);
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceWood){
					s.wood_stored = tmpcsv.MaxStoredResourceWood;
				}else{
					s.wood_stored = stored_count;								
				}
				stored_count -= s.wood_stored;	
				break;
			}
		}
		
		//将城镇中心分配剩余的资源平均分配到金库中;
		if (stored_count > 0)
			allocWoodStored(stored_count);
	}
	
	//除了，HQ，Vault外的 资源下平均分配到：Gold Storage;
	private static void allocWoodStored(int stored_wood){
		int num = 0;		
		//int over_stored = 0;
		
		//Debug.Log1("stored_wood1:" + stored_wood);
		
		//获取未装满数据;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_WOOD_STORAGE".Equals(s.tid) && s.status != BuildStatus.New){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				int max_stored = tmpcsv.MaxStoredResourceWood;
				if (max_stored > s.wood_stored){
					num += 1;
				}				
			}
		}
		//Debug.Log1("num:" + num);
		
		if (num > 0){
			//平均分配到各个仓库中;
			int avg = stored_wood / num;
			if (avg == 0){
				//不够下平均分时;
				avg = 1;
			}
			
			//Debug.Log1("avg:" + avg);
			
			foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
				if ("TID_BUILDING_WOOD_STORAGE".Equals(s.tid) && stored_wood > 0 && s.status != BuildStatus.New){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					int max_stored = tmpcsv.MaxStoredResourceWood;
					
					//可装的数量;					
					if (s.wood_stored + avg <= max_stored){
						stored_wood -= avg;
						s.wood_stored += avg;
					}else{
						if (max_stored > s.wood_stored){
							stored_wood -= max_stored - s.wood_stored;
							s.wood_stored += max_stored - s.wood_stored;
						}
					}

					s.SetStorage(s.wood_stored,max_stored);
				}
			}	
			
			//Debug.Log1("stored_wood2:" + stored_wood);
			//未分配的;
			if (stored_wood > 0 && num > 1){
				allocWoodStored(stored_wood);
			}
		}
	}
	
	//=======================================================================================================
	//将资源分配到各个资源仓库中;
	public static void autoAllocStoneStored(int stored_count = 0){
		//Debug.Log("autoAllocGoldStored");
		
		if (stored_count == 0)
			stored_count = DataManager.GetInstance.model.user_info.stone_count;
		
		//清空当前分配的;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_STONE_STORAGE".Equals(s.tid) || "TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				s.stone_stored = 0;

				if ("TID_BUILDING_STONE_STORAGE".Equals(s.tid)){
					s.SetStorage(s.stone_stored,tmpcsv.MaxStoredResourceStone);
				}
			}
		}
		
		//如果传入的参数大于用户最大可存储量，则使用最大可存储量;
		if (stored_count > DataManager.GetInstance.model.user_info.max_stone_count){
			stored_count = DataManager.GetInstance.model.user_info.max_stone_count;	
		}
		
		
		//将资源最先分配给地下仓库
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceStone){
					s.stone_stored = tmpcsv.MaxStoredResourceStone;
				}else{
					s.stone_stored = stored_count;								
				}
				stored_count -= s.stone_stored;	
				break;
			}
		}
		
		//将地下仓库分配剩余的资源分配给城镇中心;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_PALACE".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceStone){
					s.stone_stored = tmpcsv.MaxStoredResourceStone;
				}else{
					s.stone_stored = stored_count;								
				}
				stored_count -= s.stone_stored;	
				break;
			}
		}
		
		//将城镇中心分配剩余的资源平均分配到金库中;
		if (stored_count > 0)
			allocStoneStored(stored_count);
	}
	
	//除了，HQ，Vault外的 资源下平均分配到：Stone Storage;
	private static void allocStoneStored(int stored_stone){
		int num = 0;		
		//int over_stored = 0;
		
		//Debug.Log1("stored_stone1:" + stored_stone);
		
		//获取未装满数据;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_STONE_STORAGE".Equals(s.tid) && s.status != BuildStatus.New){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				int max_stored = tmpcsv.MaxStoredResourceStone;
				if (max_stored > s.stone_stored){
					num += 1;
				}				
			}
		}
		//Debug.Log1("num:" + num);
		
		if (num > 0){
			//平均分配到各个仓库中;
			int avg = stored_stone / num;
			if (avg == 0){
				//不够下平均分时;
				avg = 1;
			}
			
			//Debug.Log1("avg:" + avg);
			
			foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
				if ("TID_BUILDING_STONE_STORAGE".Equals(s.tid) && stored_stone > 0 && s.status != BuildStatus.New){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					int max_stored = tmpcsv.MaxStoredResourceStone;
					
					//可装的数量;					
					if (s.stone_stored + avg <= max_stored){
						stored_stone -= avg;
						s.stone_stored += avg;
					}else{
						if (max_stored > s.stone_stored){
							stored_stone -= max_stored - s.stone_stored;
							s.stone_stored += max_stored - s.stone_stored;
						}
					}

					s.SetStorage(s.stone_stored,max_stored);
				}
			}	
			
			//Debug.Log1("stored_stone2:" + stored_stone);
			//未分配的;
			if (stored_stone > 0 && num > 1){
				allocStoneStored(stored_stone);
			}
		}
	}
	
	//=======================================================================================================

	
	//将资源分配到各个资源仓库中;
	public static void autoAllocMetalStored(int stored_count = 0){
		//Debug.Log("autoAllocMetalStored");
		
		if (stored_count == 0)
			stored_count = DataManager.GetInstance.model.user_info.iron_count;
		
		//清空当前分配的;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_METAL_STORAGE".Equals(s.tid) || "TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				s.metal_stored = 0;
				if ("TID_BUILDING_METAL_STORAGE".Equals(s.tid)){
					s.SetStorage(s.metal_stored,tmpcsv.MaxStoredResourceIron);
				}
			}
		}
		
		//如果传入的参数大于用户最大可存储量，则使用最大可存储量;
		if (stored_count > DataManager.GetInstance.model.user_info.max_iron_count){
			stored_count = DataManager.GetInstance.model.user_info.max_iron_count;	
		}
		
		
		//将资源最先分配给地下仓库
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_VAULT".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceIron){
					s.metal_stored = tmpcsv.MaxStoredResourceIron;
				}else{
					s.metal_stored = stored_count;								
				}
				stored_count -= s.metal_stored;	
				break;
			}
		}
		
		//将地下仓库分配剩余的资源分配给城镇中心;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_PALACE".Equals(s.tid)){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				if (stored_count > tmpcsv.MaxStoredResourceIron){
					s.metal_stored = tmpcsv.MaxStoredResourceIron;
				}else{
					s.metal_stored = stored_count;								
				}
				stored_count -= s.metal_stored;	
				break;
			}
		}
		
		//将城镇中心分配剩余的资源平均分配到金库中;
		if (stored_count > 0)
			allocMetalStored(stored_count);
	}
	
	//除了，HQ，Vault外的 资源下平均分配到：Metal Storage;
	private static void allocMetalStored(int stored_metal){
		int num = 0;		
		//int over_stored = 0;
		
		//Debug.Log1("stored_metal1:" + stored_metal);
		
		//获取未装满数据;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("TID_BUILDING_METAL_STORAGE".Equals(s.tid) && s.status != BuildStatus.New){
				CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
				int max_stored = tmpcsv.MaxStoredResourceIron;
				if (max_stored > s.metal_stored){
					num += 1;
				}				
			}
		}
		//Debug.Log1("num:" + num);
		
		if (num > 0){
			//平均分配到各个仓库中;
			int avg = stored_metal / num;
			if (avg == 0){
				//不够下平均分时;
				avg = 1;
			}
			
			//Debug.Log1("avg:" + avg);
			
			foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
				if ("TID_BUILDING_METAL_STORAGE".Equals(s.tid) && stored_metal > 0 && s.status != BuildStatus.New){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					int max_stored = tmpcsv.MaxStoredResourceIron;
					
					//可装的数量;					
					if (s.metal_stored + avg <= max_stored){
						stored_metal -= avg;
						s.metal_stored += avg;
					}else{
						if (max_stored > s.metal_stored){
							stored_metal -= max_stored - s.metal_stored;
							s.metal_stored += max_stored - s.metal_stored;
						}
					}

					s.SetStorage(s.metal_stored,max_stored);
				}
			}	
			
			//Debug.Log1("stored_metal2:" + stored_metal);
			//未分配的;
			if (stored_metal > 0 && num > 1){
				allocMetalStored(stored_metal);
			}
		}
	}



	//=========================================自动分配库存代码  end =====================================================


	/*获得可建筑的坐标;*/
	public static Vector3 getBlankXY(int grid_count = 0){
		
		Vector3 ScreenCenterPoint = Vector3.zero;
		ScreenCenterPoint = new Vector3(Screen.width * 0.5f,Screen.height * 0.5f, 0);
		Ray ray = Camera.main.ViewportPointToRay(Camera.main.ScreenToViewportPoint(ScreenCenterPoint));
		RaycastHit hit;	
		
		Vector2 gameCoor;//
		
		if (Physics.Raycast(ray, out hit,1000f,1<<8))
		{
			GridInfo gi = Globals.LocateGridInfo(hit.point);
			gameCoor = new Vector2(gi.A,gi.B);
		}else{
			gameCoor = new Vector2(20,20);
		}
		
		//Debug.Log(gameCoor.x+"_"+gameCoor.y);
		if (grid_count > 0){
			//List<String> gridList = new ArrayList<String>();
			//将当前空地方添加到列表中;
			ArrayList gridList = new ArrayList();
			for(int i = 0; i < 50; i ++){
				for(int j = 0; j < 50; j ++){
					GridInfo currentGridInfo = Globals.GridArray[i,j];
					if (!currentGridInfo.isBuild)
						gridList.Add(i + "_" + j);
				}
			}
			int x = (int)gameCoor.x;
			int y = (int)gameCoor.y;
			
			//以x,y为中心,在这四周找一个可以放下grid_count格式数的空地方;
			for(int i = 0; i < 8; i ++){
				for(int j = 0; j < 8; j ++){
					int new_x = x + i;
					int new_y = y + j;
					if (is_blank(new_x,new_y,grid_count,gridList)){
						//Debug.Log(new_x+"_"+new_y);
						return new Vector3(new_x,0f,new_y);
					}
					
					new_x = x + i;
					new_y = y - j;
					if (is_blank(new_x,new_y,grid_count,gridList)){
						//Debug.Log(new_x+"_"+new_y);
						return new Vector3(new_x,0f,new_y);
					}	
					
					new_x = x - i;
					new_y = y + j;
					if (is_blank(new_x,new_y,grid_count,gridList)){
						//Debug.Log(new_x+"_"+new_y);
						return new Vector3(new_x,0f,new_y);
					}
					
					new_x = x - i;
					new_y = y - j;
					if (is_blank(new_x,new_y,grid_count,gridList)){
						//Debug.Log(new_x+"_"+new_y);
						return new Vector3(new_x,0f,new_y);
					}					
				}
			}
			
			return new Vector3(gameCoor.x,0f,gameCoor.y);
		}else{
			return new Vector3(gameCoor.x,0f,gameCoor.y);
		}			
	}
	
	//可以存放返回：true,否则返回:false
	public static bool is_blank(int x, int y, int w_h, ArrayList gridList){
		int w_h_x = x + w_h;
		int w_h_y = y + w_h;
		
		if (w_h_x >= 50 || w_h_y >= 50){
			//trace("is_blank1: " + w_h_x + "_" + w_h_y);
			return false;
		}else{
			for(int i = x; i < w_h_x; i ++){
				for(int j = y; j < w_h_y; j ++){
					//去除已经被占用的格子
					if (!gridList.Contains(i + "_" + j)){
						//如果其中有格式被占用，返回不可用;
						//trace("is_blank2: " + i + "_" + j);
						return false;
					}					
				}
			}
			//trace("is_blank3: " + x + "_" + y + "_w_h:" + w_h);
			return true;			
		}
	}


	//type = 1; 升级; 2:新建; 3:探索
	//判断用户资源是否足够,如果资源不足，使用宝石补差余;
	//当前返回：Gems > 0时，说明资源不足,需要使用宝石补差余;
	//showAll返回所有资源类型字段;
	public static ISFSObject getCostDiffToGems(string tid_level, int type, bool showAll, int gold_cost = 0){
		Hashtable ht = new Hashtable();
		int gems = 0;
		int diff_count = 0;//相差资源类型个数;
		BuildCost bc = null;
		if (type== 1)
			bc = getUpgradeCost(tid_level);
		else if (type== 0)
			bc = GetBuildCost(tid_level);
		else if (type == 3){
			bc = new BuildCost();
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.gold = gold_cost;
		}
		else{
			bc = GetBuildCost(tid_level);
			bc.wood = 0;
			bc.stone = 0;
			bc.iron = 0;
			bc.gold = gold_cost;
		}
		//bc = getBuildCost(tid_level);


		//Debug.Log(DataManager.GetInstance.model.user_info.wood_count);
		int diff_wood = bc.wood - DataManager.GetInstance.model.user_info.wood_count;
		if (diff_wood > 0){
			diff_count ++;
			ht["TID_WOOD"] = diff_wood;
			gems += CalcHelper.doCalcResourceToGems(diff_wood);
		}
		
		int diff_stone = bc.stone - DataManager.GetInstance.model.user_info.stone_count;
		if (diff_stone > 0){
			diff_count ++;
			ht["TID_STONE"] = diff_stone;
			gems += CalcHelper.doCalcResourceToGems(diff_stone);
		}

		int diff_iron = bc.iron - DataManager.GetInstance.model.user_info.iron_count;
		if (diff_iron > 0){
			diff_count ++;
			ht["TID_METAL"] = diff_iron;
			gems += CalcHelper.doCalcResourceToGems(diff_iron);
		}

		int diff_gold = bc.gold - DataManager.GetInstance.model.user_info.gold_count;
		if (diff_gold > 0){
			diff_count ++;
			ht["TID_GOLD"] = diff_gold;
			gems += CalcHelper.doCalcResourceToGems(diff_gold);
		}
	
		//=======================================
		if (bc.wood > 0 || showAll){
			if (diff_wood > 0){				
				ht["Wood"] = DataManager.GetInstance.model.user_info.wood_count;
			}else{
				ht["Wood"] = bc.wood;
			}
		}
		
		if (bc.stone > 0 || showAll){
			if (diff_stone > 0){				
				ht["Stone"] = DataManager.GetInstance.model.user_info.stone_count;
			}else{
				ht["Stone"] = bc.stone;
			}
		}
		
		if (bc.iron > 0 || showAll){
			if (diff_stone > 0){				
				ht["Iron"] = DataManager.GetInstance.model.user_info.iron_count;
			}else{
				ht["Iron"] = bc.iron;
			}
		}
		
		if (bc.gold > 0 || showAll){
			if (diff_gold > 0){				
				ht["Gold"] = DataManager.GetInstance.model.user_info.gold_count;
			}else{
				ht["Gold"] = bc.gold;
			}
		}

		//==============================

		ht["diff_count"] = diff_count;
		ht["Gems"] = gems;

		/*
			 * 
			TID_BUY_MISSING_RESOURCES_HEADER	Need more <resource1>	
			TID_BUY_MISSING_RESOURCES_TEXT	Buy the missing <count1> <resource1>?	
			TID_BUY_MISSING_RESOURCES_X2_HEADER	You need more resources	
			TID_BUY_MISSING_RESOURCES_X2_TEXT	Buy the missing <count1> <resource1> and <count2> <resource2>?	
			TID_BUY_MISSING_RESOURCES_X3_HEADER	You need more resources	
			TID_BUY_MISSING_RESOURCES_X3_TEXT	Buy the missing <count1> <resource1>, <count2> <resource2> and <count3> <resource3>?	

			TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_HEADER	Spend Diamonds	
			TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_TEXT	Complete the current building and buy the missing <count1> <resource1>?	
			TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_X2_HEADER	Spend Diamonds	
			TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_X2_TEXT	Complete the current building and buy the missing <count1> <resource1> and <count2> <resource2>?	
			TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_X3_HEADER	Spend Diamonds	
			TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_X3_TEXT	Complete the current building and buy the missing <count1> <resource1>, <count2> <resource2> and <count3> <resource3>?	

			*/
		string msg = null;
		string title = null;
		if (diff_count == 1){
			if (type == 0){
				title = LocalizationCustom.instance.Get("TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_HEADER");
				msg = LocalizationCustom.instance.Get("TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_TEXT");//建成当前的建筑并购买缺少的资源：;
			}else{
				title = LocalizationCustom.instance.Get("TID_BUY_MISSING_RESOURCES_X2_HEADER");
				msg = LocalizationCustom.instance.Get("TID_BUY_MISSING_RESOURCES_TEXT");//购买缺少的资源：;
			}

			//msg = msg + " <count1> <resource1>?";
		}else if (diff_count == 2){
			if (type == 0){				
				title = LocalizationCustom.instance.Get("TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_HEADER");
				msg = LocalizationCustom.instance.Get("TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_X2_TEXT");				
			}else{
				title = LocalizationCustom.instance.Get("TID_BUY_MISSING_RESOURCES_X2_HEADER");
				msg = LocalizationCustom.instance.Get("TID_BUY_MISSING_RESOURCES_X2_TEXT");
			}
			//msg = msg + " <count1> <resource1>,<count2> <resource2>?";
		}else if (diff_count == 3){
			if (type == 0){
				title = LocalizationCustom.instance.Get("TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_HEADER");
				msg = LocalizationCustom.instance.Get("TID_SPEED_UP_AND_BUY_MISSING_RESOURCES_X3_TEXT");				
			}else{
				title = LocalizationCustom.instance.Get("TID_BUY_MISSING_RESOURCES_X3_HEADER");
				msg = LocalizationCustom.instance.Get("TID_BUY_MISSING_RESOURCES_X3_TEXT");
			}
			//msg = msg + " <count1> <resource1>,<count2> <resource2>,<count3> <resource3>?";
		}
		
		ISFSObject dt = new SFSObject();

		int index = 1;
		foreach(string key in ht.Keys){
			int count = (int)ht[key];
			//Debug.Log(msg);
			if (key == "TID_WOOD" || key == "TID_STONE"  || key == "TID_METAL" || key == "TID_GOLD"){
				msg = msg.Replace("<count" + index.ToString() +">",count.ToString()).Replace("<resource" + index.ToString() +">", LocalizationCustom.instance.Get(key));
				//dt.PutInt(key,count);
				index++;
			}
			dt.PutInt(key,count);
		}

		dt.PutUtfString("title",title);
		dt.PutUtfString("msg",msg);

		return dt;
	}

	public static bool CheckResourceCount(string resource_type, int num){
		if(SetResourceCount(resource_type,num,true,false) < 0){
			return false;
		}
		return true;
	}

	/*return -1 药水或金额或宝石或黑药水不足;*/
	public static int SetResourceCount(string resource_type, int num, bool olny_chek = false, bool update_ui = true){
		if ("Gold".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.gold_count + num) < 0 ){
				return -1;
			}else{
				//检查容量是否超出;
				if (olny_chek){
					if (DataManager.GetInstance.model.user_info.gold_count + num > DataManager.GetInstance.model.user_info.max_gold_count){
						return -1;
					}else{
						return 1;
					}					
				}else{
					if (DataManager.GetInstance.model.user_info.gold_count + num > DataManager.GetInstance.model.user_info.max_gold_count){
						DataManager.GetInstance.model.user_info.gold_count = DataManager.GetInstance.model.user_info.max_gold_count;
					}else{
						DataManager.GetInstance.model.user_info.gold_count = DataManager.GetInstance.model.user_info.gold_count + num;
					}	
					
					autoAllocGoldStored(DataManager.GetInstance.model.user_info.gold_count);

					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.gold_count;
				}					
			}	
		}else if ("Wood".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.wood_count + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//检查容量是否超出;
					if (DataManager.GetInstance.model.user_info.wood_count + num > DataManager.GetInstance.model.user_info.max_wood_count){
						return -1;
					}else{
						return 1;
					}
				}else{		
					if (DataManager.GetInstance.model.user_info.wood_count + num > DataManager.GetInstance.model.user_info.max_wood_count){
						DataManager.GetInstance.model.user_info.wood_count = DataManager.GetInstance.model.user_info.max_wood_count;
					}else{
						DataManager.GetInstance.model.user_info.wood_count = DataManager.GetInstance.model.user_info.wood_count + num;
					}
					
					autoAllocWoodStored(DataManager.GetInstance.model.user_info.wood_count);
					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.wood_count;
				}	
			}	
		}else if ("Stone".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.stone_count + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//检查容量是否超出;
					if (DataManager.GetInstance.model.user_info.stone_count + num > DataManager.GetInstance.model.user_info.max_stone_count){
						return -1;
					}else{
						return 1;
					}
				}else{		
					if (DataManager.GetInstance.model.user_info.stone_count + num > DataManager.GetInstance.model.user_info.max_stone_count){
						DataManager.GetInstance.model.user_info.stone_count = DataManager.GetInstance.model.user_info.max_stone_count;
					}else{
						DataManager.GetInstance.model.user_info.stone_count = DataManager.GetInstance.model.user_info.stone_count + num;
					}
					
					autoAllocStoneStored(DataManager.GetInstance.model.user_info.stone_count);

					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.stone_count;
				}	
			}
		}else if ("Iron".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.iron_count + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//检查容量是否超出;
					if (DataManager.GetInstance.model.user_info.iron_count + num > DataManager.GetInstance.model.user_info.max_iron_count){
						return -1;
					}else{
						return 1;
					}
				}else{		
					if (DataManager.GetInstance.model.user_info.iron_count + num > DataManager.GetInstance.model.user_info.max_iron_count){
						DataManager.GetInstance.model.user_info.iron_count = DataManager.GetInstance.model.user_info.max_iron_count;
					}else{
						DataManager.GetInstance.model.user_info.iron_count = DataManager.GetInstance.model.user_info.iron_count + num;
					}
					
					autoAllocMetalStored(DataManager.GetInstance.model.user_info.iron_count);

					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.iron_count;
				}	
			}		
		}else if ("Gems".Equals(resource_type) || "Diamonds".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.diamond_count + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.diamond_count + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.diamond_count = DataManager.GetInstance.model.user_info.diamond_count + num;														
					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.diamond_count;
				}
			}	
		}else if ("Exp".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.exp_count + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					return 1;
				}else{				


					DataManager.GetInstance.model.user_info.exp_count = DataManager.GetInstance.model.user_info.exp_count + num;	

					ExperienceLevels el = CSVManager.GetInstance.experienceLevelsList[DataManager.GetInstance.model.user_info.exp_level.ToString()] as ExperienceLevels;
					int max_exp = 0;
					if (el == null){
						Debug.Log(CSVManager.GetInstance.experienceLevelsList.Count + ";exp_level:" + DataManager.GetInstance.model.user_info.exp_level + ";num:" + num);

						Debug.Log(CSVManager.GetInstance.experienceLevelsList.ContainsKey(DataManager.GetInstance.model.user_info.exp_level.ToString()));

							/*
						foreach(string key in CSVMananger.GetInstance.experienceLevelsList.Keys){
							ExperienceLevels el2 = CSVMananger.GetInstance.experienceLevelsList[key] as ExperienceLevels;
							
							Debug.Log("level:" + key + ";name:" + el2.Name + ";:" + el2.ExpPoints);
						}
*/
						max_exp = 9999;
						//获取某等级需要多少经验值;
						// CalcHelper.calcExperience(DataManager.GetInstance.model.user_info.exp_level + 1);
					}else{
						max_exp = el.ExpPoints;
					}
					//升一级;
					if (DataManager.GetInstance.model.user_info.exp_count >= max_exp){
						DataManager.GetInstance.model.user_info.exp_level += 1;
						DataManager.GetInstance.model.user_info.exp_count -= max_exp;

						AudioPlayer.Instance.PlaySfx("xp_level_up_01");
					}
					

					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.exp_count;
				}
			}	
		}else if ("Reward".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.reward_count + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					return 1;
				}else{				
					DataManager.GetInstance.model.user_info.reward_count = DataManager.GetInstance.model.user_info.reward_count + num;														
					if (update_ui){
						UpdateResUI(resource_type,true);
					}
					return DataManager.GetInstance.model.user_info.reward_count;
				}
			}
		}else if ("CommonPiece".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.common_piece + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.common_piece + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.common_piece = DataManager.GetInstance.model.user_info.common_piece + num;
					return DataManager.GetInstance.model.user_info.common_piece;
				}
			}
		}else if ("RarePiece".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.rare_piece + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.rare_piece + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.rare_piece = DataManager.GetInstance.model.user_info.rare_piece + num;
					return DataManager.GetInstance.model.user_info.rare_piece;
				}
			}
		}else if ("EpicPiece".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.epic_piece + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.epic_piece + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.epic_piece = DataManager.GetInstance.model.user_info.epic_piece + num;
					return DataManager.GetInstance.model.user_info.epic_piece;
				}
			}
		
		}else if ("CommonPieceIce".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.common_piece_ice + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.common_piece_ice + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.common_piece_ice = DataManager.GetInstance.model.user_info.common_piece_ice + num;
					return DataManager.GetInstance.model.user_info.common_piece_ice;
				}
			}
		}else if ("RarePieceIce".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.rare_piece_ice + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.rare_piece_ice + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.rare_piece_ice = DataManager.GetInstance.model.user_info.rare_piece_ice + num;
					return DataManager.GetInstance.model.user_info.rare_piece_ice;
				}
			}
		}else if ("EpicPieceIce".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.epic_piece_ice + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.epic_piece_ice + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.epic_piece_ice = DataManager.GetInstance.model.user_info.epic_piece_ice + num;
					return DataManager.GetInstance.model.user_info.epic_piece_ice;
				}
			}
			
		}else if ("CommonPieceFire".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.common_piece_fire + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.common_piece_fire + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.common_piece_fire = DataManager.GetInstance.model.user_info.common_piece_fire + num;
					return DataManager.GetInstance.model.user_info.common_piece_fire;
				}
			}
		}else if ("RarePieceFire".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.rare_piece_fire + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.rare_piece_fire + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.rare_piece_fire = DataManager.GetInstance.model.user_info.rare_piece_fire + num;
					return DataManager.GetInstance.model.user_info.rare_piece_fire;
				}
			}
		}else if ("EpicPieceFire".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.epic_piece_fire + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.epic_piece_fire + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.epic_piece_fire = DataManager.GetInstance.model.user_info.epic_piece_fire + num;
					return DataManager.GetInstance.model.user_info.epic_piece_fire;
				}
			}
			
		}else if ("CommonPieceDark".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.common_piece_dark + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.common_piece_dark + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.common_piece_dark = DataManager.GetInstance.model.user_info.common_piece_dark + num;
					return DataManager.GetInstance.model.user_info.common_piece_dark;
				}
			}
		}else if ("RarePieceDark".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.rare_piece_dark + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.rare_piece_dark + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.rare_piece_dark = DataManager.GetInstance.model.user_info.rare_piece_dark + num;
					return DataManager.GetInstance.model.user_info.rare_piece_dark;
				}
			}
		}else if ("EpicPieceDark".Equals(resource_type)){
			if (num < 0 && (DataManager.GetInstance.model.user_info.epic_piece_dark + num) < 0 ){
				return -1;
			}else{
				if (olny_chek){
					//资源是否足够;
					if (DataManager.GetInstance.model.user_info.epic_piece_dark + num < 0){
						return -1;
					}else{
						return 1;
					}
				}else{				
					DataManager.GetInstance.model.user_info.epic_piece_dark = DataManager.GetInstance.model.user_info.epic_piece_dark + num;
					return DataManager.GetInstance.model.user_info.epic_piece_dark;
				}
			}			
		}else{
			return -2;
		}			
	}

	//更新主界面用户资源数量显示;
	public static void UpdateResUI(string resource_type, bool playAnim = false){
		MainInterfaceCtrl mainInterfaceCtrl = BoomBeach.UIManager.GetInstance.GetController<MainInterfaceCtrl>();
		if ("Gold".Equals(resource_type) || "All".Equals(resource_type)){
			//ScreenUIManage.Instance.data.GoldCurrent = DataManager.GetInstance.model.user_info.gold_count;
			//ScreenUIManage.Instance.data.GoldStorageCapacity = DataManager.GetInstance.model.user_info.max_gold_count;
			//ScreenUIManage.Instance.UpdateGoldResource(playAnim);

			mainInterfaceCtrl.data.GoldCurrent = DataManager.GetInstance.model.user_info.gold_count;
			mainInterfaceCtrl.data.GoldStorageCapacity = DataManager.GetInstance.model.user_info.max_gold_count;
			mainInterfaceCtrl.UpdateGoldResource(playAnim);

		}
		if ("Wood".Equals(resource_type) || "All".Equals(resource_type)){
			//ScreenUIManage.Instance.data.WoodCurrent = DataManager.GetInstance.model.user_info.wood_count;
			//ScreenUIManage.Instance.data.WoodStorageCapacity = DataManager.GetInstance.model.user_info.max_wood_count;
			//ScreenUIManage.Instance.UpdateWoodResource(playAnim);

			mainInterfaceCtrl.data.WoodCurrent = DataManager.GetInstance.model.user_info.wood_count;
			mainInterfaceCtrl.data.WoodStorageCapacity = DataManager.GetInstance.model.user_info.max_wood_count;
			mainInterfaceCtrl.UpdateWoodResource(playAnim);
		}
		if ("Stone".Equals(resource_type) || "All".Equals(resource_type)){
			//ScreenUIManage.Instance.data.StoneCurrent = DataManager.GetInstance.model.user_info.stone_count;
			//ScreenUIManage.Instance.data.StoneStorageCapacity = DataManager.GetInstance.model.user_info.max_stone_count;
			//ScreenUIManage.Instance.UpdateStoneResource(playAnim);

			mainInterfaceCtrl.data.StoneCurrent = DataManager.GetInstance.model.user_info.stone_count;
			mainInterfaceCtrl.data.StoneStorageCapacity = DataManager.GetInstance.model.user_info.max_stone_count;
			mainInterfaceCtrl.UpdateStoneResource(playAnim);
		}
		if ("Iron".Equals(resource_type) || "All".Equals(resource_type)){
			//ScreenUIManage.Instance.data.IronCurrent = DataManager.GetInstance.model.user_info.iron_count;
			//ScreenUIManage.Instance.data.IronStorageCapacity = DataManager.GetInstance.model.user_info.max_iron_count;
			//ScreenUIManage.Instance.UpdateIronResource(playAnim);

			mainInterfaceCtrl.data.IronCurrent = DataManager.GetInstance.model.user_info.iron_count;
			mainInterfaceCtrl.data.IronStorageCapacity = DataManager.GetInstance.model.user_info.max_iron_count;
			mainInterfaceCtrl.UpdateIronResource(playAnim);
		}

		if ("Gems".Equals(resource_type) || "Diamonds".Equals(resource_type) || "All".Equals(resource_type)){
			//ScreenUIManage.Instance.data.DiamondCurrent = DataManager.GetInstance.model.user_info.diamond_count;
			//ScreenUIManage.Instance.UpdateDiamondResource(playAnim);

			mainInterfaceCtrl.data.DiamondCurrent = DataManager.GetInstance.model.user_info.diamond_count;
			mainInterfaceCtrl.UpdateDiamondResource(playAnim);
		}

		if ("Exp".Equals(resource_type) || "All".Equals(resource_type)){
			//int max_exp = CalcHelper.calcExperience(DataManager.GetInstance.model.user_info.exp_level);
			ExperienceLevels el = CSVManager.GetInstance.experienceLevelsList[DataManager.GetInstance.model.user_info.exp_level.ToString()] as ExperienceLevels;
			if (el != null){
				int max_exp = el.ExpPoints;
				mainInterfaceCtrl.data.UserLevel = DataManager.GetInstance.model.user_info.exp_level;
				mainInterfaceCtrl.data.CurrentExp = DataManager.GetInstance.model.user_info.exp_count;
                mainInterfaceCtrl.data.UpgradeExp = max_exp;
                mainInterfaceCtrl.UpdateUserLevel(playAnim);
				if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.data.UserLevel = DataManager.GetInstance.model.user_info.exp_level;
				if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.data.CurrentExp = DataManager.GetInstance.model.user_info.exp_count;
                if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.data.UpgradeExp = max_exp;
                if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.UpdateUserLevel(playAnim);
			}
		}

		if ("Reward".Equals(resource_type) || "All".Equals(resource_type)){
			mainInterfaceCtrl.data.UserMedal = DataManager.GetInstance.model.user_info.reward_count;
            mainInterfaceCtrl.UpdateUserMedal();

			if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.data.UserMedal = DataManager.GetInstance.model.user_info.reward_count;
            if (ScreenUIManage.Instance != null) ScreenUIManage.Instance.UpdateUserMedal();
		}
	}

	//设置资源组(确保所有资源够时，调用);
	public static void SetResource(int gold = 0,
	                              int wood = 0,
	                              int stone = 0,
	                              int iron = 0,
	                              int gems = 0, bool update_ui = true){
		if (gold != 0){
			SetResourceCount("Gold",gold,false,update_ui);
		}

		if (wood != 0){
			SetResourceCount("Wood",wood,false,update_ui);
		}

		if (stone != 0){
			SetResourceCount("Stone",stone,false,update_ui);
		}

		if (iron != 0){
			SetResourceCount("Iron",iron,false,update_ui);
		}

		if (gems != 0){
			SetResourceCount("Gems",gems,false,update_ui);
		}
	}

	/*
	public static GameObject Create3DBuild(ISFSObject item, string tid = null, int level = 1, string cardName = "", string tabPageInfo = "")
	{
		if (item != null){
			tid = item.GetUtfString("tid");
			level = item.GetInt("level");
		}

		string tid_level = tid + "_" + level;

		BuildInfo buildInfo = BuildInfo.loadFromBuildInfoCache (tid_level);

        

		if(buildInfo==null)
		{

			CsvInfo csvData = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;


			string buildLayoutPath = "";
			string buildSpritePath = "";

			if(csvData.TID=="TID_BUILDING_LANDING_SHIP")
			{
				buildLayoutPath = "Model/Layout/LandCraftLayout";
			}
			if(csvData.TID=="TID_BUILDING_GUNSHIP")
			{
				buildLayoutPath = "Model/Layout/GunShipLayout";
			}
			buildSpritePath = "Model/Build3d/"+csvData.ExportName;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("Build Name : "+cardName+ " Tid Level : " + tid_level);
            sb.AppendLine("buildLayoutPath : " + buildLayoutPath);
            sb.AppendLine("buildSpritePath : " + buildSpritePath);

            GameObject buildLayoutInstance = null;
			GameObject buildSpriteInstance = null;



			buildLayoutInstance = Instantiate(ResourceCache.load(buildLayoutPath)) as GameObject;
			buildSpriteInstance = Instantiate(ResourceCache.load(buildSpritePath)) as GameObject;


			buildSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
			buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
			buildSpriteInstance.transform.localPosition = Vector3.zero; 
			buildSpriteInstance.transform.name = "BuildMain";
			buildLayoutInstance.transform.parent = Globals.buildContainer;


			buildInfo = buildLayoutInstance.GetComponent<BuildInfo>();

			buildInfo.buildSpritePath = buildSpritePath;
			buildInfo.is3D = true;
			buildInfo.tid = tid;
			buildInfo.level = csvData.Level;
			buildInfo.tid_level = tid_level;
			buildInfo.tid_type = csvData.TID_Type;
			buildInfo.csvInfo = csvData;

#if DEBUG_WYB

            sb.Insert(0, "Build Name : " + cardName);

            if (!string.IsNullOrEmpty(cardName))
            {
                System.IO.File.WriteAllText(Application.dataPath + @"/Test1/BuildCofigs/" + tabPageInfo + "_" + cardName + ".txt", sb.ToString());

                UnityEditor.AssetDatabase.Refresh();
            }

#endif


        }


        if (MoveOpEvent.Instance.SelectedBuildInfo!=null)
		{
			MoveOpEvent.Instance.SelectedBuildInfo.ResetPosition (); 
			MoveOpEvent.Instance.UnDrawBuildPlan(MoveOpEvent.Instance.SelectedBuildInfo);
			MoveOpEvent.Instance.SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(true);
		}
		MoveOpEvent.Instance.UnSelectBuild ();

        if(buildInfo.buildUIManage!=null)
		    buildInfo.buildUIManage.AfterInit ();
        if (buildInfo.buildUI != null)
            buildInfo.buildUI.AfterInit();

        buildInfo.AfterInit ();
		

		
		//自动绑定数据;
		if (item != null){
			Helper.ISFSObjectToBean(buildInfo,item);
			
			buildInfo.transform.name = "build_"+buildInfo.building_id;
			buildInfo.status = (BuildStatus)item.GetInt("status");
			buildInfo.artifact_type = (ArtifactType)item.GetInt("artifact_type");
			//buildInfo.Position = Vector3.zero;
			buildInfo.InitBuild();
			
			PopManage.Instance.RefreshBuildBtn(buildInfo);
		}else{
			buildInfo.building_id = Helper.getNewBuildingId();
			
			buildInfo.transform.name = "build_"+buildInfo.building_id;
			buildInfo.status = BuildStatus.Create;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;
			buildInfo.artifact_type = ArtifactType.None;
			

			//buildInfo.Position = Vector3.zero;
			if(buildInfo.buildUIManage!=null)
			    buildInfo.buildUIManage.ShowNewBox(true);
            if (buildInfo.buildUI != null)
                buildInfo.buildUI.ShowNewBox(true);
            MoveOpEvent.Instance.SelectBuild(buildInfo);

			
		}

        
        //新建的，也加入建筑物列表中，注：在取消新建时,需要移除;
        DataManager.GetInstance.BuildList[buildInfo.building_id] = buildInfo;

		if(buildInfo.csvInfo.TID=="TID_BUILDING_LANDING_SHIP")
		{
			LandCraft lc = buildInfo.GetComponent<LandCraft>();
			lc.Init();	
			lc.direct = Direct.UP;
			buildInfo.transform.position = Globals.landcraftPos [Globals.LandCrafts.Count]+Globals.dockPos;
			Globals.LandCrafts.Add (buildInfo);
		}
		if (buildInfo.csvInfo.TID == "TID_BUILDING_GUNSHIP")
		{
			buildInfo.transform.position = Globals.gunboatPos;
			GunBoat gb = buildInfo.GetComponent<GunBoat>();
			gb.Init();

		}


		buildInfo.gameObject.layer = 16;
		Transform[] t = buildInfo.transform.Find("buildPos/BuildMain").GetComponentsInChildren<Transform> (true);
		for(int i =0;i<t.Length;i++)
		{
			t[i].gameObject.layer = 16;
		}


		if(buildInfo.csvInfo.TID=="TID_BUILDING_LANDING_SHIP")
		{
			LandCraft landCraft = buildInfo.GetComponentInChildren<LandCraft>();
			if(landCraft!=null)
			{
				landCraft.InitTrooper(buildInfo.troops_tid,buildInfo.troops_num);
			}
		}
		
		return buildInfo.gameObject;
	}
*/
    public static string GetFormatStringByTime(long millseconds)
    {
        System.DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds(millseconds);
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);

        return dateTime.ToString("yyyy-MM-dd-HH-mm-ss-ff");
    }


	//创建建筑物
	//创建完后，会缓存一份buildInfo到BuildList 
	//item != null 时,是从服务器中同步下来的数据; item == null时，tid,level不能为空,是本地新建;
	public static GameObject CreateBuild(ISFSObject item,string tid = null, int level = 1,string cardName ="",string tabPageInfo = "", Models.BuildingData bbd = null)
    {

        if (item != null){//如果是服务器回传回来的数据，用服务器的tid和level
            tid = item.GetUtfString("tid");
			level = item.GetInt("level");
        }
        if (bbd!=null)
        {
            tid = bbd.tid;
            level = (int)bbd.level;
        }
		string tid_level = tid + "_" + level;
		BuildInfo buildInfo = BuildInfo.loadFromBuildInfoCache (tid_level);
		BuildManager bbm = BuildManager.GetInstance;

        if (tid == "TID_BUILDING_LANDING_SHIP" || tid == "TID_BUILDING_GUNSHIP")
        {
            //return Create3DBuild(item, tid, level, cardName, tabPageInfo);
            if (buildInfo == null)
            {
                bbm.CreateBuildGo3D(out buildInfo,new BuildParam()
                {
                    tid_level = tid_level,
                    tid = tid
                });
            }
            bbm.InitBuilding3D(ref buildInfo,new BuildParam()
            {
                item = item,
                bbd = bbd,
                tid = tid,
                level = level
            });
        }
        else
        {
            //创建模型（新）
            if (buildInfo == null)
            {
                bbm.CreateBuildGo(out buildInfo, new BuildParam()
                {
                    tid_level = tid_level,
                    tid = tid
                });
            }
            //初始化建筑（新）
            bbm.InitBuilding(ref buildInfo, new BuildParam()
            {
                item = item,
                bbd = bbd,
                tid = tid,
                level = level
            });
        }
        


       

#if DEBUG_WYB

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

#endif

        #region 创建模型(老代码)
        /**
     if (buildInfo==null)
     {
      
             CsvInfo csvData = CSVManager.GetInstance.csvTable[tid_level] as CsvInfo;//该建筑level数据
             CsvInfo Lv1CsvData = CSVManager.GetInstance.csvTable[tid+"_1"] as CsvInfo;//该建筑1级时的数据
             
             //string resourcePath = string.Empty;
             string buildLayoutPath = "Model/Layout/build"+csvData.Width;
             string buildSpPath = "Model/Build/" + csvData.ExportName+"_sp";
             string buildSpritePath = "Model/Build/" + csvData.ExportName;
             string buildNewSpritePath = "Model/Build/buildnew" + csvData.Width;
             
             GameObject buildLayoutInstance = null;
             GameObject buildSpInstance = null;
             GameObject buildSpriteInstance = null;
             GameObject buildNewSpriteInstance = null;

             //定义建筑物布局实例;

#if DEBUG_WYB

             bool haveDefault_buildLayoutPath = true;
             bool haveDefault_buildSpPath = true;
             bool haveDefault_buildSpritePath = true;
             bool haveDefault_buildNewSpritePath = true;
             
#endif

             if (ResourceCache.load(buildLayoutPath)==null)//如果没有合适的Layout,则用默认的build3
             {
#if DEBUG_WYB
                 haveDefault_buildLayoutPath = false;
#endif
                 buildLayoutPath = "Model/Layout/build3";
             }
             if(ResourceCache.load(buildSpritePath)==null)//如果没有默认的Sprite，则用1级的sprite
             {

#if DEBUG_WYB
                 haveDefault_buildSpPath = false;
#endif


                 buildSpritePath = "Model/Build/" + Lv1CsvData.ExportName;
                 buildSpPath = "Model/Build/" + Lv1CsvData.ExportName+"_sp";
             }

             if(ResourceCache.load(buildSpritePath)==null)
             {

#if DEBUG_WYB
                 haveDefault_buildSpritePath = false;
#endif



                 buildSpritePath = "Model/Build/housing_lvl1";
                 buildSpPath = "Model/Build/housing_lvl1_sp";
                 buildLayoutPath = "Model/Layout/build3";
             }

             if(ResourceCache.load(buildSpPath)==null)
             {
#if DEBUG_WYB

#endif

                 buildSpPath = "Model/Build/" + Lv1CsvData.ExportName+"_sp";
             }

#if DEBUG_WYB

#endif

             //地面
             buildLayoutInstance = Instantiate(ResourceCache.load(buildLayoutPath)) as GameObject;


             buildSpriteInstance = Instantiate(ResourceCache.load(buildSpritePath)) as GameObject;
             buildSpInstance = Instantiate(ResourceCache.load(buildSpPath)) as GameObject;

             if(ResourceCache.load(buildNewSpritePath)!=null)
             {
#if DEBUG_WYB
                 haveDefault_buildNewSpritePath = false;
#endif
                
                 buildNewSpriteInstance = Instantiate(ResourceCache.load(buildNewSpritePath)) as GameObject;
             
                 buildNewSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
                 buildNewSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
                 buildNewSpriteInstance.transform.localPosition = Vector3.zero; 
                 buildNewSpriteInstance.transform.name = "BuildNew";
                 buildNewSpriteInstance.gameObject.SetActive(false);//默认情况下会被禁用
             }


#if DEBUG_WYB

             
             
             sb.AppendLine("Tid Level : " + tid_level);
             sb.AppendLine("buildLayoutPath : " + buildLayoutPath);
             sb.AppendLine("buildSpPath : " + buildSpPath);
             sb.AppendLine("buildSpritePath : " + buildSpritePath);

             if (buildNewSpriteInstance != null)
             {
                 sb.AppendLine("buildNewSpritePath : " + buildNewSpritePath);
             }
             else
             {
                 sb.AppendLine("Have no buildNewSpritePath!");
             }
             
#endif
             
         
         buildSpriteInstance.transform.parent = buildLayoutInstance.transform.Find ("buildPos");
         buildSpriteInstance.transform.localRotation = new Quaternion (0f, 0f, 0f, 0f);
         buildSpriteInstance.transform.localPosition = Vector3.zero; 
         buildSpriteInstance.transform.name = "BuildMain";
         buildSpInstance.transform.parent = buildLayoutInstance.transform;
         buildSpInstance.transform.name = "StandPoints";
         buildSpInstance.transform.localPosition = Vector3.zero;

             if (csvData.TID_Type == "OBSTACLES" || csvData.BuildingClass == "Artifact")//如果时障碍物或者遗迹，则删掉Floor
             {
                 Transform floor = buildLayoutInstance.transform.Find("Floor");
                 if (floor != null)
                     Destroy(floor.gameObject);
             }
         buildLayoutInstance.transform.parent = Globals.BuildContainer;
             buildInfo = buildLayoutInstance.GetComponent<BuildInfo>();
             buildInfo.buildSpritePath = buildSpritePath;
             buildInfo.tid = tid;
             buildInfo.level = csvData.Level;
             buildInfo.tid_level = tid_level;
             buildInfo.tid_type = csvData.TID_Type;
             buildInfo.csvInfo = csvData;
        

         }
                **/
        #endregion




        /*
        //初始化建筑UI
        buildInfo.GetComponent<BuildUIManage> ().AfterInit ();
        //初始化当前建筑的遮挡点和播放建筑Tweener动画
        buildInfo.AfterInit ();
        //清除原来的选中
		if(MoveOpEvent.Instance.SelectedBuildInfo!=null)//UI选中相关
		{
			MoveOpEvent.Instance.SelectedBuildInfo.resetPosition (); 
			MoveOpEvent.Instance.UnDrawBuildPlan(MoveOpEvent.Instance.SelectedBuildInfo);
			MoveOpEvent.Instance.SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(true);
		}
		MoveOpEvent.Instance.UnSelectBuild ();
        */


        #region 初始化建筑 
        /**
		if (isServerCreate)//与服务器创建相关
        {

			if(status==1)//如果当前建筑处于正在新建状态(New)
			{
				Transform buildNewT = buildInfo.transform.Find("buildPos/BuildNew");
				if(buildNewT!=null)
				{
					buildNewT.gameObject.SetActive(true);
					buildInfo.transform.Find("buildPos/BuildMain").gameObject.SetActive(false);
				}
			}

			if(item!=null)
                Helper.ISFSObjectToBean(buildInfo,item);
            
            #region 
            //新
            
            string name = "build_"+ (CSVManager.GetInstance.csvTable[tid_level] as CsvInfo).Width+"__" + GetFormatStringByTime(buildInfo.building_id);//设置名字
            buildInfo.transform.name = name;
            

            //原
            //buildInfo.transform.name = "build_" + buildInfo.building_id;//当前建筑building_id
            #endregion


            buildInfo.status = (BuildStatus)status;//当前建筑Build状态
			buildInfo.artifact_type = (ArtifactType)artifact_type;//遗迹神像类型
			buildInfo.Position = new Vector3(buildInfo.x,0f,buildInfo.y);//所有建筑都在y=0这个平面上
            
			buildInfo.initBuild();

			PopManage.Instance.RefreshBuildBtn(buildInfo);
		}
        else
        {
			buildInfo.building_id = Helper.getNewBuildingId();

            string name = "build_" + (CSVManager.GetInstance.csvTable[tid_level] as CsvInfo).Width+"__"+ GetFormatStringByTime(buildInfo.building_id);
            buildInfo.transform.name = name;

            //buildInfo.transform.name = "build_"+buildInfo.building_id;
			buildInfo.status = BuildStatus.Create;//-1:客户端准备创建(建筑物会出现：取消,确定 按钮),0:正常;1:正在新建;2:正在升级;3:正常在移除;4:正在研究;5:正在训练;6:正在生产神像;
			buildInfo.artifact_type = ArtifactType.None;

			if (buildInfo.csvInfo.BuildingClass == "Artifact")
            {
				BuildInfo s = Helper.getBuildInfoByTid("TID_BUILDING_ARTIFACT_WORKSHOP");
				buildInfo.artifact_boost = s.artifact_boost;
				buildInfo.artifact_type = s.artifact_type;
			}


			buildInfo.Position = Helper.getBlankXY(buildInfo.GridCount);

			buildInfo.GetComponent<BuildUIManage>().ShowNewBox(true);

            //设置当前选中的build
			MoveOpEvent.Instance.SelectBuild(buildInfo);


            MoveOpEvent.Instance.DrawBuildPlan(buildInfo);


            buildInfo.isMoving = true;
			MoveOpEvent.Instance.gridMap.GetComponent<DrawGrid>().drawLine();

		}
       

#if DEBUG_WYB

        sb.Insert(0,"Build Name : "+cardName);

        if (!string.IsNullOrEmpty(cardName))
        {
            System.IO.File.WriteAllText(Application.dataPath + @"/Test1/BuildCofigs/" + tabPageInfo + "_"+cardName + ".txt", sb.ToString());

            UnityEditor.AssetDatabase.Refresh();
        }
        
#endif

        if (buildInfo.csvInfo.BuildingClass=="Artifact")
		{
			
			Artifact artifact = buildInfo.transform.GetComponentInChildren<Artifact>();
			if(artifact!=null)
			{
				artifact.setStatus(buildInfo.artifact_type);
				artifact.buildInfo = buildInfo;
			}
		}

		if(buildInfo.status==BuildStatus.Upgrade)//如果处于正在升级状态,则显示buildin
		{
			if(buildInfo.transform.Find("buildin")!=null)
			buildInfo.transform.Find("buildin").gameObject.SetActive(true);
		}

		BuildRing buildRing = buildInfo.GetComponentInChildren<BuildRing> ();
		if(buildRing!=null)
		{
			if(buildInfo.csvInfo.TID_Type=="TRAPS")
			{
				buildRing.OuterRange = buildInfo.csvInfo.TriggerRadius / 100f;
				buildRing.InnerRange = 0f;
			}
			else
			{
				buildRing.OuterRange = buildInfo.csvInfo.AttackRange / 100f;
				buildRing.InnerRange = buildInfo.csvInfo.MinAttackRange / 100f;
			}
		}

		//新建的，也加入建筑物列表中，注：在取消新建时,需要移除;
		DataManager.GetInstance.BuildList[buildInfo.building_id] = buildInfo;
		if(buildInfo.csvInfo.TID_Type!="OBSTACLES")
			DataManager.GetInstance.buildArray.Add (buildInfo);

		
		buildInfo.buildUIManage = buildInfo.GetComponent<BuildUIManage> ();

		if (buildInfo.csvInfo.TID_Type != "TRAPS") {

			buildInfo.buildUIManage.buildBattleInfo.Health = buildInfo.csvInfo.Hitpoints;
			buildInfo.buildUIManage.buildBattleInfo.HealthAdd = Helper.getArtifactBoost (buildInfo.csvInfo.Hitpoints, ArtifactType.BoostBuildingHP);
			buildInfo.buildUIManage.buildBattleInfo.Dps = buildInfo.csvInfo.Damage;
			buildInfo.buildUIManage.buildBattleInfo.DpsAdd = Helper.getArtifactBoost (buildInfo.csvInfo.Damage, ArtifactType.BoostBuildingDamage);
				}
         **/
        #endregion
        //buildInfo.initBuild();

        /*
		if(Globals.sceneStatus == SceneStatus.ENEMYBATTLE||Globals.sceneStatus == SceneStatus.BATTLEREPLAY)
		{

			buildInfo.BattleHitpoint = buildInfo.csvInfo.Hitpoints;
			buildInfo.BattleDamage = buildInfo.csvInfo.Damage;
			buildInfo.BattleInit(); 

			if(buildInfo.csvInfo.TID_Type!="OBSTACLES"
			   &&buildInfo.csvInfo.TID_Type!="TRAPS"
			   &&buildInfo.csvInfo.TID_Type!="DECOS"
			   &&buildInfo.csvInfo.BuildingClass!="Artifact")
			{
				BattleData.Instance.BuildList.Add(buildInfo.BattleID.ToString(),buildInfo);
			}

			if(buildInfo.csvInfo.BuildingClass=="Defense"||buildInfo.csvInfo.TID_Type == "TRAPS")
			{
				BattleData.Instance.WeaponBuildList.Add(buildInfo.BattleID.ToString(),buildInfo);
				if(buildInfo.csvInfo.TID_Type=="TRAPS")
				{
					BattleData.Instance.TrapList.Add(buildInfo.BattleID.ToString(),buildInfo);
				}
			}

			if(buildInfo.csvInfo.TID=="TID_BUILDING_PALACE" || buildInfo.csvInfo.TID=="TID_BUILDING_COMMAND_CENTER")
			{
				BattleData.Instance.TownHall = buildInfo;
			}

		}*/

        return buildInfo.gameObject;
	}

	//重新计算底部版面数据
	//DiamondPanel = true,宝石销售版面; 显示宝石销售面板
	//当DiamondPanel=false时，返回当前可以新建的建筑物数量;
	//isEnabled参数在DiamondPanel = true时有效,当用户点购买宝石时，需要禁用一会儿界面;
	public static int CalcShopCates(bool DiamondPanel = false, bool isEnabled = true){
		int NoticeCount = 0;
		Globals.ShopCates.Clear();

		if (DiamondPanel){
			for(int i=0;i<1;i++)
			{
				ShopCate cate = new ShopCate();
				cate.Name = "";
				cate.ShopItems = new List<ShopItem>();
				
				for(int j=1;j<=5;j++)
				{
					ShopItem item = new ShopItem();
					item.tid = "TID_DIAMOND_PACK";
					item.tid_level = "TID_DIAMOND_PACK_" + j.ToString();
					item.isEnabled = isEnabled;
					
					string CostAmount = "";
					int diamondAmount = 0;
					if (item.tid_level == "TID_DIAMOND_PACK_1"){
						CostAmount = "CN￥30";
						diamondAmount = 500;
					}else if (item.tid_level == "TID_DIAMOND_PACK_2"){
						CostAmount = "CN￥68";
						diamondAmount = 1200;					
					}else if (item.tid_level == "TID_DIAMOND_PACK_3"){
						CostAmount = "CN￥128";
						diamondAmount = 2500;					
					}else if (item.tid_level == "TID_DIAMOND_PACK_4"){
						CostAmount = "CN￥328";
						diamondAmount = 6500;					
					}else if (item.tid_level == "TID_DIAMOND_PACK_5"){
						CostAmount = "CN￥648";
						diamondAmount = 14000;
					}
					
					
					item.Name = LocalizationCustom.instance.Get(item.tid_level);//"一大袋宝石";
					item.diamondAmount = diamondAmount;
					item.Buildtime = null;
					item.BuildMax = 0;

					
					item.ShopCosts = new List<ShopCost>();
					
					ShopCost MoneyCost = new ShopCost();
					MoneyCost.CostType = ShopCostType.Money;
					MoneyCost.CostAmount = CostAmount;//"$19.99";
					item.ShopCosts.Add(MoneyCost);
					
					item.DisableDescription = MoneyCost.CostAmount;
					
					cate.ShopItems.Add(item);
				}
				
				Globals.ShopCates.Add(cate);
			}		
		}else{
			for(int i=0;i<3;i++)
			{
				ShopCate cate = new ShopCate();
				
				string shop_type = null;
				if(i==0){
					cate.Name = LocalizationCustom.instance.Get("TID_SHOP_CATEGORY_RESOURCE_BUILDINGS");// "Economy";
					shop_type = "Economy";                //经济面板
				}
				if(i==1)
				{
					cate.Name = LocalizationCustom.instance.Get("TID_SHOP_CATEGORY_DEFENSE_BUILDINGS");//"Defense";
					shop_type = "Defense";                 //防御面板
				}
				if(i==2){
					cate.Name = LocalizationCustom.instance.Get("TID_SHOP_CATEGORY_ARMY_BUILDINGS");//"Support";
					shop_type = "Support";                  //支持面板
				}
				
				cate.ShopItems = new List<ShopItem>();
				cate.NoticeCount = 0;
				
				Dictionary<string,CsvInfo> shopItems = Helper.getShopItem(shop_type);

				foreach(KeyValuePair<string,CsvInfo> de in shopItems){
					//if (i == 1)
					//Debug.Log(de.Key);

					ShopItem item = new ShopItem();
					item.ShopCosts = new List<ShopCost>();
					cate.ShopItems.Add(item);
					CsvInfo csvInfo = de.Value;
					
					item.tid_level = csvInfo.TID_Level;
					item.tid = csvInfo.TID;
					item.Name = LocalizationCustom.instance.Get(csvInfo.TID);//"Cannon";
					item.Description = LocalizationCustom.instance.Get(csvInfo.SubtitleTID);// "Good against tough targets";

					
					item.Buildtime = Helper.GetFormatTime(Helper.GetBuildTime(csvInfo.TID_Level),1);
					item.BuildMax = Helper.getBuildingMaxCount(csvInfo.TID,0);
					item.Buildcount = Helper.getBuildingCurCount(csvInfo.TID,null);
					
					//检查是否可以新建，可以的返回null,不可以的话返回，不可新建原因;
					item.DisableDescription = Helper.checkNewBuild(csvInfo.TID);
					
					if (item.DisableDescription == null){
						NoticeCount ++;
						cate.NoticeCount += 1;
						item.isEnabled = true;
						BuildCost bc = Helper.GetBuildCost(csvInfo.TID_Level);
						
						
						
						if (bc.gold > 0){
							ShopCost sc = new ShopCost();
							sc.CostType = ShopCostType.Gold;
							sc.CostAmount = bc.gold.ToString();
							item.ShopCosts.Add(sc);
						}
						
						if (bc.wood > 0){
							ShopCost sc = new ShopCost();
							sc.CostType = ShopCostType.Wood;
							sc.CostAmount = bc.wood.ToString();
							item.ShopCosts.Add(sc);
						}
						
						if (bc.stone > 0){
							ShopCost sc = new ShopCost();
							sc.CostType = ShopCostType.Stone;
							sc.CostAmount = bc.stone.ToString();
							item.ShopCosts.Add(sc);
						}
						
						if (bc.iron > 0){
							ShopCost sc = new ShopCost();
							sc.CostType = ShopCostType.Iron;
							sc.CostAmount = bc.iron.ToString();
							item.ShopCosts.Add(sc);
						}
					}else{
						item.isEnabled = false;
						item.Buildtime = null;
					}
				}
				
				Globals.ShopCates.Add(cate);
			}
		}

		return NoticeCount;
	}


	//获得当前指定容器的最大容量;
	public static int GetMaxStored(String tid_type){
		int num  = 0;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if (s.status != BuildStatus.New){
				if (tid_type == "GOLD_STORAGE" && ("TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid) || "TID_BUILDING_GOLD_STORAGE".Equals(s.tid))){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					num += tmpcsv.MaxStoredResourceGold;				
				}else if (tid_type == "WOOD_STORAGE" && ("TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid) || "TID_BUILDING_WOOD_STORAGE".Equals(s.tid))){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					num += tmpcsv.MaxStoredResourceWood;				
				}else if (tid_type == "STONE_STORAGE" && ("TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid) || "TID_BUILDING_STONE_STORAGE".Equals(s.tid))){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					num += tmpcsv.MaxStoredResourceStone;				
				}else if (tid_type == "METAL_STORAGE" && ("TID_BUILDING_PALACE".Equals(s.tid) || "TID_BUILDING_VAULT".Equals(s.tid) || "TID_BUILDING_METAL_STORAGE".Equals(s.tid))){
					CsvInfo tmpcsv = (CsvInfo)CSVManager.GetInstance.csvTable[s.tid_level];
					num += tmpcsv.MaxStoredResourceIron;				
				}
			}
		}
		return num;
	}

	//重新设置当前指定容器的最大容量;
	public static void SetMaxStored(String tid_type, bool update_ui = true){
		int maxStored = GetMaxStored(tid_type);
		if (tid_type == "GOLD_STORAGE"){
			DataManager.GetInstance.model.user_info.max_gold_count = maxStored;
			if (update_ui)
				UpdateResUI("Gold",false);
		}

		if (tid_type == "WOOD_STORAGE"){
			DataManager.GetInstance.model.user_info.max_wood_count = maxStored;
			if (update_ui)
				UpdateResUI("Wood",false);
		}

		if (tid_type == "STONE_STORAGE"){
			DataManager.GetInstance.model.user_info.max_stone_count = maxStored;
			if (update_ui)
				UpdateResUI("Stone",false);
		}

		if (tid_type == "METAL_STORAGE"){
			DataManager.GetInstance.model.user_info.max_iron_count = maxStored;	
			if (update_ui)
				UpdateResUI("Iron",false);
		}
	}

	//设置当前金币，木材，石材，钢材的最大存储容量;
	public static void SetAllMaxStored(bool update_ui = true){
		SetMaxStored("GOLD_STORAGE",update_ui);	
		SetMaxStored("WOOD_STORAGE",update_ui);	
		SetMaxStored("STONE_STORAGE",update_ui);	
		SetMaxStored("METAL_STORAGE",update_ui);	
	}

	//重新设置指定类型（司令部，实验室，炮舰，地库) 级别到用户表，返回设置后的结果;
	public static void SetMaxLevel(String tid){
		int max_level = getTidMaxLevel(tid,0);
		if (tid == "TID_BUILDING_PALACE"){
			//司令部等级;		
			DataManager.GetInstance.model.user_info.town_hall_level = max_level;
		}
		
		if (tid == "TID_BUILDING_LABORATORY"){
			//作战学院/实验室等级			
			DataManager.GetInstance.model.user_info.laboratory_level = max_level;
		}
		
		if (tid == "TID_BUILDING_GUNSHIP"){
			//炮舰等级			
			DataManager.GetInstance.model.user_info.gunship_level = max_level;
		}
		
		if (tid == "TID_BUILDING_VAULT"){
			//TID_BUILDING_VAULT地下仓库;等级			
			DataManager.GetInstance.model.user_info.vault_level = max_level;
		}
	}

	//重新设置，司令部，实验室，炮舰，地库 级别到用户表;
	public static void SetAllMaxLevel(){
		SetMaxLevel("TID_BUILDING_PALACE");
		SetMaxLevel("TID_BUILDING_LABORATORY");
		SetMaxLevel("TID_BUILDING_GUNSHIP");
		SetMaxLevel("TID_BUILDING_VAULT");
	}

	/*获取指定tid的建筑物,没找到，则返回null;*/
	public static BuildInfo getBuildInfoByTid(string tid){
		foreach(BuildInfo s2 in DataManager.GetInstance.buildList.Values){
			if (tid.Equals(s2.tid)){
				return s2;
			}
		}
		return null;
	}
	

	//设置每个建筑物上面的，升级提示标识;
	public static void SetBuildUpgradeIcon(){
		//获得忙碌的工人数;building_id最快完工的building_id;
		long min_b_id = 0;
		int worker = GetWorkeringCount(ref min_b_id);
		bool is_show = false;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if ("BUILDING".Equals(s.tid_type) && DataManager.GetInstance.model.user_info.worker_count > worker){
				if (CheckHasUpgrade(s.tid,s.level) == null){

					//还可以升级;
					//判断，升级费用是否足够;
					ISFSObject obj = getCostDiffToGems(s.tid_level,1,true,0);


					if (obj.GetInt("Gems") == 0){
						//当前返回：Gems > 0时，说明资源不足,需要使用宝石补差余;
						//显示升级提醒;
						is_show = true;
					}else{
						//不显示升级提醒;
						is_show = false;
					}

					//Debug.Log(s.tid_level + ";gems:" + obj.GetInt("Gems") + ";is_show:" + is_show);
				}else{
					//不显示升级提醒;
					is_show = false;
				}
			}else{
				//不显示升级提醒;
				is_show = false;
			}

			if (s.buildUIManage == null)
            {
				s.buildUIManage = s.GetComponent<BuildUIManage>();
			}
            if (s.buildUI == null)
            {
                s.buildUI = s.GetComponent<BuildUI>();
            }

            //Debug.Log(s.tid_level + ";gems:" + ";is_show:" + is_show);
            if(s.buildUIManage!=null)
                s.buildUIManage.BindUpDBrand(is_show);
            if (s.buildUI != null)
                s.buildUI.BindUpDBrand(is_show);
        }
	}

	//获得每小时资源生产量（从基地);
	public static int ResPerHourByBase(String tid){
		//string tid = "TID_BUILDING_HOUSING";
		ArtifactType artifact_type = ArtifactType.None;
		//生产资源单元;
		if (tid == "TID_BUILDING_HOUSING"){			
			artifact_type = ArtifactType.None;
		}else if (tid == "TID_BUILDING_WOODCUTTER"){			
			artifact_type = ArtifactType.BoostWood;
		}else if (tid == "TID_BUILDING_STONE_QUARRY"){			
			artifact_type = ArtifactType.BoostStone;
		}else if (tid == "TID_BUILDING_METAL_MINE"){			
			artifact_type = ArtifactType.BoostMetal;
		}
		
		int rph = 0;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if (s.csvInfo.TID == tid){		
				rph += s.csvInfo.ResourcePerHour;
				int artifact_num = getArtifactBoost(s.csvInfo.ResourcePerHour, artifact_type);
				rph += artifact_num;
			}
		}
		
		return rph;
	}
	
	//获得每小时资源生产量(从资源岛);
	public static int ResPerHourByIsland(String tid){
		//string tid = "TID_BUILDING_HOUSING";
		ArtifactType artifact_type = ArtifactType.None;
		//生产资源单元;
		if (tid == "TID_BUILDING_HOUSING"){			
			artifact_type = ArtifactType.None;
		}else if (tid == "TID_BUILDING_WOODCUTTER"){			
			artifact_type = ArtifactType.BoostWood;
		}else if (tid == "TID_BUILDING_STONE_QUARRY"){			
			artifact_type = ArtifactType.BoostStone;
		}else if (tid == "TID_BUILDING_METAL_MINE"){			
			artifact_type = ArtifactType.BoostMetal;
		}
		
		int rph = 0;
		foreach(UserRegions s in DataManager.GetInstance.userRegionsList.Values){
			if (s.res_tid == tid && s.capture_id == DataManager.GetInstance.model.user_info.id){	
				if (tid == "TID_BUILDING_HOUSING"){
					rph += 40;
				}else{
					string tid_level = s.res_tid + "_" + s.res_level;
					if (CSVManager.GetInstance.csvTable.ContainsKey(tid_level)){
						CsvInfo csvInfo = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
						
						rph += csvInfo.ResourcePerHour;
						int artifact_num = getArtifactBoost(csvInfo.ResourcePerHour, artifact_type);
						rph += artifact_num;
					}
				}
			}
		}		
		return rph;
	}
	
	//获得可采集的数量(从资源岛);
	public static int getCollectNumByIsland(PartType i_partType, int collect_time=0, bool is_collect = false){
		
		if (collect_time == 0){
			collect_time = current_time();
		}
		
		int total_num = 0;
		String tid = "";
		ArtifactType artifact_type = ArtifactType.None;
		
		if (i_partType == PartType.Gold){
			tid = "TID_BUILDING_HOUSING";
			artifact_type = ArtifactType.BoostGold;
		}else if (i_partType == PartType.Wood){
			tid = "TID_BUILDING_WOODCUTTER";
			artifact_type = ArtifactType.BoostWood;
		}else if (i_partType == PartType.Stone){
			tid = "TID_BUILDING_STONE_QUARRY";
			artifact_type = ArtifactType.BoostStone;
		}else if (i_partType == PartType.Iron){
			tid = "TID_BUILDING_METAL_MINE";
			artifact_type = ArtifactType.BoostMetal;
		}		
		
		int collect_num = 0;
		foreach(UserRegions s in DataManager.GetInstance.userRegionsList.Values){

			if (s.regions_id > 1 && s.is_npc != 9 && s.capture_id == DataManager.GetInstance.model.user_info.id){
				collect_num = 0;
				if (tid == "TID_BUILDING_HOUSING" && (s.res_tid == "" || s.res_tid == null)){
					int rph = 40;
					collect_num = (int)(((float)(collect_time - s.last_collect_time)/3600.0f) * rph);

					//设置最后采集时间;
					if (is_collect) s.last_collect_time = collect_time;
					total_num += collect_num;
				}else if (s.res_tid == tid){
					string tid_level = s.res_tid + "_" + s.res_level;
					if (CSVManager.GetInstance.csvTable.ContainsKey(tid_level)){
						CsvInfo csvInfo = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
						
						int rph = csvInfo.ResourcePerHour;
						int artifact_num = getArtifactBoost(csvInfo.ResourcePerHour, artifact_type);
						rph += artifact_num;
						
						collect_num = (int)(((float)(collect_time - s.last_collect_time)/3600.0f) * rph);
						
						if (collect_num > csvInfo.ResourceMax){
							collect_num = csvInfo.ResourceMax;
						}
					}

					//设置最后采集时间;
					if (is_collect) s.last_collect_time = collect_time;
					total_num += collect_num;
				}							

			}
		}

		if (tid == "TID_BUILDING_HOUSING" && total_num > DataManager.GetInstance.model.user_info.max_gold_count){
			Debug.Log("total_num:" + total_num);
			total_num = DataManager.GetInstance.model.user_info.max_gold_count;
		}

		//设置最后采集时间;
		if (is_collect){
			//通知服务器;
			ISFSObject data = new SFSObject();
			data.PutInt("collect_time",collect_time);
			data.PutInt("collect_num",total_num);
			data.PutUtfString("res_type",i_partType.ToString());
			SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_COLLECT_ISLAND, false, null);
		}

		return total_num;		
	}

	//返回可领到的成就数量;
	public static int setAchievementsList(ISFSObject achievements){
		//Debug.Log(achievements.GetDump());
		int ClaimNum = 0;
		ISFSArray achievements_list = Helper.SFSObjToArr(achievements);
		
		for(int i = 0; i < achievements_list.Count; i ++){
			
			ISFSObject item = achievements_list.GetSFSObject(i);

			String tid = item.GetUtfString("TID");
			AchievementItem aitem = null;
			if (CSVManager.GetInstance.achievementsList.ContainsKey(tid)){
				aitem = CSVManager.GetInstance.achievementsList[tid] as AchievementItem;
			}else{
				aitem = new AchievementItem();
				CSVManager.GetInstance.achievementsList.Add(tid,aitem);
			}			
			aitem.ISFSObjectToBean(item);
			
		}

		foreach(AchievementItem item in CSVManager.GetInstance.achievementsList.Values){
			int cur_count = item.cur_count;
			int ActionCount = item.ActionCount;
			long Level = item.Level;
			if (Level < 3 && cur_count >= ActionCount){
				ClaimNum ++;
			}
		}
		//Debug.Log(CSVMananger.GetInstance.achievementsList.Count);
		return ClaimNum;
	}

	/*
	TID_COMMON_ARTIFACT
		TID_RARE_ARTIFACT
			TID_EPIC_ARTIFACT
			TID_COMMON_ARTIFACT_ICE
			TID_RARE_ARTIFACT_ICE
			TID_EPIC_ARTIFACT_ICE
			TID_COMMON_ARTIFACT_FIRE
			TID_RARE_ARTIFACT_FIRE
			TID_EPIC_ARTIFACT_FIRE
			TID_COMMON_ARTIFACT_DARK
			TID_RARE_ARTIFACT_DARK
			TID_EPIC_ARTIFACT_DARK

			TID_BUILDING_ARTIFACT1
			TID_BUILDING_ARTIFACT2
			TID_BUILDING_ARTIFACT3
			TID_BUILDING_ARTIFACT1_ICE
			TID_BUILDING_ARTIFACT2_ICE
			TID_BUILDING_ARTIFACT3_ICE
			TID_BUILDING_ARTIFACT1_FIRE
			TID_BUILDING_ARTIFACT2_FIRE
			TID_BUILDING_ARTIFACT3_FIRE
			TID_BUILDING_ARTIFACT1_DARK
			TID_BUILDING_ARTIFACT2_DARK
			TID_BUILDING_ARTIFACT3_DARK
*/
	
	//fanwe_buildings表中的Artifact tid转换成对应fanwe_artifacts表中的Artifact tid;
	public static string BuildTIDToArtifactTID(string build_tid){
		string artiact_tid = "";
		if ("TID_BUILDING_ARTIFACT1".Equals(build_tid)){
			artiact_tid = "TID_COMMON_ARTIFACT";
		}else if ("TID_BUILDING_ARTIFACT2".Equals(build_tid)){
			artiact_tid = "TID_RARE_ARTIFACT";
		}else if ("TID_BUILDING_ARTIFACT3".Equals(build_tid)){
			artiact_tid = "TID_EPIC_ARTIFACT";
		}else if ("TID_BUILDING_ARTIFACT1_ICE".Equals(build_tid)){
			artiact_tid = "TID_COMMON_ARTIFACT_ICE";
		}else if ("TID_BUILDING_ARTIFACT2_ICE".Equals(build_tid)){
			artiact_tid = "TID_RARE_ARTIFACT_ICE";
		}else if ("TID_BUILDING_ARTIFACT3_ICE".Equals(build_tid)){
			artiact_tid = "TID_EPIC_ARTIFACT_ICE";
		}else if ("TID_BUILDING_ARTIFACT1_FIRE".Equals(build_tid)){
			artiact_tid = "TID_COMMON_ARTIFACT_FIRE";
		}else if ("TID_BUILDING_ARTIFACT2_FIRE".Equals(build_tid)){
			artiact_tid = "TID_RARE_ARTIFACT_FIRE";
		}else if ("TID_BUILDING_ARTIFACT3_FIRE".Equals(build_tid)){
			artiact_tid = "TID_EPIC_ARTIFACT_FIRE";
		}else if ("TID_BUILDING_ARTIFACT1_DARK".Equals(build_tid)){
			artiact_tid = "TID_COMMON_ARTIFACT_DARK";
		}else if ("TID_BUILDING_ARTIFACT2_DARK".Equals(build_tid)){
			artiact_tid = "TID_RARE_ARTIFACT_DARK";
		}else if ("TID_BUILDING_ARTIFACT3_DARK".Equals(build_tid)){
			artiact_tid = "TID_EPIC_ARTIFACT_DARK";
		}

		return artiact_tid;
	}

	//返回当前已经布置的石像数量;
	public static int GetArtifactNum(){
		int currentAmount = Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT1") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT2") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT3");
		
		currentAmount += Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT1_ICE") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT2_ICE") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT3_ICE");
		
		currentAmount += Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT1_FIRE") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT2_FIRE") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT3_FIRE");
		
		currentAmount += Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT1_DARK") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT2_DARK") + Helper.getBuildingCurCount("TID_BUILDING_ARTIFACT3_DARK");

		return currentAmount;
	}

	//StatueItem1.Find("avatar")
	public static void CreateArtifactUI(Transform avatar, string tid_level, ArtifactType artifact_type = ArtifactType.None){
		
		while(avatar.childCount > 0){
			DestroyImmediate(avatar.GetChild(0).gameObject);
		}
		
		CsvInfo csvInfo1 = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
		string buildSpritePath = "Model/Build/" + csvInfo1.ExportName;
		
		//Debug.Log(buildSpritePath);


		
		GameObject buildSpriteInstance = Instantiate(ResourceCache.load(buildSpritePath)) as GameObject;
		
		Artifact artifact = buildSpriteInstance.transform.GetComponentInChildren<Artifact>();
		if(artifact!=null)
		{
			artifact.setStatus(artifact_type);
		}
		
		
		buildSpriteInstance.layer = LayerMask.NameToLayer("UI");
		buildSpriteInstance.transform.parent = avatar;
		buildSpriteInstance.transform.localPosition = new Vector3(1,1,1);
		buildSpriteInstance.transform.localScale = new Vector3(650,650,1);
		buildSpriteInstance.transform.name = "model";

		Transform model = buildSpriteInstance.transform.Find("model");
		model.gameObject.layer = buildSpriteInstance.layer;
		foreach(Transform tt in model)
		{
			tt.gameObject.layer = buildSpriteInstance.layer;
		}
		
		foreach(Transform tt in model.Find("status_gem"))
		{
			tt.gameObject.layer = buildSpriteInstance.layer;
			/*
			if (tt.name == "eyes"){
				tt.localPosition = new Vector3(tt.localPosition.x, tt.localPosition.y,-1);
			}
			*/
		}
		
		foreach(Transform tt in model.Find("status_none"))
		{
			tt.gameObject.layer = buildSpriteInstance.layer;
			/*
			if (tt.name == "eyes"){
				tt.localPosition = new Vector3(tt.localPosition.x, tt.localPosition.y,-1);
			}
			*/
		}
		
		foreach(Transform tt in model.Find("status_shield"))
		{
			tt.gameObject.layer = buildSpriteInstance.layer;
			/*
			if (tt.name == "eyes"){
				tt.localPosition = new Vector3(tt.localPosition.x, tt.localPosition.y,-1);
			}
			*/
		}
		
		foreach(Transform tt in model.Find("status_weapon"))
		{
			tt.gameObject.layer = buildSpriteInstance.layer;
			/*
			if (tt.name == "eyes"){
				tt.localPosition = new Vector3(tt.localPosition.x, tt.localPosition.y,-1);
			}
			*/
		}
	}

	/*获得可站工人的，正在建创的建筑物;*/
	public static BuildInfo getWorkBuilding(){
		foreach(BuildInfo s2 in DataManager.GetInstance.buildList.Values){
			if ((s2.status == BuildStatus.New || s2.status == BuildStatus.Upgrade) && !"TID_BUILDING_LANDING_SHIP".Equals(s2.tid) && !"TID_BUILDING_GUNSHIP".Equals(s2.tid)){
				return s2;
			}
		}
		return null;
	}

	//初始化用户地图数据;
	public static void HandleWolrdMap(bool only_explore = false){

		//PopManage.Instance.WorldMap
		Transform Cloud = PopManage.Instance.WorldMap.transform.Find("Cloud").transform;
		Transform House = PopManage.Instance.WorldMap.transform.Find("House").transform;
		Camera worldCamera = PopManage.Instance.WorldMap.transform.Find("Camera").GetComponent<Camera>();

		//记录已经开启的地图区域;
		List<String> open_regions = new List<String>();		
		if (only_explore == false){
			foreach(UserRegions s in DataManager.GetInstance.userRegionsList.Values){							
				if (!open_regions.Contains(s.regions_name)){
					//已开启的地图区域;
					open_regions.Add(s.regions_name);
				}
				
				//初始化岛数据;
				if (s.is_npc != 9 || (s.is_npc == 9 && s.is_collect == 0)){

					Transform house_pos = House.Find(s.regions_id.ToString());
					Transform obj = house_pos.Find("islandHouse");
					GameObject islandHouse = null;
					if (obj == null){
						islandHouse = Instantiate(ResourceCache.load("UI/islandHouse")) as GameObject;
						islandHouse.name = "islandHouse";
					}else{
						islandHouse = obj.gameObject;
					}

					/*
					while(house_pos.GetChildCount() > 0){
						Destroy(house_pos.GetChild(0).gameObject);
					}
		*/
					islandHouse.transform.parent = house_pos;
					islandHouse.transform.localPosition = Vector3.zero;
					
					WorldHouse wh = islandHouse.GetComponent<WorldHouse>();
					s.worldHouse = wh;
					wh.initData(s,worldCamera);
				}
			}
		}else{
			foreach(UserRegions s in DataManager.GetInstance.userRegionsList.Values){							
				if (!open_regions.Contains(s.regions_name)){
					//已开启的地图区域;
					open_regions.Add(s.regions_name);
				}
			}
		}
		
		int map_room_level = Helper.getTidMaxLevel("TID_BUILDING_MAP_ROOM",0);
		//Debug.Log("map_room_level:" + map_room_level);

		//open_regions.Clear();



		//初始化云朵数据;
		foreach(Regions r in  CSVManager.GetInstance.regionsList.Values){
			Transform icloud = Cloud.Find(r.Name);
			BoxCollider box = icloud.GetComponent<BoxCollider>();
			if (box != null) box.enabled = false;

			tk2dUIItem tkUIItem = icloud.GetComponent<tk2dUIItem>();
			tkUIItem.sendMessageTarget = PopManage.Instance.WorldMap;
			tkUIItem.SendMessageOnClickMethodName = "OnExplorationClick";

			//r.status = 0;//0:不可开启;1:已开启;2:可开启(但未开启);
			if (r.RequiredMapRoomLevel <= map_room_level){
				if (open_regions.Contains(r.Name)){
					r.status = 1;//1:已开启;
				}else{
					r.status = 2;//2:可开启(但未开启);
				}
				//Debug.Log("r.name:" + r.Name + ";r.status:" + r.status + ";lv:" + r.RequiredMapRoomLevel);
			}else{
				r.status = 0;
				//不可开启;
			}
			

			r.cloud = icloud;
			if (icloud != null){
				//Debug.Log(r.Name + ";r.status:" + r.status + ";r.RequiredMapRoomLevel:" + r.RequiredMapRoomLevel);

				if (r.status == 2 || (r.status == 0 && r.RequiredMapRoomLevel == map_room_level + 1)){
					//2:可开启(但未开启);0:不可开启;
					//TID_MAP_ROOM_LEVEL_REQUIRED = Radar level <number> needed!;
					r.desc = StringFormat.FormatByTid("TID_MAP_ROOM_LEVEL_REQUIRED", new object[]{r.RequiredMapRoomLevel});
					//Debug.Log(r.desc);
					GameObject Explore = r.Explore;
					if (Explore == null){
						Explore = Instantiate(ResourceCache.load("UI/Explore")) as GameObject;
						Explore.name = "Explore";
						r.Explore = Explore;
					}
					//Debug.Log(Explore.name);

					Transform ExplorePos = icloud.Find("Explore");
					if (ExplorePos == null){
						ExplorePos = icloud;
					}



					if (box != null){
						box.center = ExplorePos.localPosition;
						box.size = new Vector3(0.06f,0.03f,0.02f);
					} 

					Explore.transform.parent = ExplorePos;
					Explore.transform.localPosition = Vector3.zero;
					Explore.SetActive(true);
					//Debug.Log("dddd:1");
					r.send_sprite = Explore.transform.Find("send_sprite").gameObject;
					r.gold_sprite = Explore.transform.Find("gold_sprite").gameObject;
					r.exploration_cost = Explore.transform.Find("ExplorationCost").gameObject;
					//Debug.Log("dddd:2");
					r.send_sprite.SetActive(false);
					//Debug.Log("dddd:3");
					if (r.status == 2){
						//2:可开启(但未开启);
						if (box != null) box.enabled = true;
						//gold_sprite,ExplorationCost,desc,send_sprite
						Explore.transform.Find("gold_sprite").gameObject.SetActive(true);
						Explore.transform.Find("ExplorationCost").gameObject.SetActive(true);
						Explore.transform.Find("ExplorationCost").GetComponent<tk2dTextMesh>().text = r.ExplorationCost.ToString();
						Explore.transform.Find("desc").gameObject.SetActive(false);
					}else{
						//0:不可开启,提示雷达几级后可开启;
						Explore.transform.Find("gold_sprite").gameObject.SetActive(false);
						Explore.transform.Find("ExplorationCost").gameObject.SetActive(false);
						Explore.transform.Find("desc").gameObject.SetActive(true);
						//Explore.transform.Find("desc").GetComponent<EasyFontTextMesh>().Text = r.desc;
					}
				}



				if (r.status == 1){
					icloud.gameObject.SetActive(false);
				}else{
					icloud.gameObject.SetActive(true);
				}
			}
		}
	}
	

	//将攻击日志发到服务器;
	//immediately_send:true 立即执行发送到服务器;false 批量处理;
	public static void SendAttackLog(bool immediately_send,Queue<ReplayNodeData> BattleCommondQueue){

		//Debug.Log("SendAttackLog");
		int icount = BattleCommondQueue.Count;
		if (icount > 0){
			bool f = false;

			if (immediately_send == false){
				//所有可移动的建筑列表，不含树，石，船;
				int build_num = DataManager.GetInstance.buildArray.Count;
				int dead_num = 0;

				for(int k = 0; k < build_num; k++){
					BuildInfo bi = DataManager.GetInstance.buildArray[k];
					if (bi.IsDead){
						//已被摧毁;
						dead_num ++;
						if (bi.tid.Equals("TID_BUILDING_PALACE")){
							dead_num = build_num;
							break;
						}
					}
				}	

				int overall_damage_level = 0;
				int overall = (int)(dead_num*1f/build_num*100f);
				if (overall >= 100){
					overall_damage_level = 4;
				}else if (overall >= 80){
					overall_damage_level = 3;
				}else if (overall >= 10){
					overall_damage_level = 2;
				}else if (overall >= 5){
					overall_damage_level = 1;
				}

				//Debug.Log("overall:" + overall + ";overall_damage_level:" + overall_damage_level + ";dead_num:" + dead_num + ";build_num:" + build_num);

				if (overall_damage_level > Globals.overall_damage_level){
					//30%,50%,80%,100%
					Globals.overall_damage_level = overall_damage_level;
					f = true;
				}else{
					f = false;
				}
			}else{
				f = true;
			}

			//Debug.Log("SendAttackLog:" + f);

			if (f){
				int i = 0;
				ISFSObject data = new SFSObject();
				ISFSObject record = new SFSObject();

				int[] TimeFromBeginArray = new int[icount];    //开始后的时间(Time.delta*1000);
				int[] SelfTypeArray = new int[icount];  //当前数据的对象类型;
				int[] SelfIDArray = new int[icount];           //对象ID，用于识别相应的TID_LEVEL;
				float[] StandXArray = new float[icount];		 //当前攻击时站立的x坐标;
				float[] StandZArray = new float[icount];		 //当前攻击时站立的z坐标;
				float[] HitPointsArray = new float[icount];  	 //当前被扣血量;
				int[] IsUnderAttackArray = new int[icount];    //是否当下正被攻击，1:显示血条;
				float[] DestXArray = new float[icount];			 //用于寻路、攻击的目标点x;
				float[] DestZArray = new float[icount];			 //用于寻路、攻击的目标点z;
				int[] StateArray = new int[icount];  		 //当前的状态机;
				int[] AttackStateArray = new int[icount];  //当前的第二状态机(攻击专用的状态机);
				int[] IsRetreatArray = new int[icount];        //是否撤退0:否 1:是;
				int[] AttackTypeArray = new int[icount];        //被攻击的目标类型;
				int[] AttackIDArray = new int[icount];        //被攻击的目标ID;
				String[] walkListArray = new String[icount];  	 //行走过的路线格子;
				int[] IsInStunArray = new int[icount];
				int[] IsInSmokeArray = new int[icount];

				while(BattleCommondQueue.Count > 0 && i < icount){
					ReplayNodeData replayData = BattleCommondQueue.Dequeue();

					TimeFromBeginArray[i] = replayData.TimeFromBegin;    //开始后的时间(Time.delta*1000);
					SelfTypeArray[i] = (int)replayData.SelfType;  //当前数据的对象类型;
					SelfIDArray[i] = replayData.SelfID;           //对象ID，用于识别相应的TID_LEVEL;
					StandXArray[i] = replayData.StandX;		 //当前攻击时站立的x坐标;
					StandZArray[i] = replayData.StandZ;		 //当前攻击时站立的z坐标;
					HitPointsArray[i] = replayData.HitPoints;  	 //当前被扣血量;
					IsUnderAttackArray[i] = replayData.IsUnderAttack;    //是否当下正被攻击，1:显示血条;
					DestXArray[i] = replayData.DestX;			 //用于寻路、攻击的目标点x;
					DestZArray[i] = replayData.DestZ;			 //用于寻路、攻击的目标点z;
					StateArray[i] = (int)replayData.State;  		 //当前的状态机;
					AttackStateArray[i] = (int)replayData.AttackState;  //当前的第二状态机(攻击专用的状态机);
					IsInStunArray[i] = replayData.IsInStun;
					IsInSmokeArray[i] = replayData.IsInSmoke;
					//IsRetreatArray[i] = replayData.IsRetreat;        //是否撤退0:否 1:是;

					AttackTypeArray[i] = (int)replayData.AttackType;        //被攻击的目标类型;
					AttackIDArray[i] = replayData.AttackID;        //被攻击的目标ID;

					walkListArray[i] = replayData.walkList.ToString();  	 //行走过的路线格子;

					i++;
				}
				//Debug.Log("loop:"+i);
				record.PutIntArray("TimeFromBegin",TimeFromBeginArray);//开始后的时间(Time.delta*1000);
				record.PutIntArray("SelfType",SelfTypeArray);  //当前数据的对象类型;
				record.PutIntArray("SelfID",SelfIDArray);           //对象ID，用于识别相应的TID_LEVEL;
				record.PutFloatArray("StandX",StandXArray);		 //当前攻击时站立的x坐标;
				record.PutFloatArray("StandZ",StandZArray);		 //当前攻击时站立的z坐标;
				record.PutFloatArray("HitPoints",HitPointsArray);  	 //当前被扣血量;
				record.PutIntArray("IsUnderAttack",IsUnderAttackArray);    //是否当下正被攻击，1:显示血条;
				record.PutFloatArray("DestX",DestXArray);			 //用于寻路、攻击的目标点x;
				record.PutFloatArray("DestZ",DestZArray);			 //用于寻路、攻击的目标点z;
				record.PutIntArray("State",StateArray);  		 //当前的状态机;
				record.PutIntArray("AttackState",AttackStateArray);  //当前的第二状态机(攻击专用的状态机);
				record.PutIntArray("IsRetreat",IsRetreatArray);        //是否撤退0:否 1:是;
				record.PutIntArray("AttackType",AttackTypeArray);        //被攻击的目标类型;
				record.PutIntArray("AttackID",AttackIDArray);        //被攻击的目标ID;
				record.PutUtfStringArray("walkList",walkListArray);  	 //行走过的路线格子;

				record.PutIntArray("IsInStun",IsInStunArray);     
				record.PutIntArray("IsInSmoke",IsInSmokeArray);   
				data.PutSFSObject("record",record);

				SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_ATTACK_LOG, false, null);
			}
		}
	}

	//检查建筑物及保存对BattleID<=>building_id照表;
	public static void SendAttackCheck(){

		int icount = 0;
		int k = 0;
		Dictionary<string,CsvInfo> csv = new Dictionary<string,CsvInfo>();

		Dictionary<long,int> BattleIDs = new Dictionary<long,int>();

		//所有兵用到的csv数据;
		for(int i=0;i<Globals.battleTrooperList.Count;i++)
		{
			BattleTrooperData btd = Globals.battleTrooperList[i];
			if (!csv.ContainsKey(btd.csvInfo.TID_Level)){
				csv.Add(btd.csvInfo.TID_Level,btd.csvInfo);
			}

			//登陆艇;
			if (!BattleIDs.ContainsKey(btd.building_id)){
				BattleIDs.Add(btd.building_id,btd.id);
			}
			//btd.building_id
		}

		icount = BattleIDs.Count + DataManager.GetInstance.buildList.Count;

		int[] BattleID = new int[icount];//攻击时的建筑对照ID;
		long[] building_id = new long[icount];
		int[] type = new int[icount];//0:被攻击方(暂时最多只有8个登陆艇)；1：攻击方;

		k = 0;
		foreach(long build_id in BattleIDs.Keys){
			BattleID[k] = BattleIDs[build_id];
			building_id[k] = build_id;
			type[k] = 0;
			k ++;
		}


		//所有建筑物用到的csv数据;
		foreach(BuildInfo s in DataManager.GetInstance.buildList.Values){
			if (!csv.ContainsKey(s.csvInfo.TID_Level)){
				csv.Add(s.csvInfo.TID_Level,s.csvInfo);
			}
			BattleID[k] = s.BattleID;
			building_id[k] = s.building_id;
			type[k] = 1;
			k ++;
		}

		ISFSObject Battle = new SFSObject();		
		Battle.PutIntArray("BattleID",BattleID);
		Battle.PutLongArray("building_id",building_id);
		Battle.PutIntArray("type",type);


		//所有导弹用的csv数据;
		for(int i=0;i<DataManager.GetInstance.battleEnergyList.Count;i++)
		{
			BattleTrooperData btd = DataManager.GetInstance.battleEnergyList[i];
			if (!csv.ContainsKey(btd.csvInfo.TID_Level)){
				csv.Add(btd.csvInfo.TID_Level,btd.csvInfo);
			}
		}


		icount = csv.Count;
		string[] tid_level = new string[icount];
		int[] Hitpoints = new int[icount];
		int[] Damage = new int[icount];
		int[] AttackRange = new int[icount];
		int[] AttackSpeed = new int[icount];
		int[] MinAttackRange = new int[icount];
		int[] DamageRadius = new int[icount];
		int[] DamageSpread = new int[icount];
		int[] BoostTimeMS = new int[icount];
		int[] RandomRadius = new int[icount];
		int[] RadiusAgainstTroops = new int[icount];
		int[] TimeBetweenHitsMS = new int[icount];
		int[] Radius = new int[icount];
		int[] EnergyIncrease = new int[icount];
		int[] Energy = new int[icount];



		k = 0;
		foreach(CsvInfo csvInfo in csv.Values){
			tid_level[k] = csvInfo.TID_Level;
			Hitpoints[k] = csvInfo.Hitpoints;
			Damage[k] = csvInfo.Damage;
			AttackRange[k] = csvInfo.AttackRange;
			AttackSpeed[k] = csvInfo.AttackSpeed;
			MinAttackRange[k] = csvInfo.MinAttackRange;
			DamageRadius[k] = csvInfo.DamageRadius;
			DamageSpread[k] = csvInfo.DamageSpread;
			BoostTimeMS[k] = csvInfo.BoostTimeMS;
			RandomRadius[k] = csvInfo.RandomRadius;
			RadiusAgainstTroops[k] = csvInfo.RadiusAgainstTroops;
			TimeBetweenHitsMS[k] = csvInfo.TimeBetweenHitsMS;
			Radius[k] = csvInfo.Radius;
			EnergyIncrease[k] = csvInfo.EnergyIncrease;
			Energy[k] = csvInfo.Energy;
			
			k ++;
		}

		ISFSObject localcsv = new SFSObject();


		localcsv.PutUtfStringArray("tid_level",tid_level);
		localcsv.PutIntArray("Hitpoints",Hitpoints);
		localcsv.PutIntArray("Damage",Damage);
		localcsv.PutIntArray("AttackRange",AttackRange);
		localcsv.PutIntArray("AttackSpeed",AttackSpeed);
		localcsv.PutIntArray("MinAttackRange",MinAttackRange);
		localcsv.PutIntArray("DamageRadius",DamageRadius);
		localcsv.PutIntArray("DamageSpread",DamageSpread);
		localcsv.PutIntArray("BoostTimeMS",BoostTimeMS);
		localcsv.PutIntArray("RandomRadius",RandomRadius);
		localcsv.PutIntArray("RadiusAgainstTroops",RadiusAgainstTroops);
		localcsv.PutIntArray("TimeBetweenHitsMS",TimeBetweenHitsMS);
		localcsv.PutIntArray("Radius",Radius);
		localcsv.PutIntArray("EnergyIncrease",EnergyIncrease);
		localcsv.PutIntArray("Energy",Energy);


		ISFSObject data = new SFSObject();
		data.PutSFSObject("localcsv",localcsv);
		data.PutSFSObject("Battle",Battle);
		SFSNetworkManager.Instance.SendRequest(data, ApiConstant.CMD_ATTACK_CHECK, false, null);

	}

	//部署军队同时较验军队数据;
	public static void SendAttackDeployTroops(BattleTrooperData btd){
		//Debug.Log("SendAttackDeployTroops");
		ISFSObject data = new SFSObject();
		data.PutLong("building_id", btd.building_id);
		data.PutInt("troops_num",btd.num);
		data.PutUtfString("tid_level",btd.tidLevel);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_ATTACK_DEPLOY_TROOPS, false, null);
	}

	//有兵死亡时,通知服务器;
	public static void SendAttackDestroyedTroops(int trooper_id, BattleTrooperData btd){
		//Debug.Log("SendAttackDeployTroops");
		ISFSObject data = new SFSObject();
		data.PutInt("trooper_id",trooper_id);
		data.PutLong("building_id", btd.building_id);//运兵船 building_id	
		SFSNetworkManager.Instance.SendRequest(data, "attack_destroyed_troops", false, null);
	}

	//计算资源收集的特效粒子数量;
	public static int GetEmitCount(string res_type,int collect_num){
		int max_num = GetMaxStore (res_type);
		int c = 0;
		if(max_num>0)
		{
			c =  Mathf.CeilToInt(collect_num*100f/max_num);
		}
		if(c>15)c=15;
		int addC = Mathf.RoundToInt(collect_num/500f);
		if(addC>15)addC = 15;
		c+=addC;
		return c;
	}

	//获取最大储量
	public static int GetMaxStore(string res_type){
		int max_num = 0;
		if(res_type==RES_TYPE_GOLD)
		{
			max_num = DataManager.GetInstance.model.user_info.max_gold_count;
		}
		else if(res_type==RES_TYPE_WOOD)
		{
			max_num = DataManager.GetInstance.model.user_info.max_wood_count;
		}
		else if(res_type==RES_TYPE_STONE)
		{
			max_num = DataManager.GetInstance.model.user_info.max_stone_count;
		}
		else if(res_type==RES_TYPE_IRON)
		{
			max_num = DataManager.GetInstance.model.user_info.max_iron_count;
		}
		return max_num;
	}


	public static void EXPCollect(BuildInfo s,int exp_count){
		if (exp_count > 0){
			AudioPlayer.Instance.PlaySfx("xp_gain_06");
			//升级可获得的经验值;
			int c = Mathf.CeilToInt(exp_count*1f/3f);
			s.emitter.enabled = true;
			s.emitter.Emit(c,s.transform.position,PartType.Exp,exp_count);
			//Helper.setResourceCount("Exp", exp_count);
		}
	}
}
