namespace Informedica.SecureSettings.Cryptographers
{
    public static class CryptographyFactory
    {
        public static CryptoGraphy GetCryptographt()
        {
            return new CryptographyAdapter(new SymCryptography());
        }
    }
}
