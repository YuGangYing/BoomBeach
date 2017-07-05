using UnityEngine;
using System.Collections;
using System;

public class CalcHelper{
	
	/*黑药水与宝石之间的计算：多少黑药水需要多少宝石来换;*/
	public static int doCalcDarkElixirToGems(int resources){
		int[] ranges = {1,10,100,1000,10000,100000};
		int[] gems = {1,5,25,125,600,3000};
		int storagemax=200000;
		
		if(resources<0)
			return 0;		
		else if(resources==0)
			return 0;
		else if(resources<=ranges[0])
			return gems[0];

		//double ss = Math.Round(storagemax*10.0);
		
		for(int i=1; i<ranges.Length-1; i++){
			if(resources<=ranges[i]){				
				return (int) Math.Round((float)((resources-ranges[i-1])/((ranges[i]-ranges[i-1])/(gems[i]-gems[i-1]))+gems[i-1]));
			}
		}
		
		if(resources<=storagemax)
			return (int) Math.Round((float)(resources-ranges[ranges.Length-2])/((ranges[ranges.Length-1]-ranges[ranges.Length-2])/(gems[gems.Length-1]-gems[gems.Length-2]))+gems[gems.Length-2]);
		else 
			return  (int) (doCalcDarkElixirToGems(resources%storagemax)+Math.Floor((float)resources/storagemax)*doCalcDarkElixirToGems(storagemax));
		
	}
	
	
	
	/*金币,药水与宝石之间的计算关系: 多少金币或药水需要多少宝石来换;*/
	public static int doCalcResourceToGems(int resources){
		int[] ranges={100,1000,10000,100000,1000000,10000000};
		int[] gems={1,5,25,125,600,3000};
		int storagemax=8001000;

		if(resources<0)
			return -1;
		else if(resources==0)
			return 0 ;
		else if(resources<=ranges[0])
			return gems[0];
		
		for(int i=1;i<ranges.Length-1;i++){
			if(resources<=ranges[i])
				return (int) Math.Round((float)(resources-ranges[i-1])/((ranges[i]-ranges[i-1])/(gems[i]-gems[i-1]))+gems[i-1]);
		}
		
		if(resources<=storagemax)
			return (int) Math.Round((float)(resources-ranges[ranges.Length-2])/((ranges[ranges.Length-1]-ranges[ranges.Length-2])/(gems[gems.Length-1]-gems[gems.Length-2]))+gems[gems.Length-2]);
		else 
			return (int) (doCalcResourceToGems(resources%storagemax)+Math.Floor((float)resources/storagemax)*doCalcResourceToGems(storagemax));
				
	}	

	/*宝石与钱的计算：多少宝石需要多少钱购买;*/
	public static float doCalcGemToCash(int gemsinput)
	{
		float[] cost= {4.99f,9.99f,19.99f,49.99f,99.99f};
		int[] gems={500,1200,2500,6500,14000};

		if(gemsinput<=0)
			return 0;
		
		for(int i=gems.Length-1;i>0;i--)
			if(gemsinput>gems[i-1]*2)
			
		return (cost[i]+doCalcGemToCash(gemsinput-gems[i]));
		
		return (cost[0]+doCalcGemToCash(gemsinput-gems[0]));
	}

	
	/*时间与宝石之间的转换  返回时间秒;*/
	public static int calcGemsToTime(int gems)
	{
		
		int[] timerange={60,3600,86400,604800};
		int[] gemsrange={1,20,260,1000};
		
		int seconds=0;

		if(gems<0)
		{			
			return 0;
		}
		else if(gems==0)
		{
			seconds=0;
		}
		else if(gems<=gemsrange[0])
		{
			seconds=246;
		}
		else
		{
			gems=gems+1;
			if(gems<=gemsrange[1])
			{
				
				seconds = (int) (Math.Ceiling((float)(gems-gemsrange[0])*((timerange[1]-timerange[0])/(gemsrange[1]-gemsrange[0]))+timerange[0])-1);
			}
			else if(gems<=gemsrange[2])
			{
				seconds = (int) (Math.Ceiling((float)(gems-gemsrange[1])*((timerange[2]-timerange[1])/(gemsrange[2]-gemsrange[1]))+timerange[1])-1);
			}
			else
			{
				seconds = (int) (Math.Ceiling((float)(gems-gemsrange[2])*((timerange[3]-timerange[2])/(gemsrange[3]-gemsrange[2]))+timerange[2])-1);
			}
		}
		
		return seconds;		 
	}
	
