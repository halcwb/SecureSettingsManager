namespace Informedica.SecureSettings.Cryptographers
{
    public static class CryptographyFactory
    {
        public static CryptoGraphy GetCryptography()
        {
            return new CryptographyAdapter(new SymCryptography());
        }
    }
}
