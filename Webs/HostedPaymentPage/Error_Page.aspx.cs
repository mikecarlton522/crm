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
using TrimFuel.Business.Dao;
using TrimFuel.Business.Configuration;

namespace Payment_test
{
    public partial class Error_Page : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["errorMessage"] != null)
            {
                lblErrorMessage.Text = Request["errorMessage"];
            }
        }

    }
}