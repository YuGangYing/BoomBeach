using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public static class CollectHandle {

	//收集
	public static bool Collect(BuildInfo s){
		string res_type = "";
		PartType partType = PartType.Gold;
		if (s.tid == "TID_BUILDING_HOUSING"){
			partType = PartType.Gold;
			res_type = Helper.RES_TYPE_GOLD;
		}else if (s.tid == "TID_BUILDING_WOODCUTTER"){
			partType = PartType.Wood;
			res_type = Helper.RES_TYPE_WOOD;
		}else if (s.tid == "TID_BUILDING_STONE_QUARRY"){
			partType = PartType.Stone;
			res_type = Helper.RES_TYPE_STONE;
		}else if (s.tid == "TID_BUILDING_METAL_MINE"){
			partType = PartType.Iron;
			res_type = Helper.RES_TYPE_IRON;
		}
		//0:可以采集;1:可以采集，并可以显示图标; 2:采集器已满;3:储存器容量已满,无法再放下;
		int cStatus = Helper.CollectStatus(s);
		if (res_type != ""&&(cStatus==0||cStatus==1||cStatus==2)){
			int collect_time = Helper.current_time();
			int collect_num = Helper.getCollectNum(s,collect_time,false);
			if (collect_num > 0){
				/*
				int max_num = 0;
				if(res_type=="Gold")
				{
					max_num = Globals.userInfo.max_gold_count;
				}
				else if(res_type=="Wood")
				{
					max_num = Globals.userInfo.max_wood_count;
				}
				else if(res_type=="Stone")
				{
					max_num = Globals.userInfo.max_stone_count;
				}
				else if(res_type=="Iron")
				{
					max_num = Globals.userInfo.max_iron_count;
				}
				//collect_num = max_num;
				int c = 0;
				if(max_num>0)
				{
					c =  Mathf.CeilToInt(collect_num*100f/max_num);
				}
				if(c>15)c=15;
				int addC = Mathf.RoundToInt(collect_num/500f);
				if(addC>15)addC = 15;
				c+=addC;
*/
				int c = Helper.GetEmitCount (res_type,collect_num);
				s.emitter.enabled = true;
				s.emitter.Emit(c,s.transform.position,partType,collect_num);

				//Helper.setResourceCount(res_type,collect_num,false,true);
				s.last_collect_time = collect_time;

				SendCollectToServer (res_type,collect_num,collect_time,s);

				if(s.buildUIManage!=null)
					s.buildUIManage.SetCollectShader(true);
				if (s.buildUI != null)
					s.buildUI.SetCollectShader();
				//重新设置，所有建筑物的升级提醒标识;
				Helper.SetBuildUpgradeIcon();
				return true;
			}
		}else if (cStatus==3){
			//TID_RESOURCE_PACK_LOCKED = 没有足够的储存空间;
			PopManage.Instance.ShowMsg(StringFormat.FormatByTid("TID_RESOURCE_PACK_LOCKED"));
		}
		return false;
	}

	static void SendCollectToServer(string res_type,int collect_num,int collect_time,BuildInfo s){
		ISFSObject data = new SFSObject();
		data.PutUtfString("res_type",res_type);
		data.PutInt("collect_num",collect_num);			
		data.PutInt("collect_time",collect_time);
		data.PutLong("building_id", s.building_id);
		SFSNetworkManager.Instance.SendRequest(data,ApiConstant.CMD_Collect, false,BuildHandle.HandleResponse);
	}

}
