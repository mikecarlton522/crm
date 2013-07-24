using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Enums;
using TrimFuel.Business;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class SaleHistory : System.Web.UI.UserControl
    {
        private SaleService saleService = new SaleService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public IList<OrderView> Orders { get; set; }
        public IList<OrderChargeHistoryView> Charges { get; set; }
        public bool IsGiftAvailable { get; set; }

        public bool IsRefundAvailable(OrderSaleView saleView)
        {
            decimal invoiceAmount = saleView.Invoice.Invoice.Amount.Value;
            if (invoiceAmount > 0M)
            {
                var refundList = GetSaleRefundList(saleView.OrderSale.SaleID.Value);
                if (refundList.Count == 0 ||
                    Math.Abs(refundList.Sum(i => i.CurrencyAmount.Value)) < invoiceAmount)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool IsReturnAvailable(OrderSaleView saleView)
        {
            if (saleView.ShipmentList != null && 
                saleView.ShipmentList.Count > 0 && 
                saleView.ShipmentList.FirstOrDefault(
                    i => i.Shipment.ShipmentStatus < ShipmentStatusEnum.Returned && 
                         i.Shipment.ShipmentStatus > ShipmentStatusEnum.Blocked) != null)
            {
                return true;
            }
            return false;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (Orders != null)
            {
                IsGiftAvailable = saleService.IsGiftAvailable();

                IList<OrderView> src = new List<OrderView>();
                IList<InvoiceView> invoiceList = Orders.SelectMany(i => i.InvoiceList.Where(inv => inv.Invoice.InvoiceStatus == InvoiceStatusEnum.Paid))
                    .Distinct().OrderByDescending(inv => inv.Invoice.CreateDT).ToList();
                foreach (var invoice in invoiceList)
                {
                    if (src.Count == 0 || src.Last().Order != invoice.Order.Order)
                    {
                        OrderView splittedOrder = new OrderView()
                        {
                            Order = invoice.Order.Order,
                            InvoiceList = new List<InvoiceView>(),
                            SaleList = new List<OrderSaleView>()
                        };
                        src.Add(splittedOrder);
                    }
                    src.Last().InvoiceList.Add(invoice);
                    src.Last().SaleList = src.Last().SaleList.Union(invoice.SaleList).ToList();
                }

                rSplittedOrders.DataSource = src;
            }
        }

        protected string ShowGiftCertificates(long saleID)
        {
            string res = "No";

            IList<PromoGift> giftList = saleService.GetGiftCertificateBySale(saleID);
            if (giftList.Count > 0)
            {
                res = string.Join("<br/>", giftList.Select(i => string.Format("<a onclick=\"editPromoGift('{0}')\" href='javascript:void(0)'>{0}</a>", i.GiftNumber)).ToArray());
            }

            return res;
        }

        IList<ChargeHistoryView> GetSaleRefundList(long saleID)
        {
            var a = Charges.SelectMany(i => i.RefundsBySales);
            var b = a.Where(i => i.Key == saleID);
            var c = b.SelectMany(i => i.Value);
            return c.ToList();
        }

        protected string ShowSaleRefunds(long saleID)
        {
            return OrderHelper.ShowSaleRefunds(GetSaleRefundList(saleID));
        }

        protected string ShowSubmittedShipments(OrderSaleView saleView)
        {
            if (saleView.ShipmentList == null || saleView.ShipmentList.Count == 0)
                return "-";
            string res = string.Join("<br/>", saleView.ShipmentList.Where(i => i.Shipment.ShipmentStatus >= ShipmentStatusEnum.Submitted)
                .Select(i => i.Shipment.SendDT.ToString()).Distinct()
                .ToArray());
            if (saleView.ShipmentList.Count(i => i.Shipment.ShipmentStatus == ShipmentStatusEnum.New) > 0)
            {
                if (res.Length > 0) res += "<br/>Partial ";
                res += "Pending";
            }
            if (saleView.ShipmentList.Count(i => i.Shipment.ShipmentStatus == ShipmentStatusEnum.SubmitError) > 0)
            {
                if (res.Length > 0) res += "<br/>Partial ";
                res += "Error";
            }
            if (saleView.ShipmentList.Count(i => i.Shipment.ShipmentStatus == ShipmentStatusEnum.Blocked) > 0)
            {
                if (res.Length > 0) res += "<br/>Partial ";
                res += "Blocked";
            }
            return res;
        }

        protected string ShowShippedShipments(OrderSaleView saleView)
        {
            if (saleView.ShipmentList == null || saleView.ShipmentList.Count == 0)
                return "-";
            string res = string.Join("<br/>", saleView.ShipmentList.Where(i => i.Shipment.ShipmentStatus >= ShipmentStatusEnum.Shipped && !string.IsNullOrEmpty(i.Shipment.TrackingNumber))
                .Select(i => i.Shipment.TrackingNumber.ToString()).Distinct()
                .ToArray());
            if (saleView.ShipmentList.Count(i => i.Shipment.ShipmentStatus >= ShipmentStatusEnum.Shipped && string.IsNullOrEmpty(i.Shipment.TrackingNumber)) > 0)
            {
                if (res.Length > 0) res += "<br/>Partial ";
                res += "Shipped without tracking number";
            }
            if (string.IsNullOrEmpty(res))
                res = "No";
            return res;
        }

        protected string ShowReturnedShipments(OrderSaleView saleView)
        {
            if (saleView.ShipmentList == null || saleView.ShipmentList.Count == 0)
                return "-";
            string res = string.Join("<br/>", saleView.ShipmentList.Where(i => i.Shipment.ShipmentStatus >= ShipmentStatusEnum.Returned)
                .Select(i => i.Shipment.ReturnDT.ToString()).Distinct()
                .ToArray());
            if (string.IsNullOrEmpty(res))
                res = "No";
            return res;
        }

        protected string ShowShipmentLink(string text, long saleID)
        {
            if (string.IsNullOrEmpty(text) || text == "-" || text == "No" || text == "Pending")
                return text;
            return "<a href='javascript:void(0)' onclick='if (showSaleShipments) showSaleShipments(" + saleID.ToString() + ")'>" + text + "</a>";
        }
    }
}