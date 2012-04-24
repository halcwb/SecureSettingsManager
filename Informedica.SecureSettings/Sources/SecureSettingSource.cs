using System;
using System.Collections;
using System.Collections.Generic;
using Informedica.SecureSettings.Cryptographers;

namespace Informedica.SecureSettings.Sources
{
    public class SecureSettingSource: ISettingSource
    {
        public static readonly string SecureMarker = "[Secure]";
        private readonly SettingSource _source;
        private readonly SecretKeyManager _secretKeyManager;
        private readonly CryptoGraphy _cryptographer;

        public SecureSettingSource(SettingSource source, SecretKeyManager secretKeyManager, CryptoGraphy crypt)
        {
            _source = source;
            _secretKeyManager = secretKeyManager;
            _cryptographer = crypt;
        }

        private static string RemoveSecureMarker(string name)
        {
            return name.Replace(SecureMarker, string.Empty);
        }

        public void RemoveSetting(Setting setting)
        {
            _source.RemoveSetting(setting);
        }

        public string GetSecretKey()
        {
            return  _secretKeyManager.GetKey();
        }

        public void SetSecretKey(string secret)
        {
            _secretKeyManager.SetKey(secret);
        }

        public void WriteSetting(Setting setting)
        {
            var encrypted = new Setting(CryptoGrapher.Encrypt(setting.Name),
                                        CryptoGrapher.Encrypt(setting.Value), 
                                        setting.Type, 
                                        true);

            _source.WriteSetting(encrypted);
        }

        public Setting ReadSetting(Enum settingType, string settingName)
        {
            var setting = _source.ReadSetting(settingType, _cryptographer.Encrypt(settingName));
            var decrypted = new Setting(RemoveSecureMarker(CryptoGrapher.Decrypt(setting.Name)),
                                        CryptoGrapher.Decrypt(setting.Value),
                                        setting.Type, 
                                        true);

            return decrypted;
        }

        public CryptoGraphy CryptoGrapher
        {
            get
            {
                _cryptographer.SetKey(_secretKeyManager.GetKey());
                return _cryptographer;
            }
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Setting> GetEnumerator()
        {
            var sets = new List<Setting>();
            foreach (var setting in _source)
            {
                try
                {
                    sets.Add(new Setting(CryptoGrapher.Decrypt(setting.Name), CryptoGrapher.Decrypt(setting.Value), setting.Type, true));

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return sets.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}