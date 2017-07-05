using System;
using UnityEngine;

namespace BoomBeach
{
	public static class Constant
	{



		public const bool isLocalBattleData = false;//使用本地战斗数据
		public const bool isLocalHomeData = false;//是否用本地玩家数据


		public const bool isEnabelAutoLogin = false;//是否可以自动登录
		public const float sendingPeriod = 30f;//心跳包时间间隔
		public const string domain = "121.199.3.185";//   linux服务器的内网IP : 192.168.180.177
		public const int port = 9933;
		public const string zone = "moba";

		public const int version = 1;//软件版本号;
		public const int sys_type = 10;//10:app,ios;11:91 ios,12:pp ios,13:test ios; 14:com.fanwe.coc; 15:com.fanwe.clans 20:android; 打包时,里面要设置好,与fanwe_user.msg_type是一至的，使用标识软件类型;
		public const bool is_wap_pay = false;

		public const float obstacleAlpha = 0.3f;
		public const float baseTimeSpan = 0.02f;
		public const int LandCraftWidth = 3; //登陆舰宽;

		public readonly static Vector3[] landcraftPos = new Vector3[8]{
			new Vector3(0,0,0),
			new Vector3(4,0,0),
			new Vector3(8,0,0),
			new Vector3(12,0,0),
			new Vector3(0,0,-12),
			new Vector3(4,0,-12),
			new Vector3(8,0,-12),
			new Vector3(12,0,-12)
		};

		public readonly static Vector3 dockPos = new Vector3(16f,-1f,-7f);
		public readonly static Vector3 gunboatPos = new Vector3(8f,-1f,-10f);
	}
}

