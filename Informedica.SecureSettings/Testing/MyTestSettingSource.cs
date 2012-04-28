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
                                   IDictionary<Enum, Func<Setting, bool>> removers) : base(writers, removers)
        {
            if (writers == null) RegisterWriters();
            if (removers == null) RegisterRemovers();
        }

        public MyTestSettingSource()
        {
        }

        private void WriteAppSetting(Setting setting)
        {
            _appsetting = setting;
        }

        private void WriteConnSetting(Setting setting)
        {
            _connsetting = setting;
        }

        public static MyTestSettingSource CreateMySettingSource()
        {

            return new MyTestSettingSource();
        }

        #region Overrides of SettingSource


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

        private bool RemoveConnSetting(Setting setting)
        {
            _connsetting = null;
            return true;
        }

        private bool RemoveAppSetting(Setting setting)
        {
            _appsetting = null;
            return true;
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