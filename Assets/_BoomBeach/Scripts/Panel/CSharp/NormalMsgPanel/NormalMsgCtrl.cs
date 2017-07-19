using UnityEngine;
using System.Collections;
using BoomBeach;
using UnityEngine.Events;
using UnityEngine.UI;

//普通全局提示
public class NormalMsgCtrl : BaseCtrl
{
    NormalMsgPanelView mNormalPanelView;
    GameObject mItemPrefab;

    public void ShowPop(string msg)
    {
		Debug.Log (msg);
		ShowPanel();
        GameObject go = Instantiate<GameObject>(mItemPrefab);
        go.SetActive(true);
		go.transform.SetParent(mNormalPanelView.m_containerPoppoint.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.GetComponent<Text>().text = msg;
        go.transform.SetSiblingIndex(0);
    }

	public override void ShowPanel()
    {
        bool isCreate;
        mNormalPanelView = UIMgr.ShowPanel<NormalMsgPanelView>(UIManager.UILayerType.Top, out isCreate);
        if (isCreate)
        {
            OnCreatePanel();
        }
    }

    void OnCreatePanel()
    {
        mItemPrefab = mNormalPanelView.m_containerPoppoint.transform.Find("item").gameObject;
       
    }

    public override void Close()
    {
        
    }

    public void CloseMask()
    {
       
    }

}
