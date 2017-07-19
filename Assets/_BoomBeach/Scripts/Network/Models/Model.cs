using System;
using System.Collections.Generic;

namespace Network
{
	[System.Serializable]
	public class Model
	{
		//public Notice update_done_list;//升级完成的通知（在home场景的时候）
		public UserInfo user_info;
		public UserInfo enemy_info;
		public List<BuildingInfo> building_list;
		public List<TroopInfo> troop_list;
		public List<RegionInfo> region_list;
		public List<TechnologyInfo> technology_list;
	}


}

