using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Informedica.SecureSettings;
using Informedica.SecureSettings.CommandLine;
using Informedica.SecureSettings.Cryptographers;
using Informedica.SecureSettings.Sources;
using Informedica.SecureSettings.Testing;
using StructureMap;

namespace scsm
{
    class Program
    {
        private static SecureSettingSource _source;

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
            if (!args.Any()) return ListOptions();

            var manager = GetSecureSettingsManager();
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

        private static SecureSettingSource GetSecureSettingsManager()
        {
            if (_source == null)
                _source = new SecureSettingSource(GetRegisteredSource(), new SecretKeyManager(),
                                                  CryptographyFactory.GetCryptography());
            return _source;
        }

        private static SettingSource GetRegisteredSource()
        {
            try
            {
                ObjectFactory.Initialize(x => { x.UseDefaultStructureMapConfigFile = true; });
                return ObjectFactory.GetInstance<SettingSource>();

            }
            catch (Exception)
            {
                return MyTestSettingSource.CreateMySettingSource();
            }
        }

        private static string ListOptions()
        {
            var type = GetSecureSettingsManagerType();
            var opts = type.GetMethods().Where(HasAliasAttribute);
            var result = "\n \n Options are: \n" + opts.Aggregate(string.Empty, (current, opt) => current + ("\n  -- " + GetOptionNameFromAttribute(opt)));

            return result;
        }

        private static string GetOptionNameFromAttribute(MethodInfo opt)
        {
            return ((AliasAttribute)opt.GetCustomAttributes(typeof(AliasAttribute), true).First()).Alias;
        }

        private static bool HasAliasAttribute(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(AliasAttribute), true).Any();
        }

        private static string CallMethod(SecureSettingSource manager, MethodInfo method, object[] parameters)
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
            var type = GetSecureSettingsManagerType();
            return type.GetMethods().Single(m => HasAliasAttributeWithValue(m, command));
        }

        private static Type GetSecureSettingsManagerType()
        {
            return typeof(SecureSettingSource);
        }

        private static bool HasAliasAttributeWithValue(MethodInfo method, string command)
        {
            return method.GetCustomAttributes(typeof(AliasAttribute), true).Any(a => ((AliasAttribute)a).Alias == command);
        }
    }
}
