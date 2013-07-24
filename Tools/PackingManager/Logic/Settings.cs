using System;
using System.Configuration;
using System.IO;

namespace TrimFuel.Tools.PackingManager.Logic
{
    class Settings
    {
        public static string ArchivePath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["PrintArchivePath"];

                if (Directory.Exists(path))
                    return path;

                else
                    return null;
            }
        }

        public static string TemplatePath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["TemplatePath"];

                if (Directory.Exists(path))
                    return path;

                else
                    return null;
            }
        }

        public static int Interval
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["PrintInterval"]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static string EndiciaPassPhrase
        {
            get
            {
                return ConfigurationManager.AppSettings["EndiciaPassPhrase"];
            }
        }

        public static string EndiciaRequesterID
        {
            get
            {
                return ConfigurationManager.AppSettings["EndiciaRequesterID"];
            }
        }

        public static string EndiciaAccountID
        {
            get
            {
                return ConfigurationManager.AppSettings["EndiciaAccountID"];
            }
        }

        public static string EndiciaTestMode
        {
            get
            {
                return ConfigurationManager.AppSettings["EndiciaTestMode"];
            }
        }      
    }
}
