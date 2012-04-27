using Informedica.SecureSettings.Exceptions;

namespace Informedica.SecureSettings.Sources
{

    /// <summary>
    /// The setting class represents a setting that can be stored to a SettingSource
    /// A setting has a name to identify the setting, a value, a type to store the 
    /// type of the setting and whether the setting is encrypted or not.
    /// </summary>
    public class Setting
    {
        public Setting(string name, string value, string type, bool encrypted)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new StringCannotBeNullOrWhiteSpaceException("Name of setting");
            if (string.IsNullOrWhiteSpace(type)) 
                throw new StringCannotBeNullOrWhiteSpaceException("Type of setting");

            Key = name;
            Value = value;
            Type = type;
            IsEncrypted = encrypted;
        }

        public readonly string Key;
        public readonly string Value;
        public readonly string Type;
        public readonly bool IsEncrypted;

        public string Machine
        {
            get { return TryGetArrayAtIndex(0); }
        }

        public string Environment
        {
            get { return TryGetArrayAtIndex(1); }
        }

        public string Name
        {
            get { return TryGetArrayAtIndex(2); }
        }

        private string TryGetArrayAtIndex(int index)
        {
            try
            {
                return Key.Split('.')[index];

            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }
    }
}