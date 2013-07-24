using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Dao.EntityDataProviders;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Web.Admin.Magento;
using log4net;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SyncCategories : System.Web.UI.Page
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));
        private MagentoService _service = null;
        protected MagentoService Service
        {
            get {
                if (_service == null)
                {
                    _service = new MagentoService();
                    _service.Url = (new MagentoLocalService()).GetMagentoConfig().MagentoURL;
                }
                return _service;
            }
        }

        private string _sessionId = null;
        protected string SessionId
        {
            get
            {
                if (_sessionId == null)
                {
                    _sessionId = Service.login((new MagentoLocalService()).GetMagentoConfig().User,
                                               (new MagentoLocalService()).GetMagentoConfig().Password);
                }
                return _sessionId;
            }
        }

        private IList<Inventory> _inventoryList;
        protected IList<Inventory> InventoryList
        {
            get { return _inventoryList ?? (_inventoryList = new List<Inventory>()); }
            set { _inventoryList = value; }
        }

        protected catalogProductEntity[] MagentoList { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            try
            {
                base.OnDataBinding(e);

                InventoryList =
                    (new SubscriptionService()).GetInventoryList().Where(i => i.InventoryID > 0).ToList();
                string storeView = string.Empty;
                string[] args = {"name"};
                MagentoList = Service.catalogProductList(SessionId, new filters(), storeView);
                IList<int> categoriesList = new List<int>();
                foreach (catalogProductEntity entity in MagentoList)
                {
                    if (entity.category_ids.Count() != 0)
                    {
                        int categoryId = Convert.ToInt32(entity.category_ids[0]);
                        if (!categoriesList.Contains(categoryId))
                        {
                            categoriesList.Add(categoryId);
                        }
                    }
                }

                IList<catalogCategoryInfo> categoryInfoList = new List<catalogCategoryInfo>();
                foreach (int categoryId in categoriesList)
                {
                    catalogCategoryInfo categoryInfo = Service.catalogCategoryInfo(SessionId,
                                                                                   categoryId,
                                                                                   storeView,
                                                                                   args);
                    categoryInfoList.Add(categoryInfo);
                }

                IList<MagentoProductCategoryView> magentoProductCategoryViews =
                    (new MagentoLocalService()).GetMagentoProductCategoryViewList();



                var magentoShopList = from magento in MagentoList where magento.category_ids.Count() > 0  
                            join categoryInfo in categoryInfoList on magento.category_ids[0] equals categoryInfo.category_id into tempList
                            from categoryInfo in tempList.DefaultIfEmpty()
                            select new {magento.sku, cat = magento.category_ids[0], categoryInfo.name};

                var list = from inventory in InventoryList join mpcv in magentoProductCategoryViews on inventory.InventoryID equals mpcv.InventoryID
                           join magentoShop in magentoShopList on inventory.SKU equals magentoShop.sku
                           where mpcv.CategoryName != magentoShop.name
                           select new {product = inventory.Product, id = inventory.InventoryID, oldCategory = mpcv.CategoryName, newCategory = magentoShop.name};

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id");
                dataTable.Columns.Add("product");
                dataTable.Columns.Add("oldCategory");
                dataTable.Columns.Add("newCategory");
                foreach(var item in list)
                {
                    dataTable.Rows.Add(item.id, item.product, item.oldCategory, item.newCategory);
                }
                if (dataTable.Rows.Count > 0)
                {
                    rCategories.DataSource = dataTable;
                    rCategories.DataBind();
                    ConfirmButton.Visible = true;
                    LabelNoItems.Visible = false;
                }
                else
                {
                    rCategories.Visible = false;
                    LabelNoItems.Text = "No alerts right now";
                    LabelNoItems.Visible = true;
                    ConfirmButton.Visible = false;
                }

            }
            catch (Exception exception)
            {
                logger.Error(GetType(), exception);
            }
        }

        protected void rCategories_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ((CheckBox) e.Item.FindControl("checkBox")).Attributes.Add("id",
                                                                           ((DataRowView) e.Item.DataItem)["id"].
                                                                               ToString());
                ((CheckBox) e.Item.FindControl("checkBox")).Attributes.Add("newCategory",
                                                                           ((DataRowView)e.Item.DataItem)[
                                                                               "newCategory"].ToString());
            }
        }

        protected void ConfirmButton_Click(object sender, EventArgs e)
        {
            MagentoLocalService magentoLocalService = new MagentoLocalService();
            foreach (RepeaterItem rItem in rCategories.Items)
            {
                CheckBox checkBox = rItem.FindControl("checkBox") as CheckBox;
                if (checkBox.Checked)
                {
                    int id = Convert.ToInt32(checkBox.Attributes["id"]);
                    string categoryName = checkBox.Attributes["newCategory"];
                    magentoLocalService.SaveMagentoProductCategory(id, categoryName);
                }
            }
            DataBind();
        }
    }
}
