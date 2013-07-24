using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;
using log4net;
using System.IO;
using System.Collections;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace AragonLeads
{
    class Program
    {
        private class AragonAuth
        {
            public int CampaignID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private const string ARAGON_HOST = "ios.aragoninc.com";
        private const string PICKUP_FOLDER = "Pickup";
        private const string LEADS_FOLDER = "Dropoff";
        private const string TEMP_FOLDER = "Temp";

        private const string ARCHIVE_FOLDER = "OrdersArchive";

        private static AragonAuth EcigEU = new AragonAuth() { CampaignID = 2561, Username = "triangle", Password = "triangleios" };
        private static AragonAuth EcigUS = new AragonAuth() { CampaignID = 2618, Username = "triangleus", Password = "triangleusios" };
        private static AragonAuth EcigCA = new AragonAuth() { CampaignID = 2619, Username = "triangleca", Password = "trianglecaios" };

        private const string LEAD_FILE_NAME = "leads_{0}.csv";

        static ILog logger = null;
        static IDao dao = null;
        static void Main(string[] args)
        {
            logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                if (!Directory.Exists(ARCHIVE_FOLDER))
                {
                    Directory.CreateDirectory(ARCHIVE_FOLDER);
                }
                if (!Directory.Exists(TEMP_FOLDER))
                {
                    Directory.CreateDirectory(TEMP_FOLDER);
                }

                ProcessOrders(EcigEU);
                ProcessOrders(EcigUS);
                SendLeads(EcigEU);
                SendLeads(EcigUS);
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Program), ex); ;
            }
        }

        private static void ProcessOrders(AragonAuth auth)
        {
            if (!Directory.Exists(ARCHIVE_FOLDER + @"\" + auth.CampaignID.ToString()))
            {
                Directory.CreateDirectory(ARCHIVE_FOLDER + @"\" + auth.CampaignID.ToString());
            }

            Sftp ftp = new Sftp(ARAGON_HOST, auth.Username, auth.Password);
            try
            {
                ftp.Connect();
                ArrayList fileNames = ftp.GetFileList(PICKUP_FOLDER);

                //foreach (string fileName in fileNames)
                //{
                //    logger.Error(fileName);
                //}

                foreach (string fileName in fileNames)
                {
                    if (fileName != "." && fileName != "..")
                    {
                        string localFilePath = string.Format(ARCHIVE_FOLDER + @"\" + auth.CampaignID.ToString() + @"\" + fileName);
                        if (!File.Exists(localFilePath))
                        {
                            ftp.Get(PICKUP_FOLDER + "/" + fileName, localFilePath);
                            ProcessOrdersResult(File.ReadAllLines(localFilePath));
                        }
                    }
                }
            }
            finally
            {
                ftp.Close();
            }

            ////Test
            //string[] fileNames = Directory.GetFiles(ARCHIVE_FOLDER + @"\" + auth.CampaignID.ToString()).Select(fn => Path.GetFileName(fn)).ToArray();
            //foreach (string fileName in fileNames)
            //{
            //    string localFilePath = string.Format(ARCHIVE_FOLDER + @"\" + auth.CampaignID.ToString() + @"\" + fileName);
            //    if (File.Exists(localFilePath))
            //    {
            //        ProcessOrdersResult(File.ReadAllLines(localFilePath));
            //    }
            //}
        }

        private static void ProcessOrdersResult(string[] ordersResult)
        {
            if (ordersResult != null && ordersResult.Length > 1)
            {
                for (int i = 1; i < ordersResult.Length; i++)
                {
                    try
                    {
                        string[] fields = ordersResult[i].Split(new string[] { "\",\"" }, StringSplitOptions.None);
                        if (fields.Length < 4)
                        {
                            throw new Exception(string.Format("Invalid fields count: {0}", ordersResult[i]));
                        }
                        long? billingID = GetLong(fields[0]);
                        int? campaignID = GetInt(fields[1]);
                        string statusText = GetString(fields[2]);
                        int? upsellStatus = GetInt(fields[3]);
                        DateTime? verificationDate = GetDateTime(fields[4]);
                        SaveOrderResult(billingID, campaignID, verificationDate, upsellStatus, statusText, ordersResult[i]);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(typeof(Program), ex); ;
                    }                    
                }
            }
        }

        private static void SaveOrderResult(long? billingID, int? camapignID, DateTime? verificationDate, int? upsellStatus, string statusText, string response)
        {
            try
            {
                dao.BeginTransaction();

                bool skip = false;
                if (billingID != null)
                {
                    MySqlCommand q = new MySqlCommand("select * from AragonLead " +
                        "where BillingID = @billingID and VerificationDT = @verificationDT");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                    q.Parameters.Add("@verificationDT", MySqlDbType.Timestamp).Value = verificationDate;
                    
                    if (dao.Load<AragonLead>(q).Count > 0)
                    {
                        skip = true;
                    }
                }

                if (!skip)
                {
                    AragonLead lead = new AragonLead();
                    lead.BillingID = billingID;
                    lead.AragonCampaignID = camapignID;
                    lead.VerificationDT = verificationDate;
                    lead.UpsellStatus = upsellStatus;
                    lead.StatusText = statusText;
                    lead.Response = response;
                    lead.LeadSent = false;
                    lead.CreateDT = DateTime.Now;
                    dao.Save<AragonLead>(lead);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(typeof(Program), ex); ;
            }
        }

        private static void SendLeads(AragonAuth auth)
        {
            string fileName = string.Format(LEAD_FILE_NAME, DateTime.Now.ToString("yyyyMMddHHmm"));

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select al.* from AragonLead al " +
                    "inner join Billing b on b.BillingID = al.BillingID " +
                    "where al.LeadSent = 0 and al.AragonCampaignID = @campaignID and (al.UpsellStatus > 0)");
                q.Parameters.Add("campaignID", MySqlDbType.Int32).Value = auth.CampaignID;

                IList<AragonLead> leads = dao.Load<AragonLead>(q);
                if (leads.Count > 0)
                {
                    using (StreamWriter wr = File.CreateText(TEMP_FOLDER + @"\" + fileName))
                    {
                        foreach (AragonLead l in leads)
                        {
                            Billing b = dao.Load<Billing>(l.BillingID);
                            int expMonth = b.ExpMonth ?? 0;
                            int expYear = b.ExpYear ?? 0;
                            if (expYear >= 2000) expYear = expYear - 2000;
                            wr.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"", auth.CampaignID, b.BillingID, b.CreditCardCnt.DecryptedCreditCard, expMonth.ToString("00") + expYear.ToString("00")));
                        }
                    }

                    Sftp ftp = new Sftp(ARAGON_HOST, auth.Username, auth.Password);
                    try
                    {
                        ftp.Connect();
                        ftp.Put(TEMP_FOLDER + @"\" + fileName, LEADS_FOLDER + "/" + fileName);
                    }
                    finally
                    {
                        ftp.Close();
                    }

                    foreach (AragonLead l in leads)
                    {
                        l.LeadSent = true;
                        dao.Save<AragonLead>(l);
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(typeof(Program), ex); ;
            }

            if (File.Exists(TEMP_FOLDER + @"\" + fileName))
            {
                File.Delete(TEMP_FOLDER + @"\" + fileName);
            }
        }

        private static string GetString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Trim('"');
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

        private static decimal? GetDecimal(string value)
        {
            string decimalValue = GetString(value);
            decimal temp = 0M;
            if (!string.IsNullOrEmpty(decimalValue) && decimal.TryParse(decimalValue, out temp))
            {
                return temp;
            }
            return null;
        }

        private static long? GetLong(string value)
        {
            string decimalValue = GetString(value);
            long temp = 0;
            if (!string.IsNullOrEmpty(decimalValue) && long.TryParse(decimalValue, out temp))
            {
                return temp;
            }
            return null;
        }

        private static int? GetInt(string value)
        {
            string decimalValue = GetString(value);
            int temp = 0;
            if (!string.IsNullOrEmpty(decimalValue) && int.TryParse(decimalValue, out temp))
            {
                return temp;
            }
            return null;
        }
    }
}
