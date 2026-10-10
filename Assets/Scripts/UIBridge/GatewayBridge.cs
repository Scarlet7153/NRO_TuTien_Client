using System;
using System.Collections.Generic;
using UnityEngine;
using NRO.UI;
using NRO.UI.ViewModels;
using NRO.UI.Views;

namespace NRO.UIBridge
{
    /// <summary>
    /// Cau noi giua GatewayView/ViewModel (S1) voi he thong mang va logic cu (LoginScr, ServerListScreen, Service).
    /// </summary>
    public static class GatewayBridge
    {
        private static GatewayViewModel _viewModel;
        private static GameObject _viewInstance;

        public static bool IsLoginSucceeded = false;

        public static GatewayViewModel ViewModel => _viewModel ?? (_viewModel = CreateViewModel());

        public static void NotifyLoginSuccess()
        {
            IsLoginSucceeded = true;
            if (_viewModel != null)
            {
                _viewModel.StatusMessage = "✦ Đã vượt Thiên Môn, đang tiến vào Tiên Giới... ✦";
            }
        }

        public static bool ShouldInterceptOnGUI()
        {
            if (!UiFlags.EnableNewLogin)
            {
                if (_viewInstance != null && _viewInstance.activeSelf)
                {
                    _viewInstance.SetActive(false);
                }
                return false;
            }

            // Neu dang o man hinh Login hoac ServerList HOAC dang dien hoat animation chuyen canh vao game
            bool isInLogin = GameCanvas.currentScreen == GameCanvas.loginScr ||
                             (GameCanvas.serverScreen != null && GameCanvas.currentScreen == GameCanvas.serverScreen);
            bool isAnimating = GatewayView.IsAnimatingPortal;

            if (isInLogin || isAnimating)
            {
                if (isInLogin && !isAnimating)
                {
                    IsLoginSucceeded = false;
                }

                EnsureViewShown();

                // Dong bo phan hoi tu he thong cu sang ViewModel
                if (_viewModel != null)
                {
                    if (GameCanvas.currentDialog == GameCanvas.msgdlg && GameCanvas.msgdlg != null && GameCanvas.msgdlg.info != null && GameCanvas.msgdlg.info.Length > 0)
                    {
                        string msg = string.Join(" ", GameCanvas.msgdlg.info).Trim();
                        string lower = msg.ToLower();

                        // Chi coi la strict waiting neu la dialog mac dinh cua engine khong co so giay
                        bool isStrictWaiting = (msg == "Xin chờ" || msg == "Đang kết nối..." || lower == "xin chờ" || lower == "please wait" || lower.StartsWith("đang kết nối"));

                        if (isStrictWaiting)
                        {
                            // Dang trong tien trinh ket noi -> giu nguyen trang thai loading
                            // Khong set StatusMessage de tranh hien chu do "Xin cho" duoi form
                            if (string.IsNullOrEmpty(_viewModel.StatusMessage) || _viewModel.StatusMessage.Contains("chờ"))
                            {
                                _viewModel.StatusMessage = string.Empty;
                            }
                        }
                        else
                        {
                            // Day la thong bao loi/tu choi tu server (vi du: "Vui long cho 2s", "Sai mat khau"...)
                            _viewModel.StatusMessage = msg;
                            _viewModel.IsLoading = false;
                            GatewayView.IsAnimatingPortal = false;
                            IsLoginSucceeded = false;
                            GameCanvas.endDlg();

                            if (_viewInstance != null)
                            {
                                var view = _viewInstance.GetComponent<GatewayView>();
                                view?.ShowLoginError(msg);
                            }
                        }
                    }
                    else if (Controller.isConnectionFail)
                    {
                        string failMsg = "Không thể kết nối đến máy chủ Tiên Giới!";
                        _viewModel.StatusMessage = failMsg;
                        _viewModel.IsLoading = false;
                        GatewayView.IsAnimatingPortal = false;
                        IsLoginSucceeded = false;
                        GameCanvas.endDlg();

                        if (_viewInstance != null)
                        {
                            var view = _viewInstance.GetComponent<GatewayView>();
                            view?.ShowLoginError(failMsg);
                        }
                    }
                }

                return true;
            }

            if (_viewModel != null)
            {
                _viewModel.IsLoading = false;
            }

            if (_viewInstance != null && _viewInstance.activeSelf)
            {
                _viewInstance.SetActive(false);
            }
            return false;
        }

