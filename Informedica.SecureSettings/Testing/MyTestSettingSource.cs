using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
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

        private ISetting _appsetting;
        private ISetting _connsetting;

        public MyTestSettingSource() {}

        public MyTestSettingSource(IDictionary<Type, Action<ISetting>> writers, 
                                   IDictionary<Type, Func<ISetting, bool>> removers) : base(writers, removers)
        {
            if (writers == null) RegisterWriters();
            if (removers == null) RegisterRemovers();
        }

        private void WriteAppSetting(ISetting setting)
        {
            _appsetting = setting;
        }

        private void WriteConnSetting(ISetting setting)
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
            if (!Writers.ContainsKey(typeof(NameValueCollection)))
                Writers[typeof(NameValueCollection)] = WriteAppSetting;
            if (!Writers.ContainsKey(typeof(ConnectionStringSettings)))
                Writers[typeof(ConnectionStringSettings)] = WriteConnSetting;
        }

        protected override void RegisterRemovers()
        {
            if (!Removers.ContainsKey(typeof(NameValueCollection)))
                Removers[typeof(NameValueCollection)] = RemoveAppSetting;
            if (!Removers.ContainsKey(typeof(ConnectionStringSettings)))
                Removers[typeof(ConnectionStringSettings)] = RemoveConnSetting;
        }

        private bool RemoveConnSetting(ISetting setting)
        {
            _connsetting = null;
            return true;
        }

        private bool RemoveAppSetting(ISetting setting)
        {
            _appsetting = null;
            return true;
        }

        protected override IEnumerable<ISetting> Settings
        {
            get
            {
                var settings = new List<ISetting>();
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