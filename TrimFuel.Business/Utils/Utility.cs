using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace TrimFuel.Business.Utils
{
    public static class Utility
    {
        private const string RANDOM_STRING_CHARACTER_SET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        public static string RandomString(Random r, int len)
        {
            StringBuilder sb = new StringBuilder();

            while ((len--) > 0)
                sb.Append(RANDOM_STRING_CHARACTER_SET[(int)(r.NextDouble() * RANDOM_STRING_CHARACTER_SET.Length)]);

            return sb.ToString();
        }

        private const string PASSWORD_CHARACTER_SET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        public static string Password(Random r, int len)
        {
            StringBuilder sb = new StringBuilder();

            while ((len--) > 0)
                sb.Append(PASSWORD_CHARACTER_SET[(int)(r.NextDouble() * PASSWORD_CHARACTER_SET.Length)]);

            return sb.ToString();
        }

        public static decimal Add(decimal? d1, decimal? d2)
        {
            return ((d1 != null) ? d1.Value : 0M) + ((d2 != null) ? d2.Value : 0M);
        }

        public static double Add(double? d1, double? d2)
        {
            return ((d1 != null) ? d1.Value : 0.0) + ((d2 != null) ? d2.Value : 0.0);
        }

        //Restriction "T : class" means use nullables instead of scalar structures
        public static T GetRandom<T>(Random r, IEnumerable<KeyValuePair<int, T>> distrubution) where T : class
        {
            if (distrubution != null && distrubution.Count() > 0 && distrubution.Where(item => item.Key != 0).Count() > 0)
            {
                double sum = (double)distrubution.Sum(item => item.Key);
                double accum = 0.0;
                double variate = r.NextDouble();

                foreach (KeyValuePair<int, T> item in distrubution)
                {
                    accum += ((double)item.Key) / sum;
                    if (variate < accum)
                    {
                        return item.Value;
                    }
                }
            }

            return null;
        }

        public static T GetRandom<T>(Random r, IEnumerable<KeyValuePair<long, T>> distrubution) where T : class
        {
            if (distrubution != null && distrubution.Count() > 0 && distrubution.Where(item => item.Key != 0).Count() > 0)
            {
                double sum = (double)distrubution.Sum(item => item.Key);
                double accum = 0.0;
                double variate = r.NextDouble();

                foreach (KeyValuePair<long, T> item in distrubution)
                {
                    accum += ((double)item.Key) / sum;
                    if (variate < accum)
                    {
                        return item.Value;
                    }
                }
            }

            return null;
        }

        public static T GetRandom<T>(Random r, IEnumerable<KeyValuePair<decimal, T>> distrubution) where T : class
        {
            if (distrubution != null && distrubution.Count() > 0 && distrubution.Where(item => item.Key != 0).Count() > 0)
            {
                double sum = (double)distrubution.Sum(item => item.Key);
                double accum = 0.0;
                double variate = r.NextDouble();

                foreach (KeyValuePair<decimal, T> item in distrubution)
                {
                    accum += ((double)item.Key) / sum;
                    if (variate < accum)
                    {
                        return item.Value;
                    }
                }
            }

            return null;
        }

        public static T GetRandom<T>(Random r, IEnumerable<T> distribution, Func<T, int> powerSelector) where T : class
        {
            if (distribution != null && distribution.Count() > 0)
            {
                return GetRandom<T>(r, distribution.Select(i => new KeyValuePair<int, T>(powerSelector(i), i)));
            }

            return null;
        }

        public static T GetRandom<T>(Random r, IEnumerable<T> distribution, Func<T, decimal> powerSelector) where T : class
        {
            if (distribution != null && distribution.Count() > 0)
            {
                return GetRandom<T>(r, distribution.Select(i => new KeyValuePair<decimal, T>(powerSelector(i), i)));
            }

            return null;
        }

        public static string LoadFromEmbeddedResource(Type type, string resourceName)
        {
            string res = null;
            using (Stream s = type.Assembly.GetManifestResourceStream(type, resourceName))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    res = sr.ReadToEnd();
                }
            }
            return res;
        }

        public static string FormatPrice(decimal? price)
        {
            if (price == null)
            {
                return string.Empty;
            }
            return price.Value.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));
        }

        public static string FormatCurrency(decimal? amount, string currencySymbol)
        {
            if (amount == null)
            {
                amount = 0M;
            }
            string res = amount.Value.ToString("c", CultureInfo.GetCultureInfo("en-US"));
            if (!string.IsNullOrEmpty(currencySymbol))
            {
                res = res.Replace("$", currencySymbol);
            }
            return res;
        }

        public static byte? TryGetByte(string intStr)
        {
            byte temp = 0;
            if (!string.IsNullOrEmpty(intStr) && byte.TryParse(intStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static short? TryGetShort(string intStr)
        {
            short temp = 0;
            if (!string.IsNullOrEmpty(intStr) && short.TryParse(intStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static int? TryGetInt(string intStr)
        {
            int temp = 0;
            if (!string.IsNullOrEmpty(intStr) && int.TryParse(intStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static long? TryGetLong(string intStr)
        {
            long temp = 0;
            if (!string.IsNullOrEmpty(intStr) && long.TryParse(intStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static decimal? TryGetDecimal(string intStr)
        {
            decimal temp = 0;
            if (!string.IsNullOrEmpty(intStr) && decimal.TryParse(intStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static string TryGetStr(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }
            return null;
        }

        public static DateTime? TryGetDate(string dateStr)
        {
            DateTime temp = default(DateTime);
            if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static double? TryGetDouble(string intStr)
        {
            double temp = 0;
            if (!string.IsNullOrEmpty(intStr) && double.TryParse(intStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static bool? TryGetBool(string boolStr)
        {
            bool temp = default(bool);
            if (!string.IsNullOrEmpty(boolStr) && bool.TryParse(boolStr, out temp))
            {
                return temp;
            }
            return null;
        }

        public static string ComputeMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string XmlSerializeUTF8(object serializableObject)
        {

            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (System.IO.MemoryStream aMemStr = new System.IO.MemoryStream())
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(aMemStr, new UTF8Encoding()))
                {
                    serializer.Serialize(writer, serializableObject);
                    string strXml = (new UTF8Encoding()).GetString(aMemStr.ToArray());
                    return strXml;
                }
            }
        }

        public static Stream OpenStringAsStreamUTF8(string s)
        {
            return new MemoryStream((new UTF8Encoding()).GetBytes(s));
        }

        private const long BUFFER_SIZE = 4096;
        public static void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }

        public static string Stream2String(Stream s, Encoding enc)
        {
            if (s == null)
            {
                return null;
            }
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }
            byte[] content = new byte[s.Length];
            s.Read(content, 0, content.Length);
            return enc.GetString(content);
        }
    }
}
