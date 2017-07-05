using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunBoatController : MonoBehaviour {

	public GunBoat gunBoat;
	
	// Update is called once per frame
	void Update () {
		if(gunBoat.fireList.Count > 0)
		{
			FireItem fi = gunBoat.fireList.Dequeue();

			SpellInfo.Fire(fi.btd,gunBoat.FirePoint.position,fi.attackPoint);

			if(fi.btd.tid=="TID_BARRAGE")
				AudioPlayer.Instance.PlaySfx("artillery_02");
			else
				AudioPlayer.Instance.PlaySfx("cannon");
		}
	}
}
