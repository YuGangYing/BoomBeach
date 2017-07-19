using UnityEngine;
using System.Collections;
using BoomBeach;

public class WorldCameraOpEvent : MonoBehaviour {
	
	public Camera uiCamera;
	public Vector3 point1 = Vector3.zero;  //当前第一个触摸点坐标;
	public Vector3 point2 = Vector3.zero;  //当前第二个触摸点坐标;
	
	public Vector3 lastPoint1 = Vector3.zero;	//上一次第一个触摸点坐标;
	public Vector3 lastPoint2 = Vector3.zero;	//上一次第二个触摸点坐标;
	
	public bool isMouseDown = false;
	public bool isMouseDrag = false;
	public Vector3 mouseDownPos = Vector3.zero;  //鼠标按下的最初坐标;
	
	public bool isTouch1Down = false;
	public bool isTouch2Down = false;
	
	public float maxSize = 0.2f;
	public float minSize = 1f;
	public float currentSize = 1f;
	
	public float maxMoveY = 40f;
	public float minMoveY = -40f;
	public float maxMoveX = 40f;
	public float minMoveX = -40f;
	
	//单例定义;
	private static WorldCameraOpEvent instance;
	public static WorldCameraOpEvent Instance{
		get{ return instance; }
	}
	void Awake()
	{
		instance = this;
		currentSize = GetComponent<Camera>().orthographicSize;
	}
	
