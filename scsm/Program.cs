using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Informedica.SecureSettings;

namespace scsm
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = "";
            try
            {
                result = ProcessArguments(args);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            } 
            
            Console.WriteLine(result);
        }

        private static string ProcessArguments(IList<string> args)
        {
            var manager = new SecureSettingsManager();
            var result = "";

            for (var i = 0; i < args.Count(); i++)
            {
                var method = GetMethod(args[i]);
                var numParams = method.GetParameters().Count();
                
// ReSharper disable CoVariantArrayConversion
                result += CallMethod(manager, method, args.Skip(i + 1).Take(numParams).ToArray());
// ReSharper restore CoVariantArrayConversion

                i += numParams;
            }

            return result;
        }

        private static string CallMethod(SecureSettingsManager  manager, MethodInfo method, object[] parameters)
        {
            try
            {
                return string.Format("success: {0}\n", method.Invoke(manager, parameters));
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        private static MethodInfo GetMethod(string command)
        {
            var type = typeof (SecureSettingsManager);
            return type.GetMethods().Single(m => HasAliasAttributeWithValue(m, command));
        }

        private static bool HasAliasAttributeWithValue(MethodInfo method, string command)
        {
            return method.GetCustomAttributes(typeof (AliasAttribute), true).Any(a => ((AliasAttribute)a).Alias == command);
        }
    }
}
