using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Gateways.CBG;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Containers;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.NetworkMerchants;
//using TrimFuel.Business.Gateways.Iq2solutions;
using TrimFuel.Business.Gateways.DefaultEmail;
using TrimFuel.Business.Gateways.SelfHelpWorks;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Gateways.BadCustomer;
using TrimFuel.Business.Gateways.MaxMind;
using TrimFuel.Business.Gateways.MSC;
using TrimFuel.Business.Gateways.PrismPay;
using TrimFuel.Business.Gateways.IPG;
using System.Net;
using TrimFuel.Business.Gateways.MPS;
using TrimFuel.Business.Gateways.Pagador;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Gateways.BPagGateway;
using TrimFuel.Business.Flow;

namespace TrimFuel.Business
{
    public class SaleService : BaseService
    {
        //TODO: config
        //Settings
        public const bool CHARGE = true;
        private const bool DO_NOT_ALLOW_DUPES = true;
        private const int DEFAULT_INITIAL_INTERIM = 15;
        private DateTime NOT_RECURRING_NEXT_BILL_DATE = new DateTime(2020, 12, 31);

        //Test values
        private const string TEST_AFFILIATE_CODE = "custom1125";
        private const string TEST_CREDIT_CARD = "4111111111111111";

        private const string AFFILIATE_CODE_REC = "REC";
        private const string AFFILIATE_CODE_ADMIN = "Admin";

        //private IPaymentGateway paymentGateway = new NetworkMerchantsGateway();
        //private IFraudGateway fraudGateway = new Iq2solutionsGateway();
        private IFraudGateway fraudGateway = new MaxMindGateway();
        private ISHWGateway shwGateway = new SelfHelpWorksGateway();
        private IBadCustomerGateway badCustomerGateway = new BadCustomerGateway();

        private EmailService emailService { get { return new EmailService(); } }
        private MerchantService merchantService { get { return new MerchantService(); } }
        private RegistrationService registrationService { get { return new RegistrationService(); } }
        protected SubscriptionService subscriptionService { get { return new SubscriptionService(); } }
        private RefererService refererService { get { return new RefererService(); } }
        private BillingService billingService { get { return new BillingService(); } }

        public IPaymentGateway GetGatewayByMID(AssertigyMID mid)
        {
            if (mid == null)
            {
                throw new Exception("Can't determine Gateway. MID is null.");
            }
            if (mid.GatewayName == GatewayNameEnum.NMI)
            {
                return new NetworkMerchantsGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.MSC)
            {
                return new MSCGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.MPS)
            {
                return new MPSGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.IPG)
            {
                return new IPGGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.PrismPay)
            {
                return new PrismPayGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.CBG)
            {
                return new CBGGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.Pagador)
            {
                return new PagadorGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.BPAG)
            {
                return new BPagGateway();
            }
            else if (mid.GatewayName == GatewayNameEnum.BPAGRedecard)
            {
                return new BPagRedecardGateway();
            }
            throw new Exception(string.Format("Can't determine Gateway for MID({0}).", mid.AssertigyMIDID));
        }

        public BusinessError<Set<Registration, Billing>> CreateBillingSale(long? billingID, long? registrationID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode,
            int subscriptionID)
        {
            BusinessError<Set<Registration, Billing>> res = new BusinessError<Set<Registration, Billing>>();

            res.ReturnValue = registrationService.CreateOrUpdateRegistrationAndBilling(billingID, registrationID,
                firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                campaignID, affiliate, subAffiliate, ip, url,
                paymentTypeID, creditCard, cvv, expMonth, expYear);

            if (res.ReturnValue != null)
            {
                Subscription subscription = Load<Subscription>(subscriptionID);
                ICoupon coupon = registrationService.GetCampaignDiscount(couponCode, campaignID);
                Referer referer = refererService.GetByCode(refererCode);

                if (subscription != null)
                {
                    BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> billingSaleRes = CreateBillingSale(res.ReturnValue.Value2, subscription,
                        isSpecialOffer, coupon, referer, partnerClickClickID, null, null, null);

                    if (billingSaleRes != null)
                    {
                        res.State = billingSaleRes.State;
                        res.ErrorMessage = billingSaleRes.ErrorMessage;

                        return res;
                    }
                }
            }

            return null;
        }

        public BusinessError<Set<Registration, Billing>> CreateUpsellSale(long? billingID, long? registrationID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            string partnerClickClickID, string couponCode, string refererCode,
            int upsellTypeID, int quantity)
        {
            BusinessError<Set<Registration, Billing>> res = new BusinessError<Set<Registration, Billing>>();

            res.ReturnValue = registrationService.CreateOrUpdateRegistrationAndBilling(billingID, registrationID,
                firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                campaignID, affiliate, subAffiliate, ip, url,
                paymentTypeID, creditCard, cvv, expMonth, expYear);

            if (res.ReturnValue != null)
            {
                UpsellType upsellType = Load<UpsellType>(upsellTypeID);
                ICoupon coupon = registrationService.GetCampaignDiscount(couponCode, campaignID);
                Referer referer = refererService.GetByCode(refererCode);

                if (upsellType != null)
                {
                    BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> saleRes = CreateUpsellSale(res.ReturnValue.Value2, upsellType,
                        quantity, coupon, referer, partnerClickClickID, null, null, null, true);

                    if (saleRes != null)
                    {
                        res.State = saleRes.State;
                        res.ErrorMessage = saleRes.ErrorMessage;

                        return res;
                    }
                }
            }

            return null;
        }

