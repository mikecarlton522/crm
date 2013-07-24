using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;

namespace TrimFuel.Business.Dao
{
    public class MySqlDaoFactory
    {
        //TODO (From configuration)
        private const string TRIM_FUEL_CONNECTION_STRING_ID = "TrimFuel";
        //private const string ACAI_CRM_CONNECTION_STRING_ID = "AcaiCRM";

        //private const string WEB_CONTEXT_DAO_KEY_TEMPLATE = "dao_{0}";

        //private static string GetConnectionString(DB db)
        //{
        //    string res = null;

        //    switch (db)
        //    {
        //        case DB.TrimFuel:
        //            if (!Config.Current.CONNECTION_STRINGS.ContainsKey(TRIM_FUEL_CONNECTION_STRING_ID))
        //            {
        //                throw new Exception("Can't find COnnectionString(" + TRIM_FUEL_CONNECTION_STRING_ID + ")");
        //            }
        //            res = Config.Current.CONNECTION_STRINGS[TRIM_FUEL_CONNECTION_STRING_ID];
        //            break;
        //        case DB.AcaiCRM:
        //            if (!Config.Current.CONNECTION_STRINGS.ContainsKey(ACAI_CRM_CONNECTION_STRING_ID))
        //            {
        //                throw new Exception("Can't find COnnectionString(" + ACAI_CRM_CONNECTION_STRING_ID + ")");
        //            }
        //            res = Config.Current.CONNECTION_STRINGS[ACAI_CRM_CONNECTION_STRING_ID];
        //            break;
        //        default:
        //            break;
        //    }
        //    return res;
        //}

        private static string GetCurrentConnectionString()
        {
            string res = null;

            if (string.IsNullOrEmpty(Config.Current.CURRENT_CONNECTION_STRING))
            {
                if (!Config.Current.CONNECTION_STRINGS.ContainsKey(TRIM_FUEL_CONNECTION_STRING_ID))
                {
                    throw new Exception("Can't find COnnectionString(" + TRIM_FUEL_CONNECTION_STRING_ID + ")");
                }
                Config.Current.CURRENT_CONNECTION_STRING = Config.Current.CONNECTION_STRINGS[TRIM_FUEL_CONNECTION_STRING_ID];
            }
            res = Config.Current.CURRENT_CONNECTION_STRING;

            return res;
        }

        public static IDao CreateDao(DB db)
        {
            IDao dao = LoadDaoFromWebContext();

            if (dao == null)
            {
                dao = CreateNewDao(GetCurrentConnectionString());
                SaveDaoToWebContext(dao, GetCurrentConnectionString());
            }

            return dao;
        }

        private static IDao CreateNewDao(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                return new MySqlDao(connectionString);
            }
            return null;
        }

        private static IDao LoadDaoFromWebContext()
        {
            if (HttpContext.Current != null)
            {
                return (IDao)HttpContext.Current.Items[GetCurrentConnectionString()];
            }
            return LoadDaoFromStaticStorage();
        }

        private static void SaveDaoToWebContext(IDao dao, string connectionString)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[connectionString] = dao;
            }
            else
            {
                SaveDaoToStaticStorage(dao, connectionString);
            }
        }

        private static IDictionary<string, IDao> staticContextStorage = null;

        private static IDao LoadDaoFromStaticStorage()
        {
            if (staticContextStorage != null && staticContextStorage.ContainsKey(GetCurrentConnectionString()))
            {
                return staticContextStorage[GetCurrentConnectionString()];
            }
            return null;
        }

        private static void SaveDaoToStaticStorage(IDao dao, string connectionString)
        {
            if (staticContextStorage == null)
            {
                staticContextStorage = new Dictionary<string, IDao>();
            }
            staticContextStorage[connectionString] = dao;
        }
    }
}
