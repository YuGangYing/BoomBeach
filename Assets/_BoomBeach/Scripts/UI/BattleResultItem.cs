using UnityEngine;
using System.Collections;


public enum BattleResultType{
	Heavy,Rifleman,Tank,Warrior,Zooka,Gold,Wood,Stone,Iron,Medal,
	CommonPiece,CommonPieceDark,CommonPieceFire,CommonPieceIce,
	EpicPiece,EpicPieceDark,EpicPieceFire,EpicPieceIce,
	RarePiece,RarePieceDark,RarePieceFire,RarePieceIce,
}
public struct BattleResultData{
	public BattleResultType type;
	public int count;
}


public class BattleResultItem : MonoBehaviour {


	public BattleResultData data;

	private UISprite avatarSprite;
	private UILabel counterLabel;

	private float increaseStep = 0.05f;

	private int increaseStepCount = 0;

	private float zoomOutStep = 0.1f;

	private bool isZoomOut = false;

	private bool isUpdateCounter = false;

	private BattleResultType type{
		set{
			if(value==BattleResultType.Heavy)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "TID_HEAVY";		
				avatarSprite.transform.localScale = Vector3.one*2;
			}
			else if(value==BattleResultType.Rifleman)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "TID_RIFLEMAN";
				avatarSprite.transform.localScale = Vector3.one*2;
			}
			else if(value==BattleResultType.Tank)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "TID_TANK";
				avatarSprite.transform.localScale = Vector3.one*2;
			}
			else if(value==BattleResultType.Warrior)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "TID_WARRIOR";
				avatarSprite.transform.localScale = Vector3.one*2;
			}
			else if(value==BattleResultType.Zooka)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "TID_ZOOKA";
				avatarSprite.transform.localScale = Vector3.one*2;
			}
			else if(value==BattleResultType.Gold)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "goldIco";
				avatarSprite.MakePixelPerfect();
				avatarSprite.transform.localScale = Vector3.one*1.5f;
			}
			else if(value==BattleResultType.Iron)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "ironIco";
				avatarSprite.MakePixelPerfect();
				avatarSprite.transform.localScale = Vector3.one*1.5f;
			}
			else if(value==BattleResultType.Stone)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "stoneIco";
				avatarSprite.MakePixelPerfect();
				avatarSprite.transform.localScale = Vector3.one*1.5f;
			}
			else if(value==BattleResultType.Wood)
			{
				avatarSprite.atlas = PartEmitObj.Instance.avatarAtlas;
				avatarSprite.spriteName = "woodIco";
				avatarSprite.MakePixelPerfect();
				avatarSprite.transform.localScale = Vector3.one*1.5f;
			}
			else if(value==BattleResultType.Medal)
			{
				avatarSprite.atlas = PartEmitObj.Instance.uiAtlas;
				avatarSprite.spriteName = "Medal";
				avatarSprite.MakePixelPerfect();
				avatarSprite.transform.localScale = Vector3.one*2;
			}
			else
			{
				avatarSprite.atlas = PartEmitObj.Instance.uiAtlas;
				avatarSprite.spriteName = value.ToString();
				avatarSprite.MakePixelPerfect();
			}


		}
	}


	private int _counter;
	private int counter
	{
		set{
			_counter = value;
			increaseStepCount = Mathf.CeilToInt(increaseStep*_counter);
		}
	}

	private int currentCounter;

	public BattleResultItem nextItem;



	void Awake()
	{
		transform.localScale = Vector3.one * 2;
		avatarSprite = transform.Find ("avatar").GetComponent<UISprite> ();
		counterLabel = transform.Find ("counter").GetComponent<UILabel> ();
		counterLabel.gameObject.SetActive (false);
		type = data.type;
		counter = data.count;
		isZoomOut = true;
	}

	void OnEnable()
	{
		AudioPlayer.Instance.PlaySfx("loot_fly_in_01");
	}
	
	// Update is called once per frame
	void Update () {
		if(isZoomOut)
		{
			Vector3 currentScale = transform.localScale;
			currentScale = new Vector3(currentScale.x-zoomOutStep*Globals.TimeRatio,currentScale.y-zoomOutStep*Globals.TimeRatio,currentScale.z-zoomOutStep*Globals.TimeRatio);
			if(currentScale.x<=1f)
			{
				isZoomOut = false;
				isUpdateCounter = true;
				counterLabel.gameObject.SetActive (true);
				counterLabel.text = "0";
				transform.localScale = Vector3.one;
				if(nextItem!=null)
				{
					nextItem.gameObject.SetActive(true);
				}
			}
			else
			{
				transform.localScale = currentScale;
			}
		}

		if(isUpdateCounter)
		{
			if(currentCounter<_counter)
			{
				counterLabel.text = currentCounter.ToString();
				currentCounter+=increaseStepCount;
			}
			else
			{
				counterLabel.text = _counter.ToString();
				isUpdateCounter = false;
			}

		}
	}
}
