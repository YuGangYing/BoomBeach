using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

    public float delay = 5;
    // Use this for initialization
    void Start () {
        DestroyObject(gameObject,delay);
    }
}
