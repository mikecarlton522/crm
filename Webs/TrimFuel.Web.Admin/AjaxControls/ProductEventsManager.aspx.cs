using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductEventsManager : System.Web.UI.Page
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

        protected List<TrimFuel.Model.ProductEvent> ProductEvents
        {
            get
            {
                return ps.GetProductEvents(ProductID).ToList();
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

        protected void rProductEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                ps.DeleteProductEvent(Convert.ToInt32(e.CommandArgument));
                Note.Text = "Event was successfuly removed";
            }
            DataBind();
        }

        protected void rProductEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (rProductEvents.Items.Count < 1)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    Label lblFooter = (Label)e.Item.FindControl("lblEmptyData");
                    lblFooter.Visible = true;
                }
            }
        }
    }
}
