using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TestBattle : MonoBehaviour {

	public bool load;
	public Transform units;
	public Transform units1;

	public GameObject prefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (load) {
			load = false;
			//Load ();
			MirTexture();
		}
	}

	void MirTexture(){
		#if UNITY_EDITOR
		Object[] objs = UnityEditor.Selection.objects;
		for(int i=0;i<objs.Length;i++){
			Texture2D tex = (Texture2D)objs[i];
			MirPic(tex);
		}
		#endif
	}
	private Texture2D MirPic(Texture2D texture2d)
	{
		//Texture2D texture2d = (Texture2D)Resources.Load (path + "/" + textureName, typeof(Texture2D)); //获取原图片
		int width = texture2d.width;//得到图片的宽度.   
		int height = texture2d.height;//得到图片的高度
		Texture2D NewTexture2d = new Texture2D(width, height);//创建一张同等大小的空白图片 
		int i_start = 0; 
		int start = 0;
		while( i_start  <width  )//如果是垂直翻转的话将width  换成 height 
		{
			start++;
			NewTexture2d.SetPixels(start, 0, 1, height, texture2d.GetPixels(width - start - 1, 0, 1, height));
		}
		NewTexture2d.Apply();
		texture2d.SetPixels (NewTexture2d.GetPixels ());
		texture2d.Apply ();
		return NewTexture2d;
	}
	void Load(){
		Renderer[] rs = units.GetComponentsInChildren<MeshRenderer> ();
		foreach(MeshRenderer r in rs){
			GameObject go = Instantiate<GameObject> (prefab);
			go.transform.parent = r.transform;
			go.transform.localPosition = Vector3.zero;
			DestroyImmediate (r);
		}
		rs = units1.GetComponentsInChildren<MeshRenderer> ();
		foreach(MeshRenderer r in rs){
			GameObject go = Instantiate<GameObject> (prefab);
			go.transform.parent = r.transform;
			go.transform.localPosition = Vector3.zero;
			DestroyImmediate (r);
		}
	}
}
