using UnityEngine;
using System.Collections;

public class CountDownController : MonoBehaviour {

	public string text{
		set{
			GetComponent<UILabel>().text = value;
		}
	}

	public void OnPlayEnd()
	{
		Destroy (gameObject);
	}
}
