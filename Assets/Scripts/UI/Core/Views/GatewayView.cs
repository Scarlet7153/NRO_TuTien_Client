using System;
using UnityEngine;
using UnityEngine.UI;
using NRO.UI.ViewModels;

namespace NRO.UI.Views
{
    /// <summary>
    /// View chính Cổng Thiên Môn (Gateway View Orchestrator):
    /// Điều phối Luồng Intro Khởi Động -> Tải Dữ Liệu Ban Đầu -> Form Đăng Nhập & Chọn Vũ Trụ -> Nạp Cảnh Thế Giới.
    /// </summary>
    public class GatewayView : MonoBehaviour
    {
        private GatewayViewModel _viewModel;
        private CanvasGroup _screenCanvasGroup;

        // Cụm Form Đăng Nhập Cổng Thiên Môn
        private GameObject _loginGroupGo;
        private CanvasGroup _cardCanvasGroup;
        private RectTransform _cardRt;
        private Outline _cardOutline;
        private InputField _userInput;
        private InputField _passInput;
        private Toggle _rememberToggle;
        private Text _serverBtnText;
        private Text _statusText;
        private Button _loginBtn;
        private Text _loginBtnText;

        // Cụm Footer
        private GameObject _footerGo;
        private CanvasGroup _footerCanvasGroup;

        // Các thành phần module con được phân tách
        private GatewayQiParticles _qiParticles;
        private GatewayIntroOverlay _introOverlay;
        private GatewayWorldLoadingScreen _worldLoadingScreen;
        private GatewaySettingsModal _settingsModal;

        // Biến trạng thái điều phối
        private float _pulseTimer;
        private float _animTimer;
        private float _timeoutTimer;
        private float _downloadCompleteDelay;
        private float _smoothDownloadFill = 0f;
        private float _downloadFillVelocity = 0f;
        private float _targetDisplayProgress = 0f;
        private bool _isInitialDataDownloading = false;
        private bool _isTransitioningOut = false;
        private bool _hasTriggeredLogin = false;

        // Các delegate kết nối với Client game logic
        public static Func<bool> IsStillInLoginPredicate;
        public static Func<bool> IsLoginSucceededPredicate;
        public static Func<bool> IsDataReadyPredicate;
        public static Func<float> GetDownloadProgress01Func;
        public static Func<int> GetDownloadProgressFunc;
        public static Action StartDownloadDataAction;
        public static Action ClearDataAction;
        public static Action QuitGameAction;
        public static bool IsAnimatingPortal = false;

