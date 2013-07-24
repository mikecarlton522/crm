using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using System.Web.SessionState;
using TrimFuel.Business;
using TrimFuel.Model;

namespace Kaboom.Store1.Logic
{
    public enum KnownProduct
    {
        KaboomCombo_1x2_30_Trial = 1,
        KaboomCombo_1x12_60_Upsell = 2,
        KaboomStrips_1x12_Upsell = 3,
        KaboomDaily_1x60_Upsell = 4,
        KaboomDaily_1x30_Upsell = 5
    }

    public class ShoppingCart
    {
        private const string SHOPPING_CART_COOKIE = "shoppingCart";
        private const int CAMPAIGN_ID = 266;

        public static Dictionary<KnownProduct, ShoppingCartProduct> KnownProducts = new Dictionary<KnownProduct, ShoppingCartProduct>(){
            {KnownProduct.KaboomCombo_1x2_30_Trial, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Subscription, ProductID = 304, Price = 9.99M}},
            {KnownProduct.KaboomCombo_1x12_60_Upsell, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Upsell, ProductID = 103, Price = 79.99M}},
            {KnownProduct.KaboomStrips_1x12_Upsell, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Upsell, ProductID = 104, Price = 59.99M}},
            {KnownProduct.KaboomDaily_1x60_Upsell, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Upsell, ProductID = 105, Price = 49.99M}},
            {KnownProduct.KaboomDaily_1x30_Upsell, new ShoppingCartProduct(){ProductType = ShoppingCartProductType.Upsell, ProductID = 106, Price = 34.99M}}
        };

        public IEnumerable<KeyValuePair<ShoppingCartProduct, int>> Products { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public string ClickID { get; set; }
        //public long? FailedBillingID { get; set; }
        public string IP { get; set; }
        public string Url { get; set; }
        public string CouponCode { get; set; }
        public string RefererCode { get; set; }
        public int? CampaignID { get; set; }

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

        public int GetProductQuantity(KnownProduct product)
        {
            int count = 0;
            foreach (KeyValuePair<ShoppingCartProduct, int> p in Products)
            {
                KnownProduct prod = ShoppingCart.GetProductNumber(p.Key);
                if (prod == product)
                {
                    count = count + p.Value;
                }
            }
            return count;
        }

        private bool isLoaded = false;
        public void Load()
        {
            if (!isLoaded)
            {
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

                if (RefererCode == null && HttpContext.Current.Request.Cookies["RefererCode"] != null)
                    RefererCode = Utility.TryGetStr(HttpContext.Current.Request.Cookies["RefererCode"].Value);

                CampaignID = CAMPAIGN_ID;

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
            SaveCookie("RefererCode", RefererCode, DateTime.Now.AddDays(7));
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

        public static KnownProduct GetProductNumber(ShoppingCartProduct product)
        {
            return KnownProducts.First(item => item.Value == product).Key;
        }

        public static ShoppingCartProduct GetKnownProduct(ShoppingCartProductType productType, int productID)
        {
            return KnownProducts.FirstOrDefault(item => item.Value.ProductType == productType && item.Value.ProductID == productID).Value;
        }

        public static ShoppingCartProduct DeserializeProduct(string productString)
        {
            int enumValue = 0;
            if (int.TryParse(productString, out enumValue) &&
                Enum.IsDefined(typeof(KnownProduct), enumValue))
            {
                return KnownProducts[(KnownProduct)enumValue];
            }
            return null;
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
                        if (res.ContainsKey(product))
                        {
                            res[product]++;
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
                        res += "," + ((int)GetProductNumber(productItem.Key)).ToString();
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
