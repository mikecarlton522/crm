using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Business.Flow;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class SaleShipments : System.Web.UI.UserControl
    {
        private OrderShipmentService service = new OrderShipmentService();

        protected string TrackingNumber
        {
            get
            {
                return new SaleFlow().GetTrackingNumberBySale(SaleID);
            }
        }

        protected bool IsBlockingAllowed
        {
            get
            {
                return new SaleFlow().IsBlockingAllowed(SaleID);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SaleID == null)
                SaleID = Utility.TryGetLong(hdnSaleID.Value);
        }

        public long? SaleID { get; set; }
        protected IList<ShippingEventView> RequestList { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (SaleID != null)
            {
                RequestList = service.GetShippingHistory(SaleID.Value).OrderByDescending(i => i.CreateDT.Value).ToList();
                rRequestList.DataSource = RequestList;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (new SaleFlow().UpdateTrackingNumber(SaleID, txtTrackingNumber.Text, Utility.TryGetInt(ShipperDDL1.SelectedValue), Logic.AdminMembership.CurrentAdmin.DisplayName))
            {
                Error1.Show("Tracking number updated", BusinessErrorState.Success);
            }
            else
            {
                Error1.Show("Error when updating Tracking number", BusinessErrorState.Error);
            }
            DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (new SaleFlow().BlockShipments(SaleID))
            {
                Error1.Show("Shipment cancelled", BusinessErrorState.Success);
                btnCancelShipment.Visible = false;
            }
            else
            {
                Error1.Show("Error when cancelling Shipment", BusinessErrorState.Error);
            }
            DataBind();
        }
    }
}