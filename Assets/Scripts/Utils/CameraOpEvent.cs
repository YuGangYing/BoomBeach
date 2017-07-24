using UnityEngine;
using System.Collections;

public class CameraOpEvent : MonoBehaviour {

	public Camera UI2Camera;
	public Camera IslandCamera;

	public Vector3 point1 = Vector3.zero;  //当前第一个触摸点坐标;
	public Vector3 point2 = Vector3.zero;  //当前第二个触摸点坐标;
	
	public Vector3 lastPoint1 = Vector3.zero;	//上一次第一个触摸点坐标;
	public Vector3 lastPoint2 = Vector3.zero;	//上一次第二个触摸点坐标;
	
	public bool isMouseDown = false;
	public bool isMouseDrag = false;
	public Vector3 mouseDownPos = Vector3.zero;  //鼠标按下的最初坐标;
	
	public bool isTouch1Down = false;
	public bool isTouch2Down = false;
	
	public float maxField = 30f;
	public float minField = 4f;
	public float currentField = 0f;
	public float baseScaleRatio = 1.3f;
	public float btnScaleRatio = 1.3f;
	public float btnScaleStep = 1f;

	public float maxMoveY = 40f;
	public float minMoveY = -40f;
	public float maxMoveX = 40f;
	public float minMoveX = -40f;
	
	//单例定义;
	private static CameraOpEvent instance;
	public static CameraOpEvent Instance{
		get{ return instance; }
	}
	void Awake()
	{
		instance = this;
		UI2Camera.transform.position = transform.position;
		UI2Camera.transform.rotation = transform.rotation;

		IslandCamera.transform.position = transform.position;
		IslandCamera.transform.rotation = transform.rotation;
		currentField = GetComponent<Camera>().fieldOfView;
	}
	
	private bool status = false;  //相机事件状态开关;
	private bool eventStatus = false; //是否在lateUpdate中响应事件;	
	public bool Status{
		set{
			eventStatus = false;
			status = value;
			if(!value)
			{
				isMouseDown = false;
				isMouseDrag = false;
				isTouch1Down = false;
				isTouch2Down = false;
			}
			else
			{
				IsIntertance = false;
			}
		}
		get{
			return status;
		}
	}

	//开放eventStatus，表示相机是否正在操作中;
	public bool EventStatus{
		get{return eventStatus; }
	}
	
	// Update is called once per frame
	public float zoomSpeed = 0.5f;
	public void CameraUpdate () 
	{
		if(isReseting)
		{
			if(!Globals.IsSceneLoaded)return;

			float fov = GetComponent<Camera>().fieldOfView;
			fov = Mathf.Lerp(fov,resetFOV,zoomSpeed*Globals.TimeRatio);

			if(Mathf.Abs(fov-resetFOV)<0.1f)
			{
				fov = resetFOV;
			}

			GetComponent<Camera>().fieldOfView = fov;
			UI2Camera.fieldOfView = fov;
			IslandCamera.fieldOfView = fov;
			currentField = fov;


			return;
		}

		//开始惯性位移;
		if(IsIntertance)
		{
			point1 = Vector3.Lerp(point1,IntertanceDestPoint,IntertanceSpeed*Globals.TimeRatio);
			Vector3 cameraPos = GetComponent<Camera>().transform.parent.localPosition;					
			Vector3 moveOffset = point1 - lastPoint1;
			//float scale = GetComponent<Camera>().fieldOfView/180f;
			//moveOffset = ScreenPosClamp(moveOffset);			
			cameraPos -= moveOffset;
			if(cameraPos.x<minMoveX)
			{
				cameraPos.x=minMoveX;
				IsIntertance = false;
			}
			if(cameraPos.x>maxMoveX)
			{
				cameraPos.x=maxMoveX;
				IsIntertance = false;
			}
			if(cameraPos.y<minMoveY)
			{
				cameraPos.y=minMoveY;
				IsIntertance = false;
			}
			if(cameraPos.y>maxMoveY)
			{
				cameraPos.y=maxMoveY;
				IsIntertance = false;
			}
			
			GetComponent<Camera>().transform.parent.localPosition = cameraPos;	
			lastPoint1 = point1;

			if((point1-IntertanceDestPoint).magnitude<0.01f)
			{
				IsIntertance = false;
			}

		}

		if(status)
		{
			if(Input.touchCount>0)
			{
				
				//有点击执行相应的方法	;
				if(Input.touchCount==1)
				{
					if(!isTouch1Down&&MoveOpEvent.Instance.isSenceOp)
					{
						isTouch1Down = true;
						lastPoint1 = Input.GetTouch(0).position;
						lastPoint2 = lastPoint1;
					}
					if(Input.GetTouch(0).phase == TouchPhase.Moved&&MoveOpEvent.Instance.isSenceOp)
					{										
						point1 = Input.GetTouch(0).position;
						point2 = point1;
						if((point1-lastPoint1).magnitude>30f&&!eventStatus)
						{
							eventStatus = true;
						}
					}

					if(Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						isTouch1Down = false;
					}
				}	
				else
				{
					if(!isTouch1Down)
					{
						isTouch1Down = true;
						lastPoint1 = Input.GetTouch(0).position;
					}
					if(!isTouch2Down)
					{
						isTouch2Down = true;
						lastPoint2 = Input.GetTouch(1).position;
					}
					if(Input.GetTouch(0).phase == TouchPhase.Moved||Input.GetTouch(1).phase == TouchPhase.Moved)
					{
						point1 = Input.GetTouch(0).position;
						point2 = Input.GetTouch(1).position;
						if((point1-lastPoint1).magnitude>2f||(point2-lastPoint2).magnitude>2f)
						{
							eventStatus = true;
						}
					}
					
					if(Input.GetTouch(1).phase == TouchPhase.Ended)
					{
						point2 = point1;
						lastPoint1 = point1;
						lastPoint2 = lastPoint1;
						isTouch2Down = false;
					}
					
					if(Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						point1 = point2;
						lastPoint1 = point1;
						lastPoint2 = lastPoint1;
						isTouch1Down = false;
					}
						
				}	
			}
			else
			{
				//兼容mouse操作;		
				if(Input.GetMouseButtonUp(0)||Input.GetMouseButtonUp(1))
				{
					isMouseDown = false;
					isMouseDrag = false;
					mouseDownPos = Vector3.zero;
				}
				if(Input.GetMouseButton(0))
				{
					if(Input.GetMouseButtonDown(0))
					{
						if(!isMouseDown&&MoveOpEvent.Instance.isSenceOp)
						{
							mouseDownPos = Input.mousePosition;
							lastPoint1 = Input.mousePosition;
							isMouseDown = true;
						}
					}
					
					if(isMouseDown&&mouseDownPos!=Input.mousePosition)
					{
						isMouseDrag = true;
					}
					
					if(isMouseDrag)
					{				
						point1 = Input.mousePosition;
						if((point1-lastPoint1).magnitude>30f&&!eventStatus)
						{
							eventStatus = true;
						}
					}	
				}
				
				if(Input.GetMouseButton(1))
				{
					if(Input.GetMouseButtonDown(1))
					{
						if(!isMouseDown)
						{
							mouseDownPos = Input.mousePosition;
							lastPoint1 = Input.mousePosition;
							isMouseDown = true;
						}
					}

					if(isMouseDown&&mouseDownPos!=Input.mousePosition)
					{
						isMouseDrag = true;
					}
					
					if(isMouseDrag)
					{				
						point1 = Input.mousePosition;
						if((point1-lastPoint1).magnitude>2f)
						{
							eventStatus = true;
						}
					}	
				}
				else
				{
					BuildUISize.isZooming = false;
				}
				
				
			}
		}
	}
	
