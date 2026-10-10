using NUnit.Framework;
using NRO.UI;

namespace NRO.UI.Tests
{
    [TestFixture]
    public class LocalizerTests
    {
        private Localizer _localizer;

        [SetUp]
        public void Setup()
        {
            _localizer = new Localizer();
        }

        [Test]
        public void Get_UnknownKey_ReturnsKeyItself()
        {
            string result = _localizer.Get("unknown_key");
            Assert.AreEqual("unknown_key", result);
        }

        [Test]
        public void Get_NullOrEmptyKey_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, _localizer.Get(null));
            Assert.AreEqual(string.Empty, _localizer.Get(""));
        }

        [Test]
        public void LoadFromJson_EmptyJson_DoesNotThrowAndMaintainsFallback()
        {
            _localizer.SetFallbackProvider(k => k == "btn_ok" ? "Dong Y" : null);

            _localizer.LoadFromJson("{}");

            Assert.AreEqual("Dong Y", _localizer.Get("btn_ok"));
            Assert.AreEqual("btn_cancel", _localizer.Get("btn_cancel"));
        }

        [Test]
        public void LoadFromJson_ValidJson_OverridesAndRetrievesCorrectly()
        {
            string json = "{\"hello\": \"Xin chao\", \"score\": \"Diem so\"}";
            _localizer.LoadFromJson(json);

            Assert.AreEqual("Xin chao", _localizer.Get("hello"));
            Assert.AreEqual("Diem so", _localizer.Get("score"));
        }

        [Test]
        public void Get_WithFallbackProvider_InvokesProviderWhenKeyNotInDictionary()
        {
            _localizer.RegisterString("direct_key", "Gia tri truc tiep");
            _localizer.SetFallbackProvider(k => k == "fallback_key" ? "Gia tri du phong" : null);

            Assert.AreEqual("Gia tri truc tiep", _localizer.Get("direct_key"));
            Assert.AreEqual("Gia tri du phong", _localizer.Get("fallback_key"));
            Assert.AreEqual("missing_key", _localizer.Get("missing_key"));
        }

        [Test]
        public void Format_ValidArgs_FormatsCorrectly()
        {
            _localizer.RegisterString("welcome_user", "Xin chao {0}, cap do {1}");

            string formatted = _localizer.Format("welcome_user", "Tu Si", 99);
            Assert.AreEqual("Xin chao Tu Si, cap do 99", formatted);
        }

        [Test]
        public void Format_MismatchedArgs_DoesNotThrowAndReturnsTemplate()
        {
            _localizer.RegisterString("bad_format", "Thong bao: {5}");

            // Chi truyen 1 tham so trong khi template yeu cau {5}
            string result = _localizer.Format("bad_format", "Test");
            Assert.AreEqual("Thong bao: {5}", result);
        }
    }
}
