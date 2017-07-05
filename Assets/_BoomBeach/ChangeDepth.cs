using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeDepth : MonoBehaviour {
    
    public Image img0;
    public Image img1;
    public Button btn;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            MoveInHierarchy(img0.transform, -1);
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            MoveInHierarchy(img0.transform,1);

        }


    }

    void MoveInHierarchy(Transform trans,int delta)
    {
        int index = trans.GetSiblingIndex();
        trans.SetSiblingIndex(index + delta);
    }
}
