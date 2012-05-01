using System;
using System.Configuration;

namespace Informedica.SecureSettings.Sources
{

    public class KeyValueConfigurationElementSettingAdaptor : Setting<KeyValueConfigurationElement>
    {
        public KeyValueConfigurationElementSettingAdaptor(KeyValueConfigurationElement sourceItem): base(sourceItem)
        {
        }

        #region Overrides of Setting<KeyValueConfigurationElement>

        public override string Key
        {
            get { return SourceItem.Key; }
            set { throw new NotImplementedException();}
        }

        public override string Value
        {
            get { return  SourceItem.Value; }
            set { SourceItem.Value = value; }
        }

        #endregion
    }
}