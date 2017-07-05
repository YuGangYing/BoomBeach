using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	private static SceneLoader instance;
	public static SceneLoader Instance{
		get{
			return instance;
		}
	}
	private Animator mAnim;

	void Awake()
	{
		instance = this;
		mAnim = GetComponent<Animator>();
	}


	private bool cloudClose;
	public Globals.OnLoadBegin beginLoad;
	public void BeginLoad()
	{
		AudioPlayer.Instance.PlaySfx("cloud_appear_01");
		mAnim.Play("CloudIn");
		cloudClose = true;
	}


	public Globals.OnLoadEnd endLoad;
	public void EndLoad()
	{
		if(cloudClose)
		{
			AudioPlayer.Instance.PlaySfx("cloud_break_01");
			mAnim.Play("CloudOut");
		}
		else
		{
			OnEndLoad();
		}
	}


	//以下是开放给animator的事件;
	public void OnBeginLoad(){
		CameraOpEvent.Instance.ResetIm (new Vector3(-10,30,0));
		if(beginLoad!=null)
			beginLoad();
	}

	public void OnEndLoad(){
		if(endLoad!=null)
			endLoad();

		//先执行完加载后的操作，再运行委拖;
		CameraOpEvent.Instance.Reset (new Vector3(-10,30,0),15f);
		cloudClose = false;
	}
}
