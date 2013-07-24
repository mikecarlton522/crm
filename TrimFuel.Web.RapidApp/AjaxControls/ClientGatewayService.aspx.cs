using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientGatewayService : BaseControlPage
    {
        public class AssertigyMIDDisplay
        {
            public AssertigyMID AssertigyMID { get; set; }
            public AssertigyMIDSettings AssertigyMIDSetting { get; set; }
        }

        TPClientService tpSer = new TPClientService();
        bool IsNew
        {
            get
            {
                if (string.IsNullOrEmpty(Request.QueryString["new"]) || Request.QueryString["new"] != "new")
                    return false;
                return true;
            }
        }
        protected int? TPClientID
        {
            get
            {
                return (Utility.TryGetInt(hdnTPClientID.Value)) ?? Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected int? ServiceID
        {
            get
            {
                return (Utility.TryGetInt(hdnCompanyID.Value)) ?? Utility.TryGetInt(Request["companyID"]);
            }
        }

        protected string ServiceName
        {
            get
            {
                return string.IsNullOrEmpty(hdnServiceName.Value) ? Request["service"].ToString() : hdnServiceName.Value;
            }
        }

        public NMICompany NMIProp
        {
            get
            {
                if (ServiceID == null)
                    return new NMICompany()
                        {
                            Active = true
                        };
                return tpSer.GetClientGatewayService(ServiceID.Value, TPClientID.Value);
            }
        }

        public List<AssertigyMIDDisplay> AssertigyMIDList
        {
            get
            {
                List<AssertigyMIDDisplay> res = null;
                if (IsNew)
                {
                    res = new List<AssertigyMIDDisplay>();
                }
                else
                {
                    var settings = tpSer.GetAssertigyMIDSettingsList(ServiceID.Value, TPClientID.Value).ToList();
                    res = tpSer.GetAssertigyMIDList(ServiceID.Value, TPClientID.Value).Where(u => u.Deleted != true).Select(u => new AssertigyMIDDisplay()
                        {
                            AssertigyMID = u,
                            AssertigyMIDSetting = settings.Where(t => t.AssertigyMIDID == u.AssertigyMIDID).SingleOrDefault()
                        }).ToList();
                }
                return res;
            }
        }

        public string HeaderTitle
        {
            get
            {
                if (IsNew)
                    return "Add Gateway: " + ServiceName;
                else
                    return "Edit Gateway: " + ServiceName + " - " + NMIProp.CompanyName;
            }
        }

        protected List<string> GetPaymentTypes(int assertigyMIDID, int clientID)
        {
            var res = tpSer.GetAssertigyMIDPaymentTypes(assertigyMIDID, clientID).Select(u => u.DisplayName);
            return res.ToList();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lSaved.Visible = false;
            if (!IsPostBack)
            {
                if (TPClientID == null)
                    return;
                if (IsNew)
                    MID.Visible = false;
                DataBind();
            }
        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            NMICompany company = new NMICompany();

            company.Active = cbActive.Checked;
            company.CompanyName = tbCompanyName.Text;
            company.GatewayIntegrated = ServiceName;
            company.GatewayPassword = tbGatewayPassword.Text;
            company.GatewayUsername = tbGatewayUsername.Text;
            company.NMICompanyID = ServiceID;
            company.Deleted = false;
            tpSer.SaveGateway(company, TPClientID.Value);

            hdnCompanyID.Value = company.NMICompanyID.ToString();
            lSaved.Visible = true;
            MID.Visible = true;
            DataBind();
        }

        protected void rMID_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                tpSer.DeleteMID(TPClientID.Value, Convert.ToInt32(e.CommandArgument));
                lSaved.Visible = true;
                DataBind();
            }
        }
    }
}
