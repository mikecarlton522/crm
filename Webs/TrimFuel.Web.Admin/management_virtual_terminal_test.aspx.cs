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
    public partial class management_virtual_terminal_test : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected void btnSale_Click(object sender, EventArgs e)
        {
            int? midID = Utility.TryGetInt(ddlMID.SelectedValue);
            int? expMonth = Utility.TryGetInt(ddlAuth_expDate.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlAuth_expYear.SelectedValue);
            decimal? amount = Utility.TryGetDecimal(tbAuth_Amount.Text);
            var currency = new Currency() { CurrencyName = tbCurrency.SelectedItem.Text };
            if (midID == null)
            {
                litAuth_Response.Text = "Please, specify MID";
            }
            else if (expMonth == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Month";
            }
            else if (expYear == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Year";
            }
            else if (amount == null)
            {
                litAuth_Response.Text = "Please, specify valid Amount";
            }
            else
            {
                try
                {
                    AssertigyMID assertigyMID = new BillingService().Load<AssertigyMID>(midID.Value);
                    NMICompany nmiCompany = new MerchantService().GetNMICompanyByAssertigyMID(midID.Value);

                    if (nmiCompany == null)
                    {
                        throw new Exception("MID is not configured.");
                    }

                    IPaymentGateway paymentGateway = new SaleService().GetGatewayByMID(assertigyMID);



                    Product product = new Product();
                    product.Code = "";
                    product.ProductName = "General Products";

                    var result = paymentGateway.Sale(assertigyMID.MID,
                        nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, Convert.ToDecimal(tbAuth_Amount.Text), currency, 0,
                        createBillingObject(), product);

                    litAuth_Request.Text = Server.HtmlEncode(result.ReturnValue.Request);
                    litAuth_Response.Text = result.ReturnValue.Response;

                    Session["chargeHistoryEx"] = new ChargeHistoryEx() { Response = result.ReturnValue.Response, TransactionNumber = paymentGateway.GetTransactionID(result.ReturnValue) };
                }
                catch (Exception ex)
                {
                    litAuth_Response.Text = ex.ToString();
                }
            }
        }

        protected void btnAuth_Click(object sender, EventArgs e)
        {
            int? midID = Utility.TryGetInt(ddlMID.SelectedValue);
            int? expMonth = Utility.TryGetInt(ddlAuth_expDate.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlAuth_expYear.SelectedValue);
            decimal? amount = Utility.TryGetDecimal(tbAuth_Amount.Text);
            if (midID == null)
            {
                litAuth_Response.Text = "Please, specify MID";
            }
            else if (expMonth == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Month";
            }
            else if (expYear == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Year";
            }
            else if (amount == null)
            {
                litAuth_Response.Text = "Please, specify valid Amount";
            }
            else
            {
                try
                {
                    AssertigyMID assertigyMID = new BillingService().Load<AssertigyMID>(midID.Value);
                    NMICompany nmiCompany = new MerchantService().GetNMICompanyByAssertigyMID(midID.Value);

                    if (nmiCompany == null)
                    {
                        throw new Exception("MID is not configured.");
                    }

                    IPaymentGateway paymentGateway = new SaleService().GetGatewayByMID(assertigyMID);



                    Product product = new Product();
                    product.Code = "";
                    product.ProductName = "General Products";

                    var result = paymentGateway.AuthOnly(assertigyMID.MID,
                        nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, Convert.ToDecimal(tbAuth_Amount.Text), null,
                        createBillingObject(), product);

                    litAuth_Response.Text = result.ReturnValue.Response;

                    Session["chargeHistoryEx"] = new ChargeHistoryEx() { Response = result.ReturnValue.Response, TransactionNumber = paymentGateway.GetTransactionID(result.ReturnValue) };
                }
                catch (Exception ex)
                {
                    litAuth_Response.Text = ex.ToString();
                }
            }
        }

        protected void btnCapture_Click(object sender, EventArgs e)
        {
            int? midID = Utility.TryGetInt(ddlMID.SelectedValue);
            int? expMonth = Utility.TryGetInt(ddlAuth_expDate.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlAuth_expYear.SelectedValue);
            decimal? amount = Utility.TryGetDecimal(tbAuth_Amount.Text);
            if (midID == null)
            {
                litAuth_Response.Text = "Please, specify MID";
            }
            else if (expMonth == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Month";
            }
            else if (expYear == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Year";
            }
            else if (amount == null)
            {
                litAuth_Response.Text = "Please, specify valid Amount";
            }
            else
            {
                try
                {
                    AssertigyMID assertigyMID = new BillingService().Load<AssertigyMID>(midID.Value);
                    NMICompany nmiCompany = new MerchantService().GetNMICompanyByAssertigyMID(midID.Value);

                    if (nmiCompany == null)
                    {
                        throw new Exception("MID is not configured.");
                    }

                    IPaymentGateway paymentGateway = new SaleService().GetGatewayByMID(assertigyMID);



                    Product product = new Product();
                    product.Code = "";
                    product.ProductName = "General Products";

                    var result = paymentGateway.Capture(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, createChargeHistoryExObject, Convert.ToDecimal(tbAuth_Amount.Text), null);

                    litAuth_Response.Text = result.ReturnValue.Response;

                    Session["chargeHistoryEx"] = new ChargeHistoryEx() { Response = result.ReturnValue.Response, TransactionNumber = paymentGateway.GetTransactionID(result.ReturnValue) };
                }
                catch (Exception ex)
                {
                    litAuth_Response.Text = ex.ToString();
                }
            }
        }

        protected void btnRefund_Click(object sender, EventArgs e)
        {
            int? midID = Utility.TryGetInt(ddlMID.SelectedValue);
            int? expMonth = Utility.TryGetInt(ddlAuth_expDate.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlAuth_expYear.SelectedValue);
            decimal? amount = Utility.TryGetDecimal(tbAuth_Amount.Text);
            if (midID == null)
            {
                litAuth_Response.Text = "Please, specify MID";
            }
            else if (expMonth == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Month";
            }
            else if (expYear == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Year";
            }
            else if (amount == null)
            {
                litAuth_Response.Text = "Please, specify valid Amount";
            }
            else
            {
                try
                {
                    AssertigyMID assertigyMID = new BillingService().Load<AssertigyMID>(midID.Value);
                    NMICompany nmiCompany = new MerchantService().GetNMICompanyByAssertigyMID(midID.Value);

                    if (nmiCompany == null)
                    {
                        throw new Exception("MID is not configured.");
                    }

                    IPaymentGateway paymentGateway = new SaleService().GetGatewayByMID(assertigyMID);



                    Product product = new Product();
                    product.Code = "";
                    product.ProductName = "General Products";

                    litAuth_Response.Text = paymentGateway.Refund(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, createChargeHistoryExObject, Convert.ToDecimal(tbAuth_Amount.Text), null).ReturnValue.Response;
                }
                catch (Exception ex)
                {
                    litAuth_Response.Text = ex.ToString();
                }
            }
        }

        protected void btnVoid_Click(object sender, EventArgs e)
        {
            int? midID = Utility.TryGetInt(ddlMID.SelectedValue);
            int? expMonth = Utility.TryGetInt(ddlAuth_expDate.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlAuth_expYear.SelectedValue);
            decimal? amount = Utility.TryGetDecimal(tbAuth_Amount.Text);
            if (midID == null)
            {
                litAuth_Response.Text = "Please, specify MID";
            }
            else if (expMonth == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Month";
            }
            else if (expYear == null)
            {
                litAuth_Response.Text = "Please, specify valid Expire Year";
            }
            else if (amount == null)
            {
                litAuth_Response.Text = "Please, specify valid Amount";
            }
            else
            {
                try
                {
                    AssertigyMID assertigyMID = new BillingService().Load<AssertigyMID>(midID.Value);
                    NMICompany nmiCompany = new MerchantService().GetNMICompanyByAssertigyMID(midID.Value);

                    if (nmiCompany == null)
                    {
                        throw new Exception("MID is not configured.");
                    }

                    IPaymentGateway paymentGateway = new SaleService().GetGatewayByMID(assertigyMID);



                    Product product = new Product();
                    product.Code = "";
                    product.ProductName = "General Products";

                    litAuth_Response.Text = paymentGateway.Void(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, createChargeHistoryExObject, Convert.ToDecimal(tbAuth_Amount.Text), null).ReturnValue.Response;
                }
                catch (Exception ex)
                {
                    litAuth_Response.Text = ex.ToString();
                }
            }
        }

        private Billing createBillingObject()
        {
            int? expMonth = Utility.TryGetInt(ddlAuth_expDate.SelectedValue);
            int? expYear = Utility.TryGetInt(ddlAuth_expYear.SelectedValue);

            Billing billing = new Billing();
            billing.BillingID = Convert.ToInt64(tbAuth_BillingID.Text);
            billing.FirstName = tbAuth_FirstName.Text;
            billing.LastName = tbAuth_LastName.Text;
            billing.Address1 = tbAuth_Address1.Text;
            billing.Address2 = tbAuth_Address2.Text;
            billing.City = tbAuth_City.Text;
            billing.State = tbAuth_State.Text;
            billing.Zip = tbAuth_Zip.Text;
            billing.Country = tbAuth_Country.Text;
            billing.Phone = tbAuth_Phone.Text;
            billing.Email = tbAuth_Email.Text;
            billing.IP = tbAuth_IP.Text;
            billing.CreditCard = tbAuth_CreditCard.Text;
            billing.CVV = tbAuth_CVV.Text;
            billing.ExpMonth = expMonth;
            billing.ExpYear = expYear;

            return billing;
        }

        private ChargeHistoryEx createChargeHistoryExObject
        {
            get { return Session["chargeHistoryEx"] as ChargeHistoryEx; }
            set { createChargeHistoryExObject = value; }
        }

        public override string HeaderString
        {
            get { return "Virtual Terminal"; }
        }
    }
}
