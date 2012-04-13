using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SettingTests
    {
        [TestMethod]
        public void ThatSettingCanBeInstantiatedWithNameTestValueTestTypeTestAndIsEncrytedIsFalse()
        {
            var test = new Setting("Test", "Test", "Test", false);

            Assert.AreEqual("Test", test.Name);
            Assert.AreEqual("Test", test.Value);
            Assert.AreEqual("Test", test.Type);
            Assert.IsFalse(test.IsEncrypted);
        }

        [TestMethod]
        public void ThatSettingCannotBeCreatedWithoutAName()
        {
            try
            {
                new Setting(string.Empty, "Test", "Test", false);
                Assert.Fail("Setting cannot be created without a name");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(StringCannotBeNullOrWhiteSpaceException), e.ToString());
            }
        }

        [TestMethod]
        public void ThatSettingCannotBeCreatedWithoutAType()
        {
            try
            {
                new Setting("Test", string.Empty, string.Empty, false);
                Assert.Fail("Setting cannot be created without a type");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(StringCannotBeNullOrWhiteSpaceException), e.ToString());    
            }
        }

    }
}
