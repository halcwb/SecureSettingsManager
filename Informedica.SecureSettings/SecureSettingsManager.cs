using System.Collections.Generic;
using System.Text;

namespace Informedica.SecureSettings
{
    public class SecureSettingsManager
    {
        private string _secret;
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        [Alias("set.secret")]
        public void SetSecret(string secret)
        {
            _secret = secret;
        }

        [Alias("has.secret")]
        public bool HasSecret(string secret)
        {
            return _secret == secret;
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
            var crypt = new SymCryptography {Key = _secret};
            return crypt;
        }
    }
}