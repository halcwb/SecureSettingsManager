using System;
using Informedica.SecureSettings.Cryptographers;

namespace Informedica.SecureSettings.Sources
{
    public class SecureSetting : ISetting
    {
        private ISetting _setting;
        private SecretKeyManager _manager;
        private CryptoGraphy _crypto;

        public SecureSetting(ISetting setting, SecretKeyManager manager, CryptoGraphy crypto)
        {
            _setting = setting;
            _manager = manager;
            _crypto = crypto;
        }

        #region Implementation of ISetting

        public string Key
        {
            get { return Decrypt(_setting.Key); }
            set {  _setting.Key = Encrypt(value); }
        }
        public string Value
        {
            get { return Crypto.Decrypt(_setting.Value); }
            set { _setting.Value = Crypto.Encrypt(value); }
        }

        public Type Type
        {
            get { return _setting.Type; }
        }

        public bool IsSecure
        {
            get { return _setting.IsSecure; }
        }

        public string SecureMarker
        {
            get { return _setting.SecureMarker; }
        }

        #endregion

        #region Encryption

        private string Encrypt(string value)
        {
            if (!IsSecure) return value;
            return AddSecureMarker(Crypto.Encrypt(value));
        }

        private string AddSecureMarker(string encrypted)
        {
            return string.IsNullOrWhiteSpace(encrypted) ? string.Empty : SecureMarker + encrypted;
        }

        private string Decrypt(string value)
        {
            if (!IsSecure) return value;
            return Crypto.Decrypt(RemoveSecureMarker(value));
        }

        private string RemoveSecureMarker(string encrypted)
        {
            return string.IsNullOrWhiteSpace(encrypted) ? string.Empty : encrypted.Replace(SecureMarker, string.Empty);
        }

        private CryptoGraphy Crypto
        {
            get
            {
                _crypto.SetKey(_manager.GetKey());
                return _crypto;
            }
        }

        #endregion
    }
}