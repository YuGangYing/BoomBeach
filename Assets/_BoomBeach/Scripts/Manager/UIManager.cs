using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BoomBeach
{
	public class UIManager : SingleMonoBehaviour<UIManager>
    {

		public Camera cameraUI;
        private Dictionary<UILayerType, GameObject> m_uiLayers = new Dictionary<UILayerType, GameObject>();
        private Dictionary<string, GameObject> m_panels = new Dictionary<string, GameObject>();
		private Dictionary<string,BaseCtrl> ctrls = new Dictionary<string, BaseCtrl>();

		protected override void Awake()
        {
			base.Awake ();
			InitLayers();
			InitCtrollers ();
        }

		void Start(){
			UIManager.GetInstance.GetController<MainInterfaceCtrl> ().ShowPanel ();
		}

        void InitCtrollers()
        {
			AddCtroller<LoginCtrl>();
            AddCtroller<MainInterfaceCtrl>();
            AddCtroller<MaskCtrl>();
            AddCtroller<PopMsgCtrl>();
            AddCtroller<PlayerListCtrl>();
            AddCtroller<SettingCtrl>();
            AddCtroller<ShopCtrl>();
            AddCtroller<TeamListCtrl>();
            AddCtroller<AchivementCtrl>();
            AddCtroller<BuildDetailCtrl>();
            AddCtroller<TroopTrainCtrl>();
            AddCtroller<TroopDetailCtrl>();
            AddCtroller<ResearchCtrl>();
            AddCtroller<ArtifactCtrl>();
            AddCtroller<NormalMsgCtrl>();
            AddCtroller<IslandPopCtrl>();
			AddCtroller<BattleInterfaceCtrl> ();
			AddCtroller<BattleResultCtrl> ();
        }

		public void ShowHome(){
		
		}








		public T AddCtroller<T>() where T : BaseCtrl
        {
			if (!ctrls.ContainsKey (typeof(T).FullName)) {
				T t = gameObject.AddComponent<T>();
				ctrls.Add (typeof(T).FullName,t);
				t.UIMgr = this;
				return t;
			}
			return null;
        }

		public T GetController<T>() where T:BaseCtrl{
			if(ctrls.ContainsKey(typeof(T).FullName)){
				return ctrls [typeof(T).FullName] as T;
			}
			return null;
		}


        public T GetPanel<T>(string panelName) where T : PanelBase
        {
            foreach (string str in m_panels.Keys)
            {
                Debug.Log(str);
                Debug.Log(m_panels[str]);
            }
            GameObject go;
            if (m_panels.TryGetValue(panelName, out go))
            {
                T t = go.GetComponent<T>();
                return t;
            }
            return null;
        }

        public void InitLayers()
        {
            GameObject tempLayer;
            Debug.Log("InitLayers");
            //底层
            tempLayer = AddALayer("UILayer_Bottom");
            m_uiLayers.Add(UILayerType.Bottom, tempLayer);

            //场景辅助层
            tempLayer = AddALayer("UILayer_MapSceneAbout", false);
            m_uiLayers.Add(UILayerType.MapSceneAbout, tempLayer);

            //固定面板层
            tempLayer = AddALayer("UILayer_Fixed");
            m_uiLayers.Add(UILayerType.Fixed, tempLayer);

            //普通面板层
            tempLayer = AddALayer("UILayer_Mask");
            m_uiLayers.Add(UILayerType.Mask, tempLayer);

            //普通面板层
            tempLayer = AddALayer("UILayer_Common");
            m_uiLayers.Add(UILayerType.Common, tempLayer);

            //普通面板层
            tempLayer = AddALayer("UILayer_Movie");
            m_uiLayers.Add(UILayerType.Movie, tempLayer);

            //普通上层面板层
            tempLayer = AddALayer("UILayer_FontCommon");
            m_uiLayers.Add(UILayerType.FontCommon, tempLayer);
            ////进度条层
            //tempLayer = AddALayer("UILayer_Loading");
            //m_uiLayers.Add(UILayerType.Loading, tempLayer);

            //UI特效层层
            tempLayer = AddALayer("UIEffectLayer", false);
            m_uiLayers.Add(UILayerType.UIEffect, tempLayer);

            //顶层
            tempLayer = AddALayer("UILayer_Top");
            m_uiLayers.Add(UILayerType.Top, tempLayer);
        }

        GameObject canvas = null;
        private GameObject AddALayer(string name, bool mouseEventable = true)
        {
            if(canvas==null) canvas = GameObject.FindWithTag("UICanvas");
            GameObject retLayer;
            retLayer = new GameObject(name);
            retLayer.layer = LayerMask.NameToLayer("UI");
            RectTransform rect = retLayer.AddComponent<RectTransform>();
            retLayer.transform.SetParent(canvas.transform);
            retLayer.transform.localPosition = Vector3.zero;
            //retLayer.transform.SetSiblingIndex(1000);
            retLayer.transform.localScale = Vector3.one;
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);

            retLayer.AddComponent<CanvasGroup>().blocksRaycasts = mouseEventable;

            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            return retLayer;
        }

        /// <summary>
        /// UI层类型
        /// </summary>
        public enum UILayerType
        {
            /// <summary>
            /// //最顶层
            /// </summary>
            Top,
            /// <summary>
            /// //UI特效层
            /// </summary>
            UIEffect,
            /// <summary>
            /// //普通上一层
            /// </summary>
            FontCommon,
            /// <summary>
            /// //任务、剧情
            /// </summary>
            Movie,
            /// <summary>
            /// //普通面板层
            /// </summary>
            Common,
            /// <summary>
            /// //固定面板层
            /// </summary>
            Fixed,
            /// <summary>
            /// //遮罩层
            /// </summary>
            Mask,
            /// <summary>
            /// 场景辅助层
            /// </summary>
            MapSceneAbout,
            /// <summary>
            /// //最底层
            /// </summary>
            Bottom
        }

        public void CloseAll()
        {
            foreach (string key in m_panels.Keys)
            {
                m_panels[key].SetActive(false);
            }
        }

        //单独显示panel
        public GameObject ShowPanel(UILayerType type, string panelStr, out bool isCreate)
        {
            GameObject panelGo = null;
            if (m_panels.ContainsKey(panelStr))
            {
                panelGo = m_panels[panelStr];
                isCreate = false;
            }
            else
            {
                GameObject prefab = ResourceManager.GetInstance.LoadLocalPanelPrefab(panelStr);
                panelGo = Instantiate<GameObject>(prefab);
                m_panels.Add(panelStr, panelGo);
				panelGo.transform.SetParent(m_uiLayers[type].transform);
                panelGo.transform.localPosition = Vector3.zero;
                panelGo.transform.localRotation = Quaternion.identity;
                panelGo.transform.localScale = Vector3.one;
                RectTransform rectTrans = panelGo.GetComponent<RectTransform>();
                if (rectTrans != null)
                {
                    rectTrans.sizeDelta = Vector2.zero;
                    rectTrans.anchoredPosition = Vector2.zero;
                }
                isCreate = true;
            }
            panelGo.SetActive(true);
            return panelGo;
        }
        //显示panel并绑定Mono
        public T ShowPanel<T>(UILayerType type, string panelStr, out bool isCreate) where T :MonoBehaviour
        {
            GameObject go = ShowPanel(type, panelStr, out isCreate);
            T t = go.GetComponent<T>();
            if (t ==null)
            {
                t = go.AddComponent<T>();
            }
            return t;
        }
        //显示panel并绑定Mono（根据Mono的名字来查找panel）
        public T ShowPanel<T>(UILayerType type, out bool isCreate) where T : PanelBase
        {
            string panelStr = GetPanelByCSharp<T>();
            GameObject go = ShowPanel(type, panelStr, out isCreate);
            T t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
                t.Awake();
            }
            return t;
        }

        //显示panel并绑定Mono（根据Mono的名字来查找panel）
        public T ShowPanel<T>(UILayerType type) where T : PanelBase
        {
            bool isCreate;
            string panelStr = GetPanelByCSharp<T>();
            GameObject go = ShowPanel(type, panelStr, out isCreate);
            T t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
                t.Awake();
            }
            return t;
        }



        public void ClosePanel(string panelStr)
        {
            if (m_panels.ContainsKey(panelStr))
            {
                m_panels[panelStr].SetActive(false);
            }
        }


        public bool IsActived(string panelStr)
        {
            if (m_panels.ContainsKey(panelStr))
            {
                if (m_panels[panelStr].activeInHierarchy) {
                    return true;
                }
            }
            return false;
        }

        //显示/隐藏Panel
        public void TogglePanel(UILayerType type, string panelStr)
        {
            bool isCreate;
            if (m_panels.ContainsKey(panelStr))
            {
                if (m_panels[panelStr].activeInHierarchy)
                {
                    ClosePanel(panelStr);
                }
                else
                {
                    ShowPanel(type, panelStr,out isCreate);
                }
            }
            else
            {
                ShowPanel(type, panelStr,out isCreate);
            }
        }
        //显示/隐藏Panel
        public void TogglePanel<T>(UILayerType type)
        {
            string panelStr = GetPanelByCSharp<T>();
            TogglePanel(type, panelStr);
        }
        //根据Mono的名字来查找panel
        string GetPanelByCSharp<T>()
        {
            string panelStr = typeof(T).ToString();
            if (panelStr.IndexOf(".") != -1)
                panelStr = panelStr.Remove(0, panelStr.LastIndexOf(".") + 1);
            panelStr = panelStr.Replace("View", "");
            return panelStr;
        }


    }
}