using UnityEngine;
using System.Collections;

public class DrawPlan : MonoBehaviour {
	
	public int gridCount = 40;
	public Mesh planMesh;	
	
	void Awake()
	{
		planMesh = GetComponent<MeshFilter>().mesh;
		init ();
	}


	public bool isShow;
	public float alpha;
	private float maxAlpha;
	private bool isDrawing;
	void Update()
	{
		if (!isDrawing) 
		{
			enabled = false;
			return;
		}
		float step = maxAlpha / 20;
		if(isShow)
		{
			alpha+=step*Globals.TimeRatio;
			if(alpha>maxAlpha)
			{
				alpha=maxAlpha;	
				isDrawing = false;
			}

		}
		else
		{
			alpha-=step*Globals.TimeRatio;
			if(alpha<0)
			{
				alpha=0f;
				isDrawing = false;
				GetComponent<Renderer>().enabled = false;
			}
		}

		if(alpha>0)
		{
			Color32[] col = new Color32[planMesh.colors32.Length];
			for(int i=0;i<col.Length;i++)
			{
				col[i] = new Color(1f,1f,1f,alpha);
			}
			planMesh.colors32 = col;
			GetComponent<Renderer>().enabled = true;
		}
	}

	public void drawPlan()
	{
		isShow = true;
		isDrawing = true;
	}
	public void init()
	{
		alpha = 1f;
		maxAlpha = 1f;
		GetComponent<Renderer>().enabled = true;
		DrawMeshData meshData = new DrawMeshData();
		meshData.vex = new Vector3[4];
		meshData.uv = new Vector2[4];
		meshData.trianges = new int[6];
		meshData.vexIndex = 0;
		meshData.triIndex = 0;
		
		//上;
		meshData.vex[0] = new Vector3(gridCount,0f,gridCount);
		meshData.uv[0] = Vector2.zero;	
		
		//左;
		meshData.vex[1] = new Vector3(0f,0f,gridCount);
		meshData.uv[1] = Vector2.zero;
		
		//下;
		meshData.vex[2] = new Vector3(0,0,0);
		meshData.uv[2] = Vector2.zero;
		
		//右;
		meshData.vex[3] = new Vector3(gridCount,0f,0f);
		meshData.uv[3] = Vector2.zero;
		
		meshData.trianges = new int[6]{0,1,2,2,3,0};
		
		planMesh.triangles = null;
		planMesh.vertices = meshData.vex;
		planMesh.uv = meshData.uv;
		planMesh.SetTriangles(meshData.trianges,0);
		isShow = false;
		isDrawing = true;
		
	}

	public void clearMesh()
	{
		isShow = false;
		isDrawing = true;
		//planMesh.Clear ();
	}
}
