using UnityEngine;
using System.Collections;

[AddComponentMenu("2D Toolkit/Demo/tk2dDemoReloadController")]
public class tk2dDemoReloadController : MonoBehaviour 
{
	void Reload()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex);
		//Application.LoadLevel(Application.loadedLevel);
	}
}
