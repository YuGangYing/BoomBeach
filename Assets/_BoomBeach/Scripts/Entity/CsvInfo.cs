using UnityEngine;
using System.Collections;

//在地图上可以显示的数据集,以TID_LEVEL为索引唯一记录;

//buildings,characters,decos,obstacles,spells,traps,artifacts,artifact_bonuses,townhall_levels;

public class CsvInfo:BaseEntity {
	//buildings
	public string TID_Type;//buildings,characters,decos,obstacles,spells,traps,artifacts,artifact_bonuses
	public string TID_Level;//唯一索引标示;
	public string TID;
	public int Level;
	public string Name;
	public int DefenseValue;
	public string BuildingClass;
	public int BuildTimeD;
	public int BuildTimeH;
	public int BuildTimeM;
	public int BuildTimeS;
	public string BuildCost;//126000,25700,4900;Wood,Stone,Metal

	//以下3个属性由BuildCost分解出来;
	public int BuildCostWood=0;
	public int BuildCostStone=0;
	public int BuildCostIron=0;

	public int TownHallLevel;
	public string MaxStoredResource;
	//HQ: 0,3000,2000,0,0: 
	//Gold Storage: 0: Gold
	//Wood Storage、Stone Storage、Metal Storage：0,0,0: Wood,Stone,iron
	//Vault: 0,70000,30000,13000,4000:
	//以下4个参数由MaxStoredResource分解出来;
	public int MaxStoredResourceGold=0;
	public int MaxStoredResourceWood=0;
	public int MaxStoredResourceStone=0;
	public int MaxStoredResourceIron=0;



	public int Bunker;
	public int HousingSpace;
	public string ProducesResource;
	public int ResourcePerHour;
	public int ResourceMax;
	public int UnitProduction;
	public int UpgradesUnits;
	public int Hitpoints;
	public int RegenTime;
	public int AttackRange;
	public int AttackSpeed;
	public int Damage;
	public int AirTargets;
	public int GroundTargets;
	public int MinAttackRange;
	public int DamageRadius;
	public int DamageSpread;
	public int PushBack;
	public int CanNotSellLast;
	public int DestructionXP;
	public int Locked;
	public int Hidden;
	public int TriggerRadius;
	public int ForgesSpells;
	public int CreatesArtifacts;
	public int IsArtifact;
	public int ArtifactCapacity;
	public int DeepseaResourceReward;
	public int DeepseaCommonArtifactChance;
	public int DeepseaRareArtifactChance;
	public int DeepseaEpicArtifactChance;
	public int DeepseaRandomFactor;
	public int DeepseaPrice;
	public int IsMapRoom;
	public int StartingEnergy;
	public int MaxEnergy;
	public int EnergyGain;
	public int IsVault;
	public int IsHeroHouse;
	public int LandingBoats;
	public int CanNotMove;
	public int IsGlobalBuilding;
	public int ExplorableRegions;
	public int ReloadTime;
	public int ShotsBeforeReload;
	public int ResourceProtectionPercent;
	public int DamageOverFiveSeconds;
	public int CountersArmored;
	public string InfoTID;
	public string SubtitleTID;
	public string SWF;
	public string ExportName;
	public string ExportNameNpc;
	public string ExportNameConstruction;
	public string ModelName;
	public string TextureName;
	public string MeshName;
	public int Width;
	public int Height;
	public string Icon;
	public string ExportNameBuildAnim;
	public string DestroyEffect;
	public string AttackEffect;
	public string AttackEffect2;
	public string AttackEffect3;
	public string HitEffect;
	public string Projectile;
	public string ExportNameDamaged;
	public int BuildingW;
	public int BuildingH;
	public string ExportNameBase;
	public string ExportNameBaseNpc;
	public string PickUpEffect;
	public string PlacingEffect;
	public string DefenderCharacter;
	public int DefenderCount;
	public int DefenderZ;
	public string ExportNameTriggered;
	public string AppearEffect;
	public string MissEffect;
	public int VillagerProbability;
	public string BuildingReadyEffect;
	public string ExportNameBoss;

	public int XpGain;
	public int ProjSkewing;
	public int ProjYScaling;
	public int ArtifactType;

