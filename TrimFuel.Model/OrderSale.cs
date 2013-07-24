using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderSale : Entity
    {
        public long? SaleID { get; set; }
        public string SaleName { get; set; }
        public int? SaleType { get; set; }
        public int? Quantity { get; set; }
        public long? OrderID { get; set; }
        public decimal? PurePrice { get; set; }
        public int? SaleStatus { get; set; }
        public long? InvoiceID { get; set; }
        public DateTime? CreateDT { get; set; }
        public DateTime? ProcessDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("OrderID", OrderID);
            v.AssertNotNull("SaleType", SaleType);
            v.AssertNotNull("Quantity", Quantity);
            v.AssertNotNull("PurePrice", PurePrice);
            v.AssertNotNull("SaleStatus", SaleStatus);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertString("SaleName", SaleName, 255);
        }

        #region Logic

        public void FillFromOrderSale(OrderSale sale)
        {
            SaleID = sale.SaleID;
            SaleName = sale.SaleName;
            SaleType = sale.SaleType;
            Quantity = sale.Quantity;
            OrderID = sale.OrderID;
            PurePrice = sale.PurePrice;
            SaleStatus = sale.SaleStatus;
            InvoiceID = sale.InvoiceID;
            CreateDT = sale.CreateDT;
            ProcessDT = sale.ProcessDT;
        }

        public decimal? ChargedAmount 
        {
            get 
            {
                if (PurePrice == null || Quantity == null)
                    return null;
                return PurePrice * Quantity;
            }
        }

        #endregion
    }
}
