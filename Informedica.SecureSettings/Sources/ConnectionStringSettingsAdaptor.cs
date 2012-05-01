using System.Configuration;

namespace Informedica.SecureSettings.Sources
{
    public class ConnectionStringSettingsAdaptor : Setting<ConnectionStringSettings>
    {

        public ConnectionStringSettingsAdaptor(ConnectionStringSettings sourceItem) : base(sourceItem)
        {
        }

        #region Overrides of Setting

        public override string Key
        {
            get { return SourceItem.Name; }
            set { SourceItem.Name = value; }
        }

        public override string Value
        {
            get { return SourceItem.ConnectionString; }
            set { SourceItem.ConnectionString = value; }
        }

        #endregion
    }
}