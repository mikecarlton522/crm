using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Business.Flow;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class Order : System.Web.UI.Page
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? BillingID
        {
            get
            {
                return Utility.TryGetLong(Request["BillingID"]);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            FillScriptLiteral();

            ifCustomerExists.Condition = false;
            ifCustomerDoesntExist.Condition = true;
            if (BillingID != null)
            {
                UserBuilder user = new UserBuilder();
                user.LoadByBillingID(BillingID);
                if (user.User != null)
                {
                    ifCustomerExists.Condition = true;
                    ifCustomerDoesntExist.Condition = false;

                    shippingAddress.FirstName = user.User.Registration.FirstName;
                    shippingAddress.LastName = user.User.Registration.LastName;
                    shippingAddress.Address1 = user.User.Registration.Address1;
                    shippingAddress.Address2 = user.User.Registration.Address2;
                    shippingAddress.City = user.User.Registration.City;
                    shippingAddress.Country = (user.User.RegistrationInfo != null ? user.User.RegistrationInfo.Country : null);
                    shippingAddress.State = user.User.Registration.State;
                    shippingAddress.Zip = user.User.Registration.Zip;
                    shippingAddress.Phone = user.User.Registration.Phone;
                    shippingAddress.Email = user.User.Registration.Email;

                    billingAddress.FirstName = user.User.Billing.FirstName;
                    billingAddress.LastName = user.User.Billing.LastName;
                    billingAddress.Address1 = user.User.Billing.Address1;
                    billingAddress.Address2 = user.User.Billing.Address2;
                    billingAddress.City = user.User.Billing.City;
                    billingAddress.Country = user.User.Billing.Country;
                    billingAddress.State = user.User.Billing.State;
                    billingAddress.Zip = user.User.Billing.Zip;
                    billingAddress.Phone = user.User.Billing.Phone;
                    billingAddress.Email = user.User.Billing.Email;
                    billingAddress.FirstName = user.User.Billing.FirstName;

                    creditCard.ExpMonth = user.User.Billing.ExpMonth;
                    creditCard.ExpYear = user.User.Billing.ExpYear;
                    creditCard.CVV = user.User.Billing.CVV;
                    creditCard.CreditCard = user.User.Billing.CreditCard;
                }
            }
        }

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            //Check selected upsells
            if (string.IsNullOrEmpty(Request["ddlUpsell1"]) &&
                string.IsNullOrEmpty(Request["ddlUpsell2"]) &&
                string.IsNullOrEmpty(Request["ddlUpsell3"]) &&
                NewSubscriptionControl1.SelectedRecurringPlanID == null)
            {
                Error1.Show("Please choose at least one product or subscription", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check current campaign
            int? campaignID = Logic.AdminMembership.CurrentCampaignID;
            if (campaignID == null)
            {
                Error1.Show("Error: Can't determine Campaign", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check user default ProductID
            var product = new OrderService().GetDefaultProduct(BillingID.Value);
            if (product == null)
            {
                Error1.Show("Error: Can't determine customer Product Group", Logic.Error.TypeEnum.Error);
                return;
            }

            //check selected subscription
            if (NewSubscriptionControl1.SelectedRecurringPlanID != null)
            {
                if (string.IsNullOrEmpty(NewSubscriptionControl1.TrialSKU) || NewSubscriptionControl1.TrialPeriod == null)
                {
                    Error1.Show("Please choose Trial", Logic.Error.TypeEnum.Error);
                    return;
                }
            }

            //check referer
            Referer referer = null;
            if (!string.IsNullOrEmpty(tbRefererCode.Text))
            {
                referer = refererService.GetByCode(tbRefererCode.Text);
                if (referer == null)
                {
                    Error1.Show("Referer code was not found", Logic.Error.TypeEnum.Error);
                    return;
                }
            }

            //Check input customer data
            UserBuilder user = new UserBuilder().LoadByBillingID(BillingID);
            user.SetBillingAddress(billingAddress.FirstName, billingAddress.LastName,
                billingAddress.Address1, billingAddress.Address2, billingAddress.City, billingAddress.State, billingAddress.Zip, billingAddress.Country,
                billingAddress.Email, billingAddress.Phone)
                .SetShippingAddress(shippingAddress.FirstName, shippingAddress.LastName,
                shippingAddress.Address1, shippingAddress.Address2, shippingAddress.City, shippingAddress.State, shippingAddress.Zip, shippingAddress.Country,
                shippingAddress.Email, shippingAddress.Phone);
            if (creditCard.CreditCard != null && creditCard.CreditCard.Contains('*'))
            {
                creditCard.CreditCard = user.User.Billing.CreditCard;
            }
            user.SetCreditCard(creditCard.CreditCard, creditCard.CVV, creditCard.ExpMonth, creditCard.ExpYear);
            var isValid = user.Validate();
            if (isValid.State == BusinessErrorState.Error)
            {
                Error1.Show(isValid.ErrorMessage + "<br/>" + string.Join("<br/>", isValid.ReturnValue.ToArray()), Logic.Error.TypeEnum.Error);
                return;
            }

            //Save customer
            var resUser = user.Save();
            if (resUser.State == BusinessErrorState.Error)
            {
                Error1.Show("Error: " + resUser.ErrorMessage, Logic.Error.TypeEnum.Error);
                return;
            }

            //Check order data
            OrderBuilder orderBuilder = new OrderBuilder();
            orderBuilder.Create(user.User.Billing.BillingID, campaignID, Logic.AdminMembership.CurrentAdmin.DisplayName,
                Request.UserHostAddress, Request.Url.AbsoluteUri, user.User.Billing.Affiliate, user.User.Billing.SubAffiliate,
                product.ProductID);

            //add Subscription if selected
            if (NewSubscriptionControl1.SelectedRecurringPlanID != null)
            {
                orderBuilder.AppendSale(null, NewSubscriptionControl1.TrialPrice.Value, NewSubscriptionControl1.TrialQty.Value,
                    new OrderBuilder.Product[] { new OrderBuilder.Product() { ProductSKU = new Model.ProductSKU() { ProductSKU_ = NewSubscriptionControl1.TrialSKU }, Quantity = NewSubscriptionControl1.TrialQty.Value } },
                    new OrderBuilder.Plan() { RecurringPlanID = NewSubscriptionControl1.SelectedRecurringPlanID.Value, TrialInterim = NewSubscriptionControl1.TrialPeriod.Value });
            }

            //add upsells to order
            AddUpsellToOrderBuilder(Request["ddlUpsell1"], txtPriceUpsell1.Text, orderBuilder);
            AddUpsellToOrderBuilder(Request["ddlUpsell2"], txtPriceUpsell2.Text, orderBuilder);
            AddUpsellToOrderBuilder(Request["ddlUpsell3"], txtPriceUpsell3.Text, orderBuilder);

            isValid = orderBuilder.Validate();
            if (isValid.State == BusinessErrorState.Error)
            {
                Error1.Show(isValid.ErrorMessage + "<br/>" + string.Join("<br/>", isValid.ReturnValue.ToArray()), Logic.Error.TypeEnum.Error);
                return;
            }

            //Save Order
            var orderRes = orderBuilder.Save();
            if (orderRes.State == BusinessErrorState.Error)
            {
                Error1.Show("Error: " + orderRes.ErrorMessage, Logic.Error.TypeEnum.Error);
                return;
            }

            //Process Order
            OrderFlow orderFlow = new OrderFlow();
            var res = orderFlow.ProcessOrder(orderRes.ReturnValue);

            //add referer
            if (!string.IsNullOrEmpty(tbRefererCode.Text) && res.State == BusinessErrorState.Success)
            {
                if (referer != null)
                    refererService.CreateOrderReferer(res.ReturnValue.Last().Invoice.Order.Order, referer.RefererID, BillingID.Value);
            }

            if (res.State == BusinessErrorState.Success)
            {
                Error1.Show("Thank you. A new Order has been placed." + (res.ReturnValue != null && res.ReturnValue.Count > 0 && res.ReturnValue.Last().ChargeResult != null ? "<br/>Gateway Response: " + res.ReturnValue.Last().ChargeResult.ChargeHistory.Response : ""), res.State);
            }
            else
            {
                Error1.Show("Error. " + res.ErrorMessage + "<br/>" + (res.ReturnValue != null && res.ReturnValue.Count > 0 && res.ReturnValue.Last().ChargeResult != null ? "<br/>Gateway Response: " + res.ReturnValue.Last().ChargeResult.ChargeHistory.Response : ""), res.State);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Error1.Visible == false)
                Error1.Show("", BusinessErrorState.Success);
        }

        #region Helpers

        private void FillScriptLiteral()
        {
            //adding ProductProductCodeList to js
            ProductService prService = new ProductService();
            var productProductCodeList = prService.GetProductProductCodeList();
            var productCodeList = prService.GetProductCodeList();
            var productCodeInfoList = prService.GetProductCodeInfoList();

            StringBuilder script = new StringBuilder();
            script.AppendLine("var productGroupList = new Array();");
            int loop = 0;
            foreach (var prCode in productCodeList)
            {
                var prInfo = productCodeInfoList.FirstOrDefault(u => u.ProductCodeID == prCode.ProductCodeID);
                foreach (var prProduct in productProductCodeList.Where(u => u.ProductCodeID == prCode.ProductCodeID))
                {
                    script.Append(string.Format("productGroupList[{0}] = ", loop));
                    script.Append("{");
                    script.Append("ProductCode : \"");
                    script.Append(prCode.ProductCode_);
                    script.Append("\", Price : \"");
                    script.Append(Utility.FormatPrice((prInfo == null ? 0 : prInfo.RetailPrice)));
                    script.Append("\", ProductID : \"");
                    script.Append((prProduct == null ? 0 : prProduct.ProductID));
                    script.Append("\", ProductName : \"");
                    script.Append(prCode.Name);
                    script.AppendLine("\"};");
                    loop++;
                }
            }
            litProductProductCodeList.Text = script.ToString();
            //adding ProductProductCodeList to js
        }

        private void AddUpsellToOrderBuilder(string upsellDropDownValue, string txtPrice, OrderBuilder orderBuilder)
        {
            if (!string.IsNullOrEmpty(upsellDropDownValue))
            {
                orderBuilder.AppendProductCode(upsellDropDownValue, 1, Utility.TryGetDecimal(txtPrice) ?? 0);
            }
        }

        #endregion
    }
}