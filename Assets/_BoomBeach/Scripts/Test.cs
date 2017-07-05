using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.H)) {
            TextAsset buildings = Resources.Load<TextAsset>(@"csv/buildings");
            Helper.loadcsv(buildings, CSVManager.GetInstance().csvTable, "BUILDING", true, false);
        }
	}
}
