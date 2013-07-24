using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Dao.EntityDataProviders;
using TrimFuel.Model;
using TrimFuel.Web.Admin.Magento;
using log4net;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SyncWithWebstore : System.Web.UI.Page
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
            LabelOK.Text = "";
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
                MagentoList = Service.catalogProductList(SessionId, new filters(), storeView);
                IList<Inventory> toRemoveInventory = new List<Inventory>();
                foreach (Inventory inventory in InventoryList)
                {
                    foreach (catalogProductEntity catalogProductEntity in MagentoList)
                    {
                        if (catalogProductEntity.sku == inventory.SKU)
                        {
                            toRemoveInventory.Add(inventory);
                            break;
                        }
                    }
                }

                IList<catalogProductEntity> toRemoveMagento = new List<catalogProductEntity>();
                foreach (catalogProductEntity catalogProductEntity in MagentoList)
                {
                    foreach (Inventory inventory in InventoryList)
                    {
                        if (inventory.SKU == catalogProductEntity.sku)
                        {
                            toRemoveMagento.Add(catalogProductEntity);
                            break;
                        }
                    }
                }

                foreach (Inventory inventory in toRemoveInventory)
                {
                    InventoryList.Remove(inventory);
                }

                catalogProductEntity[] temp = new catalogProductEntity[MagentoList.Count() - toRemoveMagento.Count];
                for (int i = 0, j = 0; i < MagentoList.Count(); i++)
                {
                    if (!toRemoveMagento.Contains(MagentoList[i]))
                    {
                        temp[j++] = MagentoList[i];
                    }
                }

                MagentoList = temp;

                foreach (Inventory inventory in InventoryList)
                {
                    ToDeleteCheckBoxList.Items.Add(new ListItem(inventory.Product, inventory.InventoryID.ToString()));
                }

                foreach (catalogProductEntity catalogProductEntity in MagentoList)
                {
                    ToAddCheckBoxList.Items.Add(new ListItem(catalogProductEntity.name, catalogProductEntity.sku));
                }

                if(ToDeleteCheckBoxList.Items.Count >0)
                {
                    ToDeleteLabel.Text = "To Delete";
                }

                if (ToAddCheckBoxList.Items.Count > 0)
                {
                    ToAddLabel.Text = "To Add";
                }
            }catch(Exception exception)
            {
                logger.Error(GetType(), exception);
            }
        }

        protected void ConfirmButton_Click(object sender, EventArgs e)
        {
                       
            InventoryService service = new InventoryService();
            for (int i = 0; i < ToAddCheckBoxList.Items.Count; i++)
            {
                if (ToAddCheckBoxList.Items[i].Selected)
                {
                    string sku = ToAddCheckBoxList.Items[i].Value;
                    string product = ToAddCheckBoxList.Items[i].Text;
                    catalogProductReturnEntity entity = Service.catalogProductInfo(SessionId, sku, string.Empty, null, null);
                    decimal? cost = Convert.ToDecimal(entity.price);
                    decimal? retailPrice = 0;
                    service.CreateInventoryWithProduct(sku, product, 0, cost, retailPrice, null, null);
                }
            }

            for (int i = 0; i < ToDeleteCheckBoxList.Items.Count; i++)
            {
                if (ToDeleteCheckBoxList.Items[i].Selected)
                {
                    int inventoryID = Convert.ToInt32(ToDeleteCheckBoxList.Items[i].Value);
                    service.DeleteInventory(inventoryID);
                }
            }
            LabelOK.Text = "Inventies were successfuly updated";
            ToDeleteCheckBoxList.Items.Clear();
            ToAddCheckBoxList.Items.Clear();
        }
    }
}
