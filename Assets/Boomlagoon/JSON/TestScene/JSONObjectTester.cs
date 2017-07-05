using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine;
using Sfs2X.Entities.Data;
using BoomBeach;

public class JSONObjectTester : MonoBehaviour {

	public GUIText infoText;
	public JSONObject buildObject;
	//private string stringToEvaluate = "";
	public TextAsset[]  npccsv;
	public int index = 0;

	public void dataToTid(){
		//select  CONCAT('buildObject.Add("',data,'","',tid,'");') as dt, tid,data from fanwe_name_to_tid_copy where data != ''
		buildObject = new JSONObject();
		buildObject.Add("1000000","TID_BUILDING_PALACE");
		buildObject.Add("1000001","TID_BUILDING_HOUSING");
		buildObject.Add("1000002","TID_BUILDING_WOODCUTTER");
		buildObject.Add("1000003","TID_BUILDING_STONE_QUARRY");
		buildObject.Add("1000004","TID_BUILDING_METAL_MINE");
		buildObject.Add("1000005","TID_BUILDING_GOLD_STORAGE");
		buildObject.Add("1000006","TID_BUILDING_WOOD_STORAGE");
		buildObject.Add("1000007","TID_BUILDING_STONE_STORAGE");
		buildObject.Add("1000008","TID_BUILDING_METAL_STORAGE");//未确认;small_b_32;1000008 is not
		buildObject.Add("1000009","TID_BUILDING_VAULT");//mainland_a_25;1000009 is not
		buildObject.Add("1000010","TID_BUILDING_MAP_ROOM");

		buildObject.Add("1000014","TID_BUILDING_ARTIFACT_WORKSHOP");//Boss fight 5;1000014is not

		buildObject.Add("1000015","TID_BUILDING_GUNSHIP");
		buildObject.Add("1000016","TID_AI_HUT");
		buildObject.Add("1000017","TID_BUILDING_LANDING_SHIP");

		buildObject.Add("1000020","TID_GUARD_TOWER");
		buildObject.Add("1000021","TID_BUILDING_MORTAR");
		buildObject.Add("1000022","TID_MACHINE_GUN_NEST");
		buildObject.Add("1000023","TID_MISSILE_LAUNCHER");
		buildObject.Add("1000024","TID_FLAME_THROWER");
		buildObject.Add("1000025","TID_BUILDING_COMMAND_CENTER");
		buildObject.Add("1000026","TID_BUILDING_CANNON");

		buildObject.Add("1000027","TID_BUILDING_COMMAND_CENTER");//tutorial2;1000027is not

		buildObject.Add("1000028","TID_BUILDING_BIG_BERTHA");
		buildObject.Add("1000029","TID_BUILDING_BOSS_MACHINE_GUN");//Boss fight 2;1000029is not
		buildObject.Add("1000030","TID_BUILDING_BOSS_MORTAR");//Boss fight 3;1000030is not

		buildObject.Add("1000031","TID_AI_BUNKER");
		buildObject.Add("1000032","TID_CRATES1");
		buildObject.Add("1000033","TID_CRATES2");
		buildObject.Add("1000034","TID_CRATES3");
		buildObject.Add("1000035","TID_CRATES4");
		buildObject.Add("1000036","TID_CRATES5");
		buildObject.Add("18000000","TID_ISLANDER_CAGE");
		buildObject.Add("18000001","TID_ISLANDER_HUT");
		buildObject.Add("18000002","TID_DAMAGED_HUT1");
		buildObject.Add("18000003","TID_DAMAGED_HUT2");
		buildObject.Add("18000004","TID_DAMAGED_HUT3");
		buildObject.Add("18000005","TID_CAMPFIRE");
		buildObject.Add("18000006","TID_DEBRIS2");
		buildObject.Add("18000007","TID_DEBRIS3");
		buildObject.Add("18000008","TID_DEBRIS4");
		buildObject.Add("18000009","TID_DEBRIS5");
		buildObject.Add("12000000","TID_TRAP_MINE");
		buildObject.Add("12000001","TID_TRAP_TANK_MINE");//med_a_84;12000001 is not;
		buildObject.Add("8000000","TID_RESOURCE_OBSTACLE_WOOD");
		buildObject.Add("8000001","TID_RESOURCE_OBSTACLE_WOOD2");
		buildObject.Add("8000002","TID_RESOURCE_OBSTACLE_WOOD3");
		buildObject.Add("8000003","TID_RESOURCE_OBSTACLE_WOOD3B");
		buildObject.Add("8000004","TID_RESOURCE_OBSTACLE_STONE");
		buildObject.Add("8000005","TID_RESOURCE_OBSTACLE_STONE2");
		buildObject.Add("8000006","TID_RESOURCE_OBSTACLE_WOOD1B");
		buildObject.Add("8000007","TID_RESOURCE_OBSTACLE_WOOD1C");
		buildObject.Add("8000008","TID_RESOURCE_OBSTACLE_WOOD2B");

	}

