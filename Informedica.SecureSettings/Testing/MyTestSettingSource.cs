using System;
using System.Collections.Generic;
using Informedica.SecureSettings.Sources;

namespace Informedica.SecureSettings.Testing
{
    public sealed class MyTestSettingSource: SettingSource
    {
        public enum SettingTypes
        {
            App,
            Conn
        }

        private Setting _appsetting;
        private Setting _connsetting;

        public MyTestSettingSource(IDictionary<Enum, Action<Setting>> writers, 
                                   IDictionary<Enum, Func<string, Setting>> readers, 
                                   IDictionary<Enum, Action<Setting>> removers) : base(writers, readers, removers)
        {}

        private MyTestSettingSource()
        {
        }

        private void WriteAppSetting(Setting setting)
        {
            _appsetting = setting;
        }

        private Setting ReadAppSetting(string name)
        {
            return _appsetting;
        }

        private void WriteConnSetting(Setting setting)
        {
            _connsetting = setting;
        }

        private Setting ReadConnSetting(string name)
        {
            return _connsetting;
        }

        public static MyTestSettingSource CreateMySettingSource()
        {
            var writers = new Dictionary<Enum, Action<Setting>>();
            var readers = new Dictionary<Enum, Func<string, Setting>>();
            var removers = new Dictionary<Enum, Action<Setting>>();

            return new MyTestSettingSource();
        }

        #region Overrides of SettingSource

        protected override void RegisterReaders()
        {
            if (!Readers.ContainsKey(SettingTypes.App))
                Readers[SettingTypes.App] = ReadAppSetting;
            if (!Readers.ContainsKey(SettingTypes.Conn))
                Readers[SettingTypes.Conn] = ReadConnSetting;
        }

        protected override void RegisterWriters()
        {
            if (!Writers.ContainsKey(SettingTypes.App))
                Writers[SettingTypes.App] = WriteAppSetting;
            if (!Writers.ContainsKey(SettingTypes.Conn))
                Writers[SettingTypes.Conn] = WriteConnSetting;
        }

        protected override void RegisterRemovers()
        {
            if (!Removers.ContainsKey(SettingTypes.App))
                Removers[SettingTypes.App] = RemoveAppSetting;
            if (!Removers.ContainsKey(SettingTypes.Conn))
                Removers[SettingTypes.Conn] = RemoveConnSetting;
        }

        private void RemoveConnSetting(Setting setting)
        {
            _connsetting = null;
        }

        private void RemoveAppSetting(Setting setting)
        {
            _appsetting = null;
        }

        protected override Enum SettingTypeToEnum(Setting setting)
        {
            SettingTypes value;
            Enum.TryParse(setting.Type, out value);
            return value;
        }

        protected override IEnumerable<Setting> Settings
        {
            get
            {
                var settings = new List<Setting>();
                if (_appsetting != null) settings.Add(_appsetting);
                if (_connsetting != null) settings.Add(_connsetting);

                return settings;
            }
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}