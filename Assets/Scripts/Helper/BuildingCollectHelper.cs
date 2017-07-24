using UnityEngine;
using System.Collections;

public static class BuildingCollectHelper  {
	//获得可采集的数量;
	public static int getCollectNum(BuildInfo s, int collect_time=0,bool show_log = false){
		
		//当点：加速时,会先执行一次采集,所以 s.last_collect_time >= s.boost_start_time
		//BuildInfo s = (BuildInfo)Globals.BuildList[building_id];
		if (s == null){
			//Debug.Log1("s is null getCollectNum:building_id:" + building_id);
			return 0;
		}
		CsvInfo csvData = s.csvInfo;// (CsvInfo)Globals.csvTable[s.tid_level];
		if (collect_time == 0){
			//collect_time = current_time();
		}
		int collect_num = 0;
		//ArtifactType artifact_type = ArtifactType.None;
		int rph = csvData.ResourcePerHour;
		//获得神像加成值;
		//artifact_type:神像类型;	1BoostGold;2BoostWood;3BoostStone;4BoostMetal;5BoostTroopHP;6BoostBuildingHP;7BoostTroopDamage;8BoostBuildingDamage;
		//orgValue基础值;
		/*
		if (s.tid == "TID_BUILDING_HOUSING"){
			artifact_type = ArtifactType.BoostGold;
		}else if (s.tid == "TID_BUILDING_WOODCUTTER"){
			artifact_type = ArtifactType.BoostWood;
		}else if (s.tid == "TID_BUILDING_STONE_QUARRY"){
			artifact_type = ArtifactType.BoostStone;
		}else if (s.tid == "TID_BUILDING_METAL_MINE"){
			artifact_type = ArtifactType.BoostMetal;
		}
		*/
		int artifact_num = 0;// = getArtifactBoost(csvData.ResourcePerHour, artifact_type);
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


}
