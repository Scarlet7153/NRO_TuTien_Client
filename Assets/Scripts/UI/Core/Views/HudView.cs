using System;
using UnityEngine;
using UnityEngine.UI;
using NRO.UI.ViewModels;

namespace NRO.UI.Views
{
    /// <summary>
    /// Giao dien Greybox uGUI HUD Chien Dau Tu Tien (Lat cat S3).
    /// Tu sinh component dong khong can Prefab, tuong thich CanvasScaler 1280x720.
    /// </summary>
    public class HudView : MonoBehaviour
    {
        private HudViewModel _viewModel;

        // Top-Left (Vitality)
        private Text _txtName;
        private Text _txtRealm;
        private Image _imgHpBarFill;
        private Text _txtHpValues;
        private Image _imgMpBarFill;
        private Text _txtMpValues;

        // Top-Center (Target)
        private GameObject _goTargetPanel;
        private Text _txtTargetName;
        private Image _imgTargetHpFill;
        private Text _txtTargetHpValues;

        // Bottom-Right (Skills & Potions)
        private readonly Image[] _imgSkillCooldowns = new Image[5];
        private readonly Text[] _txtSkillNames = new Text[5];
        private readonly GameObject[] _goSkillSelections = new GameObject[5];
        private Text _txtPotionCount;
        private Image _imgPotionCooldown;

        // Top-Right (Map & Nav)
        private Text _txtMapInfo;

