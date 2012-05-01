using System.Configuration;

namespace Informedica.SecureSettings.Sources
{
    public class SettingFactory
    {
        public static Setting<ConnectionStringSettings> CreateSetting(string key, string value, bool isSecure)
        {
            return new ConnectionStringSettingsAdaptor(new ConnectionStringSettings(key, value));
        }
    }
}