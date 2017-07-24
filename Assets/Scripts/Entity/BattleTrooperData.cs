using UnityEngine;
using System.Collections;

public class BattleTrooperData  {
	public int id;  //运兵船ID或者武器的ID;
	public long building_id;//所在军舰;
	public string landingShipTidLevel;
	public bool isDeployed;
	//兵或武器的TID;
	public string tid;
	public string tidLevel;
	public int num;
	public int weaponCost;
	//public int Energy;
	public int energyIncrease;//每次使用添加WeaponCost数量;
	public int damage;//破坏力(加成后的);
	public int hitpoints;//生命力(加成后的);
	public CsvInfo csvInfo;//对就的csv数据;

	public BattleItem uiItem;

}
