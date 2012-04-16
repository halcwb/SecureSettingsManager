using System.Collections.Generic;

namespace Informedica.SecureSettings.Sources
{
    /// <summary>
    /// The SettingSource is used to write and read a setting. 
    /// Depending on the Setting.Type, the SettingSource can 
    /// determine how to write the setting.
    /// The setting source extends IEnumerable so it can be 
    /// used as an enumeration of Setting.
    /// </summary>
    public interface ISettingSource : IEnumerable<Setting>
    {
        void WriteConnectionString(string name, string connectionString);
        string ReadConnectionString(string name);
        void WriteAppSetting(string name, string setting);
        string ReadAppSetting(string name);
        void Remove(string setting);
        void Remove(Setting setting);
        void RemoveConnectionString(string name);
        string ReadSecureSetting(string name);
        void WriteSecureSetting(string key, string value);
        string GetConnectionString(string name);
        void SetConnectionString(string name, string connectionString);
        void RemoveSecureSetting(string appSettingName);
    }
}