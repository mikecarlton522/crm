using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using TrimFuel.Web.Admin.Logic;

public partial class order_report : PageX
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static List<TableRow> Search(string search, bool limit)
    {
        List<TableRow> rows = new List<TableRow>();

        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        string[] vars = search.Split(' ');

        StringBuilder query = new StringBuilder();

        query.AppendLine("select b.BillingID BID, c.DisplayName Campaign, Coalesce(fs.FraudScore, 0) FraudScore, st.DisplayName Status, b.CreateDT OrderDate, b.FirstName, b.LastName, b.Address1 Address, b.City, b.State, b.Zip, b.Phone, b.Email, b.Affiliate, b.SubAffiliate from Billing b");
        query.AppendLine("inner join BillingSubscription bs on bs.BillingID = b.BillingID");
        query.AppendLine("inner join StatusType as st on bs.StatusTID = st.StatusTypeID");
        query.AppendLine("left join FraudScore fs on fs.BillingID = b.BillingID");
        query.AppendLine("left join Campaign as c on c.CampaignID = b.CampaignID ");
        query.AppendLine("where");

        for(int i = 0; i < vars.Length; i++)
        {
            query.AppendFormat("(FirstName like '%{0}%' or", vars[i]);
            query.AppendFormat(" LastName like '%{0}%' or", vars[i]);
            query.AppendFormat(" Address1 like '%{0}%' or", vars[i]);
            query.AppendFormat(" City like '%{0}%' or", vars[i]);
            query.AppendFormat(" State like '%{0}%' or", vars[i]);
            query.AppendFormat(" Zip like '%{0}%' or ", vars[i]);
            query.AppendFormat(" Phone like '%{0}%' or", vars[i]);
            query.AppendFormat(" Email like '%{0}%' or", vars[i]);
            query.AppendFormat(" Affiliate like '%{0}%' or", vars[i]);
            query.AppendFormat(" SubAffiliate like '%{0}%')", vars[i]);

            if (i < vars.Length - 1)
                query.AppendLine(" and");
        }

        if(limit)
            query.AppendLine("limit 50");

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = query.ToString();

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                TableRow row = new TableRow();
                row.BID = reader["BID"].ToString();
                row.Campaign = reader["Campaign"].ToString();
                row.FraudScore = reader["FraudScore"].ToString();
                row.Status = reader["Status"].ToString();
                row.OrderDate = reader["OrderDate"].ToString();
                row.FirstName = reader["FirstName"].ToString();
                row.LastName = reader["LastName"].ToString();
                row.Address = reader["Address"].ToString();
                row.City = reader["City"].ToString();
                row.State = reader["State"].ToString();
                row.Zip = reader["Zip"].ToString();
                row.Phone = reader["Phone"].ToString();
                row.Email = reader["Email"].ToString();
                row.Affiliate = reader["Affiliate"].ToString();
                row.SubID = reader["SubAffiliate"].ToString();
                rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)

                command.Dispose();

        }

        return rows;
    }

    public override string HeaderString
    {
        get { return "Magic Search"; }
    }

    public class TableRow
    {
        public string BID { get; set; }
        public string Campaign { get; set; }
        public string FraudScore { get; set; }
        public string Status { get; set; }
        public string OrderDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Affiliate { get; set; }
        public string SubID { get; set; }
    }
}