using System;

namespace Informedica.SecureSettings.Cryptographers
{
    public class CryptographyAdapter: CryptoGraphy
    {
        private readonly SymCryptography _symCrypt;

        public CryptographyAdapter(SymCryptography symCrypt)
        {
            _symCrypt = symCrypt;
        }

        #region Overrides of CryptoGraphy

        public override string Encrypt(string value)
        {
            CheckIfKeyIsNullOrWhiteSpace();
            _symCrypt.Key = Key;
            try
            {
                return _symCrypt.Encrypt(value);

            }
            catch (Exception)
            {
                return value;
            }
        }

        private void CheckIfKeyIsNullOrWhiteSpace()
        {
            if (string.IsNullOrWhiteSpace(Key)) throw new Exception("Key cannot be null");
        }

        public override string Decrypt(string value)
        {
            CheckIfKeyIsNullOrWhiteSpace();
            _symCrypt.Key = Key;

            try
            {
                return _symCrypt.Decrypt(value) ?? value;

            }
            catch (Exception)
            {
                return value;
            }
        }

        #endregion
    }
}
