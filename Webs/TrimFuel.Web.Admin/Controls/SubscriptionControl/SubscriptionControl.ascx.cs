using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Flow;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionControl : System.Web.UI.UserControl
    {
        OrderService service = new OrderService();
        SubscriptionNewService serviceS = new SubscriptionNewService();
        SubscriptionPlanService spService = new SubscriptionPlanService();
        PlanFlow planService = new PlanFlow();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string ErrorMessage
        {
            get { return lError.Text; }
            set { lError.Text = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            phError.Visible = ErrorMessage.Length > 0;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GenerateID = "SubscriptionControl_" + Utility.RandomString(new Random(), 5);
        }

        public string GenerateID
        {
            get;
            private set;
        }

        public int? RecurringPlanID
        {
            get
            {
                return Subscription1.SelectedRecurringPlanID;
            }
        }

        public long? OrderRecurringPlanID { get; set; }

        public DateTime? NextBillDate
        {
            get
            {
                return Utility.TryGetDate(tbNextBillDate.Text);
            }
        }

        public int? RecurringStatus
        {
            get
            {
                return Utility.TryGetInt(ddlRecurringStatus.SelectedValue);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (OrderRecurringPlanID != null)
            {
                OrderRecurringPlan op = service.Load<OrderRecurringPlan>(OrderRecurringPlanID);
                if (op != null)
                {
                    RecurringPlanView rp = serviceS.GetPlan(op.RecurringPlanID.Value);
                    Subscription1.RecurringPlan = rp;

                    tbNextBillDate.Text = op.NextCycleDT.ToString();
                    ddlRecurringStatus.SelectedValue = op.RecurringStatus.Value.ToString();

                    DisplayDiscountSection(op);
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (OrderRecurringPlanID != null)
            {
                if (RecurringPlanID != null)
                {
                    OrderRecurringPlan op = service.Load<OrderRecurringPlan>(OrderRecurringPlanID);
                    var res = planService.UpdatePlan(OrderRecurringPlanID.Value, RecurringPlanID.Value, NextBillDate.Value, RecurringStatus.Value);
                    if (res == null)
                    {
                        ErrorMessage = "Error occurred while saving object";
                    }
                    else
                    {
                        ErrorMessage = "Subscription was successfuly updated";
                        LogNotes(op, res);
                    }
                }
                else
                {
                    ErrorMessage = "Please choose Subscription";
                }
            }
            DataBind();
        }

        private void LogNotes(OrderRecurringPlan oldPlan, OrderRecurringPlan newPlan)
        {
            try
            {
                OrderSale sl = service.Load<OrderSale>(oldPlan.SaleID);
                Order o = service.Load<Order>(sl.OrderID);
                int? adminID = (AdminMembership.CurrentAdmin != null ? AdminMembership.CurrentAdmin.AdminID : null);
                DashboardService srv = new DashboardService();

                //Status changed
                if (oldPlan.RecurringStatus.Value != newPlan.RecurringStatus.Value)
                {
                    srv.AddBillingNote(o.BillingID.Value, adminID,
                        string.Format("Status changed from {0} to {1}",
                            RecurringStatusEnum.Name[oldPlan.RecurringStatus.Value],
                            RecurringStatusEnum.Name[newPlan.RecurringStatus.Value]));
                }

                //Next Bill Date changed
                if (oldPlan.NextCycleDT != newPlan.NextCycleDT && newPlan.NextCycleDT != null)
                {
                    string oldDate = (oldPlan.NextCycleDT != null ? " from " + oldPlan.NextCycleDT.Value.ToShortDateString() : "");
                    srv.AddBillingNote(o.BillingID.Value, adminID,
                        string.Format("Next bill date changed{0} to {1}",
                            oldDate,
                            newPlan.NextCycleDT.Value.ToShortDateString()));
                }

                //Plan changed
                if (oldPlan.RecurringPlanID.Value != newPlan.RecurringPlanID.Value)
                {
                    srv.AddBillingNote(o.BillingID.Value, adminID,
                        string.Format("Subscription plan changed from #{0} to #{1}",
                            oldPlan.RecurringPlanID,
                            newPlan.RecurringPlanID));
                }
            }
            catch { }
        }

        private void DisplayDiscountSection(OrderRecurringPlan op)
        {
            phDiscount.Visible = false;
            if (op.DiscountTypeID != null && op.DiscountValue > 0)
            {
                string currencySymbol = "$";
                var orderPlanView = service.GetPlan(OrderRecurringPlanID ?? 0);
                phDiscount.Visible = true;

                if (orderPlanView != null)
                {
                    var cycle = orderPlanView.GetNextCycle();
                    if (cycle != null)
                    {
                        var currency = service.GetCurrencyByOrderReccuringPlan(OrderRecurringPlanID ?? 0);
                        if (currency != null)
                            currencySymbol = currency.HtmlSymbol;
                        var amount = (cycle.Constraint != null && cycle.Constraint.ChargeTypeID == ChargeTypeEnum.Charge ? cycle.Constraint.Amount.Value : 0M);
                        amount -= new RebillOrderFlow().GetSubscriptionDiscount(orderPlanView.OrderRecurringPlan, amount);
                        amount = amount < 0 ? 0 : amount;
                        lblChargeAmount.Text = Utility.FormatCurrency(amount, currencySymbol);
                    }
                    else
                        lblChargeAmount.Text = "Order Completed";
                }


                if (op.DiscountTypeID == (int)DiscountType.FixedPrice)
                    discountValue.Value = Utility.FormatCurrency(op.DiscountValue, currencySymbol);
                else
                    discountValue.Value = Utility.FormatPrice(op.DiscountValue) + "%";
            }
        }
    }
}