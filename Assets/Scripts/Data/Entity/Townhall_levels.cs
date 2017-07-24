using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace BattleFramework.Data{
    [System.Serializable]
    public class Townhall_levels {
        public static string csvFilePath = "CSV/townhall_levels";
        public static string[] columnNameArray = new string[24];
        public static List<Townhall_levels> LoadDatas(){
            CSVFileReader csvFile = new CSVFileReader();
            csvFile.Open(Resources.Load<TextAsset>(csvFilePath));
            List<Townhall_levels> dataList = new List<Townhall_levels>();
            //string[] strs;
            //string[] strsTwo;
            //List<int> listChild;
            columnNameArray = new string[24];
            for(int i = 0;i < csvFile.mapData.Count;i ++){
             if (csvFile.mapData[i].data.Count < columnNameArray.Length){
              Debug.LogError("csvFile.mapData[i].data.Count :" + csvFile.mapData[i].data.Count + " columnNameArray.Length:" + columnNameArray.Length);
               continue;
               }
                Townhall_levels data = new Townhall_levels();
                data.Name = csvFile.mapData[i].data[0];
                columnNameArray [0] = "Name";
                int.TryParse(csvFile.mapData[i].data[1],out data.WoodStorage);
                columnNameArray [1] = "WoodStorage";
                int.TryParse(csvFile.mapData[i].data[2],out data.StoneStorage);
                columnNameArray [2] = "StoneStorage";
                int.TryParse(csvFile.mapData[i].data[3],out data.MetalStorage);
                columnNameArray [3] = "MetalStorage";
                int.TryParse(csvFile.mapData[i].data[4],out data.Housing);
                columnNameArray [4] = "Housing";
                int.TryParse(csvFile.mapData[i].data[5],out data.StoneQuarry);
                columnNameArray [5] = "StoneQuarry";
                int.TryParse(csvFile.mapData[i].data[6],out data.MetalMine);
                columnNameArray [6] = "MetalMine";
                int.TryParse(csvFile.mapData[i].data[7],out data.BigBertha);
                columnNameArray [7] = "BigBertha";
                int.TryParse(csvFile.mapData[i].data[8],out data.GuardTower);
                columnNameArray [8] = "GuardTower";
                int.TryParse(csvFile.mapData[i].data[9],out data.Mortar);
                columnNameArray [9] = "Mortar";
                int.TryParse(csvFile.mapData[i].data[10],out data.MachineGunNest);
                columnNameArray [10] = "MachineGunNest";
                int.TryParse(csvFile.mapData[i].data[11],out data.Cannon);
                columnNameArray [11] = "Cannon";
                int.TryParse(csvFile.mapData[i].data[12],out data.FlameThrower);
                columnNameArray [12] = "FlameThrower";
                int.TryParse(csvFile.mapData[i].data[13],out data.MissileLauncher);
                columnNameArray [13] = "MissileLauncher";
                int.TryParse(csvFile.mapData[i].data[14],out data.TankMine);
                columnNameArray [14] = "TankMine";
                int.TryParse(csvFile.mapData[i].data[15],out data.Mine);
                columnNameArray [15] = "Mine";
                int.TryParse(csvFile.mapData[i].data[16],out data.ArtifactWorkshop);
                columnNameArray [16] = "ArtifactWorkshop";
                int.TryParse(csvFile.mapData[i].data[17],out data.MapRoom);
                columnNameArray [17] = "MapRoom";
                int.TryParse(csvFile.mapData[i].data[18],out data.Vault);
                columnNameArray [18] = "Vault";
                int.TryParse(csvFile.mapData[i].data[19],out data.GoldStorage);
                columnNameArray [19] = "GoldStorage";
                int.TryParse(csvFile.mapData[i].data[20],out data.Laboratory);
                columnNameArray [20] = "Laboratory";
                int.TryParse(csvFile.mapData[i].data[21],out data.Boat);
                columnNameArray [21] = "Boat";
                data.RequiredBuilding = csvFile.mapData[i].data[22];
                columnNameArray [22] = "RequiredBuilding";
                int.TryParse(csvFile.mapData[i].data[23],out data.RequiredBuildingLevel);
                columnNameArray [23] = "RequiredBuildingLevel";
                dataList.Add(data);
            }
            return dataList;
        }
        public string Name;//
        public int WoodStorage;//
        public int StoneStorage;//
        public int MetalStorage;//
        public int Housing;//
        public int StoneQuarry;//
        public int MetalMine;//
        public int BigBertha;//
        public int GuardTower;//
        public int Mortar;//
        public int MachineGunNest;//
        public int Cannon;//
        public int FlameThrower;//
        public int MissileLauncher;//
        public int TankMine;//
        public int Mine;//
        public int ArtifactWorkshop;//
        public int MapRoom;//
        public int Vault;//
        public int GoldStorage;//
        public int Laboratory;//
        public int Boat;//
        public string RequiredBuilding;//
        public int RequiredBuildingLevel;//
    }
}
