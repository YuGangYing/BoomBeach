using UnityEngine;
using System.Collections;

public class EnemyNameAndResource : MonoBehaviour {
	public string artifact_name1 = "";//实际随机获得的石像1',CommonPiece,CommonPieceIce,CommonPieceFire,CommonPieceDark
	public int artifact_num1 = 0;//实际随机获得的石像1数量',

	public string artifact_name2 = "";//实际随机获得的石像2',RarePiece,RarePieceIce,RarePieceFire,RarePieceDark
	public int artifact_num2 = 0;//实际随机获得的石像2数量',
	
	public string artifact_name3 = "";//实际随机获得的石像3',EpicPiece,EpicPieceIce,EpicPieceFire,EpicPieceDark
	public int artifact_num3 = 0;//实际随机获得的石像3数量',

	private UILabel UserLevelLabel;

	private static EnemyNameAndResource instance;
	public static EnemyNameAndResource Instance{
		get{return instance;}
	}
	void Awake()
	{
		instance = this;
	}


	//用户等级;
	public string UserLevel{
		set{
			UserLevelLabel.text = value;
		}
	}

	private UILabel UserNameLabel;
	//用户名称;
	public string UserName{
		set{
			UserNameLabel.text = value;
		}
	}

	private UILabel GoldResourceLabel;
	private int goldResource;
	//金币资源;
	public int GoldResource{
		set{
			goldResource = value;
			GoldResourceLabel.text = value.ToString();
		}
		get{
			return goldResource;
		}
	}
	private UILabel GoldResourceAddLabel;
	private int goldResourceAdd;
	//金币附加资源;
	public int GoldResourceAdd{
		set{
			goldResourceAdd = value;
			GoldResourceAddLabel.text = "+"+value.ToString();
			if(value>0)
			{
				GoldResourceAddLabel.gameObject.SetActive(true);
			}
			else
			{
				GoldResourceAddLabel.gameObject.SetActive(false);
			}
		}
		get{
			return goldResourceAdd;
		}
	}


	private UILabel WoodResourceLabel;
	private int woodResource;
	//木材资源;
	public int WoodResource{
		set{
			woodResource = value;
			WoodResourceLabel.text = value.ToString();
		}
		get{
			return woodResource;
		}
	}
	private UILabel WoodResourceAddLabel;
	private int woodResourceAdd;
	//木材附加资源;
	public int WoodResourceAdd{
		set{
			woodResourceAdd = value;
			WoodResourceAddLabel.text = "+"+value.ToString();
			if(value>0)
			{
				WoodResourceAddLabel.gameObject.SetActive(true);
			}
			else
			{
				WoodResourceAddLabel.gameObject.SetActive(false);
			}
		}
		get{
			return woodResourceAdd;
		}
	}

	private UILabel StoneResourceLabel;
	private int stoneResource;
	//石头资源;
	public int StoneResource{
		set{
			stoneResource = value;
			StoneResourceLabel.text = value.ToString();
		}
		get{
			return stoneResource;
		}
	}
	private UILabel StoneResourceAddLabel;
	private int stoneResourceAdd;
	//石头附加资源;
	public int StoneResourceAdd{
		set{
			stoneResourceAdd = value;
			StoneResourceAddLabel.text = "+"+value.ToString();
			if(value>0)
			{
				StoneResourceAddLabel.gameObject.SetActive(true);
			}
			else
			{
				StoneResourceAddLabel.gameObject.SetActive(false);
			}
		}
		get{
			return stoneResourceAdd;
		}
	}

	private UILabel IronResourceLabel;
	private int ironResource;
	//金属资源;
	public int IronResource{
		set{
			ironResource = value;
			IronResourceLabel.text = value.ToString();
		}
		get{
			return ironResource;
		}
	}
	private UILabel IronResourceAddLabel;
	private int ironResourceAdd;
	//金属附加资源;
	public int IronResourceAdd{
		set{
			ironResourceAdd = value;
			IronResourceAddLabel.text = "+"+value.ToString();
			if(value>0)
			{
				IronResourceAddLabel.gameObject.SetActive(true);
			}
			else
			{
				IronResourceAddLabel.gameObject.SetActive(false);
			}
		}
		get{
			return ironResourceAdd;
		}
	}

	private UILabel MedalResourceLabel;
	private int medalResource;
	//奖牌;
	public int MedalResource{
		set{
			medalResource = value;
			MedalResourceLabel.text = value.ToString();
		}
		get{
			return medalResource;
		}
	}

	private GameObject ChanceToGetObj;
	private int _CrystleNum;
	private int _MedalResourceAdd;

	private string crystleType;
	private UISprite CrystleTypeSprite;
	private GameObject MedalResourceAddObj;
	private UILabel MedalResourceAddLabel;

	//水晶类型;
	public string CrystleType{
		set{
			crystleType = value;
			CrystleTypeSprite.spriteName = value;
			CrystleTypeSprite.MakePixelPerfect();
		}
		get{
			return crystleType;
		}
	}
	//水晶数量;
	public int CrystleNum{
		set{
			_CrystleNum = value;
			if(_CrystleNum==0&&_MedalResourceAdd==0)
			{
				ChanceToGetObj.SetActive(false);
			}
			else
			{
				ChanceToGetObj.SetActive(true);
			}
		}
		get{
			return _CrystleNum;
		}
	}
	//附加奖牌;
	public int MedalResourceAdd{
		set{
			_MedalResourceAdd = value;
			MedalResourceAddLabel.text = value.ToString();
			if(_CrystleNum==0&&_MedalResourceAdd==0)
			{
				ChanceToGetObj.SetActive(false);
			}
			else
			{
				ChanceToGetObj.SetActive(true);
				if(_MedalResourceAdd>0)
				{
					MedalResourceAddObj.SetActive(true);
				}
				else
				{
					MedalResourceAddObj.SetActive(false);
				}
			}
		}
		get{
			return _MedalResourceAdd;
		}
	}

	// Use this for initialization
	public void Init () {


		UserLevelLabel = transform.Find ("Userlevel/LevelLabel").GetComponent<UILabel> ();
		UserNameLabel= transform.Find ("Userlevel/Label").GetComponent<UILabel> ();

		GoldResourceLabel= transform.Find ("ResourceGold/num").GetComponent<UILabel> ();
		GoldResourceAddLabel= transform.Find ("ResourceGold/addnum").GetComponent<UILabel> ();

		WoodResourceLabel= transform.Find ("ResourceWood/num").GetComponent<UILabel> ();
		WoodResourceAddLabel= transform.Find ("ResourceWood/addnum").GetComponent<UILabel> ();

		StoneResourceLabel= transform.Find ("ResourceStone/num").GetComponent<UILabel> ();
		StoneResourceAddLabel= transform.Find ("ResourceStone/addnum").GetComponent<UILabel> ();

		IronResourceLabel= transform.Find ("ResourceIron/num").GetComponent<UILabel> ();
		IronResourceAddLabel= transform.Find ("ResourceIron/addnum").GetComponent<UILabel> ();

		MedalResourceLabel= transform.Find ("ResourceMedal/num").GetComponent<UILabel> ();

		ChanceToGetObj = transform.Find ("AddedGet").gameObject;
		CrystleTypeSprite = ChanceToGetObj.transform.Find ("ResourceCrystle/ico/Sprite").GetComponent<UISprite> ();
		MedalResourceAddObj = ChanceToGetObj.transform.Find ("ResourceMedal").gameObject;
		MedalResourceAddLabel = ChanceToGetObj.transform.Find ("ResourceMedal/num").GetComponent<UILabel> ();
	}
	

}
