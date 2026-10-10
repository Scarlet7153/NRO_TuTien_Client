using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRO.UI
{
    /// <summary>
    /// Dieu huong va quan ly vong doi cac man hinh giao dien chinh (D1.3).
    /// </summary>
    public class UiRouter
    {
        private static UiRouter _instance;
        public static UiRouter Instance => _instance ?? (_instance = new UiRouter());

        public event Action<string> OnScreenChanged;

        private readonly Stack<GameObject> _screenStack = new Stack<GameObject>();
        private string _currentScreenName = string.Empty;

        public string CurrentScreenName => _currentScreenName;

        public void NavigateTo(string screenName, GameObject screenInstance)
        {
            if (screenInstance == null)
            {
                return;
            }

            UiRoot.EnsureInitialized();
            if (UiRoot.Instance != null && UiRoot.Instance.ScreensContainer != null)
            {
                screenInstance.transform.SetParent(UiRoot.Instance.ScreensContainer, false);
            }

            // An man hinh hien tai neu co
            if (_screenStack.Count > 0)
            {
                var top = _screenStack.Peek();
                if (top != null)
                {
                    top.SetActive(false);
                }
            }

            screenInstance.SetActive(true);
            _screenStack.Push(screenInstance);
            _currentScreenName = screenName;

            OnScreenChanged?.Invoke(_currentScreenName);
        }

        public bool Back()
        {
            if (_screenStack.Count <= 1)
            {
                return false;
            }

            var current = _screenStack.Pop();
            if (current != null)
            {
                UnityEngine.Object.Destroy(current);
            }

            if (_screenStack.Count > 0)
            {
                var prev = _screenStack.Peek();
                if (prev != null)
                {
                    prev.SetActive(true);
                    _currentScreenName = prev.name;
                    OnScreenChanged?.Invoke(_currentScreenName);
                    return true;
                }
            }

            _currentScreenName = string.Empty;
            OnScreenChanged?.Invoke(string.Empty);
            return true;
        }

        public void ClearAll()
        {
            while (_screenStack.Count > 0)
            {
                var screen = _screenStack.Pop();
                if (screen != null)
                {
                    UnityEngine.Object.Destroy(screen);
                }
            }
            _currentScreenName = string.Empty;
            OnScreenChanged?.Invoke(string.Empty);
        }
    }
}
