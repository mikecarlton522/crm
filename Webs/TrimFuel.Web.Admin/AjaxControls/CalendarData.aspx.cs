using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Model.Enums;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class CalendarData : System.Web.UI.Page
    {
        private OrderService orderService = new OrderService();
        private SubscriptionNewService subsService = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {
            BillingID = Utility.TryGetLong(Request["billingID"]);
            if (!IsPostBack)
            {
                DataBind();
            }            
        }

        public long? BillingID { get; set; }
        public IList<OrderView> Orders { get; set; }

        protected string FixString(string val)
        {
            if (val == null)
                return null;
            return val.Replace("'", "\"");
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (BillingID != null)
            {
                Orders = orderService.LoadOrders(BillingID);
            }

            if (Orders != null)
            {
                IList<InvoiceView> invoiceList = Orders.SelectMany(i => i.InvoiceList.Where(inv => inv.Invoice.InvoiceStatus == InvoiceStatusEnum.Paid))
                    .OrderByDescending(inv => inv.Invoice.CreateDT).ToList();

                //Upcoming Billings
                IList<OrderRecurringPlanView> activePlanList = new List<OrderRecurringPlanView>();
                foreach (var item in Orders.SelectMany(i => i.SaleList.SelectMany(j => j.PlanList)).Where(i => i.RecurringStatus == RecurringStatusEnum.Active))
                {
                    activePlanList.Add(orderService.GetPlan(item.OrderRecurringPlanID.Value));
                }

                IList<InvoiceView> plannedInvoiceList = new List<InvoiceView>();
                foreach (var item in activePlanList)
                {
                    DateTime nextBillDate = DateTime.Today.AddDays(1);
                    if (item.OrderRecurringPlan.NextCycleDT != null && item.OrderRecurringPlan.NextCycleDT > nextBillDate)
                    {
                        nextBillDate = item.OrderRecurringPlan.NextCycleDT.Value;
                    }

                    RecurringPlanCycleView nextCycle = item.GetNextCycle();
                    while (nextBillDate <= DateTime.Now.AddYears(1) && nextCycle != null)
                    {                        
                        Invoice invoice = new Invoice()
                        {
                            Amount = (nextCycle.Constraint != null && 
                                      nextCycle.Constraint.Amount > 0M && 
                                      nextCycle.Constraint.ChargeTypeID == ChargeTypeEnum.Charge
                                      ? nextCycle.Constraint.Amount
                                      : 0M),
                            CreateDT = nextBillDate
                        };
                        InvoiceView invoiceView = new InvoiceView()
                        {
                            Invoice = invoice,
                            SaleList = new List<OrderSaleView>()
                        };
                        
                        RecurringSale sale = new RecurringSale()
                        {
                            CreateDT = nextBillDate,
                            Quantity = 1,
                            PurePrice = invoice.Amount,
                            RecurringCycle = (item.SaleList.Count > 0 ? item.SaleList.Last().RecurringCycle + 1 : 1),
                            SaleStatus = SaleStatusEnum.Approved,
                            SaleType = OrderSaleTypeEnum.Rebill
                        };
                        OrderSaleView saleView = new OrderSaleView()
                        {
                            Invoice = invoiceView,
                            OrderSale = sale                            
                        };
                        saleView.ProductList = nextCycle.ShipmentList.Select(i => new OrderProductView()
                        {
                            ProductSKU = new ProductSKU()
                            {
                                ProductName = i.ProductSKU,
                                ProductSKU_ = i.ProductSKU
                            },
                            Sale = saleView,
                            OrderProduct = new OrderProduct()
                            {
                                ProductSKU = i.ProductSKU,
                                Quantity = i.Quantity
                            }
                        }).ToList();

                        invoiceView.SaleList.Add(saleView);

                        plannedInvoiceList.Add(invoiceView);

                        item.AttemptList.Add(sale);
                        nextBillDate = nextBillDate.AddDays(nextCycle.Cycle.Interim > 0 ? nextCycle.Cycle.Interim.Value : 1);
                        nextCycle = item.GetNextCycle();
                    }
                }

                rInvoices.DataSource = invoiceList;
                rPlannedInvoices.DataSource = plannedInvoiceList;
            }
        }
    }
}