        public static void ExecuteQuitGame()
        {
            try
            {
                if (QuitGameAction != null)
                {
                    QuitGameAction.Invoke();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[GatewayView] QuitGameAction exception: " + ex);
            }

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void Bind(GatewayViewModel vm)
        {
            _viewModel = vm;
            if (_userInput != null) _userInput.text = vm.Username;
            if (_passInput != null) _passInput.text = vm.Password;
            if (_rememberToggle != null) _rememberToggle.isOn = vm.RememberMe;
            UpdateServerText();
        }

        private void OnEnable()
        {
            EnsureLoginFormVisible();
        }

        public void EnsureLoginFormVisible()
        {
            _animTimer = 0f;
            _timeoutTimer = 0f;
            _isTransitioningOut = false;
            _hasTriggeredLogin = false;
            IsAnimatingPortal = false;

            _worldLoadingScreen?.Hide();

            // Nếu Intro hoặc Download dữ liệu lần đầu chưa xong thì không can thiệp
            if (_introOverlay != null && _introOverlay.IsPlaying)
            {
                return;
            }
            if (_isInitialDataDownloading)
            {
                return;
            }

            // Phục hồi hiển thị đầy đủ cho Form Đăng Nhập
            if (_cardCanvasGroup != null)
            {
                _cardCanvasGroup.alpha = 1f;
                _cardCanvasGroup.blocksRaycasts = true;
            }
            if (_cardRt != null)
            {
                _cardRt.anchoredPosition = new Vector2(270f, 0f);
            }

            // Phục hồi Footer
            if (_footerCanvasGroup != null)
            {
                _footerCanvasGroup.alpha = 1f;
            }

            // Phục hồi nút Cài Đặt bánh răng
            if (_settingsModal != null)
            {
                _settingsModal.SetButtonVisible(true);
                _settingsModal.CloseModal();
            }

            // Reset trạng thái hiển thị
            if (_viewModel != null)
            {
                _viewModel.IsLoading = false;
                _viewModel.StatusMessage = string.Empty;
                if (_userInput != null) _userInput.text = _viewModel.Username;
                if (_passInput != null) _passInput.text = _viewModel.Password;
                if (_rememberToggle != null) _rememberToggle.isOn = _viewModel.RememberMe;
            }

            SetLoginButtonState(false);

            if (_statusText != null)
            {
                _statusText.text = string.Empty;
            }
        }

        private void Awake()
        {
            // 0. Đảm bảo RectTransform của GatewayView luôn stretch toàn màn hình canvas
            var selfRt = GetComponent<RectTransform>();
            if (selfRt != null)
            {
                selfRt.anchorMin = Vector2.zero;
                selfRt.anchorMax = Vector2.one;
                selfRt.offsetMin = Vector2.zero;
                selfRt.offsetMax = Vector2.zero;
                selfRt.sizeDelta = Vector2.zero;
                selfRt.anchoredPosition = Vector2.zero;
                selfRt.pivot = new Vector2(0.5f, 0.5f);
            }

            BuildScreenCanvasGroup();
            BuildArtworkBackground();

            // Đảm bảo CanvasScaler render font chữ siêu nét (retina quality)
            var scaler = GetComponentInParent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.dynamicPixelsPerUnit = 3f;
                scaler.referencePixelsPerUnit = 100f;
            }

            // 1. Hệ thống hạt linh khí
            _qiParticles = new GatewayQiParticles();
            _qiParticles.Initialize(transform);

            // 2. Form đăng nhập chính & Footer
            BuildLoginPortalCard();
            CreateFooterInfo();

            // 3. Màn hình loading toàn màn hình (Tải dữ liệu ban đầu & Nạp cảnh thế giới)
            _worldLoadingScreen = new GatewayWorldLoadingScreen();
            _worldLoadingScreen.Initialize(transform);

            // 4. Modal cài đặt và xóa dữ liệu game
            _settingsModal = new GatewaySettingsModal();
            _settingsModal.Initialize(transform, OnConfirmExecuteClear);

            // 5. Cinematic Intro đầu game (nếu chưa từng phát)
            _introOverlay = new GatewayIntroOverlay();
            _introOverlay.Initialize(transform);

            if (_introOverlay.IsPlaying)
            {
                if (_cardCanvasGroup != null)
                {
                    _cardCanvasGroup.alpha = 0f;
                    _cardCanvasGroup.blocksRaycasts = false;
                }
                if (_footerCanvasGroup != null)
                {
                    _footerCanvasGroup.alpha = 0f;
                }
                _settingsModal?.SetButtonVisible(false);
            }
            else
            {
                if (_cardCanvasGroup != null)
                {
                    _cardCanvasGroup.alpha = 1f;
                    _cardCanvasGroup.blocksRaycasts = true;
                }
                if (_footerCanvasGroup != null)
                {
                    _footerCanvasGroup.alpha = 1f;
                }
                _settingsModal?.SetButtonVisible(true);
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Cập nhật đếm ngược thoát game nếu đang yêu cầu khởi động lại
            _settingsModal?.Update(dt);

            // 0. CINEMATIC STARTUP INTRO SEQUENCE
            if (_introOverlay != null && _introOverlay.IsPlaying)
            {
                _introOverlay.UpdateIntro(dt, OnIntroFinished);
                _qiParticles?.UpdateParticles(dt);
                return;
            }

            // 0.5 TẢI DỮ LIỆU BAN ĐẦU TỪ SERVER (NẾU CHƯA CÓ)
            if (_isInitialDataDownloading)
            {
                UpdateInitialDataDownload(dt);
                _qiParticles?.UpdateParticles(dt);
                return;
            }

            // Kiểm tra xem game đã chuyển sang màn hình tiếp theo (tạo nv, chọn nv, in-game) chưa
            bool isOutSideLogin = (IsStillInLoginPredicate != null && !IsStillInLoginPredicate());
            if (isOutSideLogin && !IsAnimatingPortal)
            {
                if (_viewModel == null || !_viewModel.IsLoading)
                {
                    _worldLoadingScreen?.Hide();
                    gameObject.SetActive(false);
                    return;
                }
            }

            // Hỗ trợ phím Tab chuyển đổi giữa các ô nhập liệu (Tab Index Navigation)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                HandleTabNavigation();
            }

            // Hỗ trợ phím Enter / KeypadEnter để đăng nhập trên PC
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_userInput != null && _userInput.isFocused && _passInput != null && string.IsNullOrEmpty(_passInput.text))
                {
                    _passInput.Select();
                    _passInput.ActivateInputField();
                }
                else if (_viewModel != null && !_viewModel.IsLoading)
                {
                    OnLoginBtnClicked();
                }
            }

