using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

public class ABSetting : EditorWindow
{
	static string ugui_prefab_path = Application.dataPath + "/Prefabs/UI";
	static string ugui_image_path = Application.dataPath + "/Images/UI";

	[MenuItem ("Tools/Set UI AssetBundle")] 
	static void InitUGUIAssetBundleName ()
	{
		SetUGUIPrefabAssetBundleName ();
		SetUGUIImageAssetBundleName ();
	}

	static void SetUGUIPrefabAssetBundleName ()
	{
		string[] files = FileManager.GetFiles (ugui_prefab_path, "*", SearchOption.TopDirectoryOnly);
		for (int i = 0; i < files.Length; i++) {
			string fileName = files [i].Substring (files [i].LastIndexOf ("/") + 1);
			Debug.Log ("fileName:" + fileName);
			fileName = fileName.Substring (0, fileName.IndexOf ('.'));
			string subDir1 = files [i].Substring (files [i].LastIndexOf ("/Assets/") + 1);
			AssetImporter assetImporter = AssetImporter.GetAtPath (subDir1);
			if (assetImporter != null) {
				assetImporter.SetAssetBundleNameAndVariant ("prefab_ui_" + fileName, "assetbundle");
			}
		}
	}

	static void SetUGUIImageAssetBundleName ()
	{
		string[] dirs = FileManager.GetDirectories (ugui_image_path, "*", SearchOption.TopDirectoryOnly);
		for (int i = 0; i < dirs.Length; i++) {
			string dirName = dirs [i].Substring (dirs [i].LastIndexOf ("/") + 1);
			string subDir = dirs [i].Substring (dirs [i].LastIndexOf ("/Assets/") + 1);
			AssetImporter assetImporter = AssetImporter.GetAtPath (subDir);
			if (assetImporter != null) {
				assetImporter.SetAssetBundleNameAndVariant ("sprite_ui_" + dirName, "assetbundle");
			}
		}
	}

