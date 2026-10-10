using System;
using UnityEngine;
using UnityEngine.UI;
using NRO.UI.ViewModels;

namespace NRO.UI.Views
{
    /// <summary>
    /// Giao dien uGUI Greybox cho Man hinh Cong Vao (Login & Server Select - S1).
    /// Tu dong sinh toan bo cay UI component hop le, khong phu thuoc file Prefab tren disk.
    /// </summary>
    public class GatewayView : MonoBehaviour
    {
        private GatewayViewModel _viewModel;

        private InputField _userInput;
        private InputField _passInput;
        private Toggle _rememberToggle;
        private Button _loginBtn;
        private Button _serverBtn;
        private Text _serverBtnText;
        private Text _statusText;

        public void Bind(GatewayViewModel vm)
        {
            _viewModel = vm;
            if (_userInput != null) _userInput.text = vm.Username;
            if (_passInput != null) _passInput.text = vm.Password;
            if (_rememberToggle != null) _rememberToggle.isOn = vm.RememberMe;
            UpdateServerText();

            _viewModel.ServerChangedEvent += _ => UpdateServerText();
        }

        private void Awake()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            var rt = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            // 1. Background che toan man hinh (Dark Dim)
            var bgImg = gameObject.AddComponent<Image>();
            bgImg.color = ThemeTokens.ColorDarkBg;

            // 2. Card trung tam (Greybox Box)
            var cardGo = new GameObject("LoginCard", typeof(RectTransform), typeof(Image));
            cardGo.transform.SetParent(transform, false);
            var cardRt = cardGo.GetComponent<RectTransform>();
            cardRt.sizeDelta = new Vector2(420, 460);
            cardRt.anchoredPosition = Vector2.zero;
            var cardImg = cardGo.GetComponent<Image>();
            cardImg.color = ThemeTokens.ColorCardBg;

            // 3. Tieu de Cổng Vào
            CreateText(cardGo.transform, "Title", "CỔNG THIÊN MÔN", 24, ThemeTokens.ColorSecondary, new Vector2(0, 180), new Vector2(380, 40));
            CreateText(cardGo.transform, "Subtitle", "Tu Tiên Chi Lộ — Vấn Đỉnh Tiên Phong", 12, ThemeTokens.ColorTextSecondary, new Vector2(0, 150), new Vector2(380, 25));

            // 4. Nut Chon May Chu
            var serverBtnGo = CreateButton(cardGo.transform, "ServerBtn", "Máy Chủ: Đang tải...", ThemeTokens.ColorCardBorder, new Vector2(0, 95), new Vector2(340, ThemeTokens.ButtonHeightStandard));
            _serverBtn = serverBtnGo.GetComponent<Button>();
            _serverBtnText = serverBtnGo.GetComponentInChildren<Text>();
            _serverBtn.onClick.AddListener(OnServerBtnClicked);

            // 5. Input Tai Khoan
            _userInput = CreateInputField(cardGo.transform, "UserInput", "Nhập tài khoản đạo hữu...", false, new Vector2(0, 35), new Vector2(340, ThemeTokens.InputFieldHeight));
            _userInput.onValueChanged.AddListener(val => { if (_viewModel != null) _viewModel.Username = val; });

            // 6. Input Mat Khau
            _passInput = CreateInputField(cardGo.transform, "PassInput", "Nhập mật khẩu...", true, new Vector2(0, -25), new Vector2(340, ThemeTokens.InputFieldHeight));
            _passInput.onValueChanged.AddListener(val => { if (_viewModel != null) _viewModel.Password = val; });

            // 7. Toggle Ghi Nho
            var toggleGo = new GameObject("RememberToggle", typeof(RectTransform), typeof(Toggle));
            toggleGo.transform.SetParent(cardGo.transform, false);
            var toggleRt = toggleGo.GetComponent<RectTransform>();
            toggleRt.anchoredPosition = new Vector2(-90, -75);
            toggleRt.sizeDelta = new Vector2(160, 25);
            _rememberToggle = toggleGo.GetComponent<Toggle>();
            _rememberToggle.onValueChanged.AddListener(val => { if (_viewModel != null) _viewModel.RememberMe = val; });
            CreateText(toggleGo.transform, "Label", "Ghi nhớ tài khoản", 12, ThemeTokens.ColorTextSecondary, Vector2.zero, new Vector2(160, 25));

            // 8. Nut Dang Nhap
            var loginBtnGo = CreateButton(cardGo.transform, "LoginBtn", "BƯỚC VÀO TIÊN GIỚI", ThemeTokens.ColorPrimary, new Vector2(0, -125), new Vector2(340, ThemeTokens.ButtonHeightLarge));
            _loginBtn = loginBtnGo.GetComponent<Button>();
            _loginBtn.onClick.AddListener(OnLoginBtnClicked);

            // 9. Status Text
            _statusText = CreateText(cardGo.transform, "StatusText", "", 12, ThemeTokens.ColorAccentRed, new Vector2(0, -180), new Vector2(340, 30));
        }

        private void OnLoginBtnClicked()
        {
            if (_viewModel != null)
            {
                _viewModel.SubmitLogin();
                _statusText.text = _viewModel.StatusMessage;
            }
        }

        private void OnServerBtnClicked()
        {
            if (_viewModel == null || _viewModel.Servers.Count == 0) return;

            // Xoay vong server don gian cho ban Greybox S1
            int nextIndex = (_viewModel.Servers.IndexOf(_viewModel.CurrentServer) + 1) % _viewModel.Servers.Count;
            _viewModel.SelectServer(nextIndex);
        }

        private void UpdateServerText()
        {
            if (_serverBtnText != null && _viewModel != null && _viewModel.CurrentServer != null)
            {
                _serverBtnText.text = $"Máy Chủ: {_viewModel.CurrentServer.Name}";
            }
        }

        // Cac helper sinh UI Runtime nhanh gon
        private Text CreateText(Transform parent, string name, string text, int size, Color color, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var txt = go.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = size;
            txt.color = color;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
            return txt;
        }

        private GameObject CreateButton(Transform parent, string name, string label, Color bgColor, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var img = go.GetComponent<Image>();
            img.color = bgColor;
            CreateText(go.transform, "Label", label, 14, Color.white, Vector2.zero, sizeDelta);
            return go;
        }

        private InputField CreateInputField(Transform parent, string name, string placeholder, bool isPass, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var img = go.GetComponent<Image>();
            img.color = new Color(0.06f, 0.05f, 0.09f, 0.9f);

            var input = go.GetComponent<InputField>();
            var text = CreateText(go.transform, "Text", "", 13, ThemeTokens.ColorTextPrimary, Vector2.zero, sizeDelta - new Vector2(20, 0));
            text.alignment = TextAnchor.MiddleLeft;

            var phText = CreateText(go.transform, "Placeholder", placeholder, 13, ThemeTokens.ColorTextMuted, Vector2.zero, sizeDelta - new Vector2(20, 0));
            phText.alignment = TextAnchor.MiddleLeft;

            input.textComponent = text;
            input.placeholder = phText;
            if (isPass) input.contentType = InputField.ContentType.Password;
            return input;
        }
    }
}