            // Đồng bộ trạng thái từ ViewModel
            if (_viewModel != null)
            {
                if (_statusText != null)
                {
                    if (!_viewModel.IsLoading && !string.IsNullOrEmpty(_viewModel.StatusMessage))
                    {
                        _statusText.text = _viewModel.StatusMessage.StartsWith("⚠") ? _viewModel.StatusMessage : "⚠ " + _viewModel.StatusMessage;
                    }
                    else if (!_viewModel.IsLoading && string.IsNullOrEmpty(_viewModel.StatusMessage))
                    {
                        _statusText.text = string.Empty;
                    }
                }

                // Quản lý trạng thái Nạp Cảnh khi đăng nhập
                if (IsAnimatingPortal)
                {
                    _animTimer += dt;
                    _timeoutTimer += dt;

                    float loadDuration = 1.6f;
                    float t = Mathf.Clamp01(_animTimer / loadDuration);
                    float progress = Mathf.Sin(t * Mathf.PI * 0.5f);

                    if (t < 1f)
                    {
                        int p = Mathf.Clamp(Mathf.RoundToInt(progress * 99f), 0, 99);
                        _worldLoadingScreen?.UpdateProgress(
                            progress,
                            $"{p}%",
                            "✦ Đang kết nối linh mạch máy chủ Tiên Giới..."
                        );
                    }
                    else
                    {
                        _worldLoadingScreen?.UpdateProgress(
                            1f,
                            "100%",
                            "✦ Tiên Môn đã mở! Đang tiến vào thế giới... ✦"
                        );

                        // ĐÚNG 100% XONG MỚI GỬI LỆNH ĐĂNG NHẬP VÀO GAME
                        if (!_hasTriggeredLogin)
                        {
                            _hasTriggeredLogin = true;
                            _viewModel.SubmitLogin();
                        }

                        // Đợi server xác nhận đăng nhập thành công hoặc chuyển scene
                        bool isLoginSuccess = (IsLoginSucceededPredicate != null && IsLoginSucceededPredicate()) ||
                                              (IsStillInLoginPredicate != null && !IsStillInLoginPredicate());

                        if (isLoginSuccess && !_isTransitioningOut)
                        {
                            _isTransitioningOut = true;
                        }

                        if (_isTransitioningOut)
                        {
                            IsAnimatingPortal = false;
                            _worldLoadingScreen?.Hide();
                            gameObject.SetActive(false);
                            return;
                        }
                    }

                    if (_timeoutTimer >= 15f)
                    {
                        ShowLoginError("Không thể kết nối đến máy chủ. Vui lòng thử lại!");
                    }
                }
            }

            // Hiệu ứng thở viền kim quang trên Card Đăng Nhập
            _pulseTimer += dt * 1.6f;
            if (_cardOutline != null)
            {
                float pulseAlpha = 0.55f + 0.35f * Mathf.Sin(_pulseTimer);
                Color c = ThemeTokens.AncientGold;
                c.a = pulseAlpha;
                _cardOutline.effectColor = c;
            }

            // Cập nhật hạt linh khí
            _qiParticles?.UpdateParticles(dt);
        }

        private void OnIntroFinished()
        {
            _introOverlay?.SetActive(false);

            bool isDataReady = (IsDataReadyPredicate == null || IsDataReadyPredicate());
            if (!isDataReady)
            {
                // Chưa có dữ liệu -> Chuyển sang màn hình tải dữ liệu tài nguyên thực tế
                _isInitialDataDownloading = true;
                _downloadCompleteDelay = 0f;
                _smoothDownloadFill = 0f;
                _downloadFillVelocity = 0f;
                _targetDisplayProgress = 0f;

                if (_cardCanvasGroup != null)
                {
                    _cardCanvasGroup.alpha = 0f;
                    _cardCanvasGroup.blocksRaycasts = false;
                }
                if (_footerCanvasGroup != null) _footerCanvasGroup.alpha = 0f;
                _settingsModal?.SetButtonVisible(false);

                _worldLoadingScreen?.ShowDownloadLoading(
                    "ĐANG TẢI DỮ LIỆU TIÊN GIỚI",
                    "✦ Đang tải gói tài nguyên Tiên Giới... ✦"
                );

                StartDownloadDataAction?.Invoke();
            }
            else
            {
                _worldLoadingScreen?.Hide();
                if (_cardCanvasGroup != null)
                {
                    _cardCanvasGroup.alpha = 1f;
                    _cardCanvasGroup.blocksRaycasts = true;
                }
                if (_cardRt != null) _cardRt.anchoredPosition = new Vector2(270f, 0f);
                if (_footerCanvasGroup != null) _footerCanvasGroup.alpha = 1f;
                _settingsModal?.SetButtonVisible(true);
            }
        }