	void Start() {
		dataToTid();
		/*
		infoText.gameObject.SetActive(false);

		//JSONObject usage example:

		//Parse string into a JSONObject:
		JSONObject jsonObject = JSONObject.Parse(stringToEvaluate);

		//You can also create an "empty" JSONObject
		JSONObject emptyObject = new JSONObject();

		//Adding values is easy (values are implicitly converted to JSONValues):
		emptyObject.Add("key", "value");
		emptyObject.Add("otherKey", 123);
		emptyObject.Add("thirdKey", false);
		emptyObject.Add("fourthKey", new JSONValue(JSONValueType.Null));

		//You can iterate through all values with a simple for-each loop
		foreach (KeyValuePair<string, JSONValue> pair in emptyObject) {
			Debug.Log("key : value -> " + pair.Key + " : " + pair.Value);
			
			//Each JSONValue has a JSONValueType that tells you what type of value it is. Valid values are: String, Number, Object, Array, Boolean or Null.
			Debug.Log("pair.Value.Type.ToString() -> " + pair.Value.Type.ToString());

			if (pair.Value.Type == JSONValueType.Number) {
				//You can access values with the properties Str, Number, Obj, Array and Boolean
				Debug.Log("Value is a number: " + pair.Value.Number);
			}
		}

		//JSONObject's can also be created using this syntax:
		JSONObject newObject = new JSONObject {{"key", "value"}, {"otherKey", 123}, {"thirdKey", false}};

		//JSONObject overrides ToString() and outputs valid JSON
		Debug.Log("newObject.ToString() -> " + newObject.ToString());

		//JSONObjects support array accessors
		Debug.Log("newObject[\"key\"].Str -> " + newObject["key"].Str);

		//It also has a method to do the same
		Debug.Log("newObject.GetValue(\"otherKey\").ToString() -> " + newObject.GetValue("otherKey").ToString());

		//As well as a method to determine whether a key exists or not
		Debug.Log("newObject.ContainsKey(\"NotAKey\") -> " + newObject.ContainsKey("NotAKey"));

		//Elements can removed with Remove() and the whole object emptied with Clear()
		newObject.Remove("key");
		Debug.Log("newObject with \"key\" removed: " + newObject.ToString());

		newObject.Clear();
		Debug.Log("newObject cleared: " + newObject.ToString());
		*/
	}
	
