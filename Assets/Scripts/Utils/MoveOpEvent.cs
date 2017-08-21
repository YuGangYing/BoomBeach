using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BoomBeach;

public class MoveOpEvent : MonoBehaviour {


	//单例定义;
	public Camera uiCamera;
	public Camera ui2Camera;
	public Camera islandCamera;
	private static MoveOpEvent instance;
	public static MoveOpEvent Instance{
		get{ return instance; }
	}
	void Awake()
	{
		instance = this;
		gridMap = GameObject.Find ("GroundGrid");
		hitBox = gridMap.transform.GetComponent<Collider>();
		planRed = Resources.Load ("Materials/BuildPlan/planRed") as Material;
		planGreen = Resources.Load ("Materials/BuildPlan/planGreen") as Material;
		gridRed = Resources.Load ("Materials/BuildPlan/gridRed") as Material;
		gridGreen = Resources.Load ("Materials/BuildPlan/gridGreen") as Material;

	}

	private Collider hitBox;
	public bool Status{
		set{
			hitBox.enabled = value;
		}
		get{
			return hitBox.enabled;
		}
	}

	public bool isDown = false;
	public bool isDrag = false;
	public bool isTouchGrid = false;
	public bool isSenceOp = false;  //是否为场景建筑的操作;
	public Vector3 mouseDownPos = Vector3.zero;  //鼠标按下的最初坐标;

	public Ray ray;

	//一些建筑移动的对象;
	public BuildInfo SelectedBuildInfo;
	public Vector3 SelectedBuildOffset;
	public GameObject gridMap;
	public Material planRed;
	public Material planGreen;
	public Material gridRed;
	public Material gridGreen;

	//判断是否正在ui层上面
	bool CheckUIOp(Vector3 touchPosition)
	{
		Ray uiRay = uiCamera.ViewportPointToRay (uiCamera.ScreenToViewportPoint(touchPosition));
		Ray ui2Ray = ui2Camera.ViewportPointToRay (ui2Camera.ScreenToViewportPoint(touchPosition));
        if (Physics.Raycast (uiRay, 1000f, 1 << 9)||Physics.Raycast (ui2Ray, 1000f, 1 << 10) || EventSystem.current.IsPointerOverGameObject()) 
		{
			return true;
		} 
		else
		{
			return false;
		}						
	}

