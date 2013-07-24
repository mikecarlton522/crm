using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Dao;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductMenu : System.Web.UI.Page
    {
        protected List<Product> Products
        {
            get
            {
                return new ProductService().GetProductList();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }
    }
}
