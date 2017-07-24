//==========================================
// Created By [yingyugang] At 2016/1/20 11:32:56
//==========================================
using System;

namespace Models
{
    public class BuildingData
    {
        public uint building_id = 0;
        public string tid = "";
        public string tid_type = "";
        public string tid_level = "";
        public string status_tid_level = "";
        public uint level = 5; 
        public uint regions_id = 6;
	    public uint artifact_boost = 7;
	    public uint artifact_type = 8;
	    public uint w_h = 9;
	    public uint cur_hit_points = 10; 
	    public uint cur_hit_points2 = 11; 
	    public uint hit_points = 12; 
	    public uint start_time = 13; 
    	public uint end_time = 14; 
    	public uint last_collect_time = 15; 
    	public uint push_id = 16; 
    	//public building.BuildingData.BuildStatus status; 
    	public bool is_destroy = false;// 是否被销毁:0否;1是(生命值为0时,被销毁)
        public uint troops_start_time = 19; 
    	public uint troops_num = 20;

        public uint x = 0;
        public uint y = 0;
        
    }
}
