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
            _symCrypt.Key = Key;
            return _symCrypt.Encrypt(value);
        }

        public override string Decrypt(string value)
        {
            _symCrypt.Key = Key;
            return _symCrypt.Decrypt(value);
        }

        #endregion
    }
}
