using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using TrimFuel.Model;
using log4net;

namespace TrimFuel.Web.Admin
{
    /// <summary>
    /// Notes:
    /// calculates shipment fee by number of sales, should be by number of shipments? (group by RegID)
    /// calculates shipment setup (and other services setup) fee for any date range, should take into account only 1 time?
    /// calculates return sale fee by date of shiment, should be by return date?
    /// calculates chargeback fees by date of charge, should be by post date of chargeback?
    /// </summary>
    public partial class salesagent_management : PageX
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));
        private SalesAgentService salesAgentService = new SalesAgentService();
        private IList<TrimFuel.Model.Admin> admins = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateFilter1.Date1 = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                DateFilter1.Date2 = DateTime.Today;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            admins = salesAgentService.GetAdminList();
            rSalesAgents.DataSource = salesAgentService.GetSalesAgentList();

            List<TableRow> rows = new List<TableRow>();
            foreach (TPClient tpclient in FetchTPClients())
            {
                if (string.IsNullOrEmpty(tpclient.ConnectionString))
                    continue;

                TableRow row = new TableRow();
                using (MySqlConnection connection = new MySqlConnection(tpclient.ConnectionString))
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand();
                    MySqlDataReader reader;

                    row.TPClientID = tpclient.ID;
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
                    row.TransactionNumber = 0;
                    row.TransactionFee = 0;
                    row.TotalTransactionFees = 0;
                    row.GatewayFee = 0;
                    row.TotalGatewayFees = 0;
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
                    row.TotalClientRevenue = 0;

                    command.Connection = connection;

                    // SHIPMENTS
                    string queryText = @"
                        select 
                            coalesce(count(*),0) as ShipmentsCount,
                            coalesce(avg(coalesce(shs.ShipmentFeeRetail, 0)), 0) as ShipmentFee,
                            coalesce(sum(coalesce(shs.ShipmentFeeRetail, 0)), 0) as TotalShipments
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        join AggShipperSale s on s.ShipperID = sh.ShipperID
                        where s.CreateDT between @start and @end
                    ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader["ShipmentsCount"].ToString());
                        row.TotalShipmentFees = Convert.ToDecimal(reader["TotalShipments"].ToString());
                        if (count > 0)
                            row.ShipmentFee = row.TotalShipmentFees / count;
                        else
                            row.ShipmentFee = Convert.ToDecimal(reader["ShipmentFee"].ToString());
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // SHIPMENTS SKU
                    queryText = @"
                        select
                            coalesce(count(*),0) as ShipmentsCount,
                            coalesce(avg(coalesce(shs.ShipmentSKUFeeRetail, 0)),0) as ShipmentSKUFee,
                            coalesce(sum(coalesce(shs.ShipmentSKUFeeRetail, 0)),0) as TotalShipmentsSKU
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        join (
                            select sh.ShipperID, s.ProductCode, s.ExtraTrialShipID as ID from ExtraTrialShip s
                            join ExtraTrialShipSale ss on ss.ExtraTrialShipID = s.ExtraTrialShipID
                            join AggShipperSale sh on sh.SaleID = ss.SaleID
                            where sh.CreateDT between @start and @end
                            union
                            select sh.ShipperID, s.ProductCode, s.UpsellID as ID from Upsell s
                            join UpsellSale ss on ss.UpsellID = s.UpsellID
                            join AggShipperSale sh on sh.SaleID = ss.SaleID
                            where sh.CreateDT between @start and @end
                            union
                            select sh.ShipperID, s.ProductCode, s.SaleID as ID from BillingSale s
                            join AggShipperSale sh on sh.SaleID = s.SaleID
                            where sh.CreateDT between @start and @end
                        ) ps on ps.ShipperID = sh.ShipperID
                        join ProductCode pc on pc.ProductCode = ps.ProductCode
                        join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID
                        join Inventory i on i.InventoryID = pci.InventoryID
                    ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader["ShipmentsCount"].ToString());
                        row.TotalShipmentSKUFees = Convert.ToDecimal(reader["TotalShipmentsSKU"].ToString());
                        if (count > 0)
                            row.ShipmentSKUFee = row.TotalShipmentSKUFees / count;
                        else
                            row.ShipmentSKUFee = Convert.ToDecimal(reader["ShipmentSKUFee"].ToString());
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // SETUP
                    queryText = @"
                        select coalesce(sum(coalesce(SetupFeeRetail,0)), 0) as SetupFee
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID;";
                    command.CommandText = queryText;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.SetupFee = Convert.ToDecimal(reader["SetupFee"].ToString());
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // RETURNS
                    queryText = @"
                        select
                            coalesce(count(*),0) as ReturnsCount,
                            coalesce(avg(coalesce(shs.ReturnsFeeRetail, 0)),0) as ReturnsFee,
                            coalesce(sum(coalesce(shs.ReturnsFeeRetail, 0)),0) as TotalReturns
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        join AggShipperSale s on s.ShipperID = sh.ShipperID
                        join ReturnedSale rs on rs.SaleID = s.SaleID
                        where s.CreateDT between @start and @end
                        ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader["ReturnsCount"].ToString());
                        row.TotalReturnsFees = Convert.ToDecimal(reader["TotalReturns"].ToString());
                        if (count > 0)
                            row.ReturnsFee = row.TotalReturnsFees / count;
                        else
                            row.ReturnsFee = Convert.ToDecimal(reader["ReturnsFee"].ToString());
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // KITTINGS
                    queryText = @"
                        select
                            coalesce(count(*),0) as KittingsCount,
                            coalesce(avg(coalesce(shs.KittingFeeRetail, 0)),0) as KittingFee,
                            coalesce(sum(coalesce(shs.KittingFeeRetail, 0)),0) as TotalKittings
                        from Shipper sh
                        join ShipperSettings shs on shs.ShipperID = sh.ShipperID
                        join AggShipperSale s on s.ShipperID = sh.ShipperID
                        where s.CreateDT between @start and @end
                    ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader["KittingsCount"].ToString());
                        row.TotalKittingFees = Convert.ToDecimal(reader["TotalKittings"].ToString());
                        if (count > 0)
                            row.KittingFee = row.TotalKittingFees / count;
                        else
                            row.KittingFee = Convert.ToDecimal(reader["KittingFee"].ToString());
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // GATEWAY - TRANSACTIONS
                    queryText = @"
                        select
                            coalesce(count(*),0) as TransactionNumber,
                            coalesce(avg(coalesce(am.TransactionFee, 0)),0) as TransactionFee,
                            coalesce(sum(coalesce(am.TransactionFee, 0)),0) as TotalTransactions,
                            coalesce(sum(case ch.Success when 1 then ch.Amount else 0 end),0) as TotalClientRevenue
                        from AssertigyMID am
                        join (
                            select MerchantAccountID, Amount, Success from ChargeHistoryEx ch
                            where ch.ChargeDate between @start and @end
                            union all
                            select MerchantAccountID, Amount, Success from FailedChargeHistory fch
                            where fch.ChargeDate between @start and @end
                        ) ch on ch.MerchantAccountID = am.AssertigyMIDID
                    ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.TotalTransactionFees = Convert.ToDecimal(reader["TotalTransactions"].ToString());
                        row.TotalClientRevenue = Convert.ToDecimal(reader["TotalClientRevenue"].ToString());
                        row.TransactionNumber = Convert.ToInt64(reader["TransactionNumber"].ToString());
                        if (row.TransactionNumber > 0)
                            row.TransactionFee = row.TotalTransactionFees / row.TransactionNumber;
                        else
                            row.TransactionFee = Convert.ToDecimal(reader["TransactionFee"].ToString());
                    }
                    reader.Close();

                    row.GatewayFee = tpclient.GatewayFee;
                    row.TotalGatewayFees = row.TransactionNumber * row.GatewayFee;

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // GATEWAY - CHARGEBACKS
                    queryText = @"
                        select
                            coalesce(count(*),0) as ChargebacksCount,
                            coalesce(avg(coalesce(am.ChargebackFee, 0)),0) as ChargebackFee,
                            coalesce(sum(coalesce(am.ChargebackFee, 0)),0) as TotalChargebacks,
                            coalesce(avg(coalesce(ams.ChargebackRepresentationFeeRetail, 0)),0) as ChargebackRepresentationFee,
                            coalesce(sum(coalesce(ams.ChargebackRepresentationFeeRetail, 0)),0) as TotalChargebackRepresentations
                        from AssertigyMID am
                        join AssertigyMIDSettings ams on am.AssertigyMIDID = ams.AssertigyMIDID
                        join (
                            select ch.MerchantAccountID from SaleChargeback sch
                            join BillingSale bs on bs.SaleID = sch.SaleID
                            join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID
                            where ch.ChargeDate between @start and @end
                            union all
                            select ch.MerchantAccountID from SaleChargeback sch
                            join UpsellSale us on us.SaleID = sch.SaleID
                            join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID
                            where ch.ChargeDate between @start and @end
                        ) chb on chb.MerchantAccountID = am.AssertigyMIDID
                    ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int count = Convert.ToInt32(reader["ChargebacksCount"].ToString());
                        row.TotalChargebackFees = Convert.ToDecimal(reader["TotalChargebacks"].ToString());
                        row.TotalChargebackRepresentationFees = Convert.ToDecimal(reader["TotalChargebackRepresentations"].ToString());
                        if (count > 0)
                        {
                            row.ChargebackFee = row.TotalChargebackFees / count;
                            row.ChargebackRepresentationFee = row.TotalChargebackRepresentationFees / count;
                        }
                        else
                        {
                            row.ChargebackFee = Convert.ToDecimal(reader["ChargebackFee"].ToString());
                            row.ChargebackRepresentationFee = Convert.ToDecimal(reader["ChargebackRepresentationFee"].ToString());
                        }
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // GATEWAY - DISCOUNTS
                    queryText = @"
                        select
                            round(coalesce(avg(coalesce(am.ProcessingRate/100, 0)),0),4) as DiscountRate,
                            round(coalesce(sum(coalesce(am.ProcessingRate*dr.ChargeAmount/100, 0)), 0),2) as TotalDiscounts
                        from AssertigyMID am
                        join (
                            select ch.MerchantAccountID, sum(Amount) as ChargeAmount from ChargeHistoryEx ch
                            where ch.Success = 1 and ch.Amount > 0 and ch.ChargeDate between @start and @end
                        ) dr on dr.MerchantAccountID = am.AssertigyMIDID
                    ";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.DiscountRate = Convert.ToDecimal(reader["DiscountRate"].ToString());
                        row.TotalDiscounts = Convert.ToDecimal(reader["TotalDiscounts"].ToString());
                    }
                    reader.Close();

                    command = new MySqlCommand();
                    command.Connection = connection;

                    // CALL CENTER
                    queryText = @"
                        select coalesce(SetupFeeRetail,0) as SetupFee, 
                            coalesce(PerPourFeeRetail,0) as PerHourFee,
                            coalesce(MonthlyFeeRetail,0) as MonthlyFee
                        from LeadPartnerSettings lps
                        join LeadPartner lp on lp.LeadPartnerID = lps.LeadPartnerID
                        where lp.DisplayName = 'Focus';";

                    command.CommandText = queryText;
                    command.Parameters.AddWithValue("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        row.CallCenterSetupFee = Convert.ToDecimal(reader["SetupFee"].ToString());
                        row.CallCenterPerHourFee = Convert.ToDecimal(reader["PerHourFee"].ToString());
                        row.CallCenterMonthlyFee = Convert.ToDecimal(reader["MonthlyFee"].ToString());
                    }
                    reader.Close();

                    using (MySqlConnection stoConnect = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]))
                    {
                        stoConnect.Open();

                        command = new MySqlCommand();
                        command.Connection = stoConnect;

                        // CALL CENTER
                        queryText = @"
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
                            row.CallCenterHours = Convert.ToDecimal(reader["Hours"].ToString());
                        }
                        reader.Close();
                        stoConnect.Close();
                    }

                    TimeSpan span = DateFilter1.Date2WithTime.Subtract(DateFilter1.Date1WithTime);
                    int months = span.Days / 30;
                    row.TotalCallCenterMonthlyFees = months * row.CallCenterMonthlyFee;
                    row.TotalCallCenterHourFees = row.CallCenterHours * row.CallCenterPerHourFee;

                    // fix Totals by TPClient flags
                    if (tpclient.TriangleFulfillment == false)
                    {
                        row.TotalShipmentFees =0;
                        row.TotalShipmentSKUFees = 0;
                        row.SetupFee = 0;
                        row.TotalReturnsFees = 0;
                        row.TotalKittingFees = 0;
                    }
                    if (tpclient.TriangleCRM == false)
                    {
                        row.TotalTransactionFees = 0;
                        row.TotalGatewayFees = 0;
                        row.TotalChargebackFees = 0;
                        row.TotalChargebackRepresentationFees = 0;
                        row.TotalDiscounts = 0;
                    }
                    if (tpclient.CallCenterServices == false)
                    {
                        row.CallCenterSetupFee = 0;
                        row.TotalCallCenterHourFees = 0;
                        row.TotalCallCenterMonthlyFees = 0;
                    }

                    row.TotalRevenue = row.TotalShipmentFees + row.TotalShipmentSKUFees + row.SetupFee + row.TotalReturnsFees + row.TotalKittingFees +
                        row.TotalTransactionFees + row.TotalGatewayFees + row.TotalChargebackFees + row.TotalChargebackRepresentationFees + row.TotalDiscounts +
                        row.CallCenterSetupFee + row.TotalCallCenterHourFees + row.TotalCallCenterMonthlyFees;

                    rows.Add(row);

                    connection.Close();
                }

                rTPClients.DataSource = rows;
            }
        }

        public override string HeaderString
        {
            get { return "Sales Agent Management "; }
        }

        private List<TPClient> FetchTPClients()
        {
            List<TPClient> tpClients = new List<TPClient>();

            MySqlConnection connection = new MySqlConnection();
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = @" select *, 1.00 as Commission from TPClient";
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TPClient tpClient = new TPClient();
                    tpClient.ID = (int)reader["TPClientID"];
                    tpClient.Name = reader["name"] as string;
                    tpClient.GatewayFee = (decimal)reader["GatewayFee"];
                    tpClient.TriangleFulfillment = Convert.ToBoolean(reader["TriangleFulfillment"]);
                    tpClient.TriangleCRM = Convert.ToBoolean(reader["TriangleCRM"]);
                    tpClient.TelephonyServices = Convert.ToBoolean(reader["TelephonyServices"]);
                    tpClient.CallCenterServices = Convert.ToBoolean(reader["CallCenterServices"]);
                    tpClient.TechnologyServices = Convert.ToBoolean(reader["TechnologyServices"]);
                    tpClient.MediaServices = Convert.ToBoolean(reader["MediaServices"]);
                    tpClient.ConnectionString = reader["connectionstringDotNet"] as string;
                    tpClients.Add(tpClient);
                }
            }
            finally
            {
                if (connection != null)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
            return tpClients;
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            DataBind();
        }

        protected string GetAdminNameByID(object adminObjID)
        {
            if (adminObjID == null)
                return "Not selected";

            int? adminID = Convert.ToInt32(adminObjID);
            var admin = admins.Where(u => u.AdminID == adminID).SingleOrDefault();
            if (admin == null)
                return "Undefined";

            return admin.DisplayName;
        }

        private class TPClient
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public decimal GatewayFee { get; set; }
            public bool TriangleFulfillment { get; set; }
            public bool TriangleCRM { get; set; }
            public bool TelephonyServices { get; set; }
            public bool CallCenterServices { get; set; }
            public bool TechnologyServices { get; set; }
            public bool MediaServices { get; set; }

            public string ConnectionString { get; set; }
        }

        private class TableRow
        {
            public int TPClientID { get; set; }
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
            public long TransactionNumber { get; set; }
            public decimal TransactionFee { get; set; }
            public decimal TotalTransactionFees { get; set; }
            public decimal GatewayFee { get; set; }
            public decimal TotalGatewayFees { get; set; }
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
            public decimal TotalClientRevenue { get; set; }

            public string Tr { get { return FormatToString(); } }

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
                    <td>{9}</td>
                    <td>{10:C2}</td>
                    <td>{11:C2}</td>
                    <td>{12:C2}</td>
                    <td>{13:C2}</td>
                    <td>{14:C2}</td>
                    <td>{15:C2}</td>
                    <td>{16:C2}</td>
                    <td>{17:C2}</td>
                    <td>{18:P2}</td>
                    <td>{19:C2}</td>
                    <td>{20:C2}</td>
                    <td>{21:C2}</td>
                    <td>{22:C2}</td>
                    <td>{23:C2}</td>
                    <td>{24:C2}</td>
                    <td>{25:C2}</td>
                    <td>{26:C2}</td>",
                    ShipmentFee, TotalShipmentFees, ShipmentSKUFee, TotalShipmentSKUFees, SetupFee,
                    ReturnsFee, TotalReturnsFees, KittingFee, TotalKittingFees, TransactionNumber, TransactionFee, TotalTransactionFees,
                    GatewayFee, TotalGatewayFees, ChargebackFee, TotalChargebackFees, ChargebackRepresentationFee, TotalChargebackRepresentationFees,
                    DiscountRate, TotalDiscounts, CallCenterSetupFee, CallCenterPerHourFee, TotalCallCenterHourFees,
                    CallCenterMonthlyFee, TotalCallCenterMonthlyFees, TotalRevenue, TotalClientRevenue);
            }
        }
    }
}