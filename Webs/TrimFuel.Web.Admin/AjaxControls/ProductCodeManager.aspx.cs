using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class ProductCodeManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }
        protected void rProductCodeList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteProductCode")
            {
                int productCodeID = Convert.ToInt32(e.CommandArgument);
                var res = new InventoryService().DeleteProductCode(productCodeID);
                if (res.State == BusinessErrorState.Error)
                {
                    Error1.Show(res.ErrorMessage, BusinessErrorState.Error);
                }
                else
                {
                    Error1.Show("Product was successfully deleted", BusinessErrorState.Success);
                }
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            var list = new InventoryService().GetProductCodeList();
            rProductCodeList.DataSource = list;
            if (list.Count > 0)
            {
                phNoRecords.Visible = false;
            }
            else
            {
                phNoRecords.Visible = true;
            }
        }
    }
}