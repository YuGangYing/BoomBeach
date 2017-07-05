using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using BoomBeach;

public class WorldHouse : MonoBehaviour {


	public tk2dSprite house_sprite;
	public tk2dSprite type_sprite;
	public tk2dSprite level_sprite;
	public tk2dSprite top_sprite;
	public tk2dSprite sigh_sprite;
	//public new EasyFontTextMesh name;
	public tk2dTextMesh level;
	public tk2dUIItem tk2dUIItem;

	public Camera worldCamera;
	private UserRegions ur;

	public void OnDown(tk2dUIItem sender){
		WorldCameraOpEvent.Instance.Status = false;
		//Debug.Log("OnDown");
	}

	public void OnUp(tk2dUIItem sender){
		//Debug.Log("OnUp");

		Vector3 touchPosition = Vector3.zero;
		if(Input.touchCount>0)
		{
			//屏幕点;
			touchPosition = Input.GetTouch(0).position;
		}
		else
		{
			//兼容mouse操作;		
			touchPosition = Input.mousePosition;
		}
		//Debug.Log(WorldCameraOpEvent.Instance.EventStatus);
		if (WorldCameraOpEvent.Instance.EventStatus == false && WorldBtnEvent.Instance.checkUIOp2(touchPosition) == false){
			//Debug.Log("OnUp2");
			//Debug.Log(sender.transform.parent.name);
			//sender.sendMessageTarget
			//int show_type = int.Parse(sender.transform.parent.name);
			if (ur.is_npc == 9){
				if (ur.sending == false){
								
					ur.sending = true;
					ISFSObject data = new SFSObject();
					data.PutInt("id",ur.id);
					data.PutInt("regions_id",ur.regions_id);
					SFSNetworkManager.Instance.SendRequest(data, "collect_treasure", false, HandleCollectTreasureResponse);
				}
			}else{
				
				//show_type;1:go home; 2:Scout,Attack; 3:Scout,Attack,Find; 4:Explore; 5:Production per hour;
				int show_type = 0;
				if (ur.capture_id == DataManager.GetInstance().userInfo.id){
					if (ur.regions_id == 0 || ur.regions_id == 1 || ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE"){
						show_type = 1;
					}else{
						show_type = 5;
					}
				}else{
					if (Helper.current_time() - ur.capture_time > Globals.FindNewOpponentTime){
						//默认22小时，可更新一次(只能更新比自己级别高的或资源岛屿;
						if (ur.is_npc == 0 || ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE"){						
							show_type = 3;
						}else{
							show_type = 2;
						}
					}else{
						show_type = 2;
					}
				}
				
				
				if (show_type > 0){
					Vector3 scPoint = worldCamera.WorldToScreenPoint(transform.position);
					UIManager.GetInstance().GetController<IslandPopCtrl>().ShowIslandPop(ur, scPoint);
					//Debug.Log("dddd");
					bool isup = scPoint.y > Screen.height / 2;
					if (!isup){
						scPoint = worldCamera.WorldToScreenPoint(top_sprite.transform.position);
					}

                    if (ScreenUIManage.Instance != null)
                        ScreenUIManage.Instance.ShowPopWorld(ur,scPoint,true,show_type,isup);
                    
                }
			}
		}

		WorldCameraOpEvent.Instance.Status = true;
	}
	/*
	public void OnClick(tk2dUIItem sender){
		Vector3 touchPosition = Vector3.zero;
		if(Input.touchCount>0)
		{
			//屏幕点;
			touchPosition = Input.GetTouch(0).position;
		}
		else
		{
			//兼容mouse操作;		
			touchPosition = Input.mousePosition;
		}
		Debug.Log(WorldCameraOpEvent.Instance.EventStatus);
		if (WorldCameraOpEvent.Instance.EventStatus == false && WorldCameraOpEvent.Instance.checkUIOp2(touchPosition)){

			//Debug.Log(sender.transform.parent.name);
			//sender.sendMessageTarget
			//int show_type = int.Parse(sender.transform.parent.name);
			if (ur.is_npc == 9){
				if (ur.sending == false){
					ur.sending = true;
					ISFSObject data = new SFSObject();
					data.PutInt("id",ur.id);
					//data.PutInt("regions_id",ur.regions_id);
					NetworkManager.Instance.SendRequest(data, "collect_treasure", false, HandleCollectTreasureResponse);
				}
			}else{

				//show_type;1:go home; 2:Scout,Attack; 3:Scout,Attack,Find; 4:Explore; 5:Production per hour;
				int show_type = 0;
				if (ur.capture_id == DataManager.GetInstance().userInfo.id){
					if (ur.regions_id == 0 || ur.regions_id == 1 || ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE"){
						show_type = 1;
					}else{
						show_type = 5;
					}
				}else{
					if (Helper.current_time() - ur.capture_time > Globals.FindNewOpponentTime){
						//默认22小时，可更新一次(只能更新比自己级别高的或资源岛屿;
						if (ur.is_npc == 0 || ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE"){						
							show_type = 3;
						}else{
							show_type = 2;
						}
					}else{
						show_type = 2;
					}
				}


				if (show_type > 0){
					Vector3 scPoint = worldCamera.WorldToScreenPoint(transform.position);
					//Debug.Log("dddd");
					bool isup = scPoint.y > Screen.height / 2;
					if (!isup){
						scPoint = worldCamera.WorldToScreenPoint(top_sprite.transform.position);
					}

					ScreenUIManage.Instance.ShowPopWorld(ur,scPoint,true,show_type,isup);
				}
			}
		}
	}
*/

