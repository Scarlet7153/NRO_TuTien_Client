using NUnit.Framework;
using NRO.UI.ViewModels;
using System.Collections.Generic;

namespace NRO.UI.Tests
{
    [TestFixture]
    public class SliceViewModelsTests
    {
        [Test]
        public void GatewayViewModel_Validation_FailsOnShortCredentials()
        {
            var vm = new GatewayViewModel
            {
                Username = "ab",
                Password = "12"
            };

            bool valid = vm.ValidateCredentials(out string error);
            Assert.IsFalse(valid);
            Assert.IsTrue(error.Contains("ít nhất 3 ký tự"));
        }

        [Test]
        public void GatewayViewModel_Validation_PassesOnValidCredentials()
        {
            var vm = new GatewayViewModel
            {
                Username = "daohu7153",
                Password = "matkhauhople"
            };

            bool valid = vm.ValidateCredentials(out string error);
            Assert.IsTrue(valid);
            Assert.IsEmpty(error);
        }

        [Test]
        public void GatewayViewModel_SubmitLogin_TriggersEventWhenValid()
        {
            var vm = new GatewayViewModel
            {
                Username = "testacc",
                Password = "testpass"
            };

            bool eventFired = false;
            vm.RequestLoginEvent += (u, p) =>
            {
                eventFired = true;
                Assert.AreEqual("testacc", u);
                Assert.AreEqual("testpass", p);
            };

            vm.SubmitLogin();
            Assert.IsTrue(eventFired);
            Assert.IsTrue(vm.IsLoading);
        }

        [Test]
        public void GatewayViewModel_SelectServer_UpdatesCurrentServer()
        {
            var vm = new GatewayViewModel();
            var servers = new List<ServerInfo>
            {
                new ServerInfo { Id = 0, Name = "Server 1" },
                new ServerInfo { Id = 1, Name = "Server 2" }
            };

            vm.SetServers(servers, 0);
            Assert.AreEqual("Server 1", vm.CurrentServer.Name);

            vm.SelectServer(1);
            Assert.AreEqual("Server 2", vm.CurrentServer.Name);
        }

        [Test]
        public void CharCreationViewModel_Validation_FailsOnInvalidName()
        {
            var vm = new CharCreationViewModel { CharacterName = "a" };
            Assert.IsFalse(vm.ValidateName(out string errShort));

            vm.CharacterName = "TenNhanVatNayQuaDaiKhongHopLeTheoQuyDinh";
            Assert.IsFalse(vm.ValidateName(out string errLong));

            vm.CharacterName = "BachTieuThuan";
            Assert.IsTrue(vm.ValidateName(out string errValid));
            Assert.IsEmpty(errValid);
        }

        [Test]
        public void CharCreationViewModel_HairCycling_CyclesWithinBounds()
        {
            var vm = new CharCreationViewModel();
            Assert.AreEqual(0, vm.SelectedHair);

            vm.NextHair();
            Assert.AreEqual(1, vm.SelectedHair);

            vm.NextHair();
            Assert.AreEqual(2, vm.SelectedHair);

            vm.NextHair();
            Assert.AreEqual(0, vm.SelectedHair); // Xoay vong ve 0

            vm.PrevHair();
            Assert.AreEqual(2, vm.SelectedHair); // Xoay nguoc ve 2
        }

        [Test]
        public void CharCreationViewModel_SelectGender_ResetsHair()
        {
            var vm = new CharCreationViewModel();
            vm.NextHair();
            Assert.AreEqual(1, vm.SelectedHair);

            vm.SelectGender(2); // Chon Ma Toc
            Assert.AreEqual(2, vm.SelectedGender);
            Assert.AreEqual(0, vm.SelectedHair); // Reset ve toc 0
        }
    }
}
