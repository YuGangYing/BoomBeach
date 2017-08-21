using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace BoomBeach
{
	/**
	 * 相当于一个自定义的 Resources
	 **/
	public class ResourceManager : SingleMonoBehaviour<ResourceManager>
	{

		static ResourceManager instance;
		public Atlas atlas;
		public Material grayMat;
		//manifest,全部アセートバンドルの関連だ。
		AssetBundleManifest mManifest;
		Dictionary<string,AssetBundle> mCachedAssetbundles;

		protected override void Awake ()
		{
			base.Awake ();
			Init ();
		}

		public void Init ()
		{
			LoadManifest ();
			atlas = Resources.Load<Atlas> ("AvatarAtlas");
			atlas.Init ();
			grayMat = Resources.Load<Material> ("Materials/GrayUGUI");
		}
		#if UNITY_EDITOR
		public const string localPanelPrefabPath = "Assets/Prefabs/UI/";
		#else
        public const string localPanelPrefabPath = "UI/CSharp/";
		#endif
		bool mIsLocal = false;

		public GameObject LoadLocalPanelPrefab (string panelName)
		{
			string path = localPanelPrefabPath + panelName + ".prefab";
			GameObject prefab = null;
			#if UNITY_EDITOR
				 prefab = AssetDatabase.LoadAssetAtPath<GameObject> (path);
			#endif
			return prefab;
		}

		//アセットバンドルがロードされる
		AssetBundle GetOrLoalAssetbundle (string abName)
		{
			string fullABName = abName + ".assetbundle";
			if (mCachedAssetbundles.ContainsKey (fullABName) && mCachedAssetbundles [fullABName] != null) {
				return mCachedAssetbundles [fullABName];
			}
			string path = PathConstant.CLIENT_AB_PATH + fullABName;
			//関連するの資源が準備される。
			string[] dependencies = this.mManifest.GetAllDependencies (fullABName);
			for (int i = 0; i < dependencies.Length; i++) {
				if (!mCachedAssetbundles.ContainsKey (dependencies [i])) {
					mCachedAssetbundles.Add (dependencies [i], AssetBundle.LoadFromFile (PathConstant.CLIENT_AB_PATH + dependencies [i]));
				} else if (mCachedAssetbundles.ContainsKey (dependencies [i]) == null) {
					mCachedAssetbundles [dependencies [i]] = AssetBundle.LoadFromFile (PathConstant.CLIENT_AB_PATH + dependencies [i]);
				}
			}
			AssetBundle assetBundle = AssetBundle.LoadFromFile (path);
			return assetBundle;
		}

		//アセットバンドルがリリースされる
		void ReleaseAssetbundle (string abName)
		{
			string fullABName = abName + ".assetbundle";
			if (!mCachedAssetbundles.ContainsKey (fullABName) || mCachedAssetbundles [fullABName] == null) {
				return;
			}
			AssetBundle assetbundle = mCachedAssetbundles [fullABName];
			assetbundle.Unload (true);
			mCachedAssetbundles.Remove (fullABName);
			//TODO 需要做自动卸载
			//string[] dependencies = this.mManifest.GetAllDependencies (fullABName);
			//HashSet<string> dependenciesHashSet = new HashSet<string> ();
		}

		public GameObject LoadLocalAssetbundlePanel (string panelName)
		{
			GameObject prefab = null;
			AssetBundle assetbundle = GetOrLoalAssetbundle ("prefab_ui_" + panelName.ToLower ());
			if (assetbundle != null) {
				prefab = assetbundle.LoadAsset<GameObject> (panelName);
			}
			return prefab;
		}

		public GameObject LoadUIPrefab (string panelName)
		{
			#if UNITY_EDITOR
			if (mIsLocal) {
				return LoadLocalPanelPrefab (panelName);
			} else {
				return LoadLocalAssetbundlePanel (panelName);
			}
			#else
			return LoadLocalAssetbundlePanel (panelName);
			#endif
		}

		void LoadManifest ()
		{
			mCachedAssetbundles = new Dictionary<string, AssetBundle> ();
			AssetBundle	mManifestAssetBundle = AssetBundle.LoadFromFile (PathConstant.CLIENT_MANIFEST_PATH);
			if(mManifestAssetBundle!=null)
				mManifest = mManifestAssetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		}

	}

}
