namespace Informedica.SecureSettings
{
    public class Setting
    {
        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public readonly string Name;
        public readonly string Value;
    }
}