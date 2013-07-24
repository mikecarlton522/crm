using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Services;
using TrimFuel.Model.Containers;
using TrimFuel.Business;
using TrimFuel.Model;
using System.Configuration;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Gateways.UspsGateway;
using TrimFuel.Web.Admin.Logic.Reports;
using System.Xml.Linq;
using System.Runtime.Serialization;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.Admin
{
    public partial class reports_chargeback : PageX
    {
        protected class CompanyInfo
        {
            public string Name { get; set; }
            public string Abbreviation { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.Params["bID"]))
                {
                    long temp = 0;
                    int intTemp = 0;
                    if (!string.IsNullOrEmpty(Request.Params["scID"]) && !string.IsNullOrEmpty(Request.Params["chID"]) && int.TryParse(Request.Params["scID"], out intTemp) && long.TryParse(Request.Params["chID"], out temp))
                    {
                        SendReport(Convert.ToInt32(Request.Params["bID"]), temp, intTemp);
                    }
                    else
                    {
                        SendReport(Convert.ToInt32(Request.Params["bID"]), null, null);
                    }
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            MySqlConnection connection;
            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                MySqlDataAdapter da = new MySqlDataAdapter("select cbs.* from ChargebackStatusType cbs order by cbs.ChargebackStatusTypeID", connection);
                DataSet ds = new DataSet();
                da.Fill(ds);

                ddlStatus.DataSource = ds;
                ddlStatus.DataTextField = "DisplayName";
                ddlStatus.DataValueField = "ChargebackStatusTypeID";

                connection.Close();
            }
            catch (Exception)
            {

            }
        }

        protected void ddlStatus_DataBound(object sender, EventArgs e)
        {
            ddlStatus.Items.Insert(0, new ListItem("Unknown Status", "0"));
            ddlStatus.Items.Insert(0, new ListItem("-- All --", ""));
        }

        [WebMethod]
        public static void RemoveUser(int billingId)
        {
            MySqlConnection connection;
            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                var command = new MySqlCommand();
                command.CommandText = "INSERT INTO ChargebackReportComplete (BillingID) VALUES (@BId)";
                command.Connection = connection;
                command.Prepare();
                command.Parameters.AddWithValue("@BId", billingId);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {

            }
        }

        public void SendReport(int billingId, long? chargeHistoryID, int? saleChargebackID)
        {
            try 
	        {
                bool newStructure = false;
                if (chargeHistoryID != null && saleChargebackID != null)
                {
                    BaseService service = new BaseService();
                    ChargeHistoryEx ch = service.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch != null && ch.BillingSubscriptionID == null)
                    {
                        newStructure = true;
                    }
                }

                if (newStructure)
                {
                    SendReport(Config.Current.APPLICATION_ID, chargeHistoryID.Value, saleChargebackID.Value);
                }
                else
                {
                    switch (Config.Current.APPLICATION_ID)
                    {
                        case "trimfuel.localhost":
                        case "teststo.trianglecrm.com":
                        case "dashboard.trianglecrm.com":
                        case "dashboard.trianglemediacorp.com":
                            SendReportSTO(billingId, chargeHistoryID, saleChargebackID);
                            break;
                        case "metabolab.trianglecrm.com":
                            SendReportMetabolab(billingId, chargeHistoryID, saleChargebackID);
                            break;
                        case "coaction.trianglecrm.com":
                            SendReportCoaction(billingId, chargeHistoryID, saleChargebackID);
                            break;
                        case "localhost":
                        case "jmb.trianglecrm.com":
                            SendReportJMB(billingId, chargeHistoryID, saleChargebackID);
                            break;
                        case "apex.trianglecrm.com":
                        case "client1.localhost":
                            SendReportApex(billingId, chargeHistoryID, saleChargebackID);
                            break;
                        default:
                            break;
                    }
                }
	        }
	        catch (Exception ex)
	        {
                HttpContext.Current.Response.Write(ex.ToString());
	        }
        }

        protected CompanyInfo DetermineCompany(string applicationId, int assertigyMIDID)
        {
            CompanyInfo res = null;
            try
            {
                var cmp = new MerchantService().GetNMICompanyByAssertigyMID(assertigyMIDID);
                if (cmp == null)
                {
                    throw new Exception(string.Format("Can't determine Company for MID({0})", assertigyMIDID));
                }

                res = new CompanyInfo();
                res.Name = cmp.CompanyName;

                switch (applicationId)
                {
                    case "trimfuel.localhost":
                    case "client1.localhost":
                    case "dashboard.trianglecrm.com":
                    case "dashboard.trianglemediacorp.com":
                        switch (res.Name)
                        {
                            case "Great Plains Nutrition":
                                res.Abbreviation = "GPN";
                                res.Address1 = "11625 Custer Road, #170";
                                res.Address2 = "Frisco, TX 75035";
                                break;
                            case "Rivers Edge Marketing":
                                res.Abbreviation = "RED";
                                res.Address1 = "One Oxford Center, 301 Grant Street #4300";
                                res.Address2 = "Pittsburgh, PA 15219";
                                break;
                            case "Vital Global Marketing":
                                res.Abbreviation = "VGM";
                                res.Address1 = "3960 Howard Hughes Parkway, #500";
                                res.Address2 = "Las Vegas, NV 89169";
                                break;
                            case "Vitality & Wellness":
                                res.Abbreviation = "V&W";
                                res.Address1 = "88 Wood Street, 10th - 15th Floor";
                                res.Address2 = "London, EC2V 7RS, UK";
                                break;
                            case "Green Valley Wellness":
                                res.Abbreviation = "GVW";
                                res.Address1 = "40 North Central Ave., #1400";
                                res.Address2 = "Phoenix, Arizona 85004";
                                break;
                        }
                        break;
                    case "metabolab.trianglecrm.com":
                        switch (res.Name)
                        {
                            case "Metabolab":
                                res.Name = "FitDiet";
                                res.Abbreviation = "FITDIET";
                                res.Address1 = "269 S. Beverly Drive, Ste 1344";
                                res.Address2 = "Beverly Hills, CA 90212";
                                break;
                        }
                        break;
                    case "coaction.trianglecrm.com":
                        switch (res.Name)
                        {
                            case "CoAction":
                                res.Name = "CoAction Media";
                                res.Abbreviation = "CoAction";
                                res.Address1 = "Coaction Media, LLC.\n1035 Pearl St. Suite 226\nBoulder, CO 80302";
                                break;
                            case "Zulu Digital":
                                res.Name = "Zulu Digital LLC.\n4580 Broadway #227\nBoulder, CO 80304";
                                res.Abbreviation = "ZuluDigital";
                                res.Address1 = "One Oxford Center, 301 Grant Street #4300";
                                res.Address2 = "Pittsburgh, PA 15219";
                                break;
                        }
                        break;
                    case "localhost":
                    case "jmb.trianglecrm.com":
                        switch (res.Name)
                        {
                            case "JMB":
                                res.Abbreviation = "JMB";
                                res.Address1 = "";
                                res.Address2 = "";
                                break;
                        }
                        break;
                    case "apex.trianglecrm.com":
                        res.Name = "Apex Marketing LLC";
                        res.Abbreviation = "APEX";
                        res.Address1 = "333 7th Avenue, 3rd Floor";
                        res.Address2 = "New York, NY 10001";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
            return res;
        }

        protected void SendReport(string applicationId, long chargeHistoryID, int saleChargebackID)
        {
            try
            {
                //find sale
                OrderService service = new OrderService();
                
                SaleChargeback slChb = service.Load<SaleChargeback>(saleChargebackID);
                OrderSaleView2 sale = service.LoadSale(slChb.SaleID.Value);
                if (sale == null)
                {
                    throw new Exception(string.Format("Can't find OrderSale({0})", slChb.SaleID));
                }

                ChargeHistoryView ch = sale.InvoiceChargeList.Where(i => i.CurrencyAmount > 0M).FirstOrDefault();
                if (ch == null)
                {
                    throw new Exception(string.Format("Can't find Charge for Sale({0})", slChb.SaleID));
                }
                if (ch.ChargeHistory.ChargeHistoryID != chargeHistoryID)
                {
                    throw new Exception(string.Format("Invalid Sale({0}) Charge: {1} doesn't match to {2}", slChb.SaleID, chargeHistoryID, ch.ChargeHistory.ChargeHistoryID));
                }

                string trackNum = sale.SaleView.ShipmentList.Select(i => i.Shipment.TrackingNumber).Where(i => !string.IsNullOrEmpty(i)).FirstOrDefault();
                string chCurrency = GetDefaultCurrency();
                if (ch.Currency != null)
                {
                    chCurrency = ch.Currency.CurrencyName;
                }

                //find trial sale
                OrderSaleView2 trialSale = null;
                ChargeHistoryView trialCh = null;
                string trialTrackNum = null;
                string trialChCurrency = string.Empty;
                if (sale.SaleView.OrderSale.SaleType == OrderSaleTypeEnum.Rebill)
                {
                    OrderRecurringPlan orp = service.Load<OrderRecurringPlan>((sale.SaleView.OrderSale as RecurringSale).OrderRecurringPlanID);

                    trialSale = service.LoadSale(orp.SaleID.Value);
                    if (trialSale == null)
                    {
                        throw new Exception(string.Format("Can't find Trial for OrderSale({0})", slChb.SaleID));
                    }

                    trialCh = trialSale.InvoiceChargeList.Where(i => i.CurrencyAmount > 0M).FirstOrDefault();
                    //trial is free
                    if (trialCh == null)
                    {
                        trialSale = null;
                    }
                    
                    if (trialSale != null)
                    {
                        trialTrackNum = trialSale.SaleView.ShipmentList.Select(i => i.Shipment.TrackingNumber).Where(i => !string.IsNullOrEmpty(i)).FirstOrDefault();

                        trialChCurrency = GetDefaultCurrency();
                        if (trialCh.Currency != null)
                        {
                            trialChCurrency = trialCh.Currency.CurrencyName;
                        }
                    }

                }

                CompanyInfo company = DetermineCompany(applicationId, ch.ChargeHistory.MerchantAccountID.Value);

                Billing billing = service.Load<Billing>(sale.SaleView.Order.Order.BillingID);

                Response.ClearHeaders();
                Response.ClearContent();
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";                
                switch (applicationId)
                {
                    case "trimfuel.localhost":
                    case "client1.localhost":
                    case "dashboard.trianglecrm.com":
                    case "dashboard.trianglemediacorp.com":
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", company.Abbreviation, slChb.CaseNumber));
                        using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                        {
                            new SaleChargebackReport(slChb.CaseNumber, slChb.ARN, company.Name, company.Address1, company.Address2,
                                ch.ChargeHistory.MerchantAccountID.ToString(), ch.ChargeHistory, (trialCh != null ? trialCh.ChargeHistory : null), 
                                billing,
                                ch.CurrencyAmount.Value, chCurrency, (trialCh != null ? trialCh.CurrencyAmount.Value : 0M), 
                                trialChCurrency, trialTrackNum, trackNum)
                                .Create(mem);
                            mem.WriteTo(HttpContext.Current.Response.OutputStream);
                        }
                        break;
                    case "metabolab.trianglecrm.com":
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", company.Abbreviation, slChb.CaseNumber));
                        using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                        {
                            new TrimFuel.Web.Admin.Logic.Reports.Metabolab.SaleChargebackReport(slChb.CaseNumber, slChb.ARN, company.Name, company.Address1, company.Address2, 
                                ch.ChargeHistory.MerchantAccountID.ToString(), ch.ChargeHistory, 
                                billing, 
                                ch.CurrencyAmount.Value, chCurrency)
                                .Create(mem);
                            mem.WriteTo(HttpContext.Current.Response.OutputStream);
                        }
                        break;
                    case "coaction.trianglecrm.com":
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", company.Abbreviation, slChb.CaseNumber));
                        using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                        {
                            new TrimFuel.Web.Admin.Logic.Reports.CoAction.SaleChargebackReport(slChb.CaseNumber, slChb.ARN, company.Name, company.Address1,
                                ch.ChargeHistory.MerchantAccountID.ToString(), ch.ChargeHistory, (trialCh != null ? trialCh.ChargeHistory : null), 
                                billing,
                                ch.CurrencyAmount.Value, chCurrency, (trialCh != null ? trialCh.CurrencyAmount.Value : 0M), 
                                trialChCurrency, trialTrackNum, trackNum)
                                .Create(mem);
                            mem.WriteTo(HttpContext.Current.Response.OutputStream);
                        }
                        break;
                    case "localhost":
                    case "jmb.trianglecrm.com":
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", company.Abbreviation, slChb.CaseNumber));
                        BillingExternalInfo bei = service.Load<BillingExternalInfo>(billing.BillingID);
                        using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                        {
                            new TrimFuel.Web.Admin.Logic.Reports.JMB.SaleChargebackReport2(slChb.CaseNumber, slChb.ARN, company.Name, company.Address1, company.Address2, 
                                ch.ChargeHistory.MerchantAccountID.ToString(), ch.ChargeHistory, 
                                billing, (bei != null ? bei.InternalID : null))
                                .Create(mem);
                            mem.WriteTo(HttpContext.Current.Response.OutputStream);
                        }
                        break;
                    case "apex.trianglecrm.com":
                        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", "Apex", slChb.CaseNumber));
//                        throw new Exception(string.Format(@"SaleChargebackReport({0}, {1}, {2}, {3}, {4}, 
//                                {5},
//                                {6}, {7}, {8}, 
//                                {9},
//                                {10}, {11}, {12}, {13})
//                                .Create(mem);", slChb.CaseNumber, slChb.ARN, company.Name, company.Address1, company.Address2,
//                                sale.OrderProduct,
//                                ch.ChargeHistory, ch.ChargeHistory, trialCh.ChargeHistory,
//                                billing,
//                                ch.CurrencyAmount.Value, chCurrency, trialCh.CurrencyAmount.Value, trialChCurrency));

                        using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                        {
                            new TrimFuel.Web.Admin.Logic.Reports.Apex.SaleChargebackReport(slChb.CaseNumber, slChb.ARN, company.Name, company.Address1, company.Address2, 
                                sale.OrderProduct.ProductID.Value,
                                ch.ChargeHistory.MerchantAccountID.ToString(), ch.ChargeHistory, (trialCh != null ? trialCh.ChargeHistory : null), 
                                billing,
                                ch.CurrencyAmount.Value, chCurrency, (trialCh != null ? trialCh.CurrencyAmount.Value : 0M), trialChCurrency)
                                .Create(mem);
                            mem.WriteTo(HttpContext.Current.Response.OutputStream);
                        }
                        break;
                    default:
                        break;
                }
                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        protected void SendReportSTO(int billingId, long? chargeHistoryID, int? saleChargebackID)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();
                var command = new MySqlCommand
                {
                    CommandText = "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "INNER JOIN BillingSubscription bs ON c.BillingSubscriptionID = bs.BillingSubscriptionID " +
                                  "WHERE bs.BillingID = " + billingId + " " +
                                  "AND c.Success = 1 ORDER BY ChargeHistoryID DESC LIMIT 1",
                    Connection = connection
                };
                if (chargeHistoryID != null)
                {
                    command.CommandText =
                                  "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "WHERE c.ChargeHistoryID = " + chargeHistoryID;
                }

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                var companyName = string.Empty;
                var companyAbbr = string.Empty;
                var companyAddress1 = string.Empty;
                var companyAddress2 = string.Empty;
                var assertigyMIDID = string.Empty;
                var chCurrency = string.Empty;
                decimal chAmount = 0;
                var trialChCurrency = string.Empty;
                decimal trialChAmount = 0;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    assertigyMIDID = row.ItemArray[1].ToString();
                    switch (row.ItemArray[0].ToString().Trim())
                    {
                        case "Great Plains Nutrition":
                            companyName = "Great Plains Nutrition";
                            companyAbbr = "GPN";
                            companyAddress1 = "11625 Custer Road, #170";
                            companyAddress2 = "Frisco, TX 75035";
                            break;
                        case "Rivers Edge Marketing":
                            companyName = "Rivers Edge Marketing";
                            companyAbbr = "RED";
                            companyAddress1 = "One Oxford Center, 301 Grant Street #4300";
                            companyAddress2 = "Pittsburgh, PA 15219";
                            break;
                        case "Vital Global Marketing":
                            companyName = "Vital Global Marketing";
                            companyAbbr = "VGM";
                            companyAddress1 = "3960 Howard Hughes Parkway, #500";
                            companyAddress2 = "Las Vegas, NV 89169";
                            break;
                        case "Vitality & Wellness":
                            companyName = "Vitality & Wellness";
                            companyAbbr = "V&W";
                            companyAddress1 = "88 Wood Street, 10th - 15th Floor";
                            companyAddress2 = "London, EC2V 7RS, UK";
                            break;
                        case "Green Valley Wellness":
                            companyName = "Green Valley Wellness";
                            companyAbbr = "GVW";
                            companyAddress1 = "40 North Central Ave., #1400";
                            companyAddress2 = "Phoenix, Arizona 85004";
                            break;
                    }
                }

                Response.ClearHeaders();
                Response.ClearContent();
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                if (chargeHistoryID == null)
                {
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", companyAbbr, billingId));

                    BillingChargebackReport report = new BillingChargebackReport(null, companyName, companyAddress1, companyAddress2, assertigyMIDID);
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                    {
                        report.Create(mem);
                        mem.WriteTo(HttpContext.Current.Response.OutputStream);
                    }
                }
                else if (saleChargebackID != null)
                {
                    string caseNumber = null;
                    string arnNumber = null;
                    string trialTrackingNumber = null;
                    string rebillTrackingNumber = null;
                    command = new MySqlCommand()
                    {
                        CommandText = "select sc.CaseNumber, sc.ARN, sc.SaleID from SaleChargeback sc where sc.SaleChargebackID = " + saleChargebackID.ToString(),
                        Connection = connection                        
                    };

                    adapter.SelectCommand = command;
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    long saleID = 0;

                    caseNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["CaseNumber"]);
                    arnNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["ARN"]);
                    if (string.IsNullOrEmpty(caseNumber))
                    {
                        caseNumber = null;
                    }
                    if (string.IsNullOrEmpty(arnNumber))
                    {
                        arnNumber = null;
                    }
                    saleID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["SaleID"]);

                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", companyAbbr, caseNumber));

                    BaseService service = new BaseService();
                    ChargeHistoryEx ch = service.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch == null)
                    {
                        throw new Exception(string.Format("ChargeHistoryEx({0}) was not found", chargeHistoryID));
                    }
                    BillingSubscription bs = service.Load<BillingSubscription>(ch.BillingSubscriptionID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("BillingSubscription({0}) was not found", ch.BillingSubscriptionID));
                    }
                    Billing b = service.Load<Billing>(bs.BillingID);
                    if (b == null)
                    {
                        throw new Exception(string.Format("Billing({0}) was not found", bs.BillingID));
                    }

                    // Retrieve chargeHistory amount and the specific currency
                    GetChargeHistoryAmountAndCurrency(service, ch, out chAmount, out chCurrency);

                    ChargeHistoryEx trialCh = null;
                    BillingSale sale = service.Load<BillingSale>(saleID);
                    if (sale != null && sale.SaleTypeID == 1)
                    {
                        //Trial, no need to serach trial

                       
                    }
                    else
                    {
                        //Search trial
                        MySqlCommand cmd = new MySqlCommand("select ch.* from ChargeHistoryEx ch " +
                            "inner join BillingSale bsale on bsale.ChargeHistoryID = ch.ChargeHistoryID " +
                            "inner join Sale s on s.SaleID = bsale.SaleID " +
                            "where ch.BillingSubscriptionID = @billingSubscriptionID " +
                            "and ch.ChargeHistoryID <> @chargeHistoryID " +
                            "and ch.Success = 1 " +
                            "and ch.Amount > 0 " +
                            "and s.SaleTypeID = 1 " +
                            "order by ch.ChargeHistoryID desc " +
                            "limit 1");
                        cmd.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int32).Value = ch.BillingSubscriptionID;
                        cmd.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;

                        IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                        trialCh = dao.Load<ChargeHistoryEx>(cmd).FirstOrDefault();

                        //try search by phone
                        // >=7 condition to avoid seach by phones like 123, 1, ...
                        if (trialCh == null && !string.IsNullOrEmpty(b.Phone) && b.Phone.Length >= 7)
                        {
                            cmd = new MySqlCommand("select ch.* from ChargeHistoryEx ch " +
                                "inner join BillingSale bsale on bsale.ChargeHistoryID = ch.ChargeHistoryID " +
                                "inner join Sale s on s.SaleID = bsale.SaleID " +
                                "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                                "inner join Billing b on b.BillingID = bs.BillingID " +
                                "where b.Phone = @phone " +
                                "and ch.ChargeHistoryID <> @chargeHistoryID " +
                                "and ch.Success = 1 " +
                                "and ch.Amount > 0 " +
                                "and s.SaleTypeID = 1 " +
                                "order by ch.ChargeHistoryID desc " +
                                "limit 1");
                            cmd.Parameters.Add("@phone", MySqlDbType.VarChar).Value = b.Phone;
                            cmd.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;

                            trialCh = dao.Load<ChargeHistoryEx>(cmd).FirstOrDefault();

                            // Retrieve trial chargeHistory amount and the specific currency
                            GetChargeHistoryAmountAndCurrency(service, trialCh, out trialChAmount, out trialChCurrency);
                        }
                    }

                    trialTrackingNumber = GetTrackingNumberByChargeHistoryID(ch, true);
                    rebillTrackingNumber = GetTrackingNumberByChargeHistoryID(trialCh, false);

                    SaleChargebackReport report = new SaleChargebackReport(caseNumber, arnNumber, companyName, companyAddress1, companyAddress2, assertigyMIDID, 
                        ch, trialCh, b, chAmount, chCurrency, trialChAmount, trialChCurrency, trialTrackingNumber, rebillTrackingNumber);
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                    {
                        report.Create(mem);
                        mem.WriteTo(HttpContext.Current.Response.OutputStream);
                    }
                }

                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        protected void SendReportCoaction(int billingId, long? chargeHistoryID, int? saleChargebackID)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();
                var command = new MySqlCommand
                {
                    CommandText = "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "INNER JOIN BillingSubscription bs ON c.BillingSubscriptionID = bs.BillingSubscriptionID " +
                                  "WHERE bs.BillingID = " + billingId + " " +
                                  "AND c.Success = 1 ORDER BY ChargeHistoryID DESC LIMIT 1",
                    Connection = connection
                };
                if (chargeHistoryID != null)
                {
                    command.CommandText =
                                  "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "WHERE c.ChargeHistoryID = " + chargeHistoryID;
                }

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                var companyName = string.Empty;
                var companyAbbr = string.Empty;
                var companyAddress1 = string.Empty;
                var companyAddress2 = string.Empty;
                var assertigyMIDID = string.Empty;
                var chCurrency = string.Empty;
                decimal chAmount = 0;
                var trialChCurrency = string.Empty;
                decimal trialChAmount = 0;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    assertigyMIDID = row.ItemArray[1].ToString();
                    switch (row.ItemArray[0].ToString().Trim())
                    {
                        case "CoAction":
                            companyName = "CoAction Media";
                            companyAbbr = "CoAction";
                            companyAddress1 = "Coaction Media, LLC.\n1035 Pearl St. Suite 226\nBoulder, CO 80302";
                            break;
                        case "Zulu Digital":
                            companyName = "Zulu Digital LLC.\n4580 Broadway #227\nBoulder, CO 80304";
                            companyAbbr = "ZuluDigital";
                            companyAddress1 = "One Oxford Center, 301 Grant Street #4300";
                            companyAddress2 = "Pittsburgh, PA 15219";
                            break;
                    }
                }

                Response.ClearHeaders();
                Response.ClearContent();
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                if (chargeHistoryID == null)
                {
                }
                else if (saleChargebackID != null)
                {
                    string caseNumber = null;
                    string arnNumber = null;
                    string trialTrackingNumber = null;
                    string rebillTrackingNumber = null;
                    command = new MySqlCommand()
                    {
                        CommandText = "select sc.CaseNumber, sc.ARN, sc.SaleID from SaleChargeback sc where sc.SaleChargebackID = " + saleChargebackID.ToString(),
                        Connection = connection
                    };

                    adapter.SelectCommand = command;
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    long saleID = 0;

                    caseNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["CaseNumber"]);
                    arnNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["ARN"]);
                    if (string.IsNullOrEmpty(caseNumber))
                    {
                        caseNumber = null;
                    }
                    if (string.IsNullOrEmpty(arnNumber))
                    {
                        arnNumber = null;
                    }
                    saleID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["SaleID"]);

                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", companyAbbr, caseNumber));

                    BaseService service = new BaseService();
                    ChargeHistoryEx ch = service.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch == null)
                    {
                        throw new Exception(string.Format("ChargeHistoryEx({0}) was not found", chargeHistoryID));
                    }
                    BillingSubscription bs = service.Load<BillingSubscription>(ch.BillingSubscriptionID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("BillingSubscription({0}) was not found", ch.BillingSubscriptionID));
                    }
                    Billing b = service.Load<Billing>(bs.BillingID);
                    if (b == null)
                    {
                        throw new Exception(string.Format("Billing({0}) was not found", bs.BillingID));
                    }

                    // Retrieve chargeHistory amount and the specific currency
                    GetChargeHistoryAmountAndCurrency(service, ch, out chAmount, out chCurrency);

                    ChargeHistoryEx trialCh = null;
                    BillingSale sale = service.Load<BillingSale>(saleID);
                    if (sale != null && sale.SaleTypeID == 1)
                    {
                        //Trial, no need to serach trial
                    }
                    else
                    {
                        //Search trial (can be BillingSale with SaleTypeID = 1 as well as UpsellSale)
                        MySqlCommand cmd = new MySqlCommand(@"
                            select ch.* from ChargeHistoryEx ch
                            inner join (
                                select min(ChargeHistoryID) as ChargeHistoryID from ChargeHistoryEx
                                where BillingSubscriptionID = @billingSubscriptionID
                                and Success = 1 and Amount > 0
                            ) chFirst on chFirst.ChargeHistoryID = ch.ChargeHistoryID
                            inner join (
                                select BillingSale.ChargeHistoryID from BillingSale
                                inner join Sale on Sale.SaleID = BillingSale.SaleID
                                inner join ChargeHistoryEx on ChargeHistoryEx.ChargeHistoryID = BillingSale.ChargeHistoryID
                                where Sale.SaleTypeID = 1 and ChargeHistoryEx.BillingSubscriptionID = @billingSubscriptionID
                                union
                                select UpsellSale.ChargeHistoryID from UpsellSale
                                inner join ChargeHistoryEx on ChargeHistoryEx.ChargeHistoryID = UpsellSale.ChargeHistoryID
                                where ChargeHistoryEx.BillingSubscriptionID = @billingSubscriptionID
                            ) trialSale on trialSale.ChargeHistoryID = ch.ChargeHistoryID
                            where ch.BillingSubscriptionID = @billingSubscriptionID
                            and ch.ChargeHistoryID <> @chargeHistoryID
                            and ch.Success = 1 and ch.Amount > 0
                            order by ch.ChargeHistoryID asc
                            limit 1");
                        cmd.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int32).Value = ch.BillingSubscriptionID;
                        cmd.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;

                        IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                        trialCh = dao.Load<ChargeHistoryEx>(cmd).FirstOrDefault();

                        // Retrieve trial chargeHistory amount and the specific currency
                        GetChargeHistoryAmountAndCurrency(service, trialCh, out trialChAmount, out trialChCurrency);
                    }

                    trialTrackingNumber = GetTrackingNumberByChargeHistoryID(ch, true);
                    rebillTrackingNumber = GetTrackingNumberByChargeHistoryID(trialCh, false);

                    TrimFuel.Web.Admin.Logic.Reports.CoAction.SaleChargebackReport report = new TrimFuel.Web.Admin.Logic.Reports.CoAction.SaleChargebackReport(caseNumber, arnNumber, companyName, companyAddress1,
                        assertigyMIDID, ch, trialCh, b, chAmount, chCurrency, trialChAmount, trialChCurrency, trialTrackingNumber, rebillTrackingNumber);
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                    {
                        report.Create(mem);
                        mem.WriteTo(HttpContext.Current.Response.OutputStream);
                    }
                }

                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        protected void SendReportMetabolab(int billingId, long? chargeHistoryID, int? saleChargebackID)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();
                var command = new MySqlCommand
                {
                    CommandText = "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "INNER JOIN BillingSubscription bs ON c.BillingSubscriptionID = bs.BillingSubscriptionID " +
                                  "WHERE bs.BillingID = " + billingId + " " +
                                  "AND c.Success = 1 ORDER BY ChargeHistoryID DESC LIMIT 1",
                    Connection = connection
                };
                if (chargeHistoryID != null)
                {
                    command.CommandText =
                                  "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "WHERE c.ChargeHistoryID = " + chargeHistoryID;
                }

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                var companyName = string.Empty;
                var companyAbbr = string.Empty;
                var companyAddress1 = string.Empty;
                var companyAddress2 = string.Empty;
                var assertigyMIDID = string.Empty;
                var chCurrency = string.Empty;
                decimal chAmount = 0;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    assertigyMIDID = row.ItemArray[1].ToString();
                    switch (row.ItemArray[0].ToString().Trim())
                    {
                        case "Metabolab":
                            companyName = "FitDiet";
                            companyAbbr = "FITDIET";
                            companyAddress1 = "269 S. Beverly Drive, Ste 1344";
                            companyAddress2 = "Beverly Hills, CA 90212";
                            break;
                    }
                }

                if (saleChargebackID != null)
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                    string caseNumber = null;
                    string arnNumber = null;
                    command = new MySqlCommand()
                    {
                        CommandText = "select sc.CaseNumber, sc.ARN, sc.SaleID from SaleChargeback sc where sc.SaleChargebackID = " + saleChargebackID.ToString(),
                        Connection = connection
                    };

                    adapter.SelectCommand = command;
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    long saleID = 0;

                    caseNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["CaseNumber"]);
                    arnNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["ARN"]);
                    if (string.IsNullOrEmpty(caseNumber))
                    {
                        caseNumber = null;
                    }
                    if (string.IsNullOrEmpty(arnNumber))
                    {
                        arnNumber = null;
                    }
                    saleID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["SaleID"]);

                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", companyAbbr, caseNumber));

                    BaseService service = new BaseService();
                    ChargeHistoryEx ch = service.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch == null)
                    {
                        throw new Exception(string.Format("ChargeHistoryEx({0}) was not found", chargeHistoryID));
                    }
                    BillingSubscription bs = service.Load<BillingSubscription>(ch.BillingSubscriptionID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("BillingSubscription({0}) was not found", ch.BillingSubscriptionID));
                    }
                    Billing b = service.Load<Billing>(bs.BillingID);
                    if (b == null)
                    {
                        throw new Exception(string.Format("Billing({0}) was not found", bs.BillingID));
                    }

                    // Retrieve chargeHistory amount and the specific currency
                    GetChargeHistoryAmountAndCurrency(service, ch, out chAmount, out chCurrency);

                    BillingSale sale = service.Load<BillingSale>(saleID);

                    TrimFuel.Web.Admin.Logic.Reports.Metabolab.SaleChargebackReport report = new TrimFuel.Web.Admin.Logic.Reports.Metabolab.SaleChargebackReport(caseNumber, arnNumber, companyName, companyAddress1, companyAddress2,
                        assertigyMIDID, ch, b, chAmount, chCurrency);
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                    {
                        report.Create(mem);
                        mem.WriteTo(HttpContext.Current.Response.OutputStream);
                    }

                    Response.Flush();
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        protected void SendReportJMB(int billingId, long? chargeHistoryID, int? saleChargebackID)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();
                var command = new MySqlCommand
                {
                    CommandText = "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "INNER JOIN BillingSubscription bs ON c.BillingSubscriptionID = bs.BillingSubscriptionID " +
                                  "WHERE bs.BillingID = " + billingId + " " +
                                  "AND c.Success = 1 ORDER BY ChargeHistoryID DESC LIMIT 1",
                    Connection = connection
                };
                if (chargeHistoryID != null)
                {
                    command.CommandText =
                                  "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "WHERE c.ChargeHistoryID = " + chargeHistoryID;
                }

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                var companyName = string.Empty;
                var companyAbbr = string.Empty;
                var companyAddress1 = string.Empty;
                var companyAddress2 = string.Empty;
                var assertigyMIDID = string.Empty;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    assertigyMIDID = row.ItemArray[1].ToString();
                    switch (row.ItemArray[0].ToString().Trim())
                    {
                        case "JMB":
                            companyName = "JMB";
                            companyAbbr = "JMB";
                            companyAddress1 = "";
                            companyAddress2 = "";
                            break;
                    }
                }

                if (saleChargebackID != null)
                {
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                    string caseNumber = null;
                    string arnNumber = null;
                    command = new MySqlCommand()
                    {
                        CommandText = "select sc.CaseNumber, sc.ARN, sc.SaleID from SaleChargeback sc where sc.SaleChargebackID = " + saleChargebackID.ToString(),
                        Connection = connection
                    };

                    adapter.SelectCommand = command;
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    long saleID = 0;

                    caseNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["CaseNumber"]);
                    arnNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["ARN"]);
                    if (string.IsNullOrEmpty(caseNumber))
                    {
                        caseNumber = null;
                    }
                    if (string.IsNullOrEmpty(arnNumber))
                    {
                        arnNumber = null;
                    }
                    saleID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["SaleID"]);

                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", companyAbbr, caseNumber));

                    BaseService service = new BaseService();
                    ChargeHistoryEx ch = service.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch == null)
                    {
                        throw new Exception(string.Format("ChargeHistoryEx({0}) was not found", chargeHistoryID));
                    }
                    BillingSubscription bs = service.Load<BillingSubscription>(ch.BillingSubscriptionID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("BillingSubscription({0}) was not found", ch.BillingSubscriptionID));
                    }
                    Billing b = service.Load<Billing>(bs.BillingID);
                    if (b == null)
                    {
                        throw new Exception(string.Format("Billing({0}) was not found", bs.BillingID));
                    }

                    BillingSale sale = service.Load<BillingSale>(saleID);

                    BillingExternalInfo bei = service.Load<BillingExternalInfo>(billingId);



                    TrimFuel.Web.Admin.Logic.Reports.JMB.SaleChargebackReport2 report = new TrimFuel.Web.Admin.Logic.Reports.JMB.SaleChargebackReport2(caseNumber, arnNumber, companyName, companyAddress1, companyAddress2,
                        assertigyMIDID, ch, b, (bei != null ? bei.InternalID : null));
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                    {
                        report.Create(mem);
                        mem.WriteTo(HttpContext.Current.Response.OutputStream);
                    }

                    Response.Flush();
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        protected void SendReportApex(int billingId, long? chargeHistoryID, int? saleChargebackID)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();
                var command = new MySqlCommand
                {
                    CommandText = "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "INNER JOIN BillingSubscription bs ON c.BillingSubscriptionID = bs.BillingSubscriptionID " +
                                  "WHERE bs.BillingID = " + billingId + " " +
                                  "AND c.Success = 1 ORDER BY ChargeHistoryID DESC LIMIT 1",
                    Connection = connection
                };
                if (chargeHistoryID != null)
                {
                    command.CommandText =
                                  "SELECT n.CompanyName, a.AssertigyMIDID " +
                                  "FROM NMICompany n " +
                                  "INNER JOIN NMICompanyMID nm ON n.NMICompanyID = nm.NMICompanyID " +
                                  "INNER JOIN AssertigyMID a ON a.AssertigyMIDID = nm.AssertigyMIDID " +
                                  "INNER JOIN ChargeHistoryEx c ON c.ChildMID = a.MID " +
                                  "WHERE c.ChargeHistoryID = " + chargeHistoryID;
                }

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                var companyName = string.Empty;
                var companyAbbr = string.Empty;
                var companyAddress1 = string.Empty;
                var companyAddress2 = string.Empty;
                var assertigyMIDID = string.Empty;
                var chCurrency = string.Empty;
                decimal chAmount = 0;
                var trialChCurrency = string.Empty;
                decimal trialChAmount = 0;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    assertigyMIDID = row.ItemArray[1].ToString();
                    //switch (row.ItemArray[0].ToString().Trim())
                    //{
                    //    case "Apex":
                    //        break;
                    //}
                }
                companyName = "Apex Marketing LLC";
                companyAbbr = "APEX";
                companyAddress1 = "333 7th Avenue, 3rd Floor";
                companyAddress2 = "New York, NY 10001";


                Response.ClearHeaders();
                Response.ClearContent();
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                if (chargeHistoryID == null)
                {
                }
                else if (saleChargebackID != null)
                {
                    string caseNumber = null;
                    string arnNumber = null;
                    command = new MySqlCommand()
                    {
                        CommandText = "select sc.CaseNumber, sc.ARN, sc.SaleID from SaleChargeback sc where sc.SaleChargebackID = " + saleChargebackID.ToString(),
                        Connection = connection
                    };

                    adapter.SelectCommand = command;
                    dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    long saleID = 0;

                    caseNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["CaseNumber"]);
                    arnNumber = Convert.ToString(dataSet.Tables[0].Rows[0]["ARN"]);
                    if (string.IsNullOrEmpty(caseNumber))
                    {
                        caseNumber = null;
                    }
                    if (string.IsNullOrEmpty(arnNumber))
                    {
                        arnNumber = null;
                    }
                    saleID = Convert.ToInt64(dataSet.Tables[0].Rows[0]["SaleID"]);

                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}.docx", "Apex", caseNumber));

                    BaseService service = new BaseService();
                    ChargeHistoryEx ch = service.Load<ChargeHistoryEx>(chargeHistoryID);
                    if (ch == null)
                    {
                        throw new Exception(string.Format("ChargeHistoryEx({0}) was not found", chargeHistoryID));
                    }
                    BillingSubscription bs = service.Load<BillingSubscription>(ch.BillingSubscriptionID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("BillingSubscription({0}) was not found", ch.BillingSubscriptionID));
                    }
                    Subscription s = service.Load<Subscription>(bs.SubscriptionID);
                    if (s == null)
                    {
                        throw new Exception(string.Format("Subscription({0}) was not found", bs.SubscriptionID));
                    }
                    Billing b = service.Load<Billing>(bs.BillingID);
                    if (b == null)
                    {
                        throw new Exception(string.Format("Billing({0}) was not found", bs.BillingID));
                    }

                    // Retrieve chargeHistory amount and the specific currency
                    GetChargeHistoryAmountAndCurrency(service, ch, out chAmount, out chCurrency);

                    ChargeHistoryEx trialCh = null;
                    BillingSale sale = service.Load<BillingSale>(saleID);
                    if (sale != null && sale.SaleTypeID == 1)
                    {
                        //Trial, no need to serach trial
                    }
                    else
                    {
                        //Search trial (can be BillingSale with SaleTypeID = 1 as well as UpsellSale)
                        MySqlCommand cmd = new MySqlCommand(@"
                            select ch.* from ChargeHistoryEx ch
                            inner join (
                                select ChargeHistoryID from ChargeHistoryEx
                                where BillingSubscriptionID = @billingSubscriptionID
                                and Success = 1 and Amount > 0
                                order by ChargeDate asc
                                limit 1
                            ) chFirst on chFirst.ChargeHistoryID = ch.ChargeHistoryID
                            inner join (
                                select BillingSale.ChargeHistoryID from BillingSale
                                inner join Sale on Sale.SaleID = BillingSale.SaleID
                                inner join ChargeHistoryEx on ChargeHistoryEx.ChargeHistoryID = BillingSale.ChargeHistoryID
                                where Sale.SaleTypeID = 1 and ChargeHistoryEx.BillingSubscriptionID = @billingSubscriptionID
                                union
                                select UpsellSale.ChargeHistoryID from UpsellSale
                                inner join ChargeHistoryEx on ChargeHistoryEx.ChargeHistoryID = UpsellSale.ChargeHistoryID
                                where ChargeHistoryEx.BillingSubscriptionID = @billingSubscriptionID
                            ) trialSale on trialSale.ChargeHistoryID = ch.ChargeHistoryID
                            where ch.BillingSubscriptionID = @billingSubscriptionID
                            and ch.ChargeHistoryID <> @chargeHistoryID
                            and ch.Success = 1 and ch.Amount > 0
                            order by ch.ChargeHistoryID asc
                            limit 1");
                        cmd.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int32).Value = ch.BillingSubscriptionID;
                        cmd.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;

                        IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                        trialCh = dao.Load<ChargeHistoryEx>(cmd).FirstOrDefault();

                        // Retrieve trial chargeHistory amount and the specific currency
                        GetChargeHistoryAmountAndCurrency(service, trialCh, out trialChAmount, out trialChCurrency);
                    }



                    TrimFuel.Web.Admin.Logic.Reports.Apex.SaleChargebackReport report = new TrimFuel.Web.Admin.Logic.Reports.Apex.SaleChargebackReport(caseNumber, arnNumber, companyName, companyAddress1, companyAddress2, s.ProductID.Value, 
                        assertigyMIDID, ch, trialCh, b, chAmount, chCurrency, trialChAmount, trialChCurrency);
                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
                    {
                        report.Create(mem);
                        mem.WriteTo(HttpContext.Current.Response.OutputStream);
                    }
                }

                Response.Flush();
                Response.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.ToString());
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();

            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                var command = new MySqlCommand
                {
                    CommandText =
                        ((ddlStatus.SelectedValue != "0") ?
                            "Select sc.SaleChargebackID, null as BillingChargebackID, ch.ChargeHistoryID, b.BillingID, ch.ChargeDate, a.DisplayName as ChildMID, CONCAT(b.FirstName,' ',b.LastName) as UserName, b.CreditCard, sc.CaseNumber, ch.Amount, cbr.Description, sc.PostDT, cst.DisplayName, sc.DisputeSentDT " +
                            "From SaleChargeback sc " +
                            "inner join BillingSale sale on sale.SaleID = sc.SaleID " +
                            "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = sale.ChargeHistoryID " +
                            "Inner Join ChargebackStatusType cst on sc.ChargebackStatusTID=cst.ChargebackStatusTypeID " +
                            "inner Join ChargebackReasonCode cbr on cbr.ChargebackReasonCodeID=sc.ChargebackReasonCodeID " +
                            "left Join AssertigyMID a on a.MID = ch.ChildMID " +
                            "Inner Join Billing b on sc.BillingID=b.BillingID " +
                            "Where " +
                            "((sc.PostDT >= '" + DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "') AND (sc.PostDT <= '" + DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "')) " +
                            ((ddlStatus.SelectedValue == "") ?
                                "AND cst.ChargebackStatusTypeID in (1, 2, 3, 4, 5, 7) " : "AND cst.ChargebackStatusTypeID = " + ddlStatus.SelectedValue + " ") +
                            "AND (b.BillingID NOT IN (SELECT BillingID FROM ChargebackReportComplete)) " +
                            " union " +
                            "Select sc.SaleChargebackID, null as BillingChargebackID, ch.ChargeHistoryID, b.BillingID, ch.ChargeDate, a.DisplayName as ChildMID, CONCAT(b.FirstName,' ',b.LastName) as UserName, b.CreditCard, sc.CaseNumber, ch.Amount, cbr.Description, sc.PostDT, cst.DisplayName, sc.DisputeSentDT " +
                            "From SaleChargeback sc " +
                            "inner join UpsellSale sale on sale.SaleID = sc.SaleID " +
                            "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = sale.ChargeHistoryID " +
                            "Inner Join ChargebackStatusType cst on sc.ChargebackStatusTID=cst.ChargebackStatusTypeID " +
                            "inner Join ChargebackReasonCode cbr on cbr.ChargebackReasonCodeID=sc.ChargebackReasonCodeID " +
                            "left Join AssertigyMID a on a.MID = ch.ChildMID " +
                            "Inner Join Billing b on sc.BillingID=b.BillingID " +
                            "Where " +
                            "((sc.PostDT >= '" + DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "') AND (sc.PostDT <= '" + DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "')) " +
                            ((ddlStatus.SelectedValue == "") ?
                                "AND cst.ChargebackStatusTypeID in (1, 2, 3, 4, 5, 7) " : "AND cst.ChargebackStatusTypeID = " + ddlStatus.SelectedValue + " ") +
                            "AND (b.BillingID NOT IN (SELECT BillingID FROM ChargebackReportComplete))" +
                            " union " +
                            "Select sc.SaleChargebackID, null as BillingChargebackID, ch.ChargeHistoryID, b.BillingID, ch.ChargeDate, a.DisplayName as ChildMID, CONCAT(b.FirstName,' ',b.LastName) as UserName, b.CreditCard, sc.CaseNumber, ch.Amount, cbr.Description, sc.PostDT, cst.DisplayName, sc.DisputeSentDT " +
                            "From SaleChargeback sc " +
                            "inner join OrderSale osl on osl.SaleID = sc.SaleID " +
                            "inner join Invoice i on i.InvoiceID = osl.InvoiceID " +
                            "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                            "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 " +
                            "Inner Join ChargebackStatusType cst on sc.ChargebackStatusTID=cst.ChargebackStatusTypeID " +
                            "inner Join ChargebackReasonCode cbr on cbr.ChargebackReasonCodeID=sc.ChargebackReasonCodeID " +
                            "left Join AssertigyMID a on a.MID = ch.ChildMID " +
                            "Inner Join Billing b on sc.BillingID=b.BillingID " +
                            "Where " +
                            "((sc.PostDT >= '" + DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "') AND (sc.PostDT <= '" + DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "')) " +
                            ((ddlStatus.SelectedValue == "") ?
                                "AND cst.ChargebackStatusTypeID in (1, 2, 3, 4, 5, 7) " : "AND cst.ChargebackStatusTypeID = " + ddlStatus.SelectedValue + " ") +
                            "AND (b.BillingID NOT IN (SELECT BillingID FROM ChargebackReportComplete))" : "") +
                        ((ddlStatus.SelectedValue == "") ?
                            " union " : "") +
                        ((ddlStatus.SelectedValue == "" || ddlStatus.SelectedValue == "0") ?
                            "select null as SaleChargebackID, bc.BillingChargebackID, null as ChargeHistoryID, b.BillingID, null as ChargeDate, null as ChildMID, CONCAT(b.FirstName, ' ', b.LastName) as UserName, b.CreditCard, null as CaseNumber, null as Amount, null as Description, bc.ChargebackDT as PostDT, 'Status Unknown' as DisplayName, null as DisputeSentDT from Billing b " +
                            "inner join BillingChargeback bc on bc.BillingID = b.BillingID " +
                            "where " +
                            "((bc.ChargebackDT >= '" + DateFilter1.Date1WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "') AND (bc.ChargebackDT <= '" + DateFilter1.Date2WithTime.ToString("yyyy-MM-dd HH:mm:ss") + "')) " : ""),
                    Connection = connection
                };

                adapter.SelectCommand = command;
                adapter.Fill(dataSet);

                GvReport.DataSource = dataSet;
                GvReport.DataBind();

                if (GvReport.HeaderRow != null)
                {
                    GvReport.HeaderRow.Cells[0].Text = "Billing ID";
                    GvReport.HeaderRow.Cells[1].Text = "Original TXN Date";
                    GvReport.HeaderRow.Cells[2].Text = "MID";
                    GvReport.HeaderRow.Cells[3].Text = "Name";
                    GvReport.HeaderRow.Cells[4].Text = "Credit Card";
                    GvReport.HeaderRow.Cells[5].Text = "Chargeback Case #";
                    GvReport.HeaderRow.Cells[6].Text = "Original TXN Amt";
                    GvReport.HeaderRow.Cells[7].Text = "Chargeback Reason";
                    GvReport.HeaderRow.Cells[8].Text = "CB Post Date";
                    GvReport.HeaderRow.Cells[9].Text = "CB Status";
                    GvReport.HeaderRow.Cells[10].Text = "Date Dispute Sent";
                    GvReport.HeaderRow.Cells[11].Text = "Tasks";
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected string ShowCC(object cc)
        {
            if (cc != null && !(cc is DBNull))
            {
                string strCc = Convert.ToString(cc);
                strCc = (new CreditCard(strCc)).DecryptedCreditCard;

                if (strCc.Length >= 8)
                {
                    strCc = strCc.Substring(0, 8) + "****" + ((strCc.Length > 12) ? strCc.Substring(12) : "");
                }

                return strCc;
            }
            return null;
        }

        protected string ShowAmount(object a)
        {
            if (a != null && !(a is DBNull))
            {
                decimal temp = Convert.ToDecimal(a);
                return temp.ToString("$0.00");
            }
            return null;
        }

        protected string ShowDate(object dt)
        {
            if (dt != null && !(dt is DBNull))
            {
                DateTime temp = Convert.ToDateTime(dt);
                return temp.ToShortDateString();
            }
            return null;
        }

        // Retrieve amount and currency from the ChargeHistoryExCurrency
        // If there are no records in this table then retrieve Amount from the ChargeHistoryEx and default currency
        protected void GetChargeHistoryAmountAndCurrency(BaseService service, ChargeHistoryEx ch,
            out decimal amount, out string currency)
        {
            if (ch != null)
            {
                ChargeHistoryExCurrency chc = service.Load<ChargeHistoryExCurrency>(ch.ChargeHistoryID);
                if (chc == null)
                {
                    amount = ch.Amount.Value;
                    currency = GetDefaultCurrency();
                }
                else
                {
                    amount = chc.CurrencyAmount.Value;
                    Currency cr = service.Load<Currency>(chc.CurrencyID);
                    if (cr == null)
                    {
                        throw new Exception(string.Format("Currency({0}) was not found", chc.CurrencyID));
                    }
                    currency = cr.CurrencyName;
                }
            }
            else
            {
                amount = 0M;
                currency = string.Empty;
            }
        }

        protected string GetDefaultCurrency()
        {
           return "US Dollar (USD)";
        }

        public override string HeaderString
        {
            get { return "Chargeback Report"; }
        }

        private string GetTrackingNumberByChargeHistoryID(ChargeHistoryEx ch, bool trial)
        {
            if (ch == null)
                return null;

            try
            {
                MySqlConnection connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);

                connection.Open();

                

                MySqlCommand cmd = new MySqlCommand(@"
                select distinct s.TrackingNumber from ChargeHistoryEx ch
                left join BillingSale bsale on bsale.BillingSubscriptionID = ch.BillingSubscriptionID
                inner join Sale s on s.SaleID = bsale.SaleID
                where ch.ChargeHistoryID = @chargeHistoryID and SaleTypeID=@SaleTypeID");

                cmd.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;
                cmd.Parameters.Add("@SaleTypeID", MySqlDbType.Int32).Value = trial ? 1 : 2;
                cmd.Connection = connection;

                return cmd.ExecuteScalar().ToString();                
            }
            catch
            {
                return null;
            }
        }
    }
}
