using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MySql.Data.MySqlClient;


using TrimFuel.Model;
using TrimFuel.Business;

public partial class EditForms_ClientList : Page
{
    private SalesAgentService salesAgentService = new SalesAgentService();

    private DataSet _tpClients;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            _tpClients = FetchTPClients();

            rClients.DataSource = _tpClients;
            rClients.DataBind();

            int id = 0;
            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                SalesAgent = salesAgentService.Load<SalesAgent>(id);

                IList<TPClient> clients = salesAgentService.GetTPClientBySalesAgent(id);

                StringBuilder html = new StringBuilder();

                foreach (TPClient client in clients)
                {
                    html.AppendLine("<div>");
                    html.AppendLine("<select name=\"clientId\">");
                    foreach (DataRow row in _tpClients.Tables[0].Rows)
                    {
                        html.AppendFormat("<option value=\"{0}\"{1} >{2}</option>", row["TPClientID"], row["TPClientID"].ToString().Equals(client.TPClientID.ToString()) ? " selected=\"selected\"" : "", row["Name"]);
                    }
                    html.AppendLine("</select>");
                    html.AppendLine("<a href=\"#\" onclick=\"return removeClient(this);\" class=\"removeIcon\" style=\"float: right;\">Remove</a>");
                    html.AppendLine("</div>");
                }
                ltClients.Text = html.ToString();
            }
        }


        if (!string.IsNullOrEmpty(Request["action"]))
        {
            Save();
        }
    }

    public SalesAgent SalesAgent { get; set; }

    private void Save()
    {
        salesAgentService.RemoveAllSalesAgentTPClients((int)SalesAgent.SalesAgentID);

        string[] clientIds = Request["clientId"].Split(',');

        foreach (string clientIdStr in clientIds)
        {
            int clientId = int.TryParse(clientIdStr, out clientId) ? clientId : -1;

            if (clientId != -1)
                salesAgentService.CreateSalesAgentTpClient((int)SalesAgent.SalesAgentID, clientId);
        }        
    }

    private DataSet FetchTPClients()
    {
        DataSet ds = new DataSet();

        MySqlConnection connection;
        try
        {
            connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);

            connection.Open();

            MySqlDataAdapter da = new MySqlDataAdapter("select tpc.TPClientID, tpc.Name from TPClient tpc", connection);
            
            da.Fill(ds);            

            connection.Close();
        }
        catch
        {
            //do nothing
        }

        return ds;
    }
}