        private static void EnsureViewShown()
        {
            GatewayView.IsStillInLoginPredicate = () => GameCanvas.currentScreen == GameCanvas.loginScr ||
                                                        (GameCanvas.serverScreen != null && GameCanvas.currentScreen == GameCanvas.serverScreen);
            GatewayView.IsLoginSucceededPredicate = () => IsLoginSucceeded;
            GatewayView.ClearDataAction = () =>
            {
                Rms.clearAll();
                Rms.clearRMS();
                Rms.saveRMSString("acc", string.Empty);
                Rms.saveRMSString("pass", string.Empty);
                ServerListScreen.bigOk = false;
                ServerListScreen.loadScreen = false;
                ServerListScreen.isGetData = false;
                ServerListScreen.percent = 0;
                ServerListScreen.demPercent = 0;
            };

            GatewayView.QuitGameAction = () =>
            {
                try
                {
                    Main.exit();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("[GatewayBridge] Exception calling Main.exit(): " + ex);
                }

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            };

            GatewayView.IsDataReadyPredicate = () =>
            {
                try
                {
                    string path = System.IO.Path.Combine(Rms.GetiPhoneDocumentsPath(), "ResVersion");
                    if (!System.IO.File.Exists(path))
                    {
                        return false;
                    }
                    if (Controller.isLoadingData)
                    {
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            };

            GatewayView.GetDownloadProgress01Func = () =>
            {
                if (ServerListScreen.nBig > 0)
                {
                    return UnityEngine.Mathf.Clamp01((float)ServerListScreen.demPercent / (float)ServerListScreen.nBig);
                }
                if (SplashScr.maxData > 0 && SplashScr.nData > 0)
                {
                    return UnityEngine.Mathf.Clamp01((float)SplashScr.nData / (float)SplashScr.maxData);
                }
                if (ServerListScreen.percent > 0)
                {
                    return UnityEngine.Mathf.Clamp01(ServerListScreen.percent / 100f);
                }
                return 0f;
            };

            GatewayView.GetDownloadProgressFunc = () =>
            {
                if (ServerListScreen.percent > 0) return ServerListScreen.percent;
                if (SplashScr.nData > 0 && SplashScr.maxData > 0)
                {
                    return (SplashScr.nData * 100) / SplashScr.maxData;
                }
                return 0;
            };

            GatewayView.StartDownloadDataAction = () =>
            {
                if (!Session_ME.gI().isConnected())
                {
                    GameCanvas.connect();
                }
                ServerListScreen.stopDownload = false;
                ServerListScreen.demPercent = 0;
                ServerListScreen.percent = 0;
                ServerListScreen.bigOk = false;
                ServerListScreen.isGetData = true;
                Controller.isLoadingData = true;
                Service.gI().getResource(1, null);
            };

            if (_viewInstance == null)
            {
                UiRoot.EnsureInitialized();
                _viewInstance = new GameObject("GatewayScreen", typeof(RectTransform), typeof(GatewayView));
                var view = _viewInstance.GetComponent<GatewayView>();
                view.Bind(ViewModel);
                UiRouter.Instance.NavigateTo("Gateway", _viewInstance);
            }
            else if (!_viewInstance.activeSelf)
            {
                if (_viewModel != null)
                {
                    string savedAcc = Rms.loadRMSString("acc") ?? string.Empty;
                    string savedPass = Rms.loadRMSString("pass") ?? string.Empty;
                    _viewModel.Username = savedAcc;
                    _viewModel.Password = savedPass;
                    _viewModel.RememberMe = !string.IsNullOrEmpty(savedAcc);
                    _viewModel.IsLoading = false;
                    _viewModel.StatusMessage = string.Empty;
                }

                _viewInstance.SetActive(true);
            }
        }

        private static GatewayViewModel CreateViewModel()
        {
            var vm = new GatewayViewModel();

            // 1. Doc thong tin tai khoan luu san trong RMS
            string savedAcc = Rms.loadRMSString("acc") ?? string.Empty;
            string savedPass = Rms.loadRMSString("pass") ?? string.Empty;
            vm.Username = savedAcc;
            vm.Password = savedPass;
            vm.RememberMe = !string.IsNullOrEmpty(savedAcc);

            // 2. Nap danh sach may chu tu ServerListScreen
            var serverList = new List<ServerInfo>();
            if (ServerListScreen.nameServer != null && ServerListScreen.address != null)
            {
                for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
                {
                    string host = (i < ServerListScreen.address.Length) ? ServerListScreen.address[i] : "192.168.2.4";
                    short port = (ServerListScreen.port != null && i < ServerListScreen.port.Length) ? ServerListScreen.port[i] : (short)14445;
                    serverList.Add(new ServerInfo
                    {
                        Id = i,
                        Name = ServerListScreen.nameServer[i],
                        Host = host,
                        Port = port
                    });
                }
            }

            if (serverList.Count == 0)
            {
                // Fallback mac dinh neu chua nap duoc mang
                serverList.Add(new ServerInfo { Id = 0, Name = "Vũ Trụ 1 (Mặc Định)", Host = "192.168.2.4", Port = 14445 });
            }

            vm.SetServers(serverList, ServerListScreen.ipSelect);

            // 3. Dang ky xu ly dang nhap
            vm.RequestLoginEvent += (user, pass) => PerformLogin(user, pass, vm.RememberMe);
            vm.ServerChangedEvent += server =>
            {
                ServerListScreen.ipSelect = server.Id;
                GameMidlet.IP = server.Host;
                GameMidlet.PORT = server.Port;
                Rms.saveRMSInt("svselect", server.Id);
                Rms.saveIP(server.Host + ":" + server.Port);
            };

            return vm;
        }

        private static void PerformLogin(string user, string pass, bool remember)
        {
            IsLoginSucceeded = false;
            // Luon luu de LoginScr va Session dung duoc
            Rms.saveRMSString("acc", user);
            Rms.saveRMSString("pass", PasswordUtil.PrepareForSend(pass));

            // Dong bo thong tin IP va Port theo server hien tai
            if (ViewModel.CurrentServer != null)
            {
                GameMidlet.IP = ViewModel.CurrentServer.Host;
                GameMidlet.PORT = ViewModel.CurrentServer.Port;
                ServerListScreen.ipSelect = ViewModel.CurrentServer.Id;
                Rms.saveRMSInt("svselect", ViewModel.CurrentServer.Id);
                Rms.saveIP(GameMidlet.IP + ":" + GameMidlet.PORT);
            }

            if (GameCanvas.loginScr == null)
            {
                GameCanvas.loginScr = new LoginScr();
            }

            GameCanvas.loginScr.tfUser.setText(user);
            GameCanvas.loginScr.tfPass.setText(pass);
            GameCanvas.loginScr.isLogin2 = false;

            if (!Session_ME.gI().isConnected())
            {
                LoginScr.isContinueToLogin = true;
                ViewModel.StatusMessage = "Đang kết nối máy chủ...";
                GameCanvas.connect();
                return;
            }

            ViewModel.StatusMessage = "Đang vượt Thiên Môn...";
            // Gui truc tiep packet login qua Service va dong bo LoginScr
            Service.gI().login(user, PasswordUtil.PrepareForSend(pass), GameMidlet.VERSION, 0);
            GameCanvas.startWaitDlg();
        }
    }
}
