using System;
using System.Configuration;
using Informedica.SecureSettings.Sources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class AConnectionStringSettingShould
    {
        private ConnectionStringSettings _fakeConnStr;
        private string _testConnStrValue = "Test Connection String";
        private ISetting _setting;

        [TestInitialize]
        public void InitializeTests()
        {
            _fakeConnStr = Isolate.Fake.Instance<ConnectionStringSettings>();
            Isolate.WhenCalled(() => _fakeConnStr.Name).WillReturn("key");
            Isolate.WhenCalled(() => _fakeConnStr.ConnectionString).WillReturn(_testConnStrValue);

            _setting = new ConnectionStringSettingsAdaptor(_fakeConnStr);
        }

        [TestMethod]
        public void UseAConnectionStringSettingsToSetTheValue()
        {
            _setting.Value = _testConnStrValue;

            try
            {
                Isolate.Verify.WasCalledWithExactArguments(() => _fakeConnStr.ConnectionString = _testConnStrValue);

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void UseAConnectionStringSettingsToGetTheValue()
        {
            try
            {
                Assert.AreEqual(_testConnStrValue, _setting.Value);
                Isolate.Verify.WasCalledWithAnyArguments(() => _fakeConnStr.ConnectionString);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [TestMethod]
        public void ReturnFalseWhenAskedIfIsSecure()
        {
            Assert.IsFalse(_setting.IsSecure);
        }
    }
}
