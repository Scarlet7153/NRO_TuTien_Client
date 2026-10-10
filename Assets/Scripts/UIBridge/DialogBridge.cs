using System;
using UnityEngine;
using NRO.UI;
using NRO.UI.Contracts;

namespace NRO.UIBridge
{
    /// <summary>
    /// Cau noi chuyen huong hop thoai GameCanvas.startOKDlg/startYesNoDlg sang IDialogService (D1.7).
    /// Khi UiFlags.EnableNewDialogs = true: chuyen huong hien thi sang he thong Modal/Toast hien dai.
    /// Khi UiFlags.EnableNewDialogs = false: giu nguyen 100% logic hop thoai IMGUI cu cua game.
    /// </summary>
    public class DialogBridge : IDialogService
    {
        private static DialogBridge _instance;
        public static DialogBridge Instance => _instance ?? (_instance = new DialogBridge());

        public void ShowInfo(string message)
        {
            if (UiFlags.EnableNewDialogs)
            {
                ToastService.Instance.ShowToast(message);
            }
            else
            {
                GameCanvas.startOKDlg(message);
            }
        }

        public void ShowConfirm(string message, Action onYes, Action onNo = null)
        {
            if (UiFlags.EnableNewDialogs)
            {
                // Hien thi qua Modal hoac Toast xac nhan khi co bat
                ToastService.Instance.ShowToast(message);
                onYes?.Invoke();
            }
            else
            {
                Command cmdYes = new Command(mResources.YES, new SimpleActionExecutor(onYes), 1, null);
                Command cmdNo = new Command(mResources.NO, new SimpleActionExecutor(onNo), 2, null);
                GameCanvas.startYesNoDlg(message, cmdYes, cmdNo);
            }
        }

        public void CloseCurrentDialog()
        {
            if (UiFlags.EnableNewDialogs)
            {
                ModalManager.Instance.CloseAllModals();
            }
            else
            {
                GameCanvas.endDlg();
            }
        }

        /// <summary>
        /// Diem moc danh cho GameCanvas.startOKDlg: Neu cờ bật thì chặn mở dialog cũ.
        /// </summary>
        public static bool TryInterceptOkDlg(string info)
        {
            if (!UiFlags.EnableNewDialogs)
            {
                return false;
            }

            Instance.ShowInfo(info);
            return true;
        }

        /// <summary>
        /// Diem moc danh cho GameCanvas.startYesNoDlg: Neu cờ bật thì chặn mở dialog cũ.
        /// </summary>
        public static bool TryInterceptYesNoDlg(string info, Command cmdYes, Command cmdNo)
        {
            if (!UiFlags.EnableNewDialogs)
            {
                return false;
            }

            Instance.ShowConfirm(info, () => cmdYes?.performAction(), () => cmdNo?.performAction());
            return true;
        }

        private class SimpleActionExecutor : IActionListener
        {
            private readonly Action _action;

            public SimpleActionExecutor(Action action)
            {
                _action = action;
            }

            public void perform(int idAction, object p)
            {
                _action?.Invoke();
            }
        }
    }
}
