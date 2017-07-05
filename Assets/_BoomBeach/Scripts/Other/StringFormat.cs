using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;

public class StringFormat : MonoBehaviour {

	public static string Format(string formatString,System.Object[] args = null)
	{
		if (args == null){
			return formatString;
		}else{
			MatchCollection matches = Regex.Matches (formatString,@"<[^>]+>");
			if(matches.Count>0)
			{
				for(int i=0;i<matches.Count;i++)
				{
					if(i<args.Length)
						formatString = formatString.Replace(matches[i].Value,args[i].ToString());
				}
			}
			return formatString;
		}
	}

	public static string FormatByTid(string tid,System.Object[] args = null)
	{
		string formatString = LocalizationCustom.instance.Get(tid);
		return Format(formatString,args);
	}
	
}