    GUIText text;
    string str;
	bool mShowDebug = false;
	Vector3 mTouchPosition;
    void Update () 
	{
		#if UNITY_EDITOR
		if (mShowDebug) {
	        if (text == null) {
	            text = GameObject.Find("GUI Text").GetComponent<GUIText>();
	        }
	        int gc = (int)System.GC.GetTotalMemory(true) / 1000000;
	        int memory = (int)UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory() / 1000000;
	        text.text = "FPS:" + ((int)(1 / Time.deltaTime)).ToString() + ";GC:" + gc + ";Memory:" + memory;
		}
		#endif

		//屏幕点;
		mTouchPosition = Vector3.zero;
		//触屏
		if(Input.touchCount>0)
		{
			mTouchPosition = Input.GetTouch(0).position;
			//if(checkUIOp(touchPosition))return;
			switch(Input.GetTouch(0).phase){
			case TouchPhase.Began:
				isDown = true;
				OnTouchDown(mTouchPosition);//call touchDown;
				break;
			case TouchPhase.Ended:
				isDown = false;
				isDrag = false;
				isTouchGrid = false;
				OnTouchUp(mTouchPosition);//call touchUp;
				break;
			case TouchPhase.Moved:
				isDrag = true;
				break;
			}
		}
		//兼容mouse操作;	
		else
		{
			mTouchPosition = Input.mousePosition;
			//if(checkUIOp(touchPosition))return;
			if(Input.GetMouseButtonUp(0)||Input.GetMouseButtonUp(1))
			{
				isDown = false;
				isDrag = false;
				isTouchGrid = false;
				mouseDownPos = Vector3.zero;
				//call touchUp;
				OnTouchUp(mTouchPosition);
			}
			if(Input.GetMouseButton(0)||Input.GetMouseButton(1))
			{
				if(Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1))
				{
					if(!isDown)
					{
						mouseDownPos = Input.mousePosition;
						isDown = true;
						//call touchDown;
						OnTouchDown(mTouchPosition);
					}
				}
				if(isDown&&mouseDownPos!=Input.mousePosition&&!isDrag)
				{
					isDrag = true;
				}	
			}
		}
		if(isDown)
		{
			ray = GetComponent<Camera>().ViewportPointToRay(GetComponent<Camera>().ScreenToViewportPoint(mTouchPosition));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit,1000f,1<<8))
			{
				isTouchGrid = true;
			}
		}
		if (isDown && isTouchGrid && isDrag) 
		{
			OnDrag(mTouchPosition);
		} 
		CameraOpEvent.Instance.CameraUpdate ();
	}

	void LateUpdate()
	{
		CameraOpEvent.Instance.CameraLateUpdate ();
	}

	//一直执行;
	void OnDrag(Vector3 touchPosition)
	{
        if (DataManager.GetInstance.sceneStatus != SceneStatus.HOME && DataManager.GetInstance.sceneStatus != SceneStatus.HOMERESOURCE)
        {
            return;
        }
		if(SelectedBuildInfo ==null || !SelectedBuildInfo.isGrap)
		{
			return;
		}
		//修改by hc 2014.1.21,登录舰不可移动;
		if(SelectedBuildInfo.csvInfo.TID=="TID_BUILDING_LANDING_SHIP" || SelectedBuildInfo.csvInfo.TID_Type=="OBSTACLES")
		{
			return;
		}
		Vector3 worldPosition = screenToWorldPosition (touchPosition);
		if(!SelectedBuildInfo.isMoving)
		{
			gridMap.GetComponent<DrawGrid>().drawLine();
			DrawBuildPlan(SelectedBuildInfo);
			if(SelectedBuildInfo.status!=BuildStatus.Create){
				SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(false);
                if(SelectedBuildInfo.buildUIManage!=null)
                    SelectedBuildInfo.buildUIManage.ResourceBtnDc = null;
			}
		}
		SelectedBuildInfo.isMoving = true;	
		worldPosition-=SelectedBuildOffset;
		GridInfo currentGrid = Globals.LocateGridInfo(worldPosition);
		if(SelectedBuildInfo.Position!=currentGrid.GridPosition)
		{
			AudioPlayer.Instance.PlaySfx("moving_07");
		}
		SelectedBuildInfo.Position = currentGrid.GridPosition;
		if(SelectedBuildInfo.CheckBuildPlaceAble())
		{
			SetGround (SelectedBuildInfo,"green");
		}
		else
		{ 
			SetGround (SelectedBuildInfo,"red");
		}
		if(SelectedBuildInfo.status==BuildStatus.Create)
			SelectedBuildInfo.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(SelectedBuildInfo);
	}

	//执行一次;
	void OnTouchDown(Vector3 touchPosition)
	{
		if (!CheckUIOp(touchPosition))
		{
			isSenceOp = true;
		}

		if(isSenceOp)
		{
			Vector3 worldPosition = screenToWorldPosition (touchPosition);
			if (SelectedBuildInfo != null && isHoverBuild (SelectedBuildInfo,worldPosition)) 
			{
				SelectedBuildInfo.isGrap = true;
				if(DataManager.GetInstance.sceneStatus==SceneStatus.HOME||DataManager.GetInstance.sceneStatus==SceneStatus.HOMERESOURCE)
				{
					CameraOpEvent.Instance.Status = false;	
				}
				else
				{
					CameraOpEvent.Instance.Status = true;	
				}
			}
			else 
			{
				CameraOpEvent.Instance.Status = true;	
			}
		}

	}



	//执行一次;
	void OnTouchUp(Vector3 touchPosition)
	{
		Vector3 wp = screenToWorldPosition(touchPosition);
		Vector2 wp2d = new Vector2(wp.x,wp.z);
		wp2d = Globals.TransformCoordinate(wp2d,45);
		//wp2d = new Vector2(wp2d.y,wp2d.x);
		//Debug.Log(wp+" "+wp2d);

		if (CameraOpEvent.Instance.EventStatus&&CameraOpEvent.Instance.Status&&isSenceOp)
		{
			CameraOpEvent.Instance.Inertance();
		}
		if (DataManager.GetInstance.sceneStatus == SceneStatus.BATTLEREPLAY) 
		{
			isSenceOp = false;
			return;
		}
		else if(DataManager.GetInstance.sceneStatus == SceneStatus.ENEMYBATTLE)
		{
            
			if(isSenceOp)
			{
				if (!CameraOpEvent.Instance.EventStatus)
				{
					CameraOpEvent.Instance.Status = false;
					BattleController.PlaceTroopAndWeapon(touchPosition);//部署小兵
				}
			}
			isSenceOp = false;
			return;
		}
		/*
		Vector3 touchWorldPosition = screenToWorldPosition (touchPosition);

		int i = 0;
		foreach(BuildInfo binfo in Globals.BuildList.Values)
		{



				Weapon wp = binfo.GetComponentInChildren<Weapon>();
				Vector2 beginp = new Vector2(binfo.Position.x+1.5f,binfo.Position.z+1.5f);
				Vector3 endp = new Vector2(touchWorldPosition.x,touchWorldPosition.z);

				GameObject.Find("Cube").transform.position = touchWorldPosition;
				float degree = Globals.CaclDegree(beginp,endp);
//				Debug.Log(binfo.name+" "+degree);
				wp.aim(degree);			


			i++;
		}
		*/

		if(isSenceOp)
		{
			PopUI.closePops (null);
			if (!CameraOpEvent.Instance.EventStatus)
			{
				CameraOpEvent.Instance.Status = false;
				if(SelectedBuildInfo!=null&&SelectedBuildInfo.isGrap)
				{
					SelectedBuildInfo.isGrap = false;

					//修改 by hc 2014.1.21，3d建筑不处理;
					if(SelectedBuildInfo.CheckBuildPlaceAble()&&SelectedBuildInfo.status != BuildStatus.Create&&!SelectedBuildInfo.is3D)
					{
						AudioPlayer.Instance.PlaySfx("build_placing");
						SelectedBuildInfo.SetBuild();
						gridMap.GetComponent<DrawGrid>().clearMesh();
						UnDrawBuildPlan(SelectedBuildInfo);
						SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(true);
					}
					else
					{
						if(!SelectedBuildInfo.CheckBuildPlaceAble())
						AudioPlayer.Instance.PlaySfx("bad_move_06");
					}
				}
				else
				{

						Vector3 worldPosition = screenToWorldPosition (touchPosition);
						GridInfo gridInfo = Globals.LocateGridInfo (worldPosition);

						//修改by hc,2014.1.21，增加3D建筑的选取;
						BuildInfo build3D = locate3DBuild(touchPosition);
						if (build3D!=null||gridInfo.isBuild) 
						{
							if(SelectedBuildInfo!=null&&SelectedBuildInfo.status!=BuildStatus.Create)
							{
								if(SelectedBuildInfo.isMoving)
								{
									SelectedBuildInfo.ResetPosition();
									gridMap.GetComponent<DrawGrid>().clearMesh();
									UnDrawBuildPlan(SelectedBuildInfo);		
									SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(true);
									
								}
								UnSelectBuild();

							}

							if((SelectedBuildInfo!=null&&SelectedBuildInfo.status!=BuildStatus.Create)||SelectedBuildInfo==null)
							{		
								BuildInfo clickBuild = null;
								if(build3D!=null)
									clickBuild = build3D;
								else
									clickBuild = gridInfo.buildInfo;
								if(DataManager.GetInstance.sceneStatus==SceneStatus.HOME)
								{
									
									//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
									int cStatus = Helper.CollectStatus(clickBuild);
									//Debug.Log(cStatus+" "+gridInfo.buildInfo.name);
									if(cStatus==1||cStatus==2)
									{
									CollectHandle.Collect(clickBuild);
									}
									else
									{
                                        if (cStatus==3){
											//TID_RESOURCE_PACK_LOCKED = 没有足够的储存空间;
											PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_RESOURCE_PACK_LOCKED"));
										}
										AudioPlayer.Instance.PlaySfx("build_pickup_05");
										SelectBuild(clickBuild);		
									}
								}
								else
								{
									AudioPlayer.Instance.PlaySfx("build_pickup_05");
									SelectBuild(clickBuild);
								}
								
							}
						}
						else
						{
							ResetBuild();
						}

				}
			}
		}
		isSenceOp = false;
	}

	public void ResetBuild()
	{
		if(SelectedBuildInfo!=null&&SelectedBuildInfo.isMoving&&SelectedBuildInfo.status!=BuildStatus.Create)
		{
			gridMap.GetComponent<DrawGrid>().clearMesh();
			UnDrawBuildPlan(SelectedBuildInfo);
			SelectedBuildInfo.ResetPosition();
			SelectedBuildInfo.transform.Find("UI/UIS").gameObject.SetActive(true);
			
		}
		
		if(SelectedBuildInfo!=null&&SelectedBuildInfo.status!=BuildStatus.Create)
		{							
			UnSelectBuild();							
		}
	}

	public Vector3 screenToWorldPosition(Vector3 touchPosition)
	{
		ray = GetComponent<Camera>().ViewportPointToRay(GetComponent<Camera>().ScreenToViewportPoint(touchPosition));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit,1000f,1<<8))
		{
			return hit.point;
		}
		return Vector3.zero;
	}

	//选择当前建筑;
	public void SelectBuild(BuildInfo buildInfo)
	{
		if(buildInfo.status!=BuildStatus.Removaling)
		{
			SelectedBuildInfo = buildInfo;

			buildInfo.isSelected = true;

			//修改by hc,2014.1.21，增加3d建筑不显示部份UI;
			if(!buildInfo.is3D&&buildInfo.csvInfo.TID_Type!="OBSTACLES")
			{
                if (DataManager.GetInstance.sceneStatus == SceneStatus.HOME || DataManager.GetInstance.sceneStatus == SceneStatus.HOMERESOURCE)
                {
                    if (buildInfo.buildUIManage != null)
                    {
                        buildInfo.buildUIManage.ShowArrows(true);
                    }
                    else
                    {
                        buildInfo.buildUI.ShowArrows(true);
                    }
                }
				
			}
			if(SelectedBuildInfo.status!=BuildStatus.Create)
			{
				BuildTweener tweener = SelectedBuildInfo.GetComponentInChildren<BuildTweener> ();
				tweener.enabled = true;
				tweener.Pslay ();
			}
			SelectedBuildInfo.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn (SelectedBuildInfo);

			BuildRing buildRing = buildInfo.GetComponentInChildren<BuildRing> ();
			if(buildRing!=null)
			{
				buildRing.Show ();
			}
		}
	}

	public void DrawBuildPlan(BuildInfo buildInfo)
	{
		Transform buildPlan = buildInfo.transform.Find ("buildplan");
		Transform buildGrid = buildInfo.transform.Find ("buildgrid");
		DrawPlan dp = buildPlan.GetComponent<DrawPlan> ();
		dp.enabled = true;
		dp.drawPlan ();

		DrawBuildGrid dbg = buildGrid.GetComponent<DrawBuildGrid> ();
		dbg.enabled = true;
		dbg.drawLine ();

		if(buildInfo.transform.Find ("Floor")!=null)
		buildInfo.transform.Find ("Floor").gameObject.SetActive (false);
	}

	public void UnDrawBuildPlan(BuildInfo buildInfo)
	{
		if(!buildInfo.is3D)
		{
			Transform buildPlan = buildInfo.transform.Find ("buildplan");
			Transform buildGrid = buildInfo.transform.Find ("buildgrid");
			DrawPlan dp = buildPlan.GetComponent<DrawPlan> ();
			dp.enabled = true;
			dp.clearMesh ();
			//buildPlan.renderer.material = null;
			DrawBuildGrid dbg = buildGrid.GetComponent<DrawBuildGrid> ();
			dbg.enabled = true;
			dbg.clearMesh ();
			//buildGrid.renderer.material = null;
			if(buildInfo.transform.Find ("Floor")!=null)
			buildInfo.transform.Find ("Floor").gameObject.SetActive (true);
		}
	}
	
	
	//取消当前建筑的选中;
	public void UnSelectBuild()
	{
		if (SelectedBuildInfo != null) 
		{

			BuildInfo buildInfo = SelectedBuildInfo;

			buildInfo.isSelected = false;
			buildInfo.isMoving = false;
			SelectedBuildInfo = null;
			buildInfo.buildUI.RefreshBuildBtn ();
			//PopManage.Instance.RefreshBuildBtn(buildInfo);

			buildInfo.isAfterResetPos = false;

			if(!buildInfo.is3D&&buildInfo.csvInfo.TID_Type!="OBSTACLES")
			{
                if(buildInfo.buildUIManage!=null)
				    buildInfo.buildUIManage.ShowArrows (false);
                if (buildInfo.buildUI != null)
                    buildInfo.buildUI.ShowArrows(false);
            }
			buildInfo.GetComponentInChildren<BuildTweener> ().Stop ();

			BuildRing buildRing = buildInfo.GetComponentInChildren<BuildRing> ();
			if(buildRing!=null)
			{
				buildRing.Hide ();
			}
		}
	}


	public void SetGround(BuildInfo buildInfo,string color)
	{
		Transform buildPlan = buildInfo.transform.Find ("buildplan");
		Transform buildGrid = buildInfo.transform.Find ("buildgrid");
		if (color == "green") 
		{
			buildPlan.GetComponent<Renderer>().sharedMaterial = planGreen;
			buildGrid.GetComponent<Renderer>().sharedMaterial = gridGreen;
		}
		if (color == "red") 
		{
			buildPlan.GetComponent<Renderer>().sharedMaterial = planRed;
			buildGrid.GetComponent<Renderer>().sharedMaterial = gridRed;
		}
	}


	//获取3d建筑物;
	public BuildInfo locate3DBuild(Vector3 position)
	{
		Ray ray =  GetComponent<Camera>().ViewportPointToRay (GetComponent<Camera>().ScreenToViewportPoint(position));
		
		RaycastHit hiter;
	
		if (Physics.Raycast(ray,out hiter,1000f,1<<16)) 
		{
			//Debug.Log(hiter.transform.name);
			BuildInfo hitterBuild = hiter.collider.transform.GetComponent<BuildInfo>();
			if(hitterBuild!=null)
			{
				return hitterBuild;
			}

			ResourceShip resourceShip = hiter.transform.GetComponent<ResourceShip>();
			if(resourceShip!=null)
			{
				resourceShip.CollectResource();
			}

			return null;
		} 
		else
		{
			return null;
		}	
	}

	//判断该点是否在该建筑物上;
	public bool isHoverBuild(BuildInfo buildInfo, Vector3 position)
	{
		//修改by hc 2014.1.21, 3d建筑依靠碰撞检测;
		if(buildInfo.is3D||buildInfo.csvInfo.TID_Type=="OBSTACLES")
		{
			return false;
			/*
			Ray ray =  camera.ViewportPointToRay (camera.ScreenToViewportPoint(position));
		
			RaycastHit hiter;
			if (Physics.Raycast(ray,out hiter,1000f,1<<16)) 
			{

				BuildInfo hitterBuild = hiter.transform.GetComponent<BuildInfo>();
				if(hitterBuild!=null&&hitterBuild==buildInfo)
				{
					return true;
				}

				return false;
			} 
			else
			{
				return false;
			}	
			*/
		}
		else		
		{

			float beginX = buildInfo.Position.x;
			float endX = buildInfo.Position.x + (float)buildInfo.GridCount;
			float beginZ = buildInfo.Position.z;
			float endZ = buildInfo.Position.z + (float)buildInfo.GridCount;
			
			if (position.x >= beginX && position.x < endX && position.z >= beginZ && position.z < endZ) 
			{
				SelectedBuildOffset = position - buildInfo.Position;
				return true;				
			} 
			else 
			{
				SelectedBuildOffset = Vector3.zero;
				return false;		
			}
		}
	}

}
