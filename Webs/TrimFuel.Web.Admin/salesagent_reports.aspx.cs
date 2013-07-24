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
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;

public partial class salesagent_reports : PageX
{
    private SalesAgentService salesAgentService = new SalesAgentService();
    private DashboardService dashboardService = new DashboardService();

    private List<TPClient> _tpClients = new List<TPClient>();

    private SalesAgent _salesAgent;

    protected void Page_Load(object sender, EventArgs e)
    {
		//try to get by admin
		string admName4Net = (Request.Cookies["admName4Net"] != null ? Request.Cookies["admName4Net"].Value : null);
		if (!string.IsNullOrEmpty(admName4Net))
		{
			Admin admin = dashboardService.GetAdminByName(admName4Net);
			if (admin != null)
			{
				MySqlCommand q = new MySqlCommand(
					" select sa.* from SalesAgent sa" +
					" where sa.AdminID = @adminID");
				q.Parameters.Add("@adminID", MySqlDbType.Int32).Value = admin.AdminID;
				IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
				_salesAgent = dao.Load<SalesAgent>(q).FirstOrDefault();

			}
		}
		if (_salesAgent == null)
		{
			int? id = Utility.TryGetInt(Request["id"]);
			if (id != null)
			{
				_salesAgent = salesAgentService.Load<SalesAgent>(id);
			}
		}

		if (_salesAgent == null)
		{
			Response.Redirect("/login.asp");
		}
		
        if (!IsPostBack)
        {
            DateFilter1.Date1 = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            DateFilter1.Date2 = DateTime.Today;

            //Response.Write(Session["username"] + "...");

        }
    }

    protected override void OnDataBinding(EventArgs e)
    {
        base.OnDataBinding(e);

        FetchTPClients();

        if (_tpClients.Count > 0)
        {
            List<TableRow> rows = new List<TableRow>();

            foreach (TPClient tpclient in _tpClients)
            {
                if (!string.IsNullOrEmpty(tpclient.ConnectionString))
                {
                    TableRow row = new TableRow();
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
                        row.TotalShipmentFees = Convert.ToInt32(command.ExecuteScalar().ToString()) * (decimal)_salesAgent.ShipmentFee;

                        command.Dispose();
                        connection.Close();
                    }
                    catch
                    {
                        //do nothing
                    }

                    row.Client = tpclient.Name;
                    row.TransactionFee = (decimal)_salesAgent.TransactionFeeFixed; //instead of tpclient.GatewayFee
                    row.TotalTransactionFees = (decimal)_salesAgent.TransactionFeeFixed * row.NumberOfTransactions;
                    row.ShipmentFee = (decimal)_salesAgent.ShipmentFee; //instead of tpclient.ProcessorFee
                    row.CRMFeePerMonth = (decimal)_salesAgent.MonthlyCRMFee; //instead of tpclient.CRMFee
                    row.TotalCRMFees = 0; //TODO
                    row.ChargebackFee = (decimal)_salesAgent.ChargebackFee; //instead of tpclient.ChargebackFee
                    row.TotalChargebackFee = (decimal)_salesAgent.ChargebackFee * row.NumberOfChargebacks;
                    row.CallcenterFeePerCall = (decimal)_salesAgent.CallCenterFeePerCall; 
                    row.CallcenterFeePerMinute = (decimal)_salesAgent.CallCenterFeePerMinute;
                    row.TotalCallcenterFees = 0; //TODO
                    row.TotalRevenue = 0; //TODO
                    rows.Add(row);

                }
            }
            rTPClients.DataSource = rows;

            rTPClients.DataBind();
        }
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
            command.CommandText = @"
                select c.* from SalesAgent sa
                left join SalesAgentTPClient as sac on sac.SalesAgentID = sa.SalesAgentID
                left join TPClient as c on c.TPClientID = sac.TPClientID";

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                TPClient tpClient = new TPClient();
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

    public override string HeaderString
    {
        get { return "Sales Agent Report"; }
    }

    private class TPClient
    {
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

    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
        DataBind();
    }
}