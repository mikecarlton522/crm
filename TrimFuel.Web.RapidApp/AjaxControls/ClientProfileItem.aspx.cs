using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientProfileItem : BaseControlPage
    {
        TrimFuel.Business.TPClientService ser = new TrimFuel.Business.TPClientService();
        DashboardService dashboardService = new DashboardService();

        public TPClientSetting TPClientProp
        {
            get
            {
                var settings = ser.GetClientSetting(TPClientID.Value);
                if (settings == null)
                {
                    settings = new TPClientSetting();
                    settings.TPClientID = TPClientID.Value;
                }
                return settings;
            }
        }

        public List<TPClientNoteView> TPClientNotesProp
        {
            get
            {
                return ser.GetClientNotes(TPClientID.Value).Take(3).ToList();
            }
        }

        public List<TPClientEmailView> TPClientEmailsProp
        {
            get
            {
                return ser.GetClientEmails(TPClientID.Value).Take(3).ToList();
            }
        }

        protected int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        private TPClient tpClient = null;
        protected TPClient TPClient
        {
            get
            {
                if (tpClient == null)
                {
                    if (TPClientID != -1)
                        tpClient = (new BaseService()).Load<TPClient>(TPClientID);
                    else
                        tpClient = new TPClient() { Name = "Triangle", TPClientID = -1 };
                }
                return tpClient;
            }
            set
            {
                tpClient = value;
            }
        }

        //int AdminID = 334; // test
        //int AdminID = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Admin admin = null;
            //string admName4Net = (Request.Cookies["admName4Net"] != null ? Request.Cookies["admName4Net"].Value : null);
            //if (!string.IsNullOrEmpty(admName4Net))
            //{
            //    admin = dashboardService.GetAdminByName(admName4Net);
            //}
            //if (admName4Net == null || admin == null)
            //    Response.Redirect("/login.asp");

            //if (admin != null)
            //    AdminID = admin.AdminID.Value;

            lSaved.Visible = false;
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected void bSaveSettings_Click(object sender, EventArgs e)
        {
            TPClientSetting settings = null;
            int tpClientID = Convert.ToInt32(tbTPClientID.Text);
            if (Convert.ToInt32(tbTPClientSettingID.Text) > 0 && tpClientID > 0)
            {
                //update
                settings = ser.GetClientSetting(tpClientID);
            }
            else
            {
                //insert
                settings = new TPClientSetting();
                settings.TPClientID = tpClientID;
                settings.LastLoginDate = null;
            }

            settings.ContactName = tbContactName.Text;
            settings.ContactPhone = tbContactPhone.Text;
            settings.CustomerServicePhoneNumber = tbServicePhone.Text;
            settings.DBAName = tbDBAName.Text;
            settings.LegalBusinessName = tbName.Text;
            settings.MarketingPageURL = tbURL.Text;
            settings.OpenSalesRegions = tbSalesRegion.Text;
            settings.Password = tbPassword.Text;
            settings.ProductOffered = tbProductOffered.Text;
            settings.SecondaryContactName = tbSecondaryName.Text;
            settings.Username = tbUserName.Text;
            settings.BillingModel = sBillingModel.SelectedIndex == 0 ? false : true;

            ser.SaveClientSetting(settings);

            TPClient = ser.SaveTPClientBillingAPICredentials(tpClientID, tbBillingAPIUsername.Text, tbBillingAPIPassword.Text);

            DataBind();
            lSaved.Visible = true;
        }

        protected void bSendEmail_Click(object sender, EventArgs e)
        {
            EmailService emailSer = new EmailService();
            emailSer.SendTPClientEmail(AdminMembership.CurrentAdmin.AdminID.Value, tbFrom.Text, tbTo.Text, "TPClientEmailSubject", txtMessage.Value, Convert.ToInt32(tbTPClientID.Text));
            DataBind();
        }

        protected void bSendNote_Click(object sender, EventArgs e)
        {
            TPClientNote note = new TPClientNote()
            {
                AdminID = AdminMembership.CurrentAdmin.AdminID.Value,
                Content = txtNote.Value,
                CreateDT = DateTime.Now,
                TPClientID = Convert.ToInt32(tbTPClientID.Text),
            };
            ser.SaveClientNote(note);
            DataBind();
        }
    }
}
