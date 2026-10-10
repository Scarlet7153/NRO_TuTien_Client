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
        public static readonly Color ColorDarkBackground = ColorDarkBg; // Alias
        public static readonly Color ColorCardBg = new Color(0.118f, 0.11f, 0.169f, 0.9f); // #1E1C2B - Thẻ Bài / Ngọc Giản
        public static readonly Color ColorCardBackground = ColorCardBg; // Alias
        public static readonly Color ColorCardBorder = new Color(0.247f, 0.231f, 0.353f, 0.8f); // #3F3B5A - Viền Đồng Cổ

        // Typography Colors
        public static readonly Color ColorTextPrimary = new Color(0.973f, 0.98f, 0.988f); // #F8FAFC - Bạch Vân
        public static readonly Color ColorTextLight = ColorTextPrimary; // Alias
        public static readonly Color ColorTextSecondary = new Color(0.58f, 0.639f, 0.722f); // #94A3B8 - Sương Mù
        public static readonly Color ColorTextMuted = new Color(0.58f, 0.639f, 0.722f); // Alias cho ColorTextSecondary
        public static readonly Color ColorTextGold = new Color(1f, 0.9f, 0.4f); // Hoàng Kim Chữ

        // 2. Metrics & Dimensions
        public const float ButtonHeightStandard = 44f;
        public const float ButtonHeightLarge = 52f;
        public const float InputFieldHeight = 44f;
        public const float CardCornerRadius = 8f;
        public const float SpacingSmall = 8f;
        public const float SpacingMedium = 16f;
        public const float SpacingLarge = 24f;

        private static Font _cachedFont;

        /// <summary>
        /// Lay Font chu he thong an toan tren Unity 6 (LegacyRuntime.ttf) va cac ban Unity cu.
        /// </summary>
        public static Font GetDefaultFont()
        {
            if (_cachedFont != null) return _cachedFont;

            try
            {
                _cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch { }

            if (_cachedFont == null)
            {
                try
                {
                    _cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
                catch { }
            }

            if (_cachedFont == null)
            {
                _cachedFont = Font.CreateDynamicFontFromOSFont("Arial", 14);
            }

            return _cachedFont;
        }

        // Xianxia MMORPG Art Direction Palette
        public static readonly Color MidnightBlue = new Color(0.031f, 0.055f, 0.098f, 1f); // #080E19
        public static readonly Color DeepJade = new Color(0.063f, 0.173f, 0.173f, 1f); // #102C2C
        public static readonly Color AncientGold = new Color(0.784f, 0.663f, 0.420f, 1f); // #C8A96B
        public static readonly Color AncientGoldBright = new Color(0.965f, 0.886f, 0.655f, 1f); // #F6E2A7
        public static readonly Color AncientGoldDark = new Color(0.486f, 0.380f, 0.208f, 1f); // #7C6135
        public static readonly Color MoonlightSilver = new Color(0.835f, 0.863f, 0.910f, 1f); // #D5DCE8
        public static readonly Color CelestialCyan = new Color(0.459f, 0.788f, 0.816f, 1f); // #75C9D0
        public static readonly Color SpiritJade = new Color(0.18f, 0.78f, 0.68f, 1f); // Linh ngọc xanh biếc

        // Procedural Xianxia UI Sprites
        private static Sprite _cardSprite;
        private static Sprite _buttonPrimarySprite;
        private static Sprite _buttonSecondarySprite;
        private static Sprite _inputFieldSprite;
        private static Sprite _xianxiaCardSprite;
        private static Sprite _xianxiaButtonSprite;
        private static Sprite _xianxiaInputSprite;
        private static Sprite _xianxiaServerBoxSprite;

        public static Sprite GetCardSprite()
        {
            return _cardSprite ?? (_cardSprite = CreateRoundedBox(64, 14, new Color(0.08f, 0.07f, 0.13f, 0.94f), new Color(0.85f, 0.72f, 0.35f, 0.9f), 2));
        }

        public static Sprite GetXianxiaCardSprite()
        {
            return _xianxiaCardSprite ?? (_xianxiaCardSprite = CreateImperialCard(128, 128, new Color(0.025f, 0.05f, 0.09f, 0.94f), new Color(0.85f, 0.72f, 0.44f, 0.95f), new Color(0.45f, 0.35f, 0.18f, 0.8f)));
        }

        public static Sprite GetXianxiaButtonSprite()
        {
            return _xianxiaButtonSprite ?? (_xianxiaButtonSprite = CreateMetallicPlate(128, 36, new Color(1f, 0.94f, 0.65f, 1f), new Color(0.88f, 0.69f, 0.28f, 1f), new Color(0.52f, 0.36f, 0.10f, 1f), new Color(1f, 0.98f, 0.82f, 1f)));
        }

        public static Sprite GetXianxiaInputSprite()
        {
            return _xianxiaInputSprite ?? (_xianxiaInputSprite = CreateMetallicPlate(128, 32, new Color(0.04f, 0.08f, 0.13f, 0.95f), new Color(0.02f, 0.05f, 0.09f, 0.96f), new Color(0.06f, 0.12f, 0.18f, 0.95f), new Color(0.65f, 0.52f, 0.30f, 0.85f)));
        }

        public static Sprite GetXianxiaServerBoxSprite()
        {
            return _xianxiaServerBoxSprite ?? (_xianxiaServerBoxSprite = CreateMetallicPlate(128, 32, new Color(0.06f, 0.16f, 0.18f, 0.95f), new Color(0.03f, 0.09f, 0.11f, 0.96f), new Color(0.05f, 0.14f, 0.16f, 0.95f), new Color(0.85f, 0.70f, 0.40f, 0.9f)));
        }

        private static Sprite _xianxiaGearSprite;

        public static Sprite GetXianxiaGearSprite()
        {
            if (_xianxiaGearSprite != null) return _xianxiaGearSprite;
            return _xianxiaGearSprite = CreateXianxiaGearSprite(64);
        }

        public static Sprite CreateXianxiaGearSprite(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float cx = (size - 1) / 2f;
            float cy = (size - 1) / 2f;
            float maxR = size * 0.46f;
            float rootR = size * 0.35f;
            float holeR = size * 0.14f;
            float hubR = size * 0.22f;
            int teeth = 8;
            float toothWidthFactor = 0.42f;

            Color goldLight = new Color(1f, 0.95f, 0.72f, 1f);
            Color goldMid = new Color(0.90f, 0.75f, 0.38f, 1f);
            Color goldDark = new Color(0.50f, 0.36f, 0.14f, 1f);
            Color jadeCore = new Color(0.35f, 0.92f, 0.98f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float angle = Mathf.Atan2(dy, dx);
                    if (angle < 0) angle += Mathf.PI * 2f;

                    float toothAngleStep = Mathf.PI * 2f / teeth;
                    float normAngle = Mathf.Repeat(angle, toothAngleStep) / toothAngleStep;
                    float centerDist = Mathf.Abs(normAngle - 0.5f);

                    float toothProfile = Mathf.Clamp01((toothWidthFactor - centerDist * 0.9f) * 6f);
                    float targetR = Mathf.Lerp(rootR, maxR, toothProfile);

                    if (dist > targetR + 0.8f)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist < holeR - 0.5f)
                    {
                        float coreAlpha = Mathf.Clamp01(1f - dist / holeR);
                        Color c = Color.Lerp(jadeCore, Color.white, coreAlpha * 0.7f);
                        tex.SetPixel(x, y, c);
                    }
                    else if (dist < holeR + 1.2f)
                    {
                        tex.SetPixel(x, y, goldDark);
                    }
                    else
                    {
                        float spokeAngle = Mathf.Repeat(angle + Mathf.PI / 4f, Mathf.PI / 2f) / (Mathf.PI / 2f);
                        float spokeDist = Mathf.Abs(spokeAngle - 0.5f);
                        bool isSpoke = spokeDist < 0.14f;
                        bool isRim = dist >= rootR * 0.88f;
                        bool isHub = dist <= hubR;

                        if (!isRim && !isHub && !isSpoke)
                        {
                            tex.SetPixel(x, y, Color.clear);
                            continue;
                        }

                        float gradY = Mathf.Clamp01((y - (cy - maxR)) / (maxR * 2f));
                        Color metalCol = Color.Lerp(goldLight, goldMid, gradY);

                        float edgeDist = Mathf.Abs(dist - targetR);
                        if (edgeDist < 1.2f)
                        {
                            metalCol = Color.Lerp(metalCol, goldLight, 0.7f);
                        }
                        else if (dist > rootR && toothProfile < 0.15f)
                        {
                            metalCol = goldDark;
                        }

                        float alpha = Mathf.Clamp01(targetR - dist + 0.8f);
                        metalCol.a *= alpha;

                        tex.SetPixel(x, y, metalCol);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        public static Sprite CreateMetallicPlate(int width, int height, Color topColor, Color midColor, Color botColor, Color borderColor)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / (height - 1);
                Color rowBg = (t > 0.5f) ? Color.Lerp(midColor, topColor, (t - 0.5f) * 2f) : Color.Lerp(botColor, midColor, t * 2f);
                for (int x = 0; x < width; x++)
                {
                    bool isBorder = (x < 2 || x >= width - 2 || y < 2 || y >= height - 2);
                    bool isCorner = (x <= 1 && y <= 1) || (x >= width - 2 && y <= 1) || (x <= 1 && y >= height - 2) || (x >= width - 2 && y >= height - 2);

                    if (isCorner)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (isBorder)
                    {
                        // Beveled highlight at top border, shadow at bottom
                        Color bColor = (y >= height - 2) ? Color.Lerp(borderColor, Color.white, 0.4f) : borderColor;
                        tex.SetPixel(x, y, bColor);
                    }
                    else
                    {
                        tex.SetPixel(x, y, rowBg);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(6, 6, 6, 6));
        }

        public static Sprite CreateImperialCard(int width, int height, Color bg, Color outerGold, Color innerGold)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < height; y++)
            {
                float t = (float)y / (height - 1);
                Color rowBg = Color.Lerp(new Color(bg.r * 0.7f, bg.g * 0.7f, bg.b * 0.7f, bg.a), bg, t);
                for (int x = 0; x < width; x++)
                {
                    bool isOuterBorder = (x < 2 || x >= width - 2 || y < 2 || y >= height - 2);
                    bool isInnerLine = (x == 5 || x == width - 6 || y == 5 || y == height - 6);
                    bool isCornerCut = (x <= 2 && y <= 2) || (x >= width - 3 && y <= 2) || (x <= 2 && y >= height - 3) || (x >= width - 3 && y >= height - 3);

                    if (isCornerCut)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (isOuterBorder)
                    {
                        tex.SetPixel(x, y, outerGold);
                    }
                    else if (isInnerLine)
                    {
                        tex.SetPixel(x, y, innerGold);
                    }
                    else
                    {
                        tex.SetPixel(x, y, rowBg);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(8, 8, 8, 8));
        }

        public static Sprite GetButtonPrimarySprite()
        {
            return _buttonPrimarySprite ?? (_buttonPrimarySprite = CreateRoundedBox(64, 10, new Color(0f, 0.72f, 0.45f, 1f), new Color(0.65f, 1f, 0.85f, 0.95f), 2));
        }

        public static Sprite GetButtonSecondarySprite()
        {
            return _buttonSecondarySprite ?? (_buttonSecondarySprite = CreateRoundedBox(64, 8, new Color(0.13f, 0.12f, 0.20f, 0.95f), new Color(0.75f, 0.65f, 0.40f, 0.8f), 1));
        }

        public static Sprite GetInputFieldSprite()
        {
            return _inputFieldSprite ?? (_inputFieldSprite = CreateRoundedBox(64, 8, new Color(0.05f, 0.04f, 0.08f, 0.95f), new Color(0.28f, 0.27f, 0.42f, 0.85f), 1));
        }

        public static Sprite CreateRoundedBox(int size, int radius, Color fillColor, Color borderColor, int borderWidth)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float r = radius;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x < r) ? (r - x) : (x >= size - r ? (x - (size - r - 1)) : 0f);
                    float dy = (y < r) ? (r - y) : (y >= size - r ? (y - (size - r - 1)) : 0f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > r)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist > r - borderWidth || x < borderWidth || x >= size - borderWidth || y < borderWidth || y >= size - borderWidth)
                    {
                        float alpha = Mathf.Clamp01(r - dist + 0.5f);
                        Color c = borderColor;
                        c.a *= alpha;
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, fillColor);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        }

        public static Sprite CreateCircleSprite(int size, Color color)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float r = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r;
                    float dy = y - r;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > r)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01(r - dist + 0.5f);
                        Color c = color;
                        c.a *= alpha;
                        tex.SetPixel(x, y, c);
                    }
                }
            }
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }
    }
}