        private void UpdateInitialDataDownload(float dt)
        {
            float raw01 = 0f;
            if (GetDownloadProgress01Func != null)
            {
                raw01 = GetDownloadProgress01Func();
            }
            else if (GetDownloadProgressFunc != null)
            {
                raw01 = GetDownloadProgressFunc() / 100f;
            }
            raw01 = Mathf.Clamp01(raw01);

            bool isDataFinished = (IsDataReadyPredicate != null && IsDataReadyPredicate()) || (raw01 >= 0.999f);
            if (isDataFinished)
            {
                raw01 = 1f;
            }

            // Đảm bảo mục tiêu chỉ tiến tới, không bị giật lùi
            _targetDisplayProgress = Mathf.Max(_targetDisplayProgress, raw01);

            // Bổ sung micro-creep để duy trì cảm giác dòng chảy sống động
            // trong thời gian chờ nhận gói tin TCP kế tiếp (triệt tiêu cảm giác giật cục / khựng cứng)
            if (!isDataFinished && _targetDisplayProgress > 0.01f && _targetDisplayProgress < 0.98f)
            {
                _targetDisplayProgress = Mathf.Min(_targetDisplayProgress + 0.015f * dt, 0.98f);
            }

            // Dùng SmoothDamp giúp thanh tiến trình lướt cực êm ái, mượt mà chuẩn animation chuyên nghiệp
            if (_smoothDownloadFill < _targetDisplayProgress)
            {
                _smoothDownloadFill = Mathf.SmoothDamp(_smoothDownloadFill, _targetDisplayProgress, ref _downloadFillVelocity, 0.20f, 3.5f, dt);
            }
            else if (isDataFinished)
            {
                _smoothDownloadFill = Mathf.MoveTowards(_smoothDownloadFill, 1f, dt * 2.0f);
            }

            _smoothDownloadFill = Mathf.Clamp01(_smoothDownloadFill);
            int displayPercent = Mathf.Clamp(Mathf.RoundToInt(_smoothDownloadFill * 100f), 0, 100);

            string statusMsg = (displayPercent < 100)
                ? $"✦ Đang tải tài nguyên Tiên Giới ({displayPercent}%)... ✦"
                : "✦ Tải tài nguyên Tiên Giới hoàn tất! Đang khởi tạo Tiên Môn... ✦";

            _worldLoadingScreen?.UpdateProgress(_smoothDownloadFill, $"{displayPercent}%", statusMsg);

            if (isDataFinished && _smoothDownloadFill >= 0.99f)
            {
                _downloadCompleteDelay += dt;
                if (_downloadCompleteDelay >= 0.4f)
                {
                    // Fade-out loading screen
                    _worldLoadingScreen?.SetAlpha(Mathf.MoveTowards(_worldLoadingScreen.Alpha, 0f, dt * 2.5f));

                    // Fade-in form đăng nhập
                    if (_cardCanvasGroup != null)
                    {
                        _cardCanvasGroup.alpha = Mathf.MoveTowards(_cardCanvasGroup.alpha, 1f, dt * 2.5f);
                    }
                    if (_footerCanvasGroup != null)
                    {
                        _footerCanvasGroup.alpha = Mathf.MoveTowards(_footerCanvasGroup.alpha, 1f, dt * 2.5f);
                    }
                    if (_cardRt != null)
                    {
                        _cardRt.anchoredPosition = Vector2.MoveTowards(_cardRt.anchoredPosition, new Vector2(270f, 0f), dt * 360f);
                    }

                    if (_worldLoadingScreen != null && _worldLoadingScreen.Alpha <= 0.01f)
                    {
                        _isInitialDataDownloading = false;
                        _downloadCompleteDelay = 0f;
                        _worldLoadingScreen.Hide();

                        if (_cardCanvasGroup != null)
                        {
                            _cardCanvasGroup.alpha = 1f;
                            _cardCanvasGroup.blocksRaycasts = true;
                        }
                        if (_footerCanvasGroup != null) _footerCanvasGroup.alpha = 1f;
                        if (_cardRt != null) _cardRt.anchoredPosition = new Vector2(270f, 0f);
                        _settingsModal?.SetButtonVisible(true);
                    }
                }
            }
        }

        private void OnLoginBtnClicked()
        {
            if (_viewModel == null) return;

            // 1. Kiểm tra tính hợp lệ cơ bản của tài khoản / mật khẩu trước
            if (!_viewModel.ValidateCredentials(out string error))
            {
                ShowLoginError(error);
                return;
            }

            // 2. Ẩn form đăng nhập & footer
            if (_cardCanvasGroup != null)
            {
                _cardCanvasGroup.alpha = 0f;
                _cardCanvasGroup.blocksRaycasts = false;
            }
            if (_footerCanvasGroup != null) _footerCanvasGroup.alpha = 0f;
            _settingsModal?.SetButtonVisible(false);

            // 3. Bật màn hình nạp cảnh thế giới (CHƯA ĐĂNG NHẬP, ĐỢI LOAD XONG 100% MỚI GỬI ĐĂNG NHẬP VÀO GAME)
            _worldLoadingScreen?.ShowWorldLoading(
                "ĐANG TIẾN VÀO THẾ GIỚI TU TIÊN",
                "✦ Đang kết nối linh mạch máy chủ Tiên Giới..."
            );

            _animTimer = 0f;
            _timeoutTimer = 0f;
            _isTransitioningOut = false;
            _hasTriggeredLogin = false;
            IsAnimatingPortal = true;
        }

