using System.Configuration;

namespace Informedica.SecureSettings.Sources
{
    public class ConnectionStringSettingsAdaptor : Setting<ConnectionStringSettings>
    {

        public ConnectionStringSettingsAdaptor(ConnectionStringSettings source) : base(source)
        {
        }

        #region Overrides of Setting

        public override string Key
        {
            get { return Source.Name; }
            set { Source.Name = value; }
        }

        public override string Value
        {
            get { return Source.ConnectionString; }
            set { Source.ConnectionString = value; }
        }

        #endregion
    }
}