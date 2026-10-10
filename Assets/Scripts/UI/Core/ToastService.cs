using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI
{
    /// <summary>
    /// Dich vu hien thi thong bao noi (Toast notifications) tu bien mat (D1.3).
    /// </summary>
    public class ToastService : MonoBehaviour
    {
        private static ToastService _instance;
        public static ToastService Instance
        {
            get
            {
                if (_instance == null)
                {
                    UiRoot.EnsureInitialized();
                    if (UiRoot.Instance != null)
                    {
                        _instance = UiRoot.Instance.gameObject.AddComponent<ToastService>();
                    }
                }
                return _instance;
            }
        }

        public void ShowToast(string message, float duration = 2.5f)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            StartCoroutine(ShowToastRoutine(message, duration));
        }

        private IEnumerator ShowToastRoutine(string message, float duration)
        {
            UiRoot.EnsureInitialized();
            if (UiRoot.Instance == null || UiRoot.Instance.ToastsContainer == null)
            {
                yield break;
            }

            var toastGo = new GameObject("ToastItem", typeof(RectTransform), typeof(Image));
            toastGo.transform.SetParent(UiRoot.Instance.ToastsContainer, false);

            var rt = toastGo.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.85f);
            rt.anchorMax = new Vector2(0.5f, 0.85f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400, 45);

            var bgImg = toastGo.GetComponent<Image>();
            bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(toastGo.transform, false);

            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;

            var txt = textGo.GetComponent<Text>();
            txt.text = message;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontSize = 14;
            txt.font = ThemeTokens.GetDefaultFont();

            yield return new WaitForSeconds(duration);

            // Hieu ung mo dan (Fade out) trong 0.3s
            float elapsed = 0f;
            float fadeTime = 0.3f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0.85f, 0f, elapsed / fadeTime);
                if (bgImg != null) bgImg.color = new Color(0.1f, 0.1f, 0.1f, alpha);
                if (txt != null) txt.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            if (toastGo != null)
            {
                Destroy(toastGo);
            }
        }
    }
}
