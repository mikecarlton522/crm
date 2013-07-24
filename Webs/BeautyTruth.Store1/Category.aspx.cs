using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using BeautyTruth.Store1.Logic;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace BeautyTruth.Store1
{
    public partial class Category : PageX
    {
        List<KeyValuePair<string, string>> products = null;
        WebStoreService webStoreService = new WebStoreService();
        protected int CurrentPage
        {
            get
            {
                return Utility.TryGetInt(Request["page"]) ?? 1;
            }
        }
        protected int? CategoryID
        {
            get
            {
                return Utility.TryGetInt(Request["categoryID"]);
            }
        }
        protected int SortType
        {
            get
            {
                return Utility.TryGetInt(dpdSort.SelectedValue) ?? 0;
            }
        }
        protected bool ViewAll
        {
            get
            {
                return string.IsNullOrEmpty(Request["viewAll"]) ? false : true;
            }
        }
        protected string CategoryName
        {
            get
            {
                if (CategoryID != null)
                    return webStoreService.GetCategoryNameByID(CategoryID);
                else
                    return "All";
            }
        }

        protected List<KeyValuePair<string, string>> Products
        {
            get
            {
                return products;
            }
        }

        protected List<ProductCodeInfo> productInfoList
        {
            get
            {
                if (CategoryID == null)
                    return webStoreService.GetAllProductCodeInfoByCampaignID(GeneralInfo.CampaignID);
                else
                    return webStoreService.GetAllProductCodeInfoByCategoryID(CategoryID, GeneralInfo.CampaignID);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["sort"]))
                    dpdSort.SelectedValue = Request["sort"];
            }
            Sort();
        }

        protected void dpdSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sort();
            DataBind();
        }

        private void Sort()
        {
            IEnumerable<ProductCodeInfo> sorted = null;
            switch (Convert.ToInt32(dpdSort.SelectedValue))
            {
                case 2:
                    sorted = productInfoList.Reverse<ProductCodeInfo>();
                    break;
                case 3:
                    sorted = from product in productInfoList
                             orderby product.RetailPrice descending
                             select product;
                    break;
                case 4:
                    sorted = from product in productInfoList
                             orderby product.RetailPrice 
                             select product;
                    break;
                case 5:
                    sorted = from product in productInfoList
                             orderby product.Title
                             select product;
                    break;
                case 6:
                    sorted = from product in productInfoList
                             orderby product.Title descending
                             select product;
                    break;
                default:
                    sorted = productInfoList;
                    break;
            }
            products = sorted.Select((u, i) => new KeyValuePair<string, string>(u.ProductCode_, GetAdditionStringByIndex(i)))
                        .ToList();
        }

        private int? GetProductCodeID(string productCode)
        {
            return webStoreService.GetProductCodeID(productCode);
        }
    }
}
