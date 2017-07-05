using UnityEngine;
using Sfs2X;
using Sfs2X.Requests;

// Statics for holding the connection to the SFS server end
// Can then be queried from the entire game to get the connection

public class SmartFoxConnection : MonoBehaviour
{
	private static SmartFoxConnection mInstance; 
	private static SmartFox smartFox;
	public static SmartFox Connection {
		get {
            if (mInstance == null) {
                mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
				smartFox = new SmartFox(true);
			}
            return smartFox;
        }
      set {
            if (mInstance == null) {
                mInstance = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            }
            smartFox = value;
        } 
	}

	public static bool IsInitialized {
		get { 
			return (smartFox != null); 
		}
	}
	
	void OnApplicationPause(bool pauseStatus) {
		
    }
	
	// Handle disconnection automagically
	// ** Important for Windows users - can cause crashes otherwise
    void OnApplicationQuit() { 
     	if (smartFox.IsConnected) {
			Debug.Log ("OnApplicationQuit");
         	smartFox.Disconnect();
            //Debug.Log("disconnect!");
       	}		
    } 
}