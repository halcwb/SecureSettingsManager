using TypeMock.ArrangeActAssert;
using Informedica.SecureSettings;
using System;
using Informedica.SecureSettings.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SecureSettinsManagerShould
    {
        private const string Secret = "This is a secret";

        [Isolated]
        [TestMethod]
        public void BeAbleToInitialize()
        {
            ISettingSource fakeISettingSource = Isolate.Fake.Instance<ISettingSource>();
            try
            {
                new SecureSettingsManager(fakeISettingSource);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void SecureSettingsManagerShouldBeAbleToGetAndSetSecret()
        {
            var man = new SecureSettingsManager(GetFakeSource());
            man.SetSecret(Secret);

            Assert.IsTrue(man.HasSecret(Secret));
        }

        [TestMethod]
        public void SecureSettingsManagerShouldWriteToASettingSource()
        {
            var source = GetFakeSource();
            var man = new SecureSettingsManager(source);

            man.SetSetting("test", "test");
            Assert.AreEqual("test", source.ReadAppSetting("test"));
        }

        [TestMethod]
        public void BeAbleToRemoveASettingFromTheSettingSource()
        {
            var source = GetFakeSource();
            var man = new SecureSettingsManager(source);

            man.SetSetting("test", "test");
            man.RemoveSetting("test");

            try
            {
                man.GetSetting("test");
                Assert.Fail("Test setting should no longer exist");
            }
            catch (Exception e)
            {
                Assert.IsNotInstanceOfType(e, typeof(AssertFailedException));
           }
        }

        [TestMethod]
        public void BeAbleToAddAndRemoveAnSecureSetting()
        {
            var source = GetFakeSource();
            var man = new SecureSettingsManager(source);

            man.WriteSecureSetting("test", "test");
            man.ReadSecureSetting("test");

            man.RemoveSecureSetting("test");

            try
            {
                man.ReadSecureSetting("test");
                Assert.Fail("Secure setting should no longer exist");
            }
            catch (Exception e)
            {
                Assert.IsNotInstanceOfType(e, typeof(AssertFailedException));
            }
        }

        [TestMethod]
        public void BeAbleToAddAndRemoveAConnectionString()
        {
            var source = GetFakeSource();
            var man = new SecureSettingsManager(source);

            man.SetConnectionString("test", "test");
            man.RemoveConnectionString("test");

            try
            {
                man.GetConnectionString("test");
                Assert.Fail("ConnectionString should not exist anymore");
            }
            catch (Exception e)
            {
                Assert.IsNotInstanceOfType(e, typeof(AssertFailedException));
            }

        }

        private static ISettingSource GetFakeSource()
        {
            return new TestSettingSource();
        }
    }
}
