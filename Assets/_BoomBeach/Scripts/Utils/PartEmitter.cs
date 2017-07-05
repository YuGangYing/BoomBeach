using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PartType{
	Gold,Wood,Stone,Iron,Exp,Gem
}
public class PartEmitter : MonoBehaviour {

	// Use this for initialization
	public List<PartEmitItem> parts = new List<PartEmitItem>();
	private int max;
	private int emitcount; //已发射量;
	public int cpf;  //每帧生成的个数;
	public Vector3 begin;

	public Transform GoldIco;
	public Transform WoodIco;
	public Transform StoneIco;
	public Transform IronIco;
	public Transform ExpIco;
	public Transform GemIco;
	public PartType type;

	private bool isEmit;  //是否发射粒子中;
	public bool isEmitting;  //是正在发射，并且未全部到达目的地;

	public GameObject GoldPartPrefab;
	public GameObject IronPartPrefab;
	public GameObject WoodPartPrefab;
	public GameObject StonePartPrefab;
	public GameObject ExpPartPrefab;
	public GameObject GemPartPrefab;

	public Dictionary<string,GameObject> partItemPrefabs;
	private float minS = -2f;
	private float maxS = 2f;
	private int stepCount;

	private int collectNumber;
	public BuildInfo buildInfo;
	public ResourceShip resourceShip;




	void Awake()
	{
		partItemPrefabs = new Dictionary<string,GameObject> ();
		GoldIco = PartEmitObj.Instance.GoldIco;
		WoodIco = PartEmitObj.Instance.WoodIco;
		StoneIco = PartEmitObj.Instance.StoneIco;
		IronIco = PartEmitObj.Instance.IronIco;
		ExpIco = PartEmitObj.Instance.ExpIco;
		GemIco = PartEmitObj.Instance.GemIco;

		partItemPrefabs ["Gold"] = GoldPartPrefab;
		partItemPrefabs ["Iron"] = IronPartPrefab;
		partItemPrefabs ["Wood"] = WoodPartPrefab;
		partItemPrefabs ["Stone"] = StonePartPrefab;
		partItemPrefabs ["Exp"] = ExpPartPrefab;
		partItemPrefabs ["Gem"] = GemPartPrefab;

		if(partLabel==null)
		{
			GameObject p = Instantiate(ResourceCache.load("UI/PartLabel")) as GameObject;
			p.transform.parent = transform.parent.Find("UI");
			p.transform.localScale = Vector3.one;
			p.transform.localPosition = Vector3.zero;
			p.transform.localEulerAngles = Vector3.zero;
			p.name = "partLabel";
			partLabel = p.transform.Find("Label").GetComponent<UILabel>();
			partLabel.gameObject.SetActive(false);
		}

		enabled = false;
	}

	public void Emit(int count,Vector3 i_begin,PartType i_type,int i_collectNumber)
	{
		isEmit = true;
		isEmitting = true;
		begin = i_begin;
		type = i_type;
		max = count;
		emitcount = 0;
		collectNumber = i_collectNumber;
		parts = new List<PartEmitItem>();
		stepCount = Mathf.CeilToInt(i_collectNumber*1f / count);


		partLabel.depth = 3;
        if (buildInfo == null)
            partLabel.transform.localPosition = Vector3.zero;
        else
        {
            if(buildInfo.buildUIManage!=null)
                partLabel.transform.position = buildInfo.buildUIManage.PopResource.position;
            if (buildInfo.buildUI != null)
                partLabel.transform.position = buildInfo.buildUI.PopResource.position;
        }
		
		partLabel.text = collectNumber.ToString();
		partLabel.color = new Color(partLabel.color.r,partLabel.color.g,partLabel.color.b,0f);
		partLabel.gameObject.SetActive(true);
		moveDistance = 0f;

		if(type==PartType.Gold)
		{
			AudioPlayer.Instance.PlaySfx("coins_collect_02");
		}
		else if(type==PartType.Wood)
		{
			AudioPlayer.Instance.PlaySfx("wood_collect_06");
		}
		else if(type==PartType.Stone)
		{
			AudioPlayer.Instance.PlaySfx("stone_collect_02");
		}
		else if(type==PartType.Iron)
		{
			AudioPlayer.Instance.PlaySfx("metal_collect_05");
		}
	}

