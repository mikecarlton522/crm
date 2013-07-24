using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Flow
{
    public class PaymentFlow : BaseService
    {
        //public enum PaymentResult
        //{
        //    Approved = 1,
        //    Declined = 2,
        //    Queued = 3
        //}

        public BusinessError<ChargeHistoryView> ProcessFullRefund(InvoiceView invoice)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                OrderService orders = new OrderService();
                PaymentFlow payments = new PaymentFlow();
                SaleFlow sales = new SaleFlow();
                if (invoice.Invoice.Amount > 0M)
                {
                    ChargeHistoryView charge = orders.GetInvoiceChargeHistory(invoice.Invoice.InvoiceID.Value)
                        .FirstOrDefault(i => i.ChargeHistory.Amount > 0M && i.ChargeHistory.Success == true);
                    if (charge == null)
                    {
                        res.ErrorMessage = string.Format("Transaction was not found for Invoice #{0}", invoice.Invoice.InvoiceID);
                    }
                    else
                    {
                        DateTime nowDatePlus1Hour = DateTime.Now.AddHours(1);
                        bool isRefundAvailable = (nowDatePlus1Hour.Date > charge.ChargeHistory.ChargeDate.Value.Date);
                        if (isRefundAvailable)
                        {
                            res = payments.ProcessRefund(charge, invoice.Invoice.Amount.Value, invoice.Invoice.InvoiceID);
                        }
                        else
                        {
                            res = payments.ProcessVoid(charge, invoice.Invoice.InvoiceID);
                        }

                        if (res.ReturnValue != null)
                        {
                            foreach (var sale in invoice.SaleList)
                            {
                                SaleRefund saleRef = new SaleRefund();
                                saleRef.SaleID = sale.OrderSale.SaleID;
                                saleRef.ChargeHistoryID = res.ReturnValue.ChargeHistory.ChargeHistoryID;
                                dao.Save(saleRef);

                                if (invoice.SaleList.Count > 1)
                                {
                                    ChargeHistoryExSale chSale = new ChargeHistoryExSale();
                                    chSale.ChargeHistoryID = res.ReturnValue.ChargeHistory.ChargeHistoryID;
                                    chSale.SaleID = sale.OrderSale.SaleID;
                                    chSale.Amount = sale.OrderSale.ChargedAmount;
                                    if (invoice.Currency != null)
                                    {
                                        chSale.CurrencyAmount = chSale.Amount;
                                        chSale.CurrencyID = invoice.Currency.CurrencyID;
                                        chSale.Amount = invoice.Currency.ConvertToUSD(chSale.CurrencyAmount.Value);
                                    }
                                    dao.Save(chSale);
                                }

                                if (res.State == BusinessErrorState.Success)
                                {
                                    sales.OnSaleRefunded(sale.OrderSale, charge, res.ReturnValue);
                                }
                            }
                        }
                    }
                }
                else
                {
                    res.ErrorMessage = string.Format("Invoice{0} with Amount = 0.00 can not be refunded", invoice.Invoice.InvoiceID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryView> ProcessCharge(InvoiceView invoice)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                Product product = EnsureLoad<Product>(invoice.Order.Order.ProductID);
                Billing billing = invoice.Order.Billing;
                BusinessError<Set<NMICompany, AssertigyMID>> mid = ChooseMID(billing, product, invoice.Currency, invoice.Invoice.Amount.Value);
                if (mid.State == BusinessErrorState.Error)
                {
                    res.ReturnValue = null;
                    res.ErrorMessage = mid.ErrorMessage;
                    res.State = BusinessErrorState.Error;
                }
                else
                {
                    res = ProcessCharge(mid.ReturnValue, billing, product, invoice.Currency, invoice.Invoice.Amount.Value, invoice.Invoice.InvoiceID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryView> ProcessAuth(InvoiceView invoice)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                Product product = EnsureLoad<Product>(invoice.Order.Order.ProductID);
                Billing billing = invoice.Order.Billing;
                BusinessError<Set<NMICompany, AssertigyMID>> mid = ChooseMID(billing, product, invoice.Currency, invoice.Invoice.AuthAmount.Value);
                if (mid.State == BusinessErrorState.Error)
                {
                    res.ReturnValue = null;
                    res.ErrorMessage = mid.ErrorMessage;
                    res.State = BusinessErrorState.Error;
                }
                else
                {
                    res = ProcessAuth(mid.ReturnValue, billing, product, invoice.Currency, invoice.Invoice.AuthAmount.Value, invoice.Invoice.InvoiceID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryView> ProcessTestCharge(InvoiceView invoice)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                Product product = EnsureLoad<Product>(invoice.Order.Order.ProductID);
                Billing billing = invoice.Order.Billing;
                BusinessError<Set<NMICompany, AssertigyMID>> mid = ChooseMID(billing, product, invoice.Currency, invoice.Invoice.Amount.Value);
                if (mid.State == BusinessErrorState.Error)
                {
                    res.ReturnValue = null;
                    res.ErrorMessage = mid.ErrorMessage;
                    res.State = BusinessErrorState.Error;
                }
                else
                {
                    res = ProcessTestCharge(mid.ReturnValue, billing, product, invoice.Currency, invoice.Invoice.Amount.Value, invoice.Invoice.InvoiceID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryView> ProcessRefund(ChargeHistoryView chargeHistory, decimal refundAmount, long? invoiceID)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                Invoice invoice = EnsureLoad<Invoice>(invoiceID);
                Order order = EnsureLoad<Order>(invoice.OrderID);
                Billing billing = EnsureLoad<Billing>(order.BillingID);
                Set<NMICompany,AssertigyMID> mid = new Set<NMICompany,AssertigyMID>();
                mid.Value2 = EnsureLoad<AssertigyMID>(chargeHistory.ChargeHistory.MerchantAccountID);
                mid.Value1 = (new MerchantService()).GetNMICompanyByAssertigyMID(mid.Value2.AssertigyMIDID);
                res = ProcessRefund(mid, billing, chargeHistory, refundAmount, invoice.InvoiceID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryView> ProcessVoid(ChargeHistoryView chargeHistory, long? invoiceID)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                Invoice invoice = EnsureLoad<Invoice>(invoiceID);
                Order order = EnsureLoad<Order>(invoice.OrderID);
                Billing billing = EnsureLoad<Billing>(order.BillingID);
                Set<NMICompany, AssertigyMID> mid = new Set<NMICompany, AssertigyMID>();
                mid.Value2 = EnsureLoad<AssertigyMID>(chargeHistory.ChargeHistory.MerchantAccountID);
                mid.Value1 = (new MerchantService()).GetNMICompanyByAssertigyMID(mid.Value2.AssertigyMIDID);
                res = ProcessVoid(mid, billing, chargeHistory, invoice.InvoiceID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual BusinessError<Set<NMICompany, AssertigyMID>> ChooseMID(Billing billing, Product product, Currency currency, decimal amount)
        {
            BusinessError<Set<NMICompany, AssertigyMID>> res = new BusinessError<Set<NMICompany, AssertigyMID>>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                MerchantService srv = new MerchantService();
                Set<NMICompany, AssertigyMID> mid = srv.ChooseLastSuccessfulNMIMerchantAccount(product.ProductID.Value, billing, amount);
                if (mid != null)
                {
                    res.State = BusinessErrorState.Success;
                    res.ReturnValue = mid;
                    res.ErrorMessage = string.Empty;
                }
                else
                {
                    mid = srv.ChooseRandomNMIMerchantAccount(product.ProductID.Value, billing, amount);
                    if (mid != null)
                    {
                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = mid;
                        res.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        string currencyName = "USD";
                        if (currency != null)
                        {
                            currencyName = currency.CurrencyName;
                        }
                        string paymentTypeName = "Unknown";
                        int? paymentTypeID = billing.CreditCardCnt.TryGetCardType();
                        if (paymentTypeID != null && PaymentTypeEnum.Types.ContainsKey(paymentTypeID))
                        {
                            paymentTypeName = PaymentTypeEnum.Types[paymentTypeID];
                        }                        
                        res.ErrorMessage = string.Format("Billing attempt failed. Can't choose MID for Product({0}), PaymentType({1}), Currency({2}), Amount({3})",
                            product.ProductName, paymentTypeName, currencyName, amount);

                        AddNote(billing, res.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public virtual bool IsChargesBlocked(Billing billing, out string chargeBlockReason)
        {
            bool res = false;
            chargeBlockReason = null;
            try 
	        {	 
                if (billing != null && billing.BillingID != null)
                {
		            MySqlCommand q = new MySqlCommand(@"
                        select * from BillingStopCharge
                        where BillingID = @billingID
                        order by BillingID desc
                        limit 1
                    ");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billing.BillingID;
                    var block = dao.Load<BillingStopCharge>(q).FirstOrDefault();
                    if (block != null)
                    {
                        res = true;
                        chargeBlockReason = block.StopReason;
                    }
                }
	        }
	        catch (Exception ex)
	        {
                logger.Error(ex);
	        }
            return res;
        }

        protected virtual BusinessError<ChargeHistoryView> ProcessCharge(Set<NMICompany, AssertigyMID> mid, Billing billing, Product product, Currency currency, decimal amount, long? invoiceID)
        {
            string chargeBlockReason = null;
            if (IsChargesBlocked(billing, out chargeBlockReason))
            {
                return new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, chargeBlockReason);
            }

            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                IPaymentGateway paymentGateway = (new SaleService()).GetGatewayByMID(mid.Value2);

                BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(mid.Value2.MID,
                    mid.Value1.GatewayUsername, mid.Value1.GatewayPassword, amount, currency,
                    invoiceID.Value, billing, product);
                var chargeRes = ChargeLogging(paymentResult, billing,
                    mid.Value2, ChargeTypeEnum.Charge, amount, currency, invoiceID);

                res.State = paymentResult.State;
                res.ReturnValue = chargeRes.Value2;
                res.ErrorMessage = paymentResult.ErrorMessage;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual BusinessError<ChargeHistoryView> ProcessAuth(Set<NMICompany, AssertigyMID> mid, Billing billing, Product product, Currency currency, decimal amount, long? invoiceID)
        {
            string chargeBlockReason = null;
            if (IsChargesBlocked(billing, out chargeBlockReason))
            {
                return new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, chargeBlockReason);
            }

            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                IPaymentGateway paymentGateway = (new SaleService()).GetGatewayByMID(mid.Value2);

                BusinessError<GatewayResult> paymentResult = paymentGateway.AuthOnly(mid.Value2.MID,
                    mid.Value1.GatewayUsername, mid.Value1.GatewayPassword, amount, currency,
                    billing, product);
                var chargeRes = ChargeLogging(paymentResult, billing,
                    mid.Value2, ChargeTypeEnum.AuthOnly, amount, currency, invoiceID);

                res.State = paymentResult.State;
                res.ReturnValue = chargeRes.Value2;
                res.ErrorMessage = paymentResult.ErrorMessage;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual BusinessError<ChargeHistoryView> ProcessTestCharge(Set<NMICompany, AssertigyMID> mid, Billing billing, Product product, Currency currency, decimal amount, long? invoiceID)
        {
            string chargeBlockReason = null;
            if (IsChargesBlocked(billing, out chargeBlockReason))
            {
                return new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, chargeBlockReason);
            }

            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                res.ReturnValue = new ChargeHistoryView()
                {
                    ChargeHistory = new ChargeHistoryEx()
                    {
                        Amount = amount,
                        AuthorizationCode = "TEST",
                        ChargeDate = DateTime.Now,
                        ChargeHistoryID = 0,
                        ChargeTypeID = ChargeTypeEnum.Charge,
                        ChildMID = "TEST",
                        TransactionNumber = "TEST"                        
                    },
                    Currency = currency,
                    MIDName = "TEST MID"
                };
                if (currency != null)
                {
                    res.ReturnValue.ChargeHistory.Amount = currency.ConvertToUSD(amount);
                    res.ReturnValue.ChargeHistoryCurrency = new ChargeHistoryExCurrency()
                    {
                        ChargeHistoryID = 0,
                        CurrencyAmount = amount,
                        CurrencyID = currency.CurrencyID
                    };
                }
                if (billing.LastName.ToLower() == "success")
                {
                    res.State = BusinessErrorState.Success;
                    res.ErrorMessage = "Test case. SUCCESS";
                    res.ReturnValue.ChargeHistory.Success = true;
                    res.ReturnValue.ChargeHistory.Response = "Test charge. SUCCESS";
                }
                else
                {
                    res.State = BusinessErrorState.Error;
                    res.ErrorMessage = "Test case. ERROR";
                    res.ReturnValue.ChargeHistory.Success = false;
                    res.ReturnValue.ChargeHistory.Response = "Test charge. ERROR";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual BusinessError<ChargeHistoryView> ProcessRefund(Set<NMICompany, AssertigyMID> mid, Billing billing, ChargeHistoryView chargeHistory, decimal refundAmount, long? invoiceID)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                IPaymentGateway paymentGateway = (new SaleService()).GetGatewayByMID(mid.Value2);

                BusinessError<GatewayResult> paymentResult = paymentGateway.Refund(mid.Value1.GatewayUsername, mid.Value1.GatewayPassword,
                    chargeHistory.ChargeHistory, refundAmount, chargeHistory.Currency);
                if (paymentResult != null)
                {
                    if (paymentResult.State == BusinessErrorState.Error)
                    {
                        res.State = BusinessErrorState.Error;
                        res.ErrorMessage = "Refund failed";
                    }
                    else
                    {
                        res.State = BusinessErrorState.Success;
                        res.ErrorMessage = "Refund processed";
                    }
                    var resCh = ChargeLogging(paymentResult, billing,
                        mid.Value2, ChargeTypeEnum.Refund, -refundAmount, chargeHistory.Currency, invoiceID);
                    res.ReturnValue = resCh.Value2;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual BusinessError<ChargeHistoryView> ProcessVoid(Set<NMICompany, AssertigyMID> mid, Billing billing, ChargeHistoryView chargeHistory, long? invoiceID)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                IPaymentGateway paymentGateway = (new SaleService()).GetGatewayByMID(mid.Value2);

                BusinessError<GatewayResult> paymentResult = paymentGateway.Void(mid.Value1.GatewayUsername, mid.Value1.GatewayPassword,
                    chargeHistory.ChargeHistory, chargeHistory.CurrencyAmount.Value, chargeHistory.Currency);
                if (paymentResult != null)
                {
                    if (paymentResult.State == BusinessErrorState.Error)
                    {
                        res.State = BusinessErrorState.Error;
                        res.ErrorMessage = "Void failed";
                    }
                    else
                    {
                        res.State = BusinessErrorState.Success;
                        res.ErrorMessage = "Void processed";
                    }
                    var resCh = ChargeLogging(paymentResult, billing,
                        mid.Value2, ChargeTypeEnum.Void, -chargeHistory.CurrencyAmount.Value, chargeHistory.Currency, invoiceID);
                    res.ReturnValue = resCh.Value2;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public Set<Paygea, ChargeHistoryView> ChargeLogging(BusinessError<GatewayResult> chargeResult, Billing billing,
            AssertigyMID assertigyMID, int chargeType, decimal amount, Currency currency, long? invoiceID)
        {
            Set<Paygea, ChargeHistoryView> res = new Set<Paygea, ChargeHistoryView>();

            try
            {
                dao.BeginTransaction();

                Paygea paygea = new Paygea();
                paygea.BillingID = billing.BillingID;
                paygea.Request = chargeResult.ReturnValue.Request;
                paygea.Response = chargeResult.ReturnValue.Response;
                paygea.CreateDT = DateTime.Now;
                dao.Save<Paygea>(paygea);
                res.Value1 = paygea;

                decimal usdAmount = amount;
                decimal? currencyAmount = null;
                if (currency != null)
                {
                    currencyAmount = usdAmount;
                    usdAmount = currency.ConvertToUSD(usdAmount);
                }

                AddNote(billing, 
                    CreateTransactionNote(assertigyMID, chargeType, usdAmount, currency, currencyAmount, 
                        (chargeResult.State == BusinessErrorState.Success)));

                ChargeHistoryExCurrency chargeHistoryExCurrency = null;

                ChargeHistoryEx chargeHistory = new ChargeHistoryEx();
                chargeHistory.MerchantAccountID = assertigyMID.AssertigyMIDID;
                chargeHistory.Success = (chargeResult.State == BusinessErrorState.Success);
                chargeHistory.ChargeTypeID = chargeType;
                chargeHistory.Amount = usdAmount;
                chargeHistory.ChargeDate = DateTime.Now;
                chargeHistory.ChildMID = assertigyMID.MID;
                //chargeHistory.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                try
                {
                    IPaymentGateway paymentGateway = (new SaleService()).GetGatewayByMID(assertigyMID);
                    chargeHistory.AuthorizationCode = paymentGateway.GetTransactionAuthCode(chargeResult.ReturnValue);
                    chargeHistory.TransactionNumber = paymentGateway.GetTransactionID(chargeResult.ReturnValue);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
                chargeHistory.Response = chargeResult.ReturnValue.Response;


                if (chargeType == ChargeTypeEnum.AuthOnly || chargeType == ChargeTypeEnum.VoidAuthOnly)
                {
                    AuthOnlyChargeDetails details = new AuthOnlyChargeDetails();
                    details.FillFromChargeHistory(chargeHistory);
                    details.RequestedAmount = usdAmount;
                    if (currency != null)
                    {
                        details.RequestedCurrencyID = currency.CurrencyID;
                        details.RequestedCurrencyAmount = currencyAmount;
                    }
                    //0.0 for AuthOnly
                    details.Amount = 0.0M;
                    dao.Save<AuthOnlyChargeDetails>(details);
                    chargeHistory = details;
                }
                else
                {
                    dao.Save<ChargeHistoryEx>(chargeHistory);

                    if (currency != null)
                    {
                        chargeHistoryExCurrency = new ChargeHistoryExCurrency();
                        chargeHistoryExCurrency.ChargeHistoryID = chargeHistory.ChargeHistoryID;
                        chargeHistoryExCurrency.CurrencyID = currency.CurrencyID;
                        chargeHistoryExCurrency.CurrencyAmount = currencyAmount;
                        dao.Save<ChargeHistoryExCurrency>(chargeHistoryExCurrency);
                    }
                }

                var chargeHistoryCard = new ChargeHistoryCard()
                {
                    ChargeHistoryID = chargeHistory.ChargeHistoryID,
                    CreditCardRight4 = billing.CreditCardCnt.DecryptedCreditCardRight4,
                    CreditCardLeft6 = billing.CreditCardCnt.DecryptedCreditCardLeft6,
                    ExpMonth = billing.ExpMonth,
                    ExpYear = billing.ExpYear,
                    PaymentTypeID = billing.CreditCardCnt.TryGetCardType()
                };
                dao.Save<ChargeHistoryCard>(chargeHistoryCard);

                res.Value2 = new ChargeHistoryView(){
                    ChargeHistory = chargeHistory,
                    ChargeHistoryCurrency = chargeHistoryExCurrency,
                    Currency = currency,
                    MIDName = assertigyMID.DisplayName
                };

                if (chargeResult.State == BusinessErrorState.Success &&
                    !(chargeType == ChargeTypeEnum.AuthOnly || chargeType == ChargeTypeEnum.VoidAuthOnly))
                {
                    (new MerchantService()).UpdateAssertigyMIDDailyCap(assertigyMID, usdAmount);
                }

                ChargeHistoryInvoice chInvoice = new ChargeHistoryInvoice();
                chInvoice.ChargeHistoryID = chargeHistory.ChargeHistoryID;
                chInvoice.InvoiceID = invoiceID;
                dao.Save(chInvoice);

                //Check response
                string stopChargeReason = null;
                if (CheckStopCharge(chargeResult.ReturnValue.Response, out stopChargeReason))
                {
                    BillingStopCharge stopCharge = new BillingStopCharge();
                    stopCharge.BillingID = billing.BillingID;
                    stopCharge.StopReason = stopChargeReason;
                    stopCharge.CreateDT = DateTime.Now;
                    dao.Save(stopCharge);

                    AddNote(billing, stopChargeReason);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }

            return res;
        }

        public virtual bool CheckStopCharge(string paymentGatewayResponse, out string stopReason)
        {
            bool res = false;
            stopReason = null;

            if (paymentGatewayResponse != null)
            {
                if (paymentGatewayResponse.ToLower().Contains("pick up card") ||
                    paymentGatewayResponse.ToLower().Contains("pic up"))
                {
                    res = true;
                    stopReason = "Pick up card detected, stopping re-attempts immediately";
                }
            }

            return res;
        }

        public string CreateTransactionNote(AssertigyMID assertigyMID, int chargeType, decimal usdAmount, Currency currency, decimal? currencyAmount, bool success)
        {
            string res = string.Empty;
            if (success)
            {
                switch (chargeType)
                {
                    case ChargeTypeEnum.Charge:
                        res = "Billing processed.";
                        //switch (saleTypeID)
                        //{
                        //    case SaleTypeEnum.Billing:
                        //        res = "Billing processed (Billing Sale).";
                        //        break;
                        //    case SaleTypeEnum.Upsell:
                        //        res = "Billing processed (Upsell).";
                        //        break;
                        //    case SaleTypeEnum.Rebill:
                        //        res = "Billing processed (Rebill).";
                        //        break;
                        //    default:
                        //        res = "Billing processed.";
                        //        break;
                        //}
                        break;
                    case ChargeTypeEnum.AuthOnly:
                        res = "Authorization processed.";
                        break;
                    case ChargeTypeEnum.VoidAuthOnly:
                        res = "Authorization canceled.";
                        break;
                    case ChargeTypeEnum.Refund: //= ChargeTypeEnum.Credit
                    case ChargeTypeEnum.Void:
                        res = "Refund processed.";
                        break;
                    default:
                        res = "Transaction processed.";
                        break;
                }
            }
            else
            {
                switch (chargeType)
                {
                    case ChargeTypeEnum.Charge:
                        res = "Billing attempt unsuccessful.";
                        break;
                    case ChargeTypeEnum.AuthOnly:
                        res = "Authorization failed.";
                        break;
                    case ChargeTypeEnum.VoidAuthOnly:
                        res = "Authorization cancelation failed.";
                        break;
                    case ChargeTypeEnum.Refund: //= ChargeTypeEnum.Credit
                    case ChargeTypeEnum.Void:
                        res = "Refund attempt unsuccessful.";
                        break;
                    default:
                        res = "Transaction attempt unsuccessful.";
                        break;
                }
            }

            res += string.Format(" Amount: {0}.", Utility.FormatCurrency(usdAmount, null));
            if (currency != null)
            {
                res += string.Format(" {0} amount: {1}.", currency.CurrencyName, Utility.FormatCurrency(currencyAmount, currency.HtmlSymbol));
            }

            if (assertigyMID != null)
            {
                res += string.Format(" Merchant Account: {0} ({1}).", assertigyMID.MID, assertigyMID.DisplayName);
            }

            return res;
        }

        protected void AddNote(Billing billing, string note)
        {
            try
            {
                Notes notes = new Notes();
                notes.BillingID = (int)billing.BillingID;
                notes.AdminID = 0;
                notes.CreateDT = DateTime.Now;
                notes.Content = note;
                dao.Save<Notes>(notes);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
