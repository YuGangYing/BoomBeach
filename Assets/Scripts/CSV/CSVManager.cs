using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSV;
using System.IO;

public class CSVManager : SingleMonoBehaviour<CSVManager>
{

	private const string CSV_ISLANDGRID = @"m_island_grid";
	private const string CSV_UNIT = @"m_unit";

	private CsvContext mCsvContext;

	public List<MIslandGridCSVStructure> islandGridList;
	public Dictionary<string,int[,]> islandGridsDic;

	public List<MUnitCSVStructure> unitList;
	public Dictionary<int,MUnitCSVStructure> unitIdDic;
	public Dictionary<string,MUnitCSVStructure> unitTidLevelDic;


	void LoadCSV ()
	{
		mCsvContext = new CsvContext ();
		LoadIslandGrid ();
		LoadUnit ();
	}

	byte[] GetCSV (string fileName)
	{
		#if UNITY_EDITOR
		return Resources.Load<TextAsset> ("CSV/" + fileName).bytes;
		#else
		return Resources.Load<TextAsset> ("csvs/" + fileName).bytes;
//		return ResourcesManager.Ins.GetCSV (fileName);
		#endif
	}

	const int GRID_TOTAL = 50;
	//加载岛的不可通行区配置;
	void LoadIslandGrid ()
	{
		islandGridList = CreateCSVList<MIslandGridCSVStructure> (CSV_ISLANDGRID);
		islandGridsDic = new Dictionary<string, int[,]> ();
		string last_island_name = null;
		int[,] grid = null;//new int[40,40];
		for (int i = 1; i < islandGridList.Count; i++) {			

			if (!islandGridsDic.ContainsKey (islandGridList [i].name)) {
				islandGridsDic.Add (islandGridList [i].name, new int[GRID_TOTAL, GRID_TOTAL]);
			}
			int x = int.Parse (islandGridList [i].x.Trim ());
			int y = int.Parse (islandGridList [i].y.Trim ());
			islandGridsDic [islandGridList [i].name] [x, y] = 1;
		}
	}

	void LoadUnit ()
	{
		unitList = CreateCSVList<MUnitCSVStructure> (CSV_UNIT);
		unitIdDic = GetDictionary<MUnitCSVStructure> (unitList);
		unitTidLevelDic = new Dictionary<string, MUnitCSVStructure> ();
		for (int i = 0; i < unitList.Count; i++) {
			if(!unitTidLevelDic.ContainsKey(unitList [i].tid.Trim() + "_" + unitList [i].level)){
				unitTidLevelDic.Add (unitList [i].tid.Trim() + "_" + unitList [i].level, unitList [i]);
			}
		}
	}

	public List<T> CreateCSVList<T> (string csvname) where T:BaseCSVStructure, new()
	{
		var stream = new MemoryStream (GetCSV (csvname));
		var reader = new StreamReader (stream);
		IEnumerable<T> list = mCsvContext.Read<T> (reader);
		return new List<T> (list);
	}

	Dictionary<int,T> GetDictionary<T> (List<T> list) where T : BaseCSVStructure
	{
		Dictionary<int,T> dic = new Dictionary<int, T> ();
		foreach (T t in list) {
			if (!dic.ContainsKey (t.id))
				dic.Add (t.id, t);
			else
				Debug.Log (string.Format ("Multi key:{0}{1}", typeof(T).ToString (), t.id).YellowColor ());
		}
		return dic;
	}

	public MUnitCSVStructure GetUnit(string tid_level){
		MUnitCSVStructure unit = null;
		if(unitTidLevelDic.ContainsKey(tid_level)){
			unit = unitTidLevelDic[tid_level];
		}
		return unit;
	}

	public MUnitCSVStructure GetUnit (string tid, int level)
	{
		MUnitCSVStructure unit = null;
		string tid_level = tid.Trim () + "_" + level;
		return GetUnit (tid_level);
	}

	public int[,] GetGrid(string gridName){
		int[,] grid = null;
		islandGridsDic.TryGetValue (gridName,out grid);
		return grid;
	}

	public Hashtable regionsList = new Hashtable ();
	//存放了r0~r149 共150朵云; Regions //name 为key
	public Hashtable experienceLevelsList = new Hashtable ();
	//经验列表;;ExperienceLevels；name 为key即：经验等级;
	public Hashtable achievementsList = new Hashtable ();
	//成孰数据;
	//	public Dictionary<string,int[,]> islandGridsDic = new Dictionary<string,int[,]> ();
	public Hashtable csvTable = new Hashtable ();
	//以tid_level 为索引,CsvInfo为类;

