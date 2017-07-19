using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSVManager : SingleMonoBehaviour<CSVManager>{

	public Hashtable regionsList = new Hashtable();//存放了r0~r149 共150朵云; Regions //name 为key
	public Hashtable experienceLevelsList = new Hashtable();//经验列表;;ExperienceLevels；name 为key即：经验等级;
	public Hashtable achievementsList = new Hashtable();//成孰数据;
	public Dictionary<string,int[,]> island_grid_csv = new Dictionary<string,int[,]>();
	public Hashtable csvTable = new Hashtable();//以tid_level 为索引,CsvInfo为类;

	protected override void Awake ()
	{
		base.Awake ();
		Init ();
	}

	public void Init(){
		Helper.initNameToTid();
		CSVManager.GetInstance.csvTable.Clear();

		//TextAsset achievements = Resources.Load<TextAsset>(@"csv/achievements");
		TextAsset artifact_bonuses = Resources.Load<TextAsset>(@"csv/artifact_bonuses");
		TextAsset artifacts = Resources.Load<TextAsset>(@"csv/artifacts");
		TextAsset buildings = Resources.Load<TextAsset>(@"csv/buildings");
		TextAsset characters = Resources.Load<TextAsset>(@"csv/characters");
		TextAsset decos = Resources.Load<TextAsset>(@"csv/decos");
		TextAsset experience_levels = Resources.Load<TextAsset>(@"csv/experience_levels");
		TextAsset globals = Resources.Load<TextAsset>(@"csv/globals");
		TextAsset island_grid = Resources.Load<TextAsset>(@"csv/islandgrid");
		//TextAsset liberated_income = Resources.Load<TextAsset>(@"csv/liberated_income");
		//TextAsset npcs = Resources.Load<TextAsset>(@"csv/npcs");
		TextAsset obstacles = Resources.Load<TextAsset>(@"csv/obstacles");
		TextAsset projectiles = Resources.Load<TextAsset>(@"csv/projectiles");
		TextAsset regions = Resources.Load<TextAsset>(@"csv/regions");
		TextAsset spells = Resources.Load<TextAsset>(@"csv/spells");
		TextAsset townhall_levels = Resources.Load<TextAsset>(@"csv/townhall_levels");
		TextAsset traps = Resources.Load<TextAsset>(@"csv/traps");

		Helper.LoadIsLandCsv(island_grid);

		Helper.loadcsv(experience_levels,CSVManager.GetInstance.experienceLevelsList,"ExperienceLevels", false,false);

		Helper.loadcsv(regions,CSVManager.GetInstance.regionsList,"REGIONS", false,false);

		Helper.loadcsv(buildings,CSVManager.GetInstance.csvTable,"BUILDING", true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv(townhall_levels,CSVManager.GetInstance.csvTable,"BUILDING",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);

		Helper.loadcsv(characters,CSVManager.GetInstance.csvTable,"CHARACTERS",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv(decos,CSVManager.GetInstance.csvTable,"DECOS",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv(obstacles,CSVManager.GetInstance.csvTable,"OBSTACLES",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv(spells,CSVManager.GetInstance.csvTable,"SPELLS",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv(traps,CSVManager.GetInstance.csvTable,"TRAPS",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);

		//artifacts与buildings中的3个Artifact需要注意区分与关联;
		Helper.loadcsv(artifacts,CSVManager.GetInstance.csvTable,"ARTIFACTS",true,false);//没有TID，这个需要补TID，把name转为tid

		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv(artifact_bonuses,CSVManager.GetInstance.csvTable,"ARTIFACT_BONUSES",true,false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);

		Globals.projectileData.Clear();
		Hashtable ProjectilesList = new Hashtable();
		Helper.loadcsv(projectiles,ProjectilesList,"PROJECTILES",true,false);
		foreach(string key in ProjectilesList.Keys){
			Projectiles val = ProjectilesList[key] as Projectiles;
			//Debug.Log("key:" + key + ";val.name:" + val.Name + ";val.Speed:" + val.Speed);
			Globals.projectileData.Add(key,val);
		}		
		Globals.GlobalsCsv.Clear();
		Hashtable GlobalsCsvList = new Hashtable();
		Helper.loadcsv(globals,GlobalsCsvList,"GLOBALS",true,false);
		foreach(string key in GlobalsCsvList.Keys){
			GlobalsItem val = GlobalsCsvList[key] as GlobalsItem;
			//Debug.Log("key:" + key + ";val.TextValue:" + val.TextValue + ";val.NumberValue:" + val.NumberValue);
			Globals.GlobalsCsv.Add(key,val);
		}
		//Debug.Log("TOWN_HALL_DAMAGE_FROM_OTHER_BUILDINGS_PERCENT:" + Globals.GlobalsCsv["TOWN_HALL_DAMAGE_FROM_OTHER_BUILDINGS_PERCENT"].NumberValue);
		//Debug.Log("TOWN_HALL_MAX_DAMAGE_FROM_ONE_BUILDING_PERCENT:" + Globals.GlobalsCsv["TOWN_HALL_MAX_DAMAGE_FROM_ONE_BUILDING_PERCENT"].NumberValue);
	}

}
