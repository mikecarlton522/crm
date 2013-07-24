using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientGateway : BaseControlPage
    {
        TPClientService tpSer = new TPClientService();

        protected void Page_Load(object sender, EventArgs e)
        {
            lSaved.Visible = false;
            lMIDsError.Visible = false;
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            rProducts.DataSource = tpSer.GetAllProducts(TPClientID.Value).Where(u => u.ProductIsActive.Value);
            rGateways.DataSource = tpSer.GetClientGatewayServices(TPClientID.Value);
        }

        protected string GetCurrency(int productID)
        {
            var item = tpSer.GetProductCurrency(TPClientID.Value, productID);
            return item == null ? "USD" : item.CurrencyName;
        }

        protected List<AssertigyMID> GetMerchantAccounts(int productID)
        {
            return tpSer.GetProductAssertigyMIDs(productID, TPClientID.Value).ToList();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
        }

        protected void rGateways_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var assertigyList = tpSer.GetAssertigyMIDList(Convert.ToInt32(e.CommandArgument), TPClientID.Value).Where(u => u.Visible == true);
                if (assertigyList.Count() > 0)
                    lMIDsError.Visible = true;
                else
                {
                    tpSer.DeleteGateway(TPClientID.Value, Convert.ToInt32(e.CommandArgument));
                    lSaved.Visible = true;
                }
                DataBind();
            }
        }
    }
}
