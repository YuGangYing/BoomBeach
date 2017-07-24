using UnityEngine;
using System.Collections;
using BoomBeach;

public class PartEmitObj : MonoBehaviour {

	public Camera UICamera;
	public Camera MainCamera;
	public Transform GoldIco;
	public Transform WoodIco;
	public Transform StoneIco;
	public Transform IronIco;
	public Transform ExpIco;
	public Transform GemIco;
	public UIAtlas avatarAtlas;
	public UIAtlas uiAtlas;
	public UIAtlas avatarFullAtlas;

	private static PartEmitObj instance;
	public static PartEmitObj Instance{
		get{
			return instance;
		}
	}

    void Awake()
    {
        instance = this;
    }

    void Start()
	{
		UICamera = UIManager.GetInstance.cameraUI;
		//CityController.SingleTon().cameraUI;


        MainInterfacePanelView panel = UIManager.GetInstance.GetPanel<MainInterfacePanelView>("MainInterfacePanel");
        GoldIco = panel.m_btnGold.transform;
        WoodIco = panel.m_btnWood.transform;
        StoneIco = panel.m_btnStone.transform;
        IronIco = panel.m_btnIron.transform; ;
        ExpIco = panel.m_imgLevelbar.transform; ;
        GemIco = panel.m_imgDiamond.transform;


    }
}
