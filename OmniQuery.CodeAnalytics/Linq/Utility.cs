using System;
using System.Collections.Generic;
using System.Text;

namespace OmniQuery.CodeAnalytics.Linq
{
    internal static class Utility
    {
        public static string ToString(object value)
        {
            if (value == null)
            {
                return null;
            }

            string strValue = value as string;
            if (strValue != null)
            {
                return string.IsNullOrEmpty(strValue.Trim()) ? null : strValue;
            }

            byte[] byteValue = value as byte[];
            if (byteValue != null)
            {
                if (byteValue.Length == 0)
                {
                    return null;
                }
                else
                {
                    foreach (byte val in byteValue)
                    {
                        strValue += val.ToString("x2");
                    }
                    return strValue;
                }
            }

            return value.ToString();
        }

        public static T ConvertEnum<T>(Enum value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }
}
