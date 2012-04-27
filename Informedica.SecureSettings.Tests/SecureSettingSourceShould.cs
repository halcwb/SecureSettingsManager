using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Informedica.SecureSettings.Cryptographers;
using Informedica.SecureSettings.Sources;
using Informedica.SecureSettings.Testing;
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

        private ICollection<Setting> _secureSource;
        private Setting _fakeSetting;
        private SecretKeyManager _secretKeyManager;
        private ICollection<Setting> _source;
        private string _settingName = "Test";
        private const string Secret = "This is a secret";
        private Types _app = Types.App;
        private CryptoGraphy _crypt;
        private string _key = "this is a secret key";

        [TestInitialize]
        public void IsolateSecureSettingSource()
        {
            _fakeSetting = new Setting(_settingName, "Test", _app.ToString(), false);

            _source = new MyTestSettingSource();
            _source.Add(new Setting(_settingName, "Test", _app.ToString(), false));
            Isolate.WhenCalled(() => _source.GetEnumerator()).CallOriginal();

            _secretKeyManager = new SecretKeyManager();
            Isolate.WhenCalled(() => _secretKeyManager.GetKey()).WillReturn(_key);
            Isolate.WhenCalled(() => _secretKeyManager.SetKey(Secret)).IgnoreCall();

            _crypt = Isolate.Fake.Instance<CryptoGraphy>();
            var markedname = SecureSettingSource.SecureMarker + _settingName;
            Isolate.WhenCalled(() => _crypt.Encrypt(_settingName)).WillReturn(markedname);
            Isolate.WhenCalled(() => _crypt.Decrypt(_settingName)).WillReturn(_settingName);

            _secureSource = new SecureSettingSource(_source, _secretKeyManager, _crypt);
        }

        [Isolated]
        [TestMethod]
        public void CallSecretKeyManagerToGetTheSecretKey()
        {
            ReadSetting();
            try
            {
                Isolate.Verify.WasCalledWithAnyArguments(() => _secretKeyManager.GetKey());
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
                _secureSource.Add(_fakeSetting);
                Isolate.Verify.WasCalledWithAnyArguments(() => _source.Add(_fakeSetting));

            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Isolated]
        [TestMethod]
        public void UseSettingSourceWithReadASetting()
        {
            Isolate.WhenCalled(() => _source.GetEnumerator()).CallOriginal();
            try
            {
                _secureSource.SingleOrDefault(s => s.Type == _app.ToString() && s.Name == _settingName);
                Isolate.Verify.WasCalledWithAnyArguments(() => _source.GetEnumerator());
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
            _secureSource.Add(_fakeSetting);
            var setting = ReadSetting();

            Assert.AreEqual(_fakeSetting.Name, setting.Name);
            Assert.AreEqual(_fakeSetting.Value, setting.Value);
            Assert.AreEqual(_fakeSetting.Type, setting.Type);
        }

        [Isolated]
        [TestMethod]
        public void ReturnASettingWithTheSameNameAfterSecureWritingThatSettingAndReadingIt()
        {
            _secureSource.Add(_fakeSetting);
            var setting = ReadSetting();

            Assert.AreEqual(_fakeSetting.Name, setting.Name);
        }

        [Isolated]
        [TestMethod]
        public void CallSettingSourceToRemoveASetting()
        {
            try
            {
                _secureSource.Remove(_fakeSetting);
                Isolate.Verify.WasCalledWithExactArguments(() => _source.Remove(_fakeSetting));
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
                _secureSource.Add(_fakeSetting);
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
                ReadSetting();
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
                ReadSetting();
                Isolate.Verify.WasCalledWithExactArguments(() => _crypt.SetKey(_key));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }            
        }


        private Setting ReadSetting()
        {
            return _secureSource.SingleOrDefault(s => s.Type == _app.ToString() && s.Name == _fakeSetting.Name);
        }
    }
}
