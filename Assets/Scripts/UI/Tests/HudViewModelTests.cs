using NUnit.Framework;
using NRO.UI.ViewModels;

namespace NRO.UI.Tests
{
    [TestFixture]
    public class HudViewModelTests
    {
        [Test]
        public void Test_HpMpPercentages_CalculatedAccurately()
        {
            var vm = new HudViewModel
            {
                CurrentHp = 500L,
                MaxHp = 1000L,
                CurrentMp = 250L,
                MaxMp = 1000L
            };

            Assert.AreEqual(0.5f, vm.HpPercent, 0.001f);
            Assert.AreEqual(0.25f, vm.MpPercent, 0.001f);
        }

        [Test]
        public void Test_ZeroMaxHpMp_ReturnsZeroPercentSafely()
        {
            var vm = new HudViewModel
            {
                CurrentHp = 0L,
                MaxHp = 0L,
                CurrentMp = 0L,
                MaxMp = 0L
            };

            Assert.AreEqual(0f, vm.HpPercent);
            Assert.AreEqual(0f, vm.MpPercent);
        }

        [Test]
        public void Test_ComputeRealmTitle_MatchesPowerThresholds()
        {
            Assert.AreEqual("Phàm Nhân", HudViewModel.ComputeRealmTitle(500L));
            Assert.AreEqual("Luyện Khí Tầng 1", HudViewModel.ComputeRealmTitle(5000L));
            Assert.AreEqual("Trúc Cơ Sơ Kỳ", HudViewModel.ComputeRealmTitle(150000L));
            Assert.AreEqual("Kết Đan Kỳ", HudViewModel.ComputeRealmTitle(3000000L));
            Assert.AreEqual("Nguyên Anh Kỳ", HudViewModel.ComputeRealmTitle(50000000L));
            Assert.AreEqual("Đại Thừa Đỉnh Phong", HudViewModel.ComputeRealmTitle(5000000000L));
        }

        [Test]
        public void Test_TargetInfo_CalculatesPercentageCorrectly()
        {
            var vm = new HudViewModel
            {
                HasTarget = true,
                TargetName = "Mộc Nhân",
                TargetHp = 750L,
                TargetMaxHp = 1000L,
                TargetType = "Quái"
            };

            Assert.IsTrue(vm.HasTarget);
            Assert.AreEqual(0.75f, vm.TargetHpPercent, 0.001f);
            Assert.AreEqual("Mộc Nhân", vm.TargetName);
        }

        [Test]
        public void Test_SkillAndPotionEvents_TriggerProperly()
        {
            var vm = new HudViewModel
            {
                PotionCount = 5,
                PotionCooldownRemaining = 0f
            };

            int selectedSkill = -1;
            bool potionUsed = false;
            bool targetSwitched = false;
            bool bagOpened = false;

            vm.RequestUseSkillEvent += idx => selectedSkill = idx;
            vm.RequestUsePotionEvent += () => potionUsed = true;
            vm.RequestSwitchTargetEvent += () => targetSwitched = true;
            vm.RequestOpenBagEvent += () => bagOpened = true;

            vm.TriggerUseSkill(2);
            vm.TriggerUsePotion();
            vm.TriggerSwitchTarget();
            vm.TriggerOpenBag();

            Assert.AreEqual(2, selectedSkill);
            Assert.AreEqual(2, vm.SelectedSkillIndex);
            Assert.IsTrue(vm.Skills[2].IsSelected);
            Assert.IsFalse(vm.Skills[0].IsSelected);
            Assert.IsTrue(potionUsed);
            Assert.IsTrue(targetSwitched);
            Assert.IsTrue(bagOpened);
        }
    }
}
