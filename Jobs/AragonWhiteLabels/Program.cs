using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using log4net;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using TrimFuel.Business.Utils;

namespace AragonWhiteLabels
{
    class Program
    {
       
        private const string URL = "https://crm.aragoninc.com/clubjava/crmapi.nsf/gateway";
        private const string REQUEST = "q_system_key=##KEY##&q_action=LEAD_ADD&q_lead_callcenter=##CALLCENTER##&q_lead_first_name=##B_F_NAME##&q_lead_middle_name=##B_M_NAME##&q_lead_last_name=##B_L_NAME##&q_lead_ship_address1=##B_ADDRESS##&q_lead_ship_address2=##B_ADDRESS2##&q_lead_ship_address3=##B_ADDRESS3##&q_lead_ship_city=##B_CITY##&q_lead_ship_state=##B_STATE##&q_lead_ship_zip=##B_ZIP##&q_lead_ship_country=##B_COUNTRY##&q_lead_email=##B_EMAIL##&q_lead_phone=##B_PHONE##&q_lead_order=##B_DATE##&q_lead_sold=##PRODUCT##&q_lead_record=##REC##&q_lead_extid=##B_ID##";
        private const string KEY = "8PQMT4";
        private const int PRODUCT_ID = 20;
        private static IDictionary<string, int> CALL_CENTERS = new Dictionary<string, int>()
        {
            {"EliteSavings", 250},
            {"SuperShopper", 275}
        };

        static ILog logger = null;
        static IDao dao = null;
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 ||
                !(
                  args[0].ToLower() == "-all_ecigstv_last_7_days_supershopper" ||
                  args[0].ToLower() == "-csv" && args.Length > 1
                ))
            {
                Console.WriteLine("Invalid arguments:");
                Console.WriteLine("-all_ecigstv_last_7_days_supershopper - to send all Ecigs TV orders for last 7 days to SuperShoper");
                Console.WriteLine("-csv [FileName] - to send orders from CSV");
                return;
            }

            logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                if (args[0].ToLower() == "-all_ecigstv_last_7_days_supershopper")
                {
                    SendAllEcigsTVLast7DaysSuperShopper();
                }
                else if (args[0].ToLower() == "-csv")
                {
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine("Invalid arguments: File " + args[1] + " was not found.");
                        return;
                    }
                    IList<KeyValuePair<long, int>> registrationIDs = new List<KeyValuePair<long, int>>();
                    using (StreamReader sr = new StreamReader(args[1]))
                    {
                        using (CsvReader csv = new CsvReader(sr, true))
                        {
                            while (csv.ReadNextRecord())
                            {
                                long? regID = Utility.TryGetLong(csv[0]);
                                string callCenterName = csv[1];
                                int? callCenterID = (CALL_CENTERS.ContainsKey(callCenterName) ? new int?(CALL_CENTERS[callCenterName]) : null);

                                if (regID == null)
                                {
                                    Console.WriteLine("Invalid RegistrationID at Line " + csv.CurrentRecordIndex.ToString() + " Column 1");
                                }
                                else if (callCenterID == null)
                                {
                                    Console.WriteLine("Invalid Call Center (" + callCenterName + ") at Line " + csv.CurrentRecordIndex.ToString() + " Column 2");

                                }
                                else
                                {
                                    registrationIDs.Add(new KeyValuePair<long, int>(regID.Value, callCenterID.Value));
                                }
                            }
                        }
                    }
                    SendFromCSV(registrationIDs);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void SendAllEcigsTVLast7DaysSuperShopper()
        {
            try
            {

                MySqlCommand q = new MySqlCommand("select distinct b.* from Billing b " +
                    "join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "left join AragonWhiteLabelRequest ar on ar.BillingID = b.BillingID " +
                    "where s.ProductID = @ProductID and b.CreateDT >= @StartDate and coalesce(ar.Completed,0) = 0 " +
                    "order by b.BillingID asc");
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = DateTime.Now.AddDays(-7);
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = PRODUCT_ID;

                IList<Billing> billingList = dao.Load<Billing>(q);

                int callCenterID = 275;

                Console.WriteLine("BEGIN PROCESSING");
                foreach (Billing billing in billingList)
                {
                    Console.WriteLine(string.Format("Billing: {0}", billing.BillingID.ToString()));

                    string request = PrepareRequest(billing, callCenterID);

                    Console.WriteLine(string.Format(" Request: {0}", request));

                    string response = SendRequest(request);

                    Console.WriteLine(string.Format(" Response: {0}", response));

                    bool completed = ProcessResponse(response);

                    SaveAragonWhiteLabel(billing.BillingID, callCenterID, completed, request, response);
                }
                Console.WriteLine("END PROCESSING");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendFromCSV(IList<KeyValuePair<long, int>> registrationIDsWithCallCenterIDs)
        {
            try
            {
                Console.WriteLine("BEGIN PROCESSING");
                foreach (KeyValuePair<long, int> registrationIDWithCallCenterID in registrationIDsWithCallCenterIDs)
                {
                    MySqlCommand q = new MySqlCommand("select distinct b.* from Billing b " +
                        "left join AragonWhiteLabelRequest ar on ar.BillingID = b.BillingID and ar.ProductID = @ProductID and ar.CallCenterID = @CallCenterID " +
                        "where coalesce(ar.Completed,0) = 0 " +
                        "and b.RegistrationID = @RegistrationID " +
                        "order by b.BillingID asc");
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = PRODUCT_ID;
                    q.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = registrationIDWithCallCenterID.Key;
                    q.Parameters.Add("@CallCenterID", MySqlDbType.Int32).Value = registrationIDWithCallCenterID.Value;

                    IList<Billing> billingList = dao.Load<Billing>(q);
                    if (billingList.Count == 0)
                    {
                        Console.WriteLine(string.Format("RegistrationID: {0} was not found or already sent.", registrationIDWithCallCenterID.Key));
                    }
                    else
                    {
                        Billing billing = billingList.First();

                        Console.WriteLine(string.Format("Billing: {0}", billing.BillingID.ToString()));

                        string request = PrepareRequest(billing, registrationIDWithCallCenterID.Value);

                        Console.WriteLine(string.Format(" Request: {0}", request));

                        string response = SendRequest(request);

                        Console.WriteLine(string.Format(" Response: {0}", response));

                        bool completed = ProcessResponse(response);

                        SaveAragonWhiteLabel(billing.BillingID, registrationIDWithCallCenterID.Value, completed, request, response);
                    }
                }
                Console.WriteLine("END PROCESSING");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                logger.Error(ex);
            }
        }


        private static string PrepareRequest(Billing billing, int callCenterID)
        {
            string request = REQUEST;

            request = request.Replace("##KEY##", KEY);
            request = request.Replace("##CALLCENTER##", callCenterID.ToString());

            request = request.Replace("##B_F_NAME##", billing.FirstName);
            request = request.Replace("##B_M_NAME##", "");
            request = request.Replace("##B_L_NAME##", billing.LastName);
            request = request.Replace("##B_ADDRESS##", billing.Address1);
            request = request.Replace("##B_ADDRESS2##", billing.Address2);
            request = request.Replace("##B_ADDRESS3##", "");
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##B_COUNTRY##", FixCountry(billing.Country));
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##B_PHONE##", billing.Phone);
            request = request.Replace("##B_DATE##", billing.CreateDT.Value.ToString("yyyyMMdd"));

            request = request.Replace("##PRODUCT##", PRODUCT_ID.ToString());
            request = request.Replace("##REC##", "");

            request = request.Replace("##B_ID##", billing.BillingID.ToString());

            return request;
        }

        private static string SendRequest(string request)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                return wc.UploadString(URL, "POST", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                return null;
            }
        }

        private static bool ProcessResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            if (response.Contains("code=1"))
                return true; // Success
            else
                return false; // Error
        }


