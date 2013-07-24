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
    public partial class ProductInventoryManager : System.Web.UI.Page
    {
        ProductService ps = new ProductService();
        List<Product> products = null;

        protected List<Product> Products
        {
            get
            {
                if (products == null)
                    products = ps.GetProductList();
                return products;
            }
        }

        protected List<Inventory> InventoryList
        {
            get
            {
                return ps.GetInventoryList().ToList();
            }
        }

        protected List<ProductInventory> ProductInventoryList
        {
            get
            {
                return ps.GetProductInventoryList().Where(u => u.ProductID == ProductID).ToList();
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

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            List<int?> invList = new List<int?>();

            foreach(RepeaterItem item in rInventory.Items)
            {
                var hdnInvID = item.Controls[1] as HiddenField;
                var chb = item.Controls[3] as CheckBox;
                if (chb.Checked)
                    invList.Add(Utility.TryGetInt(hdnInvID.Value));
            }

            ps.SaveProductInventoryList(invList, ProductID);

            Note.Text = "Product was successfuly updated";
            DataBind();
        }
    }
}