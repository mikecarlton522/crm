using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MySql.Data.MySqlClient;

using TrimFuel.Tools.PackingManager.Endicia;

namespace TrimFuel.Tools.PackingManager.Logic
{
    public class TrimFuelService
    {
        const string QUERY_SALE_TYPE = "select SaleTypeID from Sale where SaleID = @saleID";

        public static LabelRequest FetchLabelRequest(string connectionString, long saleID, int currentIndex)
        {
            string query = @"
                select t.SaleID, t.BillingID, t.`Date`, t.FirstName, t.LastName, t.Address1, t.Address2, t.City, t.Zip, t.State, t.Phone, i.SKU SkuCode, t.Quantity * pci.Quantity SKUQuantity, i.Product SKUDescription, p.ProductName from
                (
                select s.ProductID, tfr.RegID, bsa.SaleID, b.BillingID, ch.ChargeDate `Date`, bsa.ProductCode, bsa.Quantity, r.FirstName, r.LastName, r.Address1, r.Address2, r.City, r.Zip, r.State, r.Phone from TFRecord tfr
                inner join BillingSale bsa on bsa.SaleID = tfr.SaleID
                inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsa.ChargeHistoryID
                inner join BillingSubscription bs on bs.BillingSubscriptionID = bsa.BillingSubscriptionID
                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                inner join Billing b on b.BillingID = bs.BillingID
                inner join Registration r on r.RegistrationID = b.RegistrationID
                where tfr.RegID = @saleID and tfr.Cancelled = 0
                union all
                select s.ProductID, tfr.RegID, usa.SaleID, b.BillingID, ch.ChargeDate `Date`, u.ProductCode, u.Quantity, r.FirstName, r.LastName, r.Address1, r.Address2, r.City, r.Zip, r.State, r.Phone from TFRecord tfr
                inner join UpsellSale usa on usa.SaleID = tfr.SaleID
                inner join Upsell u on u.UpsellID = usa.UpsellID
                inner join ChargeHistoryEx ch on ch.ChargeHistoryID = usa.ChargeHistoryID
                inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID
                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                inner join Billing b on b.BillingID = u.BillingID
                inner join Registration r on r.RegistrationID = b.RegistrationID
                where tfr.RegID = @saleID and tfr.Cancelled = 0
                union all
                select s.ProductID, tfr.RegID, etss.SaleID, b.BillingID, ets.CreateDT `Date`, ets.ProductCode, ets.Quantity, r.FirstName, r.LastName, r.Address1, r.Address2, r.City, r.Zip, r.State, r.Phone from TFRecord tfr         
                inner join ExtraTrialShipSale etss on etss.SaleID = tfr.SaleID     
                inner join ExtraTrialShip ets on ets.ExtraTrialShipID = etss.ExtraTrialShipID  
                inner join Billing b on b.BillingID = etss.BillingID           
                inner join BillingSubscription bs on bs.BillingID = b.BillingID       
                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID  
                inner join Registration r on r.RegistrationID = b.RegistrationID
                where tfr.RegID = @saleID and tfr.Cancelled = 0
                ) t
                inner join ProductCode pc on pc.ProductCode = t.ProductCode
                inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID
                inner join Inventory i on i.InventoryID = pci.InventoryID
                inner join Product p on p.ProductID = t.ProductID;";

            string queryNew = @"
                select t.*, p.ProductName from (
                    select os.SaleID, b.BillingID, os.ProcessDT as `Date`, r.FirstName, r.LastName, r.Address1, r.Address2, r.City, r.Zip, r.State, r.Phone, i.SKU SkuCode, count(i.SKU) as SKUQuantity, i.Product as SKUDescription, o.ProductID
                    from Shipment sh
                    inner join Inventory i on i.SKU = sh.ProductSKU
                    inner join OrderSale os on os.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    inner join Billing b on b.BillingID = o.BillingID
                    inner join Registration r on r.RegistrationID = b.RegistrationID
                    where sh.ShipperRegID = @saleID and sh.ShipmentStatus >= 20 and sh.ShipperID = 10
                    group by os.SaleID, i.SKU
                ) t
                inner join Product p on p.ProductID = t.ProductID
                ";



            LabelRequest request = null;

            MySqlConnection connection = null;

            MySqlCommand command = null;

            try
            {

                connection = new MySqlConnection(connectionString);

                connection.Open();

                command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@SaleID", saleID);

                MySqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    reader.Close();

                    command.CommandText = queryNew;

                    //command.Parameters.AddWithValue("@SaleID", saleID);

                    reader = command.ExecuteReader();
                }

                if (reader.HasRows)
                {
                    request = new LabelRequest();

                    IList<long> saleIDList = new List<long>();
                    IList<string> skuCodes = new List<string>();
                    IList<int> skuQuantities = new List<int>();
                    IList<string> skuDescriptions = new List<string>();

                    while (reader.Read())
                    {
                        saleIDList.Add(Convert.ToInt64(reader["SaleID"]));
                        request.BillingID = Convert.ToInt64(reader["BillingID"]);
                        request.Date = Convert.ToDateTime(reader["Date"]); 
                        request.ToName = string.Concat(Convert.ToString(reader["FirstName"]), " ", Convert.ToString(reader["LastName"]));
                        request.ToAddress1 = Convert.ToString(reader["Address1"]);
                        request.ToAddress2 = Convert.ToString(reader["Address2"]);
                        request.ToCity = Convert.ToString(reader["City"]);
                        request.ToPostalCode = Convert.ToString(reader["Zip"]);
                        request.ToState = Convert.ToString(reader["State"]);                        
                        request.ToPhone = Convert.ToString(reader["Phone"]);                                                                       
                        skuCodes.Add(Convert.ToString(reader["SkuCode"]));
                        skuQuantities.Add(Convert.ToInt32(reader["SKUQuantity"]));
                        skuDescriptions.Add(Convert.ToString(reader["SkuDescription"]));
                        request.ProductName = Convert.ToString(reader["ProductName"]);
                    }
                    request.CurrentIndex = currentIndex;
                    request.SaleIDList = saleIDList.ToArray();
                    request.SKUCodes = skuCodes.ToArray();
                    request.SKUQuantities = skuQuantities.ToArray();
                    request.SKUDescriptions = skuDescriptions.ToArray();
                }

                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                if (connection != null)
                    connection.Dispose();
            }
        }
        
