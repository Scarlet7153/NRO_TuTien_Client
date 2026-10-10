using NUnit.Framework;
using UnityEngine;
using NRO.UI;

namespace NRO.UI.Tests
{
    [TestFixture]
    public class UiFrameworkTests
    {
        [SetUp]
        public void Setup()
        {
            UiFlags.ResetAll();
            UiRouter.Instance.ClearAll();
            ModalManager.Instance.CloseAllModals();
        }

        [TearDown]
        public void Teardown()
        {
            UiFlags.ResetAll();
            UiRouter.Instance.ClearAll();
            ModalManager.Instance.CloseAllModals();
        }

        [Test]
        public void UiFlags_Default_AllDisabled()
        {
            Assert.IsFalse(UiFlags.EnableNewSplash);
            Assert.IsFalse(UiFlags.EnableNewLogin);
            Assert.IsFalse(UiFlags.EnableNewCharSelect);
            Assert.IsFalse(UiFlags.EnableNewHud);
            Assert.IsFalse(UiFlags.EnableNewInventory);
            Assert.IsFalse(UiFlags.EnableNewDialogs);
            Assert.IsFalse(UiFlags.IsScreenNew("HUD"));
        }

        [Test]
        public void UiFlags_EnableFlag_IsScreenNewReturnsTrue()
        {
            UiFlags.EnableNewLogin = true;
            Assert.IsTrue(UiFlags.IsScreenNew("Login"));
            Assert.IsFalse(UiFlags.IsScreenNew("HUD"));
        }

        [Test]
        public void UiRouter_NavigateTo_UpdatesCurrentScreen()
        {
            var screenGo = new GameObject("LoginScreen");
            UiRouter.Instance.NavigateTo("LoginScreen", screenGo);

            Assert.AreEqual("LoginScreen", UiRouter.Instance.CurrentScreenName);
        }

        [Test]
        public void UiRouter_Back_ReturnsToPreviousScreen()
        {
            var screen1 = new GameObject("Screen1");
            var screen2 = new GameObject("Screen2");

            UiRouter.Instance.NavigateTo("Screen1", screen1);
            UiRouter.Instance.NavigateTo("Screen2", screen2);
            Assert.AreEqual("Screen2", UiRouter.Instance.CurrentScreenName);

            bool popped = UiRouter.Instance.Back();
            Assert.IsTrue(popped);
            Assert.AreEqual("Screen1", UiRouter.Instance.CurrentScreenName);
        }

        [Test]
        public void ModalManager_ShowAndClose_UpdatesActiveState()
        {
            var modalGo = new GameObject("TestModal");
            Assert.IsFalse(ModalManager.Instance.HasActiveModal);

            ModalManager.Instance.ShowModal(modalGo);
            Assert.IsTrue(ModalManager.Instance.HasActiveModal);

            ModalManager.Instance.CloseTopModal();
            Assert.IsFalse(ModalManager.Instance.HasActiveModal);
        }
    }
}
