using UnityEngine;
using System.Collections;

public class SpriteFade : MonoBehaviour {


	private tk2dSprite sprite;
	private float fadeTime;
	private int fadeDirect; //0:不处理 1:显示 -1:隐藏;
	private float alpha;

	// Use this for initialization
	void Start () {
		sprite = GetComponent<tk2dSprite>();
		alpha = 0f;
		sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,alpha);
	}
	
	// Update is called once per frame
	void Update () {
		if(fadeDirect==1)
		{
			if(alpha<1)
			{
				alpha += Time.deltaTime/fadeTime;
				if(alpha>=1)alpha = 1f;
				sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,alpha);
			}
			else
			{
				fadeDirect = 0;
			}
		}
		else if(fadeDirect==-1)
		{
			if(alpha>0)
			{
				alpha -= Time.deltaTime/fadeTime;
				if(alpha<=0)alpha = 0;
				sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,alpha);
			}
			else
			{
				fadeDirect = 0;
			}
		}
	}

	public void FadeIn(float time)
	{
		fadeTime = time;
		fadeDirect = 1;
	}

	public void FadeOut(float time)
	{
		fadeTime = time;
		fadeDirect = -1;
	}
}
