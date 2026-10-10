using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace NRO.UI
{
    /// <summary>
    /// Component goc quan ly Canvas va EventSystem cho he thong giao dien uGUI (D1.3).
    /// Tu dong thiet lap CanvasScaler voi do phan giai chuan 1280x720, khong can keo tha thu cong tren Editor.
    /// </summary>
    public class UiRoot : MonoBehaviour
    {
        private static UiRoot _instance;
        public static UiRoot Instance => _instance;

        public Canvas RootCanvas { get; private set; }
        public CanvasScaler Scaler { get; private set; }
        public RectTransform ScreensContainer { get; private set; }
        public RectTransform ModalsContainer { get; private set; }
        public RectTransform ToastsContainer { get; private set; }
        private bool _isInitialized;

        public static void EnsureInitialized()
        {
            if (_instance != null && _instance._isInitialized)
            {
                return;
            }

            var existing = FindObjectOfType<UiRoot>();
            if (existing != null)
            {
                _instance = existing;
                _instance.InitializeImmediate();
                return;
            }

            var rootGo = new GameObject("[UiRoot]", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(UiRoot));
            _instance = rootGo.GetComponent<UiRoot>();
            _instance.InitializeImmediate();
            DontDestroyOnLoad(rootGo);
        }

        private void Awake()
        {
            InitializeImmediate();
        }

        public void InitializeImmediate()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;
            _instance = this;
            DontDestroyOnLoad(gameObject);

            SetupCanvas();
            SetupEventSystem();
            SetupContainers();
        }

        private void SetupCanvas()
        {
            RootCanvas = gameObject.GetComponent<Canvas>() ?? gameObject.AddComponent<Canvas>();
            RootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            RootCanvas.sortingOrder = 100;

            Scaler = gameObject.GetComponent<CanvasScaler>() ?? gameObject.AddComponent<CanvasScaler>();
            Scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            Scaler.referenceResolution = new Vector2(1280, 720);
            Scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            Scaler.matchWidthOrHeight = 0.5f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void SetupEventSystem()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.transform.SetParent(transform);
                esGo.AddComponent<EventSystem>();
                esGo.AddComponent<StandaloneInputModule>();
            }
        }

        private void SetupContainers()
        {
            ScreensContainer = CreateContainer("ScreensContainer", 10);
            ModalsContainer = CreateContainer("ModalsContainer", 50);
            ToastsContainer = CreateContainer("ToastsContainer", 90);
        }

        private RectTransform CreateContainer(string name, int siblingIndex)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.SetSiblingIndex(siblingIndex);
            return rt;
        }
    }
}
