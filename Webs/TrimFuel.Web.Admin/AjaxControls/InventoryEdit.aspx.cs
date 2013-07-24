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
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class InventoryEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected int? InventoryID 
        {
            get 
            {
                return Utility.TryGetInt(hdnInventoryID.Value) ?? Utility.TryGetInt(Request["InventoryID"]);
            }
            private set
            {
                hdnInventoryID.Value = value.ToString();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            phShippers.Visible = false;

            if (InventoryID != null)
            {
                InventorySKUMappingView inv = new InventoryService().GetInventorySKUMapping(InventoryID.Value);
                if (inv != null)
                {
                    tbSKU.Text = inv.Inventory.SKU;
                    tbProductName.Text = inv.Inventory.Product;
                    tbCost.Text = Utility.FormatPrice(inv.Inventory.Costs);
                    tbQty.Text = (inv.Inventory.InStock == null ? "" : inv.Inventory.InStock.ToString());
                    tbRetailPrice.Text = Utility.FormatPrice(inv.Inventory.RetailPrice);
                    chbDoesNotShip.Checked = !(inv.Inventory.InventoryType == InventoryTypeEnum.Inventory);

                    if (inv.Mapping.Count > 0)
                    {
                        phShippers.Visible = true;
                        rpMapping.DataSource = inv.Mapping;
                    }
                }
            }
            else
            {
                IList<Shipper> shipperList = new ProductService().GetShippers();
                if (shipperList.Count > 0)
                {
                    phShippers.Visible = true;
                    rpMapping.DataSource = shipperList.Where(i => i.ServiceIsActive).Select(i => new Set<Shipper, InventorySKU>() { Value1 = i, Value2 = null }).ToList();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            IList<KeyValuePair<int, string>> selectedShipperSKUs = new List<KeyValuePair<int, string>>();
            foreach (RepeaterItem item in rpMapping.Items)
            {
                TextBox tbSKU = (TextBox)item.FindControl("tbSKU");
                HiddenField hdnShipperID = (HiddenField)item.FindControl("hdnShipperID");
                if (hdnShipperID != null && tbSKU != null)
                {
                    selectedShipperSKUs.Add(new KeyValuePair<int, string>(Utility.TryGetInt(hdnShipperID.Value).Value, tbSKU.Text.Trim()));
                }
            }

            if (InventoryID == null)
            {
                var newInv = new InventoryService().CreateInventoryWithProduct(Utility.TryGetStr(tbSKU.Text),
                    Utility.TryGetStr(tbProductName.Text), Utility.TryGetInt(tbQty.Text) ?? 0,
                    Utility.TryGetDecimal(tbCost.Text), Utility.TryGetDecimal(tbRetailPrice.Text),
                    (!chbDoesNotShip.Checked ? InventoryTypeEnum.Inventory : InventoryTypeEnum.Service),
                    selectedShipperSKUs);
                if (newInv.State == BusinessErrorState.Error)
                {
                    Error1.Show(newInv.ErrorMessage, BusinessErrorState.Error);
                }
                else
                {
                    Error1.Show("Inventory Created", BusinessErrorState.Success);
                    InventoryID = newInv.ReturnValue.InventoryID;
                }
            }
            else
            {
                var inv = new InventoryService().UpdateInventory(InventoryID.Value, 
                    Utility.TryGetStr(tbProductName.Text), Utility.TryGetInt(tbQty.Text) ?? 0,
                    Utility.TryGetDecimal(tbCost.Text), Utility.TryGetDecimal(tbRetailPrice.Text),
                    (!chbDoesNotShip.Checked ? InventoryTypeEnum.Inventory : InventoryTypeEnum.Service),
                    selectedShipperSKUs);
                if (inv == null)
                {
                    Error1.Show("Error", BusinessErrorState.Error);
                }
                else
                {
                    Error1.Show("Inventory Updated", BusinessErrorState.Success);
                }
            }
            DataBind();
        }
    }
}