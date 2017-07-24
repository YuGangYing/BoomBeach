using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemPartEmitter : MonoBehaviour {

	// Use this for initialization
	public Camera mc; //主相机(显示粒子的相机);
	public Camera uc; //UI相机(显示宝石目标UI的相机);
	public List<GemPartEmitItem> parts = new List<GemPartEmitItem>();
	private int max;
	private int emitcount; //已发射量;
	public int cpf;  //每帧生成的个数;
	public Vector3 begin;

	public Transform GemIco;
		
	private bool isEmit;  //是否发射粒子中;
	public bool isEmitting;  //是正在发射，并且未全部到达目的地;

	public GameObject GemPartPrefab;
	
	public Dictionary<string,GameObject> partItemPrefabs;
	public float minS = -2f;
	public float maxS = 2f;
	public float speed = 0.1f;
	private int stepCount;
	private int collectNumber;

    static GemPartEmitter instance;
    public static GemPartEmitter Instance()
    {
        return instance;
    }

    void Awake()
	{
        instance = this;
        partItemPrefabs = new Dictionary<string,GameObject> ();
	}
	
	public void Emit(int count,Vector3 i_begin,int i_collectNumber)
	{
		isEmit = true;
		isEmitting = true;
		begin = i_begin;
		max = count;
		emitcount = 0;
		collectNumber = i_collectNumber;
		parts = new List<GemPartEmitItem>();
		stepCount = Mathf.CeilToInt(i_collectNumber*1f / count);
		AudioPlayer.Instance.PlaySfx("collect_diamonds_02");
	}
	
	void Update()
	{
		if(isEmit&&emitcount<max)
		{
			for(int i=0;i<cpf&&emitcount<max;i++)
			{
				GameObject partItem = Instantiate(GemPartPrefab) as GameObject;
				Vector3 endPos = begin+new Vector3(Random.Range(minS,maxS),Random.Range(0,maxS),Random.Range(minS,maxS));
				partItem.transform.position = begin;
				partItem.transform.parent = transform;
				partItem.transform.localScale = Vector3.one;
				GemPartEmitItem item = partItem.GetComponent<GemPartEmitItem>();
				item.begin = begin;
				item.end = endPos;
				item.dest = GemIco;
				item.emitter  = this;
				item.mc = mc;
				item.uc = uc;
				item.speed = speed;
				
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
	}
	
	public void NotifyPartReached(int resourceCount)
	{
		AudioPlayer.Instance.PlaySfx("gem_ding_01");
		if(resourceCount>0)
		{
			Helper.SetResourceCount("Gems",resourceCount,false,true);
		}
				
	}
}
