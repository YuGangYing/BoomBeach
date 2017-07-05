using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

[ExecuteInEditMode]
public class Atlas : MonoBehaviour {

    public List<Sprite> avaterSpriteList;//存放Sprite
    public List<Sprite> commonSpriteList;
    public List<Sprite> avaterFullSpriteList;
    public Dictionary<string, Sprite> avaterSpriteDic;
    public Dictionary<string, Sprite> commonSpriteDic;
    public Dictionary<string, Sprite> avaterFullSpriteDic;
    public bool load;

    public void Init()
    {
        avaterSpriteDic = new Dictionary<string, Sprite>();
        for (int i=0;i< avaterSpriteList.Count;i++)
        {
            avaterSpriteDic.Add(avaterSpriteList[i].name, avaterSpriteList[i]);
        }
        commonSpriteDic = new Dictionary<string, Sprite>();
        for (int i = 0; i < commonSpriteList.Count; i++)
        {
            commonSpriteDic.Add(commonSpriteList[i].name, commonSpriteList[i]);
        }
        avaterFullSpriteDic = new Dictionary<string, Sprite>();
        for (int i = 0; i < avaterFullSpriteList.Count; i++)
        {
            avaterFullSpriteDic.Add(avaterFullSpriteList[i].name, avaterFullSpriteList[i]);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (load)
        {
            load = false;
            LoadAvater();
        }
#endif
    }


#if UNITY_EDITOR
    void LoadAvater()
    {
        string path = "/_BoomBeach/Images/UI/Avatar/";
        string path1 = "/_BoomBeach/Images/UI/Icon/";
        string path2 = "/_BoomBeach/Images/UI/AvatarFull/";
        Debug.Log(path);
        avaterSpriteList = LoadSpritesAtPath(path);
        commonSpriteList = LoadSpritesAtPath(path1);
        avaterFullSpriteList = LoadSpritesAtPath(path2);
        //System.IO.Directory dir = new Directory();
    }

    public static List<Sprite> LoadSpritesAtPath(string path)
    {
        path = Application.dataPath + path;
        string[] files = Directory.GetFiles(path);
        List<Sprite> prefabs = new List<Sprite>();
        foreach (string file in files)
        {
            string prefabPath = file.Replace(Application.dataPath, "");
            //			prefabPath = prefabPath.Replace(".prefab","");
            Sprite prefab = AssetDatabase.LoadAssetAtPath("Assets" + prefabPath, typeof(Sprite)) as Sprite;
            if(prefab!=null)
            prefabs.Add(prefab);
        }
        return prefabs;
    }
#endif
}