	//characters
	//public string Name;
	//public int HousingSpace;
	public int HeroHouseLevel;
	public int UnlockTownHallLevel;
	public int UpgradeHouseLevel;
	public int Speed;
	//public int Hitpoints;
	public int TrainingTime;
	public string TrainingCost;
	public int UpgradeTimeH;
	public string UpgradeCost;
	public int MaxAttackRange;
	//public int AttackRange;
	//public int AttackSpeed;
	//public int Damage;
	public int LifeLeach;
	//public int DamageSpread;
	public int PreferedTargetDamageMod;
	//public int DamageRadius;
	public string PreferedTargetBuilding;
	public int IsFlying;
	//public int AirTargets;
	//public int GroundTargets;
	public int AttackCount;
	public int Energy;
	public int EnergyIncrease;
	public int UpgradeCostArtifacts;
	public string UpgradeCostArtifactResource;
	public int RepairCost;
	public int IsNpc;
	public int CanAttackWhileWalking;
	public int Armored;
	//public string TID;
	//public int Level;
	//public string InfoTID;
	//public string SubtitleTID;
	//public string SWF;
	public string SpeedTID;
	public string AttackRangeTID;
	public string IconSWF;
	public string IconExportName;
	public string BigPicture;
	//public string Projectile;
	public string DeployEffect;
	//public string AttackEffect;
	//public string AttackEffect2;
	//public string AttackEffect3;
	//public string HitEffect;
	public string DisembarkEffect;
	public string DieEffect;
	public string Animation;
	public string AnimationEnemy;
	//public string MissEffect;

	//decos
	//public string Name;
	//public string TID;
	//public int Level;
	//public string InfoTID;
	//public string SWF;
	//public string ExportName;
	//public string ExportNameConstruction;
	//public string BuildCost;
	public int RequiredExpLevel;
	public int MaxCount;
	//public int Width;
	//public int Height;
	//public string Icon;
	//public string ExportNameBase;
	//public string ExportNameBaseNpc;
	//public int VillagerProbability;
	public int EventProbability;

	//obstacles
	//public string Name;
	//public string TID;
	//public int Level;
	//public string SWF;
	//public string ExportName;
	//public string ExportNameBase;
	//public string ExportNameBaseNpc;
	public int ClearTimeSeconds;
	//public int Width;
	//public int Height;
	public int Passable;
	public string ClearCost;
	public string LootResource;
	public int LootCount;
	public string ClearEffect;
	//public string PickUpEffect;
	public int RespawnWeight;
	public string DepositResource;
	public int RequiredTownHallLevel;

	//spells
	//public string Name;
	//public int UnlockTownHallLevel;
	//public int UpgradeHouseLevel;
	public int DeployTimeMS;
	public int ChargingTimeMS;
	public int HitTimeMS;
	//public int UpgradeTimeH;
	//public string UpgradeCost;
	public int BoostTimeMS;
	public int SpeedBoost;
	public int SpeedBoost2;
	public int DamageBoostPercent;
	//public int Damage;
	public int Radius;
	public int RadiusAgainstTroops;
	public int NumberOfHits;
	public int RandomRadius;
	public int TimeBetweenHitsMS;
	public int RandomRadiusAffectsOnlyGfx;
	public int IsStun;
	public int IsFocusFire;
	public int IsSelect;
	public int ProjectileForAllHits;
	//public int Energy;
	//public int EnergyIncrease;
	//public string TID;
	//public int Level;
	//public string InfoTID;
	//public string SubtitleTID;
	//public string IconSWF;
	//public string IconExportName;
	//public string BigPicture;
	public string PreDeployEffect;
	//public string DeployEffect;
	public string DeployEffect2;
	public string ChargingEffect;
	//public string HitEffect;
	//public string Projectile;
	public string HitEffectBuildingSelected;
	public string HitEffectBuildingSelected2;

	//traps
	//public string Name;
	//public string TID;
	//public int Level;
	//public string InfoTID;
	//public string SubtitleTID;
	//public int DefenseValue;
	//public string SWF;
	//public string ExportName;
	//public string IconSWF;
	//public string BigPicture;
	//public int UpgradeTimeH;
	//public string UpgradeCost;
	//public int UpgradeHouseLevel;
	//public int Damage;
	//public int DamageRadius;
	//public int TriggerRadius;
	//public int Width;
	//public int Height;
	public string Effect;
	public string Effect2;
	public string DamageEffect;
	//public int Passable;
	//public string BuildCost;
	public int EjectVictims;
	public int EjectHousingLimit;
	//public string ExportNameTriggered;
	public int ActionFrame;
	//public string PickUpEffect;
	//public string PlacingEffect;
	//public string AppearEffect;
	//public int Hidden;
	//public int CountersArmored;

