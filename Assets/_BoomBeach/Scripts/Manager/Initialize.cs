using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using BoomBeach;

public class Initialize : SingleMonoBehaviour<Initialize> {

	public const int DEFAULT_USER_REGION_ID = 0;
	public int current_user_id;
	public int current_region_id;

	protected override void Awake()
    {
        Globals.gameManager = GameObject.Find("GameManage");
		UIManager.GetInstance.InitCtrollers ();
    }

	void Start () 
	{
//		BattleSceneUI.BattleSceneUIS = new List<BattleSceneUI> ();
//		GameLoader.Instance.play_cloud = false;
//
		GameLoader.Instance.SwitchScene (SceneStatus.HOME);
    }

	void LoadHome(){
		Globals.LastSceneUserId = current_user_id;
		Globals.LastSceneRegionsId = current_region_id;
		current_user_id = DataManager.GetInstance.model.user_info.id;
		current_region_id = DEFAULT_USER_REGION_ID;
		List<Network.BuildingModel> buildingList = DataManager.GetInstance.model.building_list;
		BuildManager.GetInstance.InitBuildings (buildingList);
		UIManager.GetInstance.ShowHome ();
	}

	void OnLoadHome(){
//		ISFSObject td = TempleData.GetPlayerData();
	}


    /*
  // Use this for initialization
  void Start () {


      Globals.BuildContainer = GameObject.Find ("PBuilds").transform;
      Globals.CharacterContainer = GameObject.Find ("PCharacters").transform;

      Globals.GridArray = new GridInfo[Globals.GridTotal, Globals.GridTotal];
      for (int a=0; a<Globals.GridTotal; a++) 
      {
          for (int b=0; b<Globals.GridTotal; b++) 
          {
              GridInfo gridInfo = new GridInfo();
              gridInfo.A = a;
              gridInfo.B = b;
              gridInfo.GridPosition = new Vector3(a,0f,b);
              gridInfo.standPoint = new Vector3((float)a+0.5f,0f,(float)b+0.5f);
              gridInfo.isInArea = true;
              gridInfo.cost = Globals.GridEmptyCost;
              Globals.GridArray[a,b] = gridInfo;
          }
      }


      //以下是模拟建造建筑;
      ArrayList arrayList = new ArrayList ();
      arrayList.Add (new Vector2(0,0));
      arrayList.Add (new Vector2(0,6));
      arrayList.Add (new Vector2(0,12));
      arrayList.Add (new Vector2(0,18));
      arrayList.Add (new Vector2(0,24));
      arrayList.Add (new Vector2(0,30));

      arrayList.Add (new Vector2(5,0));
      arrayList.Add (new Vector2(5,6));
      arrayList.Add (new Vector2(5,12));
      arrayList.Add (new Vector2(5,18));
      arrayList.Add (new Vector2(5,24));
      arrayList.Add (new Vector2(5,30));


      arrayList.Add (new Vector2(10,0));
      arrayList.Add (new Vector2(10,6));
      arrayList.Add (new Vector2(10,12));
      arrayList.Add (new Vector2(10,18));
      arrayList.Add (new Vector2(10,24));
      arrayList.Add (new Vector2(10,30));

      arrayList.Add (new Vector2(15,0));
      arrayList.Add (new Vector2(15,6));
      arrayList.Add (new Vector2(15,12));
      arrayList.Add (new Vector2(15,18));
      arrayList.Add (new Vector2(15,24));
      arrayList.Add (new Vector2(15,30));


      arrayList.Add (new Vector2(20,0));
      arrayList.Add (new Vector2(20,6));
      arrayList.Add (new Vector2(20,12));
      arrayList.Add (new Vector2(20,18));
      arrayList.Add (new Vector2(20,24));
      arrayList.Add (new Vector2(20,30));

      arrayList.Add (new Vector2(25,0));
      arrayList.Add (new Vector2(25,6));
      arrayList.Add (new Vector2(25,12));
      arrayList.Add (new Vector2(25,18));
      arrayList.Add (new Vector2(25,24));
      arrayList.Add (new Vector2(25,30));


      arrayList.Add (new Vector2(30,0));
      arrayList.Add (new Vector2(30,6));
      arrayList.Add (new Vector2(30,12));
      arrayList.Add (new Vector2(30,18));
      arrayList.Add (new Vector2(30,24));
      arrayList.Add (new Vector2(30,30));

      arrayList.Add (new Vector2(35,0));
      arrayList.Add (new Vector2(35,6));
      arrayList.Add (new Vector2(35,12));
      arrayList.Add (new Vector2(35,18));
      arrayList.Add (new Vector2(35,24));
      arrayList.Add (new Vector2(35,30));


      for (int i=0; i<arrayList.Count; i++) {
          Vector2 vec = (Vector2)arrayList[i];
          GameObject buildItem = null;
          if(i%2==0)
              buildItem = Instantiate(Resources.Load("Test/BuildModel/build1")) as GameObject;
          else
              buildItem = Instantiate(Resources.Load("Test/BuildModel/build")) as GameObject;
          buildItem.transform.parent = Globals.BuildContainer;
          BuildInfo buildInfo = buildItem.GetComponent<BuildInfo>();
          buildInfo.Position = new Vector3(vec.x,0f,vec.y);
          buildInfo.Id = "build_"+vec.x+"_"+vec.y;
          buildInfo.initBuild();
          Globals.BuildList.Add(buildInfo); 
      }


      UnityEngine.Object cPrefab = Resources.Load ("Test/CharacterModel/character");

      for (int i=0; i<200; i++) {
          GameObject c = Instantiate(cPrefab) as GameObject;
          c.transform.parent = Globals.CharacterContainer;
          CharInfo cc = c.GetComponent<CharInfo>();
          cc.Position = new Vector3(Random.Range(3f,30f),0f,Random.Range(3f,30f));
          cc.Id = "cc_"+i;
      }


  }
    */
}
