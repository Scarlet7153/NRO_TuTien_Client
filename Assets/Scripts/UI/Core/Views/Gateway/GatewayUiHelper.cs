using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI.Views
{
    /// <summary>
    /// Tiện ích hỗ trợ khởi tạo các thành phần giao diện mỹ thuật Tiên Hiệp dùng chung.
    /// </summary>
    public static class GatewayUiHelper
    {
        public static Text CreateText(Transform parent, string name, string text, int size, Color color, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var txt = go.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = size;
            txt.color = color;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = ThemeTokens.GetDefaultFont();
            txt.horizontalOverflow = HorizontalWrapMode.Overflow;
            txt.verticalOverflow = VerticalWrapMode.Overflow;
            return txt;
        }

        public static GameObject CreateCustomButton(Transform parent, string name, string label, Sprite bgSprite, Color textColor, Vector2 pos, Vector2 sizeDelta, Action onClick)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var img = go.GetComponent<Image>();
            img.sprite = bgSprite;
            img.type = Image.Type.Sliced;
            img.color = Color.white;

            var btn = go.GetComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(1.2f, 1.15f, 1.05f, 1f);
            colors.pressedColor = new Color(0.85f, 0.80f, 0.70f, 1f);
            btn.colors = colors;
            btn.onClick.AddListener(() => onClick?.Invoke());

            var labelTxt = CreateText(go.transform, "Label", label, 15, textColor, Vector2.zero, sizeDelta);
            labelTxt.fontStyle = FontStyle.Bold;
            return go;
        }

        public static InputField CreateCustomInputField(Transform parent, string name, string icon, string placeholder, bool isPass, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var img = go.GetComponent<Image>();
            img.sprite = ThemeTokens.GetXianxiaInputSprite();
            img.type = Image.Type.Sliced;
            img.color = Color.white;

            // Icon tiền tố ngọc bích
            CreateText(go.transform, "Icon", icon, 14, ThemeTokens.CelestialCyan, new Vector2(-sizeDelta.x / 2f + 20f, 0), new Vector2(30, sizeDelta.y));

            var input = go.GetComponent<InputField>();
            var text = CreateText(go.transform, "Text", "", 14, ThemeTokens.MoonlightSilver, new Vector2(15, 0), sizeDelta - new Vector2(50, 0));
            text.alignment = TextAnchor.MiddleLeft;

            var phText = CreateText(go.transform, "Placeholder", placeholder, 14, new Color(ThemeTokens.MoonlightSilver.r, ThemeTokens.MoonlightSilver.g, ThemeTokens.MoonlightSilver.b, 0.45f), new Vector2(15, 0), sizeDelta - new Vector2(50, 0));
            phText.alignment = TextAnchor.MiddleLeft;

            input.textComponent = text;
            input.placeholder = phText;
            input.lineType = InputField.LineType.SingleLine;
            if (isPass) input.contentType = InputField.ContentType.Password;
            return input;
        }

        public static void AddOutlineAndShadow(GameObject go, Color outlineColor, Color shadowColor)
        {
            if (go == null) return;
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.70f);
            shadow.effectDistance = new Vector2(1f, -1f);
        }

        public static void Add3DGoldEffect(GameObject go)
        {
            if (go == null) return;
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.04f, 0.02f, 0.01f, 0.90f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);

            var outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0.20f, 0.12f, 0.02f, 0.95f);
            outline.effectDistance = new Vector2(0.8f, -0.8f);
        }

        public static void CreateCornerOrnament(Transform parent, Vector2 pos)
        {
            var txt = CreateText(parent, "Ornament", "❖", 14, ThemeTokens.AncientGoldBright, pos, new Vector2(24, 24));
            AddOutlineAndShadow(txt.gameObject, new Color(0.1f, 0.08f, 0.02f, 0.8f), Color.clear);
        }

        public static void LoadBackgroundArtwork(Image targetImg)
        {
            if (targetImg == null) return;
            try
            {
                // 1. Thu load Sprite qua Unity Resources (nhanh nhat va chuan Unity nhat)
                var resSprite = Resources.Load<Sprite>("UI/xianxia_gateway_bg");
                if (resSprite != null)
                {
                    targetImg.sprite = resSprite;
                    targetImg.color = Color.white;
                    return;
                }

                // 2. Thu load qua Texture2D tu Resources
                var resTex = Resources.Load<Texture2D>("UI/xianxia_gateway_bg");
                if (resTex != null)
                {
                    targetImg.sprite = Sprite.Create(resTex, new Rect(0, 0, resTex.width, resTex.height), new Vector2(0.5f, 0.5f));
                    targetImg.color = Color.white;
                    return;
                }

                // 3. Quét các đường dẫn thực tế trên ổ đĩa
                string[] candidatePaths = new string[]
                {
                    Path.Combine(Application.streamingAssetsPath, "UI", "xianxia_gateway_bg.png"),
                    Path.Combine(Application.dataPath, "StreamingAssets", "UI", "xianxia_gateway_bg.png"),
                    Path.Combine(Application.dataPath, "Resources", "UI", "xianxia_gateway_bg.png"),
                    Path.Combine(Application.streamingAssetsPath, "UI", "xianxia_gateway_bg.jpg"),
                    Path.Combine(Application.dataPath, "StreamingAssets", "UI", "xianxia_gateway_bg.jpg"),
                    Path.Combine(Application.dataPath, "Resources", "UI", "xianxia_gateway_bg.jpg"),
                    Path.Combine(Application.dataPath, "login_bg_xianxia.png"),
                    Path.Combine(Application.dataPath, "Resources", "login_bg_xianxia.png"),
                    Path.Combine(Application.streamingAssetsPath, "login_bg_xianxia.png")
                };

                foreach (var path in candidatePaths)
                {
                    if (File.Exists(path))
                    {
                        byte[] fileData = File.ReadAllBytes(path);
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (tex.LoadImage(fileData))
                        {
                            tex.wrapMode = TextureWrapMode.Clamp;
                            tex.filterMode = FilterMode.Bilinear;
                            tex.anisoLevel = 8;
                            targetImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                            targetImg.color = Color.white;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[GatewayUiHelper] LoadBackgroundArtwork failed: " + ex.Message);
            }
        }
    }
}
