using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI.Views
{
    /// <summary>
    /// Màn hình Loading toàn màn hình với hiệu ứng tiên cảnh, thanh tiến độ và mẹo tu tiên.
    /// Dùng chung cho luồng Tải dữ liệu ban đầu và luồng Nạp cảnh khi đăng nhập.
    /// </summary>
    public class GatewayWorldLoadingScreen
    {
        private static readonly string[] XianxiaTips = new string[]
        {
            "💡 Mẹo Tu Tiên: Thường xuyên đả tọa luyện khí để gia tăng tốc độ tích lũy chân nguyên.",
            "💡 Mẹo Tu Tiên: Tham gia phụ bản Tông Môn cùng đạo hữu sẽ nhận được nhiều tiên thạch và đan dược quý.",
            "💡 Mẹo Tu Tiên: Đột phá cảnh giới cần chuẩn bị kỹ Phá Cảnh Đan để nâng cao tỷ lệ thành công.",
            "💡 Mẹo Tu Tiên: Kỳ ngộ thế gian ẩn chứa nhiều cơ duyên lớn, hãy chú ý các kỳ nhân dị sĩ trên bản đồ.",
            "💡 Mẹo Tu Tiên: Cường hóa pháp bảo gia tăng đáng kể linh lực và hộ thể chân khí khi giao chiến."
        };

        private GameObject _worldLoadingScreen;
        private CanvasGroup _worldLoadingCanvasGroup;
        private Image _progressBarFill;
        private Text _loadingTitleText;
        private Text _loadingStatusText;
        private Text _loadingPercentText;
        private Text _loadingTipText;

        public CanvasGroup CanvasGroup => _worldLoadingCanvasGroup;
        public GameObject GameObject => _worldLoadingScreen;
        public float Alpha => _worldLoadingCanvasGroup != null ? _worldLoadingCanvasGroup.alpha : 0f;

        public void Initialize(Transform parent)
        {
            _worldLoadingScreen = new GameObject("WorldLoadingScreen", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            _worldLoadingScreen.transform.SetParent(parent, false);
            var rt = _worldLoadingScreen.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var bgImg = _worldLoadingScreen.GetComponent<Image>();
            bgImg.color = new Color(0.015f, 0.025f, 0.045f, 1f);

            _worldLoadingCanvasGroup = _worldLoadingScreen.GetComponent<CanvasGroup>();
            _worldLoadingCanvasGroup.alpha = 0f;
            _worldLoadingCanvasGroup.blocksRaycasts = false;
            _worldLoadingScreen.transform.SetAsLastSibling();

            // Background Artwork Tiên Môn
            var artGo = new GameObject("WorldArtworkBg", typeof(RectTransform), typeof(Image));
            artGo.transform.SetParent(_worldLoadingScreen.transform, false);
            var artRt = artGo.GetComponent<RectTransform>();
            artRt.anchorMin = Vector2.zero;
            artRt.anchorMax = Vector2.one;
            artRt.sizeDelta = Vector2.zero;
            var artImg = artGo.GetComponent<Image>();
            artImg.color = new Color(0.40f, 0.40f, 0.45f, 0.28f);
            GatewayUiHelper.LoadBackgroundArtwork(artImg);

            // Lớp phủ Vignette tạo chiều sâu
            var vigGo = new GameObject("WorldVignette", typeof(RectTransform), typeof(Image));
            vigGo.transform.SetParent(_worldLoadingScreen.transform, false);
            var vigRt = vigGo.GetComponent<RectTransform>();
            vigRt.anchorMin = Vector2.zero;
            vigRt.anchorMax = Vector2.one;
            vigRt.sizeDelta = Vector2.zero;
            var vigImg = vigGo.GetComponent<Image>();
            vigImg.color = new Color(0.01f, 0.02f, 0.04f, 0.55f);

            // Container nội dung trung tâm
            var centerGo = new GameObject("WorldContent", typeof(RectTransform));
            centerGo.transform.SetParent(_worldLoadingScreen.transform, false);
            var centerRt = centerGo.GetComponent<RectTransform>();
            centerRt.anchoredPosition = Vector2.zero;
            centerRt.sizeDelta = new Vector2(700, 500);

            // 1. Tiêu đề
            var crownGo = GatewayUiHelper.CreateText(centerGo.transform, "WorldCrown", "◈ ─── ✦  ☯  ✦ ─── ◈", 13, ThemeTokens.CelestialCyan, new Vector2(0, 115f), new Vector2(500, 22));
            GatewayUiHelper.AddOutlineAndShadow(crownGo.gameObject, Color.black, Color.clear);

            _loadingTitleText = GatewayUiHelper.CreateText(centerGo.transform, "WorldTitle", "ĐANG TIẾN VÀO THẾ GIỚI TU TIÊN", 26, new Color(1f, 0.94f, 0.68f, 1f), new Vector2(0, 80f), new Vector2(560, 38));
            _loadingTitleText.fontStyle = FontStyle.Bold;
            GatewayUiHelper.Add3DGoldEffect(_loadingTitleText.gameObject);

            var loadSub = GatewayUiHelper.CreateText(centerGo.transform, "WorldSub", "❖   VẠN ĐẠO QUY TÔNG  ·  TIÊN MA HUYỄN CẢNH   ❖", 11, ThemeTokens.MoonlightSilver, new Vector2(0, 48f), new Vector2(560, 20));
            GatewayUiHelper.AddOutlineAndShadow(loadSub.gameObject, Color.black, Color.clear);

            GatewayUiHelper.CreateText(centerGo.transform, "WorldDivider", "◆━━━━━━━━━━━━━━━ ❖ ━━━━━━━━━━━━━━━◆", 11, new Color(0.85f, 0.70f, 0.35f, 0.75f), new Vector2(0, 26f), new Vector2(560, 18));

            // 2. Thanh Tải Tài Nguyên Thế Giới
            var barBgGo = new GameObject("WorldProgressBarBg", typeof(RectTransform), typeof(Image));
            barBgGo.transform.SetParent(centerGo.transform, false);
            var barBgRt = barBgGo.GetComponent<RectTransform>();
            barBgRt.anchoredPosition = new Vector2(0, -45f);
            barBgRt.sizeDelta = new Vector2(520, 18);
            var barBgImg = barBgGo.GetComponent<Image>();
            barBgImg.sprite = ThemeTokens.CreateRoundedBox(32, 6, new Color(0.04f, 0.07f, 0.12f, 0.98f), ThemeTokens.AncientGold, 1);
            barBgImg.type = Image.Type.Sliced;

            var barFillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            barFillGo.transform.SetParent(barBgGo.transform, false);
            var barFillRt = barFillGo.GetComponent<RectTransform>();
            barFillRt.anchorMin = new Vector2(0f, 0f);
            barFillRt.anchorMax = new Vector2(1f, 1f);
            barFillRt.sizeDelta = new Vector2(-4, -4);
            barFillRt.anchoredPosition = Vector2.zero;

            _progressBarFill = barFillGo.GetComponent<Image>();
            _progressBarFill.sprite = ThemeTokens.CreateRoundedBox(32, 4, ThemeTokens.CelestialCyan, Color.clear, 0);
            _progressBarFill.type = Image.Type.Filled;
            _progressBarFill.fillMethod = Image.FillMethod.Horizontal;
            _progressBarFill.fillAmount = 0f;

            // 3. Phần trăm hiển thị
            _loadingPercentText = GatewayUiHelper.CreateText(centerGo.transform, "PercentText", "0%", 13, ThemeTokens.AncientGoldBright, new Vector2(0, -78f), new Vector2(200, 20));
            _loadingPercentText.fontStyle = FontStyle.Bold;
            GatewayUiHelper.AddOutlineAndShadow(_loadingPercentText.gameObject, Color.black, Color.clear);

            // 4. Trạng thái
            _loadingStatusText = GatewayUiHelper.CreateText(centerGo.transform, "WorldStatus", "✦ Đang kết nối linh mạch máy chủ Tiên Giới...", 13, ThemeTokens.MoonlightSilver, new Vector2(0, -108f), new Vector2(560, 26));
            GatewayUiHelper.AddOutlineAndShadow(_loadingStatusText.gameObject, Color.black, Color.clear);

            // 5. Hộp Mẹo Tu Tiên
            var tipBoxGo = new GameObject("TipBox", typeof(RectTransform), typeof(Image));
            tipBoxGo.transform.SetParent(centerGo.transform, false);
            var tipBoxRt = tipBoxGo.GetComponent<RectTransform>();
            tipBoxRt.anchoredPosition = new Vector2(0, -195f);
            tipBoxRt.sizeDelta = new Vector2(620, 38);
            var tipBoxImg = tipBoxGo.GetComponent<Image>();
            tipBoxImg.sprite = ThemeTokens.CreateRoundedBox(32, 4, new Color(0.06f, 0.10f, 0.16f, 0.8f), new Color(ThemeTokens.AncientGold.r, ThemeTokens.AncientGold.g, ThemeTokens.AncientGold.b, 0.35f), 1);
            tipBoxImg.type = Image.Type.Sliced;

            _loadingTipText = GatewayUiHelper.CreateText(tipBoxGo.transform, "TipText", XianxiaTips[0], 11, ThemeTokens.MoonlightSilver, Vector2.zero, new Vector2(600, 32));
            GatewayUiHelper.AddOutlineAndShadow(_loadingTipText.gameObject, Color.black, Color.clear);
        }

        public void ShowWorldLoading(string title = "ĐANG TIẾN VÀO THẾ GIỚI TU TIÊN", string status = "✦ Đang kết nối linh mạch máy chủ Tiên Giới...")
        {
            if (_worldLoadingScreen != null)
            {
                _worldLoadingScreen.transform.SetAsLastSibling();
            }
            if (_worldLoadingCanvasGroup != null)
            {
                _worldLoadingCanvasGroup.alpha = 1f;
                _worldLoadingCanvasGroup.blocksRaycasts = true;
            }
            if (_loadingTitleText != null) _loadingTitleText.text = title;
            if (_loadingStatusText != null) _loadingStatusText.text = status;
            if (_progressBarFill != null) _progressBarFill.fillAmount = 0f;
            if (_loadingPercentText != null) _loadingPercentText.text = "0%";

            // Chọn ngẫu nhiên mẹo tu tiên
            if (_loadingTipText != null && XianxiaTips.Length > 0)
            {
                int tipIdx = UnityEngine.Random.Range(0, XianxiaTips.Length);
                _loadingTipText.text = XianxiaTips[tipIdx];
            }
        }

        public void ShowDownloadLoading(string title = "ĐANG TẢI DỮ LIỆU TIÊN GIỚI", string status = "✦ Đang tải gói tài nguyên Tiên Giới... ✦")
        {
            ShowWorldLoading(title, status);
        }

        public void UpdateProgress(float fill, string percentText, string statusText)
        {
            if (_progressBarFill != null) _progressBarFill.fillAmount = fill;
            if (_loadingPercentText != null) _loadingPercentText.text = percentText;
            if (_loadingStatusText != null && !string.IsNullOrEmpty(statusText)) _loadingStatusText.text = statusText;
        }

        public float FillAmount => _progressBarFill != null ? _progressBarFill.fillAmount : 0f;

        public void SetAlpha(float alpha)
        {
            if (_worldLoadingCanvasGroup != null)
            {
                _worldLoadingCanvasGroup.alpha = alpha;
                _worldLoadingCanvasGroup.blocksRaycasts = alpha > 0.05f;
            }
        }

        public void Hide()
        {
            SetAlpha(0f);
        }
    }
}
