using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class Address : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                FirstName = Utility.TryGetStr(tbFirstName.Text);
                LastName = Utility.TryGetStr(tbLastName.Text);
                Address1 = Utility.TryGetStr(tbAddress1.Text);
                Address2 = Utility.TryGetStr(tbAddress2.Text);
                City = Utility.TryGetStr(tbCity.Text);
                Country = Utility.TryGetStr(ddlCountry.SelectedValue);
                State = (Country == "US" ? Utility.TryGetStr(ddlState.SelectedValue) : Utility.TryGetStr(tbStateEx.Text));
                Zip = (Country == "US" ? Utility.TryGetStr(tbZip.Text) : Utility.TryGetStr(tbZipEx.Text));
                Phone = (Country == "US" ? Utility.TryGetStr(tbPhone1.Text + tbPhone2.Text + tbPhone3.Text) : Utility.TryGetStr(tbPhoneEx.Text));
                Email = Utility.TryGetStr(tbEmail.Text);
            }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            tbFirstName.Text = FirstName;
            tbLastName.Text = LastName;
            tbAddress1.Text = Address1;
            tbAddress2.Text = Address2;
            tbCity.Text = City;
            string country = (string.IsNullOrEmpty(Country) ? "US" : Country)
                .Replace("USA", "US")
                .Replace("us", "US")
                .Replace("usa", "US")
                .Replace("United States", "US")
                .Replace("united states", "US")
                .Replace("UNITED STATES", "US")
                .Replace("UK", "United Kingdom")
                .Replace("uk", "United Kingdom")
                .Replace("GB", "United Kingdom")
                .Replace("gb", "United Kingdom")
                .Replace("united kingdom", "United Kingdom")
                .Replace("UNITED KINGDOM", "United Kingdom")
                .Replace("great britain", "United Kingdom")
                .Replace("GREAT BRITAIN", "United Kingdom")
                .Replace("ca", "Canada")
                .Replace("CA", "Canada")
                .Replace("can", "Canada")
                .Replace("CAN", "Canada");
            try 
	        {
                ddlCountry.SelectedValue = country;
	        }
	        catch {}
            //throw new Exception(Country);
            if (country == "US")
            {
                try
                {
                    ddlState.SelectedValue = State;
                }
                catch { }
                tbZip.Text = Zip;
                try
                {
                    if (Phone != null)
                    {
                        string phone = Phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
                        tbPhone1.Text = (phone.Length > 3 ? phone.Substring(0, 3) : phone);
                        tbPhone2.Text = (phone.Length > 3 ? phone.Length > 6 ? phone.Substring(3, 3) : phone.Substring(3) : "");
                        tbPhone3.Text = (phone.Length > 6 ? phone.Substring(6) : "");
                    }
                }
                catch { }
            }
            else
            {
                tbStateEx.Text = State;
                tbZipEx.Text = Zip;
                tbPhoneEx.Text = Phone;
            }
            tbEmail.Text = Email;
        }
    }
}