using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using System.Drawing;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class CreateNewProductGroup : System.Web.UI.Page
    {
        ProductService ps = new ProductService();
        LeadService service = new LeadService();
        protected List<Product> Products
        {
            get
            {
                return new ProductService().GetProductList();
            }
        }
        protected Product ExistProduct
        {
            get;
            set;
        }
        protected Product NewProduct
        {
            get;
            set;
        }
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }
        protected void Create_Click(object sender, EventArgs e)
        {
            ExistProduct = new BaseService().Load<Product>(ddlProductGroup.SelectedValue);
            if (!string.IsNullOrEmpty(tbProductName.Text))
            {
                if(ps.DuplicateProduct(ExistProduct, tbProductName.Text))
                    Note.Text = "Product Group was successfuly added.";
                else
                    Note.Text = "<span style='color: Red;'>Product Group was not added. Unexpected error occured.</span>";
            }
            else
            {
                Note.Text = "<span style='color: Red;'>Product Group was not added. Please provide product name.</span>";
            }
        }        
    }
}