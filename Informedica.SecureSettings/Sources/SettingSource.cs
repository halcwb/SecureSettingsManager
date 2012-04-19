using System;
using System.Collections;
using System.Collections.Generic;

namespace Informedica.SecureSettings.Sources
{
    public abstract class SettingSource: IEnumerable<Setting>
    {
        private IDictionary<Enum, Action<Setting>> _writers;
        private IDictionary<Enum, Func<string, Setting>> _readers;
        private IDictionary<Enum, Action<Setting>> _removers;

        protected SettingSource()
        {
            Init();
        }

        protected SettingSource(IDictionary<Enum, Action<Setting>> writers, 
                                IDictionary<Enum, Func<string, Setting>> readers, 
                                IDictionary<Enum, Action<Setting>> removers)
        {
            _writers = writers;
            _readers = readers;
            _removers = removers;
        }

        private void Init()
        {
            RegisterReaders();
            RegisterWriters();
            RegisterRemovers();
        }

        protected IDictionary<Enum, Action<Setting>> Writers
        {
            get { return _writers ?? (_writers = new Dictionary<Enum, Action<Setting>>()); }
        }

        protected IDictionary<Enum, Func<string, Setting>> Readers
        {
            get { return _readers ?? (_readers = new Dictionary<Enum, Func<string, Setting>>()); }
        }

        protected IDictionary<Enum, Action<Setting>> Removers
        {
            get { return _removers ?? (_removers = new Dictionary<Enum, Action<Setting>>()); }
        }

        public void WriteSetting(Setting setting)
        {
            var method = Writers.ContainsKey(SettingTypeToEnum(setting)) ? Writers[SettingTypeToEnum(setting)]: null;
            if (method == null) throw new MissingMethodException("Method not found to write setting of type: " + setting.Type);
            method.Invoke(setting);
        }

        public Setting ReadSetting(Enum type, string name)
        {
            var method = Readers.ContainsKey(type) ? Readers[type] : null;
            if (method == null) throw new MissingMethodException("Method not found to read setting of type: " + type);
            return  method.Invoke(name);
        }

        public void RemoveSetting(Setting setting)
        {
            var method = Removers.ContainsKey(SettingTypeToEnum(setting)) ? Removers[SettingTypeToEnum(setting)]: null;
            if (method == null) throw new MissingMethodException("Method not found to remove setting of type: " + setting.Type);
            method.Invoke(setting);
        }

        protected abstract Enum SettingTypeToEnum(Setting setting);

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

        public abstract void Save();
    }
}