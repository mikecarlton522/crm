using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientProductEdit : BaseControlPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lSaved.Visible = false;
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected int? ProductID
        {
            get
            {
                return (Utility.TryGetInt(hdnProductID.Value)) ?? Utility.TryGetInt(Request["ProductID"]);
            }
        }

        protected Product Product { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (ProductID != null)
            {
                Product = (new TPClientService()).GetProduct(TPClientID.Value, ProductID.Value);
            }
            else
            {
                Product = new Product();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Product = (new TPClientService()).SaveProduct(TPClientID.Value, ProductID, tbProductName.Text);
            hdnProductID.Value = Product.ProductID.ToString();
            lSaved.Visible = true;
            DataBind();
        }
    }
}
