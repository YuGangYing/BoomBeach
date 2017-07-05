using UnityEngine;
using System.Collections;

using Sfs2X.Entities.Data;

public class SmallUI : MonoBehaviour{
    // 小菊花prefab
    public GameObject activityPrefab;

    public Transform normalTrans;

    public UITexture sceneFader;

    // 提示框
    public GameObject tipBox;

    // 小菊花是否显示
    public bool isShow;

    // 场景淡入淡出速度
    public float fadeSpeed = 1.5f;

    public bool isFadeIn,isFadeOut;
	
    public delegate void OnSure(ISFSObject dt);

    // 属性
    public Color SceneFaderColor
    {
        get
        {
            return sceneFader.color;
        }
        set
        {
            sceneFader.color = value;
        }
    }

    public static SmallUI Instance
    {
        get
        {
            return instance;
        }
    }

    private static SmallUI instance;

    // 小菊花
    private Transform activity;

    // 提示框确定，取消回调
    private OnSure onS, onC;

    // 提示框回调参数
    private ISFSObject dt;

	// 提示框类型数组
	private int[] types;

	private int typeCount = 3;

    void Awake()
    {
        if (instance == null)
            instance = this;

		types = new int[typeCount];
		for(int i = 1; i <= typeCount; i++){
			types[i-1] = i;
		}
    }

    void OnDestroy()
    {
        if (instance && instance == this)
            instance = null;
    }

    void Update()
    {
        if (isFadeIn)
        {
            FadeIn();
        }

        if (isFadeOut)
        {
            FadeOut();
        }
    }

    /// <summary>
    /// 打开或者关闭小菊花效果
    /// </summary>
    public void IsShowActivity(bool isS) 
    {
	/*
        if(!activity)
            activity = GameObject.FindGameObjectWithTag("centerPanel").transform.Find("Activity");

        if (!activity)
        {
            // 实例化
			Transform parent = GameObject.FindGameObjectWithTag("centerPanel").transform;
            activity = Helper.Instantiate(activityPrefab, parent, normalTrans, "Activity").transform;
        }

        activity.gameObject.SetActive(isS);

        isShow = isS;
        */
    }

    void FadeToColor(Color clr)
    {
        if (!sceneFader.enabled)
            sceneFader.enabled = true;

        sceneFader.color = Color.Lerp(sceneFader.color, clr, fadeSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 淡入效果
    /// </summary>
    void FadeIn()
    {
        FadeToColor(Color.black);

        if (sceneFader.color.a >= 0.95f)
        {
            sceneFader.color = Color.black;
            sceneFader.enabled = false;
            isFadeIn = false;
        }
    }

    /// <summary>
    /// 淡出效果
    /// </summary>
    void FadeOut()
    {
        FadeToColor(Color.clear);

        if (sceneFader.color.a <= 0.05f)
        {
            sceneFader.color = Color.clear;
            sceneFader.enabled = false;
            isFadeOut = false;
        }
    }

    /// <summary>
    /// 弹出提示框，title,message是语言包key，type 1：错误提示（标题，信息，确定按钮）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="otherParms"></param>
    /// <param name="onSure"></param>
    /// <param name="onCancel"></param>
    public void PopUpTipBox(string title, string message, ISFSObject otherParms, OnSure onSure, OnSure onCancel, int type = 1)
    {
        // 激活提示框
        tipBox.SetActive(true);

        Transform trans = tipBox.transform;
        UILabel title_lbl = trans.Find("Title").GetComponent<UILabel>();
        UILabel msg_lbl = trans.Find("Msg").GetComponent<UILabel>();

		ActiveType(trans, type);

        if (type == 1)
        {
			// 设置一些参数
        }
		else if(type == 2){
			// 设置一些参数
		}
		else if(type == 3){
			// 设置确定取消按钮文本
			Transform typeTrans = trans.Find("3");
			UILabel sure_btn_lbl = typeTrans.Find("SureButton/Label").GetComponent<UILabel>();
			UILabel cancel_btn_lbl = typeTrans.Find("CancelButton/Label").GetComponent<UILabel>();

			sure_btn_lbl.text = Localization.Get(otherParms.GetUtfString("sure_btn_lbl_key"));
			cancel_btn_lbl.text = Localization.Get(otherParms.GetUtfString("cancel_btn_lbl_key"));
		}

		title_lbl.text = Localization.Get(title);
		msg_lbl.text = Localization.Get(message);

        dt = otherParms;
        onS = onSure;
        onC = onCancel;
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type">Type.</param>
	void ActiveType(Transform tipBoxTrans, int type){
		foreach(int t in types){
			tipBoxTrans.Find(t.ToString()).gameObject.SetActive((t == type));
		}
	}

    /// <summary>
    /// 提示框确定按钮事件
    /// </summary>
    /// <param name="sender"></param>
    void OnTipBoxSureAction(GameObject sender)
    {
        // 隐藏提示框
        tipBox.SetActive(false);

        if (onS != null)
        {
            onS(dt);
        }
    }

	/// <summary>
	/// 提示框取消按钮事件
	/// </summary>
	/// <param name="sender">Sender.</param>
	void OnTipBoxCancelAction(GameObject sender)
	{
		// 隐藏提示框
		tipBox.SetActive(false);
		
		if (onC != null)
		{
			onC(dt);
		}
	}
}