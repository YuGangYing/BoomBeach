using UnityEngine;
using System.Collections;

public class CharTweener : MonoBehaviour {

	private bool isOnlyY;
	private float minC = 0.3f;
	private float cStep = 0.03f;
	private bool isPlay;

	private float color=1; 
	private int colorDir;  //1变白 -1变黑;


	private bool isPlaying;
	private float _alpha = 1f;
	private float _realAlpha = 1f;
	public float Alpha{
		set{
			_alpha = value;
		}
	}

	private bool isPlayOnce;


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

		if(isPlay)
		{
			color = color+cStep*colorDir*Globals.TimeRatio;

			if(color<=minC)
				colorDir = 1;
			if(color>=1)
			{
				if(isPlayOnce)
				{
					Stop();
				}
				else
				colorDir = -1;
			}

			sprites = GetComponentsInChildren<tk2dSprite>();
			for(int i=0;i<sprites.Length;i++)
			{
				if(isOnlyY)
					sprites[i].color = new Color(color,1f,1f,_realAlpha);
				else
					sprites[i].color = new Color(1f,color,1f,_realAlpha);
			}

		}

		if(!isPlay&&color<1f)
		{
			color = color+cStep*Globals.TimeRatio;
			sprites = GetComponentsInChildren<tk2dSprite>();			
			for(int i=0;i<sprites.Length;i++)
			{
				if(isOnlyY)
					sprites[i].color = new Color(color,1f,1f,_realAlpha);
				else
					sprites[i].color = new Color(1f,color,1f,_realAlpha);
			}
		}

		if (_alpha == _realAlpha && !isPlay && color >= 1f)
						isPlaying = false;
	}

	public void PlayLight()
	{
		isPlayOnce = true;
		minC = 0.5f;
		isOnlyY = true;
		isPlay = true;
		isPlaying = true;
		color = 1f;
		colorDir = -1;

	}

	public void PlayFlash()
	{
		isPlayOnce = false;
		minC = 0.5f;
		isOnlyY = false;
		isPlay = true;
		isPlaying = true;
		color = 1f;
		colorDir = -1;
	}

	public void Stop()
	{
		isPlay = false;
		isPlaying = true;
	}
}
