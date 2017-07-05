using UnityEngine;
using System.Collections;

public enum Direct{
	RIGHT=0,RIGHTUP=1,UP=2,LEFTUP=3,LEFT=4,LEFTDOWN=5,DOWN=6,RIGHTDOWN=7
}

public enum TrooperType
{
    //5种士兵类型
	Heavy,Rifleman,Tank,Warrior,Zooka
}

public enum SceneStatus{
	HOME,  //建设主场景;
	WORLDMAP,  //世界地图;
	HOMERESOURCE,  //己方资源场景;
	ENEMYVIEW,	 //敌方查看场景;
	ENEMYBATTLE, //敌方战斗场景;
	BATTLEREPLAY, //战斗回放;
	FRIENDVIEW	  //普通查看，好友查看
}

public enum IslandType{
	playerbase,  //0玩家岛;
	mainland_a, //1
	mainland_b,//2
	small_a,//3
	small_b,//4
	med_a//5
}