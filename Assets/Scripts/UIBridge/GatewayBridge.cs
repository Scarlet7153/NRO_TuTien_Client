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

        public static GatewayViewModel ViewModel => _viewModel ?? (_viewModel = CreateViewModel());

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

            // Neu dang o man hinh Login hoac ServerList thi kich hoat giao dien moi
            bool isInLogin = GameCanvas.currentScreen == GameCanvas.loginScr ||
                             (GameCanvas.serverScreen != null && GameCanvas.currentScreen == GameCanvas.serverScreen);

            if (isInLogin)
            {
                EnsureViewShown();
                return true;
            }

            if (_viewInstance != null && _viewInstance.activeSelf)
            {
                _viewInstance.SetActive(false);
            }
            return false;
        }

        private static void EnsureViewShown()
        {
            if (_viewInstance == null)
            {
                UiRoot.EnsureInitialized();
                _viewInstance = new GameObject("GatewayScreen", typeof(GatewayView));
                var view = _viewInstance.GetComponent<GatewayView>();
                view.Bind(ViewModel);
                UiRouter.Instance.NavigateTo("Gateway", _viewInstance);
            }
            else if (!_viewInstance.activeSelf)
            {
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
                Rms.saveRMSInt("serverSelect", server.Id);
            };

            return vm;
        }

        private static void PerformLogin(string user, string pass, bool remember)
        {
            if (remember)
            {
                Rms.saveRMSString("acc", user);
                Rms.saveRMSString("pass", pass);
            }
            else
            {
                Rms.saveRMSString("acc", "");
                Rms.saveRMSString("pass", "");
            }

            if (!Session_ME.gI().isConnected())
            {
                GameCanvas.connect();
            }

            // Goi ham dang nhap qua Service
            Service.gI().login(user, PasswordUtil.PrepareForSend(pass), GameMidlet.VERSION, 0);
        }
    }
}
