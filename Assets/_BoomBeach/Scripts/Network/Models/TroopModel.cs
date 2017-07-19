using System;

namespace Network
{
	[System.Serializable]
	public class TroopModel:BaseEntity
	{
		public int id;
		public int user_id;
		public int type;
		public int level;
		public int num;
	}
}


