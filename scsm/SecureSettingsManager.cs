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

        public string GetSecret()
        {
            return string.Empty;
        }

        public void SetSecret(string secret)
        {
        }
    }
}