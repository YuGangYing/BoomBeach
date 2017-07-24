//==========================================
// Created By [yingyugang] At 2016/1/20 10:57:20
//==========================================
using BattleFramework.Data;
using System;
using System.Collections.Generic;

namespace BoomBeach
{
    ///<summary>
    ///
    ///</summary>
    public class BuildingModel 
    {
        ///////////////////////////////////以下为静态成员//////////////////////////////

        ///////////////////////////////////以下为非静态成员///////////////////////////

        public List<Buildings> buildingDatas;//所有建筑
        public Dictionary<string, Buildings> basicBuildingDatas;//所有1级建筑
        public List<Townhall_levels> townhallDatas;//基地可建造列表

        public const string HOUSING = "Housing";
        public const string WOOD_STORAGE = "Wood Storage";
        public const string Metal_Storage = "Metal Storage";
        public const string Stone_Storage = "Stone Storage";
        public const string Stone_Quarry = "Stone Quarry";
        public const string Metal_Mine = "Metal Mine";
        public const string Big_Bertha = "Big Bertha";
        public const string Guard_Tower = "Guard Towere";
        public const string Mortar = "Mortar";
        public const string Machine_Gun_Nest = "Machine Gun Nest";
        public const string Cannon = "Cannon";
        public const string Flame_Thrower = "Flame Thrower";
        public const string Missile_Launcher = "Missile Launcher";
        public const string Tank_Mine = "Tank Mine";
        public const string Mine = "Minee";
        public const string Artifact_Workshop = "Artifact Workshop";
        public const string Map_Room = "Map Room";
        public const string Vault = "Vault";
        public const string Gold_Storage = "Gold Storage";
        public const string Laboratory = "Laboratory";
        public const string Boat = "Boat";

        public BuildingModel()
        {
            buildingDatas = Buildings.LoadDatas();
            townhallDatas = Townhall_levels.LoadDatas();
            InitBasicBuildingData();
        }

        void InitBasicBuildingData()
        {
            basicBuildingDatas = new Dictionary<string, Buildings>();
            for (int i=0;i< buildingDatas.Count;i++)
            {
                if (buildingDatas[i].Name!=null && buildingDatas[i].Name.Trim() != "")
                {
                    basicBuildingDatas.Add(buildingDatas[i].Name.Trim(), buildingDatas[i]);
                }
            }
        }

        //获取基地升级解锁列表
        public Townhall_levels GetLevelUpBuildings(int level)
        {
            Townhall_levels tl = new Townhall_levels();
            return tl;
        }










    }
}
