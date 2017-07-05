using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathFinding.Core;


public class WalkList{

	public WalkList()
	{
		path = new List<PathFinderNode> ();
	}

	public List<PathFinderNode> path;

	public void add(PathFinderNode node)
	{
		path.Add (node);
	}

	public override string ToString()
	{
		string str = "";
		for(int i=0;i<path.Count;i++)
		{
			PathFinderNode node = path[i];
			string s = node.X+"_"+node.Y;
			str+=s;
			if(i<path.Count-1)
			{
				str+=",";
			}
		}
		return str;
	}

	public static WalkList ToData(string str)
	{
		string[] stringArray = str.Split (',');
		//if (stringArray.Length > 1) Debug.Log(str);
		WalkList list = new WalkList ();
		for(int i=0;i<stringArray.Length;i++)
		{
			string s = stringArray[i];
			if (s.Length > 0){
				string[] xy = s.Split('_');
				PathFinderNode node = new PathFinderNode();
				node.X = int.Parse(xy[0]);
				node.Y = int.Parse(xy[1]);
				list.path.Add(node);
			}
		}
		return list;
	}
}
