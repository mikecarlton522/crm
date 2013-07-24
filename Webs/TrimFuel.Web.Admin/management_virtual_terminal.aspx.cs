using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic.BillingAPI;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin
{
    public partial class management_virtual_terminal : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlInstallment.Items.AddRange(GetInstallment);

                DataBind();
            }
        }

        protected void btnCharge_Click(object sender, EventArgs e){
            
            int? midID = Utility.TryGetInt(ddlMID.SelectedValue);
            int? expMonth = Utility.TryGetInt(ddlExpMonth.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlExpYear.SelectedValue);
            decimal? amount = Utility.TryGetDecimal(tbAmount.Text);
            var currency = new Currency() { CurrencyName = ddlCurrency.SelectedItem.Text };
            
            if (midID == null)
            {
                lChargeResponse.Text = "Please, specify MID";
            }
            else if (expMonth == null)
            {
                lChargeResponse.Text = "Please, specify valid Expire Month";
            }
            else if (expYear == null)
            {
                lChargeResponse.Text = "Please, specify valid Expire Year";
            }
            else if (amount == null)
            {
                lChargeResponse.Text = "Please, specify valid Amount";
            }
            else if (string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            {
                lChargeResponse.Text = "Please, specify valid Currency";
            }
            else
            {
                BillingService service = new BillingService();
                PaymentExVars.Installments = Convert.ToInt32(ddlInstallment.SelectedValue);
                BusinessError<GatewayResult> res = service.DirectGatewayCharge(amount.Value, currency, midID.Value,
                    tbFirstName.Text, tbLastName.Text, tbAddress1.Text, tbAddress2.Text, tbCity.Text, tbState.Text, tbZip.Text, tbCountry.Text, tbPhone.Text, tbEmail.Text, tbIP.Text,
                    tbCreditCard.Text, tbCVV.Text, expMonth.Value, expYear.Value);
                string response = res.ErrorMessage;
                if (res.ReturnValue != null)
                {
                    response = res.ReturnValue.Response;
                }
                lChargeResponse.Text = response;
            }
        }

        public override string HeaderString
        {
            get { return "Virtual Terminal"; }
        }

        public ListItem[] GetInstallment
        {
            get
            {
                return new[]
                           {
                               new ListItem("1", "1"),
                               new ListItem("2", "2"),
                               new ListItem("3", "3"),
                               new ListItem("4", "4"),
                               new ListItem("5", "5"),
                               new ListItem("6", "6")
                           };
            }

        }
    }
}
