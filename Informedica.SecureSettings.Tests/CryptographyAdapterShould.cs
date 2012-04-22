using System;
using Informedica.SecureSettings.Cryptographers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class CryptographyAdapterShould
    {
        private CryptoGraphy _adapter;
        private SymCryptography _symCrypt;

        [Isolated]
        [TestMethod]
        public void ReturnTheOriginalValueWhenDecryptionDoesNotSucceed()
        {
            _adapter = CryptographyFactory.GetCryptography();
            _adapter.SetKey("Some secret key");

            var value = "UnobtrusiveJavaScriptEnabled=";
            var decrypted = _adapter.Decrypt(value);

            Assert.AreEqual(value, decrypted);
        }


        [Isolated]
        [TestMethod]
        public void ReturnTheOriginalValueWhenDecryptionReturnsANullValue()
        {
            SetupIsolatedCryptographyAdapter();
            var value = "UnobtrusiveJavaScriptEnabled";
            Isolate.WhenCalled(() => _symCrypt.Decrypt(value)).WillReturn(null);
            _adapter.SetKey("Some secret key");

            var decrypted = _adapter.Decrypt(value);

            Assert.AreEqual(value, decrypted);
        }

        [Isolated]
        [TestMethod]
        public void ThrowAnErrorWhenEncryptWithoutAKey()
        {
            SetupIsolatedCryptographyAdapter();

            try
            {
                _adapter.Encrypt("value");
                Assert.Fail("Error was not thrown whan encrypting with an empty key");

            }
            catch (Exception e)
            {
                Assert.IsNotInstanceOfType(e, typeof(AssertFailedException));
            }
        }

        [Isolated]
        [TestMethod]
        public void ThrowAnErrorWhenDecryptWithoutAKey()
        {
            SetupIsolatedCryptographyAdapter();

            try
            {
                _adapter.Decrypt("value");
                Assert.Fail("Error was not thrown whan encrypting with an empty key");

            }
            catch (Exception e)
            {
                Assert.IsNotInstanceOfType(e, typeof(AssertFailedException));
            }
        }

        [Isolated]
        [TestMethod]
        public void UseSymCryptographyToSetAKey()
        {
            SetupIsolatedCryptographyAdapter();

            _adapter.SetKey("key");
            var encrypted = _adapter.Encrypt("Test");

            try
            {
                Assert.IsFalse(encrypted == "Test");
                Isolate.Verify.WasCalledWithAnyArguments(() => _symCrypt.Key = null);
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }


        [Isolated]
        [TestMethod]
        public void UseSymCryptographyToEncryptAValue()
        {
            SetupIsolatedCryptographyAdapter();

            _adapter.SetKey("key");
            _adapter.Encrypt("value");
            Isolate.Verify.WasCalledWithExactArguments(() =>_symCrypt.Encrypt("value"));
        }

        [Isolated]
        [TestMethod]
        public void UseSymCryptographyToDecryptAValue()
        {
            SetupIsolatedCryptographyAdapter();
            _adapter.SetKey("key");
            var value = _adapter.Encrypt("value");
            _adapter.Decrypt(value);
            Isolate.Verify.WasCalledWithExactArguments(() => _symCrypt.Decrypt(value));
        }

        private void SetupIsolatedCryptographyAdapter()
        {
            _symCrypt = new SymCryptography();
            Isolate.WhenCalled(() => _symCrypt.Encrypt("Test")).CallOriginal();
            Isolate.WhenCalled(() => _symCrypt.Key = "key").CallOriginal();
            Isolate.WhenCalled(() => _symCrypt.Key).CallOriginal();
            _adapter = new CryptographyAdapter(_symCrypt);
        }
    }
}
