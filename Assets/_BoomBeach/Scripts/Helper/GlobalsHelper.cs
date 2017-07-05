using UnityEngine;
using System.Collections;

public static class GlobalsHelper  {

	public static void InitGlobals(){
		SysConfig sysConfig = new SysConfig ();
		sysConfig.Init ();

		Globals.sysConfig = sysConfig;
		Globals.defaultLanguage = sysConfig.GetStringProperties (SysConfigKeys.defaultLanguage);
		Globals.isEnabelAutoLogin = sysConfig.GetBoolProperties(SysConfigKeys.isEnabelAutoLogin);
		Globals.sendingPeriod = sysConfig.GetFloatProperties (SysConfigKeys.sendingPeriod);
		Globals.domain = sysConfig.GetStringProperties (SysConfigKeys.domain);
		Globals.port = sysConfig.GetIntProperties (SysConfigKeys.port);
		Globals.zone = sysConfig.GetStringProperties (SysConfigKeys.zone);
		Globals.version = sysConfig.GetIntProperties (SysConfigKeys.version);
		Globals.obstacleAlpha =sysConfig.GetFloatProperties (SysConfigKeys.obstacleAlpha);
		Globals.baseTimeSpan = sysConfig.GetFloatProperties (SysConfigKeys.baseTimeSpan);
		Globals.landCraftWidth =sysConfig.GetIntProperties (SysConfigKeys.landCraftWidth);
		Globals.regions_id =sysConfig.GetIntProperties (SysConfigKeys.regions_id);



	}

}
