using System;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace Informedica.SecureSettings
{
    public class SecureSettingsManager
    {
        private ISettingSource _settings;
        private RegistryKey _key;
        private const string KeyName = "SecureSettingsManager";
        private const string ValueName = "Secret";

        public SecureSettingsManager(ISettingSource source)
        {
            _settings = source;
            Init();
        }

        // ToDo: Solve security issues
        private void Init()
        {
            //var s = new RegistrySecurity();
            //var r = new RegistryAccessRule(Environment.UserDomainName, RegistryRights.FullControl, AccessControlType.Allow);
            //s.AddAccessRule(r);
            //Registry.LocalMachine.SetAccessControl(s);
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            //key.SetAccessControl();
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

        [Alias("set.connstr")]
        public void SetConnectionString(string name, string value)
        {
            value = Encrypt(value);
            _settings.WriteConnectionString(name, value);
        }

        [Alias("get.connstr")]
        public string GetConnectionString(string name)
        {
            return Decrypt(_settings.ReadConnectionString(name));
        }

        [Alias("set.setting")]
        public void SetSetting(string setting, string value)
        {
            _settings.WriteAppSetting(setting, value);
        }

        [Alias("get.setting")]
        public string GetSetting(string setting)
        {
            return _settings.ReadAppSetting(setting);
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
            var value = _settings.ReadAppSetting(setting);
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