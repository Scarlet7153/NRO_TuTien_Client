using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI.Views
{
    /// <summary>
    /// Điều khiển đoạn Cinematic Startup Intro Sequence phong cách tiên hiệp khi khởi chạy game.
    /// </summary>
    public class GatewayIntroOverlay
    {
        public static bool HasPlayedStartupIntro = false;

        private GameObject _introOverlayGo;
        private CanvasGroup _introCanvasGroup;
        private CanvasGroup _introLogoCanvasGroup;
        private RectTransform _introLogoRt;
        private RectTransform _introSweepRt;
        private Text _introSkipPromptText;
        private Image _introFlashOverlay;

        private bool _isIntroPlaying = false;
        private bool _isIntroEnding = false;
        private float _introTimer = 0f;

        public bool IsPlaying => _isIntroPlaying;
        public bool IsEnding => _isIntroEnding;
        public float Alpha => _introCanvasGroup != null ? _introCanvasGroup.alpha : 0f;

        public void Initialize(Transform parent)
        {
            _introOverlayGo = new GameObject("StartupIntroOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            _introOverlayGo.transform.SetParent(parent, false);
            var rt = _introOverlayGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var bgImg = _introOverlayGo.GetComponent<Image>();
            bgImg.color = new Color(0.015f, 0.025f, 0.05f, 1f);

            _introCanvasGroup = _introOverlayGo.GetComponent<CanvasGroup>();
            _introCanvasGroup.alpha = 1f;
            _introCanvasGroup.blocksRaycasts = true;

            // 1. Cụm Logo Reveal 3D Hoàng Kim (Đặt ngay chính giữa màn hình)
            var logoGo = new GameObject("IntroLogoGroup", typeof(RectTransform), typeof(CanvasGroup));
            logoGo.transform.SetParent(_introOverlayGo.transform, false);
            _introLogoRt = logoGo.GetComponent<RectTransform>();
            _introLogoRt.anchoredPosition = new Vector2(0f, 15f);
            _introLogoRt.sizeDelta = new Vector2(650f, 220f);
            _introLogoCanvasGroup = logoGo.GetComponent<CanvasGroup>();
            _introLogoCanvasGroup.alpha = 0f;

            var subGo = GatewayUiHelper.CreateText(logoGo.transform, "IntroSubtitle", "❖   TIÊN ĐẠO TRƯỜNG SINH  ·  VẠN CỔ BẤT DIỆT   ❖", 12, ThemeTokens.AncientGoldBright, new Vector2(0, 65), new Vector2(550, 24));
            GatewayUiHelper.AddOutlineAndShadow(subGo.gameObject, Color.black, Color.clear);

            var mainTitleGo = GatewayUiHelper.CreateText(logoGo.transform, "IntroTitle", "NGỌC RỒNG TU TIÊN", 46, new Color(1f, 0.94f, 0.68f, 1f), new Vector2(0, 10), new Vector2(550, 58));
            mainTitleGo.fontStyle = FontStyle.Bold;
            GatewayUiHelper.Add3DGoldEffect(mainTitleGo.gameObject);

            // Vệt quét ánh kiếm hào quang (Light Sweep)
            var sweepGo = new GameObject("IntroLightSweep", typeof(RectTransform), typeof(Image));
            sweepGo.transform.SetParent(mainTitleGo.transform, false);
            _introSweepRt = sweepGo.GetComponent<RectTransform>();
            _introSweepRt.anchoredPosition = new Vector2(-280f, -20f);
            _introSweepRt.sizeDelta = new Vector2(160f, 3.5f);
            var sweepImg = sweepGo.GetComponent<Image>();
            sweepImg.color = new Color(1f, 0.95f, 0.75f, 0.45f);

            GatewayUiHelper.CreateText(logoGo.transform, "IntroDivider", "◆━━━━━━━━━━━━━━ ❖ ━━━━━━━━━━━━━━◆", 11, new Color(0.85f, 0.70f, 0.35f, 0.75f), new Vector2(0, -32), new Vector2(550, 18));

            var sloganGo = GatewayUiHelper.CreateText(logoGo.transform, "IntroSlogan", "◈   CHÍ TÔN ĐẠO MÔN  ·  PHỤC MA KHỞI NGUYÊN   ◈", 12, ThemeTokens.MoonlightSilver, new Vector2(0, -56), new Vector2(550, 22));
            GatewayUiHelper.AddOutlineAndShadow(sloganGo.gameObject, Color.black, Color.clear);

            // 2. Skip Prompt nhấp nháy êm ái
            _introSkipPromptText = GatewayUiHelper.CreateText(_introOverlayGo.transform, "SkipPrompt", "✦  Nhấn [Phím Bất Kỳ / Click Chuột] để vào Tiên Môn  ✦", 11, ThemeTokens.MoonlightSilver, new Vector2(0, -260), new Vector2(500, 22));
            GatewayUiHelper.AddOutlineAndShadow(_introSkipPromptText.gameObject, Color.black, Color.clear);

            // 3. Lớp chớp sáng điện ảnh (Cinematic Flash)
            var flashGo = new GameObject("IntroFlashOverlay", typeof(RectTransform), typeof(Image));
            flashGo.transform.SetParent(_introOverlayGo.transform, false);
            var flashRt = flashGo.GetComponent<RectTransform>();
            flashRt.anchorMin = Vector2.zero;
            flashRt.anchorMax = Vector2.one;
            flashRt.sizeDelta = Vector2.zero;
            _introFlashOverlay = flashGo.GetComponent<Image>();
            _introFlashOverlay.color = Color.clear;
            _introFlashOverlay.raycastTarget = false;

            _introOverlayGo.transform.SetAsLastSibling();

            if (!HasPlayedStartupIntro)
            {
                _isIntroPlaying = true;
                _isIntroEnding = false;
                _introTimer = 0f;
            }
            else
            {
                _introOverlayGo.SetActive(false);
                _isIntroPlaying = false;
            }
        }

        public void UpdateIntro(float dt, Action onIntroFinished)
        {
            if (!_isIntroEnding)
            {
                // Cho phép người chơi bỏ qua Intro bất cứ lúc nào
                if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                {
                    _isIntroEnding = true;
                }

                _introTimer += dt;

                // Phase 1 (0.0s - 1.2s): Chớp sáng nhẹ & Logo Reveal 3D từ từ hiển lộ
                if (_introTimer < 1.2f)
                {
                    float t1 = Mathf.Clamp01(_introTimer / 1.2f);
                    if (_introLogoCanvasGroup != null)
                    {
                        _introLogoCanvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t1);
                    }
                    if (_introLogoRt != null)
                    {
                        _introLogoRt.localScale = Vector3.Lerp(new Vector3(0.88f, 0.88f, 1f), Vector3.one, Mathf.SmoothStep(0f, 1f, t1));
                    }
                    if (_introSweepRt != null)
                    {
                        _introSweepRt.anchoredPosition = new Vector2(Mathf.Lerp(-280f, 280f, t1), -20f);
                    }

                    if (_introFlashOverlay != null && _introTimer >= 0.1f && _introTimer <= 0.8f)
                    {
                        float flashT = (_introTimer - 0.1f) / 0.7f;
                        float flashAlpha = Mathf.Sin(flashT * Mathf.PI) * 0.25f;
                        _introFlashOverlay.color = new Color(1f, 0.95f, 0.85f, flashAlpha);
                    }
                    else if (_introFlashOverlay != null)
                    {
                        _introFlashOverlay.color = Color.clear;
                    }
                }
                // Phase 2 (1.2s - 3.2s): Toàn cảnh tráng lệ, hào quang quét nhẹ
                else if (_introTimer < 3.2f)
                {
                    if (_introLogoCanvasGroup != null) _introLogoCanvasGroup.alpha = 1f;
                    if (_introLogoRt != null) _introLogoRt.localScale = Vector3.one;
                    if (_introFlashOverlay != null) _introFlashOverlay.color = Color.clear;

                    if (_introSweepRt != null)
                    {
                        float sweepT = Mathf.PingPong((_introTimer - 1.2f) * 0.85f, 1f);
                        _introSweepRt.anchoredPosition = new Vector2(Mathf.Lerp(-260f, 260f, sweepT), -20f);
                    }
                }
                // Phase 3 (>= 3.2s): Tự động chuyển cảnh kết thúc
                else
                {
                    _isIntroEnding = true;
                }

                // Gợi ý Skip nhấp nháy êm ái
                if (_introSkipPromptText != null)
                {
                    float promptAlpha = 0.35f + 0.45f * Mathf.PingPong(_introTimer * 1.6f, 1f);
                    var c = ThemeTokens.MoonlightSilver;
                    c.a = promptAlpha;
                    _introSkipPromptText.color = c;
                }
            }
            else
            {
                // Giai đoạn kết thúc: Fade-out Intro mượt mà
                if (_introCanvasGroup != null)
                {
                    _introCanvasGroup.alpha = Mathf.MoveTowards(_introCanvasGroup.alpha, 0f, dt * 2.5f);
                }

                if (_introCanvasGroup != null && _introCanvasGroup.alpha <= 0.01f)
                {
                    HasPlayedStartupIntro = true;
                    _isIntroPlaying = false;
                    _isIntroEnding = false;
                    if (_introOverlayGo != null)
                    {
                        _introOverlayGo.SetActive(false);
                    }
                    onIntroFinished?.Invoke();
                }
            }
        }

        public void SetActive(bool active)
        {
            if (_introOverlayGo != null)
            {
                _introOverlayGo.SetActive(active);
            }
        }
    }
}
