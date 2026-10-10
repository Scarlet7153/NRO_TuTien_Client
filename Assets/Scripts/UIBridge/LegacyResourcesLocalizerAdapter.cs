using UnityEngine;
using NRO.UI;

namespace NRO.UIBridge
{
    /// <summary>
    /// Cau noi (Adapter) tu he thong chuoi tinh mResources sang ILocalizer (D1.6).
    /// Tu dong dang ky lam FallbackProvider cho Localizer khi game khoi chay.
    /// Chi truy xuat chuoi tieng Viet hien co trong mResources, khong gia dinh thuat ngu Tu Tien o Sprint 2.
    /// </summary>
    public static class LegacyResourcesLocalizerAdapter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Localizer.Instance.SetFallbackProvider(QueryLegacyResources);
        }

        public static string QueryLegacyResources(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            switch (key.ToLowerInvariant())
            {
                case "btn_ok":
                case "ok":
                    return mResources.OK;

                case "btn_cancel":
                case "cancel":
                    return mResources.CANCEL;

                case "btn_close":
                case "close":
                    return mResources.CLOSE;

                case "btn_accept":
                case "accept":
                    return mResources.ACCEPT;

                case "btn_denied":
                case "denied":
                    return mResources.NO;

                case "enter":
                    return mResources.enter;

                case "select":
                    return mResources.SELECT;

                case "pleasewait":
                case "please_wait":
                    return mResources.PLEASEWAIT;

                case "connecting":
                    return mResources.CONNECTING;

                default:
                    return null;
            }
        }
    }
}
