using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using BoomBeach;

public class WorldBtnEvent : MonoBehaviour {
	public Camera uiCamera;
	public Camera worldCamera;
	public Color cloudColor;
	public Color cloudColor2;
	//单例定义;
	private static WorldBtnEvent instance;
	public static WorldBtnEvent Instance{
		get{ return instance; }
	}

	void Awake()
	{
		instance = this;
	}

	void OnTouchUp(Vector3 touchPosition)
	{
		Debug.Log("OnTouchUp");

	}
	//判断是否有：碰到ui层;, 有碰到返回: true, 没有碰到返回: false
	public bool checkUIOp2(Vector3 touchPosition)
	{
		//Debug.Log("xxx");
		
		Ray uiRay = uiCamera.ViewportPointToRay (uiCamera.ScreenToViewportPoint(touchPosition));
				
		//Debug.Log("uiRay:" + Physics.Raycast (uiRay, 1000f, 1 << 9));
		//Debug.Log("ui2Ray:" + Physics.Raycast (ui2Ray, 1000f, 1 << LayerMask.NameToLayer("World")));
		
		if (Physics.Raycast (uiRay, 1000f, 1 << 9)) 
		{
			//Debug.Log("checkUIOp2:true");
			return true;
		}else if(EventSystem.current.currentSelectedGameObject!=null)
        {
            return true;
        }
		else
		{
			//Debug.Log("checkUIOp2:false");
			return false;
		}						
	}

	public void OnExplorationClick(tk2dUIItem sender){
		Debug.Log(sender.name);
		Vector3 touchPosition = Vector3.zero;
		if(Input.touchCount>0)
		{
			//屏幕点;
			touchPosition = Input.GetTouch(0).position;
		}
		else
		{
			//兼容mouse操作;		
			touchPosition = Input.mousePosition;
		}

		//Vector3 scPoint = worldCamera.WorldToScreenPoint(touchPosition);
		string regions_name = sender.name;

		Regions rs = CSVManager.GetInstance().regionsList[regions_name] as Regions;
		if (rs.sending == false){
            if (ScreenUIManage.Instance != null)
                ScreenUIManage.Instance.ShowPopWorld(null,touchPosition,true,4,false,rs);
			UIManager.GetInstance().GetController<IslandPopCtrl>().ShowCloudPop(rs, touchPosition);
			//regions_name = "r0";


			tk2dSprite[] sp = rs.cloud.GetComponentsInChildren<tk2dSprite>(true);
			//Debug.Log(rs.cloud.name + ";sp.Length:" + sp.Length);
			for(int i = 0; i < sp.Length; i ++){
				if (sp[i].transform.parent.name != "Explore")
				//Debug.Log(rs.cloud.name + ";sp.Length:" + sp.Length);
					sp[i].color = cloudColor;
			}

		}

	}

	//恢复被选中的云朵颜色;
	public void ResumeCloudColor(Transform cloud){
		//Debug.Log("ResumeCloudColor");
		tk2dSprite[] sp = cloud.GetComponentsInChildren<tk2dSprite>(true);
		//Debug.Log(rs.cloud.name + ";sp.Length:" + sp.Length);
		for(int i = 0; i < sp.Length; i ++){
			//Debug.Log(rs.cloud.name + ";sp.Length:" + sp.Length);
			if (sp[i].transform.parent.name != "Explore")
				sp[i].color = cloudColor2;
		}
	}

}
