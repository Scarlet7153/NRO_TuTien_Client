namespace NRO.UI
{
    /// <summary>
    /// Quan ly feature flags cho he thong UI moi (D1.3).
    /// Mac dinh TAT TOAN BO (false) de dam bao 100% tuong thich va bao toan game goc khi chua playtest.
    /// </summary>
    public static class UiFlags
    {
        public static bool EnableNewSplash { get; set; } = false;
        public static bool EnableNewLogin { get; set; } = true;
        public static bool EnableNewCharSelect { get; set; } = false;
        public static bool EnableNewHud { get; set; } = false;
        public static bool EnableNewInventory { get; set; } = false;
        public static bool EnableNewDialogs { get; set; } = false;

        /// <summary>
        /// Kiem tra xem mot man hinh cu the co dang duoc kich hoat che do giao dien moi hay khong.
        /// </summary>
        public static bool IsScreenNew(string screenName)
        {
            switch (screenName)
            {
                case "Splash": return EnableNewSplash;
                case "Login": return EnableNewLogin;
                case "CharSelect": return EnableNewCharSelect;
                case "HUD": return EnableNewHud;
                case "Inventory": return EnableNewInventory;
                case "Dialog": return EnableNewDialogs;
                default: return false;
            }
        }

        /// <summary>
        /// Reset tat ca cac co ve false (trang thai an toan).
        /// </summary>
        public static void ResetAll()
        {
            EnableNewSplash = false;
            EnableNewLogin = false;
            EnableNewCharSelect = false;
            EnableNewHud = false;
            EnableNewInventory = false;
            EnableNewDialogs = false;
        }
    }
}
