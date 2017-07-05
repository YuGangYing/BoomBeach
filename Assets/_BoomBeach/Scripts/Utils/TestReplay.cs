using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;   
  

public class TestReplay  
	
{          
	public static void Export ()   
		
	{                
		
		//实例化BattleData              
		
		BattleData.Init ();

		
		//随便设置一些数据给content    
		for(int ti=0;ti<250;ti++) //摸拟250个兵;
		{
			for(int i=0;i<50;i++)  //每个兵有100个命令;
			{

				ReplayNodeData rd = new ReplayNodeData();
				rd.TimeFromBegin = UnityEngine.Random.Range(100000,999999);
				rd.SelfType = EntityType.Trooper;
				rd.SelfID = UnityEngine.Random.Range(100,999);
				rd.StandX = UnityEngine.Random.Range(1f,50f);
				rd.StandZ = UnityEngine.Random.Range(1f,50f);
				rd.HitPoints = UnityEngine.Random.Range(1000,9999);
				rd.IsUnderAttack = UnityEngine.Random.Range(1,9);
				rd.DestX = UnityEngine.Random.Range(1f,50f);
				rd.DestZ = UnityEngine.Random.Range(1f,50f);
				rd.State = AISTATE.ATTACKING;

				BattleData.Instance.BattleCommondQueue.Enqueue(rd);
			}
		}
		
		for(int ti=0;ti<50;ti++) //摸拟50个建筑;
		{
			for(int i=0;i<50;i++)  //每个建筑有100个命令;
			{
				ReplayNodeData rd = new ReplayNodeData();
				rd.TimeFromBegin = UnityEngine.Random.Range(100000,999999);
				rd.SelfType = EntityType.Build;
				rd.SelfID = UnityEngine.Random.Range(100,999);
				rd.HitPoints = UnityEngine.Random.Range(1000,9999);
				rd.IsUnderAttack = UnityEngine.Random.Range(1,9);
				rd.DestX = UnityEngine.Random.Range(1f,50f);
				rd.DestZ = UnityEngine.Random.Range(1f,50f);
				rd.State = AISTATE.ATTACKING;
				
				BattleData.Instance.BattleCommondQueue.Enqueue(rd);
			}
		}

		//FileStream fs = new FileStream(@"D:\bd.dat", FileMode.Create); 
		//BinaryFormatter bf = new BinaryFormatter(); 
		//bf.Serialize(fs, ss); 



		//fs.Close();   
		/*
		StreamWriter sr = File.CreateText(@"D:\bd.dat");
		while(BattleData.Instance.BattleCommondQueue.Count>0)
		{
			string[] stringArray = BattleData.Instance.BattleCommondQueue.Dequeue ();
			string s = "";
			for(int i=0;i<stringArray.Length;i++)
			{
				s+=stringArray[i]+"|";
			}
			sr.WriteLine(s);
		}
		sr.Close();
		*/
	}   
	
	
	public static void Import()
	{

	//	StreamReader sr = new StreamReader (@"D:\bd.dat");
		//StreamReader sr = File.Open (@"D:\bd.dat", FileMode.Open);
//		string line = sr.ReadLine ();
//		string[] lineArray = line.Split ('|');
//		ReplayNodeData rd = ReplayNodeData.ToData (lineArray);

		//Debug.Log (rd.TimeFromBegin+" "+rd.DestType+" "+rd.DestID+" "+rd.State+"||");
	}
	
}   