	void Update()
	{
		if(isEmit&&emitcount<max)
		{
			for(int i=0;i<cpf&&emitcount<max;i++)
			{
				GameObject partItem = Instantiate(partItemPrefabs[type.ToString()]) as GameObject;
				Vector3 endPos = begin+new Vector3(Random.Range(minS,maxS),Random.Range(0,maxS),Random.Range(minS,maxS));
				partItem.transform.position = begin;
				partItem.transform.parent = transform;
				partItem.transform.localScale = Vector3.one;
				PartEmitItem item = partItem.GetComponent<PartEmitItem>();
				item.begin = begin;
				item.end = endPos;
				if(type==PartType.Gold)
					item.dest = GoldIco;
				else if(type==PartType.Wood)
					item.dest = WoodIco;
				else if(type==PartType.Stone)
					item.dest = StoneIco;
				else if(type==PartType.Iron)
					item.dest = IronIco;
				else if(type==PartType.Exp)
					item.dest = ExpIco;
				else if(type==PartType.Gem)
					item.dest = GemIco;
				item.emitter  = this;

				if(collectNumber>stepCount)
					item.resourceCount = stepCount;
				else
					item.resourceCount = collectNumber;

				collectNumber-=stepCount;
				parts.Add(item);
				emitcount++;
			}
		}
		else
		{
			isEmit = false;
		}

		if(partLabel.gameObject.activeSelf&&moveDistance<=maxMoveDistance)
		{

			if(moveDistance<=showDistance)
			{
				partLabel.color = new Color(partLabel.color.r,partLabel.color.g,partLabel.color.b,partLabel.color.a+moveSpeed/showDistance);
			}
			if(moveDistance>hideDistance)
			{

				partLabel.color = new Color(partLabel.color.r,partLabel.color.g,partLabel.color.b,partLabel.color.a-moveSpeed/(maxMoveDistance-hideDistance));
			}
			partLabel.transform.localPosition = new Vector3(partLabel.transform.localPosition.x,partLabel.transform.localPosition.y+moveSpeed,partLabel.transform.localPosition.z);
			moveDistance+=moveSpeed;
			if(moveDistance>=maxMoveDistance)
			{
				partLabel.gameObject.SetActive(false);
			}
		}

	}

	private UILabel partLabel;
	private float moveDistance;
	private float maxMoveDistance = 50f;
	private float moveSpeed = 1f;
	private float showDistance = 10f;
	private float hideDistance = 40f;

	public void NotifyPartReached(int resourceCount)
	{
		if(type==PartType.Gold)
		{
			AudioPlayer.Instance.PlaySfx("gold_ding_01");
		}
		else if(type==PartType.Wood)
		{
			AudioPlayer.Instance.PlaySfx("wood_ding_01");
		}
		else if(type==PartType.Stone)
		{
			AudioPlayer.Instance.PlaySfx("rock_ding_01");
		}
		else if(type==PartType.Iron)
		{
			AudioPlayer.Instance.PlaySfx("metal_ding_01");
		}

		if(resourceCount>0)
		{
			Helper.SetResourceCount(type.ToString(),resourceCount,false,true);
		}

		if (parts.Count == 0&&buildInfo!=null&&buildInfo.status==BuildStatus.Removaling) {
			buildInfo.OnCancelCreate();		
		}

		if(resourceShip!=null&&parts.Count==0)
		{
			resourceShip.state = 2; //开出码头;

		}

		if(parts.Count==0&&!partLabel.gameObject.activeSelf)
		{
			enabled = false;
		}

	}

}
