<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web.Services" %>
<%@ Import Namespace="MySql.Data.MySqlClient" %>
<%@ Import Namespace="TrimFuel.Business" %>

<script runat="server">

    [WebMethod]
    public static List<JSON> Search(string search, bool limit)
    {
        List<JSON> records = new List<JSON>();

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

        for (int i = 0; i < vars.Length; i++)
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

        if (limit)
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
                JSON record = new JSON();
                record.BID = reader["BID"].ToString();
                record.Campaign = reader["Campaign"].ToString();
                record.FraudScore = reader["FraudScore"].ToString();
                record.Status = reader["Status"].ToString();
                record.OrderDate = reader["OrderDate"].ToString();
                record.FirstName = reader["FirstName"].ToString();
                record.LastName = reader["LastName"].ToString();
                record.Address = reader["Address"].ToString();
                record.City = reader["City"].ToString();
                record.State = reader["State"].ToString();
                record.Zip = reader["Zip"].ToString();
                record.Phone = reader["Phone"].ToString();
                record.Email = reader["Email"].ToString();
                record.Affiliate = reader["Affiliate"].ToString();
                record.SubID = reader["SubAffiliate"].ToString();
                records.Add(record);
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

        return records;
    }

    public class JSON
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

</script>