	public void CameraLateUpdate()
	{
		if(isReseting)
		{
			float fov = GetComponent<Camera>().fieldOfView;
			if(fov==resetFOV)
			{
				isReseting = false;
				BuildUISize.isZooming = false;
			}
		}
		if (!MoveOpEvent.Instance.isSenceOp)return;
		if(status&&eventStatus)
		{
			if((Input.touchCount>0&&Input.GetTouch(0).phase == TouchPhase.Moved)||(Input.touchCount>1&&Input.GetTouch(1).phase == TouchPhase.Moved))
			{		
				
				if(Input.touchCount>1)
				{
					Vector3 pointOffset = point1-point2;
					float pointLength = Mathf.Sqrt( Mathf.Abs(pointOffset.x)*Mathf.Abs(pointOffset.x) + Mathf.Abs(pointOffset.y)*Mathf.Abs(pointOffset.y));
					
					Vector3 lastPointOffset = lastPoint1-lastPoint2;
					float lastPointLength = Mathf.Sqrt( Mathf.Abs(lastPointOffset.x)*Mathf.Abs(lastPointOffset.x) + Mathf.Abs(lastPointOffset.y)*Mathf.Abs(lastPointOffset.y));



					float fieldView = GetComponent<Camera>().fieldOfView-(pointLength-lastPointLength)/30f;
					if(fieldView>minField&&fieldView<maxField)
					{
						GetComponent<Camera>().fieldOfView = fieldView;
						UI2Camera.fieldOfView = fieldView;
						IslandCamera.fieldOfView = fieldView;
						currentField = fieldView;
						BuildUISize.isZooming = true;
					}
				}
				
		
				
				//开始位移;
				Vector3 cameraPos = GetComponent<Camera>().transform.parent.localPosition;					
				Vector3 moveOffset = point1 - lastPoint1;
//				float scale = GetComponent<Camera>().fieldOfView/180f;
				moveOffset = ScreenPosClamp(moveOffset);			
				cameraPos -= moveOffset;
				if(cameraPos.x<minMoveX)cameraPos.x=minMoveX;
				if(cameraPos.x>maxMoveX)cameraPos.x=maxMoveX;
				if(cameraPos.y<minMoveY)cameraPos.y=minMoveY;
				if(cameraPos.y>maxMoveY)cameraPos.y=maxMoveY;
	
				GetComponent<Camera>().transform.parent.localPosition = cameraPos;	
				lastPoint1 = point1;
				lastPoint2 = point2;

				//开始记录惯性数据;
				IntertanceDestPoint = (point1+moveOffset*IntertanceStep);

			}
			else
			{
				//兼容mouse操作;
				if(Input.GetMouseButton(0)&&isMouseDrag)
				{			
					Vector3 cameraPos = GetComponent<Camera>().transform.parent.localPosition;					
					Vector3 moveOffset = point1 - lastPoint1;
//					float scale = GetComponent<Camera>().fieldOfView/180f;
					moveOffset = ScreenPosClamp(moveOffset);			
					cameraPos -= moveOffset;
					if(cameraPos.x<minMoveX)cameraPos.x=minMoveX;
					if(cameraPos.x>maxMoveX)cameraPos.x=maxMoveX;
					if(cameraPos.y<minMoveY)cameraPos.y=minMoveY;
					if(cameraPos.y>maxMoveY)cameraPos.y=maxMoveY;
		
					GetComponent<Camera>().transform.parent.localPosition = cameraPos;	
					lastPoint1 = point1;

					//开始记录惯性数据;
					IntertanceDestPoint = (point1+moveOffset*IntertanceStep);
				}
				
				
				if(Input.GetMouseButton(1)&&isMouseDrag)
				{			
					float pointLength = point1.x-lastPoint1.x;
					float fieldView = GetComponent<Camera>().fieldOfView-pointLength/10f;
					if(fieldView>minField&&fieldView<maxField)
					{
						GetComponent<Camera>().fieldOfView = fieldView;
						UI2Camera.fieldOfView = fieldView;
						IslandCamera.fieldOfView = fieldView;
						currentField = fieldView;
						BuildUISize.isZooming = true;
					}
					lastPoint1 = point1;
				}


			}
		
		}
		
	}
	
	
	public Vector3 ScreenPosClamp(Vector3 screenCoordinate)
	{
			
		float xPixScale = Screen.width / 400f;
		float yPixScale = Screen.height / 300f;		
		screenCoordinate = new Vector3(screenCoordinate.x/xPixScale,screenCoordinate.y/yPixScale,0f);	
		float scale = GetComponent<Camera>().fieldOfView/fovBase;  //根据视角计算与屏幕的缩放比例;		
		screenCoordinate.y = screenCoordinate.y*increaseY; //根据x轴转角，即视角的转角，计算相机高度对远近距离的递增;
 		screenCoordinate.x = screenCoordinate.x*increaseX;
		
		return screenCoordinate*scale;
	}
	
