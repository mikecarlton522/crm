using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Services;
using TrimFuel.Model.Containers;
using TrimFuel.Business;
using TrimFuel.Model;
using System.Configuration;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Web.Admin.Logic.Reports;
using log4net;

namespace TrimFuel.Web.Admin
{
    public partial class tpclient_report_test : PageX
    {
        private int _salesAgentID = 0;
        private List<TPClient> _tpClients = new List<TPClient>();

        protected IDictionary<string, object> Total { get; set; }
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.Params["salesAgentID"]))
            {
                _salesAgentID = Convert.ToInt32(Request.Params["salesAgentID"].ToString());
            }
            else
            {
                if (AdminMembership.CurrentAdmin != null)
                {
                    MySqlCommand q = new MySqlCommand(" select * from SalesAgent where AdminID = @adminID");
                    q.Parameters.AddWithValue("@adminID", AdminMembership.CurrentAdmin.AdminID);
                    IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                    SalesAgent salesAgent = dao.Load<SalesAgent>(q).FirstOrDefault();
                    if (salesAgent != null)
                        _salesAgentID = Convert.ToInt32(salesAgent.SalesAgentID.ToString());
                }
                else
                {
                    Response.Redirect("/login.asp");
                }
            }

            if (!IsPostBack)
            {
                DateFilter1.Date1 = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                DateFilter1.Date2 = DateTime.Today;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            FetchTPClients();

            decimal TotalShipmentFees = 0;
            decimal TotalShipmentSKUFees = 0;
            decimal TotalSetupFee = 0;
            decimal TotalReturnsFees = 0;
            decimal TotalKittingFees = 0;
            decimal TotalTransactionFees = 0;
            decimal TotalChargebackFees = 0;
            decimal TotalChargebackRepresentationFees = 0;
            decimal TotalDiscounts = 0;
            decimal TotalCallCenterSetupFee = 0;
            decimal TotalCallCenterHourFees = 0;
            decimal TotalCallCenterMonthlyFees = 0;
            decimal TotalRevenue = 0;

            List<TableRow> rows = new List<TableRow>();
            Total = new Dictionary<string, object>();

            StringBuilder html = new StringBuilder();
            string strGroupRow = @"<tr class=""master level0"" master-id=""c_{0}""><td>{1}</td>{2}</tr>";
            string strRow = @"<tr class=""master detail level1"" master-id=""c_{0}_p_{1}""detail-id=""c_{0}""><td>{1}</td>{2}</tr>";

            foreach (TPClient tpclient in _tpClients)
            {
                if (string.IsNullOrEmpty(tpclient.ConnectionString))
                    continue;

                TableRow row = new TableRow();
                MySqlConnection connection = new MySqlConnection();
                MySqlCommand command = new MySqlCommand();
                MySqlDataReader reader;


                row.Client = tpclient.Name;
                row.Managed = 0;
                row.ShipmentFee = 0;
                row.TotalShipmentFees = 0;
                row.ShipmentSKUFee = 0;
                row.TotalShipmentSKUFees = 0;
                row.SetupFee = 0;
                row.ReturnsFee = 0;
                row.TotalReturnsFees = 0;
                row.KittingFee = 0;
                row.TotalKittingFees = 0;
                row.TransactionFee = 0;
                row.TotalTransactionFees = 0;
                row.ChargebackFee = 0;
                row.TotalChargebackFees = 0;
                row.ChargebackRepresentationFee = 0;
                row.TotalChargebackRepresentationFees = 0;
                row.DiscountRate = 0;
                row.TotalDiscounts = 0;
                row.CallCenterHours = 0;
                row.CallCenterSetupFee = 0;
                row.CallCenterPerHourFee = 0;
                row.TotalCallCenterHourFees = 0;
                row.CallCenterMonthlyFee = 0;
                row.TotalCallCenterMonthlyFees = 0;
                row.TotalRevenue = 0;

                string queryShippers = @"
                select r.ShipperID, r.SaleID, sub.ProductID from 
                (
                    select 2 as ShipperID, SaleID
                    from TSNRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                    union
                    select 4 as ShipperID, SaleID
                    from ABFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                    union
                    select 5 as ShipperID, SaleID
                    from KeymailRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                    union
                    select 6 as ShipperID, SaleID
                    from ALFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                    union
                    select 7 as ShipperID, SaleID
                    from GFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                    union
                    select 8 as ShipperID, SaleID
                    from NPFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                    union
                    select 9 as ShipperID, SaleID
                    from MBRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                ) r
                left join BillingSale bs on bs.SaleID = r.SaleID
                left join UpsellSale us on us.SaleID = r.SaleID
                join ChargeHistoryEx ch on ch.ChargeHistoryID = coalesce(bs.ChargeHistoryID, us.ChargeHistoryID)
                join BillingSubscription bsub on bsub.BillingSubscriptionID = ch.BillingSubscriptionID
                join Subscription sub on sub.SubscriptionID = bsub.SubscriptionID
                ";


                try
                {
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command.Connection = connection;

                    // SHIPMENTS
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        coalesce(avg(case when Shipments > 0 then ShipmentFee else null end),0) as ShipmentFee,
                        count(Shipments) as Shipments,
                        sum(ShipmentFee*Shipments) as TotalShipments
                    from (
                        select sh.ShipperID, s.ProductID, count(*) as Shipments,
                            coalesce(shs.ShipmentFeeRetail-shs.ShipmentFee,0) as ShipmentFee
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        left join ( " + queryShippers + @" ) s on s.ShipperID = sh.ShipperID
                        group by sh.ShipperID, s.ProductID
                    ) s
                    join Product p on p.ProductID = s.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.ShipmentFee = tpclient.Commission * Convert.ToDecimal(reader["ShipmentFee"].ToString());
                        pRow.TotalShipmentFees = tpclient.Commission * Convert.ToDecimal(reader["TotalShipments"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // SHIPMENTS SKU
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        coalesce(avg(case when ShipmentsSKU > 0 then ShipmentSKUFee else null end),0) as ShipmentSKUFee,
                        sum(ShipmentSKUFee*ShipmentsSKU) as TotalShipmentsSKU
                    from (
                        select sh.ShipperID, ps.ProductID, count(*) as ShipmentsSKU,
                            coalesce(shs.ShipmentSKUFeeRetail-shs.ShipmentSKUFee,0) as ShipmentSKUFee
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        join (
                            select sh.ShipperID, sh.ProductID, s.ProductCode, s.ExtraTrialShipID as ID from ExtraTrialShip s
                            join ExtraTrialShipSale ss on ss.ExtraTrialShipID = s.ExtraTrialShipID
                            join ( " + queryShippers + @" ) sh on sh.SaleID = ss.SaleID
                            union
                            select sh.ShipperID, sh.ProductID, s.ProductCode, s.UpsellID as ID from Upsell s
                            join UpsellSale ss on ss.UpsellID = s.UpsellID
                            join ( " + queryShippers + @" ) sh on sh.SaleID = ss.SaleID
                            union
                            select sh.ShipperID, sh.ProductID, s.ProductCode, s.SaleID as ID from BillingSale s
                            join ( " + queryShippers + @" ) sh on sh.SaleID = s.SaleID
                        ) ps on ps.ShipperID = sh.ShipperID
                        join ProductCode pc on pc.ProductCode = ps.ProductCode
                        join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID
                        join Inventory i on i.InventoryID = pci.InventoryID
                        group by sh.ShipperID, ps.ProductID
                    ) s
                    join Product p on p.ProductID = s.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.ShipmentSKUFee = tpclient.Commission * Convert.ToDecimal(reader["ShipmentSKUFee"].ToString());
                        pRow.TotalShipmentSKUFees = tpclient.Commission * Convert.ToDecimal(reader["TotalShipmentsSKU"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // SETUP
                    string queryText = @"
                    select coalesce(sum(coalesce(SetupFeeRetail-SetupFee,0)), 0) as SetupFee
                    from Shipper sh
                    join ShipperSettings shs on shs.ShipperID = sh.ShipperID;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.SetupFee = tpclient.Commission * Convert.ToDecimal(reader["SetupFee"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // RETURNS
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        coalesce(avg(case when Operations > 0 then ReturnsFee else null end),0) as ReturnsFee,
                        sum(ReturnsFee*Operations) as TotalReturns
                    from (
                        select sh.ShipperID, s.ProductID, count(*) as Operations,
                            coalesce(shs.ReturnsFeeRetail-shs.ReturnsFee,0) as ReturnsFee
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        left join ( " + queryShippers + @" ) s on s.ShipperID = sh.ShipperID
                        join ReturnedSale rs on rs.SaleID = s.SaleID
                        group by sh.ShipperID, s.ProductID
                    ) s
                    join Product p on p.ProductID = s.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.ReturnsFee = tpclient.Commission * Convert.ToDecimal(reader["ReturnsFee"].ToString());
                        pRow.TotalReturnsFees = tpclient.Commission * Convert.ToDecimal(reader["TotalReturns"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // KITTINGS
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        coalesce(avg(case when Kittings > 0 then KittingFee else null end),0) as KittingFee,
                        sum(KittingFee*Kittings) as TotalKittings
                    from (
                        select sh.ShipperID, s.ProductID, count(*) as Kittings,
                            coalesce(shs.KittingFeeRetail-shs.KittingFee,0) as KittingFee
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        left join (
                          select r.ShipperID, r.RegID, sub.ProductID from 
                          (
                            select 2 as ShipperID, SaleID, RegID
                            from TSNRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                            union
                            select 4 as ShipperID, SaleID, RegID
                            from ABFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                            union
                            select 5 as ShipperID, SaleID, RegID
                            from KeymailRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                            union
                            select 6 as ShipperID, SaleID, RegID
                            from ALFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                            union
                            select 7 as ShipperID, SaleID, RegID
                            from GFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                            union
                            select 8 as ShipperID, SaleID, RegID
                            from NPFRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                            union
                            select 9 as ShipperID, SaleID, RegID
                            from MBRecord where RegID is not null and Completed = 1 and CreateDT between @start and @end
                          ) r
                          left join BillingSale bs on bs.SaleID = r.SaleID
                          left join UpsellSale us on us.SaleID = r.SaleID
                          join ChargeHistoryEx ch on ch.ChargeHistoryID = coalesce(bs.ChargeHistoryID, us.ChargeHistoryID)
                          join BillingSubscription bsub on bsub.BillingSubscriptionID = ch.BillingSubscriptionID
                          join Subscription sub on sub.SubscriptionID = bsub.SubscriptionID
                        ) s on s.ShipperID = sh.ShipperID
                        group by sh.ShipperID, s.ProductID
                    ) s
                    join Product p on p.ProductID = s.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.KittingFee = tpclient.Commission * Convert.ToDecimal(reader["KittingFee"].ToString());
                        pRow.TotalKittingFees = tpclient.Commission * Convert.ToDecimal(reader["TotalKittings"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // GATEWAY - TRANSACTIONS
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        coalesce(avg(case when Transactions > 0 then TransactionFee else null end),0) as TransactionFee,
                        sum(TransactionFee*Transactions*MIDProcessorFactor) as TotalTransactions,
                        max(Managed) as Managed
                    from (
                        select am.AssertigyMIDID, ch.ProductID,
                            coalesce(am.TransactionFee-ams.TransactionFee,0) as TransactionFee,
                            coalesce(ch.Transactions,0) as Transactions,
                            cast(coalesce(ams.Managed,0) as signed) as Managed,
                            1 - coalesce(mp.CommissionPercent, 0) / 100 as MIDProcessorFactor
                        from AssertigyMID am
                        join AssertigyMIDSettings ams on am.AssertigyMIDID = ams.AssertigyMIDID
                        left join MIDProcessor mp on mp.MIDProcessorID = ams.MIDProcessorID
                        join (
                            select MerchantAccountID, ProductID, count(*) as Transactions
                            from (
                                select ChargeHistoryID as ID, MerchantAccountID, s.ProductID from ChargeHistoryEx ch
                                join BillingSubscription bis on bis.BillingSubscriptionID = ch.BillingSubscriptionID
                                join Subscription s on s.SubscriptionID = bis.SubscriptionID
                                where ch.ChargeDate between @start and @end
                                union
                                select FailedChargeHistoryID as ID, MerchantAccountID, s.ProductID from FailedChargeHistory fch
                                join Billing b on b.BillingID = fch.BillingID
                                join BillingSubscription bis on bis.BillingID = b.BillingID
                                join Subscription s on s.SubscriptionID = bis.SubscriptionID
                                where fch.ChargeDate between @start and @end
                            ) c
                            group by MerchantAccountID, ProductID
                        ) ch on ch.MerchantAccountID = am.AssertigyMIDID
                    ) c
                    join Product p on p.ProductID = c.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.TransactionFee = tpclient.Commission * Convert.ToDecimal(reader["TransactionFee"].ToString());
                        pRow.TotalTransactionFees = tpclient.Commission * Convert.ToDecimal(reader["TotalTransactions"].ToString());

                        int Managed = Convert.ToInt32(reader["Managed"].ToString());
                        if (Managed > 0)
                        {
                            pRow.Managed = Managed;
                            row.Managed = Managed;
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // GATEWAY - CHARGEBACKS
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        coalesce(avg(case when Chargebacks > 0 then ChargebackFee else null end),0) as ChargebackFee,
                        sum(ChargebackFee*Chargebacks) as TotalChargebacks,
                        coalesce(avg(case when Chargebacks > 0 then ChargebackRepresentationFee else null end),0) as ChargebackRepresentationFee,
                        sum(ChargebackRepresentationFee*Chargebacks) as TotalChargebackRepresentations,
                        max(Managed) as Managed
                    from (
                        select am.AssertigyMIDID, chb.ProductID,
                            coalesce(am.ChargebackFee-ams.ChargebackFee,0) as ChargebackFee,
                            coalesce(ams.ChargebackRepresentationFeeRetail-ams.ChargebackRepresentationFee,0) as ChargebackRepresentationFee,
                            coalesce(chb.Chargebacks,0) as Chargebacks,
                            cast(coalesce(ams.Managed,0) as signed) as Managed
                        from AssertigyMID am
                        join AssertigyMIDSettings ams on am.AssertigyMIDID = ams.AssertigyMIDID
                        join (
                            select MerchantAccountID, ProductID, count(*) as Chargebacks
                            from (
                                select sch.SaleChargebackID, ch.MerchantAccountID, s.ProductID from SaleChargeback sch
                                join BillingSale bs on bs.SaleID = sch.SaleID
                                join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID
                                left join BillingSubscription bis on bis.BillingSubscriptionID = ch.BillingSubscriptionID
                                left join Subscription s on s.SubscriptionID = bis.SubscriptionID
                                where ch.ChargeDate between @start and @end
                                union
                                select sch.SaleChargebackID, ch.MerchantAccountID, s.ProductID from SaleChargeback sch
                                join UpsellSale us on us.SaleID = sch.SaleID
                                join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID
                                join BillingSubscription bis on bis.BillingSubscriptionID = ch.BillingSubscriptionID
                                join Subscription s on s.SubscriptionID = bis.SubscriptionID
                                where ch.ChargeDate between @start and @end
                             ) c
                            group by MerchantAccountID, ProductID
                        ) chb on chb.MerchantAccountID = am.AssertigyMIDID
                    ) c
                    join Product p on p.ProductID = c.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.ChargebackFee = tpclient.Commission * Convert.ToDecimal(reader["ChargebackFee"].ToString());
                        pRow.TotalChargebackFees = tpclient.Commission * Convert.ToDecimal(reader["TotalChargebacks"].ToString());
                        pRow.ChargebackRepresentationFee = tpclient.Commission * Convert.ToDecimal(reader["ChargebackRepresentationFee"].ToString());
                        pRow.TotalChargebackRepresentationFees = tpclient.Commission * Convert.ToDecimal(reader["TotalChargebackRepresentations"].ToString());

                        int Managed = Convert.ToInt32(reader["Managed"].ToString());
                        if (Managed > 0)
                        {
                            pRow.Managed = Managed;
                            row.Managed = Managed;
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // GATEWAY - DISCOUNTS
                    string queryText = @"
                    select p.ProductID, p.ProductName,
                        round(coalesce(avg(case when ChargeAmount > 0 then DiscountRate/100 else null end),0),4) as DiscountRate,
                        round(sum(DiscountRate*ChargeAmount*MIDProcessorFactor/100),2) as TotalDiscounts,
                        max(Managed) as Managed
                    from (
                        select am.AssertigyMIDID, dr.ProductID,
                            coalesce(am.ProcessingRate-ams.DiscountRate,0) as DiscountRate,
                            coalesce(dr.ChargeAmount,0) as ChargeAmount,
                            cast(coalesce(ams.Managed,0) as signed) as Managed,
                            1 - coalesce(mp.CommissionPercent, 0) / 100 as MIDProcessorFactor
                        from AssertigyMID am
                        join AssertigyMIDSettings ams on am.AssertigyMIDID = ams.AssertigyMIDID
                        left join MIDProcessor mp on mp.MIDProcessorID = ams.MIDProcessorID
                        join (
                            select ch.MerchantAccountID, s.ProductID, sum(Amount) as ChargeAmount from ChargeHistoryEx ch
                            left join BillingSubscription bis on bis.BillingSubscriptionID = ch.BillingSubscriptionID
                            left join Subscription s on s.SubscriptionID = bis.SubscriptionID
                            where ch.Success = 1 and ch.Amount > 0 and ch.ChargeDate between @start and @end
                            group by ch.MerchantAccountID
                        ) dr on dr.MerchantAccountID = am.AssertigyMIDID
                    ) c
                    join Product p on p.ProductID = c.ProductID
                    group by p.ProductID, p.ProductName;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TableRow pRow = tpclient.FindRow(Convert.ToInt32(reader["ProductID"].ToString()), reader["ProductName"].ToString());
                        pRow.DiscountRate = tpclient.Commission * Convert.ToDecimal(reader["DiscountRate"].ToString());
                        pRow.TotalDiscounts = tpclient.Commission * Convert.ToDecimal(reader["TotalDiscounts"].ToString());

                        int Managed = Convert.ToInt32(reader["Managed"].ToString());
                        if (Managed > 0)
                        {
                            pRow.Managed = Managed;
                            row.Managed = Managed;
                        }
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (tpclient.ConnectionString);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // CALL CENTER
                    string queryText = @"
                    select coalesce(SetupFee,0) as SetupFee, 
                        coalesce(PerPourFeeRetail-PerPourFee,0) as PerHourFee,
                        coalesce(MonthlyFeeRetail-MonthlyFee,0) as MonthlyFee
                    from LeadPartnerSettings lps
                    join LeadPartner lp on lp.LeadPartnerID = lps.LeadPartnerID
                    where lp.DisplayName = 'Focus';";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.CallCenterSetupFee = tpclient.Commission * Convert.ToDecimal(reader["SetupFee"].ToString());
                        row.CallCenterPerHourFee = tpclient.Commission * Convert.ToDecimal(reader["PerHourFee"].ToString());
                        row.CallCenterMonthlyFee = tpclient.Commission * Convert.ToDecimal(reader["MonthlyFee"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }


                try
                {
                    connection = new MySqlConnection();
                    connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                    connection.Open();
                    command = new MySqlCommand();
                    command.Connection = connection;

                    // CALL CENTER
                    string queryText = @"
                    select coalesce(sum(c.EndDT-c.StartDT)/6000,0) as Hours
                    from TPClientFocusCustomerProduct tf
                    join `Call` c on c.CustomerProduct = tf.CustomerProduct
                    where tf.TPClientID = @TPClientID and c.StartDT between @start and @end;";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@TPClientID", tpclient.ID.ToString());
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.CallCenterHours = tpclient.Commission * Convert.ToDecimal(reader["Hours"].ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger.Error(GetType(), exception);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }

                // process Products
                foreach (TableRow pRow in tpclient.Rows)
                {
                    if (pRow.Managed == 0)
                    {
                        pRow.TotalShipmentFees = 0;
                        pRow.TotalShipmentSKUFees = 0;
                        pRow.SetupFee = 0;
                        pRow.TotalReturnsFees = 0;
                        pRow.TotalKittingFees = 0;
                    }

                    row.TotalShipmentFees += pRow.TotalShipmentFees;
                    row.TotalShipmentSKUFees += pRow.TotalShipmentSKUFees;
                    row.TotalReturnsFees += pRow.TotalReturnsFees;
                    row.TotalKittingFees += pRow.TotalKittingFees;
                    row.TotalTransactionFees += pRow.TotalTransactionFees;
                    row.TotalChargebackFees += pRow.TotalChargebackFees;
                    row.TotalChargebackRepresentationFees += pRow.TotalChargebackRepresentationFees;
                    row.TotalDiscounts += pRow.TotalDiscounts;

                    pRow.TotalRevenue = pRow.TotalShipmentFees + pRow.TotalShipmentSKUFees + pRow.SetupFee + pRow.TotalReturnsFees + pRow.TotalKittingFees +
                        pRow.TotalTransactionFees + pRow.TotalChargebackFees + pRow.TotalChargebackRepresentationFees + pRow.TotalDiscounts;
                }

                TimeSpan span = DateFilter1.Date2WithTime.Subtract(DateFilter1.Date1WithTime);
                int months = span.Days / 30;
                row.TotalCallCenterMonthlyFees = months * row.CallCenterMonthlyFee;
                row.TotalCallCenterHourFees = row.CallCenterHours * row.CallCenterPerHourFee;

                if (row.Managed == 0)
                {
                    row.SetupFee = 0;
                    row.CallCenterSetupFee = 0;
                    row.TotalCallCenterHourFees = 0;
                    row.TotalCallCenterMonthlyFees = 0;
                }

                row.TotalRevenue = row.TotalShipmentFees + row.TotalShipmentSKUFees + row.SetupFee + row.TotalReturnsFees + row.TotalKittingFees +
                    row.TotalTransactionFees + row.TotalChargebackFees + row.TotalChargebackRepresentationFees + row.TotalDiscounts +
                    row.CallCenterSetupFee + row.TotalCallCenterHourFees + row.TotalCallCenterMonthlyFees;

                // format group row
                html.AppendFormat(strGroupRow, row.Client, row.Client, row.FormatToString());
                html.AppendLine();

                // temporary add Product rows
                foreach (TableRow pRow in tpclient.Rows)
                {
                    html.AppendFormat(strRow, pRow.Client, pRow.Product, pRow.FormatToString());
                    html.AppendLine();
                }

                TotalShipmentFees += row.TotalShipmentFees;
                TotalShipmentSKUFees += row.TotalShipmentSKUFees;
                TotalSetupFee += row.SetupFee;
                TotalReturnsFees += row.TotalReturnsFees;
                TotalKittingFees += row.TotalKittingFees;
                TotalTransactionFees += row.TotalTransactionFees;
                TotalChargebackFees += row.TotalChargebackFees;
                TotalChargebackRepresentationFees += row.TotalChargebackRepresentationFees;
                TotalDiscounts += row.TotalDiscounts;
                TotalCallCenterSetupFee += row.CallCenterSetupFee;
                TotalCallCenterHourFees += row.TotalCallCenterHourFees;
                TotalCallCenterMonthlyFees += row.TotalCallCenterMonthlyFees;
                TotalRevenue += row.TotalRevenue;
            }

            ltOutput.Text = html.ToString();

            Total["TotalShipmentFees"] = TotalShipmentFees;
            Total["TotalShipmentSKUFees"] = TotalShipmentSKUFees;
            Total["TotalSetupFee"] = TotalSetupFee;
            Total["TotalReturnsFees"] = TotalReturnsFees;
            Total["TotalKittingFees"] = TotalKittingFees;
            Total["TotalTransactionFees"] = TotalTransactionFees;
            Total["TotalChargebackFees"] = TotalChargebackFees;
            Total["TotalChargebackRepresentationFees"] = TotalChargebackRepresentationFees;
            Total["TotalDiscounts"] = TotalDiscounts;
            Total["TotalCallCenterSetupFee"] = TotalCallCenterSetupFee;
            Total["TotalCallCenterHourFees"] = TotalCallCenterHourFees;
            Total["TotalCallCenterMonthlyFees"] = TotalCallCenterMonthlyFees;
            Total["TotalRevenue"] = TotalRevenue;
        }

        protected string ShowPrice(object a)
        {
            if (a != null && !(a is DBNull))
            {
                decimal temp = Convert.ToDecimal(a);
                return temp.ToString("$0.00");
            }
            return null;
        }

        private void FetchTPClients()
        {
            MySqlConnection connection = new MySqlConnection();
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;
                if (_salesAgentID == 0)
                {
                    command.CommandText = @" select *, 1.00 as Commission from TPClient";
                }
                else
                {
                    command.CommandText = @"
                        select c.*, coalesce(sa.Commission/100,1.00) as Commission from TPClient c
                        join SalesAgentTPClient sac on sac.TPClientID = c.TPClientID
                        join SalesAgent sa on sa.SalesAgentID = sac.SalesAgentID
                        where sac.SalesAgentID = @SalesAgentID";
                    command.Parameters.Add("@SalesAgentID", MySqlDbType.Int32).Value = _salesAgentID;
                }

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TPClient tpClient = new TPClient();
                    tpClient.ID = (int)reader["TPClientID"];
                    tpClient.Name = reader["name"] as string;
                    tpClient.ConnectionString = reader["connectionstringDotNet"] as string;
                    tpClient.GatewayFee = (decimal)reader["gatewayFee"];
                    tpClient.ProcessorFee = (decimal)reader["processorFee"];
                    tpClient.CRMFee = (decimal)reader["CRMFee"];
                    tpClient.ChargebackFee = (decimal)reader["chargebackFee"];
                    tpClient.Commission = (decimal)reader["Commission"];
                    _tpClients.Add(tpClient);
                }
            }
            finally
            {
                if (connection != null)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
        }

        public override string HeaderString
        {
            get { return "Client Report"; }
        }

        private class TPClient
        {
            public List<TableRow> Rows = new List<TableRow>();

            public int ID { get; set; }
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public decimal GatewayFee { get; set; }
            public decimal ProcessorFee { get; set; }
            public decimal CRMFee { get; set; }
            public decimal ChargebackFee { get; set; }
            public decimal Commission { get; set; }

            public TableRow FindRow(int ProductID, String ProductName)
            {
                foreach (TableRow row in Rows)
                {
                    if (row.ProductID == ProductID)
                    {
                        return row;
                    }
                }

                TableRow newRow = new TableRow();
                newRow.Client = Name;
                newRow.ProductID = ProductID;
                newRow.Product = ProductName;
                newRow.Managed = 0;
                Rows.Add(newRow);

                return newRow;
            }
        }

        private class TableRow
        {
            public string Client { get; set; }
            public int ProductID { get; set; }
            public string Product { get; set; }
            public int Managed { get; set; }
            public decimal ShipmentFee { get; set; }
            public decimal TotalShipmentFees { get; set; }
            public decimal ShipmentSKUFee { get; set; }
            public decimal TotalShipmentSKUFees { get; set; }
            public decimal SetupFee { get; set; }
            public decimal ReturnsFee { get; set; }
            public decimal TotalReturnsFees { get; set; }
            public decimal KittingFee { get; set; }
            public decimal TotalKittingFees { get; set; }
            public decimal TransactionFee { get; set; }
            public decimal TotalTransactionFees { get; set; }
            public decimal ChargebackFee { get; set; }
            public decimal TotalChargebackFees { get; set; }
            public decimal ChargebackRepresentationFee { get; set; }
            public decimal TotalChargebackRepresentationFees { get; set; }
            public decimal DiscountRate { get; set; }
            public decimal TotalDiscounts { get; set; }
            public decimal CallCenterHours { get; set; }
            public decimal CallCenterSetupFee { get; set; }
            public decimal CallCenterPerHourFee { get; set; }
            public decimal TotalCallCenterHourFees { get; set; }
            public decimal CallCenterMonthlyFee { get; set; }
            public decimal TotalCallCenterMonthlyFees { get; set; }
            public decimal TotalRevenue { get; set; }

            public String FormatToString()
            {
                return String.Format(@"
                    <td>{0:C2}</td>
                    <td>{1:C2}</td>
                    <td>{2:C2}</td>
                    <td>{3:C2}</td>
                    <td>{4:C2}</td>
                    <td>{5:C2}</td>
                    <td>{6:C2}</td>
                    <td>{7:C2}</td>
                    <td>{8:C2}</td>
                    <td>{9:C2}</td>
                    <td>{10:C2}</td>
                    <td>{11:C2}</td>
                    <td>{12:C2}</td>
                    <td>{13:C2}</td>
                    <td>{14:C2}</td>
                    <td>{15:P2}</td>
                    <td>{16:C2}</td>
                    <td>{17:C2}</td>
                    <td>{18:C2}</td>
                    <td>{19:C2}</td>
                    <td>{20:C2}</td>
                    <td>{21:C2}</td>
                    <td>{22:C2}</td>",
                    ShipmentFee, TotalShipmentFees, ShipmentSKUFee, TotalShipmentSKUFees, SetupFee,
                    ReturnsFee, TotalReturnsFees, KittingFee, TotalKittingFees, TransactionFee, TotalTransactionFees,
                    ChargebackFee, TotalChargebackFees, ChargebackRepresentationFee, TotalChargebackRepresentationFees,
                    DiscountRate, TotalDiscounts, CallCenterSetupFee, CallCenterPerHourFee, TotalCallCenterHourFees,
                    CallCenterMonthlyFee, TotalCallCenterMonthlyFees, TotalRevenue);
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            DataBind();
        }
    }
}