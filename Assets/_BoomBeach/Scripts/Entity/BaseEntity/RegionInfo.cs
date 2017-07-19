using System;

[System.Serializable]
public class RegionInfo:BaseEntity
{
	public int id;
	public int user_id;
	public int region_id;
	public string region_name;
	public int capture_id;
	public int capture_region_id;
	public string capture_name;
	public int capture_time;
}