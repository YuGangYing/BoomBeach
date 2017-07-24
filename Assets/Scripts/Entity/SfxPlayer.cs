using UnityEngine;
using System.Collections;

public class SfxPlayer:MonoBehaviour {

	public static int TotalMaxCount = 100;
	public static int TotalPlayingCount;

	private int playingCount;
	public int PlayingCount{
		get{
			return playingCount;
		}
	}
	private int maxCount = 1;
	public int MaxCount{
		get{
			return maxCount;
		}
		set{
			maxCount = value;
		}
	}
	public AudioClip clip;
	public void Play()
	{
		playingCount++;
		TotalPlayingCount++;
		//Invoke("StartPlay",Random.Range(0f,clip.length));
		StartPlay();
	}

	void StartPlay()
	{
		transform.Find("SfxAudio").GetComponent<AudioSource>().PlayOneShot(clip);
		Invoke("StopPlay",clip.length);
	}

	void StopPlay()
	{
		playingCount--;
		TotalPlayingCount--;
		if(playingCount==0)
		{
			Destroy(this);
		}
	}


}
