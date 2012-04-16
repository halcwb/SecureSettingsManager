namespace Informedica.SecureSettings.Cryptographers
{
    public abstract class CryptoGraphy
    {
        protected string Key;
        public abstract string Encrypt(string value);
        public abstract string Decrypt(string value);
        public void SetKey(string key)
        {
            Key = key;
        }
    }
}