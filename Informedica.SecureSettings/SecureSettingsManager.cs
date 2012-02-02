using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Informedica.SecureSettings
{
    public class SecureSettingsManager
    {
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();
        private RegistryKey _key;
        private const string KeyName = "SecureSettingsManager";
        private const string ValueName = "Secret";

        public SecureSettingsManager()
        {
            Init();
        }

        private void Init()
        {
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            // ReSharper disable PossibleNullReferenceException
            key = key.CreateSubKey("Informedica");
            _key = key.CreateSubKey(KeyName);
            // ReSharper restore PossibleNullReferenceException
        }

        [Alias("set.source")]
        public void SetSource(string source)
        {
            
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

        private string GetSecret()
        {
            return (string) _key.GetValue(ValueName);
        }

        [Alias("set.setting")]
        public void SetSetting(string setting, string value)
        {
            _settings.Add(setting, value);
        }

        [Alias("get.setting")]
        public string GetSetting(string setting)
        {
            return _settings[setting];
        }

        [Alias("set.secure")]
        public void WriteSecureSetting(string setting, string value)
        {
            setting = Encrypt(setting);
            value = Encrypt(value);

            SetSetting(setting, value);
        }

        [Alias("get.secure")]
        public string GetSecureSetting(string setting)
        {
            setting = Encrypt(setting);
            var value = _settings[setting];
            value = Decrypt(value);

            return value;
        }

        [Alias("get.settings")]
        public string GetSettings()
        {
            var settings = new StringBuilder();
            foreach (var setting in _settings)
            {
                settings.AppendLine(setting.Key + ": " + setting.Value);
            }
            return settings.ToString();
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
    }
}