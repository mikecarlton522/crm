using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientFulfillment : BaseControlPage
    {
        TPClientService tpSer = new TPClientService();

        protected List<Shipper> Shippers = null;

        protected List<int> ActiveShippersID = null;

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

        protected override void OnDataBinding(EventArgs e)
        {
            Shippers = tpSer.GetClientShippers(TPClientID.Value).Where(u => u.ServiceIsActive).ToList();
            ActiveShippersID = Shippers.Select(u => u.ShipperID).ToList();
            Shippers.Insert(0, new Shipper() { Name = "--Not Set--", ShipperID = 0 });

            base.OnDataBinding(e);
            rInventories.DataSource = tpSer.GetInventoryList(TPClientID.Value);
            rProducts.DataSource = tpSer.GetProductCodeList(TPClientID.Value);
            rShipProd.DataSource = tpSer.GetShipperProductList(TPClientID.Value);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<ShipperProduct> shipperProductList = new List<ShipperProduct>();

            foreach (RepeaterItem rItem in rShipProd.Controls)
            {
                var hidden = rItem.Controls[1] as HiddenField;
                var dpDown = rItem.Controls[3] as DropDownList;
                shipperProductList.Add(new ShipperProduct()
                    {
                        ProductID = Utility.TryGetInt(hidden.Value),
                        ShipperID = Utility.TryGetInt(dpDown.SelectedValue),
                        NeedConfirm = true
                    });
            }
            tpSer.SaveShipperProducts(TPClientID.Value, shipperProductList);
            lSaved.Visible = true;
            DataBind();
        }
    }
}
