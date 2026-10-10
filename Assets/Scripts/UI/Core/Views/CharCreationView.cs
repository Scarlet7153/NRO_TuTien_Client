using UnityEngine;
using UnityEngine.UI;
using NRO.UI.ViewModels;

namespace NRO.UI.Views
{
    /// <summary>
    /// Giao dien uGUI Greybox cho Man hinh Tao Nhan Vat (S2).
    /// </summary>
    public class CharCreationView : MonoBehaviour
    {
        private CharCreationViewModel _viewModel;

        private InputField _nameInput;
        private Button[] _genderButtons = new Button[3];
        private Text _genderDescText;
        private Text _hairText;
        private Text _statusText;

        public void Bind(CharCreationViewModel vm)
        {
            _viewModel = vm;
            UpdateGenderUI();
            UpdateHairUI();
        }

        private void Awake()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            var rt = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            var bgImg = gameObject.AddComponent<Image>();
            bgImg.color = ThemeTokens.ColorDarkBg;

            // Card chinh
            var cardGo = new GameObject("CreationCard", typeof(RectTransform), typeof(Image));
            cardGo.transform.SetParent(transform, false);
            var cardRt = cardGo.GetComponent<RectTransform>();
            cardRt.sizeDelta = new Vector2(460, 500);
            cardRt.anchoredPosition = Vector2.zero;
            var cardImg = cardGo.GetComponent<Image>();
            cardImg.color = ThemeTokens.ColorCardBg;

            CreateText(cardGo.transform, "Title", "KHỞI TẠO ĐẠO THỂ", 22, ThemeTokens.ColorSecondary, new Vector2(0, 205), new Vector2(400, 35));

            // 3 Nut Mon Phai
            string[] factions = { "Kiếm Tông (Nhân)", "Linh Tộc (Thần)", "Ma Tộc (Chiến)" };
            for (int i = 0; i < 3; i++)
            {
                sbyte gender = (sbyte)i;
                float xOffset = -130 + i * 130;
                var btnGo = CreateButton(cardGo.transform, $"GenderBtn_{i}", factions[i], ThemeTokens.ColorCardBorder, new Vector2(xOffset, 140), new Vector2(120, 38));
                _genderButtons[i] = btnGo.GetComponent<Button>();
                _genderButtons[i].onClick.AddListener(() =>
                {
                    if (_viewModel != null)
                    {
                        _viewModel.SelectGender(gender);
                        UpdateGenderUI();
                        UpdateHairUI();
                    }
                });
            }

            _genderDescText = CreateText(cardGo.transform, "DescText", "Thái Hư Kiếm Tông — Phong Lôi Kiếm Ý", 12, ThemeTokens.ColorTextSecondary, new Vector2(0, 95), new Vector2(400, 25));

            // Chon Kieu Toc
            CreateText(cardGo.transform, "HairLabel", "Diện Mạo / Kiểu Tóc:", 13, ThemeTokens.ColorTextPrimary, new Vector2(0, 50), new Vector2(200, 25));
            var prevHairBtn = CreateButton(cardGo.transform, "PrevHair", "<", ThemeTokens.ColorCardBorder, new Vector2(-60, 15), new Vector2(40, 35));
            prevHairBtn.GetComponent<Button>().onClick.AddListener(() => { if (_viewModel != null) { _viewModel.PrevHair(); UpdateHairUI(); } });

            _hairText = CreateText(cardGo.transform, "HairVal", "Mẫu 1", 13, ThemeTokens.ColorSecondary, new Vector2(0, 15), new Vector2(70, 35));

            var nextHairBtn = CreateButton(cardGo.transform, "NextHair", ">", ThemeTokens.ColorCardBorder, new Vector2(60, 15), new Vector2(40, 35));
            nextHairBtn.GetComponent<Button>().onClick.AddListener(() => { if (_viewModel != null) { _viewModel.NextHair(); UpdateHairUI(); } });

            // Input Ten Nhan Vat
            _nameInput = CreateInputField(cardGo.transform, "NameInput", "Nhập đạo hiệu tu sĩ...", new Vector2(0, -50), new Vector2(340, ThemeTokens.InputFieldHeight));
            _nameInput.onValueChanged.AddListener(val => { if (_viewModel != null) _viewModel.CharacterName = val; });

            // Nut Tao Nhan Vat
            var submitBtnGo = CreateButton(cardGo.transform, "SubmitBtn", "NGƯNG TỤ KIM THÂN", ThemeTokens.ColorPrimary, new Vector2(0, -120), new Vector2(340, ThemeTokens.ButtonHeightLarge));
            submitBtnGo.GetComponent<Button>().onClick.AddListener(OnSubmitClicked);

            _statusText = CreateText(cardGo.transform, "Status", "", 12, ThemeTokens.ColorAccentRed, new Vector2(0, -170), new Vector2(340, 25));
        }

        private void OnSubmitClicked()
        {
            if (_viewModel != null)
            {
                _viewModel.SubmitCreate();
                _statusText.text = _viewModel.StatusMessage;
            }
        }

        private void UpdateGenderUI()
        {
            if (_viewModel == null) return;
            string[] descs = {
                "Thái Hư Kiếm Tông — Nhân Tộc dũng cảm, kiếm pháp tuyệt luân.",
                "Trường Sinh Linh Tộc — Thần Tộc nhân hậu, linh lực trị thương dồi dào.",
                "Cửu U Ma Tộc — Chiến Ma cuồng bạo, thể chất kim cương bất hoại."
            };
            _genderDescText.text = descs[_viewModel.SelectedGender];

            for (int i = 0; i < 3; i++)
            {
                var img = _genderButtons[i].GetComponent<Image>();
                img.color = (i == _viewModel.SelectedGender) ? ThemeTokens.ColorSecondary : ThemeTokens.ColorCardBorder;
            }
        }

        private void UpdateHairUI()
        {
            if (_viewModel == null || _hairText == null) return;
            _hairText.text = $"Mẫu {_viewModel.SelectedHair + 1}";
        }

        private Text CreateText(Transform parent, string name, string text, int size, Color color, Vector2 pos, Vector2 sizeDelta)
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
            return txt;
        }

        private GameObject CreateButton(Transform parent, string name, string label, Color bgColor, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var img = go.GetComponent<Image>();
            img.color = bgColor;
            CreateText(go.transform, "Label", label, 13, Color.white, Vector2.zero, sizeDelta);
            return go;
        }

        private InputField CreateInputField(Transform parent, string name, string placeholder, Vector2 pos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = sizeDelta;
            var img = go.GetComponent<Image>();
            img.color = new Color(0.06f, 0.05f, 0.09f, 0.9f);

            var input = go.GetComponent<InputField>();
            var text = CreateText(go.transform, "Text", "", 13, ThemeTokens.ColorTextPrimary, Vector2.zero, sizeDelta - new Vector2(20, 0));
            text.alignment = TextAnchor.MiddleLeft;

            var phText = CreateText(go.transform, "Placeholder", placeholder, 13, ThemeTokens.ColorTextMuted, Vector2.zero, sizeDelta - new Vector2(20, 0));
            phText.alignment = TextAnchor.MiddleLeft;

            input.textComponent = text;
            input.placeholder = phText;
            return input;
        }
    }
}
