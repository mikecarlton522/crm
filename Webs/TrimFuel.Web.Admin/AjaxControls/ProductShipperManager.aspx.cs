using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductShipperManager : System.Web.UI.Page
    {
        protected List<int> ActiveShippersID = null;

        protected List<Shipper> Shippers = null;

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
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            var ps = new ProductService();
            Shippers = ps.GetShippers().Where(u => u.ServiceIsActive).ToList();
            ActiveShippersID = Shippers.Select(u => u.ShipperID).ToList();
            Shippers.Insert(0, new Shipper() { Name = "--Not Set--", ShipperID = 0 });
            rShipProd.DataSource =  ps.GetShipperProductList(ProductID.Value);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<ShipperProduct> shipperProductList = new List<ShipperProduct>();

            foreach (RepeaterItem rItem in rShipProd.Controls)
            {
                var dpDown = rItem.Controls[1] as DropDownList;
                shipperProductList.Add(new ShipperProduct()
                {
                    ProductID = ProductID,
                    ShipperID = Utility.TryGetInt(dpDown.SelectedValue),
                    NeedConfirm = true
                });
            }
            new ProductService().SaveShipperProducts(shipperProductList);
            Note.Text = "Shipper was successfuly updated";
            DataBind();
        }
    }
}
