using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.services
{
    public partial class UnsentShipmentsGroup : System.Web.UI.Page
    {
        SaleService saleService = new SaleService();

        protected int? ShipperID
        {
            get
            {
                return Utility.TryGetInt(Request["shipperID"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["action"] == "save")
            {
                //Trying Save new SKU
                var saleID = Utility.TryGetLong(Request["editSaleID"]);
                var newSKU = Request["editProductSKUDDL"];
                if (!string.IsNullOrEmpty(newSKU) && saleID != null)
                    if (saleService.ChangeSKU(saleID.Value, newSKU))
                    {
                        Response.Write(newSKU);
                        Response.End();
                    }
                    else
                        Response.Write("Unknown Error");
            }
            else
            {
                //load shipments list by ShipperID
                if (ShipperID == null)
                    return;
                DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rShipments.DataSource = new ReportService().GetUnsentShippmentsFromAggTable(ShipperID);
        }
    }
}