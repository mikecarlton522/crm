using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using BeautyTruth.Store1.Logic;
using TrimFuel.Model.Views;

namespace BeautyTruth.Store1.Controls
{
    public partial class CategoryMenu : System.Web.UI.UserControl
    {
        WebStoreService webStoreService = new WebStoreService();
        public Dictionary<ProductCategory, List<ProductCodeInfo>> CategoriesEx { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var categories = webStoreService.GetAllWebStoreCategories(GeneralInfo.CampaignID);
            CategoriesEx = categories
                .Select(u => new KeyValuePair<ProductCategory, List<ProductCodeInfo>>(u, webStoreService.GetAllProductCodeInfoByCategoryID(u.ProductCategoryID, GeneralInfo.CampaignID)
                                                                                                                    .ToList()))
                .ToDictionary(u => u.Key, u => u.Value);

            if (!IsPostBack)
                DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }
    }
}