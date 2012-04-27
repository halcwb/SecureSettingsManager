using System;
using Informedica.SecureSettings.Exceptions;
using Informedica.SecureSettings.Sources;
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

            Assert.AreEqual("Test", test.Key);
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

        [TestMethod]
        public void ThatWhenSettingNameIsMyMachineDotTestEnvironmentDotNameMachineNameIsTestMachine()
        {
            var setting = new Setting("MyMachine.TestEnvironment.Name", "TestValue", "Conn", true);

            Assert.AreEqual("MyMachine", setting.Machine);
        }

        [TestMethod]
        public void ThatWhenSettingNameIsMyMachineDotTestEnvironmentDotNameEnvironmentIsTestEnvironment()
        {
            var setting = new Setting("MyMachine.TestEnvironment.Name", "TestValue", "Conn", true);

            Assert.AreEqual("TestEnvironment", setting.Environment);
        }

        [TestMethod]
        public void ThatWhenSettingNameIsMyMachineDotTestEnvironmentDotNameNameIsName()
        {
            var setting = new Setting("MyMachine.TestEnvironment.Name", "TestValue", "Conn", true);

            Assert.AreEqual("Name", setting.Name);
        }

    }
}
