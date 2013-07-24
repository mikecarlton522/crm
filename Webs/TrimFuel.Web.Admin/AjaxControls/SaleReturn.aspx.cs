using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Flow;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SaleReturn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? SaleID
        {
            get { return Utility.TryGetLong(Request["saleId"]); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            ifSaleExists.Condition = false;
            ifSaleDoesntExist.Condition = true;

            if (SaleID != null)
            {
                ifSaleExists.Condition = true;
                ifSaleDoesntExist.Condition = false;
            }
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            SaleFlow saleServ = new SaleFlow();
            var res = saleServ.ReturnSale(
                    SaleID.Value, ddlReason.SelectedValue + ". " + Utility.TryGetStr(tbNotes.Text),
                    DateTime.Now,
                    Logic.AdminMembership.CurrentAdmin.AdminID.Value);
            if (res.State == Business.BusinessErrorState.Success)
            {
                Error2.Show("Return Processed.<br/>" + res.ErrorMessage);
            }
            else
            {
                Error1.Show(res.ErrorMessage);
            }
        }
    }
}