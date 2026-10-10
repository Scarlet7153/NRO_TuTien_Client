using UnityEngine;
using NRO.UI;
using NRO.UI.ViewModels;
using NRO.UI.Views;

namespace NRO.UIBridge
{
    /// <summary>
    /// Cau noi giua CharCreationView/ViewModel (S2) voi he thong tao/chon nhan vat cu (CreateCharScr, ChooseCharScr, Service).
    /// </summary>
    public static class CharacterBridge
    {
        private static CharCreationViewModel _viewModel;
        private static GameObject _viewInstance;

        public static CharCreationViewModel ViewModel => _viewModel ?? (_viewModel = CreateViewModel());

        public static bool ShouldInterceptOnGUI()
        {
            if (!UiFlags.EnableNewCharSelect)
            {
                if (_viewInstance != null && _viewInstance.activeSelf)
                {
                    _viewInstance.SetActive(false);
                }
                return false;
            }

            bool isInCharScr = (CreateCharScr.instance != null && GameCanvas.currentScreen == CreateCharScr.instance) ||
                               (GameCanvas.chooseCharScr != null && GameCanvas.currentScreen == GameCanvas.chooseCharScr);

            if (isInCharScr)
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
                _viewInstance = new GameObject("CharCreationScreen", typeof(CharCreationView));
                var view = _viewInstance.GetComponent<CharCreationView>();
                view.Bind(ViewModel);
                UiRouter.Instance.NavigateTo("CharCreation", _viewInstance);
            }
            else if (!_viewInstance.activeSelf)
            {
                _viewInstance.SetActive(true);
            }
        }

        private static CharCreationViewModel CreateViewModel()
        {
            var vm = new CharCreationViewModel();

            vm.RequestCreateCharacterEvent += (name, gender, hair) =>
            {
                // Goi dich vu tao nhan vat thuc te cua game
                Service.gI().createChar(name, gender, hair);
            };

            vm.RequestSelectCharacterEvent += charId =>
            {
                // Goi chon nhan vat vao game qua finishUpdate
                GameCanvas.startWaitDlg();
                Service.gI().finishUpdate(charId);
            };

            return vm;
        }
    }
}
