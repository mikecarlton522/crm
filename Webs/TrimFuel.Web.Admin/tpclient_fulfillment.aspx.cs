using System;
using System.Collections.Generic;

using log4net;

using MySql.Data.MySqlClient;

using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public partial class tpclient_fulfillment : PageX
    {
        private TPClientService clientService = new TPClientService();
        private IList<PostageDetailsView> postageList = new List<PostageDetailsView>();
        private DateTime start;
        private DateTime end;


        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));

        protected void Page_Load(object sender, EventArgs e)
        {
            start = dateFilter.Date1WithTime;
            end = dateFilter.Date2WithTime;
        }

        public override string HeaderString
        {
            get { return "Triangle Fulfillment Report"; }
        }

        protected IList<PostageDetailsView> PostageDetails { get { return postageList; } }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            start = dateFilter.Date1WithTime;
            end = dateFilter.Date2WithTime;

            this.DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<TPClient> clientList = clientService.GetClientList();

            postageList.Add(GetPostageDetailsView(Config.Current.CONNECTION_STRINGS["TrimFuel"], "Triangle", 0));

            foreach (TPClient client in clientList)
            {
                decimal postageAllowed = client.PostageAllowed.HasValue ? client.PostageAllowed.Value : 0;

                PostageDetailsView pdv = GetPostageDetailsView(client.ConnectionString, client.Name, postageAllowed);

                postageList.Add(pdv);
            }

            repPostage.DataSource = postageList;
            repPostage.DataBind();
        }

        private PostageDetailsView GetPostageDetailsView(string connectionString, string clientName, decimal postageAllowed)
        {
            PostageDetailsView res = new PostageDetailsView() { ClientName = clientName, PostageAllowed = postageAllowed };

            MySqlConnection connection = new MySqlConnection();
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.ConnectionString = (connectionString);
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "select count(*) as ShipmentCount, sum(Costs) as PostageUsed from SalePostageDetails where CreateDT between @start and @end;";
                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    res.PostageUsed = Convert.ToDecimal(reader["PostageUsed"]);
                    res.ShipmentCount = Convert.ToInt32(reader["ShipmentCount"]);
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

            return res;
        }        
    }

    public class PostageDetailsView
    {
        public string ClientName { get; set; }
        public decimal PostageUsed { get; set; }
        public decimal PostageAllowed { get; set; }
        public int ShipmentCount { get; set; }
    }
}