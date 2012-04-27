using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Informedica.SecureSettings.Sources
{
    public abstract class SettingSource: ICollection<Setting>
    {
        private IDictionary<Enum, Action<Setting>> _writers;
        private IDictionary<Enum, Func<Setting, bool>> _removers;

        protected SettingSource()
        {
            Init();
        }

        protected SettingSource(IDictionary<Enum, Action<Setting>> writers, 
                                IDictionary<Enum, Func<Setting, bool>> removers)
        {
            _writers = writers;
            _removers = removers;
        }

        private void Init()
        {
            RegisterWriters();
            RegisterRemovers();
        }

        protected IDictionary<Enum, Action<Setting>> Writers
        {
            get { return _writers ?? (_writers = new Dictionary<Enum, Action<Setting>>()); }
        }

        protected IDictionary<Enum, Func<Setting, bool>> Removers
        {
            get { return _removers ?? (_removers = new Dictionary<Enum, Func<Setting, bool>>()); }
        }

        private void WriteSetting(Setting setting)
        {
            var method = Writers.ContainsKey(SettingTypeToEnum(setting)) ? Writers[SettingTypeToEnum(setting)]: null;
            if (method == null) throw new MissingMethodException("Method not found to write setting of type: " + setting.Type);
            method.Invoke(setting);
        }

        private bool RemoveSetting(Setting setting)
        {
            var method = Removers.ContainsKey(SettingTypeToEnum(setting)) ? Removers[SettingTypeToEnum(setting)]: null;
            if (method == null) throw new MissingMethodException("Method not found to remove setting of type: " + setting.Type);
            return method.Invoke(setting);
        }

        protected abstract Enum SettingTypeToEnum(Setting setting);

        protected abstract void RegisterWriters();
        protected abstract void RegisterRemovers();
        public abstract void Save();

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

        #region Implementation of ICollection<Setting>

        public void Add(Setting item)
        {
            WriteSetting(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Setting item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Setting[] array, int arrayIndex)
        {
            var i = 0;
            foreach (var setting in this)
            {
                array[i] = setting;
                i++;
            }
        }

        public bool Remove(Setting item)
        {
            return RemoveSetting(item);
        }

        public int Count
        {
            get { return Settings.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}