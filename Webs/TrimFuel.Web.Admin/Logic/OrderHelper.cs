using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Logic
{
    public static class OrderHelper
    {
        public static string ShowSaleType(object saleObject)
        {
            if (saleObject == null)
                return null;
            OrderSaleView sale = (OrderSaleView)saleObject;
            string res = OrderSaleTypeEnum.Name[sale.OrderSale.SaleType.Value];
            if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Rebill && sale.OrderSale is RecurringSale)
            {
                res += " #" + (sale.OrderSale as RecurringSale).RecurringCycle.ToString();
            }
            if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Trial && sale.OrderSale.PurePrice > 0 && sale.ProductList.Count == 0)
            {
                res = "Trial Billing";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Trial && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count > 0)
            {
                res = "Free Trial";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Trial && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count == 0)
            {
                res = "Free Subscription Sign";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Upsell && sale.OrderSale.PurePrice > 0 && sale.ProductList.Count == 0)
            {
                res = "Billing";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Upsell && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count > 0)
            {
                res = "Free Product";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Upsell && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count == 0 && sale.Invoice != null && sale.Invoice.Invoice.AuthAmount > 0M)
            {
                res = "Authorizing";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Upsell && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count == 0)
            {
                res = "Scrub";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Rebill && sale.OrderSale.PurePrice > 0 && sale.ProductList.Count == 0)
            {
                res += "<br/>Billing";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Rebill && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count > 0)
            {
                res += "<br/>Free Product";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Rebill && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count == 0 && sale.Invoice != null && sale.Invoice.Invoice.AuthAmount > 0M)
            {
                res += "<br/>Authorizing";
            }
            else if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Rebill && sale.OrderSale.PurePrice == 0 && sale.ProductList.Count == 0)
            {
                res += "<br/>Scrub";
            }
            return res;
        }

        public static string ShowSalePrice(object saleObject)
        {
            if (saleObject == null)
                return null;
            OrderSaleView sale = (OrderSaleView)saleObject;
            string currencySymbol = "$";
            if (sale.Invoice != null && sale.Invoice.Currency != null)
            {
                currencySymbol = sale.Invoice.Currency.HtmlSymbol;
            }
            decimal authAmount = 0M;
            if (sale.Invoice != null && sale.Invoice.Invoice.AuthAmount > 0M)
            {
                authAmount = sale.Invoice.Invoice.AuthAmount.Value;
            }
            if (authAmount > 0M)
            {
                return
                    Utility.FormatCurrency(sale.OrderSale.PurePrice * sale.OrderSale.Quantity, currencySymbol) +
                    "<br/>(Auth " +
                    Utility.FormatCurrency(authAmount, currencySymbol) +
                    ")";
            }
            return Utility.FormatCurrency(sale.OrderSale.PurePrice * sale.OrderSale.Quantity, currencySymbol);
        }

        public static string ShowInvoiceAmount(object invoiceObject)
        {
            if (invoiceObject == null)
                return null;
            InvoiceView invoice = (InvoiceView)invoiceObject;
            string currencySymbol = "$";
            if (invoice.Currency != null)
            {
                currencySymbol = invoice.Currency.HtmlSymbol;
            }
            if (invoice.Invoice.AuthAmount > 0M)
            {
                return
                    Utility.FormatCurrency(invoice.Invoice.Amount, currencySymbol) +
                    "<br/>(Auth " +
                    Utility.FormatCurrency(invoice.Invoice.AuthAmount, currencySymbol) +
                    ")";
            }
            return Utility.FormatCurrency(invoice.Invoice.Amount, currencySymbol);
        }

        public static string ShowSaleDescription(object saleObject)
        {
            if (saleObject == null)
                return null;
            OrderSaleView sale = (OrderSaleView)saleObject;

            string res = (!string.IsNullOrEmpty(sale.OrderSale.SaleName) ? (sale.OrderSale.Quantity == 1 ? "" : sale.OrderSale.Quantity.ToString()) + sale.OrderSale.SaleName + ":<br/>" : "");
            res += string.Join("<br/>", sale.ProductList.Select(i => (sale.OrderSale.Quantity * i.OrderProduct.Quantity).ToString() + "x " + i.ProductSKU.ProductName).ToArray());
            return res;
        }

        public static string ShowSaleSKUs(object saleObject)
        {
            if (saleObject == null)
                return null;
            OrderSaleView sale = (OrderSaleView)saleObject;

            return string.Join("<br/>", sale.ProductList.Select(i => (sale.OrderSale.Quantity * i.OrderProduct.Quantity).ToString() + "x " + i.ProductSKU.ProductSKU_).ToArray()); ;
        }

        public static string ShowSaleRefunds(IList<ChargeHistoryView> refunds)
        {
            if (refunds == null || refunds.Count == 0)
                return "No";
            string res = "";
            foreach (var item in refunds)
            {
                if (res != "")
                    res += "<br/>";
                res += ShowChargeAmount(item) + " " + item.ChargeHistory.ChargeDate.ToString();
            }
            return res;
        }

        public static string ShowChargeAmount(object chargeHistoryViewObject)
        {
            if (chargeHistoryViewObject == null)
                return null;
            ChargeHistoryView chargeView = (ChargeHistoryView)chargeHistoryViewObject;
            if (chargeView.ChargeHistory is AuthOnlyChargeDetails)
            {
                return 
                    Utility.FormatCurrency(chargeView.CurrencyAmount, chargeView.CurrencyHtmlSymbol) +
                    "<br/>(Auth " +
                    Utility.FormatCurrency(chargeView.CurrencyAuthAmount, chargeView.CurrencyHtmlSymbol) +
                    ")";
            }
            return Utility.FormatCurrency(chargeView.CurrencyAmount, chargeView.CurrencyHtmlSymbol);
        }
    }
}