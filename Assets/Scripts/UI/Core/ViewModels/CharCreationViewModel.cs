using System;
using System.Collections.Generic;

namespace NRO.UI.ViewModels
{
    public class ExistingCharInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public sbyte Gender { get; set; }
        public long Power { get; set; }
    }

    /// <summary>
    /// ViewModel quan ly logic man hinh Chon va Tao Nhan Vat (S2).
    /// </summary>
    public class CharCreationViewModel
    {
        public string CharacterName { get; set; } = string.Empty;
        public sbyte SelectedGender { get; set; } = 0; // 0: Trai Dat / Kiem Tong, 1: Namec / Linh Toc, 2: Xayda / Ma Toc
        public int SelectedHair { get; set; } = 0;
        public string StatusMessage { get; set; } = string.Empty;

        public List<ExistingCharInfo> ExistingCharacters { get; } = new List<ExistingCharInfo>();

        public event Action<string, sbyte, int> RequestCreateCharacterEvent;
        public event Action<int> RequestSelectCharacterEvent;

        public void SetExistingCharacters(IEnumerable<ExistingCharInfo> chars)
        {
            ExistingCharacters.Clear();
            if (chars != null)
            {
                ExistingCharacters.AddRange(chars);
            }
        }

        public void SelectGender(sbyte gender)
        {
            if (gender >= 0 && gender <= 2)
            {
                SelectedGender = gender;
                SelectedHair = 0; // Reset toc ve kieu dau tien
            }
        }

        public void NextHair()
        {
            SelectedHair = (SelectedHair + 1) % 3; // Mac dinh 3 kieu toc moi he phai
        }

        public void PrevHair()
        {
            SelectedHair = (SelectedHair - 1 + 3) % 3;
        }

        public bool ValidateName(out string error)
        {
            if (string.IsNullOrEmpty(CharacterName) || CharacterName.Trim().Length < 3)
            {
                error = "Đạo hiệu phải có tối thiểu 3 ký tự.";
                return false;
            }

            if (CharacterName.Length > 15)
            {
                error = "Đạo hiệu không được vượt quá 15 ký tự.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public void SubmitCreate()
        {
            if (!ValidateName(out string error))
            {
                StatusMessage = error;
                return;
            }

            StatusMessage = "Đang ngưng tụ đạo thể...";
            RequestCreateCharacterEvent?.Invoke(CharacterName.Trim(), SelectedGender, SelectedHair);
        }

        public void SelectCharacter(int charId)
        {
            RequestSelectCharacterEvent?.Invoke(charId);
        }
    }
}
