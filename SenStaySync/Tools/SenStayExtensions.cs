namespace SenStaySync
{
    using System;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    public static class SenStayExtensions
    {
        /// <summary>
        /// Exstracts SenStay ID (example: LA013)
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string ExtractSenStayID(this string Value)
        {
            try
            {
                return Regex.Match(Value, @"[a-zA-Z]+\d+").Value;
                //return Regex.Match(Value, @"[a-zA-Z]+0\d+").Value;
                //return Regex.Match(Value, @"[a-zA-Z]+[01]\d+").Value;
            }
            catch
            {
                return "";
            }
        }

        public static T ConvertTo<T>(this string value)
        {
            var foo = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                return (T)(foo.ConvertFromInvariantString(value));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static object ConvertTo(this string value, Type type)
        {
            var foo = TypeDescriptor.GetConverter(type);
            try
            {
                return foo.ConvertFromInvariantString(value);
            }
            catch (Exception)
            {
                return Activator.CreateInstance(type);
            }
        }

        public static int ExtractPortNumber(this string address)
        {
            try
            {
                var pos = address.IndexOf(':');
                if (pos > 0)
                {
                    var portFull = address.Substring(pos);
                    var result = Regex.Match(portFull, @"[0-9]+").Value;
                    ;
                    return Convert.ToInt32(result);
                }
                return 80;
            }
            catch
            {
                return 80;
            }
        }

        public static string ExtractPortNumberStr(this string address)
        {
            try
            {
                var pos = address.IndexOf(':');
                if (pos > 0)
                {
                    var portFull = address.Substring(pos);
                    var result = Regex.Match(portFull, @"[0-9]+").Value;
                    ;
                    return result;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static string ExtractIPAddress(this string address)
        {
            try
            {
                return Regex.Match(address, @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+").Value;
            }
            catch
            {
                return "";
            }
        }

        public static string ToDescription<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            return source.ToString();
        }
    }
}