	protected override void Awake ()
	{
		base.Awake ();
		//Init ();
		LoadCSV ();
	}

	public void Init ()
	{
		Helper.initNameToTid ();
		CSVManager.GetInstance.csvTable.Clear ();

		//TextAsset achievements = Resources.Load<TextAsset>(@"csv/achievements");
		TextAsset artifact_bonuses = Resources.Load<TextAsset> (@"csv/artifact_bonuses");
		TextAsset artifacts = Resources.Load<TextAsset> (@"csv/artifacts");
		TextAsset buildings = Resources.Load<TextAsset> (@"csv/buildings");
		TextAsset characters = Resources.Load<TextAsset> (@"csv/characters");
		TextAsset decos = Resources.Load<TextAsset> (@"csv/decos");
		TextAsset experience_levels = Resources.Load<TextAsset> (@"csv/experience_levels");
		TextAsset globals = Resources.Load<TextAsset> (@"csv/globals");
		TextAsset island_grid = Resources.Load<TextAsset> (@"csv/islandgrid");
		//TextAsset liberated_income = Resources.Load<TextAsset>(@"csv/liberated_income");
		//TextAsset npcs = Resources.Load<TextAsset>(@"csv/npcs");
		TextAsset obstacles = Resources.Load<TextAsset> (@"csv/obstacles");
		TextAsset projectiles = Resources.Load<TextAsset> (@"csv/projectiles");
		TextAsset regions = Resources.Load<TextAsset> (@"csv/regions");
		TextAsset spells = Resources.Load<TextAsset> (@"csv/spells");
		TextAsset townhall_levels = Resources.Load<TextAsset> (@"csv/townhall_levels");
		TextAsset traps = Resources.Load<TextAsset> (@"csv/traps");

		Helper.LoadIsLandCsv (island_grid);

		Helper.loadcsv (experience_levels, CSVManager.GetInstance.experienceLevelsList, "ExperienceLevels", false, false);

		Helper.loadcsv (regions, CSVManager.GetInstance.regionsList, "REGIONS", false, false);

		Helper.loadcsv (buildings, CSVManager.GetInstance.csvTable, "BUILDING", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv (townhall_levels, CSVManager.GetInstance.csvTable, "BUILDING", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);

		Helper.loadcsv (characters, CSVManager.GetInstance.csvTable, "CHARACTERS", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv (decos, CSVManager.GetInstance.csvTable, "DECOS", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv (obstacles, CSVManager.GetInstance.csvTable, "OBSTACLES", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv (spells, CSVManager.GetInstance.csvTable, "SPELLS", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv (traps, CSVManager.GetInstance.csvTable, "TRAPS", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);

		//artifacts与buildings中的3个Artifact需要注意区分与关联;
		Helper.loadcsv (artifacts, CSVManager.GetInstance.csvTable, "ARTIFACTS", true, false);//没有TID，这个需要补TID，把name转为tid

		//Debug.Log(CSVManager.GetInstance.csvTable.Count);
		Helper.loadcsv (artifact_bonuses, CSVManager.GetInstance.csvTable, "ARTIFACT_BONUSES", true, false);
		//Debug.Log(CSVManager.GetInstance.csvTable.Count);

		Globals.projectileData.Clear ();
		Hashtable ProjectilesList = new Hashtable ();
		Helper.loadcsv (projectiles, ProjectilesList, "PROJECTILES", true, false);
		foreach (string key in ProjectilesList.Keys) {
			Projectiles val = ProjectilesList [key] as Projectiles;
			//Debug.Log("key:" + key + ";val.name:" + val.Name + ";val.Speed:" + val.Speed);
			Globals.projectileData.Add (key, val);
		}		
		Globals.GlobalsCsv.Clear ();
		Hashtable GlobalsCsvList = new Hashtable ();
		Helper.loadcsv (globals, GlobalsCsvList, "GLOBALS", true, false);
		foreach (string key in GlobalsCsvList.Keys) {
			GlobalsItem val = GlobalsCsvList [key] as GlobalsItem;
			Globals.GlobalsCsv.Add (key, val);
		}
	}

	public CsvInfo GetBuildingData (string type, int level)
	{
		string tid_level = type + "_" + level;
		return this.csvTable [tid_level] as CsvInfo;
	}

}
