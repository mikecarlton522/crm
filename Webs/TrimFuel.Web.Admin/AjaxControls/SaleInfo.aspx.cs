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
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SaleInfo : System.Web.UI.Page
    {
        private OrderService service = new OrderService();
        private SaleFlow saleFlow = new SaleFlow();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
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
                Sale = service.LoadSale(SaleID.Value);
                ChargebackReasonCodeDDL1.PaymentTypeID = PaymentTypeID;
            }
        }

        protected void btnUpdateChargeback_Click(object sender, EventArgs e)
        {
            if (saleFlow.SetChargeback(SaleID.Value,
                Utility.TryGetInt(ChargebackStatusDDL1.SelectedValue), Utility.TryGetInt(ChargebackReasonCodeDDL1.SelectedValue),
                tbChargebackCase.Text, tbChargebackARN.Text,
                Utility.TryGetDate(tbPostDate.Text), Utility.TryGetDate(tbDisputeDate.Text)))
            {
                Error2.Show();
            }
            else
            {
                Error1.Show();
            }
            DataBind();
        }

        protected OrderSaleView2 Sale { get; set; }
        protected ChargeHistoryView Charge
        {
            get
            {
                return Sale.InvoiceChargeList.FirstOrDefault(i => i.ChargeHistory.Success == true && i.ChargeHistory.Amount > 0M);
            }
        }
        protected int PaymentTypeID
        {
            get
            {
                var billing = service.Load<Billing>(Sale.SaleView.Order.Order.BillingID);
                int? retVal = null;
                if (billing != null)
                    retVal = billing.CreditCardCnt.TryGetCardType();
                if (Charge != null && Charge.ChargeHistory != null)
                {
                    var chargeCreditCard = service.Load<ChargeHistoryCard>(Charge.ChargeHistory.ChargeHistoryID);
                    if (chargeCreditCard != null)
                        retVal = chargeCreditCard.PaymentTypeID;
                }
                return retVal ?? PaymentTypeEnum.Visa;
            }
        }
    }
}