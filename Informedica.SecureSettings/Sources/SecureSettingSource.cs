using System;
using System.Collections;
using System.Collections.Generic;
using Informedica.SecureSettings.Cryptographers;

namespace Informedica.SecureSettings.Sources
{
    public class SecureSettingSource: ICollection<Setting>
    {
        public static readonly string SecureMarker = "[Secure]";
        private readonly ICollection<Setting> _source;
        private readonly SecretKeyManager _secretKeyManager;
        private readonly CryptoGraphy _cryptographer;

        public SecureSettingSource(ICollection<Setting> source, SecretKeyManager secretKeyManager, CryptoGraphy crypt)
        {
            _source = source;
            _secretKeyManager = secretKeyManager;
            _cryptographer = crypt;
        }

        private static string RemoveSecureMarker(string name)
        {
            return name.Replace(SecureMarker, string.Empty);
        }

        public bool RemoveSetting(Setting setting)
        {
            return _source.Remove(setting);
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
            var encrypted = new Setting(CryptoGrapher.Encrypt(setting.Key),
                                        CryptoGrapher.Encrypt(setting.Value), 
                                        setting.Type, 
                                        true);

            _source.Add(encrypted);
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
                    sets.Add(new Setting(RemoveSecureMarker(CryptoGrapher.Decrypt(setting.Key)), CryptoGrapher.Decrypt(setting.Value), setting.Type, true));

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

        #region Implementation of ICollection<Setting>

        public void Add(Setting item)
        {
            WriteSetting(item);
        }

        public void Clear()
        {
        }

        public bool Contains(Setting item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Setting[] array, int arrayIndex)
        {
            var i = 0;
            foreach (var setting in this)
            {
                array[i] = setting;
                i++;
            }
        }

        public bool Remove(Setting item)
        {
            return RemoveSetting(item);
        }

        public int Count
        {
            get { return _source.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}