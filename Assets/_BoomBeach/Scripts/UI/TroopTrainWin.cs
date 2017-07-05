using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BoomBeach{
	public class TroopTrainWin : MonoBehaviour {

		public void BindTrainWin(BuildInfo buildInfo)
		{
			Dictionary<string,TrainTid> trainList = Helper.getTrainTidList (buildInfo);
			/*
			Dictionary<string,TrainTid> trainList = new Dictionary<string, TrainTid>();
			for(int k=0;k<5;k++)
			{
				TrainTid ttt = new TrainTid();
				ttt.hasTrain = null;
				ttt.trainNum = 3;
				if(k==1)ttt.hasTrain = "不能造兵不能造兵不能造兵";
				if(k==2)ttt.trainCost = 3333;
				else
				ttt.trainCost = 10000;
				ttt.trainTimeFormat = "3m";

				ttt.tid_level = "TID_ZOOKA";
				trainList.Add(k.ToString(),ttt);
			}
			*/

			Transform container = transform.Find ("GridList");


			
			int i = 0;
			foreach(TrainTid trainItem in trainList.Values)
			{

				Transform c = container.GetChild(i);
				c.gameObject.SetActive(true);
				c.GetComponent<TrainItem>().BindData(trainItem);
				i++;
			}

			for(int j=i;j<container.childCount;j++)
			{
				container.GetChild(j).gameObject.SetActive(false);
			}
		}
	}
}