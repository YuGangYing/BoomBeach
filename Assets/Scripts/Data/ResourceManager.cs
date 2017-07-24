using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif
using System.Collections;

namespace BoomBeach {

	public class ResourceManager : SingleMonoBehaviour<ResourceManager> {

        static ResourceManager instance;
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
#if UNITY_EDITOR
        public const string localPanelPrefabPath = "Assets/_BoomBeach/Prefabs/UI/CSharp/";
#else
        public const string localPanelPrefabPath = "UI/CSharp/";
#endif
        public GameObject LoadLocalPanelPrefab(string panelName)
        {
#if UNITY_EDITOR
            string path = localPanelPrefabPath + panelName + ".prefab";
            Debug.Log(path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab;
#else
            GameObject prefab = Resources.Load<GameObject>(localPanelPrefabPath + panelName);
            return prefab;
#endif
        }

        //DOTO
        public GameObject LoadLocalAssetbundlePanel(string panelName)
        {
            return null;
        }

    }

}
