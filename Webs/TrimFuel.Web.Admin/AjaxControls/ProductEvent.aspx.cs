using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductEvent : System.Web.UI.Page
    {
        ProductService ps = new ProductService();

        protected Dictionary<string, int> EventTypes = new Dictionary<string, int>()
        {
            {"Registration", 1 },
            {"Order Confirmation", 2 },
            {"Cancellation", 3},
            {"Update", 4}
        };

        protected string ProductName
        {
            get
            {
                return ps.GetProductList().Where(u => u.ProductID == ProductID).SingleOrDefault().ProductName;
            }
        }

        protected int? ProductEventID
        {
            get
            {
                return Utility.TryGetInt(Request["productEventId"]) == null ? Utility.TryGetInt(hdnProductEventID.Value) : Utility.TryGetInt(Request["productEventId"]);
            }
        }

        protected int? ProductID
        {
            get
            {
                return Utility.TryGetInt(Request["productId"]) == null ? Utility.TryGetInt(hdnProductID.Value) : Utility.TryGetInt(Request["productId"]);
            }
        }

        protected TrimFuel.Model.ProductEvent ProductEventProp
        {
            get
            {
                TrimFuel.Model.ProductEvent tmp = null;
                if (ProductEventID != null)
                    tmp = ps.GetProductEvent(ProductEventID);

                if (tmp == null)
                    tmp = new TrimFuel.Model.ProductEvent()
                    {
                        EventTypeID = 1,
                        ProductID = ProductID
                    };
                return tmp;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }

        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            var toSave = new TrimFuel.Model.ProductEvent()
            {
                EventTypeID = Utility.TryGetInt(dpEventTypes.SelectedValue),
                ProductID = ProductID,
                URl = tbURL.Text,
                ProductEventID = ProductEventID
            };
            ps.SaveProductEvent(toSave);
            Note.Text = "Changes was successfuly saved";
            hdnProductEventID.Value = toSave.ProductEventID.ToString();
        }
    }
}
