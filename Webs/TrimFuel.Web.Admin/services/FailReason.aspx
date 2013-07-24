<%@ Page Language="C#" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Text" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>
<%@ Import Namespace="System.Web.Services" %>
<%@ Import Namespace="MySql.Data.MySqlClient" %>
<%@ Import Namespace="TrimFuel.Business" %>
<script runat="server">

    [WebMethod]
    public static JSON Reasons(string startDT, string endDT, int groupMode, string affiliate, string subAffiliate, string mid, int statusTID, 
        int productID, int salesType, int excludeReattempts, string paymentTypeID)
    {
        JSON json = new JSON();

        json.AvsRejected = 0;
        json.CallVoiceCenter = 0;
        json.CurrencyNotAvailable = 0;
        json.CvvFailure = 0;
        json.DeclinedByIssuer = 0;
        json.DeclinedByRiskManagement = 0;
        json.DuplicateTransaction = 0;
        json.ExpiredCard = 0;
        json.GatewayError = 0;
        json.InsufficientFunds = 0;
        json.InvalidCardNumber = 0;
        json.InvalidExpirationDate = 0;
        json.InvalidRefundAmount = 0;
        json.LostOrStolenCard = 0;
        json.TransactionAlreadyVoided = 0;

        string statusFilter = string.Empty;
        string productFilter = string.Empty;
        string affiliateFilter = string.Empty;
        string paymentTypeFilter = string.Empty;
        string midFilter = string.Empty;

        switch (groupMode)
        {
            case 1:
                affiliateFilter = string.Format("and IfNull(b.Affiliate, '') = '{0}'", !string.IsNullOrEmpty(affiliate) ? affiliate : "");

                if (!string.IsNullOrEmpty(mid))
                    midFilter = string.Format("and ch.MerchantAccountID = '{0}'", mid);

                if (!string.IsNullOrEmpty(paymentTypeID))
                    paymentTypeFilter = string.Format("and b.PaymentTypeID = {0}", paymentTypeID);

                break;
            case 2:
                affiliateFilter = string.Format("and IfNull(b.Affiliate, '') = '{0}'", !string.IsNullOrEmpty(affiliate) ? affiliate : "");
                affiliateFilter += string.Format(" and IfNull(b.SubAffiliate, '') = '{0}'", !string.IsNullOrEmpty(subAffiliate) ? subAffiliate : "");

                if (!string.IsNullOrEmpty(mid))
                    midFilter = string.Format("and ch.MerchantAccountID = '{0}'", mid);

                if (!string.IsNullOrEmpty(paymentTypeID))
                    paymentTypeFilter = string.Format("and b.PaymentTypeID = {0}", paymentTypeID);

                break;
            case 3:
                midFilter = string.Format("and ch.MerchantAccountID = '{0}'", !string.IsNullOrEmpty(mid) ? mid : "");

                if (!string.IsNullOrEmpty(affiliate))
                    affiliateFilter = string.Format("and IfNull(b.Affiliate, '') = '{0}'", affiliate);

                if (!string.IsNullOrEmpty(paymentTypeID))
                    paymentTypeFilter = string.Format("and b.PaymentTypeID = {0}", paymentTypeID);

                break;
            case 4:
                paymentTypeFilter = string.Format("and b.PaymentTypeID = {0}", !string.IsNullOrEmpty(paymentTypeID) ? paymentTypeID : "");

                if (!string.IsNullOrEmpty(affiliate))
                    affiliateFilter = string.Format("and IfNull(b.Affiliate, '') = '{0}'", affiliate);

                if (!string.IsNullOrEmpty(mid))
                    midFilter = string.Format("and ch.MerchantAccountID = '{0}'", mid);

                break;
            default:
                break;
        }

        if (statusTID > -1)
            statusFilter = "and bs.StatusTID = " + statusTID;

        if (productID > -1)
            productFilter = "and ss.ProductID = " + productID;

        StringBuilder trialCmd = new StringBuilder();
        trialCmd.AppendLine("(");
        trialCmd.AppendLine("select Response, b.BillingID from FailedChargeHistory ch");
        trialCmd.AppendLine("inner join Billing b on b.BillingID = ch.BillingID");
        trialCmd.AppendLine("inner join Campaign cmp on cmp.CampaignID = b.CampaignID");
        trialCmd.AppendLine("inner join Subscription ss on ss.SubscriptionID = cmp.SubscriptionID");
        if (statusTID > -1)
            trialCmd.AppendLine("inner join BillingSubscription bs on bs.BillingID = b.BillingID");
        trialCmd.AppendLine("where ch.Success = 0 and ch.ChargeDate between '@startDT 00:00:00' and '@endDT 23:59:59' @statusFilter @productFilter @affiliateFilter @midFilter @paymentTypeFilter");
        trialCmd.AppendLine(")");

        StringBuilder rebillCmd = new StringBuilder();
        rebillCmd.AppendLine("(");
        rebillCmd.AppendLine("select Response, bs.BillingID from ChargeHistoryEx ch ");
        rebillCmd.AppendLine("inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID ");
        rebillCmd.AppendLine("inner join Billing b on b.BillingID = bs.BillingID ");
        rebillCmd.AppendLine("inner join Subscription ss on ss.SubscriptionID = bs.SubscriptionID ");
        rebillCmd.AppendLine("inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID ");
        if (excludeReattempts == 1)
        {
            rebillCmd.AppendLine("left join (select withPrev.ChargeHistoryID from ( ");
            rebillCmd.AppendLine(" select ch.ChargeHistoryID, max(ch1.ChargeHistoryID) as PrevChargeHistoryID ");
            rebillCmd.AppendLine(" from ChargeHistoryEx ch ");
            rebillCmd.AppendLine(" inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID ");
            rebillCmd.AppendLine(" inner join Billing b on b.BillingID = bs.BillingID ");
            rebillCmd.AppendLine(" inner join Subscription ss on ss.SubscriptionID = bs.SubscriptionID ");
            rebillCmd.AppendLine(" inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID ");
            rebillCmd.AppendLine(" inner join ChargeHistoryEx ch1 on ch1.BillingSubscriptionID = ch.BillingSubscriptionID ");
            rebillCmd.AppendLine(" inner join ChargeDetails cd1 on cd1.ChargeHistoryID = ch1.ChargeHistoryID ");
            rebillCmd.AppendLine(" where ch.Success = 0 and ch.Amount > 0 and cd.SaleTypeID = 2 ");
            rebillCmd.AppendLine("  and ch.ChargeDate between '@startDT 00:00:00' and '@endDT 23:59:59' @statusFilter @productFilter @affiliateFilter @midFilter @paymentTypeFilter");
            rebillCmd.AppendLine("  and ch1.ChargeHistoryID < ch.ChargeHistoryID and cd1.SaleTypeID = 2 ");
            rebillCmd.AppendLine(" group by ch.ChargeHistoryID ) withPrev ");
            rebillCmd.AppendLine(" inner join ChargeHistoryEx chPrev on chPrev.ChargeHistoryID = withPrev.PrevChargeHistoryID ");
            rebillCmd.AppendLine(" where chPrev.Success = 0) prev on prev.ChargeHistoryID = ch.ChargeHistoryID ");
        }
        rebillCmd.AppendLine("where ch.Success = 0 and ch.Amount > 0 and cd.SaleTypeID = 2 ");
        rebillCmd.AppendLine(" and ch.ChargeDate between '@startDT 00:00:00' and '@endDT 23:59:59' @statusFilter @productFilter @affiliateFilter @midFilter @paymentTypeFilter");
        if (excludeReattempts == 1)
        {
            rebillCmd.AppendLine(" and prev.ChargeHistoryID is null");
        }
        rebillCmd.AppendLine(")");

        StringBuilder upsellCmd = new StringBuilder();
        upsellCmd.AppendLine("(");
        upsellCmd.AppendLine("select Response, bs.BillingID from ChargeHistoryEx ch ");
        upsellCmd.AppendLine("inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID ");
        upsellCmd.AppendLine("inner join Billing b on b.BillingID = bs.BillingID ");
        upsellCmd.AppendLine("inner join Subscription ss on ss.SubscriptionID = bs.SubscriptionID ");
        upsellCmd.AppendLine("left join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID ");
        upsellCmd.AppendLine("where ch.Success = 0 and ch.Amount > 0 and cd.SaleTypeID is null ");
        upsellCmd.AppendLine(" and ch.ChargeDate between '@startDT 00:00:00' and '@endDT 23:59:59' @statusFilter @productFilter @affiliateFilter @midFilter @paymentTypeFilter");
        upsellCmd.AppendLine(")");

        StringBuilder query = new StringBuilder();
        query.AppendLine("select BillingID, ");
        query.AppendLine(" replace(substring_index(substring_index(substring_index(substring_index( ");
        query.AppendLine("  replace(substring_index(substring_index(substring_index(substring_index( ");
        query.AppendLine("   replace(substring_index(substring_index(substring_index(substring_index(substring_index(substring_index(Response, '&authcode', 1) , 'responsetext=', -1), 'REFID', 1), '  ', 1), '.', 1), '-', 1), '+', ' ') ");
        query.AppendLine("  , '&subError', 1) , 'errString=', -1), '.', 1), '-', 1), '+', ' ') ");
        query.AppendLine(" , '</description>', 1) , '<description>', -1), '.', 1), '-', 1), '+', ' ') as Error from");
        query.AppendLine("(");

        switch (salesType)
        {
            case 1:
                query.AppendLine(trialCmd.ToString());
                break;
            case 2:
                query.AppendLine(rebillCmd.ToString());
                break;
            case 3:
                query.AppendLine(upsellCmd.ToString());
                break;
            default:
                query.AppendLine(trialCmd.ToString());
                query.AppendLine("union all");
                query.AppendLine(rebillCmd.ToString());
                query.AppendLine("union all");
                query.AppendLine(upsellCmd.ToString());
                break;
        }

        query.AppendLine(") query");

        query.Replace("@startDT", startDT);
        query.Replace("@endDT", endDT);
        query.Replace("@statusFilter", statusFilter);
        query.Replace("@productFilter", productFilter);
        query.Replace("@affiliateFilter", affiliateFilter);
        query.Replace("@midFilter", midFilter);
        query.Replace("@paymentTypeFilter", paymentTypeFilter);
        

        StringBuilder mainQuery = new StringBuilder();
        mainQuery.AppendLine("select Error, count(*) as Count, CAST(GROUP_CONCAT(CAST(BillingID as char))as char(512)) as BillingIDs from");
        mainQuery.AppendLine("(");
        mainQuery.AppendLine(query.ToString());
        mainQuery.AppendLine(") AllErrors");
        mainQuery.AppendLine("group by Error order by Count desc");        

        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = mainQuery.ToString();

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string error = reader["Error"].ToString().ToLower();
                string billingIDs = reader["BillingIDs"].ToString() + ",";
                int count = Convert.ToInt32(reader["Count"].ToString());

                if (error.IndexOf("avs") >= 0)
                {
                    json.AvsRejected += count;
                    json.AvsRejectedBID += billingIDs;
                }

                else if (error.IndexOf("call") >= 0)
                {
                    json.CallVoiceCenter += count;
                    json.CallVoiceCenterBID += billingIDs;
                }

                else if (error.IndexOf("currency") >= 0)
                {
                    json.CurrencyNotAvailable += count;
                    json.CurrencyNotAvailableBID += billingIDs;
                }
                else if (error.IndexOf("cvv") >= 0 || error.IndexOf("cvd") >= 0 || error.IndexOf("security code") >= 0)
                {
                    json.CvvFailure += count;
                    json.CvvFailureBID += billingIDs;
                }
                else if (error.IndexOf("risk") >= 0)
                {
                    json.DeclinedByRiskManagement += count;
                    json.DeclinedByRiskManagementBID += billingIDs;
                }

                else if (error.IndexOf("duplicate") >= 0)
                {
                    json.DuplicateTransaction += count;
                    json.DuplicateTransactionBID += billingIDs;
                }

                else if (error.IndexOf("expired") >= 0)
                {
                    json.ExpiredCard += count;
                    json.ExpiredCardBID += billingIDs;
                }
                else if (error.IndexOf("expire") >= 0 || error.IndexOf("expiration") >= 0 || error.IndexOf("exp date") >= 0)
                {
                    json.InvalidExpirationDate += count;
                    json.InvalidExpirationDateBID += billingIDs;
                }
                else if (error.IndexOf("insufficient") >= 0)
                {
                    json.InsufficientFunds += count;
                    json.InsufficientFundsBID += billingIDs;
                }

                else if (error.IndexOf("card number") >= 0 || error.IndexOf("card no") >= 0 || error.IndexOf("ccnumber") >= 0 || error.IndexOf("card #") >= 0 || error.IndexOf("no account") >= 0)
                {
                    json.InvalidCardNumber += count;
                    json.InvalidCardNumberBID += billingIDs;
                }
                else if (error.IndexOf("hold") >= 0 || error.IndexOf("lost") >= 0 || error.IndexOf("pick up") >= 0)
                {
                    json.LostOrStolenCard += count;
                    json.LostOrStolenCardBID += billingIDs;
                }
                else if (error.IndexOf("cannot be refunded") >= 0)
                {
                    json.TransactionAlreadyVoided += count;
                    json.TransactionAlreadyVoidedBID += billingIDs;
                }

                else if (error.IndexOf("invalid refund amount") >= 0)
                {
                    json.InvalidRefundAmount += count;
                    json.InvalidRefundAmountBID += billingIDs;
                }

                else if (error.IndexOf("exceeded") >= 0 || error.IndexOf("not accepted") >= 0 || error.IndexOf("exceeds") >= 0 || error.IndexOf("declined") >= 0 || error.IndexOf("failed") >= 0 ||
                    error.IndexOf("refused") >= 0 || error.IndexOf("invalid card") >= 0 || error.IndexOf("not in store list") >= 0 || error.IndexOf("auth") >= 0 ||
                    error.IndexOf("requested") >= 0 || error.IndexOf("negative") >= 0 || error.IndexOf("permit") >= 0 || error.IndexOf("you submitted") >= 0 || error.IndexOf("overdraft") >= 0)
                {
                    json.DeclinedByIssuer += count;
                    json.DeclinedByIssuerBID += billingIDs;
                }
                else
                {
                    json.GatewayError += count;
                    json.GatewayErrorBID += billingIDs;
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)
                command.Dispose();
        }

        return json;
    }

    public class JSON
    {
        public int AvsRejected { get; set; }
        public string AvsRejectedBID { get; set; }
        public int CallVoiceCenter { get; set; }
        public string CallVoiceCenterBID { get; set; }
        public int CurrencyNotAvailable { get; set; }
        public string CurrencyNotAvailableBID { get; set; }
        public int CvvFailure { get; set; }
        public string CvvFailureBID { get; set; }
        public int DeclinedByIssuer { get; set; }
        public string DeclinedByIssuerBID { get; set; }
        public int DeclinedByRiskManagement { get; set; }
        public string DeclinedByRiskManagementBID { get; set; }
        public int DuplicateTransaction { get; set; }
        public string DuplicateTransactionBID { get; set; }
        public int ExpiredCard { get; set; }
        public string ExpiredCardBID { get; set; }
        public int GatewayError { get; set; }
        public string GatewayErrorBID { get; set; }
        public int InsufficientFunds { get; set; }
        public string InsufficientFundsBID { get; set; }
        public int InvalidCardNumber { get; set; }
        public string InvalidCardNumberBID { get; set; }
        public int InvalidExpirationDate { get; set; }
        public string InvalidExpirationDateBID { get; set; }
        public int InvalidRefundAmount { get; set; }
        public string InvalidRefundAmountBID { get; set; }
        public int LostOrStolenCard { get; set; }
        public string LostOrStolenCardBID { get; set; }
        public int TransactionAlreadyVoided { get; set; }
        public string TransactionAlreadyVoidedBID { get; set; }
    }
    
 
</script>
