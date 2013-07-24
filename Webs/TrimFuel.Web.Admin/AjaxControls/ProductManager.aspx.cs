using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductManager : System.Web.UI.Page
    {
        protected int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productId"]) == null ? Utility.TryGetInt(hdnProductID.Value) : Utility.TryGetInt(Request["productId"]);
            }
        }

        protected Product ProductProp
        {
            get
            {
                if (ProductID == null)
                    return new Product() { ProductIsActive = true };
                else
                    return new ProductService().GetProduct(ProductID.Value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            Product prodToSave = new BaseService().Load<Product>(this.ProductID);
            if (prodToSave == null)
                prodToSave = new Product();
            prodToSave.ProductIsActive = cbActive.Checked;
            prodToSave.ProductName = tbProductName.Text;

            //prodToSave = new Product()
            //   {
            //       Code = tbCode.Text,
            //       ProductIsActive = cbActive.Checked,
            //       ProductID = this.ProductID,
            //       ProductName = tbProductName.Text
            //   };
            new BaseService().Save<Product>(prodToSave);
            if (this.ProductID == null)
                Note.Text = "Product Group was successfuly added";
            else
                Note.Text = "Product Group was successfuly updated";

            hdnProductID.Value = prodToSave.ProductID.ToString();
        }
    }
}
