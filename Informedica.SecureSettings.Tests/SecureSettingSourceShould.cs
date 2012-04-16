using Informedica.SecureSettings.Cryptographers;
using Informedica.SecureSettings.Sources;
using TypeMock.ArrangeActAssert;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SecureSettingSourceShould
    {
        private enum Types
        {
            App
        }

        private SecureSettingSource _secureSource;
        private Setting _fakeSetting;
        private SecretKeyManager _secretKeyManager;
        private SettingSource _source;
        private string _settingName = "Test";
        private const string Secret = "This is a secret";
        private Types _app = Types.App;
        private CryptoGraphy _crypt;
        private string _key = "this is a secret key";

        [TestInitialize]
        public void IsolateSecureSettingSource()
        {
            _fakeSetting = new Setting(_settingName, "Test", _app.ToString(), false);

            _source = Isolate.Fake.Instance<SettingSource>();
            Isolate.WhenCalled(() => _source.WriteSetting(_fakeSetting)).IgnoreCall();
            Isolate.WhenCalled(() => _source.ReadSetting(_app, _settingName)).WillReturn(_fakeSetting);
            Isolate.WhenCalled(() => _source.RemoveSetting(_fakeSetting)).IgnoreCall();

            _secretKeyManager = new SecretKeyManager();
            Isolate.WhenCalled(() => _secretKeyManager.GetKey()).WillReturn(_key);
            Isolate.WhenCalled(() => _secretKeyManager.SetKey(Secret)).IgnoreCall();

            _crypt = Isolate.Fake.Instance<CryptoGraphy>();
            var markedname = SecureSettingSource.SecureMarker + _settingName;
            Isolate.WhenCalled(() => _crypt.Encrypt(_settingName)).WillReturn(markedname);
            Isolate.WhenCalled(() => _crypt.Decrypt(_settingName)).WillReturn(markedname);

            _secureSource = new SecureSettingSource(_source, _secretKeyManager, _crypt);
        }

        [Isolated]
        [TestMethod]
        public void CallSecretKeyManagerToGetTheSecretKey()
        {
            try
            {
                _secureSource.GetSecretKey();
                Isolate.Verify.WasCalledWithAnyArguments(() => _secretKeyManager.GetKey());
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }            
        }

        [Isolated]
        [TestMethod]
        public void CallSecretKeyManagerToSetTheSecretKey()
        {
            try
            {
                _secureSource.SetSecretKey(Secret);
                Isolate.Verify.WasCalledWithExactArguments(() => _secretKeyManager.SetKey(Secret));

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallSettingSourceToWriteASetting()
        {
            try
            {
                _secureSource.WriteSetting(_fakeSetting);
                Isolate.Verify.WasCalledWithExactArguments(() => _source.WriteSetting(_fakeSetting));

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallSettingSourceToReadASetting()
        {
            try
            {
                _secureSource.ReadSetting(_app, _settingName);
                Isolate.Verify.WasCalledWithExactArguments(() => _source.ReadSetting(_app, _settingName));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void ReturnASettingWithTheSameNameAndValueAfterWritingThatSettingAndReadingIt()
        {
            _secureSource.WriteSetting(_fakeSetting);
            var setting = _secureSource.ReadSetting(_app, _fakeSetting.Name);

            Assert.AreEqual(_fakeSetting.Name, setting.Name);
            Assert.AreEqual(_fakeSetting.Value, setting.Value);
            Assert.AreEqual(_fakeSetting.Type, setting.Type);
        }

        [Isolated]
        [TestMethod]
        public void ReturnASettingWithTheSameNameAfterSecureWritingThatSettingAndReadingIt()
        {
            _secureSource.WriteSecure(_fakeSetting);
            var setting = _secureSource.ReadSecure(_app, _fakeSetting.Name);

            Assert.AreEqual(_fakeSetting.Name, setting.Name);
        }

        [Isolated]
        [TestMethod]
        public void CallSettingSourceToRemoveASetting()
        {
            try
            {
                _secureSource.RemoveSetting(_fakeSetting);
                Isolate.Verify.WasCalledWithExactArguments(() => _source.RemoveSetting(_fakeSetting));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
           }
        }

        [Isolated]
        [TestMethod]
        public void CallCryptographyToEncryptASecureSetting()
        {
            try
            {
                _secureSource.WriteSecure(_fakeSetting);
                Isolate.Verify.WasCalledWithExactArguments(() => _crypt.Encrypt(_fakeSetting.Name));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void CallCryptographyToDecryptASecureSetting()
        {
            try
            {
                _secureSource.ReadSecure(_app, _settingName);
                Isolate.Verify.WasCalledWithExactArguments(() => _crypt.Decrypt(_settingName));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void SetASecretKeyOnCryptographyWhenUsingCryptography()
        {
            try
            {
                _secureSource.ReadSecure(_app, _settingName);
                Isolate.Verify.WasCalledWithExactArguments(() => _crypt.SetKey(_key));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }            
        }

    }
}
