using System;
using System.Security.Cryptography;
using System.Text;

namespace Informedica.SecureSettings
{
    /// <summary>CHashProtector is a password protection one way encryption algorithm</summary>
    /// <programmer>Chidi C. Ezeukwu</programmer>
    /// <written>May 16, 2004</written>
    /// <Updated>Aug 07, 2004</Updated>
		
    public class Hash
    {
        #region Private member variables...

        private string _salt;
        private readonly HashAlgorithm _cryptoService;

        #endregion

        #region Public interfaces...

        public enum ServiceProviderEnum
        {
            // Supported algorithms
// ReSharper disable InconsistentNaming
            SHA1, 
            SHA256, 
            SHA384, 
            SHA512, 
            MD5
// ReSharper restore InconsistentNaming
        }

        public Hash()
        {
            // Default Hash algorithm
            _cryptoService = new SHA1Managed();
        }

        public Hash(ServiceProviderEnum serviceProvider)
        {	
            // Select hash algorithm
            switch(serviceProvider)
            {
                case ServiceProviderEnum.MD5:
                    _cryptoService = new MD5CryptoServiceProvider();
                    break;
                case ServiceProviderEnum.SHA1:
                    _cryptoService = new SHA1Managed();
                    break;
                case ServiceProviderEnum.SHA256:
                    _cryptoService = new SHA256Managed();
                    break;
                case ServiceProviderEnum.SHA384:
                    _cryptoService = new SHA384Managed();
                    break;
                case ServiceProviderEnum.SHA512:
                    _cryptoService = new SHA512Managed();
                    break;
            }
        }

        public Hash(string serviceProviderName)
        {
            // Set Hash algorithm
            _cryptoService = (HashAlgorithm)CryptoConfig.CreateFromName(serviceProviderName.ToUpper());
        }

        public virtual string Encrypt(string plainText)
        {
            byte[] cryptoByte =  _cryptoService.ComputeHash(Encoding.ASCII.GetBytes(plainText + _salt));
			
            // Convert into base 64 to enable result to be used in Xml
            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.Length);
        }

        public string Salt
        {
            // Salt value
            get
            {
                return _salt;
            }
            set
            {
                _salt = value;
            }
        }
        #endregion
    }
}