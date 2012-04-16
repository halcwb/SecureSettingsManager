using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Informedica.SecureSettings.CommandLine;
using Informedica.SecureSettings.Cryptographers;
using Microsoft.Win32;

namespace Informedica.SecureSettings.Sources
{
    public class SecureSettingSource: ISettingSource
    {
        private readonly ISettingSource _settings;
        private RegistryKey _key;
        private const string KeyName = "SecureSettingsManager";
        private const string ValueName = "Secret";
        public static readonly string SecureMarker = "[Secure]";
        private readonly SettingSource _source;
        private SecretKeyManager _secretKeyManager;
        private CryptoGraphy _cryptographer;

        [Obsolete]
        public SecureSettingSource(ISettingSource source)
        {
            _settings = source;
            Init();
        }

        public SecureSettingSource(SettingSource source, SecretKeyManager secretKeyManager, CryptoGraphy crypt)
        {
            _source = source;
            _secretKeyManager = secretKeyManager;
            _cryptographer = crypt;
        }

        // ToDo: Solve security issues
        private void Init()
        {
            var s = new RegistrySecurity();
            var r = new RegistryAccessRule("Users", RegistryRights.FullControl, AccessControlType.Allow);
            s.AddAccessRule(r);
            Registry.LocalMachine.SetAccessControl(s);
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node", true);
            // ReSharper disable PossibleNullReferenceException
            key.SetAccessControl(s);
            key = key.CreateSubKey("Informedica");
            _key = key.CreateSubKey(KeyName);
            // ReSharper restore PossibleNullReferenceException
        }

        [Obsolete]
        [Alias("set.secret")]
        public void SetSecret(string secret)
        {
            _key.SetValue(ValueName, secret, RegistryValueKind.String);
        }

        [Obsolete]
        [Alias("has.secret")]
        public bool HasSecret(string secret)
        {
            return GetSecret() == secret;
        }

        [Obsolete]
        [Alias("get.secret")]
        public string GetSecret()
        {
            return (string) _key.GetValue(ValueName);
        }

        [Obsolete]
        [Alias("set.connstr")]
        public void SetConnectionString(string name, string value)
        {
            name = Encrypt(name);
            name = AddSecureMarker(name);
            value = Encrypt(value);
            _settings.WriteConnectionString(name, value);
        }

        private static string AddSecureMarker(string name)
        {
            return SecureMarker + name;
        }

        private static string RemoveSecureMarker(string name)
        {
            return name.Replace(SecureMarker, string.Empty);
        }

        [Obsolete]
        [Alias("get.connstr")]
        public string GetConnectionString(string name)
        {
            name = Encrypt(name);
            name = AddSecureMarker(name);
            return Decrypt(_settings.ReadConnectionString(name));
        }

        [Obsolete]
        [Alias("set.setting")]
        public void SetSetting(string name, string value)
        {
            _settings.WriteAppSetting(name, value);
        }

        [Obsolete]
        [Alias("get.setting")]
        public string GetSetting(string name)
        {
            return _settings.ReadAppSetting(name);
        }

        [Obsolete]
        [Alias("set.secure")]
        public void WriteSecureSetting(string name, string value)
        {
            name = Encrypt(name);
            value = Encrypt(value);

            SetSetting(name, value);
        }

        [Obsolete]
        [Alias("get.secure")]
        public string ReadSecureSetting(string name)
        {
            name = Encrypt(name);
            var value = _settings.ReadAppSetting(name);
            value = Decrypt(value);

            return value;
        }

        [Obsolete]
        [Alias("get.settings")]
        public string GetSettings()
        {
            var settings = new StringBuilder();
            foreach (var setting in _settings)
            {
                settings.AppendLine(setting.Name + ": " + setting.Value);
            }
            return settings.ToString();
        }

        public IEnumerable<Setting> Settings
        {
            get
            {
                return (from setting in _settings 
                        let name = setting.IsEncrypted ? Decrypt(RemoveSecureMarker(setting.Name)) : setting.Name 
                        let value = setting.IsEncrypted ? Decrypt(setting.Value) : setting.Value 
                        select new Setting(name, value, setting.Type, setting.IsEncrypted)).ToList();
            }
        }

        private string Decrypt(string value)
        {
            var crypt = GetCryptography();
            return crypt.Decrypt(value);
        }

        private string Encrypt(string value)
        {
            var crypt = GetCryptography();
            return crypt.Encrypt(value);
        }

        private SymCryptography GetCryptography()
        {
            var crypt = new SymCryptography {Key = GetSecret()};
            return crypt;
        }

        public void RemoveSetting(Setting setting)
        {
            _source.RemoveSetting(setting);
        }

        [Obsolete]
        [Alias("rm.setting")]
        public void RemoveSetting(string name)
        {
            RemoveSetting(_settings.Single(s => s.Name == name));
        }

        [Obsolete]
        [Alias("rm.secure")]
        public void RemoveSecureSetting(string name)
        {
            name = Encrypt(name);
            RemoveSetting(name);
        }

        public void WriteConnectionString(string name, string connectionString)
        {
            throw new NotImplementedException();
        }

        public string ReadConnectionString(string name)
        {
            throw new NotImplementedException();
        }

        public void WriteAppSetting(string name, string setting)
        {
            throw new NotImplementedException();
        }

        public string ReadAppSetting(string name)
        {
            throw new NotImplementedException();
        }

        public void Remove(string setting)
        {
            throw new NotImplementedException();
        }

        public void Remove(Setting setting)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public void RemoveConnectionString(string name)
        {
            name = Encrypt(name);
            _settings.RemoveConnectionString(name);
        }

        public void WriteSetting(Setting setting)
        {
            _source.WriteSetting(setting);
        }

        public string GetSecretKey()
        {
            return  _secretKeyManager.GetKey();
        }

        public void SetSecretKey(string secret)
        {
            _secretKeyManager.SetKey(secret);
        }

        public Setting ReadSetting(Enum settingType, string settingName)
        {
            return _source.ReadSetting(settingType, settingName);
        }

        public void WriteSecure(Setting setting)
        {
            var encrypted = new Setting(CryptoGrapher.Encrypt(setting.Name),
                                        CryptoGrapher.Encrypt(setting.Value), 
                                        setting.Type, 
                                        true);
            WriteSetting(encrypted);
        }

        public Setting ReadSecure(Enum settingType, string settingName)
        {
            var setting = ReadSetting(settingType, settingName);
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
            return _settings.GetEnumerator();
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