public static class ApiConstant {

	//获取ユーザー数据
	public const string CMD_USERINFO = "getUserInfo";
	//languageを変更します
	public const string CMD_CHANGE_LNG = "change_lng";
	//pwdを変更します
	public const string CMD_CHANGE_PWD = "change_pwd";
	//收集资源
	public const string CMD_Collect = "collect";
	//treasureを採集する
	public const string CMD_COLLECT_TREASURE = "collect_treasure";
	//创建建筑
	public const string CMD_CreateBuilding = "createBuilding";
	//升级建筑
	public const string CMD_UpgradeBuilding = "upgradeBuilding";
	//建物が取り除く
	public const string CMD_RemoveBuilding = "removal";
	//升级士兵
	public const string CMD_UpgradeTrooper = "upgradeCharacter";
	//升级科技
	public const string CMD_UpgradeTech = "upgradeTech";
	//立即完成升级仅升级过程中（建筑升级，科技升级，士兵升级）
	public const string CMD_SpeedUP = "speedUp";
	//训练士兵
	public const string CMD_Train = "train";
	//训练所有士兵
	public const string CMD_TrainAll = "trainAll";
	//立即完成升级仅面板（建筑升级，科技升级，士兵升级）
	public const string CMD_ImmediatelyUp ="immediatelyUp";
	//立即完成士兵训练
	public const string CMD_SpeedUPTrain = "speedUpTrain";
	//立即完成所有士兵训练
	public const string CMD_SpeedUPTrainAll = "speedUpTrainAll";
	//移动建筑
	public const string CMD_MoveBuilding = "changeXY";
	//建筑自然完成
	public const string CMD_BuildDone = "buildingActionDone";
	public const string CMD_TrooperUpgradeDone = "soldierActionDone";
	public const string CMD_TechUpgradeDone = "spellActionDone";
	//取消建筑操作
	public const string CMD_CancelBuilding = "cancelBuilding";
	//取消科技升级
	public const string CMD_CancelTech = "cancelTech";
	//取消士兵升级
	public const string CMD_CancelSoilder="cancelSoilder";
	//取消士兵训练
	public const string CMD_CancelTraining="cancelTrain";
	//攻撃日誌
	public const string CMD_ATTACK_LOG = "attack_log";
	//攻撃结果验证
	public const string CMD_ATTACK_CHECK = "attack_check";
	//部署军队到小岛
	public const  string CMD_ATTACK_DEPLOY_TROOPS = "attack_deploy_troops";
	//展开大地图云
	public const string CMD_EXPLORE = "explore";
	//刷新用户的小岛（重新寻找敌人）
	public const string CMD_UPDATE_USER_REGIONS = "update_user_regions";
	//島で資源を採集する
	public const string CMD_COLLECT_ISLAND = "collect_island";



}
