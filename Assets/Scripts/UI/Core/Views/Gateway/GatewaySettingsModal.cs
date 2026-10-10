using System;
using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI.Views
{
    /// <summary>
    /// Điều khiển nút bánh răng cài đặt, modal Cài đặt thiết bị và hộp thoại xác nhận xóa dữ liệu game an toàn.
    /// Khi xóa dữ liệu xong, hiển thị hộp thoại bắt buộc người chơi thoát game để tải lại tài nguyên mới.
    /// </summary>
    public class GatewaySettingsModal
    {
        private GameObject _settingBtnGo;
        private GameObject _settingModalGo;
        private CanvasGroup _settingModalCanvasGroup;
        private GameObject _cardGo;
        private GameObject _confirmClearDialogGo;
        private GameObject _restartDialogGo;
        private Text _restartCountdownText;
        private Text _settingStatusText;
        private Action _onConfirmClear;

        private bool _isRestarting;
        private float _restartTimeLeft;

        public void Initialize(Transform parent, Action onConfirmClear)
        {
            _onConfirmClear = onConfirmClear;

            // 1. Nút Cài đặt bánh răng phong cách Tiên Hiệp ở góc trên bên trái
            BuildSettingButton(parent);

            // 2. Modal Cài đặt toàn màn hình
            BuildSettingModal(parent);
        }

        private void BuildSettingButton(Transform parent)
        {
            _settingBtnGo = new GameObject("SettingBtn", typeof(RectTransform), typeof(Image), typeof(Button));
            _settingBtnGo.transform.SetParent(parent, false);

            var rt = _settingBtnGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(25f, -25f);
            rt.sizeDelta = new Vector2(44f, 44f);

            var img = _settingBtnGo.GetComponent<Image>();
            img.sprite = ThemeTokens.GetXianxiaGearSprite();
            img.type = Image.Type.Simple;
            img.preserveAspect = true;
            img.color = Color.white;

            var btn = _settingBtnGo.GetComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.25f, 1.2f, 1.1f, 1f);
            colors.pressedColor = new Color(0.80f, 0.75f, 0.65f, 1f);
            btn.colors = colors;
            btn.onClick.AddListener(OpenModal);

            _settingBtnGo.SetActive(false);
        }

        private void BuildSettingModal(Transform parent)
        {
            _settingModalGo = new GameObject("SettingModalOverlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            _settingModalGo.transform.SetParent(parent, false);

            var overlayRt = _settingModalGo.GetComponent<RectTransform>();
            overlayRt.anchorMin = Vector2.zero;
            overlayRt.anchorMax = Vector2.one;
            overlayRt.sizeDelta = Vector2.zero;

            var overlayImg = _settingModalGo.GetComponent<Image>();
            overlayImg.color = new Color(0.01f, 0.02f, 0.04f, 0.85f);

            _settingModalCanvasGroup = _settingModalGo.GetComponent<CanvasGroup>();
            _settingModalCanvasGroup.alpha = 0f;
            _settingModalCanvasGroup.blocksRaycasts = false;
            _settingModalGo.SetActive(false);

            // Khung Card Cài đặt
            _cardGo = new GameObject("SettingCard", typeof(RectTransform), typeof(Image));
            _cardGo.transform.SetParent(_settingModalGo.transform, false);
            var cardRt = _cardGo.GetComponent<RectTransform>();
            cardRt.anchoredPosition = Vector2.zero;
            cardRt.sizeDelta = new Vector2(440, 320);

            var cardImg = _cardGo.GetComponent<Image>();
            cardImg.sprite = ThemeTokens.GetXianxiaCardSprite();
            cardImg.type = Image.Type.Sliced;
            cardImg.color = Color.white;

            var cardOutline = _cardGo.AddComponent<Outline>();
            cardOutline.effectColor = new Color(0.85f, 0.70f, 0.35f, 0.8f);
            cardOutline.effectDistance = new Vector2(1.5f, -1.5f);

            // Bốn góc họa tiết cổ kính
            GatewayUiHelper.CreateCornerOrnament(_cardGo.transform, new Vector2(-202, 142));
            GatewayUiHelper.CreateCornerOrnament(_cardGo.transform, new Vector2(202, 142));
            GatewayUiHelper.CreateCornerOrnament(_cardGo.transform, new Vector2(-202, -142));
            GatewayUiHelper.CreateCornerOrnament(_cardGo.transform, new Vector2(202, -142));

            // Huy hiệu Tiên Giới
            var crestGo = GatewayUiHelper.CreateText(_cardGo.transform, "Crest", "☯", 22, ThemeTokens.CelestialCyan, new Vector2(0, 140), new Vector2(36, 36));
            GatewayUiHelper.AddOutlineAndShadow(crestGo.gameObject, Color.black, Color.clear);

            // Nút đóng (X)
            var closeBtnGo = new GameObject("CloseBtn", typeof(RectTransform), typeof(Button));
            closeBtnGo.transform.SetParent(_cardGo.transform, false);
            var closeRt = closeBtnGo.GetComponent<RectTransform>();
            closeRt.anchoredPosition = new Vector2(188f, 138f);
            closeRt.sizeDelta = new Vector2(34f, 34f);
            var closeBtn = closeBtnGo.GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseModal);
            var closeTxt = GatewayUiHelper.CreateText(closeBtnGo.transform, "Txt", "✕", 18, ThemeTokens.MoonlightSilver, Vector2.zero, new Vector2(34f, 34f));
            GatewayUiHelper.AddOutlineAndShadow(closeTxt.gameObject, Color.black, Color.clear);

            // Tiêu đề
            var title = GatewayUiHelper.CreateText(_cardGo.transform, "Title", "CÀI ĐẶT THIẾT BỊ", 22, new Color(1f, 0.94f, 0.68f, 1f), new Vector2(0, 106), new Vector2(380, 32));
            title.fontStyle = FontStyle.Bold;
            GatewayUiHelper.Add3DGoldEffect(title.gameObject);

            GatewayUiHelper.CreateText(_cardGo.transform, "Divider", "◆━━━━━━━━━━━ ❖ ━━━━━━━━━━━◆", 11, ThemeTokens.AncientGold, new Vector2(0, 80), new Vector2(380, 16));

            // Mô tả chức năng Xóa dữ liệu
            var descTxt = GatewayUiHelper.CreateText(_cardGo.transform, "Desc", "Dọn dẹp bộ nhớ đệm, xóa tài nguyên tải về và xóa thông tin tài khoản đã ghi nhớ trên máy.", 12, ThemeTokens.MoonlightSilver, new Vector2(0, 35), new Vector2(380, 42));
            descTxt.alignment = TextAnchor.MiddleCenter;

            // Nút Xóa Dữ Liệu
            var clearBtnGo = GatewayUiHelper.CreateCustomButton(_cardGo.transform, "ClearDataBtn", "🗑   XÓA TOÀN BỘ DỮ LIỆU GAME", ThemeTokens.GetXianxiaButtonSprite(), new Color(0.20f, 0.05f, 0.05f, 1f), new Vector2(0, -30), new Vector2(360, 48), OnClearDataBtnClicked);
            var clearBtnTxt = clearBtnGo.GetComponentInChildren<Text>();
            clearBtnTxt.fontSize = 14;
            clearBtnTxt.fontStyle = FontStyle.Bold;
            clearBtnTxt.color = new Color(0.95f, 0.20f, 0.20f, 1f);
            GatewayUiHelper.AddOutlineAndShadow(clearBtnTxt.gameObject, Color.black, Color.clear);

            // Dòng thông báo trạng thái
            _settingStatusText = GatewayUiHelper.CreateText(_cardGo.transform, "Status", "", 12, new Color(0.40f, 0.95f, 0.60f, 1f), new Vector2(0, -90), new Vector2(380, 28));
            GatewayUiHelper.AddOutlineAndShadow(_settingStatusText.gameObject, Color.black, Color.clear);

            // Hộp thoại con xác nhận
            BuildConfirmClearDialog(_cardGo.transform);

            // Hộp thoại thông báo yêu cầu thoát game sau khi xóa xong
            BuildRestartDialog(_settingModalGo.transform);
        }

        private void BuildConfirmClearDialog(Transform parent)
        {
            _confirmClearDialogGo = new GameObject("ConfirmClearDialog", typeof(RectTransform), typeof(Image));
            _confirmClearDialogGo.transform.SetParent(parent, false);
            var rt = _confirmClearDialogGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var img = _confirmClearDialogGo.GetComponent<Image>();
            img.sprite = ThemeTokens.GetXianxiaCardSprite();
            img.type = Image.Type.Sliced;
            img.color = new Color(0.12f, 0.06f, 0.06f, 0.98f);

            var outline = _confirmClearDialogGo.AddComponent<Outline>();
            outline.effectColor = new Color(0.95f, 0.35f, 0.35f, 0.9f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            var title = GatewayUiHelper.CreateText(_confirmClearDialogGo.transform, "WarnTitle", "⚠️   XÁC NHẬN XÓA DỮ LIỆU", 16, new Color(1f, 0.45f, 0.45f, 1f), new Vector2(0, 80), new Vector2(380, 28));
            title.fontStyle = FontStyle.Bold;
            GatewayUiHelper.AddOutlineAndShadow(title.gameObject, Color.black, Color.clear);

            var msg = GatewayUiHelper.CreateText(_confirmClearDialogGo.transform, "WarnMsg", "Toàn bộ tài nguyên game, bộ nhớ đệm và đạo hiệu đã ghi nhớ sẽ bị xóa sạch khỏi thiết bị!\nBạn có chắc chắn muốn thực hiện?", 12, ThemeTokens.MoonlightSilver, new Vector2(0, 20), new Vector2(380, 60));
            GatewayUiHelper.AddOutlineAndShadow(msg.gameObject, Color.black, Color.clear);

            // Nút Đồng ý
            GatewayUiHelper.CreateCustomButton(_confirmClearDialogGo.transform, "ConfirmBtn", "❖  ĐỒNG Ý XÓA", ThemeTokens.GetXianxiaButtonSprite(), new Color(0.95f, 0.20f, 0.20f, 1f), new Vector2(-95, -65), new Vector2(160, 42), OnConfirmExecute);

            // Nút Hủy
            GatewayUiHelper.CreateCustomButton(_confirmClearDialogGo.transform, "CancelBtn", "HỦY BỎ", ThemeTokens.GetXianxiaServerBoxSprite(), ThemeTokens.MoonlightSilver, new Vector2(95, -65), new Vector2(160, 42), () => _confirmClearDialogGo.SetActive(false));

            _confirmClearDialogGo.SetActive(false);
        }

        private void BuildRestartDialog(Transform parent)
        {
            _restartDialogGo = new GameObject("RestartPromptDialog", typeof(RectTransform), typeof(Image));
            _restartDialogGo.transform.SetParent(parent, false);
            var rt = _restartDialogGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(460, 280);

            var img = _restartDialogGo.GetComponent<Image>();
            img.sprite = ThemeTokens.GetXianxiaCardSprite();
            img.type = Image.Type.Sliced;
            img.color = new Color(0.08f, 0.09f, 0.12f, 0.98f);

            var outline = _restartDialogGo.AddComponent<Outline>();
            outline.effectColor = new Color(0.85f, 0.70f, 0.35f, 0.9f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            // Bốn góc họa tiết cổ kính
            GatewayUiHelper.CreateCornerOrnament(_restartDialogGo.transform, new Vector2(-212, 122));
            GatewayUiHelper.CreateCornerOrnament(_restartDialogGo.transform, new Vector2(212, 122));
            GatewayUiHelper.CreateCornerOrnament(_restartDialogGo.transform, new Vector2(-212, -122));
            GatewayUiHelper.CreateCornerOrnament(_restartDialogGo.transform, new Vector2(212, -122));

            // Huy hiệu Tiên Giới
            var crestGo = GatewayUiHelper.CreateText(_restartDialogGo.transform, "Crest", "✨", 22, ThemeTokens.CelestialCyan, new Vector2(0, 118), new Vector2(36, 36));
            GatewayUiHelper.AddOutlineAndShadow(crestGo.gameObject, Color.black, Color.clear);

            // Tiêu đề
            var title = GatewayUiHelper.CreateText(_restartDialogGo.transform, "Title", "ĐÃ DỌN SẠCH DỮ LIỆU", 20, new Color(1f, 0.94f, 0.68f, 1f), new Vector2(0, 84), new Vector2(420, 30));
            title.fontStyle = FontStyle.Bold;
            GatewayUiHelper.Add3DGoldEffect(title.gameObject);

            GatewayUiHelper.CreateText(_restartDialogGo.transform, "Divider", "◆━━━━━━━━━━━ ❖ ━━━━━━━━━━━◆", 11, ThemeTokens.AncientGold, new Vector2(0, 58), new Vector2(420, 16));

            // Mô tả
            var desc = GatewayUiHelper.CreateText(_restartDialogGo.transform, "Desc", "Toàn bộ tài nguyên và dữ liệu đệm đã được làm sạch hoàn toàn.\nĐạo hữu vui lòng thoát game và mở lại để hệ thống nạp lại tài nguyên mới nhất!", 12, ThemeTokens.MoonlightSilver, new Vector2(0, 15), new Vector2(420, 48));
            desc.alignment = TextAnchor.MiddleCenter;

            // Dòng chữ đếm ngược
            _restartCountdownText = GatewayUiHelper.CreateText(_restartDialogGo.transform, "Countdown", "Trò chơi sẽ tự động đóng sau 3 giây...", 13, new Color(0.40f, 0.95f, 0.60f, 1f), new Vector2(0, -32), new Vector2(420, 26));
            _restartCountdownText.fontStyle = FontStyle.Bold;
            GatewayUiHelper.AddOutlineAndShadow(_restartCountdownText.gameObject, Color.black, Color.clear);

            // Nút "THOÁT GAME NGAY"
            var quitBtnGo = GatewayUiHelper.CreateCustomButton(_restartDialogGo.transform, "QuitGameBtn", "❖   THOÁT GAME NGAY   ❖", ThemeTokens.GetXianxiaButtonSprite(), new Color(0.85f, 0.22f, 0.22f, 1f), new Vector2(0, -80), new Vector2(250, 44), () =>
            {
                _isRestarting = false;
                GatewayView.ExecuteQuitGame();
            });
            var quitBtnTxt = quitBtnGo.GetComponentInChildren<Text>();
            if (quitBtnTxt != null)
            {
                quitBtnTxt.fontSize = 14;
                quitBtnTxt.fontStyle = FontStyle.Bold;
                GatewayUiHelper.AddOutlineAndShadow(quitBtnTxt.gameObject, Color.black, Color.clear);
            }

            _restartDialogGo.SetActive(false);
        }

        public void OpenModal()
        {
            if (_isRestarting) return;
            if (_settingModalGo != null)
            {
                if (_cardGo != null) _cardGo.SetActive(true);
                if (_restartDialogGo != null) _restartDialogGo.SetActive(false);
                if (_confirmClearDialogGo != null) _confirmClearDialogGo.SetActive(false);
                if (_settingStatusText != null) _settingStatusText.text = string.Empty;
                _settingModalGo.SetActive(true);
                _settingModalGo.transform.SetAsLastSibling();
                _settingModalCanvasGroup.alpha = 1f;
                _settingModalCanvasGroup.blocksRaycasts = true;
            }
        }

        public void CloseModal()
        {
            if (_isRestarting) return;
            if (_settingModalGo != null)
            {
                if (_confirmClearDialogGo != null) _confirmClearDialogGo.SetActive(false);
                if (_restartDialogGo != null) _restartDialogGo.SetActive(false);
                _settingModalCanvasGroup.alpha = 0f;
                _settingModalCanvasGroup.blocksRaycasts = false;
                _settingModalGo.SetActive(false);
            }
        }

        private void OnClearDataBtnClicked()
        {
            if (_confirmClearDialogGo != null)
            {
                _confirmClearDialogGo.SetActive(true);
                _confirmClearDialogGo.transform.SetAsLastSibling();
            }
        }

        private void OnConfirmExecute()
        {
            if (_confirmClearDialogGo != null) _confirmClearDialogGo.SetActive(false);
            _onConfirmClear?.Invoke();
        }

        public void ShowRestartDialog()
        {
            if (_cardGo != null) _cardGo.SetActive(false);
            if (_confirmClearDialogGo != null) _confirmClearDialogGo.SetActive(false);
            if (_settingBtnGo != null) _settingBtnGo.SetActive(false);

            if (_settingModalGo != null)
            {
                _settingModalGo.SetActive(true);
                _settingModalGo.transform.SetAsLastSibling();
                if (_settingModalCanvasGroup != null)
                {
                    _settingModalCanvasGroup.alpha = 1f;
                    _settingModalCanvasGroup.blocksRaycasts = true;
                }
            }

            if (_restartDialogGo != null)
            {
                _restartDialogGo.SetActive(true);
                _restartDialogGo.transform.SetAsLastSibling();
            }

            _isRestarting = true;
            _restartTimeLeft = 3.5f;
            if (_restartCountdownText != null)
            {
                _restartCountdownText.text = "Trò chơi sẽ tự động đóng sau 3 giây...";
            }
        }

        public void Update(float dt)
        {
            if (_isRestarting)
            {
                _restartTimeLeft -= dt;
                int seconds = Mathf.Max(1, Mathf.CeilToInt(_restartTimeLeft));
                if (_restartCountdownText != null)
                {
                    _restartCountdownText.text = $"Trò chơi sẽ tự động đóng sau {seconds} giây...";
                }

                if (_restartTimeLeft <= 0f)
                {
                    _isRestarting = false;
                    GatewayView.ExecuteQuitGame();
                }
            }
        }

        public void SetStatus(string message, Color color)
        {
            if (_settingStatusText != null)
            {
                _settingStatusText.text = message;
                _settingStatusText.color = color;
            }
        }

        public void SetButtonVisible(bool visible)
        {
            if (_settingBtnGo != null)
            {
                _settingBtnGo.SetActive(visible);
            }
        }
    }
}
