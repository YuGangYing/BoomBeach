using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoomBeach
{
	public class ResourceManager : SingleMonoBehaviour<ResourceManager>
	{

		public Atlas atlas;
		public Material grayMat;

		protected override void Awake()
		{
			base.Awake ();
			Init();
		}

		public void Init()
		{
			atlas = Resources.Load<Atlas>("AvatarAtlas");
			atlas.Init();
			grayMat = Resources.Load<Material>("Materials/GrayUGUI");
		}
		public const string localPanelPrefabPath = "Assets/_BoomBeach/Prefabs/UI/CSharp/";

		public GameObject LoadLocalPanelPrefab(string panelName)
		{
//			#if UNITY_EDITOR
//			string path = localPanelPrefabPath + panelName + ".prefab";
//			Debug.Log(path);
//			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//			return prefab;
//			#else
			GameObject prefab = Resources.Load<GameObject>(localPanelPrefabPath + panelName);
			return prefab;
//			#endif
		}

		//DOTO
		public GameObject LoadLocalAssetbundlePanel(string panelName)
		{
			return null;
		}

		public GameObject GetBuilding(Network.BuildingModel buildingModel,MUnitCSVStructure unitCSV){
			string buildLayoutPath = "Model/LayoutNew/build" + unitCSV.width;
			GameObject buildLayoutInstance =  Instantiate (ResourceCache.load (buildLayoutPath)) as GameObject;
			string buildNewSpritePath = "Model/Build/buildnew" + unitCSV.width;
			GameObject buildNewSpriteInstance = Instantiate (ResourceCache.load (buildNewSpritePath)) as GameObject;

			string buildSpPath = "Model/Build/" + unitCSV.ExportName + "_sp";
			string buildSpritePath = "Model/Build/" + unitCSV.ExportName;
			if (ResourceCache.load (buildSpritePath) != null)
				buildSpriteInstance = Instantiate (ResourceCache.load (buildSpritePath)) as GameObject;
			if (ResourceCache.load (buildSpPath) != null)
				buildSpInstance = Instantiate (ResourceCache.load (buildSpPath)) as GameObject;



		}


	}
}
