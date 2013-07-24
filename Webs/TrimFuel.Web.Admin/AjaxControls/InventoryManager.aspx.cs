using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class InventoryManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected void rInventoryList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteInventory")
            {
                int inventoryID = Convert.ToInt32(e.CommandArgument);
                var res = new InventoryService().DeleteInventory(inventoryID);
                if (res.State == BusinessErrorState.Error)
                {
                    Error1.Show(res.ErrorMessage, BusinessErrorState.Error);
                }
                else
                {
                    Error1.Show("Inventory was successfully deleted", BusinessErrorState.Success);
                }
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            var list = new InventoryService().GetInventoryList();
            rInventoryList.DataSource = list;
            if (list.Count > 0)
            {
                phNoRecords.Visible = false;
            }
            else
            {
                phNoRecords.Visible = true;
            }
        }

        protected string ShowPrice(object price)
        {
            decimal? amount = (decimal?)price;
            return Utility.FormatCurrency(amount, "$");
        }
    }
}