	//
	//	static void _InitAnimation ()
	//	{
	//		string[] dirs = FileManager.GetDirectories (animationImagePath, "*", SearchOption.TopDirectoryOnly);
	//		List<string> jsonPaths = new List<string> ();
	//		List<string> jsonABPaths = new List<string> ();
	//		foreach (string dir in dirs) {
	//			string subDir = dir.Substring (dir.LastIndexOf ("/") + 1);
	//			string[] files = FileManager.GetFiles (dir, "*", SearchOption.AllDirectories);
	//			foreach (string file in files) {
	//				string subDir1 = file.Substring (file.LastIndexOf ("/Assets/") + 1);
	//				if (file.LastIndexOf (".meta") == -1) {
	//					AssetImporter assetImporter = AssetImporter.GetAtPath (subDir1);
	//					if (file.LastIndexOf (".png") != -1) {
	//						TextureImporter textureImporter = assetImporter as TextureImporter;
	//						if (textureImporter.textureCompression != TextureImporterCompression.Uncompressed
	//						    || textureImporter.textureType != TextureImporterType.Sprite
	//						    || textureImporter.mipmapEnabled
	//						    || string.IsNullOrEmpty (textureImporter.spritePackingTag)
	//						    || textureImporter.spritePackingTag != subDir) {
	//							textureImporter.textureType = TextureImporterType.Sprite;
	//							textureImporter.mipmapEnabled = false;
	//							if(subDir.Substring(subDir.Length-1) != "9")
	//								textureImporter.spritePackingTag = subDir;
	//							textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
	//							textureImporter.SaveAndReimport ();
	//						}
	//					}
	//					if (assetImporter != null) {
	//						assetImporter.SetAssetBundleNameAndVariant (subDir, "assetbundle");
	//					}
	//					if (file.LastIndexOf (".csv") != -1) {
	//						TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset> (subDir1);
	//						string text = ta.text;
	//						string[] strs = text.Split (new char[]{ '\n' });
	//						Debug.Log (strs.Length);
	//						string result = "";
	//						for (int i = 0; i < strs.Length; i++) {
	//							if (strs [i].Trim ().Length > 0) {
	//								if (strs [i].Trim ().Substring (0, 1) == ",") {
	//									strs [i] = i + strs [i].Trim ();
	//								}
	//								result += strs [i].Trim() + "\n";
	//							}
	//						}
	//						FileManager.WriteString (file, result);
	//						AssetDatabase.SaveAssets ();
	//						string outPutPath = dir + "/" + subDir + "_json.json";
	//						CSVToJsonWin.CoverAnimationJson (subDir1, outPutPath);
	//						subDir1 = outPutPath.Substring (file.LastIndexOf ("/Assets/") + 1);
	//						jsonPaths.Add (subDir1);
	//						jsonABPaths.Add (subDir);
	////						assetImporter = AssetImporter.GetAtPath (subDir1);
	////						if (assetImporter != null) {
	////							assetImporter.SetAssetBundleNameAndVariant (subDir, "assetbundle");
	////						}
	//					}
	//				}
	//			}
	//		}
	//
	//		for (int i=0;i<jsonPaths.Count;i++) {
	//			string str = jsonPaths[i];
	//			string abStr = jsonABPaths [i];
	//			AssetDatabase.ImportAsset (str);
	//			AssetImporter assetImporter = AssetImporter.GetAtPath (str);
	//			if (assetImporter != null) {
	//				assetImporter.SetAssetBundleNameAndVariant (abStr, "assetbundle");
	//			}
	//		}
	//	}
	//
	//	static string illustrationImagePath = Application.dataPath + "/Illustrations";
	//
	//	[MenuItem ("Tools/一緒にIllustration AB Setting")]
	//	static void InitIllustration ()
	//	{
	//		string[] dirs = FileManager.GetDirectories (illustrationImagePath, "*", SearchOption.TopDirectoryOnly);
	//		foreach (string dir in dirs) {
	//			string subDir = dir.Substring (dir.LastIndexOf ("/") + 1);
	//			string[] files = FileManager.GetFiles (dir, "*", SearchOption.AllDirectories);
	//			foreach (string file in files) {
	//				string subDir1 = file.Substring (file.LastIndexOf ("/Assets/") + 1);
	//				if (file.LastIndexOf (".meta") == -1) {
	//					AssetImporter assetImporter = AssetImporter.GetAtPath (subDir1);
	//					if (file.LastIndexOf (".png") != -1) {
	//						TextureImporter textureImporter = assetImporter as TextureImporter;
	//						if (textureImporter.textureCompression != TextureImporterCompression.Uncompressed || textureImporter.textureType != TextureImporterType.Sprite || textureImporter.mipmapEnabled || string.IsNullOrEmpty (textureImporter.spritePackingTag)) {
	//							textureImporter.textureType = TextureImporterType.Sprite;
	//							textureImporter.mipmapEnabled = false;
	//							textureImporter.spritePackingTag = subDir;
	//							textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
	//							textureImporter.SaveAndReimport ();
	//						}
	//					}
	//					if (assetImporter != null) {
	//						assetImporter.SetAssetBundleNameAndVariant (subDir, "assetbundle");
	//					}
	//				}
	//			}
	//		}
	//		Debug.Log ("Illustration AB Reset Done!");
	//	}
	//
	//	static string soundPath = Application.dataPath + "/Sounds";
	//
	//	[MenuItem ("Tools/一緒にSounds AB Setting")]
	//	static void InitSounds ()
	//	{
	//		string[] dirs = FileManager.GetDirectories (soundPath, "*", SearchOption.TopDirectoryOnly);
	//
	//		foreach (string dir in dirs) {
	//			string subDir = dir.Substring (dir.LastIndexOf ("/") + 1);
	//			string[] files = FileManager.GetFiles (dir, "*", SearchOption.AllDirectories);
	//			foreach (string file in files) {
	//				try {
	//					string subDir1 = file.Substring (file.LastIndexOf ("/Assets/") + 1);
	//					if (file.LastIndexOf (".meta") == -1) {
	//						AssetImporter assetImporter = AssetImporter.GetAtPath (subDir1);
	//						AudioImporter textureImporter = assetImporter as AudioImporter;
	//						if (textureImporter != null) {
	//							AudioImporterSampleSettings setting = new AudioImporterSampleSettings ();
	//							setting.loadType = AudioClipLoadType.CompressedInMemory;
	//							setting.compressionFormat = AudioCompressionFormat.Vorbis;
	//							if (subDir == "voice_bgm")
	//								setting.quality = 0.1f;
	//							else
	//								setting.quality = 1f;
	//							textureImporter.defaultSampleSettings = setting;
	//							textureImporter.SaveAndReimport ();
	//						}
	//						if (assetImporter != null) {
	//							assetImporter.SetAssetBundleNameAndVariant (subDir, "assetbundle");
	//						}
	//					}
	//				} catch (Exception ex) {
	//					Debug.Log (ex.Message);
	//				}
	//			}
	//		}
	//		Debug.Log ("Sounds AB Reset Done!");
	//	}
}
