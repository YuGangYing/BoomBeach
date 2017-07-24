using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace BattleFramework.Data{
    [System.Serializable]
    public class Buildings {
        public static string csvFilePath = "CSV/buildings";
        public static string[] columnNameArray = new string[97];
        public static List<Buildings> LoadDatas(){
            CSVFileReader csvFile = new CSVFileReader();
            csvFile.Open(Resources.Load<TextAsset>(csvFilePath));
            List<Buildings> dataList = new List<Buildings>();
            //string[] strs;
            //string[] strsTwo;
            //List<int> listChild;
            columnNameArray = new string[97];
            for(int i = 0;i < csvFile.mapData.Count;i ++){
             if (csvFile.mapData[i].data.Count < columnNameArray.Length){
              Debug.LogError("csvFile.mapData[i].data.Count :" + csvFile.mapData[i].data.Count + " columnNameArray.Length:" + columnNameArray.Length);
               continue;
               }
                Buildings data = new Buildings();
                data.Name = csvFile.mapData[i].data[0];
                columnNameArray [0] = "Name";
                int.TryParse(csvFile.mapData[i].data[1],out data.DefenseValue);
                columnNameArray [1] = "DefenseValue";
                data.BuildingClass = csvFile.mapData[i].data[2];
                columnNameArray [2] = "BuildingClass";
                int.TryParse(csvFile.mapData[i].data[3],out data.BuildTimeD);
                columnNameArray [3] = "BuildTimeD";
                int.TryParse(csvFile.mapData[i].data[4],out data.BuildTimeH);
                columnNameArray [4] = "BuildTimeH";
                int.TryParse(csvFile.mapData[i].data[5],out data.BuildTimeM);
                columnNameArray [5] = "BuildTimeM";
                int.TryParse(csvFile.mapData[i].data[6],out data.BuildTimeS);
                columnNameArray [6] = "BuildTimeS";
                data.BuildCost = csvFile.mapData[i].data[7];
                columnNameArray [7] = "BuildCost";
                int.TryParse(csvFile.mapData[i].data[8],out data.TownHallLevel);
                columnNameArray [8] = "TownHallLevel";
                data.MaxStoredResource = csvFile.mapData[i].data[9];
                columnNameArray [9] = "MaxStoredResource";
                bool.TryParse(csvFile.mapData[i].data[10],out data.Bunker);
                columnNameArray [10] = "Bunker";
                int.TryParse(csvFile.mapData[i].data[11],out data.HousingSpace);
                columnNameArray [11] = "HousingSpace";
                data.ProducesResource = csvFile.mapData[i].data[12];
                columnNameArray [12] = "ProducesResource";
                int.TryParse(csvFile.mapData[i].data[13],out data.ResourcePerHour);
                columnNameArray [13] = "ResourcePerHour";
                int.TryParse(csvFile.mapData[i].data[14],out data.ResourceMax);
                columnNameArray [14] = "ResourceMax";
                int.TryParse(csvFile.mapData[i].data[15],out data.UnitProduction);
                columnNameArray [15] = "UnitProduction";
                bool.TryParse(csvFile.mapData[i].data[16],out data.UpgradesUnits);
                columnNameArray [16] = "UpgradesUnits";
                int.TryParse(csvFile.mapData[i].data[17],out data.Hitpoints);
                columnNameArray [17] = "Hitpoints";
                int.TryParse(csvFile.mapData[i].data[18],out data.RegenTime);
                columnNameArray [18] = "RegenTime";
                int.TryParse(csvFile.mapData[i].data[19],out data.AttackRange);
                columnNameArray [19] = "AttackRange";
                int.TryParse(csvFile.mapData[i].data[20],out data.AttackSpeed);
                columnNameArray [20] = "AttackSpeed";
                int.TryParse(csvFile.mapData[i].data[21],out data.Damage);
                columnNameArray [21] = "Damage";
                bool.TryParse(csvFile.mapData[i].data[22],out data.AirTargets);
                columnNameArray [22] = "AirTargets";
                bool.TryParse(csvFile.mapData[i].data[23],out data.GroundTargets);
                columnNameArray [23] = "GroundTargets";
                int.TryParse(csvFile.mapData[i].data[24],out data.MinAttackRange);
                columnNameArray [24] = "MinAttackRange";
                int.TryParse(csvFile.mapData[i].data[25],out data.DamageRadius);
                columnNameArray [25] = "DamageRadius";
                int.TryParse(csvFile.mapData[i].data[26],out data.DamageSpread);
                columnNameArray [26] = "DamageSpread";
                bool.TryParse(csvFile.mapData[i].data[27],out data.PushBack);
                columnNameArray [27] = "PushBack";
                bool.TryParse(csvFile.mapData[i].data[28],out data.CanNotSellLast);
                columnNameArray [28] = "CanNotSellLast";
                int.TryParse(csvFile.mapData[i].data[29],out data.DestructionXP);
                columnNameArray [29] = "DestructionXP";
                bool.TryParse(csvFile.mapData[i].data[30],out data.Locked);
                columnNameArray [30] = "Locked";
                bool.TryParse(csvFile.mapData[i].data[31],out data.Hidden);
                columnNameArray [31] = "Hidden";
                int.TryParse(csvFile.mapData[i].data[32],out data.TriggerRadius);
                columnNameArray [32] = "TriggerRadius";
                bool.TryParse(csvFile.mapData[i].data[33],out data.ForgesSpells);
                columnNameArray [33] = "ForgesSpells";
                bool.TryParse(csvFile.mapData[i].data[34],out data.CreatesArtifacts);
                columnNameArray [34] = "CreatesArtifacts";
                int.TryParse(csvFile.mapData[i].data[35],out data.ArtifactType);
                columnNameArray [35] = "ArtifactType";
                int.TryParse(csvFile.mapData[i].data[36],out data.ArtifactCapacity);
                columnNameArray [36] = "ArtifactCapacity";
                int.TryParse(csvFile.mapData[i].data[37],out data.DeepseaResourceReward);
                columnNameArray [37] = "DeepseaResourceReward";
                int.TryParse(csvFile.mapData[i].data[38],out data.DeepseaCommonArtifactChance);
                columnNameArray [38] = "DeepseaCommonArtifactChance";
                int.TryParse(csvFile.mapData[i].data[39],out data.DeepseaRareArtifactChance);
                columnNameArray [39] = "DeepseaRareArtifactChance";
                int.TryParse(csvFile.mapData[i].data[40],out data.DeepseaEpicArtifactChance);
                columnNameArray [40] = "DeepseaEpicArtifactChance";
                int.TryParse(csvFile.mapData[i].data[41],out data.DeepseaRandomFactor);
                columnNameArray [41] = "DeepseaRandomFactor";
                int.TryParse(csvFile.mapData[i].data[42],out data.DeepseaPrice);
                columnNameArray [42] = "DeepseaPrice";
                bool.TryParse(csvFile.mapData[i].data[43],out data.IsMapRoom);
                columnNameArray [43] = "IsMapRoom";
                int.TryParse(csvFile.mapData[i].data[44],out data.StartingEnergy);
                columnNameArray [44] = "StartingEnergy";
                int.TryParse(csvFile.mapData[i].data[45],out data.MaxEnergy);
                columnNameArray [45] = "MaxEnergy";
                int.TryParse(csvFile.mapData[i].data[46],out data.EnergyGain);
                columnNameArray [46] = "EnergyGain";
                bool.TryParse(csvFile.mapData[i].data[47],out data.IsVault);
                columnNameArray [47] = "IsVault";
                bool.TryParse(csvFile.mapData[i].data[48],out data.IsHeroHouse);
                columnNameArray [48] = "IsHeroHouse";
                int.TryParse(csvFile.mapData[i].data[49],out data.LandingBoats);
                columnNameArray [49] = "LandingBoats";
                bool.TryParse(csvFile.mapData[i].data[50],out data.CanNotMove);
                columnNameArray [50] = "CanNotMove";
                bool.TryParse(csvFile.mapData[i].data[51],out data.IsGlobalBuilding);
                columnNameArray [51] = "IsGlobalBuilding";
                int.TryParse(csvFile.mapData[i].data[52],out data.ExplorableRegions);
                columnNameArray [52] = "ExplorableRegions";
                int.TryParse(csvFile.mapData[i].data[53],out data.ReloadTime);
                columnNameArray [53] = "ReloadTime";
                int.TryParse(csvFile.mapData[i].data[54],out data.ShotsBeforeReload);
                columnNameArray [54] = "ShotsBeforeReload";
                int.TryParse(csvFile.mapData[i].data[55],out data.ResourceProtectionPercent);
                columnNameArray [55] = "ResourceProtectionPercent";
                int.TryParse(csvFile.mapData[i].data[56],out data.DamageOverFiveSeconds);
                columnNameArray [56] = "DamageOverFiveSeconds";
                bool.TryParse(csvFile.mapData[i].data[57],out data.CountersArmored);
                columnNameArray [57] = "CountersArmored";
                int.TryParse(csvFile.mapData[i].data[58],out data.XpGain);
                columnNameArray [58] = "XpGain";
                data.TID = csvFile.mapData[i].data[59];
                columnNameArray [59] = "TID";
                data.InfoTID = csvFile.mapData[i].data[60];
                columnNameArray [60] = "InfoTID";
                data.SubtitleTID = csvFile.mapData[i].data[61];
                columnNameArray [61] = "SubtitleTID";
                data.SWF = csvFile.mapData[i].data[62];
                columnNameArray [62] = "SWF";
                data.ExportName = csvFile.mapData[i].data[63];
                columnNameArray [63] = "ExportName";
                data.ExportNameNpc = csvFile.mapData[i].data[64];
                columnNameArray [64] = "ExportNameNpc";
                data.ExportNameConstruction = csvFile.mapData[i].data[65];
                columnNameArray [65] = "ExportNameConstruction";
                data.ModelName = csvFile.mapData[i].data[66];
                columnNameArray [66] = "ModelName";
                data.TextureName = csvFile.mapData[i].data[67];
                columnNameArray [67] = "TextureName";
                data.MeshName = csvFile.mapData[i].data[68];
                columnNameArray [68] = "MeshName";
                int.TryParse(csvFile.mapData[i].data[69],out data.Width);
                columnNameArray [69] = "Width";
                int.TryParse(csvFile.mapData[i].data[70],out data.Height);
                columnNameArray [70] = "Height";
                data.Icon = csvFile.mapData[i].data[71];
                columnNameArray [71] = "Icon";
                data.ExportNameBuildAnim = csvFile.mapData[i].data[72];
                columnNameArray [72] = "ExportNameBuildAnim";
                data.DestroyEffect = csvFile.mapData[i].data[73];
                columnNameArray [73] = "DestroyEffect";
                data.AttackEffect = csvFile.mapData[i].data[74];
                columnNameArray [74] = "AttackEffect";
                data.AttackEffect2 = csvFile.mapData[i].data[75];
                columnNameArray [75] = "AttackEffect2";
                data.AttackEffect3 = csvFile.mapData[i].data[76];
                columnNameArray [76] = "AttackEffect3";
                data.HitEffect = csvFile.mapData[i].data[77];
                columnNameArray [77] = "HitEffect";
                data.Projectile = csvFile.mapData[i].data[78];
                columnNameArray [78] = "Projectile";
                data.ExportNameDamaged = csvFile.mapData[i].data[79];
                columnNameArray [79] = "ExportNameDamaged";
                int.TryParse(csvFile.mapData[i].data[80],out data.BuildingW);
                columnNameArray [80] = "BuildingW";
                int.TryParse(csvFile.mapData[i].data[81],out data.BuildingH);
                columnNameArray [81] = "BuildingH";
                data.ExportNameBase = csvFile.mapData[i].data[82];
                columnNameArray [82] = "ExportNameBase";
                data.ExportNameBaseNpc = csvFile.mapData[i].data[83];
                columnNameArray [83] = "ExportNameBaseNpc";
                data.PickUpEffect = csvFile.mapData[i].data[84];
                columnNameArray [84] = "PickUpEffect";
                data.PlacingEffect = csvFile.mapData[i].data[85];
                columnNameArray [85] = "PlacingEffect";
                data.DefenderCharacter = csvFile.mapData[i].data[86];
                columnNameArray [86] = "DefenderCharacter";
                int.TryParse(csvFile.mapData[i].data[87],out data.DefenderCount);
                columnNameArray [87] = "DefenderCount";
                int.TryParse(csvFile.mapData[i].data[88],out data.DefenderZ);
                columnNameArray [88] = "DefenderZ";
                data.ExportNameTriggered = csvFile.mapData[i].data[89];
                columnNameArray [89] = "ExportNameTriggered";
                data.AppearEffect = csvFile.mapData[i].data[90];
                columnNameArray [90] = "AppearEffect";
                data.MissEffect = csvFile.mapData[i].data[91];
                columnNameArray [91] = "MissEffect";
                int.TryParse(csvFile.mapData[i].data[92],out data.VillagerProbability);
                columnNameArray [92] = "VillagerProbability";
                data.BuildingReadyEffect = csvFile.mapData[i].data[93];
                columnNameArray [93] = "BuildingReadyEffect";
                data.ExportNameBoss = csvFile.mapData[i].data[94];
                columnNameArray [94] = "ExportNameBoss";
                int.TryParse(csvFile.mapData[i].data[95],out data.ProjSkewing);
                columnNameArray [95] = "ProjSkewing";
                int.TryParse(csvFile.mapData[i].data[96],out data.ProjYScaling);
                columnNameArray [96] = "ProjYScaling";
                dataList.Add(data);
            }
            return dataList;
        }
        public string Name;//
        public int DefenseValue;//
        public string BuildingClass;//
        public int BuildTimeD;//
        public int BuildTimeH;//
        public int BuildTimeM;//
        public int BuildTimeS;//
        public string BuildCost;//
        public int TownHallLevel;//
        public string MaxStoredResource;//
        public bool Bunker;//
        public int HousingSpace;//
        public string ProducesResource;//
        public int ResourcePerHour;//
        public int ResourceMax;//
        public int UnitProduction;//
        public bool UpgradesUnits;//
        public int Hitpoints;//
        public int RegenTime;//
        public int AttackRange;//
        public int AttackSpeed;//
        public int Damage;//
        public bool AirTargets;//
        public bool GroundTargets;//
        public int MinAttackRange;//
        public int DamageRadius;//
        public int DamageSpread;//
        public bool PushBack;//
        public bool CanNotSellLast;//
        public int DestructionXP;//
        public bool Locked;//
        public bool Hidden;//
        public int TriggerRadius;//
        public bool ForgesSpells;//
        public bool CreatesArtifacts;//
        public int ArtifactType;//
        public int ArtifactCapacity;//
        public int DeepseaResourceReward;//
        public int DeepseaCommonArtifactChance;//
        public int DeepseaRareArtifactChance;//
        public int DeepseaEpicArtifactChance;//
        public int DeepseaRandomFactor;//
        public int DeepseaPrice;//
        public bool IsMapRoom;//
        public int StartingEnergy;//
        public int MaxEnergy;//
        public int EnergyGain;//
        public bool IsVault;//
        public bool IsHeroHouse;//
        public int LandingBoats;//
        public bool CanNotMove;//
        public bool IsGlobalBuilding;//
        public int ExplorableRegions;//
        public int ReloadTime;//
        public int ShotsBeforeReload;//
        public int ResourceProtectionPercent;//
        public int DamageOverFiveSeconds;//
        public bool CountersArmored;//
        public int XpGain;//
        public string TID;//
        public string InfoTID;//
        public string SubtitleTID;//
        public string SWF;//
        public string ExportName;//
        public string ExportNameNpc;//
        public string ExportNameConstruction;//
        public string ModelName;//
        public string TextureName;//
        public string MeshName;//
        public int Width;//
        public int Height;//
        public string Icon;//
        public string ExportNameBuildAnim;//
        public string DestroyEffect;//
        public string AttackEffect;//
        public string AttackEffect2;//
        public string AttackEffect3;//
        public string HitEffect;//
        public string Projectile;//
        public string ExportNameDamaged;//
        public int BuildingW;//
        public int BuildingH;//
        public string ExportNameBase;//
        public string ExportNameBaseNpc;//
        public string PickUpEffect;//
        public string PlacingEffect;//
        public string DefenderCharacter;//
        public int DefenderCount;//
        public int DefenderZ;//
        public string ExportNameTriggered;//
        public string AppearEffect;//
        public string MissEffect;//
        public int VillagerProbability;//
        public string BuildingReadyEffect;//
        public string ExportNameBoss;//
        public int ProjSkewing;//
        public int ProjYScaling;//
    }
}
