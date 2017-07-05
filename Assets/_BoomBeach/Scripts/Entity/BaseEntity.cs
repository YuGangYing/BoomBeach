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
	public BaseEntity() {
		
	}
	// Use this for initialization
	public BaseEntity(Hashtable hashtable) {
		HashtableToBean(hashtable);
	}
	
	public void HashtableToBean(Hashtable hashtable){
		const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
	    
		FieldInfo[] fields = this.GetType().GetFields(flags);
		//Debug.Log1("fields.Length:" + fields.Length);
		
	    foreach (FieldInfo fieldInfo in fields)
	    {
				
				string fieldType  = fieldInfo.FieldType.ToString();			
				
				
				if (hashtable.ContainsKey(fieldInfo.Name)){
					string tmp = (string)hashtable[fieldInfo.Name];
					
					//Debug.Log1("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType + ";tmp:" + tmp);
					
					//fieldInfo.SetValue(this,tmp);
					if (fieldType.Equals("System.Int16")					
						|| fieldType.Equals("System.Int32") 
						|| fieldType.Equals("System.Int64")
						|| fieldType.Equals("System.UInt16")
						|| fieldType.Equals("System.UInt32")
						|| fieldType.Equals("System.UInt64")
						|| fieldType.Equals("System.IntPtr")){	
						
						if (!"".Equals(tmp)){
						try{

							fieldInfo.SetValue(this,int.Parse(tmp));							
						}catch (Exception e) {
							Debug.Log("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType + ";tmp:" + tmp);
                            Debug.Log(e.Message);				
						}								
							//Debug.Log1("tmp: " + tmp);
							//fieldInfo.SetValue(this,100);
						}else{
							fieldInfo.SetValue(this,0);
						}
					}else if (fieldType.Equals("System.String")					
						|| fieldType.Equals("System.StringComparer")
						){
						
						fieldInfo.SetValue(this,tmp);
                        }
                    else if (fieldType.Equals("System.Single"))
                    {
                        fieldInfo.SetValue(this, float.Parse(tmp));
                        // Debug.Log(fieldInfo.GetValue(this) + " " + item.GetFloat(fieldInfo.Name));
                    }
				}								
	    		//Debug.Log1("Value: " + fieldInfo.GetValue(this) + ", Field: " + fieldInfo.Name + ", FieldType: " + fieldInfo.FieldType);
			
	    }		
	}
	
	public BaseEntity(ISFSObject item) {
			ISFSObjectToBean(item);
	}
	
	public void ISFSObjectToBean(ISFSObject item){
		const BindingFlags flags = /*BindingFlags.NonPublic | */BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;	    
		FieldInfo[] fields = this.GetType().GetFields(flags);
        //Debug.Log(item.GetDump());			
	    foreach (FieldInfo fieldInfo in fields)
	    {				
				string fieldType  = fieldInfo.FieldType.ToString();			
								
				if (item.ContainsKey(fieldInfo.Name)){
                    /*if (fieldInfo.Name == "ap_potential")
                    {
                        Debug.Log(item.GetDump());
				        Debug.Log("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType);	
					}*/
					//string tmp = (string)hashtable[fieldInfo.Name];					
					//Debug.Log("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + fieldType);					
					//fieldInfo.SetValue(this,tmp);
					if (fieldType.Equals("System.Int16")					
						|| fieldType.Equals("System.Int32") 						
						|| fieldType.Equals("System.UInt16")
						|| fieldType.Equals("System.UInt32")						
						|| fieldType.Equals("System.IntPtr")){

                    //try
                    //{
                    //    Debug.LogError("FieldInfo name : " + fieldInfo.Name + "  info value : " + item.GetInt(fieldInfo.Name));
                    //}
                    //catch (Exception ex)
                    //{
                    //    Debug.LogError("FieldInfo Name : "+fieldInfo.Name+" Error message : "+ex.Message);
                    //}

                    
                        fieldInfo.SetValue(this, item.GetInt(fieldInfo.Name));
 
						
					}else if (fieldType.Equals("System.String")					
						|| fieldType.Equals("System.StringComparer")
						){						
						fieldInfo.SetValue(this,item.GetUtfString(fieldInfo.Name));					
					}else if (fieldType.Equals("System.UInt64") || fieldType.Equals("System.Int64")){						
						fieldInfo.SetValue(this,item.GetLong(fieldInfo.Name));					
					}	
					else if(fieldType.Equals("System.Single"))
                    {
					
					
						SFSDataType sdtype = (SFSDataType)item.GetData(fieldInfo.Name).Type;
						if (sdtype == SFSDataType.FLOAT){
							fieldInfo.SetValue(this, item.GetFloat(fieldInfo.Name));
						}else if (sdtype == SFSDataType.DOUBLE){
							//Debug.Log(item.GetDump());
							Double d = item.GetDouble(fieldInfo.Name);						
							//Debug.Log("d:" + d);
							float f = float.Parse(d.ToString());
							//Debug.Log("f:" + f);
							fieldInfo.SetValue(this, f);
							//Debug.Log("----------");
						}
					
						//Sfs2X.Entities.Data.SFSDataType.FLOAT;
						//Sfs2X.Entities.Data.SFSDataType.DOUBLE;
						
                        //Debug.Log(item.GetDump());
					
					
						//item.GetData("").ge
                        //Debug.Log("fieldInfo.Name:" + fieldInfo.Name + ";fieldType:" + sdtype.ToString());

                        //Debug.Log(item.GetData(fieldInfo.Name).Type.ToString());
                        //Debug.Log(item.GetData(fieldInfo.Name).GetType().ToString());

                       // Debug.Log(fieldInfo.GetValue(this) + " " + item.GetDouble(fieldInfo.Name));


                        //fieldInfo.SetValue(this, item.GetDouble(fieldInfo.Name));
                       // Debug.Log(fieldInfo.GetValue(this) + " " + item.GetFloat(fieldInfo.Name));
                    }
				}	    	
	    }	
	}

	public virtual void InitData(){
	}
}
