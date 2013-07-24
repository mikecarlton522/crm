using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.DefaultEmail;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class DynamicEmail_ : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            phMessage.Visible = false;
            phMessage2.Visible = false;
            phMessage3.Visible = false;

            if (!IsPostBack)
            {
                InitProps();
                DataBind();
            }
        }

        private int? DynamicEmailID 
        {
            get { return Utility.TryGetInt(hdnDynamicEmailID.Value) ?? Utility.TryGetInt(Request["dynamicEmailID"]); }
        }

        private byte? DynamicEmailTypeID
        {
            get { return Utility.TryGetByte(Request["dynamicEmailTypeID"]); }
        }

        private int? ProductID
        {
            get { return Utility.TryGetInt(Request["productID"]); }
        }

        private short? Hours
        {
            get { return Utility.TryGetShort(Request["hours"]); }
        }

        #region Custom Parameters

        private int? GiftCertificateDynamicEmail_StoreID
        {
            get { return Utility.TryGetShort(Request["storeID"]); }
        }

        #endregion

        public DynamicEmail DynamicEmail { get; set; }
        public DynamicEmailType DynamicEmailType { get; set; }
        public Product Product { get; set; }

        public Store GiftCertificateDynamicEmail_Store { get; set; }

        private void InitProps()
        {
            BaseService serv = new BaseService();
            if (DynamicEmailID != null)
            {
                DynamicEmail = serv.Load<DynamicEmail>(DynamicEmailID);
                DynamicEmailType = serv.Load<DynamicEmailType>(DynamicEmail.DynamicEmailTypeID);
                Product = serv.Load<Product>(DynamicEmail.ProductID);

                if (DynamicEmail.DynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.GiftCertificateGiven)
                    GiftCertificateDynamicEmail_Store = serv.Load<Store>(serv.Load<GiftCertificateDynamicEmail>(DynamicEmail.DynamicEmailID).StoreID);
            }
            else
            {
                DynamicEmailType = serv.Load<DynamicEmailType>(DynamicEmailTypeID);
                Product = serv.Load<Product>(ProductID);

                DynamicEmail = new DynamicEmail();
                DynamicEmail.ProductID = ProductID;
                DynamicEmail.Days = Hours;
                DynamicEmail.DynamicEmailTypeID = DynamicEmailTypeID;
                DynamicEmail.Active = true;

                //Init customization properties
                if (GiftCertificateDynamicEmail_StoreID != null)
                    GiftCertificateDynamicEmail_Store = serv.Load<Store>(GiftCertificateDynamicEmail_StoreID);
            }
        }

        protected bool AllowHours
        { 
            get { return DynamicEmail.DynamicEmailID == null && (DynamicEmailType.DynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.Abandons || DynamicEmailType.DynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.Newsletter); }
        }

        protected void bSave_Click(object sender, EventArgs e)
        {
            InitProps();
            if (AllowHours)
            {
                DynamicEmail.Days = Utility.TryGetShort(tbHours.Text);
            }

            EmailService serv = new EmailService();
            if (DynamicEmail.DynamicEmailID == null)
            {
                //DynamicEmail = serv.CreateDynamicEmail(DynamicEmail.ProductID, DynamicEmail.DynamicEmailTypeID, DynamicEmail.Days,
                //    chbActive.Checked, Utility.TryGetStr(tbFromName.Text), Utility.TryGetStr(tbFromAddress.Text),
                //    Utility.TryGetStr(tbSubject.Text), Utility.TryGetStr(tbLanding.Text), Utility.TryGetStr(tbLandingLink.Text),
                //    Utility.TryGetStr(tbBody.Text), GiftCertificateDynamicEmail_StoreID);
                DynamicEmail = serv.CreateDynamicEmail(DynamicEmail.ProductID, null, DynamicEmail.DynamicEmailTypeID, DynamicEmail.Days,
                    chbActive.Checked, Utility.TryGetStr(tbFromName.Text), Utility.TryGetStr(tbFromAddress.Text),
                    Utility.TryGetStr(tbSubject.Text), "", "",
                    Utility.TryGetStr(tbBody.Text), GiftCertificateDynamicEmail_StoreID);
            }
            else
            {
                //DynamicEmail = serv.UpdateDynamicEmail(DynamicEmail.DynamicEmailID.Value, DynamicEmail.ProductID, DynamicEmail.DynamicEmailTypeID, DynamicEmail.Days,
                //    chbActive.Checked, Utility.TryGetStr(tbFromName.Text), Utility.TryGetStr(tbFromAddress.Text),
                //    Utility.TryGetStr(tbSubject.Text), Utility.TryGetStr(tbLanding.Text), Utility.TryGetStr(tbLandingLink.Text),
                //    Utility.TryGetStr(tbBody.Text));
                DynamicEmail = serv.UpdateDynamicEmail(DynamicEmail.DynamicEmailID.Value, DynamicEmail.ProductID, null, DynamicEmail.DynamicEmailTypeID, DynamicEmail.Days,
                    chbActive.Checked, Utility.TryGetStr(tbFromName.Text), Utility.TryGetStr(tbFromAddress.Text),
                    Utility.TryGetStr(tbSubject.Text), "", "",
                    Utility.TryGetStr(tbBody.Text));
            }
            phMessage.Visible = true;
            DataBind();
        }

        protected void bSendTestEmail_Click(object sender, EventArgs e)
        {
            InitProps();

            if (AllowHours)
            {
                DynamicEmail.Days = Utility.TryGetShort(tbHours.Text);
            }
            DynamicEmail.Active = chbActive.Checked;
            DynamicEmail.FromName = Utility.TryGetStr(tbFromName.Text);
            DynamicEmail.FromAddress = Utility.TryGetStr(tbFromAddress.Text);
            DynamicEmail.Subject = Utility.TryGetStr(tbSubject.Text);
            DynamicEmail.Landing = Utility.TryGetStr(tbLanding.Text);
            DynamicEmail.LandingLink = Utility.TryGetStr(tbLandingLink.Text);
            DynamicEmail.Content = Utility.TryGetStr(tbBody.Text);

            try
            {
                IEmailGateway emailGateway = new DefaultEmailGateway();

                string email = Utility.TryGetStr(tbTestEmail.Text) ?? string.Empty;
                string fromName = Utility.TryGetStr(tbFromName.Text) ?? string.Empty;
                string fromAddress = Utility.TryGetStr(tbFromAddress.Text) ?? string.Empty;
                string subject = Utility.TryGetStr(tbSubject.Text) ?? string.Empty;
                string body = Utility.TryGetStr(tbBody.Text) ?? string.Empty;

                Billing billing = new Billing() {
                    BillingID = 0,
                    FirstName = "Steve",
                    LastName = "Miller",
                    Address1 = "1 Main Street",
                    Address2 = "",
                    City = "Los Angeles",
                    State = "CA",
                    Zip = "98110",
                    Phone = "800-555-1212",
                    Email = "dtcoder@gmail.com",
                    CreditCard = "4111111111111111",

                };

                body = body.Replace("##FNAME##", billing.FirstName);
                body = body.Replace("##LNAME##", billing.LastName);
                body = body.Replace("##ADD1##", billing.Address1);
                body = body.Replace("##ADD2##", billing.Address2);
                body = body.Replace("##CITY##", billing.City);
                body = body.Replace("##STATE##", billing.State);
                body = body.Replace("##ZIP##", billing.Zip);
                body = body.Replace("##PHONE##", billing.Phone);
                body = body.Replace("##EMAIL##", billing.Email);

                body = body.Replace("##SHIPPING_FNAME##", billing.FirstName);
                body = body.Replace("##SHIPPING_LNAME##", billing.LastName);
                body = body.Replace("##SHIPPING_ADD1##", billing.Address1);
                body = body.Replace("##SHIPPING_ADD2##", billing.Address2);
                body = body.Replace("##SHIPPING_CITY##", billing.City);
                body = body.Replace("##SHIPPING_STATE##", billing.State);
                body = body.Replace("##SHIPPING_ZIP##", billing.Zip);
                body = body.Replace("##SHIPPING_PHONE##", billing.Phone);
                body = body.Replace("##SHIPPING_EMAIL##", billing.Email);

                body = body.Replace("##RMA##", "RMA12345");

                string creditCardType = null;
                string creditCard = billing.CreditCardCnt.DecryptedCreditCard;
                if (creditCard.StartsWith("4"))
                {
                    creditCardType = "Visa";
                }
                else if (creditCard.StartsWith("5"))
                {
                    creditCardType = "MasterCard";
                }
                else
                {
                    creditCardType = "card";
                }
                creditCard = creditCard.Substring(creditCard.Length - 4, 4);

                body = body.Replace("##CARDTYPE##", creditCardType);
                body = body.Replace("##LAST4##", creditCard);

                string merchantName = "MIDNAME0000";
                body = body.Replace("##MID##", merchantName ?? "**TEST ORDER**");
                decimal shippingAmount = 9.99M;
                body = body.Replace("##SH_AMOUNT##", Utility.FormatPrice(shippingAmount));
                decimal productAmount = 0.00M;
                body = body.Replace("##PRODUCT_AMOUNT##", Utility.FormatPrice(productAmount));
                body = body.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(Utility.Add(shippingAmount, productAmount)));
                body = body.Replace("##BILLINGID##", billing.BillingID.Value.ToString());
                long emailID = 0;
                body = body.Replace("##ID##", emailID.ToString());

                string REACTIVATION_LINK_TEMPLATE = "http://www.ecigsbrandoffer.com/reactivate/default.asp?b={0}";
                body = body.Replace("##REACTIVATION_LINK##", string.Format(REACTIVATION_LINK_TEMPLATE, billing.BillingID));

                string billingCancelCode = "QWERTY";
                body = body.Replace("##CANCELCODE##", billingCancelCode);

                string ownRefererCode = "Miller1234";
                body = body.Replace("##OWN_REFERER_CODE##", ownRefererCode);
                string refererCode = "Steve1234";
                body = body.Replace("##REFERER_CODE##", refererCode);

                /////////////////////////////////////////////////////////////////////////

                string strItemTemplate = "<tr height=\"30\"><td width=\"35%\"><font face=\"geneva, verdana, arial\" size=\"2\" color=\"#000000\">##ITEM_SKU##</font></td><td width=\"65%\"><font face=\"geneva, verdana, arial\" size=\"2\" color=\"#000000\">##ITEM_COUNT##</font></td></tr><tr><td colspan=\"2\" nowrap=\"nowrap\"><img src=\"http://d3oimv5qppjae2.cloudfront.net/email/spacer-cccccc.gif\" height=\"1\" width=\"100%\" /></td></tr>";
                string strProductsTemplate = "";

                IList<TrimFuel.Model.Views.InventoryView> products = new List<TrimFuel.Model.Views.InventoryView>();
                products.Add(new TrimFuel.Model.Views.InventoryView() { 
                    InventoryID = 0,
                    Product = "Test Product",
                    SKU = "VG-TEST",
                    Quantity = 2
                });
                products.Add(new TrimFuel.Model.Views.InventoryView()
                {
                    InventoryID = 0,
                    Product = "Another Test Product",
                    SKU = "VG-TEST-ANOTHER",
                    Quantity = 1
                });
                foreach (TrimFuel.Model.Views.InventoryView iv in products)
                {
                    strProductsTemplate += strItemTemplate.Replace("##ITEM_SKU##", iv.Product).Replace("##ITEM_COUNT##", iv.Quantity.ToString());
                }

                body = body.Replace("##SHIPPING_PRODUCTS##", strProductsTemplate);
                string trackingNumber = "000000000000000000000";
                body = body.Replace("##TRACKING_NUMBER##", trackingNumber);

                /////////////////////////////////////////////////////////////////////////////

                Referer referer = new Referer() { 
                    FirstName = "Steve",
                    Password = "psw02020202"
                };
                body = body.Replace("##FNAME##", referer.FirstName);
                body = body.Replace("##PASSWORD##", referer.Password);

                /////////////////////////////////////////////////////////////////////////////

                referer.Username = "dtcoder@gmail.com";
                referer.RefererCode = "Steve00001";
                body = body.Replace("##FNAME##", referer.FirstName);
                body = body.Replace("##USERNAME##", referer.Username);
                body = body.Replace("##PASSWORD##", referer.Password);
                body = body.Replace("##REFERRALID##", referer.RefererCode);

                //new variables
                body = body.Replace("##PRODUCT_NAME##", "Test Product");
                body = body.Replace("##ORDER_DATE##", DateTime.Now.ToShortDateString());

                string reviewTable = "<table border='1' cellpadding='5'><tr><td>Product Name</td><td>Description</td><td>MID</td><td>Order Date</td><td>Subtotal</td><td>Discount</td><td>Total</td></tr>";
                string reviewTableTr = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>";
                foreach (var product in products)
                {
                    reviewTable += string.Format(reviewTableTr, product.Product,
                                    "Test Details",
                                    merchantName ?? "**TEST ORDER**",
                                    DateTime.Now.ToShortDateString(),
                                    60,
                                    0,
                                    60);
                }
                reviewTable += "</table>";
                body = body.Replace("##ORDER_REVIEW_TABLE##", reviewTable);

                string reviewTableFrench = "<table border='1' cellpadding='5'><tr><td>Produit</td><td>Description</td><td>MID</td><td>Date de la commande</td><td>Total partiel</td><td>Rabais</td><td>Total</td></tr>";
                foreach (var product in products)
                {
                    reviewTableFrench += string.Format(reviewTableTr, product.Product,
                                    "Test Details",
                                    merchantName ?? "**TEST ORDER**",
                                    DateTime.Now.ToShortDateString(),
                                    60,
                                    0,
                                    60);
                }
                reviewTableFrench += "</table>";
                body = body.Replace("##ORDER_REVIEW_TABLE_FRENCH##", reviewTableFrench);
                //new variables


                //2box variables
                body = body.Replace("##CPF##", "Test CPF");
                body = body.Replace("##BAIRRO##", "Test BAIRRO");
                body = body.Replace("##CARD_HOLDER##", "Card Holder");
                body = body.Replace("##SHIPPING_NUMERO##", "20");
                body = body.Replace("##SHIPPING_COMPLEMENTO##", "Complemento");
                //2box variables

                emailGateway.SendEmail(fromName, fromAddress, "", email, subject, body);

                phMessage2.Visible = true;
            }
            catch (Exception ex)
            {
                phMessage3.Visible = true;
                lSendEmailError.Text = ex.ToString();
            }

            DataBind();
        }
    }
}
