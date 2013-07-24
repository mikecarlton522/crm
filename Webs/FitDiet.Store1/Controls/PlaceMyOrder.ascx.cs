using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Controls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Controls;
using TrimFuel.Model.Containers;

namespace FitDiet.Store1.Controls
{
    public partial class PlaceMyOrder : System.Web.UI.UserControl, IStepControl
    {
        void IStepControl.ShowError(string errorText)
        {
            lError.Text = errorText;
            phError.Visible = true;
        }

        void IStepControl.HideError()
        {
            phError.Visible = false;
        }

        public event EventHandler<StepChangeEventArgs> StepChanged;
        protected void OnStepChanged(StepChangeEventArgs e)
        {
            if (StepChanged != null)
            {
                StepChanged(this, e);
            }
        }

        protected void ChangeStep_Click(object sender, StepChangeEventArgs e)
        {
            OnStepChanged(e);
        }

        private Dictionary<string, string> values = new Dictionary<string, string>()
        {
            {"2", "Visa"},
            {"3", "MasterCard"}
        };

        public Billing Billing;

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            rProducts.DataSource = Products;

            if (Billing != null)
            {
                if (!string.IsNullOrEmpty(Billing.CreditCard))
                    lblCreditCardNumber.Text = "************" + Billing.CreditCardCnt.DecryptedCreditCard.Substring(12, 4);
                if (Billing.PaymentTypeID != null)
                    lblPaymentType.Text = Billing.PaymentTypeID.Value == 2 ? "Visa" : "MasterCard";
                lblCreditCardCVV.Text = Billing.CVV;
                if (Billing.ExpMonth != null)
                {
                    lblExpireDate.Text = Billing.ExpMonth.ToString();
                }
                if (Billing.ExpYear != null)
                {
                    lblExpireDate.Text += "/" + Billing.ExpYear.ToString();
                }
                lblFirstName.Text = Billing.FirstName;
                lblLastName.Text = Billing.LastName;
                lblAddress1.Text = Billing.Address1;
                lblAddress2.Text = Billing.Address2;
                lblCity.Text = Billing.City;
                if (string.IsNullOrEmpty(Billing.Country) || Billing.Country == RegistrationService.DEFAULT_COUNTRY)
                {
                    phAddressUS.Visible = true;
                    phAddressEx.Visible = false;
                    lblCountry.Text = RegistrationService.DEFAULT_COUNTRY;
                    if (!string.IsNullOrEmpty(Billing.State))
                    {
                        lblState.Text = Billing.State;
                    }
                    lblZip.Text = Billing.Zip;
                    lblPhone1.Text = Billing.PhoneCnt.Code;
                    lblPhone2.Text = Billing.PhoneCnt.Part1;
                    lblPhone3.Text = Billing.PhoneCnt.Part2;
                }
                else
                {
                    phAddressUS.Visible = false;
                    phAddressEx.Visible = true;
                    lblCountry.Text = Billing.Country;
                    lblStateEx.Text = Billing.State;
                    lblZipEx.Text = Billing.Zip;
                    lblPhoneEx.Text = Billing.Phone;
                }
                lblEmail.Text = Billing.Email;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Products == null)
            {
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            Dictionary<ShoppingCartProduct, int> loadProducts = new Dictionary<ShoppingCartProduct, int>();
            foreach (RepeaterItem rItem in rProducts.Items)
            {
                if (rItem.ItemType == ListItemType.Item || rItem.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hdnProductID = (HiddenField)rItem.FindControl("hdnProductID");
                    Label lblProductCount = (Label)rItem.FindControl("lblProductCount");

                    ShoppingCartProduct product = ShoppingCart.DeserializeProduct(hdnProductID.Value);
                    if (product != null)
                    {
                        int productCount = 1;
                        int.TryParse(lblProductCount.Text, out productCount);

                        if (productCount > 0)
                        {
                            if (!loadProducts.ContainsKey(product))
                            {
                                loadProducts[product] = productCount;
                            }
                            else
                            {
                                loadProducts[product] += productCount;
                            }
                        }
                    }
                }
            }
            Products = loadProducts;
        }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("c");
        }

        public IEnumerable<KeyValuePair<ShoppingCartProduct, int>> Products { get; set; }
        public int? CampaignID { get; set; }

        protected decimal TotalCost
        {
            get
            {
                return ShoppingCart.Current.TotalCost;
            }
        }
    }
}