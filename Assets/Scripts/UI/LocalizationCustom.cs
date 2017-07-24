using System;
using UnityEngine;
using System.Collections.Generic;


public class LocalizationCustom
{
    public TextAsset[] languages;
    Dictionary<string, string> mDictionary = new Dictionary<string, string>();
    string mLanguage;
    private readonly string[] pTextAssetNames = { "Chinese", "English" };
    static LocalizationCustom mInstance;
    static public LocalizationCustom instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new LocalizationCustom();
            }
            return mInstance;
        }
    }


    private bool pInitialize = false;
    public void Init()
    {


        if (pInitialize) { return; }

        languages = new TextAsset[pTextAssetNames.Length];

        TextAsset tx = null;

        for (int i = 0; i < pTextAssetNames.Length; i++)
        {
            tx = Resources.Load<TextAsset>(@"Localization/" + pTextAssetNames[i]);

            languages[i] = tx;
        }

        pInitialize = true;
    }
    
    public string currentLanguage
    {
        get
        {
            return mLanguage;
        }
        set
        {
            if (mLanguage != value)
            {

                if (!string.IsNullOrEmpty(value))
                {
                    // Check the referenced assets first
                    if (languages != null)
                    {
                        for (int i = 0, imax = languages.Length; i < imax; ++i)
                        {
                            TextAsset asset = languages[i];

                            if (asset != null && asset.name == value)
                            {
                                Load(asset);
                                return;
                            }
                        }
                    }
                    
                }

                // Either the language is null, or it wasn't found
                mDictionary.Clear();
                PlayerPrefs.DeleteKey("Language");
            }
        }
    }


    public void Load(TextAsset asset)
    {
        ByteReader reader = new ByteReader(asset);
        Set(asset.name, reader.ReadDictionary());
    }

    public void Set(string languageName, Dictionary<string, string> dictionary)
    {
#if SHOW_REPORT
		mUsed.Clear();
#endif
        mLanguage = languageName;
        PlayerPrefs.SetString("Language", mLanguage);
        mDictionary = dictionary;
    }

    

    public string Get(string key)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return key;
#endif
#if SHOW_REPORT
		if (!mUsed.Contains(key)) mUsed.Add(key);
#endif
        string val;
#if UNITY_IPHONE || UNITY_ANDROID
        if (mDictionary.TryGetValue(key + " Mobile", out val)) return val;
#endif

#if UNITY_EDITOR
        if (mDictionary.TryGetValue(key, out val)) return val;
        Debug.LogWarning("Localization key not found: '" + key + "'");
        return key;
#else
		return (mDictionary.TryGetValue(key, out val)) ? val : key;
#endif
    }

    // Random get a value based on part string
    public string RandomGet(string partStr = "TID_HINT_")
    {
        string val = string.Empty;
        List<string> keysList = new List<string>();

        foreach (string key in mDictionary.Keys)
        {
            if (key.Contains(partStr))
                keysList.Add(key);
        }

        if ((keysList.Count > 0) && (mDictionary.TryGetValue(keysList[UnityEngine.Random.Range(0, keysList.Count)], out val)))
        {
            return val;
        }
        return val;
    }

}