	void OnGUI() {
		//stringToEvaluate = GUI.TextArea(new Rect(0, 0, 100, 100), stringToEvaluate);
		//stringToEvaluate = '{"upgrade_outpost_defenses":false,"buildings":[{"y":17,"lvl":0,"data":1000022,"x":27},{"y":23,"lvl":1,"data":1000002,"res_time":24857,"x":29},{"y":9,"lvl":0,"data":1000022,"x":29},{"y":10,"lvl":0,"data":1000025,"x":23}],"decos":[],"traps":[],"map_spawn_timer":0,"obstacles":[{"y":29,"x":16,"data":8000000},{"y":25,"x":19,"data":8000000},{"y":30,"x":20,"data":8000000},{"y":5,"x":17,"data":8000000},{"y":14,"x":15,"data":8000000},{"y":21,"x":17,"data":8000000},{"y":9,"x":19,"data":8000000},{"y":4,"x":21,"data":8000000},{"y":19,"x":20,"data":8000000},{"y":27,"x":24,"data":8000000},{"y":4,"x":26,"data":8000000},{"y":13,"x":19,"data":8000000},{"y":23,"x":5,"data":8000002},{"y":27,"x":7,"data":8000002},{"y":11,"x":5,"data":8000002},{"y":5,"x":12,"data":8000002},{"y":16,"x":5,"data":8000002},{"y":20,"x":7,"data":8000001},{"y":8,"x":7,"data":8000001},{"y":5,"x":9,"data":8000001},{"y":22,"x":10,"data":8000001},{"y":30,"x":11,"data":8000001},{"y":26,"x":12,"data":8000001},{"y":18,"x":11,"data":8000001},{"y":14,"x":10,"data":8000001},{"y":10,"x":11,"data":8000001},{"y":16,"x":15,"data":8000001},{"y":23,"x":14,"data":8000001},{"y":9,"x":14,"data":8000001}],"map_unliberation_timer":0,"resource_ships":[],"seed":112}';
		if (GUI.Button(new Rect(Screen.width - 150, 10, 145, 75), "Evaluate JSON")) {
			string sql = "";
			long building_id = Helper.getNewBuildingId();
			for(int i = 0; i < npccsv.Length; i ++){
				var jsonObject = JSONObject.Parse(npccsv[i].text);
				if (jsonObject == null) {
					Debug.LogError("Failed to parse string, JSONObject == null");
				} else {

					sql += "delete from fanwe_user_grid where user_id = (select id from fanwe_user where is_npc = 1 and player_uid = '" + npccsv[i].name + "');";


					JSONArray buildings = jsonObject.GetArray("buildings");
					sql += CreateSql(buildings,npccsv[i].name,ref building_id);
					JSONArray obstacles = jsonObject.GetArray("obstacles");
					sql += CreateSql(obstacles,npccsv[i].name,ref building_id);
					JSONArray traps = jsonObject.GetArray("traps");
					sql += CreateSql(traps,npccsv[i].name,ref building_id);
					JSONArray decos = jsonObject.GetArray("decos");
					sql += CreateSql(decos,npccsv[i].name,ref building_id);

		
				}
			}

			Debug.Log(sql);
			/*
			var jsonObject = JSONObject.Parse(stringToEvaluate);
			if (jsonObject == null) {
				Debug.LogError("Failed to parse string, JSONObject == null");
			} else {
				Debug.Log("Succesfully created JSONObject");
				Debug.Log(jsonObject.ToString());


				
				JSONArray buildings = jsonObject.GetArray("buildings");
				CreateLocalBuild(buildings);
				JSONArray obstacles = jsonObject.GetArray("obstacles");
				CreateLocalBuild(obstacles);
				JSONArray traps = jsonObject.GetArray("traps");
				CreateLocalBuild(traps);
				JSONArray decos = jsonObject.GetArray("decos");
				CreateLocalBuild(decos);
			}
*/

		}

		if (GUI.Button(new Rect(Screen.width - 150, 95, 145, 75), "Evaluate JSON index")) {
			//stringToEvaluate = string.Empty;

			if (index < npccsv.Length){

				Transform Characters = SpawnManager.GetInstance ().characterContainer;
				Transform Buildings  = BuildManager.GetInstance().buildContainer;

				Globals.Init ();

				WorkerManage.Instance.ClearWorkers();
				//Globals.LandCrafts = new ArrayList();//.Clear();
				//清空旧数据;
				while(Characters.childCount > 0){
					DestroyImmediate(Characters.GetChild(0).gameObject);
				}	
				
				while(Buildings.childCount > 0){
					DestroyImmediate(Buildings.GetChild(0).gameObject);
				}


				var jsonObject = JSONObject.Parse(npccsv[index].text);
				if (jsonObject == null) {
					Debug.LogError("Failed to parse string, JSONObject == null");
				} else {
					//Debug.Log(npccsv[i].name + ":Succesfully created JSONObject");
					string user_name = npccsv[index].name;

					bool f = true;
					JSONArray buildings = jsonObject.GetArray("buildings");
					f = CreateLocalBuild(buildings,user_name);

					JSONArray obstacles = jsonObject.GetArray("obstacles");
					if (CreateLocalBuild(obstacles,user_name) == false){
						f = false;
					}

					JSONArray traps = jsonObject.GetArray("traps");
					if (CreateLocalBuild(traps,user_name) == false){
						f = false;
					}

					JSONArray decos = jsonObject.GetArray("decos");
					if (CreateLocalBuild(decos,user_name) == false){
						f = false;
					}




					if (f){

						string sql = "select id from fanwe_user where is_npc = 1 and player_uid = '" + user_name + "';";
						 sql += "delete from fanwe_user_grid where user_id = (select id from fanwe_user where is_npc = 1 and player_uid = '" + user_name + "');";
						
						long building_id = Helper.getNewBuildingId();

						sql += CreateSql(buildings,user_name,ref building_id);

						sql += CreateSql(obstacles,user_name,ref building_id);
					
						sql += CreateSql(traps,user_name,ref building_id);

						sql += CreateSql(decos,user_name,ref building_id);

						Debug.Log(index.ToString() + ":" + user_name);

						Debug.Log(sql);

						index ++;
					}
				}


			}

		}
	}

