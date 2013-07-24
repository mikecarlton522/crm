using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using System.IO;
using TrimFuel.Model;
using TrimFuel.Business;
using System.Configuration;

namespace JMB_Import2
{
    class Program
    {
        private const string FILE_NAME = @"bb_0320-0620.csv";
        private const int CAMPAIGN_ID = 10001;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                string[] lines = File.ReadAllLines(FILE_NAME);
                foreach (string line in lines)
                {
                    if (limit <= 0)
                    {
                        break;
                    }
                    ProcessLine(dao, logger, line);
                    limit--;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static string[] DELIMITERS = { "####" };
        private static void ProcessLine(IDao dao, ILog logger, string line)
        {
            string internalID = null;
            string createDT = null;
            try
            {
                dao.BeginTransaction();

                line = line.Replace(",\\N", ",\"\"");
                //remove last quote
                if (line[line.Length - 1] == '\"')
                {
                    line = line.Substring(0, line.Length - 1);
                }
                //normalize string
                line = line.Replace("\",\"", "####");
                line = line.Replace(",\"", "####");

                string[] fields = line.Split(DELIMITERS, StringSplitOptions.None);
                if (fields.Length != 16)
                {
                    throw new Exception(string.Format("Can't process row({0}). Insufficient fields number({1})", line, fields.Length));
                }

                internalID = GetString(fields[0]);
                createDT = fields[5];

                BillingExternalInfo bInfo = new BillingExternalInfo();
                bInfo.InternalID = GetString(fields[0]);

                BillingService ss = new BillingService();
                if (!string.IsNullOrEmpty(bInfo.InternalID) && ss.GetLastBillingByInternalID(bInfo.InternalID) != null)
                {
                    throw new Exception(string.Format("InternalID({0}) already exists", bInfo.InternalID));
                }

                Billing b = new Billing();
                b.CampaignID = CAMPAIGN_ID;
                b.FirstName = GetString(fields[3]);
                b.LastName = GetString(fields[4]);
                b.Address1 = GetString(fields[9]);
                b.Address2 = GetString(fields[10]);
                b.City = GetString(fields[11]);
                b.State = GetString(fields[12]);
                b.Zip = GetString(fields[13]);
                b.Country = "US";
                b.Phone = GetString(fields[7]);
                b.Email = GetString(fields[8]);

                b.CreateDT = GetDateTime(fields[5]);
                b.IP = GetString(fields[6]);
                b.Affiliate = GetString(fields[14]);
                b.SubAffiliate = GetString(fields[15]);

                Registration r = new Registration();
                r.CampaignID = CAMPAIGN_ID;
                r.FirstName = GetString(fields[3]);
                r.LastName = GetString(fields[4]);
                r.Address1 = GetString(fields[9]);
                r.Address2 = GetString(fields[10]);
                r.City = GetString(fields[11]);
                r.State = GetString(fields[12]);
                r.Zip = GetString(fields[13]);
                r.Phone = GetString(fields[7]);
                r.Email = GetString(fields[8]);

                r.CreateDT = GetDateTime(fields[5]);
                r.IP = GetString(fields[6]);
                r.Affiliate = GetString(fields[14]);
                r.SubAffiliate = GetString(fields[15]);

                dao.Save<Registration>(r);
                b.RegistrationID = r.RegistrationID;
                dao.Save<Billing>(b);
                bInfo.BillingID = b.BillingID;
                dao.Save<BillingExternalInfo>(bInfo);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
                if (!string.IsNullOrEmpty(internalID))
                {
                    logger.Error("InternalID(" + internalID + ") CreateDT(" + createDT + ")");
                }
                logger.Error(ex);
            }
        }

        private static string GetString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Trim('"').Replace(@"\N", "");
        }

        private static DateTime GetDateTime(string value)
        {
            string dateTime = GetString(value);
            DateTime res = DateTime.Today;
            if (!string.IsNullOrEmpty(dateTime))
            {
                DateTime.TryParse(dateTime, out res);
            }
            return res;
        }
    }
}
