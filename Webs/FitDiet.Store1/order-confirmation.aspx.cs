using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;

namespace Fitdiet.Store1
{
    public partial class order_confirmation : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClearCart();
        }

        private void ClearCart()
        {
            ShoppingCart.Load();
            //ShoppingCart.FailedBillingID = null;
            ShoppingCart.Products = new Dictionary<ShoppingCartProduct, int>();
            ShoppingCart.CouponCode = null;
            ShoppingCart.GiftCertificateList.Clear();
            ShoppingCart.EcigBucksAmount = null;
            ShoppingCart.Save();
        }

        protected ComplexSaleView Order { get; private set; }
        protected Registration Registration { get; private set; }
        public RegistrationInfo RegistrationInfo { get; private set; }
        protected AssertigyMID AssertgyMid { get; private set; }
        protected ICoupon Coupon { get; private set; }
        protected string CustomerReferenceNumbers { get; private set; }
        protected Dictionary<ShoppingCartProduct, int> Products { get; private set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            LoadOrder();
            ShoppingCartView1.Coupon = Coupon;
            ShoppingCartView1.Products = Products;
            if (Order != null)
            {
                if (Order.ParentBilling != null)
                    new EmailService().ProcessEmailQueue(Order.ParentBilling.BillingID);

                ShoppingCartView1.GiftCertificateList = Order.PromoGiftList;
                ShoppingCartView1.EcigBucksAmount = Order.RefererCommissionRedeem;
            }
        }

        private void LoadRegistrationInfo()
        {
            if (Order != null && Order.Registration != null && Order.Registration.RegistrationID != null)
            {
                RegistrationInfo = (new RegistrationService()).GetRegistrationInfo(Order.Registration.RegistrationID.Value);
            }
            if (RegistrationInfo == null)
            {
                RegistrationInfo = new RegistrationInfo() { Country = RegistrationService.DEFAULT_COUNTRY };
            }
            if (string.IsNullOrEmpty(RegistrationInfo.Country))
            {
                RegistrationInfo.Country = RegistrationService.DEFAULT_COUNTRY;
            }
        }

        private void LoadOrder()
        {
            Order = ShoppingCart.LoadOrder();
            Products = new Dictionary<ShoppingCartProduct, int>();            
            if (Order != null)
            {
                if (Order.ParentBilling != null)
                {
                    new EmailService().ProcessEmailQueue(Order.ParentBilling.BillingID);
                }

                Registration = Order.Registration;
                LoadRegistrationInfo();
                Coupon = Order.Coupon;
                AssertgyMid = Order.AssertigyMID;
                CustomerReferenceNumbers = string.Join(", ",
                    Order.BillingFreeSales.Select(e => e.Value2.CustomerReferenceNumber).Union(
                    Order.BillingSales.Select(e => e.Value2.CustomerReferenceNumber)).Union(
                    Order.UpsellFreeSales.Select(e => e.Value2.CustomerReferenceNumber)).Union(
                    Order.UpsellSales.Select(e => e.Value2.CustomerReferenceNumber)).
                    Distinct().ToArray());

                foreach (Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> billingFreeSale in Order.BillingFreeSales)
                {
                    ShoppingCartProduct product = ShoppingCart.GetKnownProduct(
                        ShoppingCartProductType.Subscription,
                        billingFreeSale.Value4.SubscriptionID.Value);

                    if (product != null)
                    {
                        if (Products.ContainsKey(product))
                        {
                            Products[product] += 1;
                        }
                        else
                        {
                            Products[product] = 1;
                        }
                    }
                }
                foreach (Set<BillingSale, BillingSubscription, Subscription> billingSale in Order.BillingSales)
                {
                    ShoppingCartProduct product = ShoppingCart.GetKnownProduct(
                        ShoppingCartProductType.Subscription, 
                        billingSale.Value3.SubscriptionID.Value);

                    if (product != null)
                    {
                        if (Products.ContainsKey(product))
                        {
                            Products[product] += 1;
                        }
                        else
                        {
                            Products[product] = 1;
                        }
                    }
                }
                foreach (Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> upsellFreeSale in Order.UpsellFreeSales)
                {
                    ShoppingCartProduct product = ShoppingCart.GetKnownProduct(
                        ShoppingCartProductType.Upsell,
                        upsellFreeSale.Value4.UpsellTypeID.Value);

                    if (product != null)
                    {
                        if (upsellFreeSale.Value3 != null)
                        {
                            Products[product] = upsellFreeSale.Value3.Quantity.Value;
                        }
                        else
                        {
                            Products[product] = 1;
                        }
                    }
                }
                foreach (Set<UpsellSale, BillingSubscription, Upsell, UpsellType> upsellSale in Order.UpsellSales)
                {
                    ShoppingCartProduct product = ShoppingCart.GetKnownProduct(
                        ShoppingCartProductType.Upsell,
                        upsellSale.Value4.UpsellTypeID.Value);

                    if (product != null)
                    {
                        if (upsellSale.Value3 != null)
                        {
                            Products[product] = upsellSale.Value3.Quantity.Value;
                        }
                        else
                        {
                            Products[product] = 1;
                        }
                    }
                }
            }
            else
            {
                Registration = new Registration();
            }
        }
    }
}