        public string CreateTransactionNote(AssertigyMID assertigyMID, int chargeType, int? saleTypeID, decimal usdAmount, Currency currency, decimal? currencyAmount, bool success)
        {
            string res = string.Empty;
            if (success)
            {
                switch (chargeType)
                {
                    case ChargeTypeEnum.Charge:
                        switch (saleTypeID)
                        {
                            case SaleTypeEnum.Billing:
                                res = "Billing processed (Billing Sale).";
                                break;
                            case SaleTypeEnum.Upsell:
                                res = "Billing processed (Upsell).";
                                break;
                            case SaleTypeEnum.Rebill:
                                res = "Billing processed (Rebill).";
                                break;
                            default:
                                res = "Billing processed.";
                                break;
                        }
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

        public Set<Paygea, ChargeHistoryEx, FailedChargeHistoryView> ChargeLogging(BusinessError<GatewayResult> chargeResult, Billing billing,
            BillingSubscription billingSubscription, string productCode, int? saleTypeID,
            AssertigyMID assertigyMID, int chargeType, decimal amount, Currency currency)
        {
            Set<Paygea, ChargeHistoryEx, FailedChargeHistoryView> res = new Set<Paygea, ChargeHistoryEx, FailedChargeHistoryView>();

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

                Notes notes = new Notes();
                notes.BillingID = (int)billing.BillingID;
                notes.AdminID = 0;
                notes.CreateDT = DateTime.Now;
                notes.Content = CreateTransactionNote(assertigyMID, chargeType, saleTypeID, usdAmount, currency, currencyAmount, (chargeResult.State == BusinessErrorState.Success));
                dao.Save<Notes>(notes);

                if (billingSubscription != null)
                {
                    ChargeHistoryEx chargeHistory = new ChargeHistoryEx();
                    chargeHistory.MerchantAccountID = assertigyMID.AssertigyMIDID;
                    chargeHistory.Success = (chargeResult.State == BusinessErrorState.Success);
                    chargeHistory.ChargeTypeID = chargeType;
                    chargeHistory.Amount = usdAmount;
                    chargeHistory.ChargeDate = DateTime.Now;
                    chargeHistory.ChildMID = assertigyMID.MID;
                    chargeHistory.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                    try
                    {
                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMID);
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
                        ChargeDetails details = new ChargeDetails();
                        details.FillFromChargeHistory(chargeHistory);
                        details.SaleTypeID = saleTypeID;
                        details.SKU = productCode;
                        dao.Save<ChargeDetails>(details);
                        chargeHistory = details;

                        if (currency != null)
                        {
                            ChargeHistoryExCurrency chargeHistoryExCurrency = new ChargeHistoryExCurrency();
                            chargeHistoryExCurrency.ChargeHistoryID = details.ChargeHistoryID;
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

                    res.Value2 = chargeHistory;

                    if (chargeResult.State == BusinessErrorState.Success &&
                        !(chargeType == ChargeTypeEnum.AuthOnly || chargeType == ChargeTypeEnum.VoidAuthOnly))
                    {
                        merchantService.UpdateAssertigyMIDDailyCap(assertigyMID, usdAmount);
                    }
                }
                else
                {
                    if (chargeResult.State != BusinessErrorState.Success)
                    {
                        FailedChargeHistory failedChargeHistory = new FailedChargeHistory();
                        failedChargeHistory.BillingID = billing.BillingID;
                        failedChargeHistory.ChargeDate = DateTime.Now;
                        failedChargeHistory.Amount = usdAmount;
                        failedChargeHistory.Response = chargeResult.ReturnValue.Response;
                        failedChargeHistory.Success = false;
                        failedChargeHistory.SaleTypeID = saleTypeID;
                        failedChargeHistory.MerchantAccountID = assertigyMID.AssertigyMIDID;

                        if (chargeType == ChargeTypeEnum.AuthOnly)
                        {
                            AuthOnlyFailedChargeDetails details = new AuthOnlyFailedChargeDetails();
                            details.FillFromChargeHistory(failedChargeHistory);
                            details.RequestedAmount = usdAmount;
                            if (currency != null)
                            {
                                details.RequestedCurrencyID = currency.CurrencyID;
                                details.RequestedCurrencyAmount = currencyAmount;
                            }
                            //0.0 for AuthOnly
                            details.Amount = 0.0M;
                            dao.Save<FailedChargeHistory>(details);
                            failedChargeHistory = details;

                            res.Value3 = new FailedChargeHistoryView(details);
                            res.Value3.ChildMID = assertigyMID.MID;
                        }
                        else
                        {
                            dao.Save<FailedChargeHistory>(failedChargeHistory);

                            if (currency != null)
                            {
                                FailedChargeHistoryCurrency failedChargeHistoryCurrency = new FailedChargeHistoryCurrency();
                                failedChargeHistoryCurrency.FailedChargeHistoryID = failedChargeHistory.FailedChargeHistoryID;
                                failedChargeHistoryCurrency.CurrencyID = currency.CurrencyID;
                                failedChargeHistoryCurrency.CurrencyAmount = currencyAmount;
                                dao.Save<FailedChargeHistoryCurrency>(failedChargeHistoryCurrency);
                            }

                            res.Value3 = new FailedChargeHistoryView(failedChargeHistory);
                            res.Value3.ChildMID = assertigyMID.MID;
                        }
                    }
                }

                //Check response
                string stopChargeReason = null;
                if (CheckStopCharge(chargeResult.ReturnValue.Response, out stopChargeReason))
                {
                    BillingStopCharge stopCharge = new BillingStopCharge();
                    stopCharge.BillingID = billing.BillingID;
                    stopCharge.StopReason = stopChargeReason;
                    stopCharge.CreateDT = DateTime.Now;
                    dao.Save(stopCharge);

                    Notes stopNote = new Notes();
                    stopNote.BillingID = (int)billing.BillingID;
                    stopNote.AdminID = 0;
                    stopNote.Content = stopChargeReason;
                    stopNote.CreateDT = DateTime.Now;
                    dao.Save(stopNote);
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

        private bool CheckStopCharge(string paymentGatewayResponse, out string stopReason)
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

        public Sale GetSaleByChargeHistory(long chargeHistoryID)
        {
            Sale res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select s.* from Sale s " +
                    "left join BillingSale bs on bs.SaleID = s.SaleID " +
                    "left join UpsellSale us on us.SaleID = s.SaleID " +
                    "where bs.ChargeHistoryID = @chargeHistoryID or us.ChargeHistoryID = @chargeHistoryID " +
                    "limit 1");
                q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = chargeHistoryID;

                res = dao.Load<Sale>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public ChargeHistoryEx GetChargeHistoryBySale(long saleID)
        {
            ChargeHistoryEx res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"select ch.* from ChargeHistoryEx ch
                    inner join BillingSale bs on bs.ChargeHistoryID = ch.ChargeHistoryID
                    where bs.saleID = @saleID
                    limit 1
                    union
                    select ch.* from ChargeHistoryEx ch
                    inner join UpsellSale us on us.ChargeHistoryID = ch.ChargeHistoryID
                    where us.saleID = @saleID
                    limit 1
                    union 
                    select ch.* from ChargeHistoryEx ch 
                    inner join ChargeHistoryInvoice chi on chi.ChargeHistoryID = ch.ChargeHistoryID 
                    inner join OrderSale os on os.InvoiceID = chi.InvoiceID 
                    where os.saleID = @saleID 
                    limit 1");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                res = dao.Load<ChargeHistoryEx>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        //public IList<Set<BillingSale, BillingSubscription>> GetBillingSales(long parentBillingID)
        //{

        //}

        //public IList<Set<UpsellSale, UpsellType>> GetUpsellSales(long parentBillingID)
        //{

        //}

        private BusinessError<BillingBadCustomer> ValidateCustomer(Billing billing)
        {
            BusinessError<BillingBadCustomer> res = new BusinessError<BillingBadCustomer>();

            if (IsTestCreditCard(billing.CreditCard))
            {
                res.ReturnValue = null;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = "Test account";
                return res;
            }

            //Disabled on 2011-02-02
            return new BusinessError<BillingBadCustomer>(null, BusinessErrorState.Success, "Validation is disabled since 2011-02-02");

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select bbc.* from BillingBadCustomer bbc " +
                    "where bbc.BillingID = @billingID " +
                    "order by bbc.BillingBadCustomerID desc " +
                    "limit 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billing.BillingID;
                BillingBadCustomer c = dao.Load<BillingBadCustomer>(q).FirstOrDefault();

                if (c == null)
                {
                    BusinessError<GatewayResult> badCustomerResult = badCustomerGateway.ValidateCustomer(billing);

                    c = new BillingBadCustomer();
                    c.BillingID = billing.BillingID;
                    c.TransactionId = badCustomerResult.ReturnValue.ResponseParams.GetParam("transactionId");
                    c.Error = Utility.TryGetByte(badCustomerResult.ReturnValue.ResponseParams.GetParam("error"));
                    c.Found = Utility.TryGetByte(badCustomerResult.ReturnValue.ResponseParams.GetParam("found"));
                    c.Result = badCustomerResult.ReturnValue.ResponseParams.GetParam("result");
                    c.Request = badCustomerResult.ReturnValue.Request;
                    c.Response = badCustomerResult.ReturnValue.Response;
                    c.CreateDT = DateTime.Now;
                    dao.Save<BillingBadCustomer>(c);
                }

                res.ReturnValue = c;
                res.State = (c.Found == null || c.Found == 0) ? BusinessErrorState.Success : BusinessErrorState.Error;
                res.ErrorMessage = c.Result;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res.ReturnValue = null;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = "Unknown error occured.";
            }
            return res;
        }

        public BusinessError<BillingBadCustomer> ValidateCustomer(long billingID)
        {
            BusinessError<BillingBadCustomer> res = new BusinessError<BillingBadCustomer>();

            try
            {
                Billing billing = EnsureLoad<Billing>(billingID);
                res = ValidateCustomer(billing);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);

                res.ReturnValue = null;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = "Unknown error occured.";
            }
            return res;
        }

        //Iq2solutions implementation
        //private FraudScore SendFraud(Billing billing, Sale sale)
        //{
        //    if (IsTestCreditCard(billing.CreditCard) ||
        //        billing.Affiliate == AFFILIATE_CODE_REC || billing.Affiliate == AFFILIATE_CODE_ADMIN)
        //    {
        //        return null;
        //    }

        //    FraudScore res = null;
        //    try
        //    {
        //        dao.BeginTransaction();

        //        Registration registration = EnsureLoad<Registration>(billing.RegistrationID);

        //        BusinessError<GatewayResult> fraudResult = fraudGateway.SendFraudScore(billing, registration, sale);

        //        short? fraudScore = null;
        //        try
        //        {
        //            fraudScore = short.Parse(fraudResult.ReturnValue.ResponseParams.GetParam("FRAUDSCORE"));
        //        }
        //        catch { }

        //        FraudScore f = new FraudScore();
        //        f.SaleID = sale.SaleID;
        //        f.BillingID = billing.BillingID;
        //        f.FraudScore_ = fraudScore;
        //        f.Error = false;
        //        f.Request = fraudResult.ReturnValue.Request;
        //        f.Response = fraudResult.ReturnValue.Response;
        //        f.CreateDT = DateTime.Now;
        //        dao.Save<FraudScore>(f);

        //        dao.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(GetType(), ex);
        //        dao.RollbackTransaction();
        //        res = null;
        //    }
        //    return res;
        //}

        public Billing GetBillingBySale(long saleID)
        {
            Billing res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select b.* from Billing b " +
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " +
                    "inner join BillingSale bsale on bsale.ChargeHistoryID = ch.ChargeHistoryID " +
                    "where bsale.SaleID = @saleID " +
                    "union " +
                    "select b.* from Billing b " +
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " +
                    "inner join UpsellSale usale on usale.ChargeHistoryID = ch.ChargeHistoryID " +
                    "where usale.SaleID = @saleID " +
                    "union " +
                    "select b.* from Billing b " +
                    "inner join ExtraTrialShip ets on ets.BillingID = b.BillingID " +
                    "inner join ExtraTrialShipSale etssale on etssale.ExtraTrialShipID = ets.ExtraTrialShipID " +
                    "where etssale.SaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                res = dao.Load<Billing>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public BillingSubscription GetBillingSubscriptionBySale(long saleID)
        {
            BillingSubscription res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select bs.* from Billing b " +
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " +
                    "inner join BillingSale bsale on bsale.ChargeHistoryID = ch.ChargeHistoryID " +
                    "where bsale.SaleID = @saleID " +
                    "union " +
                    "select bs.* from Billing b " +
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " +
                    "inner join UpsellSale usale on usale.ChargeHistoryID = ch.ChargeHistoryID " +
                    "where usale.SaleID = @saleID " +
                    "union " +
                    "select bs.* from Billing b " +
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join ExtraTrialShip ets on ets.BillingID = b.BillingID " +
                    "inner join ExtraTrialShipSale etssale on etssale.ExtraTrialShipID = ets.ExtraTrialShipID " +
                    "where etssale.SaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int32).Value = saleID;

                res = dao.Load<BillingSubscription>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public BusinessError<FraudScore> ValidateFraud(long billingID, long saleID)
        {
            BusinessError<FraudScore> res = new BusinessError<FraudScore>();

            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                Sale s = EnsureLoad<Sale>(saleID);

                res = ValidateFraud(b, s);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);

                res.ReturnValue = null;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = "Unknown error occured.";
            }
            return res;
        }

        public FraudScore GetFraudScoreByBilling(long billingID)
        {
            FraudScore res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select fs.* from FraudScore fs " +
                    "where fs.BillingID = @billingID " +
                    "order by fs.ID desc " +
                    "limit 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<FraudScore>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        private BusinessError<FraudScore> ValidateFraud(Billing billing, Sale sale)
        {
            BusinessError<FraudScore> res = new BusinessError<FraudScore>();

            if (IsTestCreditCard(billing.CreditCard))
            {
                res.ReturnValue = null;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = "Test account";
                return res;
            }

            try
            {
                dao.BeginTransaction();

                FraudScore fs = GetFraudScoreByBilling(billing.BillingID.Value);
                if (fs == null)
                {
                    Registration registration = EnsureLoad<Registration>(billing.RegistrationID);
                    RegistrationInfo regInfo = registrationService.GetRegistrationInfo(registration.RegistrationID.Value);

                    BusinessError<GatewayResult> fraudResult = fraudGateway.SendFraudScore(billing, registration, regInfo, sale);

                    fs = new FraudScore();
                    fs.BillingID = billing.BillingID;
                    fs.SaleID = sale.SaleID;
                    decimal? fraudScore_ = Utility.TryGetDecimal(fraudResult.ReturnValue.ResponseParams.GetParam("riskScore"));
                    fs.FraudScore_ = (fraudScore_ != null) ? (short?)(Convert.ToInt16(fraudScore_.Value)) : null;
                    fs.Error = (fs.FraudScore_ == null);
                    fs.Request = fraudResult.ReturnValue.Request;
                    fs.Response = fraudResult.ReturnValue.Response;
                    fs.CreateDT = DateTime.Now;
                    fs.ResponseBinName = fraudResult.ReturnValue.BinName;
                    dao.Save<FraudScore>(fs);
                }

                res.ReturnValue = fs;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = null;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res.ReturnValue = null;
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = "Unknown error occured.";
            }

            return res;
        }

        private SHWRegistration SendSHW(Billing billing, Subscription subscription, DateTime createDT)
        {
            if (IsTestCreditCard(billing.CreditCard))
            {
                return null;
            }

            SHWRegistration shw = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select count(*) from CampaignInfo " +
                    "where CampaignID = @campaignID and SendSHW = 1");
                q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = billing.CampaignID;

                int? count = dao.ExecuteScalar<int>(q);
                bool campaignIsSHW = false;

                if (count != 0 || campaignIsSHW)
                {
                    q = new MySqlCommand("select * from SHWProduct " +
                        "where ProductID = @productID");
                    q.Parameters.Add("@productID", MySqlDbType.Int32).Value = subscription.ProductID;

                    SHWProduct shwProduct = dao.Load<SHWProduct>(q).FirstOrDefault();
                    if (shwProduct == null)
                    {
                        throw new Exception(string.Format("Can't find SHWProduct for Product({0})", subscription.ProductID));
                    }

                    BusinessError<GatewayResult> shwResult = shwGateway.SendSHW(shwProduct, billing);

                    string shwID = shwResult.ReturnValue.ResponseParams.GetParam("string");
                    if (!string.IsNullOrEmpty(shwID) && shwID.Split('|').Length > 1)
                    {
                        shwID = shwID.Split('|')[0];
                    }

                    shw = new SHWRegistration();
                    shw.BillingID = billing.BillingID;
                    shw.Request = shwResult.ReturnValue.Request;
                    shw.Response = shwResult.ReturnValue.Response;
                    shw.SHWID = shwID;
                    shw.CreateDT = createDT;
                    dao.Save<SHWRegistration>(shw);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                shw = null;
            }
            return shw;
        }

        private bool IsCreditCardDupe(string creditCard)
        {
            if (string.IsNullOrEmpty(creditCard))
            {
                return false;
            }
            if (IsTestCreditCard(creditCard))
            {
                return false;
            }

            bool res = false;
            try
            {
                CreditCard cc = new CreditCard(creditCard);

                MySqlCommand q = new MySqlCommand("select b.* from Billing b " +
                    "inner join BillingSubscription bs on b.BillingID = bs.BillingID " +
                    "where b.CreditCard like @creditCard");
                q.Parameters.Add("@creditCard", MySqlDbType.VarChar).Value = "%" + cc.SearchHashCode;

                IList<Billing> matches = dao.Load<Billing>(q);
                foreach (Billing match in matches)
                {
                    if (match.CreditCardCnt.DecryptedCreditCard == cc.DecryptedCreditCard)
                    {
                        res = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }

        public BusinessError<bool> ProcessRefundRequest(long billingID)
        {
            BusinessError<bool> res = new BusinessError<bool>();
            BusinessError<GatewayResult> refundResult = DoRefundOrVoidAndUpdateBillingSubscriptionToBeReturned(billingID);
            BusinessError<GatewayResult> creditResult = null;
            if (refundResult != null && refundResult.State == BusinessErrorState.Success)
            {
                res.State = BusinessErrorState.Success;
                res.ReturnValue = true;
            }
            else if (refundResult != null && refundResult.State == BusinessErrorState.Error)
            {
                creditResult = DoCredit(billingID);
                if (creditResult != null && creditResult.State == BusinessErrorState.Success)
                {
                    res.State = BusinessErrorState.Success;
                    res.ReturnValue = false; // Refund fails, Credit successful                    
                }
            }
            else
            {
                res.State = BusinessErrorState.Error;
                res.ReturnValue = false;
                res.ErrorMessage = "Unknown error occured while trying to refund.";
            }
            return res;
        }

        private BusinessError<GatewayResult> DoCredit(long billingID)
        {
            BusinessError<GatewayResult> creditResult = null;

            try
            {
                dao.BeginTransaction();

                ChargeHistoryEx chargeHistory = GetLastSuccessfulCharge(billingID);
                if (chargeHistory != null)
                {
                    BillingSubscription billingSubscription = EnsureLoad<BillingSubscription>(chargeHistory.BillingSubscriptionID);
                    Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);
                    Billing billing = EnsureLoad<Billing>(billingSubscription.BillingID);

                    Sale sale = GetSaleByChargeHistory(chargeHistory.ChargeHistoryID.Value);
                    if (sale == null)
                    {
                        throw new Exception(string.Format("Can't find Sale for ChargeHistoryEx({0})", chargeHistory.ChargeHistoryID));
                    }

                    AssertigyMID assertigyMID = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);

                    NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMID.AssertigyMIDID);
                    if (nmiCompany == null)
                    {
                        throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", assertigyMID.AssertigyMIDID));
                    }

                    decimal amount = chargeHistory.Amount.Value;
                    Currency cur = null;
                    Set<Currency, ChargeHistoryExCurrency> chCur = GetCurrencyByCharge(chargeHistory.ChargeHistoryID.Value);
                    if (chCur != null)
                    {
                        amount = chCur.Value2.CurrencyAmount.Value;
                        cur = chCur.Value1;
                    }

                    IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMID);
                    creditResult = paymentGateway.Credit(assertigyMID.MID, nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur, sale.SaleID.Value, billing);
                    ChargeLogging(creditResult, billing, billingSubscription, subscription.ProductCode, sale.SaleTypeID.Value, assertigyMID, ChargeTypeEnum.Credit, -amount, cur);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                creditResult = null;
            }

            return creditResult;
        }

        private BusinessError<GatewayResult> DoRefundOrVoidAndUpdateBillingSubscriptionToBeReturned(long billingID)
        {
            BusinessError<GatewayResult> refundResult = DoRefundOrVoid(billingID);
            if (refundResult != null && refundResult.State == BusinessErrorState.Success)
            {
                try
                {
                    dao.BeginTransaction();

                    MySqlCommand q = new MySqlCommand("update RefundAuthorizationList set Complete = 1 where BillingID = @billingID");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                    dao.ExecuteNonQuery(q);

                    q = new MySqlCommand("update RefundDiscretionaryList set Complete = 1 where BillingID = @billingID");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                    dao.ExecuteNonQuery(q);

                    q = new MySqlCommand("update BillingSubscription set StatusTID = 4 where BillingID = @billingID");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                    dao.ExecuteNonQuery(q);

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    logger.Error(GetType(), ex);
                }
            }

            return refundResult;
        }

        public BusinessError<GatewayResult> DoRefundOrVoid(ChargeHistoryEx chargeHistory, decimal refundAmount)
        {
            BusinessError<GatewayResult> refundResult = null;

            try
            {
                dao.BeginTransaction();

                BillingSubscription billingSubscription = EnsureLoad<BillingSubscription>(chargeHistory.BillingSubscriptionID);
                Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);
                Billing billing = EnsureLoad<Billing>(billingSubscription.BillingID);

                Sale sale = GetSaleByChargeHistory(chargeHistory.ChargeHistoryID.Value);
                if (sale == null)
                {
                    throw new Exception(string.Format("Can't find Sale for ChargeHistoryEx({0})", chargeHistory.ChargeHistoryID));
                }

                AssertigyMID assertigyMID = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);

                NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMID.AssertigyMIDID);
                if (nmiCompany == null)
                {
                    throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", assertigyMID.AssertigyMIDID));
                }

                decimal chargeCurrencyAmount = chargeHistory.Amount.Value;
                Currency cur = null;
                Set<Currency, ChargeHistoryExCurrency> chCur = GetCurrencyByCharge(chargeHistory.ChargeHistoryID.Value);
                if (chCur != null)
                {
                    chargeCurrencyAmount = chCur.Value2.CurrencyAmount.Value;
                    cur = chCur.Value1;
                }

                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMID);

                Set<Paygea, ChargeHistoryEx> chRes = null;
                if (chargeHistory.ChargeDate.Value.Date == DateTime.Today)
                {
                    if (chargeCurrencyAmount == refundAmount)
                    {
                        refundResult = paymentGateway.Void(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, chargeHistory, refundAmount, cur);
                        chRes = ChargeLogging(refundResult, billing, billingSubscription, subscription.ProductCode, sale.SaleTypeID.Value, assertigyMID, ChargeTypeEnum.Void, -refundAmount, cur);
                    }
                    else
                    {
                        //TODO: add to VoidQueue
                        refundResult = new BusinessError<GatewayResult>(null, BusinessErrorState.Error, "Can't perform partial Void");
                    }
                }
                else
                {
                    //do refund
                    refundResult = paymentGateway.Refund(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, chargeHistory, refundAmount, cur);
                    chRes = ChargeLogging(refundResult, billing, billingSubscription, subscription.ProductCode, sale.SaleTypeID.Value, assertigyMID, ChargeTypeEnum.Refund, -refundAmount, cur);
                }

                if (refundResult.State == BusinessErrorState.Success)
                    new EmailService().SendRefundEmail(subscription.ProductID.Value, billing, refundAmount);

                if (chRes != null)
                {
                    SaleRefund sr = new SaleRefund();
                    sr.SaleID = sale.SaleID;
                    sr.ChargeHistoryID = chRes.Value2.ChargeHistoryID;
                    dao.Save<SaleRefund>(sr);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                refundResult = null;
            }


            return refundResult;
        }

        public BusinessError<GatewayResult> DoRefundOrVoid(long billingID)
        {
            BusinessError<GatewayResult> refundResult = null;

            try
            {
                ChargeHistoryEx chargeHistory = GetLastSuccessfulCharge(billingID);
                if (chargeHistory == null)
                {
                    throw new Exception(string.Format("Last Successful Charge for Billing({0}) was not found", billingID));
                }

                DoRefundOrVoid(chargeHistory, chargeHistory.Amount.Value);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                refundResult = null;
            }

            return refundResult;
        }

        public ChargeHistoryEx GetLastSuccessfulCharge(long billingID)
        {
            ChargeHistoryEx res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select ch.* from ChargeHistoryEx ch " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "where b.BillingID = @billingID and Success = 1 and Amount > 0 " +
                    "order by ch.ChargeHistoryID desc " +
                    "limit 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<ChargeHistoryEx>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public ChargeHistoryEx GetLastSuccessfulCharge(long billingID, int productID)
        {
            ChargeHistoryEx res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select ch.* from ChargeHistoryEx ch " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "where b.BillingID = @billingID and s.ProductID = @productID and ch.Success = 1 and ch.Amount > 0 " +
                    "order by ch.ChargeHistoryID desc " +
                    "limit 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                q.Parameters.Add("@productID", MySqlDbType.Int64).Value = productID;

                res = dao.Load<ChargeHistoryEx>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public bool RemoveRefundRequest(long billingID)
        {
            bool res = true;

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("update RefundAuthorizationList set Complete = 1 where BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = false;
            }

            return res;
        }

        private BillingReferred CreateBillingReferred(long billingID, long referralBillingID)
        {
            BillingReferred res = null;

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select br.* from BillingReferred br " +
                    "where br.BillingID = @billingID and br.ReferralBillingID = @referralBillingID " +
                    "limit 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                q.Parameters.Add("@referralBillingID", MySqlDbType.Int64).Value = referralBillingID;

                res = dao.Load<BillingReferred>(q).FirstOrDefault();
                if (res == null)
                {
                    res = new BillingReferred();
                    res.BillingID = (int)billingID;
                    res.ReferralBillingID = (int)referralBillingID;
                    res.ExtraGiftCompleted = false;
                    res.CreateDT = DateTime.Now;
                    dao.Save<BillingReferred>(res);
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

        public BusinessError<ComplexSaleView> CreateComplexSale_Old_ChargeForEachUpsell(int? ownRefererID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode, decimal? refererCommissionRedeem, IList<string> giftCerificates,
            IEnumerable<KeyValuePair<int, int>> subscriptions, IEnumerable<KeyValuePair<int, int>> upsellTypes)
        {
            if (subscriptions.Count() == 0 && upsellTypes.Count() == 0)
            {
                return null;
            }

            BusinessError<ComplexSaleView> res = new BusinessError<ComplexSaleView>();
            res.ReturnValue = new ComplexSaleView();
            res.ReturnValue.BillingSales = new List<Set<BillingSale, BillingSubscription, Subscription>>();
            res.ReturnValue.BillingFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>>();
            res.ReturnValue.UpsellSales = new List<Set<UpsellSale, BillingSubscription, Upsell, UpsellType>>();
            res.ReturnValue.UpsellFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType>>();
            res.State = BusinessErrorState.Error;

            try
            {
                long? parentBillingID = null;
                if (ownRefererID != null)
                {
                    Billing lastBilling = refererService.GetLastBillingByReferer(ownRefererID.Value);
                    if (lastBilling != null)
                    {
                        parentBillingID = lastBilling.BillingID;
                    }
                }

                long? registrationID = null;
                if (parentBillingID != null)
                {
                    Billing billing = Load<Billing>(parentBillingID);
                    registrationID = (billing != null) ? billing.RegistrationID : null;
                }
                Set<Registration, Billing> parent = registrationService.CreateOrUpdateRegistrationAndBilling(parentBillingID, registrationID,
                    firstName, lastName, address1, address2, city, state, zip, country,
                    phone, email,
                    campaignID, affiliate, subAffiliate, ip, url,
                    paymentTypeID, creditCard, cvv, expMonth, expYear);

                if (parent == null)
                {
                    throw new Exception("Can't create/update Billing and Registration");
                }
                else
                {
                    res.ReturnValue.Registration = parent.Value1;
                    res.ReturnValue.ParentBilling = parent.Value2;
                }

                ICoupon coupon = registrationService.GetCampaignDiscount(couponCode, campaignID);
                res.ReturnValue.Coupon = coupon;

                Referer referer = refererService.GetByCode(refererCode);
                res.ReturnValue.Referer = referer;

                Referer ownReferer = null;
                if (ownRefererID != null)
                {
                    ownReferer = Load<Referer>(ownRefererID);
                }
                else
                {
                    BusinessError<Referer> createRefererResult = refererService.CreateOrGetRefererFromBilling(res.ReturnValue.ParentBilling, referer);
                    if (createRefererResult != null && createRefererResult.State == BusinessErrorState.Success)
                    {
                        ownReferer = createRefererResult.ReturnValue;
                    }
                }
                if (ownReferer != null)
                {
                    refererService.AddBillingToReferer(ownReferer, res.ReturnValue.ParentBilling);
                }

                foreach (KeyValuePair<int, int> s in subscriptions)
                {
                    Subscription subscription = Load<Subscription>(s.Key);
                    if (subscription != null)
                    {
                        for (int i = 0; i < s.Value; i++)
                        {
                            IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                            decimal saleAmount = CalculateBillingSaleAmount(subscription, isSpecialOffer, coupon, null);
                            decimal saleRefererCommissionRedeem = (refererCommissionRedeem == null ? 0M : CalculateRedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount));
                            decimal salePromoGiftRedeem = CalculateRedeemPromoGiftAmount(promoGiftList, saleAmount - saleRefererCommissionRedeem);
                            if (saleAmount > saleRefererCommissionRedeem + salePromoGiftRedeem)
                            {
                                BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> saleRes = CreateBillingSale(res.ReturnValue.ParentBilling, subscription,
                                    isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                                if (saleRes != null)
                                {
                                    if (saleRes.State == BusinessErrorState.Success)
                                    {
                                        res.State = BusinessErrorState.Success;
                                        res.ReturnValue.BillingSales.Add(
                                            new Set<BillingSale, BillingSubscription, Subscription>()
                                            {
                                                Value1 = saleRes.ReturnValue.Value1,
                                                Value2 = saleRes.ReturnValue.Value2,
                                                Value3 = subscription
                                            });
                                        res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? saleRes.ReturnValue.Value3;

                                        if (refererCommissionRedeem != null)
                                        {
                                            refererCommissionRedeem -= saleRefererCommissionRedeem;
                                        }
                                    }
                                    else
                                    {
                                        res.ErrorMessage = saleRes.ErrorMessage;
                                    }
                                }
                            }
                            else
                            {
                                Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> saleRes = CreateExtraTrialShipSale_BillingSale(res.ReturnValue.ParentBilling, subscription,
                                    isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                                if (saleRes != null)
                                {
                                    res.State = BusinessErrorState.Success;
                                    res.ReturnValue.BillingFreeSales.Add(saleRes);

                                    if (refererCommissionRedeem != null)
                                    {
                                        refererCommissionRedeem -= saleRefererCommissionRedeem;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<int, int> upsell in upsellTypes)
                {
                    UpsellType upsellType = Load<UpsellType>(upsell.Key);
                    if (upsellType != null)
                    {
                        IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                        decimal saleAmount = CalculateUpsellSaleAmount(upsellType, upsell.Value, coupon, null);
                        decimal saleRefererCommissionRedeem = (refererCommissionRedeem == null ? 0M : CalculateRedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount));
                        decimal salePromoGiftRedeem = CalculateRedeemPromoGiftAmount(promoGiftList, saleAmount - saleRefererCommissionRedeem);
                        if (saleAmount > saleRefererCommissionRedeem + salePromoGiftRedeem)
                        {
                            BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> upsellRes = CreateUpsellSale(res.ReturnValue.ParentBilling, upsellType,
                                upsell.Value, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, true);
                            if (upsellRes != null)
                            {
                                if (upsellRes.State == BusinessErrorState.Success)
                                {
                                    res.State = BusinessErrorState.Success;
                                    res.ReturnValue.UpsellSales.Add(
                                        new Set<UpsellSale, BillingSubscription, Upsell, UpsellType>()
                                        {
                                            Value1 = upsellRes.ReturnValue.Value1,
                                            Value2 = upsellRes.ReturnValue.Value2,
                                            Value3 = upsellRes.ReturnValue.Value3,
                                            Value4 = upsellType
                                        });
                                    res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? upsellRes.ReturnValue.Value4;

                                    if (refererCommissionRedeem != null)
                                    {
                                        refererCommissionRedeem -= saleRefererCommissionRedeem;
                                    }
                                }
                                else
                                {
                                    res.ErrorMessage = upsellRes.ErrorMessage;
                                }
                            }
                        }
                        else
                        {
                            Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> saleRes = CreateExtraTrialShipSale_UpsellSale(res.ReturnValue.ParentBilling, upsellType,
                                upsell.Value, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                            if (saleRes != null)
                            {
                                res.State = BusinessErrorState.Success;
                                res.ReturnValue.UpsellFreeSales.Add(saleRes);

                                if (refererCommissionRedeem != null)
                                {
                                    refererCommissionRedeem -= saleRefererCommissionRedeem;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public BusinessError<ComplexSaleView> CreateComplexSale(long? parentBillingID, long? registrationID,
            int? ownRefererID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2, string shippingCity, string shippingState, string shippingZip, string shippingCountry,
            string shippingPhone, string shippingEmail,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode, decimal? refererCommissionRedeem, IList<string> giftCerificates,
            IEnumerable<KeyValuePair<int, int>> subscriptions, IEnumerable<KeyValuePair<int, int>> upsellTypes)
        {
            if (subscriptions.Count() == 0 && upsellTypes.Count() == 0)
            {
                return null;
            }

            BusinessError<ComplexSaleView> res = new BusinessError<ComplexSaleView>();
            res.ReturnValue = new ComplexSaleView();
            res.ReturnValue.BillingSales = new List<Set<BillingSale, BillingSubscription, Subscription>>();
            res.ReturnValue.BillingFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>>();
            res.ReturnValue.UpsellSales = new List<Set<UpsellSale, BillingSubscription, Upsell, UpsellType>>();
            res.ReturnValue.UpsellFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType>>();
            res.State = BusinessErrorState.Error;

            try
            {
                if (parentBillingID == null && ownRefererID != null)
                {
                    Billing lastBilling = refererService.GetLastBillingByReferer(ownRefererID.Value);
                    if (lastBilling != null)
                    {
                        parentBillingID = lastBilling.BillingID;
                    }
                }

                if (parentBillingID != null && registrationID == null)
                {
                    Billing billing = Load<Billing>(parentBillingID);
                    registrationID = (billing != null) ? billing.RegistrationID : null;
                }


                Set<Registration, Billing> parent = registrationService.CreateOrUpdateRegistrationAndBilling(parentBillingID, registrationID,
                    firstName, lastName, address1, address2, city, state, zip, country,
                    phone, email,
                    shippingFirstName, shippingLastName, shippingAddress1, shippingAddress2, shippingCity, shippingState, shippingZip, shippingCountry,
                    shippingPhone, shippingEmail,
                    campaignID, affiliate, subAffiliate, ip, url,
                    paymentTypeID, creditCard, cvv, expMonth, expYear);



                if (parent == null)
                {
                    throw new Exception("Can't create/update Billing and Registration");
                }
                else
                {
                    res.ReturnValue.Registration = parent.Value1;
                    res.ReturnValue.ParentBilling = parent.Value2;
                }

                ICoupon coupon = registrationService.GetCampaignDiscount(couponCode, campaignID);
                res.ReturnValue.Coupon = coupon;

                Referer referer = refererService.GetByCode(refererCode);
                res.ReturnValue.Referer = referer;

                Referer ownReferer = null;
                if (ownRefererID != null)
                {
                    ownReferer = Load<Referer>(ownRefererID);
                }
                else
                {
                    BusinessError<Referer> createRefererResult = refererService.CreateOrGetRefererFromBilling(res.ReturnValue.ParentBilling, referer);
                    if (createRefererResult != null && createRefererResult.State == BusinessErrorState.Success)
                    {
                        ownReferer = createRefererResult.ReturnValue;
                    }
                }
                if (ownReferer != null)
                {
                    refererService.AddBillingToReferer(ownReferer, res.ReturnValue.ParentBilling);
                }

                foreach (KeyValuePair<int, int> s in subscriptions)
                {
                    Subscription subscription = Load<Subscription>(s.Key);
                    if (subscription != null)
                    {
                        for (int i = 0; i < s.Value; i++)
                        {
                            IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                            decimal saleAmount = CalculateBillingSaleAmount(subscription, isSpecialOffer, coupon, null);
                            decimal saleRefererCommissionRedeem = (refererCommissionRedeem == null ? 0M : CalculateRedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount));
                            decimal salePromoGiftRedeem = CalculateRedeemPromoGiftAmount(promoGiftList, saleAmount - saleRefererCommissionRedeem);
                            if (saleAmount > saleRefererCommissionRedeem + salePromoGiftRedeem)
                            {
                                BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> saleRes = CreateBillingSale(res.ReturnValue.ParentBilling, subscription,
                                    isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                                if (saleRes != null)
                                {
                                    if (saleRes.State == BusinessErrorState.Success)
                                    {
                                        res.State = BusinessErrorState.Success;
                                        res.ReturnValue.BillingSales.Add(
                                            new Set<BillingSale, BillingSubscription, Subscription>()
                                            {
                                                Value1 = saleRes.ReturnValue.Value1,
                                                Value2 = saleRes.ReturnValue.Value2,
                                                Value3 = subscription
                                            });
                                        res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? saleRes.ReturnValue.Value3;

                                        if (refererCommissionRedeem != null)
                                        {
                                            refererCommissionRedeem -= saleRefererCommissionRedeem;
                                        }
                                    }
                                    else
                                    {
                                        res.ErrorMessage = saleRes.ErrorMessage;
                                    }
                                }
                            }
                            else
                            {
                                Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> saleRes = CreateExtraTrialShipSale_BillingSale(res.ReturnValue.ParentBilling, subscription,
                                    isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                                if (saleRes != null)
                                {
                                    res.State = BusinessErrorState.Success;
                                    res.ReturnValue.BillingFreeSales.Add(saleRes);

                                    if (refererCommissionRedeem != null)
                                    {
                                        refererCommissionRedeem -= saleRefererCommissionRedeem;
                                    }
                                }
                            }
                        }
                    }
                }
                IList<KeyValuePair<UpsellType, int>> upsellTypeListForCharge = new List<KeyValuePair<UpsellType, int>>();
                foreach (KeyValuePair<int, int> upsell in upsellTypes)
                {
                    UpsellType upsellType = Load<UpsellType>(upsell.Key);
                    if (upsellType != null)
                    {
                        IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                        decimal saleAmount = CalculateUpsellSaleAmount(upsellType, upsell.Value, coupon, null);
                        decimal saleRefererCommissionRedeem = (refererCommissionRedeem == null ? 0M : CalculateRedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount));
                        decimal salePromoGiftRedeem = CalculateRedeemPromoGiftAmount(promoGiftList, saleAmount - saleRefererCommissionRedeem);
                        if (saleAmount > saleRefererCommissionRedeem + salePromoGiftRedeem)
                        {
                            //Add for single charge later
                            upsellTypeListForCharge.Add(new KeyValuePair<UpsellType, int>(upsellType, upsell.Value));
                        }
                        else
                        {
                            //Process free item
                            Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> saleRes = CreateExtraTrialShipSale_UpsellSale(res.ReturnValue.ParentBilling, upsellType,
                                upsell.Value, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                            if (saleRes != null)
                            {
                                res.State = BusinessErrorState.Success;
                                res.ReturnValue.UpsellFreeSales.Add(saleRes);

                                if (refererCommissionRedeem != null)
                                {
                                    refererCommissionRedeem -= saleRefererCommissionRedeem;
                                }
                            }
                        }
                    }
                }
                if (upsellTypeListForCharge.Count > 0)
                {
                    IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                    BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> upsellsRes =
                        CreateUpsellSales(res.ReturnValue.ParentBilling, upsellTypeListForCharge, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList);
                    if (upsellsRes != null)
                    {
                        if (upsellsRes.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                            res.ReturnValue.UpsellSales.AddRange(
                                upsellsRes.ReturnValue.Select(upsellRes =>
                                    new Set<UpsellSale, BillingSubscription, Upsell, UpsellType>()
                                    {
                                        Value1 = upsellRes.Value1,
                                        Value2 = upsellRes.Value2,
                                        Value3 = upsellRes.Value3,
                                        Value4 = upsellRes.Value5
                                    }));
                            AssertigyMID upsellsAssertigyMid = null;
                            if (upsellsRes.ReturnValue.Count > 0)
                            {
                                upsellsAssertigyMid = upsellsRes.ReturnValue.First().Value4;
                            }
                            res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? upsellsAssertigyMid;
                        }
                        else
                        {
                            res.ErrorMessage = upsellsRes.ErrorMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }


        public BusinessError<ComplexSaleView> CreateComplexSaleFromDynamicCampaign(long? parentBillingID, long? registrationID, int? ownRefererID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2, string shippingCity, string shippingState, string shippingZip, string shippingCountry,
            string shippingPhone, string shippingEmail,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode, decimal? refererCommissionRedeem, IList<string> giftCerificates,
            IEnumerable<KeyValuePair<int, int>> subscriptions, IEnumerable<KeyValuePair<int, int>> upsellTypes)
        {
            bool isNew = true;
            if (parentBillingID != null)
                isNew = false;
            else
                if (ownRefererID != null)
                {
                    Billing lastBilling = refererService.GetLastBillingByReferer(ownRefererID.Value);
                    if (lastBilling != null)
                        isNew = false;
                }


            var res = CreateComplexSale(parentBillingID, registrationID, ownRefererID, firstName, lastName, address1, address2, city, state, zip, country,
                phone, email, shippingFirstName, shippingLastName, shippingAddress1, shippingAddress2, shippingCity, shippingState, shippingZip,
                shippingCountry, shippingPhone, shippingEmail, campaignID, affiliate, subAffiliate, ip,
                url, paymentTypeID, creditCard, cvv, expMonth, expYear, partnerClickClickID, isSpecialOffer, couponCode,
                refererCode, refererCommissionRedeem, giftCerificates, subscriptions, upsellTypes);

            if (res != null && isNew)
            {
                try
                {
                    new EventService().RegistrationAndConfirmation(campaignID, null, email, zip, phone, firstName, lastName, res.ReturnValue.Registration.RegistrationID, EventTypeEnum.OrderConfirmation);
                }
                catch
                { }
            }

            return res;
        }

        public BusinessError<ComplexSaleView> CreateComplexSale(int? ownRefererID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode, decimal? refererCommissionRedeem, IList<string> giftCerificates,
            IEnumerable<KeyValuePair<int, int>> subscriptions, IEnumerable<KeyValuePair<int, int>> upsellTypes)
        {
            return CreateComplexSale(ownRefererID, firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                firstName, lastName, address1, address2, city, state, zip, country, phone, email,
                null,
                campaignID, affiliate, subAffiliate, ip, url, paymentTypeID, creditCard, cvv, expMonth, expYear,
                partnerClickClickID, isSpecialOffer, couponCode, refererCode, refererCommissionRedeem,
                giftCerificates, subscriptions, upsellTypes, null);
        }

        public BusinessError<ComplexSaleView> CreateComplexSale(int? ownRefererID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            string password,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode, decimal? refererCommissionRedeem, IList<string> giftCerificates,
            IEnumerable<KeyValuePair<int, int>> subscriptions, IEnumerable<KeyValuePair<int, int>> upsellTypes, decimal? shippingAmount)
        {
            return CreateComplexSale(ownRefererID, firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                firstName, lastName, address1, address2, city, state, zip, country, phone, email,
                password,
                campaignID, affiliate, subAffiliate, ip, url, paymentTypeID, creditCard, cvv, expMonth, expYear,
                partnerClickClickID, isSpecialOffer, couponCode, refererCode, refererCommissionRedeem,
                giftCerificates, subscriptions, upsellTypes, shippingAmount);
        }

        public BusinessError<ComplexSaleView> CreateComplexSale(int? ownRefererID,
                    string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
                    string phone, string email,
                    string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2, string shippingCity, string shippingState, string shippingZip, string shippingCountry,
                    string shippingPhone, string shippingEmail,
                    string password,
                    int? campaignID, string affiliate, string subAffiliate, string ip, string url,
                    int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
                    string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode, decimal? refererCommissionRedeem, IList<string> giftCerificates,
                    IEnumerable<KeyValuePair<int, int>> subscriptions, IEnumerable<KeyValuePair<int, int>> upsellTypes, decimal? shippingAmount)
        {
            if (subscriptions.Count() == 0 && upsellTypes.Count() == 0)
            {
                return null;
            }

            BusinessError<ComplexSaleView> res = new BusinessError<ComplexSaleView>();
            res.ReturnValue = new ComplexSaleView();
            res.ReturnValue.BillingSales = new List<Set<BillingSale, BillingSubscription, Subscription>>();
            res.ReturnValue.BillingFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>>();
            res.ReturnValue.UpsellSales = new List<Set<UpsellSale, BillingSubscription, Upsell, UpsellType>>();
            res.ReturnValue.UpsellFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType>>();
            res.State = BusinessErrorState.Error;

            try
            {
                long? parentBillingID = null;
                if (parentBillingID == null && ownRefererID != null)
                {
                    Billing lastBilling = refererService.GetLastBillingByReferer(ownRefererID.Value);
                    if (lastBilling != null)
                    {
                        parentBillingID = lastBilling.BillingID;
                    }
                }

                long? registrationID = null;
                if (parentBillingID != null && registrationID == null)
                {
                    Billing billing = Load<Billing>(parentBillingID);
                    registrationID = (billing != null) ? billing.RegistrationID : null;
                }
                Set<Registration, Billing> parent = registrationService.CreateOrUpdateRegistrationAndBilling(parentBillingID, registrationID,
                    firstName, lastName, address1, address2, city, state, zip, country,
                    phone, email, shippingFirstName, shippingLastName, shippingAddress1, shippingAddress2, shippingCity, shippingState, shippingZip, shippingCountry,
                    shippingPhone, shippingEmail,
                    campaignID, affiliate, subAffiliate, ip, url,
                    paymentTypeID, creditCard, cvv, expMonth, expYear);

                if (parent == null)
                {
                    throw new Exception("Can't create/update Billing and Registration");
                }
                else
                {
                    res.ReturnValue.Registration = parent.Value1;
                    res.ReturnValue.ParentBilling = parent.Value2;
                }

                ICoupon coupon = registrationService.GetCampaignDiscount(couponCode, campaignID);
                res.ReturnValue.Coupon = coupon;

                Referer referer = refererService.GetByCode(refererCode);
                res.ReturnValue.Referer = referer;

                Referer ownReferer = null;
                if (ownRefererID != null)
                {
                    ownReferer = Load<Referer>(ownRefererID);
                }
                else
                {
                    BusinessError<Referer> createRefererResult = refererService.CreateOrGetRefererFromBilling(res.ReturnValue.ParentBilling, referer, password);
                    if (createRefererResult != null && createRefererResult.State == BusinessErrorState.Success)
                    {
                        ownReferer = createRefererResult.ReturnValue;
                    }
                }
                if (ownReferer != null)
                {
                    refererService.AddBillingToReferer(ownReferer, res.ReturnValue.ParentBilling);
                }

                int? shippingOption = null;
                if (shippingAmount > 0)
                    shippingOption = ShippingOptionEnum.PriorityShipping;

                foreach (KeyValuePair<int, int> s in subscriptions)
                {
                    Subscription subscription = Load<Subscription>(s.Key);
                    if (subscription != null)
                    {
                        for (int i = 0; i < s.Value; i++)
                        {
                            IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                            decimal saleAmount = CalculateBillingSaleAmount(subscription, isSpecialOffer, coupon, shippingAmount);
                            decimal saleRefererCommissionRedeem = (refererCommissionRedeem == null ? 0M : CalculateRedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount));
                            decimal salePromoGiftRedeem = CalculateRedeemPromoGiftAmount(promoGiftList, saleAmount - saleRefererCommissionRedeem);
                            if (saleAmount > saleRefererCommissionRedeem + salePromoGiftRedeem)
                            {
                                BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> saleRes = CreateBillingSale(res.ReturnValue.ParentBilling, subscription,
                                    isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, shippingAmount, shippingOption);

                                shippingAmount = null;

                                if (saleRes != null)
                                {
                                    if (saleRes.State == BusinessErrorState.Success)
                                    {
                                        res.State = BusinessErrorState.Success;
                                        res.ReturnValue.BillingSales.Add(
                                            new Set<BillingSale, BillingSubscription, Subscription>()
                                            {
                                                Value1 = saleRes.ReturnValue.Value1,
                                                Value2 = saleRes.ReturnValue.Value2,
                                                Value3 = subscription
                                            });
                                        res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? saleRes.ReturnValue.Value3;

                                        if (refererCommissionRedeem != null)
                                        {
                                            refererCommissionRedeem -= saleRefererCommissionRedeem;
                                        }
                                    }
                                    else
                                    {
                                        res.ErrorMessage = saleRes.ErrorMessage;
                                    }
                                }
                            }
                            else
                            {
                                Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> saleRes = CreateExtraTrialShipSale_BillingSale(res.ReturnValue.ParentBilling, subscription,
                                    isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, shippingAmount, shippingOption);

                                shippingAmount = null;

                                if (saleRes != null)
                                {
                                    res.State = BusinessErrorState.Success;
                                    res.ReturnValue.BillingFreeSales.Add(saleRes);

                                    if (refererCommissionRedeem != null)
                                    {
                                        refererCommissionRedeem -= saleRefererCommissionRedeem;
                                    }
                                }
                            }
                        }
                    }
                }
                IList<KeyValuePair<UpsellType, int>> upsellTypeListForCharge = new List<KeyValuePair<UpsellType, int>>();
                foreach (KeyValuePair<int, int> upsell in upsellTypes)
                {
                    UpsellType upsellType = Load<UpsellType>(upsell.Key);
                    if (upsellType != null)
                    {
                        IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                        decimal saleAmount = CalculateUpsellSaleAmount(upsellType, upsell.Value, coupon, shippingAmount);
                        decimal saleRefererCommissionRedeem = (refererCommissionRedeem == null ? 0M : CalculateRedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount));
                        decimal salePromoGiftRedeem = CalculateRedeemPromoGiftAmount(promoGiftList, saleAmount - saleRefererCommissionRedeem);
                        if (saleAmount > saleRefererCommissionRedeem + salePromoGiftRedeem)
                        {
                            //Add for single charge later
                            upsellTypeListForCharge.Add(new KeyValuePair<UpsellType, int>(upsellType, upsell.Value));
                        }
                        else
                        {
                            //Process free item
                            //
                            Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> saleRes = CreateExtraTrialShipSale_UpsellSale(res.ReturnValue.ParentBilling, upsellType,
                                upsell.Value, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, shippingAmount, shippingOption);

                            //
                            shippingAmount = null;
                            //

                            if (saleRes != null)
                            {
                                res.State = BusinessErrorState.Success;
                                res.ReturnValue.UpsellFreeSales.Add(saleRes);

                                if (refererCommissionRedeem != null)
                                {
                                    refererCommissionRedeem -= saleRefererCommissionRedeem;
                                }
                            }
                        }
                    }
                }
                if (upsellTypeListForCharge.Count > 0)
                {
                    IList<PromoGift> promoGiftList = GetGiftCertificateByNumber(giftCerificates);
                    //
                    BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> upsellsRes =
                        CreateUpsellSales(res.ReturnValue.ParentBilling, upsellTypeListForCharge, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, shippingAmount, shippingOption);
                    if (upsellsRes != null)
                    {
                        if (upsellsRes.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                            res.ReturnValue.UpsellSales.AddRange(
                                upsellsRes.ReturnValue.Select(upsellRes =>
                                    new Set<UpsellSale, BillingSubscription, Upsell, UpsellType>()
                                    {
                                        Value1 = upsellRes.Value1,
                                        Value2 = upsellRes.Value2,
                                        Value3 = upsellRes.Value3,
                                        Value4 = upsellRes.Value5
                                    }));
                            AssertigyMID upsellsAssertigyMid = null;
                            if (upsellsRes.ReturnValue.Count > 0)
                            {
                                upsellsAssertigyMid = upsellsRes.ReturnValue.First().Value4;
                            }
                            res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? upsellsAssertigyMid;
                        }
                        else
                        {
                            res.ErrorMessage = upsellsRes.ErrorMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }
        // Creates only one charge for the only subscription and a list of charges for products
        public BusinessError<ComplexSaleView> CreateComplexSaleForHostedPayment(
                   long? registrationID,
                   string billingfirstName, string billinglastName, string billingaddress1, string billingaddress2,
                   string billingcity, string billingstate, string billingzip, string billingcountry,
                   string billingphone, string billingemail,
                   string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2,
                   string shippingCity, string shippingState, string shippingZip, string shippingCountry,
                   string shippingPhone, string shippingEmail,
                   string affiliate, string subAffiliate, decimal subcost, string ip, string url,
                   string creditCard, string cvv, int expMonth, int expYear,
                   int? subscriptionID, string[] ProductCodeID_Quantity, string[] Prices, string[] Descriptions, int? productID)
        {
            IList<UpsellType> UpsellTypes = new List<UpsellType>();
            Subscription subscription = Load<Subscription>(subscriptionID);

            if (subscriptionID == null && productID == null)
            {
                return null;
            }

            for (int i = 0; i < ProductCodeID_Quantity.Length; i++)
            {
                string[] tmp = ProductCodeID_Quantity[i].Split('-');
                int prCodeID = 0;
                short quant = 0;
                if (tmp.Length == 2)
                {
                    if (!string.IsNullOrEmpty(tmp[0]))
                        prCodeID = int.Parse(tmp[0]);
                    if (!string.IsNullOrEmpty(tmp[1]))
                        quant = short.Parse(tmp[1]);
                }
                decimal prc = 0;
                if (!string.IsNullOrEmpty(Prices[i]))
                    prc = decimal.Parse(Prices[i]);

                var productCode = dao.Load<ProductCode>(prCodeID);

                if (productCode != null)
                {
                    UpsellTypes.Add(new UpsellType()
                    {
                        ProductID = subscription != null ? subscription.ProductID : productID,
                        Quantity = quant,
                        Price = prc,
                        Description = Descriptions[i],
                        DisplayName = "",
                        DropDown = false,
                        ProductCode = productCode.ProductCode_,
                        UpsellTypeID = 0
                    });
                }
            }

            BusinessError<ComplexSaleView> res = new BusinessError<ComplexSaleView>();
            res.ReturnValue = new ComplexSaleView();
            res.ReturnValue.BillingSales = new List<Set<BillingSale, BillingSubscription, Subscription>>();
            res.ReturnValue.BillingFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>>();
            res.ReturnValue.UpsellSales = new List<Set<UpsellSale, BillingSubscription, Upsell, UpsellType>>();
            res.ReturnValue.UpsellFreeSales = new List<Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType>>();
            res.State = BusinessErrorState.Error;

            try
            {
                /*if registration exist*/

                Registration registration = null;
                RegistrationInfo registrationInfo = null;

                if (registrationID != null)
                {
                    registration = dao.Load<Registration>(registrationID);
                    registrationInfo = dao.Load<RegistrationInfo>(new MySqlCommand("Select * From RegistrationInfo Where RegistrationID=" + registrationID.ToString())).FirstOrDefault();
                }

                if (registration != null)
                {
                    shippingFirstName = registration.FirstName;
                    shippingLastName = registration.LastName;
                    shippingAddress1 = registration.Address1;
                    shippingAddress2 = registration.Address2;
                    shippingCity = registration.City;
                    shippingState = registration.State;
                    shippingZip = registration.Zip;
                    shippingPhone = registration.Phone;
                    shippingEmail = registration.Email;
                    shippingCountry = registrationInfo == null ? "US" : registrationInfo.Country;
                }
                /*if registration exist*/

                Set<Registration, Billing> parent = registrationService.CreateOrUpdateRegistrationAndBilling(null, registrationID,
                    billingfirstName, billinglastName, billingaddress1, billingaddress2, billingcity, billingstate,
                    billingzip, billingcountry, billingphone, billingemail,
                    shippingFirstName, shippingLastName, shippingAddress1, shippingAddress2, shippingCity,
                    shippingState, shippingZip, shippingCountry, shippingPhone, shippingEmail,
                    null, affiliate, subAffiliate, ip, url, null, creditCard, cvv, expMonth, expYear);

                if (parent == null)
                {
                    throw new Exception("Can't create/update Billing and Registration");
                }
                else
                {
                    res.ReturnValue.Registration = parent.Value1;
                    res.ReturnValue.ParentBilling = parent.Value2;
                }

                if (subscription != null)
                {
                    var saleRes = CreateBillingSale(res.ReturnValue.ParentBilling, subscription, false, null, null, string.Empty, null, null, null, null, null);

                    if (saleRes != null)
                    {
                        res.State = BusinessErrorState.Success;
                        res.ReturnValue.BillingSales.Add(
                                            new Set<BillingSale, BillingSubscription, Subscription>()
                                            {
                                                Value1 = saleRes.ReturnValue.Value1,
                                                Value2 = saleRes.ReturnValue.Value2,
                                                Value3 = subscription
                                            });
                        res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? saleRes.ReturnValue.Value3;
                    }
                }

                IList<KeyValuePair<UpsellType, int>> upsellTypeListForCharge = new List<KeyValuePair<UpsellType, int>>();
                foreach (UpsellType upsell in UpsellTypes)
                {
                    var upsellForCharge = GetOrCreateUpsell(upsell.ProductID, upsell.ProductCode, 1, upsell.Price);
                    if (upsellForCharge != null)
                        upsellTypeListForCharge.Add(new KeyValuePair<UpsellType, int>(upsellForCharge, (int)upsell.Quantity));
                }
                if (upsellTypeListForCharge.Count > 0)
                {

                    BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> upsellsRes =
                        CreateUpsellSales(res.ReturnValue.ParentBilling, upsellTypeListForCharge, null, null, string.Empty, null, null, null, null, null);
                    if (upsellsRes != null)
                    {
                        if (upsellsRes.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                            res.ReturnValue.UpsellSales.AddRange(
                                upsellsRes.ReturnValue.Select(upsellRes =>
                                    new Set<UpsellSale, BillingSubscription, Upsell, UpsellType>()
                                    {
                                        Value1 = upsellRes.Value1,
                                        Value2 = upsellRes.Value2,
                                        Value3 = upsellRes.Value3,
                                        Value4 = upsellRes.Value5
                                    }));
                            AssertigyMID upsellsAssertigyMid = null;
                            if (upsellsRes.ReturnValue.Count > 0)
                            {
                                upsellsAssertigyMid = upsellsRes.ReturnValue.First().Value4;
                            }
                            res.ReturnValue.AssertigyMID = res.ReturnValue.AssertigyMID ?? upsellsAssertigyMid;
                        }
                        else
                        {
                            res.ErrorMessage = upsellsRes.ErrorMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        private BillingSubscription CreateUpsellFakeBillingSubscription(Billing billing)
        {
            BillingSubscription res = null;

            try
            {
                dao.BeginTransaction();

                Subscription subscription = EnsureLoad<Subscription>(BillingSubscriptionEnum.UpsellFake);
                res = CreateBillingSubscription(billing, subscription);

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

        public BillingSubscription CreateUpsellFakeBillingSubscriptionByProduct(Billing billing, int productID)
        {
            BillingSubscription res = null;

            try
            {
                dao.BeginTransaction();

                Product p = EnsureLoad<Product>(productID);

                MySqlCommand q = new MySqlCommand("select * from Subscription " +
                    "where productID = @productID and Recurring = 0 and ProductCode is null");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                Subscription subscription = dao.Load<Subscription>(q).LastOrDefault();
                if (subscription == null)
                {
                    subscription = new Subscription();
                    subscription.DisplayName = string.Format("One-time sale subscription for {0}", p.ProductName);
                    subscription.Selectable = false;
                    subscription.ProductID = productID;
                    subscription.Recurring = false;
                    subscription.ShipFirstRebill = false;
                    dao.Save<Subscription>(subscription);
                }
                res = CreateBillingSubscription(billing, subscription);

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

        public BillingSubscription CreateBillingSubscription(Billing billing, Subscription subscription)
        {
            BillingSubscription res = null;

            try
            {
                dao.BeginTransaction();

                res = new BillingSubscription();

                res.BillingID = billing.BillingID;
                res.SubscriptionID = subscription.SubscriptionID;
                res.CreateDT = DateTime.Now;
                res.LastBillDate = DateTime.Now;
                if (subscription.Recurring != null && subscription.Recurring.Value)
                {
                    res.NextBillDate = (subscription.InitialInterim != null) ?
                        DateTime.Now.AddDays(subscription.InitialInterim.Value) :
                        DateTime.Now.AddDays(DEFAULT_INITIAL_INTERIM);
                }
                else
                {
                    res.NextBillDate = NOT_RECURRING_NEXT_BILL_DATE;
                }
                res.StatusTID = (!IsTestCreditCard(billing.CreditCard)) ? BillingSubscriptionStatusEnum.Active : BillingSubscriptionStatusEnum.Scrubbed;
                //res.SKU = subscription.SKU2;
                res.CustomerReferenceNumber = Utility.RandomString(new Random(), 6);

                dao.Save<BillingSubscription>(res);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creditCard">Enrypted or decrypted credit card number</param>
        /// <returns></returns>
        public bool IsTestCreditCard(string creditCard)
        {
            string decryptedCreditCard = string.Empty;
            try
            {
                decryptedCreditCard = (new CreditCard(creditCard)).DecryptedCreditCard;
            }
            catch { }
            return (decryptedCreditCard == TEST_CREDIT_CARD);
        }

        private BillingSubscriptionDiscount SaveBillingSubscriptionDiscount(BillingSubscription billingSubscription, bool isSpecialOffer, ICoupon coupon)
        {
            BillingSubscriptionDiscount res = null;
            try
            {
                dao.BeginTransaction();

                if (isSpecialOffer || coupon != null)
                {
                    BillingSubscriptionDiscount billingSubscriptionDiscount = new BillingSubscriptionDiscount();
                    billingSubscriptionDiscount.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                    billingSubscriptionDiscount.IsSavePrice = false;
                    if (isSpecialOffer)
                    {
                        billingSubscriptionDiscount.IsSavePrice = true;
                    }
                    else if (coupon != null && coupon.Discount != null)
                    {
                        billingSubscriptionDiscount.Discount = coupon.Discount;
                    }
                    else if (coupon != null && coupon.FixedPrice != null)
                    {
                        billingSubscriptionDiscount.NewShippingAmount = coupon.FixedPrice;
                    }
                    dao.Save<BillingSubscriptionDiscount>(billingSubscriptionDiscount);

                    if (coupon is BillingReferralCoupon && billingSubscription.BillingID != null && ((BillingReferralCoupon)coupon).BillingID != null)
                    {
                        CreateBillingReferred(((BillingReferralCoupon)coupon).BillingID.Value, billingSubscription.BillingID.Value);
                    }
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

        private Billing DuplicateBilling(Billing billing)
        {
            Billing res = null;

            try
            {
                dao.BeginTransaction();

                res = registrationService.CreateBilling(billing.CampaignID, billing.RegistrationID,
                    billing.FirstName, billing.LastName, billing.CreditCard, billing.CVV, billing.PaymentTypeID,
                    billing.ExpMonth, billing.ExpYear, billing.Address1, billing.Address2, billing.City,
                    billing.State, billing.Zip, billing.Country, billing.Email, billing.Phone, DateTime.Now,
                    billing.Affiliate, billing.SubAffiliate, billing.IP, billing.URL);

                BillingLinked billingLinked = new BillingLinked();
                billingLinked.ParentBillingID = (int)billing.BillingID.Value;
                billingLinked.BillingID = (int)res.BillingID.Value;
                dao.Save<BillingLinked>(billingLinked);

                Referer ownReferer = refererService.GetOwnRefererByBilling(billing.BillingID.Value);
                if (ownReferer != null)
                {
                    refererService.AddBillingToReferer(ownReferer, res);
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

        private decimal CalculateBillingSaleAmount(Subscription subscription, bool isSpecialOffer, ICoupon coupon, decimal? shippingAmount)
        {
            decimal amount = (isSpecialOffer) ?
                Utility.Add(subscription.SaveBilling, subscription.SaveShipping) :
                Utility.Add(subscription.InitialBillAmount, subscription.InitialShipping);

            if (shippingAmount > 0)
                amount += shippingAmount.Value;

            if (!isSpecialOffer && coupon != null)
            {
                if (coupon is ProductCoupon)
                {
                    ProductCoupon productCoupon = (ProductCoupon)coupon;
                    if (productCoupon.IsAppliedTo(subscription.ProductCode))
                    {
                        amount = productCoupon.ApplyDiscount(subscription.ProductCode, amount, DiscountType.Any);
                    }
                }
                else
                {
                    //for BillingSale apply Any discount
                    amount = coupon.ApplyDiscount(amount, DiscountType.Any);
                }
            }

            return amount;
        }

        private decimal CalculateUpsellSaleAmount(UpsellType upsellType, int quantity, ICoupon coupon, decimal? shippingAmount)
        {
            decimal amount = upsellType.Price.Value;

            if (shippingAmount > 0)
                amount += shippingAmount.Value;

            if (coupon != null)
            {
                if (coupon is ProductCoupon)
                {
                    ProductCoupon productCoupon = (ProductCoupon)coupon;
                    if (productCoupon.IsAppliedTo(upsellType.ProductCode))
                    {
                        amount = productCoupon.ApplyDiscount(upsellType.ProductCode, amount, DiscountType.Any);
                    }
                }
                else
                {
                    //for UpsellSale apply only "Discount" discount
                    amount = coupon.ApplyDiscount(amount, DiscountType.Discount);
                }
            }
            amount = amount * (decimal)quantity;

            return amount;
        }

        private BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> CreateBillingSale(Billing billing, Subscription subscription, bool isSpecialOffer, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList)
        {
            return CreateBillingSale(billing, subscription, isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, null, null);
        }

        private BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> CreateBillingSale(Billing billing, Subscription subscription, bool isSpecialOffer, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList, decimal? shippingAmount, int? shippingOptionID)
        {
            BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> res = new BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            BillingSale sale = null;
            BillingSubscription billingSubscription = null;
            AssertigyMID assertigyMid = null;
            BillingSubscription existedBillingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));
            bool isTestAffiliateCase = (billing.Affiliate == TEST_AFFILIATE_CODE);

            bool doNotAllowDupes = false;
            if ((!isTestCase) && (!isTestAffiliateCase) && doNotAllowDupes && IsCreditCardDupe(billing.CreditCard))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "This credit card has been used previously for this special offer. We regret that because of the incredible deal, we can only Offer this discount once to each household.";
            }
            else
            {
                try
                {
                    dao.BeginTransaction();

                    existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);
                    //2010-11-30, use same Billing for multiple subscriptions
                    //if (existedBillingSubscription != null)
                    //{
                    //    billing = DuplicateBilling(billing);
                    //}

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res = null;
                }

                if (res != null)
                {
                    try
                    {
                        //TODO: Implement dao.BeginTopTransaction() 
                        //means that this transaction can not be included into another
                        //throws exception if it is not top transaction
                        //(in dao increase transaction level and throw exception)
                        //Should be for all methods that returns BusinessError<>
                        dao.BeginTransaction();

                        billingSubscription = CreateBillingSubscription(billing, subscription);
                        SaveBillingSubscriptionDiscount(billingSubscription, isSpecialOffer, coupon);

                        if (!string.IsNullOrEmpty(partnerClickClickID))
                        {
                            PartnerClick click = new PartnerClick();
                            click.BillingID = billing.BillingID;
                            click.ClickID = partnerClickClickID;
                            dao.Save<PartnerClick>(click);
                        }

                        if (!isTestCase)
                        {
                            sale = new BillingSale();
                            sale.SaleTypeID = SaleTypeEnum.Billing;
                            sale.TrackingNumber = null;
                            sale.CreateDT = DateTime.Now;
                            sale.NotShip = false;
                            sale.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                            sale.RebillCycle = 0; //TODO ?
                            sale.ChargeHistoryID = null;
                            sale.PaygeaID = null;
                            sale.ProductCode = subscription.ProductCode;
                            sale.Quantity = subscription.Quantity;
                            dao.Save<BillingSale>(sale);

                            if (shippingOptionID != null)
                            {
                                MySqlCommand q = new MySqlCommand("INSERT INTO SaleShippingOption VALUES(@SaleID, @ShippingOptionID)");
                                q.Parameters.AddWithValue("@SaleID", sale.SaleID);
                                q.Parameters.AddWithValue("@ShippingOptionID", shippingOptionID);
                                dao.ExecuteNonQuery(q);
                            }

                            decimal amount = CalculateBillingSaleAmount(subscription, isSpecialOffer, coupon, shippingAmount);

                            if (refererCommissionRedeem != null)
                            {
                                amount -= RedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, amount, sale);
                            }
                            amount -= RedeemPromoGiftAmount(promoGiftList, amount, sale);

                            if (!isSpecialOffer && coupon != null)
                            {
                                if (coupon is ProductCoupon)
                                {
                                    ProductCoupon productCoupon = (ProductCoupon)coupon;
                                    if (productCoupon.IsAppliedTo(subscription.ProductCode))
                                    {
                                        SaleCouponCode saleCouponCode = new SaleCouponCode();
                                        saleCouponCode.SaleID = (int)sale.SaleID.Value;
                                        saleCouponCode.CouponCode = coupon.CouponCode;
                                        saleCouponCode.CreateDT = DateTime.Now;
                                        dao.Save<SaleCouponCode>(saleCouponCode);
                                    }
                                }
                                else
                                {
                                    SaleCouponCode saleCouponCode = new SaleCouponCode();
                                    saleCouponCode.SaleID = (int)sale.SaleID.Value;
                                    saleCouponCode.CouponCode = coupon.CouponCode;
                                    saleCouponCode.CreateDT = DateTime.Now;
                                    dao.Save<SaleCouponCode>(saleCouponCode);
                                }
                            }

                            if (referer != null)
                            {
                                SaleReferer saleReferer = new SaleReferer();
                                saleReferer.SaleID = sale.SaleID;
                                saleReferer.RefererID = referer.RefererID;
                                saleReferer.CreateDT = DateTime.Now;
                                dao.Save<SaleReferer>(saleReferer);
                            }

                            //If not new Sale(BillingSubscription already exists for parent Billing) try to use same Merchant as for previus Sales
                            Set<NMICompany, AssertigyMID> mid = null;
                            if (existedBillingSubscription != null)
                            {
                                mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value);
                            }
                            else
                            {
                                mid = merchantService.ChooseRandomNMIMerchantAccount(subscription.ProductID.Value, billing, amount);
                            }
                            if (mid != null)
                            {
                                NMICompany nmiCompany = mid.Value1;
                                assertigyMid = mid.Value2;

                                Currency cur = GetCurrencyByProduct(subscription.ProductID.Value);

                                Product product = EnsureLoad<Product>(subscription.ProductID);

                                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                                BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                                    nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                                    sale.SaleID.Value, billing, product);

                                if (paymentResult.State == BusinessErrorState.Success)
                                {
                                    res.State = BusinessErrorState.Success;
                                }
                                else
                                {
                                    dao.RollbackTransaction();

                                    sale = null;
                                    billingSubscription = null;

                                    res.State = BusinessErrorState.Error;
                                    res.ErrorMessage = paymentResult.ErrorMessage;

                                    dao.BeginTransaction();
                                }

                                Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                                    subscription.ProductCode, SaleTypeEnum.Billing, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                                if (sale != null)
                                {
                                    sale.PaygeaID = chargeLog.Value1.PaygeaID;
                                    sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                    dao.Save<BillingSale>(sale);
                                }
                            }
                            else
                            {
                                EmergencyQueue emergencyQueue = new EmergencyQueue();
                                emergencyQueue.BillingID = (int?)billing.BillingID;
                                emergencyQueue.Amount = amount;
                                emergencyQueue.CreateDT = DateTime.Now;
                                emergencyQueue.Completed = false;
                                dao.Save<EmergencyQueue>(emergencyQueue);
                            }
                        }
                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                        dao.RollbackTransaction();
                        res = null;
                    }

                    if (res != null && res.State == BusinessErrorState.Success)
                    {
                        ValidateCustomer(billing);
                        ValidateFraud(billing, sale);
                        SendSHW(billing, subscription, DateTime.Now);
                        //if (subscription.ProductID == 10)
                        {
                            emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);
                            //emailService.SendConfirmationEmail(billing, subscription,
                            //    (assertigyMid != null) ? assertigyMid.DisplayName : null,
                            //    referer, DateTime.Now);
                        }
                    }
                }
            }

            if (res != null)
            {
                res.ReturnValue = new Set<BillingSale, BillingSubscription, AssertigyMID>();
                res.ReturnValue.Value1 = sale;
                res.ReturnValue.Value2 = billingSubscription;
                res.ReturnValue.Value3 = assertigyMid;
            }

            return res;
        }

        private decimal RedeemRefererCommission(Referer referer, decimal redeemAmountInEcigDollars, decimal amount, Sale sale)
        {
            if (referer == null || referer.RefererID == null)
            {
                return 0M;
            }

            decimal res = 0M;

            try
            {
                decimal availableAmount = refererService.GetAvailableAmountToRedeemInEcigsDollars(referer.RefererID.Value);
                if (redeemAmountInEcigDollars > availableAmount)
                {
                    redeemAmountInEcigDollars = availableAmount;
                }
                redeemAmountInEcigDollars = (amount > redeemAmountInEcigDollars ? redeemAmountInEcigDollars : amount);
                res = redeemAmountInEcigDollars;

                IList<RefererCommission> comms = refererService.GetCommissions(referer.RefererID.Value);
                if (comms != null)
                {
                    comms = comms.Where(c => c.RefererCommissionTID == TrimFuel.Model.Enums.RefererCommissionTypeEnum.UseInStore && c.RemainingAmount != null).ToList();
                    if (comms.Count > 0)
                    {
                        foreach (RefererCommission c in comms)
                        {
                            if (redeemAmountInEcigDollars == 0M)
                            {
                                break;
                            }
                            decimal redeemAmountPerComm = (refererService.ConvertToUSD(redeemAmountInEcigDollars) > c.RemainingAmount.Value ? c.RemainingAmount.Value : refererService.ConvertToUSD(redeemAmountInEcigDollars));
                            decimal redeemAmountPerCommInEcigDollars = refererService.ConvertToEcigsDollars(redeemAmountPerComm);

                            MySqlCommand q = new MySqlCommand(
                                " update RefererCommission" +
                                " set RemainingAmount = RemainingAmount - @redeemAmountPerComm" +
                                " where RefererCommissionID = @refererCommissionID");
                            q.Parameters.Add("@refererCommissionID", MySqlDbType.Int32).Value = c.RefererCommissionID;
                            q.Parameters.Add("@redeemAmountPerComm", MySqlDbType.Decimal).Value = redeemAmountPerComm;
                            dao.ExecuteNonQuery(q);

                            RefererCommissionSale gsl = new RefererCommissionSale();
                            gsl.RefererCommissionID = c.RefererCommissionID;
                            gsl.SaleID = sale.SaleID;
                            gsl.RedeemAmount = redeemAmountPerCommInEcigDollars;
                            dao.Save<RefererCommissionSale>(gsl);

                            redeemAmountInEcigDollars -= redeemAmountPerCommInEcigDollars;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = 0M;
            }
            return res;
        }

        private decimal RedeemPromoGiftAmount(IList<PromoGift> promoGiftList, decimal amount, Sale sale)
        {
            decimal res = 0M;
            if (promoGiftList == null || promoGiftList.Where(i => (i.RemainingValue != null && i.RemainingValue.Value > 0M)).Count() == 0)
            {
                return res;
            }

            IDictionary<PromoGift, decimal> redeemList = null;

            try
            {
                dao.BeginTransaction();

                redeemList = new Dictionary<PromoGift, decimal>();
                foreach (PromoGift g in promoGiftList.Where(i => (i.RemainingValue != null && i.RemainingValue.Value > 0M)))
                {
                    if (amount == 0M)
                    {
                        break;
                    }
                    decimal redeemAmount = (amount > g.RemainingValue.Value ? g.RemainingValue.Value : amount);

                    MySqlCommand q = new MySqlCommand(
                        " update PromoGift" +
                        " set RemainingValue = RemainingValue - @redeemAmount" +
                        " where PromoGiftID = @promoGiftID");
                    q.Parameters.Add("@promoGiftID", MySqlDbType.Int64).Value = g.PromoGiftID;
                    q.Parameters.Add("@redeemAmount", MySqlDbType.Decimal).Value = redeemAmount;
                    dao.ExecuteNonQuery(q);

                    PromoGiftSale gsl = new PromoGiftSale();
                    gsl.PromoGiftID = g.PromoGiftID;
                    gsl.SaleID = sale.SaleID;
                    gsl.RedeemAmount = redeemAmount;
                    dao.Save<PromoGiftSale>(gsl);

                    g.RemainingValue -= redeemAmount;
                    redeemList.Add(g, redeemAmount);

                    amount -= redeemAmount;

                    res += redeemAmount;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = 0M;
                if (redeemList != null)
                {
                    foreach (KeyValuePair<PromoGift, decimal> item in redeemList)
                    {
                        item.Key.RemainingValue += item.Value;
                    }
                }
            }

            return res;
        }

        private decimal CalculateRedeemRefererCommission(Referer referer, decimal redeemAmountInEcigDollars, decimal amount)
        {
            if (referer == null || referer.RefererID == null)
            {
                return 0M;
            }

            decimal availableAmount = refererService.GetAvailableAmountToRedeemInEcigsDollars(referer.RefererID.Value);
            if (redeemAmountInEcigDollars > availableAmount)
            {
                redeemAmountInEcigDollars = availableAmount;
            }
            redeemAmountInEcigDollars = (amount > redeemAmountInEcigDollars ? redeemAmountInEcigDollars : amount);

            return redeemAmountInEcigDollars;
        }

        private decimal CalculateRedeemPromoGiftAmount(IList<PromoGift> promoGiftList, decimal amount)
        {
            decimal res = 0M;
            if (promoGiftList == null || promoGiftList.Where(i => (i.RemainingValue != null && i.RemainingValue.Value > 0M)).Count() == 0)
            {
                return res;
            }

            foreach (PromoGift g in promoGiftList.Where(i => (i.RemainingValue != null && i.RemainingValue.Value > 0M)))
            {
                if (amount == 0M)
                {
                    break;
                }
                decimal redeemAmount = (amount > g.RemainingValue.Value ? g.RemainingValue.Value : amount);
                amount -= redeemAmount;
                res += redeemAmount;
            }

            return res;
        }

        public BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> CreateBillingSale_ForExistedSubscription_WithRandomMID_MoEmergencyQueue_NoEmails(Billing billing, BillingSubscription existedBillingSubscription, decimal amount)
        {
            BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> res = new BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            BillingSale sale = null;
            BillingSubscription billingSubscription = existedBillingSubscription;
            AssertigyMID assertigyMid = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));
            bool isTestAffiliateCase = (billing.Affiliate == TEST_AFFILIATE_CODE);

            bool doNotAllowDupes = false;
            if ((!isTestCase) && (!isTestAffiliateCase) && doNotAllowDupes && IsCreditCardDupe(billing.CreditCard))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "This credit card has been used previously for this special offer. We regret that because of the incredible deal, we can only Offer this discount once to each household.";
            }
            else
            {
                if (res != null)
                {
                    try
                    {
                        //TODO: Implement dao.BeginTopTransaction() 
                        //means that this transaction can not be included into another
                        //throws exception if it is not top transaction
                        //(in dao increase transaction level and throw exception)
                        //Should be for all methods that returns BusinessError<>
                        dao.BeginTransaction();

                        Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);

                        if (!isTestCase)
                        {
                            sale = new BillingSale();
                            sale.SaleTypeID = SaleTypeEnum.Billing;
                            sale.TrackingNumber = null;
                            sale.CreateDT = DateTime.Now;
                            sale.NotShip = false;
                            sale.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                            sale.RebillCycle = 0; //TODO ?
                            sale.ChargeHistoryID = null;
                            sale.PaygeaID = null;
                            sale.ProductCode = subscription.ProductCode;
                            sale.Quantity = subscription.Quantity;
                            dao.Save<BillingSale>(sale);

                            Set<NMICompany, AssertigyMID> mid = null;
                            mid = merchantService.ChooseRandomNMIMerchantAccount(subscription.ProductID.Value, billing, amount);
                            if (mid != null)
                            {
                                NMICompany nmiCompany = mid.Value1;
                                assertigyMid = mid.Value2;

                                Product product = EnsureLoad<Product>(subscription.ProductID);

                                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                                Currency cur = GetCurrencyByProduct(subscription.ProductID.Value);

                                BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                                    nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                                    sale.SaleID.Value, billing, product);

                                if (paymentResult.State == BusinessErrorState.Success)
                                {
                                    res.State = BusinessErrorState.Success;
                                }
                                else
                                {
                                    dao.RollbackTransaction();

                                    sale = null;

                                    res.State = BusinessErrorState.Error;
                                    res.ErrorMessage = paymentResult.ErrorMessage;

                                    dao.BeginTransaction();
                                }

                                Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                                    subscription.ProductCode, SaleTypeEnum.Billing, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                                if (sale != null)
                                {
                                    sale.PaygeaID = chargeLog.Value1.PaygeaID;
                                    sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                    dao.Save<BillingSale>(sale);
                                }
                            }
                            else
                            {
                                throw new Exception("AssertigyMID can not be determined.");
                            }
                        }
                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                        dao.RollbackTransaction();
                        res = null;
                    }
                }
            }

            if (res != null)
            {
                res.ReturnValue = new Set<BillingSale, BillingSubscription, AssertigyMID>();
                res.ReturnValue.Value1 = sale;
                res.ReturnValue.Value2 = billingSubscription;
                res.ReturnValue.Value3 = assertigyMid;
            }

            return res;
        }

        public BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> CreateBillingSale_ForExistedSubscription_WithSpecifiedMID_NoEmergencyQueue_NoEmails(Billing billing, BillingSubscription existedBillingSubscription, decimal amount, AssertigyMID specifiedAssertigyMID)
        {
            BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> res = new BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            BillingSale sale = null;
            BillingSubscription billingSubscription = existedBillingSubscription;
            AssertigyMID assertigyMid = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));
            bool isTestAffiliateCase = (billing.Affiliate == TEST_AFFILIATE_CODE);

            bool doNotAllowDupes = false;
            if ((!isTestCase) && (!isTestAffiliateCase) && doNotAllowDupes && IsCreditCardDupe(billing.CreditCard))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "This credit card has been used previously for this special offer. We regret that because of the incredible deal, we can only Offer this discount once to each household.";
            }
            else
            {
                if (res != null)
                {
                    try
                    {
                        //TODO: Implement dao.BeginTopTransaction() 
                        //means that this transaction can not be included into another
                        //throws exception if it is not top transaction
                        //(in dao increase transaction level and throw exception)
                        //Should be for all methods that returns BusinessError<>
                        dao.BeginTransaction();

                        Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);

                        if (!isTestCase)
                        {
                            sale = new BillingSale();
                            sale.SaleTypeID = SaleTypeEnum.Billing;
                            sale.TrackingNumber = null;
                            sale.CreateDT = DateTime.Now;
                            sale.NotShip = false;
                            sale.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                            sale.RebillCycle = 0; //TODO ?
                            sale.ChargeHistoryID = null;
                            sale.PaygeaID = null;
                            sale.ProductCode = subscription.ProductCode;
                            sale.Quantity = subscription.Quantity;
                            dao.Save<BillingSale>(sale);

                            Set<NMICompany, AssertigyMID> mid = null;
                            NMICompany specifiedAssertigyMIDNMICompany = merchantService.GetNMICompanyByAssertigyMID(specifiedAssertigyMID.AssertigyMIDID);
                            if (specifiedAssertigyMID != null && specifiedAssertigyMIDNMICompany != null)
                            {
                                mid = new Set<NMICompany, AssertigyMID>() { Value1 = specifiedAssertigyMIDNMICompany, Value2 = specifiedAssertigyMID };
                            }

                            if (mid != null)
                            {
                                NMICompany nmiCompany = mid.Value1;
                                assertigyMid = mid.Value2;

                                Product product = EnsureLoad<Product>(subscription.ProductID);

                                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                                Currency cur = GetCurrencyByProduct(subscription.ProductID.Value);

                                BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                                    nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                                    sale.SaleID.Value, billing, product);

                                if (paymentResult.State == BusinessErrorState.Success)
                                {
                                    res.State = BusinessErrorState.Success;
                                }
                                else
                                {
                                    dao.RollbackTransaction();

                                    sale = null;

                                    res.State = BusinessErrorState.Error;
                                    res.ErrorMessage = paymentResult.ErrorMessage;

                                    dao.BeginTransaction();
                                }

                                Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                                    subscription.ProductCode, SaleTypeEnum.Billing, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                                if (sale != null)
                                {
                                    sale.PaygeaID = chargeLog.Value1.PaygeaID;
                                    sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                    dao.Save<BillingSale>(sale);
                                }
                            }
                            else
                            {
                                throw new Exception("AssertigyMID can not be determined.");
                            }
                        }
                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                        dao.RollbackTransaction();
                        res = null;
                    }
                }
            }

            if (res != null)
            {
                res.ReturnValue = new Set<BillingSale, BillingSubscription, AssertigyMID>();
                res.ReturnValue.Value1 = sale;
                res.ReturnValue.Value2 = billingSubscription;
                res.ReturnValue.Value3 = assertigyMid;
            }

            return res;
        }

        public BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> CreateUpsellSale(Billing billing, UpsellType upsellType, int quantity, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList, bool checkFraudAndCustomer)
        {
            BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> res = new BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            Upsell upsell = null;
            UpsellSale sale = null;
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);

                //If new Sale(without BillingSubscription) create Upsell Fake BillingSubscription
                billingSubscription = existedBillingSubscription;
                if (billingSubscription == null)
                {
                    billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, upsellType.ProductID.Value);
                    SaveBillingSubscriptionDiscount(billingSubscription, false, coupon);

                    if (!string.IsNullOrEmpty(partnerClickClickID))
                    {
                        PartnerClick click = new PartnerClick();
                        click.BillingID = billing.BillingID;
                        click.ClickID = partnerClickClickID;
                        dao.Save<PartnerClick>(click);
                    }
                }

                if (!isTestCase)
                {
                    upsell = new Upsell();
                    upsell.UpsellTypeID = upsellType.UpsellTypeID;
                    upsell.BillingID = billing.BillingID;
                    upsell.ProductCode = upsellType.ProductCode;
                    upsell.Quantity = quantity;
                    upsell.CreateDT = DateTime.Now;
                    upsell.Complete = false;
                    dao.Save<Upsell>(upsell);

                    sale = new UpsellSale();
                    sale.SaleTypeID = SaleTypeEnum.Upsell;
                    sale.TrackingNumber = null;
                    sale.CreateDT = DateTime.Now;
                    sale.NotShip = false;
                    sale.UpsellID = upsell.UpsellID;
                    sale.ChargeHistoryID = null;
                    sale.PaygeaID = null;
                    dao.Save<UpsellSale>(sale);

                    decimal amount = CalculateUpsellSaleAmount(upsellType, quantity, coupon, null);
                    if (refererCommissionRedeem != null)
                    {
                        amount -= RedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, amount, sale);
                    }
                    amount -= RedeemPromoGiftAmount(promoGiftList, amount, sale);

                    if (coupon != null)
                    {
                        if (coupon is ProductCoupon)
                        {
                            ProductCoupon productCoupon = (ProductCoupon)coupon;
                            if (productCoupon.IsAppliedTo(upsellType.ProductCode))
                            {
                                SaleCouponCode saleCouponCode = new SaleCouponCode();
                                saleCouponCode.SaleID = (int)sale.SaleID.Value;
                                saleCouponCode.CouponCode = coupon.CouponCode;
                                saleCouponCode.CreateDT = DateTime.Now;
                                dao.Save<SaleCouponCode>(saleCouponCode);
                            }
                        }
                        else
                        {
                            SaleCouponCode saleCouponCode = new SaleCouponCode();
                            saleCouponCode.SaleID = (int)sale.SaleID.Value;
                            saleCouponCode.CouponCode = coupon.CouponCode;
                            saleCouponCode.CreateDT = DateTime.Now;
                            dao.Save<SaleCouponCode>(saleCouponCode);
                        }
                    }

                    if (referer != null)
                    {
                        SaleReferer saleReferer = new SaleReferer();
                        saleReferer.SaleID = sale.SaleID;
                        saleReferer.RefererID = referer.RefererID;
                        saleReferer.CreateDT = DateTime.Now;
                        dao.Save<SaleReferer>(saleReferer);
                    }

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;
                    if (existedBillingSubscription != null)
                    {
                        mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value);
                    }

                    if (mid == null)
                    {
                        mid = merchantService.ChooseRandomNMIMerchantAccount(upsellType.ProductID.Value, billing, amount);
                    }

                    if (mid != null)
                    {
                        NMICompany nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = null;
                        Subscription subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
                        if (subscription != null && subscription.ProductID != null)
                        {
                            cur = GetCurrencyByProduct(subscription.ProductID.Value);
                        }

                        Product product = EnsureLoad<Product>(subscription.ProductID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            sale.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            upsell = null;
                            sale = null;
                            billingSubscription = existedBillingSubscription;

                            res.State = BusinessErrorState.Error;
                            res.ErrorMessage = paymentResult.ErrorMessage;

                            dao.BeginTransaction();
                        }

                        Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellType.ProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                        if (sale != null)
                        {
                            sale.PaygeaID = chargeLog.Value1.PaygeaID;
                            sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                            dao.Save<UpsellSale>(sale);
                        }
                        else if (existedBillingSubscription != null)
                        {
                            //If Upsell failed for existed BillingSubscription
                            if (paymentResult.ReturnValue.Response.ToLower().Contains("risk decline"))
                            {
                                DeclineUpsell declineUpsell = new DeclineUpsell();
                                declineUpsell.BillingID = billing.BillingID;
                                declineUpsell.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<DeclineUpsell>(declineUpsell);
                            }
                        }
                    }
                    else
                    {
                        EmergencyQueue emergencyQueue = new EmergencyQueue();
                        emergencyQueue.BillingID = (int?)billing.BillingID;
                        emergencyQueue.Amount = amount;
                        emergencyQueue.CreateDT = DateTime.Now;
                        emergencyQueue.Completed = false;
                        dao.Save<EmergencyQueue>(emergencyQueue);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }

            if (res != null && res.State == BusinessErrorState.Success && checkFraudAndCustomer)
            {
                ValidateCustomer(billing);
                ValidateFraud(billing, sale);
            }

            if (res != null)
            {
                res.ReturnValue = new Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>();
                res.ReturnValue.Value1 = sale;
                res.ReturnValue.Value2 = billingSubscription;
                res.ReturnValue.Value3 = upsell;
                res.ReturnValue.Value4 = assertigyMid;

                emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);
            }

            return res;
        }

        public BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> CreateUpsellSales(Billing billing, IList<KeyValuePair<UpsellType, int>> upsellTypeList, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList)
        {
            return CreateUpsellSales(billing, upsellTypeList, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, null, null);
        }

        public BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> CreateUpsellSales(Billing billing, IList<KeyValuePair<UpsellType, int>> upsellTypeList, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList, decimal? shippingAmount, int? shippingOption)
        {
            if (upsellTypeList == null || upsellTypeList.Count == 0)
            {
                return null;
            }

            BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> res = new BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            IList<KeyValuePair<Set<UpsellSale, Upsell, UpsellType>, decimal>> saleList = new List<KeyValuePair<Set<UpsellSale, Upsell, UpsellType>, decimal>>();
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                int upsellsProductID = upsellTypeList.First().Key.ProductID.Value;

                BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);

                //If new Sale(without BillingSubscription) create Upsell Fake BillingSubscription
                billingSubscription = existedBillingSubscription;
                if (billingSubscription == null)
                {
                    billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, upsellsProductID);
                    SaveBillingSubscriptionDiscount(billingSubscription, false, coupon);

                    if (!string.IsNullOrEmpty(partnerClickClickID))
                    {
                        PartnerClick click = new PartnerClick();
                        click.BillingID = billing.BillingID;
                        click.ClickID = partnerClickClickID;
                        dao.Save<PartnerClick>(click);
                    }
                }

                if (!isTestCase)
                {
                    decimal amount = 0M;

                    foreach (KeyValuePair<UpsellType, int> item in upsellTypeList)
                    {
                        UpsellType upsellType = item.Key;
                        int quantity = item.Value;

                        Upsell upsell = new Upsell();
                        upsell.UpsellTypeID = upsellType.UpsellTypeID;
                        upsell.BillingID = billing.BillingID;
                        upsell.ProductCode = upsellType.ProductCode;
                        upsell.Quantity = quantity;
                        upsell.CreateDT = DateTime.Now;
                        upsell.Complete = false;
                        dao.Save<Upsell>(upsell);

                        UpsellSale sale = new UpsellSale();
                        sale.SaleTypeID = SaleTypeEnum.Upsell;
                        sale.TrackingNumber = null;
                        sale.CreateDT = DateTime.Now;
                        sale.NotShip = false;
                        sale.UpsellID = upsell.UpsellID;
                        sale.ChargeHistoryID = null;
                        sale.PaygeaID = null;
                        dao.Save<UpsellSale>(sale);

                        //emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);

                        if (shippingOption != null)
                        {
                            MySqlCommand q = new MySqlCommand("INSERT INTO SaleShippingOption VALUES(@SaleID, @ShippingOptionID)");
                            q.Parameters.AddWithValue("@SaleID", sale.SaleID);
                            q.Parameters.AddWithValue("@ShippingOptionID", shippingOption);
                            dao.ExecuteNonQuery(q);
                        }

                        decimal saleAmount = CalculateUpsellSaleAmount(upsellType, quantity, coupon, shippingAmount);
                        decimal saleRefererCommissionRedeem = 0M;
                        if (refererCommissionRedeem != null)
                        {
                            saleRefererCommissionRedeem = RedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, saleAmount, sale);
                            saleAmount -= saleRefererCommissionRedeem;
                        }
                        saleAmount -= RedeemPromoGiftAmount(promoGiftList, saleAmount, sale);

                        if (coupon != null)
                        {
                            if (coupon is ProductCoupon)
                            {
                                ProductCoupon productCoupon = (ProductCoupon)coupon;
                                if (productCoupon.IsAppliedTo(upsellType.ProductCode))
                                {
                                    SaleCouponCode saleCouponCode = new SaleCouponCode();
                                    saleCouponCode.SaleID = (int)sale.SaleID.Value;
                                    saleCouponCode.CouponCode = coupon.CouponCode;
                                    saleCouponCode.CreateDT = DateTime.Now;
                                    dao.Save<SaleCouponCode>(saleCouponCode);
                                }
                            }
                            else
                            {
                                SaleCouponCode saleCouponCode = new SaleCouponCode();
                                saleCouponCode.SaleID = (int)sale.SaleID.Value;
                                saleCouponCode.CouponCode = coupon.CouponCode;
                                saleCouponCode.CreateDT = DateTime.Now;
                                dao.Save<SaleCouponCode>(saleCouponCode);
                            }
                        }

                        if (referer != null)
                        {
                            SaleReferer saleReferer = new SaleReferer();
                            saleReferer.SaleID = sale.SaleID;
                            saleReferer.RefererID = referer.RefererID;
                            saleReferer.CreateDT = DateTime.Now;
                            dao.Save<SaleReferer>(saleReferer);
                        }

                        //reduce referer commission
                        if (refererCommissionRedeem != null)
                        {
                            refererCommissionRedeem -= saleRefererCommissionRedeem;
                        }
                        //reload promo gifts to affect commission reduce
                        if (promoGiftList != null)
                        {
                            for (int i = 0; i < promoGiftList.Count; i++)
                            {
                                promoGiftList[i] = dao.Load<PromoGift>(promoGiftList[i].PromoGiftID);
                            }
                        }
                        //Add to result list
                        saleList.Add(new KeyValuePair<Set<UpsellSale, Upsell, UpsellType>, decimal>(new Set<UpsellSale, Upsell, UpsellType>()
                        {
                            Value1 = sale,
                            Value2 = upsell,
                            Value3 = upsellType
                        }, saleAmount));

                        amount += saleAmount;

                        shippingAmount = null;
                    }

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;

                    if (existedBillingSubscription != null)
                    {
                        mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value);
                    }
                    if (mid == null)
                    {
                        mid = merchantService.ChooseRandomNMIMerchantAccount(upsellsProductID, billing, amount);
                    }

                    if (mid != null)
                    {
                        NMICompany nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = null;
                        Subscription subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
                        if (subscription != null && subscription.ProductID != null)
                        {
                            cur = GetCurrencyByProduct(subscription.ProductID.Value);
                        }

                        Product product = EnsureLoad<Product>(subscription.ProductID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            saleList.First().Key.Value1.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            saleList = null;
                            billingSubscription = existedBillingSubscription;

                            res.State = BusinessErrorState.Error;
                            res.ErrorMessage = paymentResult.ErrorMessage;

                            dao.BeginTransaction();
                        }

                        //if several upsells in one charge do not write ChargeDetails.ProductCode
                        string upsellProductCode = null;
                        if (upsellTypeList.Count == 1)
                        {
                            upsellProductCode = upsellTypeList.First().Key.ProductCode;
                        }
                        Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                        if (saleList != null)
                        {
                            foreach (var item in saleList)
                            {
                                item.Key.Value1.PaygeaID = chargeLog.Value1.PaygeaID;
                                item.Key.Value1.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<UpsellSale>(item.Key.Value1);
                            }
                            if (saleList.Count > 1)
                            {
                                //Write amounts per sale
                                foreach (var item in saleList)
                                {
                                    ChargeHistoryExSale chPerSale = new ChargeHistoryExSale();
                                    chPerSale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                    chPerSale.SaleID = item.Key.Value1.SaleID;
                                    chPerSale.Amount = item.Value;
                                    if (cur != null)
                                    {
                                        chPerSale.CurrencyID = cur.CurrencyID;
                                        chPerSale.CurrencyAmount = chPerSale.Amount;
                                        chPerSale.Amount = cur.ConvertToUSD(chPerSale.CurrencyAmount.Value);
                                    }
                                    dao.Save<ChargeHistoryExSale>(chPerSale);
                                }
                            }
                        }
                        else if (existedBillingSubscription != null)
                        {
                            //If Upsell failed for existed BillingSubscription
                            if (paymentResult.ReturnValue.Response.ToLower().Contains("risk decline"))
                            {
                                DeclineUpsell declineUpsell = new DeclineUpsell();
                                declineUpsell.BillingID = billing.BillingID;
                                declineUpsell.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<DeclineUpsell>(declineUpsell);
                            }
                        }
                    }
                    else
                    {
                        EmergencyQueue emergencyQueue = new EmergencyQueue();
                        emergencyQueue.BillingID = (int?)billing.BillingID;
                        emergencyQueue.Amount = amount;
                        emergencyQueue.CreateDT = DateTime.Now;
                        emergencyQueue.Completed = false;
                        dao.Save<EmergencyQueue>(emergencyQueue);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }

            if (res != null && res.State == BusinessErrorState.Success && saleList != null && saleList.Count > 0)
            {
                ValidateCustomer(billing);
                ValidateFraud(billing, saleList.First().Key.Value1);
            }

            if (res != null && res.State == BusinessErrorState.Success && saleList != null)
            {
                res.ReturnValue = new List<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>();
                foreach (var item in saleList)
                {
                    emailService.PushConfirmationEmailToQueue(billing.BillingID, item.Key.Value1.SaleID);

                    res.ReturnValue.Add(new Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>()
                    {
                        Value1 = item.Key.Value1,
                        Value2 = billingSubscription,
                        Value3 = item.Key.Value2,
                        Value4 = assertigyMid,
                        Value5 = item.Key.Value3
                    });
                }
            }

            return res;
        }

        public BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> CreateUpsellSales_No_Emergency_Queue(Billing billing, IList<KeyValuePair<UpsellType, int>> upsellTypeList, decimal? amount, int? midID)
        {
            if (upsellTypeList == null || upsellTypeList.Count == 0)
            {
                return null;
            }

            BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>> res = new BusinessError<IList<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            IList<KeyValuePair<Set<UpsellSale, Upsell, UpsellType>, decimal>> saleList = new List<KeyValuePair<Set<UpsellSale, Upsell, UpsellType>, decimal>>();
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                int upsellsProductID = upsellTypeList.First().Key.ProductID.Value;

                BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);

                //If new Sale(without BillingSubscription) create Upsell Fake BillingSubscription
                billingSubscription = existedBillingSubscription;
                if (billingSubscription == null)
                {
                    billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, upsellsProductID);
                    SaveBillingSubscriptionDiscount(billingSubscription, false, null);
                }

                if (!isTestCase)
                {
                    foreach (KeyValuePair<UpsellType, int> item in upsellTypeList)
                    {
                        UpsellType upsellType = item.Key;
                        int quantity = item.Value;

                        Upsell upsell = new Upsell();
                        upsell.UpsellTypeID = upsellType.UpsellTypeID;
                        upsell.BillingID = billing.BillingID;
                        upsell.ProductCode = upsellType.ProductCode;
                        upsell.Quantity = quantity;
                        upsell.CreateDT = DateTime.Now;
                        upsell.Complete = false;
                        dao.Save<Upsell>(upsell);

                        UpsellSale sale = new UpsellSale();
                        sale.SaleTypeID = SaleTypeEnum.Upsell;
                        sale.TrackingNumber = null;
                        sale.CreateDT = DateTime.Now;
                        sale.NotShip = false;
                        sale.UpsellID = upsell.UpsellID;
                        sale.ChargeHistoryID = null;
                        sale.PaygeaID = null;
                        dao.Save<UpsellSale>(sale);

                        //Add to result list
                        decimal saleAmount = CalculateUpsellSaleAmount(upsellType, quantity, null, 0);

                        saleList.Add(new KeyValuePair<Set<UpsellSale, Upsell, UpsellType>, decimal>(new Set<UpsellSale, Upsell, UpsellType>()
                        {
                            Value1 = sale,
                            Value2 = upsell,
                            Value3 = upsellType
                        }, saleAmount));
                    }

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;
                    NMICompany nmiCompany = null;

                    if (midID != null)
                    {
                        nmiCompany = merchantService.GetNMICompanyByAssertigyMID(midID);
                        assertigyMid = Load<AssertigyMID>(midID);

                        if (nmiCompany != null && assertigyMid != null)
                            mid = new Set<NMICompany, AssertigyMID>()
                            {
                                Value1 = nmiCompany,
                                Value2 = assertigyMid
                            };
                    }
                    else
                    {
                        if (existedBillingSubscription != null)
                        {
                            mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value);
                        }
                        if (mid == null)
                        {
                            mid = merchantService.ChooseRandomNMIMerchantAccount(upsellsProductID, billing, amount ?? 0);
                        }
                    }

                    if (mid != null)
                    {
                        nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = null;
                        Subscription subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
                        if (subscription != null && subscription.ProductID != null)
                        {
                            cur = GetCurrencyByProduct(subscription.ProductID.Value);
                        }

                        Product product = EnsureLoad<Product>(subscription.ProductID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount ?? 0, cur,
                            saleList.First().Key.Value1.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            saleList = null;
                            billingSubscription = existedBillingSubscription;

                            res.State = BusinessErrorState.Error;
                            res.ErrorMessage = paymentResult.ErrorMessage;

                            dao.BeginTransaction();
                        }

                        //if several upsells in one charge do not write ChargeDetails.ProductCode
                        string upsellProductCode = null;
                        if (upsellTypeList.Count == 1)
                        {
                            upsellProductCode = upsellTypeList.First().Key.ProductCode;
                        }
                        Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount ?? 0, cur);

                        if (saleList != null)
                        {
                            foreach (var item in saleList)
                            {
                                item.Key.Value1.PaygeaID = chargeLog.Value1.PaygeaID;
                                item.Key.Value1.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<UpsellSale>(item.Key.Value1);
                            }
                            if (saleList.Count > 1)
                            {
                                //Write amounts per sale
                                foreach (var item in saleList)
                                {
                                    ChargeHistoryExSale chPerSale = new ChargeHistoryExSale();
                                    chPerSale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                    chPerSale.SaleID = item.Key.Value1.SaleID;
                                    chPerSale.Amount = item.Value;
                                    if (cur != null)
                                    {
                                        chPerSale.CurrencyID = cur.CurrencyID;
                                        chPerSale.CurrencyAmount = chPerSale.Amount;
                                        chPerSale.Amount = cur.ConvertToUSD(chPerSale.CurrencyAmount.Value);
                                    }
                                    dao.Save<ChargeHistoryExSale>(chPerSale);
                                }
                            }
                        }
                        else if (existedBillingSubscription != null)
                        {
                            //If Upsell failed for existed BillingSubscription
                            if (paymentResult.ReturnValue.Response.ToLower().Contains("risk decline"))
                            {
                                DeclineUpsell declineUpsell = new DeclineUpsell();
                                declineUpsell.BillingID = billing.BillingID;
                                declineUpsell.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<DeclineUpsell>(declineUpsell);
                            }
                        }
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }

            if (res != null && res.State == BusinessErrorState.Success && saleList != null && saleList.Count > 0)
            {
                ValidateCustomer(billing);
                ValidateFraud(billing, saleList.First().Key.Value1);
            }

            if (res != null && res.State == BusinessErrorState.Success && saleList != null)
            {
                res.ReturnValue = new List<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>>();
                foreach (var item in saleList)
                {
                    res.ReturnValue.Add(new Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID, UpsellType>()
                    {
                        Value1 = item.Key.Value1,
                        Value2 = billingSubscription,
                        Value3 = item.Key.Value2,
                        Value4 = assertigyMid,
                        Value5 = item.Key.Value3
                    });
                }
            }

            return res;
        }

        public Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> CreateExtraTrialShipSale_BillingSale(Billing billing, Subscription subscription, bool isSpecialOffer, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList)
        {
            return CreateExtraTrialShipSale_BillingSale(billing, subscription, isSpecialOffer, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, null, null);
        }

        public Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> CreateExtraTrialShipSale_BillingSale(Billing billing, Subscription subscription, bool isSpecialOffer, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList, decimal? shippingAmount, int? shippingOption)
        {
            Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> res = null;

            ExtraTrialShipSale sale = null;
            BillingSubscription billingSubscription = null;
            BillingSubscription existedBillingSubscription = null;
            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));
            bool isTestAffiliateCase = (billing.Affiliate == TEST_AFFILIATE_CODE);

            bool doNotAllowDupes = false;
            if ((!isTestCase) && (!isTestAffiliateCase) && doNotAllowDupes && IsCreditCardDupe(billing.CreditCard))
            {
                //res.State = BusinessErrorState.Error;
                //res.ErrorMessage = "This credit card has been used previously for this special offer. We regret that because of the incredible deal, we can only Offer this discount once to each household.";
            }
            else
            {
                try
                {
                    dao.BeginTransaction();

                    existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);
                    //2010-11-30, use same Billing for multiple subscriptions
                    //if (existedBillingSubscription != null)
                    //{
                    //    billing = DuplicateBilling(billing);
                    //}

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res = null;
                }

                try
                {
                    dao.BeginTransaction();

                    billingSubscription = CreateBillingSubscription(billing, subscription);
                    SaveBillingSubscriptionDiscount(billingSubscription, isSpecialOffer, coupon);

                    if (!string.IsNullOrEmpty(partnerClickClickID))
                    {
                        PartnerClick click = new PartnerClick();
                        click.BillingID = billing.BillingID;
                        click.ClickID = partnerClickClickID;
                        dao.Save<PartnerClick>(click);
                    }

                    if (!isTestCase)
                    {
                        Set<ExtraTrialShipSale, ExtraTrialShip> saleRes = CreateExtraTrialShipSale(billing, subscription.ProductCode, subscription.Quantity.Value, shippingAmount, shippingOption);
                        sale = saleRes.Value1;

                        decimal amount = CalculateBillingSaleAmount(subscription, isSpecialOffer, coupon, shippingAmount);
                        if (refererCommissionRedeem != null)
                        {
                            amount -= RedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, amount, sale);
                        }
                        amount -= RedeemPromoGiftAmount(promoGiftList, amount, sale);

                        res = new Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>();
                        res.Value1 = sale;
                        res.Value2 = billingSubscription;
                        res.Value3 = saleRes.Value2;
                        res.Value4 = subscription;
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res = null;
                }
            }

            return res;
        }
        public Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> CreateExtraTrialShipSale_BillingSale(Billing billing, Subscription subscription, decimal subcost, bool isSpecialOffer, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList, decimal? shippingAmount, int? shippingOption)
        {
            Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription> res = null;

            ExtraTrialShipSale sale = null;
            BillingSubscription billingSubscription = null;
            BillingSubscription existedBillingSubscription = null;
            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));
            bool isTestAffiliateCase = (billing.Affiliate == TEST_AFFILIATE_CODE);

            bool doNotAllowDupes = false;
            if ((!isTestCase) && (!isTestAffiliateCase) && doNotAllowDupes && IsCreditCardDupe(billing.CreditCard))
            {
                //res.State = BusinessErrorState.Error;
                //res.ErrorMessage = "This credit card has been used previously for this special offer. We regret that because of the incredible deal, we can only Offer this discount once to each household.";
            }
            else
            {
                try
                {
                    dao.BeginTransaction();

                    existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);
                    //2010-11-30, use same Billing for multiple subscriptions
                    //if (existedBillingSubscription != null)
                    //{
                    //    billing = DuplicateBilling(billing);
                    //}

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res = null;
                }

                try
                {
                    dao.BeginTransaction();

                    billingSubscription = CreateBillingSubscription(billing, subscription);
                    SaveBillingSubscriptionDiscount(billingSubscription, isSpecialOffer, coupon);

                    if (!string.IsNullOrEmpty(partnerClickClickID))
                    {
                        PartnerClick click = new PartnerClick();
                        click.BillingID = billing.BillingID;
                        click.ClickID = partnerClickClickID;
                        dao.Save<PartnerClick>(click);
                    }

                    if (!isTestCase)
                    {
                        Set<ExtraTrialShipSale, ExtraTrialShip> saleRes = CreateExtraTrialShipSale(billing, subscription.ProductCode, subscription.Quantity.Value, shippingAmount, shippingOption);
                        sale = saleRes.Value1;

                        decimal amount = subcost;
                        if (refererCommissionRedeem != null)
                        {
                            amount -= RedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, amount, sale);
                        }
                        amount -= RedeemPromoGiftAmount(promoGiftList, amount, sale);

                        res = new Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, Subscription>();
                        res.Value1 = sale;
                        res.Value2 = billingSubscription;
                        res.Value3 = saleRes.Value2;
                        res.Value4 = subscription;
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res = null;
                }
            }

            return res;
        }

        public Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> CreateExtraTrialShipSale_UpsellSale(Billing billing, UpsellType upsellType, int quantity, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList)
        {
            return CreateExtraTrialShipSale_UpsellSale(billing, upsellType, quantity, coupon, referer, partnerClickClickID, ownReferer, refererCommissionRedeem, promoGiftList, null, null);
        }

        public Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> CreateExtraTrialShipSale_UpsellSale(Billing billing, UpsellType upsellType, int quantity, ICoupon coupon, Referer referer, string partnerClickClickID, Referer ownReferer, decimal? refererCommissionRedeem, IList<PromoGift> promoGiftList, decimal? shippingAmount, int? shippingOption)
        {
            Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType> res = null;

            ExtraTrialShipSale sale = null;
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            try
            {
                dao.BeginTransaction();

                BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);

                //If new Sale(without BillingSubscription) create Upsell Fake BillingSubscription
                billingSubscription = existedBillingSubscription;
                if (billingSubscription == null)
                {
                    billingSubscription = CreateUpsellFakeBillingSubscription(billing);
                    SaveBillingSubscriptionDiscount(billingSubscription, false, coupon);

                    if (!string.IsNullOrEmpty(partnerClickClickID))
                    {
                        PartnerClick click = new PartnerClick();
                        click.BillingID = billing.BillingID;
                        click.ClickID = partnerClickClickID;
                        dao.Save<PartnerClick>(click);
                    }
                }

                if (!isTestCase)
                {
                    Set<ExtraTrialShipSale, ExtraTrialShip> saleRes = CreateExtraTrialShipSale(billing, upsellType.ProductCode, quantity, shippingAmount, shippingOption);
                    sale = saleRes.Value1;

                    decimal amount = CalculateUpsellSaleAmount(upsellType, quantity, coupon, shippingAmount);
                    if (shippingAmount > 0)
                        amount += shippingAmount.Value;

                    if (refererCommissionRedeem != null)
                    {
                        amount -= RedeemRefererCommission(ownReferer, refererCommissionRedeem.Value, amount, sale);
                    }
                    amount -= RedeemPromoGiftAmount(promoGiftList, amount, sale);

                    res = new Set<ExtraTrialShipSale, BillingSubscription, ExtraTrialShip, UpsellType>();
                    res.Value1 = sale;
                    res.Value2 = billingSubscription;
                    res.Value3 = saleRes.Value2;
                    res.Value4 = upsellType;
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

        public Set<ExtraTrialShipSale, ExtraTrialShip> CreateExtraTrialShipSale(Billing billing, string productCode, int quantity)
        {
            return CreateExtraTrialShipSale(billing, productCode, quantity, null, null);
        }

        public Set<ExtraTrialShipSale, ExtraTrialShip> CreateExtraTrialShipSale(Billing billing, string productCode, int quantity, decimal? shippingAmount, int? shippingOption)
        {
            Set<ExtraTrialShipSale, ExtraTrialShip> res = new Set<ExtraTrialShipSale, ExtraTrialShip>();

            try
            {
                dao.BeginTransaction();

                res.Value2 = new ExtraTrialShip();
                res.Value2.ExtraTrialShipTypeID = null;
                res.Value2.BillingID = billing.BillingID;
                res.Value2.ProductCode = productCode;
                res.Value2.Quantity = quantity;
                res.Value2.CreateDT = DateTime.Now;
                res.Value2.Completed = false;
                dao.Save<ExtraTrialShip>(res.Value2);

                res.Value1 = new ExtraTrialShipSale();
                res.Value1.BillingID = billing.BillingID;
                res.Value1.SaleTypeID = SaleTypeEnum.ExtraTrialShip;
                res.Value1.TrackingNumber = null;
                res.Value1.CreateDT = DateTime.Now;
                res.Value1.NotShip = false;
                res.Value1.ExtraTrialShipID = res.Value2.ExtraTrialShipID;
                dao.Save<ExtraTrialShipSale>(res.Value1);

                if (shippingOption != null)
                {
                    MySqlCommand q = new MySqlCommand("INSERT INTO SaleShippingOption VALUES(@SaleID, @ShippingOptionID)");
                    q.Parameters.AddWithValue("@SaleID", res.Value1.SaleID);
                    q.Parameters.AddWithValue("@ShippingOptionID", shippingOption);
                    dao.ExecuteNonQuery(q);
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

        public Currency GetCurrencyByProduct(int productID)
        {
            Currency res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from Currency c " +
                    "inner join ProductCurrency pc on pc.CurrencyID = c.CurrencyID " +
                    "where pc.ProductID = @productID");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<Currency>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        private Set<Currency, ChargeHistoryExCurrency> GetCurrencyByCharge(long chargeHistoryID)
        {
            Set<Currency, ChargeHistoryExCurrency> res = null;

            try
            {
                ChargeHistoryExCurrency chCur = Load<ChargeHistoryExCurrency>(chargeHistoryID);
                if (chCur != null)
                {
                    res = new Set<Currency, ChargeHistoryExCurrency>();
                    res.Value2 = chCur;
                    res.Value1 = EnsureLoad<Currency>(chCur.CurrencyID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        private const string BILL_UPSELL_DISPLAY_NAME = "Bill Upsell";
        public const string BILL_UPSELL_PRODUCT_CODE = "BILL";
        private BusinessError<ChargeHistoryEx> BillAsUpsell_Old(Billing billing, int productID, decimal amount, decimal? shippingAmount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            Upsell upsell = null;
            UpsellSale sale = null;
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select * from UpsellType " +
                    "where ProductID = @productID and DisplayName = @displayName");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@displayName", MySqlDbType.VarChar).Value = BILL_UPSELL_DISPLAY_NAME;

                UpsellType upsellType = dao.Load<UpsellType>(q).LastOrDefault();
                if (upsellType == null)
                {
                    upsellType = new UpsellType();
                    upsellType.ProductCode = "BILL";
                    upsellType.Price = 0.00M;
                    upsellType.Quantity = 1;
                    upsellType.DisplayName = BILL_UPSELL_DISPLAY_NAME;
                    upsellType.DropDown = false;
                    upsellType.Description = BILL_UPSELL_DISPLAY_NAME;
                    upsellType.ProductID = productID;
                    dao.Save<UpsellType>(upsellType);
                }

                billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, upsellType.ProductID.Value);

                if (!isTestCase)
                {
                    upsell = new Upsell();
                    upsell.UpsellTypeID = upsellType.UpsellTypeID;
                    upsell.BillingID = billing.BillingID;
                    upsell.ProductCode = upsellType.ProductCode;
                    upsell.Quantity = 1;
                    upsell.CreateDT = DateTime.Now;
                    upsell.Complete = false;
                    dao.Save<Upsell>(upsell);

                    sale = new UpsellSale();
                    sale.SaleTypeID = SaleTypeEnum.Upsell;
                    sale.TrackingNumber = null;
                    sale.CreateDT = DateTime.Now;
                    sale.NotShip = false;
                    sale.UpsellID = upsell.UpsellID;
                    sale.ChargeHistoryID = null;
                    sale.PaygeaID = null;
                    dao.Save<UpsellSale>(sale);

                    if (shippingAmount != null)
                    {
                        amount += shippingAmount.Value;
                    }

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;
                    mid = merchantService.ChooseRandomNMIMerchantAccount(upsellType.ProductID.Value, billing, amount);

                    if (mid != null)
                    {
                        NMICompany nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = null;
                        Subscription subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
                        if (subscription != null && subscription.ProductID != null)
                        {
                            cur = GetCurrencyByProduct(subscription.ProductID.Value);
                        }

                        if (shippingAmount != null)
                        {
                            SaleChargeDetails saleCD = new SaleChargeDetails();
                            saleCD.SaleID = sale.SaleID;
                            saleCD.SaleChargeTypeID = SaleChargeTypeEnum.Shipping;
                            saleCD.Amount = shippingAmount.Value;
                            if (cur != null)
                            {
                                saleCD.CurrencyID = cur.CurrencyID;
                                saleCD.CurrencyAmount = saleCD.Amount;
                                saleCD.Amount = cur.ConvertToUSD(saleCD.CurrencyAmount.Value);
                            }
                            dao.Save<SaleChargeDetails>(saleCD);
                        }

                        Product product = EnsureLoad<Product>(subscription.ProductID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            sale.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            upsell = null;
                            sale = null;
                            billingSubscription = null;

                            res.State = BusinessErrorState.Error;

                            dao.BeginTransaction();
                        }

                        Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellType.ProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                        if (sale != null)
                        {
                            sale.PaygeaID = chargeLog.Value1.PaygeaID;
                            sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                            dao.Save<UpsellSale>(sale);

                            emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);
                        }

                        res.ErrorMessage = chargeLog.Value1.Response;
                        res.ReturnValue = chargeLog.Value2;
                    }
                    else
                    {
                        res.ErrorMessage = "No MID installed";
                    }
                }
                else
                {
                    if (shippingAmount != null)
                    {
                        amount += shippingAmount.Value;
                    }
                    if (billing.FirstName.ToLower() == "success")
                    {
                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = new ChargeHistoryEx()
                        {
                            ChargeHistoryID = 0,
                            ChildMID = "Test",
                            ChargeDate = DateTime.Now,
                            Success = true,
                            Amount = amount
                        };
                        res.ErrorMessage = "Test case: SUCCESS";
                    }
                    else
                    {
                        res.State = BusinessErrorState.Error;
                        res.ReturnValue = null;
                        res.ErrorMessage = "Test case: ERROR";
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            return res;
        }

        public BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> BillAsUpsell(int billingID, int productID, int productTypeID, decimal amount)
        {
            Billing b = dao.Load<Billing>(billingID);
            ProductCode pc = dao.Load<ProductCode>(productID);

            if (b != null && pc != null)
                return BillAsUpsell(b, productTypeID, pc, amount, 0, "");
            else
                return new BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>>()
                    {
                        State = BusinessErrorState.Error,
                        ErrorMessage = "Billing or ProductCode is null"
                    };
        }


        protected BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> BillAsUpsell(Billing billing, int productID, ProductCode productCode, decimal amount, decimal? shippingAmount, string description)
        {
            BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> res = new BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            res.ReturnValue = new Set<ChargeHistoryEx, FailedChargeHistoryView>();

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            Upsell upsell = null;
            UpsellSale sale = null;
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            //Create subscription in any case
            BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBillingAndProduct(billing.BillingID, productID);
            billingSubscription = existedBillingSubscription;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                string upsellProductCode = BILL_UPSELL_PRODUCT_CODE;
                if (productCode != null)
                {
                    upsellProductCode = productCode.ProductCode_;
                }

                MySqlCommand q = new MySqlCommand("select * from UpsellType " +
                    "where ProductID = @productID and ProductCode = @productCode and Quantity = 1");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@productCode", MySqlDbType.VarChar).Value = upsellProductCode;

                UpsellType upsellType = dao.Load<UpsellType>(q).LastOrDefault();
                if (upsellType == null)
                {
                    upsellType = new UpsellType();
                    upsellType.ProductCode = upsellProductCode;
                    upsellType.Price = 0.00M;
                    upsellType.Quantity = 1;
                    upsellType.DisplayName = string.Empty;
                    upsellType.DropDown = false;
                    upsellType.Description = string.Empty;
                    upsellType.ProductID = productID;
                    dao.Save<UpsellType>(upsellType);
                }

                if (!isTestCase)
                {
                    if (billingSubscription == null)
                    {
                        billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, productID);
                    }

                    upsell = new Upsell();
                    upsell.UpsellTypeID = upsellType.UpsellTypeID;
                    upsell.BillingID = billing.BillingID;
                    upsell.ProductCode = upsellType.ProductCode;
                    upsell.Quantity = upsellType.Quantity;
                    upsell.CreateDT = DateTime.Now;
                    upsell.Complete = false;
                    dao.Save<Upsell>(upsell);

                    sale = new UpsellSale();
                    sale.SaleTypeID = SaleTypeEnum.Upsell;
                    sale.TrackingNumber = null;
                    sale.CreateDT = DateTime.Now;
                    sale.NotShip = (upsellType.ProductCode == BILL_UPSELL_PRODUCT_CODE);
                    sale.UpsellID = upsell.UpsellID;
                    sale.ChargeHistoryID = null;
                    sale.PaygeaID = null;
                    dao.Save<UpsellSale>(sale);

                    if (shippingAmount != null)
                    {
                        amount += shippingAmount.Value;
                    }

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;
                    if (existedBillingSubscription != null)
                    {
                        mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value, upsellType.ProductID.Value);
                    }
                    if (mid == null)
                    {
                        mid = merchantService.ChooseRandomNMIMerchantAccount(upsellType.ProductID.Value, billing, amount);
                    }

                    if (mid != null)
                    {
                        NMICompany nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = GetCurrencyByProduct(upsellType.ProductID.Value);

                        if (shippingAmount != null)
                        {
                            SaleChargeDetails saleCD = new SaleChargeDetails();
                            saleCD.SaleID = sale.SaleID;
                            saleCD.SaleChargeTypeID = SaleChargeTypeEnum.Shipping;
                            saleCD.Amount = shippingAmount.Value;
                            if (cur != null)
                            {
                                saleCD.CurrencyID = cur.CurrencyID;
                                saleCD.CurrencyAmount = saleCD.Amount;
                                saleCD.Amount = cur.ConvertToUSD(saleCD.CurrencyAmount.Value);
                            }
                            dao.Save<SaleChargeDetails>(saleCD);
                        }

                        if (!string.IsNullOrEmpty(description))
                        {
                            SaleDetails sd = new SaleDetails();
                            sd.SaleID = sale.SaleID;
                            sd.Description = description;
                            dao.Save<SaleDetails>(sd);
                        }

                        Product product = EnsureLoad<Product>(upsellType.ProductID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            sale.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;

                            //if (existedBillingSubscription != null && billingSubscription.StatusTID == BillingSubscriptionStatusEnum.Scrubbed)
                            //{
                            //    billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Active;
                            //}
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            upsell = null;
                            sale = null;
                            billingSubscription = existedBillingSubscription;
                            //if (existedBillingSubscription == null)
                            //{
                            //    billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                            //}

                            res.State = BusinessErrorState.Error;

                            dao.BeginTransaction();
                        }

                        //if new billingSubscription created and charge failed, set to scrubbed
                        //if new billingSubscription created and charge success and status is scrubbed, set to active
                        //dao.Save<BillingSubscription>(billingSubscription);

                        Set<Paygea, ChargeHistoryEx, FailedChargeHistoryView> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellType.ProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                        if (sale != null)
                        {
                            sale.PaygeaID = chargeLog.Value1.PaygeaID;
                            sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                            dao.Save<UpsellSale>(sale);
                        }

                        res.ErrorMessage = chargeLog.Value1.Response;
                        res.ReturnValue.Value1 = chargeLog.Value2;
                        res.ReturnValue.Value2 = chargeLog.Value3;
                    }
                    else
                    {
                        if (existedBillingSubscription == null)
                        {
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Inactive;
                            dao.Save<BillingSubscription>(billingSubscription);
                        }
                        res.ErrorMessage = "No MID installed";
                    }
                }
                else
                {
                    if (shippingAmount != null)
                    {
                        amount += shippingAmount.Value;
                    }

                    if (billing.FirstName.ToLower() == "success")
                    {
                        if (billingSubscription == null)
                        {
                            billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, productID);
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                            dao.Save<BillingSubscription>(billingSubscription);
                        }

                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                        {
                            Value1 = new ChargeHistoryEx()
                            {
                                ChargeHistoryID = 0,
                                ChildMID = "Test",
                                ChargeDate = DateTime.Now,
                                Success = true,
                                Amount = amount,
                                Response = "response=1&responsetext=Test case: SUCCESS&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=100"
                            },
                            Value2 = null
                        };
                        res.ErrorMessage = "Test case: SUCCESS";
                    }
                    else
                    {
                        res.State = BusinessErrorState.Error;
                        res.ReturnValue = new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                        {
                            Value2 = new FailedChargeHistoryView()
                            {
                                FailedChargeHistoryID = 0,
                                MerchantAccountID = null,
                                ChildMID = "Test",
                                BillingID = billing.BillingID,
                                SaleTypeID = SaleTypeEnum.Upsell,
                                ChargeDate = DateTime.Now,
                                Success = false,
                                Amount = amount,
                                Response = "response=2&responsetext=Test case: ERROR&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=200"
                            },
                            Value1 = null
                        };
                        res.ErrorMessage = "Test case: ERROR";
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res.State = BusinessErrorState.Error;
            }

            if (res != null && res.State == BusinessErrorState.Success)
            {
                emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);
                //emailService.SendConfirmationEmail(productID, billing, sale);
                try
                {
                    new EventService().RegistrationAndConfirmation(null, productID, billing.Email, billing.Zip, billing.Phone, billing.FirstName, billing.LastName, billing.RegistrationID, EventTypeEnum.OrderConfirmation);
                }
                catch
                { }
            }

            return res;
        }

        private BusinessError<ChargeHistoryWithSalesView> BillAsUpsell(Billing billing, int productID, IList<KeyValuePair<ProductCode, decimal>> productCodeList)
        {
            BusinessError<ChargeHistoryWithSalesView> res = new BusinessError<ChargeHistoryWithSalesView>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            res.ReturnValue = new ChargeHistoryWithSalesView();
            res.ReturnValue.ChargeHistoryView = new Set<ChargeHistoryEx, FailedChargeHistoryView>();
            res.ReturnValue.SaleList = new List<Set<UpsellSale, ProductCode, ChargeHistoryExSale>>();

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            if (productCodeList == null || productCodeList.Count == 0)
            {
                return res;
            }

            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            IList<KeyValuePair<Set<UpsellSale, Upsell, ProductCode, ChargeHistoryExSale>, decimal>> saleList = new List<KeyValuePair<Set<UpsellSale, Upsell, ProductCode, ChargeHistoryExSale>, decimal>>();
            //Create subscription in any case
            BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBillingAndProduct(billing.BillingID, productID);
            billingSubscription = existedBillingSubscription;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                if (!isTestCase)
                {
                    if (billingSubscription == null)
                    {
                        billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, productID);
                    }

                    decimal amount = 0M;

                    foreach (var productCode in productCodeList)
                    {
                        string upsellProductCode = productCode.Key.ProductCode_;
                        MySqlCommand q = new MySqlCommand("select * from UpsellType " +
                            "where ProductID = @productID and ProductCode = @productCode and Quantity = 1");
                        q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@productCode", MySqlDbType.VarChar).Value = upsellProductCode;

                        UpsellType upsellType = dao.Load<UpsellType>(q).LastOrDefault();
                        if (upsellType == null)
                        {
                            upsellType = new UpsellType();
                            upsellType.ProductCode = upsellProductCode;
                            upsellType.Price = 0.00M;
                            upsellType.Quantity = 1;
                            upsellType.DisplayName = string.Empty;
                            upsellType.DropDown = false;
                            upsellType.Description = string.Empty;
                            upsellType.ProductID = productID;
                            dao.Save<UpsellType>(upsellType);
                        }

                        Upsell upsell = new Upsell();
                        upsell.UpsellTypeID = upsellType.UpsellTypeID;
                        upsell.BillingID = billing.BillingID;
                        upsell.ProductCode = upsellType.ProductCode;
                        upsell.Quantity = upsellType.Quantity;
                        upsell.CreateDT = DateTime.Now;
                        upsell.Complete = false;
                        dao.Save<Upsell>(upsell);

                        UpsellSale sale = new UpsellSale();
                        sale.SaleTypeID = SaleTypeEnum.Upsell;
                        sale.TrackingNumber = null;
                        sale.CreateDT = DateTime.Now;
                        sale.NotShip = (upsellType.ProductCode == BILL_UPSELL_PRODUCT_CODE);
                        sale.UpsellID = upsell.UpsellID;
                        sale.ChargeHistoryID = null;
                        sale.PaygeaID = null;
                        dao.Save<UpsellSale>(sale);

                        //Add to result list
                        saleList.Add(new KeyValuePair<Set<UpsellSale, Upsell, ProductCode, ChargeHistoryExSale>, decimal>(new Set<UpsellSale, Upsell, ProductCode, ChargeHistoryExSale>()
                        {
                            Value1 = sale,
                            Value2 = upsell,
                            Value3 = productCode.Key,
                            Value4 = null
                        }, productCode.Value));

                        amount += productCode.Value;
                    }

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;
                    if (existedBillingSubscription != null)
                    {
                        mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value, productID);
                    }
                    if (mid == null)
                    {
                        mid = merchantService.ChooseRandomNMIMerchantAccount(productID, billing, amount);
                    }

                    if (mid != null)
                    {
                        NMICompany nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = GetCurrencyByProduct(productID);

                        Product product = EnsureLoad<Product>(productID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            saleList.First().Key.Value1.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;

                            //if (existedBillingSubscription != null && billingSubscription.StatusTID == BillingSubscriptionStatusEnum.Scrubbed)
                            //{
                            //    billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Active;
                            //}
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            saleList = null;
                            billingSubscription = existedBillingSubscription;
                            //if (existedBillingSubscription == null)
                            //{
                            //    billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                            //}

                            res.State = BusinessErrorState.Error;

                            dao.BeginTransaction();
                        }

                        //if new billingSubscription created and charge failed, set to scrubbed
                        //if new billingSubscription created and charge success and status is scrubbed, set to active
                        //dao.Save<BillingSubscription>(billingSubscription);

                        //if several upsells in one charge do not write ChargeDetails.ProductCode
                        string upsellProductCode = null;
                        if (productCodeList.Count == 1)
                        {
                            upsellProductCode = productCodeList.First().Key.ProductCode_;
                        }

                        Set<Paygea, ChargeHistoryEx, FailedChargeHistoryView> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                        if (saleList != null)
                        {
                            foreach (var item in saleList)
                            {
                                item.Key.Value1.PaygeaID = chargeLog.Value1.PaygeaID;
                                item.Key.Value1.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<UpsellSale>(item.Key.Value1);
                            }
                            if (saleList.Count > 1)
                            {
                                //Write amounts per sale
                                foreach (var item in saleList)
                                {
                                    ChargeHistoryExSale chPerSale = new ChargeHistoryExSale();
                                    chPerSale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                    chPerSale.SaleID = item.Key.Value1.SaleID;
                                    chPerSale.Amount = item.Value;
                                    if (cur != null)
                                    {
                                        chPerSale.CurrencyID = cur.CurrencyID;
                                        chPerSale.CurrencyAmount = chPerSale.Amount;
                                        chPerSale.Amount = cur.ConvertToUSD(chPerSale.CurrencyAmount.Value);
                                    }
                                    dao.Save<ChargeHistoryExSale>(chPerSale);
                                    item.Key.Value4 = chPerSale;
                                }
                            }
                        }

                        res.ErrorMessage = chargeLog.Value1.Response;
                        res.ReturnValue.ChargeHistoryView.Value1 = chargeLog.Value2;
                        res.ReturnValue.ChargeHistoryView.Value2 = chargeLog.Value3;
                        if (saleList != null)
                        {
                            foreach (var item in saleList)
                            {
                                res.ReturnValue.SaleList.Add(new Set<UpsellSale, ProductCode, ChargeHistoryExSale>()
                                {
                                    Value1 = item.Key.Value1,
                                    Value2 = item.Key.Value3,
                                    Value3 = item.Key.Value4
                                });
                            }
                        }
                    }
                    else
                    {
                        if (existedBillingSubscription == null)
                        {
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Inactive;
                            dao.Save<BillingSubscription>(billingSubscription);
                        }
                        res.ErrorMessage = "No MID installed";
                    }
                }
                else
                {
                    decimal amount = productCodeList.Sum(i => i.Value);

                    if (billing.FirstName.ToLower() == "success")
                    {
                        if (billingSubscription == null)
                        {
                            billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, productID);
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                            dao.Save<BillingSubscription>(billingSubscription);
                        }

                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = new ChargeHistoryWithSalesView()
                        {
                            ChargeHistoryView = new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                            {
                                Value1 = new ChargeHistoryEx()
                                {
                                    ChargeHistoryID = 0,
                                    ChildMID = "Test",
                                    ChargeDate = DateTime.Now,
                                    Success = true,
                                    Amount = amount,
                                    Response = "response=1&responsetext=Test case: SUCCESS&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=100"
                                },
                                Value2 = null
                            },
                            SaleList = productCodeList.Select(i => new Set<UpsellSale, ProductCode, ChargeHistoryExSale>()
                            {
                                Value1 = new UpsellSale()
                                {
                                    SaleID = 0
                                },
                                Value2 = i.Key,
                                Value3 = new ChargeHistoryExSale()
                                {
                                    Amount = i.Value
                                }
                            }).ToList()
                        };
                        res.ErrorMessage = "Test case: SUCCESS";
                    }
                    else
                    {
                        res.State = BusinessErrorState.Error;
                        res.ReturnValue = new ChargeHistoryWithSalesView()
                        {
                            ChargeHistoryView = new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                            {
                                Value2 = new FailedChargeHistoryView()
                                {
                                    FailedChargeHistoryID = 0,
                                    MerchantAccountID = null,
                                    ChildMID = "Test",
                                    BillingID = billing.BillingID,
                                    SaleTypeID = SaleTypeEnum.Upsell,
                                    ChargeDate = DateTime.Now,
                                    Success = false,
                                    Amount = amount,
                                    Response = "response=2&responsetext=Test case: ERROR&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=200"
                                },
                                Value1 = null
                            },
                            SaleList = null
                        };
                        res.ErrorMessage = "Test case: ERROR";
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res.State = BusinessErrorState.Error;
            }

            if (res != null &&
                res.State == BusinessErrorState.Success &&
                res.ReturnValue != null &&
                res.ReturnValue.ChargeHistoryView != null &&
                res.ReturnValue.ChargeHistoryView.Value1 != null &&
                res.ReturnValue.ChargeHistoryView.Value1.Amount != null &&
                assertigyMid != null)
            {
                foreach (var sale in res.ReturnValue.SaleList)
                    emailService.PushConfirmationEmailToQueue(billing.BillingID, sale.Value1 == null ? null : sale.Value1.SaleID);

                //emailService.SendConfirmationEmail(productID, billing, assertigyMid.DisplayName,
                //    0M, res.ReturnValue.ChargeHistoryView.Value1.Amount.Value,
                //    string.Empty, string.Empty, res.ReturnValue);
                try
                {
                    new EventService().RegistrationAndConfirmation(null, productID, billing.Email, billing.Zip, billing.Phone, billing.FirstName, billing.LastName, billing.RegistrationID, EventTypeEnum.OrderConfirmation);
                }
                catch
                { }
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> DoRefund(ChargeHistoryEx chargeHistory, decimal refundAmount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");

            try
            {
                dao.BeginTransaction();

                BillingSubscription billingSubscription = EnsureLoad<BillingSubscription>(chargeHistory.BillingSubscriptionID);
                Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);
                Billing billing = EnsureLoad<Billing>(billingSubscription.BillingID);

                Sale sale = GetSaleByChargeHistory(chargeHistory.ChargeHistoryID.Value);
                if (sale == null)
                {
                    throw new Exception(string.Format("Can't find Sale for ChargeHistoryEx({0})", chargeHistory.ChargeHistoryID));
                }

                AssertigyMID assertigyMID = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);

                NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMID.AssertigyMIDID);
                if (nmiCompany == null)
                {
                    throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", assertigyMID.AssertigyMIDID));
                }

                decimal chargeCurrencyAmount = chargeHistory.Amount.Value;
                Currency cur = null;
                Set<Currency, ChargeHistoryExCurrency> chCur = GetCurrencyByCharge(chargeHistory.ChargeHistoryID.Value);
                if (chCur != null)
                {
                    chargeCurrencyAmount = chCur.Value2.CurrencyAmount.Value;
                    cur = chCur.Value1;
                }

                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMID);
                //do refund
                BusinessError<GatewayResult> refundResult = paymentGateway.Refund(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, chargeHistory, refundAmount, cur);
                Set<Paygea, ChargeHistoryEx> chRes = ChargeLogging(refundResult, billing, billingSubscription, subscription.ProductCode, sale.SaleTypeID.Value, assertigyMID, ChargeTypeEnum.Refund, -refundAmount, cur);

                res.State = refundResult.State;
                if (res.State == BusinessErrorState.Success)
                {
                    //Just in case if ChargeLogging has failed
                    res.ErrorMessage = null;
                }

                if (chRes != null)
                {
                    res.ReturnValue = chRes.Value2;
                    res.ErrorMessage = chRes.Value1.Response;

                    SaleRefund sr = new SaleRefund();
                    sr.SaleID = sale.SaleID;
                    sr.ChargeHistoryID = chRes.Value2.ChargeHistoryID;
                    dao.Save<SaleRefund>(sr);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> DoRefund(ChargeHistoryEx chargeHistory, Sale sale, decimal refundAmount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");

            try
            {
                dao.BeginTransaction();

                BillingSubscription billingSubscription = EnsureLoad<BillingSubscription>(chargeHistory.BillingSubscriptionID);
                Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);
                Billing billing = EnsureLoad<Billing>(billingSubscription.BillingID);

                AssertigyMID assertigyMID = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);

                NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMID.AssertigyMIDID);
                if (nmiCompany == null)
                {
                    throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", assertigyMID.AssertigyMIDID));
                }

                decimal chargeCurrencyAmount = chargeHistory.Amount.Value;
                Currency cur = null;
                Set<Currency, ChargeHistoryExCurrency> chCur = GetCurrencyByCharge(chargeHistory.ChargeHistoryID.Value);
                if (chCur != null)
                {
                    chargeCurrencyAmount = chCur.Value2.CurrencyAmount.Value;
                    cur = chCur.Value1;
                }

                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMID);
                //do refund
                BusinessError<GatewayResult> refundResult = paymentGateway.Refund(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, chargeHistory, refundAmount, cur);
                Set<Paygea, ChargeHistoryEx> chRes = ChargeLogging(refundResult, billing, billingSubscription, subscription.ProductCode, sale.SaleTypeID.Value, assertigyMID, ChargeTypeEnum.Refund, -refundAmount, cur);

                res.State = refundResult.State;
                if (res.State == BusinessErrorState.Success)
                {
                    //Just in case if ChargeLogging has failed
                    res.ErrorMessage = null;
                }

                if (chRes != null)
                {
                    res.ReturnValue = chRes.Value2;
                    res.ErrorMessage = chRes.Value1.Response;

                    SaleRefund sr = new SaleRefund();
                    sr.SaleID = sale.SaleID;
                    sr.ChargeHistoryID = chRes.Value2.ChargeHistoryID;
                    dao.Save<SaleRefund>(sr);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> DoVoid(ChargeHistoryEx chargeHistory)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");

            try
            {
                dao.BeginTransaction();

                if (chargeHistory.ChargeTypeID != ChargeTypeEnum.Charge &&
                    chargeHistory.ChargeTypeID != ChargeTypeEnum.AuthOnly)
                {
                    res.ErrorMessage = "Can't apply Void to this type of transaction";
                }

                BillingSubscription billingSubscription = EnsureLoad<BillingSubscription>(chargeHistory.BillingSubscriptionID);
                Billing billing = EnsureLoad<Billing>(billingSubscription.BillingID);

                Sale sale = GetSaleByChargeHistory(chargeHistory.ChargeHistoryID.Value);
                if (sale == null && chargeHistory.ChargeTypeID == ChargeTypeEnum.Charge)
                {
                    throw new Exception(string.Format("Can't find Sale for ChargeHistoryEx({0})", chargeHistory.ChargeHistoryID));
                }

                AssertigyMID assertigyMID = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);

                NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMID.AssertigyMIDID);
                if (nmiCompany == null)
                {
                    throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", assertigyMID.AssertigyMIDID));
                }

                int? saleTypeID = (sale != null ? sale.SaleTypeID : null);

                decimal chargeCurrencyAmount = chargeHistory.Amount.Value;
                Currency cur = null;
                int chargeTypeID = 0;
                if (chargeHistory.ChargeTypeID == ChargeTypeEnum.AuthOnly)
                {
                    chargeTypeID = ChargeTypeEnum.VoidAuthOnly;
                    AuthOnlyChargeDetails d = dao.Load<AuthOnlyChargeDetails>(chargeHistory.ChargeHistoryID);
                    if (d != null)
                    {
                        if (d.RequestedCurrencyAmount != null && d.RequestedCurrencyID != null)
                        {
                            chargeCurrencyAmount = d.RequestedCurrencyAmount.Value;
                            cur = dao.Load<Currency>(d.RequestedCurrencyID);
                        }
                        else
                        {
                            chargeCurrencyAmount = d.RequestedAmount.Value;
                        }
                    }
                }
                else
                {
                    chargeTypeID = ChargeTypeEnum.Void;
                    Set<Currency, ChargeHistoryExCurrency> chCur = GetCurrencyByCharge(chargeHistory.ChargeHistoryID.Value);
                    if (chCur != null)
                    {
                        chargeCurrencyAmount = chCur.Value2.CurrencyAmount.Value;
                        cur = chCur.Value1;
                    }
                }
                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMID);
                //do void
                BusinessError<GatewayResult> refundResult = paymentGateway.Void(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, chargeHistory, chargeCurrencyAmount, cur);
                Set<Paygea, ChargeHistoryEx> chRes = ChargeLogging(refundResult, billing, billingSubscription, null, saleTypeID, assertigyMID, chargeTypeID, -chargeCurrencyAmount, cur);

                res.State = refundResult.State;
                if (res.State == BusinessErrorState.Success)
                {
                    //Just in case if ChargeLogging has failed
                    res.ErrorMessage = null;
                }

                if (chRes != null)
                {
                    res.ReturnValue = chRes.Value2;
                    res.ErrorMessage = chRes.Value1.Response;

                    if (sale != null)
                    {
                        SaleRefund sr = new SaleRefund();
                        sr.SaleID = sale.SaleID;
                        sr.ChargeHistoryID = chRes.Value2.ChargeHistoryID;
                        dao.Save<SaleRefund>(sr);
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> BillAsUpsell_Old(int productID, decimal amount, decimal? shippingAmount, string firstName, string lastName,
            string address1, string address2, string city, string state, string zip,
            string phone, string email, string ip, string affiliate, string subAffiliate, string internalID,
            int paymentTypeID, string creditCard, string cvv, int expMonth, int expYear)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                IList<string> insufficientFields = new List<string>();
                if (string.IsNullOrEmpty(firstName)) insufficientFields.Add("firstName");
                if (string.IsNullOrEmpty(lastName)) insufficientFields.Add("lastName");
                if (string.IsNullOrEmpty(address1)) insufficientFields.Add("address1");
                if (string.IsNullOrEmpty(city)) insufficientFields.Add("city");
                if (string.IsNullOrEmpty(state)) insufficientFields.Add("state");
                if (string.IsNullOrEmpty(zip)) insufficientFields.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ip");
                //if (string.IsNullOrEmpty(internalID)) insufficientFields.Add("internalID");
                if (string.IsNullOrEmpty(creditCard)) insufficientFields.Add("creditCard");
                //if (string.IsNullOrEmpty(cvv)) insufficientFields.Add("cvv");

                if (insufficientFields.Count > 0)
                {
                    res.ErrorMessage = string.Format("Insufficient data, the following fields are required: {0}.", string.Join(", ", insufficientFields.ToArray()));
                }
                else if (expMonth < 1 || expMonth > 12 || expYear < 2000 || expYear > 2100)
                {
                    res.ErrorMessage = string.Format("Invalid expire date.");
                }
                else if (amount <= 0M)
                {
                    res.ErrorMessage = string.Format("Invalid amount.");
                }
                else if (paymentTypeID != PaymentTypeEnum.Visa && paymentTypeID != PaymentTypeEnum.Mastercard)
                {
                    res.ErrorMessage = string.Format("Invalid payment type.");
                }
                else if (!CreditCard.ValidateDecryptedFormat(creditCard))
                {
                    res.ErrorMessage = string.Format("Invalid Credit Card.");
                }
                else
                {
                    Set<Registration, Billing> billing = registrationService.CreateOrUpdateRegistrationAndBilling(null, null,
                        firstName, lastName, address1, address2, city, state, zip, null,
                        phone, email, null, affiliate, subAffiliate, ip, null,
                        paymentTypeID, creditCard, cvv, expMonth, expYear);

                    if (billing == null)
                    {
                        throw new Exception("Can't create Billing and Registration");
                    }
                    else
                    {
                        BillingExternalInfo billingInfo = new BillingExternalInfo();
                        billingInfo.BillingID = billing.Value2.BillingID;
                        billingInfo.InternalID = internalID;
                        dao.Save<BillingExternalInfo>(billingInfo);
                    }

                    res = BillAsUpsell_Old(billing.Value2, productID, amount, shippingAmount);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> BillAsUpsell(int productID, decimal amount,
            string internalID)
        {
            BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> res = new BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                if (amount <= 0M)
                {
                    res.ErrorMessage = string.Format("Invalid amount.");
                }
                else
                {
                    Billing existed = billingService.GetLastBillingByInternalID(internalID);
                    if (existed != null)
                    {
                        res = BillAsUpsell(existed, productID, null, amount, null, null);
                    }
                    else
                    {
                        res.ErrorMessage = string.Format("Customer #" + (internalID ?? string.Empty) + " not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> BillAsUpsell(int productID, int? productCodeID,
            decimal amount, decimal? shippingAmount,
            int? campaignID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID, long? registrationID,
            int? paymentTypeID,
            string creditCard, string cvv, int expMonth, int expYear,
            string description,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> res = new BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                IList<string> insufficientFields = new List<string>();
                if (string.IsNullOrEmpty(firstName)) insufficientFields.Add("firstName");
                if (string.IsNullOrEmpty(lastName)) insufficientFields.Add("lastName");
                if (string.IsNullOrEmpty(address1)) insufficientFields.Add("address1");
                if (string.IsNullOrEmpty(city)) insufficientFields.Add("city");
                if (string.IsNullOrEmpty(state)) insufficientFields.Add("state");
                if (string.IsNullOrEmpty(zip)) insufficientFields.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ip");
                //if (string.IsNullOrEmpty(internalID)) insufficientFields.Add("internalID");
                if (string.IsNullOrEmpty(creditCard)) insufficientFields.Add("creditCard");
                //if (string.IsNullOrEmpty(cvv)) insufficientFields.Add("cvv");

                decimal tempAmount = amount;
                if (shippingAmount != null)
                    tempAmount += shippingAmount.Value;

                if (insufficientFields.Count > 0)
                {
                    res.ErrorMessage = string.Format("Insufficient data, the following fields are required: {0}.", string.Join(", ", insufficientFields.ToArray()));
                }
                else if (expMonth < 1 || expMonth > 12 || expYear < 2000 || expYear > 2100)
                {
                    res.ErrorMessage = string.Format("Invalid expire date.");
                }
                else if (tempAmount <= 0M)
                {
                    res.ErrorMessage = string.Format("Invalid amount.");
                }
                else if (paymentTypeID != null &&
                    paymentTypeID != PaymentTypeEnum.Visa &&
                    paymentTypeID != PaymentTypeEnum.Mastercard &&
                    paymentTypeID != PaymentTypeEnum.AmericanExpress &&
                    paymentTypeID != PaymentTypeEnum.Discover)
                {
                    res.ErrorMessage = string.Format("Invalid payment type.");
                }
                else if (!CreditCard.ValidateDecryptedFormat(creditCard))
                {
                    res.ErrorMessage = string.Format("Invalid Credit Card.");
                }
                else if (!string.IsNullOrEmpty(description) && description.Length > 1000)
                {
                    res.ErrorMessage = string.Format("Field description exceeds max length 1000 characters.");
                }
                else
                {
                    //Check product
                    Product p = Load<Product>(productID);
                    if (p == null)
                    {
                        res.ErrorMessage = "ProductType(" + productID.ToString() + ") is not installed.";
                    }
                    else
                    {
                        //Check ProductCode
                        ProductCode productCode = null;
                        if (productCodeID != null)
                        {
                            productCode = dao.Load<ProductCode>(productCodeID);
                        }
                        if (productCode == null && productCodeID != null)
                        {
                            res.ErrorMessage = "Product(" + productCodeID.Value.ToString() + ") is not installed.";
                        }
                        else
                        {
                            if (campaignID != null)
                            {
                                Campaign cmp = Load<Campaign>(campaignID);
                                if (cmp == null)
                                {
                                    campaignID = null;
                                }
                            }

                            BusinessError<Set<Registration, Billing>> billing = billingService.CreateOrUpdateBillingByProspectOrInternalID(registrationID, internalID,
                                firstName, lastName, address1, address2, city, state, zip, country,
                                phone, email, campaignID, affiliate, subAffiliate, ip, null,
                                paymentTypeID, creditCard, cvv, expMonth, expYear,
                                customField1, customField2, customField3, customField4, customField5);

                            if (billing.State == BusinessErrorState.Error)
                            {
                                res.ErrorMessage = billing.ErrorMessage;
                            }
                            else
                            {
                                try
                                {
                                    if (registrationID == null && billingService.GetLastBillingByInternalID(internalID) == null)
                                    {
                                        //was created new Registration

                                        if (billing.ReturnValue.Value1 != null)
                                        {
                                            new EventService().RegistrationAndConfirmation(null, productID, email, zip, phone, firstName, lastName, billing.ReturnValue.Value1.RegistrationID, EventTypeEnum.Registration);
                                        }
                                    }
                                }
                                catch
                                { }

                                // get or create referer
                                try
                                {
                                    Referer ownReferer = null;
                                    BusinessError<Referer> createRefererResult = refererService.CreateOrGetRefererFromBilling(billing.ReturnValue.Value2, null, null);
                                    if (createRefererResult != null && createRefererResult.State == BusinessErrorState.Success)
                                    {
                                        ownReferer = createRefererResult.ReturnValue;
                                    }

                                    if (ownReferer != null)
                                    {
                                        refererService.AddBillingToReferer(ownReferer, billing.ReturnValue.Value2);
                                    }
                                }
                                catch
                                { }

                                res = BillAsUpsell(billing.ReturnValue.Value2, productID, productCode, amount, shippingAmount, description);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryWithSalesView> BillAsUpsell(int productID, IList<KeyValuePair<int, decimal>> productCodeList,
            int? campaignID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID, long? registrationID,
            int? paymentTypeID,
            string creditCard, string cvv, int expMonth, int expYear,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<ChargeHistoryWithSalesView> res = new BusinessError<ChargeHistoryWithSalesView>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                IList<string> insufficientFields = new List<string>();
                if (string.IsNullOrEmpty(firstName)) insufficientFields.Add("firstName");
                if (string.IsNullOrEmpty(lastName)) insufficientFields.Add("lastName");
                if (string.IsNullOrEmpty(address1)) insufficientFields.Add("address1");
                if (string.IsNullOrEmpty(city)) insufficientFields.Add("city");
                if (string.IsNullOrEmpty(state)) insufficientFields.Add("state");
                if (string.IsNullOrEmpty(zip)) insufficientFields.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ip");
                //if (string.IsNullOrEmpty(internalID)) insufficientFields.Add("internalID");
                if (string.IsNullOrEmpty(creditCard)) insufficientFields.Add("creditCard");
                //if (string.IsNullOrEmpty(cvv)) insufficientFields.Add("cvv");

                if (insufficientFields.Count > 0)
                {
                    res.ErrorMessage = string.Format("Insufficient data, the following fields are required: {0}.", string.Join(", ", insufficientFields.ToArray()));
                }
                else if (expMonth < 1 || expMonth > 12 || expYear < 2000 || expYear > 2100)
                {
                    res.ErrorMessage = string.Format("Invalid expire date.");
                }
                else if (paymentTypeID != null &&
                    paymentTypeID != PaymentTypeEnum.Visa &&
                    paymentTypeID != PaymentTypeEnum.Mastercard &&
                    paymentTypeID != PaymentTypeEnum.AmericanExpress &&
                    paymentTypeID != PaymentTypeEnum.Discover)
                {
                    res.ErrorMessage = string.Format("Invalid payment type.");
                }
                else if (!CreditCard.ValidateDecryptedFormat(creditCard))
                {
                    res.ErrorMessage = string.Format("Invalid Credit Card.");
                }
                //else if (!string.IsNullOrEmpty(description) && description.Length > 1000)
                //{
                //    res.ErrorMessage = string.Format("Field description exceeds max length 1000 characters.");
                //}
                else
                {
                    //Check product
                    Product p = Load<Product>(productID);
                    if (p == null)
                    {
                        res.ErrorMessage = "ProductType(" + productID.ToString() + ") is not installed.";
                    }
                    else
                    {
                        //Check ProductCode List
                        bool productCodeListValid = true;
                        string productCodeListError = "Invalid Product List.";
                        IList<KeyValuePair<ProductCode, decimal>> validProductCodeList = new List<KeyValuePair<ProductCode, decimal>>();
                        if (productCodeList == null || productCodeList.Count == 0)
                        {
                            productCodeListValid = false;
                            productCodeListError += " Product List can not be empty.";
                        }
                        else
                        {
                            IList<int> productsWithInvalidAmount = new List<int>();
                            IList<int> productsWithInvalidProductID = new List<int>();
                            foreach (var item in productCodeList)
                            {
                                if (item.Value <= 0M)
                                {
                                    productsWithInvalidAmount.Add(item.Key);
                                }

                                ProductCode productCode = dao.Load<ProductCode>(item.Key);
                                if (productCode == null)
                                {
                                    productsWithInvalidProductID.Add(item.Key);
                                }
                                else
                                {
                                    validProductCodeList.Add(new KeyValuePair<ProductCode, decimal>(productCode, item.Value));
                                }
                            }
                            if (productsWithInvalidAmount.Count > 0)
                            {
                                productCodeListValid = false;
                                productCodeListError += " Products(" + string.Join(",", productsWithInvalidAmount.Select(i => i.ToString()).ToArray()) + ") have invalid Amount.";
                            }
                            if (productsWithInvalidProductID.Count > 0)
                            {
                                productCodeListValid = false;
                                productCodeListError += " Products(" + string.Join(",", productsWithInvalidProductID.Select(i => i.ToString()).ToArray()) + ") are not installed.";
                            }
                        }
                        if (!productCodeListValid)
                        {
                            res.ErrorMessage = productCodeListError;
                        }
                        else
                        {
                            if (campaignID != null)
                            {
                                Campaign cmp = Load<Campaign>(campaignID);
                                if (cmp == null)
                                {
                                    campaignID = null;
                                }
                            }

                            BusinessError<Set<Registration, Billing>> billing = billingService.CreateOrUpdateBillingByProspectOrInternalID(registrationID, internalID,
                                firstName, lastName, address1, address2, city, state, zip, country,
                                phone, email, campaignID, affiliate, subAffiliate, ip, null,
                                paymentTypeID, creditCard, cvv, expMonth, expYear,
                                customField1, customField2, customField3, customField4, customField5);

                            if (billing.State == BusinessErrorState.Error)
                            {
                                res.ErrorMessage = billing.ErrorMessage;
                            }
                            else
                            {
                                try
                                {
                                    if (registrationID == null && billingService.GetLastBillingByInternalID(internalID) == null)
                                    {
                                        //was created new Registration

                                        if (billing.ReturnValue.Value1 != null)
                                        {
                                            new EventService().RegistrationAndConfirmation(null, productID, email, zip, phone, firstName, lastName, billing.ReturnValue.Value1.RegistrationID, EventTypeEnum.Registration);
                                        }
                                    }
                                }
                                catch
                                { }

                                // get or create referer
                                try
                                {
                                    Referer ownReferer = null;
                                    BusinessError<Referer> createRefererResult = refererService.CreateOrGetRefererFromBilling(billing.ReturnValue.Value2, null, null);
                                    if (createRefererResult != null && createRefererResult.State == BusinessErrorState.Success)
                                    {
                                        ownReferer = createRefererResult.ReturnValue;
                                    }

                                    if (ownReferer != null)
                                    {
                                        refererService.AddBillingToReferer(ownReferer, billing.ReturnValue.Value2);
                                    }
                                }
                                catch
                                { }

                                res = BillAsUpsell(billing.ReturnValue.Value2, productID, validProductCodeList);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryEx> DoRefund(long chargeHistoryID, decimal refundAmount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                if (refundAmount <= 0M)
                {
                    res.ErrorMessage = "Invalid refund amount";
                }
                else
                {
                    ChargeHistoryEx ch = dao.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch == null)
                    {
                        res.ErrorMessage = string.Format("Transaction #{0} was not found", chargeHistoryID);
                    }
                    else
                    {
                        if (ch.BillingSubscriptionID == null)
                        {
                            OrderService orders = new OrderService();
                            Invoice inv = orders.GetInvoiceByChargeHistory(chargeHistoryID);

                            if (inv != null)
                            {
                                ChargeHistoryView view = orders.GetChargeHistoryView(chargeHistoryID);
                                BusinessError<ChargeHistoryView> res2 = new PaymentFlow().ProcessRefund(view, refundAmount, inv.InvoiceID);

                                res.ReturnValue = res2.ReturnValue.ChargeHistory;
                                res.State = res2.State;
                                res.ErrorMessage = res2.ErrorMessage;
                            }
                            else
                            {
                                res.ErrorMessage = "Invoice not found";
                            }
                        }
                        else
                            res = DoRefund(ch, refundAmount);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryEx> DoRefundSale(long saleID, decimal refundAmount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                if (refundAmount <= 0M)
                {
                    res.ErrorMessage = "Invalid refund amount";
                }
                else
                {
                    Sale sale = dao.Load<Sale>(saleID);
                    if (sale == null)
                    {
                        res.ErrorMessage = string.Format("Sale #{0} was not found", saleID);
                    }
                    else
                    {
                        ChargeHistoryEx ch = GetChargeHistoryBySale(saleID);
                        if (ch == null)
                        {
                            res.ErrorMessage = string.Format("Transaction was not found for Sale #{0}", saleID);
                        }
                        else
                        {
                            //MySqlCommand q = new MySqlCommand(
                            //    " select chs.* from ChargeHistoryExSale chs" +
                            //    " where chs.ChargeHistoryID = @chID and chs.SaleID = @saleID");
                            //q.Parameters.Add("@chID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;
                            //q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                            //ChargeHistoryExSale chSale = dao.Load<ChargeHistoryExSale>(q).FirstOrDefault();

                            res = DoRefund(ch, sale, refundAmount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryEx> DoVoid(long chargeHistoryID)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                ChargeHistoryEx ch = dao.Load<ChargeHistoryEx>(chargeHistoryID);
                if (ch == null)
                {
                    res.ErrorMessage = string.Format("Transaction #{0} was not found", chargeHistoryID);
                }
                else
                {
                    res = DoVoid(ch);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ChargeHistoryEx> GetChargeHistoryByInternalID(string internalID)
        {
            if (string.IsNullOrEmpty(internalID))
            {
                return new List<ChargeHistoryEx>();
            }

            IList<ChargeHistoryEx> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select ch.* from ChargeHistoryEx ch " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join BillingExternalInfo bei on bei.BillingID = b.BillingID " +
                    "where bei.InternalID = @internalID");
                q.Parameters.Add("@internalID", MySqlDbType.VarChar).Value = internalID;

                res = dao.Load<ChargeHistoryEx>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<ChargeHistoryEx> GetChargeHistoryByProductID(long productId)
        {
            IList<ChargeHistoryEx> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"select ch.*
                                                    from ChargeHistoryEx ch
                                                        inner join NMIMerchantAccountProduct a on ch.MerchantAccountID = a.AssertigyMIDID
                                                    where ch.Success = 1 
                                                        and ch.Amount > 0 
                                                        and a.ProductID = @ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productId;

                res = dao.Load<ChargeHistoryEx>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<ChargeHistoryEx> GetChargeHistoryByProspectID(long prospectID)
        {
            IList<ChargeHistoryEx> res = null;

            try
            {
                MySqlCommand cmd = new MySqlCommand("select ch.* from ChargeHistoryEx ch " +
                   "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                   "inner join Billing b on b.BillingID = bs.BillingID " +
                   "inner join Registration r on r.RegistrationID = b.RegistrationID " +
                   "where r.RegistrationID = @prospectID");
                cmd.Parameters.Add("@prospectID", MySqlDbType.Int64).Value = prospectID;

                res = dao.Load<ChargeHistoryEx>(cmd);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> GetChargeHistoryByID(long chargeHistoryID)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                ChargeHistoryEx ch = dao.Load<ChargeHistoryEx>(chargeHistoryID);
                if (ch == null)
                {
                    res.ErrorMessage = string.Format("Transaction #{0} was not found", chargeHistoryID);
                }
                else
                {
                    res.State = BusinessErrorState.Success;
                    res.ErrorMessage = string.Empty;
                    res.ReturnValue = ch;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public long? GetSaleIDByChargeHistoryID(long chargeHistoryID)
        {
            View<long> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select SaleID as Value from BillingSale where ChargeHistoryID=@ChargeHistoryID union 
                    select SaleID as Value from UpsellSale where ChargeHistoryID=@ChargeHistoryID union
                    select SaleID as Valye from SaleRefund where ChargeHistoryID=@ChargeHistoryID");
                q.Parameters.AddWithValue("@ChargeHistoryID", chargeHistoryID);

                res = dao.Load<View<long>>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            if (res == null)
                return null;

            return res.Value;
        }

        public bool IsGiftAvailable()
        {
            bool res = false;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select count(*) as Value from PromoGift" +
                    " where AssignedSaleID is null and PromoGiftTypeID = 1");
                res = dao.Load<View<int>>(q).Single().Value > 0;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<PromoGift> GetGiftCertificateBySale(long saleID)
        {
            IList<PromoGift> res = new List<PromoGift>();
            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select * from PromoGift" +
                    " where AssignedSaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                res = dao.Load<PromoGift>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public PromoGift GetGiftCertificateByNumber(string giftNumber)
        {
            if (string.IsNullOrEmpty(giftNumber))
            {
                return null;
            }

            PromoGift res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select * from PromoGift" +
                    " where GiftNumber = @giftNumber");
                q.Parameters.Add("@giftNumber", MySqlDbType.VarChar).Value = giftNumber;

                res = dao.Load<PromoGift>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<PromoGift> GetGiftCertificateByNumber(IList<string> giftNumberList)
        {
            IList<PromoGift> res = new List<PromoGift>();
            if (giftNumberList == null)
            {
                return res;
            }

            try
            {
                foreach (string item in giftNumberList)
                {
                    PromoGift promoGift = GetGiftCertificateByNumber(item);
                    if (promoGift != null)
                    {
                        res.Add(promoGift);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public SaleFullInfo GetSaleInfo(Sale sale)
        {
            SaleFullInfo res = null;
            try
            {
                MySqlCommand q = null;

                res = new SaleFullInfo();
                res.Charge = GetChargeHistoryBySale(sale.SaleID.Value);
                if (res.Charge != null)
                {
                    res.ChargeCurrency = dao.Load<ChargeHistoryExCurrency>(res.Charge.ChargeHistoryID);

                    res.ChargeMID = dao.Load<AssertigyMID>(res.Charge.MerchantAccountID);

                    q = new MySqlCommand(
                        " select chs.* from ChargeHistoryExSale chs" +
                        " where chs.ChargeHistoryID = @chID and chs.SaleID = @saleID");
                    q.Parameters.Add("@chID", MySqlDbType.Int64).Value = res.Charge.ChargeHistoryID;
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sale.SaleID;
                    res.ChargeSalePart = dao.Load<ChargeHistoryExSale>(q).FirstOrDefault();
                }

                res.Details = dao.Load<SaleDetails>(sale.SaleID);

                q = new MySqlCommand(
                    " select sch.* from SaleChargeDetails sch" +
                    " where sch.SaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sale.SaleID;
                res.ChargeDetails = dao.Load<SaleChargeDetails>(q);

                q = new MySqlCommand(
                    " select r.* from SaleReferer sr" +
                    " inner join Referer r on r.RefererID = sr.RefererID" +
                    " where sr.SaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sale.SaleID;
                res.Referer = dao.Load<Referer>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public UpsellType GetOrCreateUpsell(int? productID, string productCode, short? quantity, decimal? price)
        {
            UpsellType res = null;
            try
            {
                MySqlCommand q = null;
                q = new MySqlCommand("SELECT * FROM UpsellType WHERE ProductID=@ProductID AND ProductCode=@ProductCode AND Quantity=@Quantity AND Price=@Price");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = productCode;
                q.Parameters.Add("@Quantity", MySqlDbType.Int16).Value = quantity;
                q.Parameters.Add("@Price", MySqlDbType.Decimal).Value = price;
                res = dao.Load<UpsellType>(q).FirstOrDefault();
                if (res == null)
                {
                    res = new UpsellType()
                    {
                        Description = string.Empty,
                        DisplayName = string.Empty,
                        Price = price,
                        ProductCode = productCode,
                        ProductID = productID,
                        Quantity = quantity
                    };
                    dao.Save<UpsellType>(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void ProcessEmergencyQueue(long? billingID, int? midID)
        {
            MySqlCommand q = null;
            BillingSubscription billingSubscription = null;
            Billing billing = null;
            decimal? trialAmount = 0;
            decimal? upsellsAmount = 0;
            try
            {
                q = new MySqlCommand("Select * from EmergencyQueue where BillingID=@BillingID and Completed=0");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                var emergencyQueueList = dao.Load<EmergencyQueue>(q);
                if (emergencyQueueList == null || emergencyQueueList.Count == 0)
                    return;
                else
                {
                    billing = dao.Load<Billing>(billingID);
                    bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

                    //trials

                    //load billing sale
                    q = new MySqlCommand(@" select bsale.*, sale.* from BillingSale bsale
			                                            inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
			                                            inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                                                        inner join Sale sale on sale.SaleID=bsale.SaleID
			                                            where bs.StatusTID = 1 and bs.BillingID = @BillingID
			                                            and bsale.ChargeHistoryID = 0");

                    q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = billingID;
                    var billingSale = dao.Load<BillingSale>(q).FirstOrDefault();

                    if (billingSale != null)
                    {
                        var amount = emergencyQueueList.Select(u => u.Amount).Min();
                        if (amount > 0)
                        {
                            trialAmount = amount;
                            //load bs and billing
                            q = new MySqlCommand(@"select bs.* from BillingSubscription bs
			                               inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
			                               where bs.BillingSubscriptionID = @BillingSubscriptionID");

                            q.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = billingSale.BillingSubscriptionID;
                            billingSubscription = dao.Load<BillingSubscription>(q).FirstOrDefault();

                            if (isTestCase)
                            {
                                try
                                {
                                    dao.BeginTransaction();

                                    billingSubscription.StatusTID = 8;
                                    dao.Save(billingSubscription);

                                    q = new MySqlCommand("delete from BillingSale where SaleID = @SaleID");
                                    q.Parameters.Add("@SaleID", MySqlDbType.Int32).Value = billingSale.SaleID;
                                    dao.ExecuteNonQuery(q);

                                    dao.CommitTransaction();
                                }
                                catch (Exception ex)
                                {
                                    dao.RollbackTransaction();
                                    logger.Error(GetType(), ex);
                                    throw;
                                }
                            }
                            else
                            {
                                BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> res = null;
                                if (midID != null)
                                {
                                    var assertigyMID = dao.Load<AssertigyMID>(midID);
                                    res = CreateBillingSale_ForExistedSubscription_WithSpecifiedMID_NoEmergencyQueue_NoEmails(billing, billingSubscription, amount.Value, assertigyMID);
                                }
                                else
                                    res = CreateBillingSale_ForExistedSubscription_WithRandomMID_MoEmergencyQueue_NoEmails(billing, billingSubscription, amount.Value);

                                if (res == null || res.State == BusinessErrorState.Error)
                                {
                                    billingSubscription.StatusTID = 0;
                                    dao.Save<BillingSubscription>(billingSubscription);

                                    q = new MySqlCommand(@"select u.* from Upsell u
                    			                inner join Billing b on b.BillingID = u.BillingID
                    			                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                    			                left join UpsellSale us on us.UpsellID = u.UpsellID
                    			                where bs.StatusTID = 1 and u.BillingID = @BillingID
                    			                and us.SaleID is null");

                                    q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = billingID;
                                    var upsells = dao.Load<Upsell>(q).ToList();

                                    foreach (var upsell in upsells)
                                    {
                                        q = new MySqlCommand("Delete from Upsell where UpsellID=@UpsellID");
                                        q.Parameters.Add("@UpsellID", MySqlDbType.Int64).Value = upsell.UpsellID;
                                        dao.ExecuteNonQuery(q);
                                    }
                                }
                            }

                            q = new MySqlCommand("Delete from BillingSale where SaleID=@SaleID");
                            q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = billingSale.SaleID;
                            dao.ExecuteNonQuery(q);
                        }
                    }

                    //upsells
                    q = new MySqlCommand(@"select bs.* from Upsell u
                    			                inner join Billing b on b.BillingID = u.BillingID
                    			                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                    			                left join UpsellSale us on us.UpsellID = u.UpsellID
                    			                where bs.StatusTID = 1 and u.BillingID = @BillingID
                    			                and us.SaleID is null");

                    q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = billingID;

                    //load upsell
                    q = new MySqlCommand(@"select u.* from Upsell u
                    			                inner join Billing b on b.BillingID = u.BillingID
                    			                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                    			                left join UpsellSale us on us.UpsellID = u.UpsellID
                    			                where bs.StatusTID = 1 and u.BillingID = @BillingID
                    			                and us.SaleID is null");

                    q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = billingID;
                    var upsellList = dao.Load<Upsell>(q).ToList();
                    if (upsellList != null && upsellList.Count > 0)
                    {
                        var totalAmount = emergencyQueueList.Select(u => u.Amount).Sum();
                        if (totalAmount > 0 && totalAmount > trialAmount)
                        {
                            upsellsAmount = totalAmount - trialAmount;

                            IList<KeyValuePair<UpsellType, int>> upsellTypeList = new List<KeyValuePair<UpsellType, int>>();
                            foreach (var upsell in upsellList)
                            {
                                var upsellType = dao.Load<UpsellType>(upsell.UpsellTypeID);
                                upsellTypeList.Add(new KeyValuePair<UpsellType, int>(upsellType, upsell.Quantity ?? 0));
                            }

                            if (!isTestCase)
                            {
                                CreateUpsellSales_No_Emergency_Queue(billing, upsellTypeList, upsellsAmount, midID);
                            }

                            foreach (var upsell in upsellList)
                            {
                                q = new MySqlCommand("Delete from Upsell where UpsellID=@UpsellID");
                                q.Parameters.Add("@UpsellID", MySqlDbType.Int64).Value = upsell.UpsellID;
                                dao.ExecuteNonQuery(q);
                            }
                        }
                    }

                    foreach (var emergencyQueueItem in emergencyQueueList)
                    {
                        emergencyQueueItem.Completed = true;
                        dao.Save<EmergencyQueue>(emergencyQueueItem);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public BusinessError<ChargeHistoryEx> ProcessEmergencyQueue_Old(int? emergencyQueueID)
        {
            return ProcessEmergencyQueue_Old(emergencyQueueID, null);
        }

        public BusinessError<ChargeHistoryEx> ProcessEmergencyQueue_Old(int? emergencyQueueID, int? midID)
        {
            BusinessError<ChargeHistoryEx> res = null;
            MySqlCommand q = null;
            try
            {
                var emergencyQueueItem = dao.Load<EmergencyQueue>(emergencyQueueID);
                if (emergencyQueueItem == null)
                    res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "No ProcessEmergency Found for that ID");
                else
                {
                    if (emergencyQueueItem.Amount != new Decimal(9.98) && emergencyQueueItem.Amount < 29)
                    {
                        //trial

                        //load billing sale
                        q = new MySqlCommand(@" select bsale.*, sale.* from BillingSale bsale
			                                            inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
			                                            inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                                                        inner join Sale sale on sale.SaleID=bsale.SaleID
			                                            where bs.StatusTID = 1 and bs.BillingID = @BillingID
			                                            and bsale.ChargeHistoryID = 0");

                        q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = emergencyQueueItem.BillingID;
                        var billingSale = dao.Load<BillingSale>(q).FirstOrDefault();

                        if (billingSale != null)
                        {
                            //load bs
                            q = new MySqlCommand(@" select bs.* from BillingSubscription bs
			                               inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
			                               where bs.BillingSubscriptionID = @BillingSubscriptionID");

                            q.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = billingSale.BillingSubscriptionID;
                            var billingSubscription = dao.Load<BillingSubscription>(q).FirstOrDefault();

                            if (billingSubscription != null && billingSale != null)
                            {
                                var billing = dao.Load<Billing>(emergencyQueueItem.BillingID);
                                var subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);

                                if (billing.CreditCardCnt.DecryptedCreditCard == "4111111111111111")
                                {
                                    //test account
                                    try
                                    {
                                        dao.BeginTransaction();

                                        billingSubscription.StatusTID = 8;
                                        dao.Save(billingSubscription);

                                        q = new MySqlCommand("delete from BillingSale where SaleID = @SaleID");
                                        q.Parameters.Add("@SaleID", MySqlDbType.Int32).Value = billingSale.SaleID;
                                        dao.ExecuteNonQuery(q);

                                        dao.CommitTransaction();
                                    }
                                    catch (Exception ex)
                                    {
                                        dao.RollbackTransaction();
                                        logger.Error(GetType(), ex);
                                        throw;
                                    }
                                }
                                else
                                {
                                    res = ProcessEmergencyQueueTrialSale(billingSale, billingSubscription, billing, subscription, emergencyQueueItem.Amount.Value, midID);
                                    if (res != null && res.State == BusinessErrorState.Error)
                                    {
                                        try
                                        {
                                            dao.BeginTransaction();

                                            billingSubscription.StatusTID = 0;
                                            dao.Save<BillingSubscription>(billingSubscription);

                                            q = new MySqlCommand(@"delete from BillingSale where SaleID = @SaleID");
                                            q.Parameters.Add("@SaleID", MySqlDbType.Int32).Value = billingSale.SaleID;
                                            dao.ExecuteNonQuery(q);

                                            q = new MySqlCommand(@"select u.* from Upsell u
						                                 left join UpsellSale us on us.UpsellID = u.UpsellID
						                                 where u.BillingID = @BillingID
						                                 and us.SaleID is null");
                                            q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = emergencyQueueItem.BillingID;

                                            var upsellLst = dao.Load<Upsell>(q);
                                            foreach (var u in upsellLst)
                                            {
                                                q = new MySqlCommand("delete from Upsell where UpsellID = @UpsellID");
                                                q.Parameters.Add("@UpsellID", MySqlDbType.Int32).Value = u.UpsellID;
                                                dao.ExecuteNonQuery(q);
                                            }

                                            dao.CommitTransaction();
                                        }
                                        catch (Exception ex)
                                        {
                                            dao.RollbackTransaction();
                                            logger.Error(GetType(), ex);
                                            res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, ex.Message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //upsell

                        //load bs
                        q = new MySqlCommand(@"select bs.* from Upsell u
			                                inner join Billing b on b.BillingID = u.BillingID
			                                inner join BillingSubscription bs on bs.BillingID = b.BillingID
			                                left join UpsellSale us on us.UpsellID = u.UpsellID
			                                where bs.StatusTID = 1 and u.BillingID = @BillingID
			                                and us.SaleID is null");

                        q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = emergencyQueueItem.BillingID;
                        var billingSubscription = dao.Load<BillingSubscription>(q).FirstOrDefault();

                        //load upsell
                        q = new MySqlCommand(@"select u.* from Upsell u
			                                inner join Billing b on b.BillingID = u.BillingID
			                                inner join BillingSubscription bs on bs.BillingID = b.BillingID
			                                left join UpsellSale us on us.UpsellID = u.UpsellID
			                                where bs.StatusTID = 1 and u.BillingID = @BillingID
			                                and us.SaleID is null");

                        q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = emergencyQueueItem.BillingID;
                        var upsell = dao.Load<Upsell>(q).FirstOrDefault();

                        if (upsell != null && billingSubscription != null)
                        {
                            var billing = dao.Load<Billing>(emergencyQueueItem.BillingID);
                            if (billing.CreditCardCnt.DecryptedCreditCard == "4111111111111111")
                            {
                                //test account

                                q = new MySqlCommand("delete from Upsell where UpsellID = @UpsellID");
                                q.Parameters.Add("@UpsellID", MySqlDbType.Int32).Value = upsell.UpsellID;
                                dao.ExecuteNonQuery(q);
                            }
                            else
                            {
                                var upsellType = dao.Load<UpsellType>(upsell.UpsellTypeID);
                                res = ProcessEmergencyQueueUpsellSale(upsell, billing, upsellType, 1, midID);
                            }
                        }
                    }
                }
                //if (res != null && res.State == BusinessErrorState.Success)
                {
                    emergencyQueueItem.Completed = true;
                    dao.Save<EmergencyQueue>(emergencyQueueItem);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, ex.Message);
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> ProcessEmergencyQueueTrialSale(BillingSale sale, BillingSubscription existedBillingSubscription, Billing billing, Subscription subscription, decimal amount, int? midID)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            AssertigyMID assertigyMid = null;
            NMICompany nmiCompany = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));
            bool isTestAffiliateCase = (billing.Affiliate == TEST_AFFILIATE_CODE);

            try
            {
                dao.BeginTransaction();
                Set<NMICompany, AssertigyMID> mid = null;

                if (midID != null)
                {
                    //use custom midID
                    nmiCompany = merchantService.GetNMICompanyByAssertigyMID(midID);
                    assertigyMid = Load<AssertigyMID>(midID);

                    if (nmiCompany != null && assertigyMid != null)
                        mid = new Set<NMICompany, AssertigyMID>()
                        {
                            Value1 = merchantService.GetNMICompanyByAssertigyMID(midID),
                            Value2 = Load<AssertigyMID>(midID)
                        };
                }
                else
                {
                    if (existedBillingSubscription != null)
                    {
                        mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value);
                    }
                    if (mid == null)
                    {
                        mid = merchantService.ChooseRandomNMIMerchantAccount(subscription.ProductID.Value, billing, amount);
                    }
                }
                if (mid != null)
                {
                    nmiCompany = mid.Value1;
                    assertigyMid = mid.Value2;

                    Currency cur = GetCurrencyByProduct(subscription.ProductID.Value);

                    Product product = EnsureLoad<Product>(subscription.ProductID);

                    IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                    BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                        nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                        sale.SaleID.Value, billing, product);

                    if (paymentResult.State == BusinessErrorState.Error)
                    {
                        sale = null;
                        existedBillingSubscription = null;

                        res.State = BusinessErrorState.Error;
                        res.ErrorMessage = paymentResult.ErrorMessage;
                    }

                    Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, existedBillingSubscription,
                        subscription.ProductCode, SaleTypeEnum.Billing, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                    if (sale != null)
                    {
                        sale.PaygeaID = chargeLog.Value1.PaygeaID;
                        sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                        dao.Save<BillingSale>(sale);
                    }
                    res.ReturnValue = chargeLog.Value2;
                }
                else
                {
                    res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "No MID Found");
                    //EmergencyQueue emergencyQueue = new EmergencyQueue();
                    //emergencyQueue.BillingID = (int?)billing.BillingID;
                    //emergencyQueue.Amount = amount;
                    //emergencyQueue.CreateDT = DateTime.Now;
                    //emergencyQueue.Completed = false;
                    //dao.Save<EmergencyQueue>(emergencyQueue);
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

        public BusinessError<ChargeHistoryEx> ProcessEmergencyQueueUpsellSale(Upsell upsell, Billing billing, UpsellType upsellType, int quantity, int? midID)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>();
            res.State = BusinessErrorState.Success;

            string chargeBlockReason = null;
            if (new BillingService().IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            UpsellSale sale = null;
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!CHARGE) || IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;
            NMICompany nmiCompany = null;

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);

                //If new Sale(without BillingSubscription) create Upsell Fake BillingSubscription
                billingSubscription = existedBillingSubscription;
                if (billingSubscription == null)
                {
                    billingSubscription = CreateUpsellFakeBillingSubscriptionByProduct(billing, upsellType.ProductID.Value);
                    SaveBillingSubscriptionDiscount(billingSubscription, false, null);
                }

                if (!isTestCase)
                {
                    decimal amount = CalculateUpsellSaleAmount(upsellType, quantity, null, null);

                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;

                    if (midID != null)
                    {
                        //use custom midID
                        nmiCompany = merchantService.GetNMICompanyByAssertigyMID(midID);
                        assertigyMid = Load<AssertigyMID>(midID);

                        if (nmiCompany != null && assertigyMid != null)
                            mid = new Set<NMICompany, AssertigyMID>()
                            {
                                Value1 = merchantService.GetNMICompanyByAssertigyMID(midID),
                                Value2 = Load<AssertigyMID>(midID)
                            };
                    }
                    else
                    {
                        if (existedBillingSubscription != null)
                        {
                            mid = merchantService.GetMerchantOfLastSuccessfulCharge(existedBillingSubscription.BillingID.Value);
                        }

                        if (mid == null)
                        {
                            mid = merchantService.ChooseRandomNMIMerchantAccount(upsellType.ProductID.Value, billing, amount);
                        }
                    }
                    if (mid != null)
                    {
                        sale = new UpsellSale();
                        sale.SaleTypeID = SaleTypeEnum.Upsell;
                        sale.TrackingNumber = null;
                        sale.CreateDT = DateTime.Now;
                        sale.NotShip = false;
                        sale.UpsellID = upsell.UpsellID;
                        sale.ChargeHistoryID = null;
                        sale.PaygeaID = null;
                        dao.Save<UpsellSale>(sale);

                        nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = null;
                        Subscription subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
                        if (subscription != null && subscription.ProductID != null)
                        {
                            cur = GetCurrencyByProduct(subscription.ProductID.Value);
                        }

                        Product product = EnsureLoad<Product>(subscription.ProductID);

                        IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            sale.SaleID.Value, billing, product);

                        if (paymentResult.State == BusinessErrorState.Error)
                        {
                            dao.RollbackTransaction();

                            upsell = null;
                            sale = null;
                            billingSubscription = existedBillingSubscription;

                            res.State = BusinessErrorState.Error;
                            res.ErrorMessage = paymentResult.ErrorMessage;

                            dao.BeginTransaction();
                        }

                        Set<Paygea, ChargeHistoryEx> chargeLog = ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellType.ProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                        if (sale != null)
                        {
                            sale.PaygeaID = chargeLog.Value1.PaygeaID;
                            sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                            dao.Save<UpsellSale>(sale);

                            //emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);
                        }
                        else if (existedBillingSubscription != null)
                        {
                            //If Upsell failed for existed BillingSubscription
                            if (paymentResult.ReturnValue.Response.ToLower().Contains("risk decline"))
                            {
                                DeclineUpsell declineUpsell = new DeclineUpsell();
                                declineUpsell.BillingID = billing.BillingID;
                                declineUpsell.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<DeclineUpsell>(declineUpsell);
                            }
                        }

                        res.ReturnValue = chargeLog.Value2;
                    }
                    else
                    {
                        res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "No MID Found");
                        //EmergencyQueue emergencyQueue = new EmergencyQueue();
                        //emergencyQueue.BillingID = (int?)billing.BillingID;
                        //emergencyQueue.Amount = amount;
                        //emergencyQueue.CreateDT = DateTime.Now;
                        //emergencyQueue.Completed = false;
                        //dao.Save<EmergencyQueue>(emergencyQueue);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }

            if (res != null && res.State == BusinessErrorState.Success)
            {
                ValidateCustomer(billing);
                ValidateFraud(billing, sale);
            }

            return res;
        }

        public void ProcessRebill(int? billingSubscriptionID)
        {
            try
            {
                var billingSubscription = dao.Load<BillingSubscription>(billingSubscriptionID);
                var billing = dao.Load<Billing>(billingSubscription.BillingID);
                var bRebill = IsRebill(billingSubscriptionID);
                string resString = string.Empty;
                using (WebClient client = new WebClient())
                {
                    //for testing
                    //var url = "http://trimfuel.localhost/jobs/rebill_nmi.asp?bsid=" + billingSubscriptionID.ToString();
                    var url = "http://" + Config.Current.APPLICATION_ID + "/jobs/rebill_nmi.asp?bsid=" + billingSubscriptionID.ToString();
                    resString = client.DownloadString(url);
                }
                if (!resString.Contains("Cap hit"))
                {
                    var resArray = resString.Split('|');
                    if (!resString.Contains("Billing ID does not exist"))
                    {
                        if (bRebill)
                        {
                            if (resString.Contains("Success"))
                            {
                                //Success
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, true, true, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                            else
                            {
                                //Declined
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, false, true, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                        }
                        else
                        {
                            if (resString.Contains("Success"))
                            {
                                //Success
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, true, false, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                            else
                            {
                                //Declined
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, false, false, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void ProcessRebill(int? billingSubscriptionID, int? midID)
        {
            try
            {
                var billingSubscription = dao.Load<BillingSubscription>(billingSubscriptionID);
                var billing = dao.Load<Billing>(billingSubscription.BillingID);
                var bRebill = IsRebill(billingSubscriptionID);
                string resString = string.Empty;
                using (WebClient client = new WebClient())
                {
                    //for testing
                    //var url = string.Format("http://{0}/jobs/custom_rebill_nmi.asp?bsid={1}&midid={2}", "trimfuel.localhost", billingSubscriptionID, midID);
                    var url = string.Format("http://{0}/jobs/custom_rebill_nmi.asp?bsid={1}&midid={2}", Config.Current.APPLICATION_ID, billingSubscriptionID, midID);
                    resString = client.DownloadString(url);
                }
                if (!resString.Contains("Cap hit"))
                {
                    var resArray = resString.Split('|');
                    if (!resString.Contains("Billing ID does not exist"))
                    {
                        if (bRebill)
                        {
                            if (resString.Contains("Success"))
                            {
                                //Success
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, true, true, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                            else
                            {
                                //Declined
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, false, true, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                        }
                        else
                        {
                            if (resString.Contains("Success"))
                            {
                                //Success
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, true, false, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                            else
                            {
                                //Declined
                                UpdateDBAfterRebill(billingSubscription.SubscriptionID, false, false, Utility.TryGetDecimal(resArray[1]), billing.Affiliate, resArray[3]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public bool IsRebill(int? billingSubscriptionID)
        {
            bool res = false;
            try
            {
                MySqlCommand q = new MySqlCommand("select datediff(LastBillDate, NextBillDate) as dd from BillingSubscription where BillingSubscriptionID=@BillingSubscriptionID");
                q.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = billingSubscriptionID;
                var dateDiff = dao.ExecuteScalar<int>(q);
                if (dateDiff != null && dateDiff != 7)
                    res = true;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }

        public void UpdateDBAfterRebill(int? subscriptionID, bool bSuccess, bool bRebill, decimal? amount, string affiliate, string childMID)
        {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            try
            {
                dao.BeginTransaction();
                MySqlCommand q = new MySqlCommand("Select * From DailyBillingResults Where MID=@MID and ChargeDT=@ChargeDT and SubscriptionID=@SubscriptionID and Affiliate=@Affiliate");
                q.Parameters.Add("@MID", MySqlDbType.VarChar).Value = childMID;
                q.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = affiliate;
                q.Parameters.Add("@ChargeDT", MySqlDbType.DateTime).Value = dt;
                q.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = subscriptionID;
                var dailyBillingResult = dao.Load<DailyBillingResults>(q).SingleOrDefault();

                if (dailyBillingResult == null)
                {
                    dailyBillingResult = new DailyBillingResults();
                    dailyBillingResult.Affiliate = affiliate;
                    dailyBillingResult.Amount = amount;
                    dailyBillingResult.ChargeDT = dt;
                    dailyBillingResult.MID = childMID;
                    dailyBillingResult.SubscriptionID = subscriptionID;
                    dailyBillingResult.ReattemptFailCount = 0;
                    dailyBillingResult.ReattemptSuccessCount = 0;
                    dailyBillingResult.RebillFailCount = 0;
                    dailyBillingResult.RebillSuccessCount = 0;

                    if (bSuccess)
                    {
                        if (bRebill)
                            dailyBillingResult.RebillSuccessCount = 1;
                        else
                            dailyBillingResult.ReattemptSuccessCount = 1;
                    }
                    else
                    {
                        if (bRebill)
                            dailyBillingResult.RebillFailCount = 1;
                        else
                            dailyBillingResult.ReattemptFailCount = 1;
                    }
                }
                else
                {
                    if (bSuccess)
                    {
                        dailyBillingResult.Amount += amount;

                        if (bRebill)
                            dailyBillingResult.RebillSuccessCount++;
                        else
                            dailyBillingResult.ReattemptSuccessCount++;
                    }
                    else
                    {
                        if (bRebill)
                            dailyBillingResult.RebillFailCount++;
                        else
                            dailyBillingResult.ReattemptFailCount++;
                    }
                }

                dao.Save<DailyBillingResults>(dailyBillingResult);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public IList<ChargeHistoryView> GetSaleRefunds(long saleID)
        {
            IList<ChargeHistoryView> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join SaleRefund sr on sr.ChargeHistoryID = ch.ChargeHistoryID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where sr.SaleID = @saleID and ch.Success = 1
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                res = dao.Load<ChargeHistoryView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public SaleChargeback GetSaleChargeback(long saleID)
        {
            SaleChargeback res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from SaleChargeback
                    where SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                res = dao.Load<SaleChargeback>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public IList<Sale> GetSaleListByBillingID(long? billingID)
        {
            IList<Sale> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select sale.* from BillingSale bsale
			        inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
                    inner join Sale sale on sale.SaleID=bsale.SaleID
			        where bs.BillingID = @BillingID
                    union all
                    select sale.* from Upsell u
					inner join UpsellSale us on us.UpsellID = u.UpsellID
                    inner join Sale sale on us.SaleID=sale.SaleID
					where u.BillingID = @BillingID
                    union all
                    select sale.* from ExtraTrialShipSale ex                    
                    inner join Sale sale on ex.SaleID=sale.SaleID
					where ex.BillingID = @BillingID
                    union all
                    select sale.* from Orders o
					inner join OrderSale os on os.OrderID = o.OrderID
                    inner join Sale sale on os.SaleID=sale.SaleID
					where o.BillingID = @BillingID
                ");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<Sale>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public IList<SaleRefund> GetRefundListByBillingID(long? billingID)
        {
            IList<SaleRefund> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select refund.* from BillingSale bsale
			        inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
                    inner join Sale sale on sale.SaleID=bsale.SaleID
                    inner join SaleRefund refund on refund.SaleID=sale.SaleID
			        where bs.BillingID = @BillingID
                    union all
                    select refund.* from Upsell u
					inner join UpsellSale us on us.UpsellID = u.UpsellID
                    inner join Sale sale on us.SaleID=sale.SaleID
                    inner join SaleRefund refund on refund.SaleID=sale.SaleID
					where u.BillingID = @BillingID
                    union all
                    select refund.* from ExtraTrialShipSale ex                    
                    inner join Sale sale on ex.SaleID=sale.SaleID
                    inner join SaleRefund refund on refund.SaleID=sale.SaleID
					where ex.BillingID = @BillingID
                    union all
                    select refund.* from Orders o
					inner join OrderSale os on os.OrderID = o.OrderID
                    inner join Sale sale on os.SaleID=sale.SaleID
                    inner join SaleRefund refund on refund.SaleID=sale.SaleID
					where o.BillingID = @BillingID
                ");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<SaleRefund>(q);
            }
            catch (Exception ex)
            {
                res = new List<SaleRefund>();
                logger.Error(ex);
            }

            return res;
        }

        public IList<SaleChargeback> GetChargebackListByBillingID(long? billingID)
        {
            IList<SaleChargeback> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select sc.* from BillingSale bsale
			        inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
                    inner join Sale sale on sale.SaleID=bsale.SaleID
                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
			        where bs.BillingID = @BillingID
                    union all
                    select sc.* from Upsell u
					inner join UpsellSale us on us.UpsellID = u.UpsellID
                    inner join Sale sale on us.SaleID=sale.SaleID
                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
					where u.BillingID = @BillingID
                    union all
                    select sc.* from ExtraTrialShipSale ex                    
                    inner join Sale sale on ex.SaleID=sale.SaleID
                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
					where ex.BillingID = @BillingID
                    union all
                    select sc.* from Orders o
					inner join OrderSale os on os.OrderID = o.OrderID
                    inner join Sale sale on os.SaleID=sale.SaleID
                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
					where o.BillingID = @BillingID
                ");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<SaleChargeback>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public bool BlockShipment(long? saleID)
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();
                var sale = dao.Load<Sale>(saleID);
                if (sale != null)
                {
                    sale.NotShip = true;
                    dao.Save<Sale>(sale);
                }

                MySqlCommand q = new MySqlCommand(@"Delete from AggUnsentShipments where SaleID = @SaleID");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                res = false;
                logger.Error(ex);
            }

            return res;
        }

        public bool ChangeSKU(long saleID, string newSKU)
        {
            bool res = true;
            try
            {
                Sale sale = dao.Load<Sale>(saleID);

                if (sale == null)
                    throw new Exception(string.Format("Can't load Sale({0})", saleID));

                if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Billing)
                {
                    BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                    bsale.ProductCode = newSKU;
                    dao.Save<BillingSale>(bsale);
                }
                else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Rebill)
                {
                    BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                    bsale.ProductCode = newSKU;
                    dao.Save<BillingSale>(bsale);
                }
                else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Upsell)
                {
                    UpsellSale us = EnsureLoad<UpsellSale>(saleID);
                    Upsell u = EnsureLoad<Upsell>(us.UpsellID);
                    u.ProductCode = newSKU;
                    dao.Save<Upsell>(u);
                }
                else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip)
                {
                    ExtraTrialShipSale es = EnsureLoad<ExtraTrialShipSale>(saleID);
                    ExtraTrialShip e = EnsureLoad<ExtraTrialShip>(es.ExtraTrialShipID);
                    e.ProductCode = newSKU;
                    dao.Save<ExtraTrialShip>(e);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }

        public string GetSKUBySaleID(long saleID)
        {
            string res = string.Empty;
            try
            {
                Sale sale = dao.Load<Sale>(saleID);
                BillingSubscription bs = GetBillingSubscriptionBySale(saleID);

                if (sale == null)
                    throw new Exception(string.Format("Can't load Sale({0})", saleID));
                if (bs == null)
                    throw new Exception(string.Format("Can't determine BillingSubscription for Sale({0})", saleID));

                if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Billing)
                {
                    BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                    Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                    res = bsale.ProductCode ?? s.ProductCode;
                }
                else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Rebill)
                {
                    BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                    Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                    res = bsale.ProductCode ?? s.SKU2;
                }
                else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Upsell)
                {
                    UpsellSale us = EnsureLoad<UpsellSale>(saleID);
                    Upsell u = EnsureLoad<Upsell>(us.UpsellID);
                    res = u.ProductCode;
                }
                else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip)
                {
                    ExtraTrialShipSale es = EnsureLoad<ExtraTrialShipSale>(saleID);
                    ExtraTrialShip e = EnsureLoad<ExtraTrialShip>(es.ExtraTrialShipID);
                    res = e.ProductCode;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = string.Empty;
            }
            return res;
        }

        public void AddRebillToIgnoreUnbilledTransaction(int? billingSubscriptionID)
        {
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from IgnoreUnbilledTransaction where BillingSubscriptionID=@BillingSubscriptionID");
                q.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = billingSubscriptionID;
                var ignoreItem = dao.Load<IgnoreUnbilledTransaction>(q).SingleOrDefault();
                if (ignoreItem == null)
                {
                    ignoreItem = new IgnoreUnbilledTransaction();
                    ignoreItem.BillingID = null;
                    ignoreItem.BillingSubscriptionID = billingSubscriptionID;
                    dao.Save<IgnoreUnbilledTransaction>(ignoreItem);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void AddQueuedSaleToIgnoreUnbilledTransaction(long? billingID)
        {
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from IgnoreUnbilledTransaction where BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = billingID;
                var ignoreItem = dao.Load<IgnoreUnbilledTransaction>(q).SingleOrDefault();
                if (ignoreItem == null)
                {
                    ignoreItem = new IgnoreUnbilledTransaction();
                    ignoreItem.BillingSubscriptionID = null;
                    ignoreItem.BillingID = billingID;
                    dao.Save<IgnoreUnbilledTransaction>(ignoreItem);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public BusinessError<GatewayResult> PerformTxn(long billingID, int productID, decimal amount, int assertigyMIDID, long saleID)
        {
            BusinessError<GatewayResult> res = null;
            try
            {
                var billing = dao.Load<Billing>(billingID);
                if (billing == null)
                    throw new Exception("Can't find Billing by BillingID");

                AssertigyMID assertigyMid = dao.Load<AssertigyMID>(assertigyMIDID);
                if (assertigyMid == null)
                    throw new Exception("Can't find MID by MIDID");

                NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMIDID);
                Currency cur = GetCurrencyByProduct(productID);
                Product product = EnsureLoad<Product>(productID);
                IPaymentGateway paymentGateway = GetGatewayByMID(assertigyMid);

                BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                    nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                    saleID, billing, product);

                res = paymentResult;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.State = BusinessErrorState.Error;
                res.ReturnValue = new GatewayResult()
                {
                    Request = "billingID=" + billingID.ToString(),
                    Response = ex.Message
                };
            }

            return res;
        }
    }
}