	private bool status = true;  //相机事件状态开关;
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
		}
		get{
			return status;
		}
	}
	
	//开放eventStatus，表示相机是否正在操作中;
	public bool EventStatus{
		get{return eventStatus; }
	}

	//关闭弹出窗口;//chenfq by add 2014.4.10
	public void ClosePop(){
        //禁用弹出界面;
        //TODO
		/*
        if (ScreenUIManage.Instance == null)
            return;
        if (ScreenUIManage.Instance != null)
			ScreenUIManage.Instance.ShowPopWorld(null,Vector3.zero,false);
		PopUI.closePops (null);
		PopManage.Instance.ShowEnemyActivityWin(false);
		*/
		UIManager.GetInstance.GetController<IslandPopCtrl>().Close ();
	}

	//判断是否有：碰到ui层、小岛屿及云层开启按钮;, 有碰到返回: true, 没有碰到返回: false
	//chenfq by add 2014.4.10
	private bool checkUIOp(Vector3 touchPosition)
	{
		//Debug.Log("xxx");
		
		Ray uiRay = uiCamera.ViewportPointToRay (uiCamera.ScreenToViewportPoint(touchPosition));
		Ray ui2Ray = GetComponent<Camera>().ViewportPointToRay (GetComponent<Camera>().ScreenToViewportPoint(touchPosition));
		
		//Debug.Log("uiRay:" + Physics.Raycast (uiRay, 1000f, 1 << 9));
		//Debug.Log("ui2Ray:" + Physics.Raycast (ui2Ray, 1000f, 1 << LayerMask.NameToLayer("World")));
		
		if (Physics.Raycast (uiRay, 1000f, 1 << 9) || Physics.Raycast (ui2Ray, 1000f, 1 << LayerMask.NameToLayer("World"))) 
		{
			return true;
		} 
		else
		{
			return false;
		}						
	}
	
	


	// Update is called once per frame
	void Update () 
	{
		if(status)
		{
			if(Input.touchCount>0)
			{
				//有点击执行相应的方法	;
				if(Input.touchCount==1)
				{
					if(!isTouch1Down)
					{
						isTouch1Down = true;
						lastPoint1 = Input.GetTouch(0).position;
						lastPoint2 = lastPoint1;
					}
					if(Input.GetTouch(0).phase == TouchPhase.Moved)
					{										
						point1 = Input.GetTouch(0).position;
						point2 = point1;
						if((point1-lastPoint1).magnitude>3f&&!eventStatus)
						{
							eventStatus = true;
						}
					}
					
					if(Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						isTouch1Down = false;
						if (checkUIOp(Input.GetTouch(0).position) == false){
							//当用户点界面,而没有碰到ui按钮时，则关闭已经打开的界面;
							ClosePop();
						}
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
						if (checkUIOp(Input.GetTouch(0).position) == false){
							//当用户点界面,而没有碰到ui按钮时，则关闭已经打开的界面;
							ClosePop();
						}
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

					//chenfq by add 2014.4.10
					if (checkUIOp(Input.mousePosition) == false){
						//当用户点界面,而没有碰到ui按钮时，则关闭已经打开的界面;
						ClosePop();
					}
				}
				if(Input.GetMouseButton(0))
				{
					if(Input.GetMouseButtonDown(0))
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
						if((point1-lastPoint1).magnitude>3f&&!eventStatus)
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
				
				
			}
		}
	}
	
	float moveSpeed {
		get{
			float speed = 0.0015f*GetComponent<Camera>().orthographicSize/0.2f*300f/Screen.height;
			//Debug.Log(Screen.height+" "+speed);
			return speed;
		}
	}
	
	void LateUpdate()
	{
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
					
					
					
					float currentsize = GetComponent<Camera>().orthographicSize-(pointLength-lastPointLength)*zoomspeed;
					if(currentsize>minSize&&currentsize<maxSize)
					{
						GetComponent<Camera>().orthographicSize = currentsize;	
						currentSize = currentsize;

						//在放大缩小或移动时禁用弹出界面;//chenfq by add 2014.4.10
						ClosePop();
					}
				}
				
				
				
				//开始位移;
				Vector3 cameraPos = GetComponent<Camera>().transform.localPosition;					
				Vector3 moveOffset = point1 - lastPoint1;
				
				
				cameraPos -= moveOffset*moveSpeed;
				
				minMoveX = 4f/3f*GetComponent<Camera>().orthographicSize - 4f/3f;
				maxMoveX = 4f/3f - 4f/3f*GetComponent<Camera>().orthographicSize;
				minMoveY = GetComponent<Camera>().orthographicSize - 1f;
				maxMoveY =  1f - GetComponent<Camera>().orthographicSize;
				
				if(cameraPos.x<minMoveX)cameraPos.x=minMoveX;
				if(cameraPos.x>maxMoveX)cameraPos.x=maxMoveX;
				if(cameraPos.y<minMoveY)cameraPos.y=minMoveY;
				if(cameraPos.y>maxMoveY)cameraPos.y=maxMoveY;
				
				
				
				
				
				GetComponent<Camera>().transform.localPosition = cameraPos;	
				lastPoint1 = point1;
				lastPoint2 = point2;

				//在放大缩小或移动时禁用弹出界面;//chenfq by add 2014.4.10
				ClosePop();
			}
			else
			{
				//兼容mouse操作;
				if(Input.GetMouseButton(0)&&isMouseDrag)
				{			
					Vector3 cameraPos = GetComponent<Camera>().transform.localPosition;	
					
					
					Vector3 moveOffset = point1 - lastPoint1;
					cameraPos -= moveOffset*moveSpeed;
					
					minMoveX = 4f/3f*GetComponent<Camera>().orthographicSize - 4f/3f;
					maxMoveX = 4f/3f - 4f/3f*GetComponent<Camera>().orthographicSize;
					minMoveY = GetComponent<Camera>().orthographicSize - 1f;
					maxMoveY =  1f - GetComponent<Camera>().orthographicSize;
					
					if(cameraPos.x<minMoveX)cameraPos.x=minMoveX;
					if(cameraPos.x>maxMoveX)cameraPos.x=maxMoveX;
					if(cameraPos.y<minMoveY)cameraPos.y=minMoveY;
					if(cameraPos.y>maxMoveY)cameraPos.y=maxMoveY;
					
					
					
					GetComponent<Camera>().transform.localPosition = cameraPos;	
					lastPoint1 = point1;

					//在放大缩小或移动时禁用弹出界面;//chenfq by add 2014.4.10
					ClosePop();
				}
				
				
				if(Input.GetMouseButton(1)&&isMouseDrag)
				{			
					float pointLength = point1.x-lastPoint1.x;
					
					
					float currentsize = GetComponent<Camera>().orthographicSize-pointLength*zoomspeed;
					if(currentsize>minSize&&currentsize<maxSize)
					{
						GetComponent<Camera>().orthographicSize = currentsize;	
						currentSize = currentsize;

						//在放大缩小或移动时禁用弹出界面;//chenfq by add 2014.4.10
						ClosePop();
					}
					
					lastPoint1 = point1;
				}
				
				
			}
			
		}
		
	}
	
	public float zoomspeed = 0.005f;
}