        public static void ReducePostageQuota(decimal costs, string client)
        {
            string query = @"update TPClient set PostageAllowed=PostageAllowed-@Costs where Name=@Name";

            MySqlConnection connection = null;

            MySqlCommand command = null;

            try
            {
                connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Triangle Media Corp"].ConnectionString);

                connection.Open();

                command = new MySqlCommand(query, connection);
                
                command.Parameters.AddWithValue("@Costs", costs);
                command.Parameters.AddWithValue("@Name", client);
                
                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                if (connection != null)
                    connection.Dispose();
            }
        }

        private static bool PostageDetailsExists(string connectionString, long saleID)
        {
            string query = "select count(*) as `Count` from SalePostageDetails where SaleID=@SaleID";

            MySqlConnection connection = null;

            MySqlCommand command = null;

            try
            {
                connection = new MySqlConnection(connectionString);

                connection.Open();

                command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@SaleID", saleID);

                int count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                if (connection != null)
                    connection.Dispose();
            }
        }

        public static void FinalizeTransaction(LabelRequest request, LabelRequestResponse response, CultureInfo culture)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[request.PartnerCustomerID].ConnectionString;

            StringBuilder query = new StringBuilder();
            
            foreach (long saleID in request.SaleIDList)
            {
                if (saleID == Convert.ToInt64(request.PartnerTransactionID))
                {
                    if (PostageDetailsExists(connectionString, saleID))
                    {
                        query.AppendFormat("update SalePostageDetails set Weight={0}, CreateDT='{1}', Costs={2}, TrackingNumber='{3}', MailClass='{4}' where SaleID={5};",
                            request.WeightOz.ToString(culture), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), response.FinalPostage.ToString(culture), response.TrackingNumber, request.MailClass, saleID);
                    }
                    else
                    {
                        query.AppendFormat("insert into SalePostageDetails (SaleID, CreateDT, Weight, Costs, TrackingNumber, MailClass) values ({0}, '{1}', {2}, {3}, '{4}', '{5}');",
                            saleID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.WeightOz.ToString(culture), response.FinalPostage.ToString(culture), response.TrackingNumber, request.MailClass);
                    }
                }

