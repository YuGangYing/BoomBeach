using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;


/// <summary>
/// 模拟数据序列化使用
/// </summary>
public class BaseEntity
{
	public BaseEntity ()
	{
		
	}
	// Use this for initialization
	public BaseEntity (Hashtable hashtable)
	{
		HashtableToBean (hashtable);
	}

	public void HashtableToBean (Hashtable hashtable)
	{
		const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		FieldInfo[] fields = this.GetType ().GetFields (flags);
		foreach (FieldInfo fieldInfo in fields) {
			string fieldType = fieldInfo.FieldType.ToString ();			
			if (hashtable.ContainsKey (fieldInfo.Name)) {
				string tmp = (string)hashtable [fieldInfo.Name];
				if (fieldType.Equals ("System.Int16")
				     || fieldType.Equals ("System.Int32")
				     || fieldType.Equals ("System.Int64")
				     || fieldType.Equals ("System.UInt16")
				     || fieldType.Equals ("System.UInt32")
				     || fieldType.Equals ("System.UInt64")
				     || fieldType.Equals ("System.IntPtr")) {	
						
					if (!"".Equals (tmp)) {
						try {

							fieldInfo.SetValue (this, int.Parse (tmp));							
						} catch (Exception e) {
							Debug.Log ("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType + ";tmp:" + tmp);
							Debug.Log (e.Message);				
						}								
					} else {
						fieldInfo.SetValue (this, 0);
					}
				} else if (fieldType.Equals ("System.String")
				           || fieldType.Equals ("System.StringComparer")) {
						
					fieldInfo.SetValue (this, tmp);
				} else if (fieldType.Equals ("System.Single")) {
					fieldInfo.SetValue (this, float.Parse (tmp));
				}
			}		
		}		
	}

	public BaseEntity (ISFSObject item)
	{
		ISFSObjectToBean (item);
	}

	public void ISFSObjectToBean (ISFSObject item)
	{
		const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;	    
		FieldInfo[] fields = this.GetType ().GetFields (flags);
		foreach (FieldInfo fieldInfo in fields) {				
			string fieldType = fieldInfo.FieldType.ToString ();			
								
			if (item.ContainsKey (fieldInfo.Name)) {
				if (fieldType.Equals ("System.Int16")
				     || fieldType.Equals ("System.Int32")
				     || fieldType.Equals ("System.UInt16")
				     || fieldType.Equals ("System.UInt32")
				     || fieldType.Equals ("System.IntPtr")) {
					fieldInfo.SetValue (this, item.GetInt (fieldInfo.Name));
				} else if (fieldType.Equals ("System.String")
				           || fieldType.Equals ("System.StringComparer")) {						
					fieldInfo.SetValue (this, item.GetUtfString (fieldInfo.Name));					
				} else if (fieldType.Equals ("System.UInt64") || fieldType.Equals ("System.Int64")) {						
					fieldInfo.SetValue (this, item.GetLong (fieldInfo.Name));					
				} else if (fieldType.Equals ("System.Single")) {
					SFSDataType sdtype = (SFSDataType)item.GetData (fieldInfo.Name).Type;
					if (sdtype == SFSDataType.FLOAT) {
						fieldInfo.SetValue (this, item.GetFloat (fieldInfo.Name));
					} else if (sdtype == SFSDataType.DOUBLE) {
						Double d = item.GetDouble (fieldInfo.Name);						
						float f = float.Parse (d.ToString ());
						fieldInfo.SetValue (this, f);
					}
				}
			}	    	
		}	
	}

	public static T JsonToBean<T>(string json) where T : BaseEntity{
		return JsonUtility.FromJson<T> (json);
	}


	public virtual void InitData ()
	{
	}
}
