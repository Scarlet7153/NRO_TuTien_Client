using UnityEngine;
using NRO.UI;
using NRO.UI.ViewModels;
using NRO.UI.Views;

namespace NRO.UIBridge
{
    /// <summary>
    /// Cau noi (Bridge) giua he thong HUD moi (Lat cat S3: HudView / HudViewModel)
    /// va vong lap chien dau cu cua game (GameScr, Char, TileMap).
    /// </summary>
    public static class HudBridge
    {
        private static HudViewModel _viewModel;
        private static GameObject _hudInstance;

        public static HudViewModel ViewModel => _viewModel ?? (_viewModel = CreateViewModel());

        /// <summary>
        /// Kiem tra xem co can ngan chan viec ve HUD IMGUI cu hay khong (L1 Layering Hook).
        /// Chi tra ve true khi co EnableNewHud bat va dang o man hinh GameScr.
        /// </summary>
        public static bool ShouldSuppressLegacyHud()
        {
            if (!UiFlags.EnableNewHud)
            {
                if (_hudInstance != null && _hudInstance.activeSelf)
                {
                    _hudInstance.SetActive(false);
                }
                return false;
            }

            bool isInGame = GameCanvas.currentScreen == GameScr.instance ||
                            GameCanvas.currentScreen == GameScr.gI();

            if (isInGame)
            {
                EnsureHudActive();
                UpdateHudData();
                return true;
            }

            if (_hudInstance != null && _hudInstance.activeSelf)
            {
                _hudInstance.SetActive(false);
            }
            return false;
        }

        public static void EnsureHudActive()
        {
            if (_hudInstance == null)
            {
                UiRoot.EnsureInitialized();
                _hudInstance = new GameObject("InGameHud", typeof(RectTransform), typeof(HudView));
                var view = _hudInstance.GetComponent<HudView>();
                view.Bind(ViewModel);
                UiRouter.Instance.NavigateTo("HUD", _hudInstance);
            }
            else if (!_hudInstance.activeSelf)
            {
                _hudInstance.SetActive(true);
            }
        }

        /// <summary>
        /// Dong bo du lieu tu the gioi game cu vao ViewModel moi moi frame/tick.
        /// </summary>
        public static void UpdateHudData()
        {
            var vm = ViewModel;
            var c = Char.myCharz();
            if (c == null) return;

            // 1. Thong tin nguoi choi
            vm.PlayerName = c.cName ?? string.Empty;
            vm.CurrentHp = c.cHP;
            vm.MaxHp = c.cHPFull;
            vm.CurrentMp = c.cMP;
            vm.MaxMp = c.cMPFull;
            vm.CurrentPower = c.cPower;
            vm.RealmTitle = HudViewModel.ComputeRealmTitle(c.cPower);

            // 2. Thong tin muc tieu (Target)
            if (c.mobFocus != null)
            {
                vm.HasTarget = true;
                vm.TargetName = c.mobFocus.getTemplate() != null ? c.mobFocus.getTemplate().name : "Yêu Ma";
                vm.TargetHp = c.mobFocus.hp;
                vm.TargetMaxHp = c.mobFocus.maxHp;
                vm.TargetType = "Quái";
            }
            else if (c.charFocus != null)
            {
                vm.HasTarget = true;
                vm.TargetName = c.charFocus.cName ?? "Đạo Hữu";
                vm.TargetHp = c.charFocus.cHP;
                vm.TargetMaxHp = c.charFocus.cHPFull;
                vm.TargetType = "Tu Sĩ";
            }
            else if (c.npcFocus != null)
            {
                vm.HasTarget = true;
                vm.TargetName = c.npcFocus.template != null ? c.npcFocus.template.name : "Kỳ Nhân";
                vm.TargetHp = 1L;
                vm.TargetMaxHp = 1L;
                vm.TargetType = "NPC";
            }
            else
            {
                vm.HasTarget = false;
            }

            // 3. Ngu Hanh Than Thong (5 ky nang)
            if (GameScr.keySkill != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i < GameScr.keySkill.Length && i < vm.Skills.Count)
                    {
                        var s = GameScr.keySkill[i];
                        if (s != null)
                        {
                            vm.Skills[i].SkillId = s.skillId;
                            vm.Skills[i].Name = s.template != null ? s.template.name : $"Skill {i + 1}";
                            vm.Skills[i].IsSelected = (s == c.myskill);

                            // Tinh toan cooldown
                            long now = mSystem.currentTimeMillis();
                            long coolDownMs = s.coolDown;
                            long lastTime = s.lastTimeUseThisSkill;
                            if (coolDownMs > 0 && now - lastTime < coolDownMs)
                            {
                                vm.Skills[i].CooldownRemaining = (float)(coolDownMs - (now - lastTime)) / 1000f;
                                vm.Skills[i].CooldownMax = (float)coolDownMs / 1000f;
                            }
                            else
                            {
                                vm.Skills[i].CooldownRemaining = 0f;
                            }
                        }
                    }
                }
            }

            // 4. Dau Than
            vm.PotionCount = GameScr.hpPotion;

            // 5. Ban Do
            vm.MapName = TileMap.mapName ?? string.Empty;
            vm.ZoneId = TileMap.zoneID;
            vm.PosX = c.cx;
            vm.PosY = c.cy;
        }

        private static HudViewModel CreateViewModel()
        {
            var vm = new HudViewModel();

            // Su kien thi trien ky nang
            vm.RequestUseSkillEvent += slotIndex =>
            {
                if (GameScr.keySkill != null && slotIndex >= 0 && slotIndex < GameScr.keySkill.Length)
                {
                    var skill = GameScr.keySkill[slotIndex];
                    if (skill != null && GameScr.gI() != null)
                    {
                        GameScr.gI().doSelectSkill(skill, true);
                    }
                }
            };

            // Su kien dung dau than / dan duoc
            vm.RequestUsePotionEvent += () =>
            {
                if (GameScr.gI() != null)
                {
                    GameScr.gI().doUseHP();
                }
            };

            // Su kien doi muc tieu
            vm.RequestSwitchTargetEvent += () =>
            {
                var c = Char.myCharz();
                if (c != null && GameScr.vMob != null && GameScr.vMob.size() > 0)
                {
                    // Chuyen doi muc tieu quai gan nhat
                    int curIndex = -1;
                    if (c.mobFocus != null)
                    {
                        curIndex = GameScr.vMob.indexOf(c.mobFocus);
                    }
                    int nextIndex = (curIndex + 1) % GameScr.vMob.size();
                    c.mobFocus = (Mob)GameScr.vMob.elementAt(nextIndex);
                    c.charFocus = null;
                }
            };

            // Su kien mo hanh trang
            vm.RequestOpenBagEvent += () =>
            {
                if (GameCanvas.panel != null)
                {
                    GameCanvas.panel.setTypeMessage();
                    GameCanvas.panel.show();
                }
            };

            // Su kien mo menu
            vm.RequestOpenMenuEvent += () =>
            {
                if (GameScr.gI() != null && GameScr.gI().cmdMenu != null)
                {
                    GameScr.gI().cmdMenu.performAction();
                }
            };

            return vm;
        }
    }
}
