using UnityEngine;
using System.Collections;

namespace BoomBeach{

	//Login的UI和逻S辑要单独出来，不要去跟游戏框架耦合。
	//重新登录的时候不用拖泥带水，从根本上防止出现数据错误。
	public class LoginManager : MonoBehaviour {

		void Awake(){
			
		}

        void Start()
	    {
			UIManager.GetInstance ().AddCtroller<LoginCtrl> ();
			UIManager.GetInstance ().AddCtroller<NormalMsgCtrl> ();
			UIManager.GetInstance ().AddCtroller<PopMsgCtrl> ();
			UIManager.GetInstance ().AddCtroller<MaskCtrl> ();
	        StartCoroutine(_PlayMusic());
	    }

	    IEnumerator _PlayMusic()
	    {
	        yield return new WaitForSeconds(0.6f);
	        AudioPlayer.Instance.PlaySfx("supercell_jingle");
			yield return new WaitForSeconds(1f);
			SFSNetworkManager.Instance.ConnectToServer (string.Empty);
	    }

	}
}