	//artifacts
	//public string Name;
	//public string TID;
	//public int Level;
	public int PiecesNeeded;
	public string PieceResource;
	public int PieceDropChance;
	public string SellResource;
	public int SellResourceAmount;
	public int BoostPercentage;
	public int BoostProbability;
	//public int BuildTimeS;

	//artifact_bonuses
	//public string Name;
	//public string TID;
	//public int Level;
	//public string InfoTID;
	public int BoostMultiplier;
	public string ExportNameAdd;

	//townhall_levels 与 buildings 合并成一条记录;
	//public string Name;
	//public string TID;
	//public int Level;
	public int TID_BUILDING_WOOD_STORAGE;//     Wood Storage
    public int TID_BUILDING_STONE_STORAGE; //   Stone Storage
    public int TID_BUILDING_METAL_STORAGE;
	public int TID_BUILDING_HOUSING;//Housing
    public int TID_BUILDING_STONE_QUARRY;
	public int TID_BUILDING_METAL_MINE;
	public int TID_BUILDING_BIG_BERTHA;
	public int TID_GUARD_TOWER;
	public int TID_BUILDING_MORTAR;
	public int TID_MACHINE_GUN_NEST;
	public int TID_BUILDING_CANNON;
	public int TID_FLAME_THROWER;
	public int TID_MISSILE_LAUNCHER;
	public int TID_TRAP_TANK_MINE;
	public int TID_TRAP_MINE;
	public int TID_BUILDING_ARTIFACT_WORKSHOP;
	public int TID_BUILDING_MAP_ROOM;
	public int TID_BUILDING_VAULT;
	public int TID_BUILDING_GOLD_STORAGE;
	public int TID_BUILDING_LABORATORY;
	public int TID_BUILDING_LANDING_SHIP;
	public string RequiredBuilding;
	public int RequiredBuildingLevel;


	public new void HashtableToBean(Hashtable hashtable){
		base.HashtableToBean(hashtable);
		/*
		public string BuildCost;//126000,25700,4900;Wood,Stone,Metal
		
		//以下3个属性由BuildCost分解出来;
		public int BuildCostWood=0;
		public int BuildCostStone=0;
		public int BuildCostMetal=0;
		*/
		if (BuildCost != null && BuildCost != ""){
			string[] cost = BuildCost.Split(","[0]);
			if (cost.Length == 3){
				BuildCostWood=int.Parse(cost[0]);
				BuildCostStone=int.Parse(cost[1]);
				BuildCostIron=int.Parse(cost[2]);
			}
		}else{
			BuildCostWood=0;
			BuildCostStone=0;
			BuildCostIron=0;
		}




		/*
		public string MaxStoredResource;
		//HQ: 0,3000,2000,0,0: 
		//Gold Storage: 0: Gold
		//Wood Storage、Stone Storage、Metal Storage：0,0,0: Wood,Stone,Metal
		//Vault: 0,70000,30000,13000,4000:  0,24000,15000,3000,0: 0，gold,worod,stone,metal
		//以下4个参数由MaxStoredResource分解出来;
		public int MaxStoredResourceGold=0;
		public int MaxStoredResourceWood=0;
		public int MaxStoredResourceStone=0;
		public int MaxStoredResourceMetal=0;
		*/

		if (MaxStoredResource != null && MaxStoredResource != ""){
			string[] cost = MaxStoredResource.Split(","[0]);
			if (TID == "TID_BUILDING_PALACE"||TID == "TID_BUILDING_VAULT"){
				MaxStoredResourceGold=int.Parse(cost[1]);
				MaxStoredResourceWood=int.Parse(cost[2]);
				MaxStoredResourceStone=int.Parse(cost[3]);
				MaxStoredResourceIron=int.Parse(cost[4]);
			}else if (TID == "TID_BUILDING_GOLD_STORAGE"){
				MaxStoredResourceGold=int.Parse(cost[0]);
				MaxStoredResourceWood=0;
				MaxStoredResourceStone=0;
				MaxStoredResourceIron=0;
			}else if (TID == "TID_BUILDING_WOOD_STORAGE"||TID == "TID_BUILDING_STONE_STORAGE"||TID == "TID_BUILDING_METAL_STORAGE"){
				MaxStoredResourceGold=0;
				MaxStoredResourceWood=int.Parse(cost[0]);
				MaxStoredResourceStone=int.Parse(cost[1]);
				MaxStoredResourceIron=int.Parse(cost[2]);
			}
		}else{
			MaxStoredResourceGold=0;
			MaxStoredResourceWood=0;
			MaxStoredResourceStone=0;
			MaxStoredResourceIron=0;
		}

	}
}
