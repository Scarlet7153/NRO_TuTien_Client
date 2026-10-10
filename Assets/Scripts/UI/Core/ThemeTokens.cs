using UnityEngine;

namespace NRO.UI
{
    /// <summary>
    /// Design Tokens cho phong cach Tien Hiep - Tu Tien (D2.1).
    /// Cung cap bang mau chuan (Thanh Bich, Hoang Kim, Tu Khi, Huyet Quang, Bach Van)
    /// va cac thong so typography/khoang cach layout.
    /// </summary>
    public static class ThemeTokens
    {
        // 1. Color Palette - Tu Tien Theme
        public static readonly Color ColorPrimary = new Color(0f, 0.824f, 0.522f); // #00D285 - Thanh Bích / Linh Khí
        public static readonly Color ColorSecondary = new Color(1f, 0.843f, 0f); // #FFD700 - Hoàng Kim / Kim Đan
        public static readonly Color ColorAccentRed = new Color(0.937f, 0.267f, 0.267f); // #EF4444 - Huyết Quang / Cảnh Báo
        public static readonly Color ColorAccentPurple = new Color(0.655f, 0.443f, 0.898f); // #A771E5 - Tử Khí / Thần Thông

        // Backgrounds
        public static readonly Color ColorDarkBg = new Color(0.071f, 0.067f, 0.102f, 0.95f); // #12111A - Màn Đêm U Tịch
        public static readonly Color ColorCardBg = new Color(0.118f, 0.11f, 0.169f, 0.9f); // #1E1C2B - Thẻ Bài / Ngọc Giản
        public static readonly Color ColorCardBorder = new Color(0.247f, 0.231f, 0.353f, 0.8f); // #3F3B5A - Viền Đồng Cổ

        // Typography Colors
        public static readonly Color ColorTextPrimary = new Color(0.973f, 0.98f, 0.988f); // #F8FAFC - Bạch Vân
        public static readonly Color ColorTextSecondary = new Color(0.58f, 0.639f, 0.722f); // #94A3B8 - Sương Mù
        public static readonly Color ColorTextGold = new Color(1f, 0.9f, 0.4f); // Hoàng Kim Chữ

        // 2. Metrics & Dimensions
        public const float ButtonHeightStandard = 44f;
        public const float ButtonHeightLarge = 52f;
        public const float InputFieldHeight = 44f;
        public const float CardCornerRadius = 8f;
        public const float SpacingSmall = 8f;
        public const float SpacingMedium = 16f;
        public const float SpacingLarge = 24f;
    }
}
