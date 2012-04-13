using System;

namespace Informedica.SecureSettings
{
    public class StringCannotBeNullOrWhiteSpaceException: Exception
    {
        public StringCannotBeNullOrWhiteSpaceException(string message): base(message) {}
    }
}