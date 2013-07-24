using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ProductLeadManager : BaseControlPage
    {
        private TPClientService service = new TPClientService();

        protected int? TPClientID
        {
            get
            {
                return (Utility.TryGetInt(hdnTPClientID.Value)) ?? Utility.TryGetInt(Request["ClientId"]);
            }
        }

        private int? productID = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            productID = Utility.TryGetInt(Request["productID"]);
            if (!IsPostBack)
            {
                DataBind();
                hdnSelectedTab.Value = "0";
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            LeadPartnerList = service.GetPartnerList(TPClientID.Value);
            LeadRoutingRules = service.GetRoutingRules(TPClientID.Value, productID.Value);
        }

        protected void bSaveAbandons_Click(object sender, EventArgs e)
        {
            string res = SaveLeads(TrimFuel.Model.Enums.LeadTypeEnum.Abandons);
            if (!string.IsNullOrEmpty(res))
            {
                lSaved1.Text = res;
            }
            else
            {
                lSaved1.Text = "Rules were successfully saved";
            }
            DataBind();
        }

        protected void bSaveConfirms_Click(object sender, EventArgs e)
        {
            string res = SaveLeads(TrimFuel.Model.Enums.LeadTypeEnum.OrderConfirmations);
            if (!string.IsNullOrEmpty(res))
            {
                lSaved2.Text = res;
            }
            else
            {
                lSaved2.Text = "Rules were successfully saved";
            }
            DataBind();
        }

        protected void bSaveDeclines_Click(object sender, EventArgs e)
        {
            string res = SaveLeads(TrimFuel.Model.Enums.LeadTypeEnum.CancellationsDeclines);
            if (!string.IsNullOrEmpty(res))
            {
                lSaved3.Text = res;
            }
            else
            {
                lSaved3.Text = "Rules were successfully saved";
            }
            DataBind();
        }

        private string SaveLeads(int leadTypeID)
        {
            IDictionary<int, int> rules = new Dictionary<int, int>();
            string[] strLeadPartnerID = Request["leadPartnerID-" + leadTypeID.ToString()].Split(',');
            string[] strPercentage = Request["percentage-" + leadTypeID.ToString()].Split(',');
            if (strLeadPartnerID.Length != strPercentage.Length)
            {
                return "Invalid Input";
            }
            for (int i = 1; i < strLeadPartnerID.Length; i++)
            {
                if (Utility.TryGetInt(strLeadPartnerID[i]) == null ||
                    Utility.TryGetInt(strPercentage[i]) == null ||
                    Utility.TryGetInt(strPercentage[i]) <= 0 ||
                    Utility.TryGetInt(strPercentage[i]) > 100)
                {
                    return "Invalid Input";
                }
                rules.Add(Utility.TryGetInt(strLeadPartnerID[i]).Value, Utility.TryGetInt(strPercentage[i]).Value);
            }
            service.SetRoutingRules(TPClientID.Value, productID.Value, leadTypeID, rules);
            return null;
        }

        protected IList<LeadPartner> LeadPartnerList
        {
            get;
            set;
        }

        protected IList<LeadRouting> LeadRoutingRules
        {
            get;
            set;
        }

        protected string ShowOptions(object selectedValue)
        {
            string res = "<option value=''>-- Select --</option>";
            foreach (var item in LeadPartnerList)
            {
                res += "<option value='" + item.LeadPartnerID.ToString() + "' " + (selectedValue != null && Convert.ToInt32(selectedValue) == item.LeadPartnerID.Value ? " selected" : "") + ">" + item.DisplayName + "</option>";
            }
            return res;
        }
    }
}
