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
    public partial class ProductDocumentation : System.Web.UI.Page
    {
        ProductService ser = new ProductService();

        protected string DocLink
        {
            get
            {
                var prWiki = ser.GetDocLink(ProductID);
                if (prWiki != null)
                    return prWiki.Path;
                else
                    return null;
            }
        }

        protected int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productId"]) == null ? Utility.TryGetInt(hdnProductID.Value) : Utility.TryGetInt(Request["productId"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProductID == null)
                return;
            DataBind();
        }

        protected void doc_Delete(object sender, EventArgs e)
        {
            ser.DeleteDocLink(ProductID);
            Note.Text = "Successfully removed";
            DataBind();
        }

        protected void doc_Save(object sender, EventArgs e)
        {
            ser.SaveProductWiki(new ProductWiki() { Path = Request["txtDocLink"], ProductID = ProductID });
            Note.Text = "Successfully updated";
            DataBind();
        }
    }
}
