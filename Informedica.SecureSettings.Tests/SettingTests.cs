using TypeMock.ArrangeActAssert;
using System;
using Informedica.SecureSettings.Sources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SettingTests
    {
        private ISetting _setting;
        private Type _type;
        private TestSource _fakeSource;
        private string _keyValue = "TestKey";
        private string _value = "TestValue";

        [TestInitialize]
        public void SetupSetting()
        {
            _type = typeof (TestSource);
            _fakeSource = new TestSource();
            Isolate.WhenCalled(() => _fakeSource.Name = null).CallOriginal();
            Isolate.WhenCalled(() => _fakeSource.Name).WillReturn(_keyValue);
            Isolate.WhenCalled(() => _fakeSource.ConnectionString).WillReturn(_value);

            _setting = new TestSetting(_fakeSource);
        }

        [Isolated]
        [TestMethod]
        public void ThatASourceIsUsedToGetTheKeyOfTheSetting()
        {
            try
            {
                Assert.AreEqual(_setting.Key, _fakeSource.Name);
                Isolate.Verify.WasCalledWithAnyArguments(() => _fakeSource.Name);

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }


        [Isolated]
        [TestMethod]
        public void ThatASourceIsUsedToSetTheKeyOfTheSetting()
        {
            try
            {
                _setting.Key = "New";
                Isolate.Verify.WasCalledWithExactArguments(() => _fakeSource.Name = "New");

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void ThatASourceIsUsedToGetTheValueOfTheSetting()
        {
            try
            {
                Assert.AreEqual(_setting.Value, _fakeSource.ConnectionString);
                Isolate.Verify.WasCalledWithAnyArguments(() => _fakeSource.ConnectionString);

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void ThatASourceIsUsedToSetTheValueOfTheSetting()
        {
            try
            {
                _setting.Value = "New";
                Isolate.Verify.WasCalledWithExactArguments(() => _fakeSource.ConnectionString = "New");

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }


        [Isolated]
        [TestMethod]
        public void ThatWhenSettingKeyIsNotMarkedAsSecureTheSettingIsNotEncrypted()
        {
            Assert.IsFalse(_setting.IsSecure);
        }

        [Isolated]
        [TestMethod]
        public void ThatWhenSettingKeyIsMarkedAsSecureTheSettingIsEncrypted()
        {
            _fakeSource = new TestSource();
            _setting = new TestSetting(_fakeSource);
            _fakeSource.Name = "[Secure]key";
            Assert.IsTrue(_setting.IsSecure);
        }

    }

    public class TestSetting : Setting<TestSource>
    {
        #region Overrides of Setting

        public TestSetting(TestSource source) : base(source)
        {
        }

        public override string Key
        {
            get { return Source.Name; }
            set { Source.Name = value; }
        }

        public override string Value
        {
            get { return Source.ConnectionString; }
            set { Source.ConnectionString = value; }
        }

        #endregion
    }

    public class TestSource
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
