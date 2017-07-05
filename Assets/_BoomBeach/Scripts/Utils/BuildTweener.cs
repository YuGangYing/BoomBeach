using UnityEngine;
using System.Collections;

public class BuildTweener : MonoBehaviour {


	private float maxY = 0.1f;
	private float minC = 0.3f;
	private float yStep = 0.03f;
	private float cStep = 0.03f;
	private bool isPlay;
	private bool isOffset;

	private int direct;  //1上升，-1下落;
	private float color=1; 
	private int colorDir;  //1变白 -1变黑;

	private float _alpha = 1f;
	private float _realAlpha = 1f;
	public BuildInfo buildInfo;
	private bool isPlaying;
	public float Alpha{
		set{
			if(buildInfo!=null&&buildInfo.status!=BuildStatus.Removaling)
			{
				isPlaying = true;
				_alpha = value;
			}
		}
	}


	// Update is called once per frame
	void Update () 
	{	
		if (!isPlaying)
		{
			enabled = false;
			return;
		}

		if(_alpha!=_realAlpha)
		{
			if(_alpha<_realAlpha)
			{
				_realAlpha = _realAlpha-cStep<=_alpha?_alpha:_realAlpha-cStep*Globals.TimeRatio;
			}
			else
			{
				_realAlpha = _realAlpha+cStep>=_alpha?_alpha:_realAlpha+cStep*Globals.TimeRatio;
			}
		}

		tk2dSprite[] sprites = GetComponentsInChildren<tk2dSprite>();
		for(int i=0;i<sprites.Length;i++)
		{
			sprites[i].color = new Color(sprites[i].color.r,sprites[i].color.g,sprites[i].color.b,_realAlpha);
		}

		if(isOffset)
		{
			Vector3 position = transform.localPosition;

			position = new Vector3(position.x,position.y+(yStep*direct*Globals.TimeRatio),position.z);
			if(position.y>=maxY)
			{
				direct = -1;
			}
			if(position.y<=0)
			{
				isOffset = false;
				position = new Vector3(position.x,0,position.z);
			}
			transform.localPosition = position;
		}

		if(isPlay)
		{
			color = color+cStep*colorDir*Globals.TimeRatio;

			if(color<=minC)
				colorDir = 1;
			if(color>=1)
				colorDir = -1;

			sprites = GetComponentsInChildren<tk2dSprite>();
			for(int i=0;i<sprites.Length;i++)
			{
				sprites[i].color = new Color(1f,color,1f,_realAlpha);
			}

		}

		if(!isPlay&&color<1f)
		{
			color = color+cStep*Globals.TimeRatio;

			sprites = GetComponentsInChildren<tk2dSprite>();			
			for(int i=0;i<sprites.Length;i++)
			{
				sprites[i].color = new Color(color,color,color,_realAlpha);
			}
		}

		if (_alpha == _realAlpha && !isOffset && !isPlay && color >= 1f)
						isPlaying = false;
	}

	public void Pslay()
	{
		minC = 0.5f;
		isPlay = true;
		isPlaying = true;
		isOffset = true;
		direct = 1;
		color = 1f;
		colorDir = -1;

	}

	public void PlayFlash()
	{
		minC = 0.5f;
		isPlay = true;
		isPlaying = true;
		direct = 1;
		color = 1f;
		colorDir = -1;
	}

	public void Stop()
	{
		isPlay = false;
		isPlaying = true;
	}
}
