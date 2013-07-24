using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.IO;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using TrimFuel.Web.Admin.Logic;
using System.Web.UI;
using System.Web.SessionState;

public partial class order_report : PageX
{
    public class JSON
    {
        public string startDT { get; set; }
        public string endDT { get; set; }
        // public string reason { get; set; }
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
    public static JSON Reasons(string mode, string startDT, string endDT, string affiliate, string subAffiliate, string mid, int statusTID, int productID, int salesType)
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
        json.startDT = startDT;
        json.endDT = endDT;
        // json.reason = reason;

        string statusFilter = string.Empty;
        string productFilter = string.Empty;
        string affiliateFilter = string.Empty;
        string midFilterCh = string.Empty;
        string midFilterFch = string.Empty;

        if (statusTID > -1)
            statusFilter = "and bs.StatusTID = " + statusTID;

        if (productID > -1)
            productFilter = "and ss.ProductID = " + productID;

        if (!string.IsNullOrEmpty(subAffiliate))
            affiliateFilter = string.Format("and b.SubAffiliate = '{0}'", subAffiliate);

        else if (!string.IsNullOrEmpty(affiliate))
            affiliateFilter = string.Format("and b.Affiliate = '{0}'", affiliate);

        else if (!string.IsNullOrEmpty(mid))
        {
            midFilterCh = string.Format("and ch.MerchantAccountID = '{0}'", mid);
            midFilterFch = string.Format("and fch.MerchantAccountID = '{0}'", mid);
        }

        string[] where = { 
            "locate('<description>', Response) > 0",
            "locate('errString=', Response) > 0",
            "locate('responsetext=', Response) > 0"};

        string[] select = {
            "replace(substring_index(substring_index(substring_index(substring_index(Response, '</description>', 1) , '<description>', -1), '.', 1), '-', 1), '+', ' ') as Error, BillingID",
            "replace(substring_index(substring_index(substring_index(substring_index(Response, '&subError', 1) , 'errString=', -1), '.', 1), '-', 1), '+', ' ') as Error, BillingID",
            "replace(substring_index(substring_index(substring_index(substring_index(substring_index(substring_index(Response, '&authcode', 1) , 'responsetext=', -1), 'REFID', 1), '  ', 1), '.', 1), '-', 1), '+', ' ') as Error, BillingID" };

        string[] alias = { "A", "B", "C" };

        StringBuilder failed = new StringBuilder();
        failed.AppendLine("(");
        failed.AppendLine("select Response, b.BillingID from FailedChargeHistory fch");
        failed.AppendLine("inner join Billing b on b.BillingID = fch.BillingID");
        // failed.AppendLine("inner join Campaign cmp on cmp.CampaignID = b.CampaignID");
        // failed.AppendLine("inner join Subscription ss on ss.SubscriptionID = cmp.SubscriptionID");
        // if (!statusFilter.Equals(string.Empty))
        //     failed.AppendLine("inner join BillingSubscription bs on bs.BillingID = b.BillingID");
        failed.AppendLine("where @where and fch.ChargeDate between '@startDT 00:00:00' and '@endDT 23:59:59' @affiliateFilter @statusFilter @productFilter @midFilterFch");
        failed.AppendLine(")");

        StringBuilder unsuccesful = new StringBuilder();
        unsuccesful.AppendLine("(");
        unsuccesful.AppendLine("select Response, bs.BillingID from ChargeHistoryEx ch");
        unsuccesful.AppendLine("inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID");
        unsuccesful.AppendLine("inner join Billing b on b.BillingID = bs.BillingID");
        //unsuccesful.AppendLine("inner join Subscription ss on ss.SubscriptionID = bs.SubscriptionID");
        //unsuccesful.AppendLine("left join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID");
        unsuccesful.AppendLine("where @where and ch.Success = 0 and ch.ChargeDate between '@startDT 00:00:00' and '@endDT 23:59:59' @affiliateFilter @statusFilter @productFilter @midFilterCh");
        unsuccesful.AppendLine(")");

        StringBuilder query = new StringBuilder();
        query.AppendLine("select @select from");
        query.AppendLine("(");

        switch (salesType)
        {
            case 1:
                query.AppendLine(failed.ToString());
                break;
            case 2:
                query.AppendLine(unsuccesful.ToString());
                break;
            case 3:
                query.AppendLine(unsuccesful.ToString());
                break;
            default:
                query.AppendLine(failed.ToString());
                query.AppendLine("union all");
                query.AppendLine(unsuccesful.ToString());
                break;
        }

        query.AppendLine(") @count");

        query.Replace("@startDT", startDT);
        query.Replace("@endDT", endDT);
        query.Replace("@statusFilter", statusFilter);
        query.Replace("@productFilter", productFilter);
        query.Replace("@affiliateFilter", affiliateFilter);
        query.Replace("@midFilterCh", midFilterCh);
        query.Replace("@midFilterFch", midFilterFch);

        StringBuilder mainQuery = new StringBuilder();
        mainQuery.AppendLine("select Error, count(*) as Count, CAST(GROUP_CONCAT(CAST(BillingID as char))as char(512)) as BillingIDs from");
        mainQuery.AppendLine("(");
        for (int i = 0; i <= 2; i++)
        {
            mainQuery.AppendLine(query.ToString().Replace("@select", select[i]).Replace("@where", where[i]).Replace("@count", alias[i]));

            if (i != 2)
                mainQuery.AppendLine("union all");
        }
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
                else if (error.IndexOf("cvv") >= 0 || error.IndexOf("cvd") >= 0)
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
                else if (error.IndexOf("hold") >= 0 || error.IndexOf("lost") >= 0)
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

