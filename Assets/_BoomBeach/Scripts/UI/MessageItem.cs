using UnityEngine;
using System.Collections;

public class MessageItem : MonoBehaviour {

	private float LifeTime = 5f;
	private float ColorDown = 0.01f;
	private UILabel label;

	void Awake()
	{
		LifeTime = 5f;
		label = GetComponent<UILabel> ();
	}
	// Update is called once per frame
	void Update () 
	{
		if(LifeTime>0)
		{
			LifeTime-=Time.deltaTime;
		}
		else
		{
			float alpha = label.color.a-ColorDown;
			label.color = new Color(label.color.r,label.color.g,label.color.b,alpha);
			if(alpha<=0)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
			}

		}
	}
}