	public string CreateSql(JSONArray buildings,string user_name, ref long building_id){
		string sql = "";
		for (int i = 0; i < buildings.Length; i ++){
			JSONValue build = buildings[i];
			
			//			Debug.Log(build.Type.ToString() + ":" + build.ToString());
			
			string data = build.Obj.GetNumber("data").ToString();
			int x = (int)build.Obj.GetNumber("x");
			int y = (int)build.Obj.GetNumber("y");
			int lvl = 0;
			
			if (build.Obj.ContainsKey("lvl"))
				lvl = (int)build.Obj.GetNumber("lvl");
			
			if (lvl == 0) 
				lvl = 1;
			
			if (buildObject.ContainsKey(data)){
				string tid = buildObject.GetString(data);
				string tid_level = tid + "_" + lvl.ToString();
				CsvInfo csvInfo = CSVManager.GetInstance().csvTable[tid_level] as CsvInfo;

				sql += "insert into fanwe_user_grid(building_id,user_id,regions_id,tid,level,status,x,y,w_h,cur_hitpoints,cur_hitpoints2,hitpoints) values (";

				//building_id
				building_id ++;
				sql += building_id.ToString();// Helper.getNewBuildingId().ToString();
				//user_id
				sql += ",(select id from fanwe_user where is_npc = 1 and player_uid = '" + user_name + "')";
				//regions_id
				sql += ",0";
				//tid
				sql += ",\"" + tid + "\"";
				//level
				sql += "," + lvl.ToString();
				//status
				sql += ",0";
				//x
				int new_x = 50 - y - csvInfo.Width;
				sql += "," + new_x.ToString();
				//y
				int new_y = 50 - x - csvInfo.Height;
				sql += "," + new_y.ToString();
				//w_h
				sql += "," + csvInfo.Width.ToString();

				//cur_hitpoints
				sql += "," + csvInfo.Hitpoints.ToString();
				//cur_hitpoints2
				sql += "," + csvInfo.Hitpoints.ToString();
				//hitpoints
				sql += "," + csvInfo.Hitpoints.ToString();
				sql += ");";

				//Debug.Log("data:" + data + ";tid:" + buildObject.GetString(data) + ";lvl:" + lvl + ";x:" + x + ";y:" + y);
			}else{
				Debug.Log(user_name + ";" + data.ToString() + "is not");
			}
		}

		return sql;
	}

	public bool CreateLocalBuild(JSONArray buildings,string user_name){
		bool f = true;
		for (int i = 0; i < buildings.Length; i ++){
			JSONValue build = buildings[i];
			
//			Debug.Log(build.Type.ToString() + ":" + build.ToString());

			string data = build.Obj.GetNumber("data").ToString();
			int x = (int)build.Obj.GetNumber("x");
			int y = (int)build.Obj.GetNumber("y");
			int lvl = 0;

			if (build.Obj.ContainsKey("lvl"))
				lvl = (int)build.Obj.GetNumber("lvl");

			if (lvl == 0) 
				lvl = 1;

			if (buildObject.ContainsKey(data)){
				ISFSObject item = new SFSObject();
				string tid = buildObject.GetString(data);
				string tid_level = tid + "_" + lvl.ToString();
				CsvInfo csvInfo = CSVManager.GetInstance().csvTable[tid_level] as CsvInfo;
				item.PutUtfString("tid",tid);
				item.PutInt("level",lvl);
				item.PutInt("status",0);
				item.PutLong("building_id",Helper.getNewBuildingId());

				item.PutInt("x",50-y - csvInfo.Width);
				item.PutInt("y",50-x - csvInfo.Height);

				item.PutInt("artifact_type",0);

                //Debug.Log(item.ToString());
			    BuildManager.CreateBuild(new BuildParam()
			    {
                    item = item
			    });

//                BuildManager.CreateBuild(item,null,0);
				//Debug.Log("data:" + data + ";tid:" + buildObject.GetString(data) + ";lvl:" + lvl + ";x:" + x + ";y:" + y);
			}else{
				f = false;
				Debug.Log(user_name + ";" + data.ToString() + "is not");
			}
		}
		return f;
	}
}
