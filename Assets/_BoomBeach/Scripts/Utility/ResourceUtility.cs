using UnityEngine;
using System.Collections;

public static class ResourceUtility
{

    //tyep 0 is new , 1 is upgrade
    public static void GetBuildCostDiffToGems(string tid_level, int type, bool showAll, int gold_cost = 0)
    {

    }

    //升级成本;
    public static BuildCost getUpgradeCost(string tid_level)
    {
        BuildCost bc = new BuildCost();
        CsvInfo csvData = (CsvInfo)CSVManager.GetInstance.csvTable[tid_level];
        if ("BUILDING".Equals(csvData.TID_Type))
        {
            //需要取下一级里面的,时间级金额;
            string tid_next_level = csvData.TID + "_" + (csvData.Level + 1);
            if (CSVManager.GetInstance.csvTable.ContainsKey(tid_next_level))
            {
                CsvInfo csvNext = (CsvInfo)CSVManager.GetInstance.csvTable[tid_next_level];
                bc.stone = csvNext.BuildCostStone;
                bc.wood = csvNext.BuildCostWood;
                bc.iron = csvNext.BuildCostIron;
                bc.gold = 0;
                bc.piece = 0;
            }
        }
        else if ("CHARACTERS".Equals(csvData.TID_Type) || "SPELLS".Equals(csvData.TID_Type) || "TRAPS".Equals(csvData.TID_Type))
        {
            //当前等级中记录了,升级下一级里面的金额跟时间;
            bc.stone = 0;
            bc.wood = 0;
            bc.iron = 0;
            bc.gold = int.Parse(csvData.UpgradeCost);
            bc.piece = 0;
        }
        else if ("OBSTACLES".Equals(csvData.TID_Type))
        {
            bc.gold = int.Parse(csvData.ClearCost);
            bc.wood = 0;
            bc.stone = 0;
            bc.iron = 0;
            bc.piece = 0;
        }
        return bc;
    }

}