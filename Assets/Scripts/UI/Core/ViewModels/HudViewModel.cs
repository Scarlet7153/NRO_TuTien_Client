using System;
using System.Collections.Generic;

namespace NRO.UI.ViewModels
{
    /// <summary>
    /// ViewModel cho ky nang trong thanh Ngu Hanh Than Thong.
    /// </summary>
    public class SkillSlotViewModel
    {
        public int SlotIndex { get; set; }
        public int SkillId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortcutKey { get; set; } = string.Empty;
        public float CooldownRemaining { get; set; }
        public float CooldownMax { get; set; }
        public bool IsSelected { get; set; }

        public bool IsReady => CooldownRemaining <= 0f;
        public float CooldownPercent => CooldownMax > 0f ? Math.Max(0f, Math.Min(1f, CooldownRemaining / CooldownMax)) : 0f;
    }

    /// <summary>
    /// ViewModel cho giao dien HUD chien dau Tu Tien (Lat cat S3).
    /// Quan ly thong tin sinh menh, muc tieu, ngu hanh ky nang va ban do.
    /// </summary>
    public class HudViewModel
    {
        // 1. Thong tin nguoi choi (Sinh Menh)
        public string PlayerName { get; set; } = string.Empty;
        public string RealmTitle { get; set; } = "Phàm Nhân";
        public long CurrentHp { get; set; }
        public long MaxHp { get; set; }
        public long CurrentMp { get; set; }
        public long MaxMp { get; set; }
        public long CurrentPower { get; set; }

        public float HpPercent => MaxHp > 0L ? Math.Max(0f, Math.Min(1f, (float)CurrentHp / MaxHp)) : 0f;
        public float MpPercent => MaxMp > 0L ? Math.Max(0f, Math.Min(1f, (float)CurrentMp / MaxMp)) : 0f;

        // 2. Thong tin muc tieu (Kinh Chieu Yeu)
        public bool HasTarget { get; set; }
        public string TargetName { get; set; } = string.Empty;
        public long TargetHp { get; set; }
        public long TargetMaxHp { get; set; }
        public string TargetType { get; set; } = string.Empty; // "Mob", "Player", "Npc"

        public float TargetHpPercent => TargetMaxHp > 0L ? Math.Max(0f, Math.Min(1f, (float)TargetHp / TargetMaxHp)) : 0f;

        // 3. Ngu Hanh Than Thong (5 ky nang) & Dau Than
        public List<SkillSlotViewModel> Skills { get; } = new List<SkillSlotViewModel>();
        public int SelectedSkillIndex { get; set; } = 0;
        public int PotionCount { get; set; }
        public float PotionCooldownRemaining { get; set; }
        public float PotionCooldownMax { get; set; } = 10f;
        public bool IsPotionReady => PotionCooldownRemaining <= 0f;

        // 4. Ban do & Khung canh
        public string MapName { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }

        // Su kien hanh dong tu UI
        public event Action<int> RequestUseSkillEvent;
        public event Action RequestUsePotionEvent;
        public event Action RequestSwitchTargetEvent;
        public event Action RequestOpenBagEvent;
        public event Action RequestOpenMenuEvent;

        public HudViewModel()
        {
            string[] defaultKeys = { "1", "2", "3", "4", "5" };
            for (int i = 0; i < 5; i++)
            {
                Skills.Add(new SkillSlotViewModel
                {
                    SlotIndex = i,
                    ShortcutKey = defaultKeys[i],
                    Name = $"Thần Thông {i + 1}"
                });
            }
        }

        public void TriggerUseSkill(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < Skills.Count)
            {
                SelectedSkillIndex = slotIndex;
                for (int i = 0; i < Skills.Count; i++)
                {
                    Skills[i].IsSelected = (i == slotIndex);
                }
                RequestUseSkillEvent?.Invoke(slotIndex);
            }
        }

        public void TriggerUsePotion()
        {
            if (PotionCount > 0 && IsPotionReady)
            {
                RequestUsePotionEvent?.Invoke();
            }
        }

        public void TriggerSwitchTarget()
        {
            RequestSwitchTargetEvent?.Invoke();
        }

        public void TriggerOpenBag()
        {
            RequestOpenBagEvent?.Invoke();
        }

        public void TriggerOpenMenu()
        {
            RequestOpenMenuEvent?.Invoke();
        }

        /// <summary>
        /// Tinh toan danh xung canh gioi dua tren chi so Tu Vi (Power).
        /// </summary>
        public static string ComputeRealmTitle(long power)
        {
            if (power < 1000L) return "Phàm Nhân";
            if (power < 10000L) return "Luyện Khí Tầng 1";
            if (power < 50000L) return "Luyện Khí Tầng 9";
            if (power < 200000L) return "Trúc Cơ Sơ Kỳ";
            if (power < 1000000L) return "Trúc Cơ Viên Mãn";
            if (power < 5000000L) return "Kết Đan Kỳ";
            if (power < 20000000L) return "Nguyên Anh Kỳ";
            if (power < 100000000L) return "Hóa Thần Kỳ";
            if (power < 500000000L) return "Luyện Hư Kỳ";
            if (power < 2000000000L) return "Hợp Thể Kỳ";
            return "Đại Thừa Đỉnh Phong";
        }
    }
}
