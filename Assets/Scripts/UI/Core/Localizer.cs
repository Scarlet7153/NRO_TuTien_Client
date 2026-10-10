using System;
using System.Collections.Generic;
using NRO.UI.Contracts;

namespace NRO.UI
{
    /// <summary>
    /// Hien thuc ILocalizer (D1.6).
    /// Doc tu dien chuoi tu bo nho/JSON (mac dinh rong o Sprint 2) va fallback ve mResources (tieng Viet goc).
    /// Khong chua bat ky thuat ngu Tu Tien gia lap nao o Sprint 2.
    /// </summary>
    public class Localizer : ILocalizer
    {
        private static Localizer _instance;
        public static Localizer Instance => _instance ?? (_instance = new Localizer());

        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private Func<string, string> _fallbackProvider;

        public Localizer(Func<string, string> fallbackProvider = null)
        {
            _fallbackProvider = fallbackProvider;
        }

        /// <summary>
        /// Thiet lap ham cung cap chuoi fallback (vi du goi sang mResources qua UIBridge).
        /// </summary>
        public void SetFallbackProvider(Func<string, string> fallbackProvider)
        {
            _fallbackProvider = fallbackProvider;
        }

        /// <summary>
        /// Nap tu dien tu chuoi JSON dang key-value co ban: {"key": "value"}.
        /// Sprint 2 su dung bang JSON rong: "{}".
        /// </summary>
        public void LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json) || json.Trim() == "{}" || json.Trim() == "")
            {
                return;
            }

            // Parser JSON don gian cho cac cap key:value co ban tranh phu thuoc thu vien ben ngoai
            try
            {
                string clean = json.Trim();
                if (clean.StartsWith("{") && clean.EndsWith("}"))
                {
                    clean = clean.Substring(1, clean.Length - 2).Trim();
                    if (string.IsNullOrEmpty(clean))
                    {
                        return;
                    }

                    string[] pairs = clean.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string pair in pairs)
                    {
                        int colonIndex = pair.IndexOf(':');
                        if (colonIndex > 0)
                        {
                            string k = pair.Substring(0, colonIndex).Trim().Trim('"', '\'');
                            string v = pair.Substring(colonIndex + 1).Trim().Trim('"', '\'');
                            _dictionary[k] = v;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[Localizer] Loi parse JSON: {ex.Message}");
            }
        }

        public void RegisterString(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _dictionary[key] = value ?? string.Empty;
            }
        }

        public void ClearDictionary()
        {
            _dictionary.Clear();
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            // 1. Uu tien tim trong bang tu dien JSON / Memory
            if (_dictionary.TryGetValue(key, out string value))
            {
                return value;
            }

            // 2. Fallback sang nha cung cap ben ngoai (mResources qua UIBridge)
            if (_fallbackProvider != null)
            {
                string fallback = _fallbackProvider(key);
                if (!string.IsNullOrEmpty(fallback))
                {
                    return fallback;
                }
            }

            // 3. Neu khong tim thay o bat ky dau, tra ve chinh key
            return key;
        }

        public string Format(string key, params object[] args)
        {
            string template = Get(key);
            if (args == null || args.Length == 0)
            {
                return template;
            }

            try
            {
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                // Phong ngua loi dinh dang khong khop so luong token
                return template;
            }
        }
    }
}
