using UnityEngine;
using System.Collections;

public class BuildBattleInfo : MonoBehaviour {

	public GameObject HealthObj;
    public UILabel HealthLabel;
    public UILabel HealthAddLabel;

    public GameObject DpsObj;
    public UILabel DpsLabel;
    public UILabel DpsAddLabel;

	public void Init()
	{
		HealthObj = transform.Find ("Health").gameObject;
		HealthLabel = HealthObj.transform.Find ("HealthVal").GetComponent<UILabel> ();
		HealthAddLabel = HealthObj.transform.Find ("HealthValAdd").GetComponent<UILabel> ();

		DpsObj = transform.Find ("Dps").gameObject;
		DpsLabel = DpsObj.transform.Find ("DpsVal").GetComponent<UILabel> ();
		DpsAddLabel = DpsObj.transform.Find ("DpsValAdd").GetComponent<UILabel> ();
	}

	public int Health{
		set{
			if(value>0)
			{
				HealthObj.SetActive(true);
			}
			else
			{
				HealthObj.SetActive(false);
			}
			HealthLabel.text = value.ToString();
		}
	}

	public int HealthAdd{
		set{
			if(value>0)
			{
				HealthAddLabel.gameObject.SetActive(true);
			}
			else
			{
				HealthAddLabel.gameObject.SetActive(false);
			}
			HealthAddLabel.text = "+"+value.ToString();
		}
	}


	public int Dps{
		set{
			if(value>0)
			{
				DpsObj.SetActive(true);
			}
			else
			{
				DpsObj.SetActive(false);
			}
			DpsLabel.text = value.ToString();
		}
	}
	
	public int DpsAdd{
		set{
			if(value>0)
			{
				DpsAddLabel.gameObject.SetActive(true);
			}
			else
			{
				DpsAddLabel.gameObject.SetActive(false);
			}
			DpsAddLabel.text = "+"+value.ToString();
		}
	}
}
