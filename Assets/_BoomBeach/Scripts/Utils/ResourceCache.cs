using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceCache : MonoBehaviour {

	private static Dictionary<string,Object> objectCache = new Dictionary<string, Object>();

	public static Object load(string path)
	{
		if(objectCache.ContainsKey(path))
		{
			return objectCache[path];
		}
		else
		{
			Object obj = Resources.Load(path);
			if(obj==null)
				return null;
			else
			{
				objectCache[path] = obj;
				return obj;
			}
		}
	}

	public void unset(string path)
	{
		if(objectCache.ContainsKey(path))
		{
			objectCache.Remove(path);
		}
	}

}