	void HandleCollectTreasureResponse(ISFSObject dt,BuildInfo buildInfo = null)
	{
		Debug.Log(dt.GetDump());

		ur.sending = false;

		Transform house_sprite = ur.worldHouse.gameObject.transform.FindChild("house_sprite");

		Vector3 pos = WorldBtnEvent.Instance.uiCamera.ScreenToWorldPoint(worldCamera.WorldToScreenPoint(house_sprite.position));
		pos = new Vector3(pos.x,pos.y,0f);

		int num = dt.GetInt("treasure_count");
		//GameObject.Find ("UI Root (2D)").GetComponent<GemPartEmitter> ().Emit (num,pos,num);
        GemPartEmitter.Instance().GetComponent<GemPartEmitter>().Emit(num, pos, num);

        ur.worldHouse.gameObject.SetActive(false);
	}

	public void initData(UserRegions ur,Camera worldCamera){
		this.ur = ur;
		this.worldCamera = worldCamera;

		//capture_id'占领用户id,有可能是npc用户,也有可能是正常玩家;
		tk2dUIItem.sendMessageTarget = this.gameObject;
		//tk2dUIItem.SendMessageOnClickMethodName = "OnClick";
		sigh_sprite.gameObject.SetActive(false);

		//Debug.Log("ur.capture_id:" + ur.capture_id + ";DataManager.GetInstance().userInfo.id:" + DataManager.GetInstance().userInfo.id);
		//Debug.Log("ur.res_tid:" + ur.res_tid + ";ur.is_npc:" + ur.is_npc + ";ur.regions_id:" + ur.regions_id);

		if (ur.capture_id == DataManager.GetInstance().userInfo.id){
			//被自己占领,要显示：蓝色图片;
			top_sprite.SetSprite(top_sprite.GetSpriteIdByName("BlueCircle"));
			level_sprite.SetSprite(level_sprite.GetSpriteIdByName("SmallBlueCircle"));
			level.text = ur.capture_level.ToString();



			if (ur.res_tid == "TID_BUILDING_STONE_QUARRY"){
				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Quarry"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("stoneIco"));
				//name.Text = StringFormat.FormatByTid("TID_YOUR_STONE_OUTPOST");
			}else if (ur.res_tid == "TID_BUILDING_WOODCUTTER"){
				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Sawmill"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("woodIco"));	
				//name.Text = StringFormat.FormatByTid("TID_YOUR_WOOD_OUTPOST");
			}else if (ur.res_tid == "TID_BUILDING_METAL_MINE"){
				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("IronMine"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("ironIco"));
				//name.Text = StringFormat.FormatByTid("TID_YOUR_METAL_OUTPOST");
			}else{
				if (ur.is_npc == 9){
					//name.gameObject.SetActive(false);
					top_sprite.gameObject.SetActive(false);
					level_sprite.gameObject.SetActive(false);
					
					house_sprite.SetSprite(house_sprite.GetSpriteIdByName("treasure"));
				}else{
					if (ur.regions_id == 1 || ur.regions_id == 0){
						house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Headquarters"));
						type_sprite.SetSprite(type_sprite.GetSpriteIdByName("BluePerson"));
						//name.Text = StringFormat.FormatByTid("TID_YOU");
						level.text = DataManager.GetInstance().userInfo.exp_level.ToString();
					}else{
						//name.gameObject.SetActive(false);
						top_sprite.gameObject.SetActive(false);
						level_sprite.gameObject.SetActive(false);
						
						house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Residence"));
					}
				}
			}
		}else{

			top_sprite.SetSprite(top_sprite.GetSpriteIdByName("RedCircle"));
			level_sprite.SetSprite(level_sprite.GetSpriteIdByName("SmallRedCircle"));
			level.text = ur.capture_level.ToString();


			if (ur.res_tid == "TID_BUILDING_STONE_QUARRY"){				
				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Quarry"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("stoneIco"));
				if (ur.capture_name == "TID_RESOURCE_BASE" || ur.capture_name == "" || ur.capture_name == null){
					//name.Text = StringFormat.FormatByTid("TID_RESOURCE_BASE");
				}else{
					//name.Text = StringFormat.FormatByTid("TID_PLAYERS_STONE_OUTPOST",new object[]{ur.capture_name});
				}
			}else if (ur.res_tid == "TID_BUILDING_WOODCUTTER"){
				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Sawmill"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("woodIco"));
				if (ur.capture_name == "TID_RESOURCE_BASE" || ur.capture_name == "" || ur.capture_name == null){
					//name.Text = StringFormat.FormatByTid("TID_RESOURCE_BASE");
				}else{
					//name.Text = StringFormat.FormatByTid("TID_PLAYERS_WOOD_OUTPOST",new object[]{ur.capture_name});
				}
			}else if (ur.res_tid == "TID_BUILDING_METAL_MINE"){
				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("IronMine"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("ironIco"));
				if (ur.capture_name == "TID_RESOURCE_BASE" || ur.capture_name == "" || ur.capture_name == null){
					//name.Text = StringFormat.FormatByTid("TID_RESOURCE_BASE");
				}else{
					//name.Text = StringFormat.FormatByTid("TID_PLAYERS_METAL_OUTPOST",new object[]{ur.capture_name});
				}
			}else{
				//
				//电脑npc用户;
				if (ur.is_npc == 1){
					top_sprite.gameObject.SetActive(false);
					level_sprite.transform.localPosition = new Vector3(0,level_sprite.transform.localPosition.y,level_sprite.transform.localPosition.z);
				}



				house_sprite.SetSprite(house_sprite.GetSpriteIdByName("Headquarters"));
				type_sprite.SetSprite(type_sprite.GetSpriteIdByName("RedPerson"));
				//name.Text = StringFormat.FormatByTid(ur.capture_name);
			}


			if (ur.is_npc == 0 && Helper.current_time() - ur.capture_time > Globals.FindNewOpponentTime){
				//默认22小时，可更新一次(只能更新比自己级别高的或资源岛屿;
				//if (ur.capture_level > DataManager.GetInstance().userInfo.exp_level || ur.res_tid == "TID_BUILDING_STONE_QUARRY" || ur.res_tid == "TID_BUILDING_WOODCUTTER" || ur.res_tid == "TID_BUILDING_METAL_MINE"){						
				sigh_sprite.gameObject.SetActive(true);			
				//}
			}
			
		}

		//name.gameObject.SetActive(true);
		//name.Text = ur.regions_id.ToString();
		
		
		
	}

}
