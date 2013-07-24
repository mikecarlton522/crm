using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using System.Web.SessionState;
using TrimFuel.Business;
using TrimFuel.Model;
using BeautyTruth.Store1.Controls;

namespace BeautyTruth.Store1.Logic
{
    //public enum KnownProduct
    //{
    //    A30DayTrialKit = 1,
    //    B60DayStarterKit = 2,
    //    FitHerbalEnergyFatBurnerCapsules = 3
    //}

    public class ShoppingCart
    {
        public const string DASHBOARD_PATH = "https://dashboard.trianglecrm.com/";
        public const string PHOTO_PATH = "https://dashboard.trianglecrm.com/photo/dashboard.trianglecrm.com/products/";
        private const string SHOPPING_CART_COOKIE = "shoppingCart";

        //public static Dictionary<KnownProduct, ShoppingCartProduct> KnownProducts = new Dictionary<KnownProduct, ShoppingCartProduct>(){
        //    {KnownProduct.A30DayTrialKit, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Subscription, ProductID = 373, Price = 10.00M}},
        //    {KnownProduct.B60DayStarterKit, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Subscription, ProductID = 375, Price = 20.00M}},
        //    {KnownProduct.FitHerbalEnergyFatBurnerCapsules, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Subscription, ProductID = 386, Price = 30.00M}}
        //};

