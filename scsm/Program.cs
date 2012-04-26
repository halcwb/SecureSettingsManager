using System;

namespace scsm
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = "";
            try
            {
                result = (new ArgumentProcessor<SecureSettingsManager>()).ProcessArguments(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine(result);
        }
    }
}
