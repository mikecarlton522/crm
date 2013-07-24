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
    public partial class ClientProductCodeEdit : BaseControlPage
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

        protected int? ProductCodeID
        {
            get
            {
                return (Utility.TryGetInt(hdnProductCodeID.Value)) ?? Utility.TryGetInt(Request["productCodeID"]);
            }
        }

        protected ProductCode ProductCode { get; set; }

        protected List<Inventory> InventoryList
        {
            get
            {
                return new TPClientService().GetInventoryList(TPClientID.Value).ToList();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (ProductCodeID != null)
            {
                ProductCode = (new TPClientService()).GetProductCode(TPClientID.Value, ProductCodeID.Value);
                rProductCodeInventories.DataSource = (new TPClientService()).GetProductCodeInventoryList(TPClientID.Value, ProductCodeID.Value);
            }
            else
            {
                ProductCode = new ProductCode();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // get config values from form
            List<ProductCodeInventory> lst = new List<ProductCodeInventory>();

            string[] lstInventoryID = Request.Form.GetValues("inventoryID");
            string[] lstQuantity = Request.Form.GetValues("quantity");

            if (lstInventoryID.Length != lstQuantity.Length)
                return;

            for (int i = 0; i < lstInventoryID.Length; i++)
            {
                lst.Add(new ProductCodeInventory()
                    {
                        InventoryID = Utility.TryGetInt(lstInventoryID[i]),
                        Quantity = Utility.TryGetInt(lstQuantity[i])
                    });
            }
            ProductCode productCode = new TPClientService().SaveProductCode(TPClientID.Value, lst, ProductCodeID, tbProductCode.Text);
            hdnProductCodeID.Value = productCode.ProductCodeID.ToString();
            lSaved.Visible = true;
            DataBind();
        }
    }
}
