using System;

namespace NRO.UI.Contracts
{
    /// <summary>
    /// Hop dong giao dien hien thi hop thoai thong bao va xac nhan (D1.7).
    /// </summary>
    public interface IDialogService
    {
        void ShowInfo(string message);
        void ShowConfirm(string message, Action onYes, Action onNo = null);
        void CloseCurrentDialog();
    }
}
