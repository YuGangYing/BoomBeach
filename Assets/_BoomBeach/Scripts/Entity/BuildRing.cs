using UnityEngine;
using System.Collections;

public class BuildRing : MonoBehaviour {
	
	public float OuterRange{
		set{
			if(OuterSprite==null)
				OuterSprite = transform.Find("buildOuterRing").GetComponent<tk2dSprite>();
			OuterSprite.scale = Vector3.one * value *2f;
			if(value==0)
				OuterSprite.transform.GetComponent<Renderer>().enabled = false;
			else
				OuterSprite.transform.GetComponent<Renderer>().enabled = true;
		}
	}
	public float InnerRange{
		set{
			if(InnerSprite==null)
				InnerSprite = transform.Find("buildInnerRing").GetComponent<tk2dSprite>();
			InnerSprite.scale = Vector3.one * value *2f;
			if(value==0)
				InnerSprite.transform.GetComponent<Renderer>().enabled = false;
			else
				InnerSprite.transform.GetComponent<Renderer>().enabled = true;
		}
	}

	private tk2dSprite OuterSprite;
	private tk2dSprite InnerSprite;





	public void Show()
	{
		Animator anim = GetComponent<Animator> ();
		anim.enabled = true;
		if(anim!=null)
		{
			anim.Play("RingShow");
		}

	}

	public void Hide()
	{

		Animator anim = GetComponent<Animator> ();
		if(anim!=null)
		{
			anim.Play("RingHide");
		}
	}


	public void beginShow()
	{
		if(OuterSprite==null)
			OuterSprite = transform.Find("buildOuterRing").GetComponent<tk2dSprite>();
		if(InnerSprite==null)
			InnerSprite = transform.Find("buildInnerRing").GetComponent<tk2dSprite>();
		OuterSprite.gameObject.SetActive (true);
		InnerSprite.gameObject.SetActive (true);
	}

	public void endHide()
	{
		if(OuterSprite==null)
			OuterSprite = transform.Find("buildOuterRing").GetComponent<tk2dSprite>();
		if(InnerSprite==null)
			InnerSprite = transform.Find("buildInnerRing").GetComponent<tk2dSprite>();
		OuterSprite.gameObject.SetActive (false);
		InnerSprite.gameObject.SetActive (false);
		Animator anim = GetComponent<Animator> ();
		anim.enabled = false;
	}
}
