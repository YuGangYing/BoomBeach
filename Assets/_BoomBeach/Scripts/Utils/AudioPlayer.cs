using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour {


	private static Dictionary<string,int> sfxPlayCounter;

	private static AudioPlayer instance;
	public static AudioPlayer Instance{
		get{
			return instance;
		}
	}

	public static void Init()
	{
		if(instance==null)
		{
			GameObject obj = GameObject.Instantiate(ResourceCache.load("Audio/AudioPlayer")) as GameObject;
			obj.name = "AudioPlayer";
			instance = obj.GetComponent<AudioPlayer>();
			instance.sfxPlayerList = new Dictionary<string, SfxPlayer>();
			SfxPlayer.TotalPlayingCount = 0;
			if(sfxPlayCounter==null)
			{
				sfxPlayCounter = new Dictionary<string,int>();
				sfxPlayCounter.Add("heavy_fire_01",3); //机枪兵射击声;
				sfxPlayCounter.Add("heavy_bullet_hit_01",3); //机枪兵子弹击中声;
				sfxPlayCounter.Add("coins_collect_02",3); //采集金币;
				sfxPlayCounter.Add("gold_ding_01",3);	//金币到达;
				sfxPlayCounter.Add("wood_collect_06",3); //木材采集;
				sfxPlayCounter.Add("wood_ding_01",3); //木材到达;
				sfxPlayCounter.Add("stone_collect_02",3); //石头采集;
				sfxPlayCounter.Add("rock_ding_01",3); //石头到达;
				sfxPlayCounter.Add("metal_collect_05",3); //金属采集;
				sfxPlayCounter.Add("metal_ding_01",3); //金属到达;
				sfxPlayCounter.Add("collect_diamonds_02",3); //宝石收集;
				sfxPlayCounter.Add("gem_ding_01",3); //宝石到达;
				sfxPlayCounter.Add("loot_fly_in_01",3); //战斗结束结果项飞行音效;
				sfxPlayCounter.Add("ammo_counter_01",3);  //获得弹药;
				sfxPlayCounter.Add("building_destroyed_01",3);//建筑爆炸;
				//以下是机枪兵死亡;
				sfxPlayCounter.Add("heavy_die_05",3);

				sfxPlayCounter.Add("tank_die_01",3); //坦克死亡;
				sfxPlayCounter.Add("tank_fire_01",3); //坦克开火;
				sfxPlayCounter.Add("tank_hit_01",3); //坦克击中;

				sfxPlayCounter.Add("assault_troop_shoot_01",3); //步枪射击;
				sfxPlayCounter.Add("assault_troop_bullet_hit_01",3); //步枪兵击中;
				sfxPlayCounter.Add("assault_troop_die_04",3); //步枪兵死亡;

				sfxPlayCounter.Add("bazooka_troop_fire_01",3); //导弹兵射击;
				sfxPlayCounter.Add("bazooka_hit_01",3);  //导弹兵击中;
				sfxPlayCounter.Add("bazooka_die_04",3); //导弹兵死亡;

				sfxPlayCounter.Add("native_attack_04",3); //战士攻击;
				sfxPlayCounter.Add("native_die_04",3); //战士死亡;

				sfxPlayCounter.Add("missile_hit_01",3); //导弹击中地面;
				sfxPlayCounter.Add("artillery_02",3);  //12发导弹的发射音效;

				sfxPlayCounter.Add("cannon",3); //火炮与药水发射声;
				sfxPlayCounter.Add("machinegun_attack_01",10); //机枪声;
				sfxPlayCounter.Add("rocket_launcher_fire_01",6); //火箭;
				sfxPlayCounter.Add("flame_thrower_01",6); //火焰喷射;


			}
			DontDestroyOnLoad(instance.gameObject);
			if(PlayerPrefs.GetInt("SoundEffectSwitch",0)==0)
			{
				instance.IsPlaySfx = false;
			}
			else
			{
				instance.IsPlaySfx = true;
			}

			if(PlayerPrefs.GetInt("MusicSwitch",0)==0)
			{
				instance.IsPlayMusic = false;
			}
			else
			{
				instance.IsPlayMusic = true;
			}
		}
	}

	private Dictionary<string,SfxPlayer> sfxPlayerList;
	private bool isPlayMusic;
	private bool isPlaySfx;


	public bool IsPlayMusic{
		set{
			if(value)
			{
				GetComponent<AudioSource>().Play();
			}
			else
			{
				GetComponent<AudioSource>().Pause();
			}
			isPlayMusic = value;
		}
		get{
			return isPlayMusic;
		}
	}

	public bool IsPlaySfx{
		set{
			isPlaySfx = value;
		}
		get{
			return isPlaySfx;
		}
	}




	public void PlaySfx(string name)
	{
		if(!isPlaySfx)return;
		SfxPlayer sfxPlayer = null;
		if(sfxPlayerList.ContainsKey(name))
		{
			sfxPlayer = sfxPlayerList[name];
			if(sfxPlayer==null)
			{
				sfxPlayer = gameObject.AddComponent<SfxPlayer>();
				sfxPlayerList[name] = sfxPlayer;
			}
		}
		else
		{
			sfxPlayer = gameObject.AddComponent<SfxPlayer>();
			sfxPlayerList.Add(name,sfxPlayer);
		}
		if(sfxPlayCounter.ContainsKey(name))
		sfxPlayer.MaxCount = sfxPlayCounter[name];

		//if(sfxPlayer.PlayingCount<=sfxPlayer.MaxCount&&SfxPlayer.TotalPlayingCount<=SfxPlayer.TotalMaxCount)
		if(sfxPlayer.PlayingCount<=sfxPlayer.MaxCount)
		{
			if(sfxPlayer.clip==null){
				//sfxPlayer.clip = ResourceCache.load("Audio/sfx/"+name) as AudioClip;
				if(BoomBeach.LocalSoundManager.SingleTon ().GetSound (name)!=null)
					sfxPlayer.clip = BoomBeach.LocalSoundManager.SingleTon ().GetSound (name).clip;
				else
					sfxPlayer.clip = ResourceCache.load("Audio/sfx/"+name) as AudioClip;
			}
			if (BoomBeach.LocalSoundManager.SingleTon ().IsPlayable (name)) {
				sfxPlayer.Play();
			}
		}
	}


	public void PlayMusic(string name,bool isLoop = true)
	{
		if(name!="")
		{
			AudioClip clip = ResourceCache.load("Audio/music/"+name) as AudioClip;
			GetComponent<AudioSource>().clip = clip;
			GetComponent<AudioSource>().loop = isLoop;
			if(!isPlayMusic)return;
			GetComponent<AudioSource>().Play();
		}
		else
		{
			GetComponent<AudioSource>().Stop();
		}
	}



}
