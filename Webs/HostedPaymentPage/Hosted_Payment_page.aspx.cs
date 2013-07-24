using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Configuration;
using Payment_test.Utils;
using System.Data;
using TrimFuel.Business.Utils;

namespace Payment_test
{
    public partial class _Default : System.Web.UI.Page
    {
        protected NameValueCollection nvc = new NameValueCollection();
        protected Dictionary<string, decimal> Products = new Dictionary<string, decimal>();
        protected decimal Total = 0;
        char delimiter = ';';

        string[] ProductCodeID_Quantity = new string[0];
        string[] Prices = new string[0];
        string[] Descriptions = new string[0];

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                nvc = Request.Params;
                CheckReqData();
            }
            LoadDDLInfo();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            using (DBContext context = DBContext.UseConext(GetClientConnString()))
            {
                var service = new ProductService();

                if (!String.IsNullOrEmpty(nvc["SubscrID"]))
                {
                    var subscription = service.Load<Subscription>(nvc["SubscrID"]);
                    if (subscription != null)
                    {
                        var price = (subscription.InitialShipping ?? 0) + (subscription.InitialBillAmount ?? 0);
                        Total += price;
                        Products.Add(subscription.DisplayName, price);
                    }
                }
            }

            FillProductTypes();

            for (int i = 0; i < ProductCodeID_Quantity.Length; i++)
            {
                string[] tmp = ProductCodeID_Quantity[i].Split('-');
                short quant = 0;
                if (tmp.Length == 2)
                {
                    if (!string.IsNullOrEmpty(tmp[1]))
                        quant = short.Parse(tmp[1]);
                }

                decimal prc = 0;
                if (!string.IsNullOrEmpty(Prices[i]))
                    prc = decimal.Parse(Prices[i]);

                string desc = Descriptions[i];

                if (quant > 1)
                    desc = string.Format("{0}x {1}", quant, desc);

                Products.Add(desc, prc);
                Total += prc * quant;
            }
        }

        protected void FillProductTypes()
        {
            // PTID contains data like ProductID-Quantity,ProductID-Quantity, etc
            if (nvc["PTID"] != null)
                ProductCodeID_Quantity = nvc["PTID"].Split(delimiter);

            // TCost contains list of prices. 
            if (nvc["TCost"] != null)
                Prices = nvc["TCost"].Split(delimiter);
            // descriptions

            if (nvc["PDes"] != null)
                Descriptions = nvc["PDes"].Split(delimiter);
        }

        protected void BILL_USER_Click(object sender, EventArgs e)
        {
            nvc = (NameValueCollection)Session["nvc"];
            if (!Page.IsValid)
            {
                DataBind();
                return;
            }

            using (DBContext context = DBContext.UseConext(GetClientConnString()))
            {
                SaleService ss = new SaleService();
                BusinessError<ComplexSaleView> err = new BusinessError<ComplexSaleView>();
                IList<UpsellType> products = new List<UpsellType>();
                string ship_state = "", bill_state = "";

                // getting state
                if (DDL_Bill_State.Visible == true)
                    bill_state = DDL_Bill_State.SelectedValue;
                else bill_state = tbx_Bill_State.Text;

                if (DDL_ship_State.Visible == true)
                    ship_state = DDL_ship_State.SelectedValue;
                else ship_state = tbx_ship_state.Text;

                FillProductTypes();

                // PTID & TCost & descriptions lengths should be equal
                if (ProductCodeID_Quantity.Length != Prices.Length || ProductCodeID_Quantity.Length != Descriptions.Length)
                    Response.Redirect(nvc["CancelURL"]);

                // payment itself.
                err = ss.CreateComplexSaleForHostedPayment(Utility.TryGetLong(nvc["RegistrationID"]), tbx_bill_FirstName.Text, tbx_bill_LastName.Text, tbx_bill_Addr1.Text,
                    tbx_bill_Addr2.Text, tbx_bill_City.Text, bill_state, tbx_bill_zip.Text, DDL_Bill_country.SelectedValue, tbx_bill_phone.Text, tbx_bill_Email.Text,
                    tbx_ship_FirstName.Text, tbx_ship_LastName.Text, tbx_ship_Addr1.Text, tbx_ship_Addr2.Text,
                    tbx_ship_City.Text, ship_state, tbx_ship_zip.Text, DDL_ship_Country.SelectedValue, tbx_ship_phone.Text, tbx_ship_Email.Text,
                    nvc["aff"], nvc["sub"], decimal.Parse(nvc["SCost"]), HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Url.AbsolutePath,
                    tbx_cc_number.Text, tbx_cc_cvv.Text, int.Parse(ddlMonth.Text), int.Parse(ddlYear.Text),
                    Utility.TryGetInt(nvc["SubscrID"]), ProductCodeID_Quantity, Prices, Descriptions, Utility.TryGetInt(nvc["ProductID"]));
                if (err != null)
                {
                    if (err.State == BusinessErrorState.Success)
                    {
                        if (nvc["ConfirmURL"].Contains("?"))
                            Response.Redirect(string.Format("{0}&BillingID={1}", nvc["ConfirmURL"], err.ReturnValue.ParentBilling.BillingID));
                        else
                            Response.Redirect(string.Format("{0}?BillingID={1}", nvc["ConfirmURL"], err.ReturnValue.ParentBilling.BillingID));
                    }
                    else
                    {
                        Response.Redirect(nvc["CancelURL"]);
                    }
                }
                else
                {
                    Response.Redirect(nvc["CancelURL"]);
                }
            }
        }

        protected void CheckReqData()
        {
            string errorMessage = string.Empty;

            // NameValueCollection nvc = Request.Params;
            if (string.IsNullOrEmpty(nvc["Name"]))
                errorMessage += "<br/>Name is empty";

            if (string.IsNullOrEmpty(nvc["Pass"]))
                errorMessage += "<br/>Password is empty";

            string[] productTypes = null;
            string[] productPrices = null;

            if (!string.IsNullOrEmpty(nvc["PTID"]))
            {
                productTypes = nvc["PTID"].Split(delimiter);
                for (int i = 0; i < productTypes.Length; i++)
                {
                    string[] tm = productTypes[i].Split('-');
                    if (tm.Length < 2 || !tm[0].IsNumber() || !tm[1].IsNumber() || string.IsNullOrEmpty(tm[0]) || string.IsNullOrEmpty(tm[1]))
                        errorMessage += "<br/>Product type id and quantity should contain pair of numbers delimitered by '-'";
                }
            }
            if (nvc["PDes"] != null && nvc["PDes"].Length > 1000)
                errorMessage += "<br/>Product description is more than 1000 symbols";

            if (!string.IsNullOrEmpty(nvc["TCost"]))
            {
                productPrices = nvc["TCost"].Split(delimiter);
                for (int i = 0; i < productPrices.Length; i++)
                {
                    if (!productPrices[i].IsNumber())
                    {
                        errorMessage += "<br/>Not all product costs are numbers";
                    }
                }
            }

            if (productTypes != null && productPrices != null && !string.IsNullOrEmpty(nvc["PDes"]))
                if (productTypes.Length != productPrices.Length && nvc["PDes"].Split(delimiter).Length != productPrices.Length)
                    errorMessage += "<br/>Product descriptions count, product prices count and product type count doesn't match";

            if (string.IsNullOrEmpty(nvc["CancelURL"]))// || !nvc["CancelURL"].IsDomainRecord(true))
                errorMessage += "<br/>Cancel url is empty or is not a valid domain record";

            if (string.IsNullOrEmpty(nvc["ConfirmURL"]))// || !nvc["ConfirmURL"].IsDomainRecord(true))
                errorMessage += "<br/>Confirm url is empty or is not a valid domain record";

            if (!string.IsNullOrEmpty(nvc["SCost"]))
            {
                if (!nvc["SCost"].IsNumber())
                    errorMessage += "<br/>Plan cost is not a number";
            }
            else errorMessage += "<br/>Plan cost is empty";

            if (!string.IsNullOrEmpty(nvc["SubscrID"]) || !string.IsNullOrEmpty(nvc["ProductID"]))
            {
                if (!string.IsNullOrEmpty(nvc["SubscrID"]) && !nvc["SubscrID"].IsNumber())
                    errorMessage += "<br/>Plan Id is not a number";

                if (!string.IsNullOrEmpty(nvc["ProductID"]) && !nvc["ProductID"].IsNumber())
                    errorMessage += "<br/>ProductId is not a number";
            }
            else errorMessage += "<br/>ProductID and Plan Id are empty";

            IDbConnection connMySQL = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["STO"].ConnectionString);
            try
            {
                string clientConnString = string.Empty;
                connMySQL.Open();
                IDbCommand mySqlCmd = connMySQL.CreateCommand();

                if (nvc["Name"] != null && nvc["Pass"] != null)
                {
                    mySqlCmd.CommandText = String.Format("select ConnectionStringDotNET as val from TPClient t where t.Username='{0}' and t.Password='{1}'", nvc["Name"], nvc["Pass"]);
                    clientConnString = (mySqlCmd.ExecuteScalar() as string);
                    if (string.IsNullOrEmpty(clientConnString))
                    {
                        errorMessage += "<br/>Client is not valid";
                    }
                }

                if (!string.IsNullOrEmpty(clientConnString))
                {
                    connMySQL.Close();

                    connMySQL = new MySqlConnection(clientConnString);
                    connMySQL.Open();
                    mySqlCmd = connMySQL.CreateCommand();

                    if (productTypes != null)
                    {
                        foreach (var item in productTypes)
                        {
                            var productTypeId = item.Split('-')[0];
                            mySqlCmd.CommandText = String.Format("select ProductCode from ProductCode where ProductCodeId={0}", productTypeId);
                            var productType = mySqlCmd.ExecuteScalar();
                            if ((productType == null || productType == DBNull.Value))
                            {
                                errorMessage += "<br/>ProductType for productTypeId=" + productTypeId + " doesn't exist in the database";
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(nvc["SubscrID"]))
                    {
                        mySqlCmd.CommandText = String.Format("select DisplayName from Subscription where SubscriptionID={0}", nvc["SubscrID"]);
                        var subscription = mySqlCmd.ExecuteScalar();
                        if ((subscription == null || subscription == DBNull.Value))
                        {
                            errorMessage += "<br/>Subscription for planId=" + nvc["SubscrID"] + " doesn't exist in the database";
                        }
                    }

                    if (!String.IsNullOrEmpty(nvc["ProductID"]))
                    {
                        mySqlCmd.CommandText = String.Format("select ProductName from Product where ProductID={0}", nvc["ProductID"]);
                        var subscription = mySqlCmd.ExecuteScalar();
                        if ((subscription == null || subscription == DBNull.Value))
                        {
                            errorMessage += "<br/>Product for productId=" + nvc["ProductID"] + " doesn't exist in the database";
                        }
                    }
                }
            }
            catch (Exception connectionException)
            {
                Response.Redirect(nvc["CancelURL"]);
                throw connectionException;
            }
            finally
            {
                if (connMySQL.State == ConnectionState.Open)
                {
                    connMySQL.Close();
                }
                connMySQL.Dispose();
            }

            if (!String.IsNullOrEmpty(errorMessage))
            {
                Response.Redirect("Error_Page.aspx?errorMessage=" + errorMessage);
            }

            Session["nvc"] = nvc;
        }
        protected void LoadDDLInfo()
        {


            DDL_ship_Country.Items.Add(new ListItem("United States", "US"));
            DDL_ship_Country.Items.Add(new ListItem("Canada", "CA"));
            DDL_ship_Country.Items.Add(new ListItem("Australia", "AU"));

            DDL_Bill_country.Items.Add(new ListItem("United States", "US"));
            DDL_Bill_country.Items.Add(new ListItem("Canada", "CA"));
            DDL_Bill_country.Items.Add(new ListItem("Australia", "AU"));
            DDL_Bill_State.Visible = true;
            DDL_ship_State.Visible = true;
            tbx_Bill_State.Visible = false;
            tbx_ship_state.Visible = false;

            StringBuilder Country = new StringBuilder();
            Country.AppendLine("select * from Country order by Name");

            StringBuilder State = new StringBuilder();
            State.AppendLine("select FullName, ShortName from USState Where ListAtEnd=0 Order By FullName Asc");

            MySqlConnection connection = new MySqlConnection();
            MySqlCommand command = new MySqlCommand();
            try
            {

                connection.ConnectionString = (System.Configuration.ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString);
                connection.Open();

                command = new MySqlCommand();

                command.Connection = connection;
                command.CommandText = Country.ToString();

                MySqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    string c_name = reader["Name"].ToString();
                    string c_cde = reader["Code"].ToString();
                    DDL_ship_Country.Items.Add(new ListItem(c_name, c_cde));
                    DDL_Bill_country.Items.Add(new ListItem(c_name, c_cde));
                }

            }
            catch (Exception ex)
            {
                Response.Redirect(nvc["CancelURL"]);
                throw ex;
            }
            finally
            {
                if (connection != null)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
            try
            {

                connection.ConnectionString = (System.Configuration.ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString);
                connection.Open();

                command = new MySqlCommand();

                command.Connection = connection;
                command.CommandText = State.ToString();

                MySqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    string c_name = reader["FullName"].ToString();
                    string c_cde = reader["ShortName"].ToString();
                    DDL_ship_State.Items.Add(new ListItem(c_name, c_cde));
                    DDL_Bill_State.Items.Add(new ListItem(c_name, c_cde));
                }

            }
            catch (Exception ex)
            {
                Response.Redirect(nvc["CancelURL"]);
                throw ex;
            }
            finally
            {
                if (connection != null)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
        }
        protected void ChangeBillState(object sender, EventArgs e)
        {
            if (DDL_Bill_country.SelectedValue == "US")
            {
                DDL_Bill_State.Visible = true;
                tbx_Bill_State.Visible = false;
            }
            else
            {
                DDL_Bill_State.Visible = false;
                tbx_Bill_State.Visible = true;
            }
            //DDL_Bill_State.DataBind();
            //tbx_Bill_State.DataBind();
        }
        protected void ChangeShipState(object sender, EventArgs e)
        {
            if (DDL_ship_Country.SelectedValue == "US")
            {
                DDL_ship_State.Visible = true;
                tbx_ship_state.Visible = false;
            }
            else
            {
                DDL_ship_State.Visible = false;
                tbx_ship_state.Visible = true;
            }
            //DDL_ship_State.DataBind();
            //tbx_ship_state.DataBind();
        }
        protected void CopyBilltoShip_Click(object sender, EventArgs e)
        {
            tbx_ship_FirstName.Text = tbx_bill_FirstName.Text;
            tbx_ship_LastName.Text = tbx_bill_LastName.Text;
            tbx_ship_Addr1.Text = tbx_bill_Addr1.Text;
            tbx_ship_Addr2.Text = tbx_bill_Addr2.Text;
            tbx_ship_City.Text = tbx_bill_City.Text;
            tbx_ship_zip.Text = tbx_bill_zip.Text;
            tbx_ship_phone.Text = tbx_bill_phone.Text;
            tbx_ship_Email.Text = tbx_bill_Email.Text;
            DDL_ship_Country.SelectedValue = DDL_Bill_country.SelectedValue;
            DDL_ship_State.SelectedValue = DDL_Bill_State.SelectedValue;
            //DDL_ship_Country.SelectedItem = DDL_Bill_country.SelectedValue;
            //DDL_ship_State.SelectedValue = DDL_Bill_State.SelectedValue;
            tbx_ship_state.Text = tbx_Bill_State.Text;
            ChangeShipState(sender, e);

        }
        protected void CopyShiptoBill_Click(object sender, EventArgs e)
        {
            tbx_bill_FirstName.Text = tbx_ship_FirstName.Text;
            tbx_bill_LastName.Text = tbx_ship_LastName.Text;
            tbx_bill_Addr1.Text = tbx_ship_Addr1.Text;
            tbx_bill_Addr2.Text = tbx_ship_Addr2.Text;
            tbx_bill_City.Text = tbx_ship_City.Text;
            tbx_bill_zip.Text = tbx_ship_zip.Text;
            tbx_bill_phone.Text = tbx_ship_phone.Text;
            tbx_bill_Email.Text = tbx_ship_Email.Text;
            DDL_Bill_country.SelectedValue = DDL_ship_Country.SelectedValue;
            DDL_Bill_State.SelectedValue = DDL_ship_State.SelectedValue;
            tbx_Bill_State.Text = tbx_ship_state.Text;
            ChangeBillState(sender, e);

        }
        protected void Two_Letters_Validate(object sender, ServerValidateEventArgs e)
        {
            if (((BaseValidator)sender).Enabled)
            {
                var reg = new Regex(@"\d");
                if (reg.Match(e.Value.ToString()).Success || e.Value.Length < 2)
                {
                    e.IsValid = false;
                }
                else
                    e.IsValid = true;
            }
            else
                e.IsValid = true;
        }
        protected void Two_LettersNumbers_Validate(object sender, ServerValidateEventArgs e)
        {
            if (((BaseValidator)sender).Enabled)
            {
                if (e.Value == "" || e.Value.Length < 2)
                    e.IsValid = false;
                else
                    e.IsValid = true;
            }
            else
                e.IsValid = true;
        }
        protected void Email_Validate(object sender, ServerValidateEventArgs e)
        {
            if (((BaseValidator)sender).Enabled)
            {

                var reg = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                    + "@"
                                    + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
                e.IsValid = reg.Match(e.Value.ToString()).Success;
            }
            else
                e.IsValid = true;

        }
        protected void CredCard_Validate(object sender, ServerValidateEventArgs e)
        {

            var reg = new Regex("^[0-9]*$", RegexOptions.IgnoreCase);
            if (e.Value.Length == 16 && reg.IsMatch(e.Value))
                e.IsValid = true;
            else
                e.IsValid = false;
        }
        protected void Month_number_Validate(object sender, ServerValidateEventArgs e)
        {
            var reg = new Regex("^[0-9]*$", RegexOptions.IgnoreCase);
            if (reg.IsMatch(e.Value) && e.Value.Length < 3 && e.Value != "")
            {
                int tmp = new int();
                tmp = int.Parse(e.Value);
                if (tmp > 0 && tmp < 13)
                    e.IsValid = true;
                else e.IsValid = false;
            }
            else
                e.IsValid = false;
        }
        protected void Year_Validate(object sender, ServerValidateEventArgs e)
        {
            var reg = new Regex("^[0-9]*$", RegexOptions.IgnoreCase);
            if (reg.IsMatch(e.Value) && e.Value.Length < 5 && e.Value != "")
            {
                int tmp = new int();
                tmp = int.Parse(e.Value);
                if (tmp > 2000 && tmp < 2200)
                    e.IsValid = true;
                else e.IsValid = false;
            }
            else
                e.IsValid = false;
        }
        protected string GetClientConnString()
        {
            IDbConnection connMySQL = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["STO"].ConnectionString);
            string clientConnString = string.Empty;
            try
            {
                connMySQL.Open();
                IDbCommand mySqlCmd = connMySQL.CreateCommand();

                if (nvc["Name"] != null && nvc["Pass"] != null)
                {
                    mySqlCmd.CommandText = String.Format("select ConnectionStringDotNET as val from TPClient t where t.Username='{0}' and t.Password='{1}'", nvc["Name"], nvc["Pass"]);
                    clientConnString = (mySqlCmd.ExecuteScalar() as string);
                }
            }
            finally
            {
                connMySQL.Close();
            }
            return clientConnString;
        }
    }
}