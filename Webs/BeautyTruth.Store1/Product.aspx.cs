using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using BeautyTruth.Store1.Logic;
using TrimFuel.Business;
using TrimFuel.Model;
using System.Text.RegularExpressions;

namespace BeautyTruth.Store1
{
    public partial class Product : PageX
    {
        WebStoreService webStoreService = new WebStoreService();
        public string ProductTitle { get; set; }
        public string Description { get; set; }
        public string ProductCode { get { return Request["productCode"]; } }
        public string Photo { get; set; }
        public string LargePhoto { get; set; }
        protected int? CategoryID
        {
            get
            {
                return (new WebStoreService().GetCategoryByProductCode(ProductCode) ?? new ProductCategory()).ProductCategoryID;
            }
        }
        protected int? WebStoreProductID
        {
            get
            {
                return (new WebStoreService().GetWebStoreProductsByProductCode(ProductCode) ?? new WebStoreProduct()).WebStoreProductID;
            }
        }
        protected string CategoryName
        {
            get
            {
                return new WebStoreService().GetCategoryNameByID(CategoryID);
            }
        }
        protected decimal ProductCodeRetailPrice { get; set; }
        protected Dictionary<string, string> SalesContext { get; set; }

        protected List<TrimFuel.Model.ProductCode> Products
        {
            get
            {
                return new ProductService().GetProductCodeList().Where(u => u.ProductCode_ == ProductCode).ToList();
            }
        }

        Subscription _subscription = null;
        protected Subscription Subscription
        {
            get
            {
                if (_subscription == null)
                {
                    var webStoreProduct = webStoreService.GetWebStoreProductsByProductCode(ProductCode);
                    if (webStoreProduct != null)
                        if (webStoreProduct.SubscriptionID != null)
                            _subscription = webStoreService.Load<Subscription>(webStoreProduct.SubscriptionID);
                }
                return _subscription ?? new Subscription();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            SalesContext = new Dictionary<string, string>();
            ProductTitle = "&nbsp;";
            Photo = ShoppingCart.DASHBOARD_PATH + "images/empty.png";
            LargePhoto = ShoppingCart.DASHBOARD_PATH + "images/empty.png";
            var product = new ProductService().GetProductCodeInfo(ProductCode);
            if (product != null)
            {
                ProductTitle = product.Title;
                Description = product.Description;
                ProductCodeRetailPrice = product.RetailPrice ?? 0;
                if (!string.IsNullOrEmpty(product.Photo))
                    Photo = ShoppingCart.PHOTO_PATH + product.Photo;
                if (!string.IsNullOrEmpty(product.LargePhoto))
                    LargePhoto = ShoppingCart.PHOTO_PATH + product.LargePhoto;

                GetSalesContextDict(product.SalesContext);
            }
        }

        private void GetSalesContextDict(string saleContext)
        {
            if (string.IsNullOrEmpty(saleContext))
                return;

            int h1pos = 0;
            do
            {
                h1pos = saleContext.IndexOf("<h1>", h1pos);
                if (h1pos >= 0)
                {
                    h1pos = h1pos + 4;
                    var endh1pos = saleContext.IndexOf("</h1>", h1pos);
                    string key = saleContext.Substring(h1pos, endh1pos - h1pos);

                    string value = string.Empty;
                    if (endh1pos != saleContext.Length - 5)
                    {
                        int nexth1pos = saleContext.IndexOf("<h1>", h1pos);
                        if (nexth1pos < 0)
                            nexth1pos = saleContext.Length;
                        value = saleContext.Substring(endh1pos + 5, nexth1pos - endh1pos - 5);
                    }
                    SalesContext.Add(key, value);
                }
            }
            while (h1pos >= 0);
        }
    }
}
