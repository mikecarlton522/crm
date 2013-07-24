using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class CampaignEmailTypeList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public int? ProductID 
        {
            get
            {
                return Utility.TryGetInt(Request["productID"]);
            }
        }

        public int? CampaignID
        {
            get
            {
                return Utility.TryGetInt(Request["campaignID"]);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (ProductID != null)
            {
                rEmailType.DataSource = (new EmailService()).GetDynamicEmailByCampaign(ProductID.Value, CampaignID.Value);
            }
        }

        protected void rEmailType_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item ||e.Item.ItemType == ListItemType.AlternatingItem) 
                && e.CommandArgument != null && Utility.TryGetInt(e.CommandArgument.ToString()) != null)
            {
                EmailService serv = new EmailService();
                if (e.CommandName == "Active")
                {
                    serv.SetDynamicEmailState(Utility.TryGetInt(e.CommandArgument.ToString()).Value, true);
                }
                else if (e.CommandName == "Inactive")
                {
                    serv.SetDynamicEmailState(Utility.TryGetInt(e.CommandArgument.ToString()).Value, false);
                }
                DataBind();
            }
        }
    }
}
