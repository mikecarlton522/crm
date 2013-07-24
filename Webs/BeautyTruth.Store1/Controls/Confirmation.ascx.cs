using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BeautyTruth.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace BeautyTruth.Store1.Controls
{
    public partial class Confirmation : System.Web.UI.UserControl
    {
        public ShoppingCart ShoppingCart
        {
            get
            {
                return ShoppingCart.Current;
            }
        }

        protected decimal ShippingValue
        {
            get
            {
                if (Session["ShoppingCart"] == null)
                    return 0;
                return (Session["ShoppingCart"] as ShoppingCart_).ShippingValue;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        private void Clear()
        {
            ShoppingCart.Load();
            ShoppingCart.Products = new Dictionary<ShoppingCartProduct, int>();
            ShoppingCart.Save();
            ((ShoppingCart_)Session["ShoppingCart"]).Products = new Dictionary<ShoppingCartProduct, int>();
            //((Checkout)Session["Checkout"]).Billing = new Billing();
            ShoppingCart.CouponCode = null;
            ShoppingCart.GiftCertificateList.Clear();
            ShoppingCart.EcigBucksAmount = null;
        }

        public int? CampaignID { get; set; }
        protected ComplexSaleView Order { get; private set; }
        protected Registration Registration { get; private set; }
        public RegistrationInfo RegistrationInfo { get; private set; }
        protected AssertigyMID AssertgyMid { get; private set; }
        protected ICoupon Coupon { get; private set; }
        protected string CustomerReferenceNumbers { get; private set; }
        private Dictionary<ShoppingCartProduct, int> Products;
        public IList<PromoGift> GiftCertificateList { get; set; }
        public decimal? EcigBucksAmount { get; set; }
        public IList<PromoGift> GiftCertificateListApplied { get; set; }
        public decimal EcigBucksAmountApplied { get; set; }

        public PromoGift Gift
        {
            get
            {
                if (GiftCertificateList == null || GiftCertificateListApplied == null || GiftCertificateList.Count == 0 || GiftCertificateListApplied.Count == 0)
                    return null;
                else
                {
                    return new PromoGift()
                    {
                        RemainingValue = GiftCertificateList.FirstOrDefault().RemainingValue - GiftCertificateListApplied.FirstOrDefault().Value,
                        Value = GiftCertificateList.FirstOrDefault().RemainingValue
                    }; 
                }
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
                    var subscription = new SubscriptionService().Load<Subscription>(billingFreeSale.Value4.SubscriptionID);
                    if (subscription != null)
                    {
                        ShoppingCartProduct product = new ShoppingCartProduct()
                        {
                            ProductType = ShoppingCartProductType.Subscription,
                            Price = (subscription.InitialBillAmount ?? 0) + (subscription.InitialShipping ?? 0),
                            ProductID = subscription.SubscriptionID.Value,
                            ProductSKU = subscription.SKU2
                        };
                        var exProduct = Products.Where(u => u.Key.ProductType == ShoppingCartProductType.Subscription && u.Key.ProductID == subscription.SubscriptionID.Value).Select(u => u.Key).SingleOrDefault();
                        if (exProduct != null)
                        {
                            Products[exProduct] += 1;
                        }
                        else
                            Products.Add(product, 1);
                    }
                }
                foreach (Set<BillingSale, BillingSubscription, Subscription> billingSale in Order.BillingSales)
                {
                    var subscription = new SubscriptionService().Load<Subscription>(billingSale.Value3.SubscriptionID);
                    if (subscription != null)
                    {
                        ShoppingCartProduct product = new ShoppingCartProduct()
                        {
                            ProductType = ShoppingCartProductType.Subscription,
                            Price = (subscription.InitialBillAmount ?? 0) + (subscription.InitialShipping ?? 0),
                            ProductID = subscription.SubscriptionID.Value,
                            ProductSKU = subscription.SKU2
                        };
                        var exProduct = Products.Where(u => u.Key.ProductType == ShoppingCartProductType.Subscription && u.Key.ProductID == subscription.SubscriptionID.Value).Select(u => u.Key).SingleOrDefault();
                        if (exProduct != null)
                        {
                            Products[exProduct] += 1;
                        }
                        else
                            Products.Add(product, 1);
                    }
                }
                foreach (Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> upsellFreeSale in Order.UpsellFreeSales)
                {
                    var prInfo = new ProductService().GetProductCodeInfo(upsellFreeSale.Value4.ProductCode);
                    if (prInfo != null)
                    {
                        ShoppingCartProduct product = new ShoppingCartProduct()
                        {
                            Price = prInfo.RetailPrice ?? 0,
                            ProductID = prInfo.ProductCodeID ?? 0,
                            ProductType = ShoppingCartProductType.Upsell,
                            ProductSKU = prInfo.ProductCode_
                        };
                        if (upsellFreeSale.Value3 != null)
                        {
                            Products[product] = upsellFreeSale.Value3.Quantity.Value;
                        }
                        else
                        {
                            Products.Add(product, 1);
                        }
                    }
                }
                foreach (Set<UpsellSale, BillingSubscription, Upsell, UpsellType> upsellSale in Order.UpsellSales)
                {
                    var prInfo = new ProductService().GetProductCodeInfo(upsellSale.Value4.ProductCode);
                    if (prInfo != null)
                    {
                        ShoppingCartProduct product = new ShoppingCartProduct()
                        {
                            Price = prInfo.RetailPrice ?? 0,
                            ProductID = prInfo.ProductCodeID ?? 0,
                            ProductType = ShoppingCartProductType.Upsell,
                            ProductSKU = prInfo.ProductCode_
                        };
                        if (upsellSale.Value3 != null)
                        {
                            Products[product] = upsellSale.Value3.Quantity.Value;
                        }
                        else
                        {
                            Products.Add(product, 1);
                        }
                    }
                }
            }
            else
            {
                Registration = new Registration();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            LoadOrder();
            base.OnDataBinding(e);

            if (Order != null)
            {
                GiftCertificateList = Order.PromoGiftList;
                Coupon = Order.Coupon;
                CalclulateRedeems();
                rProducts.DataSource = Products;
                if (Order.ParentBilling != null)
                {
                    new EmailService().ProcessEmailQueue(Order.ParentBilling.BillingID);

                    if (Order.ParentBilling.PaymentTypeID != null)
                        lblCreditCard.Text = Order.ParentBilling.PaymentTypeID == 2 ? "Visa " : (Order.ParentBilling.PaymentTypeID == 3 ? "MC " : "Amex ");
                    if (!string.IsNullOrEmpty(Order.ParentBilling.CreditCard))
                        lblCreditCard.Text += Order.ParentBilling.CreditCardCnt.DecryptedCreditCard.Remove(6, 6).Insert(6, "******");
                    lblEmail1.Text = Order.ParentBilling.Email;
                    lblEmail2.Text = Order.ParentBilling.Email;
                    lblConfirmNumber.Text = CustomerReferenceNumbers;
                }
            }
        }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("c");
        }

        protected decimal TotalCost
        {
            get
            {
                return CalculateTotalCost();
            }
        }

        private void CalclulateRedeems()
        {
            decimal sum = CalculatePoorTotalCost();

            EcigBucksAmountApplied = 0M;
            if (EcigBucksAmount != null)
            {
                EcigBucksAmountApplied = (sum > EcigBucksAmount.Value ? EcigBucksAmount.Value : sum);
            }

            sum -= EcigBucksAmountApplied;

            GiftCertificateListApplied = new List<PromoGift>();
            if (GiftCertificateList != null)
            {
                foreach (PromoGift item in GiftCertificateList)
                {
                    if (sum == 0M)
                    {
                        break;
                    }
                    if (item.RemainingValue != null)
                    {
                        decimal redeemAmount = (sum > item.RemainingValue.Value ? item.RemainingValue.Value : sum);
                        sum -= redeemAmount;
                        GiftCertificateListApplied.Add(new PromoGift()
                        {
                            GiftNumber = item.GiftNumber,
                            Value = redeemAmount
                        });
                    }
                }
            }
        }

        protected decimal CalculatePoorTotalCost()
        {
            decimal sum = 0M;

            sum = (HttpContext.Current.Session["ShoppingCart"] as ShoppingCart_).ShippingValue;

            if (Coupon != null && !(Coupon is ProductCoupon))
            {
                sum = Coupon.ApplyDiscount(sum, DiscountType.Discount);
            }

            foreach (var item in Products)
            {
                decimal itemAmount = item.Key.Price;
                if (Coupon != null)
                {
                    if (Coupon is ProductCoupon)
                    {
                        itemAmount = ((ProductCoupon)Coupon).ApplyDiscount(item.Key.ProductSKU, itemAmount, DiscountType.Any);
                    }
                    else
                    {
                        itemAmount = Coupon.ApplyDiscount(itemAmount, DiscountType.Discount);
                    }
                }
                itemAmount = itemAmount * (decimal)item.Value;
                sum += itemAmount;
            }
            return sum;
        }

        protected decimal CalculateTotalCost()
        {
            decimal sum = CalculatePoorTotalCost();
            sum -= EcigBucksAmountApplied;
            if (GiftCertificateListApplied != null)
            {
                foreach (PromoGift item in GiftCertificateListApplied)
                {
                    sum -= item.Value.Value;
                }
            }
            return sum;
        }
    }
}