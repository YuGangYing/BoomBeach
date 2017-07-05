using UnityEngine;
using System.Collections;

//用于界面UI绑定使用的实体类;
[System.Serializable]
public class ScreenUIData{

	public string UserName;

	public int OldUserLevel;  //操作前等级;
	public int OldExp;        //操作前经验;
	public int OldUpgradeExp; //操作前升级上限;
	public int UserLevel;  //操作后用户等级;
	public int CurrentExp; //操作后经验数;
	public int UpgradeExp; //操作后等级的升级经验数;

	public int UserMedal;

	public int OldGoldCurrent;
	public int OldGoldStorageCapacity;
	public int GoldCurrent;
	public int GoldStorageCapacity;
	public int GoldProduce;
	public int GoldProduceFromHome;
	public int GoldProduceFromVillage;
	public int GoldProtected;


	public int OldWoodCurrent;
	public int OldWoodStorageCapacity;
	public int WoodCurrent;
	public int WoodStorageCapacity;
	public int WoodProduce;
	public int WoodProduceFromHome;
	public int WoodProduceFromVillage;
	public int WoodProtected;

	public int OldStoneCurrent;
	public int OldStoneStorageCapacity;
	public int StoneCurrent;
	public int StoneStorageCapacity;
	public int StoneProduce;
	public int StoneProduceFromHome;
	public int StoneProduceFromVillage;
	public int StoneProtected;

	public int OldIronCurrent;
	public int OldIronStorageCapacity;
	public int IronCurrent;
	public int IronStorageCapacity;
	public int IronProduce;
	public int IronProduceFromHome;
	public int IronProduceFromVillage;
	public int IronProtected;

	public int OldDiamondCurrent;
	public int DiamondCurrent;

}
