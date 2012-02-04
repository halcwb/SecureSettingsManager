using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Informedica.SecureSettings.Testing
{
    public class TestSettingSource : ISettingSource
    {
        private readonly IList<Setting> _settings = new List<Setting>();

        #region Implementation of ISettingSource

        public void WriteConnectionString(string name, string connectionString)
        {
            _settings.Add(new Setting(name, connectionString));
        }

        public string ReadConnectionString(string name)
        {
            return _settings.Single(s => s.Name == name).Value;
        }

        public void WriteAppSetting(string name, string setting)
        {
            _settings.Add(new Setting(name, setting));
        }

        public string ReadAppSetting(string name)
        {
            return _settings.Single(s => s.Name == name).Value;
        }

        public void Remove(string setting)
        {
            Remove(_settings.Single(s =>s.Name == setting));
        }

        public void Remove(Setting setting)
        {
            _settings.Remove(setting);
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Setting> GetEnumerator()
        {
            return _settings.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}