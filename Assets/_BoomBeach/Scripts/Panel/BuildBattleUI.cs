using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BoomBeach {
    [ExecuteInEditMode]
    public class BuildBattleUI : MonoBehaviour {


        public GameObject HealthObj;
        public Text HealthLabel;
        public Text HealthAddLabel;

        public GameObject DpsObj;
        public Text DpsLabel;
        public Text DpsAddLabel;

        public void Init()
        {
            HealthObj = transform.Find("Health").gameObject;
            HealthLabel = HealthObj.transform.Find("HealthVal").GetComponent<Text>();
            HealthAddLabel = HealthObj.transform.Find("HealthValAdd").GetComponent<Text>();

            DpsObj = transform.Find("Dps").gameObject;
            DpsLabel = DpsObj.transform.Find("DpsVal").GetComponent<Text>();
            DpsAddLabel = DpsObj.transform.Find("DpsValAdd").GetComponent<Text>();
        }

        public int Health
        {
            set
            {
                if (value > 0)
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

        public int HealthAdd
        {
            set
            {
                if (value > 0)
                {
                    HealthAddLabel.gameObject.SetActive(true);
                }
                else
                {
                    HealthAddLabel.gameObject.SetActive(false);
                }
                HealthAddLabel.text = "+" + value.ToString();
            }
        }


        public int Dps
        {
            set
            {
                if (value > 0)
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

        public int DpsAdd
        {
            set
            {
                if (value > 0)
                {
                    DpsAddLabel.gameObject.SetActive(true);
                }
                else
                {
                    DpsAddLabel.gameObject.SetActive(false);
                }
                DpsAddLabel.text = "+" + value.ToString();
            }
        }
    }
}
