using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace scsm
{
    public class ArgumentProcessor<T> where T : class
    {
        private T _targetObject;

        public ArgumentProcessor(T target)
        {
            _targetObject = target;
        }

        public string ProcessArguments(IList<string> args)
        {
            if (!args.Any()) return ListOptions();

            var result = "";

            for (var i = 0; i < args.Count(); i++)
            {
                var method = GetMethod(args[i]);
                var numParams = Enumerable.Count(method.GetParameters());

                // ReSharper disable CoVariantArrayConversion
                result += CallMethod(_targetObject, method, args.Skip(i + 1).Take(numParams).ToArray());
                // ReSharper restore CoVariantArrayConversion

                i += numParams;
            }

            return result;
        }


        public string ListOptions()
        {
            var type = GetTargetObjectType();
            var opts = type.GetMethods().Where(HasAliasAttribute);
            var result = "\n \n Options are: \n" + Enumerable.Aggregate(opts, String.Empty, (current, opt) => current + ("\n  -- " + GetOptionNameFromAttribute(opt)));

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

        private static string CallMethod(T target, MethodInfo method, object[] parameters)
        {
            try
            {
                return String.Format("success: {0}\n", method.Invoke(target, parameters));
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        private static MethodInfo GetMethod(string command)
        {
            var type = GetTargetObjectType();
            return Enumerable.Single(type.GetMethods(), m => HasAliasAttributeWithValue(m, command));
        }

        private static Type GetTargetObjectType()
        {
            return typeof(T);
        }

        private static bool HasAliasAttributeWithValue(MethodInfo method, string command)
        {
            return method.GetCustomAttributes(typeof(AliasAttribute), true).Any(a => ((AliasAttribute)a).Alias == command);
        }

    }

}