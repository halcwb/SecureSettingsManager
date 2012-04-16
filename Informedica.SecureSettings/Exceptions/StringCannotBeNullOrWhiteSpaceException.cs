using System;

namespace Informedica.SecureSettings.Exceptions
{
    public class StringCannotBeNullOrWhiteSpaceException: Exception
    {
        public StringCannotBeNullOrWhiteSpaceException(string message): base(message) {}
    }
}