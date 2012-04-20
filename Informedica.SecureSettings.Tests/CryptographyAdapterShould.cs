using Informedica.SecureSettings.Cryptographers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace Informedica.SecureSettings.Tests
{
    [TestClass]
    public class CryptographyAdapterShould
    {
        private CryptographyAdapter _adapter;
        private SymCryptography _symCrypt;

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
                Isolate.Verify.WasCalledWithAnyArguments(() => _symCrypt.Key);
            }
            catch (System.Exception e)
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
