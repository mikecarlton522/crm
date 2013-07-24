using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class SalesAgent_ : System.Web.UI.Page
    {
        private SalesAgentService salesAgentService = new SalesAgentService();

        private DataSet _tpAdmins;

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                SalesAgent = salesAgentService.Load<SalesAgent>(id);
            }
            else
            {
                SalesAgent = new SalesAgent();
            }
            
            // default Comission values
            if (SalesAgent.CommissionMerchant == null)
                SalesAgent.CommissionMerchant = 50;
            if (SalesAgent.Commission == null)
                SalesAgent.Commission = 15;

            _tpAdmins = FetchAdmins();

            rAdmins.DataSource = _tpAdmins;
            rAdmins.DataBind();
            
            if (!string.IsNullOrEmpty(Request["action"]))
            {
                Save();
            }
        }

        public SalesAgent SalesAgent { get; set; }

        private void Save()
        {
            if (SalesAgent.SalesAgentID != null)
            {
                BusinessError<SalesAgent> updated = salesAgentService.UpdateSalesAgent(SalesAgent.SalesAgentID, Utility.TryGetStr(Request["name"]), Utility.TryGetStr(Request["apimode"]), Utility.TryGetStr(Request["apiUsername"]),
                    Utility.TryGetStr(Request["apiPassword"]), Utility.TryGetDecimal(Request["transactionFeeFixed"]), Utility.TryGetInt(Request["transactionFeePercentage"]), Utility.TryGetDecimal(Request["shipmentFee"]), Utility.TryGetDecimal(Request["extraSKUShipmentFee"]), Utility.TryGetDecimal(Request["chargebackFee"]), Utility.TryGetDecimal(Request["callCenterFeePerMinute"]),
                    Utility.TryGetDecimal(Request["callCenterFeePerCall"]), Utility.TryGetDecimal(Request["monthlyCRMFee"]), Utility.TryGetInt(Request["adminID"]), Utility.TryGetInt(Request["commission"]), Utility.TryGetInt(Request["commissionMerchant"]));
                if (updated.State == BusinessErrorState.Success)
                {
                    SalesAgent = updated.ReturnValue;
                }
                else
                {
                    //Show error
                    //updated.ErrorMessage
                }
            }
            else
            {
                BusinessError<SalesAgent> created = salesAgentService.CreateSalesAgent(Utility.TryGetStr(Request["name"]), Utility.TryGetStr(Request["apimode"]), Utility.TryGetStr(Request["apiUsername"]),
                    Utility.TryGetStr(Request["apiPassword"]), Utility.TryGetDecimal(Request["transactionFeeFixed"]), Utility.TryGetInt(Request["transactionFeePercentage"]), Utility.TryGetDecimal(Request["shipmentFee"]), Utility.TryGetDecimal(Request["extraSKUShipmentFee"]), Utility.TryGetDecimal(Request["chargebackFee"]), Utility.TryGetDecimal(Request["callCenterFeePerMinute"]),
                    Utility.TryGetDecimal(Request["callCenterFeePerCall"]), Utility.TryGetDecimal(Request["monthlyCRMFee"]), Utility.TryGetInt(Request["adminID"]), Utility.TryGetInt(Request["commission"]), Utility.TryGetInt(Request["commissionMerchant"]));
                if (created.State == BusinessErrorState.Success)
                {
                    SalesAgent = created.ReturnValue;
                }
                else
                {
                    //Show error
                    //created.ErrorMessage
                }
            }
        }

        private DataSet FetchAdmins()
        {
            DataSet ds = new DataSet();

            MySqlConnection connection;
            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);

                connection.Open();

                MySqlDataAdapter da = new MySqlDataAdapter("select AdminID, DisplayName from Admin where RestrictLevel = 8", connection);

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
}