        public static void SaveAragonWhiteLabel(long? billingID, int callCenterID, bool Completed, string request, string response)
        {
            //MySqlCommand q = new MySqlCommand("select count(*) from AragonWhiteLabelRequest where BillingID = @BillingID and ProductID = @ProductID and CallCenterID = @CallCenterID");
            //q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
            //q.Parameters.Add("@CallCenterID", MySqlDbType.Int32).Value = callCenterID;
            //q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = PRODUCT_ID;
            //int? count = dao.ExecuteScalar<int>(q);

            //if (count == 0)
            //{
            MySqlCommand q = new MySqlCommand("insert into AragonWhiteLabelRequest (BillingID, ProductID, CallCenterID, Completed, Request, Response, CreateDT) values (@BillingID, @ProductID, @CallCenterID, @Completed, @Request, @Response, @CreateDT)");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = PRODUCT_ID;
                q.Parameters.Add("@CallCenterID", MySqlDbType.Int32).Value = callCenterID;
                q.Parameters.Add("@Completed", MySqlDbType.Bit).Value = Completed;
                q.Parameters.Add("@Request", MySqlDbType.VarChar).Value = request;
                q.Parameters.Add("@Response", MySqlDbType.VarChar).Value = response;
                q.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = DateTime.Now;
                dao.ExecuteNonQuery(q);
            //}
            //else
            //{
            //    q = new MySqlCommand("update AragonWhiteLabelRequest set Completed = @Completed where BillingID = @BillingID and ProductID = @ProductID and CallCenterID = @CallCenterID");
            //    q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
            //    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = PRODUCT_ID;
            //    q.Parameters.Add("@CallCenterID", MySqlDbType.Int32).Value = callCenterID;
            //    q.Parameters.Add("@Completed", MySqlDbType.Bit).Value = Completed;
            //    dao.ExecuteNonQuery(q);
            //}
        }

        private static string FixCountry(string country)
        {
            if (country == null)
            {
                return null;
            }
            if (country.ToLower() == "uk" ||
                country.ToLower() == "united kingdom")
            {
                return "GB";
            }
            return country;
        }
    }
}
