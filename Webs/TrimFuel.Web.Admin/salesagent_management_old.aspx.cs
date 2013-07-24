using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Model;

using MySql.Data.MySqlClient;
using TrimFuel.Business.Utils;

public partial class salesagent_management : PageX
{
    private SalesAgentService salesAgentService = new SalesAgentService();

    private List<TPClient> _tpClients = new List<TPClient>();
    private IList<Admin> _admins = null;


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

        _admins = salesAgentService.GetAdminList();
        rSalesAgents.DataSource = salesAgentService.GetSalesAgentList();

        FetchTPClients();

        if (_tpClients.Count > 0)
        {
            List<TableRow> rows = new List<TableRow>();

            foreach (TPClient tpclient in _tpClients)
            {
                if (!string.IsNullOrEmpty(tpclient.ConnectionString))
                {
                    TableRow row = new TableRow();
                    row.Client = tpclient.Name;
                    
                    SalesAgent salesAgent = FetchSalesAgent(tpclient.ID);
                    if (salesAgent != null)
                    {

                        FetchTransactionData(tpclient, ref row);
                        row.TransactionFee = salesAgent.TransactionFeeFixed != null ? (decimal)salesAgent.TransactionFeeFixed : 0;
                        row.TotalTransactionFees = row.TransactionFee * row.NumberOfTransactions;

                        FetchShipmentData(tpclient, salesAgent, ref row);
                        row.ShipmentFee = salesAgent.ShipmentFee != null ? (decimal)salesAgent.ShipmentFee : 0;

                        FetchChargebackData(tpclient, ref row);
                        row.ChargebackFee = salesAgent.ChargebackFee != null ? (decimal)salesAgent.ChargebackFee : 0;
                        row.TotalChargebackFee = row.ChargebackFee * row.NumberOfChargebacks;

                        TimeSpan span = DateFilter1.Date2WithTime.Subtract(DateFilter1.Date1WithTime);
                        int months = span.Days / 30;
                        row.CRMFeePerMonth = salesAgent.MonthlyCRMFee != null ? (decimal)salesAgent.MonthlyCRMFee : 0;
                        row.TotalCRMFees = months * row.CRMFeePerMonth;

                        FetchCallCenterData(tpclient, ref row);
                        row.CallcenterFeePerCall = salesAgent.CallCenterFeePerCall != null ? (decimal)salesAgent.CallCenterFeePerCall : 0;
                        row.CallcenterFeePerMinute = salesAgent.CallCenterFeePerMinute != null ? (decimal)salesAgent.CallCenterFeePerMinute : 0;
                        row.TotalCallcenterFees = row.CallcenterFeePerMinute * row.Minutes + row.CallcenterFeePerCall * row.Calls;
                        row.TotalRevenue = 0; //TODO
                    }
                    rows.Add(row);
                }
            }
            rTPClients.DataSource = rows;

            rTPClients.DataBind();
        }
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
            command.CommandText = "select tpc.* from TPClient tpc";

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

    private SalesAgent FetchSalesAgent(int tpclientID)
    {
        MySqlConnection connection;
        MySqlCommand command;
        SalesAgent salesAgent;

        try
        {
            connection = new MySqlConnection();
            command = new MySqlCommand();

            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;

            string query = @"
                select sa.Name from TPClient tpc
                left join SalesAgentTPClient as satpc on satpc.TPClientID = tpc.TPClientID
                left join SalesAgent as sa on sa.SalesAgentID = satpc.SalesAgentID
                where tpc.TPClientID = @clientID";

            command.CommandText = query;
            command.Parameters.Add("@clientID", tpclientID);

            string salesAgentName = (string)command.ExecuteScalar();

            salesAgent = salesAgentService.GetByName(salesAgentName);

            command.Dispose();
            connection.Close();

        }
        catch
        {
            salesAgent = null;
        }

        return salesAgent;
    }

    private void FetchTransactionData(TPClient tpclient, ref TableRow row)
    {
        MySqlConnection connection;
        MySqlCommand command;
        MySqlDataReader reader;

        try
        {
            connection = new MySqlConnection();
            command = new MySqlCommand();

            connection.ConnectionString = (tpclient.ConnectionString);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;

            string query1 = @"
                            select sum(t.TransactionCount) as TransactionCount, sum(TransactionAmount) as TransactionAmount from
                            (
                              select count(*) as TransactionCount, sum(case when ch.Success = 1 then ch.Amount else 0 end) as TransactionAmount from ChargeHistoryEx ch
                              where ch.ChargeDate between @start and @end
                              union all
                              select count(*) as TransactionCount, 0 as TransactionAmount from FailedChargeHistory fch
                              where fch.ChargeDate between @start and @end
                            ) t;";


            command.CommandText = query1;
            command.Parameters.Add("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.Add("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                row.NumberOfTransactions = Convert.ToInt32(reader["TransactionCount"].ToString());
                row.CustomerTotalRevenue = (decimal)(reader["TransactionAmount"]);
            }

            command.Dispose();
            connection.Close();
        }
        catch
        {
            //do nothing
        }
    }