        public void Bind(HudViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Awake()
        {
            BuildUI();
        }

        private void Update()
        {
            if (_viewModel == null) return;

            // Handle PC Key Shortcuts
            HandleInput();

            // Refresh UI States
            RefreshUI();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Q)) _viewModel.TriggerUseSkill(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.W)) _viewModel.TriggerUseSkill(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.E)) _viewModel.TriggerUseSkill(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.R)) _viewModel.TriggerUseSkill(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.T)) _viewModel.TriggerUseSkill(4);

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadMultiply)) _viewModel.TriggerUsePotion();
            if (Input.GetKeyDown(KeyCode.Tab)) _viewModel.TriggerSwitchTarget();
            if (Input.GetKeyDown(KeyCode.B)) _viewModel.TriggerOpenBag();
        }

        private void RefreshUI()
        {
            // 1. Top-Left Vitality
            _txtName.text = _viewModel.PlayerName;
            _txtRealm.text = $"[{_viewModel.RealmTitle}]";
            _imgHpBarFill.fillAmount = _viewModel.HpPercent;
            _txtHpValues.text = $"{_viewModel.CurrentHp:N0} / {_viewModel.MaxHp:N0}";
            _imgMpBarFill.fillAmount = _viewModel.MpPercent;
            _txtMpValues.text = $"{_viewModel.CurrentMp:N0} / {_viewModel.MaxMp:N0}";

            // 2. Top-Center Target
            if (_viewModel.HasTarget)
            {
                if (!_goTargetPanel.activeSelf) _goTargetPanel.SetActive(true);
                _txtTargetName.text = $"{_viewModel.TargetName} ({_viewModel.TargetType})";
                _imgTargetHpFill.fillAmount = _viewModel.TargetHpPercent;
                _txtTargetHpValues.text = $"{_viewModel.TargetHp:N0} / {_viewModel.TargetMaxHp:N0}";
            }
            else
            {
                if (_goTargetPanel.activeSelf) _goTargetPanel.SetActive(false);
            }

            // 3. Bottom-Right Skills
            for (int i = 0; i < 5; i++)
            {
                if (i < _viewModel.Skills.Count)
                {
                    var s = _viewModel.Skills[i];
                    _txtSkillNames[i].text = s.ShortcutKey;
                    _imgSkillCooldowns[i].fillAmount = s.CooldownPercent;
                    _goSkillSelections[i].SetActive(s.IsSelected);
                }
            }

            _txtPotionCount.text = _viewModel.PotionCount.ToString();
            _imgPotionCooldown.fillAmount = _viewModel.PotionCooldownMax > 0f
                ? Mathf.Clamp01(_viewModel.PotionCooldownRemaining / _viewModel.PotionCooldownMax)
                : 0f;

            // 4. Top-Right Map
            _txtMapInfo.text = $"{_viewModel.MapName} (Khu {_viewModel.ZoneId})";
        }

        private void BuildUI()
        {
            var rootRect = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            BuildTopLeftVitality(rootRect);
            BuildTopCenterTarget(rootRect);
            BuildBottomRightSkills(rootRect);
            BuildTopRightNavigation(rootRect);
        }

        private void BuildTopLeftVitality(RectTransform parent)
        {
            var panel = CreatePanel(parent, "VitalityPanel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(260f, 85f));
            panel.GetComponent<Image>().color = new Color(ThemeTokens.ColorDarkBackground.r, ThemeTokens.ColorDarkBackground.g, ThemeTokens.ColorDarkBackground.b, 0.85f);

            // Avatar Thai Cuc
            var avatar = CreatePanel(panel.GetComponent<RectTransform>(), "Avatar", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(10f, 0f), new Vector2(60f, 60f));
            avatar.GetComponent<Image>().color = ThemeTokens.ColorCardBackground;
            var txtAva = CreateText(avatar.GetComponent<RectTransform>(), "TxtAva", "☯", 32, ThemeTokens.ColorPrimary, TextAnchor.MiddleCenter);
            StretchFull(txtAva.rectTransform);

            // Ten & Canh Gioi
            _txtName = CreateText(panel.GetComponent<RectTransform>(), "PlayerName", "Đạo Hữu", 14, ThemeTokens.ColorTextLight, TextAnchor.MiddleLeft);
            _txtName.rectTransform.anchoredPosition = new Vector2(80f, 22f);
            _txtName.rectTransform.sizeDelta = new Vector2(100f, 20f);

            _txtRealm = CreateText(panel.GetComponent<RectTransform>(), "RealmTitle", "[Phàm Nhân]", 12, ThemeTokens.ColorSecondary, TextAnchor.MiddleLeft);
            _txtRealm.rectTransform.anchoredPosition = new Vector2(175f, 22f);
            _txtRealm.rectTransform.sizeDelta = new Vector2(80f, 20f);

            // Thanh Huyet Khi (HP)
            var hpBg = CreateBar(panel.GetComponent<RectTransform>(), "HpBar", new Vector2(80f, 0f), new Vector2(165f, 16f), ThemeTokens.ColorCardBackground, ThemeTokens.ColorAccentRed, out _imgHpBarFill, out _txtHpValues);

            // Thanh Linh Luc (MP)
            var mpBg = CreateBar(panel.GetComponent<RectTransform>(), "MpBar", new Vector2(80f, -20f), new Vector2(165f, 14f), ThemeTokens.ColorCardBackground, ThemeTokens.ColorPrimary, out _imgMpBarFill, out _txtMpValues);
        }

        private void BuildTopCenterTarget(RectTransform parent)
        {
            _goTargetPanel = CreatePanel(parent, "TargetPanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(240f, 48f));
            _goTargetPanel.GetComponent<Image>().color = new Color(ThemeTokens.ColorDarkBackground.r, ThemeTokens.ColorDarkBackground.g, ThemeTokens.ColorDarkBackground.b, 0.9f);

            _txtTargetName = CreateText(_goTargetPanel.GetComponent<RectTransform>(), "TargetName", "Mục Tiêu", 13, ThemeTokens.ColorSecondary, TextAnchor.MiddleCenter);
            _txtTargetName.rectTransform.anchoredPosition = new Vector2(0f, 11f);
            _txtTargetName.rectTransform.sizeDelta = new Vector2(220f, 18f);

            CreateBar(_goTargetPanel.GetComponent<RectTransform>(), "TargetHpBar", new Vector2(10f, -10f), new Vector2(220f, 14f), ThemeTokens.ColorCardBackground, ThemeTokens.ColorAccentRed, out _imgTargetHpFill, out _txtTargetHpValues);
            _goTargetPanel.SetActive(false);
        }

        private void BuildBottomRightSkills(RectTransform parent)
        {
            var cluster = CreatePanel(parent, "SkillCluster", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-20f, 20f), new Vector2(280f, 160f));
            cluster.GetComponent<Image>().color = Color.clear;

            // 5 Nut Ky Nang xep hinh quat
            Vector2[] skillOffsets = {
                new Vector2(-190f, 20f),   // Skill 1
                new Vector2(-140f, 60f),   // Skill 2
                new Vector2(-80f, 95f),    // Skill 3
                new Vector2(-15f, 115f),   // Skill 4
                new Vector2(40f, 40f)      // Skill 5 (Nut Danh Thuong / Thuc Chien)
            };

            for (int i = 0; i < 5; i++)
            {
                int index = i;
                float size = (i == 4) ? 64f : 50f;
                var btn = CreateButton(cluster.GetComponent<RectTransform>(), $"SkillBtn_{i}", skillOffsets[i], new Vector2(size, size), ThemeTokens.ColorCardBackground, () => _viewModel?.TriggerUseSkill(index));
                
                // Selection Border
                var sel = CreatePanel(btn.GetComponent<RectTransform>(), "Selection", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(size + 4f, size + 4f));
                sel.GetComponent<Image>().color = ThemeTokens.ColorSecondary;
                sel.transform.SetAsFirstSibling();
                _goSkillSelections[i] = sel;

                // Cooldown Mask
                var cd = CreatePanel(btn.GetComponent<RectTransform>(), "CooldownMask", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(size, size));
                var cdImg = cd.GetComponent<Image>();
                cdImg.color = new Color(0f, 0f, 0f, 0.65f);
                cdImg.type = Image.Type.Filled;
                cdImg.fillMethod = Image.FillMethod.Radial360;
                cdImg.fillAmount = 0f;
                _imgSkillCooldowns[i] = cdImg;

                _txtSkillNames[i] = CreateText(btn.GetComponent<RectTransform>(), "TxtKey", (i + 1).ToString(), (i == 4) ? 20 : 16, ThemeTokens.ColorTextLight, TextAnchor.MiddleCenter);
                StretchFull(_txtSkillNames[i].rectTransform);
            }

            // Nut Dau Than (Dan Duoc)
            var btnPotion = CreateButton(cluster.GetComponent<RectTransform>(), "BtnPotion", new Vector2(-100f, 10f), new Vector2(46f, 46f), ThemeTokens.ColorPrimary, () => _viewModel?.TriggerUsePotion());
            var pImg = CreatePanel(btnPotion.GetComponent<RectTransform>(), "PotionCd", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(46f, 46f));
            var pCd = pImg.GetComponent<Image>();
            pCd.color = new Color(0f, 0f, 0f, 0.7f);
            pCd.type = Image.Type.Filled;
            pCd.fillMethod = Image.FillMethod.Vertical;
            pCd.fillAmount = 0f;
            _imgPotionCooldown = pCd;

            _txtPotionCount = CreateText(btnPotion.GetComponent<RectTransform>(), "TxtCount", "0", 14, ThemeTokens.ColorTextLight, TextAnchor.MiddleCenter);
            StretchFull(_txtPotionCount.rectTransform);

            // Nut Doi Muc Tieu (Tab)
            var btnTab = CreateButton(cluster.GetComponent<RectTransform>(), "BtnTab", new Vector2(-20f, 10f), new Vector2(44f, 44f), ThemeTokens.ColorCardBackground, () => _viewModel?.TriggerSwitchTarget());
            var txtTab = CreateText(btnTab.GetComponent<RectTransform>(), "TxtTab", "Tab", 12, ThemeTokens.ColorSecondary, TextAnchor.MiddleCenter);
            StretchFull(txtTab.rectTransform);
        }

        private void BuildTopRightNavigation(RectTransform parent)
        {
            var panel = CreatePanel(parent, "TopRightNav", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -20f), new Vector2(220f, 42f));
            panel.GetComponent<Image>().color = new Color(ThemeTokens.ColorDarkBackground.r, ThemeTokens.ColorDarkBackground.g, ThemeTokens.ColorDarkBackground.b, 0.85f);

            _txtMapInfo = CreateText(panel.GetComponent<RectTransform>(), "MapInfo", "Bản Đồ (Khu 1)", 12, ThemeTokens.ColorTextMuted, TextAnchor.MiddleLeft);
            _txtMapInfo.rectTransform.anchoredPosition = new Vector2(10f, 0f);
            _txtMapInfo.rectTransform.sizeDelta = new Vector2(120f, 30f);

            // Nut Tui Do
            CreateButton(panel.GetComponent<RectTransform>(), "BtnBag", new Vector2(130f, 0f), new Vector2(36f, 30f), ThemeTokens.ColorCardBackground, () => _viewModel?.TriggerOpenBag());
            var txtBag = CreateText(panel.transform.Find("BtnBag").GetComponent<RectTransform>(), "TxtBag", "Túi", 11, ThemeTokens.ColorTextLight, TextAnchor.MiddleCenter);
            StretchFull(txtBag.rectTransform);

            // Nut Menu
            CreateButton(panel.GetComponent<RectTransform>(), "BtnMenu", new Vector2(175f, 0f), new Vector2(36f, 30f), ThemeTokens.ColorCardBackground, () => _viewModel?.TriggerOpenMenu());
            var txtMenu = CreateText(panel.transform.Find("BtnMenu").GetComponent<RectTransform>(), "TxtMenu", "Lệnh", 11, ThemeTokens.ColorTextLight, TextAnchor.MiddleCenter);
            StretchFull(txtMenu.rectTransform);
        }

        // --- Helper Methods ---
        private GameObject CreatePanel(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            return go;
        }

        private Button CreateButton(RectTransform parent, string name, Vector2 pos, Vector2 size, Color color, Action onClick)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            go.GetComponent<Image>().color = color;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => onClick?.Invoke());
            return btn;
        }

        private Text CreateText(RectTransform parent, string name, string text, int fontSize, Color color, TextAnchor align)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.color = color;
            t.alignment = align;
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return t;
        }

        private GameObject CreateBar(RectTransform parent, string name, Vector2 pos, Vector2 size, Color bgColor, Color fillColor, out Image fillImg, out Text txtValues)
        {
            var bg = CreatePanel(parent, name, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), pos, size);
            bg.GetComponent<Image>().color = bgColor;

            var fill = CreatePanel(bg.GetComponent<RectTransform>(), "Fill", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            StretchFull(fill.GetComponent<RectTransform>());
            fillImg = fill.GetComponent<Image>();
            fillImg.color = fillColor;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillAmount = 1f;

            txtValues = CreateText(bg.GetComponent<RectTransform>(), "Values", "0/0", 10, ThemeTokens.ColorTextLight, TextAnchor.MiddleCenter);
            StretchFull(txtValues.rectTransform);

            return bg;
        }

        private void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
