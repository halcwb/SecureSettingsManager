namespace Informedica.SecureSettings.Cryptographers
{
    public class SecretKeyManager
    {
        //ToDo Set string to secret key, have to implement registry version
        private string _key = "This is a secret";

        public string GetKey()
        {
            return _key;
        }

        public void SetKey(string secret)
        {
            _key = secret;
        }
    }
}