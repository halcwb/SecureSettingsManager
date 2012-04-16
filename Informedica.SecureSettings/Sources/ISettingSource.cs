using System;
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
        [Obsolete]
        void WriteConnectionString(string name, string connectionString);
        [Obsolete]
        string ReadConnectionString(string name);
        [Obsolete]
        void WriteAppSetting(string name, string setting);
        [Obsolete]
        string ReadAppSetting(string name);
        [Obsolete]
        void Remove(string setting);
        void Remove(Setting setting);
        [Obsolete]
        void RemoveConnectionString(string name);
        [Obsolete]
        string ReadSecureSetting(string name);
        [Obsolete]
        void WriteSecureSetting(string key, string value);
        [Obsolete]
        string GetConnectionString(string name);
        [Obsolete]
        void SetConnectionString(string name, string connectionString);
        [Obsolete]
        void RemoveSecureSetting(string appSettingName);
    }
}