    private void FetchChargebackData(TPClient tpclient, ref TableRow row)
    {
        MySqlConnection connection;
        MySqlCommand command;

        try
        {
            connection = new MySqlConnection();
            command = new MySqlCommand();

            connection.ConnectionString = (tpclient.ConnectionString);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;

            string query2 = @"                        
                            select sum(t.ChargebackCount) as ChargebackCount from
                            (
	                            select count(*) as ChargebackCount from SaleChargeback chb
	                            where chb.PostDT between @start and @end
	                            union all
	                            select count(*) as ChargebackCount from BillingChargeback chb
	                            where chb.ChargebackDT between @start and @end
                            ) t;";

            command.CommandText = query2;
            command.Parameters.Add("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.Add("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            row.NumberOfChargebacks = Convert.ToInt32(command.ExecuteScalar().ToString());

            command.Dispose();
            connection.Close();
        }
        catch
        {
            //do nothing
        }
    }

    private void FetchShipmentData(TPClient tpclient, SalesAgent agent, ref TableRow row)
    {
        MySqlConnection connection;
        MySqlCommand command;

        try
        {
            connection = new MySqlConnection();
            command = new MySqlCommand();

            connection.ConnectionString = (tpclient.ConnectionString);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;

            string query3 = @"select sum(Shipments) Shipments from 
                        (
                            select count(*) Shipments from ABFRecord
                            where Completed = 1 and CreateDT between @start and @end
                            union all
                            select count(*) Shipments from TSNRecord
                            where Completed = 1 and CreateDT between @start and @end
                            union all
                            select count(*) Shipments from KeymailRecord
                            where Completed = 1 and CreateDT between @start and @end
                        ) as t;";

            command.CommandText = query3;
            command.Parameters.Add("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.Add("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            string a = command.ExecuteScalar().ToString();
            row.TotalShipmentFees = Convert.ToInt32(command.ExecuteScalar().ToString()) * (decimal)agent.ShipmentFee;

            command.Dispose();
            connection.Close();
        }
        catch
        {
            //do nothing
        }
    }

    private void FetchCallCenterData(TPClient tpclient, ref TableRow row)
    {
        MySqlConnection connection;
        MySqlCommand command;
        MySqlDataReader reader;

        try
        {
            connection = new MySqlConnection();
            command = new MySqlCommand();

            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;

            string query4 = @"
                select COALESCE(SUM(TIME_TO_SEC(TIMEDIFF(c.EndDT, c.StartDT) / 60)), 0) as Minutes, COUNT(c.EndDT) as Calls from TPClient as tpc
                left join TPClientFocusCustomerProduct as tpccp on tpccp.TPClientID = tpc.TPClientID
                left join `Call` as c on c.CustomerProduct = tpccp.CustomerProduct
                where tpc.Name = @client and EndDT between @start and @end
                group by tpc.Name";                

            command.CommandText = query4;
            command.Parameters.Add("@start", DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.Add("@end", DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.Add("@client", tpclient.Name);

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                row.Minutes = Convert.ToInt32(reader["Minutes"]);
                row.Calls= Convert.ToInt32(reader["Calls"]);
            }

            command.Dispose();
            connection.Close();
        }
        catch
        {
            //do nothing
        }
    }

    private class TPClient
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public decimal GatewayFee { get; set; }
        public decimal ProcessorFee { get; set; }
        public decimal CRMFee { get; set; }
        public decimal ChargebackFee { get; set; }
    }

    private class TableRow
    {
        public string Client { get; set; }
        public int NumberOfTransactions { get; set; }
        public decimal TransactionFee { get; set; }
        public decimal TotalTransactionFees { get; set; }
        public decimal ShipmentFee { get; set; }
        public decimal TotalShipmentFees { get; set; }
        public decimal CRMFeePerMonth { get; set; }
        public decimal TotalCRMFees { get; set; }
        public int NumberOfChargebacks { get; set; }
        public decimal ChargebackFee { get; set; }
        public decimal TotalChargebackFee { get; set; }
        public decimal CallcenterFeePerCall { get; set; }
        public decimal CallcenterFeePerMinute { get; set; }
        public decimal TotalCallcenterFees { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CustomerTotalRevenue { get; set; }
        public int Minutes { get; set; }
        public int Calls { get; set; }
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

    public override string HeaderString
    {
        get { return "Sales Agent Management"; }
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
        var admin = _admins.Where(u => u.AdminID == adminID).SingleOrDefault();
        if (admin == null)
            return "Undefined";

        return admin.DisplayName;
    }
}