                else if (error.IndexOf("exceeded") >= 0 || error.IndexOf("not accepted") >= 0 || error.IndexOf("exceeds") >= 0 || error.IndexOf("decline") >= 0 || error.IndexOf("failed") >= 0 ||
                    error.IndexOf("refused") >= 0 || error.IndexOf("invalid card") >= 0 || error.IndexOf("not in store list") >= 0 || error.IndexOf("pick up") >= 0 || error.IndexOf("auth") >= 0 ||
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
    public string GetBidsByRequest(JSON json)
    {
        string bids = "";
        switch (Request["reason"])
        {
            case "AVS Rejected": bids = json.AvsRejectedBID; break;
            case "Call Voice Center": bids = json.CallVoiceCenterBID; break;
            case "Currency Not Available": bids = json.CurrencyNotAvailableBID; break;
            case "Cvv Failure": bids = json.CvvFailureBID; break;
            case "Declined By Issuer": bids = json.DeclinedByIssuerBID; break;
            case "Declined By Risk Management": bids = json.DeclinedByRiskManagementBID; break;
            case "Duplicate Transaction": bids = json.DuplicateTransactionBID; break;
            case "Expired Card": bids = json.ExpiredCardBID; break;
            case "Gateway Error": bids = json.GatewayErrorBID; break;
            case "Insufficient Funds": bids = json.InsufficientFundsBID; break;
            case "Invalid Card Number": bids = json.InvalidCardNumberBID; break;
            case "Invalid Expiration Date": bids = json.InvalidExpirationDateBID; break;
            case "Invalid Refund Amount": bids = json.InvalidRefundAmountBID; break;
            case "Lost Or Stolen Card": bids = json.LostOrStolenCardBID; break;
            case "Transaction Already Voided": bids = json.TransactionAlreadyVoidedBID; break;
            default: break;

        }
        return bids;
    }
    protected override void OnDataBinding(EventArgs e)
    {
        base.OnDataBinding(e);

        List<TableRow> rows = new List<TableRow>();

        try
        {
            MySqlConnection connection = new MySqlConnection();
            MySqlCommand command = new MySqlCommand();
            JSON json = Reasons(Request["mode"], Request["startDT"], Request["endDT"], Request["affiliate"], Request["subAffiliate"], Request["mid"], int.Parse(string.IsNullOrEmpty(Request["statusTID"]) ? "-1" : Request["statusTID"]), int.Parse(string.IsNullOrEmpty(Request["productID"]) ? "-1" : Request["productID"]), int.Parse(string.IsNullOrEmpty(Request["salesType"]) ? "0" : Request["salesType"]));
            string[] vars = GetBidsByRequest(json).Split(',');

            StringBuilder query = new StringBuilder();

            query.AppendLine("select b.BillingID BID, c.DisplayName Campaign, Coalesce(fs.FraudScore, 0) FraudScore, st.DisplayName Status, b.CreateDT OrderDate, b.FirstName, b.LastName, b.Address1 Address, b.City, b.State, b.Zip, b.Phone, b.Email, b.Affiliate, b.SubAffiliate from Billing b");
            query.AppendLine("left OUTER join BillingSubscription bs on bs.BillingID = b.BillingID");
            query.AppendLine("left OUTER join StatusType  st on bs.StatusTID = st.StatusTypeID");
            query.AppendLine("left OUTER join FraudScore fs on fs.BillingID = b.BillingID");
            query.AppendLine("left OUTER join Campaign  c on c.CampaignID = b.CampaignID ");
            query.AppendLine("where");
            //
            for (int i = 0; i < vars.Length - 1; i++)
            {
                query.AppendFormat(" b.BillingID = {0}", vars[i]);

                if (i < vars.Length - 2)
                    query.AppendLine(" or");
            }

            try
            {
                connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = query.ToString();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TableRow row = new TableRow();
                    row.BID = reader["BID"].ToString();
                    row.Campaign = reader["Campaign"].ToString();
                    row.FraudScore = reader["FraudScore"].ToString();
                    row.Status = reader["Status"].ToString();
                    row.OrderDate = reader["OrderDate"].ToString();
                    row.FirstName = reader["FirstName"].ToString();
                    row.LastName = reader["LastName"].ToString();
                    row.Address = reader["Address"].ToString();
                    row.City = reader["City"].ToString();
                    row.State = reader["State"].ToString();
                    row.Zip = reader["Zip"].ToString();
                    row.Phone = reader["Phone"].ToString();
                    row.Email = reader["Email"].ToString();
                    row.Affiliate = reader["Affiliate"].ToString();
                    row.SubID = reader["SubAffiliate"].ToString();
                    rows.Add(row);
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
            //TableRow row2 = new TableRow();
            //row2.BID = vars.Length.ToString();

            //rows.Add(row2);
            rOrders.DataSource = rows;

            rOrders.DataBind();
        }
        catch
        {
        }
    }

    public override string HeaderString
    {
        get { return "Failed Transactions"; }
    }

    public class TableRow
    {
        public string BID { get; set; }
        public string Campaign { get; set; }
        public string FraudScore { get; set; }
        public string Status { get; set; }
        public string OrderDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Affiliate { get; set; }
        public string SubID { get; set; }
    }
}