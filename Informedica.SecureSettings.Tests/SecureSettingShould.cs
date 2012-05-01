using Informedica.SecureSettings.Cryptographers;
using Informedica.SecureSettings.Sources;
using TypeMock.ArrangeActAssert;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class SecureSettingShould
    {
        private ISetting _fakeSetting;
        private ISetting _secureSetting;
        private SecretKeyManager _secretKeyManager;
        private const string Value = "Test";
        private CryptoGraphy _crypt;
        private const string Secret = "this is a secret key";

        [TestInitialize]
        public void IsolateSecureSettingSource()
        {
            var testSource = new TestSource {Name = Value };
            _fakeSetting = new TestSetting(testSource);
            _fakeSetting.Key = _fakeSetting.SecureMarker + Value;
            _fakeSetting.Value = _fakeSetting.SecureMarker + Value;
            Isolate.WhenCalled(() => _fakeSetting.Key = null).CallOriginal();

            _secretKeyManager = new SecretKeyManager();
            Isolate.WhenCalled(() => _secretKeyManager.GetKey()).WillReturn(Secret);
            
            _crypt = Isolate.Fake.Instance<CryptoGraphy>();
            Isolate.WhenCalled(() => _crypt.SetKey(Secret)).IgnoreCall();

            _secureSetting = new SecureSetting(_fakeSetting, _secretKeyManager, _crypt);

            Isolate.WhenCalled(() => _crypt.Encrypt(Value)).WillReturn(Value);
            var secureValue = _secureSetting.SecureMarker + Value;
            Isolate.WhenCalled(() => _crypt.Decrypt(secureValue)).WillReturn(Value);
        }

        [Isolated]
        [TestMethod]
        public void CallSecretKeyManagerToGetTheSecretKey()
        {
            try
            {
                _secureSetting.Value = "Test";
                Isolate.Verify.WasCalledWithAnyArguments(() => _secretKeyManager.GetKey());
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }            
        }

        [Isolated]
        [TestMethod]
        public void MarkAEncryptedSettingNameWithSecureMarker()
        {
            _secureSetting.Key = Value;
            var encr = _secureSetting.SecureMarker + Value;
            Isolate.Verify.WasCalledWithExactArguments(() => _fakeSetting.Key = encr);
        }

        [Isolated]
        [TestMethod]
        public void RemoveSecureMarkerWhenReadingTheKey()
        {
            _secureSetting.Key = Value;
            Assert.AreEqual(Value, _secureSetting.Key);
        }


        [Isolated]
        [TestMethod]
        public void CallCryptographyToEncryptASecureSetting()
        {
            try
            {
                _secureSetting.Key = "Test";
                Isolate.Verify.WasCalledWithAnyArguments(() => _crypt.Encrypt("Test"));
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
                Assert.AreEqual(Value, _secureSetting.Key);
                Isolate.Verify.WasCalledWithExactArguments(() => _crypt.Decrypt(Value));
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
                _secureSetting.Value = "Test";
                Isolate.Verify.WasCalledWithExactArguments(() => _crypt.SetKey(Secret));
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }            
        }

        [Isolated]
        [TestMethod]
        public void NotDecryptASettingWhenNotIsSecure()
        {
            _fakeSetting = new TestSetting(new TestSource { Name = "Test" });
            _fakeSetting.Key = "Test";
            _secureSetting = new SecureSetting(_fakeSetting, _secretKeyManager, _crypt);

            Assert.AreEqual(_fakeSetting.Key, _secureSetting.Key);
            Isolate.Verify.WasNotCalled(() => _crypt.Decrypt("Test"));
        }

    }
}
