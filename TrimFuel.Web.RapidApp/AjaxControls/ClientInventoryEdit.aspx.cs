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
    public partial class ClientInventoryEdit : BaseControlPage
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

        protected int? InventoryID
        {
            get
            {
                return (Utility.TryGetInt(hdnInventoryID.Value)) ?? Utility.TryGetInt(Request["InventoryID"]);
            }
        }

        protected Inventory Inventory { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (InventoryID != null)
            {
                Inventory = (new TPClientService()).GetInventory(TPClientID.Value, InventoryID.Value);
            }
            else
            {
                Inventory = new Inventory();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Inventory = (new TPClientService()).SaveInventory(TPClientID.Value, InventoryID, tbSKU.Text, tbProduct.Text);
            lSaved.Visible = true;
            hdnInventoryID.Value = Inventory.InventoryID.ToString();
            DataBind();
        }
    }
}
