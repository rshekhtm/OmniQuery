using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace OmniQuery.CodeAnalytics
{
    internal static class Utility
    {
        /// <summary>
        /// Retrieve an embedded resource from calling assembly by name
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>Resource contents</returns>
        public static string GetEmbeddedResource(string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            foreach (string name in assembly.GetManifestResourceNames())
            {
                if (name.EndsWith(resourceName))
                {
                    using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(name)))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Convert value from one enum to another
        /// </summary>
        /// <typeparam name="T">Return enum type</typeparam>
        /// <param name="value">Original enum value</param>
        /// <returns>Converted value</returns>
        public static T ConvertEnum<T>(Enum value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }
}
