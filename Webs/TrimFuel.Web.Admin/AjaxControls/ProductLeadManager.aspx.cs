using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductLeadManager : System.Web.UI.Page
    {
        private LeadService service = new LeadService();

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

            LeadPartnerList = service.GetPartnerList();
            LeadRoutingRules = service.GetRoutingRules(productID.Value);
            LeadTypeList = service.GetLeadTypeList();
            LeadConfigValues = service.GetLeadPartnerConfigValues(productID.Value);
        }


        protected void bSaveAll_Click(object sender, EventArgs e)
        {
            string res = SaveLeads(TrimFuel.Model.Enums.LeadTypeEnum.All);
            if (!string.IsNullOrEmpty(res))
            {
                lSaved0.Text = res;
            }
            else
            {
                lSaved0.Text = "Rules were successfully saved";
            }
            DataBind();
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

        protected void bSaveConfig_Click(object sender, EventArgs e)
        {
            string res = SaveLeadConfig();
            if (!string.IsNullOrEmpty(res))
            {
                lSaved4.Text = res;
            }
            else
            {
                lSaved4.Text = "Rules were successfully saved";
            }
            DataBind();
        }

        private string SaveLeads(int leadTypeID)
        {
            IDictionary<int, int> rules = new Dictionary<int, int>();
            string[] strLeadPartnerID = Request["leadPartnerID-" + leadTypeID.ToString()].Split(',');
            string[] strPercentage = Request["percentage-" + leadTypeID.ToString()].Split(',');

            if (leadTypeID == TrimFuel.Model.Enums.LeadTypeEnum.All)
            {
                strPercentage = new string[strLeadPartnerID.Length];

                for (int i = 0; i < strPercentage.Length; i++)
                {
                    strPercentage[i] = "100";
                }
            }

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
            service.SetRoutingRules(productID.Value, leadTypeID, rules);
            return null;
        }

        private string SaveLeadConfig()
        {
            List<LeadPartnerConfigValue> settings = new List<LeadPartnerConfigValue>();

            string[] strLeadPartnerID = Request["cfgLeadPartnerID"].Split(',');
            string[] strLeadTypeID = Request["cfgLeadTypeID"].Split(',');
            string[] strKeyID = Request["cfgLeadConfigKey"].Split(',');
            string[] strValueID = Request["cfgLeadConfigValue"].Split(',');

            if (strLeadPartnerID.Length != strLeadTypeID.Length || strLeadPartnerID.Length != strKeyID.Length || strLeadPartnerID.Length != strValueID.Length)
                return "Invalid Input";

            for (int i = 1; i < strLeadPartnerID.Length; i++)
            {
                if (Utility.TryGetInt(strLeadPartnerID[i]) == null ||
                    Utility.TryGetInt(strLeadTypeID[i]) == null ||
                    Utility.TryGetStr(strKeyID[i]) == null ||
                    Utility.TryGetStr(strValueID[i]) == null)
                {
                    return "Invalid Input";
                }
                
                settings.Add(new LeadPartnerConfigValue(){
                    LeadPartnerID = Utility.TryGetInt(strLeadPartnerID[i]).Value,
                    LeadTypeID = Utility.TryGetInt(strLeadTypeID[i]).Value,
                    Key = Utility.TryGetStr(strKeyID[i]),
                    Value = Utility.TryGetStr(strValueID[i]),
                    ProductID = productID.Value
                });
            }

            service.SetLeadConfigValues(productID.Value, settings);

            return null;
        }

        protected IList<LeadPartner> LeadPartnerList
        {
            get;
            set;
        }

        protected IList<LeadType> LeadTypeList
        {
            get;
            set;
        }

        protected IList<LeadRouting> LeadRoutingRules
        {
            get;
            set;
        }

        protected IList<LeadPartnerConfigValue> LeadConfigValues
        {
            get;
            set;
        }

        protected string ShowLeadPartnerOptions(object selectedValue)
        {
            string res = "<option value=''>-- Select --</option>";
            if (LeadPartnerList != null)
                foreach (var item in LeadPartnerList)
                {
                    res += "<option value='" + item.LeadPartnerID.ToString() + "' " + (selectedValue != null && Convert.ToInt32(selectedValue) == item.LeadPartnerID.Value ? " selected" : "") + ">" + item.DisplayName + "</option>";
                }
            return res;
        }

        protected string ShowLeadTypeOptions(object selectedValue)
        {
            string res = "<option value=''>-- Select --</option>";
            if (LeadTypeList != null)
            {
                foreach (var item in LeadTypeList)
                {
                    res += "<option value='" + item.LeadTypeID.ToString() + "' " + (selectedValue != null && Convert.ToInt32(selectedValue) == item.LeadTypeID.Value ? " selected" : "") + ">" + item.DisplayName + "</option>";
                }
            }
            return res;
        }

    }
}