        private void SetLoginButtonState(bool isVerifying)
        {
            if (_loginBtn != null)
            {
                _loginBtn.interactable = !isVerifying;
            }
            if (_loginBtnText != null)
            {
                _loginBtnText.text = isVerifying ? "✦  ĐANG XÁC THỰC...  ✦" : "❖   TIẾN VÀO TIÊN MÔN   ❖";
            }
        }

        private void OnConfirmExecuteClear()
        {
            try
            {
                ClearDataAction?.Invoke();
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                if (_userInput != null) _userInput.text = string.Empty;
                if (_passInput != null) _passInput.text = string.Empty;
                if (_rememberToggle != null) _rememberToggle.isOn = false;

                if (_viewModel != null)
                {
                    _viewModel.Username = string.Empty;
                    _viewModel.Password = string.Empty;
                    _viewModel.RememberMe = false;
                }

                _settingsModal?.ShowRestartDialog();
            }
            catch (Exception ex)
            {
                _settingsModal?.SetStatus("Lỗi khi xóa dữ liệu: " + ex.Message, Color.red);
            }
        }

        private void OnServerBtnClicked()
        {
            if (_viewModel == null || _viewModel.Servers.Count == 0) return;
            int nextIndex = (_viewModel.Servers.IndexOf(_viewModel.CurrentServer) + 1) % _viewModel.Servers.Count;
            _viewModel.SelectServer(nextIndex);
        }

        private void UpdateServerText()
        {
            if (_serverBtnText != null && _viewModel != null && _viewModel.CurrentServer != null)
            {
                _serverBtnText.text = $"💎  Vũ Trụ: {_viewModel.CurrentServer.Name}    ✦ MƯỢT MÀ ✦  ▾";
            }
        }

        // ==================== CÁC PHƯƠNG THỨC XÂY DỰNG GIAO DIỆN CHÍNH ====================

