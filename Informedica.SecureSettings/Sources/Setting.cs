using System;
using Informedica.SecureSettings.Exceptions;

namespace Informedica.SecureSettings.Sources
{

    /// <summary>
    /// The setting class represents a setting that can be stored to a SettingSource
    /// A setting has a name to identify the setting, a value, a type to store the 
    /// type of the setting and whether the setting is encrypted or not.
    /// </summary>
    public abstract class Setting<T> : ISetting
    {
        private volatile string _secureMarker = "[Secure]";
        protected T SourceItem;

        protected Setting(T sourceItem)
        {
            SourceItem = sourceItem;
            CheckIfSourceIsValid();
        }

        private void CheckIfSourceIsValid()
        { if (string.IsNullOrWhiteSpace(Key)) throw new StringCannotBeNullOrWhiteSpaceException("Key of Setting should be supplied");}

        public abstract string Key { get; set; }
        public abstract string Value { get; set; }

        public Type Type { get { return typeof(T); } }

        public bool IsSecure { get { return Key.StartsWith(SecureMarker); } }

        public string SecureMarker
        {
            get { return _secureMarker; }
        }
    }
}