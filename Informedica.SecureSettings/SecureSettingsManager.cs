using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32;

namespace Informedica.SecureSettings
{
    public class SecureSettingsManager
    {
        private readonly ISettingSource _settings;
        private RegistryKey _key;
        private const string KeyName = "SecureSettingsManager";
        private const string ValueName = "Secret";
        public static readonly string SecureMarker = "[Secure]";

        public SecureSettingsManager(ISettingSource source)
        {
            _settings = source;
            Init();
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

        [Alias("set.secret")]
        public void SetSecret(string secret)
        {
            _key.SetValue(ValueName, secret, RegistryValueKind.String);
        }

        [Alias("has.secret")]
        public bool HasSecret(string secret)
        {
            return GetSecret() == secret;
        }

        [Alias("get.secret")]
        public string GetSecret()
        {
            return (string) _key.GetValue(ValueName);
        }

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

        [Alias("get.connstr")]
        public string GetConnectionString(string name)
        {
            name = Encrypt(name);
            name = AddSecureMarker(name);
            return Decrypt(_settings.ReadConnectionString(name));
        }

        [Alias("set.setting")]
        public void SetSetting(string name, string value)
        {
            _settings.WriteAppSetting(name, value);
        }

        [Alias("get.setting")]
        public string GetSetting(string name)
        {
            return _settings.ReadAppSetting(name);
        }

        [Alias("set.secure")]
        public void WriteSecureSetting(string name, string value)
        {
            name = Encrypt(name);
            value = Encrypt(value);

            SetSetting(name, value);
        }

        [Alias("get.secure")]
        public string ReadSecureSetting(string name)
        {
            name = Encrypt(name);
            var value = _settings.ReadAppSetting(name);
            value = Decrypt(value);

            return value;
        }

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
            _settings.Remove(setting);
        }

        [Alias("rm.setting")]
        public void RemoveSetting(string name)
        {
            RemoveSetting(_settings.Single(s => s.Name == name));
        }

        [Alias("rm.secure")]
        public void RemoveSecureSetting(string name)
        {
            name = Encrypt(name);
            RemoveSetting(name);
        }

        public void RemoveConnectionString(string name)
        {
            name = Encrypt(name);
            _settings.RemoveConnectionString(name);
        }
    }
}