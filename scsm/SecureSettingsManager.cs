namespace scsm
{
    public class SecureSettingsManager
    {
        [Alias("set.setting")]
        public void SetSetting(string name, string type, string value)
        {
            
        }

        [Alias("get.setting")]
        public string GetSetting(string name, string type)
        {
            return string.Empty;
        }

        [Alias("set.conn")]
        public void SetConnectionString(string name, string value)
        {
            SetSetting(name, "Conn", value);
        }

        [Alias("get.conn")]
        public string GeConnectionString(string name)
        {
            return GetSetting(name, "Conn");
        }

        [Alias("get.secret")]
        public string GetSecret()
        {
            return string.Empty;
        }

        [Alias("set.secret")]
        public void SetSecret(string secret)
        {
        }
    }
}