        private void BuildScreenCanvasGroup()
        {
            _screenCanvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (_screenCanvasGroup == null)
            {
                _screenCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            _screenCanvasGroup.alpha = 1f;
        }

        private void BuildArtworkBackground()
        {
            var artGo = new GameObject("ArtworkBackground", typeof(RectTransform), typeof(Image));
            artGo.transform.SetParent(transform, false);
            var artRt = artGo.GetComponent<RectTransform>();
            artRt.anchorMin = Vector2.zero;
            artRt.anchorMax = Vector2.one;
            artRt.offsetMin = Vector2.zero;
            artRt.offsetMax = Vector2.zero;
            artRt.sizeDelta = Vector2.zero;

            var artImg = artGo.GetComponent<Image>();
            artImg.color = new Color(0.015f, 0.025f, 0.05f, 1f);
            GatewayUiHelper.LoadBackgroundArtwork(artImg);

            // Lớp phủ Gradient / Vignette
            var vigGo = new GameObject("BackgroundVignette", typeof(RectTransform), typeof(Image));
            vigGo.transform.SetParent(transform, false);
            var vigRt = vigGo.GetComponent<RectTransform>();
            vigRt.anchorMin = Vector2.zero;
            vigRt.anchorMax = Vector2.one;
            vigRt.offsetMin = Vector2.zero;
            vigRt.offsetMax = Vector2.zero;
            vigRt.sizeDelta = Vector2.zero;
            var vigImg = vigGo.GetComponent<Image>();
            vigImg.color = new Color(0.02f, 0.04f, 0.08f, 0.40f);
        }

        private void BuildLoginPortalCard()
        {
            // Container bọc toàn bộ Portal Aura và Card đăng nhập để quản lý Alpha đồng bộ
            _loginGroupGo = new GameObject("LoginPortalGroup", typeof(RectTransform), typeof(CanvasGroup));
            _loginGroupGo.transform.SetParent(transform, false);
            _cardRt = _loginGroupGo.GetComponent<RectTransform>();
            _cardRt.anchoredPosition = new Vector2(270f, 0f);
            _cardRt.sizeDelta = new Vector2(420f, 540f);
            _cardCanvasGroup = _loginGroupGo.GetComponent<CanvasGroup>();

            // 1. Vòng hào quang huyền bí sau Card
            var auraGo = new GameObject("PortalAura", typeof(RectTransform), typeof(Image));
            auraGo.transform.SetParent(_loginGroupGo.transform, false);
            var auraRt = auraGo.GetComponent<RectTransform>();
            auraRt.anchoredPosition = Vector2.zero;
            auraRt.sizeDelta = new Vector2(500f, 620f);
            var auraImg = auraGo.GetComponent<Image>();
            auraImg.sprite = ThemeTokens.CreateCircleSprite(128, new Color(0.08f, 0.82f, 0.72f, 0.15f));
            auraImg.raycastTarget = false;

            // 2. Khung Card Chính
            var cardGo = new GameObject("LoginCardBox", typeof(RectTransform), typeof(Image));
            cardGo.transform.SetParent(_loginGroupGo.transform, false);
            var cardBoxRt = cardGo.GetComponent<RectTransform>();
            cardBoxRt.anchorMin = Vector2.zero;
            cardBoxRt.anchorMax = Vector2.one;
            cardBoxRt.sizeDelta = Vector2.zero;
            cardBoxRt.anchoredPosition = Vector2.zero;

            var cardImg = cardGo.GetComponent<Image>();
            cardImg.sprite = ThemeTokens.GetXianxiaCardSprite();
            cardImg.type = Image.Type.Sliced;
            cardImg.color = Color.white;

            _cardOutline = cardGo.AddComponent<Outline>();
            _cardOutline.effectColor = ThemeTokens.AncientGold;
            _cardOutline.effectDistance = new Vector2(1.5f, -1.5f);

            var cardShadow = cardGo.AddComponent<Shadow>();
            cardShadow.effectColor = new Color(0f, 0f, 0f, 0.75f);
            cardShadow.effectDistance = new Vector2(8, -8);

            // Bốn góc họa tiết cổ phong
            GatewayUiHelper.CreateCornerOrnament(cardGo.transform, new Vector2(-192, 252));
            GatewayUiHelper.CreateCornerOrnament(cardGo.transform, new Vector2(192, 252));
            GatewayUiHelper.CreateCornerOrnament(cardGo.transform, new Vector2(-192, -252));
            GatewayUiHelper.CreateCornerOrnament(cardGo.transform, new Vector2(192, -252));

            // Huy hiệu Tiên Giới
            var crestGo = GatewayUiHelper.CreateText(cardGo.transform, "Crest", "☯", 26, ThemeTokens.CelestialCyan, new Vector2(0, 248), new Vector2(40, 40));
            GatewayUiHelper.AddOutlineAndShadow(crestGo.gameObject, Color.black, Color.clear);

            // Tiêu đề Cổng Thiên Môn
            var title = GatewayUiHelper.CreateText(cardGo.transform, "Title", "CỔNG THIÊN MÔN", 26, new Color(1f, 0.94f, 0.68f, 1f), new Vector2(0, 214), new Vector2(380, 36));
            title.fontStyle = FontStyle.Bold;
            GatewayUiHelper.Add3DGoldEffect(title.gameObject);

            var subtitle = GatewayUiHelper.CreateText(cardGo.transform, "Subtitle", "❖   NGỌC RỒNG TU TIÊN GIỚI   ❖", 12, ThemeTokens.AncientGoldBright, new Vector2(0, 186), new Vector2(380, 20));
            subtitle.fontStyle = FontStyle.Bold;
            GatewayUiHelper.AddOutlineAndShadow(subtitle.gameObject, Color.black, Color.clear);

            GatewayUiHelper.CreateText(cardGo.transform, "DividerTop", "◆━━━━━━━━━━━ ❖ ━━━━━━━━━━━◆", 11, ThemeTokens.AncientGold, new Vector2(0, 168), new Vector2(380, 16));

            // Hàng chọn Vũ Trụ / Máy Chủ
            CreateFieldLabel(cardGo.transform, "VŨ TRỤ LIÊN THÔNG", new Vector2(0, 146));
            var serverBtnGo = GatewayUiHelper.CreateCustomButton(cardGo.transform, "ServerSelector", "💎  Đang nạp máy chủ...  ▾", ThemeTokens.GetXianxiaServerBoxSprite(), ThemeTokens.AncientGoldBright, new Vector2(0, 116), new Vector2(350, 40), OnServerBtnClicked);
            _serverBtnText = serverBtnGo.GetComponentInChildren<Text>();
            _serverBtnText.alignment = TextAnchor.MiddleCenter;
            _serverBtnText.fontSize = 14;
            _serverBtnText.fontStyle = FontStyle.Bold;
            GatewayUiHelper.AddOutlineAndShadow(_serverBtnText.gameObject, Color.black, Color.clear);

            // Ô nhập Tài Khoản
            CreateFieldLabel(cardGo.transform, "ĐẠO HIỆU / TÀI KHOẢN", new Vector2(0, 78));
            _userInput = GatewayUiHelper.CreateCustomInputField(cardGo.transform, "UserInput", "👤", "Nhập đạo hiệu...", false, new Vector2(0, 48), new Vector2(350, 40));
            _userInput.onValueChanged.AddListener(v => { if (_viewModel != null) _viewModel.Username = v; });

            // Ô nhập Mật Khẩu
            CreateFieldLabel(cardGo.transform, "KHẨU QUYẾT / MẬT KHẨU", new Vector2(0, 10));
            _passInput = GatewayUiHelper.CreateCustomInputField(cardGo.transform, "PassInput", "🔒", "Nhập mật mã tu tiên...", true, new Vector2(0, -20), new Vector2(350, 40));
            _passInput.onValueChanged.AddListener(v => { if (_viewModel != null) _viewModel.Password = v; });

            // Thiết lập Tab Navigation giữa các ô nhập liệu
            var userNav = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = _passInput };
            _userInput.navigation = userNav;
            var passNav = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = _userInput };
            _passInput.navigation = passNav;

