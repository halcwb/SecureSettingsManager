namespace Informedica.SecureSettings
{
    public class Setting
    {
        public Setting(string name, string value, string group, bool encrypted)
        {
            Name = name;
            Value = value;
            Group = group;
            IsEncrypted = encrypted;
        }

        public readonly string Name;
        public readonly string Value;
        public readonly string Group;
        public readonly bool IsEncrypted;
    }
}