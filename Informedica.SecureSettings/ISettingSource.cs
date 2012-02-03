using System.Collections.Generic;

namespace Informedica.SecureSettings
{
    public interface ISettingSource : IEnumerable<Setting>
    {
        void WriteConnectionString(string name, string connectionString);
        string ReadConnectionString(string name);
        void WriteAppSetting(string name, string setting);
        string ReadAppSetting(string name);
    }
}