	/*时间与宝石之间的转换;*/
	public static int calcTimeToGems(int seconds)
	{
		//seconds =10*3600+ 17*60;
		int[] ranges={60,3600,86400,604800};
		int[] gems={1,20,200,600};
		int result=0;
		
		if(seconds<0)
		{
			result = 0;
		}
		else if(seconds==0)
		{
			result=0;
			
		}else if(seconds<=ranges[0])
		{
			result=1;
		}
		else if(seconds<=ranges[1])
		{
			result=(int) Math.Floor((float)(seconds-ranges[0])/((ranges[1]-ranges[0])/(gems[1]-gems[0]))+gems[0]);
		}
		else if(seconds<=ranges[2])
		{
			result=(int) Math.Floor((float)(seconds-ranges[1])/((ranges[2]-ranges[1])/(gems[2]-gems[1]))+gems[1]);
		}
		else
		{
			result=(int) Math.Floor((float)(seconds-ranges[2])/((ranges[3]-ranges[2])/(gems[3]-gems[2]))+gems[2]);
		}
				
		return result;
	}	
	/*
	//获取某等级需要多少经验值;
	public static int calcExperience(int level){
		if (level <= 1)
			return 30;
		else		
			return (level * 50 - 50);
	}
	
	//获取某等级总共需要多少经验值(即：所有等级相加和);
	public static int calcExperience2(int level){
		if (level <= 1)
			return 30;
		else		
			return (level * (level - 1) / 2 * 50);
	}	
*/
	//将时间转化成奖杯值;
	public static int calcTimeToReward(int seconds){
		//Debug.Log("seconds:" + seconds);
		return (int) Math.Ceiling(seconds/100f);
	}	
	
	/*金币,药水与宝石之间的计算关系: 多少金币或药水需要多少宝石来换;*/
	public static int doCalcGemsToMaxResource(int diamonds){
		float[] ranges={1f,5f,25f,125f,600f,3000f};
		float[] resources={100f,1000f,10000f,100000f,1000000f,10000000f};
		int storagemax=3000;

		if(diamonds<0)
			return -1;
		else if(diamonds==0)
			return 0 ;
		else if(diamonds<=ranges[0])
			return (int)resources[0];
		
		for(int i=1;i<ranges.Length-1;i++){
			if(diamonds<=ranges[i]){
				/*
				Debug.Log("diamonds:" + diamonds);//72
				Debug.Log("ranges[" + i + "]:" + ranges[i]);//125
				Debug.Log(diamonds-ranges[i-1]);//72-25=42
				Debug.Log(ranges[i]-ranges[i-1]);//125 - 25 = 100
				Debug.Log(resources[i]-resources[i-1]);//100000 - 10000 = 90000
				Debug.Log("resources[" + (i - 1) + "]:" + resources[i-1]);//10000
				//42/(100/9000)
				*/
				int res = (int) Math.Round((float)(diamonds-ranges[i-1])/((ranges[i]-ranges[i-1])/(resources[i]-resources[i-1]))+resources[i-1]);
				//Debug.Log("res:" + res);
				return res;
			}		
		}
		
		if(diamonds<=storagemax)
			return (int) Math.Round((float)(diamonds-ranges[ranges.Length-2])/((ranges[ranges.Length-1]-ranges[ranges.Length-2])/(resources[resources.Length-1]-resources[resources.Length-2]))+resources[resources.Length-2]);
		else 
			return (int) (doCalcGemsToMaxResource(diamonds%storagemax)+Math.Floor((float)diamonds/storagemax)*doCalcGemsToMaxResource(storagemax));
		
		
	}	
}
