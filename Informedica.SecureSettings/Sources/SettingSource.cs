using System;
using System.Collections;
using System.Collections.Generic;

namespace Informedica.SecureSettings.Sources
{
    public abstract class SettingSource: IEnumerable<Setting>
    {
        protected IDictionary<Enum, Action<Setting>> Writers;
        protected IDictionary<Enum, Func<string, Setting>> Readers;
        protected IDictionary<Enum, Action<Setting>> Removers;

        protected SettingSource(IDictionary<Enum, Action<Setting>> writers, 
                                IDictionary<Enum, Func<string, Setting>> readers, 
                                IDictionary<Enum, Action<Setting>> removers)
        {
            Writers = writers;
            Readers = readers;
            Removers = removers;

            Init();
        }

        private void Init()
        {
            RegisterReaders();
            RegisterWriters();
            RegisterRemovers();
        }

        public void WriteSetting(Setting setting)
        {
            Writers[SettingTypeToString(setting)].Invoke(setting);
        }

        public Setting ReadSetting(Enum type, string name)
        {
            return Readers[type].Invoke(name);
        }

        public void RemoveSetting(Setting setting)
        {
            Removers[SettingTypeToString(setting)].Invoke(setting);
        }

        protected abstract Enum SettingTypeToString(Setting setting);

        protected abstract void RegisterReaders();
        protected abstract void RegisterWriters();
        protected abstract void RegisterRemovers();

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
            return Settings.GetEnumerator();
        }

        protected abstract IEnumerable<Setting> Settings { get; }

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