        public IEnumerable<KeyValuePair<ShoppingCartProduct, int>> Products { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public string ClickID { get; set; }
        //public long? FailedBillingID { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public string CouponCode { get; set; }
        public IList<string> GiftCertificateList { get; set; }
        public decimal? EcigBucksAmount { get; set; }
        public string RefererCode { get; set; }
        public int? CampaignID { get; set; }

        #region Cost functions

        private ICoupon coupon = null;
        public ICoupon Coupon
        {
            get
            {
                if (coupon == null && CouponCode != null)
                {
                    coupon = (new RegistrationService()).GetCampaignDiscount(CouponCode, CampaignID.Value);
                }
                return coupon;
            }
        }

        private IList<PromoGift> giftCertificates = null;
        public IList<PromoGift> GiftCertificates
        {
            get
            {
                if (giftCertificates == null)
                {
                    if (GiftCertificateList != null && GiftCertificateList.Count > 0)
                    {
                        giftCertificates = (new SaleService()).GetGiftCertificateByNumber(GiftCertificateList);
                    }
                    else
                    {
                        giftCertificates = new List<PromoGift>();
                    }
                }
                return giftCertificates;
            }
        }

        private void ClearCalculation()
        {
            coupon = null;
            giftCertificates = null;
        }

        public decimal TotalPoorCost
        {
            get
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
        }

        public decimal TotalCost
        {
            get
            {
                decimal sum = TotalPoorCost;
                sum -= EcigBucksRedeem;
                sum -= PromoGiftRedeem;
                return sum;
            }
        }

        public decimal EcigBucksRedeem
        {
            get
            {
                decimal res = 0M;
                decimal sum = TotalPoorCost;
                if (EcigBucksAmount != null && EcigBucksAmount > 0M)
                {
                    res = (sum > EcigBucksAmount.Value ? EcigBucksAmount.Value : sum);
                }
                return res;
            }
        }

        public decimal PromoGiftRedeem
        {
            get
            {
                decimal res = 0M;
                IList<PromoGift> promoGiftRedeemList = GetPromoGiftRedeemList();
                if (promoGiftRedeemList != null && promoGiftRedeemList.Count > 0)
                {
                    res = promoGiftRedeemList.Sum(i => i.Value.Value);
                }
                return res;
            }
        }
        public IList<PromoGift> GetPromoGiftRedeemList()
        {
            IList<PromoGift> res = new List<PromoGift>();
            decimal sum = TotalPoorCost;
            sum -= EcigBucksRedeem;
            if (GiftCertificates != null)
            {
                foreach (PromoGift item in GiftCertificates)
                {
                    if (sum <= 0M)
                    {
                        break;
                    }
                    if (item.RemainingValue != null && item.RemainingValue.Value > 0)
                    {
                        decimal redeemAmount = (sum > item.RemainingValue.Value ? item.RemainingValue.Value : sum);
                        res.Add(new PromoGift()
                        {
                            GiftNumber = item.GiftNumber,
                            Value = redeemAmount,
                            RemainingValue = item.RemainingValue - redeemAmount
                        });
                        sum -= redeemAmount;
                    }
                }
            }
            return res;
        }

        #endregion

        public static ShoppingCart Current
        {
            get
            {
                if (HttpContext.Current.Items[SHOPPING_CART_COOKIE] == null)
                {
                    ShoppingCart cart = new ShoppingCart();
                    cart.Load();
                    HttpContext.Current.Items[SHOPPING_CART_COOKIE] = cart;
                }
                return (ShoppingCart)HttpContext.Current.Items[SHOPPING_CART_COOKIE];
            }
        }

        private bool isLoaded = false;
        public void Load()
        {
            if (!isLoaded)
            {
                ClearCalculation();

                //!!!!!!!!!!!!!!!!!!!!!!!//
                //Products = new Dictionary<ShoppingCartProduct, int>() { { ShoppingCart.KnownProducts[KnownProduct.A30DayTrialKit], 1 } };
                Products = DeserializeProductListFromCookie();

                Affiliate = Utility.TryGetStr(HttpContext.Current.Request.QueryString["aff"]);
                if (Affiliate == null && HttpContext.Current.Request.Cookies["Affiliate"] != null)
                    Affiliate = Utility.TryGetStr(HttpContext.Current.Request.Cookies["Affiliate"].Value);

                SubAffiliate = Utility.TryGetStr(HttpContext.Current.Request.QueryString["sub"]);
                if (SubAffiliate == null && HttpContext.Current.Request.Cookies["SubAffiliate"] != null)
                    SubAffiliate = Utility.TryGetStr(HttpContext.Current.Request.Cookies["SubAffiliate"].Value);

                ClickID = Utility.TryGetStr(HttpContext.Current.Request.QueryString["cid"]);
                if (ClickID == null && HttpContext.Current.Request.Cookies["ClickID"] != null)
                    ClickID = Utility.TryGetStr(HttpContext.Current.Request.Cookies["ClickID"].Value);

                //FailedBillingID = Utility.TryGetLong(HttpContext.Current.Request.QueryString["bid"]);
                //if (FailedBillingID == null && HttpContext.Current.Request.Cookies["BillingID"] != null)
                //    FailedBillingID = Utility.TryGetLong(HttpContext.Current.Request.Cookies["BillingID"].Value);

                //Do not use CouponID from query string
                //CouponID = Utility.TryGetStr(HttpContext.Current.Request.QueryString["CouponID"]);
                if (CouponCode == null && HttpContext.Current.Request.Cookies["CouponCode"] != null)
                    CouponCode = Utility.TryGetStr(HttpContext.Current.Request.Cookies["CouponCode"].Value);

                if (GiftCertificateList == null && HttpContext.Current.Request.Cookies["GiftCertificateList"] != null)
                {
                    string giftCertificateListString = Utility.TryGetStr(HttpContext.Current.Request.Cookies["GiftCertificateList"].Value);
                    if (!string.IsNullOrEmpty(giftCertificateListString))
                    {
                        GiftCertificateList = giftCertificateListString.Split(',').ToList();
                    }
                    else
                    {
                        GiftCertificateList = new List<string>();
                    }
                }
                else
                {
                    GiftCertificateList = new List<string>();
                }

                if (EcigBucksAmount == null && HttpContext.Current.Request.Cookies["EcigBucksAmount"] != null)
                    EcigBucksAmount = Utility.TryGetDecimal(HttpContext.Current.Request.Cookies["EcigBucksAmount"].Value);

                if (RefererCode == null && HttpContext.Current.Request.Cookies["RefererCode"] != null)
                    RefererCode = Utility.TryGetStr(HttpContext.Current.Request.Cookies["RefererCode"].Value);

                CampaignID = GeneralInfo.CampaignID;

                IP = Utility.TryGetStr(HttpContext.Current.Request.Params["REMOTE_ADDR"]);
                Url = Utility.TryGetStr(HttpContext.Current.Request.Params["SERVER_NAME"]);

                isLoaded = true;
            }
        }

        public void Save()
        {
            SerializeProductListToCookie(Products);

            SaveCookie("Affiliate", Affiliate);
            SaveCookie("SubAffiliate", SubAffiliate);
            SaveCookie("ClickID", ClickID);
            //SaveCookie("BillingID", FailedBillingID);
            SaveCookie("CouponCode", CouponCode);
            SaveCookie("GiftCertificateList", string.Join(",", GiftCertificateList.ToArray()));
            SaveCookie("EcigBucksAmount", (EcigBucksAmount != null ? EcigBucksAmount.Value.ToString() : string.Empty));
            SaveCookie("RefererCode", RefererCode, DateTime.Now.AddDays(7));

            ClearCalculation();
        }

        private void SaveCookie(string cookieName, string cookieValue, DateTime expireDate)
        {
            if (!string.IsNullOrEmpty(cookieValue))
            {
                HttpContext.Current.Response.Cookies[cookieName].Value = cookieValue;
                HttpContext.Current.Response.Cookies[cookieName].Path = "/";
                HttpContext.Current.Response.Cookies[cookieName].Expires = expireDate;
            }
            else
            {
                HttpContext.Current.Response.Cookies[cookieName].Value = "deleted";
                HttpContext.Current.Response.Cookies[cookieName].Path = "/";
                HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddYears(-1);
            }
        }

        private void SaveCookie(string cookieName, string cookieValue)
        {
            if (!string.IsNullOrEmpty(cookieValue))
            {
                HttpContext.Current.Response.Cookies[cookieName].Value = cookieValue;
                HttpContext.Current.Response.Cookies[cookieName].Path = "/";
            }
            else
            {
                HttpContext.Current.Response.Cookies[cookieName].Value = "deleted";
                HttpContext.Current.Response.Cookies[cookieName].Path = "/";
                HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddYears(-1);
            }
        }

        private void SaveCookie(string cookieName, long? cookieValue)
        {
            if (cookieValue != null)
            {
                SaveCookie(cookieName, cookieValue.Value.ToString());
            }
            else
            {
                SaveCookie(cookieName, string.Empty);
            }
        }

        private void SaveCookie(string cookieName, int? cookieValue)
        {
            if (cookieValue != null)
            {
                SaveCookie(cookieName, cookieValue.Value.ToString());
            }
            else
            {
                SaveCookie(cookieName, string.Empty);
            }
        }

        //public static KnownProduct GetProductNumber(ShoppingCartProduct product)
        //{
        //    return KnownProducts.First(item => item.Value == product).Key;
        //}

        public static ShoppingCartProduct GetKnownProduct(ShoppingCartProductType productType, int productID)
        {
            if (productType == ShoppingCartProductType.Upsell)
            {
                var prInfo = new ProductService().GetProductCodeInfo(productID);
                return new ShoppingCartProduct()
                    {
                        ProductType = productType,
                        ProductSKU = prInfo.ProductCode_,
                        ProductID = prInfo.ProductCodeID.Value,
                        Price = prInfo.RetailPrice ?? 0
                    };
            }
            if (productType == ShoppingCartProductType.Subscription)
            {
                var subscription = new ProductService().Load<Subscription>(productID);
                return new ShoppingCartProduct()
                {
                    ProductType = productType,
                    ProductSKU = subscription.SKU2,
                    ProductID = subscription.SubscriptionID.Value,
                    Price = subscription.InitialBillAmount ?? 0 + subscription.InitialShipping ?? 0
                };
            }
            return null;
        }

        public static ShoppingCartProduct DeserializeProduct(string productString)
        {
            ShoppingCartProduct res = new ShoppingCartProduct();

            var strParams = productString.Split('_');
            if (strParams.Length != 2)
                return null;

            if (strParams[0].ToLower() == "product")
            {
                //get Product From DB
                var prInfo = new ProductService().GetProductCodeInfo(Utility.TryGetInt(strParams[1]));
                if (prInfo == null)
                    return null;
                res.ProductType = ShoppingCartProductType.Upsell;
                res.Price = prInfo.RetailPrice ?? 0;
                res.ProductID = prInfo.ProductCodeID.Value;
                res.ProductSKU = prInfo.ProductCode_;
            }
            if (strParams[0].ToLower() == "subscription")
            {
                //get Subscription From DB
                var subscription = new SubscriptionService().Load<Subscription>(Utility.TryGetInt(strParams[1]));
                if (subscription == null)
                    return null;
                res.ProductType = ShoppingCartProductType.Subscription;
                res.Price = (subscription.InitialBillAmount ?? 0) + (subscription.InitialShipping ?? 0);
                res.ProductID = subscription.SubscriptionID.Value;
                res.ProductSKU = subscription.SKU2;
            }
            return res;
        }

        private static IEnumerable<KeyValuePair<ShoppingCartProduct, int>> DeserializeProductList(string productListString)
        {
            IDictionary<ShoppingCartProduct, int> res = new Dictionary<ShoppingCartProduct, int>();

            if (!string.IsNullOrEmpty(productListString))
            {
                foreach (string knownProductString in productListString.Split(','))
                {
                    ShoppingCartProduct product = DeserializeProduct(knownProductString);
                    if (product != null)
                    {
                        var existProd = res.Where(u => u.Key.ProductType == product.ProductType && u.Key.ProductID == product.ProductID).Select(u => u.Key).FirstOrDefault();
                        if (existProd != null)
                        {
                            res[existProd]++;
                        }
                        else
                        {
                            res[product] = 1;
                        }
                    }
                }
            }

            return res;
        }

        private static string SerializeProductList(IEnumerable<KeyValuePair<ShoppingCartProduct, int>> productList)
        {
            string res = string.Empty;

            if (productList != null && productList.Count() > 0)
            {
                foreach (KeyValuePair<ShoppingCartProduct, int> productItem in productList)
                {
                    for (int i = 0; i < productItem.Value; i++)
                    {
                        if (productItem.Key.ProductType == ShoppingCartProductType.Upsell)
                            res += ",product_" + productItem.Key.ProductID;
                        if (productItem.Key.ProductType == ShoppingCartProductType.Subscription)
                            res += ",subscription_" + productItem.Key.ProductID.ToString();
                    }
                }
            }

            return res.TrimStart(',');
        }

        private static IEnumerable<KeyValuePair<ShoppingCartProduct, int>> DeserializeProductListFromCookie()
        {
            return DeserializeProductList((
                HttpContext.Current.Request.Cookies[SHOPPING_CART_COOKIE] != null) ?
                HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[SHOPPING_CART_COOKIE].Value) :
                string.Empty);
        }

        private static void SerializeProductListToCookie(IEnumerable<KeyValuePair<ShoppingCartProduct, int>> productList)
        {
            HttpContext.Current.Response.Cookies[SHOPPING_CART_COOKIE].Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Response.Cookies[SHOPPING_CART_COOKIE].Value = HttpUtility.UrlEncode(SerializeProductList(productList));
            HttpContext.Current.Response.Cookies[SHOPPING_CART_COOKIE].Path = "/";
        }

        private const string SESSION_SALE_KEY = "sale";
        public void SaveOrder(ComplexSaleView sale)
        {
            if (HttpContext.Current.Handler is PageX)
            {
                HttpSessionState session = ((PageX)HttpContext.Current.Handler).Session;
                if (session != null)
                {
                    session[SESSION_SALE_KEY] = sale;
                }
            }
        }

        public ComplexSaleView LoadOrder()
        {
            if (HttpContext.Current.Handler is PageX)
            {
                HttpSessionState session = ((PageX)HttpContext.Current.Handler).Session;
                if (session != null)
                {
                    return (ComplexSaleView)session[SESSION_SALE_KEY];
                }
            }
            return null;
        }
    }
}
