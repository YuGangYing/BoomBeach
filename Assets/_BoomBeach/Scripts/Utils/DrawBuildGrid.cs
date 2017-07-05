using UnityEngine;
using System.Collections;



public class DrawBuildGrid : MonoBehaviour {

	public Mesh lineMesh;	
	public float lineWidth = 0.1f;
	public int gridCount = 3;




	void Awake()
	{
		lineMesh = GetComponent<MeshFilter>().mesh;
		//init ();
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
				GetComponent<Renderer>().enabled = false;
				isDrawing = false;
			}
		}

		if(alpha>0)
		{
			Color32[] col = new Color32[lineMesh.colors32.Length];
			for(int i=0;i<col.Length;i++)
			{
				col[i] = new Color(1f,1f,1f,alpha);
			}
			lineMesh.colors32 = col;
			GetComponent<Renderer>().enabled = true;
		}


		//renderer.material.SetFloat ("_Alpha",alpha);
		//drawLine();
	}

	public void drawLine()
	{
		init ();
		alpha = 0f;
		isShow = true;
		isDrawing = true;
		Color32[] col = new Color32[lineMesh.colors32.Length];
		for(int i=0;i<col.Length;i++)
		{
			col[i] = new Color(1f,1f,1f,alpha);
		}
		lineMesh.colors32 = col;
		GetComponent<Renderer>().enabled = true;
	}
	
	public void init()
	{
		maxAlpha = 1f;
		GetComponent<Renderer>().enabled = true;
		int lineVextexCount = gridCount*gridCount*4*4;		
		
		DrawMeshData lineMeshData = new DrawMeshData();
		lineMeshData.vex = new Vector3[lineVextexCount];
		
		lineMeshData.uv = new Vector2[lineVextexCount];
		lineMeshData.trianges = new int[lineVextexCount/2*3];
		lineMeshData.vexIndex = 0;
		lineMeshData.triIndex = 0;
		
		
		for(int x=0;x<gridCount;x++)
		{
			for(int z=0;z<gridCount;z++)
			{			

				if(gridCount>5)
				{
					GridInfo gridInfo = Globals.GridArray[x,z];
					if(gridInfo!=null)
					{
						if((gridInfo.cost==Globals.GridBuildCost&&!gridInfo.isBuild)||(gridInfo.isBuild&&gridInfo.buildInfo!=MoveOpEvent.Instance.SelectedBuildInfo))
						{
							continue;
						}
					}
				}


				Vector3 offset = new Vector3(x,0,z);

				Vector3 pd = Vector3.zero+offset;
				Vector3 pl = new Vector3(0,0,1)+offset;
				Vector3 pr = new Vector3(1,0,0)+offset;
				Vector3 pu = new Vector3(1,0,1)+offset;
						

				
				//上;
				Vector3 beginPoint = pl;
				Vector3 endPoint = pu;
				lineMeshData = fillLineMeshData(lineMeshData,beginPoint,endPoint,lineWidth,1);
			
										
				//下;
				 beginPoint = pd;
				 endPoint = pr;
				lineMeshData = fillLineMeshData(lineMeshData,beginPoint,endPoint,lineWidth,1);
			
				//左;
				 beginPoint = pl;
				 endPoint = pd;
				lineMeshData = fillLineMeshData(lineMeshData,beginPoint,endPoint,lineWidth,-1);
			
			
				//右;
				 beginPoint = pu;
				 endPoint = pr;
				lineMeshData = fillLineMeshData(lineMeshData,beginPoint,endPoint,lineWidth,-1);
				
			}
		}

		
		lineMesh.triangles = null;
		lineMesh.vertices = lineMeshData.vex;
		lineMesh.uv = lineMeshData.uv;
		lineMesh.SetTriangles(lineMeshData.trianges,0);

		isShow = false;


	}

	
	
	public DrawMeshData fillLineMeshData(DrawMeshData meshData,Vector3 beginPoint,Vector3 endPoint,float lineWidth,int DirectionFanwe)
	{
		lineWidth = lineWidth/2;
		Vector3 beginPointInGame = beginPoint;
		Vector3 endPointInGame = endPoint;
		
		Vector3 pd = new Vector3(beginPointInGame.x-lineWidth,0f,beginPointInGame.z-lineWidth);		
		if(DirectionFanwe==-1)
			 pd = new Vector3(endPointInGame.x-lineWidth,0f,endPointInGame.z-lineWidth);		
		Vector2 pdUv =  Vector2.zero;   //new Vector2(0.5f,4f/74f);
		
		Vector3 pl = new Vector3(beginPointInGame.x-lineWidth,0f,beginPointInGame.z+lineWidth);	
		if(DirectionFanwe==-1)
			 pl = new Vector3(beginPointInGame.x-lineWidth,0f,beginPointInGame.z+lineWidth);
		Vector2 plUv = Vector2.zero; //new Vector2(4f/96f,0.5f);
		
		Vector3 pr = new Vector3(endPointInGame.x+lineWidth,0f,endPointInGame.z-lineWidth);		
		Vector2 prUv = Vector2.zero; //new Vector2(92f/96f,0.5f);
		
		Vector3 pu = new Vector3(endPointInGame.x+lineWidth,0f,endPointInGame.z+lineWidth);	
		if(DirectionFanwe==-1)
			pu = new Vector3(beginPointInGame.x+lineWidth,0f,beginPointInGame.z+lineWidth);	

		Vector2 puUv = Vector2.zero; //new Vector2(0.5f,70f/74f);
		
		int pdIdx = 0;
		int plIdx = 0;
		int prIdx = 0;
		int puIdx = 0;
		

		meshData.vex[meshData.vexIndex] = pd;
		meshData.uv[meshData.vexIndex] = pdUv;
		pdIdx = meshData.vexIndex;
		meshData.vexIndex++;		
		
		meshData.vex[meshData.vexIndex] = pl;
		meshData.uv[meshData.vexIndex] = plUv;
		plIdx = meshData.vexIndex;
		meshData.vexIndex++;
		
		meshData.vex[meshData.vexIndex] = pu;
		meshData.uv[meshData.vexIndex] = puUv;
		puIdx = meshData.vexIndex;
		meshData.vexIndex++;
		
		meshData.vex[meshData.vexIndex] = pr;
		meshData.uv[meshData.vexIndex] = prUv;
		prIdx = meshData.vexIndex;
		meshData.vexIndex++;
		
		meshData.trianges[meshData.triIndex] = pdIdx;
		meshData.triIndex++;
		meshData.trianges[meshData.triIndex] = plIdx;
		meshData.triIndex++;
		meshData.trianges[meshData.triIndex] = puIdx;
		meshData.triIndex++;
		meshData.trianges[meshData.triIndex] = pdIdx;
		meshData.triIndex++;
		meshData.trianges[meshData.triIndex] = puIdx;
		meshData.triIndex++;
		meshData.trianges[meshData.triIndex] = prIdx;
		meshData.triIndex++;
			
		
		return meshData;
	}

	public void clearMesh()
	{
		isShow = false;
		isDrawing = true;
		//lineMesh.Clear ();
	}
}
