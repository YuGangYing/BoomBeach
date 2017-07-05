using UnityEngine;
using System.Collections;
using BoomBeach;

public class Weapon : MonoBehaviour {

	public bool isRandRoll;

	private tk2dSpriteAnimator weaponAnim;
	private float destAngle;
	private float currentAngle;

	private int direction = 1;  //旋转方向，1：逆时针 -1:顺时针;

	private int currentIdx;		//当前帧序号; 
	private int destIdx;		//目标帧序号;
		
	private float angleStep;  //每帧转过的角度;

	public delegate void OnAim();
	public OnAim onAim;



	private bool isRolling;

	// Use this for initialization
	void Start () 
	{
		weaponAnim = transform.Find ("model/weapon").GetComponent<tk2dSpriteAnimator> ();
		weaponAnim.Stop ();
		angleStep = 360f / weaponAnim.CurrentClip.frames.Length;
	}
	
	// Update is called once per frame
	private float TimeSpan = 0f;
	void Update () 
	{
		if(isRandRoll&&!isRolling&&DataManager.GetInstance().sceneStatus!=SceneStatus.ENEMYBATTLE&&DataManager.GetInstance().sceneStatus!=SceneStatus.BATTLEREPLAY)
		RandRoll ();

		if(isRolling)
		{
			float SPF = 1.0f / weaponAnim.ClipFps; //每帧花费时间;

			if(TimeSpan>SPF)
			{
				TimeSpan = 0f;
				if(currentIdx!=destIdx)
				{
					currentIdx+=direction;
					if(currentIdx<0)currentIdx+=weaponAnim.CurrentClip.frames.Length;
					if(currentIdx>=weaponAnim.CurrentClip.frames.Length)currentIdx=0;
					currentAngle = currentIdx*angleStep;
					weaponAnim.SetFrame(currentIdx);
				}
				else
				{
					if(onAim!=null)
					{
						if (DataManager.GetInstance().sceneStatus == SceneStatus.ENEMYBATTLE || DataManager.GetInstance().sceneStatus == SceneStatus.BATTLEREPLAY)
						onAim();					
					}
					isRolling = false;
					currentAngle = destAngle;
				}
			}
			else
			{
				TimeSpan+=Time.deltaTime;
			}
		}
	
	}



	public void aim(float angle)
	{

		isRolling = true;

		destAngle = angle;

		float offsetAngle = destAngle - currentAngle;

		if(offsetAngle>=0&&offsetAngle<180)
		{
			direction = 1;
		}
		else if(offsetAngle>180)
		{
			direction = -1;
		}
		else if(offsetAngle<0&&Mathf.Abs(offsetAngle)<180)
		{
			direction = -1;
		}
		else
			direction = 1;

		currentIdx = angleToIdx (currentAngle);
		destIdx = angleToIdx (destAngle);

	}

	public int angleToIdx(float angle)
	{
		angle += angleStep / 2f;
		int idx = Mathf.FloorToInt (angle / angleStep);
		if (idx >= weaponAnim.CurrentClip.frames.Length)
						idx = 0;
		if (idx < 0)
						idx += weaponAnim.CurrentClip.frames.Length;
		return idx;
	}


	//随机转向;
	public float RandRollTimeSpan = 2f;//每2秒发生一次随机转向
	private float RandRollTimeCounter = 0f;

    public float minSpanRandomAngle = 0;

    public float maxSpanRandomAngle = 360;

	void RandRoll()
	{
		if(RandRollTimeCounter>RandRollTimeSpan)
		{
			RandRollTimeCounter = 0f;
			float a = Random.Range(minSpanRandomAngle, maxSpanRandomAngle);
			aim(a);
		}
		else
		{
			RandRollTimeCounter += Time.deltaTime;
		}
	}
}
