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
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace TrimFuel.Web.Admin
{
    public partial class reports_transactions : PageX
    {
        ReportService reportService = new ReportService();
        int totalCount = 0;
        DataSet _transactionsData;

        public DataSet TransactionsData 
        {
            get { return _transactionsData; }
            set {_transactionsData=value;} 
        }

        protected void Page_Load(object sender, EventArgs e)
        { 

        }
        protected void btnGo_Click(object sender, EventArgs e)
        {
            var dataSet = new DataSet();
            MySqlConnection connection;
            var adapter = new MySqlDataAdapter();
            string query=string.Empty;
            string SUBSCRIPTION_QUERY = @"select 
                                    cht.DisplayName, Count(ch.ChargeTypeID) As ChargeCount
                                    from ChargeHistoryEx ch
                                    join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID
                                    join Billing b on b.BillingID = bs.BillingID
                                    join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID
                                    join ChargeType cht on cht.ChargeTypeID = ch.ChargeTypeID
                                    join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                                    join Subscription s on s.SubscriptionID = bs.SubscriptionID
                                    where ch.ChargeDate >= @startDate and ch.ChargeDate <= @endDate";

            string NON_SUBSCRIPTION_QUERY = @"select 
                                    cht.DisplayName, Count(ch.ChargeTypeID) As ChargeCount
                                    from ChargeHistoryEx ch
                                    join ChargeHistoryInvoice chi on chi.ChargeHistoryID = ch.ChargeHistoryID
                                    join Invoice i on i.InvoiceID = chi.InvoiceID
                                    join Orders o on o.OrderID = i.OrderID
                                    join Product p on p.ProductID = o.ProductID
                                    join ChargeType cht on cht.ChargeTypeID = ch.ChargeTypeID
                                    join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                                    where ch.ChargeDate >= @startDate and ch.ChargeDate <= @endDate";

            string FAILED_CHARGE_HISTORY_SELECT = @"select 
                                    case when ch.Amount = 0.0 then 'AuthOnly' else 'Charge' end As DisplayName , count(ch.FailedChargeHistoryID) As ChargeCount ";

            string FAILED_CHARGE_HISTORY_JOINS = " from FailedChargeHistory ch join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID ";
            
            string FAILED_CHARGE_HISTORY_FILTERS=@" where ch.ChargeDate >= @startDate and ch.ChargeDate <= @endDate ";

            string FAILED_CHARGE_HISTORY_GROUPBY = @" group by  case when ch.Amount = 0.0 then 'AuthOnly' else 'Charge' end ";

            string FAILED_CHARGE_HISTORY_QUERY = "";
            
           
           

            if (ddlProductGroup.SelectedValue != "")
            {
                SUBSCRIPTION_QUERY += " and s.ProductName =@productName";
                NON_SUBSCRIPTION_QUERY += " and p.ProductName =@productName";
                FAILED_CHARGE_HISTORY_JOINS += @" join Billing b on b.BillingID = ch.BillingID
                                                        join Campaign c on c.CampaignID=b.CampaignID
                                                        join Subscription s on s.SubscriptionID = c.SubscriptionID
                                                        join Product p on p.ProductID = s.ProductID ";
                FAILED_CHARGE_HISTORY_FILTERS += " and p.ProductName =@productName";
            }

            
            if (ddlChargeType.SelectedValue != "")
            {
                SUBSCRIPTION_QUERY += " and cht.ChargeTypeID=@chargeTypeID";
                NON_SUBSCRIPTION_QUERY += " and cht.ChargeTypeID=@chargeTypeID";
                if (ddlChargeType.SelectedItem.Text.Equals("Charge"))
                {
                    FAILED_CHARGE_HISTORY_FILTERS += " and ch.Amount > 0.0";
                }
                else if (ddlChargeType.SelectedItem.Text.Equals("Auth"))
                {
                    FAILED_CHARGE_HISTORY_FILTERS += " and ch.Amount = 0.0";
                }
               
            }

            if (ddlMid.SelectedValue != "")
            {
                SUBSCRIPTION_QUERY += " and am.AssertigyMIDID =@MID";
                NON_SUBSCRIPTION_QUERY += " and am.AssertigyMIDID =@MID";
                FAILED_CHARGE_HISTORY_FILTERS += " and am.AssertigyMIDID =@MID";
            }

            if (ddlTransactionType.SelectedItem.Text.Equals("Successful"))
            {
                SUBSCRIPTION_QUERY += " and ch.Success = 1 ";
                NON_SUBSCRIPTION_QUERY += " and ch.Success = 1 ";
               

            }
            else if (ddlTransactionType.SelectedItem.Text.Equals("Failed"))
            {
                SUBSCRIPTION_QUERY += " and ch.Success = 0 ";
                NON_SUBSCRIPTION_QUERY += " and ch.Success = 0 ";
                if (ddlChargeType.SelectedItem.Text.Equals("Charge") || ddlChargeType.SelectedItem.Text.Equals("Auth") || ddlChargeType.SelectedItem.Text.Equals("-- All --"))
                {
                    FAILED_CHARGE_HISTORY_QUERY = FAILED_CHARGE_HISTORY_SELECT + FAILED_CHARGE_HISTORY_JOINS + FAILED_CHARGE_HISTORY_FILTERS + FAILED_CHARGE_HISTORY_GROUPBY;
                }
            }
            else
            {
                if (ddlChargeType.SelectedItem.Text.Equals("Charge") || ddlChargeType.SelectedItem.Text.Equals("Auth") || ddlChargeType.SelectedItem.Text.Equals("-- All --"))
                {
                    FAILED_CHARGE_HISTORY_QUERY = FAILED_CHARGE_HISTORY_SELECT + FAILED_CHARGE_HISTORY_JOINS + FAILED_CHARGE_HISTORY_FILTERS + FAILED_CHARGE_HISTORY_GROUPBY;
                }
            }


            SUBSCRIPTION_QUERY += " Group By ch.ChargeTypeID ";
            NON_SUBSCRIPTION_QUERY += " Group By ch.ChargeTypeID ";

            query += "Select X.DisplayName As ChargeType, Sum(X.ChargeCount) As Count From ( ";
            if (FAILED_CHARGE_HISTORY_QUERY.Equals(""))
            {
                query += SUBSCRIPTION_QUERY + " union " + NON_SUBSCRIPTION_QUERY ;
            }
            else
            {
                query += SUBSCRIPTION_QUERY + " union " + NON_SUBSCRIPTION_QUERY + " union " + FAILED_CHARGE_HISTORY_QUERY;
            }
            query += ") As X  Group By X.DisplayName";



            try
            {
                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(query);

                cmd.Parameters.Add("@startDate", MySqlDbType.VarChar).Value = DateFilter1.Date1.Value.ToString("yyyy-MM-dd") + " 00:00:00";
                cmd.Parameters.Add("@endDate", MySqlDbType.VarChar).Value = DateFilter1.Date2.Value.ToString("yyyy-MM-dd") + " 23:59:59";
               if (ddlChargeType.SelectedValue != "")
               {
                   cmd.Parameters.Add("@chargeTypeID", MySqlDbType.Int32).Value = Convert.ToInt32(ddlChargeType.SelectedValue);
               }

               if (ddlProductGroup.SelectedValue != "")
               {
                   cmd.Parameters.Add("@productName", MySqlDbType.VarChar).Value = ddlProductGroup.SelectedItem.Text;
               }

               if (ddlMid.SelectedValue != "")
               {
                   cmd.Parameters.Add("@MID", MySqlDbType.VarChar).Value = ddlMid.SelectedItem.Value;
               }

               cmd.Connection = connection;
               adapter.SelectCommand = cmd;
                adapter.Fill(dataSet);
                Session["TransactionDataSet"] = dataSet;
                GvReport.DataSource = dataSet;
                GvReport.DataBind();
                
               

                connection.Close();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
       
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            MySqlConnection connection;

                connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
                connection.Open();

                MySqlDataAdapter da = new MySqlDataAdapter("select * from ChargeType", connection);
                DataSet ds = new DataSet();
                da.Fill(ds);

                ddlChargeType.DataSource = ds;
                ddlChargeType.DataTextField = "DisplayName";
                ddlChargeType.DataValueField = "ChargeTypeID";


                da = new MySqlDataAdapter("SELECT * FROM Product WHERE ProductIsActive=1 Order By ProductName", connection);
                ds = new DataSet();
                da.Fill(ds);
                ddlProductGroup.DataSource = ds;
                ddlProductGroup.DataTextField = "ProductName";
                ddlProductGroup.DataValueField = "ProductID";


                da = new MySqlDataAdapter("Select * from AssertigyMID where Visible is true", connection);
                ds = new DataSet();
                da.Fill(ds);
                ddlMid.DataSource = ds;
                ddlMid.DataTextField = "DisplayName";
                ddlMid.DataValueField = "AssertigyMIDID";


                connection.Close();
                
        }
        protected void ddlChargeType_DataBound(object sender, EventArgs e)
        {
            ddlChargeType.Items.Insert(0, new ListItem("Auth", "4"));
            ddlChargeType.Items.Insert(0, new ListItem("-- All --", ""));
        }
        protected void ddlProductGroup_DataBound(object sender, EventArgs e)
        {
            ddlProductGroup.Items.Insert(0, new ListItem("-- All --", ""));
        }
        protected void ddlMid_DataBound(object sender, EventArgs e)
        {
            ddlMid.Items.Insert(0, new ListItem("-- All --", ""));
        }
        protected void ddlTransactionType_DataBound(object sender, EventArgs e)
        {
           
            try
            {
                ddlTransactionType.Items.Insert(0, new ListItem("-- All --", ""));
                ddlTransactionType.Items.Insert(1, new ListItem("Successful", "1"));
                ddlTransactionType.Items.Insert(2, new ListItem("Failed", "2"));
            }
            catch (Exception)
            {

            }
        }
        public override string HeaderString
        {
            get { return "Transactions report"; }
        }
        protected void gridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GvReport.PageIndex = e.NewPageIndex;
                GvReport.DataSource = (DataSet)Session["TransactionDataSet"];
                GvReport.DataBind();
                GvReport.PagerStyle.HorizontalAlign = HorizontalAlign.Center; 
            }
            catch (Exception ex)
            {

            }
        }
        protected void ddlPageRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!((DropDownList)sender).SelectedValue.Equals("0"))
                {
                    GvReport.PageSize = Convert.ToInt32(((DropDownList)sender).SelectedValue);
                }
                else
                {
                    GvReport.PageSize = ((DataSet)Session["TransactionDataSet"]).Tables[0].Rows.Count;
                }
                GvReport.DataSource = (DataSet)Session["TransactionDataSet"];
                GvReport.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        protected void GvReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblCount = (Label)e.Row.FindControl("lblCount");
                int count = Int32.Parse(lblCount.Text);
                totalCount += count;
            } 
            if (e.Row.RowType == DataControlRowType.Footer)
            {

                Label lbl = (Label)(e.Row.FindControl("lblTotalCount"));
                lbl.Text = totalCount.ToString();

            }
        }
    }
}