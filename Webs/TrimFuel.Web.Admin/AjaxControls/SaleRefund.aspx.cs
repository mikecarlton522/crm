using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Business;
using TrimFuel.Business.Flow;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SaleRefund : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected void btnRefund_Click(object sender, EventArgs e)
        {
            decimal? refundAmount = Utility.TryGetDecimal(tbRefundAmount.Text);
            if (refundAmount == null)
            {
                Error1.Show("Invalid refund amount");
            }
            else
            {
                var res = (new SaleFlow()).ProcessRefundOrVoid(SaleID.Value, refundAmount.Value);
                Error1.Show(res.ErrorMessage, res.State);
                if (res.ReturnValue != null)
                {
                    Error1.Text += "<br/>Response: " + res.ReturnValue.ChargeHistory.Response;
                }
                if (res.State == BusinessErrorState.Success)
                {
                    DataBind();
                    tbRefundAmount.Text = "";
                }
            }            
        }

        public long? SaleID
        {
            get
            {
                return Utility.TryGetLong(Request["saleID"]);
            }
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
                Sale = (new OrderService()).LoadSale(SaleID.Value);
            }
        }

        public OrderSaleView2 Sale { get; set; }
        public IList<ChargeHistoryView> InvoiceChargeList 
        {
            get
            {
                return Sale.InvoiceChargeList;
            }
        }
        public IList<ChargeHistoryView> RefundList 
        {
            get
            {
                return InvoiceChargeList.Where(i => i.ChargeHistory.Amount < 0M && i.ChargeHistory.Success == true).ToList();
            }
        }
        public ChargeHistoryView Charge
        {
            get
            {
                return InvoiceChargeList.FirstOrDefault(i => i.ChargeHistory.Success == true && i.ChargeHistory.Amount > 0M);
            }
        }
    }
}