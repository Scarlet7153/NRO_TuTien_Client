using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI
{
    /// <summary>
    /// Quan ly cac hop thoai popup / modal stack voi lop man mo (D1.3).
    /// </summary>
    public class ModalManager
    {
        private static ModalManager _instance;
        public static ModalManager Instance => _instance ?? (_instance = new ModalManager());

        private readonly Stack<GameObject> _modalStack = new Stack<GameObject>();
        private GameObject _dimBackdrop;

        public bool HasActiveModal => _modalStack.Count > 0;

        public void ShowModal(GameObject modalInstance, bool closeOnBackdropClick = false, Action onClosed = null)
        {
            if (modalInstance == null)
            {
                return;
            }

            UiRoot.EnsureInitialized();
            if (UiRoot.Instance == null || UiRoot.Instance.ModalsContainer == null)
            {
                return;
            }

            EnsureBackdrop(closeOnBackdropClick);

            modalInstance.transform.SetParent(UiRoot.Instance.ModalsContainer, false);
            modalInstance.SetActive(true);
            _modalStack.Push(modalInstance);

            UpdateBackdropState();
        }

        public void CloseTopModal()
        {
            if (_modalStack.Count == 0)
            {
                return;
            }

            var top = _modalStack.Pop();
            if (top != null)
            {
                UnityEngine.Object.Destroy(top);
            }

            UpdateBackdropState();
        }

        public void CloseAllModals()
        {
            while (_modalStack.Count > 0)
            {
                var modal = _modalStack.Pop();
                if (modal != null)
                {
                    UnityEngine.Object.Destroy(modal);
                }
            }
            UpdateBackdropState();
        }

        private void EnsureBackdrop(bool closeOnClick)
        {
            if (_dimBackdrop != null)
            {
                return;
            }

            _dimBackdrop = new GameObject("DimBackdrop", typeof(RectTransform), typeof(Image), typeof(Button));
            _dimBackdrop.transform.SetParent(UiRoot.Instance.ModalsContainer, false);
            _dimBackdrop.transform.SetAsFirstSibling();

            var rt = _dimBackdrop.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var img = _dimBackdrop.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.65f);

            var btn = _dimBackdrop.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                if (closeOnClick)
                {
                    CloseTopModal();
                }
            });
        }

        private void UpdateBackdropState()
        {
            if (_dimBackdrop != null)
            {
                bool active = _modalStack.Count > 0;
                _dimBackdrop.SetActive(active);
                if (active)
                {
                    _dimBackdrop.transform.SetSiblingIndex(_modalStack.Count - 1);
                }
            }
        }
    }
}
