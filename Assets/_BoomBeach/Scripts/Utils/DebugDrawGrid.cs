using UnityEngine;
using System.Collections;



public class DebugDrawGrid : MonoBehaviour {

	public Mesh lineMesh;	
	public float lineWidth = 0.1f;
	public int gridCount = 40;




	void Awake()
	{
		lineMesh = GetComponent<MeshFilter>().mesh;
	}

	public bool isShow;
	private float alpha;

	void Update()
	{
		if(isShow)
		{
			alpha = 1f;
			if(!GetComponent<Renderer>().enabled)
			{
				drawLine();
				GetComponent<Renderer>().material.SetFloat ("_Alpha",alpha);
				GameObject idxWords = new GameObject("Idx");
				idxWords.transform.parent = this.transform;
				idxWords.transform.localPosition = Vector3.zero;
				idxWords.transform.localScale = Vector3.one;
				for(int x=0;x<gridCount;x++)
				{
					for(int z=0;z<gridCount;z++)
					{
						GameObject o = new GameObject(x+"_"+z);
						o.AddComponent<GUIText>();
						o.transform.parent = idxWords.transform;
					}
				}

			}

			for(int x=0;x<gridCount;x++)
			{
				for(int z=0;z<gridCount;z++)
				{
					Vector3 pos = new Vector3(x+0.4f,0f,z+0.8f);
					GUIText tex = transform.Find("Idx/"+x+"_"+z).GetComponent<GUIText>();
					tex.transform.position = Camera.main.ScreenToViewportPoint(Camera.main.WorldToScreenPoint (pos));
					tex.transform.position = new Vector3(tex.transform.position.x,tex.transform.position.y,0f);
					tex.text = x+"_"+z;
					tex.color = Color.black;
					tex.transform.gameObject.layer = 9;
				}
			}


		}
		else
		{
			GetComponent<Renderer>().enabled = false;
			GetComponent<Renderer>().material.SetFloat ("_Alpha",0f);
			if(transform.Find("Idx")!=null)
			DestroyImmediate(transform.Find("Idx").gameObject);
		}



		//drawLine();
	}

	
	public void drawLine()
	{




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

}