            // Ghi nhớ đạo hiệu
            var remGo = new GameObject("RememberMe", typeof(RectTransform));
            remGo.transform.SetParent(cardGo.transform, false);
            var remRt = remGo.GetComponent<RectTransform>();
            remRt.anchoredPosition = new Vector2(0, -58);
            remRt.sizeDelta = new Vector2(350, 24);

            _rememberToggle = remGo.AddComponent<Toggle>();
            var toggleBg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            toggleBg.transform.SetParent(remGo.transform, false);
            var bgRt = toggleBg.GetComponent<RectTransform>();
            bgRt.anchoredPosition = new Vector2(-160, 0);
            bgRt.sizeDelta = new Vector2(16, 16);
            var bgImg = toggleBg.GetComponent<Image>();
            bgImg.sprite = ThemeTokens.CreateRoundedBox(16, 3, new Color(0.04f, 0.08f, 0.12f, 0.9f), ThemeTokens.AncientGold, 1);

            var checkmark = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            checkmark.transform.SetParent(toggleBg.transform, false);
            var checkRt = checkmark.GetComponent<RectTransform>();
            checkRt.anchoredPosition = Vector2.zero;
            checkRt.sizeDelta = new Vector2(10, 10);
            var checkImg = checkmark.GetComponent<Image>();
            checkImg.sprite = ThemeTokens.CreateRoundedBox(10, 2, ThemeTokens.CelestialCyan, Color.clear, 0);

            _rememberToggle.targetGraphic = bgImg;
            _rememberToggle.graphic = checkImg;
            _rememberToggle.onValueChanged.AddListener(v => { if (_viewModel != null) _viewModel.RememberMe = v; });

            var remLabel = GatewayUiHelper.CreateText(remGo.transform, "Label", "Lưu đạo hiệu trên thiết bị này", 13, ThemeTokens.MoonlightSilver, new Vector2(-18, 0), new Vector2(250, 20));
            remLabel.alignment = TextAnchor.MiddleLeft;

            // Nút Đăng Nhập
            var loginBtnGo = GatewayUiHelper.CreateCustomButton(cardGo.transform, "LoginButton", "❖   TIẾN VÀO TIÊN MÔN   ❖", ThemeTokens.GetXianxiaButtonSprite(), new Color(0.12f, 0.08f, 0.02f, 1f), new Vector2(0, -100), new Vector2(350, 46), OnLoginBtnClicked);
            _loginBtn = loginBtnGo.GetComponent<Button>();
            _loginBtnText = loginBtnGo.GetComponentInChildren<Text>();

            // Dòng thông báo trạng thái
            _statusText = GatewayUiHelper.CreateText(cardGo.transform, "StatusText", "", 13, new Color(1f, 0.45f, 0.45f, 1f), new Vector2(0, -138), new Vector2(350, 22));
            GatewayUiHelper.AddOutlineAndShadow(_statusText.gameObject, Color.black, Color.clear);

            GatewayUiHelper.CreateText(cardGo.transform, "DividerBottom", "◆━━━━━━━━━━━ ❖ ━━━━━━━━━━━◆", 11, ThemeTokens.AncientGold, new Vector2(0, -158), new Vector2(380, 16));

            // Đăng Ký Tài Khoản
            var regBtnGo = new GameObject("RegisterPrompt", typeof(RectTransform), typeof(Button));
            regBtnGo.transform.SetParent(cardGo.transform, false);
            var regRt = regBtnGo.GetComponent<RectTransform>();
            regRt.anchoredPosition = new Vector2(0, -188);
            regRt.sizeDelta = new Vector2(350, 30);
            var regBtn = regBtnGo.GetComponent<Button>();
            regBtn.onClick.AddListener(() =>
            {
                if (_viewModel != null && !string.IsNullOrEmpty(_viewModel.RegisterUrl))
                {
                    Application.OpenURL(_viewModel.RegisterUrl);
                }
            });

            var regTxt = GatewayUiHelper.CreateText(regBtnGo.transform, "Label", "Chưa có đạo hiệu?  <u>Đăng Ký Tiên Môn Ngay</u>", 14, ThemeTokens.CelestialCyan, Vector2.zero, new Vector2(350, 30));
            GatewayUiHelper.AddOutlineAndShadow(regTxt.gameObject, Color.black, Color.clear);

