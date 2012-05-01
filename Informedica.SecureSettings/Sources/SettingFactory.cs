using System;
using System.Collections.Generic;
using System.Configuration;
using Informedica.SecureSettings.Cryptographers;

namespace Informedica.SecureSettings.Sources
{
    public class SettingFactory
    {
        private static Dictionary<Type, Func<object, string, string, ISetting>> _factoryMethods = new Dictionary<Type, Func<object, string, string, ISetting>>();
        private static SecretKeyManager _secretKeyManager;
        private static CryptoGraphy _cryptoGraphy;

        static SettingFactory()
        {
            _factoryMethods.Add(typeof(ConnectionStringSettings), CreateConnectionStringSettingsAdaptor);
            _factoryMethods.Add(typeof(KeyValueConfigurationElement), CreateKeyValueConfigurationElementSettingAdaptor);

            _secretKeyManager = new SecretKeyManager();
            _cryptoGraphy = CryptographyFactory.GetCryptography();
        }

        public static ISetting CreateSecureSetting<T>(T sourceItem)
        {
            return _factoryMethods[typeof(T)].Invoke(sourceItem, string.Empty, string.Empty);
        }

        public static ISetting CreateSecureSetting<T>(string key, string value)
        {
            return _factoryMethods[typeof (T)].Invoke(null, key, value);
        }

        private static ISetting CreateConnectionStringSettingsAdaptor(object sourceItem, string name, string value)
        {
            if (sourceItem == null) sourceItem = new ConnectionStringSettings(name, value);

            var setting = new ConnectionStringSettingsAdaptor((ConnectionStringSettings)sourceItem);
            return new SecureSetting(setting, _secretKeyManager, _cryptoGraphy);
        }

        private static ISetting CreateKeyValueConfigurationElementSettingAdaptor(object sourceItem, string key, string value)
        {
            //ToDo this will throw an error because key will return empty string??
            if (sourceItem == null) sourceItem = new KeyValueConfigurationElement(key, value);

            return new KeyValueConfigurationElementSettingAdaptor((KeyValueConfigurationElement) sourceItem);
        }

    }
}