using System;
using StructureMap;

namespace scsm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var result = "";
            try
            {
                result = (new ArgumentProcessor<SecureSettingsManager>(GetSecureSettingsManager())).ProcessArguments(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine(result);
        }

        private static SecureSettingsManager GetSecureSettingsManager()
        {
            try
            {
                ObjectFactory.Initialize(x => { x.UseDefaultStructureMapConfigFile = true; });
                return ObjectFactory.GetInstance<SecureSettingsManager>();

            }
            catch (Exception)
            {
                return new SecureSettingsManager();
            }
        }
    }
}
