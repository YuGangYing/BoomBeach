using UnityEngine;
using System.Collections;

public class PanelBase : MonoBehaviour {

    public static PanelBase current;
    public GameObject root;
    //public UIButton closeButton;
    //public UIButton returnButton;
    public PanelBase preBuildingPanel;

    public virtual void Awake()
    {

    }


    // Use this for initialization
    protected virtual void Start()
    {
		/*
        if (returnButton)
            returnButton.onClick.Add(new EventDelegate(Return));
        if (closeButton)
        {
            closeButton.onClick.Add(new EventDelegate(Close));
        }
        */
    }

	public virtual void Active(){
		root.SetActive (true);
		current = this;
	}

	public void Close()
	{
//		Debug.Log ("Close");
		root.SetActive (false);
//		CameraController.SingleTon ().enabled = true;
//		EasyTouch.instance.enable =true;
//		BuildingController.SingleTon ().enabled = true;
//		CityPanel.SingleTon ().root.SetActive (true);
	}
	
	void Return()
	{
		if (preBuildingPanel) {
//			Debug.Log ("Return");
			preBuildingPanel.Active();
			root.SetActive(false);
		}
	}
  
}