	public float increaseY = 1f;
	public float increaseX = 1f;
	public float fovBase = 140f;

	private bool isReseting;
	public float beginFOV;
	private float resetFOV;

	public void ResetIm(Vector3 resetPos)
	{
		GetComponent<Camera>().transform.parent.localPosition = resetPos;	
		GetComponent<Camera>().fieldOfView = beginFOV;
		UI2Camera.fieldOfView = beginFOV;
		IslandCamera.fieldOfView = beginFOV;
		currentField = beginFOV;
	}

	public void Reset(Vector3 resetPos,float destFov)
	{
		resetFOV = destFov;
		GetComponent<Camera>().transform.parent.localPosition = resetPos;	
		GetComponent<Camera>().fieldOfView = beginFOV;
		UI2Camera.fieldOfView = beginFOV;
		IslandCamera.fieldOfView = beginFOV;
		currentField = beginFOV;
		isReseting = true;
		BuildUISize.isZooming = true;
	}

	private bool IsIntertance;
	private Vector3 IntertanceDestPoint; //惯性目标点;
	public float IntertanceStep = 5f; //最后一次偏移量的倍数，即惯性距离;
	public float IntertanceSpeed = 0.1f; //减速运动百分比;
	public void Inertance()
	{
		if(Input.touchCount==1||Input.GetMouseButtonUp(0))
		{
			IsIntertance = true;			
		}
	}


	//以下是屏震;

	private bool isShake = false;

	public void Shake()
	{
		isShake = true;
		shakeTimeCount = 0f;
	}

	private float moveStep = 0.1f;
	private float moveStepCount = 0;
	private float shakeSpeed = 0.05f;
	private int shakeDirect = 1;

	private float shakeTime = 0.2f;
	private float shakeTimeCount = 0f;


	void Update()
	{
		if(isShake)
		{
			float currentSpeed = shakeSpeed * Globals.TimeRatio;
			if(moveStepCount+currentSpeed>=moveStep)
			{
				currentSpeed = moveStep - moveStepCount;
			}

			Vector3 cameraPos = GetComponent<Camera>().transform.parent.localPosition;	
			GetComponent<Camera>().transform.parent.localPosition = new Vector3(cameraPos.x+shakeDirect*currentSpeed,cameraPos.y+shakeDirect*currentSpeed,cameraPos.z);

			moveStepCount+=currentSpeed;
			if(moveStepCount==moveStep)
			{
				shakeDirect*=-1;
				moveStepCount = 0;
			}

			shakeTimeCount+=Time.deltaTime;
			if(shakeTimeCount>=shakeTime)
			{
				shakeTimeCount = 0f;
				isShake = false;
			}

		}
	}


}
