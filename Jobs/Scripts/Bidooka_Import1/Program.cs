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
using LumenWorks.Framework.IO.Csv;
using TrimFuel.Model.Enums;
using System.Globalization;

namespace Bidooka_Import1
{
    class Program
    {
        private const string FILE_NAME = @"User Data.csv";
        private const int CAMPAIGN_ID = 10000;
        private const int PRODUCT_ID = 1;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                using (CsvReader csv = new CsvReader(new StreamReader(FILE_NAME), true))
                {
                    while (csv.ReadNextRecord())
                    {
                        if (limit <= 0)
                        {
                            break;
                        }
                        ProcessLine(dao, logger, csv);
                        limit--;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void ProcessLine(IDao dao, ILog logger, CsvReader fields)
        {
            try
            {
                dao.BeginTransaction();

                string linenumber = fields[0];
                logger.Info(linenumber);

                SaleService ss = new SaleService();

                DateTime? createDT = GetDateTime(fields[6]);
                if (createDT != null && createDT.Value.Hour == 0 && createDT.Value.Minute == 0 && createDT.Value.Second == 0)
                {
                    createDT = new DateTime(createDT.Value.Year, createDT.Value.Month, createDT.Value.Day, 12, 0, 0);
                }

                Billing b = new Billing();
                b.CampaignID = CAMPAIGN_ID;
                b.FirstName = GetString(fields[2]);
                b.LastName = GetString(fields[3]);
                b.Email = GetString(fields[4]);
                b.CreditCard = "";

                b.CreateDT = createDT;
                b.IP = GetString(fields[5]);

                Registration r = new Registration();
                r.CampaignID = CAMPAIGN_ID;
                r.FirstName = GetString(fields[2]);
                r.LastName = GetString(fields[3]);
                r.Email = GetString(fields[4]);

                r.CreateDT = createDT;
                r.IP = GetString(fields[5]);

                dao.Save<Registration>(r);
                b.RegistrationID = r.RegistrationID;
                dao.Save<Billing>(b);

                BillingSubscription bs = ss.CreateUpsellFakeBillingSubscriptionByProduct(b, PRODUCT_ID);
                bs.CreateDT = createDT;
                bs.LastBillDate = createDT;
                if (GetString(fields[8]) == "Disabled")
                {
                    bs.StatusTID = BillingSubscriptionStatusEnum.Inactive;
                }
                dao.Save<BillingSubscription>(bs);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
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
                DateTime.TryParse(dateTime, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out res);
            }
            return res;
        }
    }
}
