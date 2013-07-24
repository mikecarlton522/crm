using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using TrimFuel.Model;


public partial class campaign_performance : System.Web.UI.Page
{
    protected string startDate = string.Empty;
    protected string endDate = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request["start"]))
                startDate = DateTime.Parse(Request["start"]).ToString("M/d/yyyy");

            else
                startDate = DateTime.Today.AddDays(-7).ToString("M/d/yyyy");

            if (!string.IsNullOrEmpty(Request["end"]))
                endDate = DateTime.Parse(Request["end"]).ToString("M/d/yyyy");

            else
                endDate = DateTime.Today.ToString("M/d/yyyy");

            FetchAffiliates(ref affiliates);
        }
    }

    private void FetchAffiliates(ref DropDownList dropDownList)
    {
        string selected = Request["affiliate"];

        string query = @"
            select IfNull(Affiliate, '') as Code from Billing 
            where CreditCard <> '*...............' and CreditCard <> '4111111111111111'
            group by IfNull(Affiliate, '') order by IfNull(Affiliate, '')";

        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = query.ToString();

            MySqlDataReader reader = command.ExecuteReader();

            dropDownList.Items.Add("Total");
            while (reader.Read())
            {
                string code = reader["code"] as string;

                dropDownList.Items.Add(new ListItem(code));
            }

            dropDownList.SelectedValue = selected;

            if (dropDownList.SelectedIndex <= 0 && selected != "Total")
            {
                Response.Clear();
                Response.Write("Affiliate not found");
                Response.End();
            }
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)
                command.Dispose();
        }
    }

    private static string CustomerTable(string affiliate)
    {
        bool affiliateFilter = affiliate != "Total";

        return @" 
            (
			select b.BillingID, b.FirstName, b.LastName, b.Affiliate, b.SubAffiliate, b.PaymentTypeID, b.CreateDT, min(o.CreateDT) as FirstOrderDT from Orders o
			inner join OrderSale sl on sl.OrderID = o.OrderID
			inner join Billing b on o.BillingID = b.BillingID
			inner join Orders oForDateFilter on oForDateFilter.BillingID = b.BillingID
			where sl.SaleStatus = 1 and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'"
            + AffiliateFilter("b", affiliateFilter) + @"
			and oForDateFilter.CreateDT between @start and @end
			group by b.BillingID
			) b";
    }

    [WebMethod]
    public static string FetchSeriesForGraph1(string affiliate, string start, string end)
    {
        bool affiliateFilter = affiliate != "Total";

        string prelandingQuery =
            " select sum(case c.PageTypeID when 5 then c.Hits else 0 end) as Value, date_format(c.CreateDT, '%m/%d/%Y') as Date from Conversion c" +
            " where c.CreateDT between @start and @end" + AffiliateFilter("c", affiliateFilter) +
            " group by Date" +
            " union all" +
            " select sum(case tc.PageTypeID when 5 then 1 else 0 end) as Value, date_format(tc.CreateDT, '%m/%d/%Y') as Date from TempConversion tc" +
            " where tc.CreateDT between @start and @end" + AffiliateFilter("tc", affiliateFilter) +
            " group by Date";

        string landingQuery =
            " select sum(case c.PageTypeID when 1 then c.Hits else 0 end) as Value, date_format(c.CreateDT, '%m/%d/%Y') as Date from Conversion c" +
            " where c.CreateDT between @start and @end" + AffiliateFilter("c", affiliateFilter) +
            " group by Date" +
            " union all" +
            " select sum(case tc.PageTypeID when 1 then 1 else 0 end) as Value, date_format(tc.CreateDT, '%m/%d/%Y') as Date from TempConversion tc" +
            " where tc.CreateDT between @start and @end" + AffiliateFilter("tc", affiliateFilter) +
            " group by Date";

        string billingQuery =
            " select sum(case c.PageTypeID when 2 then c.Hits else 0 end) as Value, date_format(c.CreateDT, '%m/%d/%Y') as Date from Conversion c" +
            " where c.CreateDT between @start and @end" + AffiliateFilter("c", affiliateFilter) +
            " group by Date" +
            " union all" +
            " select sum(case tc.PageTypeID when 2 then 1 else 0 end) as Value, date_format(tc.CreateDT, '%m/%d/%Y') as Date from TempConversion tc" +
            " where tc.CreateDT between @start and @end" + AffiliateFilter("tc", affiliateFilter) +
            " group by Date";

        string conversionQuery =
            " select sum(Value) as Value, Date from " +
                " (" +
                " select count(bs.BillingSubscriptionID) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs" +
                " inner join Billing b on bs.BillingID = b.BillingID" +
                " where b.FirstOrderID is null and bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" + 
                " union all " +
                " select count(b.BillingID) as Value, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
                " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
                " where firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " ) t1" +
            " group by Date";

        string prelandingJson = GetJsonForSeries(prelandingQuery, affiliate, start, end, "prelandings");

        string landingJson = GetJsonForSeries(landingQuery, affiliate, start, end, "landings");

        string billingJson = GetJsonForSeries(billingQuery, affiliate, start, end, "billings");

        string conversionJson = GetJsonForSeries(conversionQuery, affiliate, start, end, "conversions");

        string json = string.Concat("{ ", prelandingJson, ", ", landingJson, ", ", billingJson, ", ", conversionJson, " }");

        return json;
    }

    [WebMethod]
    public static string FetchSeriesForGraph2(string affiliate, string start, string end)
    {
        bool affiliateFilter = affiliate != "Total";

        string cancellationsQuery =
            " select sum(InactiveRecurringBillingSubscriptionWithoutRebills) as Value, t.Date from" +
            " (" +
            "     select 1 as InactiveRecurringBillingSubscriptionWithoutRebills, date_format(bs.CreateDT, '%m/%d/%Y') as Date" +
            "     from BillingSubscription bs" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID" +
            "     inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID" +
            "     inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
            "     where bs.StatusTID <> 1 and s.Recurring = 1 and bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            "     union all" +
            "     select 1 as InactiveRecurringBillingSubscriptionWithoutRebills, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
            "     inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            "     inner join Orders o on o.BillingID = b.BillingID" +
            "     inner join OrderSale sl on sl.OrderID = o.OrderID" +
            "     inner join OrderRecurringPlan orp on orp.SaleID = sl.SaleID" +
            "     where sl.SaleStatus = 1 and orp.RecurringStatus <> 1" +
            "     and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " ) t" +
            " group by t.Date";

        string refundsQuery =
            " select sum(Value) as Value, Date from " +
                " (" +
                " select sum(case when ch.Amount < 0 then 1 else 0 end )as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from ChargeHistoryEx ch" +
                " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID"+
                " inner join Billing b on bs.BillingID = b.BillingID"+
                " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
                " where ch.Success = 1 and bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " union all " +
                " select count(ch.ChargeHistoryID) as Value, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
                " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
                " inner join Orders o on o.BillingID = b.BillingID" +
                " inner join Invoice i on i.OrderID = o.OrderID" +
                " inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
                " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
                " where ch.Success = 1 and ch.Amount < 0" +
                " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " ) t1" +
            " group by Date";

        string chargebakcsQuery =
                " select count(t.ChargebackFee) as Value, t.Date from" +
                " (" +
                "     select distinct ch.ChargeHistoryID, ch.Amount as ChargebackAmount, am.ChargebackFee, date_format(bs.CreateDt, '%m/%d/%Y') as Date from SaleChargeback scb" +
                "     inner join BillingSale bsale on bsale.SaleID = scb.SaleID" +
                "     inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID" +
                "     inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID" +
                "     inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID" +
                "     inner join Billing b on bs.BillingID = b.BillingID" +
                "     inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
                "     where b.Affiliate = @affiliate and bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
                "     union all " +
                "     select ch.ChargeHistoryID, ch.Amount, am.ChargebackFee, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
                "     inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
                "     inner join Orders o on o.BillingID = b.BillingID" +
                "     inner join OrderSale sl on sl.OrderID = o.OrderID" +
                "     inner join SaleChargeback scb on scb.SaleID = sl.SaleID" +
                "     inner join Invoice i on i.InvoiceID = sl.InvoiceID" +
                "     inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
                "     inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
                "     inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID" +
                "     where ch.Success = 1 and ch.Amount > 0" +
                "     and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " ) t" +
                " group by Date";

        string cancellationsJson = GetJsonForSeries(cancellationsQuery, affiliate, start, end, "cancellations");

        string refundsJson = GetJsonForSeries(refundsQuery, affiliate, start, end, "refunds");

        string chargebacksJson = GetJsonForSeries(chargebakcsQuery, affiliate, start, end, "chargebacks");

        string json = string.Concat("{ ", cancellationsJson, ", ", refundsJson, ", ", chargebacksJson, " }");

        return json;
    }

    [WebMethod]
    public static string FetchSeriesForGraph3(string affiliate, string start, string end)
    {
        bool affiliateFilter = affiliate != "Total";

        string cpsQuery =
            " select sum(Value) as Value, Date from" +
                " (" +
                " select sum(IfNull(aff.CostPerSale, 0)) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date  from BillingSubscription bs" +
                " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
                " inner join Billing b on bs.BillingID = b.BillingID"+
                " left join Affiliate aff on aff.Code = b.Affiliate"+
                " where b.FirstOrderID is null and bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " union all " +
                " select sum(IfNull(aff.CostPerSale, 0)) as Value, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
                " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
                " left join Affiliate aff on aff.Code = b.Affiliate" +
                " where firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " ) t1" +
            " group by Date";

        string cpsJson = GetJsonForSeries(cpsQuery, affiliate, start, end, "sales");

        string json = string.Concat("{ ", cpsJson, " }");

        return json;
    }

    [WebMethod]
    public static string FetchSeriesForGraph4(string affiliate, string start, string end)
    {
        bool affiliateFilter = affiliate != "Total";

        string merchantingFeesQuery =
            " select sum(Value) as Value, Date from" +
                " (" +
                " select 0 - sum(Abs(ch.Amount) * IfNull(am.ProcessingRate, 0) / 100 + IfNull(am.TransactionFee, 0)) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from ChargeHistoryEx ch" +
                " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID"+
                " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID"+
                " inner join Billing b on bs.BillingID = b.BillingID"+
                " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
                " where ch.Success = 1 and bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " union all " +
                " select 0 - sum(Abs(ch.Amount) * IfNull(am.ProcessingRate, 0) / 100 + IfNull(am.TransactionFee, 0)) as Value, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
                " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
                " inner join Orders o on o.BillingID = b.BillingID" +
                " inner join Invoice i on i.OrderID = o.OrderID" +
                " inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
                " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
                " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID" +
                " where ch.Success = 1" +
                " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " ) t1" +
            " group by Date";


        string reserveAccountFeesQuery =
            " select sum(Value) as Value, Date from" +
                " (" +
                " select 0 - sum(Abs(ch.Amount) * IfNull(am.ReserveAccountRate, 0) / 100) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from ChargeHistoryEx ch" +
                " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID"+
                " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID"+
                " inner join Billing b on bs.BillingID = b.BillingID"+
                " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
                " where ch.Success = 1 and bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " union all " +
                " select 0 - sum(Abs(ch.Amount) * IfNull(am.ReserveAccountRate, 0) / 100) as Value, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
                " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
                " inner join Orders o on o.BillingID = b.BillingID" +
                " inner join Invoice i on i.OrderID = o.OrderID" +
                " inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
                " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
                " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID" +
                " where ch.Success = 1" +
                " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
                " group by Date" +
                " ) t1" +
            " group by Date";


        string merchantingFeesJson = GetJsonForSeries(merchantingFeesQuery, affiliate, start, end, "merchantingFees");

        string reserveAccountFeesJson = GetJsonForSeries(reserveAccountFeesQuery, affiliate, start, end, "reserveAccountFees");

        string json = string.Concat("{ ", merchantingFeesJson, ", ", reserveAccountFeesJson, " }");

        return json;
    }

    [WebMethod]
    public static string FetchSeriesForGraph5(string affiliate, string start, string end)
    {
        bool affiliateFilter = affiliate != "Total";

        string query = 
            " select sum(t.Value) as Value, t.Date from"+
            " ("+
            " select IfNull(sh.FulfillmentCost, 0) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs"+
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " inner join ExtraTrialShip ets on ets.BillingID = b.BillingID"+
            " inner join ExtraTrialShipSale etss on etss.ExtraTrialShipID = ets.ExtraTrialShipID"+
            " inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = etss.SaleID and ship.Completed = 1"+
            " inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            " where bs.CreateDT between @start and @end and sh.Name = '@shipper'" + AffiliateFilter("b", affiliateFilter) +
            " "+
            " union all			"+
            " "+
            " select IfNull(sh.FulfillmentCost, 0) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs"+
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            " inner join UpsellSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            " inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            " inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            " where bs.CreateDT between @start and @end and sh.Name = '@shipper'" + AffiliateFilter("b", affiliateFilter) +
			" "+
            " union all"+
            " "+
            " select IfNull(sh.FulfillmentCost, 0) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs"+
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            " inner join BillingSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            " inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            " inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            " where bs.CreateDT between @start and @end and sh.Name = '@shipper'" + AffiliateFilter("b", affiliateFilter) +
            " "+
            " union all"+
            " "+
            " select IfNull(sh.FulfillmentCost, 0) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " inner join ExtraTrialShip ets on ets.BillingID = b.BillingID"+
            " inner join ExtraTrialShipSale sl on sl.ExtraTrialShipID = ets.ExtraTrialShipID"+
            " inner join ReturnedSale rs on rs.SaleID = sl.SaleID"+
            " inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            " inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            " where bs.CreateDT between @start and @end and sh.Name = '@shipper'" + AffiliateFilter("b", affiliateFilter) +
			" "+
            " union all"+
            " "+
            " select IfNull(sh.FulfillmentCost, 0) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs			"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            " inner join UpsellSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            " inner join ReturnedSale rs on rs.SaleID = sl.SaleID"+
            " inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            " inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            " where bs.CreateDT between @start and @end and sh.Name = '@shipper'" + AffiliateFilter("b", affiliateFilter) +
            " "+
            " union all"+
            " "+
            " select IfNull(sh.FulfillmentCost, 0) as Value, date_format(bs.CreateDT, '%m/%d/%Y') as Date from BillingSubscription bs			"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            " inner join BillingSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            " inner join ReturnedSale rs on rs.SaleID = sl.SaleID"+
            " inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            " inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            " where bs.CreateDT between @start and @end and sh.Name = '@shipper'" + AffiliateFilter("b", affiliateFilter) +
            " "+
            " union all"+
            " "+
            " select case when sh.ShipmentStatus >= 40 then 2*IfNull(shp.FulfillmentCost, 0) else IfNull(shp.FulfillmentCost, 0) end as Value, date_format(firstOrder.CreateDT, '%m/%d/%Y') as Date from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
			" inner join Orders o on o.BillingID = b.BillingID" +
			" inner join OrderSale sl on sl.OrderID = o.OrderID" +
			" inner join Shipment sh on sh.SaleID = sl.SaleID" +
			" inner join Shipper shp on shp.ShipperID = sh.ShipperID" +
			" where sh.ShipmentStatus >= 30" +
            " and firstOrder.CreateDT between @start and @end and shp.Name = '@shipper' and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by sh.ShipperRegID, sh.ShipmentStatus" +
            " ) t" +
            " group by t.Date";

        string tsnJson = GetJsonForSeries(query.Replace("@shipper", "TSN/Novaship"), affiliate, start, end, "tsn");

        string abfJson = GetJsonForSeries(query.Replace("@shipper", "ABF/USellWeShip"), affiliate, start, end, "abf");

        string keymailJson = GetJsonForSeries(query.Replace("@shipper", "Keymail"), affiliate, start, end, "keymail");

        string json = string.Concat("{ ", abfJson, " }");

        return json;
    }

    [WebMethod]
    public static List<JsonRow> FetchOrdersForTable(string affiliate, string start, string end, int mode)
    {
        bool affiliateFilter = affiliate != "Total";

        List<JsonRow> rows = new List<JsonRow>();

        DateTime startDate = DateTime.Parse(start);
        DateTime endDate = DateTime.Parse(end);

        string[] query = new string[5];

        query[0] = 
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs"+
            " inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " where b.FirstOrderID is null and bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " union " +
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
	        " join Orders o on o.BillingID = b.BillingID" +
	        " join OrderSale os2 on os2.OrderID = o.OrderID" +
	        " left join OrderRecurringPlan orp on orp.SaleID = os2.SaleID and orp.RecurringStatus > 0" +
            " where firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by b.BillingID" +
            " order by CreateDT desc" +
            " limit 100";

        query[1] =
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs" +
            " inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            " inner join Billing b on bs.BillingID = b.BillingID" +
            " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            " inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID"+
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " where bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " and bs.StatusTID <> 1 and s.Recurring = 1"+
            " union all"+
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from ChargeHistoryEx ch" +
            " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID"+
            " inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            " inner join Billing b on bs.BillingID = b.BillingID" +
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " where ch.Amount < 0 and ch.Success = 1 and bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            " union all"+
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from SaleChargeback scb" +
            " inner join BillingSale bsale on bsale.SaleID = scb.SaleID"+
            " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID"+
            " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID"+
            " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID"+
            " inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            " inner join Billing b on bs.BillingID = b.BillingID" +
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID"+
            " where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            " union" +
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            " inner join Orders o on o.BillingID = b.BillingID" +
            " inner join OrderSale sl on sl.OrderID = o.OrderID" +
            " inner join OrderRecurringPlan orp on orp.SaleID = sl.SaleID" +
            " where sl.SaleStatus = 1 and orp.RecurringStatus <> 1" +
            " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by b.BillingID" +
            " union" +
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            " inner join Orders o on o.BillingID = b.BillingID" +
            " join OrderSale os2 on os2.OrderID = o.OrderID" +
            " left join OrderRecurringPlan orp on orp.SaleID = os2.SaleID and orp.RecurringStatus > 0" +
            " inner join Invoice i on i.OrderID = o.OrderID" +
            " inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
            " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
            " where ch.Success = 1 and ch.Amount < 0" +
            " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by b.BillingID" +
            " union" +
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            " inner join Orders o on o.BillingID = b.BillingID" +
            " join OrderSale os2 on os2.OrderID = o.OrderID" +
            " left join OrderRecurringPlan orp on orp.SaleID = os2.SaleID and orp.RecurringStatus > 0" +
            " inner join OrderSale sl on sl.OrderID = o.OrderID" +
            " inner join SaleChargeback scb on scb.SaleID = sl.SaleID" +
            " inner join Invoice i on i.InvoiceID = sl.InvoiceID" +
            " inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
            " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
            " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID" +
            " where ch.Success = 1 and ch.Amount > 0" +
            " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by b.BillingID" +
            " limit 100";

        query[2] =
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs" +
            " inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
            " inner join Billing b on bs.BillingID = b.BillingID"+
            " where b.FirstOrderID is null and bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " union " +
            " select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            " inner join Orders o on o.BillingID = b.BillingID" +
            " join OrderSale os2 on os2.OrderID = o.OrderID" +
            " left join OrderRecurringPlan orp on orp.SaleID = os2.SaleID and orp.RecurringStatus > 0" +
            " inner join Affiliate aff on aff.Code = b.Affiliate" +
            " where IfNull(aff.CostPerSale, 0) > 0" +
            " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by b.BillingID" +
            " order by CreateDT desc" +
            " limit 100";

        query[3] =
            " select distinct b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from ChargeHistoryEx ch" +
            " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID"+
            " inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            " inner join Billing b on b.BillingID = bs.BillingID" +
            " where ch.Success = 1 and bs.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " union " +
            " select distinct b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            " inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            " inner join Orders o on o.BillingID = b.BillingID" +
            " join OrderSale os2 on os2.OrderID = o.OrderID" +
            " left join OrderRecurringPlan orp on orp.SaleID = os2.SaleID and orp.RecurringStatus > 0" +
            " inner join Invoice i on i.OrderID = o.OrderID" +
            " inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID" +
            " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID" +
            " inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID" +
            " where ch.Success = 1" +
            " and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            " group by b.BillingID" +
            " order by CreateDT desc" +
            " limit 100";

        query[4] =
            " select * from"+
            " ("+
            "     select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs" +
            "     inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ExtraTrialShip ets on ets.BillingID = b.BillingID"+
            "     inner join ExtraTrialShipSale etss on etss.ExtraTrialShipID = ets.ExtraTrialShipID"+
            "     inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = etss.SaleID and ship.Completed = 1"+
            "     inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            "     where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            "     limit 100"+
            "         "+
            "     union all			"+
            "         "+
            "     select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs" +
            "     inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            "     inner join UpsellSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            "     inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            "     inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            "     where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            "     limit 100"+
			" "+
            "     union all"+
            "         "+
            "     select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs" +
            "     inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            "     inner join BillingSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            "     inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            "     inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            "     where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            "     limit 100"+
            "         "+
            "     union all"+
            "         "+
            "     select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs" +
            "     inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ExtraTrialShip ets on ets.BillingID = b.BillingID"+
            "     inner join ExtraTrialShipSale sl on sl.ExtraTrialShipID = ets.ExtraTrialShipID"+
            "     inner join ReturnedSale rs on rs.SaleID = sl.SaleID"+
            "     inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            "     inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            "     where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            "     limit 100"+
			" "+
            "     union all"+
            " "+
            "     select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs			" +
            "     inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            "     inner join UpsellSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            "     inner join ReturnedSale rs on rs.SaleID = sl.SaleID"+
            "     inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            "     inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            "     where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            "     limit 100"+
            "         "+
            "     union all"+
            "         "+
            "     select b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, bs.CreateDT, b.Affiliate, b.SubAffiliate, 0 as StatusTID, st.DisplayName as StatusName from BillingSubscription bs			" +
            "     inner join StatusType st on st.StatusTypeID = bs.StatusTID" +
            "     inner join Billing b on bs.BillingID = b.BillingID" +
            "     inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID"+
            "     inner join BillingSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID"+
            "     inner join ReturnedSale rs on rs.SaleID = sl.SaleID"+
            "     inner join (select SaleID, 2 as ShipperID, Completed from TSNRecord where RegID is not null union select SaleID, 4 as ShipperID, Completed from ABFRecord where RegID is not null union select SaleID, 5, Completed as ShipperID from KeymailRecord where RegID is not null) ship on ship.SaleID = sl.SaleID and ship.Completed = 1"+
            "     inner join Shipper sh on sh.ShipperID = ship.ShipperID"+
            "     where bs.CreateDT between @start and @end" + AffiliateFilter("b", affiliateFilter) +
            "     limit 100"+
            "         " +
            "     union all" +
            "         " +
            "     select distinct b.BillingID, concat(b.FirstName, ' ', b.LastName) as Name, b.CreateDT, b.Affiliate, b.SubAffiliate, coalesce(case when min(orp.RecurringStatus) is null and o.Scrub = 1 then 5 else min(orp.RecurringStatus) end, -1) as StatusTID, 'RecurringStatus' as StatusName from Billing b" +
            "     inner join Orders firstOrder on firstOrder.OrderID = b.FirstOrderID" +
            "     inner join Orders o on o.BillingID = b.BillingID" +
            "     join OrderSale os2 on os2.OrderID = o.OrderID" +
            "     left join OrderRecurringPlan orp on orp.SaleID = os2.SaleID and orp.RecurringStatus > 0" +
            "     inner join OrderSale sl on sl.OrderID = o.OrderID" +
            "     inner join Shipment sh on sh.SaleID = sl.SaleID" +
            "     inner join Shipper shp on shp.ShipperID = sh.ShipperID" +
            "     where sh.ShipmentStatus >= 30" +
            "     and firstOrder.CreateDT between @start and @end and b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'" + AffiliateFilter("b", affiliateFilter) +
            "     group by b.BillingID" +
            "     limit 100" +
            " ) t" +
            " limit 100";

        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = query[mode - 1].ToString();

            command.Parameters.AddWithValue("@affiliate", affiliate);
            command.Parameters.AddWithValue("@start", startDate.ToString("yyyy/MM/dd") + " 00:00:00");
            command.Parameters.AddWithValue("@end", endDate.ToString("yyyy/MM/dd") + " 23:59:59");

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                JsonRow row = new JsonRow();

                row.BillingID = Convert.ToInt32(reader["BillingID"].ToString());
                row.Name = reader["Name"].ToString();
                row.SignupDate = reader["CreateDT"].ToString();
                row.Affiliate = reader["Affiliate"].ToString();
                row.SubAffiliate = reader["SubAffiliate"].ToString();
                //old/new subscriptions structure
                if (reader["StatusName"].ToString() == "RecurringStatus")
                {
                    int statusTID = Convert.ToInt32(reader["StatusTID"]);
                    if (statusTID == -1)
                    {
                        row.Status = "Single Order";
                    }
                    else
                    {
                        try
                        {
                            row.Status = TrimFuel.Model.Enums.RecurringStatusEnum.Name[statusTID];
                        }
                        catch
                        {
                            row.Status = "Can't determine status";
                        }
                    }
                }
                else
                {
                    row.Status = reader["StatusName"].ToString();
                }                

                rows.Add(row);
            }
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)
                command.Dispose();
        }

        return rows;
    }

    private static string GetJsonForSeries(string query, string affiliate, string start, string end, string name)
    {
        StringBuilder json = new StringBuilder();

        json.AppendFormat("\"{0}\": [", name);

        DateTime startDate = DateTime.Parse(start);
        DateTime endDate = DateTime.Parse(end);

        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = query.ToString();

            command.Parameters.AddWithValue("@affiliate", affiliate);
            command.Parameters.AddWithValue("@start", startDate.ToString("yyyy/MM/dd") + " 00:00:00");
            command.Parameters.AddWithValue("@end", endDate.ToString("yyyy/MM/dd") + " 23:59:59");

            MySqlDataReader reader = command.ExecuteReader();

            int i = 0;


            while (reader.Read())
            {
                if (i != 0)
                    json.Append(", ");
                json.AppendFormat("[\"{0}\"]", reader["Date"].ToString());
                json.Append(", ");
                json.AppendFormat("[{0}]", reader["Value"].ToString());
                // DateTime dt = new DateTime();

                //  dt = DateTime.Parse(reader["Date"].ToString());
                // json.AppendFormat("[\"{0}\", {1}]", reader["Date"].ToString(), reader["Value"].ToString());

                i++;
            }
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)
                command.Dispose();
        }

        json.Append("]");

        return json.ToString();
    }

    public class JsonRow
    {
        public int BillingID { get; set; }
        public string Name { get; set; }
        public string SignupDate { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public string Status { get; set; }
    }

    private static string AffiliateFilter(string alias, bool filter)
    {
        if (!filter)
            return string.Empty;

        return " and IfNull(" + alias + ".Affiliate, '') = @affiliate";
    }
}