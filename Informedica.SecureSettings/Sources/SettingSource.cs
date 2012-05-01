using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Informedica.SecureSettings.Sources
{
    public abstract class SettingSource: ICollection<ISetting>
    {
        private IDictionary<Type, Action<ISetting>> _writers;
        private IDictionary<Type, Func<ISetting, bool>> _removers;

        protected SettingSource()
        {
            Init();
        }

        protected SettingSource(IDictionary<Type, Action<ISetting>> writers, 
                                IDictionary<Type, Func<ISetting, bool>> removers)
        {
            _writers = writers;
            _removers = removers;
        }

        private void Init()
        {
            RegisterWriters();
            RegisterRemovers();
        }

        protected IDictionary<Type, Action<ISetting>> Writers
        {
            get { return _writers ?? (_writers = new Dictionary<Type, Action<ISetting>>()); }
        }

        protected IDictionary<Type, Func<ISetting, bool>> Removers
        {
            get { return _removers ?? (_removers = new Dictionary<Type, Func<ISetting, bool>>()); }
        }

        private void WriteSetting(ISetting setting)
        {
            var method = Writers.ContainsKey(setting.Type) ? Writers[setting.Type]: null;
            if (method == null) throw new MissingMethodException("Method not found to write setting of type: " + setting.Type);
            method.Invoke(setting);
        }

        private bool RemoveSetting(ISetting setting)
        {
            var method = Removers.ContainsKey(setting.Type) ? Removers[setting.Type]: null;
            if (method == null) throw new MissingMethodException("Method not found to remove setting of type: " + setting.Type);
            return method.Invoke(setting);
        }

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
        public IEnumerator<ISetting> GetEnumerator()
        {
            return Settings.GetEnumerator();
        }

        protected abstract IEnumerable<ISetting> Settings { get; }

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

        public void Add(ISetting item)
        {
            var setting = Settings.SingleOrDefault(s => s.Key == item.Key);
            if (setting != null) Remove(setting);
            WriteSetting(item);
        }

        public void Clear()
        {
        }

        public bool Contains(ISetting item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ISetting[] array, int arrayIndex)
        {
            var i = 0;
            foreach (var setting in this)
            {
                array[i] = setting;
                i++;
            }
        }

        public bool Remove(ISetting item)
        {
            return Settings.Any(s => s.Key == item.Key) && RemoveSetting(item);
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