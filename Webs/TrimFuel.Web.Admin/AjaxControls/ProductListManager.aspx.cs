using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductListManager : System.Web.UI.Page
    {
        //Dictionary<string, Dictionary<ProductCode, bool>> groupList = null;
        List<int?> _ecigGroup = new List<int?>() { 10, 14, 24, 15, 22, 20 };
        ProductService ps = new ProductService();

        List<Product> products = null;
        List<ProductCode> productCodes = null;
        List<ProductProductCode> productProductCodes = null;
        List<ProductCodeInfo> productCodeInfoList = null;
        string currencySymbol = null;

        string CurrencySymbol
        {
            get
            {
                if (string.IsNullOrEmpty(currencySymbol))
                {
                    var currency = ps.GetProductCurrency(ProductID.Value);
                    if (currency == null)
                        currencySymbol = "$";
                    else
                        currencySymbol = currency.HtmlSymbol;
                }
                return currencySymbol;
            }
        }

        List<Product> Products
        {
            get
            {
                if (products == null)
                    products = ps.GetProductList();
                return products;
            }
        }

        List<ProductProductCode> ProductProductCodes
        {
            get
            {
                if (productProductCodes == null)
                    productProductCodes = ps.GetProductProductCodeList().ToList();
                return productProductCodes;
            }
        }

        List<ProductCodeInfo> ProductCodeInfoList
        {
            get
            {
                if (productCodeInfoList == null)
                    productCodeInfoList = ps.GetProductCodeInfoList().ToList();
                return productCodeInfoList;
            }
        }

        List<ProductCode> ProductCodeList
        {
            get
            {
                if (productCodes == null)
                {
                    productCodes = ps.GetProductCodeList().ToList();
                    foreach (var product in Products.Where(u => u.ProductID != ProductID))
                    {
                        if (!(_ecigGroup.Contains(ProductID) && _ecigGroup.Contains(product.ProductID)))
                        {
                            var toRemove = productCodes.Where(u => IsChecked(u, product.ProductID) == true).ToList();
                            foreach (var itemToRemove in toRemove)
                                productCodes.Remove(itemToRemove);
                        }
                    }
                }
                return productCodes;
            }
        }

        protected int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productId"]) == null ? Utility.TryGetInt(hdnProductID.Value) : Utility.TryGetInt(Request["productId"]);
            }
        }

        protected string ProductName
        {
            get
            {
                return Products.Where(u => u.ProductID == ProductID).SingleOrDefault().ProductName;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            //groupList = new Dictionary<string, Dictionary<ProductCode, bool>>();
            //List<ProductCode> singleProductsList = new List<ProductCode>();

            //int groupIndex = 1;
            //while (ProductCodeList.Count > 0)
            //{
            //    List<ProductCode> dependProdCodeList = new List<ProductCode>();
            //    var currProductCode = ProductCodeList[0];

            //    //get depends productCodes for current ProductCode
            //    FindDependProducts(currProductCode, dependProdCodeList);

            //    if (dependProdCodeList.Count > 1)
            //    {
            //        //create new group
            //        Dictionary<ProductCode, bool> lstToGroup = new Dictionary<ProductCode, bool>();
            //        foreach (var prodCode in dependProdCodeList)
            //            lstToGroup.Add(prodCode, IsChecked(prodCode, CurrProductInventoryIDList));

            //        groupList.Add("Group " + groupIndex.ToString(), lstToGroup);
            //        groupIndex++;
            //    }
            //    else
            //    {
            //        //add as single item
            //        singleProductsList.Add(currProductCode);
            //    }
            //}

            ////for each single item set checked state
            //Dictionary<ProductCode, bool> prodList = new Dictionary<ProductCode, bool>();
            //foreach (var prodCode in singleProductsList)
            //    prodList.Add(prodCode, IsChecked(prodCode, CurrProductInventoryIDList));

            Dictionary<ProductCode, bool> prodList = new Dictionary<ProductCode, bool>();
            foreach (var prodCode in ProductCodeList)
                prodList.Add(prodCode, IsChecked(prodCode, ProductID));

            rProducts.DataSource = prodList;
            //rGroupProducts.DataSource = groupList;
        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            SaveProducts();

            Note.Text = "Product was successfuly updated";
            DataBind();
        }

        protected void btnSavePrice_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem repeaterItem in rProducts.Items)
            {
                var hdnControl = repeaterItem.Controls[1] as HiddenField;
                var txtControl = repeaterItem.Controls[5] as TextBox;

                SaveNewPrice(hdnControl.Value, Utility.TryGetDecimal(txtControl.Text));
            }

            SaveProducts();
            productCodeInfoList = null;

            Note.Text = "Product was successfuly updated";
            DataBind();
        }

        protected string GetRetailPrice(string productCode)
        {
            decimal? res = 0;

            var info = ProductCodeInfoList.Where(u => u.ProductCode_ == productCode).SingleOrDefault();
            if (info != null)
                res = info.RetailPrice;

            return Utility.FormatCurrency(res ?? 0, CurrencySymbol);
        }

        protected string GetRetailPriceWithoutCurrencySymbol(string productCode)
        {
            decimal? res = 0;

            var info = ProductCodeInfoList.Where(u => u.ProductCode_ == productCode).SingleOrDefault();
            if (info != null)
                res = info.RetailPrice;

            return Utility.FormatPrice(res);
        }

        #region Helpers

        private bool IsChecked(ProductCode prodCode, int? productID)
        {
            bool res = true;
            if (ProductProductCodes.Where(u => u.ProductID == productID && u.ProductCodeID == prodCode.ProductCodeID).Count() == 0)
                res = false;
            return res;
        }

        private void SaveNewPrice(string productCode, decimal? newPrice)
        {
            var prInfo = ps.GetProductCodeInfo(productCode);
            var productCodeItem = ProductCodeList.Where(u => u.ProductCode_ == productCode).SingleOrDefault();
            if (prInfo == null)
            {
                prInfo = new ProductCodeInfo();
                prInfo.FillFromProductCode(productCodeItem);
                //we don't need save Name
                prInfo.Name = string.Empty;
                prInfo.Title = string.Empty;
            }
            prInfo.RetailPrice = newPrice;
            ps.Save<ProductCodeInfo>(prInfo);
        }

        private void SaveProducts()
        {
            List<int?> prodCodeList = new List<int?>();

            if (!string.IsNullOrEmpty(Request["prodCodeID"]))
            {
                foreach (var selectedProductCodeID in Request["prodCodeID"].Split(','))
                {
                    prodCodeList.Add(Utility.TryGetInt(selectedProductCodeID));
                }
            }

            ps.SaveProductProductCodeList(prodCodeList, ProductID);

            productProductCodes = null;
        }

        #endregion
    }
}