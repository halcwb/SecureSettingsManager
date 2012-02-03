﻿using TypeMock.ArrangeActAssert;
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

        private static ISettingSource GetFakeSource()
        {
            return new TestSettingSource();
        }
    }
}