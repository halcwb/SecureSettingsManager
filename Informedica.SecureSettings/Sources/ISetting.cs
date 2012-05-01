using System;

namespace Informedica.SecureSettings.Sources
{
    public interface ISetting
    {
        string Key { get; set; }
        string Value { get; set; }
        Type Type { get; }
        bool IsSecure { get; }
        string SecureMarker { get; }
    }
}