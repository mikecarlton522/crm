using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices.ShipperAPI
{
    /// <summary>
    /// Summary description for shipper_ws
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/", Name = "shipper_ws")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class shipper_ws : System.Web.Services.WebService
    {
        [WebMethod]
        public string UpdateReturn(int saleID, string reason, string note)
        {
            if (!SaleIsReturned(saleID))
            {
                int billingID = GetBillingID(saleID);

                if (billingID > 0)
                {
                    if (!string.IsNullOrEmpty(note))
                        reason = string.Format("{0}. {1}", reason, note);

                    ReturnSale(saleID, billingID, reason);

                    return "1";
                }
                else
                    return "-1";
            }
            else
                return "0";
        }
        
        [WebMethod]
        public string UpdateTracker(int saleID, int trackingNumber)
        {
            MySqlConnection connection;
            MySqlCommand command;

            try
            {
                connection = new MySqlConnection();
                command = new MySqlCommand();

                connection.ConnectionString = TrimFuel.Business.Config.Current.CONNECTION_STRINGS["TrimFuel"];
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;

                string query = "update Sale set TrackingNumber = @trackingNumber where SaleID = @saleID limit 1";                    

                command.CommandText = query;
                command.Parameters.AddWithValue("@saleId", saleID);
                command.Parameters.AddWithValue("@trackingNumber", trackingNumber);

                command.ExecuteNonQuery();

                command.Dispose();
                connection.Close();
            }
            catch
            {
                return "-1";
            }

            return "1";
        }        

        private int GetBillingID(int saleID)
        {
            MySqlConnection connection;
            MySqlCommand command;

            int billingID;

            try
            {
                connection = new MySqlConnection();
                command = new MySqlCommand();

                connection.ConnectionString = TrimFuel.Business.Config.Current.CONNECTION_STRINGS["TrimFuel"];
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;

                string query = @"
                    select bs.BillingID from  BillingSale bsale
                    inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
                    where bsale.SaleID = @saleID
                    union select u.BillingID from UpsellSale us
                    inner join Upsell u on u.UpsellID = us.UpsellID
                    where us.SaleID = @saleID
                    union select e.BillingID from ExtraTrialShipSale e
                    where e.SaleID = @saleID";

                command.CommandText = query;
                command.Parameters.AddWithValue("@saleId", saleID);

                billingID = (int)command.ExecuteScalar();

                command.Dispose();
                connection.Close();
            }
            catch
            {
                billingID = -1;
            }

            return billingID;
        }

        private bool SaleIsReturned(int saleID)
        {
            MySqlConnection connection;
            MySqlCommand command;

            bool isReturned = false;

            try
            {
                connection = new MySqlConnection();
                command = new MySqlCommand();

                connection.ConnectionString = TrimFuel.Business.Config.Current.CONNECTION_STRINGS["TrimFuel"];
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;

                string query = "select count(*) as Count from ReturnedSale where SaleID = @saleId";

                command.CommandText = query;
                command.Parameters.Add("@saleId", saleID);

                int count = (int)command.ExecuteScalar();

                isReturned = count > 0;

                command.Dispose();
                connection.Close();
            }
            catch
            {
                isReturned = true;
            }

            return isReturned;
        }

        private bool IsCalledIn(int billingID)
        {
            MySqlConnection connection;
            MySqlCommand command;

            bool isCalledIn = false;

            try
            {
                connection = new MySqlConnection();
                command = new MySqlCommand();

                connection.ConnectionString = TrimFuel.Business.Config.Current.CONNECTION_STRINGS["TrimFuel"];
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;

                string query = "Select count(*) as count From Notes Where BillingID= @billindgID and AdminID<>0";

                command.CommandText = query;
                command.Parameters.Add("@billingID", billingID);

                int count = (int)command.ExecuteScalar();

                isCalledIn = count > 0;

                command.Dispose();
                connection.Close();
            }
            catch
            {
                isCalledIn = false;
            }

            return isCalledIn;
        }

        private bool ReturnSale(int saleID, int billingID, string reason)
        {
            MySqlConnection connection;
            MySqlCommand command;

            bool success = false;

            try
            {
                connection = new MySqlConnection();
                command = new MySqlCommand();

                connection.ConnectionString = TrimFuel.Business.Config.Current.CONNECTION_STRINGS["TrimFuel"];
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;

                string query = @"
                    insert into ReturnedSale (SaleID, ReturnDate, Reason)
                    values (@saleID, @mysqlNow, @reason)";

                command.CommandText = query;
                command.Parameters.Add("@saleId", saleID);
                command.Parameters.Add("@mysqlNow", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.Add("@reason", reason);

                success = command.ExecuteNonQuery() == 1;

                command.Dispose();
                connection.Close();
            }
            catch
            {
                success = false;
            }

            //ProcessReturnAction(billingID, false, true, saleID, IsCalledIn(billingID));
            //value for adminNoteExists will be determined in webrequest
            ProcessReturnAction(billingID, false, true, saleID, true);

            return success;
        }

        private void ProcessReturnAction(int billingID, bool isRMA, bool isSale, int saleID, bool adminNoteExists)
        {
            string url = string.Format("http://dashboard.trianglecrm.com/service/process_return_action.asp?billingID={0}&saleID={1}", billingID, saleID);

            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

            try
            {
	            HttpWebResponse res = req.GetResponse() as HttpWebResponse;

	            StreamReader reader = new StreamReader(res.GetResponseStream());
	        
	            string result = reader.ReadToEnd();

	            reader.Close();
	        }
            catch
            {
                //catch
            }
	  
        }        
    }
}
