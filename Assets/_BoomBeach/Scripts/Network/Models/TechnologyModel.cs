using System;

namespace Network
{
	[System.Serializable]
	public class TechnologyModel:BaseEntity
	{
		public int id;
		public int type;
		public int user_id;
		public int level;
	}
}