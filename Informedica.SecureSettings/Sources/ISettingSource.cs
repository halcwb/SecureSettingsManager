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
        void Remove(Setting setting);
    }
}