                query.AppendFormat("update Sale set TrackingNumber='{0}' where SaleID={1};", response.TrackingNumber, saleID);
                query.AppendFormat("update TFRecord set Completed=1 where SaleID={0};", saleID);
                query.AppendFormat("update Shipment set ShipmentStatus=30, ShipDT='{1}', TrackingNumber='{2}' where SaleID={0} and ShipmentStatus >= 20 and ShipperID = 10;", saleID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), response.TrackingNumber);
            }

            MySqlConnection connection = null;

            MySqlCommand command = null;

            try
            {
                connection = new MySqlConnection(connectionString);

                connection.Open();

                command = new MySqlCommand(query.ToString(), connection);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                if (connection != null)
                    connection.Dispose();
            }
        }

        public static bool QuotaExceeded(string client)
        {
            string query = @"select PostageAllowed from TPClient where Name=@Name";

            MySqlConnection connection = null;

            MySqlCommand command = null;

            try
            {
                connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Triangle Media Corp"].ConnectionString);

                connection.Open();

                command = new MySqlCommand(query, connection);
                
                command.Parameters.AddWithValue("@Name", client);

                decimal quota = Convert.ToDecimal(command.ExecuteScalar());

                return quota <= 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                if (connection != null)
                    connection.Dispose();
            }
        }

        public static bool ShipmentCancelled(string connectionString)
        {
            throw new NotImplementedException();
        }        

        public static bool IsValidUSZip(string postalCode)
        {
            string pattern = @"^(\d{5}-\d{4}|\d{5}|\d{9})$"; // Allows 5 digit, 5+4 digit and 9 digit zip codes

            Regex match = new Regex(pattern);

            return match.IsMatch(postalCode);
        }

        public static IList<long> GetSaleBundle(string connectionString, long saleID)
        {
            string str = "select SaleID from TFRecord where RegID = @saleID union select distinct SaleID from Shipment where ShipperRegID = @saleID and ShipperID = 10";
            
            IList<long> saleBundle = null;
            
            MySqlConnection connection = null;
            
            MySqlCommand command = null;
            
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                
                command = new MySqlCommand(str, connection);
                command.Parameters.AddWithValue("@saleID", saleID);
                
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    saleBundle = new List<long>();
                   
                    while (reader.Read())
                    {
                        saleBundle.Add(Convert.ToInt64(reader["SaleID"]));
                    }
                }

                return saleBundle;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        public static void UpdatePostageDetails(string connectionString, long saleID, string trackingNumber, double costs, double weight, string mailClass, CultureInfo culture)
        {
            IList<long> saleBundle = GetSaleBundle(connectionString, saleID);
            
            StringBuilder query = new StringBuilder();
            
            foreach (long bundledSaleID in saleBundle)
            {
                if (saleID == bundledSaleID)
                {
                    if (PostageDetailsExists(connectionString, saleID))
                    {
                        query.AppendFormat("update SalePostageDetails set Weight={0}, CreateDT='{1}', Costs={2}, TrackingNumber='{3}', MailClass='{4}' where SaleID={5};",
                            weight.ToString(culture), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), costs.ToString(culture), trackingNumber, mailClass, bundledSaleID );
                    }
                    else
                    {
                        query.AppendFormat("insert into SalePostageDetails (SaleID, CreateDT, Weight, Costs, TrackingNumber, MailClass) values ({0}, '{1}', {2}, {3}, '{4}', '{5}');",
                            bundledSaleID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), weight.ToString(culture), costs.ToString(culture), trackingNumber, mailClass);
                    }
                }
                
                query.AppendFormat("update Sale set TrackingNumber='{0}' where SaleID={1};", trackingNumber, bundledSaleID);
                query.AppendFormat("update TFRecord set Completed=1 where SaleID={0};", bundledSaleID);
                query.AppendFormat("update Shipment set ShipmentStatus=30, ShipDT='{1}', TrackingNumber='{2}' where SaleID={0} and ShipmentStatus >= 20 and ShipperID = 10;", bundledSaleID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), trackingNumber);
            }
            
            MySqlConnection connection = null;
            
            MySqlCommand command = null;
            
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                
                command = new MySqlCommand(query.ToString(), connection);
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                if (connection != null)
                    connection.Dispose();
            }
        }
    }
}
