using System;
using System.Collections.Generic;

namespace NRO.UI.ViewModels
{
    public class ServerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    /// <summary>
    /// ViewModel quan ly logic man hinh Cong Vao (Login & Server Selection - S1).
    /// </summary>
    public class GatewayViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = true;

        public ServerInfo CurrentServer { get; private set; }
        public List<ServerInfo> Servers { get; } = new List<ServerInfo>();

        public bool IsLoading { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public string RegisterUrl { get; set; } = "https://ngocrongtutien.online";

        public event Action<string, string> RequestLoginEvent;
        public event Action<ServerInfo> ServerChangedEvent;

        public void SetServers(IEnumerable<ServerInfo> serverList, int defaultIndex = 0)
        {
            Servers.Clear();
            if (serverList != null)
            {
                Servers.AddRange(serverList);
            }

            if (Servers.Count > 0)
            {
                int index = (defaultIndex >= 0 && defaultIndex < Servers.Count) ? defaultIndex : 0;
                SelectServer(index);
            }
        }

        public void SelectServer(int index)
        {
            if (index >= 0 && index < Servers.Count)
            {
                CurrentServer = Servers[index];
                ServerChangedEvent?.Invoke(CurrentServer);
            }
        }

        public bool ValidateCredentials(out string error)
        {
            if (string.IsNullOrEmpty(Username) || Username.Trim().Length < 3)
            {
                error = "Tài khoản phải có ít nhất 3 ký tự.";
                return false;
            }

            if (string.IsNullOrEmpty(Password) || Password.Length < 3)
            {
                error = "Mật khẩu phải có ít nhất 3 ký tự.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public void SubmitLogin()
        {
            if (!ValidateCredentials(out string error))
            {
                StatusMessage = error;
                return;
            }

            StatusMessage = "Đang kết nối đến máy chủ...";
            IsLoading = true;
            RequestLoginEvent?.Invoke(Username.Trim(), Password);
        }
    }
}