            // Dòng chữ hỗ trợ
            var helpGo = GatewayUiHelper.CreateText(cardGo.transform, "HelpPrompt", "Gặp khó khăn khi nhập thế?  Liên hệ Chưởng Môn", 12, new Color(ThemeTokens.MoonlightSilver.r, ThemeTokens.MoonlightSilver.g, ThemeTokens.MoonlightSilver.b, 0.75f), new Vector2(0, -222), new Vector2(350, 20));
            GatewayUiHelper.AddOutlineAndShadow(helpGo.gameObject, Color.black, Color.clear);
        }

        private void CreateFieldLabel(Transform parent, string text, Vector2 pos)
        {
            var txt = GatewayUiHelper.CreateText(parent, "Label_" + text, text, 12, ThemeTokens.AncientGoldBright, pos, new Vector2(350, 16));
            txt.alignment = TextAnchor.MiddleLeft;
            txt.fontStyle = FontStyle.Bold;
            GatewayUiHelper.AddOutlineAndShadow(txt.gameObject, Color.black, Color.clear);
        }

        private void CreateFooterInfo()
        {
            _footerGo = new GameObject("FooterGroup", typeof(RectTransform), typeof(CanvasGroup));
            _footerGo.transform.SetParent(transform, false);
            var footerRt = _footerGo.GetComponent<RectTransform>();
            footerRt.anchorMin = Vector2.zero;
            footerRt.anchorMax = Vector2.one;
            footerRt.offsetMin = Vector2.zero;
            footerRt.offsetMax = Vector2.zero;
            footerRt.sizeDelta = Vector2.zero;
            _footerCanvasGroup = _footerGo.GetComponent<CanvasGroup>();

            // Góc dưới bên trái: Version
            var verGo = GatewayUiHelper.CreateText(_footerGo.transform, "ClientVersion", "Bản client: Tiên Giới V1.0.8 • 64-bit HD", 12, ThemeTokens.MoonlightSilver, Vector2.zero, new Vector2(320, 24));
            var verRt = verGo.GetComponent<RectTransform>();
            verRt.anchorMin = Vector2.zero;
            verRt.anchorMax = Vector2.zero;
            verRt.pivot = Vector2.zero;
            verRt.anchoredPosition = new Vector2(25f, 15f);
            verGo.alignment = TextAnchor.LowerLeft;
            GatewayUiHelper.AddOutlineAndShadow(verGo.gameObject, Color.black, Color.clear);

            // Góc dưới bên phải: Copyright
            var copyGo = GatewayUiHelper.CreateText(_footerGo.transform, "Copyright", "Ngọc Rồng Tu Tiên Giới • Vạn Đạo Quy Tông", 12, ThemeTokens.MoonlightSilver, Vector2.zero, new Vector2(340, 24));
            var copyRt = copyGo.GetComponent<RectTransform>();
            copyRt.anchorMin = new Vector2(1f, 0f);
            copyRt.anchorMax = new Vector2(1f, 0f);
            copyRt.pivot = new Vector2(1f, 0f);
            copyRt.anchoredPosition = new Vector2(-25f, 15f);
            copyGo.alignment = TextAnchor.LowerRight;
            GatewayUiHelper.AddOutlineAndShadow(copyGo.gameObject, Color.black, Color.clear);
        }

        private void HandleTabNavigation()
        {
            if (_userInput == null || _passInput == null) return;

            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (_userInput.isFocused)
            {
                _passInput.Select();
                _passInput.ActivateInputField();
            }
            else if (_passInput.isFocused)
            {
                _userInput.Select();
                _userInput.ActivateInputField();
            }
            else
            {
                // Nếu chưa focus ô nào, ưu tiên focus ô còn trống hoặc ô Tài Khoản
                if (string.IsNullOrEmpty(_userInput.text))
                {
                    _userInput.Select();
                    _userInput.ActivateInputField();
                }
                else
                {
                    _passInput.Select();
                    _passInput.ActivateInputField();
                }
            }
        }

        public void ShowLoginError(string errorMsg)
        {
            // 0. Bật lại nút đăng nhập
            SetLoginButtonState(false);

            // 1. Tắt màn hình loading thế giới ngay lập tức
            IsAnimatingPortal = false;
            _animTimer = 0f;
            _timeoutTimer = 0f;
            _isTransitioningOut = false;
            _hasTriggeredLogin = false;
            _worldLoadingScreen?.Hide();

            // 2. Phục hồi đầy đủ Form Đăng Nhập để người chơi thao tác
            if (_cardCanvasGroup != null)
            {
                _cardCanvasGroup.alpha = 1f;
                _cardCanvasGroup.blocksRaycasts = true;
            }
            if (_cardRt != null)
            {
                _cardRt.anchoredPosition = new Vector2(270f, 0f);
            }
            if (_footerCanvasGroup != null)
            {
                _footerCanvasGroup.alpha = 1f;
            }
            _settingsModal?.SetButtonVisible(true);

            // 3. Cập nhật thông điệp lỗi lên ViewModel và dòng chữ màu đỏ
            if (_viewModel != null)
            {
                _viewModel.IsLoading = false;
                _viewModel.StatusMessage = errorMsg;
            }

            if (_statusText != null)
            {
                _statusText.text = errorMsg.StartsWith("⚠") ? errorMsg : "⚠ " + errorMsg;
            }

            // 4. Hiển thị thông báo Toast nổi trên màn hình
            ToastService.Instance.ShowToast(errorMsg);
        }
    }
}
